using EPloy.ObjectPool;
using EPloy.Res;
using EPloy.ObjEntity;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// obj实体组件。
    /// </summary>
    public sealed partial class ObjEntityComponent : Component
    {
        private Dictionary<string, Type> ObjEntityTypes;
        private Dictionary<int, ObjEntityInfo> ObjEntityInfos;
        private Dictionary<ObjEntityGroupName, ObjEntityGroup> ObjEntityGroups;
        private Dictionary<int, int> EntitiesBeingLoaded;
        private HashSet<int> EntitiesToReleaseOnLoad;
        private Queue<ObjEntityInfo> RecycleQueue;
        private LoadAssetCallbacks LoadAssetCallbacks;
        private Transform ObjEntityParent;

        /// <summary>
        /// 获取实体数量。
        /// </summary>
        public int EntityCount
        {
            get
            {
                return ObjEntityInfos.Count;
            }
        }

        /// <summary>
        /// 获取实体组数量。
        /// </summary>
        public int EntityGroupCount
        {
            get
            {
                return ObjEntityGroups.Count;
            }
        }

        public override void Awake()
        {
            ObjEntityTypes = new Dictionary<string, Type>();
            ObjEntityInfos = new Dictionary<int, ObjEntityInfo>();
            ObjEntityGroups = new Dictionary<ObjEntityGroupName, ObjEntityGroup>();
            EntitiesBeingLoaded = new Dictionary<int, int>();
            EntitiesToReleaseOnLoad = new HashSet<int>();
            RecycleQueue = new Queue<ObjEntityInfo>();

            foreach (var name in Enum.GetValues(typeof(ObjEntityGroupName)))
            {
                AddEntityGroup((ObjEntityGroupName)name);
            }
        }

        public override void Update()
        {
            while (RecycleQueue.Count > 0)
            {
                ObjEntityInfo entityInfo = RecycleQueue.Dequeue();
                ObjEntityBase entity = entityInfo.Entity;
                ObjEntityGroup entityGroup = (ObjEntityGroup)entity.EntityGroup;
                if (entityGroup == null)
                {
                    Log.Error("Entity group is invalid.");
                }

                // entityInfo.Status = EntityStatus.WillRecycle;
                // entity.OnRecycle();
                // entityInfo.Status = EntityStatus.Recycled;
                // entityGroup.UnspawnEntity(entity);
                // ReferencePool.Release(entityInfo);
            }

            foreach (KeyValuePair<ObjEntityGroupName, ObjEntityGroup> entityGroup in ObjEntityGroups)
            {
                entityGroup.Value.Update();
            }
        }

        public override void OnDestroy()
        {
            HideAllEntities();
            ObjEntityGroups.Clear();
            EntitiesBeingLoaded.Clear();
            EntitiesToReleaseOnLoad.Clear();
            RecycleQueue.Clear();
        }

        /// <summary>
        /// 获取实体组。
        /// </summary>
        /// <param name="entityGroupName">实体组名称。</param>
        /// <returns>要获取的实体组。</returns>
        public ObjEntityGroup GetEntityGroup(ObjEntityGroupName entityGroupName)
        {
            ObjEntityGroup entityGroup = null;
            if (ObjEntityGroups.TryGetValue(entityGroupName, out entityGroup))
            {
                return entityGroup;
            }

            return null;
        }

        /// <summary>
        /// 获取所有实体组。
        /// </summary>
        /// <returns>所有实体组。</returns>
        public ObjEntityGroup[] GetAllEntityGroups()
        {
            int index = 0;
            ObjEntityGroup[] results = new ObjEntityGroup[ObjEntityGroups.Count];
            foreach (KeyValuePair<ObjEntityGroupName, ObjEntityGroup> entityGroup in ObjEntityGroups)
            {
                results[index++] = entityGroup.Value;
            }

            return results;
        }

        /// <summary>
        /// 增加实体组。
        /// </summary>
        /// <param name="entityGroupName">实体组名称。</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数。</param>
        /// <param name="instanceCapacity">实体实例对象池容量。</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数。</param>
        /// <param name="instancePriority">实体实例对象池的优先级。</param>
        /// <param name="entityGroupHelper">实体组辅助器。</param>
        /// <returns>是否增加实体组成功。</returns>
        public void AddEntityGroup(ObjEntityGroupName entityGroupName)
        {
            string poolName = Utility.Text.Format("ObjEntity{0}Pool", entityGroupName.ToString());
            ObjectPoolBase objEntityPool = GameEntry.ObjectPool.CreateObjectPool(typeof(ObjEntityInstance), poolName);
            ObjEntityGroup objEntityGroup = new ObjEntityGroup(entityGroupName, objEntityPool, ObjEntityParent);
            ObjEntityGroups.Add(entityGroupName, objEntityGroup);
        }

        /// <summary>
        /// 是否存在实体。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <returns>是否存在实体。</returns>
        public bool HasEntity(int entityId)
        {
            return ObjEntityInfos.ContainsKey(entityId);
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <returns>要获取的实体。</returns>
        public ObjEntityBase GetEntity(int entityId)
        {
            ObjEntityInfo entityInfo = GetEntityInfo(entityId);
            if (entityInfo == null)
            {
                return null;
            }

            return entityInfo.Entity;
        }

        /// <summary>
        /// 获取所有已加载的实体。
        /// </summary>
        /// <returns>所有已加载的实体。</returns>
        public ObjEntityBase[] GetAllLoadedEntities()
        {
            int index = 0;
            ObjEntityBase[] results = new ObjEntityBase[ObjEntityInfos.Count];
            foreach (KeyValuePair<int, ObjEntityInfo> entityInfo in ObjEntityInfos)
            {
                results[index++] = entityInfo.Value.Entity;
            }

            return results;
        }

        /// <summary>
        /// 获取所有正在加载实体的编号。
        /// </summary>
        /// <returns>所有正在加载实体的编号。</returns>
        public int[] GetAllLoadingEntityIds()
        {
            int index = 0;
            int[] results = new int[EntitiesBeingLoaded.Count];
            foreach (KeyValuePair<int, int> entityBeingLoaded in EntitiesBeingLoaded)
            {
                results[index++] = entityBeingLoaded.Key;
            }

            return results;
        }

        /// <summary>
        /// 是否正在加载实体。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <returns>是否正在加载实体。</returns>
        public bool IsLoadingEntity(int entityId)
        {
            return EntitiesBeingLoaded.ContainsKey(entityId);
        }

        /// <summary>
        /// 是否是合法的实体。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <returns>实体是否合法。</returns>
        public bool IsValidEntity(ObjEntityBase entity)
        {
            if (entity == null)
            {
                return false;
            }
            return HasEntity(entity.Id);
        }

        /// <summary>
        /// 显示实体。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <param name="entityAssetName">实体资源名称。</param>
        /// <param name="entityGroupName">实体组名称。</param>
        /// <param name="priority">加载实体资源的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void ShowEntity(int entityId, string entityAssetName, ObjEntityGroupName entityGroupName, object userData)
        {
            if (string.IsNullOrEmpty(entityAssetName))
            {
                Log.Error("Entity asset name is invalid.");
                return;
            }

            if (HasEntity(entityId))
            {
                Log.Error(Utility.Text.Format("Entity id '{0}' is already exist.", entityId.ToString()));
                return;
            }

            if (IsLoadingEntity(entityId))
            {
                Log.Error(Utility.Text.Format("Entity '{0}' is already being loaded.", entityId.ToString()));
                return;
            }

            ObjEntityGroup entityGroup = GetEntityGroup(entityGroupName);
            if (entityGroup == null)
            {
                Log.Error(Utility.Text.Format("Entity group '{0}' is not exist.", entityGroupName));
            }

            ObjEntityInstance objEntityInstance = entityGroup.SpawnObjEntityInstance(entityAssetName);
            if (objEntityInstance == null)
            {
                // EntitiesBeingLoaded.Add(entityId, serialId);
                // GameEntry.Res.LoadAsset(entityAssetName, typeof(GameObject), LoadAssetCallbacks, ShowEntityInfo.Create(serialId, entityId, entityGroup, userData));
                return;
            }

            entityGroup.ShowObjEntity(false, objEntityInstance.Target, entityId, ObjEntityTypes[entityAssetName], userData);
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void HideEntity(int entityId, object userData = null)
        {
            if (IsLoadingEntity(entityId))
            {
                EntitiesToReleaseOnLoad.Add(EntitiesBeingLoaded[entityId]);
                EntitiesBeingLoaded.Remove(entityId);
                return;
            }

            ObjEntityInfo entityInfo = GetEntityInfo(entityId);
            if (entityInfo == null)
            {
                Log.Error(Utility.Text.Format("Can not find entity '{0}'.", entityId.ToString()));
            }

            InternalHideEntity(entityInfo, userData);
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="entity">实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void HideEntity(ObjEntityBase entity, object userData = null)
        {
            if (entity == null)
            {
                Log.Error("Entity is invalid.");
                return;
            }

            HideEntity(entity.Id, userData);
        }

        /// <summary>
        /// 隐藏所有的实体。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void HideAllEntities(object userData = null)
        {
            while (ObjEntityInfos.Count > 0)
            {
                foreach (KeyValuePair<int, ObjEntityInfo> entityInfo in ObjEntityInfos)
                {
                    InternalHideEntity(entityInfo.Value, userData);
                    break;
                }
            }
            foreach (KeyValuePair<int, int> entityBeingLoaded in EntitiesBeingLoaded)
            {
                EntitiesToReleaseOnLoad.Add(entityBeingLoaded.Value);
            }

            EntitiesBeingLoaded.Clear();
        }

        /// <summary>
        /// 获取实体信息。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <returns>实体信息。</returns>
        private ObjEntityInfo GetEntityInfo(int entityId)
        {
            ObjEntityInfo entityInfo = null;
            if (ObjEntityInfos.TryGetValue(entityId, out entityInfo))
            {
                return entityInfo;
            }
            return null;
        }

        private void InternalHideEntity(ObjEntityInfo objEntityInfo, object userData)
        {
            while (objEntityInfo.ChildEntities.Count > 0)
            {
                // IEntity childEntity = objEntityInfo.GetChildEntity();
                // HideEntity(childEntity.Id, userData);
            }

            if (objEntityInfo.Status == EntityStatus.Hidden)
            {
                return;
            }

            ObjEntityBase entity = objEntityInfo.Entity;
            //  DetachEntity(entity.Id, userData);
            objEntityInfo.Status = EntityStatus.WillHide;
            objEntityInfo.Entity.Hide(userData);
            objEntityInfo.Status = EntityStatus.Hidden;

            ObjEntityGroup entityGroup = entity.EntityGroup;
            if (entityGroup == null)
            {
                Log.Error("Entity group is invalid.");
            }

            entityGroup.RemoveObjEntity(entity);
            if (!ObjEntityInfos.Remove(entity.Id))
            {
                Log.Error("Entity info is unmanaged.");
            }

            // if (HideEntityCompleteEventHandler != null)
            // {
            //     HideEntityCompleteEventArgs hideEntityCompleteEventArgs = HideEntityCompleteEventArgs.Create(entity.Id, entity.EntityAssetName, entityGroup, userData);
            //     HideEntityCompleteEventHandler(this, hideEntityCompleteEventArgs);
            //     ReferencePool.Release(hideEntityCompleteEventArgs);
            // }

            RecycleQueue.Enqueue(objEntityInfo);
        }

        private void LoadAssetSuccessCallback(string entityAssetName, object entityAsset, object userData)
        {
            ShowObjEntityInfo showEntityInfo = (ShowObjEntityInfo)userData;
            if (showEntityInfo == null)
            {
                Log.Error("Show entity info is invalid.");
            }

            if (EntitiesToReleaseOnLoad.Contains(showEntityInfo.SerialId))
            {
                EntitiesToReleaseOnLoad.Remove(showEntityInfo.SerialId);
                ReferencePool.Release(showEntityInfo);
                // EntityHelper.ReleaseEntity(entityAsset, null);
                return;
            }

            EntitiesBeingLoaded.Remove(showEntityInfo.EntityId);
            object entityInstance = UnityEngine.Object.Instantiate((UnityEngine.Object)entityAsset);
            ObjEntityInstance objEntityInstance = ObjEntityInstance.Create(entityAssetName, entityAsset, entityInstance);
            showEntityInfo.EntityGroup.RegisterObjEntity(objEntityInstance, true);
            showEntityInfo.EntityGroup.ShowObjEntity(true, objEntityInstance.Target, showEntityInfo.EntityId, ObjEntityTypes[entityAssetName], showEntityInfo.UserData);
            ReferencePool.Release(showEntityInfo);
        }

        private void LoadAssetFailureCallback(string entityAssetName, LoadResStatus status, string errorMessage, object userData)
        {
            ShowObjEntityInfo showEntityInfo = (ShowObjEntityInfo)userData;
            if (showEntityInfo == null)
            {
                Log.Error("Show entity info is invalid.");
            }

            if (EntitiesToReleaseOnLoad.Contains(showEntityInfo.SerialId))
            {
                EntitiesToReleaseOnLoad.Remove(showEntityInfo.SerialId);
                return;
            }

            EntitiesBeingLoaded.Remove(showEntityInfo.EntityId);
            string appendErrorMessage = Utility.Text.Format("Load entity failure, asset name '{0}', status '{1}', error message '{2}'.", entityAssetName, status.ToString(), errorMessage);
            // if (ShowEntityFailureEventHandler != null)
            // {
            //     ShowEntityFailureEventArgs showEntityFailureEventArgs = ShowEntityFailureEventArgs.Create(showEntityInfo.EntityId, entityAssetName, showEntityInfo.EntityGroup.Name, appendErrorMessage, showEntityInfo.UserData);
            //     ShowEntityFailureEventHandler(this, showEntityFailureEventArgs);
            //     ReferencePool.Release(showEntityFailureEventArgs);
            //     return;
            // }

            Log.Error(appendErrorMessage);
        }

    }
}
