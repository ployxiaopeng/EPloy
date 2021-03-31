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

        protected virtual void Find() { } //����Ѱ��UI���
        protected virtual void InitUIText() { } //  ���ڳ�ʼ��UI�ı���
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


