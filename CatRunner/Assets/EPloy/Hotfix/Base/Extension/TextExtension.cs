using EPloy.Game;
using EPloy.Hotfix.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EPloy.Hotfix
{
    public static class TextExtension
    {
        private static Table<DRText> _dataText = null;
        private static Table<DRText> DateText
        {
            get
            {
                if (_dataText == null)
                {
                    _dataText = HotFixMudule.DataTable.GetDataTable<DRText>();
                }
                return _dataText;
            }
        }
        private static Table<DRLanguageText> _dataLanguage = null;
        private static Table<DRLanguageText> DateLanguage
        {
            get
            {
                if (_dataLanguage == null)
                {
                    _dataLanguage = HotFixMudule.DataTable.GetDataTable<DRLanguageText>();
                }
                return _dataLanguage;
            }
        }

        /// <summary>
        /// 根据文本ID设置Text组件文本
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="textId">文本ID</param>
        public static void SetText(this Text Text, int textId)
        {
            DRText ResText = DateText.GetDataRow(textId);
            if (ResText == null)
            {
                Text.text = "Error";
                Log.Error(string.Format("没有找到id '{0}' 文字", textId));
                return;
            }
            Text.text = ResText.ChineseSimplified;
        }
        public static string GetText(this int id)
        {
            DRText ResText = DateText.GetDataRow(id);
            if (ResText == null)
            {
                Log.Error(string.Format("没有找到id '{0}' 文字", id));
                return "Error";
            }
            return ResText.ChineseSimplified;
        }

        public static void SetStaticText(this Text Text, int textId)
        {
            DRLanguageText ResText = DateLanguage.GetDataRow(textId);
            if (ResText == null)
            {
                Text.text = "Error";
                Log.Error(string.Format("没有找到id '{0}' 文字", textId));
                return;
            }
            Text.text = ResText.ChineseSimplified;
        }
    }
}