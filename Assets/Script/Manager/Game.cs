using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoginInfo
{
	public string loginID = "";
	public string pass = "";
	
	public string kakaoAccessToken = "";
	public string kakaoRefreshToken = "";
	
	public System.DateTime loginDate;
	
	public long userID = 0;
	public int charIndex = 0;
	
	public bool eula_Checked = false;
	public int eula_version = 1;
	public string eula_url = "";
	public string private_url = "";
	
	public AccountType acctountType = AccountType.MonsterSide;
}

public class StageInfo
{
	public StageButton.eStageButton stageInfo = StageButton.eStageButton.Locked;
}

public class MasterySaveData
{
	public int tableID = -1;
	public int level = 0;
}

public class CharMasteryData
{
	public int masteryID = 0;
	public int masteryLevel = 0;
}

public class CharPrivateData
{
	public int levelupRewardEventCheck = 0;
	
	//겜블
	public List<GambleItem> gambleItemList = new List<GambleItem>();
	public System.DateTime refreshGambleExpireTime;
	public System.TimeSpan SetGambleTime(int addSec)
	{
		refreshGambleExpireTime = System.DateTime.Now;
		
		//addSec = 15;
		System.TimeSpan addSpan = Game.ToTimeSpan(addSec);
		
		refreshGambleExpireTime += addSpan;
		
		return addSpan;
	}
	
	public List<int> gambleEventItemIDs = new List<int>();
	public System.DateTime gambleEventEndTime;
	public void SetGambleInfo(GambleItem[] Items, int leftTimeSec, int[] eventItemIDs)
	{
		this.gambleItemList.Clear();
		foreach(GambleItem temp in Items)
			gambleItemList.Add(temp);
		
		
		if (leftTimeSec > 0)
			gambleEventEndTime = System.DateTime.Now + Game.ToTimeSpan(leftTimeSec);
		else
			gambleEventEndTime = System.DateTime.Now;
		
		gambleEventItemIDs.Clear();
		if (eventItemIDs != null)
		{
			foreach(int itemID in eventItemIDs)
				gambleEventItemIDs.Add(itemID);
		}
	}
	
	public bool CheckGambleEvent()
	{
		System.DateTime nowTime = System.DateTime.Now;
		System.TimeSpan timeSpan = gambleEventEndTime - nowTime;
		
		bool isGambleEvent = timeSpan.TotalSeconds > 0;
		return isGambleEvent;
	}
	
	public float CheckGambleLeftTime()
	{
		System.DateTime nowTime = System.DateTime.Now;
		System.TimeSpan timeSpan = gambleEventEndTime - nowTime;
		
		return (float)timeSpan.TotalSeconds;
	}
	
	public System.TimeSpan CheckGambleEventTime()
	{
		System.DateTime nowTime = System.DateTime.Now;
		System.TimeSpan timeSpan = gambleEventEndTime - nowTime;
		
		return timeSpan;
	}
	
	public int GetGambleEventItemID(int index)
	{
		int itemID = -1;
		int nCount = gambleEventItemIDs.Count;
		if (index >= 0 && index < nCount)
			itemID = gambleEventItemIDs[index];
		
		return itemID;
	}
	
	public string NickName = "";
	
	public System.Int64 stageClearExp = 0;
	
	public int RankType = 0;
    public string platform = "";
	public GameDef.ePlayerClass playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
	
	public CharacterDBInfo baseInfo = new CharacterDBInfo();
	
	public List<EquipInfo> equipData = new List<EquipInfo>();
	public CostumeSetItem costumeSetItem = null;
	
	public static List<EquipInfo> GetEquipItemInfos(List<EquipInfo> list, CostumeSetItem costumeSet)
	{
		List<EquipInfo> newEquipList = new List<EquipInfo>();
		newEquipList.AddRange(list);
		
		if (costumeSet != null)
		{
			foreach(Item item in costumeSet.items)
			{
				int slotIndex = EquipInfo.ItemTypeToEquipSlotIndex(null, item.itemInfo.itemType);
				
				EquipInfo equipInfo = new EquipInfo();
				equipInfo.SetSlotType(slotIndex);
				equipInfo.item = item;
				
				newEquipList[slotIndex] = equipInfo;
			}
		}
		
		return newEquipList;
	}
	
	
	public int maxStage = 80;
	public Dictionary<int, List<StageInfo>> stageInfos = new Dictionary<int, List<StageInfo>>();
	public List<StageInfo> GetModeStageInfos(int stageType)
	{
		List<StageInfo> list = null;
		if (stageInfos.ContainsKey(stageType) == true)
			list = stageInfos[stageType];
		
		return list;
	}
	
	public List<MasterySaveData> masteryInfos = new List<MasterySaveData>();
	public Dictionary<int, CharMasteryData> masteryDatas = new Dictionary<int, CharMasteryData>();
	public Dictionary<int, CharMasteryData> awakenSkillDatas = new Dictionary<int, CharMasteryData>();
	
	public ArenaInfo arenaInfo = new ArenaInfo();
	public void SetArenaInfo(ArenaInfo info)
	{
		arenaInfo.groupRanking = info.groupRanking;
		arenaInfo.rankType = info.rankType;
		arenaInfo.seasonBestRank = info.seasonBestRank;
		arenaInfo.totalWinningCount = info.totalWinningCount;
		arenaInfo.winningStreakCount = info.winningStreakCount;
	}
	
	public WaveRankingInfo waveInfo = new WaveRankingInfo();
	public void SetDefenceInfo(WaveRankingInfo info)
	{
		waveInfo.UserIndexID = info.UserIndexID;
		waveInfo.CharacterIndex = info.CharacterIndex;
		waveInfo.ranking = info.ranking;
		waveInfo.RecordStep = info.RecordStep;
		waveInfo.RecordSec = info.RecordSec;
		waveInfo.NickName = info.NickName;
	}
	
	public void SetWaveRankInfo(int ranking, int waveStep, int waveTime)
	{
		waveInfo.ranking = ranking;
		waveInfo.RecordStep = waveStep;
		waveInfo.RecordSec = waveTime;
	}
	
	public void SetWaveClearInfo(int waveStep, int waveTime)
	{
		if (waveInfo.RecordStep < waveStep)
		{
			waveInfo.RecordStep = waveStep;
			waveInfo.RecordSec = waveTime;
		}
		else if (waveInfo.RecordStep == waveStep)
		{
			if (waveInfo.RecordSec > waveTime)
				waveInfo.RecordSec = waveTime;
		}
	}
	
	public void SetMasteryInfo(int id, int curLevel)
	{
		CharMasteryData data = null;
		if (masteryDatas.ContainsKey(id) == true)
			data = masteryDatas[id];
		else
		{
			data = new CharMasteryData();
			data.masteryID = id;
			data.masteryLevel = 0;
			
			masteryDatas.Add(id, data);
		}
		
		if (data != null)
			data.masteryLevel = curLevel;
	}
	
	public void ResetMastery()
	{
		masteryDatas.Clear();
	}
	
	public void ResetAwakeingSkill()
	{
		foreach(var temp in awakenSkillDatas)
		{
			temp.Value.masteryLevel = 0;
		}
	}
	
	public int GetAvailableAwakenPoint()
	{
		int availPoint = this.baseInfo.APoint + this.baseInfo.APointGift;
		int buyPoint = Mathf.Min(this.baseInfo.ALimitBuyCount, this.baseInfo.ABuyCount);
		int usedPoint = 0;
		foreach(var temp in awakenSkillDatas)
		{
			usedPoint += temp.Value.masteryLevel;
		}
		
		return availPoint + buyPoint - usedPoint;
	}
	
	public CharPrivateData()
	{
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		int maxTheme = 4;
		int stagePerTheme = 20;
		if (stringValueTable != null)
		{
			maxTheme = stringValueTable.GetData("MaxTheme");
			stagePerTheme = stringValueTable.GetData("StageCountPerTheme");
		}
		
		maxStage = maxTheme * stagePerTheme;
		
		for (int stageType = 0; stageType < Game.maxStageType; ++stageType)
		{
			List<StageInfo> emptyList = new List<StageInfo>();
			for(int i = 0; i < maxStage - 1; ++i)
				emptyList.Add(new StageInfo());
			
			StageInfo startStage = new StageInfo();
			startStage.stageInfo = StageButton.eStageButton.Normal;
			emptyList.Insert(0, startStage);
			
			List<StageInfo> initList = new List<StageInfo>();
			initList.AddRange(emptyList);
			stageInfos.Add(stageType, initList);
		}
	}
	
	public void SetAwakeningSkillData(int skillID, int level)
	{
		SetSkillData(awakenSkillDatas, skillID, level);
	}
	
	public SkillDBInfo ToSkillDBInfoFromAwakening()
	{
		return ToSkillDBInfo(awakenSkillDatas);
	}
	
	public int GetAwakeningLevel(int skillID)
	{
		return GetSkillLevel(awakenSkillDatas, skillID);
	}
	
	public void SetMasteryData(int masteryID, int level)
	{
		SetSkillData(masteryDatas, masteryID, level);
	}
	
	public SkillDBInfo ToSkillDBInfoFromMastery()
	{
		return ToSkillDBInfo(masteryDatas);
	}
	
	public int GetMasteryLevel(int masteryID)
	{
		return GetSkillLevel(masteryDatas, masteryID);
	}
	
	public int GetSkillLevel(Dictionary<int, CharMasteryData> list, int skillID)
	{
		CharMasteryData data = null;
		if (list.ContainsKey(skillID) == true)
			data = masteryDatas[skillID];
		
		int level = 0;
		if (data != null)
			level = data.masteryLevel;
		
		return level;
	}
		
	public void SetSkillData(Dictionary<int, CharMasteryData> list, int skillID, int level)
	{
		CharMasteryData data = null;
		if (list.ContainsKey(skillID) == false)
		{
			data = new CharMasteryData();
			list.Add(skillID, data);
		}
		else
			data = list[skillID];
		
		if (data != null)
		{
			data.masteryID = skillID;
			data.masteryLevel = level;
		}
	}
	
	public SkillDBInfo ToSkillDBInfo(Dictionary<int, CharMasteryData> list)
	{
		SkillDBInfo skillDBInfos = new SkillDBInfo();
		
		List<int> idList = new List<int>();
		List<int> lvList = new List<int>();
		foreach(var temp in list)
		{
			CharMasteryData data = temp.Value;
			
			idList.Add(data.masteryID);
			lvList.Add(data.masteryLevel);
		}
		
		skillDBInfos.IDs = idList.ToArray();
		skillDBInfos.Lvs = lvList.ToArray();
		
		return skillDBInfos;
	}
	
	public void InitEquipData()
	{
		equipData.Clear();
		
		int nEquipCount = 14;
		for (int i = 0; i < nEquipCount; ++ i)
		{
			EquipInfo info = new EquipInfo();
			
			info.item = null;
			info.SetSlotType(i);
			
			equipData.Add(info);
		}
	}
	
	public void InitMapInfo()
	{
		StageInfo stageInfo = null;
		List<StageInfo> modeStageInfos = null;
		
		for (int stageType = 0; stageType < Game.maxStageType; ++stageType)
		{
			int openStage = baseInfo.StageIndex[stageType];
			modeStageInfos = stageInfos[stageType];
			for (int stageIndex = 0; stageIndex < openStage; ++stageIndex)
			{
				stageInfo = GetStageInfo(modeStageInfos, stageIndex);
				if (stageInfo != null)
					stageInfo.stageInfo = StageButton.eStageButton.Clear;
			}
			
			stageInfo = GetStageInfo(modeStageInfos, openStage);
			if (stageInfo != null)
				stageInfo.stageInfo = StageButton.eStageButton.Normal;
		}
	}
	
	public bool SetModeStageClear(int stageType, int index, bool bClear)
	{
		bool isFirstClear = false;
		
		List<StageInfo> modeStageInfos = null;
		if (stageInfos.ContainsKey(stageType) == true)
			modeStageInfos = stageInfos[stageType];
		
		StageInfo curInfo = GetStageInfo(modeStageInfos, index);
		if (curInfo != null)
		{
			if (curInfo.stageInfo != StageButton.eStageButton.Clear)
				isFirstClear = true;
			
			curInfo.stageInfo = StageButton.eStageButton.Clear;
		}
		
		StageInfo nextInfo = GetStageInfo(modeStageInfos, index + 1);
		if (nextInfo != null)
		{
			if (nextInfo.stageInfo == StageButton.eStageButton.Locked)
				nextInfo.stageInfo = StageButton.eStageButton.Normal;
		}
		else
		{
			if (stageType == 0)
			{
				modeStageInfos = stageInfos[stageType + 1];
				
				nextInfo = GetStageInfo(modeStageInfos, 0);
				if (nextInfo != null && nextInfo.stageInfo == StageButton.eStageButton.Locked)
					nextInfo.stageInfo = StageButton.eStageButton.Normal;
			}
		}
		
		return isFirstClear;
	}
	
	public StageInfo GetStageInfo(List<StageInfo> stageList, int index)
	{
		StageInfo info = null;
		int nCount = stageList != null ? stageList.Count : 0;
		
		if (index >= 0 && index < nCount)
			info = stageList[index];
		
		return info;
	}
	
	public void AddEquipItem(int slotIndex, ItemDBInfo dbInfo)
	{
		Item item = Item.CreateItem(dbInfo);
		
		AddEquipItem(slotIndex, item);
	}
	
	public void AddEquipItem(int slotIndex, Item item)
	{
		EquipInfo equipInfo = new EquipInfo();
		equipInfo.SetSlotType(slotIndex);
		equipInfo.item = item;
		
		this.equipData[slotIndex] = equipInfo;
	}
	
	public void SetCostumeSetItem(CostumeSetItem costumeSet)
	{
		this.costumeSetItem = costumeSet;
	}
	
	public void RemoveEquipItem(int slotIndex, ItemDBInfo dbInfo)
	{
		int nCount = this.equipData.Count;
		if (slotIndex >= 0 && slotIndex > nCount)
		{
			this.costumeSetItem = null;
		}
		else
		{
			EquipInfo equipInfo = new EquipInfo();
			equipInfo.SetSlotType(slotIndex);
			equipInfo.item = null;
			
			this.equipData[slotIndex] = equipInfo;
		}
	}
	
	public Item GetEquipItem(int slotIndex)
	{
		EquipInfo equipInfo = null;
		
		int nCount = this.equipData.Count;
		if (slotIndex >= 0 && slotIndex < nCount)
			equipInfo = this.equipData[slotIndex];
		
		Item item = equipInfo != null ? equipInfo.item : null;
		return item;
	}
	
