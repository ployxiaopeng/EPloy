
using GameFramework;
using GameFramework.Config;

namespace ETModel
{
    [ObjectSystem]
    public class ConfigComponentAwakeSystem : AwakeSystem<ConfigComponent>
    {
        public override void Awake(ConfigComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class ConfigComponentStartSystem : StartSystem<ConfigComponent>
    {
        public override void Start(ConfigComponent self)
        {
            self.Start();
        }
    }
    public static class ConfigComponentSystem
    {

        public static void Awake(this ConfigComponent self)
        {
            self.ConfigManager = GameFrameworkEntry.GetModule<IConfigManager>();
            if (self.ConfigManager == null)
            {
                Log.Fatal("Config manager is invalid.");
                return;
            }
        }

        public static void Start(this ConfigComponent self)
        {
            self.ConfigManager.SetResourceManager(Init.Resource.ResourceManager);
            self.ConfigManager.SetConfigHelper(new ConfigHelper(self.ConfigManager));
        }
    }
}
