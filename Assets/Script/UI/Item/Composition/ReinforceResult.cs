using UnityEngine;
using System.Collections;

public class ReinforceResult : BasePopup {
	public BaseReinforceWindow parentWindow = null;
	
	public Transform origineItemSlotNode = null;
	public Transform reinforceItemSlotNode = null;
	
	public string itemSlotPrefab = "UI/Item/ItemSlot";
	
	protected ItemSlot origineItemSlot = null;
	protected ItemSlot reinforceItemSlot = null;
	
	//public UILabel origineItemInfo = null;
	//public UILabel reinforceItemInfo = null;
	public ItemInfoPage origineItemInfo = null;
	public ItemInfoPage reinforceItemInfo = null;
	
	public UILabel originStepInfoLabel = null;
	public UILabel reinforceStepInfoLabel = null;
	
	
	public int reinforceTitleFormatStringID = 263;
	public int compositionTitleFormatStringID = 263;
	private string reinforceTitleFormatStr = "";
	private string compositionTitleFormatStr = "";
	
	public int reinforceStepFormatStringID = 256;
	private string reinforceStepFormatStr = "";
	
	public int compositionStepFormatStringID = 265;
	private string compositionStepFormatStr = "";
	
	void Awake()
	{
		if (origineItemSlot == null)
			origineItemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefab, origineItemSlotNode);
		if (reinforceItemSlot == null)
			reinforceItemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefab, reinforceItemSlotNode);
		
		if (origineItemSlot != null)
			origineItemSlot.slotType = GameDef.eSlotType.Common;
		if (reinforceItemSlot != null)
			reinforceItemSlot.slotType = GameDef.eSlotType.Common;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		if (string.IsNullOrEmpty(reinforceTitleFormatStr) == true && stringTable != null && reinforceTitleFormatStringID != -1)
			reinforceTitleFormatStr = stringTable.GetData(reinforceTitleFormatStringID);
		if (string.IsNullOrEmpty(compositionTitleFormatStr) == true && stringTable != null && compositionTitleFormatStringID != -1)
			compositionTitleFormatStr = stringTable.GetData(compositionTitleFormatStringID);
		
		if (string.IsNullOrEmpty(reinforceStepFormatStr) == true && stringTable != null && reinforceStepFormatStringID != -1)
			reinforceStepFormatStr = stringTable.GetData(reinforceStepFormatStringID);
		if (string.IsNullOrEmpty(compositionStepFormatStr) == true && stringTable != null && compositionStepFormatStringID != -1)
			compositionStepFormatStr = stringTable.GetData(compositionStepFormatStringID);
	}
	
	public void SetResultReinforceItem(Item origItem, Item reinforceItem)
	{
		string titleStr = "";
		if (reinforceItem != null)
			titleStr = string.Format(reinforceTitleFormatStr, reinforceItem.itemInfo.itemName);
		
		if (titleLabel != null)
			titleLabel.text = titleStr;
		
		if (origineItemSlot != null)
			origineItemSlot.SetItem(origItem);
		
		if (reinforceItemSlot != null)
			reinforceItemSlot.SetItem(reinforceItem);
		
		string reinforceStepStr = "";
		string origStepStr = "";
		
		if (origItem != null)
			origStepStr = string.Format(reinforceStepFormatStr, origItem.reinforceStep);
			
		if (reinforceItem != null)
			reinforceStepStr = string.Format(reinforceStepFormatStr, reinforceItem.reinforceStep);
		
		if (originStepInfoLabel != null)
			originStepInfoLabel.text = origStepStr;
		if (reinforceStepInfoLabel != null)
			reinforceStepInfoLabel.text = reinforceStepStr;
		
		if (origineItemInfo != null)
			origineItemInfo.SetItem(origItem);
		if (reinforceItemInfo != null)
			reinforceItemInfo.SetItem(reinforceItem);
	}
	
	public void SetResultCompoitionItem(Item origItem, Item reinforceItem)
	{
		string titleStr = "";
		if (reinforceItem != null)
			titleStr = string.Format(compositionTitleFormatStr, reinforceItem.itemGrade + 1);
		
		if (titleLabel != null)
			titleLabel.text = titleStr;
		
		if (origineItemSlot != null)
			origineItemSlot.SetItem(origItem);
		
		if (reinforceItemSlot != null)
			reinforceItemSlot.SetItem(reinforceItem);
		
		string origStepStr = "";
		string reinforceStepStr = "";
		
		if (origItem != null)
			origStepStr = string.Format(compositionStepFormatStr, origItem.itemGrade + 1);
			
		if (reinforceItem != null)
			reinforceStepStr = string.Format(compositionStepFormatStr, reinforceItem.itemGrade + 1);
		
		if (originStepInfoLabel != null)
			originStepInfoLabel.text = origStepStr;
		if (reinforceStepInfoLabel != null)
			reinforceStepInfoLabel.text = reinforceStepStr;
		
		if (origineItemInfo != null)
			origineItemInfo.SetItem(origItem);
		if (reinforceItemInfo != null)
			reinforceItemInfo.SetItem(reinforceItem);
	}
	
	public void OnBack()
	{
		if (parentWindow != null)
			parentWindow.CloseResultPopup();
	}
}
