using EPloy.Table;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EPloy
{
    public static class TextExtension
    {
        private static IDataTable<DRText> _dataText = null;
        private static IDataTable<DRText> DateText
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
        private static IDataTable<DRLanguageText> _dataLanguage = null;
        private static IDataTable<DRLanguageText> DateLanguage
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
            if (ResText != null) Text.text = System.Text.RegularExpressions.Regex.Unescape(ResText.ChineseSimplified);

            else
            {
                Text.text = ("Error");
                Log.Error(string.Format("没有找到id '{0}' 文字", textId));
            }
        }
        public static string GetText(this int id)
        {
            DRText ResText = DateText.GetDataRow(id);
            if (ResText != null)
                return System.Text.RegularExpressions.Regex.Unescape(ResText.ChineseSimplified);
            else Log.Error(string.Format("没有找到id '{0}' 文字", id));
            return null;
        }

        public static void SetStaticText(this Text Text, int textId)
        {
            DRLanguageText ResText = DateLanguage.GetDataRow(textId);
            if (ResText != null) Text.text = System.Text.RegularExpressions.Regex.Unescape(ResText.ChineseSimplified);
            else
            {
                Text.text = ("Error");
                Log.Error(string.Format("没有找到id '{0}' 文字", textId));
            }
        }
    }
}