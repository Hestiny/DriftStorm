using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum TimerUpdateType
{
    Update,
    FixUpdate,
    LateUpdate,
}
public class TimerEvent
{

    public string m_key = "";

    /// <summary>
    /// 重复调用次数,-1代表一直调用
    /// </summary>
    public int m_repeatCount = 0;
    public int m_currentRepeat = 0;

    /// <summary>
    /// 是否忽略时间缩放
    /// </summary>
    public bool m_isIgnoreTimeScale = false;
    public float m_timeScale = 1.0f;
    public TimerCallBack m_callBack;
    public TimerUpdateType m_updateType = TimerUpdateType.Update;

    public float m_interval;//总时长
    public float m_currentTimer = 0;//当前时长

    public bool m_isDone = false;
    public bool m_isStop = false;

    public TimerEvent() { }

    public TimerEvent(string key, float interval, int repeatCount, TimerCallBack callBack, bool isRunImme = true, bool isIgnoreTimeScale = false, TimerUpdateType updateType = TimerUpdateType.Update)
    {
        m_key = key ?? GetHashCode().ToString();
        m_currentTimer = isRunImme ? interval : 0;
        m_interval = interval;
        m_repeatCount = repeatCount;
        m_currentRepeat = 0;
        m_isIgnoreTimeScale = isIgnoreTimeScale;
        m_updateType = updateType;
        m_callBack = callBack;
        m_isDone = false;
    }

    public float TimePercent()
    {
        return m_interval == 0 ? 0 : Mathf.Clamp01(m_currentTimer / m_interval);
    }

    public float LeftTime() { return m_interval - m_currentTimer; }

    // 不改变当前进度比例的情况下设置总时长
    public void ResetInterval(float interval)
    {
        m_currentTimer = TimePercent() * interval;
        m_interval = interval;
    }

    public void ReStart()
    {
        m_isDone = false;
        m_currentTimer = 0;
    }

    public void Update()
    {
        if (m_isStop)
            return;
        if (m_isIgnoreTimeScale)
        {
            m_currentTimer += Time.unscaledDeltaTime * m_timeScale;
        }
        else
        {
            m_currentTimer += Time.deltaTime * m_timeScale;
        }

        if (m_currentTimer >= m_interval)
        {
            m_isDone = true;
        }
    }

    /// <summary>
    /// 计时完成
    /// </summary>
    public void CompleteTimer()
    {
        CallBackTimer();

        if (m_repeatCount > 0)
        {
            m_currentRepeat++;
        }

        if (m_currentRepeat != m_repeatCount)
        {
            m_isDone = false;
            m_currentTimer = 0;
        }
    }

    public void CallBackTimer()
    {
        if (m_callBack != null)
        {
            m_callBack();
        }
    }

    public void SetTimeScale(float scale) { m_timeScale = scale; }

    public void ResetTimer()
    {
        m_currentTimer = 0;
        m_currentRepeat = 0;
        m_isDone = false;
    }
}
public delegate void TimerCallBack();
