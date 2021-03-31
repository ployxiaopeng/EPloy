using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace Anime
{
    public sealed class TransAnimeCrl : MonoBehaviour
    {
        #region 动画部分
        [SerializeField]
        private bool isLoop;
        [SerializeField]
        private MyAnime[] AnimationTeam;
        public int transAnimeCount
        {
            get
            {
                return AnimationTeam.Length;
            }
        }
        private int TransAnimeIndex = 0;
        private Dictionary<Transform, Trans> transDic = new Dictionary<Transform, Trans>();
        #endregion
        private void Awake()
        {
            foreach (var data in AnimationTeam)
            {
                foreach (var trans in data.TransAnime)
                {
                    if (transDic.ContainsKey(trans.Transform)) continue;
                    transDic.Add(trans.Transform, new Trans(trans.Transform));
                }
            }
        }
        private void OnEnable()
        {
            foreach (var trans in transDic)
                trans.Value.OnRest();

            TransAnimeIndex = 0;
            StartCoroutine(HandlerAnime());
        }

        IEnumerator HandlerAnime()
        {
            yield return new WaitForEndOfFrame();
            while (TransAnimeIndex < transAnimeCount)
            {
                MyAnime animeData = AnimationTeam[TransAnimeIndex];
                Image image = null; SpriteRenderer spriteRenderer = null;
                foreach (var anime in animeData.TransAnime)
                {
                    switch (anime.AnimeType)
                    {
                        case AnimeType.Position:
                            anime.Transform.DOLocalMove(anime.EndArg, anime.Time).SetEase(anime.Ease);
                            break;
                        case AnimeType.Scale:
                            anime.Transform.DOScale(anime.EndArg, anime.Time).SetEase((anime.Ease));
                            break;
                        case AnimeType.Rotation:
                            anime.Transform.DOLocalRotate(anime.EndArg, anime.Time).SetEase((anime.Ease));
                            break;
                        case AnimeType.Active:
                            anime.Transform.gameObject.SetActive(anime.EndArg.x == 1);
                            break;
                        case AnimeType.Color:
                            image = anime.Transform.GetComponent<Image>();
                            if (image != null)
                            {
                                image.DOColor(new Color(anime.EndArg.x / 255f, anime.EndArg.y / 255f, anime.EndArg.z / 255f),
                              anime.Time).SetEase((anime.Ease));
                                continue;
                            }
                            spriteRenderer = anime.Transform.GetComponent<SpriteRenderer>();
                            if (spriteRenderer != null)
                            {
                                spriteRenderer.DOColor(new Color(anime.EndArg.x / 255f, anime.EndArg.y / 255f, anime.EndArg.z / 255f),
                                     anime.Time).SetEase((anime.Ease));
                                continue;
                            }
                            Debug.LogError("颜色改变必需包含 Image或SpriteRenderer");
                            break;
                        case AnimeType.lucency:
                            image = anime.Transform.GetComponent<Image>();
                            if (image != null)
                            {
                                image.DOFade(anime.EndArg.x, anime.Time).SetEase((anime.Ease));
                                continue;
                            }
                            spriteRenderer = anime.Transform.GetComponent<SpriteRenderer>();
                            if (spriteRenderer != null)
                            {
                                spriteRenderer.DOFade(anime.EndArg.x, anime.Time).SetEase((anime.Ease));
                                continue;
                            }
                            Debug.LogError("颜色改变必需包含 Image或SpriteRenderer");
                            break;
                    }
                }
                TransAnimeIndex++;
                yield return new WaitForSeconds(animeData.Time);
                if (isLoop && TransAnimeIndex == transAnimeCount) TransAnimeIndex = 0;
            }
            yield break;
        }
    }
}