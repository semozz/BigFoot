 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum eCashEvent
{
	None,
	StarterPack,
	CashBonus,
	RandomBox,
}

[System.Serializable]
public class CashTabInfo
{
	public CashItemType type = CashItemType.JewelToGold;
	public UICheckbox checkBox = null;
	public GameObject tabWindow = null;
}

[System.Serializable]
public class EventButtonInfo
{
	public eCashEvent eventType = eCashEvent.None;
	
	public UIButtonMessage buttonMessage = null;
	public CashEventButton button = null;
}

[System.Serializable]
public class EventButtonPrefabInfo
{
	public eCashEvent eventType = eCashEvent.None;
	public string eventPrefab = "";
	public string functionName = "";
	
	public System.DateTime endTime;
}


public class CashShopWindow : BaseCashShopWindow {
	public TownUI townUI = null;
	
	public TabViewControl tabViewControl = null;
	
	public UILabel titleLabel = null;
	public int titleLabelStringID = -1;
	
	public List<Transform> eventButtonPos = new List<Transform>();
	public Vector3 GetEventButtonPos(int index)
	{
		Vector3 vPos = Vector3.zero;
		int nCount = eventButtonPos.Count;
		if (index >= 0 && index < nCount)
			vPos = eventButtonPos[index].localPosition;
		
		return vPos;
	}
	public List<EventButtonPrefabInfo> eventPrefabs = new List<EventButtonPrefabInfo>();
	public EventButtonPrefabInfo GetEventPrefab(eCashEvent type)
	{
		EventButtonPrefabInfo info = null;
		foreach(EventButtonPrefabInfo temp in eventPrefabs)
		{
			if (temp.eventType == type)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	public Transform eventButtonNode = null;
	
	public override void Start()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.CASH_SHOP;
		
		GameUI.Instance.cashShopWindow = this;
		
		InitCashItems();
	}
	
	private void InitCashItems()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		CashShopInfoTable cashShopInfo = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
			cashShopInfo = tableManager.cashShopInfoTable;
		}
		
		if (stringTable != null && titleLabel != null)
		{
			titleLabel.text = stringTable.GetData(titleLabelStringID);
		}
		
