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
        /// ����ͼ��
        /// </summary>
        public void Init()
        {
            _UIAtlas = Resources.Load<SpriteAtlas>
            ("SpriteAtlas/UI");
        }

        /// <summary>
        /// ��ȡͼƬ
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Sprite LoadSprite(string name)
        {
           
           return _UIAtlas?.GetSprite(name);
        }
    }
}

