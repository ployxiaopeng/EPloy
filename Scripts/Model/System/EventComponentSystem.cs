using GameFramework;
using GameFramework.Event;
namespace ETModel
{
    [ObjectSystem]
    public class EventComponentAwakeSystem : AwakeSystem<EventComponent>
    {
        public override void Awake(EventComponent self)
        {
            self.Awake();
        }
    }
    public static class EventComponentSystem
    {
        public static void Awake(this EventComponent self)
        {
            self.EventManager = GameFrameworkEntry.GetModule<IEventManager>();
            if (self.EventManager == null)
            {
                Log.Fatal("Event manager is invalid.");
                return;
            }
        }
    }
}