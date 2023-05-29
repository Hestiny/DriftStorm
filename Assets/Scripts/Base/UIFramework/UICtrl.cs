using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    public class UICtrl
    {
        public struct WinVariable
        {
            public string ResPath;
            public UIGroupEnum WinGroup;

            public WinVariable(string path, UIGroupEnum group)
            {
                ResPath = path;
                WinGroup = group;
            }
        }

        public static Transform Root;
        public static Transform UIContent;
        public static Transform UILayer;
        public static UIPanel CurrentWin;
        public static UIPanel MainWin;
        public static Camera UICamera { private set; get; }
        public static Dictionary<System.Type, List<UIPanel>> WindowList = new Dictionary<System.Type, List<UIPanel>>();
        public static Dictionary<string, WinVariable> WindowResPath = new Dictionary<string, WinVariable>()
        {
            { typeof(MainWindow).Name,new WinVariable(UIPrefabPath.MainWindow,UIGroupEnum.Main ) },
            { typeof(LoadingWindow).Name,new WinVariable(UIPrefabPath.LoadingWindow,UIGroupEnum.PopPu ) },
        };

        // 弹框列表
        private static Dictionary<UIGroupEnum, List<UIPanel>> _windowList = new Dictionary<UIGroupEnum, List<UIPanel>>();

        // 弹框父节点
        private static Dictionary<UIGroupEnum, Transform> _groupParentDic = new Dictionary<UIGroupEnum, Transform>();

        public static void Init()
        {
            Root = GameObject.Find("Root").transform;
            UILayer = GameObject.Find("Root/UILayer").transform;
            UIContent = UILayer.Find("UICanvas/UIContent");
            UICamera = GameObject.Find("Root/UILayer/UICamera").GetComponent<Camera>();
            UnityEngine.Object.DontDestroyOnLoad(Root);
            // 创建group节点
            var groups = Enum.GetNames(typeof(UIGroupEnum));
            var uiLayer = LayerMask.NameToLayer("UI");
            for (int i = 0; i < groups.Length; i++)
            {
                UIGroupEnum groupEnum = (UIGroupEnum)Enum.Parse(typeof(UIGroupEnum), groups[i]);
                GameObject groupObj = new GameObject(groups[i]);
                groupObj.transform.SetParent(UIContent.transform);
                groupObj.transform.localPosition = Vector3.zero;
                groupObj.transform.localScale = Vector3.one;
                groupObj.layer = uiLayer;

                Canvas cvs = groupObj.AddComponent<Canvas>();
                cvs.overrideSorting = true;
                cvs.sortingOrder = (int)groupEnum;

                _groupParentDic.Add(groupEnum, groupObj.transform);

                GraphicRaycaster rayCaster = groupObj.AddComponent<GraphicRaycaster>();
                rayCaster.ignoreReversedGraphics = true;
                rayCaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;

                var rect = groupObj.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(1f, 1f);
                rect.pivot = new Vector2(0.5f, 0.5f);
                rect.sizeDelta = Vector2.zero;
            }

        }

        public static Transform GetGroupNode(UIGroupEnum group)
        {
            return _groupParentDic[group];
        }

        //TODO: 拿到页面预制体的路径,多个实例窗口的创建限制
        private static T GeneratePanel<T, H>(H? pram, Transform target = null) where T : UIWIndow where H : struct
        {
            if (target == null)
            {
                target = UIContent;
            }
            if (!WindowResPath.TryGetValue(typeof(T).Name, out var winVariable))
            {
                Debug.LogError(typeof(T).Name + " 该窗体没有设置预制体路径WindowResPath");
                return default(T);
            }
            target = GetGroupNode(winVariable.WinGroup);
            var winObj = Resources.Load<GameObject>(winVariable.ResPath);
            var win = UnityEngine.Object.Instantiate(winObj, target).GetComponent<T>();
            if (win == null)
            {
                Debug.LogError(string.Format("Component [{0}] not find.", typeof(T).Name));
            }
            if (WindowList.TryGetValue(typeof(T), out var winList))
            {
                winList.Add(win);
            }
            else
            {
                WindowList.Add(typeof(T), new List<UIPanel> { win });
            }
            win.init(pram);
            win.Open();
            return win;
        }

        private static T GeneratePanel<T>(Transform target = null) where T : UIWIndow
        {
            if (target == null)
            {
                target = UIContent;
            }
            if (!WindowResPath.TryGetValue(typeof(T).Name, out var winVariable))
            {
                Debug.LogError(typeof(T).Name + " 该窗体没有设置预制体路径WindowResPath");
                return default(T);
            }
            target = GetGroupNode(winVariable.WinGroup);
            var winObj = Resources.Load<GameObject>(winVariable.ResPath);
            var win = UnityEngine.Object.Instantiate(winObj, target).GetComponent<T>();
            if (win == null)
            {
                Debug.LogError(string.Format("Component [{0}] not find.", typeof(T).Name));
            }
            if (WindowList.TryGetValue(typeof(T), out var winList))
            {
                winList.Add(win);
            }
            else
            {
                WindowList.Add(typeof(T), new List<UIPanel> { win });
            }
            win.Open();
            return win;
        }

        public static T OpenWindow<T, H>(H? pram = null) where T : UIWIndow where H : struct
        {
            return GeneratePanel<T, H>(pram);
        }

        public static T OpenWindow<T>() where T : UIWIndow
        {
            return GeneratePanel<T>();
        }

        public static void CloseWindow<T>() where T : UIPanel
        {
            if (WindowList.TryGetValue(typeof(T), out var winList))
            {
                winList[winList.Count - 1].Hide();
                winList.RemoveAt(winList.Count - 1);
                if (winList.Count == 0)
                    WindowList.Remove(typeof(T));
            }
            else
            {
                DebugCtrl.Log("没有加载该窗口:" + typeof(T).Name);
            }
        }

        public static void CloseWindow(UIPanel win)
        {
            var winType = win.GetType();
            if (WindowList.TryGetValue(winType, out var winList))
            {
                winList[winList.Count - 1].Hide();
                winList.RemoveAt(winList.Count - 1);
                if (winList.Count == 0)
                    WindowList.Remove(winType);
            }
            else
            {
                DebugCtrl.Log("没有加载该窗口:" + winType.Name);
            }
        }
    }

}

