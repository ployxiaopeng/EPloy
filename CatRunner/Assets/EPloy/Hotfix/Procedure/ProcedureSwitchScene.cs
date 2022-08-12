using EPloy.Event;
using EPloy.Fsm;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class ProcedureSwitchScene : FsmState
{
    private bool isComplete = false;
    private string NextScene = null;
    //检擦资源进度情况
    private Dictionary<string, bool> LoadedFlag = new Dictionary<string, bool>();

    public override void OnEnter()
    {
        base.OnEnter();
        GameModule.Event.Subscribe(FrameEvent.LoadSceneEvt, OnLoadSceneEvt);

        //隐藏所有实体
        // GameEntry.Obj.HideAllLoadingEntities();
        // GameEntry.Obj.HideAllLoadedEntities();

        NextScene = ProcedureOwner.GetData<VarString>("Secne").Value;
        isComplete = SceneManager.GetActiveScene().name == NextScene;
        if (isComplete) return;
        // 卸载所有场景
        string[] loadedSceneAssetNames = GameModule.Scene.GetLoadedSceneAssetNames();
        for (int i = 0; i < loadedSceneAssetNames.Length; i++)
            GameModule.Scene.UnloadScene(loadedSceneAssetNames[i]);

        if (NextScene == null)
        {
            Log.Error("Can not load scene NextScene is Null");
            return;
        }
        GameModule.Scene.LoadScene(UtilAsset.GetSceneAsset(NextScene));
    }
    public override void OnUpdate()
    {
        if (!isComplete) return;
        switch (NextScene)
        {
            case "start":
                ChangeState<ProcedureLogin>();
                break;
            case "game":
                ChangeState<ProcedureMap>();
                break;
        }
    }
    public override void OnLeave(bool isShutdown)
    {
        GameModule.Event.Unsubscribe(FrameEvent.LoadSceneEvt, OnLoadSceneEvt);
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