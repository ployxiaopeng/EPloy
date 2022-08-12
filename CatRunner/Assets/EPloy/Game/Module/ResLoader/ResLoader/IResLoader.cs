using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;
using UnityEngine;

namespace EPloy.Res
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public interface IResLoader
    {
        /// <summary>
        /// 关闭并清理加载资源器。
        /// </summary>
        void OnDestroy();

        /// <summary>
        /// 加载资源器轮询。
        /// </summary>
        void Update();

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        HasResult HasAsset(string assetName);
        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData);

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks);

        /// <summary>
        /// 异步卸载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        void UnloadScene(string sceneName, UnloadSceneCallbacks unloadSceneCallbacks);

        /// <summary>
        /// 异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks);

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <returns>二进制资源的实际路径。</returns>
        /// <remarks>此方法仅适用于二进制资源存储在磁盘（而非文件系统）中的情况。若二进制资源存储在文件系统中时，返回值将始终为空。</remarks>
        string GetBinaryPath(string binaryAssetName);

        /// <summary>
        /// 获取二进制资源的长度。
        /// </summary>
        /// <param name="binaryAssetName">要获取长度的二进制资源的名称。</param>
        /// <returns>二进制资源的长度。</returns>
        int GetBinaryLength(string binaryAssetName);

        /// <summary>
        /// 获取所有加载资源任务的信息。
        /// </summary>
        /// <returns>所有加载资源任务的信息。</returns>
        TaskInfo[] GetAllLoadAssetInfos();
    }
}