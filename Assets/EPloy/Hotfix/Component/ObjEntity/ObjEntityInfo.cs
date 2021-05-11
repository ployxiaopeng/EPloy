
using System.Collections.Generic;

namespace EPloy.ObjEntity
{
    /// <summary>
    /// 实体状态。
    /// </summary>
    public enum EntityStatus : byte
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
    public sealed class ObjEntityInfo : IReference
    {
        public ObjEntityBase Entity
        {
            get;
            private set;
        }

        public EntityStatus Status
        {
            get;
            set;
        }

        public ObjEntityBase ParentEntity
        {
            get;
            private set;
        }

        public List<ObjEntityBase> ChildEntities
        {
            get;
            private set;
        }


        public ObjEntityInfo()
        {
            Entity = null;
            Status = EntityStatus.Unknown;
            ParentEntity = null;
            ChildEntities = new List<ObjEntityBase>();
        }


        public static ObjEntityInfo Create(ObjEntityBase entity)
        {
            if (entity == null)
            {
                Log.Error("Entity is invalid.");
                return null;
            }

            ObjEntityInfo objEntityInfo = ReferencePool.Acquire<ObjEntityInfo>();
            objEntityInfo.Entity = entity;
            objEntityInfo.Status = EntityStatus.WillInit;
            return objEntityInfo;
        }

        public void Clear()
        {
            Entity = null;
            Status = EntityStatus.Unknown;
            ParentEntity = null;
            ChildEntities.Clear();
        }


        public void AddChildEntity(ObjEntityBase childEntity)
        {
            if (ChildEntities.Contains(childEntity))
            {
                Log.Error("Can not add child entity which is already exist.");
                return;
            }

            ChildEntities.Add(childEntity);
        }

        public void RemoveChildEntity(ObjEntityBase childEntity)
        {
            if (!ChildEntities.Remove(childEntity))
            {
                Log.Error("Can not remove child entity which is not exist.");
                return;
            }
        }
    }
}
