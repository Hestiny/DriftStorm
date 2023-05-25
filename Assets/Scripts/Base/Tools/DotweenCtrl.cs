using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public class DotweenCtrl
    {
        public static Tween Delay(float time, Action call, GameObject obj = null)
        {
            int i = 0;
            var timer = DOTween.To(() => i, x => i = x, 1, time);
            timer.OnComplete(() => { call?.Invoke(); });
            if (obj != null)
                timer.SetLink(obj);
            return timer;
        }

        public static Tween Timer(int count, float intervalTime, Action setpCall, Action endCall = null, GameObject obj = null)
        {
            int i = 0;
            var timer = DOTween.To(() => i, x => i = x, 1, intervalTime).SetLoops(count);
            timer.OnStepComplete(() => { setpCall?.Invoke(); });
            timer.OnComplete(() => { endCall?.Invoke(); });
            if (obj != null)
                timer.SetLink(obj);
            return timer;
        }

    }
}

