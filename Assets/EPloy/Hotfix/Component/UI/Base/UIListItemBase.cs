using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EPloy
{
	/// <summary>
	/// 虚拟列表 待定
	/// </summary>
	public abstract class UIListBase : IReference
	{
		private int numItem;
		protected  abstract GameObject template { get; }
		protected List<Transform> childs;
		protected object listData;
		public ScrollRect list { get; private set; }
		public bool horizontal
		{
			get
			{
				return list.horizontal;
			}
		}
		public bool vertical
		{
			get
			{
				return list.vertical;
			}
		}
		public int row{ get; private set; }
		public int column{ get; private set; }
		
		public static T CreateUIList<T>(Transform list) where T : UIListBase
		{
			UIListBase UIList = (UIListBase)ReferencePool.Acquire(typeof(T));
			UIList.childs = new List<Transform>();
			UIList.list = list.GetComponent<ScrollRect>();
			UIList.Create();
			UIList.listData = null;
			return (T)UIList;
		}
		
		public static T CreateUIList<T>(Transform list,int row,int column) where T : UIListBase
		{
			UIListBase UIList = (UIListBase)ReferencePool.Acquire(typeof(T));
			UIList.childs = new List<Transform>();
			UIList.list = list.GetComponent<ScrollRect>();
			UIList.Create();
			UIList.listData = null;
			return (T)UIList;
		}

		/// <summary>
		/// 先随便写一下
		/// </summary>
		/// <param name="data"></param>
		public virtual void SetData(object data,int count)
		{
			if (data==null)
			{
				Log.Error("Can not set data ,data is null" );
				return;
			}
			listData = data;
			numItem = count;
			try
			{
				SetItemObject();
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
				numItem = value;
				SetData(listData,numItem);
			}
		}
		
		private void SetItemObject()
		{
			foreach (var child in childs)
			{
				child.gameObject.SetActive(false);
			}

			for (int i = 0; i < numItem; i++)
			{
				Transform transform = null;
				if (numItem < childs.Count)
				{
					transform = childs[i];
				}
				else
				{
					if (template==null)
					{
						Log.Error("template item is null");
						return;
					}
					transform = Object.Instantiate(template).transform;
					transform.SetParent(list.content);
					transform.localScale = new Vector3(1, 1, 1);
					childs.Add(transform);
				}

				transform.SetSiblingIndex(i);
				transform.gameObject.SetActive(true);
				// 原版本的事件不要被覆盖
				UIEventListener.Get(transform.gameObject).onArgBeginDrag = OnBeginDrag;
				UIEventListener.Get(transform.gameObject).onArgDrag = OnDrag;
				UIEventListener.Get(transform.gameObject).onArgEndDrag = OnEndDrag;
				Itemrenderer(i, transform);
			}
		}

		protected abstract void Itemrenderer(int index, Transform transform);

		protected abstract void Create();
		
		private void OnBeginDrag(GameObject item,PointerEventData eventData)
		{
			((IBeginDragHandler)list).OnBeginDrag(eventData);
		}

		private void OnEndDrag(GameObject item,PointerEventData eventData)
		{
			((IEndDragHandler)list).OnEndDrag(eventData);
		}

		private void OnDrag(GameObject item, PointerEventData eventData)
		{
			((IDragHandler) list).OnDrag(eventData);

			for (int i = 0; i < list.content.childCount; i++)
			{
				Transform temp = list.content.GetChild(i);
				if (horizontal)
				{
					CalculateBoundary(temp.position.x,list.transform.position.x);
					continue;
				}
				CalculateBoundary(temp.position.y,list.transform.position.y);

			}
		}

		private void CalculateBoundary(float value,float standard)
		{
			float dis = value - standard;
		}
		

		public virtual void Clear()
		{
			foreach (var child in childs)
			{
				Object.Destroy(child);
			}
			childs.Clear();
		}
		
	}
}