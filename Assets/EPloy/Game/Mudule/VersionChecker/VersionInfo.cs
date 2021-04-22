
namespace EPloy
{

    //
    // 摘要:
    //     检查版本资源列表结果。
    public enum CheckVersionListResult : byte
    {
        //
        // 摘要:
        // 已经是最新的。
        Updated = 0,
        //
        // 摘要:
        // 需要更新。
        NeedUpdate = 1
    }

    public class VersionInfo
    {
        // 是否更新游戏
        public bool UpdateGame
        {
            get;
            set;
        }

        // 最新的游戏版本号
        public string LatestGameVersion
        {
            get;
            set;
        }

        // 最新的游戏内部版本号
        public int InternalGameVersion
        {
            get;
            set;
        }

        // 最新的资源内部版本号
        public int InternalResourceVersion
        {
            get;
            set;
        }

        // 资源更新下载地址
        public string UpdatePrefixUri
        {
            get;
            set;
        }

        // 资源版本列表长度
        public int VersionListLength
        {
            get;
            set;
        }

        // 资源版本列表哈希值
        public int VersionListHashCode
        {
            get;
            set;
        }

        // 资源版本列表压缩后长度
        public int VersionListZipLength
        {
            get;
            set;
        }

        // 资源版本列表压缩后哈希值
        public int VersionListZipHashCode
        {
            get;
            set;
        }
    }
}
