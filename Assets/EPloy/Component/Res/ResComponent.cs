using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;

namespace EPloy
{
    [System]
    public class ResComponentUpdateSystem : UpdateSystem<ResComponent>
    {
        public override void Update(ResComponent self)
        {
            self.Update();
        }
    }

    public partial class ResComponent : Component
    {
        private const int CachedHashBytesLength = 4;
        private string ReadWritePath;
        private Dictionary<string, AssetInfo> m_AssetInfos;//资产信息
        private Dictionary<ResName, ResInfo> m_ResInfos;//资原信息
        //private SortedDictionary<ResName, ReadWriteResourceInfo> m_ReadWriteResourceInfos;
        //private readonly Dictionary<string, ResourceGroup> m_ResourceGroups;

        protected override void Init()
        {
            TaskPool = new TaskPool<LoadResTaskBase>();
            AssetDependCount = new Dictionary<object, int>();
            ResourceDependCount = new Dictionary<object, int>();
            AssetToResourceMap = new Dictionary<object, object>();
            SceneToAssetMap = new Dictionary<string, object>(StringComparer.Ordinal);
            //LoadBytesCallbacks = new LoadBytesCallbacks(OnLoadBinarySuccess, OnLoadBinaryFailure);
            CachedHashBytes = new byte[CachedHashBytesLength];
            AssetPool = null;
            ResourcePool = null;
        }

        /// <summary>
        /// 加载资源器轮询。
        /// </summary>
        public void Update()
        {
            TaskPool.Update();
        }

        /// <summary>
        /// 关闭并清理加载资源器。
        /// </summary>
        public void OnDestroy()
        {
            TaskPool.OnDestroy();
            AssetDependCount.Clear();
            ResourceDependCount.Clear();
            AssetToResourceMap.Clear();
            SceneToAssetMap.Clear();
            //LoadResourceAgent.Clear();
        }

    }
}
