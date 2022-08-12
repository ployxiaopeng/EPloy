//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2022-08-10 16:23:20.899
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EPloy.Table
{
    /// <summary>
    /// 角色表。
    /// </summary>
    public class DRRoleData : IDataRow
    {
        private int _Id = 0;

        /// <summary>
        /// 获取角色Id。
        /// </summary>
        public override int Id
        {
            get
            {
                return _Id;
            }
        }

        /// <summary>
        /// 获取角色类型。
        /// </summary>
        public int RoleType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取角色名字。
        /// </summary>
        public string RoleName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取角色普攻。
        /// </summary>
        public int RoleATT
        {
            get;
            set;
        }

        /// <summary>
        /// 获取角色技能。
        /// </summary>
        public List<int> RoleSkills
        {
            get;
            set;
        }

        /// <summary>
        /// 获取角色五维。
        /// </summary>
        public List<int> RoleInfos
        {
            get;
            set;
        }

        public override  bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length)
        {
            //先这样 后面看看处理 GCAlloc 问题！
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    _Id = binaryReader.ReadInt32();
                    RoleType = binaryReader.ReadInt32();
                    RoleName = binaryReader.ReadString();
                    RoleATT = binaryReader.ReadInt32();
                    RoleSkills = binaryReader.ReadListint();
                    RoleInfos = binaryReader.ReadListint();
                }
            }

            return true;
        }

    }
}
