using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace EPloy.ECS
{
    public class Entity : IReference
    {
        public string Name { get; private set; }
        public long Id { get; set; }
        private readonly Dictionary<Type, CptBase> cpts = new Dictionary<Type, CptBase>();

        /// <summary>
        /// 实体是否释放
        /// </summary>
        public bool IsRelease
        {
            get { return Id == -1; }
        }

        /// <summary>
        /// 实体初始化
        /// </summary>
        public virtual void Awake(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool HasGetCpt<T>(out T cpt) where T : CptBase
        {
            bool result = HasCpt<T>();
            cpt = (T)(result ? cpts[typeof(T)] : default(T));
            return result;
        }
        public bool HasCpt<T>() where T : CptBase
        {
            return cpts.ContainsKey(typeof(T));
        }

        public T AddCpt<T>(object data = null) where T : CptBase
        {
            if (HasCpt<T>())
            {
                Log.Error("实体已经存在此组件: " + typeof(T));
                return default(T);
            }
            CptBase cptBase = ECSModule.GameScene.CreateCpt(this, typeof(T), data);
            cpts.Add(typeof(T), cptBase);
            return (T)cptBase;
        }


        public T GetCpt<T>(object data = null) where T : CptBase
        {
            if (!HasCpt<T>())
            {
                Log.Error("实体不已经存在此组件: " + typeof(T));
                return default(T);
            }
            return (T)cpts[typeof(T)];
        }

        public void RemoveCpt<T>() where T : CptBase
        {
            if (!HasCpt<T>()) return;
            CptBase cptBase = cpts[typeof(T)];
            ECSModule.GameScene.DestroyCpt(cptBase);
            cpts.Remove(typeof(T));
        }

        /// <summary>
        /// 实体清理
        /// </summary>
        public virtual void Clear()
        {
            foreach (var cpt in cpts)
            {
                ECSModule.GameScene.DestroyCpt(cpt.Value);
            }
            cpts.Clear();
            Id = -1;
        }
    }
}
