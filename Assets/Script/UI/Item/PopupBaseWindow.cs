using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupBaseWindow : MonoBehaviour {
	public TownUI.eTOWN_UI_TYPE windowType = TownUI.eTOWN_UI_TYPE.NONE;
	
	public UILabelEx gold = null;
	public UILabelEx jewel = null;
	public UILabelEx medal = null;

	public string needJewelPopupPrefabPath = "UI/Item/NeedMoneyWindow";
	public NeedMoneyWindow needMoneyWindow = null;
	public Transform popupNode = null;
	
	protected List<BasePopup> popupList = new List<BasePopup>();
	public T GetPopup<T>()
	{
		object comp = null;
		
		foreach(BasePopup popup in popupList)
		{
			if (popup.GetType() == typeof(T))
			{
				comp = popup;
				break;
			}
		}
		
		return (T)comp;
	}
	
	public void ClosePopup<T>()
	{
		BasePopup comp = null;
		
		foreach(BasePopup popup in popupList)
		{
			if (popup.GetType() == typeof(T))
			{
				comp = popup;
				break;
			}
		}
		
		if (comp != null)
		{
			popupList.Remove(comp);
			DestroyObject(comp.gameObject, 0.1f);
		}
	}
	
	public virtual void Awake()
	{
		
	}
	
	public void OnNeedMoneyCancel(GameObject obj)
	{
		ClosePopup();
	}
	
	public void OnNeedMoneyOK(GameObject obj)
	{
		CashItemType type = CashItemType.None;
		PopupBaseWindow popupBaseWindow = null;
		if (needMoneyWindow != null)
		{
			type = needMoneyWindow.needMoneyType;
			popupBaseWindow = needMoneyWindow.popupBaseWindow;
		}
		
		ClosePopup();
		
		CashShopWindow cashShop = GameUI.Instance.cashShopWindow;
		if (popupBaseWindow != cashShop)
		{
			TownUI townUI = GameUI.Instance.townUI;
			if (townUI != null)
			{
				townUI.OnCashShop(type, popupBaseWindow);
			}
		}
		else
		{
			if (cashShop != null)
			{
				cashShop.InitWindow(type, cashShop.popupBaseWindow);
			}
		}
	}
	
	public virtual void OnBack()
	{
		if (GameUI.Instance.currentWindow == this)
			GameUI.Instance.SetCurrentWindow(null);
		
		ClosePopup();
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
		{
			townUI.requestCount = 0;
			
			if (townUI.toWindowtype != TownUI.eTOWN_UI_TYPE.NONE &&	this.windowType == townUI.toWindowtype)
				townUI.toWindowtype = TownUI.eTOWN_UI_TYPE.NONE;

			switch(townUI.toWindowtype)
			{
			case TownUI.eTOWN_UI_TYPE.ACHIVEMENT:
				townUI.OnAchieveWindow();
				break;
			case TownUI.eTOWN_UI_TYPE.CASH_SHOP:
				townUI.OnCashShop();
				break;
			case TownUI.eTOWN_UI_TYPE.FIREND:
				townUI.OnFriendsWindow();
				break;
			case TownUI.eTOWN_UI_TYPE.GAMBLE:
				townUI.OnGambleWindow();
				break;
			case TownUI.eTOWN_UI_TYPE.MAPSELECT:
				townUI.OnMap();
				break;
			case TownUI.eTOWN_UI_TYPE.PICTUREBOOK:
				townUI.OnPictureBookWindow();
				break;
			case TownUI.eTOWN_UI_TYPE.POST:
				townUI.OnPostWindow();
				break;
			case TownUI.eTOWN_UI_TYPE.SHOP:
				townUI.OnShop();
				break;
			case TownUI.eTOWN_UI_TYPE.STORAGE:
				townUI.OnStorage();
				break;
			case TownUI.eTOWN_UI_TYPE.ARENA:
				townUI.OnRequestArenaInfo();
				break;
			case TownUI.eTOWN_UI_TYPE.WAVE:
				townUI.OnRequestWaveInfo();
				break;
			default:
				townUI.gameObject.SetActive(true);
				break;
			}
			
			townUI.toWindowtype = TownUI.eTOWN_UI_TYPE.NONE;
			
			if (GameUI.Instance.myCharInfos != null)
				GameUI.Instance.myCharInfos.UpdateValue();
		}
		
		this.gameObject.SetActive(false);
	}
	
	public virtual void ClosePopup()
	{
		if (needMoneyWindow != null)
		{
			DestroyObject(needMoneyWindow.gameObject, 0.0f);
			needMoneyWindow = null;
		}
		
		foreach(BasePopup popup in popupList)
		{
			DestroyObject(popup.gameObject, 0.0f);
		}
		popupList.Clear();
		
		CloseCoinUpdate();
	}
	
	public void OnNeedMoneyPopup(CashItemType type, PopupBaseWindow popupBase)
	{
		if (needMoneyWindow == null)
		{
			GameObject newObj = ResourceManager.CreatePrefab(needJewelPopupPrefabPath, popupNode, Vector3.zero);
			if (newObj != null)
				needMoneyWindow = newObj.GetComponent<NeedMoneyWindow>();
			
			if (needMoneyWindow != null)
			{
				needMoneyWindow.SetInfo(type, popupBase);
				
				if (needMoneyWindow.cancelButtonMessage != null)
				{
					needMoneyWindow.cancelButtonMessage.target = this.gameObject;
					needMoneyWindow.cancelButtonMessage.functionName = "OnNeedMoneyCancel";
				}
				
				if (needMoneyWindow.okButtonMessage != null)
				{
					needMoneyWindow.okButtonMessage.target = this.gameObject;
					needMoneyWindow.okButtonMessage.functionName = "OnNeedMoneyOK";
				}
			}
		}
	}
	
	public string staminaConfirmPopupPrefabPath = "UI/Item/NeedStaminaPopup";
	public void OnNeedStaminaPopup(Vector3 needMoney)
	{
		NeedStaminaPopup  popup = null;
		
		GameObject newObj = ResourceManager.CreatePrefab(staminaConfirmPopupPrefabPath, popupNode, Vector3.zero);
		if (newObj != null)
			popup = newObj.GetComponent<NeedStaminaPopup>();
		
		if (popup != null)
		{
			popup.cancelButtonMessage.target = this.gameObject;
			popup.cancelButtonMessage.functionName = "OnConfirmPopupCancel";
			
			popup.okButtonMessage.target = this.gameObject;
			popup.okButtonMessage.functionName = "OnNeedStaminaOK";
			
			popup.SetNeedMoney(needMoney);
			popupList.Add(popup);
		}
	}
	
	public static CashItemType CheckNeedGold(Vector3 needMoney)
	{
		return CheckNeedGold((int)needMoney.x, (int)needMoney.y, (int)needMoney.z);
	}
	
	public static CashItemType CheckNeedGold(int needGold, int needJewel, int needMedal)
	{
		CharInfoData charData = Game.Instance.charInfoData;
		int ownGold = 0;
		int ownJewel = 0;
		int ownMedal = 0;
		if (charData != null)
		{
			ownGold = charData.gold_Value;
			ownJewel = charData.jewel_Value;
			ownMedal = charData.medal_Value;
		}
		
		CashItemType checkCash = CashItemType.None;
		if (needGold > 0 && needGold > ownGold)
			checkCash = CashItemType.JewelToGold;
		else if (needJewel > 0.0f && needJewel > ownJewel)
			checkCash = CashItemType.CashToJewel;
		else if (needMedal > 0.0f && needMedal > ownMedal)
			checkCash = CashItemType.Medal;
		
		return checkCash;
	}
	
	public string emptySlotPopupPrefabPath = "UI/MapSelect/EmptySlotPopup";
	public int cleanInventoryStringID = 31;
	public void OnNeedInvenSlotPopup()
	{
		BaseConfirmPopup  popup = null;
		
		GameObject newObj = ResourceManager.CreatePrefab(emptySlotPopupPrefabPath, popupNode, Vector3.zero);
		if (newObj != null)
			popup = newObj.GetComponent<BaseConfirmPopup>();
		
		if (popup != null)
		{
			if (this.windowType == TownUI.eTOWN_UI_TYPE.STORAGE)
			{
				popup.SetMessage(cleanInventoryStringID);
				popup.okButtonMessage.target = this.gameObject;
				popup.okButtonMessage.functionName = "OnClosePopup";
			}
			else
			{
				popup.okButtonMessage.target = this.gameObject;
				popup.okButtonMessage.functionName = "OnNeedInvenSlotPopupOK";
			}

			popupList.Add(popup);
		}
	}
	
	public virtual void OnNeedInvenSlotPopupOK(GameObject obj)
	{
		OnBack();
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
		{
			townUI.toWindowtype = this.windowType;
			townUI.OnStorage();
		}
	}
	
	public string messageBoxPrefabPath = "UI/MessageBox";
	public virtual void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		ClosePopup();
		
		switch(errorCode)
		{
		case NetErrorCode.NotEnoughGold:
			OnNeedMoneyPopup(CashItemType.JewelToGold, popupBase);
			break;
		case NetErrorCode.NotEnoughCash:
			OnNeedMoneyPopup(CashItemType.CashToJewel, popupBase);
			break;
		case NetErrorCode.NotEnoughInven:
		case NetErrorCode.NotEnoughInvenMaterial:
			OnNeedInvenSlotPopup();
			break;
		default:
			NoticePopupWindow messageBox = GameUI.Instance.MessageBox;
			if (messageBox != null)
			{
				TableManager tableManager = TableManager.Instance;
				StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
				
				string msgStr = "";
				if (stringTable != null)
					msgStr = stringTable.GetData((int)errorCode);
				
				messageBox.SetMessage(msgStr);
				
				if (this.popupNode != null)
				{
					messageBox.transform.parent = this.popupNode;
					messageBox.transform.localPosition = Vector3.zero;
				}
				
				if (messageBox.buttonMessage != null)
				{
					messageBox.buttonMessage.target = messageBox.gameObject;
					messageBox.buttonMessage.functionName = "OnClose";
				}
			}
			break;
		}
	}
	
	public string normalErrorPopupPrefab = "UI/NormalErrorPopup";
	public virtual void OnErrorMessage(string title, string message)
	{
		ClosePopup();
		
		BaseConfirmPopup popup = ResourceManager.CreatePrefab<BaseConfirmPopup>(normalErrorPopupPrefab, this.popupNode);
		if (popup != null)
		{
			popup.SetMessage(title, message);
			
			popup.okButtonMessage.target = this.gameObject;
			popup.okButtonMessage.functionName = "OnConfirmPopupCancel";
			
			this.popupList.Add(popup);
		}
	}
	
	public virtual void UpdateCoinInfo(bool increaseEffect)
	{
		int ownGold = 0;
		int ownJewel = 0;
		int ownMedal = 0;
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
		{
			ownGold = charData.gold_Value;
			ownJewel = charData.jewel_Value;
			ownMedal = charData.medal_Value;
		}
		
		SetCoinInfo(ownGold, ownJewel, ownMedal, increaseEffect);
	}
	
	public virtual void UpdateCoinInfo()
	{
		// default
		UpdateCoinInfo(false);
	}
	
	public void CloseCoinUpdate()
	{
		if (gold != null)
			gold.ClearEffect();
		if (jewel != null)
			jewel.ClearEffect();
		if (medal != null)
			medal.ClearEffect();
	}
	
	public void SetCoinInfo(int fGold, int fJewel, int fMedal, bool increaseEffect)
	{
		if (gold != null)
			gold.SetValue(fGold, increaseEffect);
		if (jewel != null)
			jewel.SetValue(fJewel, increaseEffect);
		if(medal != null)
			medal.SetValue(fMedal, increaseEffect);
		
		/*
		string msg = "";
		msg = string.Format("{0:#,###,##0}", fGold);
		if (gold != null)
			gold.text = msg;
		
		msg = string.Format("{0:#,###,##0}", fJewel);
		if (jewel != null)
			jewel.text = msg;
		
		msg = string.Format("{0:#,###,##0}", fMedal);
		if (medal != null)
			medal.text = msg;
			*/
	}
	

	public static void SetLabelString(UILabel label, int stringID, StringTable stringTable)
	{
		if (stringTable != null && stringID != -1)
			SetLabelString(label, stringTable.GetData(stringID));
	}
	
	public static void SetLabelString(UILabel label, string stringMsg)
	{
		if (label != null)
			label.text = stringMsg;
	}
	
	public virtual void OnClosePopup(GameObject obj)
	{
		ClosePopup();
	}
}