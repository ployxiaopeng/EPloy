using System;

namespace EPloy
{
    /// <summary>
    /// 是否UI 逻辑类
    /// </summary>
	public class ObjAttribute : Attribute
    {
        public Type AttributeType { get; }
        public string ObjName { get; }

        public ObjAttribute(string objName)
        {
            this.AttributeType = this.GetType();
            ObjName = objName;
        }
    }
}