using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendWindow : PopupBaseWindow {
	public TownUI townUI = null;
	
	public TabButtonController tabButtonController = null;
	public UILabel maxInfo = null;
	public int limitMaxCount = 60;
	
	void Start()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.FIREND;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		if (stringValueTable != null)
		{
			limitMaxCount = stringValueTable.GetData("LimitFriends");
			limitInviteCount = stringValueTable.GetData("LimitInviteCount");
		}
		
		int curFriends = 0;
		if (Game.Instance != null && Game.Instance.myFriendList != null)
			curFriends = Game.Instance.myFriendList.Count;
		
		SetMaxInfo(curFriends);
		
		GameUI.Instance.friendWindow = this;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.friendWindow = null;
	}
	
	public BaseFriendListWindow GetTabWindow(BaseFriendListWindow.eFriendListType type)
	{
		BaseFriendListWindow listWindow = null;
		if (tabButtonController != null)
		{
			foreach(TabButtonInfo info in tabButtonController.tabInfos)
			{
				BaseFriendListWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<BaseFriendListWindow>() : null;
				if (tempWindow != null && tempWindow.listType == type)
				{
					listWindow = tempWindow;
					
					if (listWindow != null)
						listWindow.parentWindow = this.gameObject;
					
					break;
				}
			}
		}
		
		return listWindow;
	}
	
	public FriendInviteWindow GetInviteWindow()
	{
		FriendInviteWindow window = null;
		if (tabButtonController != null)
		{
			foreach(TabButtonInfo info in tabButtonController.tabInfos)
			{
				FriendInviteWindow tempWindow = info.viewWindow != null ? info.viewWindow.GetComponent<FriendInviteWindow>() : null;
				if (tempWindow != null)
				{
					window = tempWindow;
					break;
				}
			}
		}
		return window;
	}
	
	public void InitWindow()
	{
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
			sender.GetKakaoInfo();
	}
	
	public void OnTargetDetailWindow(GameObject obj)
	{
		FriendSimpleInfoPanel infoPanel = null;
		GameObject parentObj = obj != null ? obj.transform.parent.gameObject : null;
		
		if (parentObj != null)
			infoPanel = parentObj.GetComponent<FriendSimpleInfoPanel>();
		
		if (infoPanel != null)
		{
			long targetUserIndexID = -1;
			int targetCharIndex = -1;
            string platform = "kakao";
			FriendSimpleInfo simpleInfo = infoPanel.simpleInfo;
			if (simpleInfo != null)
			{
				targetUserIndexID = simpleInfo.UserID;
				targetCharIndex = simpleInfo.CharID;
                platform = simpleInfo.platform;
			}
			
			IPacketSender sender = Game.Instance.packetSender;
			if (sender != null && targetUserIndexID != -1 && targetCharIndex != -1)
			{
				if (TownUI.detailRequestCount > 0)
					return;
				
				TownUI.detailRequestCount++;
				TownUI.detailWindowRoot = this.popupNode;

                sender.SendRequestTargetEquipItem(targetUserIndexID, targetCharIndex, platform);
			}
		}
	}
	
	public int stringIDForNotSupportedDevice = 280;
	public int stringIDForOverInvited = 281;
	public int limitInviteCount = 30;
	public void OnFunc(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		FriendSimpleInfoPanel infoPanel = null;
		GameObject parentObj = obj != null ? obj.transform.parent.gameObject : null;
		
		if (parentObj != null)
			infoPanel = parentObj.GetComponent<FriendSimpleInfoPanel>();
		
		if (infoPanel != null)
		{
			BaseFriendListWindow friendListWindow = infoPanel.parentWindow;
			if (friendListWindow != null)
			{
				BaseFriendListWindow.eFriendListType listType = friendListWindow.listType;
				IPacketSender packetSender = Game.Instance.packetSender;
				if (packetSender != null)
				{
					packetSender.SendRequestFriendFunc(listType, infoPanel.simpleInfo);
					
					requestCount++;
				}
				
				infoPanel.DisableFuncButton(true);
				
				switch(listType)
				{
				case BaseFriendListWindow.eFriendListType.AcceptList:
				case BaseFriendListWindow.eFriendListType.InviteList:
				case BaseFriendListWindow.eFriendListType.FriendList:
					infoPanel.DoAction();
					break;
				}
			}
			else
			{
				KakaoFriendInfo kakaoInfo = infoPanel.kakaoInfo;
				if (kakaoInfo != null)
				{
					TableManager tableManager = TableManager.Instance;
					StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
					
					if (kakaoInfo.supported_device == "false")
					{
						string popupMsg = "Not supported device user!";
						if (stringTable != null)
							popupMsg = stringTable.GetData(stringIDForNotSupportedDevice);
						
						if (GameUI.Instance.MessageBox != null)
							GameUI.Instance.MessageBox.SetMessage(popupMsg);
					}
					else
					{
						//하루 초대 카운트 확인..
						if (Game.Instance.CheckInviteCount() == false)
						{
							string popupMsg = "Invite Count Over!";
							string formatStr = "";
							if (stringTable != null)
								formatStr = stringTable.GetData(stringIDForOverInvited);
							
							
							if (string.IsNullOrEmpty(formatStr) == false)
								popupMsg = string.Format(formatStr, limitInviteCount);
							
							if (GameUI.Instance.MessageBox != null)
								GameUI.Instance.MessageBox.SetMessage(popupMsg);
						}
						else
						{
							BaseConfirmPopup basePopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(kakaoInvitePopupPrefab, popupNode, Vector3.zero);
							if (basePopup != null)
							{
								//현재 선택된 panel을 저장해 놓는다.
								kakaoInfoPanel = infoPanel;
								
								basePopup.cancelButtonMessage.target = this.gameObject;
								basePopup.cancelButtonMessage.functionName = "OnCancelPopup";
								
								basePopup.okButtonMessage.target = this.gameObject;
								basePopup.okButtonMessage.functionName = "OnInviteOK";
								
								string formatStr = "Send Invite Message to {0}";
								if (stringTable != null)
									formatStr = stringTable.GetData(kakoInvitePopupMessageStringID);
								string popupMsg = string.Format(formatStr, kakaoInfo.nickname);
								basePopup.SetMessage(popupMsg);
								
								popupList.Add(basePopup);
							}
						}
					}
				}
			}
		}
	}
	
	protected FriendSimpleInfoPanel kakaoInfoPanel = null;
	public int kakoInvitePopupMessageStringID = 285;
	public string kakaoInvitePopupPrefab = "UI/FriendWindow/KakaoInviteConfirmPopup";
	public void OnInviteOK(GameObject obj)
	{
		KakaoFriendInfo info = kakaoInfoPanel != null ? kakaoInfoPanel.kakaoInfo : null;
		string user_id = "";
		if (info != null)
			user_id = info.user_id;
		
		/*
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null && info != null)
		{
			packetSender.SendRequsetInviteKakaoFriendByUserID(info.user_id);
			requestCount++;
			
			if (kakaoInfoPanel != null)
				kakaoInfoPanel.DisableFuncButton(true);
			
			OnCancelPopup(null);
		}
		*/
		
		if (string.IsNullOrEmpty(user_id) == true)
		{
			if (GameUI.Instance.MessageBox != null)
				GameUI.Instance.MessageBox.SetMessage("UserInfo is Invalid!!");
		}
		else
		{
			Logger.DebugLog("Kakao Invite.....");
	        if (Game.Instance.AndroidManager != null)
	            Game.Instance.AndroidManager.SendKakaoMessage(user_id);
		}
	}
	
	public void OnCancelPopup(GameObject obj)
	{
		kakaoInfoPanel = null;
		ClosePopup();
	}
	
	public void OnSendStaminaForAll(GameObject obj)
	{
		UIButton button = obj.GetComponent<UIButton>();
		
		if (requestCount > 0)
			return;
		
		IPacketSender packetSender = Game.Instance.packetSender;
		BaseFriendListWindow listWindow = GetTabWindow(BaseFriendListWindow.eFriendListType.FriendList);
		if (listWindow != null)
		{
			/*
			List<FriendSimpleInfoPanel> listInfos = listWindow.GetInfos();
			foreach(FriendSimpleInfoPanel infoPanel in listInfos)
			{
				if (infoPanel.simpleInfo != null && infoPanel.IsDisabled() == false)
				{
					if (packetSender != null)
					{
						packetSender.SendRequestSendStamina(infoPanel.simpleInfo.UserID, infoPanel.simpleInfo.platform, infoPanel.simpleInfo.nick);
					}
				}
			}
			*/
			
			if (packetSender != null)
			{
				packetSender.SendStaminaForAll();
				
				requestCount++;
			}
		}
	}
	
	public int requestCount = 0;
	public override void OnBack()
	{
		requestCount = 0;
		base.OnBack();
	}
	
	public void RemoveInviteList(long targetUserID)
	{
		BaseFriendListWindow baseFriendListWindow = GetTabWindow(BaseFriendListWindow.eFriendListType.InviteList);
		if (baseFriendListWindow != null)
			baseFriendListWindow.RemoveInfo(targetUserID);
	}
	
	public void RemoveFriendList(long targetUserID)
	{
		BaseFriendListWindow baseFriendListWindow = GetTabWindow(BaseFriendListWindow.eFriendListType.FriendList);
		if (baseFriendListWindow != null)
			baseFriendListWindow.RemoveInfo(targetUserID);
		
		Game.Instance.RemoveFriend(targetUserID);
		
		int curCount = 0;
		if (Game.Instance != null)
			curCount = Game.Instance.myFriendList.Count;
		
		SetMaxInfo(curCount);
	}
	
	public override void OnErrorMessage (NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		base.OnErrorMessage (errorCode, popupBase);
	}
	
	public void OnChildWindow(GameObject obj)
	{
		ClosePopup();
	}
	
	public void SetMaxInfo(int cur)
	{
		if (maxInfo != null)
			maxInfo.text = string.Format("{0}/{1}", cur, this.limitMaxCount);
	}
	
	public void UpdateFriendCount()
	{
		int curFriends = 0;
		if (Game.Instance != null && Game.Instance.myFriendList != null)
			curFriends = Game.Instance.myFriendList.Count;
		
		SetMaxInfo(curFriends);
	}
	
	public void UpdateKakaoInviteInfo()
	{
		FriendInviteWindow inviteWindow = GetInviteWindow();
		if (inviteWindow != null && inviteWindow.gameObject.activeInHierarchy == true)
			inviteWindow.UpdateInviteInfo();
	}
}
