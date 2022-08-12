using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EPloy.Game;
using UnityEngine;
using UnityEngine.UI;

namespace EPloy.Editor
{
    public partial class UICodeSpawner
    {
        private class WidgetData
        {
            public System.Type Type;
            public string Path;
            public WidgetData(System.Type type, string path)
            {
                Type = type; Path = path;
            }
        }
        private static Dictionary<string, System.Type> uiTypes = null;
        private static Dictionary<string, WidgetData> Widgets = null;
        private const string UINodePrefix = "node";
        private const string UIFormPrefix = "Form";
        private const string UIListPrefix = "list";
        private static GameObject SelectGo;

        static UICodeSpawner()
        {
            uiTypes = new Dictionary<string, System.Type>();
            uiTypes.Add("node", typeof(Transform));
            uiTypes.Add("group", typeof(Transform));
            uiTypes.Add("btn", typeof(GameObject));
            uiTypes.Add("txt", typeof(Text));
            uiTypes.Add("img", typeof(Image));
            uiTypes.Add("rImg", typeof(RawImage));
            uiTypes.Add("input", typeof(Input));
            uiTypes.Add("inputF", typeof(InputField));
            uiTypes.Add("scrollbar", typeof(Scrollbar));
            uiTypes.Add("toggleGroup", typeof(ToggleGroup));
            uiTypes.Add("toggle", typeof(Toggle));
            uiTypes.Add("dropdown", typeof(Dropdown));
            uiTypes.Add("sld", typeof(Slider));
            uiTypes.Add("list", typeof(ScrollRect));
        }

        [MenuItem("GameObject/SpawnEUICode", false, -2)]
        public static void CreateNewCode()
        {
            SelectGo = Selection.activeObject as GameObject;
            UICodeSpawner.SpawnEUICode(SelectGo);
        }

        public static void SpawnEUICode(GameObject gameObject)
        {
            if (null == gameObject)
            {
                Debug.LogError("UICode Select GameObject is null!");
                return;
            }

            try
            {
                string uiName = gameObject.name;
                if (uiName.EndsWith(UIFormPrefix))
                {
                    Log.Info($"----------开始生成UI:{uiName} 相关代码 ----------");
                    SpawnFormCode(gameObject);
                    Log.Info($"生成From{uiName} 完毕!!!");
                    return;
                }
                else if (uiName.StartsWith(UINodePrefix))
                {
                    Log.Info($"-------- 开始生成子UI: {uiName} 相关代码 -------------");
                    SpawnNodeCode(gameObject);
                    Log.Info($"生成子UI: {uiName} 完毕!!!");
                    return;
                }
                else if (uiName.StartsWith(UIListPrefix))
                {
                    Log.Info($"-------- 开始生成滚动列表项: {uiName} 相关代码 -------------");
                    SpawnListCode(gameObject);
                    Log.Info($" 开始生成滚动列表项: {uiName} 完毕！！！");
                    return;
                }
                Debug.LogError($"选择的预设物不属于 From, 子UI，滚动列表项，请检查 {uiName}！！！！！！");
            }
            finally
            {
                Widgets = null;
            }
        }

        public static void SpawnFormCode(GameObject gameObject)
        {
            Widgets = new Dictionary<string, WidgetData>();
            FindFromWidgets(gameObject.transform, "");
            SpawnCodeForForm(gameObject);
            SpawnCodeBindingForFrom(gameObject);
            AssetDatabase.Refresh();
        }

        private static void SpawnCodeForForm(GameObject gameObject)
        {
            string strUiName = gameObject.name;
            string strFilePath = Application.dataPath + "/EPloy/Hotfix/Mudule/UI/" + strUiName;

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strUiName + ".cs";

            if (System.IO.File.Exists(strFilePath))
            {
                Debug.LogError("已存在 " + strUiName + ".cs,将不会再次生成。");
                return;
            }

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            string className = string.Format("{0}Code", strUiName);
            strBuilder.AppendLine("using System.Collections;")
                      .AppendLine("using System.Collections.Generic;")
                      .AppendLine("using System;")
                      .AppendLine("using EPloy.Hotfix;")
                      .AppendLine("using UnityEngine;")
                      .AppendLine("using UnityEngine.UI;\r\n");

            strBuilder.AppendFormat("[UIAttribute(UIName.{0})]\r\n", strUiName);
            strBuilder.AppendFormat("public class {0} : UIForm\r\n", strUiName);
            strBuilder.AppendLine("{");
            strBuilder.AppendLine("\tprotected override bool isUpdate => false;");
            strBuilder.AppendFormat("\tprivate {0} bindingCode;\r\n", className);

            strBuilder.AppendLine("\tpublic override void Create()");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tbindingCode = {0}.Binding(transform);\n", className);
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("\tpublic override void Open(object userData)");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendLine("");
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("\tpublic override void Clear()");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendLine("\t\tbase.Clear();");
            strBuilder.AppendFormat("\t\t{0}.UnBinding(bindingCode);\r\n", className);
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("}");

            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }

