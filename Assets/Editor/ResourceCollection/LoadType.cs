
namespace EPloy.Editor.ResourceTools
{
    /// <summary>
    /// 资源加载方式类型。
    /// </summary>
    public enum LoadType : byte
    {
        /// <summary>
        /// 使用文件方式加载。
        /// </summary>
        LoadFromFile = 0,

        /// <summary>
        /// 使用内存方式加载。
        /// </summary>
        LoadFromMemory,

        /// <summary>
        /// 使用二进制方式加载。
        /// </summary>
        LoadFromBinary,

    }
}
