using EPloy.Game;
using System;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 热更游戏Module
    /// </summary>
    public static class HotFixMudule
    {
        //普通模块
        public static FsmMudule Fsm { get; private set; }
        public static EventMudule Event { get; private set; }
        public static DataStoreMudule DataStore { get; private set; }
        public static SceneMudule Scene { get; private set; }
        public static NetMudule Net { get; private set; }
        public static DataTableMudule DataTable { get; private set; }
        public static UIMudule UI { get; private set; }
        public static ObjMudule Obj { get; private set; }
        public static ProcedureMudule Procedure { get; private set; }

        public static void HotFixMudleInit()
        {
            Event = HotfixModuleMgr.CreateModule<EventMudule>();
            Fsm = HotfixModuleMgr.CreateModule<FsmMudule>();
            DataStore = HotfixModuleMgr.CreateModule<DataStoreMudule>();
            Net = HotfixModuleMgr.CreateModule<NetMudule>();
            Scene = HotfixModuleMgr.CreateModule<SceneMudule>();
            UI = HotfixModuleMgr.CreateModule<UIMudule>();
            DataTable = HotfixModuleMgr.CreateModule<DataTableMudule>();
            Obj = HotfixModuleMgr.CreateModule<ObjMudule>();
            Procedure = HotfixModuleMgr.CreateModule<ProcedureMudule>();
            GameScene = HotfixModuleMgr.CreateModule<GameScene>();
        }

        //mapEcs模块
        public static GameScene GameScene { get; private set; }

        //轮转注册
        public static void RegisterUpdate(Action update)
        {
            GameModule.ILRuntime.HotfixUpdate += update;
        }

        public static void RemoveUpdate(Action update)
        {
            GameModule.ILRuntime.HotfixUpdate -= update;
        }
    }
}
