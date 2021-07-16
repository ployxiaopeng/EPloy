
using System.Collections.Generic;

namespace EPloy.Obj
{
    /// <summary>
    /// 实体状态。
    /// </summary>
    public enum ObjStatus : byte
    {
        Unknown = 0,
        WillInit,
        Inited,
        WillShow,
        Showed,
        WillHide,
        Hidden,
        WillRecycle,
        Recycled
    }

    /// <summary>
    /// 实体信息。
    /// </summary>
    public sealed class ObjInfo : IReference
    {
        public ObjBase Obj
        {
            get;
            private set;
        }

        public ObjStatus Status
        {
            get;
            set;
        }

        public ObjBase ParentObj
        {
            get;
            private set;
        }

        public List<ObjBase> ChildObjs
        {
            get;
            private set;
        }

        public ObjInfo()
        {
            Obj = null;
            Status = ObjStatus.Unknown;
            ParentObj = null;
            ChildObjs = new List<ObjBase>();
        }

        public static ObjInfo Create(ObjBase entity)
        {
            if (entity == null)
            {
                Log.Error("Entity is invalid.");
                return null;
            }

            ObjInfo objEntityInfo = ReferencePool.Acquire<ObjInfo>();
            objEntityInfo.Obj = entity;
            objEntityInfo.Status = ObjStatus.WillInit;
            return objEntityInfo;
        }

        public void Clear()
        {
            Obj = null;
            Status = ObjStatus.Unknown;
            ParentObj = null;
            ChildObjs.Clear();
        }

        public void AddChildEntity(ObjBase childEntity)
        {
            if (ChildObjs.Contains(childEntity))
            {
                Log.Error("Can not add child entity which is already exist.");
                return;
            }

            ChildObjs.Add(childEntity);
        }

        public void RemoveChildEntity(ObjBase childEntity)
        {
            if (!ChildObjs.Remove(childEntity))
            {
                Log.Error("Can not remove child entity which is not exist.");
                return;
            }
        }
    }
}
