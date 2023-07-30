using System.Collections.Generic;
using UnityEngine;

public class Data_DailyTaskRewardBoxClass
{
    /// <summary>
    /// 宝箱id
    /// </summary>
    public int Id;
    /// <summary>
    /// 需要的任务点数
    /// </summary>
    public int StagePoint;
    /// <summary>
    /// 无通行证时奖励id
    /// </summary>
    public int[] RewardId;
    /// <summary>
    /// 无通行证时奖励数量
    /// </summary>
    public int[] RewardNum;
    /// <summary>
    /// 有通行证时奖励id
    /// </summary>
    public int[] BattlePassRewardId;
    /// <summary>
    /// 有通行证时奖励数量
    /// </summary>
    public int[] BattlePassRewardNum;

    public Data_DailyTaskRewardBoxClass(int Id, int StagePoint, int[] RewardId, int[] RewardNum, int[] BattlePassRewardId, int[] BattlePassRewardNum)
    {
        this.Id = Id;
        this.StagePoint = StagePoint;
        this.RewardId = RewardId;
        this.RewardNum = RewardNum;
        this.BattlePassRewardId = BattlePassRewardId;
        this.BattlePassRewardNum = BattlePassRewardNum;

    }
}
public class Data_DailyTaskRewardBox
{
	static List<Dictionary<int, Data_DailyTaskRewardBoxClass>> checkList = new List<Dictionary<int, Data_DailyTaskRewardBoxClass>>()
	{
		Data_DailyTaskRewardBox0.data , 
	};


    static Dictionary<int, Data_DailyTaskRewardBoxClass> mdata;
    public static Dictionary<int, Data_DailyTaskRewardBoxClass> data
    {
        get
        {
            if (null == mdata)
            {
                mdata = new Dictionary<int, Data_DailyTaskRewardBoxClass>();
                int count = checkList.Count;
                for (int i = 0; i < count; i++)
                {
                    foreach (var item in checkList[i])
                        mdata.Add(item.Key, item.Value);
                }

                for (int i = count - 1; i >= 0; i--)
                    checkList[i].Clear();

                checkList.Clear();
            }
            return mdata;
        }
    }

    public static bool Check(int key)
    {
        if (null != mdata)
            return mdata.ContainsKey(key);
        else
        {
            foreach (var item in checkList)
            {
                if (item.ContainsKey(key))
                    return true;
            }
        }

        return false;
    }

    public static bool CheckAndGet(int key, out Data_DailyTaskRewardBoxClass config)
    {
        config = null;

        if (null != mdata)
            return mdata.TryGetValue(key, out config);
        else
        {
            foreach (var item in checkList)
            {
                if (item.ContainsKey(key))
                {
                    config = item[key];
                    return true;
                }
            }
        }
        return false;
    }

    public static Data_DailyTaskRewardBoxClass Get(int key)
    {
        if (null != mdata)
        {
            if (mdata.ContainsKey(key))
                return mdata[key];
        }
        else
        {
            foreach (var item in checkList)
            {
                if (item.ContainsKey(key))
                    return item[key];
            }
        }

        return null;
    }
}
public class Data_DailyTaskRewardBox0
{
    public static Dictionary<int, Data_DailyTaskRewardBoxClass> data = new Dictionary<int, Data_DailyTaskRewardBoxClass>() 
	{ 
		        {0 , new Data_DailyTaskRewardBoxClass(0, 20, new int[] {105}, new int[] {5}, new int[] {105,20}, new int[] {5,10}) },
        {1 , new Data_DailyTaskRewardBoxClass(1, 40, new int[] {106}, new int[] {20}, new int[] {106,20}, new int[] {20,10}) },
        {2 , new Data_DailyTaskRewardBoxClass(2, 60, new int[] {101}, new int[] {1}, new int[] {101,20}, new int[] {1,10}) },
        {3 , new Data_DailyTaskRewardBoxClass(3, 80, new int[] {107}, new int[] {20}, new int[] {107,20}, new int[] {20,10}) },
        {4 , new Data_DailyTaskRewardBoxClass(4, 100, new int[] {2}, new int[] {100}, new int[] {2,20}, new int[] {100,50}) },

	};
}