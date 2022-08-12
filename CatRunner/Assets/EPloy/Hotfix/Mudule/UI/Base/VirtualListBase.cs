using EPloy.Game;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EPloy.Hotfix
{
	/// <summary>
	/// 虚拟列表 
	/// </summary>
	public abstract class VirtualListBase : IReference
	{
		public ScrollRect list { get; private set; }
		public int count { get; private set; }
		
		public RectTransform cellRect
		{
			get => cellItem.transform as RectTransform;
		}

		public RectTransform content
		{
			get => list.content;
		}

		private VirtualList virtualList;
		private GameObject cellItem;
		//语言表用
		private string cellItemName;
		//item点击事件
		protected Action<int,GameObject> onClick;
		protected IList datas;
		private bool isDarg;

		public static T CreateUIList<T>(Transform list) where T : VirtualListBase
		{
			VirtualListBase uIList = (VirtualListBase) ReferencePool.Acquire(typeof(T));
			uIList.Start(list);
			return (T) uIList;
		}

		public void SetData(IList data)
		{
			if (data == null)
			{
				Log.Error("Can not set data ,data is null");
				return;
			}

			datas = data;
			count = data.Count;
			virtualList.SetData(count);
			if (data.Count <= 0) return;
			int updateNum = Mathf.Min(virtualList.items.Count, data.Count);
			for (int i = 0; i < updateNum; i++) UpdateItem(i, virtualList.items[i]);
		}

		public virtual void Refresh()
		{
			virtualList.Refresh();
			int refreshCount = Mathf.Min(virtualList.items.Count, count);
			for (int i = 0; i < refreshCount; i++)
				UpdateItem(i + virtualList.curIndex, virtualList.items[i]);
		}

		protected abstract void Create();

		protected abstract void Itemrenderer(int index, Transform transform);

		private void Start(Transform list)
		{
			this.list = list.GetComponent<ScrollRect>();
			cellItem = this.list.content.GetChild(0).gameObject;
			cellItemName = cellItem.name;
			virtualList = this.list.content.GetComponent<VirtualList>();
			content.anchorMax = Vector2.up;
			content.anchorMin = Vector2.up;
			content.pivot = Vector2.up;
			content.anchoredPosition = new Vector2(0f, 0f);
			Create();
			virtualList.Init(this.list, cellRect.sizeDelta, UpdateItem, InstantiateItem);
		}

		private void UpdateItem(int index, RectTransform transform)
		{
			transform.anchoredPosition = virtualList.ItemPosition(index);
			transform.gameObject.SetActive(true);
			Itemrenderer(index, transform);
			transform.name = index.ToString();
		}

		private void OnBeginDrag(GameObject item, PointerEventData eventData)
		{
			isDarg = true;
			list.OnBeginDrag(eventData);
		}

		private void OnEndDrag(GameObject item, PointerEventData eventData)
		{
			isDarg = false;
			 list.OnEndDrag(eventData);
		}

		private void OnDrag(GameObject item, PointerEventData eventData)
		{
			list.OnDrag(eventData);
		}

		private void OnClickUp(GameObject item)
		{
			if (isDarg) return;
			onClick?.Invoke(int.Parse(item.name), item);
		}

		private RectTransform InstantiateItem()
		{
			RectTransform rect = Object.Instantiate(cellItem).GetComponent<RectTransform>();
			rect.SetParent(list.content, false);
			UIEventListener.Get(rect.gameObject).onArgBeginDrag = OnBeginDrag;
			UIEventListener.Get(rect.gameObject).onArgDrag = OnDrag;
			UIEventListener.Get(rect.gameObject).onArgEndDrag = OnEndDrag;
			UIEventListener.Get(rect.gameObject).onClickUp = OnClickUp;
			rect.anchorMax = Vector2.up;
			rect.anchorMin = Vector2.up;
			// 语言表待定
			return rect;
		}

		public virtual void Clear()
		{
			foreach (var child in virtualList.items)
				Object.Destroy(child);
			virtualList.items.Clear();
			foreach (var child in virtualList.oldItems)
				Object.Destroy(child);
			virtualList.oldItems.Clear();
		}
	}
}