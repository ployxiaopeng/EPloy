using System;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
    /// <summary>
    /// EcsGame场景
    /// </summary>
    public partial class GameScene : IHotfixModule
    {
        public CameraSystem cameraSys;
        public MapSystem mapSys;
        public MoveSystem moveSys;
        public RoleSystem roleSys;
        public AICommandSystem aICommandSys;
        public RoleAcitonSystem roleAcitonSys;
        public SkillSystem SkillSys;
        /// <summary>
        /// 创建系统
        /// </summary>
        /// <returns></returns>
        public void CreateSystem()
        {
            cameraSys = ReferencePool.Acquire<CameraSystem>();
            mapSys = ReferencePool.Acquire<MapSystem>();
            moveSys = ReferencePool.Acquire<MoveSystem>();
            roleSys = ReferencePool.Acquire<RoleSystem>();
            aICommandSys = ReferencePool.Acquire<AICommandSystem>();
            roleAcitonSys = ReferencePool.Acquire<RoleAcitonSystem>();
            SkillSys = ReferencePool.Acquire<SkillSystem>();
        }
    }
}