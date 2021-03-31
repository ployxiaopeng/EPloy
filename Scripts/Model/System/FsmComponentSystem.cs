using GameFramework;
using GameFramework.Fsm;

namespace ETModel
{
    [ObjectSystem]
    public class FsmComponentAwakeSystem : AwakeSystem<FsmComponent>
    {
        public override void Awake(FsmComponent self)
        {
            self.Awake();
        }
    }
    public static class FsmComponentSystem
    {
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public static void Awake(this FsmComponent self)
        {
            self.FsmManager = GameFrameworkEntry.GetModule<IFsmManager>();
            if (self.FsmManager == null)
            {
                Log.Fatal("FSM manager is invalid.");
                return;
            }
        }
    }
}
