using UnityEngine;
using System.Collections;
using System.Collections.Generic;

 public class StringValueKey
{
	public static string InitGold="InitGold";
	public static string InitCash="InitCash";
	public static string InitMedal="InitMedal";
	public static string InitStemina="InitStemina";
	public static string InvenMax="InvenMax";
	public static string CharacterMax="CharacterMax";
	public static string ReinforceMax="ReinforceMax";
	public static string CompositionMax="CompositionMax";
	public static string GambleItemListCount="GambleItemListCount";
	public static string GambleRefreshTimeSec="GambleRefreshTimeSec";
	public static string StaminaRefreshTimeSec="StaminaRefreshTimeSec";
	public static string StaminaLevelupReward="StaminaLevelupReward";
	public static string StaminaAddValue="StaminaAddValue";
	public static string WaveTowerWoodPrice="WaveTowerWoodPrice";
	public static string WaveTowerStonePrice="WaveTowerStonePrice";
	public static string WaveTowerIronPrice="WaveTowerIronPrice";
	public static string Buff1="Buff1";
	public static string Buff2="Buff2";
	public static string Buff3="Buff3";
	public static string Buff4="Buff4";
	public static string ArenaSupplyTick="ArenaSupplyTick";
	public static string ArenaCloseHour="ArenaCloseHour";
	public static string ArenaOpenHour="ArenaOpenHour";
	public static string RecoveryStaminaPrice="RecoveryStaminaPrice";
	public static string ArenaTicketPrice="ArenaTicketPrice";
	public static string SkillResetPrice = "SkillResetPrice"; //	10	보석.
	public static string GambelPriceGold="GambelPriceGold";
	public static string GamblePriceJewel = "GamblePriceJewel"; //	10	겜블아이템뽑기 가격.
	public static string GambleRefreshPrice = "GambleRefreshPrice"; //	5	겜블즉시갱신가격.

}

public class StringValueTable : BaseTable 
{
	
	public Dictionary<string, int> dataList = new Dictionary<string, int>();
	
	public int GetData(string id)
	{
		if (dataList != null && dataList.ContainsKey(id) == true)
			return dataList[id];
		
		return -1;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			string id;
			int val = 0;
			
			foreach(var data in db.data)
			{
				id = data.Value.GetValue("String").ToText();
				val = data.Value.GetValue("Value").ToInt();
				
				dataList.Add(id, val);
			}
		}
		
		Item.limitReinforceStep = GetData("ReinforceMax");
		Item.limitCompositionStep = GetData("CompositionMax");
		
		CharInfoData.limitPackageItemCount = GetData("LimitPackageItems");
		
		LifeManager.defenceCalcValue = (float)GetData("defenceCalcValue");
		LifeManager.limitDamageRate = (float)GetData("LimitDamageRate") * 0.01f;
		
		//AwakeningLevel.incGoldValue = (float)GetData("ConquerorSkillIncGold");
		AwakeningLevelManager.startJewel = GetData("BuySkillPointPriceInit");
		
		
		Item.itemGradeRates.Clear();
		Item.itemSellRates.Clear();
		
		int nCount = Item.limitCompositionStep;
		string field1Name = "";
		string field2Name = "";
		
		float rate1Value = 1.0f;
		float rate2Value = 1.0f;
		for (int index = 0; index <= nCount; ++index)
		{
			field1Name = string.Format("CompositionAbility{0:D2}", index + 1);
			field2Name = string.Format("CompositionSellPrice{0:D2}", index + 1);
			
			rate1Value = (float)GetData(field1Name) * 0.001f;
			rate2Value = (float)GetData(field2Name) * 0.01f;
			
			Item.itemGradeRates.Add(rate1Value);
			Item.itemSellRates.Add(rate2Value);
		}
		
		Item.reinforceGoldRate = (float)GetData("ReinforcePrice") * 0.01f;
		
		Game.limitTodayInvites = GetData("LimitInviteCount");
	}
}
