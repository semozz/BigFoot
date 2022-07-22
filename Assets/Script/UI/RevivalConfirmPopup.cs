using UnityEngine;
using System.Collections;

public class RevivalConfirmPopup : PopupBaseWindow {
	public UILabel revivalMoneyLabel = null;
	public int revivalJewel = 0;
	
	public UIButtonMessage cancelButtonMessage = null;
	public UIButtonMessage okButtonMessage = null;
	
	public void SetRevivalMoeny(int ownMoney, int needMoney)
	{
		this.revivalJewel = needMoney;
		
		if (revivalMoneyLabel != null)
			revivalMoneyLabel.text = string.Format("{0}", needMoney);
	}
	
	public void UpdateRevivalMoney()
	{
		int ownJewel = 0;
		if (Game.Instance != null && Game.Instance.charInfoData != null)
			ownJewel = Game.Instance.charInfoData.jewel_Value;
		
		SetRevivalMoeny(ownJewel, this.revivalJewel);
	}
	
	string cashShopPrefabPath = "UI/Item/CashShopWindow";
	CashShopWindow cashShopWindow = null;
	public void OpenCashShop(CashItemType checkType)
	{
		if (cashShopWindow == null)
			cashShopWindow = ResourceManager.CreatePrefab<CashShopWindow>(cashShopPrefabPath, this.popupNode);
		
		if (cashShopWindow != null)
		{
			cashShopWindow.onCashShopClose = new BaseCashShopWindow.OnCashShopClose(UpdateRevivalMoney);
			
			cashShopWindow.gameObject.SetActive(true);
			cashShopWindow.InitWindow(checkType, null);
		}
	}
}
