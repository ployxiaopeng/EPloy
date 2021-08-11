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
            GameEntry.GameSystem.Add(MuduleConfig.HotFixDllName, Game.ILRuntime.GetHotfixTypes);
            GameEntry.ObjectPool = GameEntry.Game.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.Game.AddComponent<EventComponent>();
            GameEntry.Fsm = GameEntry.Game.AddComponent<FsmComponent>();
            GameEntry.DataStore = GameEntry.Game.AddComponent<DataStoreComponet>();

            GameEntry.Res = GameEntry.Game.AddComponent<ResComponent>();
            GameEntry.Scene = GameEntry.Game.AddComponent<SceneComponent>();
            GameEntry.UI = GameEntry.Game.AddComponent<UIComponent>();
            GameEntry.DataTable = GameEntry.Game.AddComponent<DataTableComponent>();
            GameEntry.Obj = GameEntry.Game.AddComponent<ObjComponent>();
            GameEntry.Procedure = GameEntry.Game.AddComponent<ProcedureComponent>();
            GameEntry.Atlas = GameEntry.Game.AddComponent<AtlasComponent>();
            GameEntry.Map = GameEntry.Game.AddComponent<MapComponet>();
        }

        public static void Start()
        {
            GameEntry.Procedure.StartProcedure<ProcedurePreload>();
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
