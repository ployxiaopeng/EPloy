
namespace EPloy.ObjEntity
{
    public sealed class ShowObjEntityInfo : IReference
    {
        public int SerialId
        {
            get;
            private set;
        }

        public int EntityId
        {
            get;
            private set;
        }

        public ObjEntityGroup EntityGroup
        {
            get;
            private set;
        }

        public object UserData
        {
            get;
            private set;
        }

        public static ShowObjEntityInfo Create(int serialId, int entityId, ObjEntityGroup entityGroup, object userData)
        {
            ShowObjEntityInfo showEntityInfo = ReferencePool.Acquire<ShowObjEntityInfo>();
            showEntityInfo.SerialId = serialId;
            showEntityInfo.EntityId = entityId;
            showEntityInfo.EntityGroup = entityGroup;
            showEntityInfo.UserData = userData;
            return showEntityInfo;
        }

        public void Clear()
        {
            SerialId = 0;
            EntityId = 0;
            EntityGroup = null;
            UserData = null;
        }
    }
}