        private static void SpawnCodeBindingForFrom(GameObject gameObject)
        {
            string strUiName = gameObject.name;
            string strFilePath = Application.dataPath + "/EPloy/Hotfix/Mudule/UI/Code";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + strUiName + "CodeBinding.cs";
            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            string className = string.Format("{0}Code", strUiName);
            strBuilder.AppendLine("using UnityEngine;")
                      .AppendLine("using EPloy.Hotfix;")
                      .AppendLine("using UnityEngine.UI;\r\n");
            strBuilder.AppendLine("");
            strBuilder.AppendFormat("public  class {0} : IReference \n", className);
            strBuilder.AppendLine("{");
            foreach (var widget in Widgets)
            {
                GetSubUIBaseCode(ref strBuilder, widget.Key, widget.Value);
            }

            strBuilder.AppendFormat("\tpublic static {0} Binding(Transform transform) \n", className);
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\t{0} binding = ReferencePool.Acquire<{1}>(); \n", className, className);
            foreach (var widget in Widgets)
            {
                GetSubUICode(ref strBuilder, widget.Key, widget.Value);
            }
            strBuilder.AppendLine("\t\treturn binding;\n");
            strBuilder.AppendLine("\t}");

            strBuilder.AppendFormat("\tpublic static void UnBinding({0} binding) \n", className);
            strBuilder.AppendLine("\t{");
            strBuilder.AppendLine("\t\t ReferencePool.Release(binding); ");
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("\tpublic void Clear()");
            strBuilder.AppendLine("\t{");

            foreach (var widget in Widgets)
            {
                strBuilder.AppendFormat("		{0} = null;\r\n", widget.Key);
            }
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("}");

            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }

        private static void FindFromWidgets(Transform trans, string strPath)
        {
            if (null == trans) return;
            for (int nIndex = 0; nIndex < trans.childCount; ++nIndex)
            {
                Transform child = trans.GetChild(nIndex);
                string strTemp = strPath + "/" + child.name;

                if (child.name.StartsWith(UINodePrefix))
                {
                    Log.Info($"遇到子UI：{child.name},不生成子UI项代码");
                    Widgets.Add(child.name, new WidgetData(uiTypes[UINodePrefix], GetWidgetPath(child, SelectGo.transform)));
                    continue;
                }
                if (child.name.StartsWith(UIListPrefix))
                {
                    Log.Info($"遇到列表：{child.name},不往下生成");
                    Widgets.Add(child.name, new WidgetData(uiTypes[UIListPrefix], GetWidgetPath(child, SelectGo.transform)));
                    continue;
                }

                foreach (var key in uiTypes)
                {
                    if (!child.name.StartsWith(key.Key)) continue;
                    if (Widgets.ContainsKey(child.name))
                    {
                        Log.Error($"检查相同命名：{child.name}");
                        break;
                    }
                    Widgets.Add(child.name, new WidgetData(key.Value, GetWidgetPath(child, SelectGo.transform)));
                    break;
                }

                FindFromWidgets(child, strTemp);
            }
        }

        private static string GetWidgetPath(Transform obj, Transform root)
        {
            string path = obj.name;

            while (obj.parent != null && obj.parent != root)
            {
                obj = obj.transform.parent;
                path = obj.name + "/" + path;
            }
            return path;
        }

        private static void GetSubUIBaseCode(ref StringBuilder strBuilder, string widget, WidgetData widgetData)
        {

            strBuilder.AppendFormat("\tpublic {0} {1};\r\n", widgetData.Type.ToString(), widget);
        }

        private static void GetSubUICode(ref StringBuilder strBuilder, string widget, WidgetData widgetData)
        {
            if (widgetData.Type == typeof(GameObject))
            {
                strBuilder.AppendFormat("\t\tbinding.{0} = transform.Find({1}).gameObject; \r\n", widget, "\"" + widgetData.Path + "\"");
                return;
            }
            if (widgetData.Type == typeof(Transform))
            {
                strBuilder.AppendFormat("\t\tbinding.{0} = transform.Find({1}); \r\n", widget, "\"" + widgetData.Path + "\"");
                return;
            }
            strBuilder.AppendFormat("\t\tbinding.{0} = transform.Find({1}).GetComponent<{2}>(); \r\n", widget, "\"" + widgetData.Path + "\"", widgetData.Type.ToString());
        }

    }
}