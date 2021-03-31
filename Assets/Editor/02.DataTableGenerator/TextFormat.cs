/************************************************************
	文件: TextFormat.cs
	作者: 那位先生
	邮箱: 1279544114@qq.com
	日期: 2020/3/31 16:6:18
	功能: 文本格式标准化,游戏出现乱码,或者转表失败时使用
*************************************************************/

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using Editor.DataTableTools;

namespace Editor.DataTableTools
{
    /************************************
     * 文件的字符集在Windows下有两种，一种是ANSI，一种Unicode。
     * 对于Unicode，Windows支持了它的三种编码方式，一种是小尾编码（Unicode,UTF-16LE)，一种是大尾编码(BigEndianUnicode,UTF-16BE)，一种是UTF-8编码。
     * 我们可以从文件的头部来区分一个文件是属于哪种编码。
     * 当头部开始的两个字节为 0xFF 0xFE时，是Unicode的小尾编码；当头部的两个字节为0xFE 0xFF时，是Unicode的大尾编码；
     * 当头部两个字节为0xEF 0xBB时，是Unicode的UTF-8编码；
     * 当它不为这些时，则是ANSI编码。
     * 如上所说，我们可以通过读取文件头的两个字节来判断文件的编码格式
     ************************************/
    public class TextFormat : MonoBehaviour
    {
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
            Debug.Log("开始标准化所有文本格式");
            string Path = DataTableGenerator.DataTablePath;
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
            Debug.Log("成功标准化所有文本格式");
        }
    }
}