
using System;

namespace EPloy.Res
{
    internal sealed class LoadAssetTask : LoadResTaskBase
    {
        private LoadAssetCallbacks m_LoadAssetCallbacks;

        public override bool IsScene
        {
            get
            {
                return false;
            }
        }

        public static LoadAssetTask Create(Type assetType, ResInfo resInfo, string[] depenAssetNames, LoadAssetCallbacks loadAssetCallbacks)
        {
            LoadAssetTask loadAssetTask = ReferencePool.Acquire<LoadAssetTask>();
            loadAssetTask.Initialize(assetType, resInfo, depenAssetNames);
            loadAssetTask.m_LoadAssetCallbacks = loadAssetCallbacks;
            return loadAssetTask;
        }

        public override void Clear()
        {
            base.Clear();
            m_LoadAssetCallbacks = null;
        }

        public override void OnLoadAssetSuccess(LoadResAgent agent, object asset, float duration)
        {
            base.OnLoadAssetSuccess(agent, asset, duration);
            if (m_LoadAssetCallbacks.LoadAssetSuccessCallback != null)
            {
                m_LoadAssetCallbacks.LoadAssetSuccessCallback(AssetName, asset, duration);
            }
        }

        public override void OnLoadAssetFailure(LoadResAgent agent, LoadResStatus status, string errorMessage)
        {
            base.OnLoadAssetFailure(agent, status, errorMessage);
            if (m_LoadAssetCallbacks.LoadAssetFailureCallback != null)
            {
                m_LoadAssetCallbacks.LoadAssetFailureCallback(AssetName, status, errorMessage);
            }
        }

        public override void OnLoadDependAsset(LoadResAgent agent, string dependAssetName, object dependAsset)
        {
            base.OnLoadDependAsset(agent, dependAssetName, dependAsset);
            if (m_LoadAssetCallbacks.LoadDependAssetCallback != null)
            {
                m_LoadAssetCallbacks.LoadDependAssetCallback(AssetName, dependAssetName, DependAssets.Count, TotalDependAssetCount);
            }
        }
    }
}