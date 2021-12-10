
using EPloy.Download;
using EPloy.Res;

namespace EPloy
{
    /// <summary>
    /// 非热更游戏Module
    /// </summary>
    public static class GameModule
    {
        public static FileSystemModule FileSystem { get; private set; }

        public static DownLoadModule DownLoad { get; private set; }

        public static ResUpdaterModule ResUpdater { get; private set; }

        public static VersionCheckerModule VersionChecker { get; private set; }

        public static TsEvaModule TsEva { get; private set; }

        public static ProcedureModule Procedure { get; private set; }

        public static TimerModule Timer { get; private set; }

        public static void InitGameModule()
        {
            Timer = GameModuleMgr.CreateModule<TimerModule>();
            FileSystem = GameModuleMgr.CreateModule<FileSystemModule>();
            VersionChecker = GameModuleMgr.CreateModule<VersionCheckerModule>();
            DownLoad = GameModuleMgr.CreateModule<DownLoadModule>();
            ResUpdater = GameModuleMgr.CreateModule<ResUpdaterModule>();
            TsEva = GameModuleMgr.CreateModule<TsEvaModule>();
            Procedure = GameModuleMgr.CreateModule<ProcedureModule>();
        }
    }
}
