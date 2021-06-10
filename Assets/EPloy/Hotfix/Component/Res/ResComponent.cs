﻿using EPloy.Res;
using System;
using System.IO;
using EPloy.TaskPool;
using UnityEngine;

namespace EPloy
{
    public partial class ResComponent : Component
    {
        public static string ReadWritePath = Application.persistentDataPath;
        public static string ReadPath = Application.streamingAssetsPath;

        private string ResPath = ResComponent.ReadWritePath;

        private ResLoader ResLoader;
        private ResEditorLoader ResEditorLoader;
        private ResHelper ResHelper;

        public override void Awake()
        {
            base.Awake();
            if (GameStart.isEditorRes)
            {
                ResEditorLoader = ResEditorLoader.CreateResEditorLoader();
            }
            else
            {
                ResEditorLoader = null;
                ResLoader = ResLoader.CreateResLoader();
            }
            ResHelper = new ResHelper(GameStart.Game);
        }

        public override void Update()
        {
            if (ResEditorLoader == null)
            {
                ResLoader.Update();
            }
            else
            {
                ResEditorLoader.Update();
            }
        }

        public override void OnDestroy()
        {
            ResLoader.OnDestroy();
        }

        /// <summary>
        /// 检查资源是否存在。
        /// </summary>
        /// <param name="assetName">要检查资源的名称。</param>
        /// <returns>检查资源是否存在的结果。</returns>
        public HasResult HasAsset(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                Log.Fatal("Asset name is invalid.");
                return HasResult.NotExist;
            }
            if (ResEditorLoader == null)
            {
                ResInfo resInfo = Game.ResUpdater.GetResInfo(assetName);
                if (resInfo == null)
                {
                    return HasResult.NotExist;
                }

                if (!resInfo.Ready)
                {
                    return HasResult.NotReady;
                }
                return resInfo.IsLoadFromBinary ? HasResult.BinaryOnDisk : HasResult.AssetOnDisk;
            }
            return ResEditorLoader.HasAsset(assetName);
        }

        /// <summary>
        /// 异步加载资源。
        /// </summary>
        /// <param name="assetName">要加载资源的名称。</param>
        /// <param name="assetType">要加载资源的类型。</param>
        /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData = null)
        {
            if (ResEditorLoader == null)
            {
                ResLoader.LoadAsset(assetName, assetType, loadAssetCallbacks, userData);
            }
            else
            {
                ResEditorLoader.LoadAsset(assetName, assetType, loadAssetCallbacks, userData);
            }
        }

        /// <summary>
        /// 异步加载场景。
        /// </summary>
        /// <param name="sceneAssetName">要加载场景资源的名称。</param>
        /// <param name="priority">加载场景资源的优先级。</param>
        /// <param name="loadSceneCallbacks">加载场景回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks)
        {
            if (ResEditorLoader == null)
            {
                ResLoader.LoadScene(sceneAssetName, loadSceneCallbacks);
            }
            else
            {
                ResEditorLoader.LoadScene(sceneAssetName, loadSceneCallbacks);
            }
        }

        /// <summary>
        /// 异步加载二进制资源。
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks)
        {
            if (ResEditorLoader == null)
            {
                ResLoader.LoadBinary(binaryAssetName, loadBinaryCallbacks);
            }
            else
            {
                ResEditorLoader.LoadBinary(binaryAssetName, loadBinaryCallbacks);
            }
        }

        /// <summary>
        /// 直接加载数据流
        /// </summary>
        /// <param name="binaryAssetName">要加载二进制资源的名称。</param>
        /// <param name="loadBinaryCallbacks">加载二进制资源回调函数集。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void LoadBytes(string fileUri, LoadBytesCallbacks loadBytesCallbacks = null)
        {
            ResHelper.LoadBytes(fileUri, loadBytesCallbacks);
        }

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <returns>二进制资源的实际路径。</returns>
        /// <remarks>此方法仅适用于二进制资源存储在磁盘（而非文件系统）中的情况。若二进制资源存储在文件系统中时，返回值将始终为空。</remarks>
        public string GetBinaryPath(string binaryAssetName)
        {
            string binaryPath = null;
            if (ResEditorLoader == null)
            {
                ResInfo resInfo = Game.ResUpdater.GetResInfo(binaryAssetName);
                if (resInfo == null)
                {
                    return null;
                }

                if (!resInfo.Ready)
                {
                    return null;
                }

                if (!resInfo.IsLoadFromBinary)
                {
                    return null;
                }

                return Utility.Path.GetRegularPath(Path.Combine(ResPath, resInfo.ResName.FullName));
            }
            else
            {
                ResEditorLoader.GetBinaryPath(binaryAssetName, out binaryPath);
            }
            return binaryPath;
        }

        /// <summary>
        /// 获取二进制资源的实际路径。
        /// </summary>
        /// <param name="binaryAssetName">要获取实际路径的二进制资源的名称。</param>
        /// <param name="relativePath">二进制资源或存储二进制资源的文件系统，相对于只读区或者读写区的相对路径。</param>
        /// <param name="fileName">若二进制资源存储在文件系统中，则指示二进制资源在文件系统中的名称，否则此参数返回空。</param>
        /// <returns>是否获取二进制资源的实际路径成功。</returns>
        public bool GetBinaryPath(string binaryAssetName, out string fileName)
        {
            if (ResEditorLoader == null)
            {
                fileName = null;

                ResInfo resInfo = Game.ResUpdater.GetResInfo(binaryAssetName);
                if (resInfo == null)
                {
                    return false;
                }

                if (!resInfo.Ready)
                {
                    return false;
                }

                if (!resInfo.IsLoadFromBinary)
                {
                    return false;
                }
                fileName = resInfo.ResName.FullName;
                return true;
            }
            return ResEditorLoader.GetBinaryPath(binaryAssetName, out fileName);
        }

        /// <summary>
        /// 获取二进制资源的长度。
        /// </summary>
        /// <param name="binaryAssetName">要获取长度的二进制资源的名称。</param>
        /// <returns>二进制资源的长度。</returns>
        public int GetBinaryLength(string binaryAssetName)
        {
            if (ResEditorLoader == null)
            {
                ResInfo resInfo = Game.ResUpdater.GetResInfo(binaryAssetName);
                if (resInfo == null)
                {
                    return -1;
                }

                if (!resInfo.Ready)
                {
                    return -1;
                }

                if (!resInfo.IsLoadFromBinary)
                {
                    return -1;
                }

                return resInfo.Length;
            }
            return ResEditorLoader.GetBinaryLength(binaryAssetName);
        }

        /// <summary>
        /// 获取所有加载资源任务的信息。
        /// </summary>
        /// <returns>所有加载资源任务的信息。</returns>
        public TaskInfo[] GetAllLoadAssetInfos()
        {
            if (ResEditorLoader == null)
            {
                return ResLoader.GetAllLoadAssetInfos();
            }
            Log.Fatal("Task no use in Editor");
            return null;
        }
    }
}
