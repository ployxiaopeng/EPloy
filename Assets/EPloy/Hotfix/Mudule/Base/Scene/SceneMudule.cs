using EPloy.Res;
using System.Collections.Generic;

namespace EPloy
{
    /// <summary>
    /// 场景组件。
    /// </summary>
    public class SceneMudule : IHotfixModule
    {
        private List<string> LoadedSceneAssetNames;
        private List<string> LoadingSceneAssetNames;
        private List<string> UnloadingSceneAssetNames;
        private LoadSceneCallbacks LoadSceneCallbacks;
        private UnloadSceneCallbacks UnloadSceneCallbacks;

        private ResMudule Res
        {
            get { return HotFixMudule.Res; }
        }

        public void Awake()
        {
            LoadedSceneAssetNames = new List<string>();
            LoadingSceneAssetNames = new List<string>();
            UnloadingSceneAssetNames = new List<string>();
            LoadSceneCallbacks = new LoadSceneCallbacks(LoadSceneSuccessCallback, LoadSceneFailureCallback,
                LoadSceneDependencyAssetCallback);
            UnloadSceneCallbacks = new UnloadSceneCallbacks(UnloadSceneSuccessCallback, UnloadSceneFailureCallback);
        }

        public void Update()
        {

        }

        public void OnDestroy()
        {
            string[] loadedSceneAssetNames = LoadedSceneAssetNames.ToArray();
            foreach (string loadedSceneAssetName in loadedSceneAssetNames)
            {
                if (SceneIsUnloading(loadedSceneAssetName))
                {
                    continue;
                }

                UnloadScene(loadedSceneAssetName);
            }

            LoadedSceneAssetNames.Clear();
            LoadingSceneAssetNames.Clear();
            UnloadingSceneAssetNames.Clear();
        }

        /// <summary>
        /// 获取场景是否已加载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否已加载。</returns>
        public bool SceneIsLoaded(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Fatal("Scene asset name is invalid.");
                return false;
            }

            return LoadedSceneAssetNames.Contains(sceneAssetName);
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <returns>已加载场景的资源名称。</returns>
        public string[] GetLoadedSceneAssetNames()
        {
            return LoadedSceneAssetNames.ToArray();
        }

        /// <summary>
        /// 获取已加载场景的资源名称。
        /// </summary>
        /// <param name="results">已加载场景的资源名称。</param>
        public void GetLoadedSceneAssetNames(List<string> results)
        {
            if (results == null)
            {
                Log.Fatal("Results is invalid.");
                return;
            }

            results.Clear();
            results.AddRange(LoadedSceneAssetNames);
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
        /// 获取场景是否正在卸载。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景是否正在卸载。</returns>
        public bool SceneIsUnloading(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Fatal("Scene asset name is invalid.");
                return false;
            }

            return UnloadingSceneAssetNames.Contains(sceneAssetName);
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <returns>正在卸载场景的资源名称。</returns>
        public string[] GetUnloadingSceneAssetNames()
        {
            return UnloadingSceneAssetNames.ToArray();
        }

        /// <summary>
        /// 获取正在卸载场景的资源名称。
        /// </summary>
        /// <param name="results">正在卸载场景的资源名称。</param>
        public void GetUnloadingSceneAssetNames(List<string> results)
        {
            if (results == null)
            {
                Log.Fatal("Results is invalid.");
                return;
            }

            results.Clear();
            results.AddRange(UnloadingSceneAssetNames);
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

            if (SceneIsUnloading(sceneAssetName))
            {
                Log.Fatal(Utility.Text.Format("Scene asset '{0}' is being unloaded.", sceneAssetName));
                return;
            }

            if (SceneIsLoading(sceneAssetName))
            {
                Log.Fatal(Utility.Text.Format("Scene asset '{0}' is being loaded.", sceneAssetName));
                return;
            }

            if (SceneIsLoaded(sceneAssetName))
            {
                Log.Fatal(Utility.Text.Format("Scene asset '{0}' is already loaded.", sceneAssetName));
                return;
            }

            LoadingSceneAssetNames.Add(sceneAssetName);
            Res.LoadScene(sceneAssetName, LoadSceneCallbacks);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        public void UnloadScene(string sceneAssetName)
        {
            UnloadScene(sceneAssetName, null);
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void UnloadScene(string sceneAssetName, object userData)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Fatal("Scene asset name is invalid.");
                return;
            }

            if (SceneIsUnloading(sceneAssetName))
            {
                Log.Fatal(Utility.Text.Format("Scene asset '{0}' is being unloaded.", sceneAssetName));
                return;
            }

            if (SceneIsLoading(sceneAssetName))
            {
                Log.Fatal(Utility.Text.Format("Scene asset '{0}' is being loaded.", sceneAssetName));
                return;
            }

            if (!SceneIsLoaded(sceneAssetName))
            {
                Log.Fatal(Utility.Text.Format("Scene asset '{0}' is not loaded yet.", sceneAssetName));
                return;
            }

            UnloadingSceneAssetNames.Add(sceneAssetName);
            // Res.UnloadScene(sceneAssetName, UnloadSceneCallbacks, userData);
        }

        private void LoadSceneSuccessCallback(string sceneAssetName, float duration)
        {
            LoadingSceneAssetNames.Remove(sceneAssetName);
            LoadedSceneAssetNames.Add(sceneAssetName);
            LoadSceneEvt loadSceneEvt = ReferencePool.Acquire<LoadSceneEvt>();
            loadSceneEvt.SetSuccessData(sceneAssetName);
            HotFixMudule.Event.Fire(loadSceneEvt);
        }

        private void LoadSceneFailureCallback(string sceneAssetName, LoadResStatus status, string errorMessage)
        {
            LoadingSceneAssetNames.Remove(sceneAssetName);
            string appendErrorMessage =
                Utility.Text.Format("Load scene failure, scene asset name '{0}', status '{1}', error message '{2}'.",
                    sceneAssetName, status.ToString(), errorMessage);
            Log.Fatal(appendErrorMessage);
            LoadSceneEvt loadSceneEvt = ReferencePool.Acquire<LoadSceneEvt>();
            loadSceneEvt.SetFailureData(sceneAssetName, appendErrorMessage);
            HotFixMudule.Event.Fire(loadSceneEvt);
        }

        private void LoadSceneDependencyAssetCallback(string sceneAssetName, string dependencyAssetName,
            int loadedCount, int totalCount)
        {
            LoadSceneEvt loadSceneEvt = ReferencePool.Acquire<LoadSceneEvt>();
            loadSceneEvt.SetDependPercentData(sceneAssetName, loadedCount * 100 / totalCount);
            HotFixMudule.Event.Fire(loadSceneEvt);
        }

        private void UnloadSceneSuccessCallback(string sceneAssetName)
        {
            UnloadingSceneAssetNames.Remove(sceneAssetName);
            LoadedSceneAssetNames.Remove(sceneAssetName);
        }

        private void UnloadSceneFailureCallback(string sceneAssetName)
        {
            UnloadingSceneAssetNames.Remove(sceneAssetName);
            Log.Fatal(Utility.Text.Format("Unload scene failure, scene asset name '{0}'.", sceneAssetName));
        }
    }
}
