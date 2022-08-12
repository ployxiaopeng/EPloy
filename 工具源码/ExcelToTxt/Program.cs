using System;
using System.Text;
using Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using System.Diagnostics;


//功能目标，将当前目录下的所有xlsx的EXCEL文件导出为csv和txt格式
namespace ExcelTools
{
    class Program
    {
        private static void QuitExcel()
        {
            Process[] excels = Process.GetProcessesByName("EXCEL");
            foreach (var item in excels)
            {
                item.Kill();
            }
        }
        static void Main(string[] args)
        {
            QuitExcel();
            string InputFilePath = Environment.CurrentDirectory + @"\Table";  //excel文件目录
            string AssaetPath = @"\CatRunner\Assets\Res\DataTables\Data\";
            string OutputFilePath_txt = Directory.GetParent(System.Environment.CurrentDirectory).FullName + AssaetPath;//excel导出txt目录

            //if (Directory.Exists(OutputFilePath_txt))
            //{
            //    Directory.Delete(OutputFilePath_txt, true);
            //    Directory.CreateDirectory(OutputFilePath_txt);
            //}
            //else
            //{
            //    Directory.CreateDirectory(OutputFilePath_txt);
            //}

            foreach (FileInfo file in (new DirectoryInfo(OutputFilePath_txt)).GetFiles())
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }

            #region 读取当前目录下的所有xlsx文件
            //获取应用程序运行目录
            DirectoryInfo dir = new DirectoryInfo(InputFilePath);
            FileInfo[] filename = dir.GetFiles("*.xlsx");
            Application App = new Application();
            object nothing = Missing.Value;

            foreach (var item in filename)
            {
                try
                {
                    string newFileName = string.Empty;
                    //保存到csv
                    Workbook AppBook = App.Workbooks.Open(item.FullName, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing, nothing);
                    int count = AppBook.Worksheets.Count;
                    // 只有一张表 直接取表名
                    if (count == 1)
                    {
                        Worksheet AppSheet = (Worksheet)AppBook.Worksheets[count];
                        if (AppSheet.Name == "")
                        {
                            Console.WriteLine(item.ToString() + "分表命个名啊");
                            AppBook.Close(false, Type.Missing, Type.Missing);
                            AppBook = null;
                            return;
                        }
                        newFileName = OutputFilePath_txt + AppSheet.Name + ".txt";
                        AppSheet.SaveAs(newFileName, XlFileFormat.xlUnicodeText, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        AppBook.Close(false, Type.Missing, Type.Missing);
                        AppBook = null;
                    }
                    else
                    {
                        for (int i = 1; i <= count; i++)
                        {
                            Worksheet AppSheet = (Worksheet)AppBook.Worksheets[i];
                            if (AppSheet.Name == "")
                            {
                                Console.WriteLine(item.ToString() + "分表命个名啊");
                                AppBook.Close(false, Type.Missing, Type.Missing);
                                AppBook = null;
                                return;
                            }
                            newFileName = OutputFilePath_txt + item.Name.Replace(".xlsx", AppSheet.Name + ".txt");
                            AppSheet.SaveAs(newFileName, XlFileFormat.xlUnicodeText, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                        }
                        AppBook.Close(false, Type.Missing, Type.Missing);
                        AppBook = null;
                    }


                    //将txt文件的编码格式修改为UTF8
                    //StreamReader sr = new StreamReader(newFileName, Encoding.Unicode, false);
                    //string contenttxt = sr.ReadToEnd();
                    //sr.Close();
                    //StreamWriter sw = new StreamWriter(newFileName, false, Encoding.UTF8);
                    //sw.Write(contenttxt);
                    //sw.Close();
                    Console.WriteLine(item.ToString() + " 已完成");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

            }

            App.Quit();
            #endregion
            Console.ReadKey();
        }

        /// <summary>
        /// 就是测试
        /// </summary>
        public void Test()
        {

        }
    }
}