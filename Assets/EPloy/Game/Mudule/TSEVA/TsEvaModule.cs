using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Reflection;
using Puerts;
using UnityEngine;
using UnityEngine.Networking;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using PdbReaderProvider = ILRuntime.Mono.Cecil.Pdb.PdbReaderProvider;

namespace EPloy
{
    /// <summary>
    /// ILRuntime
    /// </summary>
    public class TsEvaModule : IGameModule
    {
        /// <summary>
        /// js 环境
        /// </summary>
        private JsEnv jsEnv;

        /// <summary>
        /// 是否开启热更层的轮转
        /// </summary>
        public bool OpenHotfixUpdate { get; set; }


        public void Awake()
        {
            jsEnv = new JsEnv();
        }

        public void Update()
        {

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
        public void StartILRuntime(bool isILRuntime)
        {

        }

    }
}
