using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetTestUnit 
{
	
	public ArrayList waitMsgList;
	public IPacketSender sender;
	public ClientConnector clientConnector;
	
	public int CharacterIndex = 0;
	
	public PlayerController PlayerController
    {
        get { 
				return Game.Instance.player;
		}
	}	
	
	public LifeManager LifeManager
    {
        get { 
			if (PlayerController)
				return PlayerController.lifeManager;
			
			Game.Instance.CreateCharacter(Vector3.zero);
			
			return Game.Instance.player.lifeManager;
		}
	}
			
	public InventoryManager InvenManager
	{
		get { 
			if (LifeManager)
				return LifeManager.inventoryManager;
			
			return null;
		}
	}
	
	public ClientConnector connector
	{
		get {
			if (clientConnector)
				return clientConnector;
			
			clientConnector = sender.Connector;
				
			return clientConnector;
		}
		
	}
		
	public NetTestUnit(IPacketSender sender, NetID WaitMsgID)
	{
		waitMsgList = new ArrayList();
		
		this.sender = sender;
		
		if (WaitMsgID != NetID.None)
			waitMsgList.Add (WaitMsgID);
	}
	
	public bool IsWaitMsgEmpty()
	{
		if (waitMsgList.Count ==0)
			return true;
		
		return false;
	}
	
	public bool IsWaitMsg(NetID msgID)
	{
		int index = waitMsgList.IndexOf(msgID);
		
		if (index >= 0)
			return true;
		
		return false;
	}
	
	public virtual void DoTest()
	{
	}
	
	public virtual void RecvMessage(NetID msgID, string packet)
	{}
}

public class NetTestCreateAccount : NetTestUnit
{
	public NetTestCreateAccount(IPacketSender sender) : base(sender, NetID.LoginDone)
	{
		
	}
	
	public override void DoTest() 
	{
		string Account = "UnitTest";
		string password= "bb";
		
		int index = Random.Range(0, 100000);
		
		sender.SendLogin(Account + index.ToString(), password, AccountType.MonsterSide);
		Debug.Log("TEST SendCreateAccount");
	}
}

public class NetTestLogin : NetTestUnit
{
	public NetTestLogin(IPacketSender sender) : base(sender, NetID.LoginDone)
	{
		
	}
	
	public override void DoTest() 
	{
		string Account = "UnitTest";
		string password= "bb";
		
		sender.SendLogin(Account, password, AccountType.MonsterSide);
		Debug.Log("TEST SendLogin");
	}
}

public class NetTestWaveStart : NetTestUnit
{
	public NetTestWaveStart(IPacketSender sender) : base(sender, NetID.WaveStartRespone)
	{
		
	}
	
	public override void DoTest()
	{
		int[] buffs = new int[1];
		buffs[0] = 1;
		sender.SendWaveStartOrContinue(1, buffs, 0, 55, true, 0, 0);
		
		Debug.Log("TEST SendWaveStartOrContinue");
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketWaveStartRespone msg = LitJson.JsonMapper.ToObject<PacketWaveStartRespone>(packet);		
		
		if (msg.errorCode == NetErrorCode.OK)
		{
			Debug.Log("TEST SendWaveStartOrContinue OK");
			return;
		}
			
		Debug.LogError("NetTestWaveStart Error:" + msg.errorCode.ToString());
	}

}

public class NetTestWaveEnd : NetTestUnit
{
	public NetTestWaveEnd(IPacketSender sender) : base(sender, NetID.WaveEndRespone)
	{
		
	}
	
	public override void DoTest()
	{
		Debug.Log("TEST SendWaveEnd");
		
		sender.SendWaveEnd(1, 10, 500, 0, 0, 0);
		
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketWaveEndRespone msg = LitJson.JsonMapper.ToObject<PacketWaveEndRespone>(packet);		
		
		if (msg.errorCode == NetErrorCode.OK)
		{
			Debug.Log("TEST SendWaveEnd OK");
			return;
		}
			
		Debug.LogError("PacketWaveEnd Error:" + msg.errorCode.ToString());
	}
}

public class NetTestBuyNormalItem : NetTestUnit
{
	int BuyItemID = 900001;			// 체력 회복 물약.
	
	public NetTestBuyNormalItem(IPacketSender sender, int BuyItemID) : base(sender, NetID.BuyNormalItem)
	{
		this.BuyItemID = BuyItemID;
	}
	
