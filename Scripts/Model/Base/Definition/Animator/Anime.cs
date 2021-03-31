using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Anime
{
    [Serializable]
    internal enum AnimeType
    {
        Position,
        Scale,
        Rotation,
        Color,
        lucency,
        Active,
    }
    [Serializable]
    internal sealed class GeneralAnime : System.Object
    {
        [SerializeField]
        private string notes;
        [SerializeField]
        private AnimeType animeType;
        [SerializeField]
        private Transform transform;
        [SerializeField]
        private Vector3 endArg;
        [SerializeField]
        private Ease ease;
        [SerializeField]
        private float time;

        public Transform Transform
        {
            get
            {
                return transform;
            }
        }
        public float Time
        {
            get
            {
                return time;
            }
        }
        public Vector3 EndArg
        {
            get
            {
                return endArg;
            }
        }
        public AnimeType AnimeType
        {
            get
            {
                return animeType;
            }
        }
        public Ease Ease
        {
            get
            {
                return ease;
            }
        }
        public string Notes
        {
            get
            {
                return notes;
            }
        }
    }
    [Serializable]
    internal sealed class MyAnime : System.Object
    {
        [SerializeField]
        private string notes;
        [SerializeField]
        [Header("动画组持续时间")]
        private float time;

        [SerializeField]
        private GeneralAnime[] transAnime;
        public GeneralAnime[] TransAnime
        {
            get
            {
                return transAnime;
            }
        }
        public float Time
        {
            get
            {
                return time;
            }
        }
        public string Notes
        {
            get
            {
                return notes;
            }
        }
    }
    [Serializable]
    internal sealed class Trans : System.Object
    {
        private Vector3 initPosition;
        private Vector3 initScale;
        private Vector3 initRotation;
        private Transform Transform;
        private Color initColor;
        public Trans(Transform transform)
        {
            Transform = transform;
            initPosition = transform.localPosition; initScale = transform.localScale;
            initRotation = transform.localEulerAngles;
            Image image = transform.GetComponent<Image>();
            if (image != null)
            {
                initColor = image.color;
                return;
            }
            SpriteRenderer spriteRenderer = transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) initColor = spriteRenderer.color;
        }
        public void OnRest()
        {
            Transform.localEulerAngles = initRotation;
            Transform.localPosition = initPosition;
            Transform.localScale = initScale;
            Image image = Transform.GetComponent<Image>();
            if (image != null)
            {
                image.color = initColor;
                return;
            }
            SpriteRenderer spriteRenderer = Transform.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.color = initColor;
        }
    }
    [Serializable]
    internal sealed class Animations : System.Object
    {
        [SerializeField]
        [Header("动画名字")]
        private string animeName;
        [SerializeField]
        [Header("帧图片")]
        private Sprite[] sprite;
        [SerializeField]
        [Header("动画帧率")]
        private int fps = 30;

        public string AnimeName
        {
            get
            {
                return animeName;
            }
        }
        public float Speed
        {
            get
            {
                return 1f / fps;
            }
        }
        public Sprite[] Sprite
        {
            get
            {
                return sprite;
            }
        }
    }
}