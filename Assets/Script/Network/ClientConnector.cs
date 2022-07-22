using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Collections.Generic;

class URLPacket
{
	public string url;
	public string json;
}

public class ClientConnector : MonoBehaviour
{
	public bool IsNetwork = true;
	public string HomeRoot;
	public string NoticeRoot;
	
	public NetConfig.PublisherType publisher;
	public NetConfig.HostType hostType;
 
    public NetMessageDispatcher messageDispatcher;	
    public INetMessageProcess messageProcess;
	public Secure.Cryptography cryptography;
	
	Stack<URLPacket> SendBuffer;
	
	public int UserIndexID;
    public string PlatformUserId; // 새 서버에서 UserIndexId를 대체할 값
    public string Platform; // kakao 따위
    public long UniversalPurchaseId; // Hive5 Purchase
	public int charIndex;
	public string gcmRegID;
	public string token;
	public string Nick;
	public string tempAccountID;
	public string tempPass;
	public AccountType tempAccountType = AccountType.MonsterSide;

    // 현재 게임 중인 스테이지 정보
    public int StageIndex;
    public int StageType;

	public CharInfoData charInfo = null;
	
	protected bool timeoutCheck;
	protected float starttimeoutTime;
	URLPacket timeoutPacket;
	protected int retrycount; 
	void Awake()
	{
		//Game.Instance.packetSender = this;
        messageDispatcher = new NetMessageDispatcher(this);
        messageProcess = new NetMessageProcess();
		
		messageProcess.Connector = this;
        messageProcess.RegisterHandler(ref messageDispatcher);
		
		SendBuffer = new Stack<URLPacket>();
		
		UserIndexID = 0;
		charIndex = 0;
		gcmRegID = "";
		token = "";
		timeoutCheck = false;
		starttimeoutTime = 0;
		retrycount = 0;
		//charInfo = Game.Instance.charInfoData;
		
		DontDestroyOnLoad(this);
		
		if (!IsNetwork)
		{
			FakeServer.Instance.Initial(this);
		}
	}
	
	bool CheckTimeOut()
	{
		if (!timeoutCheck)
			return false;
		
		starttimeoutTime += Time.deltaTime;		
		
		if (starttimeoutTime >  25)
		{
			starttimeoutTime = 0.0f;
			return true;
		}
		
		return false;
	}
	
	void Update() 
    {
		if (SendBuffer == null)
			return;
		
		if (CheckTimeOut())
		{
			ErrorMessageInternal(NetErrorCode.WebTimeOut, timeoutPacket.url, timeoutPacket.json);
			return;
		}
		
		if (SendBuffer.Count <= 0)
			return;
		
		StartCoroutine(SendPacketInternal());
	}
	
    public void OnDestroy() 
    {
        messageDispatcher = null;
    }
	
	public void MessageProcess(string packet)
	{
		messageDispatcher.MessageProcess(packet);
	}
	
	public void SendPacketReal(string url, string json)
	{
		URLPacket data = new URLPacket();
		data.url = url;
		data.json = json;
		
		SendBuffer.Push(data);
	}
	
	void SendPacketFake(string json)
	{
		FakeServer.Instance.MessageProcess(json);
	}
	
	public void SetEncrypt(bool enable)
	{
		cryptography.enable = enable;
	}
	
	public void SetToken(string token)
	{
		this.token = token;
	}
	
	public void SendPreLogin()
	{
		if (!cryptography)
			return;
		
		cryptography.messageDispatcher = messageDispatcher;		
		cryptography.PreLogin();
	}
				
	public void SendPacket(string url, Header packet)
	{
		//packet.UserIndexID = this.UserIndexID;
		packet.uniqueID = SystemInfo.deviceUniqueIdentifier;
		packet.token = token;
			
		string json = LitJson.JsonMapper.ToJson(packet);
		
		if (!IsNetwork)
		{
			SendPacketFake(json);
		}
		else
		{	
			string msg = string.Format("Send {0} {1}", url, json);
			
			Logger.DebugLog(msg);

			StringBuilder sb = new StringBuilder();
			
			sb.Append(HomeRoot);
			sb.Append(url);
			
			SendPacketReal(sb.ToString(), json);
		}
	}
	
	public void SendServerCheckPacket(Header packet)
	{
		string json = LitJson.JsonMapper.ToJson(packet);
		
		if (!IsNetwork)
		{
			SendPacketFake(json);
			return;
		}
		Logger.DebugLog(json);
		StringBuilder sb = new StringBuilder();
		
		sb.Append(NoticeRoot);
		sb.Append("CheckServerState.php");
		
		SendPacketReal(sb.ToString(), json);
	}
	
	public void ErrorMessageInternal(NetErrorCode errorCode, string url, string packet)
	{
		switch(errorCode)
		{
		case NetErrorCode.WebTimeOut:
			if (GameUI.Instance.MessageBox != null)
				GameUI.Instance.MessageBox.SetMessage("Connection Error!!");
			break;
		case NetErrorCode.NotConnected:
			{
				++retrycount;
			
				if (retrycount > 5)
				{
					Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.NetworkError);
					return;
				}
			
				PacketRequestReconnect msg = new PacketRequestReconnect();
				
				msg.errorCode = errorCode;
				msg.packet = packet;
				msg.url = url;
				
				string text = LitJson.JsonMapper.ToJson(msg);
				
				messageDispatcher.MessageProcess(text);
				
				timeoutPacket.json =  "";
				timeoutPacket.url = "";
				timeoutCheck = false;
				starttimeoutTime = 0.0f;
			}
			break;
			
		case NetErrorCode.EncryptKeyNotFound:
			{
				Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.SessionExpired);
			}
			break;
		}
	}
		
	IEnumerator SendPacketInternal()
	{
        yield return new WaitForEndOfFrame();
		
		//Logger.DebugLog("Send:" + json);
		
		URLPacket packet;
		
		while(SendBuffer.Count > 0)
		{
			timeoutCheck = true;
			starttimeoutTime = 0;
			
			packet = SendBuffer.Pop();
			timeoutPacket = packet;
			
			string decrypt = "";
			if (!cryptography.AesEncrypt(packet.json, out decrypt))
			{
				ErrorMessageInternal(NetErrorCode.EncryptKeyNotFound, packet.url, packet.json);
			}
			
	        WWWForm form = new WWWForm();
	        
	        form.AddField ("Packet", decrypt);
	        
	        WWW request = new WWW(packet.url, form.data, cryptography.keepSession.SessionCookie);
			
	        yield return request;
			
			timeoutCheck = false;
	        
	        if (!string.IsNullOrEmpty(request.error))
	        {
	            Logger.DebugLog ("Error Request: " + request.error);
				Logger.DebugLog ("URL:" + packet.url);
				
				ErrorMessageInternal(NetErrorCode.NotConnected, packet.url, packet.json);
				GameUI.Instance.CancelWait();
	        }
			else
			{
				if (string.IsNullOrEmpty(request.text))
				{
					Logger.DebugLog ("Recv Empty");
					GameUI.Instance.CancelWait();
					continue;
				}
				
				if (cryptography.enable)
				{
					string[] data = request.text.Split('|');
	
					foreach(string text in data)
					{
						if (string.IsNullOrEmpty(text)) continue;
						
						string destext = "";
						if (!cryptography.AesDecrypt(text, out destext))
						{
							ErrorMessageInternal(NetErrorCode.EncryptKeyNotFound, packet.url, packet.json);			
						}
						
						messageDispatcher.MessageProcess(destext);
					}
				}
				else
				{
					messageDispatcher.MessageProcess(request.text);
				}
			}	
		}    
	
		
	}
	
}

