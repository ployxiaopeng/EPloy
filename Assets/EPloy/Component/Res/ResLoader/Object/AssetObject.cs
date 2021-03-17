//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using EPloy.ObjectPool;
using System.Collections.Generic;

namespace EPloy.Res
{

    /// <summary>
    /// 资源对象。
    /// </summary>
    internal sealed class AssetObject : ObjectBase
    {
        public List<object> DependAssets { get; private set; }
        public object Res { get; private set; }

        public AssetObject()
        {
            DependAssets = new List<object>();
            Res = null;
        }

        public static AssetObject Create(string name, object target, List<object> dependAssets, object res)
        {
            if (dependAssets == null)
            {
                throw new EPloyException("Dependency assets is invalid.");
            }

            if (res == null)
            {
                throw new EPloyException("Resource is invalid.");
            }

            AssetObject assetObject = ReferencePool.Acquire<AssetObject>();
            assetObject.Initialize(name, target);
            assetObject.DependAssets.AddRange(dependAssets);
            assetObject.Res = res;

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
            DependAssets = new List<object>();
            Res = null;
        }

        protected internal override void OnUnspawn()
        {
            base.OnUnspawn();
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
        //             throw new EPloyException(Utility.Text.Format("Asset target '{0}' reference count is '{1}' larger than 0.", Name, targetReferenceCount.ToString()));
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
        //                 throw new EPloyException(Utility.Text.Format("Asset target '{0}' dependency asset reference count is invalid.", Name));
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

