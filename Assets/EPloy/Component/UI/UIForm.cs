
namespace EPloy
{
    /// <summary>
    /// 界面
    /// </summary>
    public abstract class UIForm : IReference
    {
        /// <summary>
        /// 获取界面序列编号。
        /// </summary>
        public int SerialId { get; private set; }
        /// <summary>
        /// 获取界面资源名称。
        /// </summary>
        public string UIFormAssetName { get; private set; }
        /// <summary>
        /// 获取界面实例。
        /// </summary>
        public object Handle { get; private set; }
        /// <summary>
        /// 获取界面所属的界面组。
        /// </summary>
        public IUIGroup UIGroup { get; private set; }
        /// <summary>
        /// 获取是否暂停
        /// </summary>
        public bool Pause { get; private set; }

        /// <summary>
        /// 初始化界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <param name="uiFormAssetName">界面资源名称。</param>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="isNewInstance">是否是新实例。</param>
        /// <param name="userData">用户自定义数据。</param>
      public  void OnInit(int serialId, string uiFormAssetName, IUIGroup uiGroup, bool isNewInstance, object userData)
    {

    }

        /// <summary>
        /// 界面回收。
        /// </summary>
        public void OnRecycle();

        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void OnOpen(object userData);

        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void OnClose(object userData);

        /// <summary>
        /// 界面暂停。
        /// </summary>
        public void OnPause();

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        public void OnResume();

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public void OnRefocus(object userData);

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public void OnUpdate();

    }
}
