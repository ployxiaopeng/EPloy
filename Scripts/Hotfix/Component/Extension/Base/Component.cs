using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public abstract class Component
    {
        public abstract void Awake();
        public virtual void Update() { }
        public virtual void LateUpdate() { }
    }
}