//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
using ETHotfix;
using GameFramework;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using UnityGameFramework.Editor.DataTableTools;

namespace Editor.DataTableTools
{
    public sealed class DataTableGeneratorMenu
    {
        [MenuItem("Tools/转表工具", false, 3)]
        public static void GenerateDataTables()
        {
            //文本格式标准化
            TextFormat.Format();
            foreach (string dataTableName in ConfigRes.DataTableNames)
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
        [MenuItem("Game Framework/转表同时转脚本")]
        public static void Func01()
        {
            ScriptingDefineSymbols.AddScriptingDefineSymbol(Table);
        }
        [MenuItem("Game Framework/只转表")]
        public static void Func02()
        {
            ScriptingDefineSymbols.RemoveScriptingDefineSymbol(Table);
        }
    }
}
