
namespace EPloy
{
    /// <summary>
    /// Module抽象类。
    /// </summary>
    public abstract class EPloyModule
    {
        /// <summary>
        /// Module 唤醒
        /// </summary>
        public abstract void Awake();

        /// <summary>
        /// Module 唤醒轮询。
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// 关闭Module 
        /// </summary>
        public abstract void OnDestroy();
    }
}
