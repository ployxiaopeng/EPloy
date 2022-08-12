using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;


public  class StartFormTestCode : IReference 
{
	public UnityEngine.UI.Text txtTitle;
	public static StartFormTestCode Binding(Transform transform) 
	{
		StartFormTestCode binding = ReferencePool.Acquire<StartFormTestCode>(); 
		binding.txtTitle = transform.Find("txtTitle").GetComponent<UnityEngine.UI.Text>(); 
		return binding;

	}
	public static void UnBinding(StartFormTestCode binding) 
	{
		 ReferencePool.Release(binding); 
	}
	public void Clear()
	{
		txtTitle = null;
	}
}
