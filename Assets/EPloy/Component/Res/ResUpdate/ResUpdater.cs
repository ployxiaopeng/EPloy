using System;
using System.Collections.Generic;
using System.IO;
using EPloy.ObjectPool;
using EPloy.TaskPool;

namespace EPloy.Res
{
    /// <summary>
    /// 检查资源回调。
    /// </summary>
    /// <param name="movedCount">已移动的资源数量。</param>
    /// <param name="removedCount">已移除的资源数量。</param>
    /// <param name="updateCount">可更新的资源数量。</param>
    /// <param name="updateTotalLength">可更新的资源总大小。</param>
    /// <param name="updateTotalZipLength">可更新的压缩后总大小。</param>
    public delegate void CheckResCompleteCallback(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength);

    public sealed class ResUpdater
    {
        internal const string RemoteVersionListFileName = "Version.dat";
        internal const string LocalVersionListFileName = "Version.dat";
        internal const string DefaultExtension = "dat";
        internal const string BackupExtension = "bak";

        internal const string ResPath = "bak";

        private static ResUpdater instance = null;
        public static ResUpdater CreateResUpdater()
        {
            if (instance == null) instance = new ResUpdater();
            return instance;
        }
        internal static ResUpdater Instance
        {
            get
            {
                return CreateResUpdater();
            }
        }
       
        /// <summary>
        /// 变体 暂时不知道不知道鬼
        /// </summary>
        public string CurrentVariant
        {
            get;
            set;
        }

        private ResUpdater()
        {

        }

        /// <summary>
        /// 检查资源。
        /// </summary>
        /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成时的回调函数。</param>
        public void CheckResources(bool ignoreOtherVariant, CheckResCompleteCallback checkResCompleteCallback)
        {
            if (checkResCompleteCallback == null)
            {
                throw new EPloyException("Check resources complete callback is invalid.");
            }
            // m_ResourceChecker.CheckResources(m_CurrentVariant, ignoreOtherVariant);
        }


    }
}
