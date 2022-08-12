using EPloy.Game;
using EPloy.Game.ObjectPool;
using EPloy.Game.Reference;
using UnityEngine;

namespace EPloy.Game.Obj
{

    /// <summary>
    /// 实体实例对象。
    /// </summary>
    public sealed class ObjectInstance : ObjectBase
    {
        private object EntityAsset;

        public ObjectInstance()
        {
            EntityAsset = null;
        }

        public static ObjectInstance Create(string name, object entityAsset, object entityInstance)
        {
            if (entityAsset == null)
            {
                Log.Error("Entity asset is invalid.");
                return null;
            }


            ObjectInstance entityInstanceObject = ReferencePool.Acquire<ObjectInstance>();
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

