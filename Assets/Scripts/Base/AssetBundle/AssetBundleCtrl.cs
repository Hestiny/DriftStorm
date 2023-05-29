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

        private const string _ABPath = "Assets/StreamingAssets/";

        /// <summary>
        /// 初始化主包信息
        /// </summary>
        public void InitMainAb()
        {
            _mainAB = AssetBundle.LoadFromFile(_ABPath + "StreamingAssets");
            _mainfest = _mainAB.LoadAsset<AssetBundleManifest>("StreamingAssets");

        }

        /// <summary>
        /// 加载ab包
        /// </summary>
        /// <param name="abName"></param>
        public AssetBundle LoadAB(string abName)
        {
            if (_mainfest != null)
            {
                //加载依赖包
                string[] strs = _mainfest.GetAllDependencies(abName);
                for (int i = 0; i < strs.Length; i++)
                {
                    string depenName = strs[i];
                    if (!_abDic.TryGetValue(depenName, out _))
                    {
                        var ab = AssetBundle.LoadFromFile(_ABPath + depenName);
                        _abDic.Add(depenName, ab);
                    }
                }
            }

            //加载目标包
            if (!_abDic.TryGetValue(abName, out _))
            {
                var ab = AssetBundle.LoadFromFile(_ABPath + abName);
                _abDic.Add(abName, ab);
            }

            return _abDic[abName];
        }

        /// <summary>
        /// 卸载包
        /// </summary>
        /// <param name="abName"></param>
        public void UnLoadAB(string abName, bool isAllUnload = false)
        {
            if (_abDic.TryGetValue(abName, out var ab))
            {
                ab.Unload(isAllUnload);
                _abDic.Remove(abName);
            }
        }

        public void UnloadAllAB(bool isAllUnload = false)
        {
            AssetBundle.UnloadAllAssetBundles(isAllUnload);
        }

        #region ====资源加载====
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        public Object Load(string resName, string abName = "")
        {
            if (abName != "")
            {
                LoadAB(abName);
                return _abDic[abName].LoadAsset(resName);
            }
            else
            {
                var ator = _abDic.GetEnumerator();
                while (ator.MoveNext())
                {
                    if (_abDic[abName].Contains(name))
                    {
                        return _abDic[abName].LoadAsset(resName);
                    }
                }
            }

            DebugCtrl.LogError($"{resName} 资源没有在包中找到!");
            return null;
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resName"></param>
        /// <param name="abName"></param>
        /// <returns></returns>
        public T Load<T>(string resName, string abName = "") where T : Object
        {
            if (abName != "")
            {
                LoadAB(abName);
                return _abDic[abName].LoadAsset<T>(resName);
            }
            else
            {
                var ator = _abDic.GetEnumerator();
                while (ator.MoveNext())
                {
                    if (_abDic[abName].Contains(name))
                    {
                        return _abDic[abName].LoadAsset<T>(resName);
                    }
                }
            }

            DebugCtrl.LogError($"{resName} 资源没有在包中找到!");
            return null;
        }
        #endregion
    }
}


