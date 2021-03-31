using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    [ObjectSystem]
    public class ILRuntimeComponentAwakeSystem : AwakeSystem<ILRuntimeComponent, bool>
    {
        public override void Awake(ILRuntimeComponent self, bool a)
        {
            self.Awake(a);
        }
    }
    [ObjectSystem]
    public class ILRuntimeComponentUpdateSystem : UpdateSystem<ILRuntimeComponent>
    {
        public override void Update(ILRuntimeComponent self)
        {
            self.Update();
        }
    }
    [ObjectSystem]
    public class ILRuntimeComponentDestroySystem : DestroySystem<ILRuntimeComponent>
    {
        public override void Destroy(ILRuntimeComponent self)
        {
            self.OnDestroy();
        }
    }

    [ObjectSystem]
    public class ILRuntimeComponentLateUpdateSystem : LateUpdateSystem<ILRuntimeComponent>
    {

        public override void LateUpdate(ILRuntimeComponent self)
        {
            self.LateUpdate();
        }
    }

    public static class ILRuntimeComponentSystem
    {

    }
}
