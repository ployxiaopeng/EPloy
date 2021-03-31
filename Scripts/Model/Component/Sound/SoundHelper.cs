//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2019 Jiang Yin. All rights reserved.
// Homepage: http://gameframework.cn/
// Feedback: mailto:jiangyin@gameframework.cn
//------------------------------------------------------------

using GameFramework.Sound;

namespace ETModel
{
    /// <summary>
    /// 默认声音辅助器。
    /// </summary>
    public class SoundHelper : ISoundHelper
    {

        /// <summary>
        /// 释放声音资源。
        /// </summary>
        /// <param name="soundAsset">要释放的声音资源。</param>
        public void ReleaseSoundAsset(object soundAsset)
        {
            Init.Resource.UnloadAsset(soundAsset);
        }


    }
}
