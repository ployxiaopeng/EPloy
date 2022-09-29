using UnityEngine;
using UnityEditor;
namespace EPloy.ECS
{
    public class CptBase : IReference
    {
        public Entity Entity { get; private set; }
        public long Id { get; set; }
        public bool IsRelease
        {
            get
            {
                return Id == -1;
            }
        }

        /// <summary>
        /// 设置所在的实体
        /// </summary>
        /// <param name="_entity"></param>
        internal void Awake(Entity entity, long id, object data)
        {
            if (entity == null)
            {
                Log.Fatal("entity is Null");
                return;
            }
            Id = id;
            Entity = entity;
            Awake(data);
        }

        protected virtual void Awake(object data)
        {

        }

        public virtual void Clear()
        {
            Entity = null;
            Id = -1;
        }
    }

    public class SingleCptBase<T> : IReference where T : SingleCptBase<T>, new()
    {
        public bool isRelease { get; private set; }
        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
        }
        public void Register()
        {
            if (instance != null) return;
            instance = (T)this;
        }

        public virtual void Clear()
        {
            if (this.isRelease)
            {
                return;
            }
            this.isRelease = true;
            T t = instance;
            instance = null;
        }
    }
}