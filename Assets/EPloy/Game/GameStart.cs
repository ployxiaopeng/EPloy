using UnityEngine;
using EPloy.Res;
using EPloy.Download;

namespace EPloy
{
    public class GameStart : MonoBehaviour
    {
        public static GameStart Instance = null;
        [SerializeField] private bool IsILRuntime = false;
        [SerializeField] private bool EditorResource = true;
        
        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            GameModule.InitGameModule();
            GameModule.Procedure.StartGame(IsILRuntime, EditorResource);
        }

        private void Update()
        {
            GameModuleMgr.ModuleUpdate();
        }

        private void OnDestroy()
        {
            GameModuleMgr.ModuleDestory();
        }
    }
}