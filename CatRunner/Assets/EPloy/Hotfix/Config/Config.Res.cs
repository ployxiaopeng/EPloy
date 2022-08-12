using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    public static class ResConfig
    {

        /// <summary>
        ///预加载图集
        /// </summary>
        public static readonly string[] PrelaodAtlasNames = new string[]
         {
          "Skill",
         };

        /// <summary>
        ///预加载数据表名
        /// </summary>
        public static readonly string[] PrelaodDataTableNames = new string[]
         {
           "Entity","ResSprite","LanguageText","Text","Map","SkillData","RoleData"
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
           "Entity","ResSprite","LanguageText","Text","Map","RoleData","SkillData"
         };

        /// <summary>
        /// 动态图集
        /// </summary>
        public static readonly Dictionary<string, string> AtlasNames = new Dictionary<string, string>
        {
            { "Skill","Assets/Res/Atlas/Skill.spriteatlas"},
        };
    }
}
