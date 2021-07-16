
namespace EPloy
{
  
    /// <summary>
    /// 切换场景事件。
    /// </summary>
    public sealed class LoadSceneEvt : EventArg
    {
        public override int id
        {
            get
            {
                return FrameEvent.LoadSceneEvt;
            }
        }

        /// <summary>
        /// 状态
        /// </summary>
        public LoadSceneState State
        {
            get;
            private set;
        }

        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneAssetName
        {
            get;
            private set;
        }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string ErrorMessage
        {
            get;
            private set;
        }

        /// <summary>
        /// 依赖资源百分比
        /// </summary>
        public int DependPercent
        {
            get;
            private set;
        }

        /// <summary>
        /// 加载成功
        /// </summary>
        public void SetSuccessData(string sceneAssetName)
        {
            State = LoadSceneState.Success;
            SceneAssetName = sceneAssetName;
        }

        /// <summary>
        /// 加载失败
        /// </summary>
        public void SetFailureData(string sceneAssetName, string errorMessage)
        {
            State = LoadSceneState.Success;
            SceneAssetName = sceneAssetName;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// 更新依赖资源百分比
        /// </summary>
        public void SetDependPercentData(string sceneAssetName, int dependPercent)
        {
            State = LoadSceneState.Success;
            SceneAssetName = sceneAssetName;
            DependPercent = dependPercent;
        }
        /// <summary>
        /// 清理读取数据成功事件。
        /// </summary>
        public override void Clear()
        {
            State = LoadSceneState.Null;
            ErrorMessage = null;
            SceneAssetName = null;
            DependPercent = 0;
        }

        public enum LoadSceneState
        {
            Success,
            Failure,
            Depend,
            Null,
        }
    }
}
