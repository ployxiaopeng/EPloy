using System.Collections.Generic;
using EPloy.Event;
using EPloy.Fsm;
using EPloy.Res;
using EPloy.Table;

public class ProcedurePreload : FsmState
{
    private List<string> LoadedFlag = new List<string>();
    public override void OnEnter()
    {
        GameModule.UI.OpenUIForm(UIName.LoadingForm, UIGroupName.Level1);
        GameModule.Atlas.RegisterAtlasLoadEvt(OnLoadAtlasSuccess);
        GameModule.Event.Subscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
        GameModule.Event.Subscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
        LoadedFlag.Clear();
        LoadedFlag.Add("Data");
        PreloadResources();
    }
    public override void OnUpdate()
    {
        if (LoadedFlag.Count > 0) return;
        ChangeState<ProcedureLogin>();
    }
    public override void OnLeave(bool isShutdown)
    {
        GameModule.Atlas.RemoveAtlasLoadEvt();
        GameModule.Event.Unsubscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
        GameModule.Event.Unsubscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
        base.OnLeave(isShutdown);
    }

    private void PreloadResources()
    {
        foreach (var tableName in ResConfig.PrelaodDataTableNames)
        {
            PreLoadDataTable(tableName);
        }

        foreach (var atlasName in ResConfig.PrelaodAtlasNames)
        {
            PreloadAtlas(atlasName);
        }

        // PreloadUIWnd();
        GameModule.Timer.InTimer(0.3f, (tid) =>
         {
             LoadedFlag.Remove("Data");
         });
    }

    #region 加载 DataTable
    private void PreLoadDataTable(string dataTableName)
    {
        LoadedFlag.Add(UtilText.Format("DataTable.{0}", dataTableName));
        GameModule.Table.LoadDataTable(dataTableName);
    }
    private void OnLoadDataTableSuccess(EventArg arg)
    {
        DataTableSuccessEvt e = (DataTableSuccessEvt)arg;
        LoadedFlag.Remove(UtilText.Format("DataTable.{0}", e.DataAssetName));
        Log.Info(UtilText.Format("预加载加表格：{0} 成功", e.DataAssetName));
    }
    private void OnLoadDataTableFailure(EventArg arg)
    {
        DataTableFailureEvt e = (DataTableFailureEvt)arg;
        Log.Info(UtilText.Format("不能加载 '{0}' 错误信息 '{1}'.", e.DataAssetName, e.ErrorMessage));
    }
    #endregion

    #region 预加载图集
    private void PreloadAtlas(string atlasName)
    {
        LoadedFlag.Add(atlasName);
        GameModule.Atlas.LoadAtlas(AtlasLoadData.Create(atlasName, ResConfig.AtlasNames[atlasName]));
    }
    private void OnLoadAtlasSuccess(string atlasName)
    {
        LoadedFlag.Remove(atlasName);
        Log.Info(UtilText.Format("预加载加图集：{0} 成功", atlasName));
    }
    #endregion

    #region 预加载UI
    private LoadAssetCallbacks loadUIWndCallbacks = null;
    private void PreloadUIWnd()
    {
        // if (loadUIWndCallbacks == null)
        //     loadUIWndCallbacks = new LoadAssetCallbacks(LoadUISuccessCallback, LoadUIFailureCallback);

        // foreach (var wnd in ConfigRes.loadUIWnd)
        //     LoadUIWnd(wnd);
    }
    private void LoadUIWnd(UIName UIName)
    {
        LoadedFlag.Remove(UtilText.Format("UI.{0}", UIName));
        // GameEntry.UI.PreloadUIWnd(UIName, loadUIWndCallbacks);
    }
    private void LoadUISuccessCallback(string assetName, object asset, float duration, object userData)
    {
        Log.Info(UtilText.Format("预加载加{0} 成功", assetName));
        string assetKey = UtilText.Format("UI.{0}", (string)userData);
        LoadedFlag.Remove(assetKey);
    }
    private void LoadUIFailureCallback(string assetName, LoadResStatus status, string errorMessage, object userData)
    {
        Log.Error(UtilText.Format("预加载加{0} ResourceStatus{1},Err:{2}", assetName, status, errorMessage));
    }
    #endregion
}