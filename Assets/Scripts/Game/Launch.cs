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
            LoadCtrl.Instance.Init();
            DataCtrl.Instance.Init();
            UICtrl.Init();
        }

        void Start()
        {
            UICtrl.OpenWindow<MainWindow, MainWindow.WindowParam>();
            ScenceCtrl.Instance.LoadSceneAsync(GameConfig.TrackGrass, UnityEngine.SceneManagement.LoadSceneMode.Single, null);
        }

        void Update()
        {

        }
    }
}

