using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public class AssetBundleCtrl : MonoSingleton<AssetBundleCtrl>
    {
        private AssetBundle _mainAB = null;
        private AssetBundleManifest _mainfest = null;

        private Dictionary<string, AssetBundle> _abDic = new Dictionary<string, AssetBundle>();

        /// <summary>
        /// 加载ab包
        /// </summary>
        /// <param name="abName"></param>
        public void LoadAB(string abName)
        {

        }

    }
}


