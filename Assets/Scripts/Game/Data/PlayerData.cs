using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DriftStorm
{
    [SerializeField]
    public class PlayerData
    {
        //玩家等级
        public int LV = 1;
        //场景id
        public int ScenesId = 1;
        //选择的赛车
        public int CarId = 1;


        //背包数据<道具id,道具数量>
        public Dictionary<int, string> BackpackInfoMap = new Dictionary<int, string>();

    }
}