		SetItems(GameDef.eItemSlotWindow.Cash_Jewel, cashShopInfo.jewelItemInfos);
		SetItems(GameDef.eItemSlotWindow.Cash_Gold, cashShopInfo.goldItemInfos);
		SetItems(GameDef.eItemSlotWindow.Cash_Buff, cashShopInfo.timeListItemInfos);
		SetItems(GameDef.eItemSlotWindow.Cash_Potion, cashShopInfo.potionItemInfos);
	}
	
	public void SetItems(GameDef.eItemSlotWindow windowType, List<CashItemInfo> list)
	{
		BaseItemScrollView scrollView = tabViewControl.GetItemWindow(windowType);
		if (scrollView != null)
		{
			int nCount = list.Count;
			CashItemInfo cashInfo = null;
			for (int index = 0; index < nCount; ++index)
			{
				cashInfo = list[index];
				scrollView.SetItem(index, cashInfo);
			}
			
			scrollView.Invoke("InitPage", 0.1f);
		}
	}
	
	List<EventButtonInfo> eventButtons = new List<EventButtonInfo>();
	
	public void InitEventButtons()
	{
		foreach(EventButtonInfo info in eventButtons)
		{
			if (info.button != null)
				DestroyObject(info.button.gameObject, 0.1f);
		}
		
		eventButtons.Clear();
	}
	
	private void CashEventInfoUpdate()
	{
		InitEventButtons();
		
		CharInfoData charData = Game.Instance.charInfoData;
		bool bStarterPackEvent = false;
		
		EventButtonPrefabInfo eventPrefabInfo = null;
		List<EventButtonPrefabInfo> prefabs = new List<EventButtonPrefabInfo>();
		
		if (charData != null)
		{
			bStarterPackEvent = charData.CheckPackageItem();
			
			
			EventShopInfoData eventShopInfo = null;//charData.eventShopInfo;
		
			System.DateTime nowTime = System.DateTime.Now;
			System.TimeSpan leftTime = System.TimeSpan.Zero;
			
			foreach(var temp in charData.eventShopInfos)
			{
				bool bCashBonusEvent = false;
		
				bool buyCountCheck = false;
				bool timeCheck = false;
				
				eventShopInfo = temp.Value;
				if (eventShopInfo != null)
				{
					if (eventShopInfo.limitCount == 0)
						buyCountCheck = true;
					else
						buyCountCheck = eventShopInfo.buyCount < eventShopInfo.limitCount;
					
					leftTime = eventShopInfo.expireTime - nowTime;
					if (leftTime.TotalSeconds > 0)
						timeCheck = true;
					else
						leftTime = System.TimeSpan.Zero;
					
					bCashBonusEvent = (buyCountCheck == true) && (timeCheck == true);
				}
				
				if (bCashBonusEvent == true)
				{
					eventPrefabInfo = GetEventPrefab(eventShopInfo.eventType);
					if (eventPrefabInfo != null)
					{
						eventPrefabInfo.endTime = eventShopInfo.expireTime;
						prefabs.Add(eventPrefabInfo);
					}
				}
			}
				
		}
		
		if (bStarterPackEvent == true)
		{
			eventPrefabInfo = GetEventPrefab(eCashEvent.StarterPack);
			if (eventPrefabInfo != null)
				prefabs.Add(eventPrefabInfo);
		}
		
		CreateEventButtons(prefabs);
	}
	
	private void CreateEventButtons(List<EventButtonPrefabInfo> prefabs)
	{
		int nCount = prefabs.Count;
		EventButtonPrefabInfo info = null;
		Vector3 vPos = Vector3.zero;
		
		
		for (int index = 0; index < nCount; ++index)
		{
			info = prefabs[index];
			vPos = GetEventButtonPos(index);
				
			CashEventButton eventButton = ResourceManager.CreatePrefab<CashEventButton>(info.eventPrefab, this.eventButtonNode, vPos);
			if (eventButton != null)
			{
				eventButton.SetEndTime(info.endTime);
				
				EventButtonInfo buttonInfo = new EventButtonInfo();
				if (buttonInfo != null)
				{
					buttonInfo.button = eventButton;
					buttonInfo.buttonMessage = eventButton.GetComponent<UIButtonMessage>();
					buttonInfo.eventType = info.eventType;
					
					if (buttonInfo.buttonMessage != null)
					{
						buttonInfo.buttonMessage.target = this.gameObject;
						buttonInfo.buttonMessage.functionName = info.functionName;
					}
					
					eventButtons.Add(buttonInfo);
				}
			}
		}
	}
	
	public override void Awake()
	{
		base.Awake();
	}
	
	void OnDestroy()
	{
		ClosePopup();
		
		GameUI.Instance.cashShopWindow = null;
	}
	
	void MakeCashItem(string prefabPath, List<CashItemInfo> cashItemInfos, List<Transform> posList, Transform root)
	{
		int nCount = cashItemInfos.Count;
		int index = 0;
		for (index = 0; index < nCount; ++index)
		{
			CashItemInfo info = cashItemInfos[index];
			Transform pos = posList[index];
			GameObject newObj = CreatePrefab(prefabPath, pos.localPosition, root);
			
			CashItemPanel cashItem = newObj.GetComponent<CashItemPanel>();
			if (cashItem != null)
			{
				cashItem.SetCashItem(info);
				
				if (cashItem.buttonMessage != null)
				{
					cashItem.buttonMessage.target = this.gameObject;
					cashItem.buttonMessage.functionName = "OnBuyCashItem";
				}
			}
		}
	}
	
	/*
	public void OnTabActivate(bool bActivate)
	{
		if (bActivate == false)
			return;
		
		foreach(CashTabInfo info in tabInfos)
		{
			if (info != null && info.tabWindow != null)
				info.tabWindow.SetActive(info.checkBox.isChecked);
		}
	}
	*/
	
	public override void InitWindow(CashItemType type, PopupBaseWindow popupBase)
	{
		this.popupBaseWindow = popupBase;
		
		if (tabViewControl == null)
			return;
		tabViewControl.InitTab(type);
		
		UpdateCoinInfo();
		
		CashEventInfoUpdate();
	}
	
	public override void OnBack()
	{
		GameUI.Instance.SetCurrentWindow(this.popupBaseWindow);
		
		base.OnBack();
	}
	
	public void OnChangeTabView(bool bActivate)
	{
		if (bActivate == false)
			return;
		
		UICheckbox checkBox = null;
		foreach(TabViewInfo info in this.tabViewControl.tabViewInfos)
		{
			if (info.tabButton != null)
				checkBox = info.tabButton.GetComponent<UICheckbox>();
			
			if (info.tabView != null)
				info.tabView.SetActive(checkBox.isChecked);
		}
	}
	
	
	public string packageItemShopWindowPrefab = "UI/Event/Starter_Package";
	PackageItemShopWindow packageItemShopWindow = null;
	public void OnPackageItemShop(GameObject obj)
	{
		if (packageItemShopWindow == null)
			packageItemShopWindow = ResourceManager.CreatePrefab<PackageItemShopWindow>(packageItemShopWindowPrefab, popupNode, Vector3.zero);
		else
			packageItemShopWindow.gameObject.SetActive(true);
		
		CharInfoData charData = Game.Instance.charInfoData;
		List<int> buyPackageItems = null;
		
		if (charData != null)
			buyPackageItems = charData.packageItems;
		
		if (packageItemShopWindow != null)
			packageItemShopWindow.InitWindow(buyPackageItems);
		
		GameUI.Instance.SetCurrentWindow(packageItemShopWindow);
	}
	
	public string eventShopWindowPrefab = "UI/Event/Event_Shop";
	public EventShopWindow eventShopWindow = null;
	public void OnEventShop(GameObject obj)
	{
		if (eventShopWindow == null)
			eventShopWindow = ResourceManager.CreatePrefab<EventShopWindow>(eventShopWindowPrefab, popupNode, Vector3.zero);
		else
			eventShopWindow.gameObject.SetActive(true);
		
		CharInfoData charData = Game.Instance.charInfoData;
		int buyCount = 0;
		int limitCount = 0;
		
		EventShopInfoData eventInfo = null;
		if (charData != null)
			eventInfo = charData.GetEventShopInfo(eCashEvent.CashBonus);
		if (eventInfo != null)
		{
			buyCount = eventInfo.buyCount;
			limitCount = eventInfo.limitCount;
		}
		
		if (eventShopWindow != null)
			eventShopWindow.InitWindow(buyCount, limitCount);
		
		GameUI.Instance.SetCurrentWindow(eventShopWindow);
	}
	
	public string randomEventShopWindowPrefab = "UI/Event/Event_RandomBox_Window";
	public RandomBoxEventWindow randomboxEventShopWindow = null;
	public void OnRandomBoxShop(GameObject obj)
	{
		if (randomboxEventShopWindow == null)
			randomboxEventShopWindow = ResourceManager.CreatePrefab<RandomBoxEventWindow>(randomEventShopWindowPrefab, popupNode, Vector3.zero);
		else
			randomboxEventShopWindow.gameObject.SetActive(true);
		
		CharInfoData charData = Game.Instance.charInfoData;
		int buyCount = 0;
		int limitCount = 0;
		
		EventShopInfoData eventInfo = charData != null ? charData.GetEventShopInfo(eCashEvent.RandomBox) : null;
		if (randomboxEventShopWindow != null)
			randomboxEventShopWindow.InitWindow(eventInfo);
		
		GameUI.Instance.SetCurrentWindow(eventShopWindow);
	}
}
