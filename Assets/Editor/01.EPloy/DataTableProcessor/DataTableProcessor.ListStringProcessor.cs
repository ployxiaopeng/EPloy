//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
using System.Collections.Generic;
using System.IO;

namespace UnityGameFramework.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ListStringProcessor : GenericDataProcessor<List<string>>
        {
            public override bool IsSystem
            {
                get
                {
                    return false;
                }
            }

            public override string LanguageKeyword
            {
                get
                {
                    return "List<string>";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "List<string>"
                };
            }

            public override List<string> Parse(string value)
            {
                string[] splitValue = value.Split(';');
                List<string> listStr = new List<string>();
                foreach (var str in splitValue)
                {
                    listStr.Add(str);
                }
                return listStr;
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                List<string> listStr = Parse(value);
                //坑写一个长度进去
                short count = (short)listStr.Count;
                stream.Write(count);
                foreach (var str in listStr)
                {
                    stream.Write(str);
                }
            }
        }
    }
}
