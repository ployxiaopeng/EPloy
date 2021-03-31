using GameFramework.DataTable;
using GameFramework.Event;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    //地图主角相关
    public partial class MapComponet : Component
    {
        private Vector2 roleVec;                                            //角色上一次更新的坐标
        private float RecordSteps;                                               //记录未遇敌步速
        private float StepsX, StepsY;
        private Queue<MapGrid> MapEventQue;                       //触发优先级 1.本格子事件 2. 角色朝向事件 3.其他方向事件
        private MapGrid EventExecuteGrid;
        private void InitRole(Vector2 roleBornPos)
        {
            MapEventQue = new Queue<MapGrid>();
            roleVec = roleBornPos;
            RecordSteps = 0;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="rolePos"></param>
        /// <param name="dir"></param>
        /// <param name="isEnermy"></param>
        public void UpdateRolePos(Vector2 rolePos, MoveDir dir, bool isEnermy)
        {
            CreateMap(MapReqionId, rolePos, dir);
            if (isEnermy)   //计算随机遇敌
            {
                //移动一格才算
                StepsX = Mathf.Abs(rolePos.x - roleVec.x);
                StepsY = Mathf.Abs(rolePos.y - roleVec.y);

                if (StepsX + StepsY < 1) return;
                RecordSteps += 1;
                roleVec = rolePos;
                RandomEnemy(rolePos);
                return;
            }
            //不计算 一般只有传送会用
            RecordSteps = 0;
            roleVec = rolePos;
        }
        //随机遇敌
        private void RandomEnemy(Vector2 rolePos)
        {
            float prob1 = Random.Range(0, 100);
            if (prob1 > 20f) return;
            Log.Error("随机遇敌.......");
            //GameEntry.UI.OpenUIWnd(UIWnd.MapBattleWnd, 1);
        }
        /// <summary>
        /// 判断角色前方能不能通过
        /// </summary>
        /// <param name="_MoveDir"></param>
        /// <returns></returns>
        public bool FontPass(MoveDir _MoveDir)
        {
            switch (_MoveDir)
            {
                case MoveDir.Up:
                    return GetMapCellPass(roleVec + Vector2.up);
                case MoveDir.Down:
                    return GetMapCellPass(roleVec + Vector2.down);
                case MoveDir.Left:
                    return GetMapCellPass(roleVec + Vector2.left);
                case MoveDir.Right:
                    return GetMapCellPass(roleVec + Vector2.right);
            }
            return false;
        }
    }
}