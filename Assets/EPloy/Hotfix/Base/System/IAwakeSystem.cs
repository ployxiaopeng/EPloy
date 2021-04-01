using System;

namespace EPloy
{
    public interface IAwake { }
    /// <summary>
    /// 系统唤醒  在对应组件添加的时候调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public abstract class AwakeSystem<T> : ISystem , IAwake  where T: IComponent
    {
		public Type Type()
		{
			return typeof(T);
		}

		public abstract void Awake(T self);

        public void Run(IComponent component)
        {
            this.Awake((T)component);
        }
    }

}
