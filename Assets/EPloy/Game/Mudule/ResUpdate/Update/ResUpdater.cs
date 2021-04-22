using EPloy.SystemFile;
using System;
using System.Collections.Generic;
using System.IO;

namespace EPloy.Res
{

    public sealed partial class ResUpdater
    {
        private const int CachedHashBytesLength = 4;
        private const int CachedBytesLength = 0x1000;

        private readonly ResManager m_ResManager;
        private readonly List<ApplyInfo> m_ApplyWaitingInfo;
        private readonly List<UpdateInfo> m_UpdateWaitingInfo;
        private readonly Dictionary<ResName, UpdateInfo> m_UpdateCandidateInfo;
        private readonly SortedDictionary<string, List<int>> m_CachedFileSystemsForGenerateReadWriteVersionList;
        private readonly byte[] m_CachedHashBytes;
        private readonly byte[] m_CachedBytes;
        private IDownloadManager m_DownloadManager;
        private bool m_CheckRessComplete;
        private string m_ApplyingResPackPath;
        private FileStream m_ApplyingResPackStream;
        private ResGroup m_UpdatingResGroup;
        private int m_GenerateReadWriteVersionListLength;
        private int m_CurrentGenerateReadWriteVersionListLength;
        private int m_UpdateRetryCount;
        private int m_UpdatingCount;
        private bool m_FailureFlag;
        private string m_ReadWriteVersionListFileName;
        private string m_ReadWriteVersionListBackupFileName;

        public EPloyAction<ResName, string, string, int, int> ResApplySuccess;
        public EPloyAction<ResName, string, string> ResApplyFailure;
        public EPloyAction<string, bool, bool> ResApplyComplete;
        public EPloyAction<ResName, string, string, int, int, int> ResUpdateStart;
        public EPloyAction<ResName, string, string, int, int> ResUpdateChanged;
        public EPloyAction<ResName, string, string, int, int> ResUpdateSuccess;
        public EPloyAction<ResName, string, int, int, string> ResUpdateFailure;
        public EPloyAction<ResGroup, bool, bool> ResUpdateComplete;

        /// <summary>
        /// 初始化资源更新器的新实例。
        /// </summary>
        /// <param name="ResManager">资源管理器。</param>
        public ResUpdater(ResManager ResManager)
        {
            m_ResManager = ResManager;
            m_ApplyWaitingInfo = new List<ApplyInfo>();
            m_UpdateWaitingInfo = new List<UpdateInfo>();
            m_UpdateCandidateInfo = new Dictionary<ResName, UpdateInfo>();
            m_CachedFileSystemsForGenerateReadWriteVersionList = new SortedDictionary<string, List<int>>(StringComparer.Ordinal);
            m_CachedHashBytes = new byte[CachedHashBytesLength];
            m_CachedBytes = new byte[CachedBytesLength];
            m_DownloadManager = null;
            m_CheckRessComplete = false;
            m_ApplyingResPackPath = null;
            m_ApplyingResPackStream = null;
            m_UpdatingResGroup = null;
            m_GenerateReadWriteVersionListLength = 0;
            m_CurrentGenerateReadWriteVersionListLength = 0;
            m_UpdateRetryCount = 3;
            m_UpdatingCount = 0;
            m_FailureFlag = false;
            m_ReadWriteVersionListFileName = Utility.Path.GetRegularPath(Path.Combine(m_ResManager.m_ReadWritePath, LocalVersionListFileName));
            m_ReadWriteVersionListBackupFileName = Utility.Text.Format("{0}.{1}", m_ReadWriteVersionListFileName, BackupExtension);

            ResApplySuccess = null;
            ResApplyFailure = null;
            ResApplyComplete = null;
            ResUpdateStart = null;
            ResUpdateChanged = null;
            ResUpdateSuccess = null;
            ResUpdateFailure = null;
            ResUpdateComplete = null;
        }

        /// <summary>
        /// 获取或设置每更新多少字节的资源，重新生成一次版本资源列表。
        /// </summary>
        public int GenerateReadWriteVersionListLength
        {
            get
            {
                return m_GenerateReadWriteVersionListLength;
            }
            set
            {
                m_GenerateReadWriteVersionListLength = value;
            }
        }

