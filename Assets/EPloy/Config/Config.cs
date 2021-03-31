
namespace EPloy
{
    public static partial class Config
    {

        /// <summary>
        /// 服务端 版本文件名称
        /// </summary>
        public const string HotFixDllName = "EPloy";

        /// <summary>
        /// 服务端 版本文件名称
        /// </summary>
        public const string RemoteVersionListFileName = "RemoteVersion.dat";

        /// <summary>
        /// 本地 版本文件名称
        /// </summary>
        public const string LocalVersionListFileName = "LocalVersion.dat";

        /// <summary>
        /// 默认数据扩展
        /// </summary>
        public const string DefaultExtension = "dat";

        /// <summary>
        ///  Null选择 
        /// </summary>
        public const string NoneOptionName = "<None>";

        /// <summary>
        /// 对象池最大容量
        /// </summary>
        public const int ObjectPoolCapacity = 32;

        /// <summary>
        /// 对象池中对象过期事件
        /// </summary>
        public const int ObjectPoolExpireTime = 320;

        /// <summary>
        /// 对象池自动释放间隔
        /// </summary>
        public const int ObjectPoolReleaseTime = 320;

        /// <summary>
        /// 文件系统设置
        /// </summary>
        public const int FileSystemMaxFileCount = 1024 * 16;

        /// <summary>
        /// 文件系统设置
        /// </summary>
        public const int FileSystemMaxBlockCount = 1024 * 256;
    }
}