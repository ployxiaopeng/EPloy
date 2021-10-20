using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EPloy
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
		protected abstract GameObject cellItem { get; set; }
		protected IList datas;

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
			((IBeginDragHandler) list).OnBeginDrag(eventData);
		}

		private void OnEndDrag(GameObject item, PointerEventData eventData)
		{
			((IEndDragHandler) list).OnEndDrag(eventData);
		}

		private void OnDrag(GameObject item, PointerEventData eventData)
		{
			((IDragHandler) list).OnDrag(eventData);
		}

		private RectTransform InstantiateItem()
		{
			RectTransform rect = Object.Instantiate(cellItem).GetComponent<RectTransform>();
			rect.name = cellItem.name;
			rect.SetParent(list.content, false);
			rect.localScale = Vector3.one;
			UIEventListener.Get(rect.gameObject).onArgBeginDrag = OnBeginDrag;
			UIEventListener.Get(rect.gameObject).onArgDrag = OnDrag;
			UIEventListener.Get(rect.gameObject).onArgEndDrag = OnEndDrag;
			rect.anchorMax = Vector2.up;
			rect.anchorMin = Vector2.up;
			rect.pivot = Vector2.up;
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