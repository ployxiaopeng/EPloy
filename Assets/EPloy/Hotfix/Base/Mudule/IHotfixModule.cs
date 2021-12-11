
namespace EPloy
{
    /// <summary>
    /// 非热更游戏Module抽象类。
    /// </summary>
    public abstract class IHotfixModule
    {
        /// <summary>
        /// Module 唤醒
        /// </summary>
        public abstract void Awake();

        /// <summary>
        /// Module 唤醒轮询。
        /// </summary>
        public virtual void Update(){}

        /// <summary>
        /// 关闭销毁Module 
        /// </summary>
        public abstract void OnDestroy();
    }
}
