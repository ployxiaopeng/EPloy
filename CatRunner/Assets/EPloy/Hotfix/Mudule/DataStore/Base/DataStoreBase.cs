using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Hotfix
{
	public abstract class DataStoreBase : IReference
	{
		protected bool isResetChangeSecene = false;
		public abstract void Create();
		public virtual void Reset(){}
		public virtual void Clear(){}
	}
}