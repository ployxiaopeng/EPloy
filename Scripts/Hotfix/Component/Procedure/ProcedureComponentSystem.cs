
using ETModel;
using GameFramework;
using GameFramework.Procedure;
using System;

namespace ETHotfix
{
    public static class ProcedureComponentSystem
    {
        public static string[] ProcedureTypeNames;
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public static void HotfixAwake(this ProcedureComponent self)
        {
            ProcedureTypeNames = new string[]
           {
               "ETHotfix.ProcedurePreload",
               "ETHotfix.ProcedureChangeScene",
               "ETHotfix.ProcedureLogin",
               "ETHotfix.ProcedureMap"
            };
            self.HotfixStart().Coroutine();
        }
        public static async ETVoid HotfixStart(this ProcedureComponent self)
        {
            ProcedureBase[] procedures = new ProcedureBase[ProcedureTypeNames.Length];
            for (int i = 0; i < ProcedureTypeNames.Length; i++)
            {
                Type procedureType = Utility.Assembly.GetType(ProcedureTypeNames[i]);
                if (procedureType == null)
                {
                    Log.Error("Can not find procedure type '{0}'.", ProcedureTypeNames[i]);
                    return;
                }

                procedures[i] = (ProcedureBase)Activator.CreateInstance(procedureType);
                if (procedures[i] == null)
                {
                    Log.Error("Can not create procedure instance '{0}'.", ProcedureTypeNames[i]);
                    return;
                }
            }
            self.ProcedureManager.Initialize(GameEntry.Fsm.FsmManager, procedures);
            await GameEntry.Timer.WaitAsync(100);
            self.ProcedureManager.StartProcedure(procedures[0].GetType());
        }
    }
}