using EPloy.ECS;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EcsGame场景
/// </summary>
public partial class GameScene : IGameModule
{
    private EntityMap entityMap;
    public List<EntityRole> entityRoles { get; private set; }
    private long EntityRecordId;

    public Dictionary<Type, IReference> singleCpts { get; private set; }
    private long cptRecordId;

    public void Awake()
    {
        entityRoles = new List<EntityRole>();
        singleCpts = new Dictionary<Type, IReference>();
    }

    public void Update()
    {
        for (int i = 0; i < entityRoles.Count; i++)
        {
            EntityRole entityRole = entityRoles[i];
            ECSModule.aICommandSys.Updata(entityRole);
            CameraFollowUpdate(entityRole);
            MoveUpdate(entityRole);
        }
    }

    public void OnDestroy()
    {
        foreach (var entity in entityRoles)
        {
            ReferencePool.Release(entity);
        }
        entityRoles.Clear();
    }

    public void EnterMap(int mapId, int playerId)
    {
        entityMap = CreateEntity<EntityMap>("map");
        entityMap.mapCpt = GetCpt<MapCpt>(entityMap);
        entityMap.mapCpt.mapId = mapId;
        entityMap.mapCpt.PlayerId = playerId;
        ECSModule.mapSys.Start(entityMap, entityMap.mapCpt);
    }

    public void ExitMap()
    {

    }

    private void CameraFollowUpdate(EntityRole entityRole)
    {
        if (entityRole.cameraFollowCpt == null) return;
        ECSModule.cameraSys.Update(entityRole, entityRole.cameraFollowCpt);
    }

    private void MoveUpdate(EntityRole entityRole)
    {
        if (entityRole.moveCpt == null) return;
        ECSModule.moveSys.Update(entityRole, entityRole.moveCpt);
    }

    public EntityRole CreateEntityRole(string name = null)
    {
        EntityRole entityRole = (EntityRole)CreateEntity<EntityRole>(name);
        entityRoles.Add(entityRole);
        return entityRole;
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
    public T GetSingleCpt<T>() where T : IReference, new()
    {
        Type type = typeof(T);
        if (singleCpts.ContainsKey(type))
        {
            return (T)singleCpts[type];
        }

        IReference component = (IReference)ReferencePool.Acquire(type);
        singleCpts.Add(type, component);
        return (T)component;
    }

    /// <summary>
    /// 添加生成组件
    /// </summary>
    public T GetCpt<T>(Entity entity) where T : CptBase, new()
    {
        Type type = typeof(T);
        CptBase cpt = (CptBase)ReferencePool.Acquire(type);
        cpt.Awake(entity, cptRecordId++);
        return (T)cpt;
    }
}