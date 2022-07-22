using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CashItemPanelInfo
{
	public ePayment paymentType;
	public string prefabName;
}

public class CashItempScrollView : BaseItemScrollView {
	public string defaultItemPrefab = "UI/Item/CashItemPanel";
	public List<CashItemPanelInfo> cashItemPrefabs = new List<CashItemPanelInfo>();
	public CashItemPanelInfo GetCashItemPrefab(CashItemInfo info)
	{
		CashItemPanelInfo panelInfo = null;
		foreach(CashItemPanelInfo temp in cashItemPrefabs)
		{
			if (info.paymentType == temp.paymentType)
			{
				panelInfo = temp;
				break;
			}
		}
		
		return panelInfo;
	}
	
	public string itemClickFuncName = "OnBuyCashItem";
	
	public List<CashItemPanel> itemPanels = new List<CashItemPanel>();
	public override void SetItem(int index, CashItemInfo cashInfo)
	{
		int nCount = itemPanels.Count;
		CashItemPanel cashItemPanel = null;
		if (index >= 0 && index < nCount)
			cashItemPanel = itemPanels[index];
		
		if (cashItemPanel == null)
		{
			CashItemPanelInfo panelInfo = GetCashItemPrefab(cashInfo);
			string prefabPath = defaultItemPrefab;
			if (panelInfo != null)
				prefabPath = panelInfo.prefabName;
			
			cashItemPanel = ResourceManager.CreatePrefab<CashItemPanel>(prefabPath, uiGrid.transform, Vector3.zero);
			if (cashItemPanel != null)
			{
				if (cashItemPanel.buttonMessage != null)
				{
					cashItemPanel.buttonMessage.target = this.parent;
					cashItemPanel.buttonMessage.functionName = itemClickFuncName;
				}
				
				itemPanels.Add(cashItemPanel);
				
				SortItemPanels();
			
				if (uiGrid != null)
					uiGrid.Invoke("Reposition", 0.2f);
			}
		}
		
		if (cashItemPanel != null)
			cashItemPanel.SetCashItem(cashInfo);
	}
	
	public void SortItemPanels()
	{
		int nCount = itemPanels.Count;
		CashItemPanel panel = null;
		for (int index = 0; index < nCount; ++index)
		{
			panel = itemPanels[index];
			//panel.slotIndex = index;
			
			if (panel != null)
			{
				panel.name = string.Format("{0:##0}_CashItem", index);
				
				panel.GetOrAddComponent<UIDragPanelContents>();
			}
		}
	}
	
	public void OnSelectItem(GameObject button)
	{
		if (parent != null)
		{
			parent.SendMessage(itemClickFuncName, button, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public override void InitSelectedSlot()
	{
		foreach(CashItemPanel panel in itemPanels)
		{
			//panel.SetSelected(false);
		}
	}
	
	public override void SetSelectedSlot(int slotIndex, bool bSelected)
	{
		CashItemPanel panel = null;
		int nCount = itemPanels.Count;
		if (slotIndex >= 0 && slotIndex < nCount)
			panel = itemPanels[slotIndex];
		
		//if (panel != null)
		//	panel.SetSelected(bSelected);
	}
	
	void OnDeactive()
	{
		InitData();
	}
	
	void OnEnable()
	{
		if (uiGrid != null)
			uiGrid.Invoke("Reposition", 0.2f);
	}
	
	public override void InitData()
	{
		foreach(CashItemPanel panel in itemPanels)
		{
			DestroyObject(panel.gameObject, 0.0f);
		}
		
		itemPanels.Clear();
	}
	
	public override void RemoveItem(int index)
	{
		CashItemPanel panel = null;
		int nCount = itemPanels.Count;
		if (index >= 0 && index < nCount)
			panel = itemPanels[index];
		
		if (panel != null)
		{
			DestroyObject(panel.gameObject, 0.0f);
			itemPanels.RemoveAt(index);
		}
		
		SortItemPanels();
		
		if (uiGrid != null)
			uiGrid.Invoke("Reposition", 0.2f);
	}
	
	public void RefreshPosition()
	{
		if (uiGrid != null)
			uiGrid.Invoke("Reposition", 0.2f);
	}
}
