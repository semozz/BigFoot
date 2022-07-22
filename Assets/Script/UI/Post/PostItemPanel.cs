using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PostTypeSpriteInfo
{
	public PostItemPanel.ePostItemType type = PostItemPanel.ePostItemType.ItemBox;
	public string spriteName = "";
}

public class PostItemPanel : MonoBehaviour {
	public PostWindow parentObj = null;
	
	public enum ePostItemType
	{
		ItemBox,
		Gold,
		Jewel,
		Stamina,
		Awaken,
		Message,
	}
	public ePostItemType postType = ePostItemType.ItemBox;
	
	public UISprite postTypeSprite = null;
	
	public UILabel msgLabel = null;
	
	public UISprite timeSprite = null;
	public UILabel leftTimeLabel = null;
	
	public Collider buttonCollider = null;
	public UILabel buttonTitleLabel = null;
	public UIButtonMessage buttonMessage = null;
	
	public List<PostTypeSpriteInfo> typeSpriteInfos = new List<PostTypeSpriteInfo>();
	
	public MailInfo itemData = null;
	
	public string defaultSpriteName = "";
	
	public int leftTimeFormatStringID = -1;
	public string leftTimeFormatString = "";
	
	public int msgPrefixStringID = -1;
	public int msgPostfixStringID = -1;			// 없으면.
	public int msgPostfixStringIDFinal = -1;		// 종성있으면.
	public int msgItemPostfixStringID = -1;			// 없으면.
	public int msgItemPostfixStringIDFinal = -1;	// 종성있으면.
	public string msgPrefixString = "";
	public string msgPostfixString = "";
	public string msgPostfixStringFinal = "";
	public string msgItemPostfixString = "";
	public string msgItemPostfixStringFinal = "";
	
	public int jewelStringFormatID = -1;
	public int goldStringFormatID = -1;
	public int staminaStringFormatID = -1;
	public int gambleCouponFormatID = -1;
	public int arenaTicketStringID = 225;
	public int potion1StringID = 240;
	public int potion2StringID = 206;
	public int awakenStringID = 159;
	
	public string jewelStr = "";
	public string goldStr = "";
	public string staminaStr = "";
	public string gambleCouponStr = "";
	public string arenaTicketStr = "";
	public string potion1Str = "";
	public string potion2Str = "";
	public string awakenStr = "";
	
	public int takeStringID = -1;
	public int viewStringID = -1;
	
	public Color messageColor = Color.magenta;
	public Color jewelColor = Color.green;
	public Color goldColor = Color.yellow;
	public Color staminaColor = Color.blue;
	public Color itemColor = Color.red;
	public Color couponColor = Color.green;
	
	public StringTable stringTable = null;
	void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			leftTimeFormatString = stringTable.GetData(leftTimeFormatStringID);
			
			msgPrefixString = stringTable.GetData(msgPrefixStringID);
			msgPostfixString = stringTable.GetData(msgPostfixStringID);
			msgPostfixStringFinal = stringTable.GetData(msgPostfixStringIDFinal);
			msgItemPostfixString = stringTable.GetData(msgItemPostfixStringID);
			msgItemPostfixStringFinal = stringTable.GetData(msgItemPostfixStringIDFinal);
			
			jewelStr = stringTable.GetData(jewelStringFormatID);
			goldStr = stringTable.GetData(goldStringFormatID);
			staminaStr = stringTable.GetData(staminaStringFormatID);
			gambleCouponStr = stringTable.GetData(gambleCouponFormatID);
			
			arenaTicketStr = stringTable.GetData(arenaTicketStringID);
			
			potion1Str = stringTable.GetData(potion1StringID);
			potion2Str = stringTable.GetData(potion2StringID);
			
