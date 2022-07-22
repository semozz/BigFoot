using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RandomBoxEventInfo
{
	public int randomEventID;
	public GameObject randomEventObj;
}

public class RandomBoxEventWindow : BaseCashShopWindow {
	public List<CashItemPanel> cashItemButtons = new List<CashItemPanel>();
	
	public List<RandomBoxEventInfo> randomEventInfos = new List<RandomBoxEventInfo>();
	public RandomBoxEventInfo GetRandomBoxEventInfo(int eventID)
	{
		RandomBoxEventInfo info = null;
		foreach(RandomBoxEventInfo temp in randomEventInfos)
		{
			if (temp != null && temp.randomEventID == eventID)
			{
				info = temp;
				break;
			}
		}
		return info;
	}
	
	public override void Start()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.RANDOMBOXSHOP;
		
		GameUI.Instance.randomBoxWindow = this;
	}
	
	void OnDestroy()
	{
		ClosePopup();
		
		GameUI.Instance.randomBoxWindow = null;
	}
	
	public void InitWindow(EventShopInfoData eventInfo)
	{
		foreach(RandomBoxEventInfo temp in randomEventInfos)
		{
			if (temp.randomEventObj != null)
				temp.randomEventObj.SetActive(false);
		}
		
		RandomBoxEventInfo info = null;
		if (eventInfo != null)
			info = GetRandomBoxEventInfo(eventInfo.eventID);
		if (info != null)
		{
			if (info.randomEventObj != null)
				info.randomEventObj.SetActive(true);
		}
		
		foreach(CashItemPanel tempCash in cashItemButtons)
			tempCash.SetCashItem(null);
		
		TableManager tableManager = TableManager.Instance;
		CashShopInfoTable cashInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
		
		List<CashItemInfo> cashShopInfos = new List<CashItemInfo>();
		if (cashInfoTable != null)
		{
			foreach(CashItemInfo tempInfo in cashInfoTable.randomBoxItemInfos)
			{
				if (tempInfo.eventID == eventInfo.eventID)
					cashShopInfos.Add(tempInfo);
			}
		}
				
		int nButtonCount = cashItemButtons.Count;
		int infoCount = cashShopInfos != null ? cashShopInfos.Count : 0;
		
		int nCount = Mathf.Min(nButtonCount, infoCount);
		bool isBuy = false;
		for (int index = 0; index < nCount; ++index)
		{
			CashItemPanel cashItem = GetCashItem(index);
			CashItemInfo cashItemInfo = GetCashItemInfo(index, cashShopInfos);
			
			if (cashItem != null)
			{
				cashItem.SetCashItem(cashItemInfo);
				
				if (cashItem.buyButton != null)
					cashItem.buyButton.isEnabled = true;
				
				if (cashItem.buttonMessage != null)
				{
					cashItem.buttonMessage.target = this.gameObject;
					cashItem.buttonMessage.functionName = "OnBuyCashItem";
				}
			}
		}
	}
	
	public CashItemPanel GetCashItem(int index)
	{
		CashItemPanel cashItem = null;
		int nCount = cashItemButtons != null ? cashItemButtons.Count : 0;
		if (index >= 0 && index < nCount)
			cashItem = cashItemButtons[index];
				
		return cashItem;
	}
	
	public CashItemInfo GetCashItemInfo(int index, List<CashItemInfo> itemList)
	{
		CashItemInfo info = null;
		int nCount = itemList != null ? itemList.Count : 0;
		if (index >= 0 && index < nCount)
			info = itemList[index];
				
		return info;
	}
	
	public override void OnResult (NetErrorCode errorCode, int cashItemID)
	{
		CloseConfirmWindow();
		
		base.OnResult (errorCode, cashItemID);
	}
	
	
	public string resultWindowPrefab = "UI/Event/RandomboxProgressWindow";
	public RandomBoxProgressWindow randomProgressWindow = null;
	public void SetRandomBox(PacketRandombox packet)
	{
		CloseConfirmWindow();
		
		if (randomProgressWindow == null)
			randomProgressWindow = ResourceManager.CreatePrefab<RandomBoxProgressWindow>(resultWindowPrefab, popupNode);
		
		if (randomProgressWindow != null)
		{
			randomProgressWindow.parentWindow = this;
			
			randomProgressWindow.gameObject.SetActive(true);
			randomProgressWindow.SetRandomBox(packet);
		}
	}
	
	public void CloseRandomResult()
	{
		if (randomProgressWindow != null)
		{
			DestroyObject(randomProgressWindow.gameObject);
			randomProgressWindow = null;
		}
	}
	
	/*
	public void OnBuyConfirm(GameObject obj)
	{
		CloseConfirmWindow();
		
		PacketRandombox tempPacket = new PacketRandombox();
		
		int randomValue = Random.Range(0, 100);
		if (randomValue < 20)
		{
			tempPacket.item = new ItemDBInfo();
			tempPacket.item.ID = 101304;
		}
		else if (randomValue < 40)
		{
			tempPacket.gold = 1200;
		}
		else if (randomValue < 60)
		{
			tempPacket.meat = 100;
		}
		else if (randomValue < 80)
		{
			tempPacket.buffPackDay = 1;
		}
		else if (randomValue < 100)
		{
			tempPacket.coupon = 2;
		}
		
		SetRandomBox(tempPacket);
	}
	*/
}
