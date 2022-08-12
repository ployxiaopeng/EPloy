using System.Collections.Generic;
using System.IO;
using EPloy.Checker;
using EPloy.Res;
using EPloy.SystemFile;

/// <summary>
/// 资源更新
/// </summary>
public sealed class ResUpdaterModule : IGameModule
{
    internal string BackupExtension = "bak";
    private ResStore ResStore;
    private ResChecker ResChecker;
    private UpdaterHandler UpdaterHandler;
    private string ResPath
    {
        get
        {
            return GameEntry.ResPath;
        }
    }
    public MemoryStream DecompressCachedStream { get; set; }

    /// <summary>
    ///  更新资源需要的文件系统
    /// </summary>
    internal Dictionary<string, IFileSystem> ReadWriteFileSystems { get; private set; }

    /// <summary>
    ///  更新资源URL
    /// </summary>
    public string UpdatePrefixUri { get; set; }

    private CheckResCompleteCallback CheckResCompleteCallback;
    private UpdateResCallBack UpdateResCallBack;


    public void Awake()
    {
        ResStore = new ResStore();
        ResChecker = new ResChecker(this, ResStore);
        ResChecker.ResCheckerCallBack = new ResCheckerCallBack(OnResNeedUpdate, OnResCheckComplete);
        UpdaterHandler = new UpdaterHandler(this, ResStore);
        ReadWriteFileSystems = new Dictionary<string, IFileSystem>();
    }

    public void Update()
    {

    }

    public void OnDestroy()
    {
        ReadWriteFileSystems.Clear();
    }

    public AssetInfo GetAssetInfo(string assetName)
    {
        return ResStore.GetAssetInfo(assetName);
    }

    public ResInfo GetResInfo(ResName resName)
    {
        return ResStore.GetResInfo(resName);
    }

    public ResInfo GetResInfo(string assetName)
    {
        return ResStore.GetResInfo(assetName);
    }

    /// <summary>
    /// 检查资源 如需更新  检查并装备好更新列表
    /// </summary>
    /// <param name="checkCallback">完成时结果的</param>
    public void CheckRes(CheckResCompleteCallback checkResCompleteCallback)
    {
        CheckResCompleteCallback = checkResCompleteCallback;
        if (CheckResCompleteCallback == null)
        {
            Log.Fatal("Check resources complete callback is invalid.");
            return;
        }

        if (GameEntry.GameModel == GameModel.HotFix)
        {
            ResChecker.CheckResources();
        }
        else
        {
            ResChecker.CheckPackageResources();
        }
    }

    /// <summary>
    /// 更新指定资源组的资源。
    /// </summary>
    /// <param name="resourceGroupName">要更新的资源组名称 默认组为   string.Empty</param>
    /// <param name="updateResCompleteCallback">更新指定资源组完成时的回调函数。</param>
    public void UpdateRes(UpdateResCallBack updateResCallBack, string resGroupName = null)
    {
        if (UpdateResCallBack == null)
        {
            Log.Fatal("UpdateResCallBack callback is invalid.");
        }

        if (resGroupName == null)
        {
            resGroupName = string.Empty;
        }

        ResGroup resGroup = ResStore.GetResGroup(resGroupName);
        if (resGroup == null)
        {
            Log.Fatal(UtilText.Format("Can not find resource group '{0}'.", resGroupName));
            return;
        }

        UpdateResCallBack = updateResCallBack;
        UpdaterHandler.UpdateRess(resGroup);
    }

