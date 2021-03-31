
using System.Collections.Generic;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using GameFramework.Procedure;
using ETModel;
using GameFramework.Event;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Config;
using GameFramework.Resource;

namespace ETHotfix
{
    public class ProcedurePreload : ProcedureBase
    {
        private Dictionary<string, bool> LoadedFlag = new Dictionary<string, bool>();
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            GameEntry.Extension.CreateHotfixComponent();
            GameEntry.Config.ConfigManager.LoadConfigSuccess += OnLoadConfigSuccess;
            GameEntry.Config.ConfigManager.LoadConfigFailure += OnLoadConfigFailure;
            GameEntry.DataTable.DataTableManager.LoadDataTableSuccess += OnLoadDataTableSuccess;
            GameEntry.DataTable.DataTableManager.LoadDataTableFailure += OnLoadDataTableFailure;
            LoadedFlag.Clear();
            LoadedFlag.Add("Data", false);
            PreloadResources();
        }
        protected override void OnUpdate(ProcedureOwner procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
            IEnumerator<bool> iter = LoadedFlag.Values.GetEnumerator();
            while (iter.MoveNext())
            {
                if (!iter.Current) return;
            }
            if (!LoadedFlag.ContainsKey("Assets"))
            {
                LoadedFlag.Clear();
                LoadedFlag.Add("Assets", false);
                PreloadUIWnd();
                return;
            }
            GameEntry.Extension.AwakeHotfixComponent();
            ChangeState<ProcedureLogin>(procedureOwner);
        }
        protected override void OnLeave(ProcedureOwner procedureOwner, bool isShutdown)
        {
            GameEntry.Config.ConfigManager.LoadConfigSuccess -= OnLoadConfigSuccess;
            GameEntry.Config.ConfigManager.LoadConfigFailure -= OnLoadConfigFailure;
            GameEntry.DataTable.DataTableManager.LoadDataTableSuccess -= OnLoadDataTableSuccess;
            GameEntry.DataTable.DataTableManager.LoadDataTableFailure -= OnLoadDataTableFailure;
            base.OnLeave(procedureOwner, isShutdown);
        }

        private async void PreloadResources()
        {
            //LoadConfig(ConfigRes.GlobalConfig);

            foreach (var tableName in ConfigRes.DataTableNames)
                LoadDataTable(tableName);
            //预加载图集
            await GameEntry.Timer.WaitAsync(300);
            LoadedFlag["Data"] = true;
        }

        #region 加载 Config
        private void LoadConfig(string configName)
        {
            LoadedFlag.Add(Utility.Text.Format("Config.{0}", configName), false);
            GameEntry.Config.LoadConfig(configName, LoadType.Text, this);
        }
        private void OnLoadConfigSuccess(object sender, LoadConfigSuccessEventArgs e)
        {
            LoadConfigInfo loadConfig = (LoadConfigInfo)e.UserData;
            if (loadConfig.UserData != this) return;

            LoadedFlag[Utility.Text.Format("Config.{0}", loadConfig.ConfigName)] = true;

        }
        private void OnLoadConfigFailure(object sender, LoadConfigFailureEventArgs e)
        {
            LoadConfigInfo loadConfig = (LoadConfigInfo)e.UserData;
            if (loadConfig.UserData != this) return;
            Log.Error("不能加载 '{0}' from '{1}' 错误信息 '{2}'.", loadConfig.ConfigName, e.ConfigAssetName, e.ErrorMessage);
        }
        #endregion

        #region 加载 DataTabl
        private void LoadDataTable(string dataTableName)
        {
            LoadedFlag.Add(Utility.Text.Format("DataTable.{0}", dataTableName), false);
            GameEntry.DataTable.LoadDataTable(dataTableName, LoadType.Bytes, this);
        }
        private void OnLoadDataTableSuccess(object sender, LoadDataTableSuccessEventArgs e)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)e.UserData;
            if (loadDataTableInfo.UserData != this) return;
            LoadedFlag[Utility.Text.Format("DataTable.{0}", loadDataTableInfo.DataTableName)] = true;
        }
        private void OnLoadDataTableFailure(object sender, LoadDataTableFailureEventArgs e)
        {
            LoadDataTableInfo loadDataTableInfo = (LoadDataTableInfo)e.UserData;
            if (loadDataTableInfo.UserData != this) return;
            Log.Error("不能加载 '{0}' 错误信息 '{2}'.", e.DataTableAssetName, e.ErrorMessage);
        }
        #endregion

        #region 预加载UI
        private LoadAssetCallbacks loadUIWndCallbacks = null;
        private void PreloadUIWnd()
        {
            if (loadUIWndCallbacks == null)
                loadUIWndCallbacks = new LoadAssetCallbacks(LoadUISuccessCallback, LoadUIFailureCallback);

            foreach (var wnd in ConfigRes.loadUIWnd)
                LoadUIWnd(wnd);

            LoadedFlag["Assets"] = true;
        }
        private void LoadUIWnd(UIWnd UIWnd)
        {
            LoadedFlag.Add(Utility.Text.Format("UI.{0}", UIWnd.ToString()), false);
            GameEntry.UI.PreloadUIWnd(UIWnd, loadUIWndCallbacks);
        }
        private void LoadUISuccessCallback(string assetName, object asset, float duration, object userData)
        {
            //Log.Info(Utility.Text.Format("预加载加{0} 成功", assetName));
            string assetKey = Utility.Text.Format("UI.{0}", (string)userData);
            if (LoadedFlag.ContainsKey(assetKey))
                LoadedFlag[assetKey] = true;
        }
        private void LoadUIFailureCallback(string assetName, LoadResourceStatus status, string errorMessage, object userData)
        {
            Log.Error(Utility.Text.Format("预加载加{0} ResourceStatus{1},Err:{2}", assetName, status, errorMessage));
        }
        #endregion
    }
}