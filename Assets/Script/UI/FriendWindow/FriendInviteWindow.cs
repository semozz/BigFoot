using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendInviteWindow : MonoBehaviour {
	public GameObject parentWindow = null;
	public Transform popupNode = null;
	
	public UIInput nickInput = null;
	
	public UILabel todayInvitePrefixLabel = null;
	public UILabel todayInviteInfoLabel = null;
	public UILabel todayInvitePostfixLabel = null;
	public int todayInvitePrefixStringID = 282;
	public int todayInvitePosfixStringID = 283;
	
	
	public UILabel totalInviteCountInfoLabel = null;
	void Start()
	{
		GameUI.Instance.friendInviteWindow = this;
		rotationGrid.OnSetIndex += setList;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (todayInvitePrefixLabel != null)
				todayInvitePrefixLabel.text = stringTable.GetData(todayInvitePrefixStringID);
			
			if (todayInvitePostfixLabel != null)
				todayInvitePostfixLabel.text = stringTable.GetData(todayInvitePosfixStringID);
		}
	}
	
	void OnDestroy()
	{
		rotationGrid.OnSetIndex += setList;
		GameUI.Instance.friendInviteWindow = null;
	}
	
	public int requestCount = 0;
	
	public int minLimit = 2;
	public int maxLimit = 8;
	public void OnRequestInviteFriendByNick(GameObject obj)
	{
		if (requestCount > 0)
			return;
		
		string nickName = "";
		if (nickInput != null)
			nickName = nickInput.text;
		
		int nCount = nickName.Length;
		if (nCount < minLimit || nCount > maxLimit)
		{
			nickInput.text = "";
			return;
		}
		
		IPacketSender sender = Game.Instance.PacketSender;
		if (sender != null)
		{
			requestCount++;
			sender.RequestInviteFriendByNick(nickName);
		}
	}
	
	public void OnRequestInviteKatalk(GameObject obj)
	{
		if (requestCount > 0)
			return;

        IPacketSender sender = Game.Instance.PacketSender;
		
		if (sender != null)
		{
			sender.RequestInviteFriendByKatalk(sender.Connector.Nick);
		}
	}
	
	public string okPopupPrefab = "UI/FriendWindow/FriendInviteOKPopup";
	public string failPopupPrefab = "UI/FriendWindow/FriendInviteFailPopup";
	
	BaseConfirmPopup popupWindow = null;
	public void OnErrorMessage(NetErrorCode error)
	{
		requestCount = 0;
		
		string prefabPath = "";
		switch(error)
		{
		case NetErrorCode.OK:
			prefabPath = okPopupPrefab;
			break;
		default:
			prefabPath = failPopupPrefab;
			break;
		}
		ClosePopup();
		
		popupWindow = ResourceManager.CreatePrefab<BaseConfirmPopup>(prefabPath, popupNode, Vector3.zero);
		
		if (popupWindow != null)
		{
			popupWindow.okButtonMessage.target = this.gameObject;
			popupWindow.okButtonMessage.functionName = "OnClosePopup";
			
			if (error != NetErrorCode.OK)
				popupWindow.SetMessage((int)error);
		}
	}
	
	public void OnClosePopup(GameObject obj)
	{
		ClosePopup();
	}
	
	public void ClosePopup()
	{
		if (this.nickInput != null)
			this.nickInput.text = "";
		
		if (popupWindow != null)
		{
			DestroyObject(popupWindow.gameObject, 0.0f);
			popupWindow = null;
		}
	}
	
	public void OnEnable()
	{
#if UNITY_EDITOR
		TestMakeDummyKakakoFriendInfo();
#endif
		
		List<KakaoFriendInfo> infoList = null;
		if (Game.Instance != null)
			infoList = Game.Instance.kakaoFriendList;
		
		SetInfos(infoList);
	}
	
	private void TestMakeDummyKakakoFriendInfo()
	{
		Game gameInstance = Game.Instance;
		if (gameInstance != null)
		{
			gameInstance.kakaoFriendList.Clear();
			
			int nCount = 25;
			
			string user_id = "";
			string nickname = "";
			string friend_nickname = "";
			string profile_image_url = "";
			string message_blocked = "";
			string hashed_talk_user_id = "";
			string supported_device = "";
			
			for (int index = 0; index < nCount; ++index)
			{
				user_id = string.Format("-{0:000000000}", index);
				nickname = string.Format("nickname {0}", index);
				friend_nickname = string.Format("friend_nickname {0}", index);
				profile_image_url = Random.Range(0, 2)==0? "":"http://th-p.talk.kakao.co.kr/th/talkp/wkitJEbhnZ/27YCncPz7UdOHqscZdttQK/snd511_110x110_c.jpg";//string.Format("profile_image_url {0}", index);
				message_blocked = string.Format("{0}", Random.Range(0, 2) == 0 ? "false" : "true");
				hashed_talk_user_id = string.Format("hashed_talk_user_id {0}", index);
				supported_device = string.Format("{0}", Random.Range(0, 2) == 0 ? "false" : "true");
				
				KakaoFriendInfo friend_info = new KakaoFriendInfo(user_id, nickname, friend_nickname, profile_image_url, message_blocked, hashed_talk_user_id, supported_device);
				
				gameInstance.kakaoFriendList.Add(friend_info);
			}
		}
	}
	
	void OnDisable()
	{
		rotationGrid.Reset();
		if (this.nickInput != null)
			this.nickInput.text = "";
		
		foreach(FriendSimpleInfoPanel infoPanel in friendSimpleInfoPanels)
			DestroyObject(infoPanel.gameObject, 0.0f);
		friendSimpleInfoPanels.Clear();
	}
		
	public string infoPanelPrefabPath = "UI/FriendWindow/KaKaoFriendInviteInfoPanel";
	public UIGrid grid = null;
	public UIDraggablePanel dragablePanel = null;
	public UIPanel scrollPanel = null;
	public RotationUIGrid rotationGrid = null;
	private List<FriendSimpleInfoPanel> friendSimpleInfoPanels = new List<FriendSimpleInfoPanel>();
	private List<KakaoFriendInfo> infos = new List<KakaoFriendInfo>();
	private int listMax = 12;
	
	public void SetInfos(List<KakaoFriendInfo> _infos)
	{
		if (_infos == null)
			return;
		
		infos = _infos;
		
		if (this.parentWindow == null)
		{
			if (GameUI.Instance.friendWindow != null)
				this.parentWindow = GameUI.Instance.friendWindow.gameObject;
		}
		
		Vector3 vPos = Vector3.zero;
		for (int index = infos.Count-1; index >= 0; --index)
		{
			KakaoFriendInfo info = infos[index];
			
			//내 게임 친구가 아닌 경우만 추가
			if (info.user_id.StartsWith("-") == false)
			{
				infos.Remove(infos[index]);
				continue;
			}
			info.isInvited = Game.Instance.CheckAlreadyInvite(info.user_id);
		}
		rotationGrid.maxNum = infos.Count - listMax;
		
		for (int index = 0; index < Mathf.Min(listMax, infos.Count); ++index)
		{			
			FriendSimpleInfoPanel infoPanel = ResourceManager.CreatePrefab<FriendSimpleInfoPanel>(infoPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
				friendSimpleInfoPanels.Add(infoPanel);
		}
		
		for (int index = 0; index < Mathf.Min(listMax, infos.Count); ++index)
		{
			setIndex(index, index);
		}
		
		UpdateTodayInviteInfo();
		
		Invoke("RefreshPanels", 0.2f);
	}
	
	private void setList(int i, bool isInc)
	{
		if(isInc)
		{
			setIndex(i+listMax-1, 0);
			friendSimpleInfoPanels.Add(friendSimpleInfoPanels[0]);
			friendSimpleInfoPanels.RemoveAt(0);
			Vector3 pos = friendSimpleInfoPanels[listMax-1].transform.localPosition;
			friendSimpleInfoPanels[listMax-1].transform.localPosition = new Vector3(pos.x, pos.y-listMax*grid.cellHeight, pos.z);
		}
		else
		{
			setIndex(i, listMax-1);
			friendSimpleInfoPanels.Insert(0, friendSimpleInfoPanels[listMax-1]);
			friendSimpleInfoPanels.RemoveAt(listMax);
			Vector3 pos = friendSimpleInfoPanels[0].transform.localPosition;
			friendSimpleInfoPanels[0].transform.localPosition = new Vector3(pos.x, pos.y+listMax*grid.cellHeight, pos.z);
		}
	}
	
	private void setIndex(int i, int index)
	{
		FriendSimpleInfoPanel infoPanel = null;
		int nCount = friendSimpleInfoPanels.Count;
		if (index >= 0 && index < nCount)
			infoPanel = friendSimpleInfoPanels[index];
		
		KakaoFriendInfo kakaoInfo = null;
		nCount = infos.Count;
		if (i >= 0 && i < nCount)
			kakaoInfo = infos[i];
		
		if (infoPanel != null && kakaoInfo != null)
		{
			infoPanel.SetFriendInfo(kakaoInfo);
				
			SetButtonMessage(infoPanel.detailViewMessage, this.parentWindow, "OnTargetDetailWindow");
			SetButtonMessage(infoPanel.funcButtonMessage, this.parentWindow, "OnFunc");
		}
	}
	
	public void UpdateInviteInfo()
	{
		foreach(FriendSimpleInfoPanel infoPanel in friendSimpleInfoPanels)
		{
			infoPanel.UpdateInviteInfo();
		}
		
		UpdateTotalInviteInfo();
		UpdateTodayInviteInfo();
	}
	
	public void UpdateTotalInviteInfo()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		string postfixStr = "";
		if (stringTable != null)
			postfixStr = stringTable.GetData(todayInvitePosfixStringID);
		
		if (totalInviteCountInfoLabel != null)
			totalInviteCountInfoLabel.text = string.Format("{0}{1}", Game.Instance.totalInvites, postfixStr);
	}
	
	public void UpdateTodayInviteInfo()
	{
		int inviteCount = Mathf.Max(0, Game.limitTodayInvites - Game.Instance.todayInvites);
		if (friendSimpleInfoPanels == null || friendSimpleInfoPanels.Count == 0)
			inviteCount = 0;
		
		if (todayInviteInfoLabel != null)
			todayInviteInfoLabel.text = string.Format("{0}", inviteCount);
	}
	
	public void RefreshPanels()
	{
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
	}
	
	public void SetButtonMessage(UIButtonMessage buttonMsg, GameObject target, string funcName)
	{
		if (buttonMsg != null)
		{
			buttonMsg.target = target;
			buttonMsg.functionName = funcName;//"OnTargetDetailWindow";
		}
	}
}
