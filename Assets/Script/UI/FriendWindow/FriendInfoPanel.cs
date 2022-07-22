using UnityEngine;
using System.Collections;

public class FriendInfoPanel : FriendSimpleInfoPanel {
	
	public GameObject sendStaminaButton = null;
	public GameObject waitButton = null;
	
	public UILabel waitTimeLabel = null;
	public UILabel staminaLabel = null;
	
	public UILabel sendLabel = null;
	public int sendStringID = -1;
	
	public FriendInfo freindInfo = null;
	
	public GameObject completeObj = null;
	
	public override void Awake()
	{
		base.Awake();	
	}
	
	public override void Update()
	{
		base.Update();
		
		UpdateTime(System.DateTime.Now);
	}
	
	public System.DateTime nextTime = System.DateTime.Now;
	public override void SetFriendInfo(FriendSimpleInfo info)
	{
		base.SetFriendInfo(info);
		
		if (info != null &&
			info.GetType() == typeof(FriendInfo))
		{
			freindInfo = (FriendInfo)info;
		}
		else
			freindInfo = null;
		
		if (freindInfo != null)
		{
			int addTimeSec = 0;
			if (freindInfo.coolTimeSec > 0)
				addTimeSec = freindInfo.coolTimeSec;
				
			System.TimeSpan addTime = Game.ToTimeSpan(addTimeSec);
			System.DateTime nowTime = System.DateTime.Now;
			nextTime = nowTime + addTime;
			
			UpdateTime(nowTime);
			
			//string infoStr = string.Format("{0} is {1}", this.freindInfo.nick, isDisable);
			//Debug.Log(infoStr);
		}
	}
	
	bool isDisable = false;
	public void UpdateTime(System.DateTime nowTime)
	{
		bool bAvailableSendStamina = false;
		
		System.TimeSpan leftTime = nextTime - nowTime;
		if (leftTime.TotalSeconds <= 0)
		{
			bAvailableSendStamina = true;
		}
		else
		{
			string leftTimeStr = string.Format("{0:D2}:{1:D2}", leftTime.Hours, leftTime.Minutes);
			if (waitTimeLabel != null)
				waitTimeLabel.text = leftTimeStr;
		}
		
		SetActivateObj(sendStaminaButton, bAvailableSendStamina);
		SetActivateObj(waitButton, !bAvailableSendStamina);
		
		SetActivateObj(completeObj, !bAvailableSendStamina);
		
		isDisable = !bAvailableSendStamina;
	}
	
	public void SetActivateObj(GameObject obj, bool isActivate)
	{
		if (obj != null)
			obj.SetActive(isActivate);
	}
	
	public override void DisableFuncButton(bool bDisable)
	{
		SetActivateObj(sendStaminaButton, !bDisable);
		SetActivateObj(waitButton, bDisable);
	}
	
	public override bool IsDisabled()
	{
		return isDisable;
	}
	
	public override void DoAction ()
	{
		
	}
}
