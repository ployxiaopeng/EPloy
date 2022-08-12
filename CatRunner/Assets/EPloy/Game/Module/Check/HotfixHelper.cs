using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace EPloy.Checker
{
    /// <summary>
    /// 热更dll 辅助
    /// </summary>
    public static class HotfixHelper
    {
        private const string HotfixName = "EPloy.Hotfix";
        private const string GameStartStr = "HotFixStart";
        private const string AwakeStr = "Awake";
        private const string StartStr = "Start";
        private const string UpdateStr = "Update";
        private const string OnDestroyStr = "OnDestroy";

        public static Assembly hotfixAssembly { get; private set; }

        private static MethodInfo HotfixAwake;
        private static MethodInfo HotfixStart;

        /// <summary>
        /// 获取所有热更新层类的Type对象
        /// </summary>
        public static Type[] GetHotfixTypes
        {
            get
            {
                return hotfixAssembly.GetTypes();
            }
        }

        /// <summary>
        /// 加载dll
        /// </summary>
        public static void StartLoadDll()
        {
            switch (GameEntry.GameModel)
            {
                case GameModel.Editor:
                    hotfixAssembly = AppDomain.CurrentDomain.GetAssemblies().First(assembly => assembly.GetName().Name == HotfixName);
                    Start();
                    break;
                default:
                    GameStart.Instance.StartCoroutine(LoadHotfixDll());
                    break;
            }
        }

        private static IEnumerator LoadHotfixDll()
        {
            UnityWebRequest hotfixDllRequest = UnityWebRequest.Get(GetHotfixAsset("Hotfix.dll"));
            yield return hotfixDllRequest.SendWebRequest();
            if (hotfixDllRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                Log.Fatal("load hotfixDll err : " + hotfixDllRequest.error);
                yield break;
            }

            byte[] hotfixDll = hotfixDllRequest.downloadHandler.data;
            hotfixDllRequest.Dispose();
            hotfixAssembly = Assembly.Load(hotfixDll);
            Debug.Log("hotfix dll加载完毕");
            Start();
        }

        private static void Start()
        {
            Type type = hotfixAssembly.GetType(GameStartStr);
            HotfixAwake = type.GetMethod(AwakeStr, BindingFlags.Public | BindingFlags.Static);
            HotfixStart = type.GetMethod(StartStr, BindingFlags.Public | BindingFlags.Static);

            HotfixAwake.Invoke(null, new[] { GameStart.Instance });
            HotfixStart.Invoke(null, null);

        }

        private static string GetHotfixAsset(string name)
        {
            return string.Format("{0}/{1}.bytes", GameEntry.ResPath, name);
        }
    }
}
