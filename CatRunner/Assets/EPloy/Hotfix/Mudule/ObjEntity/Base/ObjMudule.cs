using EPloy.Game.ObjectPool;
using EPloy.Game.Res;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EPloy.Hotfix.Obj;
using EPloy.Game;
using EPloy.Game.Obj;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 实例物体组件。
    /// </summary>
    public sealed partial class ObjMudule : IHotfixModule
    {
        private uint serialId;
        private Dictionary<uint, ObjInfo> ObjInfos;
        private Dictionary<ObjGroupName, ObjGroup> ObjGroups;
        private Dictionary<uint, string> ObjsStartLoad;
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

        public override void Awake()
        {
            serialId = 0;
            ObjParent = GameStart.Instance.transform.Find("Obj").transform;
            ObjInfos = new Dictionary<uint, ObjInfo>();
            ObjGroups = new Dictionary<ObjGroupName, ObjGroup>();
            ObjsStartLoad = new Dictionary<uint, string>();
            RecycleQueue = new Queue<ObjInfo>();
            LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback);
            foreach (var name in Enum.GetValues(typeof(ObjGroupName)))
            {
                AddObjGroup((ObjGroupName)name);
            }
            HotFixMudule.RegisterUpdate(ObjUpdate);
        }

        public void ObjUpdate()
        {
            while (RecycleQueue.Count > 0)
            {
                ObjInfo objInfo = RecycleQueue.Dequeue();
                objInfo.Status = ObjStatus.Recycled;
                objInfo.ObjGroup.UnspawnObj(objInfo.Obj);
                ReferencePool.Release(objInfo.Obj);
                ReferencePool.Release(objInfo);
            }
        }

        public override void OnDestroy()
        {
            HideAllObj();
            ObjGroups.Clear();
            ObjsStartLoad.Clear();
            RecycleQueue.Clear();
            ObjInfos.Clear();
            ObjParent = null;
            LoadAssetCallbacks = null;
            HotFixMudule.RemoveUpdate(ObjUpdate);
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
            string poolName = UtilText.Format("ObjEntity{0}Pool", objGroupName.ToString());
            ObjectPoolBase objEntityPool = GameModule.ObjectPool.CreateObjectPool(typeof(ObjectInstance), poolName);
            ObjGroup objEntityGroup = new ObjGroup(objGroupName, objEntityPool, ObjParent);
            ObjGroups.Add(objGroupName, objEntityGroup);
        }

        /// <summary>
        /// 是否存在实体。
        /// </summary>
        /// <param name="serialId">实体编号。</param>
        /// <returns>是否存在实体。</returns>
        public bool HasObj(uint serialId)
        {
            return ObjInfos.ContainsKey(serialId);
        }

        /// <summary>
        /// 获取所有已加载的实体。
        /// </summary>
        /// <returns>所有已加载的实体。</returns>
        public ObjBase[] GetAllLoadedObjs()
        {
            int index = 0;
            ObjBase[] results = new ObjBase[ObjInfos.Count];
            foreach (KeyValuePair<uint, ObjInfo> entityInfo in ObjInfos)
            {
                results[index++] = entityInfo.Value.Obj;
            }

            return results;
        }

        /// <summary>
        /// 获取所有正在加载实体的编号。
        /// </summary>
        /// <returns>所有正在加载实体的编号。</returns>
        public uint[] GetAllLoadingObjIds()
        {
            int index = 0;
            uint[] results = new uint[ObjsStartLoad.Count];
            foreach (KeyValuePair<uint, string> entityBeingLoaded in ObjsStartLoad)
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
        public bool IsLoadingObj(uint objId)
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
        public void ShowObj(ObjBase objData, Action<ObjBase> callBack = null, ObjGroupName objGroupName = ObjGroupName.Default)
        {
            if (string.IsNullOrEmpty(objData.DtObj.AssetName))
            {
                Log.Error("Obj asset name is invalid.");
                return;
            }
            serialId++;
            if (HasObj(serialId))
            {
                Log.Error(UtilText.Format("serialIdId '{0}' is already exist.", serialId.ToString()));
                return;
            }

            if (IsLoadingObj(serialId))
            {
                Log.Error(UtilText.Format("serialId '{0}' is already being loaded.", serialId.ToString()));
                return;
            }
            ObjGroup objGroup = GetObjGroup(objGroupName);
            if (objGroup == null)
            {
                Log.Error(UtilText.Format("Obj group '{0}' is not exist.", objGroupName));
            }
            objData.SetSerialData(serialId, objGroup, callBack);
            string path = UtilAsset.GetObjAsset(objData.DtObj.AssetName);
            ObjectInstance objInstance = objGroup.SpawnObjInstance(path);
            if (objInstance == null)
            {
                ObjsStartLoad.Add(serialId, objData.DtObj.AssetName);
                GameModule.Res.LoadAsset(path, typeof(GameObject), LoadAssetCallbacks, objData);
                return;
            }

            ObjBase objBase = objGroup.ShowObj(true, objInstance.Target, objData);
            ObjInfos.Add(serialId, ObjInfo.Create(objBase, objGroup));
        }

        /// <summary>
        /// 隐藏实体。
        /// </summary>
        /// <param name="serialId">实体编号。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void HideObj(uint serialId)
        {
            if (IsLoadingObj(serialId))
            {
                ObjsStartLoad.Remove(serialId);
                return;
            }

            ObjInfo objInfo = GetObjInfo(serialId);
            if (objInfo == null)
            {
                Log.Error(UtilText.Format("Can not find obj '{0}'.", serialId.ToString()));
            }

            InternalHideObj(objInfo);
        }

        public void HideObj(ObjBase objBase)
        {
            HideObj(objBase.SerialId);
        }

        /// <summary>
        /// 隐藏所有的实体。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void HideAllObj()
        {
            while (ObjInfos.Count > 0)
            {
                foreach (KeyValuePair<uint, ObjInfo> objInfo in ObjInfos)
                {
                    InternalHideObj(objInfo.Value);
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
        private ObjInfo GetObjInfo(uint serialId)
        {
            ObjInfo objInfo = null;
            if (ObjInfos.TryGetValue(serialId, out objInfo))
            {
                return objInfo;
            }
            return null;
        }

        private void InternalHideObj(ObjInfo objInfo)
        {
            if (objInfo.Obj.Obj)
            {
                if (objInfo.Status == ObjStatus.Hidden)
                {
                    return;
                }
                ObjBase obj = objInfo.Obj;
                objInfo.Status = ObjStatus.WillHide;

                obj.Obj.transform.SetParent(objInfo.ObjGroup.Handle.transform);
                obj.Obj.SetActive(false);
                objInfo.ObjGroup.RemoveObjEntity(obj);
                if (!ObjInfos.Remove(obj.SerialId))
                {
                    Log.Error("obj info is unmanaged.");
                }
                objInfo.Status = ObjStatus.Hidden;
                RecycleQueue.Enqueue(objInfo);
            }
        }

        private void LoadAssetSuccessCallback(string objAssetName, object objAsset, float duration, object userData)
        {
            ObjBase showInfo = (ObjBase)userData;
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
            ObjectInstance objInstance = Game.Obj.ObjectInstance.Create(objAssetName, objAsset, ObjectInstance);
            showInfo.ObjGroup.RegisterObj(objInstance, true);

            ObjBase objBase = showInfo.ObjGroup.ShowObj(true, objInstance.Target, showInfo);
            ObjInfos.Add(showInfo.SerialId, ObjInfo.Create(objBase, showInfo.ObjGroup));

            ObjsStartLoad.Remove(showInfo.SerialId);
        }

        private void LoadAssetFailureCallback(string objAssetName, LoadResStatus status, string errorMessage)
        {
            string appendErrorMessage = UtilText.Format("Load entity failure, asset name '{0}', status '{1}', error message '{2}'.", objAssetName, status.ToString(), errorMessage);
            Log.Fatal(appendErrorMessage);
        }
    }
}
