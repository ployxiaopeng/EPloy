using EPloy.SystemFile;
using System;
using System.Collections.Generic;
using System.IO;
using EPloy.Download;

namespace EPloy.Res
{
    /// <summary>
    /// 资源更新器
    /// </summary>
    internal sealed partial class UpdaterHandler
    {
        private const int CachedHashBytesLength = 4;
        private const int CachedBytesLength = 0x1000;
        private DownLoadModule Download
        {
            get
            {
                return GameEntry.DownLoad;
            }
        }
        private ResUpdaterModule ResUpdater;
        private ResStore ResStore;
        private UpdatableVersionListSerializer UpdatableVersionListSerializer;
        private LocalVersionListSerializer ReadWriteVersionListSerializer;
        private readonly List<ApplyInfo> ApplyWaitingInfo;
        private readonly List<UpdateInfo> UpdateWaitingInfo;
        private readonly Dictionary<ResName, UpdateInfo> UpdateCandidateInfo;
        private readonly SortedDictionary<string, List<int>> CachedFileSystemsForGenerateReadWriteVersionList;
        private readonly byte[] CachedHashBytes;
        private readonly byte[] CachedBytes;

        private bool CheckRessComplete;
        private FileStream ApplyingResPackStream;
        private int CurrentGenerateReadWriteVersionListLength;
        private bool FailureFlag;
        private string ReadWriteVersionListFileName;
        private string ReadWriteVersionListBackupFileName;

        private DownloadCallBack DownloadCallBack;

        public Action<string, bool, bool> ResApplyComplete;
        public Action<string> ResUpdateStart;
        public Action<string, int> ResUpdateChanged;
        public Action<string, int> ResUpdateSuccess;
        public Action<string, string, int, int> ResUpdateFailure;
        public Action<bool, bool> ResUpdateComplete;

        /// <summary>
        /// 初始化资源更新器的新实例。
        /// </summary>
        /// <param name="ResManager">资源管理器。</param>
        public UpdaterHandler(ResUpdaterModule resUpdater, ResStore resStore)
        {
            ResUpdater = resUpdater; ResStore = resStore;
            ApplyWaitingInfo = new List<ApplyInfo>();
            UpdateWaitingInfo = new List<UpdateInfo>();
            UpdateCandidateInfo = new Dictionary<ResName, UpdateInfo>();
            ReadWriteVersionListSerializer = new LocalVersionListSerializer();
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
            CachedFileSystemsForGenerateReadWriteVersionList = new SortedDictionary<string, List<int>>(StringComparer.Ordinal);
            CachedHashBytes = new byte[CachedHashBytesLength];
            CachedBytes = new byte[CachedBytesLength];
            CheckRessComplete = false;
            ApplyingResPackPath = null; ApplyingResPackStream = null; UpdatingResGroup = null;
            GenerateReadWriteVersionListLength = 0; CurrentGenerateReadWriteVersionListLength = 0;
            UpdateRetryCount = 3; UpdatingCount = 0; FailureFlag = false;
            ReadWriteVersionListFileName = UtilPath.GetRegularPath(Path.Combine(GameEntry.ResPath, ConfigVersion.LocalVersionListFileName));
            ReadWriteVersionListBackupFileName = UtilText.Format("{0}.{1}", ReadWriteVersionListFileName, ResUpdater.BackupExtension);

            ResApplyComplete = null; ResUpdateStart = null; ResUpdateChanged = null;
            ResUpdateSuccess = null; ResUpdateFailure = null; ResUpdateComplete = null;
            DownloadCallBack = new DownloadCallBack(OnDownloadStart, OnDownloadUpdate, OnDownloadSuccess, OnDownloadFailure);
        }

        /// <summary>
        /// 获取或设置每更新多少字节的资源，重新生成一次版本资源列表。
        /// </summary>
        public int GenerateReadWriteVersionListLength
        {
            get;
            set;
        }

