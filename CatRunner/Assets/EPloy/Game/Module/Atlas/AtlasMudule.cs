using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using System;

    /// <summary>
    /// 图集加载数据
    /// </summary>
    public class AtlasLoadData : IReference
    {
        public string atlasName { get; private set; }
        public string atlasPath { get; private set; }

        public static AtlasLoadData Create(string atlasName, string atlasPath)
        {
            AtlasLoadData loadData = ReferencePool.Acquire<AtlasLoadData>();
            loadData.atlasName = atlasName;
            loadData.atlasPath = atlasPath;
            return loadData;
        }
        public void Clear()
        {
            atlasName = null;
            atlasPath = null;
        }
    }

/// <summary>
/// 图集管理组件
/// </summary>
public class AtlasMudule : IGameModule
{
    private Action<string> AtlasLoadEvt;
    private Dictionary<string, SpriteAtlas> AtlasCache;//已经完成加载的图集
    private bool isLoad = false;
    private Queue<AtlasLoadData> LoadQueue;//加载队列

    public void Awake()
    {
        AtlasLoadEvt = null;
        AtlasCache = new Dictionary<string, SpriteAtlas>();
        LoadQueue = new Queue<AtlasLoadData>();
        isLoad = false;
    }

    public void Update()
    {

    }

    public void OnDestroy()
    {
        AtlasLoadEvt = null;
        AtlasCache.Clear();
        LoadQueue.Clear();
        isLoad = false;
    }

    public void RegisterAtlasLoadEvt(Action<string> action)
    {
        AtlasLoadEvt = action;
    }

    public void RemoveAtlasLoadEvt()
    {
        AtlasLoadEvt = null;
    }

    /// <summary>
    /// 加载图集
    /// </summary>
    public async void LoadAtlas(AtlasLoadData loadData)
    {
        if (AtlasCache.ContainsKey(loadData.atlasName)) return;
        //是否正在加载中
        if (isLoad)
        {
            LoadQueue.Enqueue(loadData); return;
        }

        isLoad = true;
        SpriteAtlas spriteAtlas = await GameEntry.Res.AwaitLoadAsset<SpriteAtlas>(loadData.atlasPath);
        if (spriteAtlas == null)
        {
            Log.Error(string.Format("图集 {0} 未加载到", loadData.atlasName));
            return;
        }
        AtlasCache.Add(loadData.atlasName, spriteAtlas);
        AtlasLoadEvt?.Invoke(loadData.atlasName);
        isLoad = false;
        ReferencePool.Release(loadData);
        if (LoadQueue.Count > 0) LoadAtlas(LoadQueue.Dequeue());
    }

    public void UnLoadAtlas(string atlasName)
    {
        if (!AtlasCache.ContainsKey(atlasName))
        {
            return;
        }
        AtlasCache.Remove(atlasName);
    }

    /// <summary>
    /// 设置图片
    /// </summary>
    public void SetSprite(string atlasName, string spriteName, Image image)
    {
        //DRResSprite drSprite = DTSprite.GetDataRow(spriteId);
        //if (drSprite == null)
        //{
        //    Log.Error(UtilText.Format("图片资源配置表不存在ID = {1}的配置", spriteId));
        //    image.sprite = null;
        //    return;
        //}
        if (!AtlasCache.ContainsKey(atlasName))
        {
            Log.Error(UtilText.Format("图片所在图集{0}尚未加载", atlasName));
            return;
        }
        SpriteAtlas spriteAtlas = AtlasCache[atlasName];
        Sprite sprite = spriteAtlas.GetSprite(spriteName);
        if (sprite == null)
        {
            Log.Error(UtilText.Format("表里配置的图片 {0} 图集{1}没找到", spriteName, atlasName));
            return;
        }
        image.sprite = sprite;
    }

    /// <summary>
    /// 设置图片
    /// </summary>
    public void SetSprite(string atlasName, string spriteName, SpriteRenderer spriteRenderer)
    {
        if (!AtlasCache.ContainsKey(atlasName))
        {
            Log.Info(UtilText.Format("图片所在图集{0}尚未加载", atlasName));
            return;
        }
        SpriteAtlas spriteAtlas = AtlasCache[atlasName];
        Sprite sprite = spriteAtlas.GetSprite(spriteName);
        if (sprite == null)
        {
            Log.Error(UtilText.Format("表里配置的图片 {0} 图集{1}没找到", spriteName, atlasName));
            return;
        }
        spriteRenderer.sprite = sprite;
    }
}