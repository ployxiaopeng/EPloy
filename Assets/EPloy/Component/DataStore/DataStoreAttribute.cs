using System;

namespace EPloy
{
    /// <summary>
    /// 是否数据存储
    /// </summary>
	public class DataStoreAttribute : Attribute
    {
        public Type AttributeType { get; }
        public DataStoreAttribute()
        {
            this.AttributeType = this.GetType();
        }
    }
}