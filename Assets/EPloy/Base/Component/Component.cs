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
                new EPloyException("entity is Null");
                return;
            }
            InstanceId = instanceId;
            entity = _entity;

            Init();
        }

        protected virtual void Init() { }

        public virtual void Clear()
        {
            entity = null;
            InstanceId = -1;
        }
    }
}