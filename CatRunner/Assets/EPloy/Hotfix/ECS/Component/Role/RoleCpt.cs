using EPloy.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{
    /// <summary>
    /// 角色类型
    /// </summary>
    public enum RoleType
    {
        Player,
        NPC,
        Monster,
    }

    public enum SkillType
    {
        Harm,//伤害
        Buff,
        DeBuff,

    }

    /// <summary>
    /// 角色状态
    /// </summary>
    public enum RoleState
    {
        Idle,
        Move,
        Att,
        Die
    }

    /// <summary>
    /// 角色组件
    /// </summary>
    public class RoleCpt : CptBase
    {
        // 类型 玩家0 npc 之类的 
        public RoleType roleType;
        public RoleState roleState;
        public DRRoleData playerData;
        public MapRoleData roleData;
        public RoleActionHandler actionHandler;
         
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
            playerData = GameModule.Table.GetDataTable<DRRoleData>().GetDataRow(id);

            maxHp = playerData.RoleInfos[0];
            maxMp = playerData.RoleInfos[1];
            curHp = playerData.RoleInfos[0];
            curMp = playerData.RoleInfos[1];
            att = playerData.RoleInfos[2];
            def = playerData.RoleInfos[3];
            crit = playerData.RoleInfos[4];
        }

        public Vector3 rolePos
        {
            get { return role.transform.localPosition; }
        }
        public Vector3 roleRotate
        {
            get { return role.transform.localEulerAngles; }
        }
        public GameObject role
        {
            get { return roleData.Obj; }
        }

        public override void Clear()
        {
            base.Clear();
            actionHandler.RemoveHandler();
            playerData = null;
            actionHandler = null;
            ReferencePool.Release(roleData);
        }
    }
}

