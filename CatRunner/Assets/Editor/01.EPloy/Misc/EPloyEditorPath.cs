
using System;
using System.IO;
using UnityEngine;

namespace EPloy.Editor
{
    /// <summary>
    /// 配置路径属性。
    /// </summary>
    public static class EPloyEditorPath
    {
        public const string BuildSettings = "Res/Configs/BuildSettings.xml";
        public const string ResBuilder = "Res/Configs/ResBuilder.xml";
        public const string ResEditor = "Res/Configs/ResEditor.xml";
        public const string DataTable = "Res/DataTable";
        public const string ResCollection = "Res/Configs/ResCollection.xml";

        public const string DataTablePath = "Assets/Res/Tables/Data";
        public const string DataBinaryPath = "Assets/Res/Tables/Binary";
        public const string CSharpCodePath = "Assets/EPloy/Hotfix/Table";
        public const string CSharpCodeTemplateFileName = "Assets/Res/Configs/TableCodeTemplate.txt";

        public const string AssetHotfixDLL = "Assets/StreamingAssets/Hotfix.dll.bytes";

        public static string EditorHotfixDLL = System.Environment.CurrentDirectory + "/Library/ScriptAssemblies/EPloy.Hotfix.dll";
    }
}
