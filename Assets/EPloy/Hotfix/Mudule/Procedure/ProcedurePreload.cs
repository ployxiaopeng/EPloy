
using System.Collections.Generic;
using EPloy.Fsm;
using EPloy.Res;

namespace EPloy
{
    public class ProcedurePreload : ProcedureBase
    {
        private Dictionary<string, bool> LoadedFlag = new Dictionary<string, bool>();
        public override void OnEnter()
        {
            HotFixMudule.UI.OpenUIForm(UIName.LoadingForm, UIGroupName.Level1);
            HotFixMudule.Event.Subscribe(FrameEvent.AtlasSuccessEvt, OnLoadAtlasSuccess);
            HotFixMudule.Event.Subscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
            HotFixMudule.Event.Subscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
            LoadedFlag.Clear();
            LoadedFlag.Add("Data", false);
            PreloadResources();
        }
        public override void OnUpdate()
        {
            IEnumerator<bool> iter = LoadedFlag.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                if (!iter.Current) return;
            }
            ChangeState<ProcedureLogin>();
        }
        public override void OnLeave(bool isShutdown)
        {
            HotFixMudule.Event.Unsubscribe(FrameEvent.AtlasSuccessEvt, OnLoadAtlasSuccess);
            HotFixMudule.Event.Unsubscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
            HotFixMudule.Event.Unsubscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
            base.OnLeave(isShutdown);
        }

        private async void PreloadResources()
        {
            foreach (var tableName in Config.PrelaodDataTableNames)
            {
                PreLoadDataTable(tableName);
            }

            foreach (var atlasName in Config.PrelaodAtlasNames)
            {
                PreloadAtlas(atlasName);
            }

            // PreloadUIWnd();
            await GameModule.Timer.WaitAsync(1000);
            LoadedFlag["Data"] = true;
        }

        #region 加载 DataTable
        private void PreLoadDataTable(string dataTableName)
        {
            LoadedFlag.Add(Utility.Text.Format("DataTable.{0}", dataTableName), false);
            HotFixMudule.DataTable.LoadDataTable(dataTableName);
        }
        private void OnLoadDataTableSuccess(EventArg arg)
        {
            DataTableSuccessEvt e = (DataTableSuccessEvt)arg;
            LoadedFlag[Utility.Text.Format("DataTable.{0}", e.DataAssetName)] = true;
           // Log.Info(Utility.Text.Format("预加载加表格：{0} 成功", e.DataAssetName));
        }
        private void OnLoadDataTableFailure(EventArg arg)
        {
            DataTableFailureEvt e = (DataTableFailureEvt)arg;
            Log.Error(Utility.Text.Format("不能加载 '{0}' 错误信息 '{2}'.", e.DataAssetName, e.ErrorMessage));
        }
        #endregion

        #region 预加载图集
        private void PreloadAtlas(string atlasName)
        {
            LoadedFlag[atlasName] = false;
            HotFixMudule.Atlas.LoadAtlas(atlasName);
        }
        private void OnLoadAtlasSuccess(EventArg arg)
        {
            AtlasSuccessEvt atlasSuccessEvt = (AtlasSuccessEvt)arg;
            LoadedFlag[atlasSuccessEvt.AtlasName] = true;
          //  Log.Info(Utility.Text.Format("预加载加图集：{0} 成功", atlasSuccessEvt.AtlasName));
        }
        #endregion

        #region 预加载UI
        private LoadAssetCallbacks loadUIWndCallbacks = null;
        private void PreloadUIWnd()
        {
            LoadedFlag.Add("Assets", false);
            // if (loadUIWndCallbacks == null)
            //     loadUIWndCallbacks = new LoadAssetCallbacks(LoadUISuccessCallback, LoadUIFailureCallback);

            // foreach (var wnd in ConfigRes.loadUIWnd)
            //     LoadUIWnd(wnd);

            LoadedFlag["Assets"] = true;
        }
        private void LoadUIWnd(UIName UIName)
        {
            LoadedFlag.Add(Utility.Text.Format("UI.{0}", UIName.ToString()), false);
            // GameEntry.UI.PreloadUIWnd(UIName, loadUIWndCallbacks);
        }
        private void LoadUISuccessCallback(string assetName, object asset, float duration, object userData)
        {
            Log.Info(Utility.Text.Format("预加载加{0} 成功", assetName));
            string assetKey = Utility.Text.Format("UI.{0}", (string)userData);
            if (LoadedFlag.ContainsKey(assetKey))
                LoadedFlag[assetKey] = true;
        }
        private void LoadUIFailureCallback(string assetName, LoadResStatus status, string errorMessage, object userData)
        {
            Log.Error(Utility.Text.Format("预加载加{0} ResourceStatus{1},Err:{2}", assetName, status, errorMessage));
        }
        #endregion
    }
}