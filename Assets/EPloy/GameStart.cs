using System.Collections;
using UnityEngine;

namespace EPloy
{
    public static class GameStart
    {
        public static bool isEditorRes = true;
        public static Init InitMono;

        public static void Awake(Init mono)
        {
            InitMono = mono;
            Log.Info("HotFix Awake");
        }

        public static void Start()
        {
            GameEntry.GameSystem.Add(Config.HotFixDllName, Init.ILRuntimeMgr.GetHotfixTypes);

            GameEntry.FileSystem = GameEntry.Game.AddComponent<FileSystemComponent>();
            GameEntry.ObjectPool = GameEntry.Game.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.Game.AddComponent<EventComponent>();
            GameEntry.DataStore = GameEntry.Game.AddComponent<DataStoreComponet>();

            GameEntry.Res = GameEntry.Game.AddComponent<ResComponent>();
            GameEntry.UI = GameEntry.Game.AddComponent<UIComponent>();



            GameEntry.UI.OpenUIForm(UIName.StartUI, GroupName.Default);
        }

        public static void Update()
        {
            GameEntry.GameSystem.Update();
        }
        public static void LateUpdate()
        {

        }
        public static void FixUpdate()
        {

        }

        public static void OnDestroy()
        {

        }
    }
}
