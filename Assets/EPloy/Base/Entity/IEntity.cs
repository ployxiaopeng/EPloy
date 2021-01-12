using UnityEngine;
using UnityEditor;

namespace EPloy
{
    public interface IEntity
    {
        int id { get; set; }
        void Awake(int _id);
        T AddComponent<T>() where T : Component;
        T GetComponent<T>() where T : Component;
        void RemoveComponent<T>() where T : Component;
        void RemoveAllComponent();
    }
}