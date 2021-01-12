using UnityEngine;
using UnityEditor;
namespace EPloy
{
    public interface IComponent
    {
        IEntity entity { get; set; }
        long InstanceId { get; set; }
        void SetEntity(IEntity _entity, long instanceId);
    }
}