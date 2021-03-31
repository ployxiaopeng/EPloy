using ETModel;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Resource;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 图集管理组件
    /// </summary>
    [HotfixExtension]
    public class AtlasComponent : Component
    {
        private IDataTable<DRResSprite> m_DTSprite { get { return GameEntry.DataTable.GetDataTable<DRResSprite>(); } }
        private Dictionary<string, GameObject> m_AtlasCache;//已经完成加载的图集
        private Dictionary<string, Sprite> m_SpriteCacheDict;//缓存图片
        private bool isLoad = false;
        private Queue<string> LoadQueue;//加载队列

        public override void Awake()
        {
            m_AtlasCache = new Dictionary<string, GameObject>();
            m_SpriteCacheDict = new Dictionary<string, Sprite>();
            LoadQueue = new Queue<string>();
        }

        /// <summary>
        /// 加载图集
        /// </summary>
        /// <param name="atlasName"></param>
        public async void LoadAtlas(string atlasName)
        {
            if (m_AtlasCache.ContainsKey(atlasName)) return;
            //是否正在加载中
            if (isLoad)
            {
                LoadQueue.Enqueue(atlasName); return;
            }

            isLoad = true;
            GameObject atlasGo = await GameEntry.Resource.AwaitLoadAsset<GameObject>(ConfigRes.AtlasNames[atlasName]);
            if (atlasGo == null)
            {
                Log.Error(string.Format("图集 {0} 未加载到", atlasName));
                return;
            }
            m_AtlasCache.Add(atlasName, atlasGo);
            Altas altas = atlasGo.GetComponent<Altas>();
            foreach (var sprite in altas.Sprites)
                m_SpriteCacheDict.Add(sprite.name, sprite);

            isLoad = false;
            if (LoadQueue.Count > 0) LoadAtlas(LoadQueue.Dequeue());
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="spriteId"></param>
        /// <param name="image"></param>
        public void SetSprite(int spriteId, Image image)
        {
            var drSprite = m_DTSprite.GetDataRow(spriteId);
            if (drSprite == null)
            {
                Log.Error(Utility.Text.Format("图片资源配置表{0}不存在ID = {1}的配置", m_DTSprite.Name, spriteId));
                image.sprite = null;
                return;
            }
            if (!m_AtlasCache.ContainsKey(drSprite.AtlasName))
            {
                Log.Error(Utility.Text.Format("图片所在图集{0}尚未加载", drSprite.AtlasName));
                return;
            }
            if (!m_SpriteCacheDict.ContainsKey(drSprite.ImageName))
            {
                Log.Error(Utility.Text.Format("图集{0}中不存在ID = {1},名称为{2}的图片", drSprite.AtlasName, drSprite.Id, drSprite.ImageName));
                return;
            }
            image.sprite = m_SpriteCacheDict[drSprite.ImageName];
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="spriteId"></param>
        /// <param name="spriteRenderer"></param>
        public void SetSprite(int spriteId, SpriteRenderer spriteRenderer)
        {
            var drSprite = m_DTSprite.GetDataRow(spriteId);
            if (drSprite == null)
            {
                Log.Error(Utility.Text.Format("图片资源配置表{0}不存在ID = {1}的配置", m_DTSprite.Name, spriteId));
                spriteRenderer.sprite = null;
                return;
            }
            if (!m_AtlasCache.ContainsKey(drSprite.AtlasName))
            {
                Log.Info(Utility.Text.Format("图片所在图集{0}尚未加载", drSprite.AtlasName));
                return;
            }
            if (!m_SpriteCacheDict.ContainsKey(drSprite.ImageName))
            {
                Log.Error(Utility.Text.Format("图集{0}中不存在ID = {1},名称为{2}的图片", drSprite.AtlasName, drSprite.Id, drSprite.ImageName));
                return;
            }
            spriteRenderer.sprite = m_SpriteCacheDict[drSprite.ImageName];
        }

    }
}