
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace EPloy.Game.Res
{

    /// <summary>
    /// 默认资源辅助器。
    /// </summary>
    public class ResHelper
    {
        public byte[] CachedHashBytes { get; private set; }
        private const int CachedHashBytesLength = 4;

        private MonoBehaviour Mono;
        public ResHelper(MonoBehaviour mono)
        {
            CachedHashBytes = new byte[CachedHashBytesLength];
            Mono = mono;
        }

        /// <summary>
        /// 直接从指定文件路径加载数据流。
        /// </summary>
        /// <param name="fileUri">文件路径。</param>
        /// <param name="loadBytesCallbacks">加载数据流回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBytes(string fileUri, LoadBytesCallbacks loadBytesCallbacks)
        {
            Mono.StartCoroutine(LoadBytesCo(fileUri, loadBytesCallbacks));
        }

        /// <summary>
        /// 直接从指定文件路径加载数据流。
        /// </summary>
        /// <param name="fileUri">文件路径。</param>
        /// <param name="loadBytesCallbacks">加载数据流回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBinary(string fileUri, ResInfo resInfo, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            Mono.StartCoroutine(LoadBinaryCo(fileUri, resInfo, loadBinaryCallbacks));
        }

        /// <summary>
        /// 卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            Mono.StartCoroutine(UnloadSceneCo(sceneAssetName, unloadSceneCallbacks));

        }

        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="objectToRelease">要释放的资源。</param>
        public void Release(object objectToRelease)
        {
            AssetBundle assetBundle = objectToRelease as AssetBundle;
            if (assetBundle != null)
            {
                assetBundle.Unload(true);
                return;
            }

            /* Unity 当前 Resources.UnloadAsset 在 iOS 设备上会导致一些诡异问题，先不用这部分
            SceneAsset sceneAsset = objectToRelease as SceneAsset;
            if (sceneAsset != null)
            {
                return;
            }

            Object unityObject = objectToRelease as Object;
            if (unityObject == null)
            {
                Log.Warning("Asset is invalid.");
                return;
            }

            if (unityObject is GameObject || unityObject is MonoBehaviour)
            {
                // UnloadAsset may only be used on individual assets and can not be used on GameObject's / Components or AssetBundles.
                return;
            }

            Resources.UnloadAsset(unityObject);
            */
        }

        private IEnumerator LoadBytesCo(string fileUri, LoadBytesCallbacks loadBytesCallbacks)
        {
            bool isError = false;
            byte[] bytes = null;
            string errorMessage = null;
            DateTime startTime = DateTime.UtcNow;

            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);

            yield return unityWebRequest.SendWebRequest();

            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;

            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();
            if (!isError)
            {
                float elapseSeconds = (float)(DateTime.UtcNow - startTime).TotalSeconds;
                loadBytesCallbacks.LoadBytesSuccessCallback(fileUri, bytes, elapseSeconds);
            }
            else if (loadBytesCallbacks.LoadBytesFailureCallback != null)
            {
                loadBytesCallbacks.LoadBytesFailureCallback(fileUri, errorMessage);
            }
        }

        private IEnumerator LoadBinaryCo(string fileUri, ResInfo resInfo, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            bool isError = false;
            byte[] bytes = null;
            string errorMessage = null;
            DateTime startTime = DateTime.UtcNow;
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
            yield return unityWebRequest.SendWebRequest();
            isError = unityWebRequest.isNetworkError || unityWebRequest.isHttpError;

            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();

            if (!isError)
            {
                float elapseSeconds = (float)(DateTime.UtcNow - startTime).TotalSeconds;
                DecryptResCallback(bytes, resInfo.LoadType, resInfo.HashCode);
                loadBinaryCallbacks.LoadBinarySuccessCallback(fileUri, bytes, elapseSeconds);
            }
            else
            {
                loadBinaryCallbacks.LoadBinaryFailureCallback?.Invoke(fileUri, LoadResStatus.AssetError, errorMessage);
            }
        }

        private IEnumerator UnloadSceneCo(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(sceneAssetName));
            if (asyncOperation == null)
            {
                yield break;
            }

            yield return asyncOperation;

            if (asyncOperation.allowSceneActivation)
            {
                unloadSceneCallbacks.UnloadSceneSuccessCallback?.Invoke(sceneAssetName);
            }
            else
            {
                unloadSceneCallbacks.UnloadSceneFailureCallback?.Invoke(sceneAssetName);
            }
        }

        /// <summary>
        ///  解密函数
        /// </summary>
        public void DecryptResCallback(byte[] bytes, LoadType loadType, int hashCode)
        {
            UtilConverter.GetBytes(hashCode, CachedHashBytes);
            switch (loadType)
            {
                case LoadType.LoadFromMemory:
                    UtilEncryption.GetQuickSelfXorBytes(bytes, CachedHashBytes);
                    break;

                case LoadType.LoadFromBinary:
                    UtilEncryption.GetSelfXorBytes(bytes, CachedHashBytes);
                    break;

                default:
                    Log.Fatal("Not supported load type when decrypt resource.");
                    return;
            }
            Array.Clear(CachedHashBytes, 0, CachedHashBytesLength);
        }
    }
}
