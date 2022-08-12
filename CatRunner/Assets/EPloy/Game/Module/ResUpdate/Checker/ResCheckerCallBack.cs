using System;
using System.IO;
using UnityEngine;

namespace EPloy.Res
{
    public class ResCheckerCallBack : IDisposable
    {
        public Action<ResName, string, LoadType, int, int, int, int> NeedUpdate { get; private set; }
        public Action<int, int, int, long, long> CheckComplete { get; private set; }
        public ResCheckerCallBack() : this(null, null)
        {
        }

        public ResCheckerCallBack(Action<ResName, string, LoadType, int, int, int, int> needUpdate, Action<int, int, int, long, long> checkComplete)
        {
            NeedUpdate = needUpdate; CheckComplete = checkComplete;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}