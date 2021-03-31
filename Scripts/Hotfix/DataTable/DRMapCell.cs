//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-09-03 17:13:03.000
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
    /// 地图单元格配置。
    /// </summary>
    public class DRMapCell : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取单元格Id。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取单元格索引。
        /// </summary>
        public Vector2 CellIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 获取地图区域Id。
        /// </summary>
        public int RegionId
        {
            get;
            set;
        }

        /// <summary>
        /// 获取背景图片参数。
        /// </summary>
        public int ResMain
        {
            get;
            set;
        }

        /// <summary>
        /// 获取背景图片旋转。
        /// </summary>
        public Vector3 ResMainRotate
        {
            get;
            set;
        }

        /// <summary>
        /// 获取中景图片参数。
        /// </summary>
        public int ResBg
        {
            get;
            set;
        }

        /// <summary>
        /// 获取中景图片旋转。
        /// </summary>
        public Vector3 ResBgRotate
        {
            get;
            set;
        }

        /// <summary>
        /// 获取是否能可以通过。
        /// </summary>
        public bool Pass
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
                    CellIndex = binaryReader.ReadVector2();
                    RegionId = binaryReader.ReadInt32();
                    ResMain = binaryReader.ReadInt32();
                    ResMainRotate = binaryReader.ReadVector3();
                    ResBg = binaryReader.ReadInt32();
                    ResBgRotate = binaryReader.ReadVector3();
                    Pass = binaryReader.ReadBoolean();
                }
            }

            return true;
        }

    }
}
