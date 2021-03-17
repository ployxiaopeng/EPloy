
namespace EPloy
{
    /// <summary>
    /// 读取表失败事件。
    /// </summary>
    public sealed class DataTableFailureEvt : EventArg
    {

        public override int id
        {
            get
            {
                return FrameEvent.DataTableFailureEvt;
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
        /// 获取错误信息。
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }

        /// <summary>
        /// 设置失败事件数据
        /// </summary>
        /// <param name="dataAssetName">内容资源名称。</param>
        /// <param name="errorMessage">错误信息。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void SetData(string dataAssetName, string errorMessage, object userData)
        {
            DataAssetName = dataAssetName;
            ErrorMessage = errorMessage;
            UserData = userData;
        }

        /// <summary>
        /// 清理读取数据失败事件。
        /// </summary>
        public override void Clear()
        {
            DataAssetName = null;
            ErrorMessage = null;
            UserData = null;
        }
    }
}
