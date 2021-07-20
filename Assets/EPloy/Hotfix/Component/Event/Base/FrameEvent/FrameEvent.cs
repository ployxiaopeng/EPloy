
using System;

namespace EPloy
{
    /// <summary>
    /// 框架事件 预留 id 100-1000
    /// </summary>
    public static class FrameEvent
    {
        /// <summary>
        /// 读取表成功事件。
        /// </summary>
        public static int DataTableSuccessEvt = 100;
        /// <summary>
        /// 读取表失败事件。
        /// </summary>
        public static int DataTableFailureEvt = 101;
        /// <summary>
        /// 读图集成功事件。
        /// </summary>
        public static int AtlasSuccessEvt = 102;

        /// <summary>
        /// 切换场景事件
        /// </summary>
        public static int LoadSceneEvt = 103;
    }
}
