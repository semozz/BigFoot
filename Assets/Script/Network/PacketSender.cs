using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PacketSender : IPacketSender
{
	// fortest.
	string [] TStoreCode = new string []{ "0910020887", "0910020888", "0910020890", "0910020891", "0910020892"};
	//

	private ClientConnector connector;
	
	public ClientConnector Connector
    {
        get { 
			return this.connector; 
		}
        set { this.connector = value; }
    }
	
	public PacketSender()
	{
	}
	
	~PacketSender()
	{
		Connector = null;
	}
	
	void SendPacket(string url, Header packet)
	{
		try
		{
			GameUI.Instance.DoWait();
			
			Connector.SendPacket(url, packet);
		}
		catch (LitJson.JsonException e)
		{
			Logger.DebugLog(e.Message);
		}
	}
	
	public void SendPreLogin()
	{
		Connector.SendPreLogin();
		//cryptography.SetRemotePhpScriptLocation
	}
	
	public void SendRegisterGCMID()
	{
		PacketRegisterGCMID  msg = new PacketRegisterGCMID();
		
		msg.UserIndexID = connector.UserIndexID;
		msg.regID = connector.gcmRegID;
		
		SendPacket("RegisterGCMID.php", msg);
	}
	
	public void SendLogin(string Account, string password, AccountType accountType)
	{
		string deviceModel = SystemInfo.deviceModel;
		string UniqueID = SystemInfo.deviceUniqueIdentifier;
		string OS = SystemInfo.operatingSystem;
		
		SendLogin(Account, password, deviceModel, UniqueID, OS, accountType);
	}

	void SendLogin(string Account, string password, string deviceModel, string UniqueID, string OS, AccountType accountType)
	{
		connector.tempAccountID = Account;
		connector.tempPass = password;
		connector.tempAccountType = accountType;

		PacketLogin msg = new PacketLogin();
		
		msg.AccountID = Account;
		msg.Password = password;
		msg.uniqueID = UniqueID;
		msg.OS = OS;
		msg.version = Version.NetVersion;
		msg.EmailType = accountType;
		msg.publisherID = connector.publisher;
		
		SendPacket ("Login.php", msg);
	}
	
	public void SendKakaoLogin(string kakaoUserID)
	{
		throw new NotImplementedException();
	}
	
	public void UpdateKakaoFriends(string[] friends_user_ids)
	{
		throw new NotImplementedException();
	}

    public void SendRequestBuyArtifactItem(int ItemID, int buyCount, int slotIndex, GameDef.eItemSlotWindow window)
    {
        throw new NotImplementedException();
    }
		
	public void SendRequestBuyNormalItem(int ItemID, int buyCount, int slotIndex, GameDef.eItemSlotWindow window)
	{
		PacketBuyNormalItem packet = new PacketBuyNormalItem();
		
		packet.UserIndexID = Connector.UserIndexID;
		
		packet.ItemID = ItemID;
		packet.buyCount = buyCount;
		packet.slotIndex = slotIndex;
		packet.windowType = window;
		
		SendPacket ("ShopBuy.php", packet);
	}
	
	public void SendRequestBuyCostumeItem(int ItemID, int slotIndex)
	{
		PacketBuyCostumeItem packet = new PacketBuyCostumeItem();
		//packet.tradeInfo.slotIndex = slotIndex;
		packet.UserIndexID = Connector.UserIndexID;
		packet.ItemID = ItemID;
		packet.slotIndex = slotIndex;		
		
		SendPacket ("ShopBuy.php", packet);
	}
	
	public void SendRequestBuyCostumeSetItem(int itemID, int slotIndex)
	{
		PacketBuyCostumeSetItem packet = new PacketBuyCostumeSetItem();
		
		packet.UserIndexID = Connector.UserIndexID;
		packet.ItemID = itemID;
		packet.slotIndex = slotIndex;		
		
		SendPacket ("ShopBuy.php", packet);
	}
	
	public void SendRequestSellCostumeItem(int charIndex, int slotIndex, int ItemID, string ItemUID)
	{
		PacketSellItem packet = new PacketSellItem();
		
		packet.MsgID = NetID.SellCostumeItem;		
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = ItemUID;
		packet.windowType = GameDef.eItemSlotWindow.Costume;
		packet.Shop = true;
		
		SendPacket ("ShopSell.php", packet);
	}
	
	public void SendRequestSellEquipItem(int charIndex, int slotIndex, int ItemID, string ItemUID)
	{
		PacketSellItem packet = new PacketSellItem();
		
		packet.MsgID = NetID.SellEquipItem;
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = ItemUID;
		packet.windowType = GameDef.eItemSlotWindow.Equip;
		packet.Shop = true;
		
		SendPacket ("ShopSell.php", packet);
	}
	
	//---------------------------------------------------------------------------------------------------------------
	public void SendRequestSellNormalItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
	{
		PacketSellItem packet = new PacketSellItem();
		
		packet.MsgID = NetID.SellInvenItem;
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = ItemUID;
		packet.windowType = GameDef.eItemSlotWindow.Inventory;
		packet.Shop = false;
		
		SendPacket ("ShopSell.php", packet);
	}
	
	public void SendRequestSellEquipItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
	{
		PacketSellItem packet = new PacketSellItem();
		
		packet.MsgID = NetID.SellEquipItem;
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = ItemUID;
		packet.windowType = GameDef.eItemSlotWindow.Equip;
		packet.Shop = false;
		
		SendPacket ("ShopSell.php", packet);
	}
	
	public void SendRequestSellCostumeItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
	{
		PacketSellItem packet = new PacketSellItem();
		
		packet.MsgID = NetID.SellCostumeItem;
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = ItemUID;
		packet.windowType = GameDef.eItemSlotWindow.Costume;
		packet.Shop = false;
		
		SendPacket ("ShopSell.php", packet);
	}
	
	public void SendRequestSellCostumeSetItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
	{
		PacketSellItem packet = new PacketSellItem();
		
		packet.MsgID = NetID.SellCostumeSetItem;
		
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = ItemUID;
		packet.windowType = GameDef.eItemSlotWindow.CostumeSet;
		packet.Shop = false;
		
		SendPacket ("ShopSell.php", packet);
	}
	
	public void SendRequestSellMaterialItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
	{
		PacketSellItem packet = new PacketSellItem();
		
		packet.MsgID = NetID.SellMaterialItem;
		
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = ItemUID;
		packet.windowType = GameDef.eItemSlotWindow.MaterialItem;
		packet.Shop = false;
		
		SendPacket ("ShopSell.php", packet);
	}

	public void SendRequestDoEquipItem(int equipSlotIndex, int InvenSlotIndex, int ItemID, string UID, GameDef.eItemSlotWindow slotWindow)
	{
		PacketDoEquipItem packet = new PacketDoEquipItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		packet.Info = new BaseTradeItemInfo();
		
		packet.eEquipSlot = equipSlotIndex;
		packet.Info.slotIndex = InvenSlotIndex;
		packet.Info.windowType = slotWindow;
		packet.Info.ItemID = ItemID;
		packet.Info.UID = UID;
		
		SendPacket ("DoEquipItem.php", packet);
	}
	
	public void SendRequestDoEquipCostumeItem(int equipSlotIndex, int slotIndex, int ItemID, string UID)
	{
		PacketWearCostumeItem packet = new PacketWearCostumeItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.ItemID = ItemID;
		packet.UID = UID;
		packet.equipSlotIndex = equipSlotIndex;
		
		SendPacket ("DoEquipItem.php", packet);
	}
	
	public void SendRequestDoEquipCostumeSetItem(int invenSlotIndex, int itemID, string UID, GameDef.eItemSlotWindow slotWindow)
	{
		PacketWearCostumeSetItem packet = new PacketWearCostumeSetItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.ItemID = itemID;
		packet.UID = UID;
		packet.slotIndex = invenSlotIndex;
		
		SendPacket("DoEquipItem.php", packet);
	}
	
	public void SendRequestDoUnEquipItem(int slotIndex, string UID, int ItemID)
	{
		PacketDoUnEquipItem packet = new PacketDoUnEquipItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.tradeInfo = new BaseTradeItemInfo();
		
		packet.tradeInfo.slotIndex = slotIndex;
		packet.tradeInfo.UID = UID;
		packet.tradeInfo.ItemID = ItemID;
		
		SendPacket ("DoUnEquipItem.php", packet);
	}
	
	public void SendRequestDoUnEquipCostume(int slotIndex, string UID, int ItemID)
	{
		PacketUnwearCostumeItem packet = new PacketUnwearCostumeItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.ItemID = ItemID;
		packet.UID = UID;
		packet.slotIndex = slotIndex;
		
		SendPacket ("DoUnEquipItem.php", packet);
	}
	
	public void SendRequestDoUnEquipCostumeSetItem(string UID, int itemID)
	{
		PacketUnwearCostumeSetItem packet = new PacketUnwearCostumeSetItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.UID = UID;
		
		SendPacket ("DoUnEquipItem.php", packet);
	}
	
	public void SendRequestReinforceItem(int slotIndex, GameDef.eItemSlotWindow slotWindow, string UID, int ItemID, string[] delItems)
	{
		PacketReinforceItemEx packet = new PacketReinforceItemEx();
		packet.UserIndexID = Connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.slotIndex = slotIndex;
		packet.windowType = slotWindow;
		packet.UID = UID;
		packet.ItemID = ItemID;
		
		packet.DelItemUIDs = delItems;
		
		//packet.reinforceMaterialID = reinforceMaterialID;
		SendPacket ("DoReinforceItem.php", packet);
	}
	
	public void SendRequestCompositionItem(int slotIndex, GameDef.eItemSlotWindow slotWindow, 
																	string UID, int ItemID,
																	string composMaterialUID, string composAddMaterialUID, bool bCash, bool isTutorial)
	{
		PacketCompositionItem packet = new PacketCompositionItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.Info.UID = UID;
		packet.Info.ItemID = ItemID;
		packet.Info.slotIndex = slotIndex;
		packet.Info.windowType = slotWindow;
		
		packet.DelItemUID = composMaterialUID;
		
		packet.materialUID = composAddMaterialUID;
		packet.ByJewel = bCash == true ? 1 : 0;
		
		packet.tutorial = isTutorial == true ? 1 : 0;
		
		SendPacket ("DoCompositionItem.php", packet);
	}
	
	public void SendRequestCompositionItemEx(int slotIndex, GameDef.eItemSlotWindow slotWindow, 
																	string UID, int ItemID,
																	string composMaterialUID, string composAddMaterialUID, bool bCash, bool isTutorial)
	{
		PacketCompositionItemEx packet = new PacketCompositionItemEx();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.UID = UID;
		packet.ItemID = ItemID;
		packet.slotIndex = slotIndex;
		packet.windowType = slotWindow;
		
		packet.DelItemUID = composMaterialUID;
		
		packet.materialUID = composAddMaterialUID;
		packet.ByJewel = bCash == true ? 1 : 0;
		
		packet.tutorial = isTutorial == true ? 1 : 0;
		
		SendPacket ("DoCompositionItem.php", packet);
	}
	
	public void SendRequestBuyCashItemNotify(CashItemInfo info)
	{
		/* yyMMddHHmmss_UserIndexID */
		string timestr = System.DateTime.Now.ToString("yyMMddHHmmss");
		
		PacketRequestBuyCashItem msg = new PacketRequestBuyCashItem();
		
		msg.characterIndex = connector.charIndex;
		
		msg.UserIndexID = connector.UserIndexID;
		msg.Store = connector.publisher;
		msg.ItemID = info.ItemID;
		msg.TStoreProductCode = info.GetStoreItemCode(connector.publisher);
		msg.TStoreTID = timestr + "_" + connector.UserIndexID.ToString();
		
		/*
		msg.type = (int)info.type;
		
		msg.addInfo = info.addInfo;		
		msg.itemName = info.itemName;
		msg.amount = (int)info.amount;
		msg.spriteName = info.spriteName;
		*/
		SendPacket ("BuyCashItem.php", msg);
		
		Logger.DebugLog ("SendRequestBuyCashItemNotify" + msg.TStoreTID);
	}
	
	public void SendResponeBuyCashItem(TStoreCashItemInfo info)
	{
		Logger.DebugLog ("SendResponeBuyCashItem" + info.TStoreTID);
		// 성공을 서버에 알린다. PacketBuyCashItem이 온다.
		PacketResponeBuyCashItem msg = new PacketResponeBuyCashItem();
		
		msg.characterIndex = connector.charIndex;
		
		msg.UserIndexID = connector.UserIndexID;
		msg.errorCode = NetErrorCode.OK;
		msg.ItemID = info.ItemID;
		msg.Store = connector.publisher;
		msg.TStoreProductCode = info.TStoreProductCode;
		msg.TStoreTID = info.TStoreTID;
		msg.OriginalJson = info.OriginalJson;
		msg.Siginature = info.Siginature;
		
		SendPacket ("BuyCashItem.php", msg);
	}

	
	public void SendResponeBuyCashItemFailed(TStoreCashItemInfo info)
	{
		// 구매실패. 
		// todo. 구매창을 닫는다.
		// 실패를 서버에 알린다. 응답은 없다.
		PacketResponeBuyCashItemFailed msg = new PacketResponeBuyCashItemFailed();
		
		msg.characterIndex = connector.charIndex;
		
		msg.UserIndexID = connector.UserIndexID;
		msg.errorString = info.errorString;
		msg.Store = connector.publisher;
		msg.ItemID = info.ItemID;
		msg.TStoreProductCode = info.TStoreProductCode;
		msg.TStoreTID = info.TStoreTID;		
		
		SendPacket ("BuyCashItem.php", msg);
	}
	
	public void SendRequestBuyCashItem(CashItemInfo info)
	{
		switch(info.paymentType)
		{
		case ePayment.Cash:
			// 현찰사용. 작업중.
			SendRequestBuyCashItemNotify(info);
			break;
		default:
			PacketBuyCashItem packet = new PacketBuyCashItem();
			
			packet.characterIndex = connector.charIndex;
			
			packet.UserIndexID = connector.UserIndexID; 
			packet.cashID = info.ItemID;
			
			SendPacket ("BuyCashItem.php", packet);
			break;
		}
	}
	
	/*
	public void SendRequestGambleInfo()
	{
		PacketRequestGambleInfo packet = new PacketRequestGambleInfo();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("Gamble.php", packet);
	}
	*/
	
	public void SendRefreshGambleItems(bool ByCash, int CharacterIndex, ref List<Item> Items)
	{
		PacketGambleRefresh packet = new PacketGambleRefresh();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		packet.ByCash = ByCash;
		
		int nCount = Items.Count;
		packet.Infos = new GambleItem[nCount];
		
		CharPrivateData privateData = null;
		CharInfoData charData = connector.charInfo;
		if (charData != null)
			privateData = charData.GetPrivateData(connector.charIndex);
		
		if (privateData != null)
			privateData.gambleItemList.Clear();
		
		for(int i = 0; i < nCount; ++i)
		{
			Item item = Items[i];
			GambleItem newGambleItem = new GambleItem();
			newGambleItem.ID = item.itemInfo.itemID;
			newGambleItem.Grade = (int)item.itemGrade;
			newGambleItem.itemRate = item.itemRateStep;
			
			if (privateData != null)
				privateData.gambleItemList.Add(newGambleItem);
			
			packet.Infos[i] = newGambleItem;
		}
		
		SendPacket("Gamble.php", packet);
	}
	
	public void SendSelectGambleItem(int CharacterIndex, int gambleType, int again)
	{
		PacketSelectGambleItem packet = new PacketSelectGambleItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		
		packet.what = gambleType;
		packet.bAgain = again;
		
		SendPacket("Gamble.php", packet);
	}


	public void SendChangeGambleItem(int index, Item newItem)
	{
		PacketChangeGambleItem packet = new PacketChangeGambleItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.GambleIndex = index;
		
		GambleItem gambleItem = null;
		if (newItem != null)
		{
			gambleItem = new GambleItem();
			gambleItem.ID = newItem.itemInfo.itemID;
			gambleItem.Grade = (int)newItem.itemGrade;
			gambleItem.itemRate = newItem.itemRateStep;
		}
		
		packet.Item = gambleItem;
		
		SendPacket("Gamble.php", packet);
	}
	
	public void SendWaveStartOrContinue(int CharacterIndex, int[] SelectedBuffs, int SeletedTowner, int curStamina, bool Start, int buyPotion1, int buyPotion2)
	{
		PacketWaveStart packet = new PacketWaveStart();
		
		if (!Start)
			packet.MsgID = NetID.WaveContinue;			
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		packet.SelectedBuffs = SelectedBuffs;
		packet.SelectedTower = SeletedTowner;
		packet.CurStamina = curStamina;
		
		packet.buyPotion1 = buyPotion1;
		packet.buyPotion2 = buyPotion2;
		
		SendPacket("WaveStart.php", packet);
	}
	
	public void SendWaveEnd(int CharacterIndex, int WaveStep, int DurationSec, int isClear, int usedPotion1, int usedPotion2)
	{
		PacketWaveEnd packet = new PacketWaveEnd();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		packet.WaveStep = WaveStep;
		packet.DurationSec = DurationSec;
		packet.Clear = isClear;
		//packet.usedItems = useItems;
		packet.usedPotion1 = usedPotion1;
		packet.usedPotion2 = usedPotion2;
		
		SendPacket("WaveEnd.php", packet);
	}
	
	public void SendBuyCashItem(int ItemID)
	{
		PacketBuyCashItem packet = new PacketBuyCashItem();
		
		packet.characterIndex = connector.charIndex;
		packet.UserIndexID = connector.UserIndexID;
		packet.cashID = ItemID;
		
		SendPacket("BuyCashItem.php", packet);
	}
	
	public void SendGMCheat(GMCMD cmd, int Value)
	{
		PacketGMCheat packet = new PacketGMCheat();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.cmd = cmd;
		packet.intValue = Value;
		
		SendPacket("CheatCommand.php", packet);
	}
	
	public void SendStageStart(int CharacterIndex, int stageType, int stageIndex, int[] selectedBuffs, int curStamina, int buyPotion1, int buyPotion2)
	{
		PacketStageStart packet = new PacketStageStart();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		packet.StageType = stageType;
		packet.StageIndex = stageIndex;
		packet.SelectedBuffs = selectedBuffs;
		packet.curStamina = curStamina;	
		
		packet.buyPotion1 = buyPotion1;
		packet.buyPotion2 = buyPotion2;

		SendPacket("StageStart.php", packet);
	}
	
	public void SendStageEndFailed(int CharacterIndex, int usedPotion1, int usedPotion2)
	{
		PacketStageEndFailed packet = new PacketStageEndFailed();
		
		// xgreen temp.
		//packet.success = 0;
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		//packet.usedItems = useItems;
		packet.usedPotion1 = usedPotion1;
		packet.usedPotion2 = usedPotion2;
	
		SendPacket("StageEnd.php", packet);
	}
	
	public void SendStageReward(int CharacterIndex, int stageType, int stageIndex, int price)
	{
		PacketStageReward packet = new PacketStageReward();
	
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		
		packet.stageType = stageType;
		packet.stageIndex = stageIndex;
		
		SendPacket("StageEnd.php", packet);
	}
	
	public void SendStageEnd( int CharacterIndex, int curLevel, int gainGold, int gainCash, GainItemInfo [] gainItems, GainItemInfo [] gainMaterialItems, int usedPotion1, int usedPotion2)
	{
		PacketStageEnd packet = new PacketStageEnd();
		
		// xgreen temp.
		//packet.success = 1;
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		
		packet.curLevel = curLevel;
		
		packet.gainGold = gainGold;
		packet.gainCash = gainCash;
		
		packet.gainNormalItems = gainItems;
		packet.gainMaterialItems = gainMaterialItems;
		
		//packet.usedItems = useItems;
		packet.usedPotion1 = usedPotion1;
		packet.usedPotion2 = usedPotion2;
	
		SendPacket("StageEnd.php", packet);
	}
	
	public void SendTutorialEnd(int charIndex)
	{
		PacketStageTutorial packet = new PacketStageTutorial();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = charIndex;
		
		SendPacket("StageEnd.php", packet);
	}
	
	public void SendTownTutorialEnd(int charIndex)
	{
		PacketTutorialDone packet = new PacketTutorialDone();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = charIndex;
		
		SendPacket("StageEnd.php", packet);
	}
	
	public void SendRecoveryStaminaByStage(int charIndex, int[] selectedBuffs, int stageIndex, int stageType, int buyPotion1, int buyPotion2)
	{
		PacketStageStartRecoveryStamina packet = new PacketStageStartRecoveryStamina();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = charIndex;
		
		packet.StageType = stageType;
		packet.StageIndex = stageIndex;
		packet.SelectedBuffs = selectedBuffs; 		// 0: None, 1:ê³µê²©?¥ê°??2:ë°©ìŽ?¥ê°??
		
		packet.buyPotion1 = buyPotion1;
		packet.buyPotion2 = buyPotion2;
		
		SendPacket("StageStartRecoveryStamina.php", packet);
	}
	
	public void SendRecoveryStamina(int CharacterIndex, int[] SelectedBuffs, int SeletedTower, bool Start, bool isWaveMode, int buyPotion1, int buyPotion2)
	{
		PacketWaveStartRecoveryStamina packet = new PacketWaveStartRecoveryStamina();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = CharacterIndex;
		
		packet.bWave = isWaveMode;
		packet.bStart = Start; 
		packet.SelectedTower = SeletedTower;
		packet.SelectedBuffs = SelectedBuffs;
		
		packet.buyPotion1 = buyPotion1;
		packet.buyPotion2 = buyPotion2;
		
		SendPacket("WaveStartBuyStamina.php", packet);
	}
	
	public void SendRequestMasteryUpgrade(SkillUpgradeDBInfo info)
	{
		PacketUpgradeSkill packet = new PacketUpgradeSkill();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.Info = info;
		
		SendPacket("Skill.php", packet);
	}
	
	public void SendRequestMasteryReset()
	{
		PacketResetSkill packet = new PacketResetSkill();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("Skill.php", packet);
	}
	
	public void SendRequestArenaInfo()
	{
		PacketEnterArena packet = new PacketEnterArena();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("ArenaEnter.php", packet);
	}

    public void SendRequestArenaStart(bool recovery = false)
	{
		PacketArenaStart packet = new PacketArenaStart();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("ArenaStart.php", packet);
	}
	
	public void SendRequestArenaEnd(bool bWin, long targetUserIndex, int targetCharIndex, string platform)
	{
		PacketArenaEnd packet = new PacketArenaEnd();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.bWin = bWin;
		packet.TargetUserIndexID = targetUserIndex;
		packet.TargetCharacterIndex = targetCharIndex;
		
		SendPacket("ArenaEnd.php", packet);
	}
	
	public void SendRequestArenaRanking(int rankType, int rank, bool bDown)
	{
		PacketRequestArenaRanking packet = new PacketRequestArenaRanking();
		
		packet.RankType = rankType;
		packet.ranking = rank;
		packet.bDown = bDown;
		
		SendPacket("ArenaRequestRanking.php", packet);
	}
	
	public void SendRequestEnterTown()
	{
		PacketEnterTown packet = new PacketEnterTown();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("EnterTown.php", packet);
	}
	
	public void SendRequestTargetEquipItem(long targetUserIndexID, int targetCharIndex, string platform)
	{
		PacketTargetEquipItem packet = new PacketTargetEquipItem();
		
		packet.UserIndexID = connector.UserIndexID;
		
		packet.TargetUserIndexID = targetUserIndexID;
		packet.TargetCharacterIndex = targetCharIndex;
		
		SendPacket("RequestTargetInfo.php", packet);
	}
	
	public void SendRequestArenaStartBuyTicket()
	{
		PacketArenaStartBuyTicket packet = new PacketArenaStartBuyTicket();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("ArenaStartBuyTicket.php", packet);
	}
	
	public void SendRequestWaveInfo()
	{
		PacketEnterWave packet = new PacketEnterWave();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("WaveEnter.php", packet);
	}
	
	public void SendRequestWaveRanking(int ranking, bool bDown)
	{
		PacketWaveRanking packet = new PacketWaveRanking();
		
		packet.ranking = ranking;
		packet.bDown = bDown;
		
		SendPacket("WaveRequestRanking.php", packet);
	}
	
	public void SendRequestWaveContinue()
	{
		PacketWaveContinue packet = new PacketWaveContinue();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("WaveStart.php", packet);
	}

    public void SendRequestPostInfo(string id)
	{
		Game.Instance.postItemList.Clear();
		
		PacketEnterPost packet = new PacketEnterPost();
		
		packet.UserIndexID = connector.UserIndexID;
		
		SendPacket("Post.php", packet);
	}
	
	public void SendRequestPostItem(MailInfo info)
	{
		PacketRequestPostItem packet = new PacketRequestPostItem();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		packet.mailIndex = info.Index;
		
		SendPacket("Post.php", packet);
	}
	
	public void SendRequestPostMessage(MailInfo info)
	{
		PacketRequestPostMsg packet = new PacketRequestPostMsg();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.mailIndex = info.Index;
		
		SendPacket("Post.php", packet);
	}
	
	public void SendRequestPostItemAll()
	{
		PacketRequestPostItemAll packet = new PacketRequestPostItemAll();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("Post.php", packet);
	}
	
	public void SendRequestFriendList(BaseFriendListWindow.eFriendListType listType)
	{
		switch(listType)
		{
		case BaseFriendListWindow.eFriendListType.FriendList:
			PacketFriendList friendList = new PacketFriendList();
			friendList.CharacterIndex = connector.charIndex;
			friendList.UserIndexID = connector.UserIndexID;
			
			SendPacket("Messenger.php", friendList);
			break;
		case BaseFriendListWindow.eFriendListType.InviteList:
			PacketRecommandFriendList inviteFriendList = new PacketRecommandFriendList();
			inviteFriendList.CharacterIndex = connector.charIndex;
			inviteFriendList.UserIndexID = connector.UserIndexID;
			
			SendPacket("Messenger.php", inviteFriendList);
			break;
		case BaseFriendListWindow.eFriendListType.AcceptList:
			PacketInvitedUserList acceptFriendList = new PacketInvitedUserList();
			acceptFriendList.CharacterIndex = connector.charIndex;
			acceptFriendList.UserIndexID = connector.UserIndexID;
			
			SendPacket("Messenger.php", acceptFriendList);
			break;
		}
	}
	
	public void SendRequestFriendFunc(BaseFriendListWindow.eFriendListType listType, FriendSimpleInfo info)
	{
		if (info == null)
			return;
		
		switch(listType)
		{
		case BaseFriendListWindow.eFriendListType.FriendList:
			SendRequestSendStamina(info.UserID);
			break;
		case BaseFriendListWindow.eFriendListType.InviteList:
			SendRequestInviteFriend(info.UserID);
			break;
		case BaseFriendListWindow.eFriendListType.AcceptList:
			SendRequestAcceptFriend(info.UserID);
			break;
		}
	}

    public void SendRequestSendStamina(long targetUserID, string platform = "kakao", string nick = "")
	{
		PacketSendStaminaToFriend sendStamina = new PacketSendStaminaToFriend();
		sendStamina.UserIndexID = connector.UserIndexID;
		sendStamina.FriendID = targetUserID;
		
		SendPacket("Messenger.php", sendStamina);
	}

    public void SendRequestInviteFriend(long targetUserID, string platform = "kakao")
	{
		PacketFriendInvite inviteFriend = new PacketFriendInvite();
		inviteFriend.UserIndexID = connector.UserIndexID;
		inviteFriend.InvitedUserID = targetUserID;
		
		SendPacket("Messenger.php", inviteFriend);
	}
	
	public void SendRequestAcceptFriend(long targetUserID, string platform = "kakao")
	{
		PacketFriendInviteAccept acceptFriend = new PacketFriendInviteAccept();
		acceptFriend.UserIndexID = connector.UserIndexID;
		acceptFriend.Friend = targetUserID;
		
		SendPacket("Messenger.php", acceptFriend);
	}

    public void SendRequestDeleteFriend(long targetUserID, string platform)
	{
		PacketFriendDelete deleteFriend = new PacketFriendDelete();
		deleteFriend.UserIndexID = connector.UserIndexID;
		deleteFriend.FriendID = targetUserID;
		
		SendPacket("Messenger.php", deleteFriend);
	}
	
	public void SendAchievementProcess(List<Achievement> achievementList)
	{
		PacketAchievementProgress achievementProcess = new PacketAchievementProgress();
		
		List<int> groupIDList = new List<int>();
		List<int> countList = new List<int>();
		
		int groupID = -1;
		int totalCount = 0;
		foreach(Achievement achievement in achievementList)
		{
			groupID = achievement.id;
			totalCount = achievement.curCount + achievement.addCount;
			
			Game.Instance.AndroidManager.CallUnityAchievement(groupID, achievement.addCount);
			
			groupIDList.Add(groupID);
			countList.Add(totalCount);
		}
		
		achievementProcess.UserIndexID = connector.UserIndexID;
		achievementProcess.characterIndex = connector.charIndex;
		
		achievementProcess.groupIDs = groupIDList.ToArray();
		achievementProcess.counts = countList.ToArray();
		
		SendPacket("Achievement.php", achievementProcess);		
	}
	
	public void SendRequestAcceptAchieveReward(int id, int step, int achieveCharIndex)
	{
		PacketAchievementReward achievementReward = new PacketAchievementReward();
		achievementReward.UserIndexID = connector.UserIndexID;
		
		achievementReward.characterIndex = achieveCharIndex;
		achievementReward.groupID = id;
		achievementReward.stepID = step;
		
		SendPacket("Achievement.php", achievementReward);
	}
	
	public void SendDailyAchievementProcess(List<Achievement> achievementList)
	{
		PacketDailyMissionProgress achievementProcess = new PacketDailyMissionProgress();
		
		List<int> groupIDList = new List<int>();
		List<int> countList = new List<int>();
		
		int groupID = -1;
		int totalCount = 0;
		foreach(Achievement achievement in achievementList)
		{
			groupID = achievement.id;
			totalCount = achievement.curCount + achievement.addCount;
			
			groupIDList.Add(groupID);
			countList.Add(totalCount);
		}
		
		achievementProcess.UserIndexID = connector.UserIndexID;
		
		achievementProcess.ids = groupIDList.ToArray();
		achievementProcess.counts = countList.ToArray();
		
		SendPacket("DailyMission.php", achievementProcess);
	}
	
	public void SendRequestAcceptDailyAchieveReward(int id, int step)
	{
		PacketDailyMissionReward achievementReward = new PacketDailyMissionReward();
		achievementReward.id = id;
		achievementReward.UserIndexID = connector.UserIndexID;
		
		SendPacket("DailyMission.php", achievementReward);
	}
	
	public void SendUpdateStamina(int curStamina)
	{
		PacketUpdateStamina updateStamina = new PacketUpdateStamina();
		updateStamina.curStamina = curStamina;
		updateStamina.UserIndexID = connector.UserIndexID;
		updateStamina.CharacterIndex = connector.charIndex;
		
		SendPacket("Stamina.php", updateStamina);
	}
	
	public void SendRequestBossRaidEnter()
	{
		PacketBossRaidEnter bossRaidEnter = new PacketBossRaidEnter();
		bossRaidEnter.UserIndexID = connector.UserIndexID;
		
		SendPacket("BossRaid.php", bossRaidEnter);
	}
	
	public void SendRequestBossRaidStart(long bossIndex, bool recovery, string platform, string owner_id)
	{
		PacketBossRaidStart bossRaidStart = new PacketBossRaidStart();
		bossRaidStart.UserIndexID = connector.UserIndexID;
		bossRaidStart.CharacterIndex = connector.charIndex;
		
		bossRaidStart.index = bossIndex;
		
		SendPacket("BossRaid.php", bossRaidStart);
	}

    public void SendRecoveryStaminaByBossRaidStart(long bossIndex, string platform, string owner_id)
	{
		PacketBossRaidStartRecoveryStamina bossRaidStart = new PacketBossRaidStartRecoveryStamina();
		bossRaidStart.UserIndexID = connector.UserIndexID;
		bossRaidStart.CharacterIndex = connector.charIndex;
		
		bossRaidStart.index = bossIndex;
		
		SendPacket("BossRaid.php", bossRaidStart);
	}

    public void SendBossRaidEnd(long bossIndex, float damageValue, bool isPhase2, int curHP, string platform, string owner_id, int boss_id)
	{
		PacketBossRaidEnd bossRaidEnd = new PacketBossRaidEnd();
		
		bossRaidEnd.UserIndexID = connector.UserIndexID;
		bossRaidEnd.index = bossIndex;
		bossRaidEnd.damage = (int)damageValue;
		
		if (isPhase2 == true)
			bossRaidEnd.transform = 1;
		else
			bossRaidEnd.transform = 0;
		
		bossRaidEnd.bossHP = curHP;
		
		SendPacket("BossRaid.php", bossRaidEnd);
	}
	
	public void SendPopupNoticeIgnore(long noticeID)
	{
		PacketPopupNoticeIgnore popupNoticeIgnore = new PacketPopupNoticeIgnore();
		
		popupNoticeIgnore.UserIndexID = connector.UserIndexID;
		popupNoticeIgnore.ID = noticeID;
		
		SendPacket("NoticeIgnore.php", popupNoticeIgnore);
	}
	
	public void SendRequestRevival(int gold, int cash)
	{
		PacketRevival revival = new PacketRevival();
		
		revival.UserIndexID = connector.UserIndexID;
		revival.CharacterIndex = connector.charIndex;
		
		revival.TotalGold = gold;
		revival.TotalCash = cash;
		
		SendPacket("Revival.php", revival);
		
	}
	
	public void RequestCheckNickName(string nickName)
	{
        PacketCheckNickName checkNick = new PacketCheckNickName();
        checkNick.UserIndexID = connector.UserIndexID;
        checkNick.NickName = nickName;

        SendPacket("NickName.php", checkNick);
	}
	
	public void RequestCreateNickName(string nickName)
	{
        PacketCreateNickName createNick = new PacketCreateNickName();
        createNick.UserIndexID = connector.UserIndexID;
        createNick.NickName = nickName;
        createNick.publisherID = connector.publisher;

        SendPacket("NickName.php", createNick);
	}
	
	public void RequestInviteFriendByNick(string nickName)
	{
		PacketFriendInviteByNickName inviteByNick = new PacketFriendInviteByNickName();
		
		inviteByNick.UserIndexID = connector.UserIndexID;
		
		inviteByNick.InvitedNickName = nickName;
		
		SendPacket("Messenger.php", inviteByNick);
	}
	
	public void RequestInviteFriendByKatalk(string Nick)
	{
		if (Game.Instance.AndroidManager)
			Game.Instance.AndroidManager.OnClickKakaoLink(Nick);
	}
	
	public void RequestSignup(string id, string pass1, string pass2)
	{
        PacketCreateAccount createAccount = new PacketCreateAccount();

        createAccount.Account = id;
        createAccount.Password = pass1;
        createAccount.EmailType = 0;

        SendPacket("Login.php", createAccount);
	}
	
	public void SendRequestGambleInfo()
	{
		PacketRequestGambleInfo requestGambleInfo = new PacketRequestGambleInfo();
		
		requestGambleInfo.UserIndexID = connector.UserIndexID;
		requestGambleInfo.CharacterIndex = connector.charIndex;
		
		SendPacket("Gamble.php", requestGambleInfo);
	}
	
	public void SendRequestExpandSlots(GameDef.eItemSlotWindow slotWindow)
	{
		PacketInvenExtend extend = new PacketInvenExtend();
		
		extend.UserIndexID = connector.UserIndexID;
		extend.CharacterIndex = connector.charIndex;
		
		switch(slotWindow)
		{
		case GameDef.eItemSlotWindow.Inventory:
			extend.bNormalInven = 1;
			break;
		case GameDef.eItemSlotWindow.MaterialItem:
			extend.bNormalInven = 0;
			break;
		default:
			extend.bNormalInven = -1;
			break;
		}
		
		SendPacket("Inventory.php", extend);
	}
	
	public void SendRequestServerChecking(string cookie)
	{
		PacketRequestServerChecking packet = new PacketRequestServerChecking();
		packet.version = Version.NetVersion;
		packet.cookie = cookie;
		
		Connector.SendServerCheckPacket(packet);
	}
	
	public void SendRequestMemberSecession(string accountStr, string passStr, AccountType type)
	{
		PacketDropout packet = new PacketDropout();
		packet.UserIndexID = connector.UserIndexID;
		
		packet.AccountID = accountStr;
		packet.Password = passStr;
		packet.EmailType = type;
		
		SendPacket("Login.php", packet);
		
		
		Game.Instance.AndroidManager.OnClickGoogleLogout();
	}
	
	public void SendRequestCoupon(string couponNumber)
	{
		PacketCoupon packet = new PacketCoupon();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.serialno = couponNumber;
		
		SendPacket("coupon.php", packet);
	}
	
	public void SendRequestGameReview()
	{
		PacketRequestReviewReward packet = new PacketRequestReviewReward();
		
		packet.UserIndexID = connector.UserIndexID;
		
		SendPacket("option.php", packet);
		
	}
	
	public void SendKeepAlive()
	{
		Header packet = new Header();
		
		packet.UserIndexID = connector.UserIndexID;
		
		Connector.SendPacket("KeepAlive.php", packet);
	}
	
	public void SendIgnorePush(bool bToggle)
	{
		PacketIgnorePush packet = new PacketIgnorePush();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.off = bToggle == true ? 1 : 0;
		
		SendPacket("option.php", packet);
	}
	
	public void SendSpecialAchievementProcess(List<Achievement> achievementList)
	{
		PacketSpecialMissionProgress achievementProcess = new PacketSpecialMissionProgress();
		
		List<int> groupIDList = new List<int>();
		List<int> countList = new List<int>();
		
		int groupID = -1;
		int totalCount = 0;
		foreach(Achievement achievement in achievementList)
		{
			groupID = achievement.id;
			totalCount = achievement.curCount + achievement.addCount;
			
			Game.Instance.AndroidManager.CallUnityAchievement(groupID, achievement.addCount);
			
			groupIDList.Add(groupID);
			countList.Add(totalCount);
		}
		
		achievementProcess.UserIndexID = connector.UserIndexID;
		achievementProcess.characterIndex = connector.charIndex;
		
		achievementProcess.groupIDs = groupIDList.ToArray();
		achievementProcess.counts = countList.ToArray();
		
		SendPacket("SpecialMission.php", achievementProcess);		
	}
	
	public void SendRequestAcceptSpecialAchieveReward(int id, int step, int charIndex)
	{
		PacketSpecialMissionReward packet = new PacketSpecialMissionReward();
		packet.UserIndexID = connector.UserIndexID;
		
		packet.groupID = id;
		packet.stepID = step;
		packet.characterIndex = charIndex;
		
		SendPacket("SpecialMission.php", packet);
	}
	
	public void SendRequestAwakeningUpgrade(SkillUpgradeDBInfo info)
	{
		PacketAwakningUpgradeSkill packet = new PacketAwakningUpgradeSkill();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.Info = info;
		
		SendPacket("Skill.php", packet);
	}
	
	public void SendRequestAwakeningReset()
	{
		PacketAwakeningResetSkill packet = new PacketAwakeningResetSkill();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		SendPacket("Skill.php", packet);
	}
	
	public void SendRequestAwakeningBuyPoint(int buyCount)
	{
		PacketAwakeningBuyPoint packet = new PacketAwakeningBuyPoint();
		
		packet.UserIndexID = connector.UserIndexID;
		packet.CharacterIndex = connector.charIndex;
		
		packet.Count = buyCount;
		
		SendPacket("Skill.php", packet);
	}
	
	public void SendRequsetInviteKakaoFriendByUserID(string user_id)
	{
		throw new NotImplementedException();
	}
	
	public void SendProfileImageOnOff(bool bToggle)
	{
		throw new NotImplementedException();
	}
	
	public void GetKakaoInfo()
	{
		throw new NotImplementedException();
	}
	
	public void SendStaminaForAll()
	{
		throw new NotImplementedException();
	}
	
	public void CheckNickName()
	{
		throw new NotImplementedException();
	}
	
	public void GetUserInfo()
	{
		throw new NotImplementedException();
	}

    public void SelectHero(int hero_type)
    {
        throw new NotImplementedException();
    }
	
	public void TimeOutProcess(string functionName, string jsonString)
	{
		throw new NotImplementedException();
	}
	
	public void SendUpdateFailedKakaoFriends(string jsonString)
	{
		throw new NotImplementedException();
	}
}
