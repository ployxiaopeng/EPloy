//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2022-08-10 16:23:20.902
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EPloy.Table
{
    /// <summary>
    /// 技能表。
    /// </summary>
    public class DRSkillData : IDataRow
    {
        private int _Id = 0;

        /// <summary>
        /// 获取技能Id。
        /// </summary>
        public override int Id
        {
            get
            {
                return _Id;
            }
        }

        /// <summary>
        /// 获取名字。
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 获取描述。
        /// </summary>
        public string Describe
        {
            get;
            set;
        }

        /// <summary>
        /// 获取Icon。
        /// </summary>
        public int Icon
        {
            get;
            set;
        }

        /// <summary>
        /// 获取冷却。
        /// </summary>
        public float CDTime
        {
            get;
            set;
        }

        /// <summary>
        /// 获取类型。
        /// </summary>
        public int SkillType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取伤害系数。
        /// </summary>
        public float HarmArg
        {
            get;
            set;
        }

        /// <summary>
        /// 获取附加效果类型。
        /// </summary>
        public int AddType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取附加效果参数。
        /// </summary>
        public int AddArg
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
                    Name = binaryReader.ReadString();
                    Describe = binaryReader.ReadString();
                    Icon = binaryReader.ReadInt32();
                    CDTime = binaryReader.ReadSingle();
                    SkillType = binaryReader.ReadInt32();
                    HarmArg = binaryReader.ReadSingle();
                    AddType = binaryReader.ReadInt32();
                    AddArg = binaryReader.ReadInt32();
                }
            }

            return true;
        }

    }
}
