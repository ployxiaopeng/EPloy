using EPloy.ObjectPool;

namespace EPloy
{
    /// <summary>
    /// UI实例对象。
    /// </summary>
    internal sealed class UIFormObject : ObjectBase
    {
        public object FormAsset
        {
            get;
            private set;
        }
        /// <summary>
        ///  设置数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="UI资源"></param>
        /// <param name="UI实例"></param>
        public static UIFormObject Create(string name, object formAsset, object formInstance)
        {
            if (formAsset == null)
            {
                Log.Fatal("UI form asset is invalid.");
                return null;
            }
            UIFormObject uIFormObject = ReferencePool.Acquire<UIFormObject>();
            uIFormObject.Initialize(name, formInstance);
            uIFormObject.FormAsset = formAsset;
            return uIFormObject;
        }

        public override void Clear()
        {
            base.Clear();
            FormAsset = null;
        }
    }
}
