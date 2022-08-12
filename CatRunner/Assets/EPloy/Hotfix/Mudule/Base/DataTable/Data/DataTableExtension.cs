

using EPloy.Game;
using EPloy.Hotfix.Table;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix.Table
{
    public static class DataTableExtension
    {
        private const string DataRowClassPrefixName = "EPloy.Hotfix.Table.DR";
        internal static readonly char[] DataSplitSeparators = new char[] { '\t' };
        internal static readonly char[] DataTrimSeparators = new char[] { '\"' };

        public static void LoadDataTable(this DataTableMudule self, string dataTableName)
        {
            if (string.IsNullOrEmpty(dataTableName))
            {
                Log.Fatal("Data table name is invalid.");
                return;
            }

            string[] splitedNames = dataTableName.Split('_');
            if (splitedNames.Length > 2)
            {
                Log.Fatal("Data table name is invalid.");
                return;
            }

            string dataRowClassName = DataRowClassPrefixName + splitedNames[0];
            Type dataRowType = Type.GetType(dataRowClassName);
            if (dataRowType == null)
            {
                Log.Fatal(UtilText.Format("Can not get data row type with class name '{0}'.", dataRowClassName));
                return;
            }

            string name = splitedNames.Length > 1 ? splitedNames[1] : "";
            DataTableBase dataTable = self.CreateDataTable(dataRowType, name);
            string assetPath = UtilAsset.GetDataTableAsset(dataTableName);
            self.LoadDataTable(dataTable, assetPath);
        }

        public static Color32 ParseColor32(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Color32(byte.Parse(splitedValue[0]), byte.Parse(splitedValue[1]), byte.Parse(splitedValue[2]), byte.Parse(splitedValue[3]));
        }

        public static Color ParseColor(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Color(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Quaternion ParseQuaternion(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Quaternion(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Rect ParseRect(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Rect(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static Vector2 ParseVector2(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector2(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]));
        }

        public static Vector3 ParseVector3(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector3(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]));
        }

        public static Vector4 ParseVector4(string value)
        {
            string[] splitedValue = value.Split(',');
            return new Vector4(float.Parse(splitedValue[0]), float.Parse(splitedValue[1]), float.Parse(splitedValue[2]), float.Parse(splitedValue[3]));
        }

        public static List<string> ParseListstring(string value)
        {
            string[] splitValue = value.Split(';');
            List<string> ListStr = new List<string>();
            foreach (var str in splitValue)
                ListStr.Add(str);
            return ListStr;
        }

        public static List<int> ParseListint(string value)
        {
            string[] splitValue = value.Split(';');
            List<int> ListInt = new List<int>();
            foreach (var str in splitValue)
                ListInt.Add(int.Parse(str));

            return ListInt;
        }
    }
}
