using UnityEngine;
using System.Collections;

public class DamageUI : BaseDamageUI {
	public UILabel damageLabel = null;
	public UILabel dumyLabel = null;
	
	//private bool isCriticalMode = false;
	
	public Color monsterCriticalColor = Color.red;
	public Color monsterNormalColor = Color.white;
	public Color characterCriticalColor = Color.yellow;
	public Color characterNormalColor = Color.magenta;
	
	public Color curseColor = Color.magenta;
	public Color poisonColor = Color.green;
	
	public Color monsterHealColor = Color.green;
	public Color characterHealColor = Color.blue;
	
	public Color goldColor = Color.yellow;
	
	private bool isCharacterDamage = false;
	public bool IsCharacterDamage
	{
		get { return isCharacterDamage; }
		set { isCharacterDamage = value; }
	}
	
	
	public Animation anim = null;
	public string criticalAnim = "";
	public string normalAnim = "";
	
	public void SetDamage(int damageValue, bool isCritical, bool isCharacterDamage, GameDef.eBuffType buffType)
	{
		switch(buffType)
		{
		case GameDef.eBuffType.BT_CURSE:
			SetCurseDamage(damageValue, isCharacterDamage);
			break;
		case GameDef.eBuffType.BT_POISION:
			SetPoisonDamage(damageValue, isCharacterDamage);
			break;
		case GameDef.eBuffType.BT_REGENHP:
		case GameDef.eBuffType.BT_RED_POTION:
		case GameDef.eBuffType.BT_YELLOW_POTION:
			SetHeal(damageValue, isCharacterDamage);
			break;
		default:
			SetDamage(damageValue, isCritical, isCharacterDamage);
			break;
		}
	}
	
	public void SetDamage(int damageValue, bool isCritical, bool isCharacterDamage)
	{
		//Debug.Log("Damage : " + damageValue);
		
		//this.isCriticalMode = isCritical;
		this.isCharacterDamage = isCharacterDamage;
		
		if (damageLabel != null)
		{
			string damageStr = string.Format("{0}", damageValue);
			damageLabel.text = damageStr;
			
			if (dumyLabel != null)
				dumyLabel.text = damageStr;
			
			Color selectedColor = Color.white;
			if (isCharacterDamage == true)
			{
				if (isCritical == true)
					selectedColor = this.characterCriticalColor;
				else
					selectedColor = this.characterNormalColor;
			}
			else
			{
				if (isCritical == true)
					selectedColor = this.monsterCriticalColor;
				else
					selectedColor = this.monsterNormalColor;
			}
			
			damageLabel.color = selectedColor;
		}
		
		if (anim != null)
		{
			string animName = normalAnim;
			if (isCritical == true)
				animName = criticalAnim;
			
			anim.Play(animName);
		}
	}
	
	public void SetCurseDamage(int damageValue, bool isCharacterDamage)
	{
		//this.isCriticalMode = false;
		this.isCharacterDamage = isCharacterDamage;
		
		if (damageLabel != null)
		{
			string damageStr = string.Format("{0}", damageValue);
			damageLabel.text = damageStr;
			
			if (dumyLabel != null)
				dumyLabel.text = damageStr;
			
			damageLabel.color = curseColor;
		}
		
		if (anim != null)
			anim.Play(normalAnim);
	}
	
	public void SetPoisonDamage(int damageValue, bool isCharacterDamage)
	{
		//this.isCriticalMode = false;
		this.isCharacterDamage = isCharacterDamage;
		
		if (damageLabel != null)
		{
			string damageStr = string.Format("{0}", damageValue);
			damageLabel.text = damageStr;
			
			if (dumyLabel != null)
				dumyLabel.text = damageStr;
			
			damageLabel.color = poisonColor;
		}
		
		if (anim != null)
			anim.Play(normalAnim);
	}
	
	public void SetHeal(int damageValue, bool isCharacterHeal)
	{
		//Debug.Log("Heal : " + damageValue);
		
		//isCriticalMode = true;
		this.isCharacterDamage = isCharacterHeal;
		
		if (damageLabel != null)
		{
			string damageStr = string.Format("{0}", damageValue);
			damageLabel.text = damageStr;
			
			if (dumyLabel != null)
				dumyLabel.text = damageStr;
			
			Color selectedColor = Color.white;
			if (isCharacterHeal == true)
				selectedColor = this.characterHealColor;
			else
				selectedColor = this.monsterHealColor;
			
			damageLabel.color = selectedColor;
		}
		
		if (anim != null)
			anim.Play(normalAnim);
	}
	
	public void SetGoldValue(int goldValue)
	{
		//isCriticalMode = true;
		this.isCharacterDamage = false;
		
		if (damageLabel != null)
		{
			string infoStr = string.Format("{0}G", goldValue);
			damageLabel.text = infoStr;
			if (dumyLabel != null)
				dumyLabel.tag = infoStr;
			
			damageLabel.color = goldColor;
		}
		
		if (anim != null)
			anim.Play(normalAnim);
	}
}
