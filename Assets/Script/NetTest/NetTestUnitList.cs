using UnityEngine;
using System.Collections.Generic;

public class NetTestUnitList 
{
	Stack<NetTestUnit> taskList = new Stack<NetTestUnit>();
	
	NetTestUnit curTest; 
	IPacketSender sender;
	
	public NetTestUnitList(IPacketSender sender)
	{
		this.sender = sender;
	}
	
	public void RunTestAll()
	{
		Debug.Log("Run Test");
		
		RunTest();
	}
	
	void RunTest()
	{
		while(taskList.Count > 0)
		{
			curTest = taskList.Pop();
			curTest.DoTest();
		
			// error Handling.
			if (!curTest.IsWaitMsgEmpty())
				break;
		}
		
		if (taskList.Count == 0)
			Debug.Log("Run Test Done");
	}
	
	public void ErrorHandler(PacketError msg)
	{
		switch((NetErrorCode)msg.ErrorCode)
		{
		case NetErrorCode.NotEnoughCash : {} break;
		case NetErrorCode.NotEnoughGold : {} break;
		case NetErrorCode.NotEnoughInven : {} break;
		case NetErrorCode.NotEnoughCostumeInven : {} break;
		}
		
		Debug.LogError("Error:" + msg.ErrorMessage);
	}
	
	public void MessageProcess(NetID msgID, string recv)
	{
		if (curTest == null)
			return;
		
		if (msgID == NetID.Error)
		{
			ErrorHandler(LitJson.JsonMapper.ToObject<PacketError>(recv));
		}
		
		if (curTest.IsWaitMsg(msgID))
		{
			curTest.RecvMessage(msgID, recv);
			
			RunTest();
		}
	}
	
	public void  RegisterTest()
	{
		
		taskList.Push (new NetTestRefreshGamble(sender));
		
		taskList.Push (new NetTestWaveEnd(sender));
		taskList.Push (new NetTestWaveStart(sender));
		
		taskList.Push (new NetTestSellNormalItem(sender));				// 팔고.
		taskList.Push (new NetTestUnEquipItem(sender));					// 장착해제하고.	
		taskList.Push (new NetTestEquipItem(sender));						// 물약장착하고.	
		taskList.Push (new NetTestBuyNormalItem(sender, 900001)); 	// 물약 사고.
		taskList.Push (new NetTestLogin(sender));	
		
		
		// taskList.Push (new NetTestCreateAccount(sender));	
		
	}
		
}