        /// <summary>
        /// 获取正在应用的资源包路径。
        /// </summary>
        public string ApplyingResPackPath
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取等待应用资源数量。
        /// </summary>
        public int ApplyWaitingCount
        {
            get
            {
                return ApplyWaitingInfo.Count;
            }
        }

        /// <summary>
        /// 获取或设置资源更新重试次数。
        /// </summary>
        public int UpdateRetryCount
        {
            get;
            set;
        }

        /// <summary>
        /// 获取正在更新的资源组。
        /// </summary>
        public ResGroup UpdatingResGroup
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取等待更新资源数量。
        /// </summary>
        public int UpdateWaitingCount
        {
            get
            {
                return UpdateWaitingInfo.Count;
            }
        }

        /// <summary>
        /// 获取候选更新资源数量。
        /// </summary>
        public int UpdateCandidateCount
        {
            get
            {
                return UpdateCandidateInfo.Count;
            }
        }

        /// <summary>
        /// 获取正在更新资源数量。
        /// </summary>
        public int UpdatingCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 资源更新器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update()
        {
            if (ApplyingResPackStream != null)
            {
                while (ApplyWaitingInfo.Count > 0)
                {
                    ApplyInfo applyInfo = ApplyWaitingInfo[0];
                    ApplyWaitingInfo.RemoveAt(0);
                    if (ApplyRes(applyInfo))
                    {
                        return;
                    }
                }

                Array.Clear(CachedBytes, 0, CachedBytesLength);
                string ResPackPath = ApplyingResPackPath;
                ApplyingResPackPath = null;
                ApplyingResPackStream.Dispose();
                ApplyingResPackStream = null;
                if (ResApplyComplete != null)
                {
                    ResApplyComplete(ResPackPath, !FailureFlag, UpdateCandidateInfo.Count <= 0);
                }
            }

            if (UpdateWaitingInfo.Count > 0)
            {
                if (Download.FreeAgentCount > 0)
                {
                    UpdateInfo updateInfo = UpdateWaitingInfo[0];
                    UpdateWaitingInfo.RemoveAt(0);
                   string hashCodeName= UtilText.Format("{0}.{1:x8}.{2}", updateInfo.ResName.Name, updateInfo.HashCode, ConfigVersion.DefaultExtension);
                    Download.AddDownload(updateInfo.ResPath, UtilPath.GetRemotePath(Path.Combine(ResUpdater.UpdatePrefixUri, hashCodeName)),
                    DownloadCallBack, updateInfo); 
                    UpdatingCount++;
                }

                return;
            }

            if (UpdatingResGroup != null && UpdatingCount <= 0)
            {
                ResGroup updatingResGroup = UpdatingResGroup;
                UpdatingResGroup = null;
                if (ResUpdateComplete != null)
                {
                    ResUpdateComplete(!FailureFlag, UpdateCandidateInfo.Count <= 0);
                }

                return;
            }
        }

        /// <summary>
        /// 关闭并清理资源更新器。
        /// </summary>
        public void OnDestroy()
        {
            UpdateWaitingInfo.Clear();
            UpdateCandidateInfo.Clear();
            CachedFileSystemsForGenerateReadWriteVersionList.Clear();
        }

        /// <summary>
        /// 增加资源更新。
        /// </summary>
        /// <param name="ResName">资源名称。</param>
        /// <param name="fileSystemName">资源所在的文件系统名称。</param>
        /// <param name="loadType">资源加载方式。</param>
        /// <param name="length">资源大小。</param>
        /// <param name="hashCode">资源哈希值。</param>
        /// <param name="zipLength">压缩后大小。</param>
        /// <param name="zipHashCode">压缩后哈希值。</param>
        /// <param name="ResPath">资源路径。</param>
        public void AddResUpdate(ResName ResName, string fileSystemName, LoadType loadType, int length, int hashCode, int zipLength, int zipHashCode, string ResPath)
        {
            UpdateCandidateInfo.Add(ResName, new UpdateInfo(ResName, fileSystemName, loadType, length, hashCode, zipLength, zipHashCode, ResPath));
        }

