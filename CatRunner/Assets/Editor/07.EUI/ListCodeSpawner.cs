
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using System.Text;

namespace EPloy.Editor
{
    public partial class UICodeSpawner
    {
        private static Transform ListItem;
        public static void SpawnListCode(GameObject gameObject, string uiName = null)
        {
            ScrollRect scroll = gameObject.GetComponent<ScrollRect>();
            if (scroll == null)
            {
                Log.Error("通用的List 要带 ScrollRect");
                return;
            }
            ListItem = scroll.content.GetChild(0);
            if (ListItem == null)
            {
                Log.Error("通用的List 要有生成 的模板");
                return;
            }
            if (uiName == null)
            {
                Transform transform = scroll.transform;
                while (true)
                {
                    if (transform.parent.parent != null)
                    {
                        transform = transform.parent;
                    }
                    else
                    {
                        uiName = transform.name;
                        break;
                    }
                }
            }
            Widgets = new Dictionary<string, WidgetData>();
            FindListItemWidgets(ListItem, "");
            SpawnCodeForList(scroll, uiName);
            SpawnCodeBindingCodeForList(scroll, uiName);
            AssetDatabase.Refresh();
        }

        private static void FindListItemWidgets(Transform transform, string strPath)
        {
            if (null == transform)
            {
                return;
            }
            for (int nIndex = 0; nIndex < transform.childCount; ++nIndex)
            {
                Transform child = transform.GetChild(nIndex);
                string strTemp = strPath + "/" + child.name;
                foreach (var key in uiTypes)
                {
                    if (!child.name.StartsWith(key.Key)) continue;
                    if (Widgets.ContainsKey(child.name))
                    {
                        Log.Error($"检查相同命名：{child.name}");
                        break;
                    }
                    Widgets.Add(child.name, new WidgetData(key.Value, GetWidgetPath(child, ListItem)));
                    break;
                }
                FindFromWidgets(child, strTemp);
            }
        }

        private static void SpawnCodeForList(ScrollRect scroll, string uiName)
        {
            string strFilePath = Application.dataPath + "/EPloy/Hotfix/Mudule/UI/" + uiName;

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }
            uiName = UtilText.Format("{0}{1}", uiName, scroll.name);
            strFilePath = strFilePath + "/" + uiName + ".cs";

            if (System.IO.File.Exists(strFilePath))
            {
                Debug.LogError("已存在 " + uiName + ".cs,将不会再次生成。");
                return;
            }

            bool isVirtual = scroll.content.GetComponent<VirtualList>() != null;

            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            string className = string.Format("{0}Code", uiName);
            strBuilder.AppendLine("using System.Collections;")
                      .AppendLine("using System.Collections.Generic;")
                      .AppendLine("using System;")
                      .AppendLine("using UnityEngine;")
                      .AppendLine("using EPloy.Hotfix;")
                      .AppendLine("using UnityEngine.UI;\r\n");

            strBuilder.AppendFormat("public class {0} : {1}\r\n", uiName, isVirtual ? "VirtualListBase" : "ListBase");
            strBuilder.AppendLine("{");
            strBuilder.AppendLine("\tprivate  List<string> data=>(List<string>)base.datas;//这是测试生产的模板 请改为自己用的数据后删除");

            strBuilder.AppendLine("\tprotected override void Create()");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tbase.onClick = OnItemclick;\n", className);
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("\tprotected override void Itemrenderer(int index, Transform transform)");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\t{0} bindingCode = {1}.Binding(transform);\n", className, className);
            strBuilder.AppendLine("\t\t//逻辑写在这个位置 bindingCode 渲染完毕要复用 ");
            strBuilder.AppendFormat("\t\t{0}.UnBinding(bindingCode);\r\n", className);
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("\tprivate void OnItemclick(int index, GameObject go)");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendLine("");
            strBuilder.AppendLine("\t}");
            strBuilder.AppendLine("}");

            sw.Write(strBuilder);
            sw.Flush();
            sw.Close();
        }

        private static void SpawnCodeBindingCodeForList(ScrollRect scroll, string uiName)
        {
            uiName = UtilText.Format("{0}{1}", uiName, scroll.name);
            string strFilePath = Application.dataPath + "/EPloy/Hotfix/Mudule/UI/Code";

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }

            strFilePath = strFilePath + "/" + uiName + "CodeBinding.cs";
            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            string className = string.Format("{0}Code", uiName);
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
    }
}
