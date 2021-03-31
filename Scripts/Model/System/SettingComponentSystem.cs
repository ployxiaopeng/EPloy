using GameFramework;
using GameFramework.Setting;

namespace ETModel
{
    [ObjectSystem]
    public class SettingComponentAwakeSystem : AwakeSystem<SettingComponent>
    {
        public override void Awake(SettingComponent self)
        {
            self.Awake();
        }
    }
    public static class SettingComponentSystem
    {
         public static void Awake( this SettingComponent self)
        {
            self.SettingManager = GameFrameworkEntry.GetModule<ISettingManager>();
            if (self.SettingManager == null)
            {
                Log.Fatal("Setting manager is invalid.");
                return;
            }
            self.SettingManager.SetSettingHelper(new SettingHelper());
        }
    }
}
