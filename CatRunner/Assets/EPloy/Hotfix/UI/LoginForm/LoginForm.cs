using System.Collections;
using System.Collections.Generic;
using System;
using EPloy.UI;
using UnityEngine;
using UnityEngine.UI;
using EPloy.Event;

[UIAttribute(UIName.LoginForm)]
public class LoginForm : UIForm
{
	private LoginFormCode bindingCode;
	public override void Create()
	{
		bindingCode = LoginFormCode.Binding(transform);
		UIEventListener.Get(bindingCode.btnStart).onClick = (go) =>
		{
			GameModule.UI.OpenUIForm(UIName.LoadingForm, UIGroupName.Level1);
			GameModule.Event.Fire(SwitchSceneEvt.Create("game"));
		};
	}
	public override void Open(object userData)
	{

	}
	public override void Clear()
	{
		base.Clear();
		LoginFormCode.UnBinding(bindingCode);
	}
}
