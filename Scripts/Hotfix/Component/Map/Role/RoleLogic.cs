using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace ETHotfix
{
    public class RoleLogic : RoleLogicBase
    {
        private MapComponet MapCom
        {
            get
            {
                return GameEntry.Extension.GetComponent<MapComponet>();
            }
        }

        #region 角色移动字段
        private float moveSpeed
        {
            get
            {
                return 1.5f;
            }
        }
        private float rotTime = 0.35f;
        private Vector3 targetRot;
        public MoveDir lastDir { get; private set; }             //角色上次停止时方向
        public MoveDir Dir { get; private set; }                 //当前角色移动方向
        private Vector2 deltaNewVec;                             //下一个移动的坐标
        private Vector2 curStandVec;                               //当前角色所在坐标
        private bool FontPass                                   //前方路是否通畅
        {
            get { return MapCom.FontPass(Dir); }
        }
        private bool isReachTarge = false;
        #endregion

        public override void OnShow(object userData)
        {
            base.OnShow(userData);
            lastDir = Dir = MoveDir.Stop;
            transform.localScale = Vector3.one * 0.7f;
            deltaNewVec = default(Vector2); isReachTarge = false;
            transform.localPosition = MapCom.MapData.RoleBornPos;
            transform.localEulerAngles = MapCom.MapData.RolelRotate;
            MapCom.StartCamera(transform);
        }
        public override void OnUpdate()
        {
            if (isReachTarge)  //已经到达当前目标格子,下一帧判断是否要继续设置下一个目标格子
            {
                isReachTarge = false;
                if (Dir != MoveDir.Stop && FontPass)
                {
                    localPosition += GetVector2Direction(Dir) * moveSpeed * Time.deltaTime;
                    deltaNewVec = GetDeltaNewVec(Dir);
                    return;
                }
                deltaNewVec = default(Vector2);
                SetStateStand();
            }

            if (Dir != MoveDir.Stop && !FontPass) //前方路是否同
                Dir = MoveDir.Stop;

            //移动
            if (deltaNewVec == default(Vector2)) return;
            SetStateWalk();
            localPosition = Vector2.MoveTowards(localPosition, deltaNewVec, Time.fixedDeltaTime * moveSpeed);
            // 到达目的
            if (Vector2.Distance(localPosition, deltaNewVec) > 0.01f) return;
            localPosition = deltaNewVec;
            curStandVec = localPosition;
            MapCom.UpdateRolePos(curStandVec, lastDir, true);
            isReachTarge = true;
        }

        public override void SetRoleMoveDir(MoveDir _dir)
        {
            //设置按键方向不变
            if (this.Dir == _dir) return;
            //停止
            if (_dir == MoveDir.Stop)
            {
                Dir = _dir;
                return;
            }
            //按键方向改变
            Dir = MoveDir.Stop;
            if (roleAction != RoleAction.Stand) return;
            Dir = _dir;
            SetRoleMoveRotate(_dir);
        }

        private void SetRoleMoveRotate(MoveDir _MoveDir)
        {
            targetRot = GetRotateirection(Dir);
            transform.DORotate(targetRot, rotTime);
            if (!FontPass)
            {

            }
            else
            {
                //开始行动
                localPosition += GetVector2Direction(Dir) * moveSpeed * Time.deltaTime;
                deltaNewVec = GetDeltaNewVec(Dir);
            }
            if (lastDir != Dir) lastDir = Dir;
        }
        public override void SetRoleRotate(MoveDir MoveDir)
        {
            if (lastDir == MoveDir) return;
            targetRot = GetRotateirection(MoveDir);
            lastDir = MoveDir;
            SetStateRotate();
            //旋转角色角度
            transform.DORotate(targetRot, rotTime);
        }
        public override void SetRoleTransfer(Vector2 target)
        {
            localPosition = target;
            MapCom.UpdateRolePos(curStandVec, lastDir, true);
        }

        #region 动作切换
        public override void SetStateStand()
        {
            if (roleAction == RoleAction.Stand) return;
            roleAction = RoleAction.Stand;
            PlayAnim(roleAction.ToString());
        }
        public override void SetIdle()
        {
            roleAction = RoleAction.Stand;
        }

        public override void SetStateCut()
        {
            if (roleAction == RoleAction.Cut) return;
            roleAction = RoleAction.Cut;
            PlayAnim(roleAction.ToString());
        }
        public override void SetStateRotate()
        {
            if (roleAction == RoleAction.Rotate) return;
            roleAction = RoleAction.Rotate;
        }
        public override void SetStateSearch()
        {
            if (roleAction == RoleAction.Search) return;
            roleAction = RoleAction.Search;
            PlayAnim(roleAction.ToString());
        }
        public override void SetStateWalk()
        {
            if (roleAction == RoleAction.Walk) return;
            roleAction = RoleAction.Walk;
            PlayAnim(roleAction.ToString());
        }
        #endregion

        public override void SetRoleSearchPath(List<Vector2> pathList)
        {
            //StartCoroutine(SearchPath(pathList));
        }
        bool isHave = false;
        IEnumerator SearchPath(List<Vector2> pathList)
        {
            int index = 1;
            MoveDir dir = MoveDir.Stop;
            while (index < pathList.Count)
            {
                yield return new WaitForFixedUpdate();
                if (!isHave)
                {
                    dir = GetSearchPathDirection(localPosition, pathList[index]);
                    isHave = true;
                }

                if (Vector2.Distance(localPosition, pathList[index]) > 0.01f)
                {
                    SetRoleMoveDir(dir);
                    continue;
                }
                index++;
                isHave = false;
                yield return new WaitForFixedUpdate();
            }
            SetRoleMoveDir(MoveDir.Stop);
            //GameEntry.UI.CloseUICommon(UICommon.BackMaskWnd);
        }
        private MoveDir GetSearchPathDirection(Vector2 start, Vector2 end)
        {
            Vector2 vector = end - start;
            if (vector.x > 0) return MoveDir.Right;
            if (vector.x < 0) return MoveDir.Left;
            if (vector.y > 0) return MoveDir.Up;
            if (vector.y < 0) return MoveDir.Down;
            return MoveDir.Stop;
        }
        public override void SetRoleActive(bool isActive)
        {
            transform.gameObject.SetActive(isActive);
        }
    }
}