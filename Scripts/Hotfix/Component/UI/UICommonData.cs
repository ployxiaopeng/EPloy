using GameFramework;
using UnityEngine;

namespace ETHotfix
{
    public abstract class UICommonData : IReference
    {
        public Transform parentLevel { get; protected set; }
        public UICommon UICommon { get; protected set; }
        public UICommonLogic CommonLogic { get; set; }
        public abstract void SetCommonData(Transform _parentLevel, UICommon _uiCommon);
        public virtual void Clear()
        {
            UICommon = UICommon.Undefined;
            parentLevel = null;
            CommonLogic = null;
        }
    }
}