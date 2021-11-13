//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2021-07-15 16:29:35.598
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图单元格配置。
    /// </summary>
    public class DRMapCell : IDataRow
    {
        private int _Id = 0;

        /// <summary>
        /// 获取单元格Id。
        /// </summary>
        public int Id
        {
            get { return _Id; }
        }

        /// <summary>
        /// 获取单元格索引。
        /// </summary>
        public Vector2 CellIndex { get; set; }

        /// <summary>
        /// 获取地图区域Id。
        /// </summary>
        public int RegionId { get; set; }

        /// <summary>
        /// 获取背景图片参数。
        /// </summary>
        public int ResMain { get; set; }

        /// <summary>
        /// 获取背景图片旋转。
        /// </summary>
        public Vector3 ResMainRotate { get; set; }

        /// <summary>
        /// 获取中景图片参数。
        /// </summary>
        public int ResBg { get; set; }

        /// <summary>
        /// 获取中景图片旋转。
        /// </summary>
        public Vector3 ResBgRotate { get; set; }

        /// <summary>
        /// 获取是否能可以通过。
        /// </summary>
        public bool Pass { get; set; }

        public bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length)
        {
            //先这样 后面看看处理 GCAlloc 问题！
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    _Id = binaryReader.ReadInt32();
                   // CellIndex = binaryReader.ReadVector2();
                    RegionId = binaryReader.ReadInt32();
                    ResMain = binaryReader.ReadInt32();
                    ResMainRotate = binaryReader.ReadVector3();
                    ResBg = binaryReader.ReadInt32();
                    ResBgRotate = binaryReader.ReadVector3();
                    Pass = binaryReader.ReadBoolean();
                    Log.Error("ResBgRotate " + ResBgRotate);
                }
            }

            return true;
        }
    }
}
