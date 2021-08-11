using System.Collections;
using UnityEngine;

namespace EPloy
{
    public static class HotFixStart
    {
        public static bool isEditorRes = true;
        public static GameStart GameStart;

        public static void Awake(GameStart gameStart)
        {
            GameStart = gameStart;
            Log.Info("HotFix Awake");
            HotFixMudule.HotFixMudleInit();
        }

        public static void Start()
        {
            HotFixMudule.Procedure.StartProcedure<ProcedurePreload>();
        }

        public static void Update()
        {
            HotFixMudule.GameSystem.Update();
            HotfixModuleMgr.ModuleUpdate();
        }

        public static void LateUpdate()
        {

        }

        public static void FixUpdate()
        {

        }

        public static void OnDestroy()
        {
            HotfixModuleMgr.ModuleDestory();
        }
    }
}
