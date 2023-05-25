using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DriftStorm
{
    public class ScenceCtrl : MonoSingleton<ScenceCtrl>
    {
        public void LoadScene(string sceneName, LoadSceneMode mode, Action completed)
        {
            SceneManager.LoadScene(sceneName, mode);
            if (null != completed)
                completed();
        }

        public void LoadSceneAsync(string sceneName, LoadSceneMode mode, Action completed)
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName, mode);
            asyncOperation.completed += (AsyncOperation obj) =>
            {
                if (null != completed)
                    completed();
            };
        }

        public void UnloadSceneAsync(string sceneName, UnloadSceneOptions options, Action completed)
        {
            AsyncOperation asyncOperation = SceneManager.UnloadSceneAsync(sceneName, options);
            asyncOperation.completed += (AsyncOperation obj) =>
            {
                if (null != completed)
                    completed();
            };
        }
    }
}


