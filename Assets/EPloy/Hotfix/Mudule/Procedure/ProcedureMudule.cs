
using EPloy.Fsm;
using System;

namespace EPloy
{
    /// <summary>
    /// 流程管理器。
    /// </summary>
    public sealed class ProcedureMudule : IHotfixModule
    {
        private IFsm ProcedureFsm;

        /// <summary>
        /// 获取当前流程。
        /// </summary>
        public ProcedureBase CurrentProcedure
        {
            get
            {
                if (ProcedureFsm == null)
                {
                    Log.Error("You must initialize procedure first.");
                    return null;
                }

                return (ProcedureBase) ProcedureFsm.CurrentState;
            }
        }

        /// <summary>
        /// 获取当前流程持续时间。
        /// </summary>
        public float CurrentProcedureTime
        {
            get
            {
                if (ProcedureFsm == null)
                {
                    Log.Error("You must initialize procedure first.");
                    return 0;
                }

                return ProcedureFsm.CurrentStateTime;
            }
        }

        /// <summary>
        /// 初始化流程管理器。
        /// </summary>
        public void Awake()
        {
            ProcedureBase[] procedures = new ProcedureBase[4];
            procedures[0] = ReferencePool.Acquire<ProcedurePreload>();
            procedures[1] = ReferencePool.Acquire<ProcedureLogin>();
            procedures[2] = ReferencePool.Acquire<ProcedureSwitchScene>();
            procedures[3] = ReferencePool.Acquire<ProcedureMap>();
            ProcedureFsm = HotFixMudule.Fsm.CreateFsm(this, procedures);
        }

        public void Update()
        {
        }

        /// <summary>
        /// 关闭并清理流程管理器。
        /// </summary>
        public void OnDestroy()
        {
            if (ProcedureFsm != null)
            {
                HotFixMudule.Fsm.DestroyFsm(ProcedureFsm);
                ProcedureFsm = null;
            }

        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <typeparam name="T">要开始的流程类型。</typeparam>
        public void StartProcedure<T>() where T : ProcedureBase
        {
            if (ProcedureFsm == null)
            {
                Log.Error("You must initialize procedure first.");
                return;
            }

            ProcedureFsm.Start<T>();
        }

        /// <summary>
        /// 开始流程。
        /// </summary>
        /// <param name="procedureType">要开始的流程类型。</param>
        public void StartProcedure(Type procedureType)
        {
            if (ProcedureFsm == null)
            {
                Log.Error("You must initialize procedure first.");
                return;
            }

            ProcedureFsm.Start(procedureType);
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <typeparam name="T">要检查的流程类型。</typeparam>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure<T>() where T : ProcedureBase
        {
            if (ProcedureFsm == null)
            {
                Log.Error("You must initialize procedure first.");
                return false;
            }

            return ProcedureFsm.HasState<T>();
        }

        /// <summary>
        /// 是否存在流程。
        /// </summary>
        /// <param name="procedureType">要检查的流程类型。</param>
        /// <returns>是否存在流程。</returns>
        public bool HasProcedure(Type procedureType)
        {
            if (ProcedureFsm == null)
            {
                Log.Error("You must initialize procedure first.");
                return false;
            }

            return ProcedureFsm.HasState(procedureType);
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <typeparam name="T">要获取的流程类型。</typeparam>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure<T>() where T : ProcedureBase
        {
            if (ProcedureFsm == null)
            {
                Log.Error("You must initialize procedure first.");
                return null;
            }

            return ProcedureFsm.GetState<T>();
        }

        /// <summary>
        /// 获取流程。
        /// </summary>
        /// <param name="procedureType">要获取的流程类型。</param>
        /// <returns>要获取的流程。</returns>
        public ProcedureBase GetProcedure(Type procedureType)
        {
            if (ProcedureFsm == null)
            {
                Log.Error("You must initialize procedure first.");
                return null;
            }

            return (ProcedureBase) ProcedureFsm.GetState(procedureType);
        }
    }
}
