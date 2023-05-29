using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIPanel : MonoBehaviour
{
    public bool IsActive { get; set; }
    /// <summary>
    /// 所有按钮的点击回调
    /// </summary>
    protected Dictionary<Button, UnityAction> buttonClickActionMap = new Dictionary<Button, UnityAction>();
    /// <summary>
    /// 所有事件的触发回调
    /// </summary>
    protected Dictionary<string, Action<string>> eventActionMap = new Dictionary<string, Action<string>>();

    protected virtual void RegisterUIAction()
    {
        foreach (var item in buttonClickActionMap)
        {
            item.Key.onClick.AddListener(item.Value);
        }
    }

    protected virtual void OnDestroy()
    {
        foreach (var item in buttonClickActionMap)
            item.Key.onClick.RemoveAllListeners();
    }

    private void Awake()
    {
        OnAwake();
        RegisterUIAction();
    }

    public abstract void OnAwake();

    //TODO: 音效 ,事件,动画
    public virtual void Open()
    {
        BeforeShow();
        IsActive = true;
        StopCoroutine("DelayDestroy");
        Show();
    }

    public virtual void BeforeShow()
    {

    }

    private IEnumerator DelayDestroy()
    {

        transform.localPosition = new Vector3(10000, 10000, 0);
        //transform.gameObject.SetActive(false);
        yield return new WaitForSeconds(10.0f);
        ImmediateDestroy();
    }

    private void ImmediateDestroy()
    {
        Destroy(gameObject);
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    public virtual void Show()
    {

    }

    public virtual void Hide(bool isImmediateDestroy = false)
    {
        if (isImmediateDestroy)
        {
            ImmediateDestroy();
        }
        else
        {
            StartCoroutine("DelayDestroy");
        }
    }
}
