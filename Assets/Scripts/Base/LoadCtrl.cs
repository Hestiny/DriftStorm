using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace DriftStorm
{
    public class LoadCtrl : Singleton<LoadCtrl>
    {
        private SpriteAtlas _UIAtlas;
        /// <summary>
        /// 加载图集
        /// </summary>
        public void Init()
        {
            _UIAtlas = Resources.Load<SpriteAtlas>
            ("SpriteAtlas/UI");
        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Sprite LoadSprite(string name)
        {
            var img = _UIAtlas?.GetSprite(name);
            if (img == null)
                DebugCtrl.LogError($"{name} 图片资源未找到");
            return img;
        }

        public T Load<T>(string Resname, string abName = "carmod") where T : Object
        {
            return AssetBundleCtrl.Instance.Load<T>(Resname, abName);
        }

        public T Load<T>(string name, Transform parent) where T : Object
        {
            var obj = Load<T>(name);
            var pref = GameObject.Instantiate(obj, default, default, parent);
            return pref;
        }

        /// <summary>
        /// 从resource中加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T LoadPrefab<T>(string name, Transform parent) where T : Object
        {
            var pre = Resources.Load<T>(name);
            T obj = GameObject.Instantiate(pre, default, default, parent);
            return obj;
        }
    }
}

