using GameFramework;
using GameFramework.Download;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETModel
{
    [ObjectSystem]
    public class DownloadComponentAwakeSystem : AwakeSystem<DownloadComponent>
    {
        public override void Awake(DownloadComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class DownloadComponentStartSystem : StartSystem<DownloadComponent>
    {
        public override void Start(DownloadComponent self)
        {
            self.Start();
        }
    }
    [ObjectSystem]
    public class DownloadComponentUpdateSystem : UpdateSystem<DownloadComponent>
    {
        public override void Update(DownloadComponent self)
        {
            self.Update();
        }
    }

    public static class DownloadComponentSystem
    {
        private static int DownloadAgentHelperCount = 3;
        private static float Timeout = 30f;
        private static int FlushSize = 1024 * 1024;
        public static void Awake(this DownloadComponent self)
        {
            self.DownloadManager = GameFrameworkEntry.GetModule<IDownloadManager>();
            if (self.DownloadManager == null)
            {
                Log.Fatal("Download manager is invalid.");
                return;
            }
            self.DownloadManager.FlushSize = FlushSize;
            self.DownloadManager.Timeout = Timeout;
            self.DownloadAgentHelpers = new DownloadAgentHelper[DownloadAgentHelperCount];
        }
        public static void Start(this DownloadComponent self)
        {
            for (int i = 0; i < DownloadAgentHelperCount; i++)
                self.AddDownloadAgentHelper(i);
        }
        public static void Update(this DownloadComponent self)
        {
            foreach (var helper in self.DownloadAgentHelpers)
                helper.Update();
        }
    }
}