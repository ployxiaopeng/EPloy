//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2021-07-15 16:29:32.577
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 实体表。
    /// </summary>
    public class DREntity : IDataRow
    {
        private int _Id = 0;

        /// <summary>
        /// 获取实体编号。
        /// </summary>
        public  int Id
        {
            get
            {
                return _Id;
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

        public  bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length)
        {
            //先这样 后面看看处理 GCAlloc 问题！
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    _Id = binaryReader.ReadInt32();
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
