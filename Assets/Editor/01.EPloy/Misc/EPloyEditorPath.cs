//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2021 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;

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
        public const string CSharpCodePath = "Assets/EPloy/Component/DataTable/Table";
        public const string CSharpCodeTemplateFileName = "Assets/Res/Configs/DataTableCodeTemplate.txt";

        public const string ILRuntimeGenerated = "Assets/GameMain/ILRuntime/Generated";
        public const string HotfixDLL = "Assets/Res/HotfixDLL/Hotfix.dll.bytes";
    }
}
