using EPloy.Fsm;
using EPloy.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{

	public enum AIType
	{
		Monster,
		Npc,
		Boss,
		Leader,
		Quest,
	}

	public enum AIState
	{
		Think,
		Att,
		Skill,
		Idle,
		Search,
		Pathfinding,
	}

	/// <summary>
	/// 通用ai组件
	/// </summary>
	public class CommonAICpt : CptBase
	{
		public AIType aIType;
		public AIState aIState;
		public string aiName;
		public bool isRuning;
		public Entity target;

		public override void Clear()
        {
            base.Clear();
			isRuning = false;

		}
    }
}