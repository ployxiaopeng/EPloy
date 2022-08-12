
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using System.IO;
using EPloy.Game;

namespace EPloy.Editor
{
    /// <summary>
    /// 打开文件夹相关的实用函数。
    /// </summary>
    public static class OpenFolder
    {
        /// <summary>
        /// 打开 Data Path 文件夹。
        /// </summary>
        [MenuItem("EPloy/OpenFolder/Data Path", false, 10)]
        public static void OpenFolderDataPath()
        {
            Execute(Application.dataPath);
        }

        /// <summary>
        /// 打开 Persistent Data Path 文件夹。
        /// </summary>
        [MenuItem("EPloy/OpenFolder/PersistentData Path", false, 11)]
        public static void OpenFolderPersistentDataPath()
        {
            Execute(Application.persistentDataPath);
        }

        /// <summary>
        /// 打开 Streaming Assets Path 文件夹。
        /// </summary>
        [MenuItem("EPloy/OpenFolder/StreamingAssets Path", false, 12)]
        public static void OpenFolderStreamingAssetsPath()
        {
            Execute(Application.streamingAssetsPath);
        }

        /// <summary>
        /// 打开 Temporary Cache Path 文件夹。
        /// </summary>
        [MenuItem("EPloy/OpenFolder/TemporaryCache Path", false, 13)]
        public static void OpenFolderTemporaryCachePath()
        {
            Execute(Application.temporaryCachePath);
        }

        /// <summary>
        /// 打开 DataTable 文件夹。
        /// </summary>
        [MenuItem("EPloy/OpenFolder/DataTable Path", false, 13)]
        public static void OpenFolderDataTablePath()
        {
            Execute(UtilPath.GetRegularPath(Path.Combine(Application.dataPath, EPloyEditorPath.DataTable)));
        }

#if UNITY_2018_3_OR_NEWER

        /// <summary>
        /// 打开 Console Log Path 文件夹。
        /// </summary>
        [MenuItem("EPloy/OpenFolder/ConsoleLog Path", false, 14)]
        public static void OpenFolderConsoleLogPath()
        {
            Execute(Path.GetDirectoryName(Application.consoleLogPath));
        }

#endif

        /// <summary>
        /// 打开指定路径的文件夹。
        /// </summary>
        /// <param name="folder">要打开的文件夹的路径。</param>
        public static void Execute(string folder)
        {
            folder = UtilText.Format("\"{0}\"", folder);
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    Process.Start("Explorer.exe", folder.Replace('/', '\\'));
                    break;

                case RuntimePlatform.OSXEditor:
                    Process.Start("open", folder);
                    break;

                default:
                    Log.Fatal(UtilText.Format("Not support OpenFolder on '{0}' platform.", Application.platform.ToString()));
                    break;
            }
        }
    }
}
