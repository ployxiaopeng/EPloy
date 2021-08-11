using System.Collections.Generic;
using System.Threading.Tasks;
using EPloy.Table;
using UnityEngine;
using UnityEngine.UI;

namespace EPloy
{
    /// <summary>
    /// 图集管理组件
    /// </summary>
    public class AtlasMudule : IHotfixModule
    {
        private IDataTable<DRResSprite> DTSprite { get { return HotFixMudule.DataTable.GetDataTable<DRResSprite>(); } }
        private Dictionary<string, GameObject> AtlasCache;//已经完成加载的图集
        private Dictionary<string, SpriteInfo> SpriteCacheDict;//缓存图片
        private bool isLoad = false;
        private Queue<string> LoadQueue;//加载队列

        public void Awake()
        {
            AtlasCache = new Dictionary<string, GameObject>();
            SpriteCacheDict = new Dictionary<string, SpriteInfo>();
            LoadQueue = new Queue<string>();
        }

        public void Update()
        {
           
        }

        public void OnDestroy()
        {
           
        }

        /// <summary>
        /// 加载图集
        /// </summary>
        /// <param name="atlasName"></param>
        public async void LoadAtlas(string atlasName)
        {
            if (AtlasCache.ContainsKey(atlasName)) return;
            //是否正在加载中
            if (isLoad)
            {
                LoadQueue.Enqueue(atlasName); return;
            }

            isLoad = true;
            GameObject atlasGo = await HotFixMudule.Res.AwaitLoadAsset<GameObject>(Config.AtlasNames[atlasName]);
            if (atlasGo == null)
            {
                Log.Error(string.Format("图集 {0} 未加载到", atlasName));
                return;
            }
            AtlasCache.Add(atlasName, atlasGo);
            Altas altas = atlasGo.GetComponent<Altas>();
            foreach (var sprite in altas.Sprites)
            {
                SpriteInfo spriteInfo = ReferencePool.Acquire<SpriteInfo>();
                spriteInfo.SetInfo(atlasName, sprite);
                SpriteCacheDict.Add(sprite.name, spriteInfo);
            }
            AtlasSuccessEvt atlasSuccessEvt = ReferencePool.Acquire<AtlasSuccessEvt>();
            atlasSuccessEvt.SetData(atlasName);
            HotFixMudule.Event.Fire(atlasSuccessEvt);
            isLoad = false;
            if (LoadQueue.Count > 0) LoadAtlas(LoadQueue.Dequeue());
        }

        public void UnLoadAtlas(string atlasName)
        {
            if (!AtlasCache.ContainsKey(atlasName))
            {
                return;
            }

            foreach (var info in SpriteCacheDict)
            {
                if (info.Value.AtlasName != atlasName)
                {
                    continue;
                }
                SpriteCacheDict.Remove(info.Key);
            }
            AtlasCache.Remove(atlasName);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="spriteId"></param>
        /// <param name="image"></param>
        public void SetSprite(int spriteId, Image image)
        {
            var drSprite = DTSprite.GetDataRow(spriteId);
            if (drSprite == null)
            {
                Log.Error(Utility.Text.Format("图片资源配置表不存在ID = {1}的配置", spriteId));
                image.sprite = null;
                return;
            }
            if (!AtlasCache.ContainsKey(drSprite.AtlasName))
            {
                Log.Error(Utility.Text.Format("图片所在图集{0}尚未加载", drSprite.AtlasName));
                return;
            }
            if (!SpriteCacheDict.ContainsKey(drSprite.ImageName))
            {
                Log.Error(Utility.Text.Format("图集{0}中不存在ID = {1},名称为{2}的图片", drSprite.AtlasName, drSprite.Id, drSprite.ImageName));
                return;
            }
            image.sprite = SpriteCacheDict[drSprite.ImageName].Sprite;
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="spriteId"></param>
        /// <param name="spriteRenderer"></param>
        public void SetSprite(int spriteId, SpriteRenderer spriteRenderer)
        {
            var drSprite = DTSprite.GetDataRow(spriteId);
            if (drSprite == null)
            {
                Log.Error(Utility.Text.Format("图片资源配置表不存在ID = {1}的配置", spriteId));
                spriteRenderer.sprite = null;
                return;
            }
            if (!AtlasCache.ContainsKey(drSprite.AtlasName))
            {
                Log.Info(Utility.Text.Format("图片所在图集{0}尚未加载", drSprite.AtlasName));
                return;
            }
            if (!SpriteCacheDict.ContainsKey(drSprite.ImageName))
            {
                Log.Error(Utility.Text.Format("图集{0}中不存在ID = {1},名称为{2}的图片", drSprite.AtlasName, drSprite.Id, drSprite.ImageName));
                return;
            }
            spriteRenderer.sprite = SpriteCacheDict[drSprite.ImageName].Sprite;
        }

        private class SpriteInfo : IReference
        {
            public void SetInfo(string atlasName, Sprite sprite)
            {
                AtlasName = atlasName; Sprite = sprite;
            }
            public string AtlasName { get; private set; }
            public Sprite Sprite { get; private set; }
            public string Name
            {
                get
                {
                    return Sprite.name;
                }
            }

            public void Clear()
            {
                AtlasName = null;
                Sprite = null;
            }
        }
    }
}