﻿using System;

namespace EPloy.Editor.ResourceTools
{
    /// <summary>
    /// 对 AssetBundle 应用的压缩算法类型。
    /// </summary>
    public enum AssetBundleZipType : byte
    {
        /// <summary>
        /// 不使用压缩算法。
        /// </summary>
        UnZip = 0,

        /// <summary>
        /// 使用 LZ4 压缩算法。
        /// </summary>
        LZ4,

        /// <summary>
        /// 使用 LZMA 压缩算法。
        /// </summary>
        LZMA
    }

    [Flags]
    public enum Platform : int
    {
        Undefined = 0,

        /// <summary>
        /// Windows 32 位。
        /// </summary>
        Windows = 1 << 0,

        /// <summary>
        /// Windows 64 位。
        /// </summary>
        Windows64 = 1 << 1,

        /// <summary>
        /// macOS。
        /// </summary>
        MacOS = 1 << 2,

        /// <summary>
        /// Linux。
        /// </summary>
        Linux = 1 << 3,

        /// <summary>
        /// iOS。
        /// </summary>
        IOS = 1 << 4,

        /// <summary>
        /// Android。
        /// </summary>
        Android = 1 << 5,

        /// <summary>
        /// Windows Store。
        /// </summary>
        WindowsStore = 1 << 6,

        /// <summary>
        /// WebGL。
        /// </summary>
        WebGL = 1 << 7,
    }
}
