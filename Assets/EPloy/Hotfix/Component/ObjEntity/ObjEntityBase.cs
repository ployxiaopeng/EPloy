using System;
using UnityEngine;

namespace EPloy.ObjEntity
{
    /// <summary>
    /// obj实体基类
    /// </summary>
    public abstract class ObjEntityBase : IReference
    {
        /// <summary>
        ///obj实体编号。
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 实体实例。
        /// </summary>
        public GameObject Handle
        {
            get;
            private set;
        }

        /// <summary>
        /// 所属的实体组。
        /// </summary>
        public ObjEntityGroup EntityGroup
        {
            get;
            private set;
        }

        protected Transform transform
        {
            get
            {
                return Handle.transform;
            }
        }

        /// <summary>
        /// 实体初始化。
        /// </summary>
        /// <param name="entityId">实体编号。</param>
        /// <param name="entityAssetName">实体资源名称。</param>
        /// <param name="entityGroup">实体所属的实体组。</param>
        /// <param name="isNewInstance">是否是新实例。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void Initialize(bool isNew, GameObject handle, int entityId, ObjEntityGroup entityGroup, object userData)
        {
            EntityGroup = entityGroup;
            if (isNew)
            {
                Id = entityId;
                Handle = handle;
                Create();
            }

            Activate(userData);
        }
        /// <summary>
        /// obj生成。
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// obj激活
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public abstract void Activate(object userData);


        /// <summary>
        /// 实体隐藏。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public abstract void Hide(object userData);


        /// <summary>
        /// 实体解除子实体。
        /// </summary>
        /// <param name="childEntity">解除的子实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void OnDetached(ObjEntityBase childEntity, object userData)
        {
        }

        /// <summary>
        /// 实体附加子实体。
        /// </summary>
        /// <param name="childEntity">子实体。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void OnAttachTo(ObjEntityBase childEntity, object userData)
        {

        }


        /// <summary>
        /// 实体轮询。
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// Obj清理回收。
        /// </summary>
        public virtual void Clear() { }
    }
}
