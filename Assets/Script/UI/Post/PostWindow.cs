using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PostWindow : PopupBaseWindow {
	public TownUI townUI = null;
	
	public UILabel titleLabel = null;
	public int titleStringID = -1;
	
	public PostListWindow postListWindow = null;
	
	public UILabel postCountInfoLabel = null;
	
	public StringTable stringTable = null;
	
	public UIButton takeAllButton = null;
	
	public StringValueTable stringValueTable = null;
	void Start()
	{
		this.windowType = TownUI.eTOWN_UI_TYPE.POST;
		
		TableManager tableManager = TableManager.Instance;
		stringTable = tableManager != null ? tableManager.stringTable : null;
		stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		if (titleLabel != null && stringTable != null && privateData != null && titleStringID != -1)
		{
			titleLabel.text = string.Format(stringTable.GetData(titleStringID), charData.NickName);
		}
		
		GameUI.Instance.postWindow = this;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.postWindow = null;
	}
	
	public void InitWindow(List<MailInfo> postInfos)
	{
		if (postListWindow != null)
		{
			postListWindow.parentWindow = this;
			
			SetPostCountLabel();
			postListWindow.SetPostInfos(postInfos);
		}
		
		int unReadCount = CheckUnReadPostCount();
		
		if (takeAllButton != null)
			takeAllButton.isEnabled = unReadCount > 0;
	}
	
	public void AddPostItems(List<MailInfo> postInfos)
	{
		if (postListWindow != null)
		{
			postListWindow.parentWindow = this;
			
			SetPostCountLabel();
			postListWindow.AddPostInfos(postInfos);
		}
		
		int unReadCount = CheckUnReadPostCount();
		
		if (takeAllButton != null)
			takeAllButton.isEnabled = unReadCount > 0;
	}
	
	public void SetPostCountLabel()
	{
		TableManager tableManager = TableManager.Instance;
		stringTable = tableManager != null ? tableManager.stringTable : null;
		stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		int maxCount = stringValueTable != null ? stringValueTable.GetData("MaxPost") : 50;
		if (postCountInfoLabel != null)
		{
			postCountInfoLabel.text = string.Format("{0}/{1}", Game.Instance.postItemCount, maxCount);
		}
	}
	
	public int requestCount = 0;
	public void OnTakeItem(MailInfo mailInfo)
	{
		if (requestCount > 0)
			return;
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestPostItem(mailInfo);
			
			requestCount++;
		}
	}
	
	public void OnViewMessage(MailInfo mailInfo)
	{
		if (requestCount > 0)
			return;
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestPostMessage(mailInfo);
			
			requestCount++;
		}
	}
	
	public void OnTakeAll()
	{
		if (requestCount > 0)
			return;
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestPostItemAll();

            //foreach (var mailInfo in postListWindow.lstPostInfos)
            //{
            //    packetSender.SendRequestPostItem(mailInfo);
            //}

			this.takeAllButton.isEnabled = false;
			
			requestCount++;
		}
	}
	
	public int CheckUnReadPostCount()
	{
		int nCount = 0;
		if (postListWindow != null)
			nCount = postListWindow.CheckUnReadPostCount();
		
		return nCount;
	}
	
	public void SetRead(string mailIndex)
	{
		if (postListWindow != null)
			postListWindow.SetReadMail(mailIndex);
		
		int nCount = CheckUnReadPostCount();
		if (this.takeAllButton != null)
			this.takeAllButton.isEnabled = (nCount > 0);
	}

    public void SetRead(string[] mailIndex)
    { }
	
	public void SetReadAll()
	{
        if (postListWindow != null)
        {
            postListWindow.SetReadMailAll();
            postListWindow.RefreshList();
        }

        if (this.takeAllButton != null)
            this.takeAllButton.isEnabled = false;
    }
	
	public string msgPrefabPath = "";
	public BaseConfirmPopup msgWindow = null;
	public void OnReadMessage(string title, string msg)
	{
		if (msgWindow == null)
			msgWindow = ResourceManager.CreatePrefab<BaseConfirmPopup>(msgPrefabPath, popupNode, Vector3.zero);
		
		if (msgWindow != null)
		{
			msgWindow.titleLabel.text = title;
			msgWindow.messageLabel.text = msg;
			
			msgWindow.okButtonMessage.target = this.gameObject;
			msgWindow.okButtonMessage.functionName = "OnReadDone";
		}
	}
	
	public void OnReadDone(GameObject obj)
	{
		if (msgWindow != null)
		{
			DestroyObject(msgWindow.gameObject, 0.0f);
			
			msgWindow = null;
		}
		
		requestCount = 0;
	}
	
	public override void OnBack()
	{
		requestCount = 0;
		base.OnBack();
		
		if (postListWindow != null)
			postListWindow.InitWindow();
		
		Game.Instance.RemoveReadPost();
	}
	
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		requestCount = 0;
		
		base.OnErrorMessage(errorCode, popupBase);
	}
	
	public void OnChildWindow(bool bFlag)
	{
		ClosePopup();
	}
	
	public void RequestMoreItems(string lastID)
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			packetSender.SendRequestPostInfo(lastID);
			
			requestCount++;
		}
	}
}
