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
        public virtual void Awake(Entity entity, long id)
        {
            if (entity == null)
            {
                Log.Fatal("entity is Null");
                return;
            }
            Id = id;
            Entity = entity;
        }

        public virtual void Clear()
        {
            Entity = null;
            Id = -1;
        }
    }
}