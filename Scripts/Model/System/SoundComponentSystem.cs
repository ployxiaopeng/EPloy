using GameFramework;
using GameFramework.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ETModel
{
    [ObjectSystem]
    public class SoundComponentAwakeSystem : AwakeSystem<SoundComponent>
    {
        public override void Awake(SoundComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class SoundComponentDestroySystem : DestroySystem<SoundComponent>
    {
        public override void Destroy(SoundComponent self)
        {
            self.OnDestroy();
        }
    }
    public static class SoundComponentSystem 
    {
        public static void Awake(this SoundComponent self )
        {
            self.SoundManager = GameFrameworkEntry.GetModule<ISoundManager>();
            if (self.SoundManager == null)
            {
                Log.Fatal("Sound manager is invalid.");
                return;
            }
            self.AudioListener = Init.Instance.transform.Find("Sound").gameObject.GetOrAddComponent<AudioListener>();
            SceneManager.sceneLoaded += self.OnSceneLoaded;
            SceneManager.sceneUnloaded += self.OnSceneUnloaded;
            self.defaultSoundGroup= new SoundGroup
            {
                Name = "Default"
            };
        }

        public static void OnDestroy(this SoundComponent self)
        {
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded -= self.OnSceneLoaded;
            SceneManager.sceneUnloaded -= self.OnSceneUnloaded;
#endif
        }
    }
}
