using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ETHotfix
{
    public class BattleRole : UIExtenLogic
    {
        private MapBattleWnd MapBattleWnd;
        public BattleRole(Transform transform, MapBattleWnd mapBattleWnd) : base(transform)
        {
            MapBattleWnd = mapBattleWnd;
        }

        private CanvasGroup imgCanvasGroup;
        private CanvasGroup roleCanvasGroup;
        public bool isCanAction { get; set; }
        private bool isAtt;
        protected override void Find()
        {
            imgCanvasGroup = transform.Find("img").GetComponent<CanvasGroup>();
            roleCanvasGroup = transform.GetComponent<CanvasGroup>();
            isCanAction = false; isAtt = false;
        }

        public void CanAction()
        {
            imgCanvasGroup.alpha = 0;
            if (!isCanAction) return;
            imgCanvasGroup.DOFade(1, 0.3f).OnComplete(() =>
             {
                 imgCanvasGroup.DOFade(0, 0.3f).OnComplete(CanAction);
             });
        }

        public void Att()
        {
            if (isAtt) return;
            isCanAction = isAtt;
            isAtt = true;
            //飞到敌人身边
            transform.DOMove(MapBattleWnd.Monster.position, 1).OnComplete(() =>
            {
                //闪一下
                roleCanvasGroup.DOFade(0, 0.2f).OnComplete(() =>
                    {
                        roleCanvasGroup.DOFade(1, 0.3f).OnComplete(() =>
                            {
                                //再飞回来
                                transform.DOLocalMove(new Vector3(-829, 276, 0), 1).OnComplete(() =>
                                {
                                    isAtt = false;
                                }).SetEase(Ease.OutSine);
                            });
                    });
            }).SetEase(Ease.OutSine);
        }
    }
}