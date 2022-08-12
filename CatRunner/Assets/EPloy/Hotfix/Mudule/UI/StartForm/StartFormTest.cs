using System.Collections;
using System.Collections.Generic;
using System;
using EPloy.Hotfix;
using UnityEngine;
using UnityEngine.UI;

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
