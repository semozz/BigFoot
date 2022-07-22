using UnityEngine;
using System.Collections;

public class ItemCountConfirmPopup : BaseConfirmPopup {
	public UILabel buyCountLabel = null;
	
	private ePayment payment = ePayment.Jewel;
	private int buyPrice = 0;
	private int totalPrice = 0;
	
	//private Vector3 ownMoney = Vector3.zero;
	private int ownGold = 0;
	private int ownJewel = 0;
	private int ownMedal = 0;
	
	private int maxCount = 0;
	private int buyCount = 1;
	
	public int BuyCount
	{
		get { return buyCount; }
	}
	
	public void SetBuyInfo(Item item, int ownGold, int ownJewel, int ownMedal)
	{
		this.ownGold = ownGold;
		this.ownJewel = ownJewel;
		this.ownMedal = ownMedal;
		
		buyPrice = 0;
		payment = ePayment.Jewel;
		
		Vector3 buyValue = Vector3.zero;
		if (item != null && item.itemInfo != null)
			buyValue = item.itemInfo.buyPrice;
		
		if (buyValue.x > 0.0f)
		{
			buyPrice = (int)buyValue.x;
			payment = ePayment.Gold;
			maxCount = (ownGold / (int)buyValue.x);
		}
		else if (buyValue.y > 0.0f)
		{
			buyPrice = (int)buyValue.y;
			payment = ePayment.Jewel;
			maxCount = (ownJewel / (int)buyValue.y);
		}
		else if (buyValue.z > 0.0f)
		{
			buyPrice = (int)buyValue.z;
			payment = ePayment.Medal;
			maxCount = (ownMedal / (int)buyValue.z);
		}
		
		UpdatePaymentSprite();
		UpdateCountInfo();
	}
	
	public void UpdatePaymentSprite()
	{
		string spriteName = "";
		switch(payment)
		{
		case ePayment.Cash:
			spriteName = cashMoneySpriteName;
			break;
		case ePayment.Gold:
			spriteName = goldMoneySpriteName;
			break;
		case ePayment.Jewel:
			spriteName = jewelMoneySpriteName;
			break;
		case ePayment.Medal:
			spriteName = medalMoneySpriteName;
			break;
		}
		
		if (moneyType != null)
			moneyType.spriteName = spriteName;
	}
	
	public void UpdateCountInfo()
	{
		this.okButtonCollider.enabled = buyCount > 0;
		
		totalPrice = buyPrice * buyCount;
		
		if (priceValueLabel != null)
			priceValueLabel.text = string.Format("{0:#,###,###,##0}", totalPrice);
		
		if (buyCountLabel != null)
			buyCountLabel.text = string.Format("{0:#,###,###,##0}", buyCount);
	}
	
	public void IncBuyPoint()
	{
		buyCount = Mathf.Min(maxCount, buyCount + 1);
		
		UpdateCountInfo();
	}
	
	public void DecBuyPoint()
	{
		buyCount = Mathf.Max(1, buyCount - 1);
		
		UpdateCountInfo();
	}
	
	public void MaxBuyPoint()
	{
		buyCount = maxCount;
		
		UpdateCountInfo();
	}
	
	public void ResetBuyPoint()
	{
		buyCount = 1;
		
		UpdateCountInfo();
	}
}
