//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using System;
using UnityEngine;

namespace ETModel
{
    public class EntityGroup
    {
        public string Name { get; set; } = null;
        public float InstanceAutoReleaseInterval { get; set; } = 60f;
        public int InstanceCapacity { get; set; } = 16;
        public float InstanceExpireTime { get; set; } = 60f;
        public int InstancePriority { get; set; } = 0;
    }
}
