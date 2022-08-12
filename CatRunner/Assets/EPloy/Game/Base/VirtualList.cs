using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;


    enum Direction
    {
        Horizontal, //水平方向
        Vertical //垂直方向
    }

public class VirtualList : MonoBehaviour
{
    [SerializeField, Header("X上Y下边界")] private Vector2 verticalPadding = new Vector2(0, 0);
    [SerializeField, Header("间隔")] private Vector2 cellGap = new Vector2(0, 0);
    [SerializeField, Header("方向")] Direction direction = Direction.Horizontal;
    [SerializeField, Header("行列")] private Vector2 page = new Vector2(1, 1);

    [SerializeField, Header("增加缓存"), Range(0, 8)]
    private int cache;

    public List<RectTransform> items { get; private set; }
    public List<RectTransform> oldItems { get; private set; }
    public int curIndex { get; private set; }

    private bool isFirst;
    private ScrollRect list;
    private int dataCount;
    private Vector2 itemSize;
    private float prevPos;
    private Vector2 cellRect;

    private Action<int, RectTransform> updateItem;
    private Func<RectTransform> instantiateItem;

    private float CellScale
    {
        get { return direction == Direction.Horizontal ? cellRect.x : cellRect.y; }
    }

    //实际应该生成的格子数(行x列)
    private int CellCount
    {
        get { return (int)ItemSize.x * (int)ItemSize.y; }
    }

    //当前移动的方向位置
    private float DirectionPos
    {
        get
        {
            return direction == Direction.Horizontal
                ? list.content.anchoredPosition.x
                : list.content.anchoredPosition.y;
        }
    }

    private Vector2 ItemSize
    {
        get
        {
            if (itemSize == Vector2.zero)
            {
                float rows, cols;
                if (direction == Direction.Horizontal)
                {
                    rows = page.x; //行数
                    cols = page.y + cache; //列数(+缓存的格子数)
                }
                else
                {
                    rows = page.x + cache;
                    cols = page.y;
                }

                itemSize = new Vector2(rows, cols);
            }

            return itemSize;
        }
    }

    private int PageScale
    {
        get { return direction == Direction.Horizontal ? (int)page.x : (int)page.y; }
    }

    private float MaxPrevPos
    {
        get
        {
            float result;
            Vector2 max = ListPage(dataCount);
            if (direction == Direction.Horizontal)
            {
                result = max.y - page.y;
            }
            else
            {
                result = max.x - page.x;
            }

            return result * CellScale;
        }
    }

    //当坐标系位于左上角时的相对位置
    private float Scale
    {
        get { return direction == Direction.Horizontal ? 1f : -1f; }
    }

    public void Init(ScrollRect list, Vector2 cellRect,
        Action<int, RectTransform> updateItem, Func<RectTransform> instantiateItem)
    {
        items = new List<RectTransform>();
        oldItems = new List<RectTransform>();
        this.list = list;
        this.list.horizontal = direction == Direction.Horizontal;
        this.list.vertical = direction == Direction.Vertical;
        this.cellRect = cellGap + cellRect;
        this.updateItem = updateItem;
        this.instantiateItem = instantiateItem;
        oldItems.Add(this.list.content.GetChild(0).transform as RectTransform);
        itemSize = Vector2.zero;
        isFirst = true;
    }

    public void SetData(int dataCount)
    {
        Reset();
        this.dataCount = dataCount;
        SetContentSizeDslta();
        if (this.dataCount > CellCount)
        {
            while (items.Count < CellCount)
            {
                CreateItem();
            }
        }
        else
        {
            while (items.Count > this.dataCount)
            {
                RemoveItem(items.Count - 1);
            }

            while (items.Count < this.dataCount)
            {
                CreateItem();
            }
        }
    }

    public void Refresh()
    {
        SetContentSizeDslta();
        if (dataCount <= 0)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].gameObject.SetActive(false);
                oldItems.Add(items[i]);
            }
        }

        int refreshCount = Mathf.Min(items.Count, dataCount);
        if (curIndex + refreshCount > dataCount) curIndex = 0;
    }

    public Vector2 ItemPosition(int index)
    {
        return direction == Direction.Horizontal
            ? new Vector2(Mathf.Floor(index / ItemSize.x) * cellRect.x,
                -(index % ItemSize.x) * cellRect.y)
            : new Vector2((index % ItemSize.y) * cellRect.x,
                -Mathf.Floor(index / ItemSize.y) * cellRect.y - verticalPadding.x);
    }

    private void Reset()
    {
        list.content.anchoredPosition = new Vector2(0f, 0);
        for (int i = 0; i < items.Count; i++)
        {
            items[i].gameObject.SetActive(false);
            oldItems.Add(items[i]);
        }

        items.Clear();
        prevPos = 0;
        curIndex = 0;
    }

    private void SetContentSizeDslta()
    {
        int PageCount = (int)page.x * (int)page.y;
        if (dataCount > PageCount)
        {
            SetContentSize(ListPage(dataCount));
        }
        else
        {
            SetContentSize(page);
        }
    }

    // 由格子数量获取多少行多少列
    private Vector2 ListPage(int index)
    {
        return direction == Direction.Horizontal
            ? new Vector2(page.x, Mathf.CeilToInt(index / page.x))
            : new Vector2(Mathf.CeilToInt(index / page.y), page.y);
    }

    // 设置content的大小
    private void SetContentSize(Vector2 bound)
    {
        list.content.sizeDelta =
            new Vector2(bound.y * cellRect.x, bound.x * cellRect.y + verticalPadding.x + verticalPadding.y);
    }

    private void Update()
    {
        if (isFirst)
        {
            isFirst = false;
            return;
        }

        if (dataCount == 0) return;
        while (Scale * DirectionPos - prevPos < -CellScale * 2)
        {
            //Down
            if (prevPos <= -MaxPrevPos) return;
            prevPos -= CellScale;
            List<RectTransform> range = items.GetRange(0, PageScale);
            items.RemoveRange(0, PageScale);
            items.AddRange(range);
            for (int i = 0; i < range.Count; i++)
            {
                int index = curIndex * PageScale + items.Count + i;
                if (index < dataCount) updateItem(index, range[i]);
            }

            curIndex++;
        }

        while (Scale * DirectionPos - prevPos > -CellScale)
        {
            //Up
            if (Mathf.RoundToInt(prevPos) >= 0) return;
            prevPos += CellScale;
            curIndex--;
            if (curIndex < 0) return;
            List<RectTransform> range =
                items.GetRange(items.Count - PageScale, PageScale);
            items.RemoveRange(items.Count - PageScale, PageScale);
            items.InsertRange(0, range);
            for (int i = 0; i < range.Count; i++)
            {
                int index = curIndex * PageScale + i;
                if (index < dataCount) updateItem(index, range[i]);
            }
        }
    }

    private void CreateItem()
    {
        RectTransform rect;
        if (oldItems.Count > 0)
        {
            rect = oldItems[0];
            oldItems.Remove(rect);
        }
        else
        {
            rect = instantiateItem();
        }

        items.Add(rect);
    }

    private void RemoveItem(int index)
    {
        RectTransform item = items[index];
        items.Remove(item);
        oldItems.Add(item);
        item.gameObject.SetActive(false);
    }
}
