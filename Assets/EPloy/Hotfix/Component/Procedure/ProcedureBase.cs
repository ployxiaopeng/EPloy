
using EPloy.Fsm;


namespace EPloy
{
    /// <summary>
    /// 流程基类。
    /// </summary>
    public abstract class ProcedureBase : FsmState
    {
        /// <summary>
        /// 状态初始化时调用。
        /// </summary>
        public override void OnInit(IFsm procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        /// <summary>
        /// 进入状态时调用。
        /// </summary>
        public override void OnEnter()
        {
            base.OnEnter();
        }

        /// <summary>
        /// 状态轮询时调用。
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();
        }

        /// <summary>
        /// 离开状态时调用。
        /// </summary>
        /// <param name="isShutdown">是否是关闭状态机时触发。</param>
        public override void OnLeave(bool isShutdown)
        {
            base.OnLeave(isShutdown);
        }

        /// <summary>
        /// 状态销毁时调用。
        /// </summary>
        public override void Clear()
        {
            base.Clear();
        }
    }
}
