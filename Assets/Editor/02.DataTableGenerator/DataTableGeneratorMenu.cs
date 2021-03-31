
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;

namespace EPloy.Editor.DataTableTools
{
    public sealed class DataTableGeneratorMenu
    {
        [MenuItem("EPloy/Table工具")]
        public static void GenerateDataTables()
        {
            //文本格式标准化
            Format();
            foreach (string dataTableName in Config.DataTableNames)
            {
                DataTableProcessor dataTableProcessor = DataTableGenerator.CreateDataTableProcessor(dataTableName);
                if (!DataTableGenerator.CheckRawData(dataTableProcessor, dataTableName))
                {
                    Debug.LogError(Utility.Text.Format("Check raw data failure. DataTableName='{0}'", dataTableName));
                    break;
                }

                DataTableGenerator.GenerateDataFile(dataTableProcessor, dataTableName);
                //数据暂时不需要改这个
                DataTableGenerator.GenerateCodeFile(dataTableProcessor, dataTableName);
            }
            AssetDatabase.Refresh();
        }

        public const string Table = "TABLE";
        [MenuItem("EPloy/Table工具/TableEndClass")]
        public static void Func01()
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol(Table);
        }
        [MenuItem("EPloy/Table工具/Table")]
        public static void Func02()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(Table);
        }

        //标准格式,Unicode小尾编码
        public static System.Text.UnicodeEncoding UTF16_LE = new System.Text.UnicodeEncoding();

        /// <summary> 
        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型 
        /// </summary> 
        /// <param name=“FILE_NAME“>文件路径</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// <summary> 
        /// 通过给定的文件流，判断文件的编码类型 
        /// </summary> 
        /// <param name=“fs“>文件流</param> 
        /// <returns>文件的编码类型</returns> 
        public static System.Text.Encoding GetType(FileStream fs)
        {
            //默认编码就是ANSI编码
            Encoding reVal = Encoding.Default;
            //读取前俩个字节
            BinaryReader r = new BinaryReader(fs);
            byte[] bytes = r.ReadBytes(2);
            r.Close();
            //小尾Unicode编码
            if (bytes[0] == 0xFF && bytes[1] == 0xFE)
            {
                return System.Text.Encoding.Unicode;
            }
            //大尾Unicode编码
            if (bytes[0] == 0xFE && bytes[1] == 0xFF)
            {
                return System.Text.Encoding.BigEndianUnicode;
            }
            //UTF-8编码
            if (bytes[0] == 0xEF && bytes[1] == 0xBB)
            {
                return System.Text.Encoding.UTF8;
            }
            return reVal;

        }

        /// <summary>
        /// 修改文本编码格式
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="oldEncoding">旧编码</param>
        /// <param name="newEncoding">新编码</param>
        public static void ChangeFileEncoding(string path, Encoding oldEncoding, Encoding newEncoding)
        {
            string text = "";
            if (oldEncoding == Encoding.Default)
            {
                StreamReader sr = new StreamReader(File.Open(path, FileMode.Open), Encoding.GetEncoding("gb2312"));
                text = sr.ReadToEnd();
                sr.Close();
            }
            else { text = File.ReadAllText(path, oldEncoding); }
            byte[] byts1 = oldEncoding.GetBytes(text);
            byte[] byts2 = Encoding.Convert(oldEncoding, newEncoding, byts1);
            string text1 = newEncoding.GetString(byts2);
            using (StreamWriter sw = new StreamWriter(path, false, newEncoding))
            {
                sw.Write(text1);
                sw.Close();
            }
        }

        //[MenuItem("Tools/YQ/08.文本格式标准化")]
        public static void Format()
        {
            Log.Info("开始标准化所有文本格式");
            string Path = EPloyEditorPath.DataTablePath;
            //C#遍历指定文件夹中的所有文件 
            DirectoryInfo TheFolder = new DirectoryInfo(Path);
            //遍历文件
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                if (NextFile.Name.EndsWith(".txt"))
                {
                    Encoding encoding = GetType(NextFile.FullName);
                    if (encoding == UTF16_LE)
                    {
                        continue;
                    }
                    ChangeFileEncoding(NextFile.FullName, encoding, UTF16_LE);
                }
            }
            Log.Info("成功标准化所有文本格式");
        }
    }
}
