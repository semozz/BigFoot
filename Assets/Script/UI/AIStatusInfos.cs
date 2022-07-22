using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIStatusInfos : MonoBehaviour {
	public UISlider abilityMana = null;
	public UISlider abilityRage = null;
	public UISlider abilityVital = null;
	
	public UISlider hp = null;
	
	public BattleFace battleFace = null;
	
	public UILabel charLevel = null;
	
	public UILabel awakeningLevel = null;
	public GameObject awakeningLevelRoot = null;
	
	public UILabel hpInfoLabel = null;
	public UILabel abilityInfoLabel = null;

	public void SetAbilityType(AttributeValue.eAttributeType type)
	{
		List<UISlider> list = new List<UISlider>();
		list.Add(abilityMana);
		list.Add(abilityRage);
		list.Add(abilityVital);
		foreach(UISlider obj in list)
		{
			if (obj != null)
				obj.gameObject.SetActive(false);
		}
		
		switch(type)
		{
		case AttributeValue.eAttributeType.Mana:
			if (abilityMana != null)
				abilityMana.gameObject.SetActive(true);
			break;
		case AttributeValue.eAttributeType.Rage:
			if (abilityRage != null)
				abilityRage.gameObject.SetActive(true);
			break;
		case AttributeValue.eAttributeType.Vital:
			if (abilityVital != null)
				abilityVital.gameObject.SetActive(true);
			break;
		}
	}
	
	public void SetInfos(PlayerController player)
	{
		if (player == null)
			return;
		
		switch(player.classType)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			player.abilityUI = this.abilityRage;
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			player.abilityUI = this.abilityVital;
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			player.abilityUI = this.abilityMana;
			break;
		}
		
		player.abilityInfoLabel = this.abilityInfoLabel;
		SetAbilityType(player.abilityValueType);
			
		player.hpUI = this.hp;
		player.hpInfoLabel = this.hpInfoLabel;
		
		player.charLevelLabel = this.charLevel;
		
		LifeManager lifeManager = player.lifeManager;
		if (lifeManager != null)
		{
			lifeManager.charLevelLabel = player.charLevelLabel;
			if (lifeManager.charLevelLabel != null)
			{
				lifeManager.charLevelLabel.text = lifeManager.charLevel.ToString();
			}
			
			if (lifeManager.attributeManager != null)
			{
				lifeManager.attributeManager.abilityUI = player.abilityUI;
				lifeManager.attributeManager.hpUI = player.hpUI;
				
				lifeManager.attributeManager.abilityInfoLabel = this.abilityInfoLabel;
				lifeManager.attributeManager.hpInfoLabel = this.hpInfoLabel;
			}
			
			SetAwakeningInfo(lifeManager.awakeningLevelManager);
		}
		
		if (this.battleFace != null)
			this.battleFace.SetClass(player.classType);
	}
	
	public void SetAwakeningInfo(AwakeningLevelManager awakeningManager)
	{
		int level = 0;
		long exp = 0L;
		
		if (awakeningManager != null)
		{
			level = awakeningManager.curLevel;
			exp = awakeningManager.curExp;
		}
		
		if (awakeningLevelRoot != null)
			awakeningLevelRoot.SetActive(exp > 0);
		
		if (awakeningLevel != null)
			awakeningLevel.text = string.Format("{0}", level);
	}
}
