using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EPloy;

[UIAttribute(UIName.StartUI)]
public class StartUIForm : UIForm
{
    private Text Text;
    private Button btnClose;
    private StartUIDataStore dataStore;
    public override void Create()
    {
        Text = transform.Find("Text").GetComponent<Text>();
        btnClose = transform.Find("btnClose").GetComponent<Button>();
        btnClose.onClick.AddListener(() =>
              {
                  Text.text = dataStore.txt;
                  GameEntry.Event.Fire(ReferencePool.Acquire<TestEvt>());
              });
        GameEntry.Event.Subscribe(EventId.TestEvt, EvtTest);
    }

    public override void Open(object userData)
    {
        dataStore = GameEntry.DataStore.GetDataStore<StartUIDataStore>();
    }

    public void EvtTest(EventArg eventArg)
    {
        Log.Info("事件测试");
    }


    public override void Close(object userData)
    {

    }
}
