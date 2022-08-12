
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
        public static void SpawnNodeCode(GameObject gameObject, string uiName = null)
        {
            if (uiName == null)
            {
                Transform transform = gameObject.transform;
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
            FindNodeWidgets(gameObject.transform, "");
            SpawnCodeForNode(gameObject, uiName);
            SpawnCodeBindingCodeForNode(gameObject, uiName);
            AssetDatabase.Refresh();
        }

        private static void FindNodeWidgets(Transform trans, string strPath)
        {
            if (null == trans)
            {
                return;
            }
            for (int nIndex = 0; nIndex < trans.childCount; ++nIndex)
            {
                Transform child = trans.GetChild(nIndex);
                string strTemp = strPath + "/" + child.name;
                bool isSubUI = child.name.StartsWith(UINodePrefix);

                if (isSubUI)
                {
                    Log.Error("本身已经是子ui 不允许在出现子ui设定 请重新设计");
                    continue;
                }
                else
                {
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
                }
                FindFromWidgets(child, strTemp);
            }
        }

        private static void SpawnCodeForNode(GameObject gameObject, string uiName)
        {
            string strFilePath = Application.dataPath + "/EPloy/Hotfix/Mudule/UI/" + uiName;

            if (!System.IO.Directory.Exists(strFilePath))
            {
                System.IO.Directory.CreateDirectory(strFilePath);
            }
            uiName = UtilText.Format("{0}{1}", uiName, gameObject.name.Substring(4, gameObject.name.Length-4));
            strFilePath = strFilePath + "/" + uiName + ".cs";

            if (System.IO.File.Exists(strFilePath))
            {
                Debug.LogError("已存在 " + uiName + ".cs,将不会再次生成。");
                return;
            }
            StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
            StringBuilder strBuilder = new StringBuilder();
            string className = string.Format("{0}Code", uiName);
            strBuilder.AppendLine("using System.Collections;")
                        .AppendLine("using System.Collections.Generic;")
                        .AppendLine("using System;")
                        .AppendLine("using EPloy.Hotfix;")
                        .AppendLine("using UnityEngine;")
                        .AppendLine("using UnityEngine.UI;\r\n");

            strBuilder.AppendFormat("public class {0} : UIFormLogic\r\n", className);
            strBuilder.AppendLine("{");
            strBuilder.AppendFormat("\tprivate {0} bindingCode;\r\n", className);

            strBuilder.AppendLine("\tprotected override void Create()");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendFormat("\t\tbindingCode = {0}.Binding(transform);\n", className);
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("\tpublic override void ShowView()");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendLine("\t\tbase.ShowView();");
            strBuilder.AppendLine("\t}");

            strBuilder.AppendLine("\tpublic override void CloseView()");
            strBuilder.AppendLine("\t{");
            strBuilder.AppendLine("\t\tbase.CloseView();");
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

        private static void SpawnCodeBindingCodeForNode(GameObject gameObject, string uiName)
        {
            uiName = UtilText.Format("{0}{1}", uiName, gameObject.name.Substring(4, gameObject.name.Length - 4));
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
