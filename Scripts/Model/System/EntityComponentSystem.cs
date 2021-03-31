using GameFramework;
using GameFramework.Entity;

namespace ETModel
{
    [ObjectSystem]
    public class EntityComponentAwakeSystem : AwakeSystem<EntityComponent>
    {
        public override void Awake(EntityComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class EntityComponentStartSystem : StartSystem<EntityComponent>
    {
        public override void Start(EntityComponent self)
        {
            self.Start();
        }
    }

    public static class EntityComponentSystem
    {
        private static EntityGroup defaultEntityGroups = new EntityGroup()
        {
            Name = "Default"
        };
        public static void Awake(this EntityComponent self)
        {
            self.EntityManager = GameFrameworkEntry.GetModule<IEntityManager>();
            if (self.EntityManager == null)
            {
                Log.Fatal("EntityLogic manager is invalid.");
                return;
            }
        }
        public static void Start(this EntityComponent self)
        {
            self.EntityManager.SetResourceManager(Init.Resource.ResourceManager);
            self.EntityManager.SetObjectPoolManager(Init.ObjectPool.ObjectPoolManager);
            self.EntityManager.SetEntityHelper(new EntityHelper());

            if (!self.AddEntityGroup(defaultEntityGroups.Name, defaultEntityGroups.InstanceAutoReleaseInterval, defaultEntityGroups.InstanceCapacity, defaultEntityGroups.InstanceExpireTime, defaultEntityGroups.InstancePriority))
            {
                Log.Warning("Add EntityLogic group '{0}' failure.", defaultEntityGroups.Name);
            }
        }
    }
}