﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 游戏老总
    /// </summary>
    public static class GameEntry
    {
        public static ObjectPoolComponent ObjectPool { get; set; }
        public static FsmComponent Fsm { get; set; }
        public static EventComponent Event { get; set; }
        public static DataStoreComponet DataStore { get; set; }
        public static ResComponent Res { get; set; }
        public static SceneComponent Scene { get; set; }

        public static DataTableComponent DataTable { get; set; }
        public static UIComponent UI { get; set; }
        public static ObjComponent Obj { get; set; }
        public static ProcedureComponent Procedure { get; set; }
        public static AtlasComponent Atlas { get; set; }

        private static GameEntity game = null;
        public static GameEntity Game
        {
            get
            {
                if (game == null)
                {
                    game = ReferencePool.Acquire<GameEntity>();
                    game.Awake(0, "Game");
                }
                return game;
            }
        }

        private static GameSystem gameSystem = null;
        public static GameSystem GameSystem
        {
            get
            {
                if (gameSystem == null)
                {
                    gameSystem = new GameSystem();
                }
                return gameSystem;
            }
        }
    }
}
