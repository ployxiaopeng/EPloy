﻿
namespace EPloy.TaskPool
{
    /// <summary>
    /// 任务基类 存储人信息 以便代理处理
    /// </summary>
    public abstract class TaskBase : IReference
    {
        /// <summary>
        /// 任务默认优先级。
        /// </summary>
        public const int DefaultPriority = 0;

        private int m_SerialId;
        private int m_Priority;
        private bool m_Done;

        /// <summary>
        /// 初始化任务基类的新实例。
        /// </summary>
        public TaskBase()
        {
            m_SerialId = 0;
            m_Priority = DefaultPriority;
            m_Done = false;
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
        /// 获取或设置任务是否完成。
        /// </summary>
        public bool Done
        {
            get
            {
                return m_Done;
            }
            set
            {
                m_Done = value;
            }
        }

        /// <summary>
        /// 获取任务描述。
        /// </summary>
        public virtual string Description
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// 初始化任务基类。
        /// </summary>
        /// <param name="serialId">任务的序列编号。</param>
        /// <param name="priority">任务的优先级。</param>
        internal void Initialize(int serialId, int priority)
        {
            m_SerialId = serialId;
            m_Priority = priority;
            m_Done = false;
        }

        /// <summary>
        /// 清理任务基类。
        /// </summary>
        public virtual void Clear()
        {
            m_SerialId = 0;
            m_Priority = DefaultPriority;
            m_Done = false;
        }
    }
}