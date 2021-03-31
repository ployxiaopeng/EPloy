//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Fsm;
using GameFramework.Procedure;
using System;

namespace ETModel
{
    /// <summary>
    /// 流程组件。
    /// </summary>
    public  class ProcedureComponent : Component
    {
        public IProcedureManager ProcedureManager { get; set; }
        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                return (ProcedureBase)ProcedureManager.CurrentProcedure;
            }
        }
        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            return ProcedureManager.HasProcedure<T>();
        }
        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <returns>要获取的流程。</returns>
        /// <typeparam name="T">流程类型。</typeparam>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            return (ProcedureBase)ProcedureManager.GetProcedure<T>();
        }
    }
}
