using UnityEngine;
using System;

namespace EPloy
{
    public class Init : MonoBehaviour
    {
        public static Init instance = null;
        [SerializeField]
        private bool isILRuntime;
        public bool EditorResource;

        public Action HotfixStart;
        public Action HotfixUpdate;

        public static ILRuntimeMgr ILRuntimeMgr
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
            ILRuntimeMgr = ILRuntimeMgr.CreateILRuntimeMgr(isILRuntime);

            // 暂定流程  还需要一个资源版本检查系统  通过之后加载 hotfixDLL
            // 我现在直接测试加载dll 



            ILRuntimeMgr.ILRuntimeInit();
        }


        private void Update()
        {
            ILRuntimeMgr.Update();
        }
    }
}