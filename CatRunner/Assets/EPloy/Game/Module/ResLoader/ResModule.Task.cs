using EPloy.Res;
using System;
using System.IO;
using EPloy.TaskPool;
using UnityEngine;
using System.Threading.Tasks;


public partial class ResModule : IGameModule
{
    private TaskCompletionSource<object> s_LoadAssetTcs;
    private LoadAssetCallbacks s_LoadAssetCallbacks;

    /// <summary>
    /// 加载资源（可等待）
    /// </summary>
    public async Task<T> AwaitLoadAsset<T>(string assetPath)
    {
        object asset = await AwaitInternalLoadAsset<T>(assetPath);
        return (T)asset;
    }

    private Task<object> AwaitInternalLoadAsset<T>(string assetPath)
    {
        if (s_LoadAssetCallbacks == null) s_LoadAssetCallbacks = new LoadAssetCallbacks(OnLoadAssetSuccess, OnLoadAssetFailure);
        s_LoadAssetTcs = new TaskCompletionSource<object>();
        LoadAsset(assetPath, typeof(T), s_LoadAssetCallbacks);
        return s_LoadAssetTcs.Task;
    }

    private void OnLoadAssetSuccess(string assetName, object asset, float duration, object userData)
    {
        s_LoadAssetTcs.SetResult(asset);
    }
    private void OnLoadAssetFailure(string assetName, LoadResStatus status, string errorMessage)
    {
        Log.Error(assetName + ": " + errorMessage);
    }
}
