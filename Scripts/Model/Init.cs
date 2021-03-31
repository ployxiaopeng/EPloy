using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using DG.Tweening;

namespace ETModel
{
    public class Init : MonoBehaviour
    {
        [SerializeField]
        private bool isILRuntime;
        [SerializeField]
        private bool EditorResource;
        [SerializeField]
        private CanvasGroup StartImg;

        public static Init Instance = null;

        #region 周期函数
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }
        private void Start()
        {
            AddBaseComponen().Coroutine();
            StartImg.DOFade(0, 2f).OnComplete(() =>
             {
                 StartImg.gameObject.SetActive(false);
             });
        }
        private void Update()
        {
            OneThreadSynchronizationContext.Instance.Update();
            ModelGame.EventSystem.Update();
        }
        private void LateUpdate()
        {
            ModelGame.EventSystem.LateUpdate();
        }
        private void OnApplicationQuit()
        {
            ModelGame.Close();
        }
        #endregion

        public static TimerComponent Timer
        {
            get;
            private set;
        }
        public static ILRuntimeComponent ILRuntime
        {
            get;
            private set;
        }
        public static BaseComponent Base
        {
            get;
            private set;
        }
        public static ConfigComponent Config
        {
            get;
            private set;
        }
        public static ResourceComponent Resource
        {
            get;
            private set;
        }
        public static EventComponent Event
        {
            get;
            private set;
        }
        public static ProcedureComponent Procedure
        {
            get;
            private set;
        }
        public static FsmComponent Fsm
        {
            get;
            private set;
        }
        public static ObjectPoolComponent ObjectPool
        {
            get;
            private set;
        }
        public static UIComponent UI
        {
            get;
            private set;
        }
        public static DataTableComponent DataTable
        {
            get;
            private set;
        }
        public static EntityComponent Entity
        {
            get;
            private set;
        }
        public static SettingComponent Setting
        {
            get;
            private set;
        }
        public static SceneComponent Scene
        {
            get;
            private set;
        }
        public static SoundComponent Sound
        {
            get;
            private set;
        }
        public static NetworkComponent Network
        {
            get;
            private set;
        }
        public static WebRequestComponent WebRequest
        {
            get;
            private set;
        }
        public static DownloadComponent Download
        {
            get;
            private set;
        }
        public static ExtensionComponent Extension
        {
            get;
            private set;
        }

        private async ETVoid AddBaseComponen()
        {
            try
            {
                SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
                ModelGame.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);
                //添加最基本模块组件
                Timer = ModelGame.Scene.AddComponent<TimerComponent>();
                Base = ModelGame.Scene.AddComponent<BaseComponent, bool>(EditorResource);
                Resource = ModelGame.Scene.AddComponent<ResourceComponent>();
                ILRuntime = ModelGame.Scene.AddComponent<ILRuntimeComponent, bool>(isILRuntime);
                ObjectPool = ModelGame.Scene.AddComponent<ObjectPoolComponent>();
                Event = ModelGame.Scene.AddComponent<EventComponent>();
                Config = ModelGame.Scene.AddComponent<ConfigComponent>();
                Fsm = ModelGame.Scene.AddComponent<FsmComponent>();
                UI = ModelGame.Scene.AddComponent<UIComponent>();
                Entity = ModelGame.Scene.AddComponent<EntityComponent>();
                DataTable = ModelGame.Scene.AddComponent<DataTableComponent>();
                Scene = ModelGame.Scene.AddComponent<SceneComponent>();
                Procedure = ModelGame.Scene.AddComponent<ProcedureComponent>();
                Setting = ModelGame.Scene.AddComponent<SettingComponent>();
                Sound = ModelGame.Scene.AddComponent<SoundComponent>();
                Network = ModelGame.Scene.AddComponent<NetworkComponent>();
                WebRequest = ModelGame.Scene.AddComponent<WebRequestComponent>();
                Download = ModelGame.Scene.AddComponent<DownloadComponent>();
                Extension = ModelGame.Scene.AddComponent<ExtensionComponent>();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}