	public override void DoTest()
	{
		Debug.Log("TEST SendRequestBuyNormalItem");
		
		sender.SendRequestBuyNormalItem(BuyItemID, 1, 0,GameDef.eItemSlotWindow.Inventory);
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketBuyNormalItem msg = LitJson.JsonMapper.ToObject<PacketBuyNormalItem>(packet);		
		
		if (msg.result == NetErrorCode.OK)
		{
			Debug.Log("TEST SendRequestBuyNormalItem OK");
			return;
		}
			
		Debug.LogError("PacketBuyNormalItem Error:" + msg.result.ToString());
		
		if (msg.result == NetErrorCode.NotEnoughGold)
		{
			sender.SendGMCheat(GMCMD.AddGold, 10000);
		}
	}
}

public class NetTestBuyCostumItem : NetTestUnit
{
	int BuyItemID= 91003;		// 힙합 워리어.
	
	public NetTestBuyCostumItem(IPacketSender sender, int BuyItemID) : base(sender, NetID.BuyCostumeItem)
	{
		this.BuyItemID = BuyItemID;
	}
	
	public override void DoTest()
	{
		Debug.Log("TEST SendRequestBuyCostumeItem");
		
		sender.SendRequestBuyCostumeItem(BuyItemID, 0);
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketBuyCostumeItem msg = LitJson.JsonMapper.ToObject<PacketBuyCostumeItem>(packet);		
		
		if (msg.result == NetErrorCode.OK)
		{
			Debug.Log("TEST SendRequestBuyCostumeItem OK");
			return;
		}
			
		Debug.LogError("PacketBuyCostumeItem Error:" + msg.result.ToString());
		
		if (msg.result == NetErrorCode.NotEnoughGold)
		{
			sender.SendGMCheat(GMCMD.AddGold, 10000);			
		}
	}
}


public class NetTestSellEquipItem : NetTestUnit
{
	public NetTestSellEquipItem(IPacketSender sender) : base(sender, NetID.SellEquipItem)	{}
	
	public override void DoTest()
	{
		Debug.Log("TEST SendNetTestSellEquipItem");
		
//		if (connector.charInfolist[CharacterIndex].equipList.Count <= 0)
//			Debug.LogError("Test SellEquipItem 팔 장착아이템이 없다.");
//		
//		foreach (var pair in connector.charInfolist[CharacterIndex].equipList)
//		{
//			Item info = pair.Value;
//			
//			if (info == null)
//			{
//				Debug.LogError("Test SendRequestSellNormalItemFromStorage Item null");
//				return;
//			}
//	
//			sender.SendRequestSellEquipItem(CharacterIndex,  (int)pair.Key, info.itemInfo.itemID, info.uID);
//		}
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketSellItem msg = LitJson.JsonMapper.ToObject<PacketSellItem>(packet);		
		
		if (msg.result == NetErrorCode.OK)
		{
			Debug.Log("TEST SendNetTestSellEquipItem OK");
			return;
		}
		
		Debug.LogError("PacketSellItem Error:" + msg.result.ToString());
	}
}
	
	
public class NetTestSellNormalItem : NetTestUnit
{
	public NetTestSellNormalItem(IPacketSender sender) : base(sender, NetID.SellInvenItem)	{}
	
	public override void DoTest()
	{
		Debug.Log("TEST SendRequestSellNormalItemFromStorage");
		
		Item info = InvenManager.GetItem(0);
		
		if (info == null)
		{
			Debug.LogError("Test SendRequestSellNormalItemFromStorage Item null");
			return;
		}

		sender.SendRequestSellNormalItemFromStorage(CharacterIndex, 0, info.itemInfo.itemID, info.uID);
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketSellItem msg = LitJson.JsonMapper.ToObject<PacketSellItem>(packet);		
		
		if (msg.result == NetErrorCode.OK)
		{
			Debug.Log("TEST SendRequestSellNormalItemFromStorage OK");
			return;
		}
		
		Debug.LogError("PacketSellItem Error:" + msg.result.ToString());
	}
}

public class NetTestSellCostumeItem : NetTestUnit
{
	public NetTestSellCostumeItem(IPacketSender sender) : base(sender, NetID.SellCostumeItem)
	{
		
	}
	
