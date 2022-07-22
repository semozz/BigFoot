using UnityEngine;
using System.Collections;

public class SelectAvatarButton : MonoBehaviour {
	public SelectAvatarWindow selectWindow = null;
	
	public AvatarCam avatarCam = null;
	public GameDef.ePlayerClass playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
	
	public UILabel levelLabel = null;
	public UILabel nameLabel = null;
	
	public UICheckbox checkBox = null;
	
	public string defaultAnim = "stand_pose02";
	public string selectAnim = "stand_pose02_to_pose";
	public string seletLoopAnim = "stand_pose";
	public string unSelectAnim = "stand_pose_to_pose02";
	
	public float selectLoopAnimDelay = 1.5f;
	public float unSelectLoopAnimDelay = 1.5f;
	
	public void Start()
	{
		if (checkBox != null)
			checkBox.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnAvatarSelected);
		
		if (avatarCam != null)
		{
			avatarCam.ChangeAvatar(playerClass);
			
			if (avatarCam.avatar != null)
				avatarCam.avatar.SetAnim(defaultAnim);
		}
		
		
		SetBaseInfo();
	}
	
	private void SetBaseInfo()
	{
		long curExp = 0L;
		int level = 1;
		int charIndex = (int)playerClass;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		TableManager tableManager = TableManager.Instance;
		CharExpTable expTable = null;
		StringTable stringTable = null;
		
		if (tableManager != null)
		{
			expTable = tableManager.charExpTable;
			stringTable = tableManager.stringTable;
		}
		
		if (privateData != null && privateData.baseInfo != null)
			curExp = privateData.baseInfo.ExpValue;
			
		if (expTable != null)
			level = expTable.GetLevel(curExp);
			
		if (levelLabel != null)
			levelLabel.text = string.Format("Lv.{0}", level);
		
		int nameStringID = 1 + (int)playerClass;
		if (nameLabel != null)
			nameLabel.text = stringTable.GetData(nameStringID);
	}
	
	public void OnAvatarSelected(UICheckbox checkBox, bool bActive)
	{
		if (avatarCam != null)
		{
			string animName = defaultAnim;
			if (bActive == true)
			{
				animName = selectAnim;
				Invoke("DoLoopAnim", selectLoopAnimDelay);
				
				if (selectWindow != null)
					selectWindow.SetSelectedClass(this.playerClass);
			}
			else
			{
				animName = unSelectAnim;
				CancelInvoke();
			}
			
			if (avatarCam.avatar != null)
				avatarCam.avatar.SetAnim(animName);
		}
	}
	
	public void DoLoopAnim()
	{
		if (avatarCam != null)
		{
			if (avatarCam.avatar != null)
				avatarCam.avatar.SetAnim(seletLoopAnim);
		}
	}
	
}
