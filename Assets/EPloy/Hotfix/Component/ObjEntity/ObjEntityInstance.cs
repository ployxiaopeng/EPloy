using EPloy.ObjectPool;
using UnityEngine;

namespace EPloy.ObjEntity
{

    /// <summary>
    /// 实体实例对象。
    /// </summary>
    public sealed class ObjEntityInstance : ObjectBase
    {
        private object EntityAsset;

        public ObjEntityInstance()
        {
            EntityAsset = null;
        }

        public static ObjEntityInstance Create(string name, object entityAsset, object entityInstance)
        {
            if (entityAsset == null)
            {
                Log.Error("Entity asset is invalid.");
                return null;
            }


            ObjEntityInstance entityInstanceObject = ReferencePool.Acquire<ObjEntityInstance>();
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

