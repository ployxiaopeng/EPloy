using System.Collections;
using UnityEngine;

namespace EPloy
{
    public static class GameStart
    {
        public static bool isEditorRes = true;
        public static Transform Init;

        public static void Awake(Transform init)
        {
            Init = init;
        }
        public static void Start()
        {
            GameEntry.GameSystem.Add(Config.HotFixDllName, typeof(GameStart).Assembly);

            GameEntry.FileSystem = GameEntry.Game.AddComponent<FileSystemComponent>();
            GameEntry.ObjectPool = GameEntry.Game.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.Game.AddComponent<EventComponent>();
            GameEntry.DataStore = GameEntry.Game.AddComponent<DataStoreComponet>();

            GameEntry.Res = GameEntry.Game.AddComponent<ResComponent>();
            GameEntry.UI = GameEntry.Game.AddComponent<UIComponent>();
        }


        public static void Update()
        {
            GameEntry.GameSystem.Update();
        }
    }
}