        /// <summary>
        /// 检查资源完成。
        /// </summary>
        /// <param name="needGenerateReadWriteVersionList">是否需要生成读写区版本资源列表。</param>
        public void CheckResComplete(bool needGenerateReadWriteVersionList)
        {
            CheckRessComplete = true;
            if (needGenerateReadWriteVersionList)
            {
                GenerateReadWriteVersionList();
            }
        }

        /// <summary>
        /// 应用指定资源包的资源。
        /// </summary>
        /// <param name="ResPackPath">要应用的资源包路径。</param>
        public void ApplyRess(string ResPackPath)
        {
            if (!CheckRessComplete)
            {
                Log.Fatal("You must check Ress complete first.");
                return;
            }

            if (ApplyingResPackStream != null)
            {
                Log.Fatal(UtilText.Format("There is already a Res pack '{0}' being applied.", ApplyingResPackPath));
                return;
            }

            if (UpdatingResGroup != null)
            {
                Log.Fatal(UtilText.Format("There is already a Res group '{0}' being updated.", UpdatingResGroup.Name));
                return;
            }

            try
            {
                long length = 0L;
                UpdatableVersionList versionList = default(UpdatableVersionList);
                using (FileStream fileStream = new FileStream(ResPackPath, FileMode.Open, FileAccess.Read))
                {
                    length = fileStream.Length;
                    versionList = UpdatableVersionListSerializer.Deserialize(fileStream);
                }

                if (!versionList.IsValid)
                {
                    Log.Fatal("Deserialize Res pack version list failure.");
                }

                //if (versionList.Offset + versionList.Length != length)
                //{
                //    Log.Fatal("Res pack length is invalid.");
                //}

                ApplyingResPackPath = ResPackPath;
                ApplyingResPackStream = new FileStream(ResPackPath, FileMode.Open, FileAccess.Read);
                ApplyingResPackStream.Position = 0;
                FailureFlag = false;

                UpdatableVersionList.Resource[] Ress = versionList.GetResources();
                foreach (UpdatableVersionList.Resource Res in Ress)
                {
                    ResName ResName = new ResName(Res.Name);
                    UpdateInfo updateInfo = null;
                    if (!UpdateCandidateInfo.TryGetValue(ResName, out updateInfo))
                    {
                        continue;
                    }

                    if (updateInfo.LoadType == (LoadType)Res.LoadType && updateInfo.Length == Res.Length && updateInfo.HashCode == Res.HashCode)
                    {
                        //Res.Offset 单机包的资源偏移 后面再说
                        ApplyWaitingInfo.Add(new ApplyInfo(ResName, updateInfo.FileSystemName, (LoadType)Res.LoadType, 0, Res.Length, Res.HashCode, Res.ZipLength, Res.ZipHashCode, updateInfo.ResPath));
                    }
                }
            }
            catch (Exception exception)
            {
                if (ApplyingResPackStream != null)
                {
                    ApplyingResPackStream.Dispose();
                    ApplyingResPackStream = null;
                }

                Log.Fatal(UtilText.Format("Apply Ress '{0}' with exception '{1}'.", ResPackPath, exception.ToString()));
            }
        }

        /// <summary>
        /// 更新指定资源组的资源。
        /// </summary>
        /// <param name="ResGroup">要更新的资源组。</param>
        public void UpdateRess(ResGroup ResGroup)
        {
            if (Download == null)
            {
                Log.Fatal("You must set download manager first.");
            }

            if (!CheckRessComplete)
            {
                Log.Fatal("You must check Ress complete first.");
            }

            if (ApplyingResPackStream != null)
            {
                Log.Fatal(UtilText.Format("There is already a Res pack '{0}' being applied.", ApplyingResPackPath));
            }

            if (UpdatingResGroup != null)
            {
                Log.Fatal(UtilText.Format("There is already a Res group '{0}' being updated.", UpdatingResGroup.Name));
            }

            if (string.IsNullOrEmpty(ResGroup.Name))
            {
                foreach (KeyValuePair<ResName, UpdateInfo> updateInfo in UpdateCandidateInfo)
                {
                    UpdateWaitingInfo.Add(updateInfo.Value);
                }

                UpdateCandidateInfo.Clear();
            }
            else
            {
                ResName[] ResNames = ResGroup.InternalGetResNames();
                foreach (ResName ResName in ResNames)
                {
                    UpdateInfo updateInfo = null;
                    if (!UpdateCandidateInfo.TryGetValue(ResName, out updateInfo))
                    {
                        continue;
                    }

                    UpdateWaitingInfo.Add(updateInfo);
                    UpdateCandidateInfo.Remove(ResName);
                }
            }

            UpdatingResGroup = ResGroup;
            FailureFlag = false;
        }

