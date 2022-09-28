using Pathfinding;
using System;
using UnityEngine;


//角色行动处理组件
public class RoleActionHandler : MonoBehaviour
{
    private Seeker seeker;
    private Animator animator;

    private object Entity;
    private Action<int, object> ActionHarmHandler;
    private Action<int, object> ActionOverHandler;
    public static RoleActionHandler AddAnimationHandler(GameObject gameObject)
    {
        RoleActionHandler animationHandler = gameObject.GetComponent<RoleActionHandler>();
        if (animationHandler == null) animationHandler = gameObject.AddComponent<RoleActionHandler>();
        animationHandler.animator = gameObject.GetComponent<Animator>();
        animationHandler.seeker = gameObject.AddComponent<Seeker>();
        return animationHandler;
    }

    public void OnMove(float speed)
    {
        animator.SetFloat("Move", speed);
    }

    public void OnAtt(string name)
    {
        animator.SetTrigger(name);
    }
    public void TriggerHarmEvent(int arg)
    {
        ActionHarmHandler?.Invoke(arg, Entity);
    }

    public void TriggerOverEvent(int arg)
    {
        ActionOverHandler?.Invoke(arg, Entity);
    }

    public void RegisterHandler(object entity, Action<int, object> harmHandler, Action<int, object> overHandler)
    {
        Entity = entity;
        ActionHarmHandler = harmHandler;
        ActionOverHandler = overHandler;
    }

    public void RemoveHandler()
    {
        ActionOverHandler = null;
        ActionHarmHandler = null;
        Entity = null;
    }
}

