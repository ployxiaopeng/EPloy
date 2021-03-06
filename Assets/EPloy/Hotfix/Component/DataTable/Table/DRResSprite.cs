﻿//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2021-07-15 16:29:32.599
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 图片配置表。
    /// </summary>
    public class DRResSprite : IDataRow
    {
        private int _Id = 0;

        /// <summary>
        /// 获取图片Id。
        /// </summary>
        public  int Id
        {
            get
            {
                return _Id;
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

        public  bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length)
        {
            //先这样 后面看看处理 GCAlloc 问题！
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    _Id = binaryReader.ReadInt32();
                    ImageName = binaryReader.ReadString();
                    AtlasName = binaryReader.ReadString();
                }
            }

            return true;
        }

    }
}
