using UnityEngine;
using UnityEditor;
namespace EPloy
{
    public class Component : IComponent, IReference
    {
        public IEntity entity { get; set; }
        public long InstanceId { get; set; }

        public bool IsRelease
        {
            get
            {
                return InstanceId == -1;
            }
        }

        /// <summary>
        /// 设置所在的实体
        /// </summary>
        /// <param name="_entity"></param>
        public virtual void SetEntity(IEntity _entity, long instanceId)
        {
            if(_entity==null)
            {
               Log.Fatal("entity is Null");
                return;
            }
            InstanceId = instanceId;
            entity = _entity;

            InitComponent();
        }

        protected virtual void InitComponent() { }

        public virtual void Clear()
        {
            entity = null;
            InstanceId = -1;
        }
    }
}