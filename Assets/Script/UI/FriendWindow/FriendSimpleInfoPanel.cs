using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FriendSimpleInfoPanel : MonoBehaviour {
	public WebImageTexture profilePicture = null;
	public UILabel profileNameLabel = null;
	
	public UISprite characterPicture = null;
	public UILabel charLevelLabel = null;
	public UILabel connectInfoLabel = null;
	
	public UIButtonMessage detailViewMessage = null;
	public UIButtonMessage funcButtonMessage = null;
	public UILabel funcButtonLabel = null;
	public UIButton funcButton = null;
	public int funcButtonStringID = -1;
	
	public List<string> charFacePics = new List<string>();
	
	public int timePrefixStringID = -1;
	
	public int miniuteTimeInfoStringID = -1;
	public int hourTimeInfoStringID = -1;
	public int dayTimeInfoStringID = -1;
	public int monthTimeInfoStringID = -1;
	
	public BaseFriendListWindow parentWindow = null;
	
	public StringTable stringTable = null;
	public virtual void Awake()
	{
		TableManager tableManager = TableManager.Instance;
		stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null && funcButtonLabel != null && funcButtonStringID != -1)
		{
			funcButtonLabel.text = stringTable.GetData(funcButtonStringID);
		}
	}
	
	public string GetCharFaceName(int charIndex)
	{
		int nCount = charFacePics.Count;
		
		string facePicName = "";
		if (charIndex >= 0 && charIndex < nCount)
			facePicName = charFacePics[charIndex];
		
		return facePicName;
	}
	
	public FriendSimpleInfo simpleInfo = null;
	public virtual void SetFriendInfo(FriendSimpleInfo info)
	{
		simpleInfo = info;
		
		string profilePicURL = "";
		string profileName = "";
		string charLevel = "";
		string charFacePic = "";
		
		if (simpleInfo != null)
		{
			profileName = simpleInfo.nick;
			charLevel = string.Format("Lv. {0}", simpleInfo.Lv);
			charFacePic = GetCharFaceName(simpleInfo.CharID);
			
			//프로필 사진을 보여 주겠다고 되어 있는 친구만..
			if (simpleInfo.ShowProfileImage == true)
				profilePicURL = Game.Instance.GetKakaoProfileURL(simpleInfo.UserID.ToString());
		}
		
		if (profileNameLabel != null)
			profileNameLabel.text = profileName;
		if (charLevelLabel != null)
			charLevelLabel.text = charLevel;
		
		if (characterPicture != null)
			characterPicture.spriteName = charFacePic;
		if (profilePicture != null)
			profilePicture.SetURL(profilePicURL);
		
		UpdateConnectTimeInfo();
	}
	
	public KakaoFriendInfo kakaoInfo = null;
	public virtual void SetFriendInfo(KakaoFriendInfo info)
	{
		kakaoInfo = info;
		
		string profilePicURL = "";
		string profileName = "";
		string charLevel = "";
		string charFacePic = "";
		
		if (kakaoInfo != null)
		{
			profileName = kakaoInfo.nickname;
			profilePicURL = kakaoInfo.profile_image_url;
		}
		
		if (profileNameLabel != null)
			profileNameLabel.text = profileName;
		if (charLevelLabel != null)
			charLevelLabel.text = charLevel;
		
		if (characterPicture != null)
			characterPicture.spriteName = charFacePic;
		if (profilePicture != null)
			profilePicture.SetURL(profilePicURL);
		
		//UpdateConnectTimeInfo();
		if (kakaoInfo != null)
			DisableFuncButton(kakaoInfo.isInvited);
	}
	
	public virtual void Update()
	{
		UpdateConnectTimeInfo();
	}
	
	public void UpdateConnectTimeInfo()
	{
		string timeInfoStr = "";
		if (simpleInfo != null)
		{
			string timeFormatStr = "";
			
			System.TimeSpan timeSpan = System.DateTime.Now - simpleInfo.connTime;
			int totalDay = (int)timeSpan.TotalDays;
			if ( totalDay >= 30)
			{
				int month = (int)totalDay / 30;
				timeFormatStr = string.Format(stringTable.GetData(monthTimeInfoStringID), month);
			}
			else if (totalDay >= 1)
			{
				timeFormatStr = string.Format(stringTable.GetData(dayTimeInfoStringID), timeSpan.Days, timeSpan.Hours);
			}
			else if (timeSpan.TotalHours >= 1)
			{
				timeFormatStr = string.Format(stringTable.GetData(hourTimeInfoStringID), timeSpan.Hours, timeSpan.Minutes);
			}
			else
			{
				timeFormatStr = string.Format(stringTable.GetData(miniuteTimeInfoStringID), timeSpan.Minutes);
			}
			
			timeInfoStr = string.Format("{0} : {1}", stringTable.GetData(timePrefixStringID), timeFormatStr);
		}
		else
		{
			timeInfoStr = string.Format("{0} : --:--", stringTable.GetData(timePrefixStringID));
		}
		
		if (connectInfoLabel != null)
			connectInfoLabel.text = timeInfoStr;
	}
	
	public Collider funcButtonCollider = null;
	public virtual void DisableFuncButton(bool bDisable)
	{
		if (funcButtonCollider != null)
			funcButtonCollider.enabled = !bDisable;
		
		if (funcButton != null)
		{
			funcButton.gameObject.SetActive(false);
			funcButton.gameObject.SetActive(true);
			
			funcButton.isEnabled = !bDisable;
		}
	}
	
	public virtual void DoAction()
	{
		//this.transform.parent = null;
		Collider boxCollider = this.collider;
		if (boxCollider != null)
			boxCollider.enabled = false;
		
		if (this.parentWindow != null && this.parentWindow.disappearTarget != null)
		{
			TweenPosition tweenPosition = this.gameObject.AddComponent<TweenPosition>();
			if (tweenPosition != null)
			{
				tweenPosition.from = this.transform.localPosition;
				tweenPosition.to	= this.parentWindow.disappearTarget.localPosition;
				tweenPosition.style = UITweener.Style.Once;
				tweenPosition.steeperCurves = true;
				tweenPosition.method = UITweener.Method.EaseIn;
				
				tweenPosition.eventReceiver = this.parentWindow.gameObject;
				tweenPosition.callWhenFinished = "OnTweenFinished";
				
				tweenPosition.duration = 0.5f;
			}
			
			TweenScale tweenScale = this.gameObject.AddComponent<TweenScale>();
			if (tweenScale != null)
			{
				tweenScale.duration = 0.5f;
				tweenScale.to = Vector3.zero;
			}
		}
		
		this.gameObject.name = "Deleting";
	}
	
	public virtual bool IsDisabled()
	{
		bool isDisabled = false;
		
		if (funcButtonCollider != null)
			isDisabled = !funcButtonCollider.enabled;
		
		return isDisabled;
	}
	
	public void UpdateInviteInfo()
	{
		if (kakaoInfo == null)
			return;
		
		kakaoInfo.isInvited = Game.Instance.CheckAlreadyInvite(kakaoInfo.user_id);
		
		DisableFuncButton(kakaoInfo.isInvited);
	}
}