	public void SetEquipItemList(int Count, EquipItemDBInfo [] Info)
	{
		for (int i = 0; i < Count; ++i)
		{
			EquipItemDBInfo equipDBInfo = Info[i];
			
			int slotIndex = equipDBInfo.SlotIndex;
			
			EquipInfo equipInfo = new EquipInfo();
			equipInfo.SetSlotType(slotIndex);
			equipInfo.item = Item.CreateItem(equipDBInfo);
			equipInfo.slotIndex = slotIndex;
			
			this.equipData[slotIndex] = equipInfo;
		}
	}
	
	public EquipItemDBInfo[] ToEquipItemDBInfos()
	{
		EquipItemDBInfo[] equipItemDBInfos = null;
		
		List<EquipItemDBInfo> tempList = new List<EquipItemDBInfo>();
		int nCount = this.equipData.Count;
		for (int index = 0; index < nCount; ++index)
		{
			EquipInfo equipInfo = equipData[index];
			EquipItemDBInfo equipDbInfo = new EquipItemDBInfo();
			
			if (equipInfo != null && equipDbInfo != null)
			{
				equipDbInfo.SlotIndex = index;
				
				if (equipInfo.item != null && equipInfo.item.itemInfo != null)
				{
					equipDbInfo.ID = equipInfo.item.itemInfo.itemID;
					equipDbInfo.Grade = (int)equipInfo.item.itemGrade;
					equipDbInfo.Reinforce = equipInfo.item.reinforceStep;
					equipDbInfo.Rate = equipInfo.item.itemRateStep;
					equipDbInfo.Count = equipInfo.item.itemCount;
					equipDbInfo.UID = equipInfo.item.uID;
					equipDbInfo.Exp = (int)equipInfo.item.itemExp;
				}
				
				tempList.Add(equipDbInfo);
			}
		}
		
		equipItemDBInfos = tempList.ToArray();
		
		return equipItemDBInfos;
	}

    public void SetCostumeSetData(string uid, int id)
    {
        CostumeItemDBInfo info = new CostumeItemDBInfo();
        info.ID = id;
        info.UID = uid;


        CostumeSetItem newCostumeSet = null;
        if (info != null && info.ID > 0 /*&& costumeSetItemInfo.UID != ""*/)
        {
            newCostumeSet = CostumeSetItem.Create(info.ID, info.UID);
        }

        SetCostumeSetItem(newCostumeSet);

    }

    public void SetEquipData(EquipItemDBInfo equipDBInfo)
    {
        EquipInfo equipInfo = new EquipInfo();
        equipInfo.SetSlotType(equipDBInfo.SlotIndex);
        equipInfo.item = Item.CreateItem(equipDBInfo);
        equipInfo.slotIndex = equipDBInfo.SlotIndex;

        if (equipDBInfo.SlotIndex >= 0 && equipDBInfo.SlotIndex < this.equipData.Count)
            this.equipData[equipDBInfo.SlotIndex] = equipInfo;
    }

	public void SetEquipItemList(int Count, EquipItemDBInfo [] Info, CostumeItemDBInfo costumeSetItemInfo)
	{
		for (int index = 0; index < Count; ++index)
		{
			EquipItemDBInfo equipDBInfo = Info[index];
			
			int slotIndex = equipDBInfo.SlotIndex;
			
			if (slotIndex == 100)
			{
				if (costumeSetItemInfo == null)
				{
					costumeSetItemInfo = new CostumeItemDBInfo();
					costumeSetItemInfo.ID = equipDBInfo.ID;
					costumeSetItemInfo.UID = equipDBInfo.UID;
				}
				
				continue;
			}
			
			EquipInfo equipInfo = new EquipInfo();
			equipInfo.SetSlotType(slotIndex);
			equipInfo.item = Item.CreateItem(equipDBInfo);
			equipInfo.slotIndex = slotIndex;
			
			if (slotIndex >= 0 && slotIndex < this.equipData.Count)
				this.equipData[slotIndex] = equipInfo;
		}
		
		CostumeSetItem newCostumeSet = null;
		if (costumeSetItemInfo != null && costumeSetItemInfo.ID > 0 /*&& costumeSetItemInfo.UID != ""*/)
		{
			newCostumeSet = CostumeSetItem.Create(costumeSetItemInfo.ID, costumeSetItemInfo.UID);
		}
		
		SetCostumeSetItem(newCostumeSet);
	}
	
	public System.DateTime refreshStaminaExpireTime = System.DateTime.Now;
	public void SetStamina(int LeftTimeSec, int Cur)
	{
		this.baseInfo.StaminaCur = Cur;
		
		if (LeftTimeSec != 0)
		{
			this.baseInfo.StaminaLeftTimeSec = LeftTimeSec;
		
			refreshStaminaExpireTime = System.DateTime.Now;
			
			System.TimeSpan addSpan = Game.ToTimeSpan(LeftTimeSec);
			
			refreshStaminaExpireTime += addSpan;
		}
	}
	
	public void SetStamina(int LeftTimeSec, int Cur, int presentValue)
	{
		this.baseInfo.StaminaCur = Cur;
		this.baseInfo.StaminaPresent = presentValue;
		
		if (LeftTimeSec != 0)
		{
			this.baseInfo.StaminaLeftTimeSec = LeftTimeSec;
		
			refreshStaminaExpireTime = System.DateTime.Now;
			
			System.TimeSpan addSpan = Game.ToTimeSpan(LeftTimeSec);
			
			refreshStaminaExpireTime += addSpan;
		}
	}

	public void SetStaminaRefreshTime(int leftTimeSec)
	{
		if (leftTimeSec != 0)
		{
			this.baseInfo.StaminaLeftTimeSec = leftTimeSec;
		
			refreshStaminaExpireTime = System.DateTime.Now;
			
			System.TimeSpan addSpan = Game.ToTimeSpan(leftTimeSec);
			
			refreshStaminaExpireTime += addSpan;
		}
	}

	public void AddStamina(int val)
	{
		if (val < 0)
			return;
		
		this.baseInfo.StaminaCur += val;
	}

    public void SetPresentStamina(int val)
    {
        if (val < 0)
            return;

        this.baseInfo.StaminaPresent = val;
    }
	
	public void AddPresendStamina(int _value)
	{
		if (_value < 0)
			return;
		
		this.baseInfo.StaminaPresent += _value;
	}

    public void AddPresentAwakenPoint(int val)
    {
        if (val <= 0)
            return;

        this.baseInfo.APointGift += val;
		
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
		{
			player.lifeManager.awakeningLevelManager.giftPoint += val;
		}
    }
	
	public void SetPresentAwakenPoint(int val)
	{
		if (val <= 0)
            return;
		
		this.baseInfo.APointGift = val;
		
		PlayerController player = Game.Instance.player;
		if (player != null && player.lifeManager != null)
		{
			player.lifeManager.awakeningLevelManager.giftPoint = val;
		}
	}
}

public class EventShopInfoData
{
	public eCashEvent eventType;
	public int eventID = 0;
	
	public int limitCount = 0;
	public int buyCount = 0;
	
	public System.DateTime expireTime;
	
	public void SetCountInfo(int buyCount, int limitCount)
	{
        if (limitCount >= 0)
		    this.limitCount = limitCount;

        if (buyCount >= 0)
		    this.buyCount = buyCount;
	}
	
	public void SetLimitTimeInfo(int leftTime, System.DateTime nowTime)
	{
		if (leftTime >= 0)
			expireTime = nowTime + Game.ToTimeSpan(leftTime);
		else
			expireTime = System.DateTime.MinValue;
	}

    public void SetLimitTimeInfo(string expireTimeStr)
    {
        expireTime = System.Convert.ToDateTime(expireTimeStr);   
    }
}

public class SpecialEventInfo
{
	public System.DateTime eventStartTime;
	public System.DateTime eventEndTime;
	
	public string eventBannerURL = "";
	
	public int eventID;
}

public class CharInfoData
{
	public List<int> specialStageInfo = new List<int>();
	
	public int potion1;			//물약.
	public int potion2; 		//고기.
	public int potion1Present;	//물약 선물 받음. 선물 받은게 먼저 까이고, 0이 되면 potion1이 까임..
	public int potion2Present;	//고기 선물 받음.
	
	public int equipPotion1Count = 0;	//스테이지에 들고 들어 가는 물약 갯수.최대 9개.
	public int equipPotion2Count = 0;	//스테이지에 들고 들어 가는 고기 갯수.최대 9개.
	
	public System.DateTime GetBuffEndTime(TimeLimitBuffItemInfo.eTimeLimitBuffItemType type)
	{
		System.DateTime endTime = System.DateTime.MinValue;
		
		if (timeLimitBuffList != null)
		{
			System.DateTime nowTime = System.DateTime.Now;
		
			System.TimeSpan timeSpan;
			double maxValue = double.MinValue;
			double tempValue = 0;
			foreach(TimeLimitBuffInfo info in timeLimitBuffList)
			{
				if (info.itemType == type)
				{
					timeSpan = info.endTime - nowTime;
					
					tempValue = timeSpan.TotalSeconds;
					if (tempValue <= 0)
						continue;
					
					if (tempValue > maxValue)
					{
						maxValue = tempValue;
						endTime = info.endTime;
					}
				}
			}
		}
		
		return endTime;
	}
	
	public void AddTimeLimitBuff(System.DateTime endTime, AttributeValue.eAttributeType buffType, TimeLimitBuffItemInfo.eTimeLimitBuffItemType itemType, float buffValue)
	{
		TimeLimitBuffInfo newTimeLimtBuffInfo = new TimeLimitBuffInfo();
		newTimeLimtBuffInfo.endTime = endTime;
		newTimeLimtBuffInfo.buffType = buffType;
		newTimeLimtBuffInfo.itemType = itemType;
		newTimeLimtBuffInfo.buffValue = buffValue;
		
		timeLimitBuffList.Add(newTimeLimtBuffInfo);
	}
	public void CreateTimeLimitBuffList()
	{
		timeLimitBuffList = new List<TimeLimitBuffInfo>();
	}
	public void InitTimeLimitBuffList()
	{
		timeLimitBuffList.Clear();
	}
	public List<TimeLimitBuffInfo> timeLimitBuffList = null;
	
	public SpecialEventInfo specialEventInfo = null;
	
	public Dictionary<int, EventShopInfoData> eventShopInfos = new Dictionary<int, EventShopInfoData>();
	public void SetEventShopInfo(int eventID, EventShopInfoData newInfo)
	{
		if (eventShopInfos.ContainsKey(eventID) == true)
			eventShopInfos.Remove(eventID);
		
		eventShopInfos.Add(eventID, newInfo);
	}
	
	public EventShopInfoData GetEventShopInfo(int eventID)
	{
		EventShopInfoData info = null;
		if (eventShopInfos.ContainsKey(eventID) == true)
			info = eventShopInfos[eventID];
		
		return info;
	}
	
	public EventShopInfoData GetEventShopInfo(eCashEvent eventType)
	{
		EventShopInfoData info = null;
		foreach(var temp in eventShopInfos)
		{
			EventShopInfoData tempInfo = temp.Value;
			if (tempInfo != null && tempInfo.eventType == eventType)
			{
				info = tempInfo;
				break;
			}
		}
		return info;
	}
	
	public List<int> packageItems = new List<int>();
	public static int limitPackageItemCount = 3;
	public bool CheckPackageItem()
	{
		int nCount = packageItems.Count;
		return (nCount < CharInfoData.limitPackageItemCount);
	}

    public int packageItemLimit = 3;        // 스타터팩 구매제한 횟수.
    public void SetPackageItemLimit(int val)
    {
        packageItemLimit = val;
    }
	
	public void SetPackageItem(int itemID, int buyCount)
	{
        if (buyCount <= packageItemLimit)
		{
			if (packageItems.Contains(itemID) == false)
				packageItems.Add(itemID);
		}
		else
		{
			if (packageItems.Contains(itemID) == true)
				packageItems.Remove(itemID);
		}
	}
	
	public bool isTutorialComplete = false;
	
	public string NickName = "";
	
	public int gold_Value = 0;
	public int jewel_Value = 0;
	public int medal_Value = 0;
	
	public int gambleCoupon = 0;
	
	public int attandanceCheck = 0;
	public string gameReviewURL = "";

    public int ticket = 0;

	public CharPrivateData[] privateDatas = null;//new CharPrivateData[] { new CharPrivateData(), new CharPrivateData(), new CharPrivateData() };
	
	public void SetNickName(string nickName)
	{
		this.NickName = nickName;
		foreach(CharPrivateData data in privateDatas)
		{
			data.NickName = nickName;
		}
	}
	
	public CharPrivateData GetPrivateData(int index)
	{
		CharPrivateData data = null;
		if (index >= 0 && index < 3)
			data = privateDatas[index];
		
		return data;
	}
	
	public int CheckEmptyInventory()
	{
		int maxCount = baseItemSlotCount + expandNormalItemSlotCount;
		int useCount = 0;
		foreach(Item temp in inventoryNormalData)
		{
			if (temp != null && temp.itemInfo != null && temp.itemInfo.itemID > 0)
				useCount++;
		}
		
		int itemEmptyCount = Mathf.Max(0, maxCount - useCount);
		
		return itemEmptyCount;
		/*
		maxCount = baseItemSlotCount + expandMaterialItemSlotCount;
		useCount = 0;
		foreach(Item temp in inventoryMaterialData)
		{
			if (temp != null && temp.itemInfo != null && temp.itemInfo.itemID > 0)
				useCount++;
		}
		
		int materialEmptyCount = Mathf.Max(0, maxCount - useCount);
		
		return Mathf.Min(itemEmptyCount, materialEmptyCount);
		*/
	}
	
	public List<Item> stageRewardItems = new List<Item>();
	public List<Item> inventoryNormalData = new List<Item>();
	public List<Item> inventoryMaterialData = new List<Item>();
	public int baseItemSlotCount = 35;
	public int expandNormalItemSlotCount = 0;
	public int expandMaterialItemSlotCount = 0;
	
	public List<Item> inventoryCostumeData = new List<Item>();
	public List<CostumeSetItem> inventoryCostumeSetData = new List<CostumeSetItem>();
	
	public AchievementManager achievementManager = new AchievementManager();
	
	public void SetGold(int gold, int cash, int medal)
	{
		this.SetGold(gold, cash);
		if (medal >= 0)
			medal_Value = medal;
	}
	
	public void SetGold(int gold, int cash)
	{
		if (gold >= 0)
			gold_Value = gold;
		
		if (cash >= 0)
			jewel_Value = cash;
	}
	
	public void SetCharacterInfo(PacketCharacterInfo packet)
	{
		
	}
	
	public void Clear()
	{
		InitPrivateData();
		
		stageRewardItems.Clear();
		
		inventoryNormalData.Clear();
		inventoryCostumeData.Clear();
		inventoryMaterialData.Clear();
		inventoryCostumeSetData.Clear();
		
		gold_Value = jewel_Value = medal_Value = 0;		
		
		this.isTutorialComplete = false;
		this.packageItems.Clear();
	}
	
