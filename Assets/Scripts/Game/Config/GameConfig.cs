using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/游戏配置", order = 0)]
public static class GameConfig
{
    #region ====场景====
    public const string TrackGrass = "Track1_Grassy";
    public const string TrackSnowy = "Track2_Snowy";
    public const string TrackDesert = "Track3_Desert";
    #endregion

    #region ====汽车模型名称====
    /// <summary>
    /// carmod ab包中加载
    /// id 和模型名字
    /// </summary>
    public static Dictionary<int, string> CarDic = new Dictionary<int, string>()
    {
        { 1,"sedanSports"},
        { 2,"ambulance"},
        { 3,"delivery"},
        { 4,"deliveryFlat"},
        { 5,"firetruck"},
        { 6,"hatchbackSports"},
        { 7,"police"},
        { 8,"race"},
        { 9,"raceFuture"},
        { 10,"sedan"},
        { 11,"sedanSports"},
        { 12,"suv"},
        { 13,"suvLuxury"},
        { 14,"taxi"},
        { 15,"tractor"},
    }; 

    /// <summary>
    /// 通过id查找汽车名
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static string GetCarNameById(int id)
    {
        if(!CarDic.TryGetValue(id,out string carname))
        {
            DebugCtrl.LogError($"{id} 字典不存在该id的汽车ab名");
            foreach (var item in CarDic)
            {
                return item.Value;
            }
        }
        return carname;
    }
    #endregion

    #region ====预制体路径====
    public const string CarPath = "Prefabs/Car/";
    public const string CarPrefab = CarPath + "car";
    #endregion
}
