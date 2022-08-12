using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EPloy.UI
{
	/// <summary>
	/// 普通列表
	/// </summary>
	public abstract class ListBase : IReference
	{
		public ScrollRect list { get; private set; }
		private int numItem;
		private List<RectTransform> items;
		private List<RectTransform> oldItems;
		private GameObject cellItem { get; set; }
		//item点击事件
		protected Action<int, GameObject> onClick;
		protected IList datas;
		private bool isDarg;
		//语言表用
		private string cellItemName;

		public static T CreateUIList<T>(Transform list) where T : ListBase
		{
			ListBase uIList = (ListBase) ReferencePool.Acquire(typeof(T));
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

			Reset();
			datas = data;
			numItem = data.Count;
			try
			{
				for (int i = 0; i < numItem; i++)
				{
					UpdateItem(i, CreateItem());
				}
			}
			catch (Exception e)
			{
				Log.Error(e.ToString());
			}
		}

		public int NumItem
		{
			get { return numItem; }
			set
			{
				if (datas == null)
				{
					Log.Error("Can not set data ,data is null");
					return;
				}

				if (numItem > value)
				{
					for (int i = value; i < numItem; i++)
					{
						RectTransform item = items[0];
						items.Remove(item);
						RemoveItem(item);
					}
				}

				numItem = value;
				for (int i = 0; i < numItem; i++)
				{
					if (i < items.Count)
					{
						items[i].SetSiblingIndex(i);
						items[i].gameObject.SetActive(true);
						Itemrenderer(i, items[i]);
					}
					else UpdateItem(i, CreateItem());
				}
			}
		}

		protected virtual void Reset()
		{
			foreach (var child in items)
			{
				RemoveItem(child);
			}

			items.Clear();
		}

		protected abstract void Create();
		
		protected abstract void Itemrenderer(int index, Transform transform);
		
		private void Start(Transform list)
		{
			items = new List<RectTransform>();
			oldItems = new List<RectTransform>();
			this.list = list.GetComponent<ScrollRect>();
			cellItem = this.list.content.GetChild(0).gameObject;
			cellItemName = cellItem.name;
			oldItems.Add(cellItem.transform as RectTransform);
			Create();
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

		private void UpdateItem(int index, RectTransform transform)
		{
			transform.SetSiblingIndex(index);
			transform.name = index.ToString();
			transform.gameObject.SetActive(true);
			Itemrenderer(index, transform);
			items.Add(transform);
		}

		private RectTransform CreateItem()
		{
			RectTransform rect = null;
			if (oldItems.Count > 0)
			{
				rect = oldItems[0];
				oldItems.RemoveAt(0);
			}
			else
			{
				rect = Object.Instantiate(cellItem).GetComponent<RectTransform>();
				rect.SetParent(list.content, false);
				UIEventListener.Get(rect.gameObject).onArgBeginDrag = OnBeginDrag;
				UIEventListener.Get(rect.gameObject).onArgDrag = OnDrag;
				UIEventListener.Get(rect.gameObject).onArgEndDrag = OnEndDrag;
				UIEventListener.Get(rect.gameObject).onClickUp = OnClickUp;
				// 语言表待定
			}

			return rect;
		}

		private void RemoveItem(RectTransform item)
		{
			oldItems.Add(item);
			item.gameObject.SetActive(false);
		}

		public virtual void Clear()
		{
			Reset();
			foreach (var child in oldItems)
			{
				Object.Destroy(child);
			}

			oldItems.Clear();
		}
	}
}