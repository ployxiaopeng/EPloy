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

namespace EPloy
{
    /// <summary>
    /// ILRuntime
    /// </summary>
    public class ILRuntimeMgr
    {
        private static ILRuntimeMgr instance = null;

        /// <summary>
        /// ILRuntime获取
        /// </summary>
        public static ILRuntimeMgr CreateILRuntimeMgr(bool isILRuntime)
        {
            if (instance == null) instance = new ILRuntimeMgr();
            instance.IsILRuntime = isILRuntime;
            return instance;
        }

        // public static string HotfixDLLAsset = string.Format("Assets/Res/HotfixDLL/{0}.bytes");
        private const string HotfixName = "EPloy";
        private const string GameStartStr = "EPloy.GameStart";
        private const string AwakeStr = "Awake";
        private const string StartStr = "Start";
        private const string UpdateStr = "Update";
        private const string LateUpdateStr = "LateUpdate";
        private const string FixUpdateStr = "FixUpdate";
        private const string OnDestroyStr = "OnDestroy";

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

        /// <summary>
        /// 初始化 ILRuntime
        /// </summary>
        public void ILRuntimeInit()
        {
            OpenHotfixUpdate = false;
            if (!IsILRuntime)
            {
#if UNITY_EDITOR
                Debug.Log("当前为 LRuntime Editor模式");
                Init.instance.StartCoroutine(EditorHotfixStart());
#else
                Debug.LogError("移动平台只有ILRuntime 模式不开会报错");
#endif
                return;
            }
            AppDomain = new AppDomain();
            ILRuntimeHelper.InitILRuntime(AppDomain);
            StartLoad();
        }

        /// <summary>
        /// 加载热更新DLL
        /// </summary>
        private void StartLoad()
        {
            if (!IsILRuntime)
            {
                Debug.LogError("ERR: is not Editor module in mobile platform");
                return;
            }
            // TextAsset dllAsset = await Init.Resource.AwaitLoadAsset<TextAsset>(AssetUtility.GetHotfixDLLAsset("Hotfix.dll"));
            // byte[] dll = dllAsset.bytes;
            // Debug.Log("hotfix dll加载完毕");

#if  UNITY_EDITOR
            // TextAsset pdbAsset = await Init.Resource.AwaitLoadAsset<TextAsset>(AssetUtility.GetHotfixDLLAsset("Hotfix.pdb"));
            // byte[] pdb = pdbAsset.bytes;
            // Debug.Log("hotfix pdb加载完毕");

            // AppDomain.LoadAssembly(new MemoryStream(dll), new MemoryStream(pdb), new Mono.Cecil.Pdb.PdbReaderProvider());

            //启动调试服务器
            AppDomain.DebugService.StartDebugService(56000);
            //设置Unity主线程ID 这样就可以用Profiler看性能消耗了
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#else
            AppDomain.LoadAssembly(new MemoryStream(dll));
#endif
            Init.instance.StartCoroutine(HotfixStart());
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
                AppDomain.Invoke(GameStartStr, OnDestroyStr, null, null);
                return;
            }
#if UNITY_EDITOR
            Type type = editorAssembly.GetType(GameStartStr);
            var method = type.GetMethod(OnDestroyStr, BindingFlags.Public | BindingFlags.Static);
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
            IType type = AppDomain.LoadedTypes[GameStartStr];

            AppDomain.Invoke(GameStartStr, AwakeStr, null, null);
            AppDomain.Invoke(GameStartStr, StartStr, null, null);
            hUpdate = type.GetMethod(UpdateStr, 0);
            hLateUpdate = type.GetMethod(LateUpdateStr, 0);
            instance.OpenHotfixUpdate = true;
        }

#if UNITY_EDITOR
        private IEnumerator EditorHotfixStart()
        {
            editorAssembly = Assembly.Load(HotfixName);
            Types = editorAssembly.GetTypes().ToList();
            Type type = editorAssembly.GetType(GameStartStr);
            MethodInfo awake = type.GetMethod(AwakeStr, BindingFlags.Public | BindingFlags.Static);
            awake.Invoke(null, new[] { Init.instance });
            yield return new WaitForEndOfFrame();
            MethodInfo start = type.GetMethod(StartStr, BindingFlags.Public | BindingFlags.Static);
            start.Invoke(null, null);

            eUpdate = type.GetMethod(UpdateStr, BindingFlags.Public | BindingFlags.Static);
            eLateUpdate = type.GetMethod(LateUpdateStr, BindingFlags.Public | BindingFlags.Static);
            instance.OpenHotfixUpdate = true;
        }
#endif
    }
}
