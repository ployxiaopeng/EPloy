using EPloy.Event;
using EPloy.Res;
using System.Collections.Generic;

/// <summary>
/// 场景组件。
/// </summary>
public class SceneModule : IGameModule
{
    private List<string> LoadingSceneAssetNames;
    private LoadSceneCallbacks LoadSceneCallbacks;
    private UnloadSceneCallbacks UnloadSceneCallbacks;

    private ResModule Res
    {
        get { return GameModule.Res; }
    }

    public void Awake()
    {
        LoadingSceneAssetNames = new List<string>();
        LoadSceneCallbacks = new LoadSceneCallbacks(LoadSceneSuccessCallback, LoadSceneFailureCallback,
            LoadSceneDependencyAssetCallback);
    }

    public void Update()
    {

    }

    public void OnDestroy()
    {
        LoadingSceneAssetNames.Clear();
    }

    /// <summary>
    /// 获取场景是否正在加载。
    /// </summary>
    /// <param name="sceneAssetName">场景资源名称。</param>
    /// <returns>场景是否正在加载。</returns>
    public bool SceneIsLoading(string sceneAssetName)
    {
        if (string.IsNullOrEmpty(sceneAssetName))
        {
            Log.Fatal("Scene asset name is invalid.");
            return false;
        }

        return LoadingSceneAssetNames.Contains(sceneAssetName);
    }

    /// <summary>
    /// 获取正在加载场景的资源名称。
    /// </summary>
    /// <returns>正在加载场景的资源名称。</returns>
    public string[] GetLoadingSceneAssetNames()
    {
        return LoadingSceneAssetNames.ToArray();
    }

    /// <summary>
    /// 获取正在加载场景的资源名称。
    /// </summary>
    /// <param name="results">正在加载场景的资源名称。</param>
    public void GetLoadingSceneAssetNames(List<string> results)
    {
        if (results == null)
        {
            Log.Fatal("Results is invalid.");
            return;
        }

        results.Clear();
        results.AddRange(LoadingSceneAssetNames);
    }

    /// <summary>
    /// 检查场景资源是否存在。
    /// </summary>
    /// <param name="sceneAssetName">要检查场景资源的名称。</param>
    /// <returns>场景资源是否存在。</returns>
    public bool HasScene(string sceneAssetName)
    {
        return Res.HasAsset(sceneAssetName) != HasResult.NotExist;
    }

    /// <summary>
    /// 加载场景。
    /// </summary>
    /// <param name="sceneAssetName">场景资源名称。</param>
    public void LoadScene(string sceneAssetName)
    {
        if (string.IsNullOrEmpty(sceneAssetName))
        {
            Log.Fatal("Scene asset name is invalid.");
            return;
        }

        if (SceneIsLoading(sceneAssetName))
        {
            Log.Fatal(UtilText.Format("Scene asset '{0}' is being loaded.", sceneAssetName));
            return;
        }

        LoadingSceneAssetNames.Add(sceneAssetName);
        Res.LoadScene(sceneAssetName, LoadSceneCallbacks);
    }

    /// <summary>
    /// 慎用卸载场景 在非编辑器模式下 直接删除ab包
    /// </summary>
    /// <param name="sceneAssetName">场景资源名称。</param>
    public void UnloadScene(string sceneAssetName)
    {
        if (string.IsNullOrEmpty(sceneAssetName))
        {
            Log.Fatal("Scene asset name is invalid.");
            return;
        }
        Res.UnloadScene(sceneAssetName, UnloadSceneCallbacks);
    }

    private void LoadSceneSuccessCallback(string sceneAssetName, float duration)
    {
        LoadingSceneAssetNames.Remove(sceneAssetName);
        LoadSceneEvt loadSceneEvt = ReferencePool.Acquire<LoadSceneEvt>();
        loadSceneEvt.SetSuccessData(sceneAssetName);
        GameModule.Event.Fire(loadSceneEvt);
    }

    private void LoadSceneFailureCallback(string sceneAssetName, LoadResStatus status, string errorMessage)
    {
        LoadingSceneAssetNames.Remove(sceneAssetName);
        string appendErrorMessage =
            UtilText.Format("Load scene failure, scene asset name '{0}', status '{1}', error message '{2}'.",
                sceneAssetName, status.ToString(), errorMessage);
        Log.Fatal(appendErrorMessage);
        LoadSceneEvt loadSceneEvt = ReferencePool.Acquire<LoadSceneEvt>();
        loadSceneEvt.SetFailureData(sceneAssetName, appendErrorMessage);
        GameModule.Event.Fire(loadSceneEvt);
    }

    private void LoadSceneDependencyAssetCallback(string sceneAssetName, string dependencyAssetName,
        int loadedCount, int totalCount)
    {
        LoadSceneEvt loadSceneEvt = ReferencePool.Acquire<LoadSceneEvt>();
        loadSceneEvt.SetDependPercentData(sceneAssetName, loadedCount * 100 / totalCount);
        GameModule.Event.Fire(loadSceneEvt);
    }
}
