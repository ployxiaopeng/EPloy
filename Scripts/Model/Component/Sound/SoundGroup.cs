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
    public class SoundGroup
    {
        public string Name { get; set; } = null;
        public bool AvoidBeingReplacedBySamePriority { get; set; } = false;
        public bool Mute { get; set; } = false;
        public float Volume { get; set; } = 1f;
        public int AgentHelperCount { get; set; } = 1;
    }
}

