using System.Collections.Generic;

namespace EPloy.Res
{
    /// <summary>
    /// 资源组。
    /// </summary>
    public sealed class ResGroup
    {
        private readonly string m_Name;
        private readonly Dictionary<ResName, ResInfo> m_ResourceInfos;
        private readonly HashSet<ResName> m_ResNames;
        private long m_TotalLength;
        private long m_TotalZipLength;

        /// <summary>
        /// 初始化资源组的新实例。
        /// </summary>
        /// <param name="name">资源组名称。</param>
        /// <param name="resourceInfos">资源信息引用。</param>
        public ResGroup(string name, Dictionary<ResName, ResInfo> resourceInfos)
        {
            if (name == null)
            {
                Log.Fatal("Name is invalid.");
                return;
            }

            if (resourceInfos == null)
            {
                Log.Fatal("Resource infos is invalid.");
                return;
            }

            m_Name = name;
            m_ResourceInfos = resourceInfos;
            m_ResNames = new HashSet<ResName>();
        }

        /// <summary>
        /// 获取资源组名称。
        /// </summary>
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// 获取资源组是否准备完毕。
        /// </summary>
        public bool Ready
        {
            get
            {
                return ReadyCount >= TotalCount;
            }
        }

        /// <summary>
        /// 获取资源组包含资源数量。
        /// </summary>
        public int TotalCount
        {
            get
            {
                return m_ResNames.Count;
            }
        }

        /// <summary>
        /// 获取资源组中已准备完成资源数量。
        /// </summary>
        public int ReadyCount
        {
            get
            {
                int readyCount = 0;
                foreach (ResName ResName in m_ResNames)
                {
                    ResInfo resourceInfo = null;
                    if (m_ResourceInfos.TryGetValue(ResName, out resourceInfo) && resourceInfo.Ready)
                    {
                        readyCount++;
                    }
                }

                return readyCount;
            }
        }

        /// <summary>
        /// 获取资源组包含资源的总大小。
        /// </summary>
        public long TotalLength
        {
            get
            {
                return m_TotalLength;
            }
        }

        /// <summary>
        /// 获取资源组包含资源压缩后的总大小。
        /// </summary>
        public long TotalZipLength
        {
            get
            {
                return m_TotalZipLength;
            }
        }

        /// <summary>
        /// 获取资源组中已准备完成资源的总大小。
        /// </summary>
        public long ReadyLength
        {
            get
            {
                long totalReadyLength = 0L;
                foreach (ResName ResName in m_ResNames)
                {
                    ResInfo resourceInfo = null;
                    if (m_ResourceInfos.TryGetValue(ResName, out resourceInfo) && resourceInfo.Ready)
                    {
                        totalReadyLength += resourceInfo.Length;
                    }
                }

                return totalReadyLength;
            }
        }

        /// <summary>
        /// 获取资源组的完成进度。
        /// </summary>
        public float Progress
        {
            get
            {
                return m_TotalLength > 0L ? (float)ReadyLength / m_TotalLength : 1f;
            }
        }

        /// <summary>
        /// 获取资源组包含的资源名称列表。
        /// </summary>
        /// <returns>资源组包含的资源名称列表。</returns>
        public string[] GetResNames()
        {
            int index = 0;
            string[] ResNames = new string[m_ResNames.Count];
            foreach (ResName ResName in m_ResNames)
            {
                ResNames[index++] = ResName.FullName;
            }

            return ResNames;
        }

        /// <summary>
        /// 获取资源组包含的资源名称列表。
        /// </summary>
        /// <param name="results">资源组包含的资源名称列表。</param>
        public void GetResNames(List<string> results)
        {
            if (results == null)
            {
                Log.Fatal("Results is invalid.");
                return;
            }

            results.Clear();
            foreach (ResName ResName in m_ResNames)
            {
                results.Add(ResName.FullName);
            }
        }

        /// <summary>
        /// 获取资源组包含的资源名称列表。
        /// </summary>
        /// <returns>资源组包含的资源名称列表。</returns>
        public ResName[] InternalGetResNames()
        {
            int index = 0;
            ResName[] ResNames = new ResName[m_ResNames.Count];
            foreach (ResName ResName in m_ResNames)
            {
                ResNames[index++] = ResName;
            }

            return ResNames;
        }

        /// <summary>
        /// 获取资源组包含的资源名称列表。
        /// </summary>
        /// <param name="results">资源组包含的资源名称列表。</param>
        public void InternalGetResNames(List<ResName> results)
        {
            if (results == null)
            {
               Log.Fatal("Results is invalid.");
               return;
            }

            results.Clear();
            foreach (ResName ResName in m_ResNames)
            {
                results.Add(ResName);
            }
        }

        /// <summary>
        /// 检查指定资源是否属于资源组。
        /// </summary>
        /// <param name="ResName">要检查的资源的名称。</param>
        /// <returns>指定资源是否属于资源组。</returns>
        public bool HasResource(ResName ResName)
        {
            return m_ResNames.Contains(ResName);
        }

        /// <summary>
        /// 向资源组中增加资源。
        /// </summary>
        /// <param name="ResName">资源名称。</param>
        /// <param name="length">资源大小。</param>
        /// <param name="zipLength">资源压缩后的大小。</param>
        public void AddResource(ResName ResName, int length, int zipLength)
        {
            m_ResNames.Add(ResName);
            m_TotalLength += length;
            m_TotalZipLength += zipLength;
        }
    }
}

