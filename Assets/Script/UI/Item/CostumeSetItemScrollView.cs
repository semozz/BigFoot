using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CostumeSetItemScrollView : BaseItemScrollView {
	public string costumeItemPrefab = "UI/Item/CostumeSetItemPanel";
	
	public string itemClickFuncName = "OnSelectItem";
	
	public List<CostumeSetItemPanel> costumeSetItemPanels = new List<CostumeSetItemPanel>();
	public override void SetItem(int index, CostumeSetItem item)
	{
		int nCount = costumeSetItemPanels.Count;
		CostumeSetItemPanel costumeSetItemPanel = null;
		if (index >= 0 && index < nCount)
			costumeSetItemPanel = costumeSetItemPanels[index];
		
		if (costumeSetItemPanel == null)
		{
			costumeSetItemPanel = ResourceManager.CreatePrefab<CostumeSetItemPanel>(costumeItemPrefab, uiGrid.transform, Vector3.zero);
			
			if (costumeSetItemPanel.buttonMessage != null)
			{
				costumeSetItemPanel.buttonMessage.target = this.gameObject;
				costumeSetItemPanel.buttonMessage.functionName = itemClickFuncName;
			}
			
			costumeSetItemPanel.slotIndex = index;
			costumeSetItemPanel.slotWindowType = this.slotWindow;
			costumeSetItemPanels.Add(costumeSetItemPanel);
			
			SortCostumeSetItemPanels();
			
			if (uiGrid != null)
				uiGrid.Invoke("Reposition", 0.2f);
		}
		
		if (costumeSetItemPanel != null)
			costumeSetItemPanel.SetCostumeItem(item);
	}
	
	public void SortCostumeSetItemPanels()
	{
		int nCount = costumeSetItemPanels.Count;
		CostumeSetItemPanel panel = null;
		
		UIPanel deletePanel = null;
		for (int index = 0; index < nCount; ++index)
		{
			panel = costumeSetItemPanels[index];
			panel.slotIndex = index;
			
			if (panel != null)
			{
				panel.name = string.Format("{0:##0}_CostumeSetItem", index);
				
				deletePanel = panel.GetComponent<UIPanel>();
				if (deletePanel != null)
					GameObject.DestroyObject(deletePanel);
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
		foreach(CostumeSetItemPanel panel in costumeSetItemPanels)
		{
			panel.SetSelected(false);
		}
	}
	
	public override void SetSelectedSlot(int slotIndex, bool bSelected)
	{
		CostumeSetItemPanel panel = null;
		int nCount = costumeSetItemPanels.Count;
		if (slotIndex >= 0 && slotIndex < nCount)
			panel = costumeSetItemPanels[slotIndex];
		
		if (panel != null)
			panel.SetSelected(bSelected);
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
		foreach(CostumeSetItemPanel panel in costumeSetItemPanels)
		{
			DestroyObject(panel.gameObject, 0.0f);
		}
		
		costumeSetItemPanels.Clear();
	}
	
	public override void RemoveItem(int index)
	{
		CostumeSetItemPanel panel = null;
		int nCount = costumeSetItemPanels.Count;
		if (index >= 0 && index < nCount)
			panel = costumeSetItemPanels[index];
		
		if (panel != null)
		{
			DestroyObject(panel.gameObject, 0.0f);
			costumeSetItemPanels.RemoveAt(index);
		}
		
		SortCostumeSetItemPanels();
		
		if (uiGrid != null)
			uiGrid.Invoke("Reposition", 0.2f);
	}
}