	public void InitPrivateData()
	{
		privateDatas = new CharPrivateData[] { new CharPrivateData(), new CharPrivateData(), new CharPrivateData() };
		
		int charIndex = 0;
		foreach(CharPrivateData data in privateDatas)
		{
			data.InitEquipData();
			
			data.playerClass = (GameDef.ePlayerClass)charIndex;
			
			++charIndex;
		}
		
		//InitSlots();
	}
	
	/*
	public void InitSlots()
	{
		int totalCount = baseItemSlotCount + expandNormalItemSlotCount;
		inventoryNormalData.Clear();
		for(int index = 0; index < totalCount; ++index)
		{
			inventoryNormalData.Add(null);
		}
		
		totalCount = baseItemSlotCount + expandMaterialItemSlotCount;
		this.inventoryMaterialData.Clear();
		for(int index = 0; index < totalCount; ++index)
		{
			inventoryMaterialData.Add(null);
		}
	}
	*/
	
	public int FindSlotIndex(Item item, CharPrivateData privateData, GameDef.eItemSlotWindow window)
	{
		int slotIndex = -1;
		int nCount = 0;
		int index = 0;
		
		switch(window)
		{
		case GameDef.eItemSlotWindow.Equip:
			nCount = privateData != null ? privateData.equipData.Count : 0;
			for (index = 0; index < nCount; ++index)
			{
				EquipInfo equipInfo = privateData.equipData[index];
				if (equipInfo == null)
					continue;
				
				if (equipInfo.item == item)
				{
					slotIndex = index;
					break;
				}
			}
			break;
		case GameDef.eItemSlotWindow.Inventory:
			nCount = inventoryNormalData.Count;
			for (index = 0; index < nCount; ++index)
			{
				Item temp = inventoryNormalData[index];
				if (temp == item)
				{
					slotIndex = index;
					break;
				}
			}
			break;
		case GameDef.eItemSlotWindow.Costume:
			nCount = inventoryCostumeData.Count;
			for (index = 0; index < nCount; ++index)
			{
				Item temp = inventoryCostumeData[index];
				if (temp == item)
				{
					slotIndex = index;
					break;
				}
			}
			break;
		}
		
		return slotIndex;
	}
	
	public Item ReinforceItem(CharPrivateData privateData, MaterialItemInfo [] MaterialItems, BaseTradeItemInfo ReinforceItem, int ReinforceStep)
	{
		Item resultItem = null;
		
		if (ReinforceItem != null)
		{
			switch(ReinforceItem.windowType)
			{
			case GameDef.eItemSlotWindow.Equip:
				EquipInfo equipInfo = null;
				if (privateData != null)
					equipInfo = privateData.equipData[ReinforceItem.slotIndex];
				
				resultItem = equipInfo != null ? equipInfo.item : null;
				break;
			case GameDef.eItemSlotWindow.Inventory:
				resultItem = this.inventoryNormalData[ReinforceItem.slotIndex];
				break;
			case GameDef.eItemSlotWindow.Costume:
				resultItem = this.inventoryCostumeData[ReinforceItem.slotIndex];
				break;
			}
		}
		
		for (int i = 0; i < MaterialItems.Length; ++i)
		{
			if (string.IsNullOrEmpty(MaterialItems[i].UID))
				break;
			
			if (MaterialItems[i].Count == 0)
				RemoveItemByUID(MaterialItems[i].UID, inventoryMaterialData);	
			else
				UpdateItemCount(MaterialItems[i].UID, MaterialItems[i].Count, inventoryMaterialData);	
		}
		
		return resultItem;
	}
	
	public Item ReinforceItem(CharPrivateData privateData, string[] delUIDs, BaseTradeItemInfo ReinforceItem, int ReinforceStep)
	{
		Item resultItem = null;
		
		if (ReinforceItem != null)
		{
			switch(ReinforceItem.windowType)
			{
			case GameDef.eItemSlotWindow.Equip:
				EquipInfo equipInfo = null;
				if (privateData != null)
					equipInfo = privateData.equipData[ReinforceItem.slotIndex];
				
				resultItem = equipInfo != null ? equipInfo.item : null;
				break;
			case GameDef.eItemSlotWindow.Inventory:
				resultItem = this.inventoryNormalData[ReinforceItem.slotIndex];
				break;
			case GameDef.eItemSlotWindow.Costume:
				resultItem = this.inventoryCostumeData[ReinforceItem.slotIndex];
				break;
			}
		}
		
		for (int i = 0; i < delUIDs.Length; ++i)
		{
			if (string.IsNullOrEmpty(delUIDs[i]))
				break;
			
			RemoveItemByUID(delUIDs[i], inventoryNormalData);
			RemoveItemByUID(delUIDs[i], inventoryMaterialData);
		}
		
		return resultItem;
	}
	
	public Item CompositionItem(CharPrivateData privateData, string deleteItemUID, string deleteMaterialUID, int deleteMaterialCount, BaseTradeItemInfo itemInfo, int itemGrade)
	{
		Item resultItem = null;
		
		if (itemInfo != null)
		{
			switch(itemInfo.windowType)
			{
			case GameDef.eItemSlotWindow.Equip:
				EquipInfo equipInfo = null;
				if (privateData != null)
					equipInfo = privateData.equipData[itemInfo.slotIndex];
				
				resultItem = equipInfo != null ? equipInfo.item : null;
				break;
			case GameDef.eItemSlotWindow.Inventory:
				resultItem = this.inventoryNormalData[itemInfo.slotIndex];
				break;
			case GameDef.eItemSlotWindow.Costume:
				resultItem = this.inventoryCostumeData[itemInfo.slotIndex];
				break;
			}
		}
		
		RemoveItemByUID(deleteItemUID, inventoryNormalData);
		
		if (deleteMaterialCount > 0)
		{
			int index = GetItemIndexByUID(deleteMaterialUID, inventoryMaterialData);
			Item oldItem = inventoryMaterialData[index];
			oldItem.itemCount = deleteMaterialCount;
		}
		else if (deleteMaterialCount == 0)
		{
			RemoveItemByUID(deleteMaterialUID, inventoryMaterialData);
		}
		
		return resultItem;
	}
	
	public int GetItemIndexByUID(string uID, List<Item> itemList)
	{
		int itemIndex = -1;
		int nCount = itemList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			Item item = itemList[index];
			
			if (item != null && item.uID == uID)
			{
				itemIndex = index;
				break;
			}
		}
		return itemIndex;
	}
	
	public int GetItemIndexByUID(string uID, List<CostumeSetItem> itemList)
	{
		int itemIndex = -1;
		int nCount = itemList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			CostumeSetItem item = itemList[index];
			
			if (item != null && item.UID == uID)
			{
				itemIndex = index;
				break;
			}
		}
		return itemIndex;
	}
	
	public void RemoveItemByUID(string uID, List<Item> itemList)
	{
		int slotIndex = GetItemIndexByUID(uID, itemList);
		if (slotIndex != -1)
		{
			int nCount = itemList.Count;
			if (slotIndex < 0 || slotIndex >= nCount)
				return;
			
			//Item oldItem = itemList[slotIndex];
			itemList[slotIndex] = null;
		}
	}
	
	public void UpdateItemCount(string UID, int ItemCount, List<Item> itemList)
	{
		int slotIndex = GetItemIndexByUID(UID, itemList);
		if (slotIndex != -1)
		{
			int nCount = itemList.Count;
			if (slotIndex < 0 || slotIndex >= nCount)
				return;
			
			Item oldItem = itemList[slotIndex];
			if (oldItem != null)
				oldItem.itemCount = ItemCount;
		}
	}
	
	public void AddItem(Item item)
	{
		if (item != null && item.itemInfo != null)
		{
			if (item.itemInfo.itemType == ItemInfo.eItemType.Potion_1 ||
				item.itemInfo.itemType == ItemInfo.eItemType.Potion_2)
				return;
		}
		
		int maxCount = baseItemSlotCount + expandNormalItemSlotCount;
		
		int index = GetItemIndexByUID(item != null ? item.uID : "", inventoryNormalData);
		if (index != -1)
		{
			Item oldItem = inventoryNormalData[index];
			if (oldItem != null && item != null)
			{
				oldItem.itemCount = item.itemCount;
			}
		}
		else
		{
			int emptyIndex = GetEmptyIndex(inventoryNormalData);
			if (emptyIndex == -1)
				inventoryNormalData.Add(item);
			else if (emptyIndex < maxCount)
				inventoryNormalData[emptyIndex] = item;
		}
	}
	
	public void AddCostume(Item item)
	{
		int index = GetItemIndexByUID(item != null ? item.uID : "", inventoryCostumeData);
		if (index != -1)
		{
			Item oldItem = inventoryCostumeData[index];
			if (oldItem != null && item != null)
			{
				oldItem.itemCount = item.itemCount;
			}
		}
		else
			inventoryCostumeData.Add(item);
	}
	
	public void AddMaterial(Item item)
	{
		int maxCount = baseItemSlotCount + expandMaterialItemSlotCount;
		
		int index = GetItemIndexByUID(item != null ? item.uID : "", inventoryMaterialData);
		if (index != -1)
		{
			Item oldItem = inventoryMaterialData[index];
			if (oldItem != null && item != null)
			{
				oldItem.itemCount = item.itemCount;
			}
		}
		else
		{
			int emptyIndex = GetEmptyIndex(inventoryMaterialData);
			if (emptyIndex == -1)
				inventoryMaterialData.Add(item);
			else if (emptyIndex < maxCount)
				inventoryMaterialData[emptyIndex] = item;
		}
	}
	
	public void AddItemOnEmptySlotOrEnd(Item item)
	{
		if (item == null || item.itemInfo == null)
			return;
		
		if (item.itemInfo.itemType == ItemInfo.eItemType.Potion_1 ||
			item.itemInfo.itemType == ItemInfo.eItemType.Potion_2)
			return;
			
		ItemInfo.eItemType itemType = item.itemInfo.itemType;
		
		List<Item> itemList = null;
		
		switch(itemType)
		{
		case ItemInfo.eItemType.Material:
		case ItemInfo.eItemType.Material_Compose:
			itemList = this.inventoryMaterialData;
			break;
		case ItemInfo.eItemType.Costume_Back:
		case ItemInfo.eItemType.Costume_Body:
		case ItemInfo.eItemType.Costume_Head:
			itemList = this.inventoryCostumeData;
			break;
		default:
			itemList = this.inventoryNormalData;
			break;
		}
		
		if (itemList != null)
		{
			int addIndex = GetEmptyIndex(itemList);
			if (addIndex == -1)
				itemList.Add(item);
			else
				itemList[addIndex] = item;
		}
	}
	
	public int GetEmptyIndex(List<Item> list)
	{
		int index = -1;
		int nCount = list.Count;
		
		Item item = null;
		for (int i = 0; i < nCount; ++i)
		{
			item = list[i];
			if (item == null)
			{
				index = i;
				break;
			}
		}
		
		return index;
	}
	
	public void AddCostumeSetItem(CostumeSetItem costumeSet)
	{
		int index = GetItemIndexByUID(costumeSet != null ? costumeSet.UID : "", inventoryCostumeSetData);
		if (index != -1)
		{
			//CostumeSetItem oldItem = inventoryCostumeSetData[index];
			//if (oldItem != null && costumeSet != null)
			//	oldItem.itemCount = costumeSet
		}
		else
			inventoryCostumeSetData.Add(costumeSet);
	}
	
	public void RemoveItemByIndex(int slotIndex)
	{
		RemoveItemByIndex(slotIndex, "");
	}
	
	public void RemoveItemByIndex(int slotIndex, string uID)
	{
		int nCount = inventoryNormalData.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
		{
			string msg = string.Format("Remove Item error ..slotIndex[{0}]", slotIndex);
			Logger.DebugLog(msg);
			return;
		}
		
		Item item = inventoryNormalData[slotIndex];
		if (uID != "" &&
			item != null && item.uID != uID)
		{
			string msg = string.Format("Remove Item error ..mismatch UID[{0}] - [{1}]", item.uID, uID);
			Logger.DebugLog(msg);
			return;
		}
		
		inventoryNormalData[slotIndex] = null;
	}
	
	public void RemoveCostumeByIndex(int slotIndex)
	{
		RemoveCostumeByIndex(slotIndex, "");
	}
	public void RemoveCostumeByIndex(int slotIndex, string uID)
	{
		int nCount = inventoryCostumeData.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
		{
			string msg = string.Format("Remove Item error ..slotIndex[{0}]", slotIndex);
			Logger.DebugLog(msg);
			return;
		}
		
		Item item = inventoryCostumeData[slotIndex];
		if (uID != "" &&
			item != null && item.uID != uID)
		{
			string msg = string.Format("Remove Item error ..mismatch UID[{0}] - [{1}]", item.uID, uID);
			Logger.DebugLog(msg);
			return;
		}
		
		inventoryCostumeData[slotIndex] = null;
	}
	
	public void RemoveCostumeSetByIndex(int slotIndex)
	{
		RemoveCostumeSetByIndex(slotIndex, "");
	}
	public void RemoveCostumeSetByIndex(int slotIndex, string uID)
	{
		int nCount = this.inventoryCostumeSetData.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
		{
			string msg = string.Format("Remove Item error ..slotIndex[{0}]", slotIndex);
			Logger.DebugLog(msg);
			return;
		}
		
		CostumeSetItem item = inventoryCostumeSetData[slotIndex];
		if (uID != "" &&
			item != null && item.UID != uID)
		{
			string msg = string.Format("Remove Item error ..mismatch UID[{0}] - [{1}]", item.UID, uID);
			Logger.DebugLog(msg);
			return;
		}
		
		inventoryCostumeSetData.RemoveAt(slotIndex);
	}
	
	public void RemovMaterialItemByIndex(int slotIndex)
	{
		RemovMaterialItemByIndex(slotIndex, "");
	}
	public void RemovMaterialItemByIndex(int slotIndex, string uID)
	{
		int nCount = this.inventoryMaterialData.Count;
		if (slotIndex < 0 || slotIndex >= nCount)
		{
			string msg = string.Format("Remove Item error ..slotIndex[{0}]", slotIndex);
			Logger.DebugLog(msg);
			return;
		}
		
		Item item = inventoryMaterialData[slotIndex];
		if (uID != "" &&
			item != null && item.uID != uID)
		{
			string msg = string.Format("Remove Item error ..mismatch UID[{0}] - [{1}]", item.uID, uID);
			Logger.DebugLog(msg);
			return;
		}
		
		inventoryMaterialData[slotIndex] = null;
	}
	
	public void RemoveEquipItem(int charIndex, int slotIndex)
	{
		CharPrivateData privateData = privateDatas[charIndex];
		if (privateData != null)
		{
			privateData.RemoveEquipItem(slotIndex, null);
		}
	}
	
	public Vector3 dropGold = Vector3.zero;
	public Vector3 dropAddGold = Vector3.zero;
	public List<GainItemInfo> dropItems = new List<GainItemInfo>();
	public List<GainItemInfo> dropMaterialItems = new List<GainItemInfo>();
	public List<GainItemInfo> dropEventItems = new List<GainItemInfo>();
	public int addSkillPoint = 0;
	public int stageRewardItemID = -1;
	public long stageClearExp = 0;
	
	/*
	public List<GainItemInfo> useItems = new List<GainItemInfo>();
	public void AddUseItem(int itemID, int count)
	{
		GainItemInfo useItem = null;
		foreach(GainItemInfo temp in useItems)
		{
			if (temp.ID == itemID)
			{
				useItem = temp;
				break;
			}
		}
		
		if (useItem == null)
		{
			useItem = new GainItemInfo();
			useItem.ID = itemID;
			useItem.Count = 1;
			
			useItems.Add(useItem);
		}
		else
		{
			useItem.Count += count;
		}
	}
	*/
	
	public int usedPotion1 = 0;
	public int usedPotion2 = 0;
	
	public void InitDropInfos()
	{
		dropGold = Vector3.zero;
		dropAddGold = Vector3.zero;
		
		dropItems.Clear();
		dropMaterialItems.Clear();
		
		dropEventItems.Clear();
		
		addSkillPoint = 0;
		stageRewardItemID = -1;
		stageClearExp = 0;
		
		//useItems.Clear();
		usedPotion1 = usedPotion2 = 0;
	}
	
	public void AddDropItem(BaseDropItem item)
	{
		if (item == null)
			return;
		
		int sameItemIndex = -1;
		int nCount = 0;
		
		switch(item.dropType)
		{
		case BaseDropItem.eDropType.Coin:
			dropGold.x += item.dropInfoValue;
			dropAddGold.x += item.addValue;
			
			PlayerController player = Game.Instance.player;
			if (player != null && player.lifeManager != null)
				player.lifeManager.ApplyDropGold(item.dropInfoValue + item.addValue);
			
			break;
		case BaseDropItem.eDropType.Jewel:
			dropGold.y += item.dropInfoValue;
			break;
		case BaseDropItem.eDropType.Potion:
		case BaseDropItem.eDropType.Item:
			UpdateDropInfo(dropItems, item);
			break;
		case BaseDropItem.eDropType.MaterialItem:
			UpdateDropInfo(dropMaterialItems, item);
			break;
		case BaseDropItem.eDropType.EventDropItem:
			UpdateDropInfo(dropEventItems, item);
			break;
		}
	}
	
	public void UpdateDropInfo(List<GainItemInfo> list, BaseDropItem dropItem)
	{
		int nCount = list.Count;
		int sameItemIndex = -1;
		
		for(int index = 0; index < nCount; ++index)
		{
			GainItemInfo temp = list[index];
			if (temp.ID == dropItem.dropInfoValue)
			{
				sameItemIndex = index;
				break;
			}
		}
		
		if (sameItemIndex == -1)
		{
			GainItemInfo newInfo = new GainItemInfo();
			newInfo.ID = dropItem.dropInfoValue;
			newInfo.Count = 1;
			
			list.Add(newInfo);
		}
		else
		{
			GainItemInfo temp = list[sameItemIndex];
			temp.Count += 1;
		}
	}
	
	public MonsterPictureBook monsterPictureBook = new MonsterPictureBook();
	/*
	public void OpenMonsterPictureBook(int stageID, bool isHardMode)
	{
		if (monsterPictureBook == null)
			return;
		
		TableManager tableManager = TableManager.Instance;
		StageTable stageTable = tableManager != null ? tableManager.stageTable : null;
		
		if (stageTable != null)
		{
			StageTableInfo stageInfo = stageTable.GetData(stageID);
			List<string> mobFaceList = null;
			if (stageInfo != null)
			{
				if (isHardMode == true)
					mobFaceList = stageInfo.mobFaceListForHard;
				else
					mobFaceList = stageInfo.mobFaceListForNormal;
			}
			
			if (mobFaceList != null)
			{
				foreach(string idStr in mobFaceList)
				{
					int monsterID = int.Parse(idStr);
					monsterPictureBook.OpenMonsterPictureBook(monsterID, isHardMode);
				}
			}
		}
	}
	*/
	
	public ItemPictureBook itemPictureBook = null;//new ItemPictureBook();
	
	public void ResetNewItems()
	{
		foreach(Item item in inventoryNormalData)
		{
			if (item != null)
				item.IsNewItem = false;
		}
		
		foreach(Item item in inventoryMaterialData)
		{
			if (item != null)
				item.IsNewItem = false;
		}
		
		foreach(Item item in inventoryCostumeData)
		{
			if (item != null)
				item.IsNewItem = false;
		}
	}
	
	public bool CheckNewItems()
	{
		bool hasNewItem = false;
		
		foreach(Item item in inventoryNormalData)
		{
			if (item != null && item.IsNewItem == true)
			{
				hasNewItem = true;
				break;
			}
		}
		
		if (hasNewItem == true)
			return hasNewItem;
		
		foreach(Item item in inventoryMaterialData)
		{
			if (item != null && item.IsNewItem == true)
			{
				hasNewItem = true;
				break;
			}
		}
		
		if (hasNewItem == true)
			return hasNewItem;
		
		foreach(Item item in inventoryCostumeData)
		{
			if (item != null && item.IsNewItem == true)
			{
				hasNewItem = true;
				break;
			}
		}
		
		return hasNewItem;
	}
}

