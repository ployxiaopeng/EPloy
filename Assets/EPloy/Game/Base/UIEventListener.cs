using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace EPloy
{
    public class UIEventListener : EventTrigger
    {
        private static UIEventListener listener = null;
        public delegate void UIListenerDelegate(GameObject go);
        public delegate void UIArgListenerDelegate(GameObject go, PointerEventData eventData);
        public UIListenerDelegate onClick;

        public UIListenerDelegate onClickDown;
        public UIArgListenerDelegate onArgClickDown;

        public UIListenerDelegate onClickUp;
        public UIArgListenerDelegate onArgClickUp;
        
        public UIListenerDelegate onDrag;
        public UIArgListenerDelegate onArgDrag;
        
        public UIListenerDelegate onPointerEnter;
        public UIArgListenerDelegate onArgPointerEnter;

        public UIListenerDelegate onPointerExit;
        public UIArgListenerDelegate onArgPointerExit;

        public UIListenerDelegate onBeginDrag;
        public UIArgListenerDelegate onArgBeginDrag;

        public UIListenerDelegate onEndDrag;
        public UIArgListenerDelegate onArgEndDrag;
        

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            onClick?.Invoke(gameObject);
        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            onClickDown?.Invoke(gameObject);
            onArgClickDown?.Invoke(gameObject,eventData);

        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            onClickUp?.Invoke(gameObject);
            onArgClickUp?.Invoke(gameObject,eventData);

        }
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            onDrag?.Invoke(gameObject);
            onArgDrag?.Invoke(gameObject,eventData);

        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            onPointerEnter?.Invoke(gameObject);
            onArgPointerEnter?.Invoke(gameObject,eventData);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            onPointerExit?.Invoke(gameObject);
            onArgPointerExit?.Invoke(gameObject,eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            onBeginDrag?.Invoke(gameObject);
            onArgBeginDrag?.Invoke(gameObject,eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            onEndDrag?.Invoke(gameObject);
            onArgEndDrag?.Invoke(gameObject,eventData);
        }

        public static UIEventListener Get(GameObject Go)
        {
            listener = Go.GetComponent<UIEventListener>();
            if (listener == null)
                listener = Go.gameObject.AddComponent<UIEventListener>();
            return listener;
        }

        public static void RemoveListener(GameObject Go)
        {
            listener = Go.GetComponent<UIEventListener>();
            if (listener == null) return;
            Destroy(listener);
        }
    }
}
