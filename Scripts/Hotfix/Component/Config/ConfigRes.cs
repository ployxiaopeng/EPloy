using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 资源静态设置
    /// </summary>
    public static class ConfigRes
    {

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
            { "Map10100","Assets/Res/Textures/Map/10100.prefab"},
            { "Map10101","Assets/Res/Textures/Map/10101.prefab"},
        };

        /// <summary>
        /// 预加载的UI
        /// </summary>
        public static readonly UIWnd[] loadUIWnd = new UIWnd[]
        {
            UIWnd.LoginWnd,
        };
    }
}