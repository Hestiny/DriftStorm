using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DriftStorm
{
    public class DataCtrl : Singleton<DataCtrl>
    {
        #region ====��������====
        const string PalyeDataKey = "PalyeDataKey";

        private PlayerData _playData;

        public void Init()
        {
            _playData = new PlayerData();
            string defaultData = JsonUtility.ToJson(_playData);
            string json = PlayerPrefs.GetString(PalyeDataKey, defaultData);
            _playData = JsonUtility.FromJson<PlayerData>(json);
            DebugCtrl.Log(json, Color.green);
        }

        public void Save()
        {
            string json = JsonUtility.ToJson(_playData);
            PlayerPrefs.SetString(PalyeDataKey, json);
        }

        public void Clear()
        {
            PlayerPrefs.DeleteAll();
        } 
        #endregion

        /// <summary>
        /// ��ȡ�ȼ�
        /// </summary>
        public void GetLv()
        {

        }
    }

}
