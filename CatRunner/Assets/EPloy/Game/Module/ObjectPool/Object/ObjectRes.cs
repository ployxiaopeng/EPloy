using System.Collections.Generic;

namespace EPloy.ObjectPool
{

    /// <summary>
    /// 资源对象。
    /// </summary>
    internal sealed class ObjectRes : ObjectBase
    {
        private List<object> m_DependencyResources;

        public ObjectRes()
        {
            m_DependencyResources = new List<object>();
        }

        public static ObjectRes Create(string name, object target)
        {
            ObjectRes resourceObject = ReferencePool.Acquire<ObjectRes>();
            resourceObject.Initialize(name, target);
            return resourceObject;
        }

        public override void Clear()
        {
            base.Clear();
            m_DependencyResources.Clear();
        }

        public void AddDependencyResource(object dependencyResource)
        {
            if (Target == dependencyResource)
            {
                return;
            }

            if (m_DependencyResources.Contains(dependencyResource))
            {
                return;
            }

            m_DependencyResources.Add(dependencyResource);
        }

        protected internal override void Unspawn()
        {
            base.Unspawn();
        }
    }
}
    

