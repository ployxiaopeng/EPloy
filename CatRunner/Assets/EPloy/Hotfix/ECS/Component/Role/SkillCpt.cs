using EPloy.Table;
using System;
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
        public DRSkillData AttData;
        public List<DRSkillData> Skills = new List<DRSkillData>();
        public Dictionary<int, float> skillCDs = new Dictionary<int, float>();

        public Action<int> Action;

        //暂时判断前方 2m 的正方形
        public float dis = 2;


        public void SetSkillData(DRRoleData roleData)
        {
            Table<DRSkillData> DTSkill = GameModule.Table.GetDataTable<DRSkillData>();
            AttData = DTSkill.GetDataRow(roleData.RoleATT);
            skillCDs.Add(AttData.Id, 0);

            for (int i = 0; i < roleData.RoleSkills.Count; i++)
            {
                DRSkillData data = DTSkill.GetDataRow(roleData.RoleSkills[i]);
                Skills.Add(data);
                skillCDs.Add(data.Id, 0);
            }
        }

        public override void Clear()
        {
            Skills.Clear();
            skillCDs.Clear();
        }
    }
}