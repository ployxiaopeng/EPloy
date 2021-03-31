using System.Collections.Generic;
using System.IO;

namespace EPloy.Editor.DataTableTools
{
    public sealed partial class DataTableProcessor
    {
        private sealed class ListFloatProcessor : GenericDataProcessor<List<float>>
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
                    return "List<float>";
                }
            }

            public override string[] GetTypeStrings()
            {
                return new string[]
                {
                    "List<float>"
                };
            }

            public override List<float> Parse(string value)
            {
                string[] splitValue = value.Split(';');
                List<float> listInt = new List<float>();
                foreach (var str in splitValue)
                {
                    listInt.Add(float.Parse(str));
                }
                return listInt;
            }

            public override void WriteToStream(BinaryWriter stream, string value)
            {
                List<float> listInt = Parse(value);
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
