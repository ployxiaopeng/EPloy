//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-09-03 17:13:02.810
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
    /// 图片配置表。
    /// </summary>
    public class DRResSprite : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取图片Id。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取图片名称。
        /// </summary>
        public string ImageName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取图集名称。
        /// </summary>
        public string AtlasName
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
                    ImageName = binaryReader.ReadString();
                    AtlasName = binaryReader.ReadString();
                }
            }

            return true;
        }

    }
}
