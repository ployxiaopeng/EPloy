using UnityEngine;

namespace EPloy
{
    public class Init : MonoBehaviour
    {
        public static Init instance = null;
        [SerializeField]
        private bool isILRuntime;
        public bool EditorResource;

        public static ILRuntimeMgr ILRuntimeMgr
        {
            get;
            private set;
        }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            ILRuntimeMgr = ILRuntimeMgr.CreateILRuntimeMgr(isILRuntime);




            ILRuntimeMgr.ILRuntimeInit();
        }


        private void Update()
        {
            ILRuntimeMgr.Update();
        }
    }
}