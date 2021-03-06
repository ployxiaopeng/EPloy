﻿
using ETModel;
using GameFramework.Procedure;
using GameFramework.Scene;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;

namespace ETHotfix
{
    public class ProcedureChangeScene : ProcedureBase
    {
        private bool isComplete = false;
        private string NextScene = null;
        //检擦资源进度情况
        private Dictionary<string, bool> LoadedFlag = new Dictionary<string, bool>();

        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.Scene.SceneManager.LoadSceneSuccess += OnLoadSceneSuccess;
            GameEntry.Scene.SceneManager.LoadSceneFailure += OnLoadSceneFailure;
            GameEntry.Scene.SceneManager.LoadSceneUpdate += OnLoadSceneUpdate;
            GameEntry.Scene.SceneManager.LoadSceneDependencyAsset += OnLoadSceneDependencyAsset;

            //隐藏所有实体
            GameEntry.Entity.HideAllLoadingEntities();
            GameEntry.Entity.HoxfixHideAllLoadedEntities();

            NextScene = procedureOwner.GetData<VarString>("Secne").Value;
            isComplete = SceneManager.GetActiveScene().name == NextScene;
            if (isComplete) return;
            // 卸载所有场景
            string[] loadedSceneAssetNames = GameEntry.Scene.GetLoadedSceneAssetNames();
            for (int i = 0; i < loadedSceneAssetNames.Length; i++)
                GameEntry.Scene.UnloadScene(loadedSceneAssetNames[i]);

            if (NextScene == null)
            {
                Log.Error("Can not load scene NextScene is Null");
                return;
            }
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(NextScene), Constant.AssetPriority.SceneAsset, this);
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            if (!isComplete) return;
            switch (NextScene)
            {
                case "GameLogin":
                    ChangeState<ProcedureLogin>(procedureOwner);
                    break;
                case "GameMap":
                    ChangeState<ProcedureMap>(procedureOwner);
                    break;
            }
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Scene.SceneManager.LoadSceneSuccess -= OnLoadSceneSuccess;
            GameEntry.Scene.SceneManager.LoadSceneFailure -= OnLoadSceneFailure;
            GameEntry.Scene.SceneManager.LoadSceneUpdate -= OnLoadSceneUpdate;
            GameEntry.Scene.SceneManager.LoadSceneDependencyAsset += OnLoadSceneDependencyAsset;
            base.OnLeave(procedureOwner, isShutdown);
        }

        private void OnLoadSceneSuccess(object sender, LoadSceneSuccessEventArgs e)
        {
            if (e.UserData != this) return;
            isComplete = true;
        }
        private void OnLoadSceneFailure(object sender, LoadSceneFailureEventArgs e)
        {
            if (e.UserData != this) return;
            Log.Error("Load scene '{0}' failure, error message '{1}'.", e.SceneAssetName, e.ErrorMessage);
        }
        private void OnLoadSceneUpdate(object sender, LoadSceneUpdateEventArgs e)
        {
            if (e.UserData != this) return;
            // Log.Info("Load scene '{0}' update, progress '{1}'.", ne.SceneAssetName, ne.Progress.ToString("P2"));
        }
        private void OnLoadSceneDependencyAsset(object sender, LoadSceneDependencyAssetEventArgs e)
        {
            if (e.UserData != this) return;
            Log.Info("Load scene '{0}' dependency asset '{1}', count '{2}/{3}'.", e.SceneAssetName, e.DependencyAssetName, e.LoadedCount.ToString(), e.TotalCount.ToString());
        }
    }
}