using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CompositionWindow : BaseReinforceWindow 
{
	public ItemSlot compositionItemSlot = null;
	public string itemSlotPrefabPath = "UI/Item/ItemSlot";
	public Transform itemSlotNode = null;
	
	public ItemSlot compositionMaterialItemSlot = null;
	public Transform materialSlotNode = null;
	
	public ItemSlot compositionAddMaterialItemSlot = null;
	public Transform materialAddSlotNode = null;
	
	public UILabel composInfoLabel = null;
	public int composStringID = -1;
	
	public ItemSlot curItemSlot = null;
	public Transform curItemSlotNode = null;
	public UILabel curItemGradeLabel = null;
	
	public ItemSlot nextItemSlot = null;
	public Transform nextItemSlotNode = null;
	public UILabel nextItemGradeLabel = null;
	
	public Item compositionItem = null;
	public int slotIndex = -1;
	public GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
	
	public UIButton cashStartButton = null;
	public UILabel cashStartLabel = null;
	public UILabel cashInfoLabel = null;
	
	public int composCashValue = 0;
	
	public int baseComposeRateValue = 50;
	public int addComposeRateValue = 0;
	public UILabel composeRateInfoLabel = null;
	
	
	public GameObject compositionNode = null;
	public GameObject gotoNode = null;
	
	public override void Awake()
	{
		base.Awake();
		
		if (compositionItemSlot == null)
		{
			compositionItemSlot = CreateItemSlot(itemSlotNode);
			if (compositionItemSlot != null)
				compositionItemSlot.slotWindowType = GameDef.eItemSlotWindow.Composition_Item;
		}
		if (compositionMaterialItemSlot == null)
		{
			compositionMaterialItemSlot = CreateItemSlot(materialSlotNode);
			
			if (compositionMaterialItemSlot != null)
				compositionMaterialItemSlot.slotWindowType = GameDef.eItemSlotWindow.Composition_Material;
		}
		
		if (compositionAddMaterialItemSlot == null)
		{
			compositionAddMaterialItemSlot = CreateItemSlot(materialAddSlotNode);
			
			if (compositionAddMaterialItemSlot != null)
				compositionAddMaterialItemSlot.slotWindowType = GameDef.eItemSlotWindow.Composition_Material;
		}
		
		if (curItemSlot == null)
		{
			curItemSlot = CreateItemSlot(curItemSlotNode);
			if (curItemSlot != null)
				curItemSlot.slotWindowType = GameDef.eItemSlotWindow.Composition_Item;
		}
		if (nextItemSlot == null)
		{
			nextItemSlot = CreateItemSlot(nextItemSlotNode);
			if (nextItemSlot != null)
				nextItemSlot.slotWindowType = GameDef.eItemSlotWindow.Composition_Item;
		}
		
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		if (stringValueTable != null)
			composCashValue = stringValueTable.GetData("ComposeCashValue");
		
		SetNeedCash(composCashValue, cashInfoLabel);
		
		if (composInfoLabel != null)
			composInfoLabel.text = stringTable != null ? stringTable.GetData(composStringID) : "";
		
		SetCompositionItem(null, -1, GameDef.eItemSlotWindow.Inventory);
		SetCompositionMaterials();

		SetMode(eReinforceStep.None);
	}
	
	public void SetNeedCash(int needJewel, UILabel label)
	{
		CharInfoData charData = Game.Instance.charInfoData;
		int ownJewel = charData != null ? charData.jewel_Value : 0;
		
		string needGoldStr = "";
		if (ownJewel >= needJewel)
			needGoldStr = string.Format("{1:#,###,###,##0}", GameDef.RGBToHex(plusColor), needJewel);
		else
			needGoldStr = string.Format("{1:#,###,###,##0}", GameDef.RGBToHex(minusColor), needJewel);
		
		if (label != null)
			label.text = needGoldStr;
	}
	
	public ItemSlot CreateItemSlot(Transform root)
	{
		ItemSlot itemSlot = null;
		
		GameObject go = null;
		GameObject itemSlotPrefab = ResourceManager.LoadPrefab(itemSlotPrefabPath);
		//Vector3 origScale = Vector3.one;
		if (itemSlotPrefab != null)
		{
			go = (GameObject)Instantiate(itemSlotPrefab);
		}
		
		if (go != null)
		{
			go.transform.parent = root != null ? root : this.transform;
			
			go.transform.localPosition = Vector3.zero;
			go.transform.localScale = Vector3.one;
			go.transform.localRotation = Quaternion.identity;
				
			itemSlot = go.GetComponent<ItemSlot>();
			if (itemSlot != null)
			{
				Vector3 origLocalPos = Vector3.zero;
				GameObject labelObj = itemSlot.itemIcon.frame.reinforce.gameObject;
				origLocalPos = labelObj.transform.localPosition;
				origLocalPos.z = -70.0f;
				labelObj.transform.localPosition = origLocalPos;
				
				itemSlot.SetItem(null);
			}
		}
		
		return itemSlot;
	}
	
	public bool bReciveResult = false;
	private int newItemGrade = 0;
	public void OnCompositionResult(int slotIndex, NetErrorCode errorCode, int itemGrade)
	{
		resultErrorCode = errorCode;
		bSuccess = errorCode == NetErrorCode.OK;
		
		newItemGrade = itemGrade;
		bReciveResult = true;
		
		switch(errorCode)
		{
		case NetErrorCode.OK:
		case NetErrorCode.CompositionFailed:
			break;
		default:
			SetMode(eReinforceStep.ResultWait);
			OnClose();
			
			this.parentWindow.OnErrorMessage(errorCode, null);
			break;
		}
	}
	
	public override void Update()
	{
		base.Update();
		
		switch(curStep)
		{
		case eReinforceStep.Progress:
			float rateValue = 1.0f - (progressDelayTime / progressTime);
			if (rateValue >= 1.0f && bReciveResult == true)
			{
				//UseGold();
				
				if (bSuccess == true)
				{
					if (compositionItem != null)
					{
						//GameDef.ePlayerClass _class = Game.Instance.playerClass;
						PlayerController player = Game.Instance.player;
						//InventoryManager invenManager = null;
						EquipManager equipManager = null;
						LifeManager lifeManager = null;
						
						if (player != null)
							lifeManager = player.lifeManager;
						
						if (lifeManager != null)
						{
							//invenManager = lifeManager.inventoryManager;
							equipManager = lifeManager.equipManager;
						}
						
						if (slotWindow == GameDef.eItemSlotWindow.Equip)
						{
							equipManager.SetEquipItem(slotIndex, null);
						}
						
						this.origItem = Item.CreateItem(compositionItem);
						compositionItem.SetGradeInfo(newItemGrade, 0);
						compositionItem.SetExp(resultItemExp);
						this.resultItem = compositionItem;
						
						if (parentWindow != null)
							parentWindow.SetReinforceResult(compositionItem, slotIndex, slotWindow);
						
						if (parentWindow != null)
							parentWindow.UpdateInventoryWindow(false);
					}
				}
				else
				{
					//bSuccess = false;
				}
				
				bReciveResult = false;
				SetMode(eReinforceStep.Result);
			}
			
			if (progressBar != null)
				progressBar.sliderValue = rateValue;
			
			progressDelayTime -= Time.deltaTime;
			break;
		case eReinforceStep.Result:
			resultDelayTime -= Time.deltaTime;
			if (resultDelayTime <= 0.0f)
			{
				SetMode(eReinforceStep.ResultWait);
				
				if (bSuccess == true)
					OnResultPopup(this.origItem, this.resultItem);
				else
					CloseResultPopup();
				
				bReciveResult = false;
			}
			break;
		}
	}
	
	public void UpdateCompositionItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		compositionItem = item;
		this.slotIndex = slotIndex;
		this.slotWindow = slotWindow;
	}
	
	public void SetCompositionItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		compositionItem = item;
		this.slotIndex = slotIndex;
		this.slotWindow = slotWindow;
		
		Vector3 needGoldValue = Vector3.zero;
		if (item != null)
			needGoldValue = item.NeedGoldForComposition();
		SetNeedGold((int)needGoldValue.x);
		
		composCashValue = (int)(needGoldValue.x / 3000.0f) + 10;
		SetNeedCash(composCashValue, cashInfoLabel);
		
		if (compositionItemSlot != null)
		{
			compositionItemSlot.SetItem(item);
		}
		
		if (curItemSlot != null)
		{
			curItemSlot.SetItem(item);
			
			if (curItemGradeLabel != null)
				curItemGradeLabel.text = item != null ? GameDef.MakeItemGradeToString(item.itemGrade) : "";
		}
		
		Item nextItem = null;
		if (nextItemSlot != null && item != null && item.itemInfo != null)
		{
			nextItem = new Item();
			nextItem.SetItem(item.itemInfo.itemID);
			
			nextItem.SetItemRate(item.itemRateStep);
			
			int curGrade = item.itemGrade;
			curGrade++;
			nextItem.SetGradeInfo(curGrade, 0);
				
			nextItemSlot.SetItem(nextItem);
			
			if (nextItemGradeLabel != null)
				nextItemGradeLabel.text = nextItem != null ? GameDef.MakeItemGradeToString(nextItem.itemGrade) : "";
		}
		
		if (titleLabel != null)
		{
			if (item != null && item.itemInfo != null)
				titleLabel.text = item.itemInfo.itemName;
			else
				titleLabel.text = "None";
		}
		
		UpdateItemInfos(item, 0);
	}
	
	
	public List<CompositionMaterialInfo> materials = new List<CompositionMaterialInfo>();
	public List<CompositionMaterialInfo> addMmaterials = new List<CompositionMaterialInfo>();
	public int selectedMaterialIndex = -1;
	public int selectedAddMaterialIndex = -1;
	
	public void SetCompositionMaterials()
	{
		PlayerController player = Game.Instance.player;
		InventoryManager invenManager = null;
		
		if (player != null && player.lifeManager != null)
		{
			invenManager = player.lifeManager.inventoryManager;
		}
		
		int errorCount = 0;
		if (compositionItem == null || compositionItem.itemInfo == null ||
			compositionItem.itemGrade >= Item.limitCompositionStep ||
			compositionItem.reinforceStep < Item.limitReinforceStep)
			errorCount++;
		
		ItemInfo itemInfo = compositionItem != null ? compositionItem.itemInfo : null;
		if (itemInfo != null)
		{
			switch(itemInfo.itemType)
			{
			case ItemInfo.eItemType.Potion_1:
			case ItemInfo.eItemType.Common:
			case ItemInfo.eItemType.Costume_Body:
			case ItemInfo.eItemType.Costume_Back:
			case ItemInfo.eItemType.Costume_Head:
				errorCount++;
				break;
			}
		}
		
		materials.Clear();
		addMmaterials.Clear();
		if (invenManager != null && compositionItem != null)
		{
			invenManager.GetCompositionMaterials(compositionItem, materials);
			invenManager.GetCompositionAddMaterials(compositionItem, addMmaterials);
			
			materials.Sort(CompositionMaterialInfo.SortByDefault);
			
			if (materials.Count <= 0)
				errorCount++;
		}
		
		if (addMmaterials.Count > 0)
			selectedAddMaterialIndex = 1;
		else
			selectedAddMaterialIndex = 0;
		
		if (materials.Count > 0)
			selectedMaterialIndex = 1;
		else
			selectedMaterialIndex = 0;
		
		CompositionMaterialInfo nullInfo = new CompositionMaterialInfo();
		nullInfo.slotIndex = -1;
		nullInfo.item = null;
		materials.Insert(0, nullInfo);
		
		nullInfo = new CompositionMaterialInfo();
		nullInfo.slotIndex = -1;
		nullInfo.item = null;
		addMmaterials.Insert(0, nullInfo);
		
		UpdateMaterialItems();
		UpdateAddMaterialItems();
	}
							
	public void UpdateMaterialItems()
	{
		int nCount = materials.Count;
		if (nCount == 1)
		{
			if (compositionNode != null)
				compositionNode.SetActive(false);
			if (gotoNode != null)
				gotoNode.SetActive(true);
		}
		else
		{
			if (compositionNode != null)
				compositionNode.SetActive(true);
			if (gotoNode != null)
				gotoNode.SetActive(false);
		}
		
		CompositionMaterialInfo curInfo = null;
		if (selectedMaterialIndex >= 0 && selectedMaterialIndex < nCount)
		{
			curInfo = materials[selectedMaterialIndex];
		}
		
		if (compositionMaterialItemSlot != null)
			compositionMaterialItemSlot.SetItem(curInfo != null ? curInfo.item : null);
		
		int errorCount = 0;
		if (curInfo == null || curInfo.item == null)
			errorCount++;
		
		if (Game.Instance.isDebugTestMode == true)
			errorCount = 0;
		
		SetStartButtonEnable(errorCount == 0);
	}
	
	public void UpdateAddMaterialItems()
	{
		CompositionMaterialInfo curInfo = null;
		int nCount = this.addMmaterials.Count;
		if (selectedAddMaterialIndex >= 0 && selectedAddMaterialIndex < nCount)
			curInfo = addMmaterials[selectedAddMaterialIndex];
		
		if (compositionAddMaterialItemSlot != null)
			compositionAddMaterialItemSlot.SetItem(curInfo != null ? curInfo.item : null);
		
		
		addComposeRateValue = 0;
		if (curInfo != null && curInfo.item != null)
			addComposeRateValue = curInfo.item.itemInfo.addComposeRate;
		
		UpdateComposeRateInfo();
		
		//SetStartButtonEnable(errorCount == 0);
	}
	
	public void UpdateComposeRateInfo()
	{
		Item curItem = null;
		if (curItemSlot != null && curItemSlot.itemIcon != null)
			curItem = curItemSlot.itemIcon.item;
		
		string fieldName = "";
		if (curItem != null)
			fieldName = string.Format("ItemCompositionRate{0}", (int)curItem.itemGrade);
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		int baseRate = baseComposeRateValue;
		if (stringValueTable != null)
			baseRate = stringValueTable.GetData(fieldName);
		
		int composeRate = Mathf.Min(100, baseRate + addComposeRateValue);
		if (StorageWindow.isTutorialMode == true)
			composeRate = 100;
		
		string rateStr = string.Format("{0}%", composeRate);
		
		if (composeRateInfoLabel != null)
			composeRateInfoLabel.text = rateStr;
	}
	
	public override void SetStartButtonEnable(bool bEnable)
	{
		base.SetStartButtonEnable(bEnable);
		
		
		if (StorageWindow.isTutorialMode == true)
			bEnable = false;
		
		if (cashStartButton != null)
		{
			cashStartButton.isEnabled = bEnable;
			
			if (cashStartLabel != null)
				cashStartLabel.color = bEnable == true ? startButton.defaultColor : startButton.disabledColor;
		}
	}
	
	public void PrevMaterial()
	{
		if (StorageWindow.isTutorialMode == true)
			return;
		int nCount = materials.Count;
		
		if (selectedMaterialIndex > 0)
		{
			selectedMaterialIndex--;
		}
		else
		{
			selectedMaterialIndex = nCount - 1;
		}
		
		UpdateMaterialItems();
	}
	
	public void NextMaterial()
	{
		if (StorageWindow.isTutorialMode == true)
			return;
		int nCount = materials.Count;
		if (nCount == 0)
			selectedMaterialIndex = -1;
		else
			selectedMaterialIndex = (selectedMaterialIndex + 1) % nCount;
		
		UpdateMaterialItems();
	}
	
	public CompositionMaterialInfo GetCompsitionMaterial()
	{
		CompositionMaterialInfo curInfo = null;
		int nCount = materials.Count;
		if (selectedMaterialIndex >= 0 && selectedMaterialIndex < nCount)
			curInfo = materials[selectedMaterialIndex];
		
		return curInfo;
	}
	
	public CompositionMaterialInfo GetCompsitionAddMaterial()
	{
		CompositionMaterialInfo curInfo = null;
		int nCount = this.addMmaterials.Count;
		if (selectedAddMaterialIndex >= 0 && selectedAddMaterialIndex < nCount)
			curInfo = this.addMmaterials[selectedAddMaterialIndex];
		
		return curInfo;
	}
	
	public override void SendRequest()
	{
		SendRequestCompose(false);
	}
	
	
	public void PrevAddMaterial()
	{
		int nCount = addMmaterials.Count;
		
		if (selectedAddMaterialIndex > 0)
		{
			selectedAddMaterialIndex--;
		}
		else
		{
			selectedAddMaterialIndex = nCount - 1;
		}
		
		UpdateAddMaterialItems();
	}
	
	public void NextAddMaterial()
	{
		int nCount = this.addMmaterials.Count;
		if (nCount == 0)
			selectedAddMaterialIndex = -1;
		else
			selectedAddMaterialIndex = (selectedAddMaterialIndex + 1) % nCount;
		
		UpdateAddMaterialItems();
	}
	
	public void OnStartCash()
	{
		if (curStep == eReinforceStep.Wait ||
			curStep == eReinforceStep.ResultWait)
		{
			//UseGold();
			
			CashItemType cashCheck = PopupBaseWindow.CheckNeedGold(0, this.composCashValue, 0);
			
			if (cashCheck != CashItemType.None)
			{
				OnNeedMoneyPopup(cashCheck, this);
				return;
			}
			
			
			SendRequestByCash();
			
			SetStartButtonEnable(false);
			SetMode(eReinforceStep.Progress);
		}
	}
	
	public void SendRequestByCash()
	{
		SendRequestCompose(true);
	}
	
	public void SendRequestCompose(bool bCash)
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			CompositionMaterialInfo curInfo = GetCompsitionMaterial();
			string composMaterialUID = "";
			if (curInfo != null && curInfo.item != null)
				composMaterialUID = curInfo.item.uID;
			
			CompositionMaterialInfo addMaterialInfo = GetCompsitionAddMaterial();
			string composAddMaterialUID = "";
			if (addMaterialInfo != null && addMaterialInfo.item != null)
				composAddMaterialUID = addMaterialInfo.item.uID;
			
			packetSender.SendRequestCompositionItem(this.slotIndex, this.slotWindow, 
																			compositionItem.uID, compositionItem.itemInfo.itemID, 
																			composMaterialUID, composAddMaterialUID, bCash, StorageWindow.isTutorialMode);
		}
	}
	
	public override void OnClose ()
	{
		if (StorageWindow.isTutorialMode == true &&
			this.curStep != BaseReinforceWindow.eReinforceStep.ResultWait)
			return;
		
		base.OnClose ();
	}
	
	
	public string gotoWindowPrefab = "UI/Item/Composition/CompositionMaterialWindow";
	public void OnGoto()
	{
		CompositionMaterialWindow gotoWindow = GetPopup<CompositionMaterialWindow>();
		
		if (gotoWindow == null)
			gotoWindow = ResourceManager.CreatePrefab<CompositionMaterialWindow>(gotoWindowPrefab, popupNode);
		
		if (gotoWindow != null)
		{
			gotoWindow.parentWindow = this;
			gotoWindow.SetItem(compositionItem);
			
			this.popupList.Add(gotoWindow);
		}
	}
	
	public override void OnResultPopup(Item origItem, Item reinforceItem)
	{
		ReinforceResult resultPopup = ResourceManager.CreatePrefab<ReinforceResult>(resultPopupPrefab, popupNode);
		resultPopup.SetResultCompoitionItem(origItem, reinforceItem);
		resultPopup.parentWindow = this;
		
		this.popupList.Add(resultPopup);
	}
	
	public override void CloseResultPopup()
	{
		OnClose();
	}
}
