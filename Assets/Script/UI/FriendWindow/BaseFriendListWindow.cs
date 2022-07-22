using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseFriendListWindow : MonoBehaviour {
	public GameObject parentWindow = null;
	
	public Transform disappearTarget = null;
	
	public UIButtonMessage windowFunctionMessage = null;
	
	public enum eFriendListType
	{
		FriendList,
		InviteList,
		AcceptList,
	}
	public eFriendListType listType = eFriendListType.FriendList;
	
	public string infoPanelPrefabPath = "";
	public UIGrid grid = null;
	public UIDraggablePanel dragablePanel = null;
	public UIPanel scrollPanel = null;
	
	public void SetButtonMessage(UIButtonMessage buttonMsg, GameObject target, string funcName)
	{
		if (buttonMsg != null)
		{
			buttonMsg.target = target;
			buttonMsg.functionName = funcName;//"OnTargetDetailWindow";
		}
	}
	
	private List<FriendSimpleInfoPanel> friendSimpleInfoPanels = new List<FriendSimpleInfoPanel>();
	public List<FriendSimpleInfoPanel> GetInfos()
	{
		return friendSimpleInfoPanels;
	}
	
	public void SetInfos(FriendSimpleInfo[] infos)
	{
		if (infos == null)
			return;
		
		if (this.parentWindow == null)
		{
			if (GameUI.Instance.friendWindow != null)
				this.parentWindow = GameUI.Instance.friendWindow.gameObject;
		}
		
		if (windowFunctionMessage != null)
		{
			switch(this.listType)
			{
			case eFriendListType.FriendList:
				SetButtonMessage(windowFunctionMessage, this.parentWindow, "OnSendStaminaForAll");
				break;
			}
		}
		
		Vector3 vPos = Vector3.zero;
		int nCount = infos.Length;
		for (int index = 0; index < nCount; ++index)
		{
			FriendSimpleInfo info = infos[index];
			
			FriendSimpleInfoPanel infoPanel = ResourceManager.CreatePrefab<FriendSimpleInfoPanel>(infoPanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.parentWindow = this;
				
				infoPanel.SetFriendInfo(info);
				
				SetButtonMessage(infoPanel.detailViewMessage, this.parentWindow, "OnTargetDetailWindow");
				SetButtonMessage(infoPanel.funcButtonMessage, this.parentWindow, "OnFunc");
				
				friendSimpleInfoPanels.Add(infoPanel);
				
				//vPos.y += grid.cellHeight;
			}
		}
		
		switch(this.listType)
		{
		case eFriendListType.FriendList:
			if (this.grid != null)
				this.grid.sorted = true;
			
			Invoke("SortFriendsList", 0.1f);
			break;
		default:
			Invoke("RefreshPanels", 0.2f);
			break;
		}
	}
	
	public void SortFriendsList()
	{
		if (listType == eFriendListType.FriendList)
		{
			foreach(FriendSimpleInfoPanel infoPanel in friendSimpleInfoPanels)
			{
				string nickName = infoPanel.profileNameLabel.text;
				string sortName = "";
				
				if (infoPanel.IsDisabled() == true)
					sortName = string.Format("ZZZ_{0}_FirendInfo", nickName);
				else
					sortName = string.Format("AAA_{0}_FirendInfo", nickName);
				
				infoPanel.name = sortName;
			}
			
			if (this.parentWindow != null)
				this.parentWindow.SendMessage("UpdateFriendCount", SendMessageOptions.DontRequireReceiver);
			
			Invoke("RefreshPanels", 0.1f);
		}
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
	
	public void RemoveInfo(FriendSimpleInfoPanel infoPanel)
	{
		foreach(FriendSimpleInfoPanel temp in friendSimpleInfoPanels)
		{
			if (temp == infoPanel)
			{
				friendSimpleInfoPanels.Remove(temp);
				DestroyObject(temp.gameObject, 0.0f);
				break;
			}
		}
		
		this.Invoke("RefreshPanels", 0.1f);
	}
	
	public void RemoveInfo(long targetUserID)
	{
		foreach(FriendSimpleInfoPanel temp in friendSimpleInfoPanels)
		{
			if (temp == null)
				continue;
			
			if (temp.simpleInfo.UserID == targetUserID)
			{
				friendSimpleInfoPanels.Remove(temp);
				DestroyObject(temp.gameObject, 0.0f);
				break;
			}
		}
		
		this.Invoke("RefreshPanels", 0.1f);
	}
	
	public void UpdateInfo(long friendID, int coolTimeSec, string platform = "kakao")
	{
		if (this.listType != eFriendListType.FriendList)
			return;
		
		foreach(FriendSimpleInfoPanel infoPanel in friendSimpleInfoPanels)
		{
            if (infoPanel != null && infoPanel.simpleInfo.UserID == friendID/* && infoPanel.simpleInfo.platform == platform*/)
			{
				FriendInfoPanel tempPanel = (FriendInfoPanel)infoPanel;
				FriendInfo friendInfo = (FriendInfo)infoPanel.simpleInfo;
				
				if (friendInfo != null)
					friendInfo.coolTimeSec = coolTimeSec;
				
				tempPanel.SetFriendInfo(friendInfo);
			}
		}
		
		switch(this.listType)
		{
		case eFriendListType.FriendList:
			Invoke("SortFriendsList", 0.2f);
			break;
		}
	}
	
	public bool CheckSednStamina()
	{
		if (this.listType != eFriendListType.FriendList)
			return true;
		
		bool bCheck = true;
		foreach(FriendSimpleInfoPanel infoPanel in friendSimpleInfoPanels)
		{
            if (infoPanel != null)
			{
				FriendInfoPanel tempPanel = (FriendInfoPanel)infoPanel;
				
				if (tempPanel != null && tempPanel.IsDisabled() == false)
				{
					bCheck = false;
					break;
				}
			}
		}
		
		return bCheck;
	}
	
	public void OnEnable()
	{
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		int delayTime = 5;
		if (stringValueTable != null)
			delayTime = stringValueTable.GetData("FriendDelayTime");
		
		string msg = string.Format("{0} is Activate....", this.listType);
		Debug.Log(msg);
		
		bool isRequestable = false;
		System.TimeSpan deltaTime;
		List<FriendInfo> myFriendInfoList = null;
		List<FriendSimpleInfo> recommandInfoList = null;
		List<FriendSimpleInfo> acceptInfoList = null;
		
		switch(this.listType)
		{
		case eFriendListType.FriendList:	//친구 리스트.
			myFriendInfoList = Game.Instance.myFriendList;
			deltaTime = System.DateTime.Now - Game.Instance.myFriendUpdateTime;
			if (deltaTime.TotalMinutes > delayTime)
				isRequestable = true;
			break;
		case eFriendListType.InviteList:	//추천 리스트.
			recommandInfoList = Game.Instance.recommandFriendList;
			deltaTime = System.DateTime.Now - Game.Instance.recommandFriendUpdateTime;
			if (deltaTime.TotalMinutes > delayTime)
				isRequestable = true;
			break;
		case eFriendListType.AcceptList:	//친구 요청 받은 리스트.
			acceptInfoList = Game.Instance.acceptFriendList;
			/*
			deltaTime = System.DateTime.Now - Game.Instance.acceptFriendUpdateTime;
			if (deltaTime.TotalMinutes > delayTime)
				isRequestable = true;
			*/
			isRequestable = true;	//친구 요청 리스트는 항상 갱신..
			break;
		}
		
		if (isRequestable == true)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
				packetSender.SendRequestFriendList(this.listType);
		}
		else
		{
			switch(this.listType)
			{
			case eFriendListType.FriendList:
				SetInfos(myFriendInfoList.ToArray());
				break;
			case eFriendListType.InviteList:
				SetInfos(recommandInfoList.ToArray());
				break;
			case eFriendListType.AcceptList:
				SetInfos(acceptInfoList.ToArray());
				break;
			}
		}
	}
	
	public void OnDisable()
	{
		string msg = string.Format("{0} is UnActivate....", this.listType);
		Debug.Log(msg);
		
		foreach(FriendSimpleInfoPanel infoPanel in friendSimpleInfoPanels)
		{
			DestroyObject(infoPanel.gameObject, 0.0f);
		}
		friendSimpleInfoPanels.Clear();
	}
	
	public void OnTweenFinished()
	{
		List<FriendSimpleInfoPanel> deleteList = new List<FriendSimpleInfoPanel>();
		foreach(FriendSimpleInfoPanel infoPanel in friendSimpleInfoPanels)
		{
			if (infoPanel.name == "Deleting")
				deleteList.Add(infoPanel);
		}
		
		foreach(FriendSimpleInfoPanel delete in deleteList)
		{
			friendSimpleInfoPanels.Remove(delete);
			DestroyObject(delete.gameObject, 0.1f);
		}
		
		this.Invoke("RefreshPanels", 0.2f);
	}
}
