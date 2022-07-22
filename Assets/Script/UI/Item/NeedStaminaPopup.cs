using UnityEngine;
using System.Collections;

public class NeedStaminaPopup : BasePopup {
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (cancelButtonLabel != null)
				cancelButtonLabel.text = stringTable.GetData(cancelButtonStringID);
			
			if (okButtonLabel != null)
				okButtonLabel.text = stringTable.GetData(okButtonStringID);
			
			if (titleLabel != null)
				titleLabel.text = stringTable.GetData(titleStringID);
			
			if (messageLabel != null)
				messageLabel.text = stringTable.GetData(messageStringID);
		}
	}
	
	public int prefixStringID = -1;
	public UISprite moneyType = null;
	public UILabel moneyInfo = null;
	
	public string goldSpriteName = "Price_money_gold";
	public string jewelSpriteName = "Price_money_jewel";
	public void SetNeedMoney(Vector3 money)
	{
		float needValue = 0.0f;
		string spriteName = jewelSpriteName;
		if (money.x > 0.0f)
		{
			needValue = money.x;
			spriteName = goldSpriteName;
		}
		else if (money.y > 0.0f)
		{
			needValue = money.y;
			spriteName = jewelSpriteName;
		}
		
		if (moneyType != null)
			moneyType.spriteName = spriteName;
		if (moneyInfo != null)
			moneyInfo.text = string.Format("{0:#,###,###}", needValue);
	}
	
	
}
