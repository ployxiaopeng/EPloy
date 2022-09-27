﻿
using EPloy.Fsm;
using System;

/// <summary>
/// 流程管理器。
/// </summary>
public sealed class ProcedureModule : IGameModule
{
    private IFsm ProcedureFsm;

    /// <summary>
    /// 获取当前流程。
    /// </summary>
    public FsmState CurrentProcedure
    {
        get
        {
            if (ProcedureFsm == null)
            {
                Log.Error("You must initialize procedure first.");
                return null;
            }

            return ProcedureFsm.CurrentState;
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
            GameModule.Fsm.DestroyFsm(ProcedureFsm);
            ProcedureFsm = null;
        }

    }

    /// <summary>
    /// 必须继承ProcedureBase 否则报错
    /// </summary>
    /// <param name="procedures"></param>
    public void RegisterProcedure(params string[] procedures)
    {
        FsmState[] contents = new FsmState[procedures.Length];
        for (int i = 0; i < procedures.Length; i++)
        {
            Type type = Type.GetType(procedures[i]);
            contents[i] = (FsmState)ReferencePool.Acquire(type);
        }
        ProcedureFsm = GameModule.Fsm.CreateFsm(this, contents);
    }

    /// <summary>
    /// 开始流程。
    /// </summary>
    /// <typeparam name="T">要开始的流程类型。</typeparam>
    public void StartProcedure<T>() where T : FsmState
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
    public bool HasProcedure<T>() where T : FsmState
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
    public FsmState GetProcedure<T>() where T : FsmState
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
    public FsmState GetProcedure(Type procedureType)
    {
        if (ProcedureFsm == null)
        {
            Log.Error("You must initialize procedure first.");
            return null;
        }

        return ProcedureFsm.GetState(procedureType);
    }
}