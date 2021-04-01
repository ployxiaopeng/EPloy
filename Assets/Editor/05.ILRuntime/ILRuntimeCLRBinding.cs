using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;

namespace EPloy.Editor
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class ILRuntimeCLRBinding
    {
        [MenuItem("EPloy/ILRuntime/ICR绑定Code")]
        static void GenerateCLRBinding()
        {
            List<Type> types = new List<Type>();
            types.Add(typeof(int));
            types.Add(typeof(float));
            types.Add(typeof(long));
            types.Add(typeof(object));
            types.Add(typeof(string));
            types.Add(typeof(Array));
            types.Add(typeof(Vector2));
            types.Add(typeof(Vector3));
            types.Add(typeof(Quaternion));
            types.Add(typeof(GameObject));
            types.Add(typeof(UnityEngine.Object));
            types.Add(typeof(Transform));
            types.Add(typeof(RectTransform));
            types.Add(typeof(Time));
            types.Add(typeof(Debug));
            //所有DLL内的类型的真实C#类型都是ILTypeInstance
            types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, EPloyEditorPath.ILRuntimeGenerated);
            AssetDatabase.Refresh();
        }

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
                AssetDatabase.Refresh();
            }
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

        [MenuItem("EPloy/ILRuntime/生成适配器")]
        static void GenerateCrossbindAdapter()
        {
            //由于跨域继承特殊性太多，自动生成无法实现完全无副作用生成，所以这里提供的代码自动生成主要是给大家生成个初始模版，简化大家的工作
            //大多数情况直接使用自动生成的模版即可，如果遇到问题可以手动去修改生成后的文件，因此这里需要大家自行处理是否覆盖的问题
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter("Assets/Samples/ILRuntime/1.6.7/Demo/Scripts/Examples/04_Inheritance/InheritanceAdapter.cs"))
            {
                sw.WriteLine(ILRuntime.Runtime.Enviorment.CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(typeof(IEnumerator), "ILRuntimeDemo"));
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
