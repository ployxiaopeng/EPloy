using System.Collections;
using UnityEngine;

namespace EPloy
{
    public static class HotFixStart
    {
        public static bool isEditorRes = true;
        public static GameStart GameStart;

        public static void Awake(GameStart gameStart)
        {
            GameStart = gameStart;
            Log.Info("HotFix Awake");
            GameEntry.GameSystem.Add(MuduleConfig.HotFixDllName, GameModule.ILRuntime.GetHotfixTypes);
            GameEntry.ObjectPool = GameEntry.GameEntity.AddComponent<ObjectPoolComponent>();
            GameEntry.Event = GameEntry.GameEntity.AddComponent<EventComponent>();
            GameEntry.Fsm = GameEntry.GameEntity.AddComponent<FsmComponent>();
            GameEntry.DataStore = GameEntry.GameEntity.AddComponent<DataStoreComponet>();

            GameEntry.Res = GameEntry.GameEntity.AddComponent<ResComponent>();
            GameEntry.Scene = GameEntry.GameEntity.AddComponent<SceneComponent>();
            GameEntry.UI = GameEntry.GameEntity.AddComponent<UIComponent>();
            GameEntry.DataTable = GameEntry.GameEntity.AddComponent<DataTableComponent>();
            GameEntry.Obj = GameEntry.GameEntity.AddComponent<ObjComponent>();
            GameEntry.Procedure = GameEntry.GameEntity.AddComponent<ProcedureComponent>();
            GameEntry.Atlas = GameEntry.GameEntity.AddComponent<AtlasComponent>();
            GameEntry.Map = GameEntry.GameEntity.AddComponent<MapComponet>();
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
