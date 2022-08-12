using System.Collections.Generic;
using UnityEngine;

namespace EPloy.Game.Sound
{
    /// <summary>
    /// 声音管理器。
    /// </summary>
    public sealed partial class SoundMudule : IGameModule
    {
        /// <summary>
        /// 声音组。
        /// </summary>
        private sealed class SoundGroup
        {
            /// <summary>
            /// 获取或设置声音组中的声音是否避免被同优先级声音替换。
            /// </summary>
            public bool AvoidBeingReplacedBySamePriority { get; set; }
            /// <summary>
            /// 获取声音组名称。
            /// </summary>
            public string Name { get; private set; }
            public Transform soundParent { get; private set; }
            private List<SoundAgent> soundAgents;
            private bool mute;
            private float volume;

            /// <summary>
            /// 初始化声音组的新实例。
            /// </summary>
            /// <param name="name">声音组名称。</param>
            /// <param name="soundGroupHelper">声音组辅助器。</param>
            public SoundGroup(string name, Transform parent, int agentCount = 3)
            {
                if (string.IsNullOrEmpty(name))
                {
                    Log.Error("Sound group name is invalid.");
                }
                Name = name;
                soundParent = parent;
                soundAgents = new List<SoundAgent>();
                for (int i = 0; i < agentCount; i++)
                {
                    AddSoundAgentHelper(UtilText.Format("{0}_{1}", name, i));
                }
            }

            /// <summary>
            /// 获取声音代理数。
            /// </summary>
            public int SoundAgentCount
            {
                get
                {
                    return soundAgents.Count;
                }
            }
            /// <summary>
            /// 获取或设置声音组静音。
            /// </summary>
            public bool Mute
            {
                get
                {
                    return mute;
                }
                set
                {
                    mute = value;
                    foreach (SoundAgent soundAgent in soundAgents)
                    {
                        soundAgent.Mute = mute;
                    }
                }
            }

            /// <summary>
            /// 获取或设置声音组音量。
            /// </summary>
            public float Volume
            {
                get
                {
                    return volume;
                }
                set
                {
                    volume = value;
                    foreach (SoundAgent soundAgent in soundAgents)
                    {
                        soundAgent.Volume = volume;
                    }
                }
            }

            /// <summary>
            /// 增加声音代理辅助器。
            /// </summary>
            public void AddSoundAgentHelper(string name)
            {
                soundAgents.Add(SoundAgent.CreateSoundAgentHelper(name, soundParent));
            }

            /// <summary>
            /// 播放声音。
            /// </summary>
            /// <param name="serialId">声音的序列编号。</param>
            /// <param name="soundAsset">声音资源。</param>
            /// <param name="playSoundParams">播放声音参数。</param>
            /// <param name="errorCode">错误码。</param>
            /// <returns>用于播放的声音代理。</returns>
            public SoundAgent PlaySound(int serialId, object soundAsset, SoundPlayData playSoundParams, out PlaySoundErrorCode? errorCode)
            {
                errorCode = null;
                SoundAgent candidateAgent = null;
                foreach (SoundAgent soundAgent in soundAgents)
                {
                    if (!soundAgent.IsPlaying)
                    {
                        candidateAgent = soundAgent;
                        break;
                    }

                    if (soundAgent.Priority < playSoundParams.Priority)
                    {
                        if (candidateAgent == null || soundAgent.Priority < candidateAgent.Priority)
                        {
                            candidateAgent = soundAgent;
                        }
                    }
                    else if (!AvoidBeingReplacedBySamePriority && soundAgent.Priority == playSoundParams.Priority)
                    {
                        if (candidateAgent == null || soundAgent.SetSoundAssetTime < candidateAgent.SetSoundAssetTime)
                        {
                            candidateAgent = soundAgent;
                        }
                    }
                }

                if (candidateAgent == null)
                {
                    errorCode = PlaySoundErrorCode.IgnoredDueToLowPriority;
                    return null;
                }

                if (!candidateAgent.SetSoundAsset(soundAsset))
                {
                    errorCode = PlaySoundErrorCode.SetSoundAssetFailure;
                    return null;
                }

                candidateAgent.SerialId = serialId;
                candidateAgent.Time = playSoundParams.Time;
                candidateAgent.Mute = playSoundParams.MuteInSoundGroup;
                candidateAgent.Loop = playSoundParams.Loop;
                candidateAgent.Priority = playSoundParams.Priority;
                candidateAgent.Volume = playSoundParams.VolumeInSoundGroup;
                candidateAgent.Pitch = playSoundParams.Pitch;
                candidateAgent.PanStereo = playSoundParams.PanStereo;
                candidateAgent.SpatialBlend = playSoundParams.SpatialBlend;
                candidateAgent.MaxDistance = playSoundParams.MaxDistance;
                candidateAgent.DopplerLevel = playSoundParams.DopplerLevel;
                candidateAgent.Play(playSoundParams.FadeInSeconds);
                return candidateAgent;
            }

            /// <summary>
            /// 停止播放声音。
            /// </summary>
            /// <param name="serialId">要停止播放声音的序列编号。</param>
            /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
            /// <returns>是否停止播放声音成功。</returns>
            public bool StopSound(int serialId, float fadeOutSeconds)
            {
                foreach (SoundAgent soundAgent in soundAgents)
                {
                    if (soundAgent.SerialId != serialId)
                    {
                        continue;
                    }

                    soundAgent.Stop(fadeOutSeconds);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 暂停播放声音。
            /// </summary>
            /// <param name="serialId">要暂停播放声音的序列编号。</param>
            /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
            /// <returns>是否暂停播放声音成功。</returns>
            public bool PauseSound(int serialId, float fadeOutSeconds)
            {
                foreach (SoundAgent soundAgent in soundAgents)
                {
                    if (soundAgent.SerialId != serialId)
                    {
                        continue;
                    }

                    soundAgent.Pause(fadeOutSeconds);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 恢复播放声音。
            /// </summary>
            /// <param name="serialId">要恢复播放声音的序列编号。</param>
            /// <param name="fadeInSeconds">声音淡入时间，以秒为单位。</param>
            /// <returns>是否恢复播放声音成功。</returns>
            public bool ResumeSound(int serialId, float fadeInSeconds)
            {
                foreach (SoundAgent soundAgent in soundAgents)
                {
                    if (soundAgent.SerialId != serialId)
                    {
                        continue;
                    }

                    soundAgent.Resume(fadeInSeconds);
                    return true;
                }

                return false;
            }

            /// <summary>
            /// 停止所有已加载的声音。
            /// </summary>
            public void StopAllLoadedSounds()
            {
                foreach (SoundAgent soundAgent in soundAgents)
                {
                    if (soundAgent.IsPlaying)
                    {
                        soundAgent.Stop(Constant.DefaultFadeOutSeconds);
                    }
                }
            }

            /// <summary>
            /// 停止所有已加载的声音。
            /// </summary>
            /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
            public void StopAllLoadedSounds(float fadeOutSeconds)
            {
                foreach (SoundAgent soundAgent in soundAgents)
                {
                    if (soundAgent.IsPlaying)
                    {
                        soundAgent.Stop(fadeOutSeconds);
                    }
                }
            }
        }
    }
}
