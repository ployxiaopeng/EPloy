using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Res
{
    //
    // 摘要:
    //     检查版本资源列表结果。
    public enum CheckVersionListResult : byte
    {
        //
        // 摘要:
        // 已经是最新的。
        Updated = 0,
        //
        // 摘要:
        // 需要更新。
        NeedUpdate = 1
    }
}