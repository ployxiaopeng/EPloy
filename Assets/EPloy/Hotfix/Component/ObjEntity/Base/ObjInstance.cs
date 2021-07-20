using EPloy.ObjectPool;
using UnityEngine;

namespace EPloy.Obj
{

    /// <summary>
    /// 实体实例对象。
    /// </summary>
    public sealed class ObjInstance : ObjectBase
    {
        private object EntityAsset;

        public ObjInstance()
        {
            EntityAsset = null;
        }

        public static ObjInstance Create(string name, object entityAsset, object entityInstance)
        {
            if (entityAsset == null)
            {
                Log.Error("Entity asset is invalid.");
                return null;
            }


            ObjInstance entityInstanceObject = ReferencePool.Acquire<ObjInstance>();
            entityInstanceObject.Initialize(name, entityInstance);
            entityInstanceObject.EntityAsset = entityAsset;
            return entityInstanceObject;
        }

        public override void Clear()
        {
            base.Clear();
            // EntityHelper.ReleaseEntity(EntityAsset, Target);
            GameObject.Destroy((Object)Target);
            EntityAsset = null;
        }
    }
}

