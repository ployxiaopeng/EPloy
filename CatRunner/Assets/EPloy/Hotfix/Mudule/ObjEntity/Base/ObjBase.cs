using EPloy.Hotfix.Table;
using System;
using UnityEngine;

namespace EPloy.Hotfix.Obj
{
    /// <summary>
    /// objData基类
    /// </summary>
    public abstract class ObjBase : IReference
    {
        /// <summary>
        /// 实例是否存在
        /// </summary>
        public bool isInstance
        {
            get { return Obj != null; }
        }

        /// <summary>
        ///obj实体编号。
        /// </summary>
        public uint SerialId
        {
            get;
            protected set;
        }

        /// <summary>
        /// 表格数据 
        /// </summary>
        public DREntity DtObj
        {
            get;
            protected set;
        }

        /// <summary>
        /// 所属的实体组。
        /// </summary>
        public ObjGroup ObjGroup
        {
            get;
            private set;
        }

        /// <summary>
        /// 实体本身
        /// </summary>
        public GameObject Obj
        {
            get;
            private set;
        }

        protected int ObjId;
        protected Vector3 initPostion;
        protected Vector3 initRotate;
        protected Vector3 initScale;
        protected Transform initParent;
        protected Action<ObjBase> instanceCallBack;

        protected void InitData(int objId, Transform parent)
        {
            this.ObjId = objId;
            initScale = Vector3.one;
            initRotate = Vector3.zero;
            initPostion = Vector3.zero;
            initParent = parent;
            DtObj = objId.GetDtObj();
        }

        public void SetInstance(GameObject obj)
        {
            Obj = obj;
            obj.SetActive(true);
            obj.transform.SetParent(initParent, false);
            obj.transform.localPosition = initPostion;
            obj.transform.localEulerAngles = initRotate;
            obj.transform.localScale = initScale;
            instanceCallBack?.Invoke(this);
        }

        /// <summary>
        /// 设置编号归宿数据
        /// </summary>
        public void SetSerialData(uint serialId, ObjGroup objGroup, Action<ObjBase> callBack)
        {
            SerialId = serialId;
            ObjGroup = objGroup;
            instanceCallBack = callBack;
        }

        public virtual void Clear()
        {
            initScale = Vector3.one;
            initRotate = Vector3.zero;
            initPostion = Vector3.zero;
            initParent = null;
            DtObj = null;
            SerialId = 0;
            ObjGroup = null;
            Obj = null;
        }
    }
}
