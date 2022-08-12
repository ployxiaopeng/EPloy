using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using EPloy.Game;

namespace EPloy.Editor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ILRuntimeCLRBinding
    {
        [MenuItem("EPloy/ILRuntime/构建ICR绑定")]
        static void GenerateCLRBindingByAnalysis()
        {
            //用新的分析热更dll调用引用来生成绑定代码
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (System.IO.FileStream fs = new System.IO.FileStream(EPloyEditorPath.AssetHotfixDLL, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                domain.LoadAssembly(fs);
                //Crossbind Adapter is needed to generate the correct binding code
                ILRuntimeHelper.InitILRuntime(domain);
                ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, EPloyEditorPath.ILRuntimeGenerated);

            }
            AssetDatabase.Refresh();
        }

        [MenuItem("EPloy/ILRuntime/更新HotfixDll")]
        static void UpdateHotfixDll()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                //复制dll pdb
                File.Copy(EPloyEditorPath.EditorHotfixDLL, EPloyEditorPath.AssetHotfixDLL, true);
                File.Copy(EPloyEditorPath.EditorHotfixPdb, EPloyEditorPath.AssetHotfixPdb, true);
                //刷新资源
                AssetDatabase.Refresh();

                Log.Info("更新HotFix dll!");
            }
        }

        [MenuItem("EPloy/ILRuntime/模板生成适配器")]
        static void GenerateCrossbindAdapter()
        {
            //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
            //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("Assets/EPloy/Game/Mudule/ILRuntime/Adaptor/IObjectBaseAdaptor.cs"))
            {
                sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(EPloy.Game.ObjectPool.ObjectBase), "EPloy"));
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("EPloy/ILRuntime/安装VS调试插件")]
        static void InstallDebugger()
        {
            EditorUtility.OpenWithDefaultApp("Assets/Libraries/ILRuntime//Debugger/ILRuntimeDebuggerLauncher.vsix");
        }
    }
}
