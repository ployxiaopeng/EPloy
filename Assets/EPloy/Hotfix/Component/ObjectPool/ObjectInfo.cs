
using System;

namespace EPloy.ObjectPool
{
    /// <summary>
    /// 对象信息。
    /// </summary>
    public struct ObjectInfo
    {
        private readonly string m_Name;
        private readonly DateTime m_LastUseTime;
        private readonly int m_SpawnCount;

        /// <summary>
        /// 初始化对象信息的新实例。
        /// </summary>
        /// <param name="name">对象名称。</param>
        /// <param name="locked">对象是否被加锁。</param>
        /// <param name="customCanReleaseFlag">对象自定义释放检查标记。</param>
        /// <param name="priority">对象的优先级。</param>
        /// <param name="lastUseTime">对象上次使用时间。</param>
        /// <param name="spawnCount">对象的获取计数。</param>
        public ObjectInfo(string name,  DateTime lastUseTime, int spawnCount)
        {
            m_Name = name;
            m_LastUseTime = lastUseTime;
            m_SpawnCount = spawnCount;
        }

        /// <summary>
        /// 获取对象名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取对象上次使用时间。
        /// </summary>
        public DateTime LastUseTime
        {
            get
            {
                return m_LastUseTime;
            }
        }

        /// <summary>
        /// 获取对象是否正在使用。
        /// </summary>
        public bool IsInUse
        {
            get
            {
                return m_SpawnCount > 0;
            }
        }

        /// <summary>
        /// 获取对象的获取计数。
        /// </summary>
        public int SpawnCount
        {
            get
            {
                return m_SpawnCount;
            }
        }
    }
}
