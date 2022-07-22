using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompositionMaterialWindow : BasePopup {
	public CompositionWindow parentWindow = null;
	
	public ItemSlot materialItemSlot = null;
	public Transform itemSlotNode = null;
	
	public string itemSlotPrefab = "UI/Item/ItemSlot";
	
	public UILabel itemNameLabel = null;
	
	public UILabel noticeLabel = null;
	public int noticeStringID = 260;
	private string noticeFormatString = "";
	
	
	public UIGrid buttonGrid = null;
	public string gotoButtonPrefab = "UI/Item/Composition/GotoButton";
	public List<GoToMapButton> gotoButtons = new List<GoToMapButton>();
	
	private Item materialItem = null;
	void Awake()
	{
		materialItemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefab, itemSlotNode);
		if (materialItemSlot != null)
			materialItemSlot.slotType = GameDef.eSlotType.Common;
		
		SetItem(null);
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		PopupBaseWindow.SetLabelString(titleLabel, titleStringID, stringTable);
		
		if (stringTable != null && noticeStringID != -1)
			noticeFormatString = stringTable.GetData(noticeStringID);
	}
	
	public void OnBack()
	{
		if (this.parentWindow != null)
			this.parentWindow.ClosePopup<CompositionMaterialWindow>();
	}
	
	public void OnCloseByGoto()
	{
		if (this.parentWindow != null)
			this.parentWindow.OnBack();
	}
	
	public void SetItem(Item item)
	{
		materialItem = item;
		
		if (materialItemSlot != null)
			materialItemSlot.SetItem(item);
		
		string itemNameStr = "";
		string noticeString = "";
		
		if (item != null)
		{
			if (item.itemInfo != null)
				itemNameStr = item.itemInfo.itemName;
			
			noticeString = string.Format(noticeFormatString, item.itemGrade + 1);
		}
		
		if (itemNameLabel != null)
			itemNameLabel.text = itemNameStr;
		if (noticeLabel != null)
			noticeLabel.text = noticeString;
		
		foreach(GoToMapButton button in gotoButtons)
			DestroyObject(button.gameObject, 0.1f);
		
		gotoButtons.Clear();
		
		SetGotoButtons(item);
	}
	
	public void SetGotoButtons(Item item)
	{
		if (item != null && item.itemInfo != null)
		{
			bool isHellModeItem = false;
			foreach(var tempInfo in item.itemInfo.stageIDs)
			{
				GotoMapInfo info = tempInfo.Value;
				
				switch(info.gotoType)
				{
				case GotoMapInfo.eGotoType.eGoto_Stage:
					foreach(var temp in info.gotoStageInfos)
						MakeGotoButton(info.gotoType, temp.Key, temp.Value);
					break;
				case GotoMapInfo.eGotoType.eGoto_Gamble:
				case GotoMapInfo.eGotoType.eGoto_Defence:
					MakeGotoButton(info.gotoType, 0, 0);
					break;
				case GotoMapInfo.eGotoType.eGoto_Shop:
					MakeGotoButton(info.gotoType, info.tabID, 0);
					break;
				}
			}
			
			if (buttonGrid != null)
				buttonGrid.repositionNow = true;
		}
	}
	
	public void MakeGotoButton(GotoMapInfo.eGotoType gotoType, int stageType, int stageID)
	{
		GoToMapButton gotoButton = ResourceManager.CreatePrefab<GoToMapButton>(gotoButtonPrefab, buttonGrid.transform);
		
		if (gotoButton != null)
		{
			gotoButton.parentWindow = this.gameObject;
			gotoButton.SetGotoButtonInfo(gotoType, stageType, stageID);
			
			this.gotoButtons.Add(gotoButton);
		}
	}
}
