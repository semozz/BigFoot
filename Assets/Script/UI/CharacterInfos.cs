using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterInfos : MonoBehaviour {

	public UILabel infoLabel = null;
	
	public List<AttributeValue.eAttributeType> valueTypes = new List<AttributeValue.eAttributeType>();
	public void SetCharInfo(PlayerController player)
	{
		string infoStr = "";
		
		if (player != null)
		{
			GameDef.ePlayerClass classType = GameDef.ePlayerClass.CLASS_NONE;
			if (player != null)
				classType = player.classType;
			
			List<AttributeValue.eAttributeType> addTypes = new List<AttributeValue.eAttributeType>();
			switch(classType)
			{
			case GameDef.ePlayerClass.CLASS_WARRIOR:
				addTypes.Add(AttributeValue.eAttributeType.RageMax);
				//addTypes.Add(AttributeValue.eAttributeType.RageRegen);
				break;
			case GameDef.ePlayerClass.CLASS_ASSASSIN:
				addTypes.Add(AttributeValue.eAttributeType.VitalMax);
				//addTypes.Add(AttributeValue.eAttributeType.VitalRegen);
				break;
			case GameDef.ePlayerClass.CLASS_WIZARD:
				addTypes.Add(AttributeValue.eAttributeType.ManaMax);
				addTypes.Add(AttributeValue.eAttributeType.ManaRegen);
				break;
			}
			
			List<AttributeValue.eAttributeType> infoTypes = new List<AttributeValue.eAttributeType>();
			infoTypes.AddRange(valueTypes);
			foreach(AttributeValue.eAttributeType type in addTypes)
				infoTypes.Add(type);
			
			AttributeManager attMgr = null;
			if (player.lifeManager != null)
				attMgr = player.lifeManager.attributeManager;
			
			if (attMgr != null)
			{
				float addValue = 0.0f;
				foreach(AttributeValue.eAttributeType type in infoTypes)
				{
					AttributeValue attValue = attMgr.GetAttribute(type);
					if (attValue != null)
					{
						if (classType == GameDef.ePlayerClass.CLASS_WARRIOR &&
							type == AttributeValue.eAttributeType.AttackDamage)
						{
							addValue = player.lifeManager.GetAddAttackDamageByEquipWeapon();
							infoStr += attValue.GetInfoStr(addValue) + "\n";
						}
						else if (classType == GameDef.ePlayerClass.CLASS_WIZARD &&
							type == AttributeValue.eAttributeType.HealthRegen)
						{
							float recoveryHPRate = player.lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.RegenHPWhenRecoverAbility);
							float manaRegen = player.lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.ManaRegen);
							addValue = manaRegen * recoveryHPRate;
							infoStr += attValue.GetInfoStr(addValue) + "\n";
						}
						else
							infoStr += attValue.GetInfoStr() + "\n";
					}
				}
			}
		}
		
		if (infoLabel != null)
			infoLabel.text = infoStr;
	}
	
	public void OnToggle(bool isActive)
	{
		this.gameObject.SetActive(isActive);
	}
	
	public void SetActivate()
	{
		OnToggle(true);
	}
	
	public void DeActivate()
	{
		OnToggle(false);
	}
}
