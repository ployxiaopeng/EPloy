
using System;
using System.Collections.Generic;
using EPloy.ObjectPool;
using EPloy.TaskPool;

namespace EPloy.Res
{
    internal abstract class LoadResTaskBase : TaskBase
    {
        public abstract bool IsScene { get; }

        public Type AssetType { get; private set; }
        public ResInfo ResInfo { get; private set; }
        public object UserData { get; private set; }
        public string[] DependAssetsName { get; private set; }
        public List<object> DependAssets { get; private set; }
        public ResObject ResObject { get; private set; }

        public int TotalDependAssetCount { get; set; }
        public DateTime StartTime { get; set; }

        public LoadResTaskBase()
        {
            AssetType = null;
            UserData = null;
            StartTime = default(DateTime);
            ResInfo = null;
            DependAssetsName = null;
            DependAssets = null;
        }

        public string AssetName
        {
            get
            {
                return ResInfo.ResName.Name;
            }
        }

        /// <summary>
        /// 初始化这个加载任务
        /// </summary>
        protected void Initialize(Type assetType, ResInfo resInfo, string[] dependAssetNames, object userData)
        {
            AssetType = assetType;
            ResInfo = resInfo;
            DependAssetsName = dependAssetNames;
            UserData = userData;
        }

        public void LoadAsset(LoadResAgent loadResAgent, ObjectBase resObject)
        {

        }

        public virtual void OnLoadAssetSuccess(LoadResAgent agent, object asset, float duration)
        {
        }

        public virtual void OnLoadAssetFailure(LoadResAgent agent, LoadResStatus status, string errorMessage)
        {
        }

        public virtual void OnLoadAssetUpdate(LoadResAgent agent, LoadResProgress type, float progress)
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
            UserData = null;
            StartTime = default(DateTime);
            DependAssetsName = null;
            DependAssets = null;
        }
    }
}