        /// <summary>
        /// 获取正在应用的资源包路径。
        /// </summary>
        public string ApplyingResPackPath
        {
            get
            {
                return m_ApplyingResPackPath;
            }
        }

        /// <summary>
        /// 获取等待应用资源数量。
        /// </summary>
        public int ApplyWaitingCount
        {
            get
            {
                return m_ApplyWaitingInfo.Count;
            }
        }

        /// <summary>
        /// 获取或设置资源更新重试次数。
        /// </summary>
        public int UpdateRetryCount
        {
            get
            {
                return m_UpdateRetryCount;
            }
            set
            {
                m_UpdateRetryCount = value;
            }
        }

        /// <summary>
        /// 获取正在更新的资源组。
        /// </summary>
        public IResGroup UpdatingResGroup
        {
            get
            {
                return m_UpdatingResGroup;
            }
        }

        /// <summary>
        /// 获取等待更新资源数量。
        /// </summary>
        public int UpdateWaitingCount
        {
            get
            {
                return m_UpdateWaitingInfo.Count;
            }
        }

        /// <summary>
        /// 获取候选更新资源数量。
        /// </summary>
        public int UpdateCandidateCount
        {
            get
            {
                return m_UpdateCandidateInfo.Count;
            }
        }

        /// <summary>
        /// 获取正在更新资源数量。
        /// </summary>
        public int UpdatingCount
        {
            get
            {
                return m_UpdatingCount;
            }
        }

        /// <summary>
        /// 资源更新器轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (m_ApplyingResPackStream != null)
            {
                while (m_ApplyWaitingInfo.Count > 0)
                {
                    ApplyInfo applyInfo = m_ApplyWaitingInfo[0];
                    m_ApplyWaitingInfo.RemoveAt(0);
                    if (ApplyRes(applyInfo))
                    {
                        return;
                    }
                }

                Array.Clear(m_CachedBytes, 0, CachedBytesLength);
                string ResPackPath = m_ApplyingResPackPath;
                m_ApplyingResPackPath = null;
                m_ApplyingResPackStream.Dispose();
                m_ApplyingResPackStream = null;
                if (ResApplyComplete != null)
                {
                    ResApplyComplete(ResPackPath, !m_FailureFlag, m_UpdateCandidateInfo.Count <= 0);
                }
            }

            if (m_UpdateWaitingInfo.Count > 0)
            {
                if (m_DownloadManager.FreeAgentCount > 0)
                {
                    UpdateInfo updateInfo = m_UpdateWaitingInfo[0];
                    m_UpdateWaitingInfo.RemoveAt(0);
                    string ResFullNameWithCrc32 = updateInfo.ResName.Variant != null ? Utility.Text.Format("{0}.{1}.{2:x8}.{3}", updateInfo.ResName.Name, updateInfo.ResName.Variant, updateInfo.HashCode, DefaultExtension) : Utility.Text.Format("{0}.{1:x8}.{2}", updateInfo.ResName.Name, updateInfo.HashCode, DefaultExtension);
                    m_DownloadManager.AddDownload(updateInfo.ResPath, Utility.Path.GetRemotePath(Path.Combine(m_ResManager.m_UpdatePrefixUri, ResFullNameWithCrc32)), updateInfo);
                    m_UpdatingCount++;
                }

                return;
            }

            if (m_UpdatingResGroup != null && m_UpdatingCount <= 0)
            {
                ResGroup updatingResGroup = m_UpdatingResGroup;
                m_UpdatingResGroup = null;
                if (ResUpdateComplete != null)
                {
                    ResUpdateComplete(updatingResGroup, !m_FailureFlag, m_UpdateCandidateInfo.Count <= 0);
                }

                return;
            }
        }

        /// <summary>
        /// 关闭并清理资源更新器。
        /// </summary>
        public void Shutdown()
        {
            if (m_DownloadManager != null)
            {
                m_DownloadManager.DownloadStart -= OnDownloadStart;
                m_DownloadManager.DownloadUpdate -= OnDownloadUpdate;
                m_DownloadManager.DownloadSuccess -= OnDownloadSuccess;
                m_DownloadManager.DownloadFailure -= OnDownloadFailure;
            }

            m_UpdateWaitingInfo.Clear();
            m_UpdateCandidateInfo.Clear();
            m_CachedFileSystemsForGenerateReadWriteVersionList.Clear();
        }

