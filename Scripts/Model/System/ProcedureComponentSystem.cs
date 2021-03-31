using GameFramework;
using GameFramework.Procedure;

namespace ETModel
{
    [ObjectSystem]
    public class ProcedureComponentAwakeSystem : AwakeSystem<ProcedureComponent>
    {
        public override void Awake(ProcedureComponent self)
        {
            self.Awake();
        }
    }

    public static class ProcedureComponentSystem
    {
        private static ProcedureBase[] Procedures = new ProcedureBase[]
        {
            new ProcedureLaunch(),
            new ProcedureCheckVersion(),
           new ProcedureUpdateResource()
        };
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public static void Awake(this ProcedureComponent self)
        {
            self.ProcedureManager = GameFrameworkEntry.GetModule<IProcedureManager>();
            if (self.ProcedureManager == null)
            {
                Log.Fatal("Procedure manager is invalid.");
                return;
            }
            if (Procedures.Length <= 0)
            {
                Log.Error("当前无流程");
                return;
            }
            self.Start().Coroutine();
        }
        public static async ETVoid Start(this ProcedureComponent self)
        {
            self.ProcedureManager.Initialize(Init.Fsm.FsmManager, Procedures);
            await Init.Timer.WaitAsync(100);
            self.ProcedureManager.StartProcedure(Procedures[0].GetType());
        }
    }
}