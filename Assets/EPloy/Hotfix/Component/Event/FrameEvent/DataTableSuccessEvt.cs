
namespace EPloy
{
    /// <summary>
    /// 读取表成功事件。
    /// </summary>
    public sealed class DataTableSuccessEvt : EventArg
    {
        public override int id
        {
            get
            {
                return FrameEvent.DataTableSuccessEvt;
            }
        }

        /// <summary>
        /// 获取内容资源名称。
        /// </summary>
        public string DataAssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取加载持续时间。
        /// </summary>
        public float Duration
        {
            get;
            private set;
        }


        /// <summary>
        /// 设置成功事件数据
        /// </summary>
        /// <param name="dataAssetName">内容资源名称。</param>
        /// <param name="duration">加载持续时间。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void SetData(string dataAssetName, float duration)
        {
            string[] paths = dataAssetName.Split('/');
            string[] names = paths[paths.Length - 1].Split('.');
            DataAssetName = names[0];
            Duration = duration;
        }

        /// <summary>
        /// 清理读取数据成功事件。
        /// </summary>
        public override void Clear()
        {
            DataAssetName = null;
            Duration = 0f;
        }
    }
}
