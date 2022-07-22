using UnityEngine;
using System.Collections;

public class NetworkUnitTest : MonoBehaviour 
{
	public NetConfig.HostType hostType;
	
	NetTestUnitList netTestUnitList;
	
	ClientConnector connector;
	IPacketSender sender;
	
	void Awake () 
	{
		connector = gameObject.AddComponent<ClientConnector>();
			
		sender = new PacketSender();
		sender.Connector = connector;
		
		Game.Instance.Connector = connector;
		Game.Instance.PacketSender = sender;
		
		netTestUnitList = new NetTestUnitList(sender);
		netTestUnitList.RegisterTest();
		
		switch(hostType)
		{
		case NetConfig.HostType.DevHost :  connector.HomeRoot = NetConfig.DevHostURL; break;
		case NetConfig.HostType.TestHost : connector.HomeRoot = NetConfig.TestHostURL; break;
		case NetConfig.HostType.RealHost : connector.HomeRoot = NetConfig.RealHostURL; break;
		}
		
		connector.messageDispatcher.netTestUnitList = netTestUnitList;
		
		TableManager.Instance.LoadTables();
		
	}
	
	void Start()
	{
		netTestUnitList.RunTestAll();
	}

	void Update()
	{
		
	}



}
