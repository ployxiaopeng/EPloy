
using EPloy.Res;
using System;
using EPloy.TaskPool;

namespace EPloy
{

    public partial class ResComponent : Component
    {
        private PackVersionListSerializer PackVersionListSerializer;
        private UpdatableVersionListSerializer UpdatableVersionListSerializer;
        private LocalVersionListSerializer LocalVersionListSerializer;

        private void initTool()
        {
            LocalVersionListSerializer = new LocalVersionListSerializer();
            PackVersionListSerializer = new PackVersionListSerializer();
            UpdatableVersionListSerializer = new UpdatableVersionListSerializer();
        }

        /// <summary>
        /// 使用可更新模式并检查版本资源列表。
        /// </summary>
        /// <param name="latestInternalResourceVersion">最新的内部资源版本号。</param>
        /// <returns>检查版本资源列表结果。</returns>
        public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
        {
            return CheckVersionListResult.Updated;
        }

    }
}