        /// <summary>
        /// 设置下载管理器。
        /// </summary>
        /// <param name="downloadManager">下载管理器。</param>
        public void SetDownloadManager(IDownloadManager downloadManager)
        {
            if (downloadManager == null)
            {
                throw new GameFrameworkException("Download manager is invalid.");
            }

            m_DownloadManager = downloadManager;
            m_DownloadManager.DownloadStart += OnDownloadStart;
            m_DownloadManager.DownloadUpdate += OnDownloadUpdate;
            m_DownloadManager.DownloadSuccess += OnDownloadSuccess;
            m_DownloadManager.DownloadFailure += OnDownloadFailure;
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
            m_UpdateCandidateInfo.Add(ResName, new UpdateInfo(ResName, fileSystemName, loadType, length, hashCode, zipLength, zipHashCode, ResPath));
        }

        /// <summary>
        /// 检查资源完成。
        /// </summary>
        /// <param name="needGenerateReadWriteVersionList">是否需要生成读写区版本资源列表。</param>
        public void CheckResComplete(bool needGenerateReadWriteVersionList)
        {
            m_CheckRessComplete = true;
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
            if (!m_CheckRessComplete)
            {
                throw new GameFrameworkException("You must check Ress complete first.");
            }

            if (m_ApplyingResPackStream != null)
            {
                throw new GameFrameworkException(Utility.Text.Format("There is already a Res pack '{0}' being applied.", m_ApplyingResPackPath));
            }

            if (m_UpdatingResGroup != null)
            {
                throw new GameFrameworkException(Utility.Text.Format("There is already a Res group '{0}' being updated.", m_UpdatingResGroup.Name));
            }

            try
            {
                long length = 0L;
                ResPackVersionList versionList = default(ResPackVersionList);
                using (FileStream fileStream = new FileStream(ResPackPath, FileMode.Open, FileAccess.Read))
                {
                    length = fileStream.Length;
                    versionList = m_ResManager.m_ResPackVersionListSerializer.Deserialize(fileStream);
                }

                if (!versionList.IsValid)
                {
                    throw new GameFrameworkException("Deserialize Res pack version list failure.");
                }

                if (versionList.Offset + versionList.Length != length)
                {
                    throw new GameFrameworkException("Res pack length is invalid.");
                }

                m_ApplyingResPackPath = ResPackPath;
                m_ApplyingResPackStream = new FileStream(ResPackPath, FileMode.Open, FileAccess.Read);
                m_ApplyingResPackStream.Position = versionList.Offset;
                m_FailureFlag = false;

                ResPackVersionList.Res[] Ress = versionList.GetRess();
                foreach (ResPackVersionList.Res Res in Ress)
                {
                    ResName ResName = new ResName(Res.Name, Res.Variant, Res.Extension);
                    UpdateInfo updateInfo = null;
                    if (!m_UpdateCandidateInfo.TryGetValue(ResName, out updateInfo))
                    {
                        continue;
                    }

                    if (updateInfo.LoadType == (LoadType)Res.LoadType && updateInfo.Length == Res.Length && updateInfo.HashCode == Res.HashCode)
                    {
                        m_ApplyWaitingInfo.Add(new ApplyInfo(ResName, updateInfo.FileSystemName, (LoadType)Res.LoadType, Res.Offset, Res.Length, Res.HashCode, Res.ZipLength, Res.ZipHashCode, updateInfo.ResPath));
                    }
                }
            }
            catch (Exception exception)
            {
                if (m_ApplyingResPackStream != null)
                {
                    m_ApplyingResPackStream.Dispose();
                    m_ApplyingResPackStream = null;
                }

                throw new GameFrameworkException(Utility.Text.Format("Apply Ress '{0}' with exception '{1}'.", ResPackPath, exception.ToString()), exception);
            }
        }

