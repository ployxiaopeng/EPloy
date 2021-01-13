//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace EPloy.TaskPool
{
    /// <summary>
    /// 任务信息。
    /// </summary>
    public struct TaskInfo
    {
        private readonly int m_SerialId;
        private readonly int m_Priority;
        private readonly TaskStatus m_Status;
        private readonly string m_Description;

        /// <summary>
        /// 初始化任务信息的新实例。
        /// </summary>
        /// <param name="serialId">任务的序列编号。</param>
        /// <param name="priority">任务的优先级。</param>
        /// <param name="status">任务状态。</param>
        /// <param name="description">任务描述。</param>
        public TaskInfo(int serialId, int priority, TaskStatus status, string description)
        {
            m_SerialId = serialId;
            m_Priority = priority;
            m_Status = status;
            m_Description = description;
        }

        /// <summary>
        /// 获取任务的序列编号。
        /// </summary>
        public int SerialId
        {
            get
            {
                return m_SerialId;
            }
        }

        /// <summary>
        /// 获取任务的优先级。
        /// </summary>
        public int Priority
        {
            get
            {
                return m_Priority;
            }
        }

        /// <summary>
        /// 获取任务状态。
        /// </summary>
        public TaskStatus Status
        {
            get
            {
                return m_Status;
            }
        }

        /// <summary>
        /// 获取任务描述。
        /// </summary>
        public string Description
        {
            get
            {
                return m_Description;
            }
        }
    }

    /// <summary>
    /// 任务状态。
    /// </summary>
    public enum TaskStatus : byte
    {
        /// <summary>
        /// 未开始。
        /// </summary>
        Todo = 0,

        /// <summary>
        /// 执行中。
        /// </summary>
        Doing,

        /// <summary>
        /// 完成。
        /// </summary>
        Done
    }

    /// <summary>
    /// 开始处理任务的状态。
    /// </summary>
    public enum StartTaskStatus : byte
    {
        /// <summary>
        /// 可以立刻处理完成此任务。
        /// </summary>
        Done = 0,

        /// <summary>
        /// 可以继续处理此任务。
        /// </summary>
        CanResume,

        /// <summary>
        /// 不能继续处理此任务，需等待其它任务执行完成。
        /// </summary>
        HasToWait,

        /// <summary>
        /// 不能继续处理此任务，出现未知错误。
        /// </summary>
        UnknownError
    }
}
