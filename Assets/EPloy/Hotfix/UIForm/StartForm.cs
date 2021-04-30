﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using EPloy;

[UIAttribute(UIName.StartForm)]
public class StartForm : UIForm
{
    private Text txtTips;
    private Button btnEvtTest;
    private Button btnTipsForm;
    private StartFormData dataStore;
    public override void Create()
    {
        txtTips = transform.Find("txtTips").GetComponent<Text>();
        btnEvtTest = transform.Find("btnEvtTest").GetComponent<Button>();
        btnTipsForm = transform.Find("btnTipsForm").GetComponent<Button>();

        GameEntry.Event.Subscribe(EventId.TestEvt, EvtTest);
        btnEvtTest.onClick.AddListener(() =>
              {
                  GameEntry.Event.Fire(ReferencePool.Acquire<TestEvt>());
              });
        btnTipsForm.onClick.AddListener(() =>
       {
           GameEntry.UI.OpenUIForm(UIName.TipsForm, GroupName.Default);
       });
    }

    public override void Open(object userData)
    {
        dataStore = GameEntry.DataStore.GetDataStore<StartFormData>();
    }

    public void EvtTest(EventArg eventArg)
    {
        txtTips.text = "事件测试成功";
    }


    public override void Close(object userData)
    {

    }
}