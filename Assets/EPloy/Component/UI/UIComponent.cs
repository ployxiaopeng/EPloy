
using System;
using System.Collections.Generic;
using EPloy.ObjectPool;
using EPloy.Res;
using UnityEngine;

namespace EPloy
{

    [System]
    public class UIComponentUpdateSystem : UpdateSystem<UIComponent>
    {
        public override void Update(UIComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 界面管理器。
    /// </summary>
    public class UIComponent : Component
    {
        private Dictionary<GroupName, UIGroup> UIGroups;
        private Dictionary<UIName, GroupName> UINames;
        private Transform UIParent;
        private ObjectPoolBase UIPool;

        public UIComponent()
        {
            UIGroups = new Dictionary<GroupName, UIGroup>();
            UINames = new Dictionary<UIName, GroupName>();
            foreach (var name in Enum.GetValues(typeof(GroupName)))
            {
                AddUIGroup((GroupName)name);
            }
        }

        /// <summary>
        /// 获取界面组数量。
        /// </summary>
        public int UIGroupCount
        {
            get
            {
                return UIGroups.Count;
            }
        }

        /// <summary>
        /// 界面管理器轮询。
        /// </summary>
        public void Update()
        {
            foreach (var key in UIGroups)
            {
                key.Value.Update();
            }
        }

        /// <summary>
        /// 关闭并清理界面管理器。
        /// </summary>
        public void OnDestroy()
        {
            UIGroups.Clear();
            UINames.Clear();
        }

        /// <summary>
        /// 获取界面组。
        /// </summary>
        /// <param name="groupName">界面组名称。</param>
        /// <returns>要获取的界面组。</returns>
        public UIGroup GetUIGroup(GroupName groupName)
        {
            UIGroup uiGroup = null;
            if (UIGroups.TryGetValue(groupName, out uiGroup))
            {
                return uiGroup;
            }

            return null;
        }

        /// <summary>
        /// 增加界面组。
        /// </summary>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <returns>是否增加界面组成功。</returns>
        private bool AddUIGroup(GroupName groupName)
        {
            UIGroup group = ReferencePool.Acquire<UIGroup>();
            group.Initialize(groupName, UIParent);
            UIGroups.Add(groupName, group);
            return true;
        }

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="results">要获取的界面。</param>
        public UIForm GetUIForm(UIName uiName)
        {
            if (!UINames.ContainsKey(uiName))
            {
                return null;
            }
            UIGroup group = UIGroups[UINames[uiName]];
            return group.GetUIForm(uiName);
        }

        /// <summary>
        /// 是否存在界面。
        /// </summary>
        /// <param name="uiForm">界面。</param>
        /// <returns>界面是否合法。</returns>
        public bool HasUIForm(UIName uiName)
        {
            return GetUIForm(uiName) != null;
        }

        /// <summary>
        /// 打开界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="uiGroupName">界面组名称。</param>
        /// <param name="priority">加载界面资源的优先级。</param>
        /// <param name="pauseCoveredUIForm">是否暂停被覆盖的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>界面的序列编号。</returns>
        public void OpenUIForm(UIName uiName, GroupName groupName, object userData)
        {
            UIGroup uiGroup = GetUIGroup(groupName);
            if (UINames.ContainsKey(uiName))
            {
                UINames[uiName] = groupName;
            }
            else
            {
                UINames.Add(uiName, groupName);
            }

            //TODO: 路径
            string path = "uiName";
            AssetObject assetObject = (AssetObject)UIPool.Spawn(path);
            if (assetObject == null)
            {
                //TODO: 加载
            }
            else
            {
                uiGroup.OpenUIForm(false, assetObject.Res, uiName, userData);
            }
        }

        /// <summary>
        /// 关闭界面。
        /// </summary>
        /// <param name="uiForm">要关闭的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void CloseUIForm(UIName uiName, object userData)
        {
            if (!UINames.ContainsKey(uiName))
            {
                throw new EPloyException(Utility.Text.Format("UIName {0} is invalid.", uiName));
            }
            UIGroup group = UIGroups[UINames[uiName]];
            group.CloseUIForm(uiName, userData);
        }

        /// <summary>
        /// 激活界面。
        /// </summary>
        /// <param name="uiForm">要激活的界面。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void RefocusUIForm(UIName uiName, object userData)
        {
            if (!UINames.ContainsKey(uiName))
            {
                throw new EPloyException(Utility.Text.Format("UIName {0} is invalid.", uiName));
            }
            UIGroup group = UIGroups[UINames[uiName]];
            group.RefocusUIForm(uiName, userData);
        }

        /// <summary>
        /// 设置对象池管理器。
        /// </summary>
        /// <param name="objectPoolManager">对象池管理器。</param>
        private void SetObjectPool()
        {
            UIPool = GameEntry.ObjectPool.CreateObjectPool(typeof(AssetObject), "UIPool");
        }
    }
}