public class GameOption
{
	public static bool bgmToggle = true;
	public static bool effectToggle = true;
	public static bool noticeToggle = true;
	public static bool faceToggle = true;
}

public class Game : MonoSingleton<Game>
{
    public System.DateTime now;     // 서버의 현재시간이다. 클라이언트의 현재시간을 가져오지 않고 이값을 이용. procedure에서 server_time을 갱신해준다. 현재 enter_town에서 갱신함.

	//public static Game sInstance = null;
	public static int maxStageType = 3;	//stringValueTable...
	
	public delegate void OnEvent();
	public OnEvent onPause = null;
	public OnEvent onInputPause = null;
	
	public GameDef.ePlayerClass playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
	public PlayerController player = null;
	
	
	public StageManager stageManager = null;
	
	public CharInfoData charInfoData = null;//new CharInfoData();
	public List<ReinforceInfo> selectedReinforceInfos = null;//new List<ReinforceInfo>();
	public TowerInfo selectedTowerInfo = null;
	
	public bool isDebugTestMode = false;
	
	public AndroidManager androidManager = null;
	public ClientConnector connector;
	public IPacketSender packetSender;
	
	void Awake()
	{
		Logger.DebugLog("Game Awake");
		androidManager = gameObject.AddComponent<AndroidManager>();
		
		//RequestSettingInfos();
	}
	
	public void RequestSettingInfos()
	{
		if (androidManager != null)
			androidManager.RequestSettingInfos();
	}
	
	public AndroidManager AndroidManager
	{
		get { 
			return this.androidManager;
		}
		
		
	}
	
	public ClientConnector Connector
    {
        get { 
			return this.connector; 
		}
        set { 
			this.connector = value; 
		}
    }
	
	public IPacketSender PacketSender
    {
        get { 
			return this.packetSender; 
		}
        set { this.packetSender = value; }
    }
	
	public static System.TimeSpan ToTimeSpan(int sec)
	{
		int day = sec / 86400;
		int hour = sec % 86400 / 3600;
		int minite = sec % 86400 % 3600 / 60;
		int second = sec % 86400 % 3600 % 60;
		
		System.TimeSpan timeSpan = new System.TimeSpan(day, hour, minite, second);
		
		return timeSpan;
	}
	
	public static System.TimeSpan ToTimeSpan(double sec)
	{
		int day = (int)(sec / 86400);
		int hour = (int)(sec % 86400 / 3600);
		int minite = (int)(sec % 86400 % 3600 / 60);
		int second = (int)(sec % 86400 % 3600 % 60);
		
		System.TimeSpan timeSpan = new System.TimeSpan(day, hour, minite, second);
		
		return timeSpan;
	}
	
	public static System.TimeSpan ToTimeSpan(float sec)
	{
		int intValue = (int)sec;
		
		float temp = sec - (float)intValue;
		int milliseconds = (int)(temp * 1000.0f);
		
		int day = intValue / 86400;
		int hour = intValue % 86400 / 3600;
		int minite = intValue % 86400 % 3600 / 60;
		int second = intValue % 86400 % 3600 % 60;
		
		System.TimeSpan timeSpan = new System.TimeSpan(day, hour, minite, second, milliseconds);
		
		return timeSpan;
	}
	
	/*
	public void CopyCharInfoData(GameDef.ePlayerClass _class, LifeManager lifeManager)
	{
		return;
		
		int index = (int)_class;
		
		InventoryManager invenManager = null;
		EquipManager equipManager = null;
		MasteryManager masteryManager = null;
		
		if (lifeManager != null)
		{
			invenManager = lifeManager.inventoryManager;
			equipManager = lifeManager.equipManager;
			masteryManager = lifeManager.masteryManager;
		}
		
		if (invenManager != null)
		{
			CopyToItems(invenManager.itemList, charInfoData.inventoryNormalData);
			CopyToItems(invenManager.costumeList, charInfoData.inventoryCostumeData);
		}
		
		if (equipManager != null)
		{
			CopyToEquipItems(equipManager.equipItems, charInfoData.privateDatas[index].equipData);
		}
		
		if (masteryManager != null)
		{
			CopyToMasterySaveData(masteryManager.masteries, charInfoData.privateDatas[index].masteryInfos);
		}
		
		if (lifeManager != null)
		{
			charInfoData.goldInfo = lifeManager.ownGoldValue;
			
			CharacterDBInfo charDBInfo = charInfoData.privateDatas[index].baseInfo;
			
			charDBInfo.Exp = (int)lifeManager.expValue;
			charDBInfo.SkillPoint = (int)lifeManager.skillPoint;
			charDBInfo.StaminaCur = (int)lifeManager.staminaValue.x;
		}
		
		XMLTools.SaveCharacterInfos(charInfoData);
	}
	*/

	/*
	public void CopyToMasterySaveData(List<MasteryInfo> ownMasteryList, List<MasterySaveData> saveDataList)
	{
		saveDataList.Clear();
		foreach(MasteryInfo info in ownMasteryList)
		{
			MasterySaveData newData = new MasterySaveData();
			newData.tableID = info.tableID;
			newData.level = info._level;
			
			saveDataList.Add(newData);
		}
	}
	*/

	public void CopyToItems(List<Item> origList, List<Item> toList)
	{
		toList.Clear();
		foreach(Item item in origList)
		{
			toList.Add(item);
		}
	}
	
	public void CopyToEquipItems(List<EquipInfo> origList, List<EquipInfo> toList)
	{
		toList.Clear();
		
		foreach(EquipInfo data in origList)
		{
			toList.Add(data);
		}
	}
	
	/*
	public static Game Instance
	{
		get 
		{
			if (sInstance == null)
			{
				sInstance = new Game();
				
				sInstance.PreInitData();
				
#if UNITY_EDITOR
		sInstance.isDebugTestMode = false;
#else
		sInstance.isDebugTestMode = false;
#endif
			}
			
			return sInstance;
		}
	}
	*/
	
	public LoginInfo loginInfo = null;
	
	public NetworkManager networkManager = null;
	public string networkPrefabPath = "Network/NetworkManager";
	
	public void ResetInitData()
	{
		charInfoData = null;
		selectedReinforceInfos = null;
		noticeItems = null;
	}
	
	public void PreInitData()
	{
		LoadGameOption();
		
		EncryptedPlayerPrefs.keys=new string[5];
        EncryptedPlayerPrefs.keys[0]="23Wrudre";
        EncryptedPlayerPrefs.keys[1]="SP9DupHa";
        EncryptedPlayerPrefs.keys[2]="frA5rAS3";
        EncryptedPlayerPrefs.keys[3]="tHat2epr";
        EncryptedPlayerPrefs.keys[4]="jaw3eDAs";
		
		GetLoginData();
	}
	
	public void InitCharInfoData()
	{
		if (charInfoData == null)
			charInfoData = new CharInfoData();
		
		if (selectedReinforceInfos == null)
			selectedReinforceInfos = new List<ReinforceInfo>();
		
		if (noticeItems == null)
			noticeItems = new List<NoticeItem>();
		
		if (charInfoData != null)
		{
			charInfoData.Clear();
		
			if (charInfoData.achievementManager != null)
				charInfoData.achievementManager.InitData();
			
			if (charInfoData.monsterPictureBook != null)
				charInfoData.monsterPictureBook.InitData();
			
			if (charInfoData.itemPictureBook != null)
				charInfoData.itemPictureBook.InitData();
		}
	}
	
	
	public void CreateNetwork(bool Hive5)
	{
		if (networkManager == null)
		{
			Logger.DebugLog("Network Create.....");
			//networkManager = ResourceManager.CreatePrefab<NetworkManager>(networkPrefabPath, null, Vector3.zero);
			networkManager = ResourceManager.CreatePrefabByResource<NetworkManager>(networkPrefabPath, null, Vector3.zero);
			Logger.DebugLog(networkManager.ToString());
		}
		
		if (networkManager != null)
			networkManager.InitNetwork(Hive5);

		AndroidManager.CallUntiyNetworkReady();		
	}
	
	public void GetLoginData()
	{
		Logger.DebugLog("GetLoginData");
		if (loginInfo == null)
			loginInfo = new LoginInfo();
		
		string loginID = PlayerPrefs.GetString("LoginID");
		string pass = EncryptedPlayerPrefs.GetStringByEncrypted("Password");
		
		string loginDateInfo = PlayerPrefs.GetString("LoginDate");
		int eulaChecked = PlayerPrefs.GetInt("EULA_Checked");
		long userID = GetLong("UserID");
		int charIndex = PlayerPrefs.GetInt("CharIndex");
		int accountType = PlayerPrefs.GetInt("AccountType");
		
		int eula_version = PlayerPrefs.GetInt("EULA_Version");
		string eula_url = PlayerPrefs.GetString("EULA_URL");
		string private_url = PlayerPrefs.GetString("Private_URL");
		
		if (loginInfo != null)
		{
			loginInfo.loginID = loginID;
			loginInfo.pass = pass;
			loginInfo.acctountType = (AccountType)accountType;
			
			loginInfo.userID = userID;
			loginInfo.charIndex = charIndex;
			
			if (loginDateInfo != null && loginDateInfo.Length > 0)
				loginInfo.loginDate = System.DateTime.Parse(loginDateInfo);
			
			loginInfo.eula_Checked = eulaChecked == 1;
			loginInfo.eula_version = eula_version;
			loginInfo.eula_url = eula_url;
			loginInfo.private_url = private_url;
		}
	}
	
