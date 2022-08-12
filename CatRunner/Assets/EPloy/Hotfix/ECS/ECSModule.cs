using EPloy.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ECSModule 
{
    public static GameScene GameScene { get; private set; }

    public static void ECSActivate()
    {
        GameScene =GameModuleMgr.CreateModule<GameScene>();
        CreateSystem();
    }

    public static void ECSDisable()
    {
        GameModuleMgr.ModuleDestory<GameScene>();
        GameScene = null;
        DestroySystem();
    }

    public static CameraSystem cameraSys;
    public static MapSystem mapSys;
    public static MoveSystem moveSys;
    public static RoleSystem roleSys;
    public static AICommandSystem aICommandSys;
    public static RoleAcitonSystem roleAcitonSys;
    public static SkillSystem SkillSys;

    /// <summary>
    /// 创建系统
    /// </summary>
    /// <returns></returns>
    public static void CreateSystem()
    {
        cameraSys = ReferencePool.Acquire<CameraSystem>();
        mapSys = ReferencePool.Acquire<MapSystem>();
        moveSys = ReferencePool.Acquire<MoveSystem>();
        roleSys = ReferencePool.Acquire<RoleSystem>();
        aICommandSys = ReferencePool.Acquire<AICommandSystem>();
        roleAcitonSys = ReferencePool.Acquire<RoleAcitonSystem>();
        SkillSys = ReferencePool.Acquire<SkillSystem>();
    }

    public static void DestroySystem()
    {
        ReferencePool.Release(cameraSys);
        ReferencePool.Release(mapSys);
        ReferencePool.Release(moveSys);
        ReferencePool.Release(roleSys);
        ReferencePool.Release(aICommandSys);
        ReferencePool.Release(roleAcitonSys);
        ReferencePool.Release(SkillSys);
    }
}
