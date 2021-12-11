using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using Puerts;
using UnityEngine;
using UnityEngine.Networking;

namespace EPloy
{
    /// <summary>
    /// ILRuntime
    /// </summary>
    public class TsEvaModule : IGameModule
    {
        
        public bool WaitForDebugger = false;

        /// <summary>
        /// js 环境
        /// </summary>
        private JsEnv jsEnv;

        private ILoader loader;

        private string DebuggerRoot = Path.Combine(Application.streamingAssetsPath, "scripts");
        private int DebuggerPort = 5556;


        /// <summary>
        /// 是否开启热更层的轮转
        /// </summary>
        public bool OpenHotfixUpdate { get; set; }
        
        public void Awake()
        {
            loader = new TsLoader(DebuggerRoot);
            jsEnv = new JsEnv(loader, DebuggerPort);
            if (WaitForDebugger)
            {
                jsEnv.WaitDebugger();
            }

            RegisterClasses(jsEnv);
        }

        public void Update()
        {
            jsEnv.Eval("const HotFixStart = require('HotFixStart'); HotFixStart.Update ");
        }

        public void LateUpdate()
        {

        }

        public void OnDestroy()
        {

        }

        /// <summary>
        /// 初始化 ILRuntime
        /// </summary>
        public void StartILRuntime()
        {
            jsEnv.Eval("const HotFixStart = require('HotFixStart'); HotFixStart.Awake ");
        }

        private void RegisterClasses(JsEnv env)
        {
            env.UsingAction<int>();
            env.UsingAction<float>();
            env.UsingAction<string>();
            env.UsingAction<string, string>();
        }
    }
}
