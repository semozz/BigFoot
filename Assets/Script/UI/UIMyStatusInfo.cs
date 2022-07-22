using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIMyStatusInfo : MonoBehaviour {
	public UISlider abilityMana = null;
	public UISlider abilityRage = null;
	public UISlider abilityVital = null;
	
	public UISlider hp = null;
	
	public BattleFace battleFace = null;
	
	public NormalHP bossHP = null;
	public UILabel bossHPInfoLabel = null;
	
	public UILabel charLevel = null;
	
	public UILabel awakeningLevel = null;
	public GameObject awakeningLevelRoot = null;
	
	public Transform bossFaceNode = null;
	public GameObject bossFace = null;
	
	
	public UIButton goBackButton = null;
	
	public AIStatusInfos aiStatusInfos = null;
	
	public UILabel hpInfoLabel = null;
	public UILabel abilityInfoLabel = null;
	
	
	// Use this for initialization
	void Start () {
		GameUI.Instance.myStatusInfo = this;
		
		ShowBossHP(false);
		ShowAIInfos(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ShowBossHP(bool bShow)
	{
		if (bossHP != null)
			bossHP.gameObject.SetActive(bShow);
	}
	
	public void ShowAIInfos(bool bShow)
	{
		if (aiStatusInfos != null)
			aiStatusInfos.gameObject.SetActive(bShow);
	}
	
	public void SetAIInfos(PlayerController player)
	{
		if (player == null)
		{
			ShowAIInfos(false);
			return;
		}
		
		if (aiStatusInfos != null)
			aiStatusInfos.SetInfos(player);
	}
	
	public void DisableBossHP()
	{
		ShowBossHP(false);
		
		if (bossFace != null)
		{
			DestroyObject(bossFace, 0.0f);
			bossFace = null;
		}
	}
	
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
	
	public void SetAIAwakeningInfo(AwakeningLevelManager awakeningManager)
	{
		if (aiStatusInfos != null)
			aiStatusInfos.SetAwakeningInfo(awakeningManager);
	}
}
