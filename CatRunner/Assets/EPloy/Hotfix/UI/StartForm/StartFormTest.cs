using EPloy.UI;

public class StartFormTest : UIFormLogic
{
    private StartFormTestCode bindingCode;
    protected override void Create()
    {
        bindingCode = StartFormTestCode.Binding(transform);
    }
    public override void ShowView()
    {
        base.ShowView();
    }
    public override void CloseView()
    {
        base.CloseView();
    }
    public override void Clear()
    {
        base.Clear();
        StartFormTestCode.UnBinding(bindingCode);
    }
}
