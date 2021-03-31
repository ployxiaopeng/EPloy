//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using ETModel;
using GameFramework;
using UnityEngine;

namespace ETHotfix
{
    public abstract class HotfixEntityLogic : IHotfixEntityLogic
    {
        public int AssetId
        {
            get
            {
                return EntityData.AssetId;
            }
        }
        public HotfixEntityData EntityData { get; set; }
        public Transform transform
        {
            get
            {
                return EntityLogic.transform;
            }
        }
        public EntityModelLogic EntityLogic { get; set; }

        /// <summary>
        /// 实体初始化
        /// </summary>
        /// <param name="entityLogic"></param>
        public virtual void OnInit(EntityModelLogic entityLogic)
        {
            EntityLogic = entityLogic;
        }
        /// <summary>
        /// 实体show
        /// </summary>
        /// <param name="userData"></param>
        public virtual void OnShow(object userData)
        {
            EntityData = userData as HotfixEntityData;
            if (EntityData == null)
            {
                Log.Error("Entity data is invalid.");
                return;
            }
            if (EntityData.ParentLevel != null)
                transform.SetParent(EntityData.ParentLevel);
            transform.localPosition = EntityData.Position;
            transform.localRotation = Quaternion.Euler(EntityData.Rotation);
            transform.localScale = EntityData.DREntity.ScaleSize;
            transform.name = EntityData.DREntity.AssetName.ToString();
            EntityData.hotfixEntityLogic = this;
        }
        public void Clear()
        {
            OnHide();
        }
        /// <summary>
        /// 实体隐藏
        /// </summary>
        public virtual void OnHide()
        {
            EntityGroupHelper entityGroupHelper = (EntityGroupHelper)EntityLogic.EntityGroup.Helper;
            transform.SetParent(entityGroupHelper.Group.transform);
            if (EntityData != null)
                HotfixReferencePool.Release(EntityData);
            EntityData = null; EntityLogic = null;
        }
        /// <summary>
        /// 实体Update
        /// </summary>
        public virtual void OnUpdate() { }
        protected void HideEntity()
        {
            try
            {
                GameEntry.Entity.HideEntity(EntityLogic);
            }
            catch
            {
                Debug.LogError("这 玄学" + AssetId);
            }
        }
    }
}