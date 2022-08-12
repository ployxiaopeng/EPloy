using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class UIEventListener : EventTrigger
{
    private static UIEventListener listener = null;
    public Action<GameObject> onClick;

    public Action<GameObject> onClickDown;
    public Action<GameObject, PointerEventData> onArgClickDown;

    public Action<GameObject> onClickUp;
    public Action<GameObject, PointerEventData> onArgClickUp;

    public Action<GameObject> onDrag;
    public Action<GameObject, PointerEventData> onArgDrag;

    public Action<GameObject> onPointerEnter;
    public Action<GameObject, PointerEventData> onArgPointerEnter;

    public Action<GameObject> onPointerExit;
    public Action<GameObject, PointerEventData> onArgPointerExit;

    public Action<GameObject> onBeginDrag;
    public Action<GameObject, PointerEventData> onArgBeginDrag;

    public Action<GameObject> onEndDrag;
    public Action<GameObject, PointerEventData> onArgEndDrag;


    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        onClick?.Invoke(gameObject);
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        onClickDown?.Invoke(gameObject);
        onArgClickDown?.Invoke(gameObject, eventData);

    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        onClickUp?.Invoke(gameObject);
        onArgClickUp?.Invoke(gameObject, eventData);

    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        onDrag?.Invoke(gameObject);
        onArgDrag?.Invoke(gameObject, eventData);

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        onPointerEnter?.Invoke(gameObject);
        onArgPointerEnter?.Invoke(gameObject, eventData);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        onPointerExit?.Invoke(gameObject);
        onArgPointerExit?.Invoke(gameObject, eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        onBeginDrag?.Invoke(gameObject);
        onArgBeginDrag?.Invoke(gameObject, eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        onEndDrag?.Invoke(gameObject);
        onArgEndDrag?.Invoke(gameObject, eventData);
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

