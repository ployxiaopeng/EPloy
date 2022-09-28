using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;


public  class LoginFormCode : IReference 
{
	public UnityEngine.GameObject btnServer;
	public UnityEngine.GameObject btnStart;
	public UnityEngine.GameObject btnAgree;
	public static LoginFormCode Binding(Transform transform) 
	{
		LoginFormCode binding = ReferencePool.Acquire<LoginFormCode>(); 
		binding.btnServer = transform.Find("btnServer").gameObject; 
		binding.btnStart = transform.Find("btnStart").gameObject; 
		binding.btnAgree = transform.Find("btnAgree").gameObject; 
		return binding;

	}
	public static void UnBinding(LoginFormCode binding) 
	{
		 ReferencePool.Release(binding); 
	}
	public void Clear()
	{
		btnServer = null;
		btnStart = null;
		btnAgree = null;
	}
}
