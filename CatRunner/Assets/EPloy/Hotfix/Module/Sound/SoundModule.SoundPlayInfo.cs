
using EPloy.Sound;
/// <summary>
/// 播放声音错误码。
/// </summary>
public enum PlaySoundErrorCode : byte
    {
        /// <summary>
        /// 未知错误。
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 声音组不存在。
        /// </summary>
        SoundGroupNotExist,
        /// <summary>
        /// 声音组没有声音代理。
        /// </summary>
        SoundGroupHasNoAgent,
        /// <summary>
        /// 加载资源失败。
        /// </summary>
        LoadAssetFailure,
        /// <summary>
        /// 播放声音因优先级低被忽略。
        /// </summary>
        IgnoredDueToLowPriority,
        /// <summary>
        /// 设置声音资源失败。
        /// </summary>
        SetSoundAssetFailure
    }

/// <summary>
/// 声音管理器。
/// </summary>
public sealed partial class SoundModule : IGameModule
{
    private sealed class SoundPlayInfo : IReference
    {
        public int SerialId { get; private set; }
        public SoundGroup SoundGroup { get; private set; }
        public SoundPlayData PlaySoundParams { get; private set; }

        public SoundPlayInfo()
        {
            SerialId = 0;
            SoundGroup = null;
            PlaySoundParams = null;
        }

        public static SoundPlayInfo Create(int serialId, SoundGroup soundGroup, SoundPlayData playSoundParams)
        {
            SoundPlayInfo playSoundInfo = ReferencePool.Acquire<SoundPlayInfo>();
            playSoundInfo.SerialId = serialId;
            playSoundInfo.SoundGroup = soundGroup;
            playSoundInfo.PlaySoundParams = playSoundParams;
            return playSoundInfo;
        }

        public void Clear()
        {
            SerialId = 0;
            SoundGroup = null;
            PlaySoundParams = null;
        }
    }
}
