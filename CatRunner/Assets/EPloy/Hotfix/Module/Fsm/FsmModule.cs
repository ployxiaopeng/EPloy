using EPloy.Fsm;
using System;
using System.Collections.Generic;

/// <summary>
/// 有限状态机管理器。
/// </summary>
public sealed class FsmModule : IGameModule
{
    private Dictionary<string, FsmBase> Fsms;

    /// <summary>
    /// 获取有限状态机数量。
    /// </summary>
    public int Count
    {
        get
        {
            return Fsms.Count;
        }
    }

    /// <summary>
    /// 初始化有限状态机管理器
    /// </summary>
    public void Awake()
    {
        Fsms = new Dictionary<string, FsmBase>();
    }

    public void Update()
    {
        foreach (KeyValuePair<string, FsmBase> fsm in Fsms)
        {
            fsm.Value.Update();
        }

    }

    /// <summary>
    /// 关闭并清理有限状态机管理器。
    /// </summary>
    public void OnDestroy()
    {
        foreach (KeyValuePair<string, FsmBase> fsm in Fsms)
        {
            ReferencePool.Release(fsm.Value);
        }

        Fsms.Clear();
    }

    /// <summary>
    /// 检查是否存在有限状态机。
    /// </summary>
    /// <param name="ownerType">有限状态机持有者类型。</param>
    /// <returns>是否存在有限状态机。</returns>
    public bool HasFsm(Type ownerType)
    {
        if (ownerType == null)
        {
            Log.Error("Owner type is invalid.");
            return false;
        }

        return InternalHasFsm(GetFullName(ownerType, string.Empty));
    }

    /// <summary>
    /// 检查是否存在有限状态机。
    /// </summary>
    /// <param name="ownerType">有限状态机持有者类型。</param>
    /// <param name="name">有限状态机名称。</param>
    /// <returns>是否存在有限状态机。</returns>
    public bool HasFsm(Type ownerType, string name)
    {
        if (ownerType == null)
        {
            Log.Error("Owner type is invalid.");
            return false;
        }

        return InternalHasFsm(GetFullName(ownerType, name));
    }

    /// <summary>
    /// 获取有限状态机。
    /// </summary>
    /// <param name="ownerType">有限状态机持有者类型。</param>
    /// <returns>要获取的有限状态机。</returns>
    public FsmBase GetFsm(Type ownerType)
    {
        if (ownerType == null)
        {
            Log.Error("Owner type is invalid.");
            return null;
        }

        return InternalGetFsm(GetFullName(ownerType, string.Empty));
    }
    /// <summary>
    /// 获取有限状态机。
    /// </summary>
    /// <param name="ownerType">有限状态机持有者类型。</param>
    /// <param name="name">有限状态机名称。</param>
    /// <returns>要获取的有限状态机。</returns>
    public FsmBase GetFsm(Type ownerType, string name)
    {
        if (ownerType == null)
        {
            Log.Error("Owner type is invalid.");
            return null;
        }
        return InternalGetFsm(GetFullName(ownerType, name));
    }

    /// <summary>
    /// 获取所有有限状态机。
    /// </summary>
    /// <returns>所有有限状态机。</returns>
    public FsmBase[] GetAllFsms()
    {
        int index = 0;
        FsmBase[] results = new FsmBase[Fsms.Count];
        foreach (KeyValuePair<string, FsmBase> fsm in Fsms)
        {
            results[index++] = fsm.Value;
        }

        return results;
    }

    /// <summary>
    /// 获取所有有限状态机。
    /// </summary>
    /// <param name="results">所有有限状态机。</param>
    public void GetAllFsms(List<FsmBase> results)
    {
        if (results == null)
        {
            Log.Error("Results is invalid.");
            return;
        }

        results.Clear();
        foreach (KeyValuePair<string, FsmBase> fsm in Fsms)
        {
            results.Add(fsm.Value);
        }
    }

    /// <summary>
    /// 创建有限状态机。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    /// <param name="owner">有限状态机持有者。</param>
    /// <param name="states">有限状态机状态集合。</param>
    /// <returns>要创建的有限状态机。</returns>
    public IFsm CreateFsm(object owner, params FsmState[] states)
    {
        return CreateFsm(string.Empty, owner, states);
    }

    /// <summary>
    /// 创建有限状态机。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    /// <param name="name">有限状态机名称。</param>
    /// <param name="owner">有限状态机持有者。</param>
    /// <param name="states">有限状态机状态集合。</param>
    /// <returns>要创建的有限状态机。</returns>
    public IFsm CreateFsm(string name, object owner, params FsmState[] states)
    {
        if (HasFsm(owner.GetType(), name))
        {
            Log.Error(UtilText.Format("Already exist FSM '{0}: {1}'.", owner.GetType(), name));
            return null;
        }

        LimitFsm fsm = ReferencePool.Acquire<LimitFsm>();
        fsm.InitLimitFsm(name, owner, states);
        Fsms.Add(GetFullName(owner.GetType(), name), fsm);
        return fsm;
    }

    /// <summary>
    /// 销毁有限状态机。
    /// </summary>
    /// <param name="ownerType">有限状态机持有者类型。</param>
    /// <returns>是否销毁有限状态机成功。</returns>
    public bool DestroyFsm(Type ownerType)
    {
        if (ownerType == null)
        {
            Log.Error("Owner type is invalid.");
            return false;
        }

        return InternalDestroyFsm(GetFullName(ownerType, string.Empty));
    }

    /// <summary>
    /// 销毁有限状态机。
    /// </summary>
    /// <param name="ownerType">有限状态机持有者类型。</param>
    /// <param name="name">要销毁的有限状态机名称。</param>
    /// <returns>是否销毁有限状态机成功。</returns>
    public bool DestroyFsm(Type ownerType, string name)
    {
        if (ownerType == null)
        {
            Log.Error("Owner type is invalid.");
            return false;
        }

        return InternalDestroyFsm(GetFullName(ownerType, name));
    }

    /// <summary>
    /// 销毁有限状态机。
    /// </summary>
    /// <typeparam name="T">有限状态机持有者类型。</typeparam>
    /// <param name="fsm">要销毁的有限状态机。</param>
    /// <returns>是否销毁有限状态机成功。</returns>
    public bool DestroyFsm(IFsm fsm)
    {
        if (fsm == null)
        {
            Log.Error("FSM is invalid.");
            return false;
        }
        return InternalDestroyFsm(GetFullName(fsm.Owner.GetType(), fsm.Name));
    }

    /// <summary>
    /// 销毁有限状态机。
    /// </summary>
    /// <param name="fsm">要销毁的有限状态机。</param>
    /// <returns>是否销毁有限状态机成功。</returns>
    public bool DestroyFsm(FsmBase fsm)
    {
        if (fsm == null)
        {
            Log.Error("FSM is invalid.");
            return false;
        }

        return InternalDestroyFsm(GetFullName(fsm.OwnerType, fsm.Name));
    }

    public bool InternalHasFsm(string fullName)
    {
        return Fsms.ContainsKey(fullName);
    }
    public FsmBase InternalGetFsm(string fullName)
    {
        FsmBase fsm = null;
        if (Fsms.TryGetValue(fullName, out fsm))
        {
            return fsm;
        }

        return null;
    }
    public bool InternalDestroyFsm(string fullName)
    {
        FsmBase fsm = null;
        if (Fsms.TryGetValue(fullName, out fsm))
        {
            ReferencePool.Release(fsm);
            return Fsms.Remove(fullName);
        }
        return false;
    }

    private string GetFullName(Type type, string name)
    {
        return UtilText.Format("{0}_{1}", type.ToString(), name);
    }

}