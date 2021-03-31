using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 角色移动
    /// </summary>
    public class RoleMoveHelper : UIExtenLogic
    {
        private GameObject btnUp;
        private GameObject btnDown;
        private GameObject btnLeft;
        private GameObject btnRight;

        public RoleMoveHelper(Transform transform) : base(transform) { }

        protected override void Find()
        {
            base.Find();
            btnUp = transform.Find("btnUp").gameObject;
            btnDown = transform.Find("btnDown").gameObject;
            btnLeft = transform.Find("btnLeft").gameObject;
            btnRight = transform.Find("btnRight").gameObject;
        }

        protected override void AddEvent()
        {
            base.AddEvent();
            //上
            UIEventListener.Get(btnUp).onClickDown = DirectionUpClick;
            UIEventListener.Get(btnUp).onClickUp = DirectionUpClickUp;
            //下
            UIEventListener.Get(btnDown).onClickDown = DirectionDownClick;
            UIEventListener.Get(btnDown).onClickUp = DirectionDownClickUp;
            //左
            UIEventListener.Get(btnLeft).onClickDown = DirectionLeftClick;
            UIEventListener.Get(btnLeft).onClickUp = DirectionLeftClickUp;
            //右
            UIEventListener.Get(btnRight).onClickDown = DirectionRightClick;
            UIEventListener.Get(btnRight).onClickUp = DirectionRightClickUp;
        }
        //可以设置行走方向
        private List<bool> isHaveClickList = new List<bool> { false, false, false, false };
        private MoveDir roleDir = MoveDir.Stop;
        public void DirectionUpClick(GameObject go)
        {
            isHaveClickList[0] = true;
            roleDir = MoveDir.Up;
        }
        public void DirectionUpClickUp(GameObject go)
        {
            isHaveClickList[0] = false;
            //还有方向键在按下状态
            for (int i = 0; i < isHaveClickList.Count; i++)
            {
                if (i == 0) continue;
                if (isHaveClickList[i])
                {
                    Debug.LogError(i);
                    return;
                }
            }
            //没有方向键按下
            roleDir = MoveDir.Stop;
        }
        public void DirectionDownClick(GameObject go)
        {
            isHaveClickList[1] = true;
            roleDir = MoveDir.Down;
        }
        public void DirectionDownClickUp(GameObject go)
        {
            isHaveClickList[1] = false;
            //还有方向键在按下状态
            for (int i = 0; i < isHaveClickList.Count; i++)
            {
                if (i == 0) continue;
                if (isHaveClickList[i])
                {
                    Debug.LogError(i);
                    return;
                }
            }
            //没有方向键按下
            roleDir = MoveDir.Stop;
        }
        public void DirectionLeftClick(GameObject go)
        {
            isHaveClickList[2] = true;
            roleDir = MoveDir.Left;
        }
        public void DirectionLeftClickUp(GameObject go)
        {
            isHaveClickList[2] = false;
            //还有方向键在按下状态
            for (int i = 0; i < isHaveClickList.Count; i++)
            {
                if (i == 0) continue;
                if (isHaveClickList[i])
                {
                    Debug.LogError(i);
                    return;
                }
            }
            //没有方向键按下
            roleDir = MoveDir.Stop;
        }
        public void DirectionRightClick(GameObject go)
        {
            isHaveClickList[3] = true;
            roleDir = MoveDir.Right;
        }
        public void DirectionRightClickUp(GameObject go)
        {
            isHaveClickList[3] = false;
            //还有方向键在按下状态
            for (int i = 0; i < isHaveClickList.Count; i++)
            {
                if (i == 0) continue;
                if (isHaveClickList[i])
                {
                    Debug.LogError(i);
                    return;
                }
            }
            //没有方向键按下
            roleDir = MoveDir.Stop;
        }

        public void StopMove()
        {
            roleDir = MoveDir.Stop;
            isHaveClickList = new List<bool> { false, false, false, false };
        }

        private RoleLogicBase roleLogic
        {
            get
            {
                return (RoleLogicBase)GameEntry.Extension.GetComponent<MapComponet>().roleData.hotfixEntityLogic;
            }
        }

        public void FixedUpdate()
        {
            if (roleLogic == null) return;
            roleLogic.SetRoleMoveDir(roleDir);
        }
    }
}