
using EPloy;
using EPloy.Table;

[DataStore]
public class StartFormData : Component
{
    public string txt = "";
    public override void Awake()
    {
        IDataTable<DRMap> mapTable = GameEntry.DataTable.GetDataTable<DRMap>();
        Log.Error(mapTable.GetDataRow(10100).RoleBornPos);
    }
}