        /// <summary>
        /// 更新指定资源组的资源。
        /// </summary>
        /// <param name="ResGroup">要更新的资源组。</param>
        public void UpdateRess(ResGroup ResGroup)
        {
            if (m_DownloadManager == null)
            {
                throw new GameFrameworkException("You must set download manager first.");
            }

            if (!m_CheckRessComplete)
            {
                throw new GameFrameworkException("You must check Ress complete first.");
            }

            if (m_ApplyingResPackStream != null)
            {
                throw new GameFrameworkException(Utility.Text.Format("There is already a Res pack '{0}' being applied.", m_ApplyingResPackPath));
            }

            if (m_UpdatingResGroup != null)
            {
                throw new GameFrameworkException(Utility.Text.Format("There is already a Res group '{0}' being updated.", m_UpdatingResGroup.Name));
            }

            if (string.IsNullOrEmpty(ResGroup.Name))
            {
                foreach (KeyValuePair<ResName, UpdateInfo> updateInfo in m_UpdateCandidateInfo)
                {
                    m_UpdateWaitingInfo.Add(updateInfo.Value);
                }

                m_UpdateCandidateInfo.Clear();
            }
            else
            {
                ResName[] ResNames = ResGroup.InternalGetResNames();
                foreach (ResName ResName in ResNames)
                {
                    UpdateInfo updateInfo = null;
                    if (!m_UpdateCandidateInfo.TryGetValue(ResName, out updateInfo))
                    {
                        continue;
                    }

                    m_UpdateWaitingInfo.Add(updateInfo);
                    m_UpdateCandidateInfo.Remove(ResName);
                }
            }

            m_UpdatingResGroup = ResGroup;
            m_FailureFlag = false;
        }

        public void UpdateRes(ResName ResName)
        {
            if (m_DownloadManager == null)
            {
                throw new GameFrameworkException("You must set download manager first.");
            }

            if (!m_CheckRessComplete)
            {
                throw new GameFrameworkException("You must check Ress complete first.");
            }

            if (m_ApplyingResPackStream != null)
            {
                throw new GameFrameworkException(Utility.Text.Format("There is already a Res pack '{0}' being applied.", m_ApplyingResPackPath));
            }

            UpdateInfo updateInfo = null;
            if (m_UpdateCandidateInfo.TryGetValue(ResName, out updateInfo))
            {
                m_UpdateWaitingInfo.Add(updateInfo);
                m_UpdateCandidateInfo.Remove(ResName);
            }
        }

