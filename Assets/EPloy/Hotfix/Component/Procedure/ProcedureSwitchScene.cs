using EPloy.Fsm;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace EPloy
{
    public class ProcedureSwitchScene : ProcedureBase
    {
        private bool isComplete = false;
        private string NextScene = null;
        //检擦资源进度情况
        private Dictionary<string, bool> LoadedFlag = new Dictionary<string, bool>();

        public override void OnEnter()
        {
            base.OnEnter();
            GameEntry.Event.Subscribe(FrameEvent.LoadSceneEvt, OnLoadSceneEvt);

            //隐藏所有实体
            // GameEntry.Obj.HideAllLoadingEntities();
            // GameEntry.Obj.HideAllLoadedEntities();

            NextScene = ProcedureOwner.GetData<VarString>("Secne").Value;
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
            GameEntry.Scene.LoadScene(AssetUtility.GetSceneAsset(NextScene));
        }
        public override void OnUpdate()
        {
            if (!isComplete) return;
            switch (NextScene)
            {
                case "Start":
                    ChangeState<ProcedureLogin>();
                    break;
                case "Game":
                    ChangeState<ProcedureMap>();
                    break;
            }
        }
        public override void OnLeave(bool isShutdown)
        {
            GameEntry.Event.Unsubscribe(FrameEvent.LoadSceneEvt, OnLoadSceneEvt);
            base.OnLeave(isShutdown);
        }

        public void OnLoadSceneEvt(EventArg arg)
        {
            LoadSceneEvt switchSceneEvt = (LoadSceneEvt)arg;
            switch (switchSceneEvt.State)
            {
                case LoadSceneEvt.LoadSceneState.Success:
                    isComplete = true;
                    break;
                case LoadSceneEvt.LoadSceneState.Failure:
                    break;
                case LoadSceneEvt.LoadSceneState.Depend:
                    break;
                case LoadSceneEvt.LoadSceneState.Null:
                    Log.Warning("SwitchSceneState is Null");
                    break;
            }
        }
    }
}