using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EPloy.ObjectPool;

namespace EPloy
{
    /// <summary>
    /// 界面 层级组
    /// </summary>
    public sealed class UIGroup : IReference
    {
        private ObjectPoolBase UIPool;
        private Transform Parent;
        private TypeLinkedList<UIForm> UIForms;
        private List<UIForm> ActiveUIForms;
        public UIGroupName GroupName { get; private set; }
        public int Depth { get; private set; }
        public GameObject Handle { get; private set; }

        /// <summary>
        /// 获取界面组中界面数量。
        /// </summary>
        public int FormCount
        {
            get
            {
                return UIForms.Count;
            }
        }

        /// <summary>
        /// 初始化界面组的新实例。
        /// </summary>
        /// <param name="name">界面组名称。</param>
        /// <param name="depth">界面组深度。</param>
        public void Initialize(UIGroupName groupName, Transform parent, ObjectPoolBase uIPool)
        {
            Depth = (int)groupName;
            GroupName = groupName;
            UIForms = new TypeLinkedList<UIForm>();
            ActiveUIForms = new List<UIForm>();
            UIPool = uIPool;
            Parent = parent;
            CreateGroupInstance();
        }

      

        /// <summary>
        /// 界面轮询
        /// </summary>
        public void Update()
        {
            foreach (var uIForm in ActiveUIForms)
            {
                if (uIForm.isPause)
                {
                    continue;
                }
                uIForm.Update();
            }
        }

        /// <summary>
        /// 界面组中是否存在界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>界面组中是否存在界面。</returns>
        public bool HasUIForm(UIName uiName)
        {
            foreach (UIForm uIForm in UIForms)
            {
                if (uIForm.UIName == uiName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 界面是否已经激活
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <returns>界面组中是否存在界面。</returns>
        public bool HasActiveUIForm(UIName uiName)
        {
            UIForm uiForm = GetUIForm(uiName);
            if (uiForm == null)
            {
                return false;
            }
            else
            {
                return ActiveUIForms.Contains(uiForm);
            }
        }

        /// <summary>
        /// 从界面组中获取界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <returns>要获取的界面。</returns>
        public UIForm GetUIForm(UIName uiName)
        {
            foreach (UIForm uiForm in UIForms)
            {
                if (uiForm.UIName == uiName)
                {
                    return uiForm;
                }
            }
            return null;
        }

        /// <summary>
        /// 从界面组中获取所有界面。
        /// </summary>
        /// <returns>界面组中的所有界面。</returns>
        public UIForm[] GetAllUIForms()
        {
            UIForm[] results = new UIForm[UIForms.Count];
            int index = 0;
            foreach (UIForm uiForm in UIForms)
            {
                results[index] = uiForm;
                index++;
            }
            return results;
        }

        /// <summary>
        /// 往界面组增加界面。
        /// </summary>
        /// <param name="uiForm">要增加的界面。</param>
        public void AddUIForm(UIForm uiForm)
        {
            UIForms.AddFirst(uiForm);
        }

        public void CloseUIForm(UIName uiName, object userData)
        {
            UIForm uiForm = GetUIForm(uiName);
            if (uiForm == null)
            {
                Log.Fatal(Utility.Text.Format("UIForm {0} is invalid.", uiName));
                return;
            }
            if (!uiForm.Handle.activeInHierarchy)
            {
                Log.Fatal(Utility.Text.Format("UIForm {0} is Close", uiName));
                return;
            }
            uiForm.Close(userData);
            uiForm.Handle.SetActive(false);
            UIPool.Unspawn(uiForm.Handle);
            ActiveUIForms.Remove(uiForm);
        }

        public void OpenUIForm(bool isNew, object obj, UIName uiName, Type uiFormType, object userData)
        {
            UIForm uiForm = null;
            if (isNew)
            {
                if (uiFormType == null || uiFormType.IsInstanceOfType(typeof(UIForm)))
                {
                    Log.Fatal("can not fand ui C# class  uiName : " + uiName);
                    return;
                }
                uiForm = (UIForm)ReferencePool.Acquire(uiFormType);
                UIForms.AddFirst(uiForm);
            }
            else
            {
                uiForm = GetUIForm(uiName);
            }

            if (uiForm == null)
            {
                Log.Fatal(Utility.Text.Format("UIForm {0} is invalid.", uiName));
                return;
            }
            GameObject uiGo = (GameObject)obj;
            uiGo.transform.SetParent(Handle.transform);
            SetRectTransform(uiGo.transform as RectTransform);
            uiForm.Initialize(isNew, uiGo, uiName, GroupName, userData);
            uiGo.SetActive(true);
            ActiveUIForms.Add(uiForm);
        }

        /// <summary>
        /// 界面激活 刷新
        /// </summary>
        /// <param name="uiForm">要激活的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void RefocusUIForm(UIName uiName, object userData)
        {
            UIForm uiForm = GetUIForm(uiName);
            UIForms.Remove(uiForm);
            UIForms.AddFirst(uiForm);
            uiForm.Refocus(userData);
        }

        /// <summary>
        /// 从界面组移除界面。
        /// </summary>
        /// <param name="uiForm">要移除的界面。</param>
        public void RemoveUIForm(UIForm uiForm)
        {
            if (!UIForms.Remove(uiForm))
            {
                Log.Fatal(Utility.Text.Format("UI group '{0}' not exists specified UI form {1}", GroupName.ToString(), uiForm.UIName));
                return;
            }
            ReferencePool.Release(uiForm);
        }
        
        
        private void CreateGroupInstance()
        {
            Handle = new GameObject(GroupName.ToString());
            Handle.transform.SetParent(Parent);
            Canvas canvas = Handle.AddComponent<Canvas>();
            Handle.AddComponent<GraphicRaycaster>();

            RectTransform transform = Handle.GetComponent<RectTransform>();
            
            SetRectTransform(Handle.transform as RectTransform);
            canvas.sortingOrder = Depth;
            canvas.overrideSorting = true;

            Handle.layer = 5;
        }

        private void SetRectTransform(RectTransform rect)
        {
            rect.localScale = Vector3.one;
            rect.localPosition = Vector3.zero;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMax = Vector2.zero;
            rect.offsetMin = Vector2.zero;
        }
        

        /// <summary>
        /// 界面清理回收。
        /// </summary>
        public void Clear()
        {
            foreach (UIForm uiForm in UIForms)
            {
                UIPool.Unspawn(uiForm.Handle);
                ReferencePool.Release(uiForm);
            }
            UIForms.Clear();
            UIPool.ReleaseAllUnused();
        }
    }
}