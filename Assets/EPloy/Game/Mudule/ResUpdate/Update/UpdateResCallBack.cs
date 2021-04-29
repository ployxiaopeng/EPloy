
using System;
using System.IO;
using UnityEngine;

namespace EPloy.Res
{
    /// <summary>
    /// 检查资源回调。
    /// </summary>
    /// <param name="movedCount">已移动的资源数量。</param>
    /// <param name="removedCount">已移除的资源数量。</param>
    /// <param name="updateCount">可更新的资源数量。</param>
    /// <param name="updateTotalLength">可更新的资源总大小。</param>
    /// <param name="updateTotalZipLength">可更新的压缩后总大小。</param>
    public delegate void CheckResCompleteCallback(int movedCount, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength);

    /// <summary>
    /// 资源更新回调
    /// </summary>
    public class UpdateResCallBack : IDisposable
    {
        public EPloyAction<string, bool> ResApplyComplete { get; set; }
        public EPloyAction<string> ResUpdateStart { get; private set; }
        public EPloyAction<string, int> ResUpdateChanged { get; private set; }
        public EPloyAction<string, int> ResUpdateSuccess { get; private set; }
        public EPloyAction<string, string, int, int> ResUpdateFailure { get; private set; }
        public EPloyAction<bool> ResUpdateComplete { get; private set; }

        public UpdateResCallBack(EPloyAction<string> resUpdateStart, EPloyAction<string, int> resUpdateChanged,
        EPloyAction<string, int> resUpdateSuccess, EPloyAction<string, string, int, int> resUpdateFailure, EPloyAction<bool> resUpdateComplete)
         : this(resUpdateStart, resUpdateChanged, resUpdateSuccess, resUpdateFailure, resUpdateComplete, null)
        {
        }

        public UpdateResCallBack(EPloyAction<string> resUpdateStart, EPloyAction<string, int> resUpdateChanged, EPloyAction<string, int> resUpdateSuccess
      , EPloyAction<string, string, int, int> resUpdateFailure, EPloyAction<bool> resUpdateComplete, EPloyAction<string, bool> resApplyComplete)
        {
            ResApplyComplete = resApplyComplete; ResUpdateStart = resUpdateStart; ResUpdateChanged = resUpdateChanged;
            ResUpdateSuccess = resUpdateSuccess; ResUpdateFailure = resUpdateFailure; ResUpdateComplete = resUpdateComplete;
        }
        public void Dispose()
        {
            ResApplyComplete = null; ResUpdateStart = null; ResUpdateChanged = null;
            ResUpdateSuccess = null; ResUpdateFailure = null; ResUpdateComplete = null;
            GC.SuppressFinalize(this);
        }
    }
}