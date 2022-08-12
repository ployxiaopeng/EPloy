using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Player;
using UnityEngine;

namespace HybridCLR
{
    internal class CompileDllHelper
    {
        public static void CompileDll(string buildDir, BuildTarget target)
        {
            var group = BuildPipeline.GetBuildTargetGroup(target);

            ScriptCompilationSettings scriptCompilationSettings = new ScriptCompilationSettings();
            scriptCompilationSettings.group = group;
            scriptCompilationSettings.target = target;
            Directory.CreateDirectory(buildDir);
            ScriptCompilationResult scriptCompilationResult = PlayerBuildInterface.CompilePlayerScripts(scriptCompilationSettings, buildDir);
            foreach (var ass in scriptCompilationResult.assemblies)
            {
                Debug.LogFormat("compile assemblies:{1}/{0}", ass, buildDir);
            }
        }

        public static void CompileDll(BuildTarget target)
        {
            CompileDll(BuildConfig.GetHotFixDllsOutputDirByTarget(target), target);
        }

        [MenuItem("EPloy/Hotfix/CompileDllActiveBuildTarget",false, 100)]
        public static void CompileDllActiveBuildTarget()
        {
            CompileDll(EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem("EPloy/Hotfix/CompileDllWin32", false, 100)]
        public static void CompileDllWin32()
        {
            CompileDll(BuildTarget.StandaloneWindows);
        }

        [MenuItem("EPloy/Hotfix/CompileDllWin64", false, 100)]
        public static void CompileDllWin64()
        {
            CompileDll(BuildTarget.StandaloneWindows64);
        }

        [MenuItem("EPloy/Hotfix/CompileDllAndroid", false, 100)]
        public static void CompileDllAndroid()
        {
            CompileDll(BuildTarget.Android);
        }

        [MenuItem("EPloy/Hotfix/CompileDllIOS", false, 100)]
        public static void CompileDllIOS()
        {
            CompileDll(BuildTarget.iOS);
        }
    }
}
