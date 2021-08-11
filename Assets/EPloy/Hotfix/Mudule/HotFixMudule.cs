using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 热更游戏Module
    /// </summary>
    public static class HotFixMudule
    {
        public static ObjectPoolMudule ObjectPool { get; set; }
        public static FsmMudule Fsm { get; set; }
        public static EventMudule Event { get; set; }
        public static DataStoreMudule DataStore { get; set; }
        public static ResMudule Res { get; set; }
        public static SceneMudule Scene { get; set; }

        public static DataTableMudule DataTable { get; set; }
        public static UIMudule UI { get; set; }
        public static ObjMudule Obj { get; set; }
        public static ProcedureMudule Procedure { get; set; }
        public static AtlasMudule Atlas { get; set; }
        public static MapMudule Map { get; set; }

        private static GameEntity gameEntity = null;

        public static GameEntity GameEntity
        {
            get
            {
                if (gameEntity == null)
                {
                    gameEntity = ReferencePool.Acquire<GameEntity>();
                    gameEntity.Awake(0, "Game");
                }

                return gameEntity;
            }
        }

        private static GameSystem gameSystem = null;

        public static GameSystem GameSystem
        {
            get
            {
                if (gameSystem == null)
                {
                    gameSystem = new GameSystem();
                }

                return gameSystem;
            }
        }
        
        public static void HotFixMudleInit()
        {
            GameSystem.Add(MuduleConfig.HotFixDllName, GameModule.ILRuntime.GetHotfixTypes);
            ObjectPool = HotfixModuleMgr.CreateModule<ObjectPoolMudule>();
            Event =HotfixModuleMgr.CreateModule<EventMudule>();
            Fsm =HotfixModuleMgr.CreateModule<FsmMudule>();
            DataStore =HotfixModuleMgr.CreateModule<DataStoreMudule>();

            Res =HotfixModuleMgr.CreateModule<ResMudule>();
            Scene =HotfixModuleMgr.CreateModule<SceneMudule>();
            UI =HotfixModuleMgr.CreateModule<UIMudule>();
            DataTable =HotfixModuleMgr.CreateModule<DataTableMudule>();
            Obj =HotfixModuleMgr.CreateModule<ObjMudule>();
            Procedure =HotfixModuleMgr.CreateModule<ProcedureMudule>();
            Atlas =HotfixModuleMgr.CreateModule<AtlasMudule>();
            Map =HotfixModuleMgr.CreateModule<MapMudule>();
        }
    }
}
