using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemReinforceTableInfo
{
	public int step = 0;				//강화 레벨?단계.
	
	public float expRate = 0.0f;		//다음 강화 레벨이 되기 위해 필요한 경험치 비율..
	public float sellPriceRate = 0.0f;	//현재 강화 단계의 판매 가격 비율.
	
}

public class ItemReinforceInfo
{
	public int step;
	
	public uint startExp;
	public uint limitExp;
	
	public float expRate;
	public float sellPriceRate;
}

public class ItemReinforceInfoTable : BaseTable {
	public Dictionary<int, ItemReinforceTableInfo> dataList = new Dictionary<int, ItemReinforceTableInfo>();
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int step = 0;
			ValueData valueData = null;
			foreach(var data in db.data)
			{
				if (data.Key == "")
					break;
				
				step = int.Parse(data.Key);
				
				ItemReinforceTableInfo newData = new ItemReinforceTableInfo();
				
				newData.step = step;
				newData.expRate = data.Value.GetValue("EXP").ToFloat();
				newData.sellPriceRate = data.Value.GetValue("SellPrice").ToFloat();
				
				dataList.Add(step, newData);
			}
		}
	}
	
	public ItemReinforceInfo GetItemReinforceInfo(uint baseExp, uint totalExp, float itemRate)
	{
		uint startExp = 0;
		uint limitExp = 0;
		int reinforceStep = 0;
		float sellPriceRate = 0.0f;
		float expRate = 0.0f;
		
		uint addExp = 0;
		foreach(var temp in dataList)
		{
			ItemReinforceTableInfo info = temp.Value;
			
			addExp = (uint)((float)baseExp * info.expRate * itemRate);
			limitExp = startExp + addExp;
			
			
			//마지막 addExp가 0인 경우는 startExp만 비교. 그 외에는 startExp <= temp < limitExp 비교.
			if (addExp != 0)
			{
				if (startExp <= totalExp && totalExp < limitExp)
				{
					reinforceStep = info.step;
					sellPriceRate = info.sellPriceRate;
					expRate = info.expRate;
					break;
				}
			}
			else
			{
				if (startExp <= totalExp)
				{
					reinforceStep = info.step;
					sellPriceRate = info.sellPriceRate;
					expRate = info.expRate;
					break;
				}
			}
			
			startExp = limitExp;
		}
		
		ItemReinforceInfo result = new ItemReinforceInfo();
		result.step = reinforceStep;
		result.startExp = startExp;
		result.limitExp = limitExp;
		result.sellPriceRate = sellPriceRate;
		result.expRate = expRate;
		
		return result;
	}
	
	public ItemReinforceInfo GetMaxReinforceInfo(uint baseExp, float itemRate)
	{
		uint startExp = 0;
		uint limitExp = 0;
		int reinforceStep = 0;
		float sellPriceRate = 0.0f;
		float expRate = 0.0f;
		
		uint addExp = 0;
		foreach(var temp in dataList)
		{
			ItemReinforceTableInfo info = temp.Value;
			
			addExp = (uint)((float)baseExp * info.expRate * itemRate);
			limitExp = startExp + addExp;
			
			startExp = limitExp;
		}
		
		ItemReinforceInfo result = new ItemReinforceInfo();
		result.step = reinforceStep;
		result.startExp = startExp;
		result.limitExp = limitExp;
		result.sellPriceRate = sellPriceRate;
		result.expRate = expRate;
		
		return result;
	}
}
