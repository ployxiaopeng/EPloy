using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;


public  class StartFormlistTestCode : IReference 
{
	public UnityEngine.UI.Text txtTest;
	public static StartFormlistTestCode Binding(Transform transform) 
	{
		StartFormlistTestCode binding = ReferencePool.Acquire<StartFormlistTestCode>(); 
		binding.txtTest = transform.Find("txtTest").GetComponent<UnityEngine.UI.Text>(); 
		return binding;

	}
	public static void UnBinding(StartFormlistTestCode binding) 
	{
		 ReferencePool.Release(binding); 
	}
	public void Clear()
	{
		txtTest = null;
	}
}
