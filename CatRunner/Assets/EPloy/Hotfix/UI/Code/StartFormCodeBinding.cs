using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;


public  class StartFormCode : IReference 
{
	public UnityEngine.UI.Text txtTips;
	public UnityEngine.GameObject btnTipsForm;
	public UnityEngine.GameObject btnEvtTest;
	public UnityEngine.GameObject btnObjTest;
	public UnityEngine.UI.ScrollRect listTest;
	public UnityEngine.UI.ScrollRect listVirtualTest;
	public UnityEngine.GameObject btnSvrTest;
	public UnityEngine.GameObject btnECSTest;
	public UnityEngine.Transform nodeTest;
	public static StartFormCode Binding(Transform transform) 
	{
		StartFormCode binding = ReferencePool.Acquire<StartFormCode>(); 
		binding.txtTips = transform.Find("txtTips").GetComponent<UnityEngine.UI.Text>(); 
		binding.btnTipsForm = transform.Find("btnTipsForm").gameObject; 
		binding.btnEvtTest = transform.Find("btnEvtTest").gameObject; 
		binding.btnObjTest = transform.Find("btnObjTest").gameObject; 
		binding.listTest = transform.Find("listTest").GetComponent<UnityEngine.UI.ScrollRect>(); 
		binding.listVirtualTest = transform.Find("listVirtualTest").GetComponent<UnityEngine.UI.ScrollRect>(); 
		binding.btnSvrTest = transform.Find("btnSvrTest").gameObject; 
		binding.btnECSTest = transform.Find("btnECSTest").gameObject; 
		binding.nodeTest = transform.Find("nodeTest"); 
		return binding;

	}
	public static void UnBinding(StartFormCode binding) 
	{
		 ReferencePool.Release(binding); 
	}
	public void Clear()
	{
		txtTips = null;
		btnTipsForm = null;
		btnEvtTest = null;
		btnObjTest = null;
		listTest = null;
		listVirtualTest = null;
		btnSvrTest = null;
		btnECSTest = null;
		nodeTest = null;
	}
}
