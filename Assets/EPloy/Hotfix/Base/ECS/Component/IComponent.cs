using UnityEngine;
using UnityEditor;
namespace EPloy
{
    public interface IComponent
    {
        IEntity Entity { get; set; }
        long Id { get; set; }
        void Awake(IEntity entity, long id);
    }
}