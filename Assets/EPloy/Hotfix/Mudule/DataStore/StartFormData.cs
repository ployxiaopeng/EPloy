using EPloy;
using EPloy.Table;

namespace EPloy
{
    public class StartFormData : DataStoreBase
    {
        public string txt = "";
        public DRMap DRMap;

        public override void Create()
        {
            IDataTable<DRMap> mapTable = HotFixMudule.DataTable.GetDataTable<DRMap>();
            DRMap = mapTable.GetDataRow(10100);
        }
    }
}