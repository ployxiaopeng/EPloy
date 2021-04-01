using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace EPloy.Res
{
    /// <summary>
    /// 默认加载资源代理辅助器。
    /// </summary>
    internal sealed partial class LoadResAgent
    {
        private string m_FileFullPath = null;
        private string m_FileName = null;
        private string m_BytesFullPath = null;
        private string m_AssetName = null;
        private bool m_Disposed = false;

        private UnityWebRequest m_UnityWebRequest = null;
        private AssetBundleCreateRequest m_FileAssetBundleCreateRequest = null;
        private AssetBundleCreateRequest m_BytesAssetBundleCreateRequest = null;
        private AssetBundleRequest m_AssetBundleRequest = null;
        private AsyncOperation m_AsyncOperation = null;

        /// <summary>
        /// 通过加载资源代理辅助器开始异步读取资源文件。
        /// </summary>
        /// <param name="fullPath">要加载资源的完整路径名。</param>
        public void ReadFile(string fullPath)
        {
            m_FileFullPath = fullPath;
            m_FileAssetBundleCreateRequest = AssetBundle.LoadFromFileAsync(fullPath);
        }

        /// <summary>
        /// 通过加载资源代理辅助器开始异步读取资源二进制流。
        /// </summary>
        /// <param name="fullPath">要加载资源的完整路径名。</param>
        public void ReadBytes(string fullPath)
        {
            m_BytesFullPath = fullPath;
            m_UnityWebRequest.SendWebRequest();
        }

        /// <summary>
        /// 通过加载资源代理辅助器开始异步将资源二进制流转换为加载对象。
        /// </summary>
        /// <param name="bytes">要加载资源的二进制流。</param>
        public void ParseBytes(byte[] bytes)
        {
            m_BytesAssetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(bytes);
        }

        /// <summary>
        /// 在 AssetBundle 中加载具体资源 
        /// </summary>
        /// <param name="resource">资源。</param>
        /// <param name="assetName">要加载的资源名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="isScene">要加载的资源是否是场景。</param>
        public void LoadAsset(object resource, string assetName, Type assetType, bool isScene)
        {
            AssetBundle assetBundle = resource as AssetBundle;
            if (assetBundle == null)
            {
                string errMsg = "Can not load asset bundle from loaded resource which is not an asset bundle.";
                OnError(LoadResStatus.TypeError, errMsg);
                return;
            }

            if (string.IsNullOrEmpty(assetName))
            {
                string errMsg = "Can not load asset from asset bundle which child name is invalid.";
                OnError(LoadResStatus.AssetError, errMsg);
                return;
            }

            m_AssetName = assetName;
            if (isScene)
            {
                int sceneNamePositionStart = assetName.LastIndexOf('/');
                int sceneNamePositionEnd = assetName.LastIndexOf('.');
                if (sceneNamePositionStart <= 0 || sceneNamePositionEnd <= 0 || sceneNamePositionStart > sceneNamePositionEnd)
                {
                    string errMsg = Utility.Text.Format("Scene name '{0}' is invalid.", assetName);
                    OnError(LoadResStatus.AssetError, errMsg);
                    return;
                }

                string sceneName = assetName.Substring(sceneNamePositionStart + 1, sceneNamePositionEnd - sceneNamePositionStart - 1);
                m_AsyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
            else
            {
                if (assetType != null)
                {
                    m_AssetBundleRequest = assetBundle.LoadAssetAsync(assetName, assetType);
                }
                else
                {
                    m_AssetBundleRequest = assetBundle.LoadAssetAsync(assetName);
                }
            }
        }

        public void Update()
        {
            UpdateUnityWebRequest();
            UpdateFileAssetBundleCreateRequest();
            UpdateBytesAssetBundleCreateRequest();
            UpdateAssetBundleRequest();
            UpdateAsyncOperation();
        }

        private void UpdateUnityWebRequest()
        {
            if (m_UnityWebRequest != null)
            {
                if (m_UnityWebRequest.isDone)
                {
                    if (string.IsNullOrEmpty(m_UnityWebRequest.error))
                    {
                        OnReadBytesComplete(m_UnityWebRequest.downloadHandler.data);
                        m_UnityWebRequest.Dispose();
                        m_UnityWebRequest = null;
                        m_BytesFullPath = null;
                    }
                    else
                    {
                        bool isError = false;
                        isError = m_UnityWebRequest.isNetworkError || m_UnityWebRequest.isHttpError;
                        string errMsg = Utility.Text.Format("Can not load asset bundle '{0}' with error message '{1}'.", m_BytesFullPath, isError ? m_UnityWebRequest.error : null);
                        OnError(LoadResStatus.NotExist, errMsg);
                    }
                }
            }
        }

        private void UpdateFileAssetBundleCreateRequest()
        {
            if (m_FileAssetBundleCreateRequest != null)
            {
                if (m_FileAssetBundleCreateRequest.isDone)
                {
                    AssetBundle assetBundle = m_FileAssetBundleCreateRequest.assetBundle;
                    if (assetBundle != null)
                    {
                        AssetBundleCreateRequest oldFileAssetBundleCreateRequest = m_FileAssetBundleCreateRequest;
                        OnReadFileComplete(assetBundle);
                        if (m_FileAssetBundleCreateRequest == oldFileAssetBundleCreateRequest)
                        {
                            m_FileAssetBundleCreateRequest = null;
                        }
                    }
                    else
                    {
                        string errMsg = Utility.Text.Format("Can not load asset bundle from file '{0}' which is not a valid asset bundle.", m_FileName == null ? m_FileFullPath : Utility.Text.Format("{0} | {1}", m_FileFullPath, m_FileName));
                        OnError(LoadResStatus.NotExist, errMsg);
                    }
                }
            }
        }

        private void UpdateBytesAssetBundleCreateRequest()
        {
            if (m_BytesAssetBundleCreateRequest != null)
            {
                if (m_BytesAssetBundleCreateRequest.isDone)
                {
                    AssetBundle assetBundle = m_BytesAssetBundleCreateRequest.assetBundle;
                    if (assetBundle != null)
                    {
                        AssetBundleCreateRequest oldBytesAssetBundleCreateRequest = m_BytesAssetBundleCreateRequest;
                        OnReadFileComplete(assetBundle);
                        if (m_BytesAssetBundleCreateRequest == oldBytesAssetBundleCreateRequest)
                        {
                            m_BytesAssetBundleCreateRequest = null;
                        }
                    }
                    else
                    {
                        string errMsg = "Can not load asset bundle from memory which is not a valid asset bundle.";
                        OnError(LoadResStatus.NotExist, errMsg);
                    }
                }
            }
        }

        private void UpdateAssetBundleRequest()
        {
            if (m_AssetBundleRequest != null)
            {
                if (m_AssetBundleRequest.isDone)
                {
                    if (m_AssetBundleRequest.asset != null)
                    {
                        OnComplete(m_AssetBundleRequest.asset);
                        m_AssetName = null;
                        m_AssetBundleRequest = null;
                    }
                    else
                    {
                        string errMsg = Utility.Text.Format("Can not load asset '{0}' from asset bundle which is not exist.", m_AssetName);
                        OnError(LoadResStatus.NotExist, errMsg);
                    }
                }
            }
        }

        private void UpdateAsyncOperation()
        {
            if (m_AsyncOperation != null)
            {
                if (m_AsyncOperation.isDone)
                {
                    if (m_AsyncOperation.allowSceneActivation)
                    {
                        OnComplete(null);
                        m_AssetName = null;
                        m_AsyncOperation = null;
                    }
                    else
                    {
                        string errMsg = Utility.Text.Format("Can not load scene asset '{0}' from asset bundle.", m_AssetName);
                        OnError(LoadResStatus.NotExist, errMsg);
                    }
                }
            }
        }

        /// <summary>
        /// 重置加载资源代理辅助器。
        /// </summary>
        public void Reset()
        {
            Task = null;
            m_FileFullPath = null;
            m_FileName = null;
            m_BytesFullPath = null;
            m_AssetName = null;
            if (m_UnityWebRequest != null)
            {
                m_UnityWebRequest.Dispose();
                m_UnityWebRequest = null;
            }
            m_FileAssetBundleCreateRequest = null;
            m_BytesAssetBundleCreateRequest = null;
            m_AssetBundleRequest = null;
            m_AsyncOperation = null;
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记。</param>
        public void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                if (m_UnityWebRequest != null)
                {
                    m_UnityWebRequest.Dispose();
                    m_UnityWebRequest = null;
                }
            }

            m_Disposed = true;
        }

    }
}
