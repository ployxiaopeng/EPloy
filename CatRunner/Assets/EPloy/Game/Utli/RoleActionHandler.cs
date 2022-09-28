using Pathfinding;
using System;
using UnityEngine;


//角色行动处理组件
public class RoleActionHandler : MonoBehaviour
{
    private Seeker seeker;
    private SimpleSmoothModifier simpleSmooth;
    private Animator animator;

    private object Entity;
    private Action<int, object> ActionHarmHandler;
    private Action<int, object> ActionOverHandler;
    public static RoleActionHandler AddActionHandler(GameObject gameObject)
    {
        RoleActionHandler actionHandler = gameObject.GetComponent<RoleActionHandler>();
        if (actionHandler == null) actionHandler = gameObject.AddComponent<RoleActionHandler>();
        actionHandler.animator = gameObject.GetComponent<Animator>();
        actionHandler.seeker = gameObject.GetComponent<Seeker>();
        if (actionHandler.seeker == null)
        {
            actionHandler.seeker = gameObject.AddComponent<Seeker>();
            actionHandler.simpleSmooth = gameObject.AddComponent<SimpleSmoothModifier>();
        }
        else
        {
            actionHandler.simpleSmooth = gameObject.GetComponent<SimpleSmoothModifier>();
        }
        actionHandler.simpleSmooth.maxSegmentLength =1f;
        return actionHandler;
    }

    public Path OnPathfinding(Vector3 targetPos, OnPathDelegate onPathDelegate)
    {
        return seeker.StartPath(transform.position, targetPos, onPathDelegate);
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

