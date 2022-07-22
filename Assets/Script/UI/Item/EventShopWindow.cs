using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventShopWindow : BaseCashShopWindow {
	public List<CashItemPanel> cashItemButtons = new List<CashItemPanel>();
	
	public UILabel limitInfoLabel = null;
	public int limitInfoStringID = 222;
	
	public override void Start()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.EVENTSHOP;
		
		GameUI.Instance.eventShopWindow = this;
	}
	
	void OnDestroy()
	{
		ClosePopup();
		
		GameUI.Instance.eventShopWindow = null;
	}
	
	public void InitWindow(int buyCount, int limitCount)
	{
		TableManager tableManager = TableManager.Instance;
		CashShopInfoTable cashInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		List<CashItemInfo> eventShopItems = null;
		if (cashInfoTable != null)
			eventShopItems = cashInfoTable.eventShopItemInfos;
		
		foreach(CashItemPanel tempCash in cashItemButtons)
			tempCash.SetCashItem(null);
		
		int nButtonCount = cashItemButtons.Count;
		int eventItemCount = eventShopItems != null ? eventShopItems.Count : 0;
		
		int nCount = Mathf.Min(nButtonCount, eventItemCount);
		for (int index = 0; index < nCount; ++index)
		{
			CashItemPanel cashItem = GetEventShopItem(index);
			CashItemInfo cashItemInfo = GetEventShopItemInfo(index, eventShopItems);
			
			if (cashItem != null)
			{
				cashItem.SetCashItem(cashItemInfo);
				
				if (cashItem.buttonMessage != null)
				{
					cashItem.buttonMessage.target = this.gameObject;
					cashItem.buttonMessage.functionName = "OnBuyCashItem";
				}
			}
		}
		
		SetLimitInfo(buyCount, limitCount);
	}
	
	public void SetLimitInfo(int buyCount, int limitCount)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		int enableBuyCount = Mathf.Max(0, (limitCount - buyCount));
		string limitCountInfoStr = "{0}";
		if (stringTable != null)
			limitCountInfoStr = stringTable.GetData(limitInfoStringID);
		
		if (limitInfoLabel != null)
			limitInfoLabel.text = string.Format(limitCountInfoStr, enableBuyCount);
		
		bool canBuy = limitCount > buyCount;
		foreach(CashItemPanel tempCash in cashItemButtons)
		{
			if (tempCash.buyButton != null)
				tempCash.buyButton.isEnabled = canBuy;
		}
	}
	
	public CashItemPanel GetEventShopItem(int index)
	{
		CashItemPanel cashItem = null;
		int nCount = cashItemButtons != null ? cashItemButtons.Count : 0;
		if (index >= 0 && index < nCount)
			cashItem = cashItemButtons[index];
				
		return cashItem;
	}
	
	public CashItemInfo GetEventShopItemInfo(int index, List<CashItemInfo> itemList)
	{
		CashItemInfo info = null;
		int nCount = itemList != null ? itemList.Count : 0;
		if (index >= 0 && index < nCount)
			info = itemList[index];
				
		return info;
	}
	
	public override void OnBack()
	{
		GameUI.Instance.SetCurrentWindow(null);
		
		base.OnBack();
	}
}
