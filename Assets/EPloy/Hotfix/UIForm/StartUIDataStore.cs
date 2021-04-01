
using EPloy;


[DataStore]
public class StartUIDataStore : Component
{
    public string txt = "";
    protected override void InitComponent()
    {
        txt = "你猜我猜不猜你猜我";
    }
}