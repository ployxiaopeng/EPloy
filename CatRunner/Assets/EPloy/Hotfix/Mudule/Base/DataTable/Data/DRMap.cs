//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2022-06-20 10:13:25.448
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EPloy.Hotfix.Table
{
    /// <summary>
    /// 地图。
    /// </summary>
    public class DRMap : IDataRow
    {
        private int _Id = 0;

        /// <summary>
        /// 获取地图Id。
        /// </summary>
        public override int Id
        {
            get
            {
                return _Id;
            }
        }

        /// <summary>
        /// 获取地图名字。
        /// </summary>
        public int NameId
        {
            get;
            set;
        }

        /// <summary>
        /// 获取地图行列。
        /// </summary>
        public Vector2 RowAndColumn
        {
            get;
            set;
        }

        /// <summary>
        /// 获取角色出生坐标。
        /// </summary>
        public Vector3 RoleBornPos
        {
            get;
            set;
        }

        /// <summary>
        /// 获取角色旋转。
        /// </summary>
        public Vector3 RolelRotate
        {
            get;
            set;
        }

        /// <summary>
        /// 获取回城点。
        /// </summary>
        public Vector2 BackHome
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
                    NameId = binaryReader.ReadInt32();
                    RowAndColumn = binaryReader.ReadVector2();
                    RoleBornPos = binaryReader.ReadVector3();
                    RolelRotate = binaryReader.ReadVector3();
                    BackHome = binaryReader.ReadVector2();
                }
            }

            return true;
        }

    }
}
