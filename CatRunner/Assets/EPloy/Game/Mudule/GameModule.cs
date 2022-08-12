using EPloy.Game.Download;
using EPloy.Game.Res;
using EPloy.Game.Sound;
using UnityEngine;

namespace EPloy.Game
{
    /// <summary>
    /// 非热更游戏Module
    /// </summary>
    public static class GameModule
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
                        case GameModel.EditorILRuntime:
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

        public static FileSystemModule FileSystem { get; private set; }

        public static DownLoadModule DownLoad { get; private set; }

        public static ObjectPoolMudule ObjectPool;

        public static ResMudule Res { get; private set; }

        public static ResUpdaterModule ResUpdater { get; private set; }

        public static VersionCheckerModule VersionChecker { get; private set; }

        public static ILRuntimeModule ILRuntime { get; private set; }

        public static ProcedureModule Procedure { get; private set; }

        public static AtlasMudule Atlas { get; private set; }

        public static SoundMudule Sound { get; private set; }

        public static TimerModule Timer { get; private set; }

        public static void InitGameModule(GameModel gameModel)
        {
            GameModel = gameModel;
            Timer = GameModuleMgr.CreateModule<TimerModule>();
            FileSystem = GameModuleMgr.CreateModule<FileSystemModule>();
            ObjectPool = GameModuleMgr.CreateModule<ObjectPoolMudule>();
            VersionChecker = GameModuleMgr.CreateModule<VersionCheckerModule>();
            DownLoad = GameModuleMgr.CreateModule<DownLoadModule>();
            ResUpdater = GameModuleMgr.CreateModule<ResUpdaterModule>();
            Res = GameModuleMgr.CreateModule<ResMudule>();
            ILRuntime = GameModuleMgr.CreateModule<ILRuntimeModule>();
            Atlas = GameModuleMgr.CreateModule<AtlasMudule>();
            Sound = GameModuleMgr.CreateModule<SoundMudule>();
            Procedure = GameModuleMgr.CreateModule<ProcedureModule>();
        }
    }
}
