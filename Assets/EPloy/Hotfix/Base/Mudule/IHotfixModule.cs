
namespace EPloy
{
    /// <summary>
    /// 非热更游戏Module抽象类。
    /// </summary>
    public interface IHotfixModule
    {
        /// <summary>
        /// Module 唤醒
        /// </summary>
        void Awake();

        /// <summary>
        /// Module 唤醒轮询。
        /// </summary>
        void Update();

        /// <summary>
        /// 关闭销毁Module 
        /// </summary>
        void OnDestroy();
    }
}
