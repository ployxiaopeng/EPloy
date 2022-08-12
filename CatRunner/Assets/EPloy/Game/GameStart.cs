using EPloy.Download;
using UnityEngine;

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
        GameEntry.InitGameModule(GameModel);
        GameEntry.CheckModule.CheckGameModel();
        Instance.GetComponent<ConsoleToSceen>().enabled = false;
#else
            GameEntry.InitGameModule(GameModel.Local); //打包先默认本地模式
            GameEntry.CheckModule.CheckGameModel();
            Instance.GetComponent<ConsoleToSceen>().enabled = true;
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

/// <summary>
/// 非热更游戏入口
/// </summary>
public static partial class GameEntry
{
    public static GameModel GameModel { get; private set; }
    private static string resPath = null;
    public static string ResPath
    {
        get
        {
            if (resPath == null)
            {
                switch (GameModel)
                {
                    case GameModel.Editor:
                        resPath = Application.dataPath;
                        break;
                    case GameModel.EditorHotfix:
                        resPath = Application.streamingAssetsPath;
                        break;
                    case GameModel.EditorABPack:
                        resPath = Application.streamingAssetsPath;
                        break;
                    case GameModel.Local:
                        resPath = Application.streamingAssetsPath;
                        break;
                    case GameModel.HotFix:
                        resPath = Application.dataPath;
                        break;
                }
            }
            return resPath;
        }
    }

    public static TimerModule Timer { get; private set; }

    public static DownLoadModule DownLoad { get; private set; }

    public static FileSystemModule FileSystem { get; private set; }

    public static ObjectPoolMudule ObjectPool { get; private set; }

    public static ResUpdaterModule ResUpdater { get; private set; }

    public static ResModule Res { get; private set; }

    public static AtlasMudule Atlas { get; private set; }

    public static CheckModule CheckModule { get; private set; }

    public static void InitGameModule(GameModel gameModel)
    {
        GameModel = gameModel;
        Timer = GameModuleMgr.CreateModule<TimerModule>();
        DownLoad = GameModuleMgr.CreateModule<DownLoadModule>();
        FileSystem = GameModuleMgr.CreateModule<FileSystemModule>();
        ObjectPool = GameModuleMgr.CreateModule<ObjectPoolMudule>();
        ResUpdater = GameModuleMgr.CreateModule<ResUpdaterModule>();
        Res = GameModuleMgr.CreateModule<ResModule>();
        Atlas = GameModuleMgr.CreateModule<AtlasMudule>();
        CheckModule = GameModuleMgr.CreateModule<CheckModule>();
    }
}