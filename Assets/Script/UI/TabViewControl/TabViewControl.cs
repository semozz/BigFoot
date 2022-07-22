using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TabViewInfo
{
	//public GameObject tabViewPrefab = null;
	public string tabViewPrefabPath = "";
	public GameObject tabButton = null;
	
	public GameObject tabView = null;
	
	public GameDef.eItemSlotWindow slotWindowType = GameDef.eItemSlotWindow.Inventory;
}

public class TabViewControl : MonoBehaviour {
	public List<TabViewInfo> tabViewInfos = new List<TabViewInfo>();
	public Transform tabWindowNode = null;
	
	public GameObject parent = null;
	
	public int normalTabIndex = -1;
	public int costumeTabIndex = -1;
	public int cashTabIndex = -1;
	public int invenTabIndex = -1;
	
	public Dictionary<GameDef.eItemSlotWindow, BaseItemScrollView> itemWindows = new Dictionary<GameDef.eItemSlotWindow, BaseItemScrollView>();
	
	void Awake()
	{
		foreach(TabViewInfo info in tabViewInfos)
		{
			if (info == null)
				return;
			
			GameObject tabViewPrefab = ResourceManager.LoadPrefab(info.tabViewPrefabPath);
			
			if (tabViewPrefab != null)
			{
				BaseItemScrollView baseItemScrollView = tabViewPrefab.GetComponent<BaseItemScrollView>();
				if (baseItemScrollView != null)
				{
					baseItemScrollView.parent = this.parent;
					baseItemScrollView.slotWindow = info.slotWindowType;
				}
				
				GameObject tabView = (GameObject)Instantiate(tabViewPrefab);
				
				if (tabView != null)
				{
					tabView.transform.parent = tabWindowNode;
					
					tabView.transform.localPosition = Vector3.zero;
					tabView.transform.localScale = Vector3.one;
					tabView.transform.localRotation = Quaternion.identity;
					
					baseItemScrollView = tabView.GetComponent<BaseItemScrollView>();
					
					if (baseItemScrollView != null)
					{
						baseItemScrollView.InitAwake();
						itemWindows.Add(info.slotWindowType, baseItemScrollView);
					}
				}
				
				info.tabView = tabView;
			}
		}
	}
	
	public void InitDefaultTab()
	{
		int nCount = tabViewInfos.Count;
		if (nCount > 0)
		{
			TabViewInfo info = tabViewInfos[0];
			
			UICheckbox checkbox = info.tabButton.GetComponent<UICheckbox>();
			if (checkbox != null)
			{
				if (checkbox.eventReceiver != null)
					checkbox.eventReceiver.SendMessage(checkbox.functionName, true, SendMessageOptions.DontRequireReceiver);
				
				checkbox.isChecked = true;
			}
		}
	}
	
	public void InitTab(CashItemType type)
	{
		if (type == CashItemType.None)
			type = CashItemType.JewelToGold;
		
		GameDef.eItemSlotWindow slotWindowType = GameDef.eItemSlotWindow.Cash_Jewel;
		switch(type)
		{
		case CashItemType.CashToJewel:
			slotWindowType = GameDef.eItemSlotWindow.Cash_Jewel;
			break;
		case CashItemType.JewelToGold:
			slotWindowType = GameDef.eItemSlotWindow.Cash_Gold;
			break;
		}
		
		foreach(TabViewInfo info in tabViewInfos)
		{
			if (info.slotWindowType == slotWindowType)
			{
				UICheckbox checkbox = info.tabButton.GetComponent<UICheckbox>();
				if (checkbox != null)
				{
					if (checkbox.eventReceiver != null)
						checkbox.eventReceiver.SendMessage(checkbox.functionName, true, SendMessageOptions.DontRequireReceiver);
					
					checkbox.isChecked = true;
				}
			}
		}
	}
	
