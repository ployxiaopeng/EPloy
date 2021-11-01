using UnityEngine;
using UnityEditor;

namespace EPloy
{
    public interface IEntity
    {
        long Id { get; set; }
        void Awake(long id, string name);
        void AddComponent(Component component);
        bool HasComponent<T>() where T : Component;
        T GetComponent<T>() where T : Component;
        Component[] GetAllComponent();
        void RemoveComponent<T>() where T : Component;
        void RemoveAllComponent();
    }
}