    internal IFileSystem GetFileSystem(string fileSystemName, bool storageInReadOnly)
    {
        if (string.IsNullOrEmpty(fileSystemName))
        {
            Log.Fatal("File system name is invalid.");
            return null;
        }

        IFileSystem fileSystem = null;
        if (!ReadWriteFileSystems.TryGetValue(fileSystemName, out fileSystem))
        {
            string fullPath = UtilPath.GetRegularPath(Path.Combine(ResPath,
                UtilText.Format("{0}.{1}", fileSystemName, ConfigVersion.DefaultExtension)));
            fileSystem = GameEntry.FileSystem.GetFileSystem(fullPath);
            if (fileSystem == null)
            {
                if (File.Exists(fullPath))
                {
                    fileSystem = GameEntry.FileSystem.LoadFileSystem(fullPath, FileSystemAccess.ReadWrite);
                }
                else
                {
                    string directory = Path.GetDirectoryName(fullPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    fileSystem = GameEntry.FileSystem.CreateFileSystem(fullPath, FileSystemAccess.ReadWrite,
                        ConfigVersion.FileSystemMaxFileCount, ConfigVersion.FileSystemMaxBlockCount);
                }

                ReadWriteFileSystems.Add(fileSystemName, fileSystem);
            }
        }

        return fileSystem;
    }

    private void OnResNeedUpdate(ResName resourceName, string fileSystemName, LoadType loadType, int length,
        int hashCode, int zipLength, int zipHashCode)
    {
        UpdaterHandler.AddResUpdate(resourceName, fileSystemName, loadType, length, hashCode, zipLength,
            zipHashCode, UtilPath.GetRegularPath(Path.Combine(ResPath, resourceName.Name)));
    }

    private void OnResCheckComplete(int movedCount, int removedCount, int updateCount, long updateTotalLength,
        long updateTotalZipLength)
    {
        CheckerHelper.OnDestroy();
        ResChecker.OnDestroy();
        UpdaterHandler.CheckResComplete(movedCount > 0 || removedCount > 0);

        if (updateCount <= 0)
        {
            UpdaterHandler.OnDestroy();

            ResStore.ReadWriteResInfos.Clear();
            ResStore.ReadWriteResInfos = null;

            if (DecompressCachedStream != null)
            {
                DecompressCachedStream.Dispose();
                DecompressCachedStream = null;
            }
        }

        CheckResCompleteCallback(movedCount, removedCount, updateCount, updateTotalLength, updateTotalZipLength);
        CheckResCompleteCallback = null;
    }

    private void OnResApplyComplete(string resPackPath, bool result, bool isAllDone)
    {
        if (isAllDone)
        {
            UpdaterHandler.ResApplyComplete -= OnResApplyComplete;
            UpdaterHandler.ResUpdateStart -= OnUpdateStart;
            UpdaterHandler.ResUpdateChanged -= OnUpdateChanged;
            UpdaterHandler.ResUpdateSuccess -= OnUpdateSuccess;
            UpdaterHandler.ResUpdateFailure -= OnUpdateFailure;
            UpdaterHandler.ResUpdateComplete -= OnUpdateComplete;
            UpdaterHandler.OnDestroy();
            UpdaterHandler = null;

            ResStore.ReadWriteResInfos.Clear();
            ResStore.ReadWriteResInfos = null;

            if (DecompressCachedStream != null)
            {
                DecompressCachedStream.Dispose();
                DecompressCachedStream = null;
            }

            UtilPath.RemoveEmptyDirectory(ResPath);
        }

        if (UpdateResCallBack.ResApplyComplete != null)
        {
            UpdateResCallBack.ResApplyComplete(resPackPath, result);
            UpdateResCallBack.ResApplyComplete = null;
        }
    }

    private void OnUpdateStart(string resName)
    {
        UpdateResCallBack.ResUpdateStart(resName);
    }

    private void OnUpdateChanged(string resName, int currentLength)
    {
        UpdateResCallBack.ResUpdateChanged(resName, currentLength);
    }

    private void OnUpdateSuccess(string resName, int zipLength)
    {
        UpdateResCallBack.ResUpdateSuccess(resName, zipLength);
    }

    private void OnUpdateFailure(string resName, string errMsg, int retryCount, int totalRetryCount)
    {
        UpdateResCallBack.ResUpdateFailure(resName, errMsg, retryCount, totalRetryCount);
    }

    private void OnUpdateComplete(bool result, bool isAllDone)
    {
        if (isAllDone)
        {
            UpdaterHandler.ResApplyComplete -= OnResApplyComplete;
            UpdaterHandler.ResUpdateStart -= OnUpdateStart;
            UpdaterHandler.ResUpdateChanged -= OnUpdateChanged;
            UpdaterHandler.ResUpdateSuccess -= OnUpdateSuccess;
            UpdaterHandler.ResUpdateFailure -= OnUpdateFailure;
            UpdaterHandler.ResUpdateComplete -= OnUpdateComplete;
            UpdaterHandler.OnDestroy();
            UpdaterHandler = null;

            ResStore.ReadWriteResInfos.Clear();
            ResStore.ReadWriteResInfos = null;

            if (DecompressCachedStream != null)
            {
                DecompressCachedStream.Dispose();
                DecompressCachedStream = null;
            }

            UtilPath.RemoveEmptyDirectory(ResPath);
        }

        UpdateResCallBack.ResUpdateComplete(result);
        UpdateResCallBack.Dispose();
    }
}