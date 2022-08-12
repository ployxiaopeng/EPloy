
namespace EPloy.Game.Res
{
    /// <summary>
    /// 加载资源状态。
    /// </summary>
    public enum LoadResStatus : byte
    {
        /// <summary>
        /// 加载资源完成。
        /// </summary>
        Success = 0,

        /// <summary>
        /// 资源不存在。
        /// </summary>
        NotExist,

        /// <summary>
        /// 资源尚未准备完毕。
        /// </summary>
        NotReady,

        /// <summary>
        /// 依赖资源错误。
        /// </summary>
        DependencyError,

        /// <summary>
        /// 资源类型错误。
        /// </summary>
        TypeError,

        /// <summary>
        /// 加载资源错误。
        /// </summary>
        AssetError
    }

    /// <summary>
    /// 加载资源进度类型。
    /// </summary>
    public enum LoadResProgress : byte
    {
        /// <summary>
        /// 未知类型。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 读取资源包。
        /// </summary>
        ReadRes,

        /// <summary>
        /// 加载资源包。
        /// </summary>
        LoadRes,

        /// <summary>
        /// 加载资源。
        /// </summary>
        LoadAsset,

        /// <summary>
        /// 加载场景。
        /// </summary>
        LoadScene
    }

    /// <summary>
    /// 检查资源是否存在的结果。
    /// </summary>
    public enum HasResult : byte
    {
        /// <summary>
        /// 资源不存在。
        /// </summary>
        NotExist = 0,

        /// <summary>
        /// 资源尚未准备完毕。
        /// </summary>
        NotReady,

        /// <summary>
        /// 存在资源且存储在磁盘上。
        /// </summary>
        AssetOnDisk,

        /// <summary>
        /// 存在二进制资源且存储在磁盘上。
        /// </summary>
        BinaryOnDisk,
    }
}