	public override void DoTest()
	{
		Debug.Log("TEST SendRequestSellCostumeItemFromStorage");
		
		Item info = InvenManager.GetCostumeItem(0);
		
		if (info == null)
		{
			Debug.LogError("Test SendRequestSell Item null");
			return;
		}

		sender.SendRequestSellCostumeItemFromStorage(CharacterIndex, 0, info.itemInfo.itemID, info.uID);
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketSellItem msg = LitJson.JsonMapper.ToObject<PacketSellItem>(packet);		
		
		if (msg.result == NetErrorCode.OK)
		{
			Debug.Log("TEST SendRequestSellCostumeItemFromStorage OK");
			return;
		}
		
		Debug.LogError("PacketSellItem Error:" + msg.result.ToString());
	}
}

public class NetTestEquipItem : NetTestUnit
{
	GameDef.eSlotType equipSlot = GameDef.eSlotType.Potion_1;
	
	public NetTestEquipItem(IPacketSender sender) : base(sender, NetID.DoEquipItemRespone)	{}
	
	public override void DoTest()
	{
		Debug.Log("TEST NetTestEquipItem");
		
		// 900001 체력 회복 물약.
		Item info = InvenManager.GetItem(0);
		
		if (info == null)
		{
			Debug.LogError("장착할 아이템이 없다.");
			info = InvenManager.GetItem(0);
			return;
		}	
		
		sender.SendRequestDoEquipItem((int)equipSlot, 0, info.itemInfo.itemID, info.uID, GameDef.eItemSlotWindow.Inventory);
		
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketDoEquipItemRespone pd = LitJson.JsonMapper.ToObject<PacketDoEquipItemRespone>(packet);		
		
		if (pd.errorCode == NetErrorCode.OK)
		{
			CharPrivateData privateData = connector.charInfo.privateDatas[pd.CharacterIndex];
			
			if (privateData != null)
				privateData.AddEquipItem(pd.equipSlotIndex, pd.Equip);

			Debug.Log("TEST NetTestEquipItem OK");
			return;
		}
		
		Debug.LogError("TEST NetTestEquipItem Error:" + pd.errorCode.ToString());
	}
}

public class NetTestUnEquipItem : NetTestUnit
{
	GameDef.eSlotType equipSlot = GameDef.eSlotType.Potion_1;
	//int ItemID =  900001;
	
	public NetTestUnEquipItem(IPacketSender sender) : base(sender, NetID.DoUnEquipItemRespone)	{}
	
	public override void DoTest()
	{
		Debug.Log("TEST NetTestUnEquipItem");
		
		CharPrivateData privateData = connector.charInfo.privateDatas[CharacterIndex];
		Item info = privateData.GetEquipItem((int)equipSlot);
		
		if (info == null)
		{
			Debug.LogError("탈착아이템이 없다.");
			return;
		}	
		sender.SendRequestDoUnEquipItem((int)equipSlot, info.uID, info.itemInfo.itemID);
		
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketDoUnEquipItemRespone msg = LitJson.JsonMapper.ToObject<PacketDoUnEquipItemRespone>(packet);		
		
		if (msg.errorCode == NetErrorCode.OK)
		{
			Debug.Log("TEST NetTestUnEquipItem OK");
			return;
		}
		
		Debug.LogError("TEST NetTestUnEquipItem Error:" + msg.errorCode.ToString());
	}
}


public class NetTestRefreshGamble : NetTestUnit
{
	public NetTestRefreshGamble(IPacketSender sender) : base(sender, NetID.GambleRefreshRespone)
	{
		
	}
	
	public override void DoTest()
	{
		Debug.Log("TEST SendRefreshGambleItems");
		List<Item> Items = new List<Item>();
		
		for(int i = 0; i < 12; ++i)
		{
			Item pd = new Item();
			pd.itemInfo = new ItemInfo();
			pd.itemInfo.itemID = 91201;
			pd.itemGrade = 0;
			pd.itemCount = 1;
			Items.Add(pd);
		}
		
		sender.SendRefreshGambleItems(true, 1, ref Items);
	}
	
	public override void RecvMessage(NetID msgID, string packet)
	{
		PacketGambleRefreshRespone msg = LitJson.JsonMapper.ToObject<PacketGambleRefreshRespone>(packet);		
		
		if (msg.errorCode == NetErrorCode.OK)
		{
			Debug.Log("TEST SendRefreshGambleItems OK");
			return;
		}
		
		Debug.LogError("TEST SendRefreshGambleItems Error:" + msg.errorCode.ToString());
	}
}
