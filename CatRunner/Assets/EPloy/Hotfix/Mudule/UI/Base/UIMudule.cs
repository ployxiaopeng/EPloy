
using System;
using System.Collections.Generic;
using System.Linq;
using EPloy.Game;
using EPloy.Game.Obj;
using EPloy.Game.ObjectPool;
using EPloy.Game.Res;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 界面管理器。
    /// </summary>
    public class UIMudule : IHotfixModule
    {
        private Dictionary<UIGroupName, UIGroup> UIGroups;
        private Dictionary<UIName, UIGroupName> UINames;
        private Dictionary<UIName, Type> UIFormTypes;
        private LoadAssetCallbacks LoadAssetCallbacks;
        private Transform UIParent;
        private ObjectPoolBase UIPool;
        public  Camera UICamera { get; private set; }

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
            UIGroups = new Dictionary<UIGroupName, UIGroup>();
            UINames = new Dictionary<UIName, UIGroupName>();

            LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback, LoadAssetDependencyAssetCallback);
            UIParent = GameStart.Instance.transform.Find("UI/Canvas").transform;
            UICamera = GameStart.Instance.transform.Find("UI/UICamera").GetComponent<Camera>();
            UIPool = GameModule.ObjectPool.CreateObjectPool(typeof(ObjectUIForm), "UIPool");
            foreach (var name in Enum.GetValues(typeof(UIGroupName)))
            {
                AddUIGroup((UIGroupName)name);
            }
            GetUIFormTypes();
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
                Log.Fatal(UtilText.Format("UIName {0} is open.", uiName));
                return;
            }
            string path = UtilAsset.GetUIFormAsset(uiName.ToString());
            ObjectUIForm assetObject = (ObjectUIForm)UIPool.Spawn(path);
            if (assetObject == null)
            {
                UIFormInfo UIFormInfo = ReferencePool.Acquire<UIFormInfo>();
                UIFormInfo.SetInfo(uiName, uiGroup, userData);
                GameModule.Res.LoadAsset(path, typeof(GameObject), LoadAssetCallbacks, UIFormInfo);
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
                Log.Fatal(UtilText.Format("UIName {0} is invalid.", uiName));
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
                Log.Fatal(UtilText.Format("UIName {0} is invalid.", uiName));
                return;
            }
            UIGroup group = UIGroups[UINames[uiName]];
            group.RefocusUIForm(uiName, userData);
        }

        /// <summary>
        /// 获取界面。
        /// </summary>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="results">要获取的界面。</param>
        private UIForm GetUIForm(UIName uiName)
        {
            if (!UINames.ContainsKey(uiName))
            {
                return null;
            }
            UIGroup group = UIGroups[UINames[uiName]];
            return group.GetUIForm(uiName);
        }

        /// <summary>
        /// 获取所有uiUIFormType
        /// </summary>
        private void GetUIFormTypes()
        {
            UIFormTypes = new Dictionary<UIName, Type>();
            Type[] Types = HotFixDllUtil.GetHotfixTypes();
            foreach (Type type in Types)
            {
                object[] objects = type.GetCustomAttributes(typeof(UIAttribute), false);
                if (objects.Length != 0)
                {
                    UIAttribute uiAttribute = (UIAttribute)objects[0];
                    if (UIFormTypes.ContainsKey(uiAttribute.UIName))
                    {
                        Log.Error(UtilText.Format("uiName: {0} type: {1} is in UIFormTypes", uiAttribute.UIName, type));
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
                string appendErrorMessage = UtilText.Format("can not fand UIFormInfo : {}", formAssetName);
                Log.Error(appendErrorMessage);
                return;
            }
            object formInstance = UnityEngine.Object.Instantiate((UnityEngine.Object)formAsset);
            ObjectUIForm assetObject = ObjectUIForm.Create(formAssetName, formAsset, formInstance);
            UIPool.Register(assetObject, true);
            uIFormInfo.UIGroup.OpenUIForm(true, formInstance, uIFormInfo.UIName, UIFormTypes[uIFormInfo.UIName], uIFormInfo.UserData);
            ReferencePool.Release(uIFormInfo);
        }

        private void LoadAssetFailureCallback(string uiFormAssetName, LoadResStatus status, string errorMessage)
        {
            string appendErrorMessage = UtilText.Format("Load UI form failure, asset name '{0}', status '{1}', error message '{2}'.", uiFormAssetName, status.ToString(), errorMessage);
            Log.Fatal(appendErrorMessage);
        }

        private void LoadAssetDependencyAssetCallback(string assetName, string dependAssetName, int loadedCount, int totalCount)
        {
            //if (m_OpenUIFormDependencyAssetEventHandler != null)
            //{
            //    OpenUIFormDependencyAssetEventArgs openUIFormDependencyAssetEventArgs = OpenUIFormDependencyAssetEventArgs.Create(openUIFormInfo.SerialId, uiFormAssetName, openUIFormInfo.UIGroup.Name, openUIFormInfo.PauseCoveredUIForm, dependencyAssetName, loadedCount, totalCount, openUIFormInfo.UserData);
            //    m_OpenUIFormDependencyAssetEventHandler(this, openUIFormDependencyAssetEventArgs);
            //    ReferencePool.Release(openUIFormDependencyAssetEventArgs);
            //}
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