	public void ResetLoginInfo(bool bReset)
	{
		if (bReset == false)
		{
			//시스템 에러인 경우에도 로그인 정보는 그대로 두고, 비번만 제거..
			if (loginInfo != null)
				loginInfo.pass = "";
		}
		else
		{
			loginInfo = null;
		}
		
		SaveLoginData();
		GetLoginData();
	}
	
	public void SaveLoginData()
	{
		if (loginInfo == null)
		{
			PlayerPrefs.DeleteKey("LoginID");
			EncryptedPlayerPrefs.DeleteKey("Password");
			PlayerPrefs.DeleteKey("LoginDate");
			PlayerPrefs.DeleteKey("EULA_Checked");
			PlayerPrefs.DeleteKey("UserID");
			PlayerPrefs.DeleteKey("CharIndex");
			PlayerPrefs.DeleteKey("AccountType");
			
			PlayerPrefs.DeleteKey("EULA_Version");
			PlayerPrefs.DeleteKey("EULA_URL");
			PlayerPrefs.DeleteKey("Private_URL");
		}
		else
		{
			PlayerPrefs.SetString("LoginID", loginInfo.loginID);
			EncryptedPlayerPrefs.SetStringByEncrypted("Password", loginInfo.pass);
			SetLong("UserID", loginInfo.userID);
			PlayerPrefs.SetInt("CharIndex", loginInfo.charIndex);
			PlayerPrefs.SetInt("AccountType", (int)loginInfo.acctountType);
			
			string loginDateStr = "";
			//if (loginInfo.loginDate != null)
				loginDateStr = loginInfo.loginDate.ToString();
			PlayerPrefs.SetString("LoginDate", loginDateStr);
			
			PlayerPrefs.SetInt("EULA_Checked", loginInfo.eula_Checked == true ? 1 : 0);
			PlayerPrefs.SetInt("EULA_Version", loginInfo.eula_version);
			PlayerPrefs.SetString("EULA_URL", loginInfo.eula_url);
			PlayerPrefs.SetString("Private_URL", loginInfo.private_url);
		}
	}
	
    public static long GetLong(string key)
    {
        string longAsString = PlayerPrefs.GetString(key);

        long value = -1;

        long.TryParse(longAsString, out value);

        return value;
    }

    public static void SetLong(string key, long value)
    {
        PlayerPrefs.SetString(key, value.ToString());
    }


	public int pauseCount = 0;
	public bool Pause
	{
		get { return pauseCount > 0; }
		set 
		{
			if (value == true)
			{
				if (onPause != null)
					onPause();
				if (onInputPause != null)
					onInputPause();
				
				pauseCount++;
			}
			else
				pauseCount--;
		}
	}
	
	public int inputPauseCount = 0;
	public bool InputPause
	{
		get { return inputPauseCount > 0; }
		set
		{
			if (value == true)
			{
				if (onInputPause != null)
					onInputPause();
				
				inputPauseCount++;
			}
			else
				inputPauseCount--;
		}	
	}
	
	public void ResetPause()
	{
		pauseCount = inputPauseCount = 0;
	}
	
	public CharPrivateData GetCharPrivateData(GameDef.ePlayerClass _class)
	{
		int index = (int)_class;
		CharPrivateData privateData = charInfoData != null ? charInfoData.GetPrivateData(index) : null;
		
		return privateData;
	}
	
	//public List<Item> stageRewardItemList = new List<Item>();
	public void InitStageRewardItemList(GameDef.ePlayerClass _class)
	{
		if (charInfoData != null)
			charInfoData.stageRewardItems.Clear();
	}
	
	public void AddStageRewardItem(GameDef.ePlayerClass _class, Item item)
	{
		if (charInfoData != null)
			charInfoData.stageRewardItems.Add(item);
	}
	
	public void AddExp(GameDef.ePlayerClass _class, System.Int64 addExp)
	{
		int index = (int)_class;
		CharPrivateData privateData = charInfoData.GetPrivateData(index);
		
		if (privateData != null)
			privateData.baseInfo.ExpValue += (System.Int64)addExp;
	}
	
	public void AddStageClearExp(GameDef.ePlayerClass _class, System.Int64 addExp)
	{
		int index = (int)_class;
		CharPrivateData privateData = charInfoData.GetPrivateData(index);
		
		if (privateData != null)
			privateData.stageClearExp += addExp;
	}
	
	public void CreateCharacter(Vector3 startPos)
	{
		if (charInfoData == null)
		{
			InitCharInfoData();
		}
		
		if (player != null && player.classType != playerClass)
		{
			GameObject.DestroyObject(player.gameObject, 0.2f);
			player = null;
		}
		
		if (player != null)
			return;
		
		string path = "NewAsset/Character/";
		string fileName = "";
		
		CharPrivateData privateData = null;
		//playerClass = GameDef.ePlayerClass.CLASS_WIZARD;
		int index = -1;
		bool abilityFullCheck = false;
		switch(playerClass)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			fileName = "Warrior";
			index = (int)playerClass;
			abilityFullCheck = false;
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			fileName = "Assassin";
			index = (int)playerClass;
			abilityFullCheck = true;
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			fileName = "Wizard";
			index = (int)playerClass;
			abilityFullCheck = true;
			break;
		}
		
		if (index != -1)
			privateData = charInfoData.GetPrivateData(index);
		
