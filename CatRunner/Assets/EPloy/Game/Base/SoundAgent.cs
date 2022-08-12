using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace EPloy.Sound
{
    /// <summary>
    /// 默认声音代理辅助器 
    /// </summary>
    public class SoundAgent : MonoBehaviour
    {
        private AudioSource AudioSource = null;
        private Transform BindingGo = null;
        private float VolumeWhenPause = 0f;
        /// <summary>
        /// 当前播放的声音唯一编号
        /// </summary>
        public int SerialId { get; set; }
        /// <summary>
        /// 声音创建时间
        /// </summary>
        public DateTime SetSoundAssetTime { get; set; }
        /// <summary>
        /// 获取当前是否正在播放。
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                return AudioSource.isPlaying;
            }
        }
        /// <summary>
        /// 获取声音长度。
        /// </summary>
        public float Length
        {
            get
            {
                return AudioSource.clip != null ? AudioSource.clip.length : 0f;
            }
        }
        /// <summary>
        /// 获取或设置播放位置。
        /// </summary>
        public float Time
        {
            get
            {
                return AudioSource.time;
            }
            set
            {
                AudioSource.time = value;
            }
        }
        /// <summary>
        /// 获取或设置是否静音。
        /// </summary>
        public bool Mute
        {
            get
            {
                return AudioSource.mute;
            }
            set
            {
                AudioSource.mute = value;
            }
        }
        /// <summary>
        /// 获取或设置是否循环播放。
        /// </summary>
        public bool Loop
        {
            get
            {
                return AudioSource.loop;
            }
            set
            {
                AudioSource.loop = value;
            }
        }
        /// <summary>
        /// 获取或设置声音优先级。
        /// </summary>
        public int Priority
        {
            get
            {
                return 128 - AudioSource.priority;
            }
            set
            {
                AudioSource.priority = 128 - value;
            }
        }
        /// <summary>
        /// 获取或设置音量大小。
        /// </summary>
        public float Volume
        {
            get
            {
                return AudioSource.volume;
            }
            set
            {
                AudioSource.volume = value;
            }
        }
        /// <summary>
        /// 获取或设置声音音调。
        /// </summary>
        public float Pitch
        {
            get
            {
                return AudioSource.pitch;
            }
            set
            {
                AudioSource.pitch = value;
            }
        }
        /// <summary>
        /// 获取或设置声音立体声声相。
        /// </summary>
        public float PanStereo
        {
            get
            {
                return AudioSource.panStereo;
            }
            set
            {
                AudioSource.panStereo = value;
            }
        }
        /// <summary>
        /// 获取或设置声音空间混合量。
        /// </summary>
        public float SpatialBlend
        {
            get
            {
                return AudioSource.spatialBlend;
            }
            set
            {
                AudioSource.spatialBlend = value;
            }
        }
        /// <summary>
        /// 获取或设置声音最大距离。
        /// </summary>
        public float MaxDistance
        {
            get
            {
                return AudioSource.maxDistance;
            }

            set
            {
                AudioSource.maxDistance = value;
            }
        }
        /// <summary>
        /// 获取或设置声音多普勒等级。
        /// </summary>
        public float DopplerLevel
        {
            get
            {
                return AudioSource.dopplerLevel;
            }
            set
            {
                AudioSource.dopplerLevel = value;
            }
        }
        /// <summary>
        /// 获取或设置声音代理辅助器所在的混音组。
        /// </summary>
        public AudioMixerGroup AudioMixerGroup
        {
            get
            {
                return AudioSource.outputAudioMixerGroup;
            }
            set
            {
                AudioSource.outputAudioMixerGroup = value;
            }
        }

        public static SoundAgent CreateSoundAgentHelper(string name, Transform parent)
        {
            GameObject game = new GameObject(name);
            SoundAgent soundAgent = game.AddComponent<SoundAgent>();
            soundAgent.transform.SetParent(parent, false);
            soundAgent.AudioSource = game.AddComponent<AudioSource>();
            soundAgent.AudioSource.playOnAwake = false;
            soundAgent.AudioSource.rolloffMode = AudioRolloffMode.Custom;
            return soundAgent;
        }

        public void Update()
        {
            if (BindingGo != null)
            {
                UpdateAgentPosition();
            }
        }

        /// <summary>
        /// 播放声音。
        /// </summary>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位。</param>
        public void Play(float fadeInSeconds)
        {
            StopAllCoroutines();
            AudioSource.Play();
            if (fadeInSeconds > 0f)
            {
                float volume = AudioSource.volume;
                AudioSource.volume = 0f;
                StartCoroutine(FadeToVolume(AudioSource, volume, fadeInSeconds));
            }
        }

        /// <summary>
        /// 停止播放声音。
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
        public void Stop(float fadeOutSeconds)
        {
            StopAllCoroutines();

            if (fadeOutSeconds > 0f && gameObject.activeInHierarchy)
            {
                StartCoroutine(StopCo(fadeOutSeconds));
            }
            else
            {
                AudioSource.Stop();
            }
        }

        /// <summary>
        /// 暂停播放声音。
        /// </summary>
        /// <param name="fadeOutSeconds">声音淡出时间，以秒为单位。</param>
        public void Pause(float fadeOutSeconds)
        {
            StopAllCoroutines();

            VolumeWhenPause = AudioSource.volume;
            if (fadeOutSeconds > 0f && gameObject.activeInHierarchy)
            {
                StartCoroutine(PauseCo(fadeOutSeconds));
            }
            else
            {
                AudioSource.Pause();
            }
        }

        /// <summary>
        /// 恢复播放声音。
        /// </summary>
        /// <param name="fadeInSeconds">声音淡入时间，以秒为单位。</param>
        public void Resume(float fadeInSeconds)
        {
            StopAllCoroutines();

            AudioSource.UnPause();
            if (fadeInSeconds > 0f)
            {
                StartCoroutine(FadeToVolume(AudioSource, VolumeWhenPause, fadeInSeconds));
            }
            else
            {
                AudioSource.volume = VolumeWhenPause;
            }
        }

        /// <summary>
        /// 重置声音代理辅助器。
        /// </summary>
        public void Reset()
        {
            transform.localPosition = Vector3.zero;
            AudioSource.clip = null;
            BindingGo = null;
            VolumeWhenPause = 0f;
        }

        /// <summary>
        /// 设置声音资源。
        /// </summary>
        /// <param name="soundAsset">声音资源。</param>
        /// <returns>是否设置声音资源成功。</returns>
        public bool SetSoundAsset(object soundAsset)
        {
            AudioClip audioClip = soundAsset as AudioClip;
            if (audioClip == null)
            {
                return false;
            }

            AudioSource.clip = audioClip;
            return true;
        }

        /// <summary>
        /// 设置声音绑定的实体。
        /// </summary>
        /// <param name="bindingEntity">声音绑定的物体。</param>
        public void SetBindingEntity(Transform bindingGo)
        {
            BindingGo = bindingGo;
            if (BindingGo != null)
            {
                UpdateAgentPosition();
                return;
            }
        }

        /// <summary>
        /// 设置声音所在的世界坐标。
        /// </summary>
        /// <param name="worldPosition">声音所在的世界坐标。</param>
        public void SetWorldPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        private void UpdateAgentPosition()
        {
            if (BindingGo.gameObject.activeInHierarchy)
            {
                transform.position = BindingGo.position;
                return;
            }
        }

        private IEnumerator StopCo(float fadeOutSeconds)
        {
            yield return FadeToVolume(AudioSource, 0f, fadeOutSeconds);
            AudioSource.Stop();
        }

        private IEnumerator PauseCo(float fadeOutSeconds)
        {
            yield return FadeToVolume(AudioSource, 0f, fadeOutSeconds);
            AudioSource.Pause();
        }

        private IEnumerator FadeToVolume(AudioSource audioSource, float volume, float duration)
        {
            float time = 0f;
            float originalVolume = audioSource.volume;
            while (time < duration)
            {
                time += UnityEngine.Time.deltaTime;
                audioSource.volume = Mathf.Lerp(originalVolume, volume, time / duration);
                yield return new WaitForEndOfFrame();
            }

            audioSource.volume = volume;
        }
    }
}