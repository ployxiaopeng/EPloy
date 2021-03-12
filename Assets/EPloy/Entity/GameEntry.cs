using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 游戏老总
    /// </summary>
    public static class GameEntry
    {
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

        public static EventComponent Event { get; set; }
        public static ObjectPoolComponent ObjectPool { get; set; }
        public static ResComponent Res { get; set; }

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
