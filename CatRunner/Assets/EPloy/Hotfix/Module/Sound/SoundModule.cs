using EPloy.Res;
using System;
using System.Collections.Generic;
using UnityEngine;
using EPloy.Sound;

    /// <summary>
    /// 暂定声音组
    /// </summary>
    public enum SoundGroupName
    {
        D2,
        D3,
    }

/// <summary>
/// 声音管理器
/// </summary>
public sealed partial class SoundModule : IGameModule
{
    private Dictionary<string, SoundGroup> SoundGroups;
    private List<int> SoundsBeingLoaded;
    private HashSet<int> SoundsToReleaseOnLoad;
    private LoadAssetCallbacks LoadAssetCallbacks;
    private int Seriald;
    private Transform SoundParent;

    public void Awake()
    {
        Seriald = 0;
        SoundGroups = new Dictionary<string, SoundGroup>(StringComparer.Ordinal);
        SoundsBeingLoaded = new List<int>();
        SoundsToReleaseOnLoad = new HashSet<int>();
        LoadAssetCallbacks = new LoadAssetCallbacks(LoadAssetSuccessCallback, LoadAssetFailureCallback);
        SoundParent = GameStart.Instance.transform.Find("Sound");

        AddSoundGroup(SoundGroupName.D2.ToString(), true, false, 1f);
    }

    public void Update()
    {

    }

    public void OnDestroy()
    {
        Seriald = 0;
        LoadAssetCallbacks = null;
        SoundsToReleaseOnLoad.Clear();
        SoundsBeingLoaded.Clear();
        SoundGroups.Clear();
        StopAllLoadedSounds();
    }

    /// <summary>
    /// 增加声音组。
    /// </summary>
    /// <param name="soundGroupName">声音组名称。</param>
    /// <param name="avoidBeingReplacedBySamePriority">声音组中的声音是否避免被同优先级声音替换。</param>
    /// <param name="soundGroupMute">声音组是否静音。</param>
    /// <param name="soundGroupVolume">声音组音量。</param>
    /// <returns>是否增加声音组成功。</returns>
    public bool AddSoundGroup(string soundGroupName, bool avoidBeingReplacedBySamePriority, bool soundGroupMute, float soundGroupVolume)
    {
        if (string.IsNullOrEmpty(soundGroupName))
        {
            Log.Error("Sound group name is invalid.");
        }
        if (SoundGroups.ContainsKey(soundGroupName))
        {
            return false;
        }
        GameObject game = new GameObject(soundGroupName);
        game.transform.SetParent(SoundParent, false);
        SoundGroup soundGroup = new SoundGroup(soundGroupName, game.transform)
        {
            AvoidBeingReplacedBySamePriority = avoidBeingReplacedBySamePriority,
            Mute = soundGroupMute,
            Volume = soundGroupVolume
        };
        SoundGroups.Add(soundGroupName, soundGroup);
        return true;
    }

    /// <summary>
    /// 是否正在加载声音。
    /// </summary>
    /// <param name="serialId">声音序列编号。</param>
    /// <returns>是否正在加载声音。</returns>
    public bool IsLoadingSound(int serialId)
    {
        return SoundsBeingLoaded.Contains(serialId);
    }

    /// <summary>
    /// 播放声音。
    /// </summary>
    /// <param name="soundAssetName">声音资源名称。</param>
    /// <param name="soundGroupName">声音组名称。</param>
    /// <param name="priority">加载声音资源的优先级。</param>
    /// <param name="playSoundParams">播放声音参数。</param>
    /// <param name="userData">用户自定义数据。</param>
    /// <returns>声音的序列编号。</returns>
    public int PlaySound(string soundAssetName, string soundGroupName, SoundPlayData playSoundParams = null)
    {
        if (playSoundParams == null)
        {
            playSoundParams = SoundPlayData.Create();
        }

        int serialId = ++Seriald;
        PlaySoundErrorCode? errorCode = null;
        string errorMessage = null;
        SoundGroup soundGroup = SoundGroups[soundGroupName];
        if (soundGroup == null)
        {
            errorCode = PlaySoundErrorCode.SoundGroupNotExist;
            errorMessage = UtilText.Format("Sound group '{0}' is not exist.", soundGroupName);
        }
        else if (soundGroup.SoundAgentCount <= 0)
        {
            errorCode = PlaySoundErrorCode.SoundGroupHasNoAgent;
            errorMessage = UtilText.Format("Sound group '{0}' is have no sound agent.", soundGroupName);
        }

        if (errorCode.HasValue)
        {
            if (playSoundParams.Referenced)
            {
                ReferencePool.Release(playSoundParams);
            }
            Log.Error(errorMessage);
            return serialId;
        }

        SoundsBeingLoaded.Add(serialId);
        GameModule.Res.LoadAsset(soundAssetName, typeof(AudioClip), LoadAssetCallbacks, SoundPlayInfo.Create(serialId, soundGroup, playSoundParams));
        return serialId;
    }

    /// <summary>
    /// 停止播放声音。
    /// </summary>
    /// <param name="serialId">要停止播放声音的序列编号。</param>
    /// <returns>是否停止播放声音成功。</returns>
    public bool StopSound(int serialId)
    {
        return StopSound(serialId, Constant.DefaultFadeOutSeconds);
    }

