using System.Collections;
using UnityEngine;

namespace EPloy
{
    public static class GameStart
    {
        public static bool isEditorRes = true;
        public static Game Game;

        public static void Awake(Game game)
        {
            Game = game;
            Log.Info("HotFix Awake");
        }

        public static void Start()
        {
            GameEntry.GameSystem.Add(MuduleConfig.HotFixDllName, Game.ILRuntime.GetHotfixTypes);
            GameEntry.ObjectPool = GameEntry.Game.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.Game.AddComponent<EventComponent>();
            GameEntry.DataStore = GameEntry.Game.AddComponent<DataStoreComponet>();

            GameEntry.Res = GameEntry.Game.AddComponent<ResComponent>();
            GameEntry.UI = GameEntry.Game.AddComponent<UIComponent>();



            GameEntry.UI.OpenUIForm(UIName.StartForm, UIGroupName.Default);
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
