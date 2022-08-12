using EPloy.Hotfix.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{

    public enum SkillType
    {
        Harm,//伤害
        Buff,
        DeBuff,

    }

    /// <summary>
    /// 角色组件
    /// </summary>
    public class RoleBaseCpt : CptBase
    {
        // 类型 玩家0 npc 之类的 
        public RoleType roleType;
        public DRRoleData playerData;
        public DRSkillData AttData;
        public List<DRSkillData> Skills = new List<DRSkillData>();
        public Dictionary<int, float> skillCDs = new Dictionary<int, float>();
        //角色当前五维 
        public int maxHp;
        public int maxMp;
        public int curHp;
        public int curMp;
        public int att;
        public int def;
        public int crit;

        public void SetRoleData(int id)
        {
            Table<DRSkillData> DTSkill = HotFixMudule.DataTable.GetDataTable<DRSkillData>();
            playerData = HotFixMudule.DataTable.GetDataTable<DRRoleData>().GetDataRow(id);

            AttData = DTSkill.GetDataRow(playerData.RoleATT);
            skillCDs.Add(AttData.Id, 0);

            for (int i = 0; i < playerData.RoleSkills.Count; i++)
            {
                DRSkillData data = DTSkill.GetDataRow(playerData.RoleSkills[i]);
                Skills.Add(data);
                skillCDs.Add(data.Id, 0);
            }
            maxHp = playerData.RoleInfos[0];
            maxMp = playerData.RoleInfos[1];
            curHp = playerData.RoleInfos[0];
            curMp = playerData.RoleInfos[1];
            att = playerData.RoleInfos[2];
            def = playerData.RoleInfos[3];
            crit = playerData.RoleInfos[4];
        }

        public override void Clear()
        {
            base.Clear();
            Skills = null;
            Skills.Clear();
            skillCDs.Clear();
        }
    }
}