    /// <summary>
    /// 停止播放声音。
    /// </summary>
    /// <param name="serialId">要停止播放声音的序列编号。</param>
    /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
    /// <returns>是否停止播放声音成功。</returns>
    public bool StopSound(int serialId, float fadeOutSeconds)
    {
        //if (IsLoadingSound(serialId))
        //{
        //    SoundsToReleaseOnLoad.Add(serialId);
        //    SoundsBeingLoaded.Remove(serialId);
        //    return true;
        //}

        foreach (KeyValuePair<string, SoundGroup> soundGroup in SoundGroups)
        {
            if (soundGroup.Value.StopSound(serialId, fadeOutSeconds))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 停止所有已加载的声音。
    /// </summary>
    public void StopAllLoadedSounds()
    {
        StopAllLoadedSounds(Constant.DefaultFadeOutSeconds);
    }

    /// <summary>
    /// 停止所有已加载的声音。
    /// </summary>
    /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
    public void StopAllLoadedSounds(float fadeOutSeconds)
    {
        foreach (KeyValuePair<string, SoundGroup> soundGroup in SoundGroups)
        {
            soundGroup.Value.StopAllLoadedSounds(fadeOutSeconds);
        }
    }

    /// <summary>
    /// 暂停播放声音。
    /// </summary>
    /// <param name="serialId">要暂停播放声音的序列编号。</param>
    public void PauseSound(int serialId)
    {
        PauseSound(serialId, Constant.DefaultFadeOutSeconds);
    }

    /// <summary>
    /// 暂停播放声音。
    /// </summary>
    /// <param name="serialId">要暂停播放声音的序列编号。</param>
    /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
    public void PauseSound(int serialId, float fadeOutSeconds)
    {
        foreach (KeyValuePair<string, SoundGroup> soundGroup in SoundGroups)
        {
            if (soundGroup.Value.PauseSound(serialId, fadeOutSeconds))
            {
                return;
            }
        }

        Log.Error(UtilText.Format("Can not find sound '{0}'.", serialId));
    }

    /// <summary>
    /// 恢复播放声音。
    /// </summary>
    /// <param name="serialId">要恢复播放声音的序列编号。</param>
    public void ResumeSound(int serialId)
    {
        ResumeSound(serialId, Constant.DefaultFadeInSeconds);
    }

    /// <summary>
    /// 恢复播放声音。
    /// </summary>
    /// <param name="serialId">要恢复播放声音的序列编号。</param>
    /// <param name="fadeInSeconds">声音淡入时间，以秒为单位。</param>
    public void ResumeSound(int serialId, float fadeInSeconds)
    {
        foreach (KeyValuePair<string, SoundGroup> soundGroup in SoundGroups)
        {
            if (soundGroup.Value.ResumeSound(serialId, fadeInSeconds))
            {
                return;
            }
        }

        Log.Error(UtilText.Format("Can not find sound '{0}'.", serialId.ToString()));
    }

    private void LoadAssetSuccessCallback(string soundAssetName, object soundAsset, float duration, object userData)
    {
        SoundPlayInfo playSoundInfo = (SoundPlayInfo)userData;
        if (SoundsToReleaseOnLoad.Contains(playSoundInfo.SerialId))
        {
            SoundsToReleaseOnLoad.Remove(playSoundInfo.SerialId);
            if (playSoundInfo.PlaySoundParams.Referenced)
            {
                ReferencePool.Release(playSoundInfo.PlaySoundParams);
            }
            ReferencePool.Release(playSoundInfo);
            //GameModule.Res.UnloadScene.ReleaseSoundAsset(soundAsset); Todo
            return;
        }

        SoundsBeingLoaded.Remove(playSoundInfo.SerialId);
        PlaySoundErrorCode? errorCode = null;
        SoundAgent soundAgent = playSoundInfo.SoundGroup.PlaySound(playSoundInfo.SerialId, soundAsset, playSoundInfo.PlaySoundParams, out errorCode);
        if (soundAgent != null)
        {
            if (playSoundInfo.PlaySoundParams.Referenced)
            {
                ReferencePool.Release(playSoundInfo.PlaySoundParams);
            }
            ReferencePool.Release(playSoundInfo);
            return;
        }

        SoundsToReleaseOnLoad.Remove(playSoundInfo.SerialId);
        //GameModule.Res.UnloadScene.ReleaseSoundAsset(soundAsset); Todo
        string errorMessage = UtilText.Format("Sound group '{0}' play sound '{1}' failure.", playSoundInfo.SoundGroup.Name, soundAssetName);

        if (playSoundInfo.PlaySoundParams.Referenced)
        {
            ReferencePool.Release(playSoundInfo.PlaySoundParams);
        }
        ReferencePool.Release(playSoundInfo);
        Log.Error(errorMessage);
    }

    private void LoadAssetFailureCallback(string soundAssetName, LoadResStatus status, string errorMessage)
    {
        string appendErrorMessage = UtilText.Format("一定要处理 Load sound failure, asset name '{0}', status '{1}', error message '{2}'.", soundAssetName, status.ToString(), errorMessage);
        Log.Error(appendErrorMessage);
    }
}