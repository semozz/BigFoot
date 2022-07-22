using UnityEngine;
using System.Collections;

public class ReinforceItem : MonoBehaviour {
	public ReinforceInfo.eReinforceType reinforceType = ReinforceInfo.eReinforceType.Offense;
	
	public UISprite iconSprite = null;
	
	public UISprite moneySprite = null;
	public UILabel needMoneyLabel = null;
	
	public UILabel itemInfoLabel = null;
	
	public UIButtonMessage buttonMessage = null;
	public UICheckbox checkBox = null;
	
	public ReinforceInfo info = null;
	
	public void SetType(ReinforceInfo.eReinforceType type)
	{
		reinforceType = type;
		
		string spriteName = "Buff_Button_Offense";
		
		switch(reinforceType)
		{
		case ReinforceInfo.eReinforceType.None:
			spriteName = "Black_Frame";
			break;
		case ReinforceInfo.eReinforceType.Offense:
			spriteName = "Buff_Button_Offense";
			break;
		case ReinforceInfo.eReinforceType.Defense:
			spriteName = "Buff_Button_Defense";
			break;
		case ReinforceInfo.eReinforceType.AddLife:
			spriteName = "Buff_Button_Health";
			break;
		case ReinforceInfo.eReinforceType.AddExp:
			spriteName = "Buff_Button_EXP";
			break;
		}
		
		if (iconSprite != null)
			iconSprite.spriteName = spriteName;
	}
	
	public void SetGold(Vector3 needGold)
	{
		string goldSprite = "Shop_Money01";
		float needValue = 0.0f;
		
		if (needGold.x > 0.0f)
		{
			goldSprite = "Shop_Money01";
			needValue = needGold.x;
		}
		else if (needGold.y > 0.0f)
		{
			goldSprite = "Shop_Money02";
			needValue = needGold.y;
		}
		else if (needGold.z > 0.0f)
		{
			goldSprite = "Shop_Money03";
			needValue = needGold.z;
		}
		
		if (moneySprite != null)
			moneySprite.spriteName = goldSprite;
		
		if (needMoneyLabel != null)
			needMoneyLabel.text = string.Format("{0:#,###,###}", needValue);
	}
	
	public void SetInfo(ReinforceInfo.eReinforceType type, string itemInfo, Vector3 needGold)
	{
		SetType(type);
		
		if (itemInfoLabel != null)
			itemInfoLabel.text = itemInfo;
		
		SetGold(needGold);
	}
	
	public void SetInfo(ReinforceInfo info)
	{
		this.info = info;
		
		string infoStr = "";
		ReinforceInfo.eReinforceType type = ReinforceInfo.eReinforceType.None;
		Vector3 needGold = Vector3.zero;
		if (this.info != null)
		{
			type = this.info.type;
			needGold = this.info.needGold;
			
			infoStr = this.info.infoMsg;
		}
		
		SetInfo(type, infoStr, needGold);
	}
}
