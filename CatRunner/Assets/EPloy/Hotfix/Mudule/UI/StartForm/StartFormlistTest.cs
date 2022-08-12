using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;

public class StartFormlistTest : ListBase
{
	private  List<string> data=>(List<string>)base.datas;//这是测试生产的模板 请求改为自己用的数据后删除
	protected override void Create()
	{
        Log.Info("ListTest Create");
        base.onClick = OnItemclick;;
	}
	protected override void Itemrenderer(int index, Transform transform)
	{
		StartFormlistTestCode bindingCode = StartFormlistTestCode.Binding(transform);
        //逻辑写在这个位置 bindingCode 渲染完毕要复用 
        bindingCode.txtTest.text = data[index];
        StartFormlistTestCode.UnBinding(bindingCode);
	}
	private void OnItemclick(int index, GameObject go)
	{
        Log.Info(UtilText.Format("点击了 {0}", index));
    }
}
