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
        //普通模块
        public static ObjectPoolMudule ObjectPool { get; private set; }
        public static FsmMudule Fsm { get; private set; }
        public static EventMudule Event { get; private set; }
        public static DataStoreMudule DataStore { get; private set; }
        public static ResMudule Res { get; private set; }
        public static SceneMudule Scene { get; private set; }

        public static DataTableMudule DataTable { get; private set; }
        public static UIMudule UI { get; private set; }
        public static ObjMudule Obj { get; private set; }
        public static ProcedureMudule Procedure { get; private set; }
        public static AtlasMudule Atlas { get; private set; }
        public static MapMudule Map { get; private set; }
        
        public static void HotFixMudleInit()
        {
            ObjectPool = HotfixModuleMgr.CreateModule<ObjectPoolMudule>();
            Event = HotfixModuleMgr.CreateModule<EventMudule>();
            Fsm = HotfixModuleMgr.CreateModule<FsmMudule>();
            DataStore = HotfixModuleMgr.CreateModule<DataStoreMudule>();

            Res = HotfixModuleMgr.CreateModule<ResMudule>();
            Scene = HotfixModuleMgr.CreateModule<SceneMudule>();
            UI = HotfixModuleMgr.CreateModule<UIMudule>();
            DataTable = HotfixModuleMgr.CreateModule<DataTableMudule>();
            Obj = HotfixModuleMgr.CreateModule<ObjMudule>();
            Procedure = HotfixModuleMgr.CreateModule<ProcedureMudule>();
            Atlas = HotfixModuleMgr.CreateModule<AtlasMudule>();
            Map = HotfixModuleMgr.CreateModule<MapMudule>();
        }

        //Ecs模块
        public static GameScene GameScene { get; private set; }

        public static void ECSActivate()
        {
            GameScene = HotfixModuleMgr.CreateModule<GameScene>();
        }
    }
}
