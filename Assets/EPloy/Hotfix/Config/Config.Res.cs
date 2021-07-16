using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    public static partial class Config
    {

        /// <summary>
        ///预加载图集
        /// </summary>
        public static readonly string[] PrelaodAtlasNames = new string[]
         {
           "Map10100","Map10101",
         };

        /// <summary>
        ///预加载数据表名
        /// </summary>
        public static readonly string[] PrelaodDataTableNames = new string[]
         {
           "Entity","ResSprite","MapCell","LanguageText","Text",
           "Map",
         };

        /// <summary>
        /// 预加载的UI
        /// </summary>
        public static readonly UIName[] PreloadUIForm = new UIName[]
        {
            UIName.StartForm,
        };

        /// <summary>
        /// 基本数据表名
        /// </summary>
        public static readonly string[] DataTableNames = new string[]
         {
           "Entity","ResSprite","MapCell","LanguageText","Text",
           "Map",//"MapEvt",
         };

        /// <summary>
        /// 动态图集
        /// </summary>
        public static readonly Dictionary<string, string> AtlasNames = new Dictionary<string, string>
        {
            { "Map10100","Assets/Res/Map/10100.prefab"},
            { "Map10101","Assets/Res/Map/10101.prefab"},
        };

    }
}
