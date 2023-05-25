using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EventDispatcher
{

    #region ====新事件系统====
    static Dictionary<string, Action<IBaseEventStruct>> _route = new Dictionary<string, Action<IBaseEventStruct>>();

    public static Dictionary<string, Action<IBaseEventStruct>> Route
    {
        get { return _route; }
    }

    public static void Clear()
    {
        _route.Clear();
    }

    static void _CheckAddingListenerParam(string eventType, Delegate delegateToAdd)
    {
        if (!_route.ContainsKey(eventType))
        {
            _route.Add(eventType, null);
        }

        Delegate d = _route[eventType];
        if (d != null && d.GetType() != delegateToAdd.GetType())
        {
            throw new Exception(string.Format(
                    "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                    eventType, d.GetType().Name, delegateToAdd.GetType().Name));
        }
    }

    static bool _CheckRemoveListenerParam(string eventType, Delegate delegateToRemove)
    {
        if (!_route.ContainsKey(eventType))
            return false;

        Delegate d = _route[eventType];
        if ((d != null) && (d.GetType() != delegateToRemove.GetType()))
        {
            throw new Exception(string.Format(
                "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                eventType, d.GetType(), delegateToRemove.GetType()));
        }
        else
            return true;
    }

    public static void AddListener(string eventType, Action<IBaseEventStruct> handler)
    {
        _CheckAddingListenerParam(eventType, handler);
        _route[eventType] += handler;
    }

    public static void RemoveListener(string eventType, Action<IBaseEventStruct> handler)
    {
        if (_CheckRemoveListenerParam(eventType, handler))
        {
            _route[eventType] -= handler;
            if (_route[eventType] == null)
            {
                _route.Remove(eventType);
            }
        }
    }

    public static void TriggerEvent(IBaseEventStruct eventType)
    {

        Action<IBaseEventStruct> handler;
        if (!_route.TryGetValue(eventType.GetType().Name, out handler))
        {
            return;
        }

        var callbacks = handler.GetInvocationList();
        for (int i = 0; i < callbacks.Length; ++i)
        {
            Action<IBaseEventStruct> callback = callbacks[i] as Action<IBaseEventStruct>;

            if (callback == null)
                throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));

            try
            {
                callback(eventType);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("TriggerEvent {0} error: ", eventType) + e.Message);
                //Foundation.LogUtility.LogError(string.Format("TriggerEvent {0} error: ", eventType) + e.Message);
                Debug.LogError($"TriggerEvent {e.Message} error: {eventType}");
                throw new Exception("TriggerEvent error", e);
            }
        }
    }
    #endregion

    #region ====旧事件系统====

    // 消息路由表
    //Dictionary<string, Action<EventArgs>> _router = new Dictionary<string, Action<EventArgs>>();
    static Dictionary<EnumEventType, Action<BaseEventArgs>> _router = new Dictionary<EnumEventType, Action<BaseEventArgs>>();

    public static Dictionary<EnumEventType, Action<BaseEventArgs>> Router
    {
        get { return _router; }
    }

    //public static void Clear()
    //{
    //    _router.Clear();
    //}

    //static void _CheckAddingListenerParam(EnumEventType eventType, Delegate delegateToAdd)
    //{
    //    if (!_router.ContainsKey(eventType))
    //    {
    //        _router.Add(eventType, null);
    //    }

    //    Delegate d = _router[eventType];
    //    if (d != null && d.GetType() != delegateToAdd.GetType())
    //    {
    //        throw new Exception(string.Format(
    //                "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
    //                eventType, d.GetType().Name, delegateToAdd.GetType().Name));
    //    }
    //}

    //static bool _CheckRemoveListenerParam(EnumEventType eventType, Delegate delegateToRemove)
    //{
    //    if (!_router.ContainsKey(eventType))
    //        return false;

    //    Delegate d = _router[eventType];
    //    if ((d != null) && (d.GetType() != delegateToRemove.GetType()))
    //    {
    //        throw new Exception(string.Format(
    //            "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
    //            eventType, d.GetType(), delegateToRemove.GetType()));
    //    }
    //    else
    //        return true;
    //}

    //public static void AddListener(EnumEventType eventType, Action<BaseEventArgs> handler)
    //{
    //    _CheckAddingListenerParam(eventType, handler);
    //    _router[eventType] += handler;
    //}

    //public static void RemoveListener(EnumEventType eventType, Action<BaseEventArgs> handler)
    //{
    //    if (_CheckRemoveListenerParam(eventType, handler))
    //    {
    //        _router[eventType] -= handler;
    //        if (_router[eventType] == null)
    //        {
    //            _router.Remove(eventType);
    //        }
    //    }
    //}

    public static void TriggerEvent(BaseEventArgs args)
    {

        EnumEventType eventType = args.eventType;
        Action<BaseEventArgs> handler;
        if (!_router.TryGetValue(eventType, out handler))
        {
            return;
        }

        var callbacks = handler.GetInvocationList();
        for (int i = 0; i < callbacks.Length; ++i)
        {
            Action<BaseEventArgs> callback = callbacks[i] as Action<BaseEventArgs>;

            if (callback == null)
                throw new Exception(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));

            try
            {
                callback(args);
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("TriggerEvent {0} error: ", eventType) + e.Message);
                //Foundation.LogUtility.LogError(string.Format("TriggerEvent {0} error: ", eventType) + e.Message);
                Debug.LogError($"TriggerEvent {e.Message} error: {eventType}");
                //throw e;
                //以下两种写法可以定位到具体异常堆栈点
                //throw;
                throw new Exception("TriggerEvent error", e);
            }
        }
    }
    #endregion
}
