
using System;
using System.Collections.Generic;
using EPloy.TaskPool;

namespace EPloy
{
    internal class LoadResTaskbase : TaskBase
    {
        private Type m_AssetType;
        private ResInfo m_ResInfo;
        private object m_UserData;
        private DateTime m_StartTime;
        private string[] m_DependencyAssetsName;
        private List<object> m_DependencyAssets;

        public LoadResTaskbase()
        {
            m_AssetType = null;
            m_UserData = null;
            m_StartTime = default(DateTime);
            m_ResInfo = null;
            m_DependencyAssetsName = null;
            m_DependencyAssets = null;
        }

        public string AssetName
        {
            get
            {
                return m_ResInfo.ResName.Name;
            }
        }

        public Type AssetType
        {
            get
            {
                return m_AssetType;
            }
        }

        public ResInfo ResourceInfo
        {
            get
            {
                return m_ResourceInfo;
            }
        }

        public object UserData
        {
            get
            {
                return m_UserData;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        public string[] DependencyAssetsName
        {
            get
            {
                return m_DependencyAssetsName;
            }
        }

        public List<object> GetDependencyAssets()
        {
            return m_DependencyAssets;
        }

        public void LoadMain(LoadResourceAgent agent, ResourceObject resourceObject)
        {
            m_ResourceObject = resourceObject;
            agent.Helper.LoadAsset(resourceObject.Target, AssetName, AssetType, IsScene);
        }

        public virtual void OnLoadAssetSuccess(LoadResAgent agent, object asset, float duration)
        {
        }

        public virtual void OnLoadAssetFailure(LoadResAgent agent, LoadResourceStatus status, string errorMessage)
        {
        }

        public virtual void OnLoadAssetUpdate(LoadResAgent agent, LoadResourceProgress type, float progress)
        {
        }

        public virtual void OnLoadDependencyAsset(LoadResAgent agent, string dependencyAssetName, object dependencyAsset)
        {
            m_DependencyAssets.Add(dependencyAsset);
        }

        protected void Initialize(string assetName, Type assetType, , ResInfo resInfo, string[] dependencyAssetNames, object userData)
        {
            m_AssetName = assetName;
            m_AssetType = assetType;
            m_ResourceInfo = resourceInfo;
            m_DependencyAssetNames = dependencyAssetNames;
            m_UserData = userData;
        }

        public override void Clear()
        {
            base.Clear();
            m_AssetType = null;
            m_UserData = null;
            m_StartTime = default(DateTime);
            m_DependencyAssetsName = null;
            m_DependencyAssets = null;
        }
    }
}


