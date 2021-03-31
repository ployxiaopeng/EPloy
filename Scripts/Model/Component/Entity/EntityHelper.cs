//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Entity;
using UnityEngine;

namespace ETModel
{
    /// <summary>
    /// 默认实体辅助器。
    /// </summary>
    public class EntityHelper : IEntityHelper
    {
        /// <summary>
        /// 实例化实体。
        /// </summary>
        /// <param name="entityAsset">要实例化的实体资源。</param>
        /// <returns>实例化后的实体。</returns>
        public object InstantiateEntity(object entityAsset)
        {
            GameObject Entity = UnityEngine.Object.Instantiate((GameObject)entityAsset);
            Entity.AddComponent<EntityModelLogic>();
            return Entity;
        }

        /// <summary>
        /// 创建实体。
        /// </summary>
        /// <param name="entityInstance">实体实例。</param>
        /// <param name="entityGroup">实体所属的实体组。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>实体。</returns>
        public IEntity CreateEntity(object entityInstance, IEntityGroup entityGroup, object userData)
        {
            GameObject gameObject = entityInstance as GameObject;
            if (gameObject == null)
            {
                Log.Error("Entity instance is invalid.");
                return null;
            }
            EntityGroupHelper groupHelper = (EntityGroupHelper)entityGroup.Helper;
            gameObject.transform.SetParent(groupHelper.Group.transform);
            return gameObject.GetComponent<EntityModelLogic>();
        }

        /// <summary>
        /// 释放实体。
        /// </summary>
        /// <param name="entityAsset">要释放的实体资源。</param>
        /// <param name="entityInstance">要释放的实体实例。</param>
        public void ReleaseEntity(object entityAsset, object entityInstance)
        {
            Init.Resource.UnloadAsset(entityAsset);
            UnityEngine.Object.Destroy((UnityEngine.Object)entityInstance);
        }
    }
}
