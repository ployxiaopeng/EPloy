using UnityEngine;
using System.Collections.Generic;

namespace EPloy
{
    public class MapGrid : Component
    {
        private SpriteRenderer mainSprite;
        private SpriteRenderer bgSprite;
        private SpriteRenderer frontSprite; //暂时不设置
        public int posX
        {
            get { return (int)mapCell.CellIndex.x; }
        }
        public int posY
        {
            get { return (int)mapCell.CellIndex.y; }
        }
        public DRMapCell mapCell { get; private set; }
        public Transform transform;

        public MapGrid(Transform transform)
        {
            this.transform = transform;
            mainSprite = transform.GetComponent<SpriteRenderer>();
            bgSprite = transform.Find("bgSprite").GetComponent<SpriteRenderer>();
            frontSprite = transform.Find("frontSprite").GetComponent<SpriteRenderer>();
        }

        public void Init(DRMapCell mapCell)
        {
            Reset();
            if (mapCell == null)
            {
                Log.Info(Utility.Text.Format("没有找到 格子数据 坐标 {0}", transform.localPosition));
                return;
            }

            this.mapCell = mapCell;
            SetMainSprite();
            SetbgSprite();
        }

        public void Init()
        {
            Reset();
        }

        public void Reset()
        {
            if (bgSprite.sprite != null) bgSprite.sprite = null;
            if (frontSprite.sprite != null) frontSprite.sprite = null;
        }
        //主图片
        private void SetMainSprite()
        {
            if (mapCell.ResMain == -1)
            {
                Log.Error(string.Format("没有找到 '{0}'格子数据", mapCell.CellIndex));
                return;
            }
            mainSprite.SetSprite(mapCell.ResMain);
            mainSprite.transform.localEulerAngles = mapCell.ResMainRotate;
        }
        //背景图片
        private void SetbgSprite()
        {
            if (mapCell.ResBg == -1) return;
            bgSprite.SetSprite(mapCell.ResBg);
            bgSprite.transform.localEulerAngles = mapCell.ResBgRotate;
        }
    }
}