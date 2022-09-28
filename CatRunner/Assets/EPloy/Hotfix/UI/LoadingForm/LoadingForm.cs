using System.Collections;
using System.Collections.Generic;
using System;
using EPloy.UI;
using UnityEngine;
using UnityEngine.UI;

[UIAttribute(UIName.LoadingForm)]
public class LoadingForm : UIForm
{
	private LoadingFormCode bindingCode;
	public override void Create()
	{
		bindingCode = LoadingFormCode.Binding(transform);
	}
	public override void Open(object userData)
	{

	}
	public override void Clear()
	{
		base.Clear();
		LoadingFormCode.UnBinding(bindingCode);
	}
}