        private bool ApplyRes(ApplyInfo applyInfo)
        {
            long position = m_ApplyingResPackStream.Position;
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

                m_ApplyingResPackStream.Position += applyInfo.Offset;
                using (FileStream fileStream = new FileStream(applyInfo.ResPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    while ((bytesRead = m_ApplyingResPackStream.Read(m_CachedBytes, 0, bytesLeft < CachedBytesLength ? bytesLeft : CachedBytesLength)) > 0)
                    {
                        bytesLeft -= bytesRead;
                        fileStream.Write(m_CachedBytes, 0, bytesRead);
                    }

                    if (zip)
                    {
                        fileStream.Position = 0L;
                        int hashCode = Utility.Verifier.GetCrc32(fileStream);
                        if (hashCode != applyInfo.ZipHashCode)
                        {
                            if (ResApplyFailure != null)
                            {
                                string errorMessage = Utility.Text.Format("Res zip hash code error, need '{0}', applied '{1}'.", applyInfo.ZipHashCode.ToString(), hashCode.ToString());
                                ResApplyFailure(applyInfo.ResName, m_ApplyingResPackPath, errorMessage);
                            }

                            return false;
                        }

                        if (m_ResManager.m_DecompressCachedStream == null)
                        {
                            m_ResManager.m_DecompressCachedStream = new MemoryStream();
                        }

                        fileStream.Position = 0L;
                        m_ResManager.m_DecompressCachedStream.Position = 0L;
                        m_ResManager.m_DecompressCachedStream.SetLength(0L);
                        if (!Utility.Zip.Decompress(fileStream, m_ResManager.m_DecompressCachedStream))
                        {
                            if (ResApplyFailure != null)
                            {
                                string errorMessage = Utility.Text.Format("Unable to decompress Res '{0}'.", applyInfo.ResPath);
                                ResApplyFailure(applyInfo.ResName, m_ApplyingResPackPath, errorMessage);
                            }

                            return false;
                        }

                        fileStream.Position = 0L;
                        fileStream.SetLength(0L);
                        fileStream.Write(m_ResManager.m_DecompressCachedStream.GetBuffer(), 0, (int)m_ResManager.m_DecompressCachedStream.Length);
                    }
                    else
                    {
                        int hashCode = 0;
                        fileStream.Position = 0L;
                        if (applyInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || applyInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt
                            || applyInfo.LoadType == LoadType.LoadFromBinaryAndQuickDecrypt || applyInfo.LoadType == LoadType.LoadFromBinaryAndDecrypt)
                        {
                            Utility.Converter.GetBytes(applyInfo.HashCode, m_CachedHashBytes);
                            if (applyInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || applyInfo.LoadType == LoadType.LoadFromBinaryAndQuickDecrypt)
                            {
                                hashCode = Utility.Verifier.GetCrc32(fileStream, m_CachedHashBytes, Utility.Encryption.QuickEncryptLength);
                            }
                            else if (applyInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt || applyInfo.LoadType == LoadType.LoadFromBinaryAndDecrypt)
                            {
                                hashCode = Utility.Verifier.GetCrc32(fileStream, m_CachedHashBytes, applyInfo.Length);
                            }

                            Array.Clear(m_CachedHashBytes, 0, CachedHashBytesLength);
                        }
                        else
                        {
                            hashCode = Utility.Verifier.GetCrc32(fileStream);
                        }

                        if (hashCode != applyInfo.HashCode)
                        {
                            if (ResApplyFailure != null)
                            {
                                string errorMessage = Utility.Text.Format("Res hash code error, need '{0}', applied '{1}'.", applyInfo.HashCode.ToString(), hashCode.ToString());
                                ResApplyFailure(applyInfo.ResName, m_ApplyingResPackPath, errorMessage);
                            }

                            return false;
                        }
                    }
                }

                if (applyInfo.UseFileSystem)
                {
                    IFileSystem fileSystem = m_ResManager.GetFileSystem(applyInfo.FileSystemName, false);
                    bool retVal = fileSystem.WriteFile(applyInfo.ResName.FullName, applyInfo.ResPath);
                    if (File.Exists(applyInfo.ResPath))
                    {
                        File.Delete(applyInfo.ResPath);
                    }

                    return retVal;
                }

                m_UpdateCandidateInfo.Remove(applyInfo.ResName);
                m_ResManager.m_ResInfos[applyInfo.ResName].MarkReady();
                m_ResManager.m_ReadWriteResInfos.Add(applyInfo.ResName, new ReadWriteResInfo(applyInfo.FileSystemName, applyInfo.LoadType, applyInfo.Length, applyInfo.HashCode));

                if (ResApplySuccess != null)
                {
                    ResApplySuccess(applyInfo.ResName, applyInfo.ResPath, m_ApplyingResPackPath, applyInfo.Length, applyInfo.ZipLength);
                }

                string downloadingRes = Utility.Text.Format("{0}.download", applyInfo.ResPath);
                if (File.Exists(downloadingRes))
                {
                    File.Delete(downloadingRes);
                }

                m_CurrentGenerateReadWriteVersionListLength += applyInfo.ZipLength;
                if (m_ApplyWaitingInfo.Count <= 0 || m_CurrentGenerateReadWriteVersionListLength >= m_GenerateReadWriteVersionListLength)
                {
                    m_CurrentGenerateReadWriteVersionListLength = 0;
                    GenerateReadWriteVersionList();
                    return true;
                }

                return false;
            }
            catch (Exception exception)
            {
                if (ResApplyFailure != null)
                {
                    ResApplyFailure(applyInfo.ResName, m_ApplyingResPackPath, exception.ToString());
                }

                return false;
            }
            finally
            {
                m_ApplyingResPackStream.Position = position;
            }
        }

