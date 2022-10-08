using EPloy.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ECSModule
{
    public static GameScene GameScene { get; private set; }

    //所以创建的行为树
    public static Dictionary<string, BTRoot> behaviorTrees { get; private set; }

    public static void ECSActivate()
    {
        behaviorTrees = new Dictionary<string, BTRoot>();
        GameScene = GameModuleMgr.CreateModule<GameScene>();
        CreateSystem();
        CreateBehaviorTree();
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
        SkillSys = ReferencePool.Acquire<SkillSystem>();
    }

    public static void DestroySystem()
    {
        ReferencePool.Release(cameraSys);
        ReferencePool.Release(mapSys);
        ReferencePool.Release(moveSys);
        ReferencePool.Release(roleSys);
        ReferencePool.Release(aICommandSys);
        ReferencePool.Release(SkillSys);
    }

    public static void CreateBehaviorTree()
    {
        BTRoot bTRoot = ReferencePool.Acquire<BTRoot>();
        CommonStartNode startNode = CommonStartNode.Create();
        CommonNullNode nullNode = CommonNullNode.Create();
        CommonCompositesNode compositesNode = CommonCompositesNode.Create();
        startNode.AddChildNode(nullNode);
        startNode.AddChildNode(compositesNode);

        CommonFindNode findNode = CommonFindNode.Create();
        CommonAttNode attNode = CommonAttNode.Create();
        compositesNode.AddChildNode(findNode);
        compositesNode.AddChildNode(attNode);
        bTRoot.Create(startNode, "testAi");
        behaviorTrees.Add(bTRoot.bTName, bTRoot);
    }

    public static BTRoot GetBehaviorTree(string name)
    {
        if (behaviorTrees.ContainsKey(name))
        {
            return behaviorTrees[name];
        }
        return null;
    }
}
