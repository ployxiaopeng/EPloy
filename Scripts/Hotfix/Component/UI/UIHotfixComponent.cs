using ETModel;
using GameFramework;
using GameFramework.Event;
using System;
using System.Collections.Generic;

namespace ETHotfix
{
    [HotfixExtension]
    public class UIHotfixComponent : Component
    {
        public override void Awake()
        {
            GameEntry.Event.Subscribe(HotfixUIEvent.EventId, UIEventFun);
            UIHotfixLogicDic = new Dictionary<int, IUIHotfixLogic>();
            UIOpenDataDic = new Dictionary<int, object>();
        }

        //int 是界面编号
        private Dictionary<int, object> UIOpenDataDic;
        //int 是界面编号
        private Dictionary<int, IUIHotfixLogic> UIHotfixLogicDic;

        private void UIEventFun(object sender, GameEventArgs e)
        {
            if ((int)sender != HotfixUIEvent.EventId) return;
            HotfixUIEvent ne = (HotfixUIEvent)e;
            try
            {
                switch (ne.uIFunType)
                {
                    case UIFunType.Null:
                        break;
                    case UIFunType.OnInit:
                        IUIHotfixLogic IUIHotfixLogic = CreateIUIHotfixLogic(ne.uIFormLogic);
                        IUIHotfixLogic.OnInit(ne.uIFormLogic);
                        break;
                    case UIFunType.OnOpen:
                        UIHotfixLogicDic[ne.uIFormLogic.SerialId].OnOpen(GetUIOpenDataDic(ne.uIFormLogic.SerialId));
                        break;
                    case UIFunType.OnClose:
                        HotfixReferencePool.Release(UIHotfixLogicDic[ne.uIFormLogic.SerialId]);
                        UIHotfixLogicDic.Remove(ne.uIFormLogic.SerialId);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception err)
            {
                Log.Error("UI: {0} fun: {1}  Err: {2}", ne.uIFormLogic.HotfixWndName, ne.uIFunType, err.ToString());
            }
        }

        private IUIHotfixLogic CreateIUIHotfixLogic(UIFormLogic uIFormLogic)
        {
            string name = string.Format("ETHotfix.{0}", uIFormLogic.HotfixWndName);
            Type hotfixType = Utility.Assembly.GetType(name);
            if (hotfixType == null) Log.Error(name);
            IUIHotfixLogic IUIHotfixLogic = (IUIHotfixLogic)HotfixReferencePool.Acquire(hotfixType);
            UIHotfixLogicDic.Add(uIFormLogic.SerialId, IUIHotfixLogic);
            return IUIHotfixLogic;
        }

        private object GetUIOpenDataDic(int uiSerialId)
        {
            object obj = null;
            if (UIOpenDataDic.ContainsKey(uiSerialId))
            {
                obj = UIOpenDataDic[uiSerialId];
                UIOpenDataDic.Remove(uiSerialId);
            }
            return obj;
        }
        public void OnOpen(int uiSerialId, object userData)
        {
            try
            {
                if (UIOpenDataDic.ContainsKey(uiSerialId))
                {
                    UIOpenDataDic[uiSerialId] = userData;
                    return;
                }
                UIOpenDataDic.Add(uiSerialId, userData);
            }
            catch (Exception e)
            {
                Log.Error("UI界面编号: {0} OnOpen  Err: {1}", uiSerialId, e.ToString());
            }
        }
        public void OnRefocus(int uiSerialId, object userData)
        {
            try
            {
                if (UIHotfixLogicDic.ContainsKey(uiSerialId))
                    UIHotfixLogicDic[uiSerialId].OnRefocus(userData);
            }
            catch (Exception e)
            {
                Log.Error("UI界面编号: {0} OnRefocus  Err: {1}", uiSerialId, e.ToString());
            }
        }
        public override void Update()
        {
            foreach (var logic in UIHotfixLogicDic)
            {
                if (logic.Value.UIFormLogic.Visible)
                    logic.Value.OnUpdate();
            }
        }
    }
}