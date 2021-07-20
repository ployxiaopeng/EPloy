
using EPloy;
using EPloy.Table;

[DataStore]
public class StartFormData : Component
{
    public string txt = "";
    public DRMap DRMap;
    public override void Awake()
    {
        IDataTable<DRMap> mapTable = GameEntry.DataTable.GetDataTable<DRMap>();
        DRMap = mapTable.GetDataRow(10100);
    }
}