        public void UpdateRes(ResName ResName)
        {
            if (Download == null)
            {
                Log.Fatal("You must set download manager first.");
            }

            if (!CheckRessComplete)
            {
                Log.Fatal("You must check Ress complete first.");
            }

            if (ApplyingResPackStream != null)
            {
                Log.Fatal(UtilText.Format("There is already a Res pack '{0}' being applied.", ApplyingResPackPath));
            }

            UpdateInfo updateInfo = null;
            if (UpdateCandidateInfo.TryGetValue(ResName, out updateInfo))
            {
                UpdateWaitingInfo.Add(updateInfo);
                UpdateCandidateInfo.Remove(ResName);
            }
        }

        private bool ApplyRes(ApplyInfo applyInfo)
        {
            long position = ApplyingResPackStream.Position;
            try
            {
                bool zip = applyInfo.Length != applyInfo.ZipLength || applyInfo.HashCode != applyInfo.ZipHashCode;

                int bytesRead = 0;
                int bytesLeft = applyInfo.ZipLength;
                string directory = Path.GetDirectoryName(applyInfo.ResPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                ApplyingResPackStream.Position += applyInfo.Offset;
                using (FileStream fileStream = new FileStream(applyInfo.ResPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    while ((bytesRead = ApplyingResPackStream.Read(CachedBytes, 0, bytesLeft < CachedBytesLength ? bytesLeft : CachedBytesLength)) > 0)
                    {
                        bytesLeft -= bytesRead;
                        fileStream.Write(CachedBytes, 0, bytesRead);
                    }

                    if (zip)
                    {
                        fileStream.Position = 0L;
                        int hashCode = UtilVerifier.GetCrc32(fileStream);
                        if (hashCode != applyInfo.ZipHashCode)
                        {
                            Log.Fatal(UtilText.Format("Res zip hash code error, need '{0}', applied '{1}'.", applyInfo.ZipHashCode.ToString(), hashCode.ToString()));
                            return false;
                        }

                        if (ResUpdater.DecompressCachedStream == null)
                        {
                            ResUpdater.DecompressCachedStream = new MemoryStream();
                        }

                        fileStream.Position = 0L;
                        ResUpdater.DecompressCachedStream.Position = 0L;
                        ResUpdater.DecompressCachedStream.SetLength(0L);
                        if (!UtilZip.Decompress(fileStream, ResUpdater.DecompressCachedStream))
                        {
                            Log.Fatal(UtilText.Format("Unable to decompress Res '{0}'.", applyInfo.ResPath));
                            return false;
                        }

                        fileStream.Position = 0L;
                        fileStream.SetLength(0L);
                        fileStream.Write(ResUpdater.DecompressCachedStream.GetBuffer(), 0, (int)ResUpdater.DecompressCachedStream.Length);
                    }
                    else
                    {
                        int hashCode = 0;
                        fileStream.Position = 0L;
                        if (applyInfo.LoadType == LoadType.LoadFromMemory || applyInfo.LoadType == LoadType.LoadFromBinary)
                        {
                            // 正常解密
                            UtilConverter.GetBytes(applyInfo.HashCode, CachedHashBytes);
                            hashCode = UtilVerifier.GetCrc32(fileStream, CachedHashBytes, applyInfo.Length);
                            Array.Clear(CachedHashBytes, 0, CachedHashBytesLength);
                        }
                        else
                        {
                            hashCode = UtilVerifier.GetCrc32(fileStream);
                        }

                        if (hashCode != applyInfo.HashCode)
                        {
                            Log.Fatal(UtilText.Format("Res hash code error, need '{0}', applied '{1}'.", applyInfo.HashCode.ToString(), hashCode.ToString()));
                            return false;
                        }
                    }
                }

                if (applyInfo.UseFileSystem)
                {
                    IFileSystem fileSystem = ResUpdater.GetFileSystem(applyInfo.FileSystemName, false);
                    bool retVal = fileSystem.WriteFile(applyInfo.ResName.Name, applyInfo.ResPath);
                    if (File.Exists(applyInfo.ResPath))
                    {
                        File.Delete(applyInfo.ResPath);
                    }

                    return retVal;
                }

                UpdateCandidateInfo.Remove(applyInfo.ResName);
                ResStore.ResInfos[applyInfo.ResName].MarkReady();
                ResStore.ReadWriteResInfos.Add(applyInfo.ResName, new ReadWriteResInfo(applyInfo.FileSystemName, applyInfo.LoadType, applyInfo.Length, applyInfo.HashCode));

                string downloadingRes = UtilText.Format("{0}.download", applyInfo.ResPath);
                if (File.Exists(downloadingRes))
                {
                    File.Delete(downloadingRes);
                }

                CurrentGenerateReadWriteVersionListLength += applyInfo.ZipLength;
                if (ApplyWaitingInfo.Count <= 0 || CurrentGenerateReadWriteVersionListLength >= GenerateReadWriteVersionListLength)
                {
                    CurrentGenerateReadWriteVersionListLength = 0;
                    GenerateReadWriteVersionList();
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                Log.Fatal(exception.ToString());
                return false;
            }
            finally
            {
                ApplyingResPackStream.Position = position;
            }
        }

        private void GenerateReadWriteVersionList()
        {
            if (File.Exists(ReadWriteVersionListFileName))
            {
                if (File.Exists(ReadWriteVersionListBackupFileName))
                {
                    File.Delete(ReadWriteVersionListBackupFileName);
                }

                File.Move(ReadWriteVersionListFileName, ReadWriteVersionListBackupFileName);
            }

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(ReadWriteVersionListFileName, FileMode.Create, FileAccess.Write);
                LocalVersionList.Resource[] Ress = ResStore.ReadWriteResInfos.Count > 0 ? new LocalVersionList.Resource[ResStore.ReadWriteResInfos.Count] : null;
                if (Ress != null)
                {
                    int index = 0;
                    foreach (KeyValuePair<ResName, ReadWriteResInfo> i in ResStore.ReadWriteResInfos)
                    {
                        Ress[index] = new LocalVersionList.Resource(i.Key.Name,(byte)i.Value.LoadType, i.Value.Length, i.Value.HashCode);
                        if (i.Value.UseFileSystem)
                        {
                            List<int> ResIndexes = null;
                            if (!CachedFileSystemsForGenerateReadWriteVersionList.TryGetValue(i.Value.FileSystemName, out ResIndexes))
                            {
                                ResIndexes = new List<int>();
                                CachedFileSystemsForGenerateReadWriteVersionList.Add(i.Value.FileSystemName, ResIndexes);
                            }

                            ResIndexes.Add(index);
                        }

                        index++;
                    }
                }

                LocalVersionList.FileSystem[] fileSystems = CachedFileSystemsForGenerateReadWriteVersionList.Count > 0 ? new LocalVersionList.FileSystem[CachedFileSystemsForGenerateReadWriteVersionList.Count] : null;
                if (fileSystems != null)
                {
                    int index = 0;
                    foreach (KeyValuePair<string, List<int>> i in CachedFileSystemsForGenerateReadWriteVersionList)
                    {
                        fileSystems[index++] = new LocalVersionList.FileSystem(i.Key, i.Value.ToArray());
                        i.Value.Clear();
                    }
                }

                LocalVersionList versionList = new LocalVersionList(Ress, fileSystems);
                if (!ReadWriteVersionListSerializer.Serialize(fileStream, versionList))
                {
                    Log.Fatal("Serialize read write version list failure.");
                }

                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }

                if (File.Exists(ReadWriteVersionListBackupFileName))
                {
                    File.Delete(ReadWriteVersionListBackupFileName);
                }
            }
            catch (Exception exception)
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }

                if (File.Exists(ReadWriteVersionListFileName))
                {
                    File.Delete(ReadWriteVersionListFileName);
                }

                if (File.Exists(ReadWriteVersionListBackupFileName))
                {
                    File.Move(ReadWriteVersionListBackupFileName, ReadWriteVersionListFileName);
                }

                Log.Fatal(UtilText.Format("Generate read write version list exception '{0}'.", exception.ToString()));
            }
        }

