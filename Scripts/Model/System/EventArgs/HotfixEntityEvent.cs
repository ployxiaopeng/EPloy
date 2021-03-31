using GameFramework.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    public enum EntityFunType
    {
        Null,
        OnInit,
        OnShow,
        OnHide,
    }
    public class HotfixEntityEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(HotfixEntityEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public EntityModelLogic  entityModelLogic { get; private set; }
        public EntityFunType  entityFunType { get; private set; }
        public void SetEventData(EntityFunType funType, EntityModelLogic modelLogic)
        {
            entityFunType = funType; entityModelLogic = modelLogic;
        }

        public override void Clear()
        {
            entityFunType = EntityFunType.Null;
            entityModelLogic = null;
        }
    }
}