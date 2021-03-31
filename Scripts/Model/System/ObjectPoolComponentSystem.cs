using GameFramework;
using GameFramework.ObjectPool;

namespace ETModel
{
    [ObjectSystem]
    public class ObjectPoolComponentAwakeSystem : AwakeSystem<ObjectPoolComponent>
    {
        public override void Awake(ObjectPoolComponent self)
        {
            self.Awake();
        }
    }
    public static class ObjectPoolComponentSystem
    {
        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public static void Awake(this ObjectPoolComponent self)
        {

            self.ObjectPoolManager = GameFrameworkEntry.GetModule<IObjectPoolManager>();
            if (self.ObjectPoolManager == null)
            {
                Log.Fatal("Object pool manager is invalid.");
                return;
            }
        }

    }
}