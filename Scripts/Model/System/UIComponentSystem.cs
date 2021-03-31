using GameFramework;
using GameFramework.UI;

namespace ETModel
{
    [ObjectSystem]
    public class UIComponentAwakeSystem : AwakeSystem<UIComponent>
    {
        public override void Awake(UIComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class UIComponentStartSystem : StartSystem<UIComponent>
    {
        public override void Start(UIComponent self)
        {
            self.Start();
        }
    }
    public static class UIComponentSystem
    {

        public static void Awake(this UIComponent self)
        {
            self.UIManager = GameFrameworkEntry.GetModule<IUIManager>();
            self.DefaultUIGroup = new UIGroup() { Name = "Default", Depth = 0 };
        }

        public static void Start(this UIComponent self)
        {
            self.UIManager.SetResourceManager(Init.Resource.ResourceManager);
            self.UIManager.SetObjectPoolManager(Init.ObjectPool.ObjectPoolManager);
            self.UIManager.InstanceAutoReleaseInterval = self.InstanceAutoReleaseInterval;
            self.UIManager.InstanceCapacity = self.InstanceCapacity;
            self.UIManager.InstanceExpireTime = self.InstanceExpireTime;
            self.UIManager.InstancePriority = self.InstancePriority;
            self.UIManager.SetUIFormHelper(new UIFormHelper());
            if (!self.AddUIGroup(self.DefaultUIGroup.Name, self.DefaultUIGroup.Depth))
                Log.Warning("Add UI group '{0}' failure.", self.DefaultUIGroup.Name);
        }
    }

    public class UIGroup
    {
        public string Name { get; set; }
        public int Depth { get; set; }
    }
}
