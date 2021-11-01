using UnityEngine;

namespace EPloy
{
    /// <summary>
    /// 地图格子组件
    /// </summary>
    public class MapGirdCpt : Component
    {
        public bool isUpdate;
        public Transform transform;
        public SpriteRenderer mainSprite;
        public SpriteRenderer bgSprite;
        public SpriteRenderer frontSprite; //暂时不设置
        public DRMapCell mapCell { get; private set; }

        public void UpdateMapCell(DRMapCell mapCell)
        {
            isUpdate = true;
            this.mapCell = mapCell;
        }

        public int posX
        {
            get { return (int) mapCell.CellIndex.x; }
        }

        public int posY
        {
            get { return (int) mapCell.CellIndex.y; }
        }

        public void Init(Transform transform)
        {
            this.transform = transform;
            mainSprite = transform.GetComponent<SpriteRenderer>();
            bgSprite = transform.Find("bgSprite").GetComponent<SpriteRenderer>();
            frontSprite = transform.Find("frontSprite").GetComponent<SpriteRenderer>();
        }
    }
}

