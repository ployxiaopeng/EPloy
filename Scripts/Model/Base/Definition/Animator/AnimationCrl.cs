using ETModel;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Anime
{
    public sealed class AnimationCrl : MonoBehaviour
    {
        #region 动画部分
        [SerializeField]
        [Header("动画组")]
        private Animations[] Animation;
        [SerializeField]
        [Header("默认初始动画")]
        private string DefaultAnimeName;
        private Image image;
        private void Awake()
        {
            image = transform.GetComponent<Image>();
            if (image == null) Debug.LogError("必需包含 Image");
        }
        private void OnEnable()
        {
            if (image == null) return;
            if (DefaultAnimeName != "")
                Play(DefaultAnimeName);
            StartCoroutine(HandlerAnime());
        }
        #endregion

        private Animations PlayAnimeData;
        private bool IsLoop;
        private int StartIndex;
        public void Play(string name, bool isLoop = true)
        {
            foreach (var anim in Animation)
            {
                if (name != anim.AnimeName) continue;
                PlayAnimeData = anim;
                IsLoop = isLoop;
                StartIndex = 0;
                return;
            }
            PlayAnimeData = null;
            IsLoop = false;
            Log.Error("没有发现动画{0}", name);
        }

        IEnumerator HandlerAnime()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();
                if (PlayAnimeData == null) continue;
                while (true)
                {
                    if (StartIndex < PlayAnimeData.Sprite.Length)
                    {
                        image.sprite = PlayAnimeData.Sprite[StartIndex];
                        StartIndex++;
                        if (IsLoop && StartIndex >= PlayAnimeData.Sprite.Length)
                            StartIndex = 0;
                    }
                    else
                    {
                        PlayAnimeData = null;
                        IsLoop = false;
                        break;
                    }
                    yield return new WaitForSeconds(PlayAnimeData.Speed);
                }
            }
        }
    }
}
