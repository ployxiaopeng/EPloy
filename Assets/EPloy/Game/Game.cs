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

            // 暂定流程  还需要一个资源版本检查系统  通过之后加载 hotfixDLL
            // 我现在直接测试加载dll 

            ILRuntime.StartGame(isILRuntime);
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