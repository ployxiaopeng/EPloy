using EPloy.ObjectPool;

namespace EPloy
{
    /// <summary>
    /// UI实例对象。
    /// </summary>
    internal sealed class UIFormObject : ObjectBase
    {
        private object UIFormAsset;
        /// <summary>
        ///  设置数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="UI资源"></param>
        /// <param name="UI实例"></param>
        public static UIFormObject Create(string name, object uiFormAsset, object uiFormInstance)
        {
            if (uiFormAsset == null)
            {
                Log.Fatal("UI form asset is invalid.");
                return null;
            }
            UIFormObject uIFormObject = ReferencePool.Acquire<UIFormObject>();
            uIFormObject.Initialize(name, uiFormInstance);
            uIFormObject.UIFormAsset = uiFormAsset;
            return uIFormObject;
        }

        public override void Clear()
        {
            base.Clear();
            UIFormAsset = null;
        }
    }
}