		GameObject resObj = ResourceManager.LoadPrefab(path + fileName);
		if (resObj != null)
		{
			resObj.transform.position = startPos;
			
			GameObject obj = (GameObject)GameObject.Instantiate(resObj);
			
			if (obj != null)
			{
				player = obj.GetComponent<PlayerController>();
				
				if (player != null && player.lifeManager != null)
				{
					InventoryManager invenManager = player.lifeManager.inventoryManager;
					EquipManager equipManager = player.lifeManager.equipManager;
					//MasteryManager masteryManager = player.lifeManager.masteryManager;
					MasteryManager_New masteryManager = player.lifeManager.masteryManager_New;
					AwakeningLevelManager awakeningLevelManager = player.lifeManager.awakeningLevelManager;
					
					if (invenManager != null && charInfoData != null)
					{
						invenManager.SetInvenItemData(charInfoData.inventoryNormalData);
						invenManager.SetInvenCostumeData(charInfoData.inventoryCostumeData);
					}
					
					long expValue = privateData.baseInfo.ExpValue + privateData.stageClearExp;
					privateData.stageClearExp = 0L;
					player.lifeManager.SetExp(expValue);
					
					if (equipManager != null && privateData != null)
						equipManager.SetEquipItemData(privateData.equipData, privateData.costumeSetItem);
					
					if (awakeningLevelManager != null && privateData != null)
					{
						awakeningLevelManager.updateAwakenLevel = new AwakeningLevelManager.UpdateAwakenLevel(player.UpdateAwakenLevel);
						
						awakeningLevelManager.SetInfo(privateData.baseInfo.AExp, privateData.baseInfo.ALevel, 
														privateData.baseInfo.APoint, privateData.baseInfo.APointGift, privateData.baseInfo.ALimitBuyCount, privateData.baseInfo.ABuyCount);
						awakeningLevelManager.ApplyLevelInfos(privateData.awakenSkillDatas);
					}
					
					if (masteryManager != null && privateData != null)
						masteryManager.ApplyMasteryLevelInfos(privateData.masteryDatas);
					
					player.lifeManager.ApplyTimeLimitBuff(charInfoData.timeLimitBuffList);
					
					player.lifeManager.skillPoint = privateData.baseInfo.SkillPoint;
					//player.lifeManager.ownGoldValue = charInfoData.goldInfo;
					player.lifeManager.staminaValue.x = privateData.baseInfo.StaminaCur;
					player.lifeManager.staminaValue.y = privateData.baseInfo.StaminaMax;
					
					
					//????�� ?�보???�기??..
					player.myRankValue = privateData.arenaInfo.rankType;
					
					if (stageManager != null)
					{
						switch(stageManager.StageType)
						{
						case StageManager.eStageType.ST_WAVE:
						case StageManager.eStageType.ST_FIELD:
						case StageManager.eStageType.ST_EVENT:
							foreach(ReinforceInfo info in selectedReinforceInfos)
							{
								foreach(ReinforceAttributeInfo attInfo in info.attributeInfoList)
								{
									player.lifeManager.attributeManager.AddValueRate(attInfo.type, attInfo.attributeValue);
								}
							}
							break;
						}
						
						if (abilityFullCheck == true)
							player.AbilityFull();
						
						player.lifeManager.attributeManager.HpFull();
					}
				}
			}
		}
	}
	
	
	public GameDef.ePlayerClass arenaClassType = GameDef.ePlayerClass.CLASS_WARRIOR;
	public PlayerController arenaPlayer = null;
	public void CreateArenarCharacter(Transform trans)
	{
		string path = "NewAsset/ArenaMonster/";
		string fileName = "";
		
		if (this.arenaTargetInfo == null)
			return;
		
		arenaClassType = (GameDef.ePlayerClass)arenaTargetInfo.baseInfo.CharacterIndex;
		//arenaClassType = GameDef.ePlayerClass.CLASS_ASSASSIN;
		bool abilityFullCheck = false;
		switch(arenaClassType)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			fileName = "ArenaWarrior";
			abilityFullCheck = false;
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			fileName = "ArenaAssassin";
			abilityFullCheck = true;
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			fileName = "ArenaWizard";
			abilityFullCheck = true;
			break;
		}
		
		GameObject resObj = ResourceManager.LoadPrefab(path + fileName);
		if (resObj != null)
		{
			GameObject obj = (GameObject)GameObject.Instantiate(resObj);
			
			if (obj != null)
			{
				obj.transform.position = trans.position;
				
				if (arenaPlayer != null)
				{
					GameObject.DestroyObject(arenaPlayer.gameObject, 0.0f);
					arenaPlayer = null;
				}
				
				arenaPlayer = obj.GetComponent<PlayerController>();
				
				if (arenaPlayer != null && arenaPlayer.lifeManager != null)
				{
					//InventoryManager invenManager = arenaPlayer.lifeManager.inventoryManager;
					EquipManager equipManager = arenaPlayer.lifeManager.equipManager;
					MasteryManager_New masteryManager = arenaPlayer.lifeManager.masteryManager_New;
					AwakeningLevelManager awakeningLevelManager = arenaPlayer.lifeManager.awakeningLevelManager;
					
					if (arenaTargetInfo != null)
					{
						long expValue = arenaTargetInfo.baseInfo.ExpValue;
						arenaPlayer.lifeManager.SetExp(expValue);
						
						if (equipManager != null)
							equipManager.SetEquipItemData(arenaTargetInfo.equipData, arenaTargetInfo.costumeSetItem);
						
						arenaPlayer.myRankValue = arenaTargetInfo.RankType;
						
						if (awakeningLevelManager != null && arenaTargetInfo != null)
						{
							awakeningLevelManager.updateAwakenLevel = new AwakeningLevelManager.UpdateAwakenLevel(player.UpdateAIAwakenLevel);
							
							awakeningLevelManager.SetInfo(arenaTargetInfo.baseInfo.AExp, arenaTargetInfo.baseInfo.ALevel, 
															arenaTargetInfo.baseInfo.APoint, arenaTargetInfo.baseInfo.APointGift, arenaTargetInfo.baseInfo.ALimitBuyCount, arenaTargetInfo.baseInfo.ABuyCount);
							awakeningLevelManager.ApplyLevelInfos(arenaTargetInfo.awakenSkillDatas);
						}
						
						if (masteryManager != null)
							masteryManager.ApplyMasteryLevelInfos(arenaTargetInfo.masteryDatas);
					}
					
					if (abilityFullCheck == true)
						arenaPlayer.AbilityFull();
					arenaPlayer.lifeManager.attributeManager.HpFull();
				}
				else
				{
					
				}
			}
		}
	}
	
	static public void RerangeItemList(List<Item> tempList)
	{
		tempList.Sort(SortByNull);
	}
	
	static public int SortByNull (Item a, Item b)
	{
		int aValue = -1;
		int bValue = -1;
		
		if (a != null && a.itemInfo != null)
			aValue = (int)a.itemInfo.itemType;
		if (b != null && b.itemInfo != null)
			bValue = (int)b.itemInfo.itemType;
		
		if (aValue == bValue)
		{
			if (a != null && a.itemInfo != null)
				aValue = a.itemInfo.itemID;
			if (b != null && b.itemInfo != null)
				bValue = b.itemInfo.itemID;
		}
		
		if (aValue == bValue)
		{
			if (a != null && a.itemInfo != null)
				aValue = a.itemGrade;
			if (b != null && b.itemInfo != null)
				bValue = b.itemGrade;
		}
		
		if (aValue == bValue)
		{
			if (a != null && a.itemInfo != null)
				aValue = a.reinforceStep;
			if (b != null && b.itemInfo != null)
				bValue = b.reinforceStep;
		}
		
		if (aValue == bValue)
		{
			if (a != null && a.itemInfo != null)
				aValue = a.itemCount;
			if (b != null && b.itemInfo != null)
				bValue = b.itemCount;
		}
		
		return bValue - aValue;
	}
	
	public static int SortReinforceItem(Item a, Item b)
	{
		uint aValue = 0;
		uint bValue = 0;
		
		if (a != null)
			aValue = a.GetItemExp();
		if (b != null)
			bValue = b.GetItemExp();
		
		return (int)(bValue - aValue);
	}
	
	
	public CharPrivateData arenaTargetInfo = null;
	public long arenaTargetUserIndexID = -1;
	public void SetArenaPlayerInfo(PacketArenaMatchingTarget packet)
	{
		TableManager tableManager = TableManager.Instance;
		CharExpTable awakenExpTable = tableManager != null ? tableManager.awakenExpTable : null;
		
		if (arenaTargetInfo != null)
			arenaTargetInfo = null;

        long hive5TargetUserId = 0;
        long.TryParse(packet.targetUserId, out hive5TargetUserId);

        arenaTargetUserIndexID = hive5TargetUserId == 0 ? packet.UserIndexID : hive5TargetUserId;
		
		arenaTargetInfo = new CharPrivateData();
		arenaTargetInfo.InitEquipData();
		arenaTargetInfo.RankType = packet.TargetRankType;

        arenaTargetInfo.platform = packet.targetUserPlatform;
		arenaTargetInfo.baseInfo.CharacterIndex = packet.CharacterIndex;
		arenaTargetInfo.baseInfo.ExpValue = packet.TargetExp;
		arenaTargetInfo.baseInfo.AExp = packet.targetAwakenExp;
		if (awakenExpTable != null)
			arenaTargetInfo.baseInfo.ALevel = awakenExpTable.GetLevel(arenaTargetInfo.baseInfo.AExp);
		
		arenaTargetInfo.NickName = packet.TargetNickName;
		
		arenaTargetInfo.arenaInfo.rankType = packet.TargetRankType;
		arenaTargetInfo.arenaInfo.groupRanking = packet.TargetGroupRanking;
		
		int equipCount = packet.TargetEquipInfos.Length;
		arenaTargetInfo.SetEquipItemList(equipCount, packet.TargetEquipInfos, packet.costumeSetItem);
		
		int nCount = 0;
		int skillID = 0;
		int skillLv = 0;
			
		if (packet.TargetSkillInfo != null)
		{
			nCount = Mathf.Min(packet.TargetSkillInfo.IDs.Length, packet.TargetSkillInfo.Lvs.Length);
			for (int index = 0; index < nCount; ++index)
			{
				skillID = packet.TargetSkillInfo.IDs[index];
				skillLv = packet.TargetSkillInfo.Lvs[index];
				
				arenaTargetInfo.SetMasteryData(skillID, skillLv);
			}
		}
		
		if (packet.awakenSkillInfo != null)
		{
			nCount = Mathf.Min(packet.awakenSkillInfo.IDs.Length, packet.awakenSkillInfo.Lvs.Length);
			skillID = 0;
			skillLv = 0;
			for (int index = 0; index < nCount; ++index)
			{
				skillID = packet.awakenSkillInfo.IDs[index];
				skillLv = packet.awakenSkillInfo.Lvs[index];
				
				arenaTargetInfo.SetAwakeningSkillData(skillID, skillLv);
			}
		}
	}
	
	public List<Header> rewardPacketList = new List<Header>();
	public void AddReward(Header packet)
	{
		rewardPacketList.Add(packet);
	}
	
	
	public static PlayerController CreateDummyArenaCharacter(CharPrivateData privateData, Transform targetPos)
	{
		PlayerController player = null;
		
		string path = "NewAsset/ArenaMonster/";
		string fileName = "";
		
		if (privateData == null)
			return null;
		
		GameDef.ePlayerClass classType = (GameDef.ePlayerClass)privateData.baseInfo.CharacterIndex;
		switch(classType)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			fileName = "ArenaWarrior";
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			fileName = "ArenaAssassin";
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			fileName = "ArenaWizard";
			break;
		}
		
		GameObject resObj = ResourceManager.LoadPrefab(path + fileName);
		if (resObj != null)
		{
			GameObject obj = (GameObject)GameObject.Instantiate(resObj);
			
			if (obj != null)
			{
				if (targetPos != null)
					obj.transform.position = targetPos.position;
				
				player = obj.GetComponent<PlayerController>();
				
				if (player != null && player.lifeManager != null)
				{
					//InventoryManager invenManager = player.lifeManager.inventoryManager;
					EquipManager equipManager = player.lifeManager.equipManager;
					MasteryManager_New masteryManager = player.lifeManager.masteryManager_New;
					AwakeningLevelManager awakeningManager = player.lifeManager.awakeningLevelManager;
					
					long expValue = privateData.baseInfo.ExpValue;
					player.lifeManager.SetExp(expValue);
					
					if (equipManager != null && privateData != null)
						equipManager.SetEquipItemData(privateData.equipData, privateData.costumeSetItem);
					
					if (awakeningManager != null && privateData != null)
					{
						awakeningManager.SetInfo(privateData.baseInfo.AExp, privateData.baseInfo.ALevel, 
														privateData.baseInfo.APoint, privateData.baseInfo.APointGift, privateData.baseInfo.ALimitBuyCount, privateData.baseInfo.ABuyCount);
						awakeningManager.ApplyLevelInfos(privateData.awakenSkillDatas);
					}
					
					if (masteryManager != null && privateData != null)
						masteryManager.ApplyMasteryLevelInfos(privateData.masteryDatas);
					
					player.isAIMode = false;
				}
			}
		}
		
		return player;
	}
	
	public List<MailInfo> postItemList = new List<MailInfo>();
	public int postItemCount = 0;
	public System.DateTime postUpdateTime;

	public void AddMailInfos(MailInfo[] mailInfos)
	{
		if (mailInfos == null)
			return;
		
		postUpdateTime = System.DateTime.Now;
		foreach(MailInfo info in mailInfos)
			postItemList.Add(info);
	}
	
	public void ResetPostUpdateTime()
	{
		postUpdateTime = System.DateTime.MinValue;
	}
	
	public void RemoveReadPost()
	{
		List<MailInfo> readList = new List<MailInfo>();
		
		int nCount = postItemList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			MailInfo info = postItemList[index];
			if (info != null && info.bOpened == 1)
				readList.Add(info);
		}
		
		foreach(MailInfo readMail in readList)
		{
			postItemList.Remove(readMail);
			postItemCount--;
		}
		postItemCount = Mathf.Max(postItemCount, postItemList.Count);
	}
		
	////////////////////////////////////////////////////////////////////////////////////////
	//Friend List 관리..
	public void ResetFriendUpdateTime()
	{
		myFriendUpdateTime = acceptFriendUpdateTime = recommandFriendUpdateTime = System.DateTime.MinValue;
	}
	
	public List<FriendSimpleInfo> recommandFriendList = new List<FriendSimpleInfo>();
	public System.DateTime recommandFriendUpdateTime;
	public void AddRecommandFirendList(FriendSimpleInfo[] infoList)
	{
		if (infoList == null)
			return;
		
		recommandFriendList.Clear();
		
		recommandFriendUpdateTime = System.DateTime.Now;
		foreach(FriendSimpleInfo info in infoList)
			recommandFriendList.Add(info);
	}
	
	public void RemoveRecommandFriend(string userID)
	{
		long tempID = long.Parse(userID);
		foreach(FriendSimpleInfo info in recommandFriendList)
		{
			if (info.UserID == tempID)
			{
				recommandFriendList.Remove(info);
				break;
			}
		}
	}
	public void RemoveRecommandFriend(long friendID)
	{
		foreach(FriendSimpleInfo info in recommandFriendList)
		{
			if (info.UserID == friendID)
			{
				recommandFriendList.Remove(info);
				break;
			}
		}
	}
	
	public List<FriendSimpleInfo> acceptFriendList = new List<FriendSimpleInfo>();
	public System.DateTime acceptFriendUpdateTime;
	public void AddAcceptFriendList(FriendSimpleInfo[] infoList)
	{
		if (infoList == null)
			return;
		
		acceptFriendList.Clear();
		
		acceptFriendUpdateTime = System.DateTime.Now;
		foreach(FriendSimpleInfo info in infoList)
			acceptFriendList.Add(info);
	}
	
	public void RemoveAcceptFriend(string friendID)
	{
		
	}
	public void RemoveAcceptFriend(long friendID)
	{
		foreach(FriendSimpleInfo info in acceptFriendList)
		{
			if (info.UserID == friendID)
			{
				acceptFriendList.Remove(info);
				break;
			}
		}
	}
	
	public List<FriendInfo> myFriendList = new List<FriendInfo>();
	public System.DateTime myFriendUpdateTime;
	public void AddMyFriendList(FriendInfo[] infoList)
	{
		if (infoList == null)
			return;
		
		myFriendList.Clear();
		
		myFriendUpdateTime = System.DateTime.Now;
		foreach(FriendInfo info in infoList)
			myFriendList.Add(info);
	}
	public void AddMyFriendList(FriendInfo newInfo)
	{
		if (newInfo == null)
			return;
		
		//myFriendUpdateTime = System.DateTime.Now;
		
		bool isAlready = false;
		foreach(FriendInfo tempInfo in myFriendList)
		{
			if (tempInfo.UserID == newInfo.UserID)
			{
				isAlready = true;
				break;
			}
		}
		
		if (isAlready == false)
			myFriendList.Add(newInfo);
	}
	
	public void RemoveFriend(long friendID)
	{
		foreach(FriendInfo info in myFriendList)
		{
			if (info.UserID == friendID)
			{
				myFriendList.Remove(info);
				break;
			}
		}
	}
	////////////////////////////////////////////////////////////////////////
	
	public Color itemAttPlusColor = Color.cyan;
	public Color itemAttMinusColor = Color.red;
	
	public void ApplyAchievement(Achievement.eAchievementType type, int ID, int count)
	{
		CharInfoData charData =this.charInfoData;
		int charIndex = -1;
		if (this.connector != null)
			charIndex = this.connector.charIndex;
		
		AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;
		
		if (achieveMgr != null)
			achieveMgr.ApplyAchievement(type, charIndex, ID, count);
	}
	
	public void ApplyAchievement(Achievement.eAchievementType type, int _value)
	{
		CharInfoData charData =this.charInfoData;
		int charIndex = -1;
		if (this.connector != null)
			charIndex = this.connector.charIndex;
		
		AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;
		
		if (achieveMgr != null)
			achieveMgr.ApplyAchievement(type, charIndex, _value);
	}
	
	public void SendUpdateAchievmentInfo()
	{
		CharInfoData charData =this.charInfoData;
		int charIndex = -1;
		if (this.connector != null)
			charIndex = this.connector.charIndex;
		
		AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;
		
		if (achieveMgr != null)
			achieveMgr.SendUpdateAchievementInfo(charIndex);
	}
	
	
	public void SetBossAppear(int bossID)
	{
        this.bossID = bossID;
	}

    public int appearBossID = -1;
	public int bossID = -1;
	public long bossIndex = -1;
	public bool isPhase2 = false;
	public int bossHP = 0;
	public int bossHPMax = 0;
    public string ownerPlatform;
    public string ownerPlatformUserID;
	
	public float damageBossRaid = 0.0f;
	public void ApplyBossRaidDamage(float attackValue, LifeManager actor)
	{
		if (actor != null && actor.isBossRaidMonster == true)
			damageBossRaid += attackValue;
	}
	
	public BaseMonster bossRaidMonster = null;
	public void CreateBossRaid(Transform trans1, Transform trans2)
	{
		TableManager tableManager = TableManager.Instance;
		BossRaidTable bossRaidTable = tableManager != null ? tableManager.bossRaidTable : null;
		
		if (bossRaidTable == null || bossID == -1)
			return;
		
		BossRaidData bossRaidData = bossRaidTable.GetData(bossID);
		if (bossRaidData == null)
			return;
		
		GameObject resObj = ResourceManager.LoadPrefab(bossRaidData.bossPrefabPath);
		if (resObj != null)
		{
			GameObject obj = (GameObject)GameObject.Instantiate(resObj);
			
			if (obj != null)
			{
				obj.transform.position = trans1.position;
				
				bossRaidMonster = obj.GetComponent<BaseMonster>();
				
				BaseMonster twinMonster = null;
				if (bossRaidMonster != null && bossRaidMonster.lifeManager != null)
				{
					SetBossRaidInfo(bossRaidMonster, this.bossHP, bossRaidData.maxHP, this.isPhase2);
					
					MonsterMiddleWizard middleWizard = bossRaidMonster.gameObject.GetComponent<MonsterMiddleWizard>();
					if (middleWizard != null)
					{
						GameObject twinObj = (GameObject)GameObject.Instantiate(resObj);
						if (twinObj != null)
						{
							twinObj.transform.position = trans2.position;
							
							twinMonster = twinObj.GetComponent<BaseMonster>();
						}
						
						SetBossRaidInfo(twinMonster, this.bossHP, bossRaidData.maxHP, this.isPhase2);
					}
					
					if (this.isPhase2 == false)
					{
						this.stageManager.DeactivateMonsterGenerator();
						this.stageManager.ActivateMonsterGenerator(bossRaidMonster.attributeTableID);
					}
					else
					{
						//bossRaidMonster.lifeManager.ActivatePhase2();
						bossRaidMonster.lifeManager.Invoke("ActivatePhase2", 0.5f);
						
						if (twinMonster != null)
							twinMonster.lifeManager.Invoke("ActivatePhase2", 0.5f);
					}
				}
			}
		}
	}
	
	public void SetBossRaidInfo(BaseMonster bossRaidMonster, int curHP, int maxHP, bool isPhase2)
	{
		if (bossRaidMonster == null)
			return;
		
		bossRaidMonster.lifeManager.isBossRaidMonster = true;
		
		AttributeValue hpMaxValue = bossRaidMonster.lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.HealthMax);
		if (hpMaxValue != null)
			hpMaxValue.baseValue = maxHP;
		
		bossRaidMonster.lifeManager.attributeManager.UpdateValue(hpMaxValue);
		
		AttributeValue hpValue = bossRaidMonster.lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
		if (hpValue != null)
			hpValue.baseValue = curHP;
		
		bossRaidMonster.lifeManager.attributeManager.UpdateValue(hpValue);
		
		bossRaidMonster.lifeManager.isPhase2 = isPhase2;
	}
	
	public PacketBossRaidEnd bossRaidEnd = null;
	
	public void ApplyIgnoreCollision(BaseMonster monster)
	{
		if (monster == null || monster.moveController == null)
			return;
		
		PlayerController player = this.player;
		if (player != null && player.moveController != null)
		{
			if (player.moveController.ignoreMonsterBody == true)
			{
				if (player.myInfo.myTeam != monster.myInfo.myTeam)
				{
					Physics.IgnoreCollision(player.collider, monster.collider, true);
				}
			}
		}
		
		player = this.arenaPlayer;
		if (player != null && player.moveController != null)
		{
			if (player.moveController.ignoreMonsterBody == true)
			{
				if (player.myInfo.myTeam != monster.myInfo.myTeam)
				{
					Physics.IgnoreCollision(player.collider, monster.collider, true);
				}
			}
		}
	}
	
	public EscortNPC escortNPC = null;
	
	public float volumeScale = 1.0f;
	public string stageName = "";
	public int stageIndex = 0;
	public int lastSelectStageType = 0;
	public float effectSoundScale = 0.5f;
	public float bgmVolume = 0.5f;
	
	public List<NoticeItem> noticeItems = null;//new List<NoticeItem>();
	public List<string> noticeInTown = new List<string>();//new List<NoticeItem>();
	
	public static bool CheckClass(ItemInfo.eClass itemLimitClass, GameDef.ePlayerClass playerClass)
	{
		if (itemLimitClass == ItemInfo.eClass.Common)
			return true;
		
		bool bCheck = false;
		switch(itemLimitClass)
		{
		case ItemInfo.eClass.Warrior:
			if (playerClass == GameDef.ePlayerClass.CLASS_WARRIOR)
				bCheck = true;
			break;
		case ItemInfo.eClass.Assassin:
			if (playerClass == GameDef.ePlayerClass.CLASS_ASSASSIN)
				bCheck = true;
			break;
		case ItemInfo.eClass.Wizard:
			if (playerClass == GameDef.ePlayerClass.CLASS_WIZARD)
				bCheck = true;
			break;
		}
		
		return bCheck;
	}
	
	private NetConfig.HostType testHostType = NetConfig.HostType.DevHost;
	public NetConfig.HostType TestHostType
	{
		set { testHostType = value; }
		get { return testHostType; }
	}
	
	public static bool isHive5 = false;
	public void InvokeXignCode(string data)
	{
		//Logger.DebugLog("InvokeXignCode:" + data);
		
		try
        {
			/*
			Game game = Game.Instance;
			if (game != null)
				game.CreateNetwork(Game.isHive5);
			*/
			
			AndroidReady info = null;
			
#if UNITY_EDITOR
			info = new AndroidReady();
			info.cookie = "";
			info.publisher = NetConfig.PublisherType.TStore;
			info.hostType = testHostType;
			info.netVersion = Version.NetVersion;
#else
			info = LitJson.JsonMapper.ToObject<AndroidReady>(data);
#endif		
			Version.NetVersion = info.netVersion;
			
			if (networkManager != null)
				networkManager.SetHostAndPublisherType(info.hostType, info.publisher);
			
			if (packetSender != null)
				packetSender.SendRequestServerChecking(info.cookie);
        }    
		catch (LitJson.JsonException e)
		{
			Logger.DebugLog (e.Message);
			// todo.
		}	
	}
	
	public void InvokeRegisterID(string regID)
	{
		Logger.DebugLog("InvokeRegisterID:" + regID);
		
		connector.gcmRegID = regID;
		
		if (packetSender != null)// && connector.UserIndexID > 0)
		{
			packetSender.SendRegisterGCMID();	
		}
	}
	
	public void InvokeRestartApp(string arg)
	{
		Logger.DebugLog("InvokeRestartApp");
		
		Resources.UnloadUnusedAssets();
		Application.LoadLevelAsync(0);
	}
	
	public void InvokeGoogleLoginInfo(string googleInfoJson)
	{
		LoginPage loginPage = GameUI.Instance.loginPage;
		if (loginPage == null)
			return;
			
		GoogleInfo info = LitJson.JsonMapper.ToObject<GoogleInfo>(googleInfoJson);
		
		Logger.DebugLog("InvokeGoogleLoginInfo:" + info.Email);
		Logger.DebugLog("InvokeGoogleLoginInfo:" + info.currentPersonName);
		
		if (packetSender != null)
			packetSender.SendLogin(info.Email, "", AccountType.GooglePlus);
		
	}
	
	public void InvokeTStorePaymentSucceed(string TStoreBuyItemString)
	{
	    try
        {
			TStoreCashItemInfo info = LitJson.JsonMapper.ToObject<TStoreCashItemInfo>(TStoreBuyItemString);
			
			Logger.DebugLog("InvokeTStorePaymentSucceed:" + info.TStoreProductCode);
			
			TableManager tableManager = TableManager.Instance;
			CashShopInfoTable cashShopInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
			
			CashItemInfo itemInfo = null;
			if (cashShopInfoTable != null)
				itemInfo = cashShopInfoTable.GetItemInfoByStoreItemCode(info.TStoreProductCode, (NetConfig.PublisherType)info.publisherType);
			
			if (itemInfo != null)
			{
				info.ItemID = itemInfo.ItemID;
                info.Price = (int)itemInfo.price.x;
				info.itemName = itemInfo.itemName;
			}
			else
			{
				string infoStr = string.Format("Not Found ... Code : {0} , publisher : {1}", info.TStoreProductCode, info.publisherType);
				Debug.Log(infoStr);
			}
			
			if (packetSender != null)
				packetSender.SendResponeBuyCashItem(info);
        }    
		catch (LitJson.JsonException e)
		{
			Logger.DebugLog (e.Message);
			// todo.
		}	
	}
	
	public void InvokeTStorePaymentFailed(string TStoreBuyItemString)
	{
	    try
        {
			TStoreCashItemInfo info = LitJson.JsonMapper.ToObject<TStoreCashItemInfo>(TStoreBuyItemString);
			
			TableManager tableManager = TableManager.Instance;
			CashShopInfoTable cashShopInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
			
			CashItemInfo itemInfo = null;
			if (cashShopInfoTable != null)
				itemInfo = cashShopInfoTable.GetItemInfoByStoreItemCode(info.TStoreProductCode, (NetConfig.PublisherType)info.publisherType);
			
			if (itemInfo != null)
			{
				info.ItemID = itemInfo.ItemID;
				
				info.itemName = itemInfo.itemName;
			}
			
			if (packetSender != null)
				packetSender.SendResponeBuyCashItemFailed(info);
        }    
		catch (LitJson.JsonException e)
		{
			Logger.DebugLog (e.Message);
			// todo.
		}
	}
	
	public EmptyLoadingPage loadingPage = null;
	
	public void LoadGameOption()
	{
		int toggle = PlayerPrefs.GetInt("BGM Toggle", 1);
		GameOption.bgmToggle = toggle == 1 ? true : false;
		
		toggle = PlayerPrefs.GetInt("Effect Toggle", 1);
		GameOption.effectToggle = toggle == 1 ? true : false;
		
		toggle = PlayerPrefs.GetInt("Notice Toggle", 1);
		GameOption.noticeToggle = toggle == 1 ? true : false;
		
		Logger.DebugLog(string.Format("LoadGameOption :: noticeToggle {0}", GameOption.noticeToggle.ToString()));
		
		toggle = PlayerPrefs.GetInt("Face Toggle", 1);
		GameOption.faceToggle = toggle == 1 ? true : false;
	}
	
	public void SaveGameOption()
	{
		PlayerPrefs.SetInt("BGM Toggle", GameOption.bgmToggle == true ? 1 : 0);
		PlayerPrefs.SetInt("Effect Toggle", GameOption.effectToggle == true ? 1 : 0);
		PlayerPrefs.SetInt("Notice Toggle", GameOption.noticeToggle == true ? 1 : 0);
		PlayerPrefs.SetInt("Face Toggle", GameOption.faceToggle == true ? 1 : 0);
	}
	
	public void BGMToggle(bool bToggle)
	{
		if (GameOption.bgmToggle != bToggle)
		{
			GameOption.bgmToggle = bToggle;
			
			if (bToggle == false)
			{
				AudioManager.StopBGM();
			}
			else
			{
				float bgmVolume = Game.Instance.bgmVolume;
				
				StageManager stageManager = Game.Instance.stageManager;
				if (stageManager != null)
					AudioManager.PlayBGM(stageManager.bgmClip, bgmVolume, 1.0f);
			}
		}
	}
	
	public void EffectSoundToggle(bool bToggle)
	{
		GameOption.effectToggle = bToggle;
	}
	
	public void NoticeToggle(bool bToggle)
	{
		GameOption.noticeToggle = bToggle;
	}
	
	public void FaceToggle(bool bToggle)
	{
		GameOption.faceToggle = bToggle;
	}
	
	public System.DateTime lastClickTime = System.DateTime.Now;
	public float keepAliveTime = 20.0f * 60.0f;
	private float keepAliveDelay = 20.0f * 60.0f;
	public void Update()
	{
		//UpdateKeepAlive();
		
		//UpdateInputCheck();
		
#if UNITY_EDITOR
		UpdateKeyEvent();	
#endif
		UpdateDelayCall();

        // todo.
        // this.now에 deltatime더해주어야함. 매번 update마다 할필요는없고 10간격이나 30초 간격이면 될것같음. 초단위로 필요한데가 있나?
	}
	
