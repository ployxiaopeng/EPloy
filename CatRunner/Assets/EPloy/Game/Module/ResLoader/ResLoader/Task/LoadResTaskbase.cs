
using System;
using System.Collections.Generic;
using EPloy.ObjectPool;
using EPloy.TaskPool;

namespace EPloy.Res
{
    /// <summary>
    ///  资源任务信息 基类
    /// </summary>
    internal abstract class LoadResTaskBase : TaskBase
    {
        public abstract bool IsScene { get; }
        public object UserData { get; private set; }
        public Type AssetType { get; private set; }
        public ResInfo ResInfo { get; private set; }
        public string[] DependAssetsName { get; private set; }
        public List<object> DependAssets { get; private set; }
        public ObjectRes ObjectRes { get; private set; }

        public int TotalDependAssetCount { get; set; }
        public DateTime StartTime { get; set; }

        public string AssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化这个加载任务
        /// </summary>
        protected void Initialize(string assetName, Type assetType, ResInfo resInfo, string[] dependAssetNames, object userData)
        {
            AssetType = assetType;
            AssetName = assetName;
            ResInfo = resInfo;
            UserData = userData;
            DependAssetsName = dependAssetNames;
            DependAssets = new List<object>();
        }

        public void SetResObject(ObjectRes objectRes)
        {
            ObjectRes = objectRes;
        }

        public virtual void OnLoadAssetSuccess(LoadResAgent agent, object asset, float duration)
        {
           
        }

        public virtual void OnLoadAssetFailure(LoadResAgent agent, LoadResStatus status, string errorMessage)
        {
        }

        public virtual void OnLoadDependAsset(LoadResAgent agent, string dependencyAssetName, object dependencyAsset)
        {
            DependAssets.Add(dependencyAsset);
        }

        public override void Clear()
        {
            base.Clear();
            AssetType = null;
            StartTime = default(DateTime);
            DependAssetsName = null;
            DependAssets.Clear();
        }
    }
}
