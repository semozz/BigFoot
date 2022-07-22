using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExpInfo
{
	public System.Int64 needExp = 0;
	
	public System.Int64 baseExp = 0;
	public System.Int64 limitExp = 0;
}

public class CharExpTable : BaseTable {
	public Dictionary<int, ExpInfo> dataList = new Dictionary<int, ExpInfo>();
	
	public int maxLevel = 1;
	
	public ExpInfo GetData(int id)
	{
		ExpInfo data = null;
		if (dataList != null &&
			dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			ExpInfo expInfo = null;
			
			ValueData valueData = null;
			
			long needExp = 0; long test = 0;
			long sumExp = 0;
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				id = int.Parse(data.Key);
				
				expInfo = new ExpInfo();
				
				needExp = 0L;
				valueData = data.Value.GetValue("EXP");
				if (valueData != null)
					needExp = valueData.ToLong();
				
				expInfo.baseExp = sumExp;
				expInfo.needExp = needExp;
				
				sumExp += needExp;
				expInfo.limitExp = sumExp;
				
				this.dataList.Add(id, expInfo);
				
				this.maxLevel = id;
			}
		}
	}

    public int GetLevel(long exp)
    {
		int level = -1;
		ExpInfo info = null;
		
		int lastLevel = 1;
		foreach(var temp in dataList)
		{
			lastLevel = temp.Key;
			
			info = temp.Value;
			if (info.baseExp <= exp && exp < info.limitExp)
			{
				level = temp.Key;
				break;
			}
		}
		
		if (level == -1)
			level = lastLevel;
		
		return level;
    }


    public long GetNeedExp(int level)
    {
		long needExp = 0L;
		
		ExpInfo info = null;
		if (dataList.ContainsKey(level) == true)
		{
			info = dataList[level];
			needExp = info.needExp;
		}
        
		return needExp;
    }

    public long GetBaseExp(int level)
    {
		long baseExp = 0L;
		
		ExpInfo info = null;
		if (dataList.ContainsKey(level) == true)
		{
			info = dataList[level];
			baseExp = info.baseExp;
		}
        
		return baseExp;
    }
	
	public ExpInfo GetExpInfo(int level)
	{
		ExpInfo info = null;
		if (dataList.ContainsKey(level) == true)
			info = dataList[level];
		
		return info;
	}

    public float GetProgressRate(long exp)
    {
        int curLevel = GetLevel(exp);
        long curBaseExp = GetBaseExp(curLevel);

        long needExp = GetNeedExp (curLevel );
        long totalExp = exp - curBaseExp;
		
		if (needExp == 0.0f)
			return 1.0f;
		else
        	return (float)totalExp / (float)needExp;
    }
}
