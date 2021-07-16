using UnityEngine;
using System;
using EPloy.Res;
using EPloy.Download;

namespace EPloy
{
    public class Game : MonoBehaviour
    {
        public static Game Instance = null;
        [SerializeField]
        private bool IsILRuntime = false;
        [SerializeField]
        private bool EditorResource = true;

        public static FileSystemModule FileSystem
        {
            get;
            private set;
        }

        public static DownLoadModule DownLoad
        {
            get;
            private set;
        }

        public static ResUpdaterModule ResUpdater
        {
            get;
            private set;
        }

        public static VersionCheckerModule VersionChecker
        {
            get;
            private set;
        }

        public static ILRuntimeModule ILRuntime
        {
            get;
            private set;
        }

        public static ProcedureModule Procedure
        {
            get;
            private set;
        }
        public static TimerModule Timer
        {
            get;
            private set;
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Timer = EPloyModuleMgr.CreateModule<TimerModule>();
            FileSystem = EPloyModuleMgr.CreateModule<FileSystemModule>();
            VersionChecker = EPloyModuleMgr.CreateModule<VersionCheckerModule>();
            DownLoad = EPloyModuleMgr.CreateModule<DownLoadModule>();
            ResUpdater = EPloyModuleMgr.CreateModule<ResUpdaterModule>();
            ILRuntime = EPloyModuleMgr.CreateModule<ILRuntimeModule>();
            Procedure = EPloyModuleMgr.CreateModule<ProcedureModule>();

            Procedure.StartGame(IsILRuntime, EditorResource);
        }

        private void Update()
        {
            EPloyModuleMgr.ModuleUpdate();
        }

        private void OnDestroy()
        {
            EPloyModuleMgr.ModuleDestory();
        }
    }
}