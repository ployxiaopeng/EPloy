//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework;
using GameFramework.Entity;

namespace ETModel
{
    /// <summary>
    /// 实体逻辑基类。
    /// </summary>
    public class EntityModelLogic : EntityLogic
    {
        /// <summary>
        /// 对应的热更新层实体逻辑类名
        /// </summary>
        public string HotfixEntityLogicName { get; private set; }
        /// <summary>
        ///  设置对应的热更新层实体逻辑类名
        /// </summary>
        /// <param name="wndName"></param>
        private void SetHotfixEntityName(string entityName)
        {
            HotfixEntityLogicName = entityName;
        }

        /// <summary>
        /// 实体初始化。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public override void OnInit(int entityId, string entityAssetName, IEntityGroup entityGroup, bool isNewInstance, object userData)
        {
            base.OnInit(entityId, entityAssetName, entityGroup, isNewInstance, userData);
            SetHotfixEntityName((string)userData);
            HotfixEntityEvent entityEvent = ReferencePool.Acquire<HotfixEntityEvent>();
            entityEvent.SetEventData(EntityFunType.OnInit, this);
            Init.Event.FireNow(HotfixEntityEvent.EventId, entityEvent);
        }

        /// <summary>
        /// 实体显示。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public override void OnShow(object userData)
        {
            base.OnShow(userData);
            HotfixEntityEvent entityEvent = ReferencePool.Acquire<HotfixEntityEvent>();
            entityEvent.SetEventData(EntityFunType.OnShow, this);
            Init.Event.Fire(HotfixEntityEvent.EventId, entityEvent);
        }

        /// <summary>
        /// 实体隐藏。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public override void OnHide(object userData)
        {
            base.OnHide(userData);
            HotfixEntityEvent entityEvent = ReferencePool.Acquire<HotfixEntityEvent>();
            entityEvent.SetEventData(EntityFunType.OnHide, this);
            Init.Event.Fire(HotfixEntityEvent.EventId, entityEvent);
        }
    }
}
