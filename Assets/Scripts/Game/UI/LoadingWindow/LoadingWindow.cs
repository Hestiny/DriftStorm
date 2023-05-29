using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace UIFramework
{
    public class LoadingWindow : UIWIndow
    {
        public struct WindowParam
        {

        }

        public override void OnAwake()
        {

        }

        protected override void InitWindow<T>(T? obj)
        {

        }

        [SerializeField]
        private CanvasGroup _canvasGroup;

        public override void Show()
        {
            DotweenCtrl.Delay(1f, () => { _canvasGroup.DOFade(0, 0.3f).OnComplete(() => { ToCloseWindow(); }); });
        }

    }
}

