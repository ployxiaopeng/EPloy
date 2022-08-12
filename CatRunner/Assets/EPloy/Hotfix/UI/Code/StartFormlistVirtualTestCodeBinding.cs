using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;


public  class StartFormlistVirtualTestCode : IReference 
{
	public UnityEngine.UI.Text txtTest;
	public static StartFormlistVirtualTestCode Binding(Transform transform) 
	{
		StartFormlistVirtualTestCode binding = ReferencePool.Acquire<StartFormlistVirtualTestCode>(); 
		binding.txtTest = transform.Find("txtTest").GetComponent<UnityEngine.UI.Text>(); 
		return binding;

	}
	public static void UnBinding(StartFormlistVirtualTestCode binding) 
	{
		 ReferencePool.Release(binding); 
	}
	public void Clear()
	{
		txtTest = null;
	}
}
