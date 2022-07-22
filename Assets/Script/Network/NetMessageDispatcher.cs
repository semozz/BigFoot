using UnityEngine;
using System.Collections.Generic;

using LitJson;


public delegate void MessageHandler(string packet);

public class NetMessageDispatcher 
{
	ClientConnector connector;
	public NetTestUnitList netTestUnitList;

    Dictionary<NetID, MessageHandler> lstData;
    
    public NetMessageDispatcher(ClientConnector connector)
    {
        lstData = new Dictionary<NetID, MessageHandler>();
		
		this.connector = connector;
    }
    
    public void RegisterHandler(NetID msgID, MessageHandler handler)
    {
        lstData.Add(msgID, handler);    
    }
	
	public void SendJsonError()
	{
		PacketError msg = new PacketError();
		msg.ErrorCode = (int)NetErrorCode.JsonException;
		string str = LitJson.JsonMapper.ToJson(msg);
		lstData[NetID.Error](str);
	}
	
	public void MessageProcessInternal(string onePacket)
	{
		Header msg = new Header();

        try
        {
			msg = LitJson.JsonMapper.ToObject<Header>(onePacket);		
			//Logger.DebugLog ("Recv MsgID:" + msg.MsgID);
			connector.token = msg.token;
		
			if (lstData.ContainsKey(msg.MsgID) == true)
			{
				GameUI.Instance.CancelWait();
				
				lstData[msg.MsgID](onePacket);
			}
			else
				  Logger.ErrorLog(msg.MsgID + ": The given key was not present in the dictionary.");
			
			if (netTestUnitList != null)
				netTestUnitList.MessageProcess(msg.MsgID, onePacket);

        }
    
		catch (JsonException e)
		{
			SendJsonError();
			Logger.DebugLog(onePacket);
			Logger.DebugLog(e.Message);
		}	
	}
    
    public void MessageProcess(string json)
    {
		//if (netTestUnitList != null)
			Logger.DebugLog(json);
		
		int Index = 0;
		int Count = 0;
		
		// Todo xgreen. 개선합시다.
		for(int i = 0; i < json.Length; ++i)
		{
			if (json[i] == '{')
			{
				if (Count == 0)
					Index = i;
				
				++Count;
			}
			
			if (json[i] == '}')
			{
				--Count;
				
				if (Count <= 0)
				{
					MessageProcessInternal(json.Substring(Index, i-Index+1));
				}					
			}
		}
	}
}
