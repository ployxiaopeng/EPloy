using ETModel;
using UnityEngine;
using GameFramework;


namespace ETHotfix
{
    /// <summary>
    /// 热更新层实体
    /// </summary>
    public interface IHotfixEntityLogic : IReference
    {
        Transform transform { get; }

        HotfixEntityData EntityData { get;  set; }
        /// <summary>
        /// 主工程的实体逻辑脚本
        /// </summary>
        EntityModelLogic EntityLogic { get; set; }
        /// <summary>
        /// 实体初始化
        /// </summary>
        void OnInit(EntityModelLogic entityLogic);
        /// <summary>
        /// 实体显示
        /// </summary>
        void OnShow(object userData);
        /// <summary>
        /// 实体轮询
        /// </summary>
        void OnUpdate();
    }
}

