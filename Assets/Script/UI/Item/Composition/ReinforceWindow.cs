using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ReinforceWindow : BaseReinforceWindow 
{
	public UILabel nextReinfoceTitle = null;
	public int nextReinforceStringTableID = -1;
	
	public ItemSlotPanel reinforceMaterialPanel = null;
	public ItemSlot reinforceItemSlot = null;
	public ItemSlot reinforceInfoItemSlot = null;
	
	public string itemSlotPrefabPath = "UI/Item/ItemSlot";
	public Transform itemSlotNode = null;
	public Transform itemInfoSlotNode = null;
	
	public Item reinforceItem = null;
	public int slotIndex = -1;
	public GameDef.eItemSlotWindow slotWindow = GameDef.eItemSlotWindow.Inventory;
	
	public int nextReinforceStep = -1;
	
	public UILabel materialStageActInfoLabel = null;
	public int materialStageActInfoStringID = -1;
	
	float reinforceNeedMoneyRate = 1.0f;
	int limitReinforceStep = 30;
	public override void Awake()
	{
		base.Awake();
		
		CreateItemSlot();
		
		SetReinforceItem(null, -1, GameDef.eItemSlotWindow.Inventory);
		SetMode(eReinforceStep.None);
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		if (stringTable != null && costLabel != null)
			costLabel.text = stringTable.GetData(costLabelStringID);
		
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		if (stringValueTable != null)
		{
			limitReinforceStep = stringValueTable.GetData("ReinforceMax");
			reinforceNeedMoneyRate = stringValueTable.GetData("ReinforcePrice") * 0.01f;
		}
		
		reinforceStepFormatStr = stringTable.GetData(reinforceStepStringID);
	}
	
	public void CreateItemSlot()
	{
		if (reinforceItemSlot != null)
			return;
		
		if (reinforceItemSlot == null)
		{
			reinforceItemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefabPath, itemSlotNode, Vector3.zero);
			if (reinforceItemSlot != null)
			{
				reinforceItemSlot.slotWindowType = GameDef.eItemSlotWindow.Reinforce_Item;
				reinforceItemSlot.SetItem(null);
			}
		}
		
		if (reinforceInfoItemSlot == null)
		{
			reinforceInfoItemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefabPath, itemInfoSlotNode, Vector3.zero);
			if (reinforceInfoItemSlot != null)
			{
				reinforceInfoItemSlot.slotWindowType = GameDef.eItemSlotWindow.Reinforce_Item;
				reinforceInfoItemSlot.SetItem(null);
			}
		}
	}
	
	public bool bReciveResult = false;
	public int newReinforceStep = 0;
	public void OnReinforceResult(NetErrorCode err, ref BaseTradeItemInfo info, int reinforceStep)
	{
		resultErrorCode = err;
		bSuccess = (resultErrorCode == NetErrorCode.OK);
		
		newReinforceStep = reinforceStep;
		bReciveResult = true;
	}
	
	public override void SetResultFX(NetErrorCode errorCode)
	{
		if (resultErrorCode == NetErrorCode.OK)
			SetFX(successFx, bSuccess);
		else
			//SetFX(failFx, !bSuccess);
			OnErrorMessage(resultErrorCode, null);
	}
	
	public override void Update()
	{
		switch(curStep)
		{
		case eReinforceStep.Wait:
			UpdateExpRate();
			break;
		case eReinforceStep.Progress:
			float rateValue = 1.0f - (progressDelayTime / progressTime);
			if (rateValue >= 1.0f && bReciveResult == true)
			{
				// 돈은 서버가 깎는다.
				// UseGold();
				
				if (resultErrorCode == NetErrorCode.OK)
				{
					if (reinforceItem != null)
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
						
						this.origItem = Item.CreateItem(reinforceItem);
						reinforceItem.SetExp(resultItemExp);
						this.resultItem = reinforceItem;
						
						if (parentWindow != null)
							parentWindow.SetReinforceResult(reinforceItem, slotIndex, slotWindow);
					}
				}
				else
				{
					nextReinforceStep = -1;
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
				if (bSuccess == true)
				{
					if (reinforceItem == null || reinforceItem.reinforceStep >= Item.limitReinforceStep)
					{
						SetMode(eReinforceStep.ResultWait);
						
						//OnClose();
					}
					else
					{
						SetReinforceItem(reinforceItem, slotIndex, slotWindow);
						int materialID = -1;
						if (reinforceItem != null)
						{
							materialID = reinforceItem.GetMaterialID();
						}
						
						if (materialItemGrid != null)
						{
							if (materialItemGrid.gameObject.transform.parent != null)
							{
								UIPanel panel = materialItemGrid.gameObject.transform.parent.gameObject.GetComponent<UIPanel>();
								if (panel != null)
								{
									Vector3 origPos = panel.transform.localPosition;
									origPos.x = origPos.y = 0.0f;
									panel.transform.localPosition = origPos;
									
									Vector4 clipRange = panel.clipRange;
									clipRange.x = clipRange.y = 0.0f;
									panel.clipRange = clipRange;
								}
							}
						}
						
						SetMode(ReinforceWindow.eReinforceStep.Wait);
					}
					
					OnResultPopup(this.origItem, this.resultItem);
				}
				else
				{
					SetMode(ReinforceWindow.eReinforceStep.Wait);
				}
				
				bReciveResult = false;
			}
			break;
		}
	}
	
	public void UpdateReinforceItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		reinforceItem = item;
		this.slotIndex = slotIndex;
		this.slotWindow = slotWindow;
	}
	
	
	public Item reinforceInfoItem = null;
	public void SetReinforceItem(Item item, int slotIndex, GameDef.eItemSlotWindow slotWindow)
	{
		resultErrorCode = NetErrorCode.OK;
		
		reinforceItem = item;
		this.slotIndex = slotIndex;
		this.slotWindow = slotWindow;
		
		/*
		Vector3 needGoldValue = Vector3.zero;
		if (item != null)
			needGoldValue = item.NeedGoldForReinforce();
		SetNeedGold(needGoldValue);
		*/
		
		if (reinforceItemSlot != null)
		{
			reinforceItemSlot.SetItem(item);
		}
		
		if (reinforceInfoItemSlot != null)
		{
			reinforceInfoItemSlot.SetItem(item);
		}
		
		if (titleLabel != null)
		{
			if (item != null && item.itemInfo != null)
				titleLabel.text = item.itemInfo.itemName;
			else
				titleLabel.text = "None";
		}
		
		if (item != null)
			reinforceInfoItem = Item.CreateItem(item);
		else
			reinforceInfoItem = null;
		
		this.nowStep = item != null ? item.reinforceStep : 0;
		
		UpdateItemInfos(reinforceInfoItem, 0);
		UpdateReinforceStep(reinforceInfoItem);
		
		MakeReinforceMaterialItems(item);
	}
	
	public void UpdateReinforceStep(Item item)
	{
		if (nextReinfoceTitle != null)
		{
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = null;
			if (tableManager != null)
				stringTable = tableManager.stringTable;
			
			if (item != null && item.itemInfo != null)
			{
				int nextReinforce = Mathf.Min(item.reinforceStep, Item.limitReinforceStep);
				string titleMsg = "Item Infos";
				if (stringTable != null)
					titleMsg = stringTable.GetData(nextReinforceStringTableID);
				
				nextReinfoceTitle.text = string.Format("+{0:D1} {1}", nextReinforce, titleMsg);
			}
			else
				nextReinfoceTitle.text = "";
		}
	}
	
	public override void SendRequest()
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			//packetSender.SendRequestReinforceItem(this.slotIndex, this.slotWindow, reinforceItem.uID, reinforceItem.itemInfo.itemID);
			
			List<string> delList = new List<string>();
			foreach(Item item in usedItemList)
				delList.Add(item.uID);
				
			packetSender.SendRequestReinforceItem(this.slotIndex, this.slotWindow, reinforceItem.uID, reinforceItem.itemInfo.itemID, delList.ToArray());
		}
	}
	
	public override void OnClose ()
	{
		if (StorageWindow.isTutorialMode == true &&
			this.curStep != BaseReinforceWindow.eReinforceStep.Wait)
			return;
		
		curItemExpValue = nextItemExpValue = 0;
		SetReinforceStep(0, 0);
		targetCurItemExpValue = targetNextItemExpValue = 0.0f;
		nowStep = startStep = endStep = 0;
		
		base.OnClose ();
	}
	
	
	
	public int limitItemCount = 10;
	
	public List<Item> usedItemList = new List<Item>();
	public List<ItemSlot> usedItemSlotList = new List<ItemSlot>();
	
	public List<Item> materialItemList = new List<Item>();
	public List<ItemSlot> materialItemSlotList = new List<ItemSlot>();
	
	public UIGrid materialItemGrid = null;
	public UIGrid usedItemGrid = null;
	
	public void MakeReinforceMaterialItems(Item reinforceItem)
	{
		materialItemList.Clear();
		usedItemList.Clear();
		
		foreach(ItemSlot itemSlot in usedItemSlotList)
			DestroyObject(itemSlot.gameObject, 0.1f);
		
		usedItemSlotList.Clear();
		
		foreach(ItemSlot itemSlot in materialItemSlotList)
			DestroyObject(itemSlot.gameObject, 0.1f);
		
		materialItemSlotList.Clear();
		
		if (reinforceItem == null)
			return;
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			if (StorageWindow.isTutorialMode != true)
			{
				//일반 아이템 탭.
				foreach(Item temp in charData.inventoryNormalData)
				{
					if (temp == null)
						continue;
					
					if (temp == reinforceItem)
						continue;
					
					if (temp.GetItemExp() == 0)
						continue;
					
					materialItemList.Add(temp);
				}
			}
			
			
			//재료 탭.
			foreach(Item temp in charData.inventoryMaterialData)
			{
				if (temp == null)
					continue;
				
				if (temp == reinforceItem)
					continue;
				
				if (temp.GetItemExp() == 0)
					continue;
				
				materialItemList.Add(temp);
			}
			
			
			materialItemList.Sort(Game.SortReinforceItem);
		}
		
		Invoke("SetMaterialItems", 0.2f);
		UpdateUsedItemList();
	}
	
	public void SetMaterialItems()
	{
		foreach(Item item in materialItemList)
		{
			ItemSlot itemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefabPath, materialItemGrid.transform, Vector3.zero);
			if (itemSlot != null)
			{
				if (itemSlot.buttonMsg != null)
				{
					itemSlot.buttonMsg.target = this.gameObject;
					itemSlot.buttonMsg.functionName = "OnSelectMaterial";
				}
				
				itemSlot.SetItem(item);
				materialItemSlotList.Add(itemSlot);
			}
		}
		
		if (materialItemGrid != null)
			materialItemGrid.repositionNow = true;
	}
	
	public void OnSelectMaterial(GameObject obj)
	{
		ItemSlot itemSlot = obj.transform.parent.GetComponent<ItemSlot>();
		if (itemSlot != null)
			OnSelectMaterial(itemSlot);
	}
	
	public string limitReinforcePopupPrefab = "UI/Item/Composition/LimitReinforcePopup";
	public BaseConfirmPopup limitReinforcePopup = null;
	public void OnSelectMaterial(ItemSlot itemSlot)
	{
		Item item = null;
		
		if (itemSlot != null)
			item = itemSlot.GetItem();
		
		if (item != null)
		{
			if (usedItemList.Contains(item) == false)
			{
				uint addExp = 0;
				foreach(Item temp in usedItemList)
					addExp += temp.GetItemExp();
				
				ItemReinforceInfo curReinfoceInfo = Item.GetReinforceInfo(reinforceItem.itemInfo.baseExp, reinforceItem.itemExp + addExp, reinforceItem.itemRateValue);
				if (curReinfoceInfo.step >= limitReinforceStep)
				{
					if (limitReinforcePopup == null)
						limitReinforcePopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(limitReinforcePopupPrefab, popupNode, Vector3.zero);
					
					if (limitReinforcePopup != null)
					{
						if (limitReinforcePopup.okButtonMessage != null)
						{
							limitReinforcePopup.okButtonMessage.target = this.gameObject;
							limitReinforcePopup.okButtonMessage.functionName = "OnCancelPopup";
						}
						
						this.popupList.Add(limitReinforcePopup);
					}
					
					return;
				}
				
				if (usedItemList.Count < limitItemCount)
				{
					if (itemSlot != null)
						itemSlot.SetSelected(true);
					
					usedItemList.Add(item);
					
					ItemSlot newItemSlot = ResourceManager.CreatePrefab<ItemSlot>(itemSlotPrefabPath, usedItemGrid.transform, new Vector3(0.0f, 0.0f, -10.0f));
					if (newItemSlot != null)
					{
						if (newItemSlot.buttonMsg != null)
						{
							newItemSlot.buttonMsg.target = this.gameObject;
							newItemSlot.buttonMsg.functionName = "OnUnSelectMaterial";
						}
						
						newItemSlot.SetItem(item);
						usedItemSlotList.Add(newItemSlot);
						
						Invoke("UpdateUsedItemList", 0.1f);
					}
				}
			}
			else
			{
				ItemSlot unSelectSlot = null;
				foreach(ItemSlot tempSlot in this.usedItemSlotList)
				{
					if (tempSlot.GetItem() == item)
						unSelectSlot = tempSlot;
				}
				
				OnUnSelectMaterial(unSelectSlot);
			}
		}
	}
	
	public void OnCancelPopup(GameObject obj)
	{
		ClosePopup();
	}
	
	public void OnUnSelectMaterial(GameObject obj)
	{
		ItemSlot itemSlot = obj.transform.parent.GetComponent<ItemSlot>();
		if (itemSlot != null)
			OnUnSelectMaterial(itemSlot);
	}
	
	public void OnUnSelectMaterial(ItemSlot itemSlot)
	{
		Item item = null;
		if (itemSlot != null)
			item = itemSlot.GetItem();
		
		if (item != null)
		{
			if (usedItemList.Contains(item) == true)
			{
				foreach(ItemSlot tempSlot in this.materialItemSlotList)
				{
					if (tempSlot.GetItem() == item)
						tempSlot.SetSelected(false);
				}
				
				usedItemList.Remove(item);
				usedItemSlotList.Remove(itemSlot);
				
				DestroyObject(itemSlot.gameObject, 0.0f);
				
				Invoke("UpdateUsedItemList", 0.1f);
			}
		}
	}
	
	public UISlider itemCurExpSlider = null;
	public UISlider itemNextExpSlider = null;
	public UILabel itemCurExpLabel = null;
	
	public void UpdateUsedItemList()
	{
		if (usedItemGrid != null)
			usedItemGrid.repositionNow = true;
		
		uint addExp = 0;
		foreach(Item temp in usedItemList)
			addExp += temp.GetItemExp();
		
		uint newItemExp = 0;
		if (reinforceItem != null)
			newItemExp = reinforceItem.itemExp;
		newItemExp += addExp;
		
		Vector3 needGoldValue = Item.NeedGoldForReinforce(addExp);
		SetNeedGold((int)needGoldValue.x);
		
		ItemReinforceInfo curReinfoceInfo = Item.GetReinforceInfo(reinforceItem.itemInfo.baseExp, reinforceItem.itemExp, reinforceItem.itemRateValue);
		ItemReinforceInfo nextReinfoceInfo = Item.GetReinforceInfo(reinforceItem.itemInfo.baseExp, reinforceItem.itemExp + addExp, reinforceItem.itemRateValue);
		
		SetReinforceStep(curReinfoceInfo.step, nextReinfoceInfo.step);
		
		if (curReinfoceInfo.step == nextReinfoceInfo.step)
		{
			if (itemCurExpSlider != null)
				itemCurExpSlider.gameObject.SetActive(true);
			
			SetItemCurExpValue(itemCurExpSlider, null, reinforceItem.itemExp, curReinfoceInfo);
			SetItemNextExpValue(itemNextExpSlider, itemCurExpLabel, reinforceItem.itemExp + addExp, nextReinfoceInfo);
		}
		else
		{
			if (itemCurExpSlider != null)
				itemCurExpSlider.gameObject.SetActive(false);
			
			SetItemCurExpValue(itemCurExpSlider, null, reinforceItem.itemExp, curReinfoceInfo);
			SetItemNextExpValue(itemNextExpSlider, itemCurExpLabel, reinforceItem.itemExp + addExp, nextReinfoceInfo);
		}
		
		SetStartButtonEnable(usedItemList.Count > 0);
		
		UpdateReinforceNeedMoney(addExp);
		
		UpdateItemInfos(reinforceItem, addExp);
		
		if (reinforceInfoItem != null)
			reinforceInfoItem.SetExp(newItemExp);
		
		UpdateReinforceStep(reinforceInfoItem);
	}
	
	public void UpdateReinforceNeedMoney(uint addExp)
	{
		int needGoldValue = (int)(addExp * reinforceNeedMoneyRate);
		
		SetNeedGold(needGoldValue);
	}
	
	public void SetItemCurExpValue(UISlider slider, UILabel valueLabel, uint itemExpValue, ItemReinforceInfo info)
	{
		uint needExpValue = info.limitExp - info.startExp;
		uint curExpValue = itemExpValue - info.startExp;
		
		curItemExpValue = (float)curExpValue / (float)needExpValue;
		
		//needExpValue = info.limitExp;
		//curExpValue = itemExpValue;
		
		SetItemExpValue(slider, valueLabel, curItemExpValue, curExpValue, needExpValue);
		
		//this.targetNextItemExpValue = curItemExpValue;
	}
	
	public void SetItemNextExpValue(UISlider slider, UILabel valueLabel, uint itemExpValue, ItemReinforceInfo info)
	{
		uint needExpValue = info.limitExp - info.startExp;
		uint curExpValue = itemExpValue - info.startExp;
		if (info.limitExp == info.startExp)
		{
			needExpValue = info.startExp;
			curExpValue = info.startExp;
		}
		
		nextItemExpValue = (float)curExpValue / (float)needExpValue;
		
		//needExpValue = info.limitExp;
		//curExpValue = itemExpValue;
		
		SetItemExpValue(slider, valueLabel, nextItemExpValue, curExpValue, needExpValue);
	}
	
	private void SetItemExpValue(UISlider slider, UILabel valueLabel, float expRate, uint curExp, uint needExp)
	{
		if (slider != null)
			slider.sliderValue = expRate;
		
		if (valueLabel != null)
			valueLabel.text = string.Format("{0:N0} / {1:N0}", curExp, needExp);
	}
	
	private float curItemExpValue = 0.0f;
	private float targetCurItemExpValue = 0.0f;
	private float nextItemExpValue = 0.0f;
	private float targetNextItemExpValue = 0.0f;
	
	public UILabel curReinforceStepLabel = null;
	public UILabel nextReinforceStepLabel = null;
	public Color nextStepColor = Color.yellow;
	public int reinforceStepStringID = 256;
	private string reinforceStepFormatStr = "";
	private void SetReinforceStep(int curStep, int nextStep)
	{
		Color labelColor = Color.white;
		if (curStep != nextStep)
			labelColor = nextStepColor;
		
		if (curReinforceStepLabel != null)
			curReinforceStepLabel.text = string.Format(reinforceStepFormatStr, curStep);
		
		if (nextReinforceStepLabel != null)
		{
			nextReinforceStepLabel.color = labelColor;
			nextReinforceStepLabel.text = string.Format(reinforceStepFormatStr, nextStep);
		}
		
		startStep = curStep;
		//nowStep = 
		endStep = nextStep;
	}
	
	private int startStep = 0;
	private int endStep = 0;
	private int nowStep = 0;
	private void UpdateExpRate()
	{
		targetCurItemExpValue = Mathf.Lerp(targetCurItemExpValue, curItemExpValue, 0.3f);
		
		float targetExp = 1.0f;
		if (nowStep == endStep)
			targetExp = nextItemExpValue;
		else
		{
			if (nowStep < endStep)
				targetExp = 1.0f;
			else
				targetExp = 0.0f;
		}
				
		targetNextItemExpValue = Mathf.Lerp(targetNextItemExpValue, targetExp, 0.3f);
		
		if (Mathf.Abs(targetExp - targetNextItemExpValue) <= 0.01f)
		{
			targetNextItemExpValue = targetExp;
			if (nowStep != endStep)
			{
				if (nowStep < endStep)
				{
					targetNextItemExpValue = 0.0f;
					nowStep = Mathf.Min(endStep, nowStep + 1);
				}
				else
				{
					targetNextItemExpValue = 1.0f;
					nowStep = Mathf.Max(endStep, nowStep - 1);
				}
			}
		}
		
		//Logger.DebugLog(string.Format("exp value : {0} -> {1}", targetNextItemExpValue, targetExp));
		
		if (itemCurExpSlider != null)
			itemCurExpSlider.sliderValue = targetCurItemExpValue;
		if (itemNextExpSlider != null)
			itemNextExpSlider.sliderValue = targetNextItemExpValue;
	}
	
	public override void OnResultPopup(Item origItem, Item reinforceItem)
	{
		//강화 단계가 변경(증가?) 될 때만 팝업 표시.
		if (origItem != null && reinforceItem != null)
		{
			if (origItem.reinforceStep != reinforceItem.reinforceStep)
			{
				ReinforceResult resultPopup = ResourceManager.CreatePrefab<ReinforceResult>(resultPopupPrefab, popupNode);
				resultPopup.SetResultReinforceItem(origItem, reinforceItem);
				resultPopup.parentWindow = this;
				
				this.popupList.Add(resultPopup);
			}
		}
	}
	
	public override void CloseResultPopup()
	{
		ClosePopup<ReinforceResult>();
		
		if (this.resultItem == null || this.resultItem.reinforceStep >= Item.limitReinforceStep)
		{
			OnClose();
		}
	}
}
