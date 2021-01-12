using System;

namespace EPloy
{
    /// <summary>
    /// 是否是系统流程
    /// </summary>
	public class SystemAttribute: Attribute
	{
		public Type AttributeType { get; }

        public SystemAttribute()
		{
			this.AttributeType = this.GetType();
        }
	}
}