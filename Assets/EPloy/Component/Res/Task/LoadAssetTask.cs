
using System;

namespace EPloy
{

    internal sealed class LoadAssetTask : LoadResTaskBase
    {
        private LoadAssetCallbacks m_LoadAssetCallbacks;

        public LoadAssetTask()
        {
            m_LoadAssetCallbacks = null;
        }

        public override bool IsScene
        {
            get
            {
                return false;
            }
        }

        public static LoadAssetTask Create(Type assetType, ResInfo resInfo, string[] depenAssetNames, LoadAssetCallbacks loadAssetCallbacks, object userData)
        {
            LoadAssetTask loadAssetTask = ReferencePool.Acquire<LoadAssetTask>();
            loadAssetTask.Initialize(assetType, resInfo, depenAssetNames, userData);
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
                m_LoadAssetCallbacks.LoadAssetSuccessCallback(AssetName, asset, duration, UserData);
            }
        }

        public override void OnLoadAssetFailure(LoadResAgent agent, LoadResStatus status, string errorMessage)
        {
            base.OnLoadAssetFailure(agent, status, errorMessage);
            if (m_LoadAssetCallbacks.LoadAssetFailureCallback != null)
            {
                m_LoadAssetCallbacks.LoadAssetFailureCallback(AssetName, status, errorMessage, UserData);
            }
        }

        public override void OnLoadAssetUpdate(LoadResAgent agent, LoadResProgress type, float progress)
        {
            base.OnLoadAssetUpdate(agent, type, progress);
            if (type == LoadResProgress.LoadAsset)
            {
                m_LoadAssetCallbacks.LoadAssetUpdateCallback?.Invoke(AssetName, progress, UserData);
            }
        }

        public override void OnLoadDependAsset(LoadResAgent agent, string dependAssetName, object dependAsset)
        {
            base.OnLoadDependAsset(agent, dependAssetName, dependAsset);
            if (m_LoadAssetCallbacks.LoadAssetDependAssetCallback != null)
            {
                //m_LoadAssetCallbacks.LoadAssetDependAssetCallback(AssetName, dependAssetName, LoadedDependencyAssetCount, TotalDependeAssetCount, UserData);
            }
        }
    }
}