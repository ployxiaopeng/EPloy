using EPloy.Table;
using UnityEngine;
using UnityEngine.UI;

namespace EPloy
{
    public static class SpriteExtension
    {
        private static IDataTable<DRResSprite> _dataResSprite = null;
        private static IDataTable<DRResSprite> DateResSprite
        {
            get
            {
                if (_dataResSprite == null)
                {
                    _dataResSprite = HotFixMudule.DataTable.GetDataTable<DRResSprite>();
                }
                return _dataResSprite;
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
            HotFixMudule.Atlas.SetSprite(resSpriteId, image);
        }
        /// <summary>
        /// 设置图片通过Id
        /// </summary>
        /// <param name="SpriteRenderer"></param>
        /// <param name="resSpriteId"></param>
        public static void SetSprite(this SpriteRenderer SpriteRenderer, int resSpriteId)
        {
            HotFixMudule.Atlas.SetSprite(resSpriteId, SpriteRenderer);
        }
    }
}