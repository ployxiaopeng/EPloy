
namespace EPloy.TaskPool
{
    /// <summary>
    /// 任务代理接口。
    /// </summary>
    /// <typeparam name="T">任务类型。</typeparam>
    public interface ITaskAgent<T> where T : TaskBase
    {
        /// <summary>
        /// 获取任务。
        /// </summary>
        T Task
        {
            get;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        void Initialize(string resPath);

        /// <summary>
        /// 任务代理轮询。
        /// </summary>
        void Update();

        /// <summary>
        /// 关闭并清理任务代理。
        /// </summary>
        void OnDestroy();

        /// <summary>
        /// 开始处理任务。
        /// </summary>
        /// <param name="task">要处理的任务。</param>
        /// <returns>开始处理任务的状态。</returns>
        StartTaskStatus Start(T task);

        /// <summary>
        /// 停止正在处理的任务并重置任务代理。
        /// </summary>
        void Reset();
    }
}