        private void GenerateReadWriteVersionList()
        {
            if (File.Exists(m_ReadWriteVersionListFileName))
            {
                if (File.Exists(m_ReadWriteVersionListBackupFileName))
                {
                    File.Delete(m_ReadWriteVersionListBackupFileName);
                }

                File.Move(m_ReadWriteVersionListFileName, m_ReadWriteVersionListBackupFileName);
            }

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(m_ReadWriteVersionListFileName, FileMode.Create, FileAccess.Write);
                LocalVersionList.Res[] Ress = m_ResManager.m_ReadWriteResInfos.Count > 0 ? new LocalVersionList.Res[m_ResManager.m_ReadWriteResInfos.Count] : null;
                if (Ress != null)
                {
                    int index = 0;
                    foreach (KeyValuePair<ResName, ReadWriteResInfo> i in m_ResManager.m_ReadWriteResInfos)
                    {
                        Ress[index] = new LocalVersionList.Res(i.Key.Name, i.Key.Variant, i.Key.Extension, (byte)i.Value.LoadType, i.Value.Length, i.Value.HashCode);
                        if (i.Value.UseFileSystem)
                        {
                            List<int> ResIndexes = null;
                            if (!m_CachedFileSystemsForGenerateReadWriteVersionList.TryGetValue(i.Value.FileSystemName, out ResIndexes))
                            {
                                ResIndexes = new List<int>();
                                m_CachedFileSystemsForGenerateReadWriteVersionList.Add(i.Value.FileSystemName, ResIndexes);
                            }

                            ResIndexes.Add(index);
                        }

                        index++;
                    }
                }

                LocalVersionList.FileSystem[] fileSystems = m_CachedFileSystemsForGenerateReadWriteVersionList.Count > 0 ? new LocalVersionList.FileSystem[m_CachedFileSystemsForGenerateReadWriteVersionList.Count] : null;
                if (fileSystems != null)
                {
                    int index = 0;
                    foreach (KeyValuePair<string, List<int>> i in m_CachedFileSystemsForGenerateReadWriteVersionList)
                    {
                        fileSystems[index++] = new LocalVersionList.FileSystem(i.Key, i.Value.ToArray());
                        i.Value.Clear();
                    }
                }

                LocalVersionList versionList = new LocalVersionList(Ress, fileSystems);
                if (!m_ResManager.m_ReadWriteVersionListSerializer.Serialize(fileStream, versionList))
                {
                    throw new GameFrameworkException("Serialize read write version list failure.");
                }

                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }

