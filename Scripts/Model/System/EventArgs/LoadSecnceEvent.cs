using GameFramework.Event;
namespace ETModel
{
    public class LoadSecnceEvent : GameEventArgs
    {
        public static readonly int EventId = typeof(LoadSecnceEvent).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        public override void Clear()
        {
            UserData = null;
        }

        public LoadSecnceEvent(string data)
        {
            UserData = data;
        }

        /// <summary>
        /// 获取用户自定义数据。
        /// </summary>
        public object UserData
        {
            get;
            private set;
        }
    }
}