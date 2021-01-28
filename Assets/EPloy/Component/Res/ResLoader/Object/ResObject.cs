using EPloy.ObjectPool;
using System.Collections.Generic;

namespace EPloy.Res
{

    /// <summary>
    /// 资源对象。
    /// </summary>
    internal sealed class ResObject : ObjectBase
    {
        private List<object> m_DependencyResources;

        public ResObject()
        {
            m_DependencyResources = new List<object>();
        }

        public static ResObject Create(string name, object target)
        {
            ResObject resourceObject = ReferencePool.Acquire<ResObject>();
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

        protected internal override void OnUnspawn()
        {
            
        }
    }
}
    

