using System;

namespace EPloy
{
    public interface IStart { }
    /// <summary>
    /// 系统唤醒  在Awake之后
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StartSystem<T> : ISystem, IStart where T: IComponent
    {
		public Type Type()
		{
			return typeof(T);
		}

        public void Run(IComponent component)
        {
            this.Start((T)component);
        }

        public abstract void Start(T self);

    }
}
