using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;


public  class LoadingFormCode : IReference 
{
	public UnityEngine.UI.Slider sldPrg;
	public UnityEngine.UI.Text txtTips;
	public static LoadingFormCode Binding(Transform transform) 
	{
		LoadingFormCode binding = ReferencePool.Acquire<LoadingFormCode>(); 
		binding.sldPrg = transform.Find("sldPrg").GetComponent<UnityEngine.UI.Slider>(); 
		binding.txtTips = transform.Find("txtTips").GetComponent<UnityEngine.UI.Text>(); 
		return binding;

	}
	public static void UnBinding(LoadingFormCode binding) 
	{
		 ReferencePool.Release(binding); 
	}
	public void Clear()
	{
		sldPrg = null;
		txtTips = null;
	}
}
