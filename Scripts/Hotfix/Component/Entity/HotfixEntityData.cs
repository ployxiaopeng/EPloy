using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
namespace ETHotfix
{
    /// <summary>
    /// 热更新层实体数据
    /// </summary>
    public abstract class HotfixEntityData : IReference
    {
        //实体数据编号
        public int EntityId { get; set; }
        public Transform ParentLevel { get; protected set; }
        public DREntity DREntity { get; protected set; }
        public Vector3 Position { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        public HotfixEntityLogic hotfixEntityLogic { get; set; }

        public float DestroyTime
        {
            get { return DREntity.DestroyTime; }
        }

        /// <summary>
        /// 设置实体数据
        /// </summary>
        /// <param name="_DREntity"></param>
        /// <param name="parentLevel"></param>
        public abstract void SetDREntityData(DREntity _dREntity, Transform parentLevel);

        /// <summary>
        /// 资源编号。
        /// </summary>
        public int AssetId
        {
            get
            {
                return DREntity.Id;
            }
        }
        public string AssetType
        {
            get
            {
                return DREntity.AssetType;
            }
        }

        public virtual void Clear()
        {
            ParentLevel = null;
            DREntity = null;
            Position = Vector3.zero;
            Rotation = Vector3.zero;
            EntityId = 0;
        }
    }
}