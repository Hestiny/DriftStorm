using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;
namespace DriftStorm
{
    public class Launch : MonoBehaviour
    {
        private void Awake()
        {
            AssetBundleCtrl.Instance.InitMainAb();
            LoadCtrl.Instance.Init();
            DataCtrl.Instance.Init();
            UICtrl.Init();
        }

        void Start()
        {
            UICtrl.OpenWindow<LoadingWindow>();
            UICtrl.OpenWindow<MainWindow>();
            ScenceCtrl.Instance.LoadSceneAsync(GameConfig.TrackGrass, UnityEngine.SceneManagement.LoadSceneMode.Single, null);
            var obj = LoadCtrl.Instance.Load<GameObject>("sedanSports", UICtrl.Root);
            //var prefab = Instantiate(obj);
            //prefab.transform.position = default;
        }

        void Update()
        {

        }
    }
}

