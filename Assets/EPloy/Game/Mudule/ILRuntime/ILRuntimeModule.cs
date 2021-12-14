using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using PdbReaderProvider = ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider;

namespace EPloy
{
    /// <summary>
    /// ILRuntime
    /// </summary>
    public class ILRuntimeModule : IGameModule
    {
        private const string HotfixName = "EPloy.Hotfix";
        private const string GameStartStr = "EPloy.HotFixStart";
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
        private Type[] Types;
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
        public AppDomain AppDomain { get; private set; }

        public void Awake()
        {

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
        /// 初始化 ILRuntime
        /// </summary>
        public void StartILRuntime(bool isILRuntime)
        {
            OpenHotfixUpdate = false;
            IsILRuntime = isILRuntime;
            if (!IsILRuntime)
            {
#if UNITY_EDITOR
                Debug.Log("当前为 LRuntime Editor模式");
                GameStart.Instance.StartCoroutine(EditorHotfixStart());
#else
                Debug.LogError("移动平台只有ILRuntime 模式不开会报错");
#endif
                return;
            }

            AppDomain = new AppDomain();
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
            if (!IsILRuntime)
            {
                Debug.LogError("ERR: is not Editor module in mobile platform");
                yield break;
                ;
            }

            # region 加载  Hotfix.dll

            bool isError = false;
            UnityWebRequest hotfixDllRequest = UnityWebRequest.Get(GetHotfixAsset("Hotfix.dll"));
            yield return hotfixDllRequest.SendWebRequest();
            isError = hotfixDllRequest.isNetworkError || hotfixDllRequest.isHttpError;
            if (isError)
            {
                Debug.LogError("load hotfixDll err : " + hotfixDllRequest.error);
                yield break;
                ;
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
                ;
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
            IType type = AppDomain.LoadedTypes[GameStartStr];

            AppDomain.Invoke(GameStartStr, AwakeStr, null, new[] {GameStart.Instance});
            AppDomain.Invoke(GameStartStr, StartStr, null, null);
            hUpdate = type.GetMethod(UpdateStr, 0);
            hLateUpdate = type.GetMethod(LateUpdateStr, 0);
            OpenHotfixUpdate = true;
        }

#if UNITY_EDITOR
        private IEnumerator EditorHotfixStart()
        {
            editorAssembly = Assembly.Load(HotfixName);
            Types = editorAssembly.GetTypes().ToArray();
            Type type = editorAssembly.GetType(GameStartStr);
            MethodInfo awake = type.GetMethod(AwakeStr, BindingFlags.Public | BindingFlags.Static);
            awake.Invoke(null, new[] {GameStart.Instance});
            yield return new WaitForEndOfFrame();
            MethodInfo start = type.GetMethod(StartStr, BindingFlags.Public | BindingFlags.Static);
            start.Invoke(null, null);

            eUpdate = type.GetMethod(UpdateStr, BindingFlags.Public | BindingFlags.Static);
            eLateUpdate = type.GetMethod(LateUpdateStr, BindingFlags.Public | BindingFlags.Static);
            OpenHotfixUpdate = true;
        }
#endif

        private string GetHotfixAsset(string name)
        {
#if UNITY_EDITOR
            return string.Format("{0}/{1}.bytes", Application.streamingAssetsPath, name);
#else
            return string.Format("{0}/{1}.bytes", Application.persistentDataPath, name);
#endif
        }
    }
}
