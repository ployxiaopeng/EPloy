using EPloy.Fsm;
using EPloy.Table;
using UnityEngine;

namespace EPloy
{
    public class ProcedureMap : ProcedureBase
    {
        private string loadSecnce = null;

        // private MapComponet MapComponet;
        private static IDataTable<DRMap> _dataMap = null;

        private static IDataTable<DRMap> DateMap
        {
            get
            {
                if (_dataMap == null)
                {
                    _dataMap = HotFixMudule.DataTable.GetDataTable<DRMap>();
                }

                return _dataMap;
            }
        }

        public override void OnEnter()
        {
            base.OnEnter();
            loadSecnce = null;
            Log.Info("Map  success");
            //HotFixMudule.UI.CloseUIForm(UIName.LoadingForm);
            HotFixMudule.Map.MapData = DateMap.GetDataRow(10101);
            HotFixMudule.Map.OnEnterMap(GameObject.Find("Map").transform);
            // GameEntry.UI.OpenUIWnd(UIWnd.MapWnd);
            // GameEntry.UI.CloseUIWnd(UIWnd.LoadingWnd);
        }
    }
}