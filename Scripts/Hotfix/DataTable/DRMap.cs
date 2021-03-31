//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-09-03 17:13:03.040
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 地图。
    /// </summary>
    public class DRMap : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取地图Id。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
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
        /// 获取区域Id。
        /// </summary>
        public int MapRegionId
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
        public Vector2 RoleBornPos
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

        public override bool ParseDataRow(GameFrameworkSegment<byte[]> dataRowSegment)
        {
            //先这样 后面看看处理 GCAlloc 问题！
            using (MemoryStream memoryStream = new MemoryStream(dataRowSegment.Source, dataRowSegment.Offset, dataRowSegment.Length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.ReadInt32();
                    NameId = binaryReader.ReadInt32();
                    MapRegionId = binaryReader.ReadInt32();
                    RowAndColumn = binaryReader.ReadVector2();
                    RoleBornPos = binaryReader.ReadVector2();
                    RolelRotate = binaryReader.ReadVector3();
                    BackHome = binaryReader.ReadVector2();
                }
            }

            return true;
        }

    }
}
