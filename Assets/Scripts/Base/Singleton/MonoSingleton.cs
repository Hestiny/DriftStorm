using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    protected static T _Instance = null;
    private static bool _applicationIsQuit = false;
    static Transform mRoot;
    static Transform root
    {
        get
        {
            if (null == mRoot)
                mRoot = GameObject.Find("Root").transform;
            return mRoot;
        }
    }

    public static T Instance
    {
        get
        {
            if (_applicationIsQuit)
                return null;

            if (_Instance != null) return _Instance;

            Component[] managers = root.GetComponentsInChildren(typeof(T));
            //GameObject.FindObjectsOfTypeAll(typeof(T)) as T[];
            if (managers.Length != 0)
            {
                if (managers.Length == 1)
                {
                    _Instance = managers[0] as T;
                    _Instance.gameObject.name = typeof(T).Name;
                    DontDestroyOnLoad(_Instance.gameObject);
                    return _Instance;
                }
                else
                {
                    foreach (T manager in managers)
                    {
#if UNITY_EDITOR
                        Destroy(manager.gameObject);
#else
                        Destroy(manager.gameObject);
#endif
                    }
                }
            }

            GameObject gO = new GameObject(typeof(T).Name, typeof(T));
            _Instance = gO.GetComponent<T>();
            gO.transform.parent = root;
            //DontDestroyOnLoad(_Instance.gameObject);
            return _Instance;
        }
    }
    protected virtual void Awake()
    {
        if (null == _Instance)
            _Instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
    public static void ReleaseInstance()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
    }
    public virtual void Dispose()
    {

    }
    public virtual void OnDestroy()
    {
        _applicationIsQuit = true;
        _Instance = null;
    }
    private void OnApplicationQuit()
    {
        if (_Instance != null)
            Destroy(_Instance.gameObject);
    }
}
