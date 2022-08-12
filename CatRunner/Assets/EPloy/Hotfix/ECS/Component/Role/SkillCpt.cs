using EPloy.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{
    /// <summary>
    ///技能组件
    /// </summary>
    public class SkillCpt : CptBase
    {
        //暂时判断前方 2m 的正方形
        public float dis = 2;

        public DRSkillData skillData;

        public float hurt;

        public GameObject effect;
        //目标实体
        public List<EntityRole> targetEntitys = new List<EntityRole>();

        public override void Clear()
        {
            targetEntitys.Clear();
            skillData = null;
        }
    }
}