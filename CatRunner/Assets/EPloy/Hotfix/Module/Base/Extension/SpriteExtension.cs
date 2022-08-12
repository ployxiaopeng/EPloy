using EPloy.Table;
using UnityEngine;
using UnityEngine.UI;

namespace EPloy.UI
{
    public static class SpriteExtension
    {
        private static Table<DRResSprite> dTSprite = null;
        private static Table<DRResSprite> DTSprite
        {
            get
            {
                if (dTSprite == null)
                {
                    dTSprite = GameModule.Table.GetDataTable<DRResSprite>();
                }
                return dTSprite;
            }
        }

        /// <summary>
        /// 设置图片通过Id
        /// </summary>
        /// <param name="image"></param>
        /// <param name="spriteName"></param>
        /// <param name="groupName"></param>
        public static void SetSprite(this Image image, int resSpriteId)
        {
            DRResSprite drSprite = DTSprite.GetDataRow(resSpriteId);
            if (drSprite == null)
            {
                Log.Error(UtilText.Format("图片资源配置表不存在ID = {0}的配置", resSpriteId));
                image.sprite = null;
                return;
            }
            GameModule.Atlas.SetSprite(drSprite.AtlasName, drSprite.ImageName, image);
        }
        /// <summary>
        /// 设置图片通过Id
        /// </summary>
        /// <param name="SpriteRenderer"></param>
        /// <param name="resSpriteId"></param>
        public static void SetSprite(this SpriteRenderer SpriteRenderer, int resSpriteId)
        {
            DRResSprite drSprite = DTSprite.GetDataRow(resSpriteId);
            if (drSprite == null)
            {
                Log.Error(UtilText.Format("图片资源配置表不存在ID = {0}的配置", resSpriteId));
                SpriteRenderer.sprite = null;
                return;
            }
            GameModule.Atlas.SetSprite(drSprite.AtlasName, drSprite.ImageName, SpriteRenderer);
        }
    }
}