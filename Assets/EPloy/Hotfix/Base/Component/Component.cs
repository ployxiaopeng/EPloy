using UnityEngine;
using UnityEditor;
namespace EPloy
{
    public class Component : IComponent, IReference
    {
        public IEntity Entity { get; set; }
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
        public void SetEntity(IEntity _entity, long instanceId)
        {
            if (_entity == null)
            {
                Log.Fatal("entity is Null");
                return;
            }
            InstanceId = instanceId;
            Entity = _entity;
        }

        public virtual void Awake() { }
        public virtual void Start() { }
        public virtual void Update() { }
        public virtual void OnDestroy() { }

        public virtual void Clear()
        {
            Entity = null;
            InstanceId = -1;
            OnDestroy();
        }
    }
}