using EPloy.ObjectPool;
using EPloy.Res;
using EPloy.Obj;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 实例物体组件。
    /// </summary>
    public sealed partial class ObjMudule : IHotfixModule
    {
        private Dictionary<string, Type> ObjTypes;
        private Dictionary<int, ObjInfo> ObjInfos;
        private Dictionary<ObjGroupName, ObjGroup> ObjGroups;
        private Dictionary<int, string> ObjsStartLoad;
        private Queue<ObjInfo> RecycleQueue;
        private LoadAssetCallbacks LoadAssetCallbacks;
        private Transform ObjParent;

        /// <summary>
        /// 获取实体数量。
        /// </summary>
        public int ObjCount
        {
            get
            {
                return ObjInfos.Count;
            }
        }

        /// <summary>
        /// 获取实体组数量。
        /// </summary>
        public int ObjGroupCount
        {
            get
            {
                return ObjGroups.Count;
            }
        }

        public void Awake()
        {
            ObjParent = GameStart.Instance.transform.Find("Obj").transform;
            ObjInfos = new Dictionary<int, ObjInfo>();
            ObjGroups = new Dictionary<ObjGroupName, ObjGroup>();
            ObjsStartLoad = new Dictionary<int, string>();
            RecycleQueue = new Queue<ObjInfo>();
            LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback);
            foreach (var name in Enum.GetValues(typeof(ObjGroupName)))
            {
                AddObjGroup((ObjGroupName)name);
            }
            GetObjTypes();
        }

        public void Update()
        {
            while (RecycleQueue.Count > 0)
            {
                ObjInfo objInfo = RecycleQueue.Dequeue();
                ObjBase objBase = objInfo.Obj;
                ObjGroup objGroup = (ObjGroup)objBase.ObjGroup;
                if (objGroup == null)
                {
                    Log.Error("Entity group is invalid.");
                }
                objInfo.Status = ObjStatus.Recycled;
                objGroup.UnspawnObj(objBase);
                ReferencePool.Release(objInfo);
            }

            foreach (KeyValuePair<ObjGroupName, ObjGroup> entityGroup in ObjGroups)
            {
                entityGroup.Value.Update();
            }
        }

        public void OnDestroy()
        {
            HideAllObj();
            ObjGroups.Clear();
            ObjsStartLoad.Clear();
            RecycleQueue.Clear();
        }

        /// <summary>
        /// 获取实体组。
        /// </summary>
        /// <param name="objGroupName">实体组名称。</param>
        /// <returns>要获取的实体组。</returns>
        public ObjGroup GetObjGroup(ObjGroupName objGroupName)
        {
            ObjGroup objGroup = null;
            if (ObjGroups.TryGetValue(objGroupName, out objGroup))
            {
                return objGroup;
            }
            return null;
        }

        /// <summary>
        /// 获取所有实体组。
        /// </summary>
        /// <returns>所有实体组。</returns>
        public ObjGroup[] GetAllObjGroups()
        {
            int index = 0;
            ObjGroup[] results = new ObjGroup[ObjGroups.Count];
            foreach (KeyValuePair<ObjGroupName, ObjGroup> entityGroup in ObjGroups)
            {
                results[index++] = entityGroup.Value;
            }
            return results;
        }

        /// <summary>
        /// 增加实体组。
        /// </summary>
        /// <param name="objGroupName">实体组名称。</param>
        /// <param name="instanceAutoReleaseInterval">实体实例对象池自动释放可释放对象的间隔秒数。</param>
        /// <param name="instanceCapacity">实体实例对象池容量。</param>
        /// <param name="instanceExpireTime">实体实例对象池对象过期秒数。</param>
        /// <param name="instancePriority">实体实例对象池的优先级。</param>
        /// <param name="entityGroupHelper">实体组辅助器。</param>
        /// <returns>是否增加实体组成功。</returns>
        public void AddObjGroup(ObjGroupName objGroupName)
        {
            string poolName = Utility.Text.Format("ObjEntity{0}Pool", objGroupName.ToString());
            ObjectPoolBase objEntityPool = HotFixMudule.ObjectPool.CreateObjectPool(typeof(ObjInstance), poolName);
            ObjGroup objEntityGroup = new ObjGroup(objGroupName, objEntityPool, ObjParent);
            ObjGroups.Add(objGroupName, objEntityGroup);
        }

        /// <summary>
        /// 是否存在实体。
        /// </summary>
        /// <param name="serialId">实体编号。</param>
        /// <returns>是否存在实体。</returns>
        public bool HasObj(int serialId)
        {
            return ObjInfos.ContainsKey(serialId);
        }

        /// <summary>
        /// 获取实体。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <returns>要获取的实体。</returns>
        public ObjBase GetObj(int entityId)
        {
            ObjInfo entityInfo = GetObjInfo(entityId);
            if (entityInfo == null)
            {
                return null;
            }

            return entityInfo.Obj;
        }

        /// <summary>
        /// 获取所有已加载的实体。
        /// </summary>
        /// <returns>所有已加载的实体。</returns>
        public ObjBase[] GetAllLoadedObjs()
        {
            int index = 0;
            ObjBase[] results = new ObjBase[ObjInfos.Count];
            foreach (KeyValuePair<int, ObjInfo> entityInfo in ObjInfos)
            {
                results[index++] = entityInfo.Value.Obj;
            }

            return results;
        }

        /// <summary>
        /// 获取所有正在加载实体的编号。
        /// </summary>
        /// <returns>所有正在加载实体的编号。</returns>
        public int[] GetAllLoadingObjIds()
        {
            int index = 0;
            int[] results = new int[ObjsStartLoad.Count];
            foreach (KeyValuePair<int, string> entityBeingLoaded in ObjsStartLoad)
            {
                results[index++] = entityBeingLoaded.Key;
            }

            return results;
        }

        /// <summary>
        /// 是否正在加载实体。
        /// </summary>
        /// <param name="objId">实体编号。</param>
        /// <returns>是否正在加载实体。</returns>
        public bool IsLoadingObj(int objId)
        {
            return ObjsStartLoad.ContainsKey(objId);
        }

        /// <summary>
        /// 是否是合法的实体。
        /// </summary>
        /// <param name="obj">实体。</param>
        /// <returns>实体是否合法。</returns>
        public bool IsValidObj(ObjBase obj)
        {
            if (obj == null)
            {
                return false;
            }
            return HasObj(obj.SerialId);
        }

        /// <summary>
        /// 显示实体。
        /// </summary>
        /// <param name="serialId">实体编号。</param>
        /// <param name="objName">实体资源名称 这里要对表的不急。</param>
        /// <param name="objGroupName">实体组名称 这里要对表的不急。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void ShowObj(int serialId, string objName, ObjGroupName objGroupName, object userData)
        {
            if (string.IsNullOrEmpty(objName))
            {
                Log.Error("Obj asset name is invalid.");
                return;
            }

            if (HasObj(serialId))
            {
                Log.Error(Utility.Text.Format("serialIdId '{0}' is already exist.", serialId.ToString()));
                return;
            }

            if (IsLoadingObj(serialId))
            {
                Log.Error(Utility.Text.Format("serialId '{0}' is already being loaded.", serialId.ToString()));
                return;
            }

            ObjGroup objGroup = GetObjGroup(objGroupName);
            if (objGroup == null)
            {
                Log.Error(Utility.Text.Format("Obj group '{0}' is not exist.", objGroupName));
            }
            string path = AssetUtility.GetObjAsset(objName);
            ObjInstance objInstance = objGroup.SpawnObjInstance(path);
            if (objInstance == null)
            {
                // 这里要把 表格的id 凡在后面待定
                ObjsStartLoad.Add(serialId, objName);
                HotFixMudule.Res.LoadAsset(path, typeof(GameObject), LoadAssetCallbacks, ShowObjInfo.Create(serialId, serialId, objGroup, userData));
                return;
            }

            ObjBase objBase = objGroup.ShowObj(true, objInstance.Target, serialId, ObjTypes[objName], userData);
            ObjInfos.Add(serialId, ObjInfo.Create(objBase));
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="serialId">实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void HideObj(int serialId, object userData = null)
        {
            if (IsLoadingObj(serialId))
            {
                ObjsStartLoad.Remove(serialId);
                return;
            }

            ObjInfo objInfo = GetObjInfo(serialId);
            if (objInfo == null)
            {
                Log.Error(Utility.Text.Format("Can not find obj '{0}'.", serialId.ToString()));
            }

            InternalHideObj(objInfo, userData);
        }

        public void HideObj(ObjBase objBase, object userData = null)
        {
            HideObj(objBase.SerialId, userData);
        }

        /// <summary>
        /// 隐藏所有的实体。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void HideAllObj(object userData = null)
        {
            while (ObjInfos.Count > 0)
            {
                foreach (KeyValuePair<int, ObjInfo> objInfo in ObjInfos)
                {
                    InternalHideObj(objInfo.Value, userData);
                    break;
                }
            }
            ObjsStartLoad.Clear();
        }

        /// <summary>
        /// 获取实体信息。
        /// </summary>
        /// <param name="serialId">实体编号。</param>
        /// <returns>实体信息。</returns>
        private ObjInfo GetObjInfo(int serialId)
        {
            ObjInfo objInfo = null;
            if (ObjInfos.TryGetValue(serialId, out objInfo))
            {
                return objInfo;
            }
            return null;
        }

        /// <summary>
        /// 获取所有ObjType
        /// </summary>
        private void GetObjTypes()
        {
            ObjTypes = new Dictionary<string, Type>();
            Type[] Types = HotFixDllUtil.GetHotfixTypes();
            foreach (Type type in Types)
            {
                object[] objects = type.GetCustomAttributes(typeof(ObjAttribute), false);
                if (objects.Length != 0)
                {
                    ObjAttribute objAttribute = (ObjAttribute)objects[0];
                    if (ObjTypes.ContainsKey(objAttribute.ObjName))
                    {
                        Log.Error(Utility.Text.Format("uiName: {0} type: {1} is in UIFormTypes", objAttribute.ObjName, type));
                        continue;
                    }
                    ObjTypes.Add(objAttribute.ObjName, type);
                }
            }
        }

        private void InternalHideObj(ObjInfo objInfo, object userData)
        {
            while (objInfo.ChildObjs.Count > 0)
            {
                HideObj(objInfo.ChildObjs[0].SerialId, userData);
            }
            if (objInfo.Status == ObjStatus.Hidden)
            {
                return;
            }
            ObjBase obj = objInfo.Obj;
            objInfo.Status = ObjStatus.WillHide;
            obj.Handle.transform.SetParent(obj.ObjGroup.Handle.transform);
            obj.Handle.SetActive(false);
            obj.Hide(userData);
            obj.ObjGroup.RemoveObjEntity(obj);
            if (!ObjInfos.Remove(obj.SerialId))
            {
                Log.Error("obj info is unmanaged.");
            }
            objInfo.Status = ObjStatus.Hidden;
            RecycleQueue.Enqueue(objInfo);
        }

        private void LoadAssetSuccessCallback(string objAssetName, object objAsset, float duration, object userData)
        {
            ShowObjInfo showInfo = (ShowObjInfo)userData;
            if (showInfo == null)
            {
                Log.Error("Show obj info is invalid.");
            }

            if (!ObjsStartLoad.ContainsKey(showInfo.SerialId))
            {
                ReferencePool.Release(showInfo);
                // GameEntry.Res.UnloadAsset(entityAsset);
                return;
            }

            object ObjectInstance = UnityEngine.Object.Instantiate((UnityEngine.Object)objAsset);
            ObjInstance objInstance = ObjInstance.Create(objAssetName, objAsset, ObjectInstance);
            showInfo.ObjGroup.RegisterObj(objInstance, true);

            Type type = ObjTypes[ObjsStartLoad[showInfo.SerialId]];
            ObjBase objBase = showInfo.ObjGroup.ShowObj(true, objInstance.Target, showInfo.ObjId, type, showInfo.UserData);
            ObjInfos.Add(showInfo.SerialId, ObjInfo.Create(objBase));

            ObjsStartLoad.Remove(showInfo.SerialId);
            ReferencePool.Release(showInfo);
        }

        private void LoadAssetFailureCallback(string objAssetName, LoadResStatus status, string errorMessage)
        {
            // ShowObjInfo showEntityInfo = (ShowObjInfo)userData;
            // if (showEntityInfo == null)
            // {
            //     Log.Error("Show entity info is invalid.");
            // }

            // if (ObjsToReleaseOnLoad.Contains(showEntityInfo.SerialId))
            // {
            //     ObjsToReleaseOnLoad.Remove(showEntityInfo.SerialId);
            //     return;
            // }

            // ObjsBeingLoaded.Remove(showEntityInfo.ObjId);
            string appendErrorMessage = Utility.Text.Format("Load entity failure, asset name '{0}', status '{1}', error message '{2}'.", objAssetName, status.ToString(), errorMessage);
            Log.Fatal(appendErrorMessage);
        }
    }
}
