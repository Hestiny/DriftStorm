using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/��Ϸ����", order = 0)]
public class GameConfig 
{
   
    public const string TrackGrass = "Track1_Grassy";
    public const string TrackSnowy = "Track2_Snowy";
    public const string TrackDesert = "Track3_Desert";

    /// <summary>
    /// id ��ģ������
    /// </summary>
    public Dictionary<int, string> CarDic = new Dictionary<int, string>()
    {

    };
}
