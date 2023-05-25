using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public class TimerCtrl : MonoSingleton<TimerCtrl>
    {

        #region ====事件字典 只涉及单纯的事件存储 不涉及任何业务逻辑====
        public Dictionary<TimerUpdateType, List<TimerEvent>> m_timers = new Dictionary<TimerUpdateType, List<TimerEvent>>();

        public List<TimerEvent> GetTimersByUpdateType(TimerUpdateType updateType)
        {
            if (m_timers.TryGetValue(updateType, out var timers))
            {
                return timers;
            }
            return new List<TimerEvent>();
        }

        public TimerEvent GetTimerByKey(string key)
        {
            var ator = m_timers.GetEnumerator();
            while (ator.MoveNext())
            {
                foreach (var item in ator.Current.Value)
                {
                    if (item.m_key == key)
                        return item;
                }
            }
            return null;
        }

        void SaveTimerToPool(TimerEvent timer)
        {
            if (timer == null)
                return;

            if (m_timers.TryGetValue(timer.m_updateType, out var timers))
            {
                timers.Add(timer);
            }
            else
            {
                m_timers.Add(timer.m_updateType, new List<TimerEvent> { timer });
            }
        }

        void RemoveTimerFromPool(TimerEvent timer)
        {
            if (timer == null)
                return;

            if (m_timers.TryGetValue(timer.m_updateType, out var timers))
            {
                if (timers.Contains(timer))
                    timers.Remove(timer);
            }
        }

        public void RemoveTimerByKey(string key)
        {
            RemoveTimerFromPool(GetTimerByKey(key));
        }

        public void RemoveAllTimers()
        {
            m_timers.Clear();
        }
        #endregion

        #region ====计时器添加移除相关操作====
        /// <summary>
        /// 添加计时器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="interval">间隔</param>
        /// <param name="repeatCount">循环次数</param>
        /// <param name="callBack">CompleteEvent</param>
        /// <param name="isRunImme">是否立刻执行一次</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="updateType">更新类型</param>
        /// <returns></returns>
        public TimerEvent AddTimer(string key, float interval, int repeatCount, TimerCallBack callBack, bool isRunImme = true, bool isIgnoreTimeScale = false, TimerUpdateType updateType = TimerUpdateType.Update)
        {
            TimerEvent timer = new TimerEvent(key, interval, repeatCount, callBack, isRunImme, isIgnoreTimeScale, updateType);

            SaveTimerToPool(timer);
            return timer;
        }

        /// <summary>
        /// 自动生成Timer的key
        /// 需要在本地存储事件或者记录事件的key
        /// 移除事件优先用事件本身
        /// </summary>
        /// <param name="interval">间隔</param>
        /// <param name="repeatCount">循环次数</param>
        /// <param name="callBack">CompleteEvent</param>
        /// <param name="isRunImme">是否立刻执行一次</param>
        /// <param name="isIgnoreTimeScale">是否忽略时间缩放</param>
        /// <param name="updateType">更新类型</param>
        /// <returns></returns>
        //TODO: GetHashCode()方法的原理
        public TimerEvent AddTimer(float interval, int repeatCount, TimerCallBack callBack, bool isRunImme = true, bool isIgnoreTimeScale = false, TimerUpdateType updateType = TimerUpdateType.Update)
        {
            TimerEvent timer = new TimerEvent(null, interval, repeatCount, callBack, isRunImme, isIgnoreTimeScale, updateType);

            SaveTimerToPool(timer);
            return timer;
        }

        int _delayID = 1;

        public TimerEvent Delay(float interval, TimerCallBack timerCallBack)
        {
            return AddTimer("TimerDelay" + (++_delayID), interval, 0, timerCallBack, false);
        }

        public TimerEvent AddTimerToUpdate(string key, TimerCallBack timerCallBack)
        {
            return AddTimer(key, 0, -1, timerCallBack, false);
        }

        public TimerEvent AddTimerTolateUpdate(string key, TimerCallBack timerCallBack)
        {
            return AddTimer(key, 0, -1, timerCallBack, false, updateType: TimerUpdateType.LateUpdate);
        }

        public void RemoveTimer(TimerEvent timer)
        {
            RemoveTimerFromPool(timer);
        }
        #endregion

        #region ====计时器循环逻辑====
        private void UpdateByType(TimerUpdateType updateType)
        {
            var timeList = GetTimersByUpdateType(updateType);
            foreach (var item in timeList)
            {
                item.Update();
                if (item.m_isDone)
                {
                    item.CompleteTimer();

                    if (item.m_isDone)
                        RemoveTimer(item);
                }
            }
        }

        void Update()
        {
            UpdateByType(TimerUpdateType.Update);
        }

        private void LateUpdate()
        {
            UpdateByType(TimerUpdateType.LateUpdate);
        }
        #endregion

    }
}