	public void InitByTabIndex(int tabIndex)
	{
		if (tabIndex == -1)
			return;
		
		int nCount = tabViewInfos.Count;
		for (int index = 0; index < nCount; ++index)
		{
			TabViewInfo info = tabViewInfos[index];
			
			UICheckbox checkbox = info.tabButton.GetComponent<UICheckbox>();
			if (checkbox != null)
			{
				checkbox.isChecked = (tabIndex == index);
				if (checkbox.isChecked == true)
				{
					if (checkbox.eventReceiver != null)
						checkbox.eventReceiver.SendMessage(checkbox.functionName, true, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
	
	public BaseItemScrollView GetItemWindow(GameDef.eItemSlotWindow type)
	{
		BaseItemScrollView window = null;
		
		if (itemWindows.ContainsKey(type) == true)
			window = itemWindows[type];
		
		return window;
	}
	
	public void InitSelectedSlot()
	{
		foreach(var temp in itemWindows)
		{
			BaseItemScrollView view = temp.Value;
			if (view != null)
				view.InitSelectedSlot();
		}
	}
	
	public void OnInventoryTabActivate(bool bActivate)
	{
		int nTabCount = tabViewInfos.Count;
		if (invenTabIndex < 0 || invenTabIndex >= nTabCount)
			return;
		
		TabViewInfo invenTab = tabViewInfos[invenTabIndex];
		if (invenTab != null)
		{
			if (invenTab.tabView != null)
				invenTab.tabView.SetActive(bActivate);
		}
		
		if (bActivate == true)
		{
			//OnNormalTabActivate(false);
			//OnCostumeTabActivate(false);
			//OnCashTabActivate(false);
			
			if (parent != null)
			{
				parent.SendMessage("InitSelectItem",  SendMessageOptions.DontRequireReceiver);
				parent.SendMessage("OnCostumeTabActive", true, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public void OnNormalTabActivate(bool bActivate)
	{
		int nTabCount = tabViewInfos.Count;
		if (normalTabIndex < 0 || normalTabIndex >= nTabCount)
			return;
		
		TabViewInfo normalTab = tabViewInfos[normalTabIndex];
		if (normalTab != null)
		{
			if (normalTab.tabView != null)
				normalTab.tabView.SetActive(bActivate);
		}
		
		if (bActivate == true)
		{
			//OnInventoryTabActivate(false);
			//OnCostumeTabActivate(false);
			//OnCashTabActivate(false);
			
			if (parent != null)
			{
				parent.SendMessage("InitSelectItem",  SendMessageOptions.DontRequireReceiver);
				parent.SendMessage("OnCostumeTabActive", false, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public void OnCostumeTabActivate(bool bActivate)
	{
		int nTabCount = tabViewInfos.Count;
		if (costumeTabIndex < 0 || costumeTabIndex >= nTabCount)
			return;
		
		TabViewInfo costumeTab = tabViewInfos[costumeTabIndex];
		if (costumeTab != null)
		{
			if (costumeTab.tabView != null)
				costumeTab.tabView.SetActive(bActivate);
		}
		
		if (bActivate == true)
		{
			//OnInventoryTabActivate(false);
			//OnNormalTabActivate(false);
			//OnCashTabActivate(false);
			
			if (parent != null)
			{
				parent.SendMessage("InitSelectItem",  SendMessageOptions.DontRequireReceiver);
				parent.SendMessage("OnCostumeTabActive", true, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	public void OnCashTabActivate(bool  bActivate)
	{
		int nTabCount = tabViewInfos.Count;
		if (cashTabIndex < 0 || cashTabIndex >= nTabCount)
			return;
		
		TabViewInfo cashTab = tabViewInfos[cashTabIndex];
		if (cashTab != null)
		{
			if (cashTab.tabView != null)
				cashTab.tabView.SetActive(bActivate);
		}
		
		if (bActivate == true)
		{
			//OnInventoryTabActivate(false);
			//OnNormalTabActivate(false);
			//OnCostumeTabActivate(false);
			
			if (parent != null)
			{
				parent.SendMessage("InitSelectItem",  SendMessageOptions.DontRequireReceiver);
				parent.SendMessage("OnCostumeTabActive", false, SendMessageOptions.DontRequireReceiver);
			}
		}
	}
}
