using UnityEngine;
using System.Collections.Generic;

namespace EPloy
{
    [UIAttribute(UIName.MainMapForm)]
    public class MainMapForm : UIForm
    {
        private GameObject btnUp;
        private GameObject btnDown;
        private GameObject btnLeft;
        private GameObject btnRight;

        private MapCpt mapCpt;
        private MapEntityCpt mapEntityCpt;

        public override void Create()
        {
            btnUp = transform.Find("bottom/Move/btnUp").gameObject;
            btnDown = transform.Find("bottom/Move/btnDown").gameObject;
            btnLeft = transform.Find("bottom/Move/btnLeft").gameObject;
            btnRight = transform.Find("bottom/Move/btnRight").gameObject;

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

        public override void Open(object userData)
        {
            HotFixMudule.UI.CloseUIForm(UIName.LoadingForm);
        }

        public override void Close(object userData)
        {

        }

        private void DirectionUpClick(GameObject go)
        {
            SetMoveDir(MoveDir.Up);
        }

        private void DirectionUpClickUp(GameObject go)
        {
            SetMoveDir(MoveDir.Stop);
        }

        private void DirectionDownClick(GameObject go)
        {
            SetMoveDir(MoveDir.Down);
        }

        private void DirectionDownClickUp(GameObject go)
        {
            SetMoveDir(MoveDir.Stop);
        }

        private void DirectionLeftClick(GameObject go)
        {
            SetMoveDir(MoveDir.Left);
        }

        private void DirectionLeftClickUp(GameObject go)
        {
            SetMoveDir(MoveDir.Stop);
        }

        private void DirectionRightClick(GameObject go)
        {
            SetMoveDir(MoveDir.Right);
        }

        private void DirectionRightClickUp(GameObject go)
        {
            SetMoveDir(MoveDir.Stop);
        }

        private void SetMoveDir(MoveDir moveDir)
        {
            if (mapCpt == null || mapEntityCpt == null)
            {
                mapCpt = HotFixMudule.GameScene.GetSingleCpt<MapCpt>();
                mapEntityCpt = mapCpt.map.GetComponent<MapEntityCpt>();
            }

            mapEntityCpt.role.GetComponent<MoveCpt>().moveDir = moveDir;
        }
    }
}