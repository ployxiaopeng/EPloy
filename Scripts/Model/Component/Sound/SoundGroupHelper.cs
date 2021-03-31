//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.Sound;
using UnityEngine;
using UnityEngine.Audio;

namespace ETModel
{
    /// <summary>
    /// 默认声音组辅助器。
    /// </summary>
    public class SoundGroupHelper : ISoundGroupHelper
    {
        public GameObject SoundGroup
        {
            get;
            private set;
        }

        private AudioMixerGroup m_AudioMixerGroup = null;
        /// <summary>
        /// 获取或设置声音组辅助器所在的混音组。
        /// </summary>
        public AudioMixerGroup AudioMixerGroup
        {
            get
            {
                return m_AudioMixerGroup;
            }
            set
            {
                m_AudioMixerGroup = value;
            }
        }

        public SoundGroupHelper(string name, Transform parent)
        {
            SoundGroup = new GameObject(name);
            SoundGroup.transform.SetParent(parent, false);
        }

    }
}
