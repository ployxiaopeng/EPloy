using ETModel;
using GameFramework;
using GameFramework.Event;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [HotfixExtension]
    public class EntityHotfixComponent : Component
    {
        public override void Awake()
        {
            GameEntry.Event.Subscribe(HotfixEntityEvent.EventId, EntityEventFun);
            EntityHotfixLogicDic = new Dictionary<int, IHotfixEntityLogic>();
            EntityShowDataDic = new Dictionary<int, object>();
        }

        //int 是界面编号
        private Dictionary<int, object> EntityShowDataDic;
        //int 是界面编号
        private Dictionary<int, IHotfixEntityLogic> EntityHotfixLogicDic;

        private void EntityEventFun(object sender, GameEventArgs e)
        {
            if ((int)sender != HotfixEntityEvent.EventId) return;
            HotfixEntityEvent ne = (HotfixEntityEvent)e;
            try
            {
                switch (ne.entityFunType)
                {
                    case EntityFunType.Null:
                        break;
                    case EntityFunType.OnInit:
                        IHotfixEntityLogic IHotfixEntityLogic = CreateIHotfixEntityLogic(ne.entityModelLogic);
                        IHotfixEntityLogic.OnInit(ne.entityModelLogic);
                        break;
                    case EntityFunType.OnShow:
                        object obj = GetEntityOpenDataDic(ne.entityModelLogic.Id);
                        if (obj == null) break;
                        EntityHotfixLogicDic[ne.entityModelLogic.Id].OnShow(obj);
                        break;
                    case EntityFunType.OnHide:
                        if (!EntityHotfixLogicDic.ContainsKey(ne.entityModelLogic.Id)) break;
                        HotfixReferencePool.Release(EntityHotfixLogicDic[ne.entityModelLogic.Id]);
                        EntityHotfixLogicDic.Remove(ne.entityModelLogic.Id);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception err)
            {
                Log.Error("entity: {0} fun: {1}  Err: {2}", ne.entityModelLogic.HotfixEntityLogicName, ne.entityFunType, err.ToString());
            }
        }

        private IHotfixEntityLogic CreateIHotfixEntityLogic(EntityModelLogic entityFormLogic)
        {
            string name = entityFormLogic.HotfixEntityLogicName;
            Type hotfixType = Utility.Assembly.GetType(name);
            if (hotfixType == null) Log.Error(name);
            IHotfixEntityLogic IHotfixEntityLogic = (IHotfixEntityLogic)HotfixReferencePool.Acquire(hotfixType);
            if (EntityHotfixLogicDic.ContainsKey(entityFormLogic.Id))
            {
                EntityHotfixLogicDic[entityFormLogic.Id] = IHotfixEntityLogic;
                return IHotfixEntityLogic;
            }
            EntityHotfixLogicDic.Add(entityFormLogic.Id, IHotfixEntityLogic);
            return IHotfixEntityLogic;
        }

        private object GetEntityOpenDataDic(int entitySerialId)
        {
            object obj = null;
            if (EntityShowDataDic.ContainsKey(entitySerialId))
            {
                obj = EntityShowDataDic[entitySerialId];
                EntityShowDataDic.Remove(entitySerialId);
            }
            else Log.Error("没找到实体数据：" + entitySerialId);
            return obj;
        }
        public void OnShow(int entitySerialId, object userData)
        {
            try
            {
                if (EntityShowDataDic.ContainsKey(entitySerialId))
                {
                    EntityShowDataDic[entitySerialId] = userData;
                    return;
                }
                EntityShowDataDic.Add(entitySerialId, userData);
            }
            catch (Exception e)
            {
                Log.Error("entity界面编号: {0} OnOpen  Err: {1}", entitySerialId, e.ToString());
            }
        }

        public override void Update()
        {
            foreach (var logic in EntityHotfixLogicDic)
            {
                if (logic.Value.EntityData != null)
                {
                    logic.Value.OnUpdate();
                }
            }
        }

        public void HideAllLoadedEntities()
        {
            foreach (var logic in EntityHotfixLogicDic)
                HotfixReferencePool.Release(logic.Value);
            EntityHotfixLogicDic.Clear();
            EntityShowDataDic.Clear();
        }
    }
}