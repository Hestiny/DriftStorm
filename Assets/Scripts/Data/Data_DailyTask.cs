using System.Collections.Generic;
using UnityEngine;

public class Data_DailyTaskClass
{
    /// <summary>
    /// 任务Id
    /// </summary>
    public int TaskId;
    /// <summary>
    /// 出现条件(关卡)
    /// </summary>
    public int LevelId;

    public Data_DailyTaskClass(int TaskId, int LevelId)
    {
        this.TaskId = TaskId;
        this.LevelId = LevelId;

    }
}
public class Data_DailyTask
{
	static List<Dictionary<int, Data_DailyTaskClass>> checkList = new List<Dictionary<int, Data_DailyTaskClass>>()
	{
		Data_DailyTask0.data , 
	};


    static Dictionary<int, Data_DailyTaskClass> mdata;
    public static Dictionary<int, Data_DailyTaskClass> data
    {
        get
        {
            if (null == mdata)
            {
                mdata = new Dictionary<int, Data_DailyTaskClass>();
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

    public static bool CheckAndGet(int key, out Data_DailyTaskClass config)
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

    public static Data_DailyTaskClass Get(int key)
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
public class Data_DailyTask0
{
    public static Dictionary<int, Data_DailyTaskClass> data = new Dictionary<int, Data_DailyTaskClass>() 
	{ 
		        {1001 , new Data_DailyTaskClass(1001, 0) },
        {1002 , new Data_DailyTaskClass(1002, 0) },
        {1003 , new Data_DailyTaskClass(1003, 0) },
        {1004 , new Data_DailyTaskClass(1004, 0) },
        {1005 , new Data_DailyTaskClass(1005, 0) },
        {1006 , new Data_DailyTaskClass(1006, 5) },
        {1007 , new Data_DailyTaskClass(1007, 10) },
        {1008 , new Data_DailyTaskClass(1008, 15) },
        {1009 , new Data_DailyTaskClass(1009, 20) },
        {1010 , new Data_DailyTaskClass(1010, 30) },

	};
}