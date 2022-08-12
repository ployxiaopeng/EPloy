//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：2022-08-10 16:23:20.891
//------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace EPloy.Table
{
    /// <summary>
    /// 语言表。
    /// </summary>
    public class DRLanguageText : IDataRow
    {
        private int _Id = 0;

        /// <summary>
        /// 获取文本Id(int)。
        /// </summary>
        public override int Id
        {
            get
            {
                return _Id;
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
        /// 获取英文(string)。
        /// </summary>
        public string English
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
                    ChineseSimplified = binaryReader.ReadString();
                    ChineseTraditional = binaryReader.ReadString();
                    English = binaryReader.ReadString();
                }
            }

            return true;
        }

    }
}
