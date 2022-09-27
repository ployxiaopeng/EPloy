using System.Collections.Generic;
using EPloy.Event;
using EPloy.Net;
using EPloy.UI;

[UIAttribute(UIName.StartForm)]
public class StartForm : UIForm
{
    private StartFormCode bindingCode;
    private StartFormData dataStore;
    private StartFormTest test;
    private StartFormlistTest listTest;
    private StartFormlistVirtualTest listVirtualTest;

    private UIEffectData uIEffectData;
    public override void Create()
    {
        bindingCode = StartFormCode.Binding(transform);
        test = CreateChildLogic<StartFormTest>(bindingCode.nodeTest);
        listTest = ListBase.CreateUIList<StartFormlistTest>(bindingCode.listTest.transform);
        listVirtualTest = VirtualListBase.CreateUIList<StartFormlistVirtualTest>(bindingCode.listVirtualTest.transform);

        GameModule.Event.Subscribe(EventId.TestEvt, EvtTest);
        GameModule.Net.AddListener(HotfixOpcode.G2C_Register, G2C_Register);
        UIEventListener.Get(bindingCode.btnEvtTest).onClick = (go) =>
          {
              GameModule.Event.Fire(ReferencePool.Acquire<TestEvt>());
          };

        UIEventListener.Get(bindingCode.btnTipsForm).onClick = (go) =>
        {
            GameModule.UI.OpenUIForm(UIName.TipsForm, UIGroupName.Default);
        };
        UIEventListener.Get(bindingCode.btnObjTest).onClick = (go) =>
        {
            if (uIEffectData== null)
            {
                GameModule.Obj.ShowObj(UIEffectData.Create(1001, transform), (effec) =>
                {
                    uIEffectData = (UIEffectData)effec;
                    Log.Info("show UIEffect " + effec.Obj.name);
                });
            }
            else
            {
                GameModule.Obj.HideObj(uIEffectData.SerialId);
                uIEffectData = null;
            }
   
        };

        UIEventListener.Get(bindingCode.btnSvrTest).onClick = (go) =>
        {
            C2G_Register register = GameModule.Net.CreateMessage<C2G_Register>();
            register.Account = "123456";
            register.Password = "123456";
            GameModule.Net.SendMessage(register);
            Log.Info("开始注册");
        };
        UIEventListener.Get(bindingCode.btnECSTest).onClick = (go) =>
        {
            GameModule.UI.OpenUIForm(UIName.LoadingForm, UIGroupName.Level1);
            GameModule.Event.Fire(SwitchSceneEvt.Create("game"));
        };

        GameModule.UI.CloseUIForm(UIName.LoadingForm);
    }
    public override void Open(object userData)
    {
        dataStore = GameModule.DataStore.GetDataStore<StartFormData>();
        List<string> data = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            data.Add(string.Format("item_{0}", i));
        }
        listTest.SetData(data);
        List<string> data1 = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            data1.Add(string.Format("itemVirtual_{0}", i));
        }
        listVirtualTest.SetData(data1);
    }
    public override void Clear()
    {
        base.Clear();
        StartFormCode.UnBinding(bindingCode);
    }

    public void G2C_Register(object rsp)
    {
        G2C_Register register = (G2C_Register)rsp;
        Log.Info(register.Message);
    }

    public void EvtTest(EventArg eventArg)
    {
        dataStore.txtId++;
        bindingCode.txtTips.SetText(dataStore.txtId);
        test.ShowView();
    }
}
