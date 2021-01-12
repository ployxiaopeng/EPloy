
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 全局唯一 记录组件的实体ID  其编号默认0 其他所有的 都>1
    /// </summary>
	public  class WithIdComponent : Component
	{
        private long RecordId;
        private readonly Dictionary<long, Component> allComponents = new Dictionary<long, Component>();
        /// <summary>
        /// 所有激活组件集合
        /// </summary>
        public Dictionary<long, Component> AllComponents
        {
            get
            {
                return allComponents;
            }
        }

        public WithIdComponent()
		{
            this.RecordId = 1;
		}
        /// <summary>
        /// 生成一个组件实体
        /// </summary>
        /// <returns></returns>
        public Component CreateComponent(IEntity _entity, Type type)
        {
            Component component = (Component)ReferencePool.Acquire(type);
            component.SetEntity(_entity, GetNextInstanceId());
            allComponents.Add(RecordId, component);
            return component;
        }

        /// <summary>
        /// 销毁一个组件
        /// </summary>
        public void ReleaseComponent(Component component)
        {
            if (component.IsRelease) return;
            allComponents.Remove(component.InstanceId);
            ReferencePool.Release(component);
        }
        /// <summary>
        /// 获取一个不存在的组件实体Id
        /// </summary>
        /// <returns></returns>
        public long GetNextInstanceId()
        {
            while (allComponents.ContainsKey(RecordId))
            {
                RecordId++;
            }
            return RecordId;
        }

        public void ResetEntityId()
        {
            //0是自己  其他所有的从1开始
            RecordId = 1;
        }

        public override void Clear()
        {
            base.Clear();
            RecordId = -1;
        }
    }
}