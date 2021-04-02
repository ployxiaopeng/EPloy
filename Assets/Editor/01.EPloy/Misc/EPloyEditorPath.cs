
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

        public const string DataTablePath = "Assets/Res/DataTables/Data";
        public const string DataBinaryPath = "Assets/Res/DataTables/Binary";
        public const string CSharpCodePath = "Assets/EPloy/Hotfix/Component/DataTable/Table";
        public const string CSharpCodeTemplateFileName = "Assets/Res/Configs/DataTableCodeTemplate.txt";

        public const string ILRuntimeGenerated = "Assets/EPloy/Game/Mudule/ILRuntime/Generated";
        public const string AssetHotfixDLL = "Assets/StreamingAssets/Hotfix.dll.bytes";
        public const string AssetHotfixPdb = "Assets/StreamingAssets/Hotfix.pdb.bytes";

        public static string EditorHotfixDLL = System.Environment.CurrentDirectory + "/Library/ScriptAssemblies/EPloy.dll";
        public static string EditorHotfixPdb = System.Environment.CurrentDirectory + "/Library/ScriptAssemblies/EPloy.pdb";
    }
}
