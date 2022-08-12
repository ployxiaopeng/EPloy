using EPloy.Game.ObjectPool;
using EPloy.Game.Reference;
using EPloy.Game.Res;
using System.Collections.Generic;

namespace EPloy.Game.Obj
{

    /// <summary>
    /// 资源对象。
    /// </summary>
    internal sealed class ObjectAsset : ObjectBase
    {
        public List<object> DependAssets { get; private set; }
        /// <summary>
        /// 存储的资源对象。
        /// </summary>
        public object Asset { get; private set; }

        public ObjectAsset()
        {
            DependAssets = new List<object>();
            Asset = null;
        }

        public static ObjectAsset Create(string name, object target, List<object> dependAssets, object res)
        {
            if (res == null)
            {
                Log.Fatal("Resource is invalid.");
                return null;
            }

            ObjectAsset assetObject = ReferencePool.Acquire<ObjectAsset>();
            assetObject.Initialize(name, target);
            assetObject.Asset = res;
            assetObject.DependAssets.AddRange(dependAssets);
            foreach (object dependAsset in dependAssets)
            {
                int referenceCount = 0;
                if (ResLoader.Instance.AssetDependCount.TryGetValue(dependAsset, out referenceCount))
                {
                    ResLoader.Instance.AssetDependCount[dependAsset] = referenceCount + 1;
                }
                else
                {
                    ResLoader.Instance.AssetDependCount.Add(dependAsset, 1);
                }
            }
            return assetObject;
        }

        public override void Clear()
        {
            base.Clear();
            DependAssets.Clear();
            Asset = null;
        }

        protected internal override void Unspawn()
        {
            base.Unspawn();
            foreach (object dependencyAsset in DependAssets)
            {
                ResLoader.Instance.AssetPool.Unspawn(dependencyAsset);
            }
        }

        // protected internal override void Release(bool isShutdown)
        // {
        //     if (!isShutdown)
        //     {
        //         int targetReferenceCount = 0;
        //         if (m_ResourceLoader.m_AssetDependencyCount.TryGetValue(Target, out targetReferenceCount) && targetReferenceCount > 0)
        //         {
        //             throw new EPloyException(UtilText.Format("Asset target '{0}' reference count is '{1}' larger than 0.", Name, targetReferenceCount.ToString()));
        //         }

        //         foreach (object dependencyAsset in m_DependencyAssets)
        //         {
        //             int referenceCount = 0;
        //             if (m_ResourceLoader.m_AssetDependencyCount.TryGetValue(dependencyAsset, out referenceCount))
        //             {
        //                 m_ResourceLoader.m_AssetDependencyCount[dependencyAsset] = referenceCount - 1;
        //             }
        //             else
        //             {
        //                 throw new EPloyException(UtilText.Format("Asset target '{0}' dependency asset reference count is invalid.", Name));
        //             }
        //         }

        //         m_ResourceLoader.m_ResourcePool.Unspawn(m_Resource);
        //     }

        //     m_ResourceLoader.m_AssetDependencyCount.Remove(Target);
        //     m_ResourceLoader.m_AssetToResourceMap.Remove(Target);
        //     m_ResourceHelper.Release(Target);
        // }
    }
}

