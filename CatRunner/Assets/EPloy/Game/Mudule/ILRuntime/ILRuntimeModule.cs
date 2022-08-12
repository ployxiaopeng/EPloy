using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using PdbReaderProvider = ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider;
using System.Collections.Generic;

namespace EPloy.Game
{
    /// <summary>
    /// ILRuntime
    /// </summary>
    public class ILRuntimeModule : IGameModule
    {
        private const string HotfixName = "EPloy.Hotfix";
        private const string GameStartStr = "EPloy.Hotfix.HotFixStart";
        private const string AwakeStr = "Awake";
        private const string StartStr = "Start";
        private const string OnDestroyStr = "OnDestroy";
        private Type[] Types;

        /// <summary>
        /// 非ILRuntime模式下使用
        /// </summary>
        private Assembly editorAssembly;
        /// <summary>
        /// 是否启用ILRuntime
        /// </summary>
        private bool isILRuntime
        {
            get
            {
                return GameModule.GameModel != GameModel.Editor;
            }
        }
        /// <summary>
        /// ILRuntime入口对象
        /// </summary>
        public AppDomain AppDomain { get; private set; }
        //轮转需要单独优化
        public Action HotfixUpdate { get; set; }

        public void Awake()
        {
            HotfixUpdate = null;
        }

        public void Update()
        {
            HotfixUpdate?.Invoke();
        }

        public void OnDestroy()
        {
            if (isILRuntime)
            {
                AppDomain.Invoke(GameStartStr, OnDestroyStr, null, null);
            }
            else
            {
                Type type = editorAssembly.GetType(GameStartStr);
                var method = type.GetMethod(OnDestroyStr, BindingFlags.Public | BindingFlags.Static);
                method.Invoke(null, null);
            }
            HotfixUpdate = null;
        }

        /// <summary>
        /// 初始化 ILRuntime
        /// </summary>
        public void StartILRuntime()
        {
            if (!isILRuntime)
            {
                GameStart.Instance.StartCoroutine(EditorHotfixStart());
                return;
            }
            //升级到寄存器版本 暂采用 ILRuntime 自己选择的方式
            AppDomain = new AppDomain(ILRuntime.Runtime.ILRuntimeJITFlags.JITOnDemand);
            ILRuntimeHelper.InitILRuntime(AppDomain);
            GameStart.Instance.StartCoroutine(LoadHotfixDll());
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
        public Type[] GetHotfixTypes
        {
            get
            {
                if (Types == null) Types = AppDomain.LoadedTypes.Values.Select(x => x.ReflectionType).ToArray();
                return Types;
            }
        }

        private IEnumerator LoadHotfixDll()
        {
            # region 加载  Hotfix.dll
            bool isError = false;
            UnityWebRequest hotfixDllRequest = UnityWebRequest.Get(GetHotfixAsset("Hotfix.dll"));
            yield return hotfixDllRequest.SendWebRequest();
            isError = hotfixDllRequest.isNetworkError || hotfixDllRequest.isHttpError;
            if (isError)
            {
                Debug.LogError("load hotfixDll err : " + hotfixDllRequest.error);
                yield break;
            }

            byte[] hotfixDll = hotfixDllRequest.downloadHandler.data;
            hotfixDllRequest.Dispose();
            Debug.Log("hotfix dll加载完毕");

            #endregion

            #region 加载  Hotfix.pdb

#if UNITY_EDITOR
            UnityWebRequest pdbRequest = UnityWebRequest.Get(GetHotfixAsset("Hotfix.pdb"));
            yield return pdbRequest.SendWebRequest();
            isError = pdbRequest.isNetworkError || pdbRequest.isHttpError;
            if (isError)
            {
                Debug.LogError("load hotfix pdb err : " + pdbRequest.error);
                yield break;
            }

            byte[] hotfixPbd = pdbRequest.downloadHandler.data;
            pdbRequest.Dispose();
            Debug.Log("hotfix pdb加载完毕");

            AppDomain.LoadAssembly(new MemoryStream(hotfixDll), new MemoryStream(hotfixPbd), new PdbReaderProvider());
            //启动调试服务器
            AppDomain.DebugService.StartDebugService(56000);
            //设置Unity主线程ID 这样就可以用Profiler看性能消耗了
            AppDomain.UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            //AppDomain.LoadAssembly(new MemoryStream(hotfixDll));
#else
            AppDomain.LoadAssembly(new MemoryStream(hotfixDll));
#endif

            #endregion

            GameStart.Instance.StartCoroutine(HotfixStart());
        }

        /// <summary>
        /// 开始执行热更新层代码
        /// </summary>
        private IEnumerator HotfixStart()
        {
            yield return null;
            AppDomain.Invoke(GameStartStr, AwakeStr, null, new[] { GameStart.Instance });
            AppDomain.Invoke(GameStartStr, StartStr, null, null);
        }

        private IEnumerator EditorHotfixStart()
        {
            editorAssembly = Assembly.Load(HotfixName);
            Types = editorAssembly.GetTypes().ToArray();
            Type type = editorAssembly.GetType(GameStartStr);
            MethodInfo awake = type.GetMethod(AwakeStr, BindingFlags.Public | BindingFlags.Static);
            awake.Invoke(null, new[] { GameStart.Instance });
            yield return new WaitForEndOfFrame();
            MethodInfo start = type.GetMethod(StartStr, BindingFlags.Public | BindingFlags.Static);
            start.Invoke(null, null);
        }

        private string GetHotfixAsset(string name)
        {
            return string.Format("{0}/{1}.bytes", GameModule.ResPath, name);
        }
    }
}

