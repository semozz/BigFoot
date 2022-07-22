using UnityEngine;
using System.Collections;

public class CharInfoButton : MonoBehaviour {
	public UILabel infoLabel = null;
	public UILabel infoLabel2 = null;
	public UIButton button = null;
	
	public int index = -1;
	
	public GameObject selectedObj = null;
	public bool isSelected = false;
	public bool IsSelected
	{
		set {
			isSelected = value;
			if (selectedObj != null)
				selectedObj.SetActive(isSelected);
			
		}
		
		get { return isSelected; }
	}
	
	public int warriorNameStringID = -1;
	public int assassinNameStringID = -1;
	public int wizardNameStringID = -1;
	
	public Transform dummyPos = null;
	
	public PlayerController player = null;
	public CharPrivateData targetPrivateData = null;
	public void SetPlayerInfo(int charIndex, TargetInfoAll targetInfo)
	{
		this.index = charIndex;
		
		if (targetPrivateData != null)
			targetPrivateData = null;
		
		if (button != null)
			button.isEnabled = (targetInfo != null);
		
		targetPrivateData = new CharPrivateData();
		targetPrivateData.InitEquipData();
		
		targetPrivateData.baseInfo.CharacterIndex = charIndex;
		targetPrivateData.baseInfo.ExpValue = targetInfo.Exp;
		targetPrivateData.NickName ="";
		
		int equipCount = targetInfo.equips.Length;
		targetPrivateData.SetEquipItemList(equipCount, targetInfo.equips, targetInfo.costumeSetItem);
		
		int nCount = 0;
		if (targetInfo.skills != null)
			nCount = Mathf.Min(targetInfo.skills.IDs.Length, targetInfo.skills.Lvs.Length);
		int skillID = 0;
		int skillLv = 0;
		for (int index = 0; index < nCount; ++index)
		{
			skillID = targetInfo.skills.IDs[index];
			skillLv = targetInfo.skills.Lvs[index];
			
			targetPrivateData.SetMasteryData(skillID, skillLv);
		}
		
		nCount = 0;
		if (targetInfo.awakenSkills != null)
			nCount = Mathf.Min(targetInfo.awakenSkills.IDs.Length, targetInfo.awakenSkills.Lvs.Length);
		skillID = 0;
		skillLv = 0;
		for (int index = 0; index < nCount; ++index)
		{
			skillID = targetInfo.awakenSkills.IDs[index];
			skillLv = targetInfo.awakenSkills.Lvs[index];
			
			targetPrivateData.SetAwakeningSkillData(skillID, skillLv);
		}
		
		if (player != null)
			DestroyObject(player.gameObject, 0.0f);
		
		player = Game.CreateDummyArenaCharacter(targetPrivateData, dummyPos);
		
		string charName = "";
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		GameDef.ePlayerClass classType = (GameDef.ePlayerClass)charIndex;
		int nameStringID = -1;
		switch(classType)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			nameStringID = warriorNameStringID;
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			nameStringID = assassinNameStringID;
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			nameStringID = wizardNameStringID;
			break;
		}
		
		charName = stringTable.GetData(nameStringID);
		int charLevel = 0;
		if (player != null && player.lifeManager != null)
			charLevel = player.lifeManager.charLevel;
		
		string infoStr = string.Format("{0} / Lv. {1}", charName, charLevel);
		if (infoLabel != null)
			infoLabel.text = infoStr;
		
		if (infoLabel2 != null)
			infoLabel2.text = infoStr;
	}
	
	void OnDestroy()
	{
		if (player != null)
		{
			DestroyObject(player.gameObject, 0.0f);
			player = null;
		}
	}
}
