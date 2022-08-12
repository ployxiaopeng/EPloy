using EPloy.Game.Reference;

namespace EPloy.Game.Sound
{
    /// <summary>
    /// 声音相关常量。
    /// </summary>
    internal static class Constant
    {
        internal const float DefaultTime = 0f;
        internal const bool DefaultMute = false;
        internal const bool DefaultLoop = false;
        internal const int DefaultPriority = 0;
        internal const float DefaultVolume = 1f;
        internal const float DefaultFadeInSeconds = 0f;
        internal const float DefaultFadeOutSeconds = 0f;
        internal const float DefaultPitch = 1f;
        internal const float DefaultPanStereo = 0f;
        internal const float DefaultSpatialBlend = 0f;
        internal const float DefaultMaxDistance = 100f;
        internal const float DefaultDopplerLevel = 1f;
    }

    /// <summary>
    /// 播放声音参数。
    /// </summary>
    public sealed class SoundPlayData : IReference
    {
        /// <summary>
        /// 获取或设置播放位置。
        /// </summary>
        public float Time { get; private set; }
        /// <summary>
        /// 获取或设置在声音组内是否静音。
        /// </summary>
        public bool MuteInSoundGroup { get; private set; }
        /// <summary>
        /// 获取或设置是否循环播放。
        /// </summary>
        public bool Loop { get; private set; }
        /// <summary>
        /// 获取或设置声音优先级。
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// 获取或设置在声音组内音量大小。
        /// </summary>
        public float VolumeInSoundGroup { get; private set; }
        /// <summary>
        /// 获取或设置声音淡入时间，以秒为单位。
        /// </summary>
        public float FadeInSeconds { get; private set; }
        /// <summary>
        /// 获取或设置声音音调。
        /// </summary>
        public float Pitch { get; private set; }
        /// <summary>
        /// 获取或设置声音立体声声相。
        /// </summary>
        public float PanStereo { get; private set; }
        /// <summary>
        /// 获取或设置声音空间混合量。
        /// </summary>
        public float SpatialBlend { get; private set; }
        /// <summary>
        /// 获取或设置声音最大距离。
        /// </summary>
        public float MaxDistance { get; private set; }
        /// <summary>
        /// 获取或设置声音多普勒等级。
        /// </summary>
        public float DopplerLevel { get; private set; }
        internal bool Referenced { get; private set; }
        /// <summary>
        /// 初始化播放声音参数的新实例。
        /// </summary>
        public SoundPlayData()
        {
            Referenced = false;
            Time = Constant.DefaultTime;
            MuteInSoundGroup = Constant.DefaultMute;
            Loop = Constant.DefaultLoop;
            Priority = Constant.DefaultPriority;
            VolumeInSoundGroup = Constant.DefaultVolume;
            FadeInSeconds = Constant.DefaultFadeInSeconds;
            Pitch = Constant.DefaultPitch;
            PanStereo = Constant.DefaultPanStereo;
            SpatialBlend = Constant.DefaultSpatialBlend;
            MaxDistance = Constant.DefaultMaxDistance;
            DopplerLevel = Constant.DefaultDopplerLevel;
        }
        /// <summary>
        /// 创建播放声音参数。
        /// </summary>
        /// <returns>创建的播放声音参数。</returns>
        public static SoundPlayData Create()
        {
            SoundPlayData playSoundParams = ReferencePool.Acquire<SoundPlayData>();
            playSoundParams.Referenced = true;
            return playSoundParams;
        }

        /// <summary>
        /// 清理播放声音参数。
        /// </summary>
        public void Clear()
        {
            Time = Constant.DefaultTime;
            MuteInSoundGroup = Constant.DefaultMute;
            Loop = Constant.DefaultLoop;
            Priority = Constant.DefaultPriority;
            VolumeInSoundGroup = Constant.DefaultVolume;
            FadeInSeconds = Constant.DefaultFadeInSeconds;
            Pitch = Constant.DefaultPitch;
            PanStereo = Constant.DefaultPanStereo;
            SpatialBlend = Constant.DefaultSpatialBlend;
            MaxDistance = Constant.DefaultMaxDistance;
            DopplerLevel = Constant.DefaultDopplerLevel;
        }
    }
}
