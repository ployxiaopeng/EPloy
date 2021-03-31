using GameFramework.DataTable;

namespace ETHotfix
{
    public static class SystemExtension
    {
        /// <summary>
        /// 竖杠拆分
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] VerticalBar(this string str)
        {
            return str.Split('|');
        }
        /// <summary>
        /// 下划线拆分
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] Underlined(this string str)
        {
            return str.Split('_');
        }
        /// <summary>
        /// 逗号拆分
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] Comma(this string str)
        {
            return str.Split(',');
        }
        /// <summary>
        /// string TO Int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            return int.Parse(str);
        }
        /// <summary>
        /// string TO Float
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static float ToFloat(this string str)
        {
            return float.Parse(str);
        }
        /// <summary>
        /// string TO Bool
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool ToBool(this string str)
        {
            return str == "true" ? true : false;
        }

        #region 表格相关

        #endregion
    }
}