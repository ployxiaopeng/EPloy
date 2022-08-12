using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// EcsGame场景
    /// </summary>
    public partial class GameScene : IHotfixModule
    {
        /// <summary>
        /// 所有单例Component，Component Type 关闭后不销毁 可永久保存数据
        /// </summary>
        private readonly Dictionary<Type, IReference> singleCpts = new Dictionary<Type, IReference>();
        private long cptRecordId;

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
}