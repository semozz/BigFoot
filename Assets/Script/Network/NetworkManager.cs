using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
	
	private ClientConnector connector;
	private Secure.Cryptography cryptography;
	private IPacketSender sender;
	
	public string loginStage = "Login_Test";
	public string townStage = "TownTest";
	
	void Awake () 
	{
	}
	
	public void SetHostAndPublisherType(NetConfig.HostType hostType, NetConfig.PublisherType publisherType)
	{
		NetConfig.curPublisher = publisherType;
		NetConfig.hostType = hostType;
		
		if (connector != null)
		{
			connector.publisher = publisherType;
			connector.hostType = hostType;
		
			switch(NetConfig.hostType)
			{
			case NetConfig.HostType.TestHost : 
				{
					connector.HomeRoot = NetConfig.TestHostURL; 
					connector.NoticeRoot = NetConfig.NoticeTestHostURL; 
					
					Logger.SetLevel(LogLevel.Debug);
				}
				break;
			case NetConfig.HostType.DevHost :  
				{
					connector.HomeRoot = NetConfig.DevHostURL; 
					connector.NoticeRoot = NetConfig.NoticeDevHostURL; 
	
					Logger.SetLevel(LogLevel.Debug);
					
				}
				break;
			case NetConfig.HostType.RealHost : 
				{
					connector.HomeRoot = NetConfig.RealHostURL; 
					connector.NoticeRoot = NetConfig.NoticeRealHostURL; 
				
					Logger.SetLevel(LogLevel.Error);
				}
				break;
			}
			
			if (cryptography != null)
				cryptography.SetRemotePhpScriptLocation(connector.HomeRoot);
		}
	}
	
	public void InitNetwork(bool Hive5)
	{
		if (connector == null)
			connector = gameObject.AddComponent<ClientConnector>();
		
		if (cryptography == null)
			cryptography = gameObject.AddComponent<Secure.Cryptography>();
		
		if (sender == null)
		{
            if (Hive5)
                sender = new Hive5PacketSender();
            else
                sender = new PacketSender();

			sender.Connector = connector;
		}
		
		cryptography.enable = false;
		
		connector.cryptography = cryptography;
		
		Game.Instance.Connector = connector;
		Game.Instance.PacketSender = sender;
	}
	
	void Start()
	{
		
	}
	
	void OnDestory()
	{
		
	}
}

