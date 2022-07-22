using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GambleProgressWindow : MonoBehaviour {
	public enum eGambleMode
	{
		Wait,
		Progress,
		Result,
		Result_Wait,
	}
	public eGambleMode curMode = eGambleMode.Wait;
	
	public float progressDurationTime = 2.5f;
	public float suffleTime = 0.2f;
	public float suffleDelayTime = 0.0f;
	
	public float resultDurationTime = 1.0f;
	
	public float delayTime = 0.0f;
	
	public GambleWindow parent = null;
	
	public int resultStringID = -1;
	public UILabel titleLabel = null;
	public GameObject goStorageButton = null;
	public GameObject closeButton = null;
	
	public UILabel cashTitleLabel = null;
	public UILabel goldTitleLabel = null;
	
	public GameObject cashGoStorageButton = null;
	public GameObject cashCloseButton = null;
	
	public UIButton oneMoreByCash = null;
	public UILabel oneMoreByCashLabel = null;
	public UIButton oneMoreByCoupon = null;
	public UILabel oneMoreByCouponLabel = null;
	
	public GameObject normalGoStorageButton = null;
	public GameObject normalCloseButton = null;
	
	public string itemSlotPrefabPath = "UI/Item/ItemSlot";
	public Transform itemSlotNodeByCash = null;
	public Transform itemSlotNodeByNormal = null;
	public ItemSlot itemSlot = null;
	
	public UISprite gambleItemGrade = null;
	public UISprite cashItemGrade = null;
	public UISprite goldItemGrade = null;
	public List<string> gambleItemGradeSpriteNames = new List<string>();
	
	public Item gambleResultItem = null;
	
	public List<Item> gambleItems = new List<Item>();
	public Dictionary<int, int> randomIDs = new Dictionary<int, int>();
	
	void Awake()
	{
		//itemSlot = CreateItemSlot(itemSlotNode);
		gambleResultItem = null;
		
		GameUI.Instance.gambleProgressWindow = this;
		
	}
	
	public GambleWindow.eGambleType gambleType = GambleWindow.eGambleType.ByGold;
	
	public GameObject cashEffect = null;
	public GameObject normalEffect = null;
	void Start()
	{
		AudioSource audioSource = null;
		string animName = "";
		if (cashEffect != null)
		{
			audioSource = cashEffect.audio;
			if (audioSource != null)
				audioSource.mute = !GameOption.effectToggle;
			
			cashEffect.SetActive(gambleType != GambleWindow.eGambleType.ByGold);
		}
		
		if (normalEffect != null)
		{
			audioSource = normalEffect.audio;
			if (audioSource != null)
				audioSource.mute = !GameOption.effectToggle;
			
			normalEffect.SetActive(gambleType == GambleWindow.eGambleType.ByGold);
		}
		
		Transform root = null;
		
		switch(gambleType)
		{
		case GambleWindow.eGambleType.ByCash:
		case GambleWindow.eGambleType.ByCoupon:
			root = itemSlotNodeByCash;
			
			goStorageButton = cashGoStorageButton;
			closeButton = cashCloseButton;
			
			titleLabel = cashTitleLabel;
			
			gambleItemGrade = cashItemGrade;
			
			SetOneMoreInfo(gambleType);
			break;
		case GambleWindow.eGambleType.ByGold:
			root = itemSlotNodeByNormal;
			
			goStorageButton = normalGoStorageButton;
			closeButton = normalCloseButton;
			
			titleLabel = goldTitleLabel;
			
			gambleItemGrade = goldItemGrade;
			break;
		}
		
		itemSlot = CreateItemSlot(root);
	}
	
	public void SetOneMoreInfo(GambleWindow.eGambleType type)
	{
		CharInfoData charData = Game.Instance.charInfoData;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = null;
		StringValueTable stringValueTable = null;
		if (tableManager != null)
		{
			stringTable = tableManager.stringTable;
			stringValueTable = tableManager.stringValueTable;
		}
		
		int couponCount = 0;
		Vector3 gambleJewel = Vector3.zero;
		
		if (parent != null)
			gambleJewel = parent.oneMoreGambleCashPriceValue;
		
		if (charData != null)
			couponCount = charData.gambleCoupon;
		
		bool isAvailable = false;
		
		switch(type)
		{
		case GambleWindow.eGambleType.ByCash:
			CashItemType checkType = BaseItemWindow.CheckNeedGold(gambleJewel);
			isAvailable = checkType == CashItemType.None;
			
			if (oneMoreByCash != null)
			{
				oneMoreByCash.gameObject.SetActive(true);
				oneMoreByCash.isEnabled = true;
			}
			
			if (oneMoreByCoupon != null)
				oneMoreByCoupon.gameObject.SetActive(false);
						
			if (oneMoreByCashLabel != null)
				oneMoreByCashLabel.text = string.Format("{0:#,###,###}", gambleJewel.y);
			
			break;
		case GambleWindow.eGambleType.ByCoupon:
			isAvailable = couponCount > 0;
			
			if (oneMoreByCash != null)
				oneMoreByCash.gameObject.SetActive(false);
			
			if (oneMoreByCoupon != null)
			{
				oneMoreByCoupon.gameObject.SetActive(isAvailable);
				oneMoreByCash.isEnabled = isAvailable;
			}
			
			if (oneMoreByCouponLabel != null)
			{
				if (isAvailable == true)
					oneMoreByCouponLabel.text = string.Format("{0:#,###,###}", couponCount);
				else
					oneMoreByCouponLabel.text = string.Format("{0}{1:#,###,###}[-]", GameDef.RGBToHex(Color.red), couponCount);
			}
			
			if (parent != null)
				parent.UpdateCouponCount();
			
			break;
		case GambleWindow.eGambleType.ByGold:
			break;
		}
	}
	
	public void SetGambleItemIDs(List<Item> gamblItems)
	{
		gambleItems.Clear();
		gambleItems.AddRange(gamblItems);
		
		randomIDs.Clear();
		
		SetMode(eGambleMode.Progress);
	}
	
	public void SetMode(eGambleMode mode)
	{
		switch(mode)
		{
		case eGambleMode.Progress:
			delayTime = progressDurationTime;
			suffleDelayTime = suffleTime;
			SetButton(false);
			break;
		case eGambleMode.Wait:
			SetButton(false);
			break;
		case eGambleMode.Result:
			delayTime = resultDurationTime;
			SetButton(false);
			
			SetGambleItemInfo(gambleResultItem, gambleIndex);
			break;
		case eGambleMode.Result_Wait:
			SetButton(true);
			break;
		}
		
		curMode = mode;
	}
	
	public void SetButton(bool bActive)
	{
		if (goStorageButton != null)
			goStorageButton.SetActive(bActive);
		if (closeButton != null)
			closeButton.SetActive(bActive);
	}
	
	public void SuffleItems()
	{
		Item randItem = null;
		
		int nCount = gambleItems.Count;
		if (nCount > 0)
		{
			int randIndex = Random.Range(0, nCount);
			while(randomIDs.ContainsKey(randIndex) == true)
			{
				randIndex = Random.Range(0, nCount);
			}
			randomIDs.Add(randIndex, randIndex);
			if (randomIDs.Count == nCount)
				randomIDs.Clear();
			
			randItem = gambleItems[randIndex];
		}
		
		if (itemSlot != null)
			itemSlot.SetItem(randItem);
		
		if (titleLabel != null)
		{
			string itemName = "";
			if (randItem != null && randItem.itemInfo != null)
				itemName = randItem.itemInfo.itemName;
			
			titleLabel.text = itemName;
		}
	}
	
	public void Update()
	{
		switch(curMode)
		{
		case eGambleMode.Wait:
			break;
		case eGambleMode.Progress:
			if (suffleDelayTime <= 0.0f)
			{
				SuffleItems();
				suffleDelayTime = suffleTime;
			}
			
			if (delayTime <= 0.0f && brecievedResult == true)
			{
				SetMode(eGambleMode.Result);
			}
			
			delayTime -= Time.deltaTime;
			suffleDelayTime -= Time.deltaTime;
			break;
		case eGambleMode.Result:
			if (delayTime <= 0.0f)
			{
				SetMode(eGambleMode.Result_Wait);
			}
			delayTime -= Time.deltaTime;
			break;
		case eGambleMode.Result_Wait:
			break;
		}
	}
	
	public void AddInventory(Item item)
	{
		if (item == null || item.itemInfo == null)
			return;
		
		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		InventoryManager invenManager = lifeManager != null ? lifeManager.inventoryManager : null;
		
		if (invenManager != null)
		{
			switch(item.itemInfo.itemType)
			{
			case ItemInfo.eItemType.Costume_Body:
			case ItemInfo.eItemType.Costume_Head:
			case ItemInfo.eItemType.Costume_Back:
				invenManager.AddCostume(item);
				break;
			default:
				invenManager.AddItem(item);
				break;
			}
		}
	}
	
	public void SetGambleItemInfo(Item item, int index)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (itemSlot != null)
			itemSlot.SetItem(item);
		
		string titleMsg = "";
		if (titleLabel != null)
		{
			if (errorCode == NetErrorCode.OK)
			{
				string itemName = "";
				string msg = stringTable != null ? stringTable.GetData(resultStringID) : "";
				
				if (item != null && item.itemInfo != null)
				{
					itemName = item.itemInfo.itemName;
				
					titleMsg = string.Format("{0} {1}!!!", itemName, msg);
				}
				else
				{
					titleMsg = string.Format("GambleError");
				}
			}
			else
			{
				titleMsg = string.Format("GambleError");
			}
			
			titleLabel.text = titleMsg;
		}
		
		int gambleGradeIndex = -1;
		if (index != -1)
			gambleGradeIndex = index / GambleWindow.gambleItemListCount;
		
		string gambleGradeSpriteName = GetGambleGradeSpriteName(gambleGradeIndex);
		if (gambleItemGrade != null)
		{
			gambleItemGrade.gameObject.SetActive(gambleGradeSpriteName != "");
			gambleItemGrade.spriteName = gambleGradeSpriteName;
		}
		
		//갬블 아이템 슬롯 갱신...
		if (this.parent != null)
			this.parent.UpdateGambleInfo(gambleIndex);
		
		if (this.errorCode != NetErrorCode.OK || titleMsg == "GambleError")
		{
			//겜블 에러가 발생했으면.. 겜블진행창은 닫는다...
			OnClose();
			
			switch(this.errorCode)
			{
			case NetErrorCode.NotEnoughInven:
			case NetErrorCode.NotEnoughInvenMaterial:
				this.parent.OnNeedInventoryPopup();
				break;
			default:
				string errorMsg = "Gamble Error!!!";
				if (stringTable != null)
					errorMsg = stringTable.GetData((int)this.errorCode);
				
				if (GameUI.Instance.MessageBox != null)
					GameUI.Instance.MessageBox.SetMessage(errorMsg);
				break;
			}
		}
	}
	
	public string GetGambleGradeSpriteName(int index)
	{
		int nCount = gambleItemGradeSpriteNames.Count;
		string spriteName = "";
		if (index >= 0 && index < nCount)
			spriteName = gambleItemGradeSpriteNames[index];
		
		return spriteName;
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
				itemSlot.slotWindowType = GameDef.eItemSlotWindow.Gamble;
				
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
	
	/*
	public Item MakeGambleItem()
	{
		int randValue = Random.Range(0, 1000);
		
		int beginIndex = 0;
		int endIndex = 0;
		
		int nCount = gambleItems.Count;
		
		int startValue = 0;
		int endValue = 600;
		if (randValue >= startValue && randValue < endValue)
		{
			beginIndex = 0;
			endIndex = Mathf.Min(4, nCount);
			
			return SelectItem(beginIndex, endIndex);
		}
		
		startValue = endValue;
		endValue += 300;
		if (randValue >= startValue && randValue < endValue)
		{
			beginIndex = 4;
			endIndex = Mathf.Min(8, nCount);
			
			return SelectItem(beginIndex, endIndex);
		}
		
		startValue = endValue;
		endValue += 100;
		if (randValue >= startValue && randValue < endValue)
		{
			beginIndex = 8;
			endIndex = Mathf.Min(12, nCount);
			
			return SelectItem(beginIndex, endIndex);
		}
		
		return null;
	}
	*/
	
	public Item SelectItem(int beginIndex, int endIndex)
	{
		int nCount = gambleItems.Count;
		
		
		int itemIndex = -1;
		Item resultItem = null;
		Item selectItem = null;
		
		itemIndex = Random.Range(beginIndex, endIndex);
		if (itemIndex >= 0 && itemIndex < nCount)
			selectItem = gambleItems[itemIndex];
		
		if (selectItem != null)
		{
			resultItem = Item.CreateItem(selectItem);
		}
		
		return resultItem;
	}
	
	public void OnGoStorage()
	{
		this.parent.OnStorage();
	}
	
	public void OnClose()
	{
		this.parent.CloseGambleProgress();
	}
	
	private NetErrorCode errorCode = NetErrorCode.OK;
	private int gambleIndex = -1;
	private bool brecievedResult = false;
	public void OnGambleResultItem(NetErrorCode errorCode, Item resultItem, int selectedIndex)
	{
		brecievedResult = true;
		
		this.errorCode = errorCode;
		gambleResultItem = null;
		
		if (errorCode == NetErrorCode.OK)
		{
			gambleResultItem = resultItem;
			gambleIndex = selectedIndex;
		}
		else
			gambleIndex = -1;
		
		if (this.parent != null)
		{
			this.parent.requestCount = 0;
			this.parent.UpdateCoinInfo(true);
			//this.parent.UpdateCouponCount();
			
			SetOneMoreInfo(this.gambleType);
		}
	}
	
	public void OneMoreGamble()
	{
		if (parent != null)
			parent.OnOneMoreGamble(gambleType);
	}
}
