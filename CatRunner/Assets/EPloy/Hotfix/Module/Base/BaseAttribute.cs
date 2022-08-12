using System;

namespace EPloy.Hotfix
{
    /// <summary>
    /// 是否需要
    /// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class BaseAttribute: Attribute
	{
		public Type AttributeType { get; }

		public BaseAttribute()
		{
			this.AttributeType = this.GetType();
		}
	}
}