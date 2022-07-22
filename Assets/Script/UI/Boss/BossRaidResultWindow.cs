using UnityEngine;
using System.Collections;

public class BossRaidResultWindow : PopupBaseWindow {
	public UILabel bossClearLabel = null;
	public UILabel lastAttackLabel = null;
	public UILabel topDamageLabel = null;
	public UILabel rewardInfoMsgLabel = null;
	
	public int bossClearStringID = -1;
	public int lastAttackStringID = -1;
	public int topDamageStringID = -1;
	public int rewardInfoMsgStringID = -1;
	
	
	public float minLifeTime = 2.0f;
	private System.DateTime createTime;
	
	void Start()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			SetLabelText(bossClearLabel, bossClearStringID, stringTable);
			SetLabelText(lastAttackLabel, lastAttackStringID, stringTable);
			SetLabelText(topDamageLabel, topDamageStringID, stringTable);
			SetLabelText(rewardInfoMsgLabel, rewardInfoMsgStringID, stringTable);
		}
	}
	
	public void SetLabelToggle(UILabel label, bool isActive)
	{
		if (label != null)
			label.gameObject.SetActive(isActive);
	}
	
	public void SetLabelText(UILabel label, int stringID, StringTable stringTable)
	{
		if (label != null && stringID != -1 && stringTable != null)
			label.text = stringTable.GetData(stringID);
	}
	
	public void InitWindow()
	{
		PacketBossRaidEnd bossRaidEnd = Game.Instance.bossRaidEnd;
		
		bool isTopDamage = false;
		NetErrorCode errorCode = NetErrorCode.OK;
		if (bossRaidEnd != null)
		{
			errorCode = bossRaidEnd.errorCode;
			
			isTopDamage = bossRaidEnd.bTop == 1;
		}
		
		if (errorCode == NetErrorCode.OK)
		{
			if (topDamageLabel != null)
				topDamageLabel.gameObject.SetActive(isTopDamage);
		}
		else
		{
			SetLabelToggle(bossClearLabel, false);
			SetLabelToggle(lastAttackLabel, false);
			SetLabelToggle(topDamageLabel, false);
			
			this.OnErrorMessage(errorCode, null);
		}
		
		createTime = System.DateTime.Now;
		
		//Invoke("OnOk", lifeTime);
	}
	
	public string failPopupPrefabPath = "UI/Boss/BossRaidFailPopup";
	public int bossAlreadyDeathMessageStringID = -1;
	public int bossRunAwayMessageStringID = -1;
	public override void OnErrorMessage(NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		switch(errorCode)
		{
		default:
			base.OnErrorMessage(errorCode, popupBase);
			break;
		case NetErrorCode.BossAlreadyDeath:
		case NetErrorCode.BossRunAway:
			BaseConfirmPopup basePopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(failPopupPrefabPath, popupNode, Vector3.zero);
			if (basePopup != null)
			{
				int messageID = -1;
				if (errorCode == NetErrorCode.BossAlreadyDeath)
					messageID = bossAlreadyDeathMessageStringID;
				else
					messageID = bossRunAwayMessageStringID;
				
				basePopup.okButtonMessage.target = this.gameObject;
				basePopup.okButtonMessage.functionName = "OnClosePopup";
				
				basePopup.SetMessage(messageID);
				
				this.popupList.Add(basePopup);
			}
			break;
		}
	}
	
	public void OnOk(GameObject obj)
	{
		System.TimeSpan timeSpan = System.DateTime.Now - createTime;
		if (timeSpan.TotalSeconds < (double)minLifeTime)
			return;
		
		OnOk();
	}
	
	public void OnOk()
	{
		DestroyObject(this.gameObject, 0.1f);
		
		Game.Instance.bossRaidEnd = null;
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
			townUI.OnEnterTown();
	}
}
