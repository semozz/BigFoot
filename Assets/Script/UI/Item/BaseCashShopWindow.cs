using UnityEngine;
using System.Collections;

public class BaseCashShopWindow : PopupBaseWindow {
	public PopupBaseWindow popupBaseWindow = null;
	
	public string buyConfirmPrefabPath = "UI/Item/CashItemConfirmWindow";
	public string buyCompletePrefabPath = "UI/Item/BuyCompleteWindow";
	
	public delegate void OnCashShopClose();
	public OnCashShopClose onCashShopClose = null;
	
	public int requestCount = 0;
	public virtual void Start()
	{
		
	}
	
	public GameObject CreatePrefab(string path, Vector3 localPos, Transform root)
	{
		GameObject newObj = null;
		GameObject prefab = ResourceManager.LoadPrefab(path);
		if (prefab != null)
			newObj = (GameObject)Instantiate(prefab);
		
		//Vector3 origPos = Vector3.zero;
		Vector3 origScale = Vector3.one;
		Quaternion origQuat = Quaternion.identity;
		if (newObj != null)
		{
			//origPos = newObj.transform.localPosition;
			origScale = newObj.transform.localScale;
			origQuat = newObj.transform.localRotation;
		}
		
		if (newObj != null)
		{
			if (root != null)
				newObj.transform.parent = root;
			
			newObj.transform.localPosition = localPos;
			newObj.transform.localScale = origScale;
			newObj.transform.localRotation = origQuat;
		}
		
		return newObj;
	}
	
	public CashItemConfirmWindow confirmWindow = null;
	public void OnBuyCashItem(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		CashItemPanel cashItem = null;
		GameObject parent = null;
		if (obj != null)
			parent = obj.transform.parent.gameObject;
		
		if (parent != null)
			cashItem = parent.GetComponent<CashItemPanel>();
		
		if (cashItem != null)
		{
			CashItemType checkType = CashItemType.None;
			if (cashItem.itemInfo.paymentType != ePayment.Cash)
				checkType = CheckNeedGold(cashItem.itemInfo.price);
			
			if (checkType != CashItemType.None)
			{
				ClosePopup();
				OnNeedMoneyPopup(checkType, this);
				return;
			}
			
			if (confirmWindow == null)
			{
				GameObject newObj = CreatePrefab(buyConfirmPrefabPath, Vector3.zero, this.popupNode);
				confirmWindow = newObj != null ? newObj.GetComponent<CashItemConfirmWindow>() : null;
			}
			
			if (confirmWindow != null)
			{
				confirmWindow.SetCashItemInfo(cashItem.itemInfo);
				
				confirmWindow.cancelButtonMessage.target = this.gameObject;
				confirmWindow.cancelButtonMessage.functionName = "OnCancel";
				
				confirmWindow.okButtonMessage.target = this.gameObject;
				confirmWindow.okButtonMessage.functionName = "OnBuyConfirm";
			}
		}
	}
	
	public void OnCancel(GameObject obj)
	{
		CloseConfirmWindow();
	}
	
	public void CloseConfirmWindow()
	{
		if (confirmWindow != null)
			DestroyObject(confirmWindow.gameObject, 0.0f);
		
		confirmWindow = null;
	}
	
	public void OnBuyConfirm(GameObject obj)
	{
		if (confirmWindow != null)
		{
			confirmWindow.Wait();
			
			CashItemInfo cashItemInfo = confirmWindow.itemInfo;
			CloseConfirmWindow();
			
			Game.Instance.packetSender.SendRequestBuyCashItem(cashItemInfo);
		}
	}
	
	public BuyCompleteWindow completeWindow = null;
	public virtual void OnResult(NetErrorCode errorCode, int cashItemID)
	{
		requestCount = 0;
		
		CloseConfirmWindow();
		
		CashItemInfo buyInfo = null;
		TableManager tableManger = TableManager.Instance;
		CashShopInfoTable cashShopInfoTable = tableManger != null ? tableManger.cashShopInfoTable : null;
		
		if (cashShopInfoTable != null)
			buyInfo = cashShopInfoTable.GetItemInfo(cashItemID);
		
		if (buyInfo != null &&
			errorCode == NetErrorCode.OK)
		{
			if (completeWindow == null)
			{
				GameObject newObj = CreatePrefab(buyCompletePrefabPath, Vector3.zero, this.popupNode);
				completeWindow = newObj != null ? newObj.GetComponent<BuyCompleteWindow>() : null;
			}
			
			if (completeWindow != null)
			{
				completeWindow.SetCashItemInfo(buyInfo);
				
				completeWindow.okButtonMessage.target = this.gameObject;
				completeWindow.okButtonMessage.functionName = "OnBuyComplete";
			}
			
			return;
		}
		else if (errorCode == NetErrorCode.NotForSale)
		{
			// xgreen. 팔지않는상품.
		}
		else
		{
			OnErrorMessage(errorCode, null);
		}
	}
	
	public void OnBuyComplete(GameObject obj)
	{
		CloseCompleteWindow();
	}
	
	
	public void CloseCompleteWindow()
	{
		this.UpdateCoinInfo(true);
		
		if (popupBaseWindow != null && popupBaseWindow != this)
			popupBaseWindow.UpdateCoinInfo(true);
		
		if (completeWindow != null)
			DestroyObject(completeWindow.gameObject, 0.2f);
		
		completeWindow = null;
	}
	
	public override void OnBack()
	{
		base.OnBack();
		
		this.popupBaseWindow = null;
		
		CloseCompleteWindow();
		CloseConfirmWindow();
		
		if (onCashShopClose != null)
			onCashShopClose();
	}
	
	public virtual void InitWindow(CashItemType type, PopupBaseWindow popupBase)
	{
		
	}
}