			awakenStr = stringTable.GetData(awakenStringID);
		}
		
		if (readObj != null)
			readObj.SetActive(false);
	}
	
	public PostItemPanel.ePostItemType GetType(MailInfo mail)
	{
		ePostItemType type = ePostItemType.Message;
        if (mail != null)
		{
            foreach (var data in mail.rewards)
            {
                if (data.ItemID != 0 ||
                    mail.Type == MailType.PackageItem)
                    type = ePostItemType.ItemBox;
                else if (data.Gold != 0)
                    type = ePostItemType.Gold;
                else if (data.Jewel != 0)
                    type = ePostItemType.Jewel;
                else if (data.Stamina != 0)
                    type = ePostItemType.Stamina;
				else if (data.awaken_point != 0)
					type = ePostItemType.Awaken;
                else
                    type = ePostItemType.Message;
            }
		}
		
		return type;
	}
	
	public System.DateTime expireTime = System.DateTime.Now;
	public bool isExpiredItem = false;
	public void SetPostItem(MailInfo data)
	{
		itemData = data;
		
		if (itemData != null)
		{
			System.DateTime nowTime = System.DateTime.Now;
			System.TimeSpan leftTime = Game.ToTimeSpan(itemData.LeftDestroySec);
			
			expireTime = nowTime + leftTime;
		}
		
		if (itemData.bOpened == 1)
			OnRead();
		else
			UpdateItem();
	}
	
	public void Update()
	{
		if (itemData == null)
			return;
		
		System.TimeSpan leftTime = expireTime - System.DateTime.Now;
		if (isExpiredItem == false && leftTime.TotalSeconds <= 0)
		{
			isExpiredItem = true;
			UpdateItem();
		}
		else
		{
			string leftTimeStr = string.Format(leftTimeFormatString, leftTime.Days, leftTime.Hours, leftTime.Minutes);
			
			if (leftTimeLabel != null)
				leftTimeLabel.text = leftTimeStr;
		}
	}
	
	public int achievePrefixStringID = -1;
	public int dailyAchievePrefixStringID = -1;
	public int sendStaminaPrefixStringID = -1;
	public int recieveGiftBySendStaminaPrefixStringID = -1;
	public int operatorEventGiftPrefixStringID = -1;
	public int operatorEventMsgPrefixStringID = -1;
	public int bossRaid01PrefixStringID = -1;
	public int bossRaid02PrefixStringID = -1;
	public int bossRaid03PrefixStringID = -1;
	public int couponEventPrefixStringID = -1;
	public int eventRewardPrefixStringID = 216;
	public int packageItemPrefixStringID = 224;
	public int bossFinderPrefixStringID = 252;
	public int inventoryFullPrefixStringID = 1006;
	
	public string GetPrefixString(MailInfo mailInfo)
	{
		string prefixStr = "";
		
		string formatStr = "";
		switch(mailInfo.Type)
		{
		case MailType.AchievementReward:
			formatStr = stringTable.GetData(achievePrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.MissionReward:
			formatStr = stringTable.GetData(dailyAchievePrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.Stamina:
			formatStr = stringTable.GetData(sendStaminaPrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.StaminaReward:
			formatStr = stringTable.GetData(recieveGiftBySendStaminaPrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.Msg:
			prefixStr = "";//stringTable.GetData(operatorEventMsgPrefixStringID);
			break;
		case MailType.Item:
		case MailType.System:
			formatStr = stringTable.GetData(operatorEventGiftPrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Title);
			break;
		case MailType.BossRaid01:
			formatStr = stringTable.GetData(bossRaid01PrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.BossRaid02:
			formatStr = stringTable.GetData(bossRaid02PrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.BossRaid03:
			formatStr = stringTable.GetData(bossRaid03PrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.BossFinder:
			formatStr = stringTable.GetData(bossFinderPrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.CouponReward:
			prefixStr = stringTable.GetData(couponEventPrefixStringID);
			break;
		case MailType.EventReward:
			formatStr = stringTable.GetData(eventRewardPrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.PackageItem:
			formatStr = stringTable.GetData(packageItemPrefixStringID);
			
			string packageName = string.Format("{0}{1}[-]", GameDef.RGBToHex(itemColor), mailInfo.Sender);
			
			prefixStr = string.Format(formatStr, packageName);
			break;
		case MailType.JewelPack:
			formatStr = stringTable.GetData(eventRewardPrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		case MailType.InventoryFull:
			formatStr = stringTable.GetData(inventoryFullPrefixStringID);
			prefixStr = string.Format(formatStr, mailInfo.Sender);
			break;
		}
		return prefixStr;
	}
	
	public void UpdateItem()
	{
		string typeSpriteName = defaultSpriteName;
		string leftTimeStr = "--:--:--";
		
		string msgStr = "";
		string buttonStr = "";
		
		if (itemData != null)
		{
			msgPrefixString = string.Format("{0}{1} [-]", GameDef.RGBToHex(messageColor), GetPrefixString(itemData));
			
			msgStr = msgPrefixString + " ";
			
			ePostItemType postType = GetType(itemData);
			typeSpriteName = GetPostItemSpriteName(postType);
			
			System.TimeSpan leftTime = expireTime - System.DateTime.Now;
			leftTimeStr = string.Format(leftTimeFormatString, leftTime.Days, leftTime.Hours, leftTime.Minutes);
			ItemInfo itemInfo = null;
			
			string itemInfoStr = "";

            foreach (var reward in itemData.rewards)
            {
                if (reward.Jewel > 0)
                {
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(jewelColor), jewelStr, reward.Jewel);
                }
                if (reward.Gold > 0)
                {
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(goldColor), goldStr, reward.Gold);
                }
                if (reward.Stamina > 0)
                {
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(staminaColor), staminaStr, reward.Stamina);
                }
                if (reward.coupon > 0)
                {
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(couponColor), gambleCouponStr, reward.coupon);
                }
                if (reward.ticket > 0)
                {
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(couponColor), arenaTicketStr, reward.ticket);
                }
                if (reward.potion1 > 0)
                {
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(itemColor), potion1Str, reward.potion1);
                }
                if (reward.potion2 > 0)
                {
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(itemColor), potion2Str, reward.potion2);
                }
				if (reward.awaken_point > 0)
				{
                    if (itemInfoStr.Length > 0)
                        itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                    itemInfoStr += string.Format("{0}{1} {2:#,###,###}[-]", GameDef.RGBToHex(couponColor), awakenStr, reward.awaken_point);
                }	

                if (itemData.Type != MailType.PackageItem &&
                    reward.ItemID > 0)
                {
                    TableManager tableManager = TableManager.Instance;
                    ItemTable itemTable = tableManager != null ? tableManager.itemTable : null;

                    itemInfo = itemTable.GetData(reward.ItemID);

                    if (itemInfo != null)
                    {
                        if (itemInfoStr.Length > 0)
                            itemInfoStr += string.Format("{0}, [-]", GameDef.RGBToHex(messageColor));

                        itemInfoStr += string.Format("{0}{1}[-]", GameDef.RGBToHex(itemColor), itemInfo.itemName);
                    }
                }

            }

			
			
			string finalPostFixString = "";//string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgPostfixString);
			switch(itemData.Type)
			{
			case MailType.Stamina:
				//itemInfoStr += " " + msgPostfixString; // 을 보냈습니다.
				finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgPostfixString);
				break;
			case MailType.Item:
				if (itemInfo != null && !string.IsNullOrEmpty(itemInfo.itemName))
				{
					char [] test = itemInfo.itemName.ToCharArray();
					if (StringUtil.HangulJaso.IsFinalConsonant(test[itemInfo.itemName.Length-1]))
					{
						//itemInfoStr += " " + msgPostfixString;
						finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgPostfixString);
					}
					else
					{
						//itemInfoStr += " " + msgPostfixStringFinal;
						finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgPostfixStringFinal);
					}	
				}
				break;
			case MailType.StaminaReward:
			case MailType.AchievementReward:
			case MailType.MissionReward:
			case MailType.BossRaid01:
			case MailType.BossRaid02:
			case MailType.BossRaid03:
			case MailType.BossFinder:
			case MailType.InventoryFull:
				if (itemInfo != null && !string.IsNullOrEmpty(itemInfo.itemName))
				{
					char [] test = itemInfo.itemName.ToCharArray();
					if (StringUtil.HangulJaso.IsFinalConsonant(test[itemInfo.itemName.Length-1]))
					{
						//itemInfoStr += " " + msgItemPostfixString;
						finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgItemPostfixStringFinal);
					}
					else
					{
						//itemInfoStr += " " + msgItemPostfixStringFinal;
						finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgItemPostfixString);
					}
				}
				else
				{
					//itemInfoStr += "[ ]" + msgItemPostfixStringFinal;
					finalPostFixString = string.Format("{0}{1}[-]", GameDef.RGBToHex(messageColor), msgItemPostfixStringFinal);
				}

				break;
			case MailType.Msg:
				finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), itemData.Title);
				break;
			case MailType.PackageItem:
				finalPostFixString = "";
				break;
			case MailType.JewelPack:
				finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgItemPostfixStringFinal);
				break;
			default:
				//itemInfoStr += " " + msgPostfixString;
				finalPostFixString = string.Format(" {0}{1}[-]", GameDef.RGBToHex(messageColor), msgPostfixString);
				break;
			}
			
			if (string.IsNullOrEmpty(finalPostFixString) == false)
				itemInfoStr += finalPostFixString;
		
			msgStr += itemInfoStr;
			
			
			if (itemData.Type == MailType.Msg)
				buttonStr = stringTable.GetData(this.viewStringID);
			else
				buttonStr = stringTable.GetData(this.takeStringID);
		}
		
		Color effectColor = Color.white;
		if (isExpiredItem == true || itemData == null || itemData.bOpened == 1)
			effectColor = Color.gray;
		
		if (postTypeSprite != null)
		{
			postTypeSprite.spriteName = typeSpriteName;
			postTypeSprite.color = effectColor;
		}
		
		if (leftTimeLabel != null)
			leftTimeLabel.text = leftTimeStr;
		
		if (msgLabel != null)
		{
			msgLabel.text = msgStr;
			msgLabel.color = effectColor;
		}
		
		if (buttonTitleLabel != null)
		{
			buttonTitleLabel.text = buttonStr;
			buttonTitleLabel.color = effectColor;
		}
	}
	
	public string GetPostItemSpriteName(ePostItemType type)
	{
		string spriteName = defaultSpriteName;
		foreach(PostTypeSpriteInfo info in typeSpriteInfos)
		{
			if (info != null && info.type == type)
			{
				spriteName = info.spriteName;
				break;
			}
		}
		
		return spriteName;
	}
	
	public void OnPostItemClick(GameObject obj)
	{
		if (parentObj.requestCount > 0 )
			return;
		
		if (itemData == null)
			return;
		
		if (itemData.Type == MailType.Msg)
			parentObj.SendMessage("OnViewMessage", itemData, SendMessageOptions.DontRequireReceiver);
		else
			parentObj.SendMessage("OnTakeItem", itemData, SendMessageOptions.DontRequireReceiver);
			
	}
	
	public GameObject readObj = null;
	public void OnRead()
	{
		if (itemData != null)
			itemData.bOpened = 1;
		
		if (readObj != null)
			readObj.SetActive(true);
		
		if (buttonCollider != null)
			buttonCollider.enabled = false;
		
		UpdateItem();
	}
	
	public bool IsRead()
	{
		bool isRead = false;
		
		if (itemData != null)
			isRead = itemData.bOpened == 1;
		
		return isRead;
	}
}
