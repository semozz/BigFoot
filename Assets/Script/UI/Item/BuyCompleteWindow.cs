using UnityEngine;
using System.Collections;

public class BuyCompleteWindow : MonoBehaviour {
	public UILabel titleLabel = null;
	
	public UISprite cashItemSprite = null;
	public UILabel amountLabel = null;
	
	public int okButtonStringID = -1;
	public UILabel okButtonLabel = null;
	public UIButtonMessage okButtonMessage = null;
	
	public string goldSpriteName = "";
	public string jewelSpriteName = "";
	public string meatSpriteName ="Price_Meat";
	public string potionSpriteName = "Potion";
	public string packageSpriteName = "BoxPackage";
	
	
	public int goldStringID = -1;
	public int jewelStringID = -1;
	public int meatStringID = 206;
	public int potionStringID = 240;
	public int messageString_1_ID = -1;
	public int messageString_2_ID = -1;
	
	public string goldString = "";
	public string jewelString = "";
	public string meatString = "";
	public string potionString = "";
	public string messageString_1 = "";
	public string messageString_2 = "";
	
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			goldString = stringTable.GetData(goldStringID);
			jewelString = stringTable.GetData(jewelStringID);
			meatString = stringTable.GetData(meatStringID);
			potionString = stringTable.GetData(potionStringID);
			
			messageString_1 = stringTable.GetData(messageString_1_ID);
			messageString_2 = stringTable.GetData(messageString_2_ID);
			
			if (okButtonLabel != null)
				okButtonLabel.text = stringTable.GetData(okButtonStringID);
		}
	}
	
	public void OnOK()
	{
		DestroyObject(this.gameObject, 0.0f);
	}
	
	public void SetCashItemInfo(CashItemInfo info)
	{
	
		string titleStr = "";
		string amountStr = "";
		string spriteName = goldSpriteName;
		if (info != null)
		{
			switch(info.cashItemType)
			{
			case eCashItemType.Jewel:
				spriteName = jewelSpriteName;
				titleStr = string.Format("{0}{1}", jewelString, messageString_1);
				amountStr = string.Format("{0:#,###,###}", info.amount);
				break;
			case eCashItemType.Gold:
				spriteName = goldSpriteName;
				titleStr = string.Format("{0}{1}", goldString, messageString_2);
				amountStr = string.Format("{0:#,###,###}", info.amount);
				break;
			case eCashItemType.Potion1:
				spriteName = potionSpriteName;
				titleStr = string.Format("{0}{1}", potionString, messageString_2);
				amountStr = string.Format("{0:#,###,###}", info.amount);
				break;
			case eCashItemType.Potion2:
				spriteName = meatSpriteName;
				titleStr = string.Format("{0}{1}", meatString, messageString_2);
				amountStr = string.Format("{0:#,###,###}", info.amount);
				break;
			default:
				spriteName = packageSpriteName;
				titleStr = string.Format("{0}{1}", info.itemName, messageString_2);
				amountStr = string.Format("{0}", info.itemName);
				break;
			}
		}
		
		if (cashItemSprite != null)
			cashItemSprite.spriteName = spriteName;
		if (titleLabel != null)
			titleLabel.text = titleStr;
		if (amountLabel != null)
			amountLabel.text = amountStr;
	}
}
