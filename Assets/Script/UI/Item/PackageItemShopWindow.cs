using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PackageItemShopWindow : BaseCashShopWindow {
	public List<CashItemPanel> cashItemButtons = new List<CashItemPanel>();
	
	public override void Start()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.PACKAGEITEMSHOP;
		
		GameUI.Instance.packageItemShopWindow = this;
	}
	
	void OnDestroy()
	{
		ClosePopup();
		
		GameUI.Instance.packageItemShopWindow = null;
	}
	
	public void InitWindow(List<int> packageItemIDs)
	{
		TableManager tableManager = TableManager.Instance;
		CashShopInfoTable cashInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		List<CashItemInfo> packageItems = null;
		if (cashInfoTable != null)
			packageItems = cashInfoTable.packageItemInfos;
		
		foreach(CashItemPanel tempCash in cashItemButtons)
			tempCash.SetCashItem(null);
		
		int nButtonCount = cashItemButtons.Count;
		int packageItemCount = packageItems != null ? packageItems.Count : 0;
		
		int nCount = Mathf.Min(nButtonCount, packageItemCount);
		bool isBuy = false;
		for (int index = 0; index < nCount; ++index)
		{
			CashItemPanel cashItem = GetPackageItem(index);
			CashItemInfo cashItemInfo = GetPackageItemInfo(index, packageItems);
			
			if (cashItem != null)
			{
				cashItem.SetCashItem(cashItemInfo);
				
				isBuy = CheckBuy(cashItemInfo.ItemID, packageItemIDs);
				
				if (cashItem.buyButton != null)
					cashItem.buyButton.isEnabled = !isBuy;
				
				if (cashItem.buttonMessage != null)
				{
					cashItem.buttonMessage.target = this.gameObject;
					cashItem.buttonMessage.functionName = "OnBuyCashItem";
				}
			}
		}
	}
				
	public bool CheckBuy(int itemID, List<int> packageItemIDs)
	{
		bool isBuy = false;
		
		if (packageItemIDs != null)
		{
			foreach(int id in packageItemIDs)
			{
				if (id == itemID)
				{
					isBuy = true;
					break;
				}
			}
		}
		
		return isBuy;
	}
	
	public CashItemPanel GetPackageItem(int index)
	{
		CashItemPanel cashItem = null;
		int nCount = cashItemButtons != null ? cashItemButtons.Count : 0;
		if (index >= 0 && index < nCount)
			cashItem = cashItemButtons[index];
				
		return cashItem;
	}
	
	public CashItemInfo GetPackageItemInfo(int index, List<CashItemInfo> itemList)
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
	
	public override void OnResult (NetErrorCode errorCode, int cashItemID)
	{
		base.OnResult (errorCode, cashItemID);
		
		List<int> buyList = null;
		if (Game.Instance != null && Game.Instance.charInfoData != null)
			buyList = Game.Instance.charInfoData.packageItems;
		
		InitWindow(buyList);
	}
}
