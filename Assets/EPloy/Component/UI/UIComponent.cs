
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
        private Entity UIEntity;
        private Dictionary<GroupName, UIGroup> UIGroups;
        private Dictionary<UIName, GroupName> UINames;
        private Dictionary<UIName, Type> UIFormTypes;
        private LoadAssetCallbacks LoadAssetCallbacks;
        private Transform UIParent;
        private ObjectPoolBase UIPool;

        protected override void InitComponent()
        {
            UIEntity = GameEntry.Game.CreateEntity("UI");
            UIGroups = new Dictionary<GroupName, UIGroup>();
            UINames = new Dictionary<UIName, GroupName>();

            LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback);
            UIParent = GameStart.Init.transform.Find("UI/Canvas").transform;
            UIPool = GameEntry.ObjectPool.CreateObjectPool(typeof(UIFormObject), "UIPool");
            foreach (var name in Enum.GetValues(typeof(GroupName)))
            {
                AddUIGroup((GroupName)name);
            }
            GetUIFormTypes();
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
            group.Initialize(groupName, UIParent, UIPool);
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
        /// <param name="userData">用户自定义数据。</param>
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

            // 这个判断待定
            if (uiGroup.HasActiveUIForm(uiName))
            {
                throw new EPloyException(Utility.Text.Format("UIName {0} is open.", uiName));
            }
            string path = AssetUtility.GetUIFormAsset(uiName.ToString());
            UIFormObject assetObject = (UIFormObject)UIPool.Spawn(path);
            if (assetObject == null)
            {
                UIFormInfo UIFormInfo = ReferencePool.Acquire<UIFormInfo>();
                UIFormInfo.SetInfo(uiName, uiGroup, userData);
                GameEntry.Res.LoadAsset(path, typeof(GameObject), LoadAssetCallbacks, UIFormInfo);
            }
            else
            {
                uiGroup.OpenUIForm(false, assetObject.Target, uiName, UIFormTypes[uiName], userData);
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
        public void RefocusUIForm(UIName uiName, object userData = null)
        {
            if (!UINames.ContainsKey(uiName))
            {
                throw new EPloyException(Utility.Text.Format("UIName {0} is invalid.", uiName));
            }
            UIGroup group = UIGroups[UINames[uiName]];
            group.RefocusUIForm(uiName, userData);
        }

        /// <summary>
        /// 获取所有uiUIFormType
        /// </summary>
        private void GetUIFormTypes()
        {
            UIFormTypes = new Dictionary<UIName, Type>();
            Type[] Types = GameEntry.GameSystem.GetAssembly(Config.HotFixDllName).GetTypes();
            foreach (Type type in Types)
            {
                object[] objects = type.GetCustomAttributes(typeof(UIAttribute), false);
                if (objects.Length != 0)
                {
                    UIAttribute uiAttribute = (UIAttribute)objects[0];
                    if (UIFormTypes.ContainsKey(uiAttribute.UIName))
                    {
                        Log.Error(Utility.Text.Format("uiName: {0} type: {1} is in UIFormTypes", uiAttribute.UIName, type));
                        continue;
                    }
                    UIFormTypes.Add(uiAttribute.UIName, type);
                }
            }
        }

        private void LoadAssetSuccessCallback(string uiFormAssetName, object uiFormAsset, float duration, object userData)
        {
            UIFormInfo uIFormInfo = userData as UIFormInfo;
            if (uIFormInfo == null)
            {
                string appendErrorMessage = Utility.Text.Format("can not fand UIFormInfo : {}", uiFormAssetName);
                Log.Error(appendErrorMessage);
                return;
            }
            object uiFormInstance = UnityEngine.Object.Instantiate((UnityEngine.Object)uiFormAsset);
            UIFormObject assetObject = UIFormObject.Create(uiFormAssetName, uiFormAsset, uiFormInstance);
            UIPool.Register(assetObject, true);
            uIFormInfo.UIGroup.OpenUIForm(true, uiFormInstance, uIFormInfo.UIName, UIFormTypes[uIFormInfo.UIName], uIFormInfo.UserData);
            ReferencePool.Release(uIFormInfo);
        }

        private void LoadAssetFailureCallback(string uiFormAssetName, LoadResStatus status, string errorMessage)
        {
            string appendErrorMessage = Utility.Text.Format("Load UI form failure, asset name '{0}', status '{1}', error message '{2}'.", uiFormAssetName, status.ToString(), errorMessage);
            throw new EPloyException(appendErrorMessage);
        }

        /// <summary>
        /// 异步资源时需要存储的信息
        /// </summary>
        private class UIFormInfo : IReference
        {
            public UIName UIName
            {
                get;
                private set;
            }

            public UIGroup UIGroup
            {
                get;
                private set;
            }

            public object UserData
            {
                get;
                private set;
            }

            public void SetInfo(UIName uIName, UIGroup uiGroup, object userData)
            {
                UIName = uIName;
                UIGroup = uiGroup;
                UserData = userData;
            }

            public void Clear()
            {
                UIName = UIName.Default;
                UIGroup = null;
            }
        }
    }
}