using System;

namespace EPloy
{
    /// <summary>
    /// 是否UI 逻辑类
    /// </summary>
	public class UIAttribute : Attribute
    {
        public Type AttributeType { get; }
        public UIName UIName { get; }

        public UIAttribute(UIName uiName)
        {
            this.AttributeType = this.GetType();
            UIName = uiName;
        }
    }
}