using UnityEngine;
using System;
using EPloy.Res;

namespace EPloy
{
    public class Game : MonoBehaviour
    {
        public static Game instance = null;
        [SerializeField]
        private bool isILRuntime;
        public bool EditorResource;

        private static ResUpdaterModule ResUpdater;

        public static FileSystemModule FileSystem
        {
            get;
            private set;
        }

        public static ILRuntimeModule ILRuntime
        {
            get;
            private set;
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            FileSystem = EPloyModuleMgr.CreateModule<FileSystemModule>();
            ResUpdater = EPloyModuleMgr.CreateModule<ResUpdaterModule>();
            ILRuntime = EPloyModuleMgr.CreateModule<ILRuntimeModule>();
            ResUpdater.CheckRes(CheckResCallback);


        }

        private void CheckResCallback(bool result, string msg)
        {
            if (result)
            {
                ILRuntime.StartGame(isILRuntime);
                return;
            }
            Log.Fatal(msg);
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