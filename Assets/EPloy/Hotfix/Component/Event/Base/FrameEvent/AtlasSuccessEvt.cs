
namespace EPloy
{
    /// <summary>
    /// 读取表成功事件。
    /// </summary>
    public sealed class AtlasSuccessEvt : EventArg
    {
        public override int id
        {
            get
            {
                return FrameEvent.AtlasSuccessEvt;
            }
        }

        /// <summary>
        /// 获取内容资源名称。
        /// </summary>
        public string AtlasName
        {
            get;
            private set;
        }

        /// <summary>
        /// 设置成功事件数据
        /// </summary>
        /// <param name="atlasName">图集名称。</param>
        public void SetData(string atlasName)
        {
            AtlasName = atlasName;
        }

        /// <summary>
        /// 清理读取数据成功事件。
        /// </summary>
        public override void Clear()
        {
            AtlasName = null;
        }
    }
}
