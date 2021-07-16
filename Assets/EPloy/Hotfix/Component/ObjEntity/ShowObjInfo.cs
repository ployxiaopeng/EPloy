
namespace EPloy.Obj
{
    public sealed class ShowObjInfo : IReference
    {
        public int SerialId
        {
            get;
            private set;
        }

        public int ObjId
        {
            get;
            private set;
        }

        public ObjGroup ObjGroup
        {
            get;
            private set;
        }

        public object UserData
        {
            get;
            private set;
        }

        public static ShowObjInfo Create(int serialId, int objId, ObjGroup objGroup, object userData)
        {
            ShowObjInfo showObjInfo = ReferencePool.Acquire<ShowObjInfo>();
            showObjInfo.SerialId = serialId;
            showObjInfo.ObjId = objId;
            showObjInfo.ObjGroup = objGroup;
            showObjInfo.UserData = userData;
            return showObjInfo;
        }

        public void Clear()
        {
            SerialId = 0;
            ObjId = 0;
            ObjGroup = null;
            UserData = null;
        }
    }
}

