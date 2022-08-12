using EPloy.Hotfix.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
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


    /// <summary>
    /// 角色组件
    /// </summary>
    public class RoleCpt : CptBase
    {
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

        public MapRoleData  roleData;

        public override void Clear()
        {
            base.Clear();
        }
    }
}

