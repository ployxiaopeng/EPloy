using UnityEngine;
using EPloy.Hotfix;
using UnityEngine.UI;


public  class GameFormCode : IReference 
{
	public UnityEngine.GameObject btnMove;
	public UnityEngine.UI.Image imgMove;
	public UnityEngine.GameObject btnAtt;
	public UnityEngine.UI.Image imgSkill0;
	public UnityEngine.UI.Image imgSkill1;
	public UnityEngine.UI.Image imgMask1;
	public UnityEngine.UI.Image imgSkill2;
	public UnityEngine.UI.Image imgMask2;
	public UnityEngine.UI.Image imgSkill3;
	public UnityEngine.UI.Image imgMask3;
	public static GameFormCode Binding(Transform transform) 
	{
		GameFormCode binding = ReferencePool.Acquire<GameFormCode>(); 
		binding.btnMove = transform.Find("btnMove").gameObject; 
		binding.imgMove = transform.Find("btnMove/imgMove").GetComponent<UnityEngine.UI.Image>(); 
		binding.btnAtt = transform.Find("skill/btnAtt").gameObject; 
		binding.imgSkill0 = transform.Find("skill/Skill0/imgSkill0").GetComponent<UnityEngine.UI.Image>(); 
		binding.imgSkill1 = transform.Find("skill/Skill1/imgSkill1").GetComponent<UnityEngine.UI.Image>(); 
		binding.imgMask1 = transform.Find("skill/Skill1/imgMask1").GetComponent<UnityEngine.UI.Image>(); 
		binding.imgSkill2 = transform.Find("skill/Skill2/imgSkill2").GetComponent<UnityEngine.UI.Image>(); 
		binding.imgMask2 = transform.Find("skill/Skill2/imgMask2").GetComponent<UnityEngine.UI.Image>(); 
		binding.imgSkill3 = transform.Find("skill/Skill3/imgSkill3").GetComponent<UnityEngine.UI.Image>(); 
		binding.imgMask3 = transform.Find("skill/Skill3/imgMask3").GetComponent<UnityEngine.UI.Image>(); 
		return binding;

	}
	public static void UnBinding(GameFormCode binding) 
	{
		 ReferencePool.Release(binding); 
	}
	public void Clear()
	{
		btnMove = null;
		imgMove = null;
		btnAtt = null;
		imgSkill0 = null;
		imgSkill1 = null;
		imgMask1 = null;
		imgSkill2 = null;
		imgMask2 = null;
		imgSkill3 = null;
		imgMask3 = null;
	}
}
