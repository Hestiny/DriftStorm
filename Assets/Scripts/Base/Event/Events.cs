using UnityEngine;

public interface IBaseEventStruct
{
    EnumEventType GetEventType();
}

/// <summary>
/// 开始游戏
/// </summary>
public struct StartGameEventPram : IBaseEventStruct
{
    public EnumEventType GetEventType()
    {
        return EnumEventType.StartGameEventPram;
    }
}

public struct TestEventPram : IBaseEventStruct
{
    public int param1;

    public EnumEventType GetEventType()
    {
        return EnumEventType.TestEvebtPram;
    }
}
public struct SpritePram : IBaseEventStruct
{
    public Sprite img;

    public EnumEventType GetEventType()
    {
        return EnumEventType.SpritePram;
    }
}public struct TrackSpritePram : IBaseEventStruct
{
    public Sprite img;

    public EnumEventType GetEventType()
    {
        return EnumEventType.trackSpritePram;
    }
}

#region ====旧事件系统====

[System.Serializable]
public enum EnumEventType
{
    None,
    TestEvebtPram,
    SpritePram,
    trackSpritePram,
    EventTypeEnd,
    StartGameEventPram,
}

[System.Serializable]
public class BaseEventArgs
{
    public EnumEventType eventType;

    public BaseEventArgs(EnumEventType type)
    {
        eventType = type;
    }
}

[System.Serializable]
public class EventArgsOne<T> : BaseEventArgs
{
    public T param1;

    public EventArgsOne(EnumEventType type, T p1) : base(type)
    {
        param1 = p1;
    }
}

[System.Serializable]
public class EventArgsTwo<T1, T2> : EventArgsOne<T1>
{
    public T2 param2;

    public EventArgsTwo(EnumEventType type, T1 p1, T2 p2) : base(type, p1)
    {
        param2 = p2;
    }
}

[System.Serializable]
public class EventArgsThree<T1, T2, T3> : EventArgsTwo<T1, T2>
{
    public T3 param3;

    public EventArgsThree(EnumEventType type, T1 p1, T2 p2, T3 p3) : base(type, p1, p2)
    {
        param3 = p3;
    }
}

public delegate void OnTouchEventHandle(GameObject _listener, object _args, params object[] _params);
public enum EnumTouchEventType
{
    OnClick,
    OnDoubleClick,
    OnDown,
    OnUp,
    OnEnter,
    OnExit,
    OnSelect,
    OnUpdateSelect,
    OnDeSelect,
    OnDrag,
    OnDragEnd,
    OnDrop,
    OnScroll,
    OnMove,
}
#endregion
