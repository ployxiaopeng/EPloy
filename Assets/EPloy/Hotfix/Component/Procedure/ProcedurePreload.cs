
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
            // GameEntry.Config.ConfigManager.LoadConfigSuccess += OnLoadConfigSuccess;
            // GameEntry.Config.ConfigManager.LoadConfigFailure += OnLoadConfigFailure;
            GameEntry.Event.Subscribe(FrameEvent.DataTableSuccessEvt, OnLoadDataTableSuccess);
            GameEntry.Event.Subscribe(FrameEvent.DataTableFailureEvt, OnLoadDataTableFailure);
            // LoadedFlag.Clear();
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
            // if (!LoadedFlag.ContainsKey("Assets"))
            // {
            //     LoadedFlag.Clear();
            //     LoadedFlag.Add("Assets", false);
            //     PreloadUIWnd();
            //     return;
            // }
            ChangeState<ProcedureLogin>();
        }
        public override void OnLeave(bool isShutdown)
        {
            // GameEntry.Config.ConfigManager.LoadConfigSuccess -= OnLoadConfigSuccess;
            // GameEntry.Config.ConfigManager.LoadConfigFailure -= OnLoadConfigFailure;
            // GameEntry.DataTable.DataTableManager.LoadDataTableSuccess -= OnLoadDataTableSuccess;
            // GameEntry.DataTable.DataTableManager.LoadDataTableFailure -= OnLoadDataTableFailure;
            base.OnLeave(isShutdown);
        }

        private async void PreloadResources()
        {
            //LoadConfig(ConfigRes.GlobalConfig);

            foreach (var tableName in Config.DataTableNames)
            {
                LoadDataTable(tableName);
            }

            // 预加载图集
            await Game.Timer.WaitAsync(1000);
            LoadedFlag["Data"] = true;
        }

        // #region 加载 Config
        // private void LoadConfig(string configName)
        // {
        //     LoadedFlag.Add(Utility.Text.Format("Config.{0}", configName), false);
        //     // GameEntry.Config.LoadConfig(configName, LoadType.Text, this);
        // }
        // private void OnLoadConfigSuccess(object sender, LoadConfigSuccessEventArgs e)
        // {
        //     LoadConfigInfo loadConfig = (LoadConfigInfo)e.UserData;
        //     if (loadConfig.UserData != this) return;

        //     LoadedFlag[Utility.Text.Format("Config.{0}", loadConfig.ConfigName)] = true;

        // }
        // private void OnLoadConfigFailure(object sender, LoadConfigFailureEventArgs e)
        // {
        //     LoadConfigInfo loadConfig = (LoadConfigInfo)e.UserData;
        //     if (loadConfig.UserData != this) return;
        //     Log.Error("不能加载 '{0}' from '{1}' 错误信息 '{2}'.", loadConfig.ConfigName, e.ConfigAssetName, e.ErrorMessage);
        // }
        // #endregion

        #region 加载 DataTabl
        private void LoadDataTable(string dataTableName)
        {
            LoadedFlag.Add(Utility.Text.Format("DataTable.{0}", dataTableName), false);
            GameEntry.DataTable.LoadDataTable(dataTableName, this);
        }
        private void OnLoadDataTableSuccess(EventArg arg)
        {
            DataTableSuccessEvt e = (DataTableSuccessEvt)arg;
            //  LoadedFlag[Utility.Text.Format("DataTable.{0}", loadDataTableInfo.DataTableName)] = true;
            Log.Error(e.DataAssetName + "牛逼！！！");
        }
        private void OnLoadDataTableFailure(EventArg arg)
        {
            DataTableFailureEvt e = (DataTableFailureEvt)arg;
            Log.Error(Utility.Text.Format("不能加载 '{0}' 错误信息 '{2}'.", e.DataAssetName, e.ErrorMessage));
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