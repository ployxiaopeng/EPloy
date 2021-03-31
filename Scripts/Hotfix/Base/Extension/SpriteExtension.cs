/****************************************************
    文件：SpriteRenderer.cs
	作者：那位先生
    邮箱: 1279544114@qq.com
    日期：2019/12/2 18:2:17
	功能：SpriteRenderer扩展
*****************************************************/

using GameFramework.DataTable;
using UnityEngine;
using UnityEngine.UI;
using ETModel;

namespace ETHotfix
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
                    _dataResSprite = GameEntry.DataTable.GetDataTable<DRResSprite>();
                }
                return _dataResSprite;
            }
        }
        private static AtlasComponent Atlas
        {
            get
            {
                return GameEntry.Extension.GetComponent<AtlasComponent>();
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
            Atlas.SetSprite(resSpriteId, image);
        }
        /// <summary>
        /// 设置图片通过Id
        /// </summary>
        /// <param name="SpriteRenderer"></param>
        /// <param name="resSpriteId"></param>
        public static void SetSprite(this SpriteRenderer SpriteRenderer, int resSpriteId)
        {
            Atlas.SetSprite(resSpriteId, SpriteRenderer);
        }
    }
}