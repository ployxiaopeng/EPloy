using ETModel;
using GameFramework.Procedure;
using System;
using System.Threading;
using UnityEngine;

namespace ETHotfix
{
    public static class GameEntry
    {
        #region 组件
        public static Transform Transform;
        public static TimerComponent Timer
        {
            get
            {
                return Init.Timer;
            }
        }
        public static BaseComponent Base
        {
            get
            {
                return Init.Base;
            }
        }
        public static ConfigComponent Config
        {
            get
            {
                return Init.Config;
            }
        }
        public static ResourceComponent Resource
        {
            get
            {
                return Init.Resource;
            }
        }
        public static ILRuntimeComponent ILRuntime
        {
            get
            {
                return Init.ILRuntime;
            }
        }
        public static EventComponent Event
        {
            get
            {
                return Init.Event;
            }
        }
        public static ProcedureComponent Procedure
        {
            get
            {
                return Init.Procedure;
            }
        }
        public static FsmComponent Fsm
        {
            get
            {
                return Init.Fsm;
            }
        }
        public static ObjectPoolComponent ObjectPool
        {
            get
            {
                return Init.ObjectPool;
            }
        }
        public static UIComponent UI
        {
            get
            {
                return Init.UI;
            }
        }
        public static DataTableComponent DataTable
        {
            get
            {
                return Init.DataTable;
            }
        }
        public static EntityComponent Entity
        {
            get
            {
                return Init.Entity;
            }
        }
        public static SettingComponent Setting
        {
            get
            {
                return Init.Setting;
            }
        }
        public static SoundComponent Sound
        {
            get
            {
                return Init.Sound;
            }
        }
        public static SceneComponent Scene
        {
            get
            {
                return Init.Scene;
            }
        }
        public static NetworkComponent Network
        {
            get
            {
                return Init.Network;
            }
        }
        public static WebRequestComponent WebRequest
        {
            get
            {
                return Init.WebRequest;
            }
        }

        public static ExtensionComponent Extension
        {
            get
            {
                return Init.Extension;
            }
        }
        #endregion

        public static void Awake()
        {
            try
            {
                Fsm.FsmManager.DestroyFsm<IProcedureManager>();
            }
            catch (Exception e)
            {
                Log.Error(string.Format("热更新层初始化一些必要组件ERR: {0}", e));
            }
        }
        public static void Start()
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
                //热更组件扩展和添加新的组件
                Extension.HotfixAwake().Coroutine();
                Network.HotfixStart();
                Entity.HotfixAwake().Coroutine();
                UI.HotfixAwake().Coroutine();
                //热更内流程进入
                Procedure.HotfixAwake();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void Update()
        {
            Extension.UpdateHotfixComponent();
        }

        public static void LateUpdate()
        {
            Extension.LateUpdateHotfixComponent();
        }
        public static void Shutdown()
        {

        }
    }
}