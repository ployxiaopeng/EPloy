
using GameFramework.DataTable;
using GameFramework.Event;
using GameFramework.Procedure;
using ProcedureOwner = GameFramework.Fsm.IFsm<GameFramework.Procedure.IProcedureManager>;
using UnityEngine;

namespace ETHotfix
{
    public class ProcedureMap : ProcedureBase
    {
        private string loadSecnce = null;
        private MapComponet MapComponet;
        private static IDataTable<DRMap> _dataMap = null;
        private static IDataTable<DRMap> DateMap
        {
            get
            {
                if (_dataMap == null)
                {
                    _dataMap = GameEntry.DataTable.GetDataTable<DRMap>();
                }
                return _dataMap;
            }
        }
        protected override void OnEnter(ProcedureOwner procedureOwner)
        {
            base.OnEnter(procedureOwner);
            loadSecnce = null;
            MapComponet = GameEntry.Extension.GetComponent<MapComponet>();
            MapComponet.MapData = DateMap.GetDataRow(10101);
            MapComponet.OnEnterMap(GameObject.Find("Map").transform);
            GameEntry.UI.OpenUIWnd(UIWnd.MapWnd);
            GameEntry.UI.CloseUIWnd(UIWnd.LoadingWnd);
        }
    }
}