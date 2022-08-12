namespace EPloy.Res
{
    /// <summary>
    /// 资产信息 暂时理解为存在本地的资产
    /// </summary>
    public sealed class AssetInfo
    {
        private readonly string m_AssetName;
        private readonly ResName m_ResName;
        private readonly string[] m_DependencyAssetNames;

        /// <summary>
        /// 初始化资源信息的新实例。
        /// </summary>
        /// <param name="assetName">资源名称。</param>
        /// <param name="resourceName">所在资源名称。</param>
        /// <param name="dependencyAssetNames">依赖资源名称。</param>
        public AssetInfo(string assetName, ResName resName, string[] dependencyAssetNames)
        {
            m_AssetName = assetName;
            m_ResName = resName;
            m_DependencyAssetNames = dependencyAssetNames;
        }

        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string AssetName
        {
            get
            {
                return m_AssetName;
            }
        }

        /// <summary>
        /// 资产名称。
        /// </summary>
        public ResName ResName
        {
            get
            {
                return m_ResName;
            }
        }

        /// <summary>
        /// 获取依赖资产
        /// </summary>
        /// <returns>依赖资产</returns>
        public string[] GetDependencyAssetNames()
        {
            return m_DependencyAssetNames;
        }
    }
}

