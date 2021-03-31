//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2020-09-03 17:13:03.024
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
    /// 文本表。
    /// </summary>
    public class DRText : DataRowBase
    {
        private int m_Id = 0;

        /// <summary>
        /// 获取文本Id(int)。
        /// </summary>
        public override int Id
        {
            get
            {
                return m_Id;
            }
        }

        /// <summary>
        /// 获取简体中文(string)。
        /// </summary>
        public string ChineseSimplified
        {
            get;
            set;
        }

        /// <summary>
        /// 获取繁体中文(string)。
        /// </summary>
        public string ChineseTraditional
        {
            get;
            set;
        }

        /// <summary>
        /// 获取英文文(string)。
        /// </summary>
        public string English
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
                    ChineseSimplified = binaryReader.ReadString();
                    ChineseTraditional = binaryReader.ReadString();
                    English = binaryReader.ReadString();
                }
            }

            return true;
        }

    }
}