        private void OnDownloadStart(DownloadInfo info)
        {
            UpdateInfo updateInfo = info.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (Download == null)
            {
                Log.Fatal("You must set download manager first.");
            }

            if (ResUpdateStart != null)
            {
                ResUpdateStart(updateInfo.ResName.Name);
            }
        }

        private void OnDownloadUpdate(DownloadInfo info)
        {
            UpdateInfo updateInfo = info.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (Download == null)
            {
                Log.Fatal("You must set download manager first.");
            }

            if (info.CurrentLength > updateInfo.ZipLength)
            {
                Download.RemoveDownload(info.SerialId);
                string downloadFile = UtilText.Format("{0}.download", info.DownloadPath);
                if (File.Exists(downloadFile))
                {
                    File.Delete(downloadFile);
                }
                info.ErrMsg = UtilText.Format("When download update, downloaded length is larger than zip length, need '{0}', downloaded '{1}'.", updateInfo.ZipLength, info.CurrentLength);
                OnDownloadFailure(info);
                return;
            }

            if (ResUpdateChanged != null)
            {
                ResUpdateChanged(updateInfo.ResName.Name, info.CurrentLength);
            }
        }

        private void OnDownloadSuccess(DownloadInfo info)
        {
            UpdateInfo updateInfo = info.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            using (FileStream fileStream = new FileStream(info.DownloadPath, FileMode.Open, FileAccess.ReadWrite))
            {
                bool zip = updateInfo.Length != updateInfo.ZipLength || updateInfo.HashCode != updateInfo.ZipHashCode;

                int length = (int)fileStream.Length;
                if (length != updateInfo.ZipLength)
                {
                    fileStream.Close();
                    info.ErrMsg = UtilText.Format("Res zip length error, need '{0}', downloaded '{1}'.", updateInfo.ZipLength, length);
                    OnDownloadFailure(info);
                    return;
                }

                if (zip)
                {
                    fileStream.Position = 0L;
                    int hashCode = UtilVerifier.GetCrc32(fileStream);
                    if (hashCode != updateInfo.ZipHashCode)
                    {
                        fileStream.Close();
                        info.ErrMsg = UtilText.Format("Res zip hash code error, need '{0}', downloaded '{1}'.", updateInfo.ZipHashCode, hashCode);
                        OnDownloadFailure(info);
                        return;
                    }

                    if (ResUpdater.DecompressCachedStream == null)
                    {
                        ResUpdater.DecompressCachedStream = new MemoryStream();
                    }

                    try
                    {
                        fileStream.Position = 0L;
                        ResUpdater.DecompressCachedStream.Position = 0L;
                        ResUpdater.DecompressCachedStream.SetLength(0L);
                        if (!UtilZip.Decompress(fileStream, ResUpdater.DecompressCachedStream))
                        {
                            fileStream.Close();
                            info.ErrMsg = UtilText.Format("Unable to decompress Res '{0}'.", info.DownloadPath);
                            OnDownloadFailure(info);
                            return;
                        }

                        if (ResUpdater.DecompressCachedStream.Length != updateInfo.Length)
                        {
                            fileStream.Close();
                            info.ErrMsg = UtilText.Format("Res length error, need '{0}', downloaded '{1}'.", updateInfo.Length.ToString(), ResUpdater.DecompressCachedStream.Length);
                            OnDownloadFailure(info);
                            return;
                        }

                        fileStream.Position = 0L;
                        fileStream.SetLength(0L);
                        fileStream.Write(ResUpdater.DecompressCachedStream.GetBuffer(), 0, (int)ResUpdater.DecompressCachedStream.Length);
                    }
                    catch (Exception exception)
                    {
                        fileStream.Close();
                        info.ErrMsg = UtilText.Format("Unable to decompress Res '{0}' with error message '{1}'.", info.DownloadPath, exception);
                        OnDownloadFailure(info);
                        return;
                    }
                    finally
                    {
                        ResUpdater.DecompressCachedStream.Position = 0L;
                        ResUpdater.DecompressCachedStream.SetLength(0L);
                    }
                }
                else
                {
                    int hashCode = 0;
                    fileStream.Position = 0L;
                    if (updateInfo.LoadType == LoadType.LoadFromBinary || updateInfo.LoadType == LoadType.LoadFromMemory)
                    {
                        //  todo: 默认正常解密  以后再说
                        UtilConverter.GetBytes(updateInfo.HashCode, CachedHashBytes);
                        hashCode = UtilVerifier.GetCrc32(fileStream, CachedHashBytes, length);
                        Array.Clear(CachedHashBytes, 0, CachedHashBytesLength);
                    }
                    else
                    {
                        hashCode = UtilVerifier.GetCrc32(fileStream);
                    }

                    if (hashCode != updateInfo.HashCode)
                    {
                        fileStream.Close();
                        info.ErrMsg = UtilText.Format("Res hash code error, need '{0}', downloaded '{1}'.", updateInfo.HashCode, hashCode);
                        OnDownloadFailure(info);
                        return;
                    }
                }
            }

            if (updateInfo.UseFileSystem)
            {
                IFileSystem fileSystem = ResUpdater.GetFileSystem(updateInfo.FileSystemName, false);
                bool retVal = fileSystem.WriteFile(updateInfo.ResName.Name, updateInfo.ResPath);
                if (File.Exists(updateInfo.ResPath))
                {
                    File.Delete(updateInfo.ResPath);
                }

                if (!retVal)
                {
                    info.ErrMsg = UtilText.Format("Write Res to file system '{0}' error.", fileSystem.FullPath);
                    OnDownloadFailure(info);
                    return;
                }
            }

            UpdatingCount--;
            ResStore.ResInfos[updateInfo.ResName].MarkReady();
            ResStore.ReadWriteResInfos.Add(updateInfo.ResName, new ReadWriteResInfo(updateInfo.FileSystemName, updateInfo.LoadType, updateInfo.Length, updateInfo.HashCode));
            CurrentGenerateReadWriteVersionListLength += updateInfo.ZipLength;
            if (UpdatingCount <= 0 || CurrentGenerateReadWriteVersionListLength >= GenerateReadWriteVersionListLength)
            {
                CurrentGenerateReadWriteVersionListLength = 0;
                GenerateReadWriteVersionList();
            }

            if (ResUpdateSuccess != null)
            {
                ResUpdateSuccess(updateInfo.ResName.Name, updateInfo.ZipLength);
            }
        }

        private void OnDownloadFailure(DownloadInfo info)
        {
            UpdateInfo updateInfo = info.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (File.Exists(info.DownloadPath))
            {
                File.Delete(info.DownloadPath);
            }

            UpdatingCount--;

            if (ResUpdateFailure != null)
            {
                ResUpdateFailure(updateInfo.ResName.Name, info.ErrMsg, updateInfo.RetryCount, UpdatingCount);
            }

            if (updateInfo.RetryCount < UpdateRetryCount)
            {
                updateInfo.RetryCount++;
                UpdateWaitingInfo.Add(updateInfo);
            }
            else
            {
                FailureFlag = true;
                updateInfo.RetryCount = 0;
                UpdateCandidateInfo.Add(updateInfo.ResName, updateInfo);
            }
        }
    }
}
