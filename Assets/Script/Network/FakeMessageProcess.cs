using UnityEngine;
using System.Collections;

public class FakeMessageProcess
{
	ClientConnector _connector;
    int UserIndexID = 100001;
	
	void SendConnector(string packet)
	{
		_connector.MessageProcess(packet);
	}
	
	void LoginProcess(string packet)
	{
        //PacketLogin pd = LitJson.JsonMapper.ToObject<PacketLogin>(packet);
		
		// sendUserInfo
		
		PacketUserInfo msg = new PacketUserInfo();
		
		msg.UserIndexID = UserIndexID;
		//msg.AccountID = pd.AccountID;
		msg.NickName = "asdfdf";
		msg.Cash = 100;
		msg.Gold = 100000;
		msg.Medal = 7;

		
		SendConnector(LitJson.JsonMapper.ToJson(msg));
		
		
		// sendCharacterInfo
		
		PacketCharacterInfo charInfo = new PacketCharacterInfo();
		
		// send EquipItem
		
		// Send MastaryInfo
		
		// Send LoginDone
		SendConnector(LitJson.JsonMapper.ToJson(charInfo));
		
		
		Header done = new Header();
		done.MsgID = NetID.LoginDone;
		
		SendConnector(LitJson.JsonMapper.ToJson(done));
		
	}
	
	void BuyNormalItemProcess(string packet)
	{
		PacketBuyNormalItem pd = LitJson.JsonMapper.ToObject<PacketBuyNormalItem>(packet);
		
		pd.result = 0;
		//pd.UID = "";
		
		SendConnector(LitJson.JsonMapper.ToJson(pd));
	}
	
	public void RegisterHandler(ClientConnector connector, ref NetMessageDispatcher dispatcher)
	{
		_connector = connector;
		
		dispatcher.RegisterHandler(NetID.Login, LoginProcess);
		dispatcher.RegisterHandler(NetID.BuyNormalItem, BuyNormalItemProcess);
	}
}
