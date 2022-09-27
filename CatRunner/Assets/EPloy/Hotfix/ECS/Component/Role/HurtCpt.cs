using EPloy.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{
    /// <summary>
    ///伤害组件
    /// </summary>
    public class HurtCpt : CptBase
    {
        public DRSkillData skillData;
        public float hurt;
        public GameObject effect;
        public List<EntityRole> targetEntitys = new List<EntityRole>();

        public override void Clear()
        {
            hurt = 0;
            effect = null;
            skillData = null;
            targetEntitys.Clear();
        }
    }
}