using System.Collections;
using System.Collections.Generic;
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
           
           return _UIAtlas?.GetSprite(name);
        }
    }
}

