using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GambleWindow : BaseItemWindow {
	public enum eGambleType
	{
		ByGold = 0,
		ByCash = 1,
		ByCoupon = 2,
	}
	
	public enum eGambleWindoMode
	{
		Wait,
		Gamble,
		Refresh,
		AutoRefresh,
		GambleItemRefresh,
	}
	public eGambleWindoMode curMode = eGambleWindoMode.Wait;
	
	public ItemSlotPanel gambleItemPanel = null;
	
	public static System.DateTime refreshExpireTime;
	public static System.TimeSpan addTime = new System.TimeSpan(0, 0, 60);
	public static bool isRequestedByServer = false;
	
	public UILabel timerLabel = null;
	
	public float refreshWaitTime = 1.5f;
	public float refreshWaitDelayTime = 0.0f;
	
	public Vector3 oneMoreGambleCashPriceValue = Vector3.zero;
	public Vector3 gambleCashPriceValue = Vector3.zero;
	public Vector3 gambleGoldPriceValue = Vector3.zero;
	public Vector3 refreshPriceValue = Vector3.zero;
	
	public UISprite gambleGoldPriceSprite = null;
	public UISprite gambleCashPriceSprite = null;
	public UISprite refreshPriceSprite = null;
	
	public UILabel gambleGoldPriceLabel = null;
	public UILabel gambleCashPriceLabel = null;
	public UILabel refreshPriceLabel = null;
	
	public GameObject refreshObj = null;
	
	public UIButton start_Gold = null;
	public UIButton start_Gem = null;
	public UIButton start_Coupon = null;
	public UILabel couponCountLabel = null;
	
	public GameObject refreshEffect = null;
	
	public UILabel npcTalkLabel = null;
	public UILabel tryTalkLabel = null;
	public int normalTalkID = 250;
	public int eventTalkID = 251;
	private string normalTalkStr = "";
	private string eventTalkStr = "";
	
	public int defaultTryTalkID = 242;
	public int dayLeftStringID = 243;
	public int hourLeftStringID = 244;
	public int minLeftStringID = 245;
	public int secLeftStringID = 246;
	
	private string defaultTryTalkString = "";
	private string dayTimeFormatString = "{0}";
	private string hourTimeFormatString = "{0}";
	private string minTimeFormatString = "{0}";
	private string secTimeFormatString = "{0}";
	
	public override void Awake()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.GAMBLE;
		
		base.Awake();
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTabel = null;
		StringTable stringTable = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
			stringValueTabel = tableManager.stringValueTable;
		}
		
		if (stringTable != null)
		{
			normalTalkStr = stringTable.GetData(normalTalkID);
			eventTalkStr = stringTable.GetData(eventTalkID);
			
			defaultTryTalkString = stringTable.GetData(defaultTryTalkID);
			
			dayTimeFormatString = stringTable.GetData(dayLeftStringID);
			hourTimeFormatString = stringTable.GetData(hourLeftStringID);
			minTimeFormatString = stringTable.GetData(minLeftStringID);
			secTimeFormatString = stringTable.GetData(secLeftStringID);
		}
		
		int addSec = 21600;
		if (stringValueTabel != null)
			addSec = stringValueTabel.GetData(StringValueKey.GambleRefreshTimeSec);
		
		//addSec = 15;
		addTime = Game.ToTimeSpan(addSec);
		
		if (refreshObj != null)
			refreshObj.SetActive(false);
		
		if (refreshEffect != null)
			refreshEffect.SetActive(false);
		
		GameUI.Instance.gambleWindow = this;
	}
	
	public override void OnSelectItem(GameObject button)
	{
		ItemSlot itemSlot = null;
		GameObject parent = null;
		
		if (button != null)
			parent = button.transform.parent.gameObject;
		
		if (parent != null)
			itemSlot = parent.GetComponent<ItemSlot>();
		
		Item item = null;
		int slotIndex = -1;
		GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
		
		if (itemSlot != null)
		{
			if (itemSlot.itemIcon != null)
				item = itemSlot.itemIcon.item;
			
			slotIndex = itemSlot.slotIndex;
			slotWindow = itemSlot.slotWindowType;
		}
		
		OnSelectItem(item, slotIndex, slotWindow);
	}
	
	/*
	public void InitSelectItem()
	{
		Item item = null;
		int slotIndex = -1;
		GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
		
		OnSelectItem(item, slotIndex, slotWindow);
	}
	
	public void OnSelectItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		selectedItem = item;
		selectedItemIndex = slotIndex;
		selectedSlotWindow = slotWindow;
		
		if (itemInfoPage != null)
			itemInfoPage.SetItem(selectedItem);
		
		UpdateSelectedSlotFrame();
	}
	*/
	
	public override void OnSelectItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		base.OnSelectItem(item, slotIndex, slotWindow);
		
		UpdateSelectedSlotFrame();
	}
	
	public override void UpdateSelectedSlotFrame()
	{
		if (gambleItemPanel != null)
			gambleItemPanel.InitSelectedSlot();
		
		if (selectedItemIndex != -1)
		{
			if (gambleItemPanel != null)
				gambleItemPanel.SetSelectedSlot(selectedItemIndex, true);
		}
	}
	
	public void SetGambleButton(int couponCount)
	{
		if (this.couponCountLabel != null)
			this.couponCountLabel.text = string.Format("{0}", couponCount);
		
		bool isCoupon = couponCount > 0;
		if (this.start_Coupon != null)
			this.start_Coupon.gameObject.SetActive(isCoupon);
		
		if (this.start_Gem != null)
			this.start_Gem.gameObject.SetActive(!isCoupon);
	}
	
	public override void InitWindow()
	{
		//CloseGambleProgress();
		
		if (itemInfoPage != null)
			itemInfoPage.SetItem(null);
		
		//PlayerController player = Game.Instance.player;
		//LifeManager lifeManager = player != null ? player.lifeManager : null;
		
		UpdateCoinInfo();
		
		TableManager tableManager = TableManager.Instance;
		CharExpTable expTable = null;
		GamblePriceTable gamblePriceTable = null;
		if (tableManager != null)
		{
			expTable = tableManager.charExpTable;
			gamblePriceTable = tableManager.gamblePriceTable;
		}
		int charLevel = 1;
		
		int charIndex = 0;
		ClientConnector connector = Game.Instance.Connector;
		if (connector != null)
			charIndex = connector.charIndex;
		
		CharInfoData charInfo = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int couponCount = 0;
		if (charInfo != null)
		{
			privateData = charInfo.GetPrivateData(charIndex);
			couponCount = charInfo.gambleCoupon;
		}
		
		if (privateData != null && expTable != null)
			charLevel = expTable.GetLevel(privateData.baseInfo.ExpValue);
		
		if (gamblePriceTable != null)
			gambleGoldPriceValue = gamblePriceTable.GetGamblePrice(charLevel);
		
		// xgreen. todo 못읽어오면 어쩌지...
		TableManager.Instance.GetGamblePriceValue(out gambleCashPriceValue, out refreshPriceValue, out oneMoreGambleCashPriceValue);

		SetGamblePrice(gambleGoldPriceValue, gambleCashPriceValue, refreshPriceValue);
		
		SetGambleButton(couponCount);
		
		
		if (isRequestedByServer == true)
		{
			if (gambleItemPanel != null)
				gambleItemPanel.ResetItems();
		
			RefreshGambleItems(false);
			
			isRequestedByServer = false;
			
			SetMode(eGambleWindoMode.Wait);
		}
		else
		{
			gambleItemList.Clear();
			List<GambleItem> itemList = null;
			bool isGambleEvent = false;
			if (privateData != null)
			{
				itemList = privateData.gambleItemList;
				isGambleEvent = privateData.CheckGambleEvent();
			}
			
			UpdateNPCTalkMsg(isGambleEvent);
			
			if (itemList != null && itemList.Count > 0)
			{
				if (gambleItemPanel != null)
				{
					gambleItemPanel.InitSelectedSlot();
					gambleItemPanel.ResetItems();
				}
				
				int index = 0;
				int itemID = 0;
				foreach(GambleItem info in itemList)
				{
					itemID = info.ID;
					Item item = Item.CreateItem(itemID, "", info.Grade, 0, 1, info.itemRate, 0);
					gambleItemList.Add(item);
					
					Item tempItem = item;
					if (isGambleEvent == true)
					{
						itemID = privateData.GetGambleEventItemID(index);
					
						if (itemID != -1)
							tempItem = Item.CreateItem(itemID, "", 0, 0, 1, -1, 0);
					}
					
					gambleItemPanel.SetItem(index, tempItem);
					++index;
				}
				
				if (isGambleEvent == true)
					Invoke("UpdateGambleItemList", privateData.CheckGambleLeftTime());
				
				System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
				if (UpdateRefreshTime(out restTime) == true)
				{
					if (gambleItemPanel != null)
						gambleItemPanel.ResetItems();
				
					RefreshGambleItems(false);
				}
				
				SetMode(eGambleWindoMode.Wait);
			}
			else
			{
				SetMode(eGambleWindoMode.AutoRefresh);
			}
		}
	}
	
	public static bool UpdateRefreshTime(out System.TimeSpan restTime)
	{
		bool isNeedRefresh = false;
		
		System.DateTime defaultTime = new System.DateTime();
		if (refreshExpireTime == defaultTime)
		{
			restTime = new System.TimeSpan(0);
			return isNeedRefresh;
		}
		
		System.DateTime nowTime = System.DateTime.Now;
		restTime = refreshExpireTime - nowTime;
		
		
		double totalSeconds = restTime.TotalSeconds;
		if (totalSeconds <= 0)
		{
			double restTotalSecs = -restTime.TotalSeconds;
			double addTotalSecs = addTime.TotalSeconds;
			int nCount = (int)(restTotalSecs / addTotalSecs);
			if (((int)restTotalSecs % (int)addTotalSecs) != 0)
				nCount++;
			
			System.TimeSpan newAddTime = Game.ToTimeSpan(addTotalSecs * nCount);
			refreshExpireTime += newAddTime;
			
			restTime = refreshExpireTime - nowTime;
			
			totalSeconds = restTime.TotalSeconds;
			
			isNeedRefresh = true;
		}
		
		return isNeedRefresh;
	}
	
	public void SetGamblePrice(Vector3 gambleGold, Vector3 gambleCash, Vector3 refresh)
	{
		SetPrice(gambleGoldPriceSprite, gambleGoldPriceLabel, gambleGold);
		SetPrice(gambleCashPriceSprite, gambleCashPriceLabel, gambleCash);
		SetPrice(refreshPriceSprite, refreshPriceLabel, refresh);
	}
	
	public void SetPrice(UISprite sprite, UILabel label, Vector3 price)
	{
		string spriteName = "Shop_Money02";
		string priceStr = "0";
		if (price.x > 0.0f)
		{
			spriteName = "Shop_Money01";
			priceStr = string.Format("{0:#,###,###}", price.x);
		}
		else if (price.y > 0.0f)
		{
			spriteName = "Shop_Money02";
			priceStr = string.Format("{0:#,###,###}", price.y);
		}
		else if (price.z > 0.0f)
		{
			spriteName = "Shop_Money03";
			priceStr = string.Format("{0:#,###,###}", price.z);
		}
		else
		{
			spriteName = "Shop_Money02";
		}
		
		if (sprite != null)
			sprite.spriteName = spriteName;
		if (label != null)
			label.text = priceStr;
	}
	
	/*
	public void UpdateCoinInfo()
	{
		float fGold = 0.0f;
		float fJewel = 0.0f;
		float fMedal = 0.0f;
		
		fGold = Game.Instance.charInfoData.goldInfo.x;
		fJewel = Game.Instance.charInfoData.goldInfo.y;
		fMedal = Game.Instance.charInfoData.goldInfo.z;
		
		SetCoinInfo(fGold, fJewel, fMedal);
	}
	*/
	
	public string gambleProgressPrefabPath = "UI/Item/GambleProgressWindow";
	public GambleProgressWindow gambleProgressWindow = null;
	
	public void OnOneMoreGamble(eGambleType type)
	{
		switch(type)
		{
		case eGambleType.ByCash:
		case eGambleType.ByCoupon:
			List<Item> tempGambleList = new List<Item>();
			int nCount = GambleWindow.gambleItemList.Count;
			int maxCount = Mathf.Min(nCount, GambleWindow.gambleItemListCount * 2);
			for(int index = 0; index < maxCount; ++index)
			{
				Item item = GambleWindow.gambleItemList[index];
				
				tempGambleList.Add(item);
			}
			
			Vector3 price = Vector3.zero;
			if (type == eGambleType.ByCash)
				price = oneMoreGambleCashPriceValue;
			
			OnGambleStart(type, tempGambleList, price, 1);
			break;
		}
	}
	
	public void OnGambleStartByCash(GameObject obj)
	{
		List<Item> tempGambleList = new List<Item>();
		int nCount = GambleWindow.gambleItemList.Count;
		int maxCount = Mathf.Min(nCount, GambleWindow.gambleItemListCount * 2);
		for(int index = 0; index < maxCount; ++index)
		{
			Item item = GambleWindow.gambleItemList[index];
			
			tempGambleList.Add(item);
		}
		
		OnGambleStart(eGambleType.ByCash, tempGambleList, gambleCashPriceValue, 0);
	}
	
	public void OnGambleStartByNormal(GameObject obj)
	{
		List<Item> tempGambleList = new List<Item>();
		int nCount = GambleWindow.gambleItemList.Count;
		
		for(int index = GambleWindow.gambleItemListCount; index < nCount; ++index)
		{
			Item item = GambleWindow.gambleItemList[index];
			
			tempGambleList.Add(item);
		}
		
		OnGambleStart(eGambleType.ByGold, tempGambleList, gambleGoldPriceValue, 0);
	}
	
	public void OnGambleStartByCoupon(GameObject obj)
	{
		List<Item> tempGambleList = new List<Item>();
		int nCount = GambleWindow.gambleItemList.Count;
		
		for(int index = GambleWindow.gambleItemListCount; index < nCount; ++index)
		{
			Item item = GambleWindow.gambleItemList[index];
			
			tempGambleList.Add(item);
		}
		
		OnGambleStart(eGambleType.ByCoupon, tempGambleList, Vector3.zero, 0);
	}
	
	private eGambleType curGambleType = eGambleType.ByGold;
	private List<Item> curGambleList = new List<Item>();
	public void OnGambleStart(eGambleType gambleType, List<Item> gambleList, Vector3 gamblePrice, int again)
	{
		if (requestCount > 0)
			return;
		
		if (curMode != eGambleWindoMode.Wait)
			return;
		
		CharInfoData charData = Game.Instance.charInfoData;
		int emptyInventoryCount = -1;
		int couponCount = 0;
		if (charData != null)
		{
			emptyInventoryCount = charData.CheckEmptyInventory();
			couponCount = charData.gambleCoupon;
		}
	
		if (emptyInventoryCount == 0)
		{
			OnNeedInventoryPopup();
			return;
		}
		
		if (gambleType == eGambleType.ByCoupon && couponCount <= 0)
		{
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			
			string errorMsg = "Not Enough Gamble Coupon";
			if (stringTable != null)
				errorMsg = stringTable.GetData((int)NetErrorCode.NotEnoughGambleCoupon);
			
			if (GameUI.Instance.MessageBox != null)
				GameUI.Instance.messageBox.SetMessage(errorMsg);
			
			return;
		}
		
		if (gambleType != eGambleType.ByCoupon)
		{
			CashItemType cashCheck = CheckNeedGold(gamblePrice);
			if (cashCheck != CashItemType.None)
			{
				OnNeedMoneyPopup(cashCheck, this);
				return;
			}
		}
		
		CloseGambleProgress();
		
		//xgreen.
		Game.Instance.packetSender.SendSelectGambleItem((int)Game.Instance.playerClass, (int)gambleType, again);
		
		this.curGambleType = gambleType;
		this.curGambleList.Clear();
		this.curGambleList.AddRange(gambleList);
		
		requestCount++;
	}
	
	public void StartGambleProgress(NetErrorCode errorCode, Item resultItem, int selectedIndex)
	{
		if (gambleProgressWindow == null)
		{
			Transform uiRoot = this.transform;
			gambleProgressWindow = ResourceManager.CreatePrefab<GambleProgressWindow>(gambleProgressPrefabPath, uiRoot, Vector3.zero);
			
			if (gambleProgressWindow != null)
			{
				gambleProgressWindow.gambleType = this.curGambleType;
				gambleProgressWindow.parent = this;
			}
		}
		
		if (gambleProgressWindow != null)
		{
			gambleProgressWindow.SetGambleItemIDs(this.curGambleList);
			
			UpdateCoinInfo(true);
			UpdateCouponCount();
			SetMode(eGambleWindoMode.Gamble);
			
			gambleProgressWindow.OnGambleResultItem(errorCode, resultItem, selectedIndex);
		}
	}
	
	public void UpdateCouponCount()
	{
		int couponCount = 0;
		CharInfoData charInfo = Game.Instance.charInfoData;
		if (charInfo != null)
			couponCount = charInfo.gambleCoupon;
		
		SetGambleButton(couponCount);
	}
	
	public void RefreshGambleItems(bool isCash)
	{
		UpdateCoinInfo(true);
		OnSelectItem(null);
		
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		CharPrivateData privateData = null;
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		bool isGambleEvent = false;
		if (privateData != null)
		{
			isGambleEvent = privateData.CheckGambleEvent();
		}
		
		MakeGambleItems(gambleItemList);
		
		IPacketSender packetSender = Game.Instance.packetSender;
		packetSender.SendRefreshGambleItems(isCash, charIndex, ref gambleItemList);
		
		GambleWindow.bSendRefresh = true;
		
		if (isCash == true)
		{
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUpdateGamble, 0);
			
			Game.Instance.SendUpdateAchievmentInfo();
		}
		
		int nCount = gambleItemList.Count;
		Item item = null;
		
		gambleItemPanel.InitSelectedSlot();
		gambleItemPanel.ResetItems();
		
		Item origItem = null;
		for(int index = 0; index < nCount; ++index)
		{
			origItem = gambleItemList[index];
			
			int itemID = -1;
			if (isGambleEvent == true)
			{
				itemID = privateData.GetGambleEventItemID(index);
			
				if (itemID != -1)
					item = Item.CreateItem(itemID, "", 0, 0, 1, -1, 0);
				else
					item = origItem;
			}
			else
				item = origItem;
			
			gambleItemPanel.SetItem(index, item);
		}
	}
	
	public static bool bSendRefresh = false;
	public static List<Item> gambleItemList = new List<Item>();
	public static int gambleItemListCount = 6;
	public static void MakeGambleItems(List<Item> gambleItems)
	{
		gambleItems.Clear();
		
		TableManager tableManager = TableManager.Instance;
		ItemTable itemTable = null;
		//GambleItemTable gambleItemTable = null;
		GambleItemTable gambleItem_A_Grade = null;
		GambleItemTable gambleItem_B_Grade = null;
		GambleSGradeList gambleSGradeList = null;
		GambleItemRateTable gambleItemRateTable = null;
		GambleItemGradeRateTable gambleItemGradeRateTable = null;
		CharExpTable expTable= null;
		
		if (tableManager != null)
		{
			itemTable = tableManager.itemTable;
			//gambleItemTable = tableManager.gambleItemTable;
			gambleItem_A_Grade = tableManager.gambleItem_A_Grade;
			gambleItem_B_Grade = tableManager.gambleItem_B_Grade;
			gambleSGradeList = tableManager.gambleSGradeTable;
			
			gambleItemGradeRateTable = tableManager.gambleItemGradeRateTable;
			gambleItemRateTable = tableManager.gambleItemRateTable;
			expTable = tableManager.charExpTable;
		}
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if(charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int curLevel = 1;
		if (expTable != null && privateData != null)
			curLevel = expTable.GetLevel(privateData.baseInfo.ExpValue);
		
		Item item = null;
		
		GambleItemInfos gambleInfos = null;
		int gambleItemCount = 0;
		
		Dictionary<int, int> gambleItemIndex = new Dictionary<int, int>();
		
		int i = 0;
		
		GambleItemGradeRateInfo rateInfo = null;
		if (gambleItemRateTable != null)
			rateInfo = gambleItemRateTable.GetData(3);
		
		GambleItemGradeRateInfo gradeInfo = null;
		if (gambleItemGradeRateTable != null)
			gradeInfo = gambleItemGradeRateTable.GetData(3);
		
		//////////////////////////////////////////////////////////////////
		//S 등급...
		Dictionary<int, int> indexList = new Dictionary<int, int>();
		int itemCount = 0;
		if (gambleSGradeList != null)
			itemCount = gambleSGradeList.dataList.Count;
		
		for(i = 0; i < gambleItemListCount; ++i)
		{
			if (itemCount > 0)
			{
				int randIndex = Random.Range(0, itemCount);
				while(indexList.ContainsKey(randIndex) == true)
				{
					randIndex = Random.Range(0, itemCount);
				}
				indexList.Add(randIndex, randIndex);
				if (indexList.Count == itemCount)
					indexList.Clear();
				
				int itemID = gambleSGradeList.dataList[randIndex];
				
				int itemRate = 0;
				ItemInfo itemInfo = itemTable.GetData(itemID);
				if (itemInfo != null)
				{
					switch(itemInfo.itemType)
					{
					case ItemInfo.eItemType.Costume_Back:
					case ItemInfo.eItemType.Costume_Body:
					case ItemInfo.eItemType.Costume_Head:
						itemRate = -1;
						break;
					default:
						itemRate = 0;
						if (rateInfo != null)
							itemRate = GetItemRate(rateInfo.rateList);
						break;
					}
				}
				
				int grade = 0;
				if (gradeInfo != null)
					grade = GetItemGrade(gradeInfo.rateList);
				
				item = Item.CreateItem(itemID, "", 0, 0, 1, itemRate, 0);
				item.SetGradeInfo(grade, 0);
			}
			else
				item = null;
			
			gambleItems.Add(item);
		}
		////////////////////////////////////////////////////////////////////
		
		
		/////////////////////////////////////////////////////////////////////
		//A 등급.
		gambleInfos = null;
		if (gambleItem_A_Grade != null)
			gambleInfos = gambleItem_A_Grade.GetData(curLevel);
		
		if (gambleInfos != null)
			gambleItemCount = gambleInfos.gambleItems.Count;
		
		rateInfo = null;
		if (gambleItemRateTable != null)
			rateInfo = gambleItemRateTable.GetData(2);
		
		gradeInfo = null;
		if (gambleItemGradeRateTable != null)
			gradeInfo = gambleItemGradeRateTable.GetData(1);
		
		for(i = 0; i < gambleItemListCount; ++i)
		{
			int randIndex = Random.Range(0, gambleItemCount);
			while(gambleItemIndex.ContainsKey(randIndex) == true)
			{
				randIndex = Random.Range(0, gambleItemCount);
			}
			gambleItemIndex.Add(randIndex, randIndex);
			if (gambleItemIndex.Count == gambleItemCount)
				gambleItemIndex.Clear();
			
			GambleItemInfo info = gambleInfos.gambleItems[randIndex];
			
			int itemRate = 0;
			if (rateInfo != null)
				itemRate = GetItemRate(rateInfo.rateList);
			
			int grade = 0;
			if (gradeInfo != null)
				grade = GetItemGrade(gradeInfo.rateList);
			
			item = Item.CreateItem(info.itemID, "", 0, 0, 1, itemRate, 0);
			item.SetGradeInfo(grade, 0);
			
			gambleItems.Add(item);
		}
		//////////////////////////////////////////////////////////////////////////
		
		//////////////////////////////////////////////////////////////////////////
		//B 등급..
		gambleInfos = null;
		if (gambleItem_B_Grade != null)
			gambleInfos = gambleItem_B_Grade.GetData(curLevel);
		if (gambleInfos != null)
			gambleItemCount = gambleInfos.gambleItems.Count;
		
		rateInfo = null;
		if (gambleItemRateTable != null)
			rateInfo = gambleItemRateTable.GetData(1);
		
		gradeInfo = null;
		if (gambleItemGradeRateTable != null)
			gradeInfo = gambleItemGradeRateTable.GetData(0);
		
		for(i = 0; i < gambleItemListCount; ++i)
		{
			int randIndex = Random.Range(0, gambleItemCount);
			while(gambleItemIndex.ContainsKey(randIndex) == true)
			{
				randIndex = Random.Range(0, gambleItemCount);
			}
			gambleItemIndex.Add(randIndex, randIndex);
			if (gambleItemIndex.Count == gambleItemCount)
				gambleItemIndex.Clear();
			
			GambleItemInfo info = gambleInfos.gambleItems[randIndex];
			
			int itemRate = 0;
			if (rateInfo != null)
				itemRate = GetItemRate(rateInfo.rateList);
			
			int grade = 0;
			if (gradeInfo != null)
				grade = GetItemGrade(gradeInfo.rateList);
			
			item = Item.CreateItem(info.itemID, "", 0, 0, 1, itemRate, 0);
			item.SetGradeInfo(grade, 0);
			
			gambleItems.Add(item);
		}
		
		
		if (gambleItems.Count < 8)
		{
			Debug.LogWarning("GambleItem Count wrong...");
			foreach(Item temp in gambleItems)
			{
				Debug.Log(temp);
			}
		}
	}
	
	public static int GetItemGrade(List<int> gambleItemGradeRate)
	{
		int randValue = Random.Range(0, 100);
		
		int startValue = 0;
		int endValue = 0;
		
		int itemGrade = 0;
		int nCount = gambleItemGradeRate.Count;
		for(int index = 0; index < nCount; ++ index)
		{
			endValue = startValue + gambleItemGradeRate[index];
			if (randValue >= startValue && randValue < endValue)
			{
				itemGrade = index;
				break;
			}
			
			startValue = endValue;
		}
		
		return itemGrade;
	}
	
	public static int GetItemRate(List<int> gambleItemRate)
	{
		int randValue = Random.Range(0, 100);
		
		int itemRate = 0;
		int nCount = gambleItemRate != null ? gambleItemRate.Count : 0;
		
		int startValue = 0;
		int endValue = startValue;
		
		for(int index = 0; index < nCount; ++index)
		{
			endValue += gambleItemRate[index];
			if (randValue >= startValue && randValue < endValue)
			{
				break;
			}
			
			startValue = endValue;
			itemRate++;
		}
		
		return itemRate;
	}
	
	public void OnStorage()
	{
		SetMode(eGambleWindoMode.Wait);
		
		this.requestCount = 0;
		
		OnBack();
		
		if (townUI != null)
			townUI.gameObject.SendMessage("OnStorage", SendMessageOptions.DontRequireReceiver);
	}
	
	public void SetMode(eGambleWindoMode mode)
	{
		switch(mode)
		{
		case eGambleWindoMode.Wait:
			if (refreshObj != null)
				refreshObj.SetActive(false);
			break;
		case eGambleWindoMode.AutoRefresh:
		case eGambleWindoMode.Refresh:
			if (gambleItemPanel != null)
				gambleItemPanel.ResetItems();
			
			if (refreshObj != null)
			{
				if (refreshObj.audio != null)
					refreshObj.audio.mute = !GameOption.effectToggle;
				
				refreshObj.SetActive(true);
			}
			
			refreshWaitDelayTime = refreshWaitTime;
			break;
		case eGambleWindoMode.Gamble:
			if (refreshObj != null)
				refreshObj.SetActive(false);
			break;
		case eGambleWindoMode.GambleItemRefresh:
			if (refreshObj != null)
				refreshObj.SetActive(false);
			break;
		}
		
		curMode = mode;
	}
	
	public override void OnBack()
	{
		if (curMode != eGambleWindoMode.Wait)
			return;
		
		base.OnBack();
		
		CloseGambleProgress();
		
		CancelInvoke();
	}
	
	//private int refreshItemIndex = -1;
	public void UpdateGambleInfo(int refreshIndex)
	{
		int nCount = GambleWindow.gambleItemList.Count;
		if (refreshIndex >= 0 && refreshIndex < nCount)
		{
			//refreshItemIndex = refreshIndex;
		
			GambleWindow.gambleItemList[refreshIndex] = null;
			
			if (gambleItemPanel != null)
				gambleItemPanel.SetItem(refreshIndex, null);
			
			MakeRefreshGambleItem(refreshIndex, gambleItemList);
			
			SetMode(eGambleWindoMode.Wait);
		}
		else
		{
			SetMode(eGambleWindoMode.Wait);
		}
		
		//CloseGambleProgress();
	}
	
	
	public bool isAutoRefresh = false;
	public void Update()
	{
		System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
		bool needRefresh = UpdateRefreshTime(out restTime);
		double totalSeconds = restTime.TotalSeconds;
		
		//if (totalSeconds <= 0 && totalSeconds > -1)
		if (needRefresh == true && GambleWindow.bSendRefresh == false)
		{
			SetMode(eGambleWindoMode.AutoRefresh);
			isAutoRefresh = true;
		}
		
		string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}", restTime.Hours, restTime.Minutes, restTime.Seconds);
		timerLabel.text = timeText;
		
		switch(curMode)
		{
		case eGambleWindoMode.AutoRefresh:
		case eGambleWindoMode.Refresh:
			if (refreshWaitDelayTime <= 0.0f)
			{
				bool isCash = curMode == eGambleWindoMode.Refresh;
				RefreshGambleItems(isCash);
				SetMode(eGambleWindoMode.Wait);
			}
			refreshWaitDelayTime -= Time.deltaTime;
			break;
		}
				
		UpdateGambleEventTime();
	}
	
	private void UpdateGambleEventTime()
	{
		int charIndex = 0;
		ClientConnector connector = Game.Instance.Connector;
		if (connector != null)
			charIndex = connector.charIndex;
		
		CharInfoData charInfo = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charInfo != null)
			privateData = charInfo.GetPrivateData(charIndex);
		
		System.TimeSpan timeSpan = System.TimeSpan.MinValue;
		
		if (privateData != null)
			timeSpan = privateData.CheckGambleEventTime();
		
		if (timeSpan.TotalSeconds > 0)
		{
			int totalDay = (int)timeSpan.TotalDays;
            
			string timeFormatStr = "";
			if (totalDay >= 1)
			{
                timeFormatStr = string.Format(dayTimeFormatString, totalDay);
			}
			else if (timeSpan.TotalHours >= 1)
			{
				timeFormatStr = string.Format(hourTimeFormatString, timeSpan.Hours);
			}
			else if (timeSpan.TotalMinutes >= 1)
			{
				timeFormatStr = string.Format(minTimeFormatString, timeSpan.Minutes);
			}
			else
			{
				timeFormatStr = string.Format(secTimeFormatString, timeSpan.Seconds);
			}
			
			PopupBaseWindow.SetLabelString(tryTalkLabel, timeFormatStr);
		}
		else
		{
			PopupBaseWindow.SetLabelString(tryTalkLabel, defaultTryTalkString);
		}
	}
	
	private void UpdateNPCTalkMsg(bool isGambleEvent)
	{
		if (isGambleEvent == true)
			PopupBaseWindow.SetLabelString(npcTalkLabel, eventTalkStr);
		else
			PopupBaseWindow.SetLabelString(npcTalkLabel, normalTalkStr);
	}
	
	private void UpdateGambleItemList()
	{
		if (itemInfoPage != null)
			itemInfoPage.SetItem(null);
		
		int charIndex = 0;
		ClientConnector connector = Game.Instance.Connector;
		if (connector != null)
			charIndex = connector.charIndex;
		
		CharInfoData charInfo = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charInfo != null)
			privateData = charInfo.GetPrivateData(charIndex);
		
		List<GambleItem> itemList = null;
		bool isGambleEvent = false;
		
		if (privateData != null)
		{
			itemList = privateData.gambleItemList;
			isGambleEvent = privateData.CheckGambleEvent();
		}
		
		UpdateNPCTalkMsg(isGambleEvent);
		
		gambleItemList.Clear();
		
		if (itemList != null && itemList.Count > 0)
		{
			if (gambleItemPanel != null)
			{
				gambleItemPanel.InitSelectedSlot();
				gambleItemPanel.ResetItems();
			}
			
			int index = 0;
			int itemID = 0;
			foreach(GambleItem info in itemList)
			{
				itemID = info.ID;
				if (privateData != null && isGambleEvent == true)
					itemID = privateData.GetGambleEventItemID(index);
				
				Item item = null;
				if (itemID == -1)
					item = Item.CreateItem(info.ID, "", info.Grade, 0, 1, info.itemRate, 0);
				else
					item = Item.CreateItem(itemID, "", 0, 0, 1, -1, 0);
				
				gambleItemList.Add(item);
				
				gambleItemPanel.SetItem(index, item);
				++index;
			}
		}
		
		if (isGambleEvent == true)
			Invoke("UpdateGambleItemList", privateData.CheckGambleLeftTime());
	}
	
	public void OnImmediateRefresh()
	{
		CashItemType cashCheck = CheckNeedGold(this.refreshPriceValue);
		if (cashCheck != CashItemType.None)
		{
			OnNeedMoneyPopup(cashCheck, this);
			return;
		}
		
		SetMode(eGambleWindoMode.Refresh);
	}
	
	public string inventoryConfirmPopupPrefabPath = "UI/Item/NeedInventoryPopup";
	public void OnNeedInventoryPopup()
	{
		BasePopup  popup = ResourceManager.CreatePrefab<BasePopup>(inventoryConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (popup != null)
		{
			popup.cancelButtonMessage.target = this.gameObject;
			popup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
			
			popup.okButtonMessage.target = this.gameObject;
			popup.okButtonMessage.functionName = "OnNeedInventoryOK";
			
			popupList.Add(popup);
		}
	}
	
	public void OnNeedInventoryOK(GameObject obj)
	{
		OnBack();
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
		{
			townUI.toWindowtype = TownUI.eTOWN_UI_TYPE.GAMBLE;
			townUI.OnStorage();
		}
	}
	
	public void OnConfirmPopupCancel(GameObject obj)
	{
		ClosePopup();
	}
	
	public static void MakeRefreshGambleItem(int index, List<Item> gambleItems)
	{
		TableManager tableManager = TableManager.Instance;
		//ItemTable itemTable = null;
		GambleItemTable gambleItem_A_Grade = null;
		GambleItemTable gambleItem_B_Grade = null;
		
		CharExpTable expTable= null;
		
		if (tableManager != null)
		{
			//itemTable = tableManager.itemTable;
			gambleItem_A_Grade = tableManager.gambleItem_A_Grade;
			gambleItem_B_Grade = tableManager.gambleItem_B_Grade;
			
			expTable = tableManager.charExpTable;
		}
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if(charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int curLevel = 1;
		if (expTable != null && privateData != null)
			curLevel = expTable.GetLevel(privateData.baseInfo.ExpValue);
		
		GambleItemInfos gambleInfos = null;
		Item newItem = null;
		
		Dictionary<int, int> gambleItemIDs = new Dictionary<int, int>();
		foreach(Item oldItem in gambleItems)
		{
			if (oldItem == null || oldItem.itemInfo == null)
				continue;
			
			int itemID = oldItem.itemInfo.itemID;
			gambleItemIDs.Add(itemID, itemID);
		}
		
		if (index >= 0 && index < GambleWindow.gambleItemListCount)
		{
			newItem = MakeRefreshGradeS(index, gambleItemIDs);
		}
		else if (index >= GambleWindow.gambleItemListCount && index < GambleWindow.gambleItemListCount * 2)
		{
			gambleInfos = gambleItem_A_Grade.GetData(curLevel);
			newItem = MakeRefreshGradeA(index, gambleItemIDs, gambleInfos.gambleItems);
		}
		else
		{
			gambleInfos = gambleItem_B_Grade.GetData(curLevel);
			newItem = MakeRefreshGradeB(index, gambleItemIDs, gambleInfos.gambleItems);
		}
		
		IPacketSender sender = Game.Instance.packetSender;
		if (sender != null)
		{
			sender.SendChangeGambleItem(index, newItem);
		}
	}
	
	public void ChangeGambleItem(int index, Item newItem)
	{
		gambleItemList[index] = newItem;
		
		if (gambleItemPanel != null)
		{
			CharInfoData charData = Game.Instance.charInfoData;
			CharPrivateData privateData = null;
			int charIndex = 0;
			if (Game.Instance.connector != null)
				charIndex = Game.Instance.connector.charIndex;
			
			if (charData != null)
				privateData = charData.GetPrivateData(charIndex);
			
			bool isGambleEvent = false;
			if (privateData != null)
				isGambleEvent = privateData.CheckGambleEvent();
			
			int itemID = -1;
			if (isGambleEvent == true)
				itemID = privateData.GetGambleEventItemID(index);
			
			Item tempItem = newItem;
			if (itemID != -1)
				tempItem = Item.CreateItem(itemID, newItem.uID, 0, 0, newItem.itemCount, -1, 0);
			
			gambleItemPanel.SetItem(index, tempItem);
			gambleItemPanel.SetEffect(index, this.refreshEffect);
		}
	}
	
	public static Item MakeRefreshGradeS(int index, Dictionary<int, int> gambleItemIDs)
	{
		TableManager tableManager = TableManager.Instance;
		GambleItemGradeRateTable gambleItemGradeRateTable = null;
		ItemTable itemTable = null;
		GambleSGradeList gambleSGradeList = null;
		GambleItemRateTable gambleItemRateTable = null;
		
		if (tableManager != null)
		{
			itemTable = tableManager.itemTable;
			gambleSGradeList = tableManager.gambleSGradeTable;
			gambleItemRateTable = tableManager.gambleItemRateTable;
			gambleItemGradeRateTable = tableManager.gambleItemGradeRateTable;
		}
		
		if (itemTable == null || gambleItemGradeRateTable == null || gambleSGradeList == null)
			return null;
		
		Item newItem = null;
		
		Dictionary<int, int> indexList = new Dictionary<int, int>();
		
		int itemCount = 0;
		if (gambleSGradeList != null)
			itemCount = gambleSGradeList.dataList.Count;
		int itemID = -1;
		
		if (itemCount > 0)
		{
			do
			{
				int randIndex = Random.Range(0, itemCount);
				while(indexList.ContainsKey(randIndex) == true)
				{
					randIndex = Random.Range(0, itemCount);
				}
				indexList.Add(randIndex, randIndex);
				if (indexList.Count == itemCount)
					indexList.Clear();
				
				itemID = gambleSGradeList.dataList[randIndex];
			}
			while(gambleItemIDs.ContainsKey(itemID) == true);
		}
		
		if (itemID != -1)
		{
			GambleItemGradeRateInfo rateInfo = null;
			if (gambleItemRateTable != null)
				rateInfo = gambleItemRateTable.GetData(3);
			
			GambleItemGradeRateInfo gradeInfo = null;
			if (gambleItemGradeRateTable != null)
				gradeInfo = gambleItemGradeRateTable.GetData(3);
			
			int itemRate = 0;
			ItemInfo itemInfo = itemTable.GetData(itemID);
			if (itemInfo != null)
			{
				switch(itemInfo.itemType)
				{
				case ItemInfo.eItemType.Costume_Back:
				case ItemInfo.eItemType.Costume_Body:
				case ItemInfo.eItemType.Costume_Head:
					itemRate = -1;
					break;
				default:
					itemRate = 0;
					if (rateInfo != null)
						itemRate = GetItemRate(rateInfo.rateList);
					break;
				}
			}
			
			int grade = 0;
			if (gradeInfo != null)
				grade = GetItemGrade(gradeInfo.rateList);
			
			newItem = Item.CreateItem(itemID, "", 0, 0, 1, itemRate, 0);
			newItem.SetGradeInfo(grade, 0);
		}
		
		return newItem;
	}
	
	public static Item MakeRefreshGradeA(int index, Dictionary<int, int> gambleItemIDs, List<GambleItemInfo> gambleInfos)
	{
		TableManager tableManager = TableManager.Instance;
		GambleItemGradeRateTable gambleItemGradeRateTable = null;
		GambleItemRateTable gambleItemRateTable = null;
		ItemTable itemTable = null;
		
		if (tableManager != null)
		{
			itemTable = tableManager.itemTable;
			gambleItemGradeRateTable = tableManager.gambleItemGradeRateTable;
			gambleItemRateTable = tableManager.gambleItemRateTable;
			
		}
		
		if (itemTable == null || gambleItemGradeRateTable == null || gambleItemRateTable == null)
			return null;
		
		Item newItem = null;
		
		int nCount = gambleInfos.Count;
		GambleItemInfo newInfo = null;
		do
		{
			int randIndex = Random.Range(0, nCount);
			newInfo = gambleInfos[randIndex];
		}while(newInfo != null && gambleItemIDs.ContainsKey(newInfo.itemID) == true);
		
		if (newInfo != null)
		{
			GambleItemGradeRateInfo rateInfo = null;
			if (gambleItemRateTable != null)
				rateInfo = gambleItemRateTable.GetData(2);
			
			GambleItemGradeRateInfo gradeInfo = null;
			if (gambleItemGradeRateTable != null)
				gradeInfo = gambleItemGradeRateTable.GetData(1);
		
			int itemRate = 1;
			if (rateInfo != null)
				itemRate = GetItemRate(rateInfo.rateList);
			
			int grade = 0;
			if (gradeInfo != null)
				grade = GetItemGrade(gradeInfo.rateList);
			
			newItem = Item.CreateItem(newInfo.itemID, "", 0, 0, 1, itemRate, 0);
			newItem.SetGradeInfo(grade, 0);
		}
				
		return newItem;
	}
	
	public static Item MakeRefreshGradeB(int index, Dictionary<int, int> gambleItemIDs, List<GambleItemInfo> gambleInfos)
	{
		TableManager tableManager = TableManager.Instance;
		GambleItemGradeRateTable gambleItemGradeRateTable = null;
		GambleItemRateTable gambleItemRateTable = null;
		ItemTable itemTable = null;
		
		if (tableManager != null)
		{
			itemTable = tableManager.itemTable;
			gambleItemGradeRateTable = tableManager.gambleItemGradeRateTable;
			gambleItemRateTable = tableManager.gambleItemRateTable;
			
		}
		
		if (itemTable == null || gambleItemGradeRateTable == null || gambleItemRateTable == null)
			return null;
		
		Item newItem = null;
		
		int nCount = gambleInfos.Count;
		GambleItemInfo newInfo = null;
		do
		{
			int randIndex = Random.Range(0, nCount);
			newInfo = gambleInfos[randIndex];
		}while(newInfo != null && gambleItemIDs.ContainsKey(newInfo.itemID) == true);
		
		if (newInfo != null)
		{
			GambleItemGradeRateInfo rateInfo = null;
			if (gambleItemRateTable != null)
				rateInfo = gambleItemRateTable.GetData(1);
			
			GambleItemGradeRateInfo gradeInfo = null;
			if (gambleItemGradeRateTable != null)
				gradeInfo = gambleItemGradeRateTable.GetData(0);
		
			int itemRate = 1;
			if (rateInfo != null)
				itemRate = GetItemRate(rateInfo.rateList);
			
			int grade = 0;
			if (gradeInfo != null)
				grade = GetItemGrade(gradeInfo.rateList);
			
			newItem = Item.CreateItem(newInfo.itemID, "", 0, 0, 1, itemRate, 0);
			newItem.SetGradeInfo(grade, 0);
		}
		
		return newItem;
	}
	
	
	public void CloseGambleProgress()
	{
		if (gambleProgressWindow != null)
		{
			DestroyObject(gambleProgressWindow.gameObject, 0.1f);
			gambleProgressWindow = null;
		}
	}
}
