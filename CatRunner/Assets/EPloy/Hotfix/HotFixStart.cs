using EPloy.Download;
using System.Collections;
using UnityEngine;

public static class HotFixStart
{
    public static GameStart GameStart { get; private set; }

    /// <summary>
    /// 游戏流程
    /// </summary>
    private static string[] Procedure =
    {
            "ProcedurePreload",
            "ProcedureLogin",
            "ProcedureSwitchScene",
            "ProcedureMap",
        };

    public static void Awake(GameStart gameStart)
    {
        GameStart = gameStart;
        GameModule.InitGameModule(GameEntry.GameModel);
    }

    public static void Start()
    {
        Log.Info("HotFixStart 开始了");
        GameModule.Procedure.RegisterProcedure(Procedure);
        GameModule.Procedure.StartProcedure<ProcedurePreload>();
    }
}

/// <summary>
/// 非热更游戏Module
/// </summary>
public static partial class GameModule
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

    public static ProcedureModule Procedure { get; private set; }

    public static UIModule UI { get; private set; }

    public static DataStoreModule DataStore { get; private set; }

    public static NetModule Net { get; private set; }

    public static SoundModule Sound { get; private set; }

    public static FsmModule Fsm { get; private set; }

    public static EventModule Event { get; private set; }

    public static SceneModule Scene { get; private set; }

    public static TableModule Table { get; private set; }

    public static ObjModule Obj { get; private set; }

    public static void InitGameModule(GameModel gameModel)
    {
        GameModel = gameModel;
        Timer = GameEntry.Timer;
        DownLoad = GameEntry.DownLoad;
        FileSystem = GameEntry.FileSystem;
        ObjectPool = GameEntry.ObjectPool;
        ResUpdater = GameEntry.ResUpdater;
        Res = GameEntry.Res;
        Atlas = GameEntry.Atlas;
        CheckModule = GameEntry.CheckModule;

        Procedure = GameModuleMgr.CreateModule<ProcedureModule>();
        UI = GameModuleMgr.CreateModule<UIModule>();
        Net = GameModuleMgr.CreateModule<NetModule>();
        DataStore = GameModuleMgr.CreateModule<DataStoreModule>();
        Sound = GameModuleMgr.CreateModule<SoundModule>();
        Event = GameModuleMgr.CreateModule<EventModule>();
        Fsm = GameModuleMgr.CreateModule<FsmModule>();
        Scene = GameModuleMgr.CreateModule<SceneModule>();
        Table = GameModuleMgr.CreateModule<TableModule>();
        Obj = GameModuleMgr.CreateModule<ObjModule>();

        ECSModule.ECSActivate();
    }
}