                if (File.Exists(m_ReadWriteVersionListBackupFileName))
                {
                    File.Delete(m_ReadWriteVersionListBackupFileName);
                }
            }
            catch (Exception exception)
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                    fileStream = null;
                }

                if (File.Exists(m_ReadWriteVersionListFileName))
                {
                    File.Delete(m_ReadWriteVersionListFileName);
                }

                if (File.Exists(m_ReadWriteVersionListBackupFileName))
                {
                    File.Move(m_ReadWriteVersionListBackupFileName, m_ReadWriteVersionListFileName);
                }

                throw new GameFrameworkException(Utility.Text.Format("Generate read write version list exception '{0}'.", exception.ToString()), exception);
            }
        }

        private void OnDownloadStart(object sender, DownloadStartEventArgs e)
        {
            UpdateInfo updateInfo = e.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (m_DownloadManager == null)
            {
                throw new GameFrameworkException("You must set download manager first.");
            }

            if (ResUpdateStart != null)
            {
                ResUpdateStart(updateInfo.ResName, e.DownloadPath, e.DownloadUri, e.CurrentLength, updateInfo.ZipLength, updateInfo.RetryCount);
            }
        }

        private void OnDownloadUpdate(object sender, DownloadUpdateEventArgs e)
        {
            UpdateInfo updateInfo = e.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (m_DownloadManager == null)
            {
                throw new GameFrameworkException("You must set download manager first.");
            }

            if (e.CurrentLength > updateInfo.ZipLength)
            {
                m_DownloadManager.RemoveDownload(e.SerialId);
                string downloadFile = Utility.Text.Format("{0}.download", e.DownloadPath);
                if (File.Exists(downloadFile))
                {
                    File.Delete(downloadFile);
                }

                string errorMessage = Utility.Text.Format("When download update, downloaded length is larger than zip length, need '{0}', downloaded '{1}'.", updateInfo.ZipLength.ToString(), e.CurrentLength.ToString());
                DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                OnDownloadFailure(this, downloadFailureEventArgs);
                ReferencePool.Release(downloadFailureEventArgs);
                return;
            }

            if (ResUpdateChanged != null)
            {
                ResUpdateChanged(updateInfo.ResName, e.DownloadPath, e.DownloadUri, e.CurrentLength, updateInfo.ZipLength);
            }
        }

        private void OnDownloadSuccess(object sender, DownloadSuccessEventArgs e)
        {
            UpdateInfo updateInfo = e.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            using (FileStream fileStream = new FileStream(e.DownloadPath, FileMode.Open, FileAccess.ReadWrite))
            {
                bool zip = updateInfo.Length != updateInfo.ZipLength || updateInfo.HashCode != updateInfo.ZipHashCode;

                int length = (int)fileStream.Length;
                if (length != updateInfo.ZipLength)
                {
                    fileStream.Close();
                    string errorMessage = Utility.Text.Format("Res zip length error, need '{0}', downloaded '{1}'.", updateInfo.ZipLength.ToString(), length.ToString());
                    DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                    OnDownloadFailure(this, downloadFailureEventArgs);
                    ReferencePool.Release(downloadFailureEventArgs);
                    return;
                }

                if (zip)
                {
                    fileStream.Position = 0L;
                    int hashCode = Utility.Verifier.GetCrc32(fileStream);
                    if (hashCode != updateInfo.ZipHashCode)
                    {
                        fileStream.Close();
                        string errorMessage = Utility.Text.Format("Res zip hash code error, need '{0}', downloaded '{1}'.", updateInfo.ZipHashCode.ToString(), hashCode.ToString());
                        DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                        OnDownloadFailure(this, downloadFailureEventArgs);
                        ReferencePool.Release(downloadFailureEventArgs);
                        return;
                    }

                    if (m_ResManager.m_DecompressCachedStream == null)
                    {
                        m_ResManager.m_DecompressCachedStream = new MemoryStream();
                    }

                    try
                    {
                        fileStream.Position = 0L;
                        m_ResManager.m_DecompressCachedStream.Position = 0L;
                        m_ResManager.m_DecompressCachedStream.SetLength(0L);
                        if (!Utility.Zip.Decompress(fileStream, m_ResManager.m_DecompressCachedStream))
                        {
                            fileStream.Close();
                            string errorMessage = Utility.Text.Format("Unable to decompress Res '{0}'.", e.DownloadPath);
                            DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                            OnDownloadFailure(this, downloadFailureEventArgs);
                            ReferencePool.Release(downloadFailureEventArgs);
                            return;
                        }

                        if (m_ResManager.m_DecompressCachedStream.Length != updateInfo.Length)
                        {
                            fileStream.Close();
                            string errorMessage = Utility.Text.Format("Res length error, need '{0}', downloaded '{1}'.", updateInfo.Length.ToString(), m_ResManager.m_DecompressCachedStream.Length.ToString());
                            DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                            OnDownloadFailure(this, downloadFailureEventArgs);
                            ReferencePool.Release(downloadFailureEventArgs);
                            return;
                        }

                        fileStream.Position = 0L;
                        fileStream.SetLength(0L);
                        fileStream.Write(m_ResManager.m_DecompressCachedStream.GetBuffer(), 0, (int)m_ResManager.m_DecompressCachedStream.Length);
                    }
                    catch (Exception exception)
                    {
                        fileStream.Close();
                        string errorMessage = Utility.Text.Format("Unable to decompress Res '{0}' with error message '{1}'.", e.DownloadPath, exception.ToString());
                        DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                        OnDownloadFailure(this, downloadFailureEventArgs);
                        ReferencePool.Release(downloadFailureEventArgs);
                        return;
                    }
                    finally
                    {
                        m_ResManager.m_DecompressCachedStream.Position = 0L;
                        m_ResManager.m_DecompressCachedStream.SetLength(0L);
                    }
                }
                else
                {
                    int hashCode = 0;
                    fileStream.Position = 0L;
                    if (updateInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || updateInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt
                        || updateInfo.LoadType == LoadType.LoadFromBinaryAndQuickDecrypt || updateInfo.LoadType == LoadType.LoadFromBinaryAndDecrypt)
                    {
                        Utility.Converter.GetBytes(updateInfo.HashCode, m_CachedHashBytes);
                        if (updateInfo.LoadType == LoadType.LoadFromMemoryAndQuickDecrypt || updateInfo.LoadType == LoadType.LoadFromBinaryAndQuickDecrypt)
                        {
                            hashCode = Utility.Verifier.GetCrc32(fileStream, m_CachedHashBytes, Utility.Encryption.QuickEncryptLength);
                        }
                        else if (updateInfo.LoadType == LoadType.LoadFromMemoryAndDecrypt || updateInfo.LoadType == LoadType.LoadFromBinaryAndDecrypt)
                        {
                            hashCode = Utility.Verifier.GetCrc32(fileStream, m_CachedHashBytes, length);
                        }

                        Array.Clear(m_CachedHashBytes, 0, CachedHashBytesLength);
                    }
                    else
                    {
                        hashCode = Utility.Verifier.GetCrc32(fileStream);
                    }

                    if (hashCode != updateInfo.HashCode)
                    {
                        fileStream.Close();
                        string errorMessage = Utility.Text.Format("Res hash code error, need '{0}', downloaded '{1}'.", updateInfo.HashCode.ToString(), hashCode.ToString());
                        DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                        OnDownloadFailure(this, downloadFailureEventArgs);
                        ReferencePool.Release(downloadFailureEventArgs);
                        return;
                    }
                }
            }

            if (updateInfo.UseFileSystem)
            {
                IFileSystem fileSystem = m_ResManager.GetFileSystem(updateInfo.FileSystemName, false);
                bool retVal = fileSystem.WriteFile(updateInfo.ResName.FullName, updateInfo.ResPath);
                if (File.Exists(updateInfo.ResPath))
                {
                    File.Delete(updateInfo.ResPath);
                }

                if (!retVal)
                {
                    string errorMessage = Utility.Text.Format("Write Res to file system '{0}' error.", fileSystem.FullPath);
                    DownloadFailureEventArgs downloadFailureEventArgs = DownloadFailureEventArgs.Create(e.SerialId, e.DownloadPath, e.DownloadUri, errorMessage, e.UserData);
                    OnDownloadFailure(this, downloadFailureEventArgs);
                    return;
                }
            }

            m_UpdatingCount--;
            m_ResManager.m_ResInfos[updateInfo.ResName].MarkReady();
            m_ResManager.m_ReadWriteResInfos.Add(updateInfo.ResName, new ReadWriteResInfo(updateInfo.FileSystemName, updateInfo.LoadType, updateInfo.Length, updateInfo.HashCode));
            m_CurrentGenerateReadWriteVersionListLength += updateInfo.ZipLength;
            if (m_UpdatingCount <= 0 || m_CurrentGenerateReadWriteVersionListLength >= m_GenerateReadWriteVersionListLength)
            {
                m_CurrentGenerateReadWriteVersionListLength = 0;
                GenerateReadWriteVersionList();
            }

            if (ResUpdateSuccess != null)
            {
                ResUpdateSuccess(updateInfo.ResName, e.DownloadPath, e.DownloadUri, updateInfo.Length, updateInfo.ZipLength);
            }
        }

        private void OnDownloadFailure(object sender, DownloadFailureEventArgs e)
        {
            UpdateInfo updateInfo = e.UserData as UpdateInfo;
            if (updateInfo == null)
            {
                return;
            }

            if (File.Exists(e.DownloadPath))
            {
                File.Delete(e.DownloadPath);
            }

            m_UpdatingCount--;

            if (ResUpdateFailure != null)
            {
                ResUpdateFailure(updateInfo.ResName, e.DownloadUri, updateInfo.RetryCount, m_UpdateRetryCount, e.ErrorMessage);
            }

            if (updateInfo.RetryCount < m_UpdateRetryCount)
            {
                updateInfo.RetryCount++;
                m_UpdateWaitingInfo.Add(updateInfo);
            }
            else
            {
                m_FailureFlag = true;
                updateInfo.RetryCount = 0;
                m_UpdateCandidateInfo.Add(updateInfo.ResName, updateInfo);
            }
        }
    }
}
