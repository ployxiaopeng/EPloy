using UnityEngine;
using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
    public class MapGrid : UIExtenLogic
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

        public MapGrid(Transform transform) : base(transform)
        {
            mainSprite = transform.GetComponent<SpriteRenderer>();
            bgSprite = transform.Find("bgSprite").GetComponent<SpriteRenderer>();
            frontSprite = transform.Find("frontSprite").GetComponent<SpriteRenderer>();
        }
        public void Init(DRMapCell _mapCell)
        {
            Reset();
            if (_mapCell == null)
            {
                Log.Info("没有找到 格子数据 坐标 {0}", transform.localPosition);
                return;
            }
            mapCell = _mapCell;
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