
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 界面
    /// </summary>
    public abstract class UIForm : IReference
    {
        /// <summary>
        /// ui名字
        /// </summary>
        /// <value></value>
        public UIName UIName { get; private set; }
        /// <summary>
        /// 获取界面实例。
        /// </summary>
        public GameObject Handle { get; private set; }
        /// <summary>
        /// 获取界面所属的界面组。
        /// </summary>
        public UIGroupName GroupName { get; private set; }
        /// <summary>
        /// 获取是否暂停
        /// </summary>
        public bool isPause { get; private set; }

        protected Transform transform
        {
            get
            {
                return Handle.transform;
            }
        }

        /// <summary>
        /// 初始化界面。
        /// </summary>
        /// <param name="serialId">界面序列编号。</param>
        /// <param name="assetName">界面资源名称。</param>
        /// <param name="uiGroup">界面所属的界面组。</param>
        /// <param name="isNew">是否是新实例。</param>
        /// <param name="userData">用户自定义数据。</param>
        public void Initialize(bool isNew, GameObject handle, UIName uiName, UIGroupName groupName, object userData)
        {
            if (isNew)
            {
                Handle = handle;
                UIName = uiName;
                Handle.name = UIName.ToString();
                Create();
            }
            GroupName = groupName;
            isPause = false;
            Open(userData);
        }

        /// <summary>
        /// 界面生成。
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public abstract void Open(object userData);

        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public abstract void Close(object userData);

        /// <summary>
        /// 界面暂停。
        /// </summary>
        public virtual void Pause()
        {
            isPause = true;
        }

        /// <summary>
        /// 界面激活 刷新
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void Refocus(object userData)
        {
            isPause = false;
        }

        /// <summary>
        /// 界面轮询。
        /// </summary>
        public virtual void Update() { }

        /// <summary>
        /// 界面清理回收。
        /// </summary>
        public virtual void Clear() { }

    }
}
