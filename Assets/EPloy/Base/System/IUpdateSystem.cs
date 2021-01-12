using System;

namespace EPloy
{
    public interface IUpdate { }
    /// <summary>
    /// 系统轮转
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class UpdateSystem<T> : ISystem , IUpdate where T: IComponent
    {
		public Type Type()
		{
			return typeof(T);
		}

        public void Run(IComponent component)
        {
            this.Update((T)component);
        }

        public abstract void Update(T self);
	}
}
