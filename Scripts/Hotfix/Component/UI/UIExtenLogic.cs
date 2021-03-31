using GameFramework;
using UnityEngine;


namespace ETHotfix
{
    public class UIExtenLogic : IReference
    {
        public Transform transform { get; protected set; }

        public UIExtenLogic(Transform _transform)
        {
            transform = _transform;
            Find();
            InitUIText();
            AddEvent();
        }

        protected virtual void Find() { } //用于寻找UI组件
        protected virtual void InitUIText() { } //  用于初始化UI文本，
        protected virtual void AddEvent() { }
        public void Clear()
        {
            transform = null;
        }
        public void SetActive(bool type)
        {
            transform.gameObject.SetActive(type);
        }
    }
}


