
using System;
using System.Collections.Generic;
using EPloy.ObjectPool;
using EPloy.Res;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 界面管理器。
    /// </summary>
    public class UIComponent : Component
    {
        private Entity UIEntity;
        private Dictionary<UIGroupName, UIGroup> UIGroups;
        private Dictionary<UIName, UIGroupName> UINames;
        private Dictionary<UIName, Type> UIFormTypes;
        private LoadAssetCallbacks LoadAssetCallbacks;
        private Transform UIParent;
        private ObjectPoolBase UIPool;

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

        public override void Awake()
        {
            UIEntity = GameEntry.Game.CreateEntity("UI");
            UIGroups = new Dictionary<UIGroupName, UIGroup>();
            UINames = new Dictionary<UIName, UIGroupName>();

            LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback);
            UIParent = GameStart.Game.transform.Find("UI/Canvas").transform;
            UIPool = GameEntry.ObjectPool.CreateObjectPool(typeof(UIFormObject), "UIPool");
            foreach (var name in Enum.GetValues(typeof(UIGroupName)))
            {
                AddUIGroup((UIGroupName)name);
            }
            GetUIFormTypes();
        }

        public override void Update()
        {
            foreach (var key in UIGroups)
            {
                key.Value.Update();
            }
        }

        public override void OnDestroy()
        {
            UIGroups.Clear();
            UINames.Clear();
        }

        /// <summary>
        /// 获取界面组。
        /// </summary>
        /// <param name="groupName">界面组名称。</param>
        /// <returns>要获取的界面组。</returns>
        public UIGroup GetUIGroup(UIGroupName groupName)
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
        private bool AddUIGroup(UIGroupName groupName)
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
        public void OpenUIForm(UIName uiName, UIGroupName groupName, object userData = null)
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
                Log.Fatal(Utility.Text.Format("UIName {0} is open.", uiName));
                return;
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
        public void CloseUIForm(UIName uiName, object userData = null)
        {
            if (!UINames.ContainsKey(uiName))
            {
                Log.Fatal(Utility.Text.Format("UIName {0} is invalid.", uiName));
                return;
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
                Log.Fatal(Utility.Text.Format("UIName {0} is invalid.", uiName));
                return;
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
            Type[] Types = GameEntry.GameSystem.GetTypes(MuduleConfig.HotFixDllName);
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

        private void LoadAssetSuccessCallback(string formAssetName, object formAsset, float duration, object userData)
        {
            UIFormInfo uIFormInfo = userData as UIFormInfo;
            if (uIFormInfo == null)
            {
                string appendErrorMessage = Utility.Text.Format("can not fand UIFormInfo : {}", formAssetName);
                Log.Error(appendErrorMessage);
                return;
            }
            object formInstance = UnityEngine.Object.Instantiate((UnityEngine.Object)formAsset);
            UIFormObject assetObject = UIFormObject.Create(formAssetName, formAsset, formInstance);
            UIPool.Register(assetObject, true);
            uIFormInfo.UIGroup.OpenUIForm(true, formInstance, uIFormInfo.UIName, UIFormTypes[uIFormInfo.UIName], uIFormInfo.UserData);
            ReferencePool.Release(uIFormInfo);
        }

        private void LoadAssetFailureCallback(string uiFormAssetName, LoadResStatus status, string errorMessage)
        {
            string appendErrorMessage = Utility.Text.Format("Load UI form failure, asset name '{0}', status '{1}', error message '{2}'.", uiFormAssetName, status.ToString(), errorMessage);
            Log.Fatal(appendErrorMessage);
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