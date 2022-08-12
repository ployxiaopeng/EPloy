using System.Collections;
using UnityEngine;

namespace EPloy.Hotfix
{
    public static class HotFixStart
    {
        public static GameStart GameStart;

        /// <summary>
        /// 游戏流程
        /// </summary>
        private static string[] Procedure =
        {
            "EPloy.Hotfix.ProcedurePreload",
            "EPloy.Hotfix.ProcedureLogin",
            "EPloy.Hotfix.ProcedureSwitchScene",
            "EPloy.Hotfix.ProcedureMap",
        };

        public static void Awake(GameStart gameStart)
        {
            GameStart = gameStart;
            HotFixMudule.HotFixMudleInit();
        }

        public static void Start()
        {
            Log.Info("HotFix Start");
            HotFixMudule.Procedure.RegisterProcedure(Procedure);
            HotFixMudule.Procedure.StartProcedure<ProcedurePreload>();
        }

        public static void OnDestroy()
        {
            HotfixModuleMgr.ModuleDestory();
        }
    }
}
