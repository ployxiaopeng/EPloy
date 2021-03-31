//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-09-03 17:13:02.797
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
    /// 实体表。
    /// </summary>
    public class DREntity : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取实体编号。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取类别。
        /// </summary>
        public string AssetType
        {
            get;
            set;
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取多少秒自动销毁。
        /// </summary>
        public float DestroyTime
        {
            get;
            set;
        }

        /// <summary>
        /// 获取初始缩放。
        /// </summary>
        public Vector3 ScaleSize
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
                    AssetType = binaryReader.ReadString();
                    AssetName = binaryReader.ReadString();
                    DestroyTime = binaryReader.ReadSingle();
                    ScaleSize = binaryReader.ReadVector3();
                }
            }

            return true;
        }

    }
}
