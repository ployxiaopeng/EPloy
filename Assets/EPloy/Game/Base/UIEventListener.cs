using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EPloy
{
    public class UIEventListener : EventTrigger
    {
        private static UIEventListener listener = null;
        public delegate void UIListenerDelegate(GameObject go);
        public delegate void UIEventListenerDelegate(GameObject go, object obj);
        public UIListenerDelegate onClick;
        public UIEventListenerDelegate onEventClick;

        public UIListenerDelegate onClickDown;
        public UIEventListenerDelegate onEventClickDown;

        public UIListenerDelegate onClickUp;
        public UIEventListenerDelegate onEventClickUp;

        public UIListenerDelegate onDrag;
        public UIEventListenerDelegate onEventDrag;

        public UIListenerDelegate onPointerEnter;
        public UIEventListenerDelegate onEventPointerEnter;

        public UIListenerDelegate onPointerExit;
        public UIEventListenerDelegate onEventPointerExit;

        public UIListenerDelegate onBeginDrag;
        public UIEventListenerDelegate onEventBeginDrag;

        public UIListenerDelegate onEndDrag;
        public UIEventListenerDelegate onEventEndDrag;

        private object EventObj = null;

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (onClick != null) onClick(gameObject);
            if (onEventClick != null) onEventClick(gameObject, EventObj);

        }
        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (onClickDown != null) onClickDown(gameObject);
            if (onEventClickDown != null) onEventClickDown(gameObject, EventObj);

        }
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (onClickUp != null) onClickUp(gameObject);
            if (onEventClickUp != null) onEventClickUp(gameObject, EventObj);

        }
        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            if (onDrag != null) onDrag(gameObject);
            if (onEventDrag != null) onEventDrag(gameObject, EventObj);

        }
        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (onPointerEnter != null) onPointerEnter(gameObject);
            if (onEventPointerEnter != null) onEventPointerEnter(gameObject, EventObj);
        }
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (onPointerExit != null) onPointerExit(gameObject);
            if (onEventPointerExit != null) onEventPointerExit(gameObject, EventObj);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (onBeginDrag != null) onBeginDrag(gameObject);
            if (onEventBeginDrag != null) onEventBeginDrag(gameObject, EventObj);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (onEndDrag != null) onEndDrag(gameObject);
            if (onEventEndDrag != null) onEventEndDrag(gameObject, EventObj);
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
            listener.onClick = null;
            listener.onEventClick = null;
        }


        public static UIEventListener Get(GameObject Go, object obj)
        {
            listener = Go.GetComponent<UIEventListener>();
            if (listener == null)
                listener = Go.gameObject.AddComponent<UIEventListener>();
            listener.EventObj = obj;
            return listener;
        }
    }
}
