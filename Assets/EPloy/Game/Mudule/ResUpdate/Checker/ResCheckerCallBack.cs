
using System;
using System.IO;
using UnityEngine;

namespace EPloy.Res
{
    public class ResCheckerCallBack : IDisposable
    {
        public EPloyAction<ResName, string, LoadType, int, int, int, int> NeedUpdate { get; private set; }
        public EPloyAction<int, int, int, long, long> CheckComplete { get; private set; }
        public ResCheckerCallBack() : this(null, null)
        {
        }

        public ResCheckerCallBack(EPloyAction<ResName, string, LoadType, int, int, int, int> needUpdate, EPloyAction<int, int, int, long, long> checkComplete)
        {
            NeedUpdate = needUpdate; CheckComplete = checkComplete;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}