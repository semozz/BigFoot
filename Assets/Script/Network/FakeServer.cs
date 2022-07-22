using UnityEngine;
using System.Collections;

public class FakeServer : Singleton<FakeServer>
{
	NetMessageDispatcher dispatcher;
	FakeMessageProcess process;
	
	public ClientConnector _connector;
	
	public void Initial(ClientConnector connector)
	{
		_connector = connector;
		
		dispatcher = new NetMessageDispatcher(_connector);
		process = new FakeMessageProcess();
		
		process.RegisterHandler(_connector, ref dispatcher);
	}
	
	public void MessageProcess(string json)
	{
		dispatcher.MessageProcess(json);
	}
}
