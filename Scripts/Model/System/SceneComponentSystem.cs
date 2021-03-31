using GameFramework;
using GameFramework.Scene;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETModel
{
    [ObjectSystem]
    public class SceneComponentAwakeSystem : AwakeSystem<SceneComponent>
    {
        public override void Awake(SceneComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class SceneComponentStartSystem : StartSystem<SceneComponent>
    {
        public override void Start(SceneComponent self)
        {
            self.Start();
        }
    }
    public static class SceneComponentSystem
    {

        public static void Awake(this SceneComponent self)
        {
            self.SceneManager = GameFrameworkEntry.GetModule<ISceneManager>();
            if (self.SceneManager == null)
            {
                Log.Fatal("Scene manager is invalid.");
                return;
            }
            self.SceneManager.LoadSceneSuccess += self.OnLoadSceneSuccess;
            self.SceneManager.UnloadSceneSuccess += self.OnUnloadSceneSuccess;
            self.GameFrameworkScene = SceneManager.GetActiveScene();
        }

        public static void Start(this SceneComponent self)
        {
            self.SceneManager.SetResourceManager(Init.Resource.ResourceManager);
        }

        private static void OnLoadSceneSuccess(this SceneComponent self,object sender, LoadSceneSuccessEventArgs e)
        {
            self.MainCamera = Camera.main;
            if (SceneManager.GetActiveScene() == self.GameFrameworkScene)
            {
                Scene scene = SceneManager.GetSceneByName(self.GetSceneName(e.SceneAssetName));
                if (!scene.IsValid())
                {
                    Log.Error("Loaded scene '{0}' is invalid.", e.SceneAssetName);
                    return;
                }
                SceneManager.SetActiveScene(scene);
            }
        }

        private static void OnUnloadSceneSuccess(this SceneComponent self,object sender, UnloadSceneSuccessEventArgs e)
        {
            self.MainCamera = Camera.main;
        }
    }
}