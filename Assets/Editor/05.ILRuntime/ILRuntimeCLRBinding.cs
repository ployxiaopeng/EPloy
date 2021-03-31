using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

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
    }
}
