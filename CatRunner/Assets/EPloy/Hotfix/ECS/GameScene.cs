using EPloy.ECS;
using System;
using System.Collections.Generic;

/// <summary>
/// EcsGame场景
/// </summary>
public partial class GameScene : IGameModule
{
    public Entity entityPlayer { get; set; }
    public List<Entity> monsterEntitys { get; private set; }
    public List<Entity> npcEntitys { get; private set; }

    private long EntityRecordId;
    private long cptRecordId;
    public Dictionary<Type, IReference> singleCpts { get; private set; }


    private bool isEnterMap;

    public void Awake()
    {
        isEnterMap = false;
        monsterEntitys = new List<Entity>();
        npcEntitys = new List<Entity>();
        singleCpts = new Dictionary<Type, IReference>();
    }

    public void OnDestroy()
    {
        ReferencePool.Release(entityPlayer);
        foreach (var entity in monsterEntitys)
        {
            ReferencePool.Release(entity);
        }
        foreach (var entity in npcEntitys)
        {
            ReferencePool.Release(entity);
        }
        monsterEntitys.Clear();
        npcEntitys.Clear();
    }

    public void EnterMap(int mapId, int playerId)
    {
        MapCpt mapCpt = GetSingleCpt<MapCpt>();
        mapCpt.mapId = mapId;
        mapCpt.PlayerId = playerId;
        ECSModule.mapSys.Start(mapCpt);
        isEnterMap = true;
    }

    public void ExitMap()
    {
        isEnterMap = false;
        OnDestroy();
    }

    public void Update()
    {
        if (!isEnterMap) return;
        //玩家
        ECSModule.aICommandSys.PlayerUpdateInput(entityPlayer);
        CameraFollowUpdate(entityPlayer);
        MoveUpdate(entityPlayer);

        //怪物
        for (int i = 0; i < monsterEntitys.Count; i++)
        {
            Entity entityRole = monsterEntitys[i];
            ECSModule.aICommandSys.MoserAIUodate(entityRole);
            MoveUpdate(entityRole);
        }
        //Npc
        for (int i = 0; i < npcEntitys.Count; i++)
        {
            Entity entityRole = monsterEntitys[i];
            ECSModule.aICommandSys.MoserAIUodate(entityRole);
            MoveUpdate(entityRole);
        }
    }


    private void CameraFollowUpdate(Entity entity)
    {
        CameraFollowCpt followCpt = null;
        if (entity.HasGetCpt(out followCpt))
        {
            ECSModule.cameraSys.Update(entity, followCpt);
        }
    }

    private void MoveUpdate(Entity entity)
    {
        MoveCpt moveCpt = null;
        if (entity.HasGetCpt(out moveCpt))
        {
            ECSModule.moveSys.Update(entity, moveCpt);
        }
    }


    /// <summary>
    /// 生成玩家实体
    /// </summary>
    /// <returns></returns>
    public T CreatePlayerEntity<T>(string name) where T : Entity
    {
        entityPlayer = CreateEntity<T>(name);
        return (T)entityPlayer;
    }

    /// <summary>
    /// 生成怪物实体
    /// </summary>
    /// <returns></returns>
    public T CreateMonsterEntity<T>(string name) where T : Entity
    {
        Entity entity = CreateEntity<T>(name);
        monsterEntitys.Add(entity);
        return (T)entity;
    }

    /// <summary>
    /// 销毁怪物实体
    /// </summary>
    /// <returns></returns>
    public void DestroyMonsterEntity(Entity entity)
    {
        if (monsterEntitys.Contains(entity)) monsterEntitys.Remove(entity);
        DestroyEntity(entity);
    }

    /// <summary>
    /// 生成怪物实体
    /// </summary>
    /// <returns></returns>
    public T CreateNpcEntity<T>(string name) where T : Entity
    {
        Entity entity = CreateEntity<T>(name);
        npcEntitys.Add(entity);
        return (T)entity;
    }

    /// <summary>
    /// 销毁怪物实体
    /// </summary>
    /// <returns></returns>
    public void DestroyNpcEntity(Entity entity)
    {
        if (npcEntitys.Contains(entity)) npcEntitys.Remove(entity);
        DestroyEntity(entity);
    }

    /// <summary>
    /// 生成一个实体
    /// </summary>
    /// <returns></returns>
    public T CreateEntity<T>(string name) where T : Entity
    {
        Entity entity = (Entity)ReferencePool.Acquire(typeof(T));
        EntityRecordId++;
        entity.Awake(EntityRecordId, name);
        return (T)entity;
    }

    /// <summary>
    /// 销毁一个实体
    /// </summary>
    public void DestroyEntity(Entity entity)
    {
        //还要对应list 移除 待定
        if (entity.IsRelease) return;
        ReferencePool.Release(entity);
    }

    /// <summary>
    /// 获取单例的组件
    /// </summary>
    public T GetSingleCpt<T>() where T : SingleCptBase<T>, new()
    {
        Type type = typeof(T);
        if (singleCpts.ContainsKey(type))
        {
            return (T)singleCpts[type];
        }
        SingleCptBase<T> cpt = (SingleCptBase<T>)ReferencePool.Acquire(type);
        cpt.Register();
        singleCpts.Add(type, cpt);
        return (T)cpt;
    }

    public void DestroySingleCpt<T>() where T : SingleCptBase<T>, new()
    {
        Type type = typeof(T);
        if (singleCpts.ContainsKey(type))
        {
            ReferencePool.Release(singleCpts[type]);
            singleCpts.Remove(type);
        }
    }

    /// <summary>
    /// 生成组件
    /// </summary>
    internal CptBase CreateCpt(Entity entity, Type cptType, object data)
    {
        CptBase cpt = (CptBase)ReferencePool.Acquire(cptType);
        cpt.Awake(entity, cptRecordId++, data);
        return cpt;
    }

    /// <summary>
    /// 销毁组件
    /// </summary>
    internal void DestroyCpt(CptBase cptBase)
    {
        ReferencePool.Release(cptBase);
    }
}