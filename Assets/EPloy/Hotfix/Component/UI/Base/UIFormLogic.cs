using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EPloy
{
    /// <summary>
    /// ui子逻辑 当主逻辑太大可拆分
    /// </summary>
    public abstract class UIFormLogic : IReference
    {
        protected UIForm UIForm;
        private Transform LogicTransform;
        protected Transform transform
        {
            get
            {
                return LogicTransform;
            }
        }

        /// <summary>
        /// 生成ui子逻辑
        /// </summary>
        /// <param name="uIForm"></param>
        /// <param name="transform"></param>
        public void CreateLogic(UIForm uIForm,Transform transform )
        {
            LogicTransform = transform;
            UIForm = uIForm;
            Create();
        }
   
        protected virtual  void Create()
        {

        }
        public void Clear()
        {
            UIForm = null;
            LogicTransform = null;
        }

    }
}