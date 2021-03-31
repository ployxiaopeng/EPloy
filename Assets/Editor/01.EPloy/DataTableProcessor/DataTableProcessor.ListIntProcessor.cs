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
        private sealed class ListIntProcessor : GenericDataProcessor<List<int>>
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
                    return "List<int>";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "List<int>"
                };
            }

            public override List<int> Parse(string value)
            {
                string[] splitValue = value.Split(';');
                List<int> listInt = new List<int>();
                foreach (var str in splitValue)
                {
                    listInt.Add(int.Parse(str));
                }
                return listInt;
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                List<int> listInt = Parse(value);
                //坑 写一个长度进去
                short count = (short)listInt.Count;
                stream.Write(count);
                foreach (var num in listInt)
                {
                    stream.Write(num);
                }
            }
        }
    }
}
