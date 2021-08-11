using System;

namespace EPloy
{
    /// <summary>
    /// gamesystem type
    /// </summary>
    public enum LifeCycle
    {
        Awake,
        Start,
        Update,
    }

    public class LifeCycleCheck : IReference
    {
        public bool isAwake;
        public bool isStart;
        public bool isUpdate;
        public void Clear()
        {
            isAwake = false;
            isStart = false;
            isUpdate = false;
        }
    }
}