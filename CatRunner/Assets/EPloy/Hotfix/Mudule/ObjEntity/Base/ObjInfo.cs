
using System.Collections.Generic;

namespace EPloy.Hotfix.Obj
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

        public ObjGroup ObjGroup
        {
            get;
            private set;
        }

        public ObjStatus Status
        {
            get;
            set;
        }

        public ObjInfo()
        {
            Obj = null;
            Status = ObjStatus.Unknown;
        }

        public static ObjInfo Create(ObjBase entity, ObjGroup objGroup)
        {
            if (entity == null)
            {
                Log.Error("Entity is invalid.");
                return null;
            }

            ObjInfo objEntityInfo = ReferencePool.Acquire<ObjInfo>();
            objEntityInfo.Obj = entity;
            objEntityInfo.ObjGroup = objGroup;
            objEntityInfo.Status = ObjStatus.WillInit;
            return objEntityInfo;
        }

        public void Clear()
        {
            Obj = null;
            Status = ObjStatus.Unknown;
        }
    }
}
