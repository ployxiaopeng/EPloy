using GameFramework;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace ETModel
{
    /// <summary>
    /// ILRuntime组件
    /// </summary>
    public class ILRuntimeComponent : Component
    {
        private const string HotfixStr = "Hotfix";
        private const string GameEntryStr = "ETHotfix.GameEntry";
        private const string AwakeStr = "Awake";
        private const string StartStr = "Start";
        private const string UpdateStr = "Update";
        private const string LateUpdateStr = "LateUpdate";
        private const string FixUpdateStr = "FixUpdate";
        private const string ShutdownStr = "Shutdown";

        /// <summary>
        /// 非热更模式下使用
        /// </summary>
        private Assembly editorAssembly;

        private bool IsILRuntime;
        private List<Type> Types;
        private IMethod hUpdate;
        private IMethod hLateUpdate;
        private IMethod hFixUpdate;

        private MethodInfo eUpdate;
        private MethodInfo eLateUpdate;
        private MethodInfo eFixUpdate;

        /// <summary>
        /// 是否开启ILRuntime模式
        /// </summary>
        public bool ILRuntimeMode
        {
            get
            {
                return IsILRuntime;
            }
        }
        /// <summary>
        /// 是否开启热更层的轮转
        /// </summary>
        public bool OpenHotfixUpdate { get; set; }

        /// <summary>
        /// ILRuntime入口对象
        /// </summary>
        public AppDomain AppDomain
        {
            get;
            private set;
        }

        public void Awake(bool isILRuntime)
        {
            OpenHotfixUpdate = false;
            IsILRuntime = isILRuntime;
            if (!IsILRuntime)
            {
#if UNITY_EDITOR
                Log.Info("当前为 LRuntime Editor模式");
#else
                Log.Error("移动平台只有ILRuntime 模式不开会报错");
#endif
                return;
            }
            AppDomain = new AppDomain();
            ILRuntimeHelper.InitILRuntime(AppDomain);
        }
        /// <summary>
        /// 加载热更新DLL
        /// </summary>
        public async void StartAsync()
        {
            if (!IsILRuntime)
            {
#if UNITY_EDITOR
                Init.Instance.StartCoroutine(EditorHotfixStart());
#else
                Log.Error("ERR: is not Editor module in mobile platform");
#endif
                return;
            }
            TextAsset dllAsset = await Init.Resource.AwaitLoadAsset<TextAsset>(AssetUtility.GetHotfixDLLAsset("Hotfix.dll"));
            byte[] dll = dllAsset.bytes;
            Log.Info("hotfix dll加载完毕");

#if  UNITY_EDITOR
            TextAsset pdbAsset = await Init.Resource.AwaitLoadAsset<TextAsset>(AssetUtility.GetHotfixDLLAsset("Hotfix.pdb"));
            byte[] pdb = pdbAsset.bytes;
            Log.Info("hotfix pdb加载完毕");

            AppDomain.LoadAssembly(new MemoryStream(dll), new MemoryStream(pdb), new Mono.Cecil.Pdb.PdbReaderProvider());

            //启动调试服务器
            AppDomain.DebugService.StartDebugService(56000);
            //设置Unity主线程ID 这样就可以用Profiler看性能消耗了
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#else
            AppDomain.LoadAssembly(new MemoryStream(dll));
#endif
            Init.Instance.StartCoroutine(HotfixStart());
        }

        public void Update()
        {
            if (!OpenHotfixUpdate) return;
            if (IsILRuntime)
            {
                AppDomain.Invoke(hUpdate, null, null);
                return;
            }
#if UNITY_EDITOR
            eUpdate.Invoke(null, null);
#endif
        }

        public void LateUpdate()
        {
            if (!OpenHotfixUpdate) return;
            if (IsILRuntime)
            {
                AppDomain.Invoke(hLateUpdate, null, null);
                return;
            }
#if UNITY_EDITOR
            eLateUpdate.Invoke(null, null);
#endif
        }

        public void OnDestroy()
        {
            if (IsILRuntime)
            {
                AppDomain.Invoke(GameEntryStr, ShutdownStr, null, null);
                return;
            }
#if UNITY_EDITOR
            Type type = editorAssembly.GetType(GameEntryStr);
            var method = type.GetMethod(ShutdownStr, BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, null);
#endif
        }

        /// <summary>
        /// 获取热更新层类的Type对象
        /// </summary>
        public Type GetHotfixType(string hotfixTypeFullName)
        {
            return AppDomain.LoadedTypes[hotfixTypeFullName].ReflectionType;
        }

        /// <summary>
        /// 获取所有热更新层类的Type对象
        /// </summary>
        public List<Type> GetHotfixTypes
        {
            get
            {
                if (Types == null) Types = AppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToList();
                return Types;
            }
        }

        /// <summary>
        /// 开始执行热更新层代码
        /// </summary>
        private IEnumerator HotfixStart()
        {
            yield return null;
            IType type = AppDomain.LoadedTypes[GameEntryStr];

            AppDomain.Invoke(GameEntryStr, AwakeStr, null, null);
            AppDomain.Invoke(GameEntryStr, StartStr, null, null);
            hUpdate = type.GetMethod(UpdateStr, 0);
            hLateUpdate = type.GetMethod(LateUpdateStr, 0);
        }

#if UNITY_EDITOR
        private IEnumerator EditorHotfixStart()
        {
            editorAssembly = Assembly.Load(HotfixStr);
            Types = editorAssembly.GetTypes().ToList();
            Type type = editorAssembly.GetType(GameEntryStr);
            var method = type.GetMethod(AwakeStr, BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, null);
            yield return new WaitForEndOfFrame();
            method = type.GetMethod(StartStr, BindingFlags.Public | BindingFlags.Static);
            method.Invoke(null, null);

            eUpdate = type.GetMethod(UpdateStr, BindingFlags.Public | BindingFlags.Static);
            eLateUpdate = type.GetMethod(LateUpdateStr, BindingFlags.Public | BindingFlags.Static);
        }
#endif
    }
}
