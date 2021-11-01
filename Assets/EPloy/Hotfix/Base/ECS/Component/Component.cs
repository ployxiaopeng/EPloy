using UnityEngine;
using UnityEditor;
namespace EPloy
{
    public class Component : IComponent, IReference
    {
        public IEntity Entity { get; set; }
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
        public void Awake(IEntity entity, long id)
        {
            if (entity == null)
            {
                Log.Fatal("entity is Null");
                return;
            }
            Id = id;
            Entity = entity;
        }
        
        public virtual void OnDestroy() { }

        public void Clear()
        {
            Entity = null;
            Id = -1;
            OnDestroy();
        }
    }
}