﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EPloy
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
		protected abstract GameObject cellItem { get; set; }
		protected IList datas;

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
			Create();
		}

		private void OnBeginDrag(GameObject item, PointerEventData eventData)
		{
			list.OnBeginDrag(eventData);
		}

		private void OnEndDrag(GameObject item, PointerEventData eventData)
		{
			list.OnEndDrag(eventData);
		}

		private void OnDrag(GameObject item, PointerEventData eventData)
		{
			list.OnDrag(eventData);
		}

		private void UpdateItem(int index, RectTransform transform)
		{
			transform.SetSiblingIndex(index);
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
			}
			else
			{
				rect = Object.Instantiate(cellItem).GetComponent<RectTransform>();
				rect.name = cellItem.name;
				rect.SetParent(list.content, false);
				UIEventListener.Get(rect.gameObject).onArgBeginDrag = OnBeginDrag;
				UIEventListener.Get(rect.gameObject).onArgDrag = OnDrag;
				UIEventListener.Get(rect.gameObject).onArgEndDrag = OnEndDrag;
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