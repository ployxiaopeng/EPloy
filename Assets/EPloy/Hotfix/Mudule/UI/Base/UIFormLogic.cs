﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// ui子逻辑 当主逻辑太大可拆分
    /// </summary>
    public abstract class UIFormLogic : IReference
    {
        private Transform logicTransform;

        protected Transform transform
        {
            get { return logicTransform; }
        }

        public bool isActive
        {
            get
            {
                if (logicTransform)
                {

                }

                return transform.gameObject.activeInHierarchy;
            }
        }

        /// <summary>
        /// 生成ui子逻辑
        /// </summary>
        /// <param name="uIForm"></param>
        /// <param name="transform"></param>
        public void CreateLogic(UIForm uIForm, Transform transform)
        {
            logicTransform = transform;
            Create();
        }

        public virtual void ShowView()
        {
            logicTransform.gameObject.SetActive(true);
        }

        public virtual void CloseView()
        {
            logicTransform.gameObject.SetActive(false);
        }

        protected virtual void Create()
        {

        }

        public virtual void Clear()
        {
            logicTransform = null;
        }
    }
}