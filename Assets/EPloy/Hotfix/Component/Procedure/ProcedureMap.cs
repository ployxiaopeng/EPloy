using EPloy.Fsm;
using UnityEngine;

namespace EPloy
{
    public class ProcedureMap : ProcedureBase
    {
        private string loadSecnce = null;
        // private MapComponet MapComponet;
        // private static IDataTable<DRMap> _dataMap = null;
        // private static IDataTable<DRMap> DateMap
        // {
        //     get
        //     {
        //         if (_dataMap == null)
        //         {
        //             _dataMap = GameEntry.DataTable.GetDataTable<DRMap>();
        //         }
        //         return _dataMap;
        //     }
        // }
        public override void OnEnter()
        {
            base.OnEnter();
            loadSecnce = null;
            // MapComponet = GameEntry.Extension.GetComponent<MapComponet>();
            // MapComponet.MapData = DateMap.GetDataRow(10101);
            // MapComponet.OnEnterMap(GameObject.Find("Map").transform);
            // GameEntry.UI.OpenUIWnd(UIWnd.MapWnd);
            // GameEntry.UI.CloseUIWnd(UIWnd.LoadingWnd);
        }
    }
}