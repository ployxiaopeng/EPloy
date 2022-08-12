using UnityEngine;
using EPloy.Game;

public class GameStart : MonoBehaviour
{
    public static GameStart Instance = null;
    [Header("游戏模式")]
    [Header("HotFix热更模式")]
    [Header("Local本地模式 打包已默认")]
    [Header("EditorILRuntime 编辑器ILRuntime模式")]
    [Header("EditorABPack  编辑器AB包测试模式")]
    [Header("Editor 编辑器模式")]
    [SerializeField] private GameModel GameModel;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
#if UNITY_EDITOR
        GameModule.InitGameModule(GameModel);
        GameModule.Procedure.StartGame();
#else
            GameModule.InitGameModule(GameModel.Local); //打包先默认本地模式
            GameModule.Procedure.StartGame();
#endif
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