#if UNITY_EDITOR
	public void UpdateKeyEvent()
	{
		if (Input.GetKeyUp(KeyCode.Escape) == true)
		{
			Game.Instance.CloseWindow();
		}
	}
#endif
	
	
	public bool isResourceLoading = false;
	public void UpdateInputCheck()
	{
		int cnt = 0;
		
#if (UNITY_IPHONE || UNITY_ANDROID) && !UNITY_EDITOR
		cnt = Input.touchCount;
#else
		if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
			cnt = 1;
#endif
		
		System.DateTime nowTime = System.DateTime.Now;
		
		if (cnt > 0 ||
			Application.isLoadingLevel == true ||
			isResourceLoading == true)
		{
			lastClickTime = nowTime;
			
			//string infoStr = string.Format("touched.. {0}", lastClickTime.ToString());
			//Logger.DebugLog(infoStr);
		}
		
		System.TimeSpan delta = nowTime - lastClickTime;
		if (delta.TotalSeconds > keepAliveTime)
		{
			lastClickTime = nowTime;
			Logger.DebugLog("KeepAlive limit.....");
			Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.SessionExpired);
		}
	}
	
	public void UpdateKeepAlive()
	{
		keepAliveDelay -= Time.deltaTime;
		if (keepAliveDelay <= 0.0f)
		{
			IPacketSender sender = Game.Instance.PacketSender;
			if (sender != null)
				sender.SendKeepAlive();
			
			keepAliveDelay = keepAliveTime;
		}
	}
	
	public void CloseWindow()
	{
		bool bPopup = false;
		StageManager stageManager = Game.Instance.stageManager;
		if (stageManager != null)
		{
			switch(stageManager.StageType)
			{
			case StageManager.eStageType.ST_FIELD:
			case StageManager.eStageType.ST_EVENT:
			case StageManager.eStageType.ST_TUTORIAL:
			case StageManager.eStageType.ST_BOSSRAID:
			case StageManager.eStageType.ST_WAVE:
				UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
				
				bPopup = PopupCheck(uiRoot.gameObject);
				
				if (bPopup == false)
				{
					//Pause 클릭 구현..
					if (uiRoot != null)
					{
						PauseButton pauseButton = uiRoot.GetComponentInChildren<PauseButton>();
						if (pauseButton != null)
							pauseButton.OnOk(null);
					}
				}
				break;
			case StageManager.eStageType.ST_ARENA:
				break;
			case StageManager.eStageType.ST_TOWN:
				TownUI townUI = GameUI.Instance.townUI;
				if (townUI != null)
				{
					if (townUI.popupNode != null)
						bPopup = PopupCheck(townUI.popupNode.gameObject);
					
					if (bPopup == false)
					{
						PopupBaseWindow currentWindow = GameUI.Instance.currentWindow;
						if (currentWindow != null)
						{
							if (currentWindow.popupNode != null)
								bPopup = PopupCheck(currentWindow.popupNode.gameObject);
							
							if (bPopup == false)
								currentWindow.OnBack();
						}
						else
						{
							Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.KeyDownClose);
						}
					}
				}
				break;
			}
		}
		else
		{
			UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
			PopupCheck(uiRoot.gameObject);
		}
	}
	
	public bool PopupCheck(GameObject root)
	{
		bool bPopup = false;
		
		RevivalConfirmPopup revivalPopup = root.GetComponentInChildren<RevivalConfirmPopup>();
		if (revivalPopup != null)
		{
			bPopup = true;
			
			PopupBaseWindow cashBase = revivalPopup.popupNode.GetComponentInChildren<PopupBaseWindow>();
			BuyCompleteWindow tempBuyCompletePopup = revivalPopup.popupNode.GetComponentInChildren<BuyCompleteWindow>();
			BasePopup tempPopup = revivalPopup.gameObject.GetComponentInChildren<BasePopup>();
			
			if (tempBuyCompletePopup != null)
			{
				tempBuyCompletePopup.OnOK();
			}
			else if (tempPopup != null)
			{
				if (tempPopup.cancelButtonMessage != null)
					ProcessButtonMessage(tempPopup.cancelButtonMessage);
				else if (tempPopup.okButtonMessage != null)
					ProcessButtonMessage(tempPopup.okButtonMessage);
			}
			else if (cashBase != null)
			{
				cashBase.OnBack();
			}
			else
			{
				revivalPopup.OnBack();
			}
			
			return bPopup;
		}
		
		BasePopup basePopup = root.GetComponentInChildren<BasePopup>();
		NoticePopupWindow noticePopup = root.GetComponentInChildren<NoticePopupWindow>();
		PopupBaseWindow popupBase = root.GetComponentInChildren<PopupBaseWindow>();
		BuyCompleteWindow buyCompletePopup = root.GetComponentInChildren<BuyCompleteWindow>();
		
		if (popupBase != null)
		{
			LoadingPanel loadingPanel = root.GetComponentInChildren<LoadingPanel>();
			if (loadingPanel != null)
				return true;
		}
		
		BaseWeekRewardWindow rewardPopup = root.GetComponentInChildren<BaseWeekRewardWindow>();
		BaseReinforceWindow baseReinforce = root.GetComponentInChildren<BaseReinforceWindow>();
		
		if (buyCompletePopup != null)
		{
			bPopup = true;
			buyCompletePopup.OnOK();
		}
		else if (baseReinforce != null)
		{
			bPopup = true;
			
			baseReinforce.OnClose();
		}
		else if (basePopup != null)
		{
			bPopup = true;
			
			if (basePopup.cancelButtonMessage != null)
				ProcessButtonMessage(basePopup.cancelButtonMessage);
			else if (basePopup.okButtonMessage != null)
				ProcessButtonMessage(basePopup.okButtonMessage);
		}
		else if (noticePopup != null)
		{
			bPopup = true;
			
			if (noticePopup.buttonMessage != null)
				ProcessButtonMessage(noticePopup.buttonMessage);
		}
		else if (popupBase != null)
		{
			bPopup = true;
			
			popupBase.OnBack();
		}
		else if (rewardPopup != null)
		{
			bPopup = true;
			
			rewardPopup.OnOK();
		}
		
		
		return bPopup;
	}
	
	public void ProcessButtonMessage(UIButtonMessage buttonMessage)
	{
		if (buttonMessage != null && buttonMessage.target != null && buttonMessage.functionName != "")
			buttonMessage.target.SendMessage(buttonMessage.functionName, buttonMessage.gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	public void SetPlayerSuperArmorMode(PlayerController player)
	{
		if (player != null)
		{
			if (player.lifeManager != null)
				player.lifeManager.bSuperAmmor = true;
			if (player.buffManager != null)
				player.buffManager.Init();
		}
	}
	
	public int GetEventID()
	{
		int eventID = -1;
		SpecialEventInfo specialEventInfo = null;
		if (this.charInfoData != null)
			specialEventInfo = this.charInfoData.specialEventInfo;
		
		if (specialEventInfo != null)
		{
			System.DateTime nowTime = System.DateTime.Now;
			System.TimeSpan start = specialEventInfo.eventStartTime - nowTime;
			System.TimeSpan end = specialEventInfo.eventEndTime - nowTime;
			
			if (start.TotalSeconds < 0 && end.TotalSeconds > 0)
				eventID = specialEventInfo.eventID;
		}
		
		return eventID;
	}
	
	public class EventInfo
	{
		public CMSEventType eventType;
		public System.DateTime endTime;
		public int eventValue;

        public EventInfo(CMSEventType type, int leftSec, int _value)
		{
			eventType = type;
			endTime = System.DateTime.Now + Game.ToTimeSpan(leftSec);
			eventValue = _value;
		}
	}
	
	public Dictionary<CMSEventType, EventInfo> eventList = new Dictionary<CMSEventType, EventInfo>();
	public void AddEvent(CMSEventType type, int leftSec, int eventValue)
	{
		EventInfo eventInfo = null;
		if (eventList.ContainsKey(type) == false)
		{
			eventInfo = new EventInfo(type, leftSec, eventValue);
			eventList.Add(type, eventInfo);
		}
		else
		{
			eventInfo = eventList[type];
			eventInfo.endTime = System.DateTime.Now + Game.ToTimeSpan(leftSec);
			eventInfo.eventValue = eventValue;
		}
	}
	
	public EventInfo GetEventInfo(CMSEventType type)
	{
		EventInfo eventInfo = null;
		if (eventList.ContainsKey(type) == true)
		{
			eventInfo = eventList[type];
			
			System.TimeSpan timeSpan = eventInfo.endTime - System.DateTime.Now;
			if (timeSpan.TotalSeconds <= 0)
				eventInfo = null;
		}
		
		return eventInfo;
	}
	
	
	public void LoginKakaoComplete(string json)
	{
		var resultObj = LitJson.JsonMapper.ToObject<LitJson.JsonData>(json);
		string kakaoUserID = resultObj["user_id"].ToString();
		
		//메시지 블럭 여부..
		GameOption.noticeToggle = (bool)resultObj["message_blocked"] == false;
		
		Logger.DebugLog(string.Format("LocalUser info : {0}, msgBlock : {1}", json, GameOption.noticeToggle));
			
		if (loginInfo == null)
		{
			Logger.DebugLog(string.Format("LoginInfo Create....!!"));
			loginInfo = new LoginInfo();
		}
		
		loginInfo.loginID = kakaoUserID;
		loginInfo.acctountType = AccountType.Kakao;
		loginInfo.eula_Checked = true;
		
		SaveLoginData();
		
		LoginPage loginPage = GameUI.Instance.loginPage;
		if (loginPage != null)
			loginPage.OnLoginKakao(kakaoUserID);
	}
	
	public void LoginKakaoFailed(string json)
	{
		LoginPage loginPage = GameUI.Instance.loginPage;
		if (loginPage != null)
			loginPage.OnLoginError(0);
	}
	
	public void UpdateLocalUser(string json)
	{
		if (string.IsNullOrEmpty(json) == false)
		{
			var resultObj = LitJson.JsonMapper.ToObject<LitJson.JsonData>(json);
			string kakaoUserID = resultObj["user_id"].ToString();
			
			//메시지 블럭 여부..
			GameOption.noticeToggle = (bool)resultObj["message_blocked"] == false;
			
			Logger.DebugLog(string.Format("Update LocalUser info : {0}, msgBlock : {1}", json, GameOption.noticeToggle));
			
			OptionWindow optionWindow = GameUI.Instance.optionWindow;
			if (optionWindow != null && optionWindow.gameObject.activeInHierarchy == true)
				optionWindow.UpdateKakaoNotice();
		}
	}
	
	public List<KakaoFriendInfo> kakaoFriendList = new List<KakaoFriendInfo>();
	public List<string> kakaoInvitedIDs = new List<string>();
	public static int limitTodayInvites = 30;
	public int todayInvites = 0;
	public int totalInvites = 0;
	public void UpdateKakaoFriends(string json)
	{
		string infoStr = string.Format("UpdateFriends : {0}", json);
		Debug.Log(infoStr);
		
		kakaoFriendList.Clear();
		
		var resultObj = LitJson.JsonMapper.ToObject<LitJson.JsonData>(json);
		var friends_infos = resultObj["friends_info"];
		var app_friends_infos = resultObj["app_friends_info"];
		
		List<string> friends_user_ids = new List<string>();
		
		string user_id = "";
		string nickname = "";
		string friend_nickname = "";
		string profile_image_url = "";
		string message_blocked = "";
		string hashed_talk_user_id = "";
		string supported_device = "";
		
		if (app_friends_infos != null)
		{
			for (int i = 0; i < app_friends_infos.Count; i++) {
				var userJson = app_friends_infos[i];
				
				user_id = userJson["user_id"].ToString();
				nickname = userJson["nickname"].ToString();
				friend_nickname = userJson["friend_nickname"].ToString();
				profile_image_url = userJson["profile_image_url"].ToString();
				message_blocked = userJson["message_blocked"].ToString();
				hashed_talk_user_id = userJson["hashed_talk_user_id"].ToString();
				supported_device = "true";
					
				friends_user_ids.Add(user_id);
				
				KakaoFriendInfo friend_info = new KakaoFriendInfo(user_id, nickname, friend_nickname, profile_image_url, message_blocked, hashed_talk_user_id, supported_device);
				kakaoFriendList.Add(friend_info);
			}
		}
		
		if (friends_infos != null)
		{
			for (int i = 0; i < friends_infos.Count; i++) {
				var userJson = friends_infos[i];
				
				user_id = userJson["user_id"].ToString();
				nickname = userJson["nickname"].ToString();
				friend_nickname = userJson["friend_nickname"].ToString();
				profile_image_url = userJson["profile_image_url"].ToString();
				message_blocked = userJson["message_blocked"].ToString();
				hashed_talk_user_id = userJson["hashed_talk_user_id"].ToString();
				supported_device = (bool)userJson["supported_device"] == true ? "true" : "false";
				
				//friends_user_ids.Add(user_id);
				
				KakaoFriendInfo friend_info = new KakaoFriendInfo(user_id, nickname, friend_nickname, profile_image_url, message_blocked, hashed_talk_user_id, supported_device);
				kakaoFriendList.Add(friend_info);
			}
		}
		
		Game.Instance.PacketSender.UpdateKakaoFriends(friends_user_ids.ToArray());
	}

    public string GetKakaoProfileURL(string kakaoUserID)
    {
        string url = "";
        foreach (KakaoFriendInfo info in kakaoFriendList)
        {
            if (info.user_id == kakaoUserID)
            {
                url = info.profile_image_url;
                break;
            }
        }

        return url;
    }
	
	public bool CheckAlreadyInvite(string user_id)
	{
		if (todayInvites >= limitTodayInvites)
			return true;
		
		bool isInvited = false;
		
		foreach (string invited_id in kakaoInvitedIDs)
        {
            if (invited_id == user_id)
            {
                isInvited = true;
                break;
            }
        }
		
		return isInvited;
	}
	
	public bool CheckInviteCount()
	{
		return (Game.limitTodayInvites > this.todayInvites);
	}
	
	
	private List<string> delayCallList = new List<string>();
	public void AddDelayCall(string funcName)
	{
		delayCallList.Add(funcName);
	}
	
	private void UpdateDelayCall()
	{
		string funcName = "";
		if (delayCallList.Count > 0)
		{
			funcName = delayCallList[0];
			delayCallList.RemoveAt(0);
		}
		
		if (string.IsNullOrEmpty(funcName) == false)
			Invoke(funcName, 0.1f);
	}
	
	public void CheckNickName()
	{
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.CheckNickName();
	}
	
	public void GetUserInfo()
	{
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.GetUserInfo();
	}
	
	public static bool LitJsonTest = true;
	public void ChangeLitJsonTest(string arg)
	{
		Game.LitJsonTest = arg.Equals("true");
	}
	
	public void OnKakaoInviteComplete(string userid)
	{
		FriendWindow friendWindow = GameUI.Instance.friendWindow;
		if (friendWindow != null)
			friendWindow.OnCancelPopup(null);
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.SendRequsetInviteKakaoFriendByUserID(userid);
	}
	
	public int kakao_InviteFailedStringID = 1104;
	public int kakao_ExceedInviteStringID = 1100;
	public int kakao_InviteFailedByOtherStringID = 1101;
	public int kakao_Unspported_device_stringID = 1103;
	public int kakao_Message_Blocked_StringID = 1102;
	public void OnKakaoInviteFailed(string statusCodeStr)
	{
		FriendWindow friendWindow = GameUI.Instance.friendWindow;
		if (friendWindow != null)
			friendWindow.OnCancelPopup(null);
		
		string infoStr = "Kakako Message Error!";
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		int statusCode = int.Parse(statusCodeStr);
		int errorStringID = -1;
		switch(statusCode)
    	{
    	case -32:	//초대 메세지 1일 쿼터 초과
			errorStringID = kakao_InviteFailedStringID;
    		break;
    	case -31:	//초대메시지는 같은 수신자에게는 30일에 1번만 보낼 수 있습니다.
			errorStringID = kakao_ExceedInviteStringID;
    		break;
    	case -17:
    	case -16:	//메시지 수신을 차단한 사용자입니다.
			errorStringID = kakao_Message_Blocked_StringID;
    		break;
    	case -14:	//지원하지 않는 OS를 사용하는 유저입니다.
    		errorStringID = kakao_Unspported_device_stringID;
    		break;
    	default:
			errorStringID = kakao_InviteFailedByOtherStringID;
    		break;
    	}
		
		if (stringTable != null)
			infoStr = stringTable.GetData(errorStringID);
		
		if (GameUI.Instance.MessageBox != null)
			GameUI.Instance.MessageBox.SetMessage(infoStr);
	}
	
	public void OnGamePause(string args)
	{
		UIRootPanel uiRootPanel = null;
		if (GameUI.Instance != null)
			uiRootPanel = GameUI.Instance.uiRootPanel;
		
		PauseButton pauseButton = null;
		if (uiRootPanel != null)
			pauseButton = uiRootPanel.GetComponentInChildren<PauseButton>();
		
		if (pauseButton != null)
		{
			if (args == "Pause")
			{
				if (pauseButton.IsPauseMode == false)
					pauseButton.OnOk(null);
			}
			else
			{
				if (pauseButton.IsPauseMode == true)
					pauseButton.OnClosePopup(null);
			}
		}
	}
	
	public void OnSetHive5AppKey(string appKey)
	{
		Logger.DebugLog(string.Format("Hive5 AppKey Changed : {0}", appKey));
		Hive5.Hive5Config.AppKey = appKey;
		
		CreateNetwork(Game.isHive5);
	}
	
	public int kakaoFriendsFailedStringID = 1107;
	public void OnFailedKakaoFriends(string arg)
	{
		string infoStr = "Failed Update Kakako Friends!";
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
			infoStr = stringTable.GetData(kakaoFriendsFailedStringID);
		
		if (GameUI.Instance.MessageBox != null)
			GameUI.Instance.MessageBox.SetMessage(infoStr);
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.SendUpdateFailedKakaoFriends(arg);
	}
	
	/*
	public void OnKakaoExceedInvite(string userid)
	{
		FriendWindow friendWindow = GameUI.Instance.friendWindow;
		if (friendWindow != null)
			friendWindow.OnCancelPopup(null);
		
		string infoStr = "EXCEED_INVITE_CHAT_LIMIT";
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		if (stringTable != null)
			infoStr = stringTable.GetData(kakaoExceedInviteStringID);
		
		if (GameUI.Instance.MessageBox != null)
			GameUI.Instance.MessageBox.SetMessage(infoStr);
	}
	
	public void OnKakaoInviteFailedByOther(string userid)
	{
		FriendWindow friendWindow = GameUI.Instance.friendWindow;
		if (friendWindow != null)
			friendWindow.OnCancelPopup(null);
		
		string infoStr = "EXCEED_INVITE_CHAT_LIMIT";
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		if (stringTable != null)
			infoStr = stringTable.GetData(kakaoInviteFailedByOtherStringID);
		
		if (GameUI.Instance.MessageBox != null)
			GameUI.Instance.MessageBox.SetMessage(infoStr);
	}
	*/
}

namespace BigFoot
{
	public class ConverJson
	{
		public static string MakeToJson(LitJson.JsonData json)
		{
			if (Game.LitJsonTest == true)
			{
				System.IO.StringWriter stringWriter = new System.IO.StringWriter ();
				LitJson.JsonWriter tempWrite = new LitJson.JsonWriter(stringWriter);
				tempWrite.Validate = false;
				WriteJson(json, tempWrite);
				
				return stringWriter.ToString();
			}
			else
				return json.ToJson();
		}
		
		private static void WriteJson (LitJson.IJsonWrapper obj, LitJson.JsonWriter writer)
	    {
			if (obj == null)
			{
				writer.Write (null);
				return;
			}
			
	        if (obj.IsString) {
	            writer.Write (obj.GetString ());
	            return;
	        }
	
	        if (obj.IsBoolean) {
	            writer.Write (obj.GetBoolean ());
	            return;
	        }
	
	        if (obj.IsDouble) {
	            writer.Write (obj.GetDouble ());
	            return;
	        }
	
	        if (obj.IsInt) {
	            writer.Write (obj.GetInt ());
	            return;
	        }
	
	        if (obj.IsLong) {
	            writer.Write (obj.GetLong ());
	            return;
	        }
	
	        if (obj.IsArray) {
	            writer.WriteArrayStart ();
	            foreach (object elem in (System.Collections.IList) obj)
	                WriteJson ((LitJson.JsonData) elem, writer);
	            writer.WriteArrayEnd ();
	
	            return;
	        }
	
	        if (obj.IsObject) {
	            writer.WriteObjectStart ();
				
				try {
	                /*
					foreach (System.Collections.DictionaryEntry entry in ((System.Collections.IDictionary) obj)) {
	                    UnityEngine.Debug.Log(string.Format("WriteJson : {0}, {1}", entry.Key, entry.Value));
						
						writer.WritePropertyName ((string) entry.Key);
	                    WriteJson ((LitJson.JsonData) entry.Value, writer);
	                }*/
					
					System.Collections.ICollection keys = obj.Keys;
					foreach(object key in keys)
					{
						if (key == null || !(key is string))
						{
							continue;
						}
						
						writer.WritePropertyName ((string) key);
						LitJson.JsonData data = (LitJson.JsonData)((System.Collections.IDictionary) obj)[key];
						WriteJson(data, writer);
					}
					
				}
				catch(System.ArgumentException ex)
				{
					UnityEngine.Debug.Log(string.Format("WriteJson {0}", ex.ToString()));
				}
				
	            writer.WriteObjectEnd ();
	
	            return;
	        }
	    }
	}
}


