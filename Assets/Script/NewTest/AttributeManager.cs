using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AttributeValue
{
	public enum eAttributeType
	{
		None = -1,
		AbilityPower,
		AttackDamage,
		CriticalHitRate,
		CriticalDamageRate,
		Health,
		HealthRegen,
		HealthMax,
		Armor,
		MagicResist,
		ArmorPenetration,
		MagicPenetration,
		Rage,
		RageMax,
		RageRegen,
		Vital,
		VitalMax,
		VitalRegen,
		Mana,
		ManaMax,
		ManaRegen,
		
		LifeSteal,
		
		Stamina,
		
		IncDamageWhenUnderHP50,
		IncDamageWhenOverHP50,
		IncDamageWhenHP100,
		IncAbilityGainRateByDamage,
		IncAbilityGainRateUnderHP35,
		IncAbilityGainRateUnderHP50,
		IncAttackDamageByAxe,
		IncAttackDamageByHammer,
		IncAttackDamageWhenStun,
		IncAttackDamageByArmor,
		IncAttackDamageUnderHP35,
		IncStunRateByHammer,
		IncGainExp,
		RegenHPWhenKnockdown,
		DecDamageWhenBerserk,
		IncDamageOnWeek2,
		IncGainGold,
		DecMoveSpeedWhenPoison,
		RecoverUseAbiliyValue,
		RegenHPWhenRecoverAbility,
		IncDamageOnSlow,
		IncReflectDamage,
		IncAbilityPowerWhenManaShield,
		IncDamageOnPoisionByAction,
		DecAttackDamageOnPoison,
		IncDamageOnLowMana,
		DecDamageOnBuff,
		IncPoisonInfectRate,
		DailyGainJewel,
		DailyGainAwakenPoint,
	}
	public eAttributeType valueType = eAttributeType.None;
	
	public enum eAttributeCalcType
	{
		CharacterAttribute,
		ItemAttribute,
		CostumeAttribute,
	}
	public eAttributeCalcType calcType = eAttributeCalcType.CharacterAttribute;
	public int itemGradeValue = 0;
	public int itemReinforceStep = 0;
	public float itemRateValue = 1.0f;
	public float itemGradeRateValue = 0.03f;
	
	public float baseValue = 0.0f;
	public float incValue = 0.0f;
	public float level = 1.0f;
	
	public float addValue = 0.0f;
	public float addRate = 0.0f;
	
	public float addValueByMastery = 0.0f;
	public float addRateByMastery = 0.0f;
	
	//투기장 아이템인 경우 능력치 제한을 위해.
	public float level_limitRate = 1.0f;
	
	public AttributeValue(eAttributeType type, float bValue)
	{
		valueType = type;
		
		this.baseValue = bValue;
		
		this.incValue = 0.0f;
		this.level = 0.0f;
		
		this.addValue = 0.0f;
		this.addRate = 0.0f;
		
		addValueByMastery = 0.0f;
		addRateByMastery = 0.0f;
	}
	
	public AttributeValue(eAttributeType type, float bValue, float incValue,  float level)
	{
		valueType = type;
		
		this.baseValue = bValue;
		
		this.incValue = incValue;
		this.level = level;
		
		//this.baseValue = this.incValue * (this.level - 1.0f);
		
		this.addValue = 0.0f;
		this.addRate = 0.0f;
		
		addValueByMastery = 0.0f;
		addRateByMastery = 0.0f;
	}
	
	public void SetLevel(float level)
	{
		this.level = level;
		
		//this.baseValue = this.initValue * this.level;
	}
	
	public void SetItemGrade(int gradeValue, int reinforceStep)
	{
		this.itemGradeValue = gradeValue;
		this.itemReinforceStep = reinforceStep;
	}
	
	public float Value
	{
		get
		{
			float result = 0.0f;
			
			float curLevel = 0;
			switch(calcType)
			{
			case eAttributeCalcType.CharacterAttribute:
				curLevel =level;
				break;
			case eAttributeCalcType.ItemAttribute:
			case eAttributeCalcType.CostumeAttribute:
				curLevel = (float)itemReinforceStep;
				break;
			}
			
			result = GetLevelValue(curLevel);
			
			return result;
		}
	}
	
	public float ValueForUI
	{
		get
		{
			float result = 0.0f;
			
			float curLevel = 0;
			switch(calcType)
			{
			case eAttributeCalcType.CharacterAttribute:
				curLevel =level;
				break;
			case eAttributeCalcType.ItemAttribute:
			case eAttributeCalcType.CostumeAttribute:
				curLevel = (float)itemReinforceStep;
				break;
			}
			
			result = GetLevelValue(curLevel);
			
			return result;
		}
	}
	
	public float GetLevelValue(float level)
	{
		float result = 0.0f;
		
		switch(calcType)
		{
		case eAttributeCalcType.CharacterAttribute:
			result = baseValue;
			//level 증가 적용.
			result += CalcAddValue(incValue * Mathf.Max(0.0f, level));
			//addValue추가
			result += CalcAddValue(addValue + addValueByMastery);
			
			//비율 증가
			float addingValue = CalcAddValue(result * addRate + addRateByMastery);
			
			result += addingValue;
			break;
		case eAttributeCalcType.ItemAttribute:
		case eAttributeCalcType.CostumeAttribute:
			/*
			result = baseValue;
			float rateValue = (1.0f - itemRateValue * ((level * incRateValue) + 1.0f));
			float tempValue = CalcAddValue(baseValue * rateValue);
			result -= tempValue;
			*/
			
			/*
			float startValue = baseValue + CalcAddValue(baseValue * (this.level_limitRate - 1.0f));
			float rateValue = (1.0f - itemRateValue * ((level * itemGradeRateValue) + 1.0f));
			float tempValue = CalcAddValue(startValue * rateValue);
			result = startValue - tempValue;
			*/
			
			float startValue = CalcAddValue(baseValue * itemRateValue * level_limitRate);
			float addRateValue = level * itemGradeRateValue;
			float tempAdd = CalcAddValue(startValue * addRateValue);
			result = startValue + tempAdd;
			break;
		}
		
		return result;
	}
	
	public float CalcAddValue(float addValue)
	{
		float resultValue = addValue;
		
		if (resultValue != 0.0f && Mathf.Abs(resultValue) < 1.0f)
		{
			string floatStr = string.Format("{0:F4}", addValue);
			resultValue = float.Parse(floatStr);
		}
		else
		{
			resultValue = (float)((int)addValue);
		}
		
		return resultValue;
	}
	
	public Vector2 GetMinMaxValue(float minRate, float maxRate)
	{
		Vector2 vResult = Vector2.zero;
		
		float result = 0.0f;
		switch(calcType)
		{
		case eAttributeCalcType.CharacterAttribute:
			result = baseValue;
			//level 증가 적용.
			result += CalcAddValue(incValue * Mathf.Max(0.0f, level));
			//addValue추가
			result += CalcAddValue(addValue + addValueByMastery);
			
			//비율 증가
			float addingValue = CalcAddValue(result * addRate + addRateByMastery);
			
			vResult.x = vResult.y = result + addingValue;
			break;
		case eAttributeCalcType.ItemAttribute:
			/*
			result = baseValue * ((level * incRateValue) + 1.0f);
			
			//result *= itemRateValue;
			vResult.x = CalcAddValue(result * minRate);
			vResult.y = CalcAddValue(result * maxRate);
			*/
			
			/*
			float startValue = baseValue + CalcAddValue(baseValue * (this.level_limitRate - 1.0f));
			result = startValue * ((level * itemGradeRateValue) + 1.0f);
			vResult.x = CalcAddValue(result * minRate);
			vResult.y = CalcAddValue(result * maxRate);
			*/
			
			float startValue = CalcAddValue(baseValue * itemRateValue * level_limitRate);
			float addRateValue = level * itemGradeRateValue;
			float tempAdd = CalcAddValue(startValue * addRateValue);
			result = startValue + tempAdd;
			
			vResult.x = CalcAddValue(result * minRate);
			vResult.y = CalcAddValue(result * maxRate);
			break;
		}
		
		return vResult;
	}
	
	public float GetNextLevelIncValue(int nextLevel)
	{
		float curValue = Value;
		
		float nextValue = curValue;
		
		if (nextLevel != -1)
		{
			nextValue = GetLevelValue(nextLevel);
		}
		
		return nextValue - curValue;
	}
	
	public void DebugInfo()
	{
		//Debug.Log("Attribute Type : " + valueType);
		float result = baseValue;
		float incAddValue = incValue * Mathf.Max(0.0f, (level - 1.0f));
		
		result += incAddValue;
		
		//float addingValue = result * addRate;
		
		//Debug.Log("Value : " + Value + " = baseValue(" + baseValue + ") + " + " addValue(" + addValue + ") + " + " incValue(" +  incAddValue + ") + " + "addRate(" + addingValue + ")");
	}
	
	public string GetInfoStr(float addValue)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable strTable = null;
		if (tableManager != null)
			strTable = tableManager.stringTable;
		
		float resultValue = ValueForUI + addValue;
		
		string msg = "";
		
		if (this.calcType == eAttributeCalcType.CostumeAttribute)
		{
			switch(valueType)
			{
			case eAttributeType.AbilityPower:
				msg = MakeInfoPercentString(strTable.GetData(102), resultValue); //"Magic : "
				break;
			case eAttributeType.AttackDamage:
				msg = MakeInfoPercentString(strTable.GetData(101), resultValue); //AttackDamage
				break;
			case eAttributeType.CriticalHitRate:
				resultValue = Mathf.Clamp(resultValue, 0.0f, 1.0f);
				msg = MakeInfoPercentString(strTable.GetData(103), resultValue); //"CriticalRate : " + Value;
				break;
			case eAttributeType.CriticalDamageRate:
				msg = MakeInfoPercentString(strTable.GetData(104), resultValue); //"CriticalDamage : " + Value;
				break;
			case eAttributeType.Health:
				msg = MakeInfoPercentString(strTable.GetData(105), resultValue); //"Health : " + Value;
				break;
			case eAttributeType.HealthRegen:
				msg = MakeInfoPercentString(strTable.GetData(106), resultValue); //"HealthRegen : " + Value;
				break;
			case eAttributeType.HealthMax:
				msg = MakeInfoPercentString(strTable.GetData(105), resultValue); //"MaxHealth : " + Value;
				break;
			case eAttributeType.Armor:
				msg = MakeInfoPercentString(strTable.GetData(113), resultValue); //"Armor : " + Value;
				break;
			case eAttributeType.MagicResist:
				msg = MakeInfoPercentString(strTable.GetData(114), resultValue); //"MagicResist : " + Value;
				break;
			case eAttributeType.ArmorPenetration:
				msg = MakeInfoPercentString(strTable.GetData(115), resultValue); //"ArmorPenetration : " + Value;
				break;
			case eAttributeType.MagicPenetration:
				msg = MakeInfoPercentString(strTable.GetData(116), resultValue); //"MagicPenetration : " + Value;
				break;
			case eAttributeType.Rage:
				msg = MakeInfoPercentString(strTable.GetData(107), resultValue); //"Rage : " + Value;
				break;
			case eAttributeType.RageMax:
				msg = MakeInfoPercentString(strTable.GetData(107), resultValue); //"MaxRage : " + Value;
				break;
			case eAttributeType.RageRegen:
				msg = MakeInfoPercentString(strTable.GetData(108), resultValue); //"RageRegen : " + Value;
				break;
			case eAttributeType.Vital:
				msg = MakeInfoPercentString(strTable.GetData(109), resultValue); //"Vital : " + Value;
				break;
			case eAttributeType.VitalMax:
				msg = MakeInfoPercentString(strTable.GetData(109), resultValue); //"MaxVital : " + Value;
				break;
			case eAttributeType.VitalRegen:
				msg = MakeInfoPercentString(strTable.GetData(110), resultValue); //"VitalRegen : " + Value;
				break;
			case eAttributeType.Mana:
				msg = MakeInfoPercentString(strTable.GetData(111), resultValue); //"Mana : " + Value;
				break;
			case eAttributeType.ManaMax:
				msg = MakeInfoPercentString(strTable.GetData(111), resultValue); //"MaxMana : " + Value;
				break;
			case eAttributeType.ManaRegen:
				msg = MakeInfoPercentString(strTable.GetData(112), resultValue); //"ManaRegen : " + Value;
				break;
			case eAttributeType.LifeSteal:
				msg = MakeInfoPercentString(strTable.GetData(118) , resultValue);
				break;
			case eAttributeType.IncGainGold:
				msg = MakeInfoPercentString(strTable.GetData(235), resultValue);
				break;
			}
		}
		else
		{
			switch(valueType)
			{
			case eAttributeType.AbilityPower:
				msg = MakeInfoString(strTable.GetData(102), resultValue); //"Magic : "
				break;
			case eAttributeType.AttackDamage:
				msg = MakeInfoString(strTable.GetData(101), (int)(resultValue)); //AttackDamage
				break;
			case eAttributeType.CriticalHitRate:
				resultValue = Mathf.Clamp(resultValue, 0.0f, 1.0f);
				msg = MakeInfoPercentString(strTable.GetData(103), resultValue); //"CriticalRate : " + Value;
				break;
			case eAttributeType.CriticalDamageRate:
				msg = MakeInfoPercentString(strTable.GetData(104), resultValue); //"CriticalDamage : " + Value;
				break;
			case eAttributeType.Health:
				msg = MakeInfoString(strTable.GetData(105), resultValue); //"Health : " + Value;
				break;
			case eAttributeType.HealthRegen:
				msg = MakeInfoString(strTable.GetData(106), resultValue); //"HealthRegen : " + Value;
				break;
			case eAttributeType.HealthMax:
				msg = MakeInfoString(strTable.GetData(105), resultValue); //"MaxHealth : " + Value;
				break;
			case eAttributeType.Armor:
				msg = MakeInfoString(strTable.GetData(113), resultValue); //"Armor : " + Value;
				break;
			case eAttributeType.MagicResist:
				msg = MakeInfoString(strTable.GetData(114), resultValue); //"MagicResist : " + Value;
				break;
			case eAttributeType.ArmorPenetration:
				msg = MakeInfoString(strTable.GetData(115), resultValue); //"ArmorPenetration : " + Value;
				break;
			case eAttributeType.MagicPenetration:
				msg = MakeInfoString(strTable.GetData(116), resultValue); //"MagicPenetration : " + Value;
				break;
			case eAttributeType.Rage:
				msg = MakeInfoString(strTable.GetData(107), resultValue); //"Rage : " + Value;
				break;
			case eAttributeType.RageMax:
				msg = MakeInfoString(strTable.GetData(107), resultValue); //"MaxRage : " + Value;
				break;
			case eAttributeType.RageRegen:
				msg = MakeInfoString(strTable.GetData(108), resultValue); //"RageRegen : " + Value;
				break;
			case eAttributeType.Vital:
				msg = MakeInfoString(strTable.GetData(109), resultValue); //"Vital : " + Value;
				break;
			case eAttributeType.VitalMax:
				msg = MakeInfoString(strTable.GetData(109), resultValue); //"MaxVital : " + Value;
				break;
			case eAttributeType.VitalRegen:
				msg = MakeInfoString(strTable.GetData(110), resultValue); //"VitalRegen : " + Value;
				break;
			case eAttributeType.Mana:
				msg = MakeInfoString(strTable.GetData(111), resultValue); //"Mana : " + Value;
				break;
			case eAttributeType.ManaMax:
				msg = MakeInfoString(strTable.GetData(111), resultValue); //"MaxMana : " + Value;
				break;
			case eAttributeType.ManaRegen:
				msg = MakeInfoString(strTable.GetData(112), resultValue); //"ManaRegen : " + Value;
				break;
			case eAttributeType.LifeSteal:
				msg = MakeInfoPercentString(strTable.GetData(118) , resultValue);
				break;
			case eAttributeType.IncGainGold:
				msg = MakeInfoPercentString(strTable.GetData(235), resultValue);
				break;
			}
		}
		
		return msg;
	}
	
	public string GetInfoStr()
	{
		return GetInfoStr(0.0f);
	}
	
	public string GetInfoStrTempLevel(int tempLevel)
	{
		int curLevel = (int)this.level;
		string infoStr = "";
		SetLevel((float)tempLevel);
		
		infoStr = GetInfoStrByTitle("");
		
		SetLevel((float)curLevel);
		
		return infoStr;
	}
	
	public string GetInfoStrByTitle(string titleString)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable strTable = null;
		if (tableManager != null)
			strTable = tableManager.stringTable;
		
		float resultValue = ValueForUI + addValue;
		
		string msg = "";
		
		if (this.calcType == eAttributeCalcType.CostumeAttribute)
		{
			switch(valueType)
			{
			case eAttributeType.AbilityPower:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"Magic : "
				break;
			case eAttributeType.AttackDamage:
				msg = MakeInfoPercentString(titleString, "", resultValue); //AttackDamage
				break;
			case eAttributeType.CriticalHitRate:
				resultValue = Mathf.Clamp(resultValue, 0.0f, 1.0f);
				msg = MakeInfoPercentString(titleString, "", resultValue); //"CriticalRate : " + Value;
				break;
			case eAttributeType.CriticalDamageRate:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"CriticalDamage : " + Value;
				break;
			case eAttributeType.Health:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"Health : " + Value;
				break;
			case eAttributeType.HealthRegen:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"HealthRegen : " + Value;
				break;
			case eAttributeType.HealthMax:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"MaxHealth : " + Value;
				break;
			case eAttributeType.Armor:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"Armor : " + Value;
				break;
			case eAttributeType.MagicResist:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"MagicResist : " + Value;
				break;
			case eAttributeType.ArmorPenetration:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"ArmorPenetration : " + Value;
				break;
			case eAttributeType.MagicPenetration:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"MagicPenetration : " + Value;
				break;
			case eAttributeType.Rage:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"Rage : " + Value;
				break;
			case eAttributeType.RageMax:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"MaxRage : " + Value;
				break;
			case eAttributeType.RageRegen:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"RageRegen : " + Value;
				break;
			case eAttributeType.Vital:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"Vital : " + Value;
				break;
			case eAttributeType.VitalMax:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"MaxVital : " + Value;
				break;
			case eAttributeType.VitalRegen:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"VitalRegen : " + Value;
				break;
			case eAttributeType.Mana:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"Mana : " + Value;
				break;
			case eAttributeType.ManaMax:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"MaxMana : " + Value;
				break;
			case eAttributeType.ManaRegen:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"ManaRegen : " + Value;
				break;
			case eAttributeType.LifeSteal:
				msg = MakeInfoPercentString(titleString, "", resultValue);
				break;
			case eAttributeType.IncGainGold:
				msg = MakeInfoPercentString(titleString, "", resultValue);
				break;
			}
		}
		else
		{
			switch(valueType)
			{
			case eAttributeType.AbilityPower:
				msg = MakeInfoString(titleString, "", resultValue); //"Magic : "
				break;
			case eAttributeType.AttackDamage:
				msg = MakeInfoString(titleString, "", resultValue); //AttackDamage
				break;
			case eAttributeType.CriticalHitRate:
				resultValue = Mathf.Clamp(resultValue, 0.0f, 1.0f);
				msg = MakeInfoPercentString(titleString, "", resultValue); //"CriticalRate : " + Value;
				break;
			case eAttributeType.CriticalDamageRate:
				msg = MakeInfoPercentString(titleString, "", resultValue); //"CriticalDamage : " + Value;
				break;
			case eAttributeType.Health:
				msg = MakeInfoString(titleString, "", resultValue); //"Health : " + Value;
				break;
			case eAttributeType.HealthRegen:
				msg = MakeInfoString(titleString, "", resultValue); //"HealthRegen : " + Value;
				break;
			case eAttributeType.HealthMax:
				msg = MakeInfoString(titleString, "", resultValue); //"MaxHealth : " + Value;
				break;
			case eAttributeType.Armor:
				msg = MakeInfoString(titleString, "", resultValue); //"Armor : " + Value;
				break;
			case eAttributeType.MagicResist:
				msg = MakeInfoString(titleString, "", resultValue); //"MagicResist : " + Value;
				break;
			case eAttributeType.ArmorPenetration:
				msg = MakeInfoString(titleString, "", resultValue); //"ArmorPenetration : " + Value;
				break;
			case eAttributeType.MagicPenetration:
				msg = MakeInfoString(titleString, "", resultValue); //"MagicPenetration : " + Value;
				break;
			case eAttributeType.Rage:
				msg = MakeInfoString(titleString, "", resultValue); //"Rage : " + Value;
				break;
			case eAttributeType.RageMax:
				msg = MakeInfoString(titleString, "", resultValue); //"MaxRage : " + Value;
				break;
			case eAttributeType.RageRegen:
				msg = MakeInfoString(titleString, "", resultValue); //"RageRegen : " + Value;
				break;
			case eAttributeType.Vital:
				msg = MakeInfoString(titleString, "", resultValue); //"Vital : " + Value;
				break;
			case eAttributeType.VitalMax:
				msg = MakeInfoString(titleString, "", resultValue); //"MaxVital : " + Value;
				break;
			case eAttributeType.VitalRegen:
				msg = MakeInfoString(titleString, "", resultValue); //"VitalRegen : " + Value;
				break;
			case eAttributeType.Mana:
				msg = MakeInfoString(titleString, "", resultValue); //"Mana : " + Value;
				break;
			case eAttributeType.ManaMax:
				msg = MakeInfoString(titleString, "", resultValue); //"MaxMana : " + Value;
				break;
			case eAttributeType.ManaRegen:
				msg = MakeInfoString(titleString, "", resultValue); //"ManaRegen : " + Value;
				break;
			case eAttributeType.LifeSteal:
				msg = MakeInfoPercentString(titleString, "", resultValue);
				break;
			case eAttributeType.IncGainGold:
				msg = MakeInfoPercentString(titleString, "", resultValue);
				break;
			}
		}
		
		return msg;
	}

    public string MakeInfoString(string title, int value1)
    {
        return MakeInfoString(title, " : ", value1);
    }
	
	public string MakeInfoString(string title, float value1)
	{
		return MakeInfoString(title, " : ", value1);
	}
	
	public string MakeInfoPercentString(string title, float value1)
	{
		return MakeInfoPercentString(title, " : ", value1);
	}
	
	public string MakeInfoString(string title, string seperator, float value1)
	{
		if (seperator == "" && title == "")
		{
			if (value1 > 0.0f)
				title = "+ ";
		}
		
		string msg = string.Format("{0}{1}{2:#,###,###,##0.##}", title, seperator, value1);
		return msg;
	}
	
	public string MakeInfoPercentString(string title, string seperator, float value1)
	{
		if (seperator == "" && title == "")
		{
			if (value1 > 0.0f)
				title = "+ ";
		}
		
		value1 = value1 * 100.0f;
		string msg = string.Format("{0}{1}{2:#,###,###,##0.##}%", title, seperator, value1);
		
		return msg;
	}
	
	public string MakeInfoString(string title, float value1, float value2, Color plusColor, Color minusColor)
	{
		string msg = string.Format("{0} : {1:#0.##}", title, value1);
		
		if (value2 > 0.0f)
			msg += GameDef.RGBToHex(plusColor) + string.Format(" (+{0:#,###,###,##0.##})", value2) + "[-]";
		else if (value2 < 0.0f)
			msg += GameDef.RGBToHex(minusColor) + string.Format(" ({0:#,###,###,##0.##})", value2) + "[-]";
		
		return msg;
	}
	
	public string MakeInfoPercentString(string title, float value1, float value2, Color plusColor, Color minusColor)
	{
		value1 = value1 * 100.0f;
		value2 = value2 * 100.0f;
		
		string msg = string.Format("{0} : {1:#0.##}%", title, value1);
		
		if (value2 > 0.0f)
			msg += GameDef.RGBToHex(plusColor) + string.Format(" (+{0:#,###,###,##0.##}%)", value2) + "[-]";
		else if (value2 < 0.0f)
			msg += GameDef.RGBToHex(minusColor) + string.Format(" ({0:#,###,###,##0.##}%)", value2) + "[-]";
		
		return msg;
	}
	
	public string MakeInfoString(float value1, Color plusColor, Color minusColor)
	{
		string msg = "";
		if (value1 > 0.0f)
			msg = string.Format("{0}(+{1:#,###,###,##0.##})[-]", GameDef.RGBToHex(plusColor), value1);
		else if (value1 < 0.0f)
			msg = string.Format("{0}({1:#,###,###,##0.##})[-]", GameDef.RGBToHex(minusColor), value1);
		
		return msg;
	}
	
	public string MakeInfoPercentString(float value1, Color plusColor, Color minusColor)
	{
		string msg = "";
		value1 = value1 * 100.0f;
		if (value1 > 0.0f)
			msg = string.Format("{0}(+{1:#,###,###,##0.##}%)[-]", GameDef.RGBToHex(plusColor), value1);
		else if (value1 < 0.0f)
			msg = string.Format("{0}({1:#,###,###,##0.##}%)[-]", GameDef.RGBToHex(minusColor), value1);
		
		return msg;
	}
	
	public string MakeInfoString(string title, Vector2 range)
	{
		string msg = string.Format("{0} : {1:#,###,###,##0.##} ~ {2:#,###,###,##0.##}", title, range.x, range.y);
		
		return msg;
	}
	
	public string MakeInfoPercentString(string title, Vector2 range)
	{
		range.x = range.x * 100.0f;
		range.y = range.y * 100.0f;
		
		string msg = string.Format("{0} : {1:#,###,###,##0.##} ~ {2:#,###,###,##0.##}%", title, range.x, range.y);
		
		return msg;
	}
	
	public string GetInfoStrAddNextLevel(int nextLevel, Color origPlusColor, Color origMinusColor, Color plusColor, Color minusColor)
	{
		string msg = "";
		string addInfoStr = "";
		
		string baseInfoStr = GetItemInfoStr(origPlusColor, origMinusColor);
		
		float nextValue = GetNextLevelIncValue(nextLevel);
		
		switch(valueType)
		{
		case eAttributeType.CriticalHitRate:
			nextValue = Mathf.Clamp(nextValue, 0.0f, 1.0f);
			addInfoStr = MakeInfoPercentString(nextValue, plusColor, minusColor);
			break;
		case eAttributeType.LifeSteal:
			addInfoStr = MakeInfoPercentString(nextValue, plusColor, minusColor);
			break;
		default:
			addInfoStr = MakeInfoString(nextValue, plusColor, minusColor);
			break;
		}
		
		msg = string.Format("{0}{1}", baseInfoStr, addInfoStr);
		
		return msg;
	}
	
	public static AttributeValue.eAttributeType GetAttributeType(string typeStr)
	{
		AttributeValue.eAttributeType attType = AttributeValue.eAttributeType.None;
		
		if (typeStr == "AbilityPower")
			attType = AttributeValue.eAttributeType.AbilityPower;
		else if (typeStr == "AttackDamage")
			attType = AttributeValue.eAttributeType.AttackDamage;
		else if (typeStr == "CriticalHitRate")
			attType = AttributeValue.eAttributeType.CriticalHitRate;
		else if (typeStr == "CriticalDamageRate")
			attType = AttributeValue.eAttributeType.CriticalDamageRate;
		else if (typeStr == "Health")
			attType = AttributeValue.eAttributeType.Health;
		else if (typeStr == "HealthRegen")
			attType = AttributeValue.eAttributeType.HealthRegen;
		else if (typeStr == "HealthMax")
			attType = AttributeValue.eAttributeType.HealthMax;
		else if (typeStr == "Armor")
			attType = AttributeValue.eAttributeType.Armor;
		else if (typeStr == "MagicResist")
			attType = AttributeValue.eAttributeType.MagicResist;
		else if (typeStr == "ArmorPenetration")
			attType = AttributeValue.eAttributeType.ArmorPenetration;
		else if (typeStr == "MagicPenetration")
			attType = AttributeValue.eAttributeType.MagicPenetration;
		else if (typeStr == "Rage")
			attType = AttributeValue.eAttributeType.Rage;
		else if (typeStr == "RageMax")
			attType = AttributeValue.eAttributeType.RageMax;
		else if (typeStr == "RageRegen")
			attType = AttributeValue.eAttributeType.RageRegen;
		else if (typeStr == "Vital")
			attType = AttributeValue.eAttributeType.Vital;
		else if (typeStr == "VitalMax")
			attType = AttributeValue.eAttributeType.VitalMax;
		else if (typeStr == "VitalRegen")
			attType = AttributeValue.eAttributeType.VitalRegen;
		else if (typeStr == "Mana")
			attType = AttributeValue.eAttributeType.Mana;
		else if (typeStr == "ManaMax")
			attType = AttributeValue.eAttributeType.ManaMax;
		else if (typeStr == "ManaRegen")
			attType = AttributeValue.eAttributeType.ManaRegen;
		else if (typeStr == "LifeSteal")
			attType = AttributeValue.eAttributeType.LifeSteal;
		else if (typeStr == "Stamina")
			attType = AttributeValue.eAttributeType.Stamina;
		else if (typeStr == "IncGainGold")
			attType = AttributeValue.eAttributeType.IncGainGold;
		
		return attType;
	}
	
	public static string GetAttributeName(eAttributeType type)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable strTable = null;
		if (tableManager != null)
			strTable = tableManager.stringTable;
		
		string msg = "";
		switch(type)
		{
		case eAttributeType.AbilityPower:
			msg = strTable.GetData(102);
			break;
		case eAttributeType.AttackDamage:
			msg = strTable.GetData(101);
			break;
		case eAttributeType.CriticalHitRate:
			msg = strTable.GetData(103);
			break;
		case eAttributeType.CriticalDamageRate:
			msg = strTable.GetData(104);
			break;
		case eAttributeType.Health:
			msg = strTable.GetData(105);
			break;
		case eAttributeType.HealthRegen:
			msg = strTable.GetData(106);
			break;
		case eAttributeType.HealthMax:
			msg = strTable.GetData(105);
			break;
		case eAttributeType.Armor:
			msg = strTable.GetData(113);
			break;
		case eAttributeType.MagicResist:
			msg = strTable.GetData(114);
			break;
		case eAttributeType.ArmorPenetration:
			msg = strTable.GetData(115);
			break;
		case eAttributeType.MagicPenetration:
			msg = strTable.GetData(116);
			break;
		case eAttributeType.Rage:
			msg = strTable.GetData(107);
			break;
		case eAttributeType.RageMax:
			msg = strTable.GetData(107);
			break;
		case eAttributeType.RageRegen:
			msg = strTable.GetData(108);
			break;
		case eAttributeType.Vital:
			msg = strTable.GetData(109);
			break;
		case eAttributeType.VitalMax:
			msg = strTable.GetData(109);
			break;
		case eAttributeType.VitalRegen:
			msg = strTable.GetData(110);
			break;
		case eAttributeType.Mana:
			msg = strTable.GetData(111);
			break;
		case eAttributeType.ManaMax:
			msg = strTable.GetData(111);
			break;
		case eAttributeType.ManaRegen:
			msg = strTable.GetData(112);
			break;
		case eAttributeType.LifeSteal:
			msg = strTable.GetData(118);
			break;
		case eAttributeType.IncGainGold:
			msg = strTable.GetData(235);
			break;
		}
		
		return msg;
	}
	
	public string GetItemInfoStr(Color plusColor, Color minusColor)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable strTable = null;
		if (tableManager != null)
			strTable = tableManager.stringTable;
		
		float resultValue = ValueForUI;
		float initValue = CalcAddValue(baseValue * itemRateValue * level_limitRate);
		float addValue = 0.0f;
		
		if (valueType == eAttributeType.CriticalHitRate)
		{
			resultValue = Mathf.Clamp(resultValue, 0.0f, 1.0f);
			initValue = Mathf.Clamp(initValue, 0.0f, 1.0f);
		}
		
		addValue = resultValue - initValue;
		
		string msg = "";
		if (this.calcType == eAttributeCalcType.CostumeAttribute)
		{
			switch(valueType)
			{
			case eAttributeType.AbilityPower:
				msg = MakeInfoPercentString(strTable.GetData(102), initValue, addValue, plusColor, minusColor); //"Magic : "
				break;
			case eAttributeType.AttackDamage:
				msg = MakeInfoPercentString(strTable.GetData(101), initValue, addValue, plusColor, minusColor); //AttackDamage
				break;
			case eAttributeType.CriticalHitRate:
				msg = MakeInfoPercentString(strTable.GetData(103), initValue, addValue, plusColor, minusColor); //"CriticalRate : " + Value;
				break;
			case eAttributeType.CriticalDamageRate:
				msg = MakeInfoPercentString(strTable.GetData(104), initValue, addValue, plusColor, minusColor); //"CriticalDamage : " + Value;
				break;
			case eAttributeType.Health:
				msg = MakeInfoPercentString(strTable.GetData(105), initValue, addValue, plusColor, minusColor); //"Health : " + Value;
				break;
			case eAttributeType.HealthRegen:
				msg = MakeInfoPercentString(strTable.GetData(106), initValue, addValue, plusColor, minusColor); //"HealthRegen : " + Value;
				break;
			case eAttributeType.HealthMax:
				msg = MakeInfoPercentString(strTable.GetData(105), initValue, addValue, plusColor, minusColor); //"MaxHealth : " + Value;
				break;
			case eAttributeType.Armor:
				msg = MakeInfoPercentString(strTable.GetData(113), initValue, addValue, plusColor, minusColor); //"Armor : " + Value;
				break;
			case eAttributeType.MagicResist:
				msg = MakeInfoPercentString(strTable.GetData(114), initValue, addValue, plusColor, minusColor); //"MagicResist : " + Value;
				break;
			case eAttributeType.ArmorPenetration:
				msg = MakeInfoPercentString(strTable.GetData(115), initValue, addValue, plusColor, minusColor); //"ArmorPenetration : " + Value;
				break;
			case eAttributeType.MagicPenetration:
				msg = MakeInfoPercentString(strTable.GetData(116), initValue, addValue, plusColor, minusColor); //"MagicPenetration : " + Value;
				break;
			case eAttributeType.Rage:
				msg = MakeInfoPercentString(strTable.GetData(107), initValue, addValue, plusColor, minusColor); //"Rage : " + Value;
				break;
			case eAttributeType.RageMax:
				msg = MakeInfoPercentString(strTable.GetData(107), initValue, addValue, plusColor, minusColor); //"MaxRage : " + Value;
				break;
			case eAttributeType.RageRegen:
				msg = MakeInfoPercentString(strTable.GetData(108), initValue, addValue, plusColor, minusColor); //"RageRegen : " + Value;
				break;
			case eAttributeType.Vital:
				msg = MakeInfoPercentString(strTable.GetData(109), initValue, addValue, plusColor, minusColor); //"Vital : " + Value;
				break;
			case eAttributeType.VitalMax:
				msg = MakeInfoPercentString(strTable.GetData(109), initValue, addValue, plusColor, minusColor); //"MaxVital : " + Value;
				break;
			case eAttributeType.VitalRegen:
				msg = MakeInfoPercentString(strTable.GetData(110), initValue, addValue, plusColor, minusColor); //"VitalRegen : " + Value;
				break;
			case eAttributeType.Mana:
				msg = MakeInfoPercentString(strTable.GetData(111), initValue, addValue, plusColor, minusColor); //"Mana : " + Value;
				break;
			case eAttributeType.ManaMax:
				msg = MakeInfoPercentString(strTable.GetData(111), initValue, addValue, plusColor, minusColor); //"MaxMana : " + Value;
				break;
			case eAttributeType.ManaRegen:
				msg = MakeInfoPercentString(strTable.GetData(112), initValue, addValue, plusColor, minusColor); //"ManaRegen : " + Value;
				break;
			case eAttributeType.LifeSteal:
				msg = MakeInfoPercentString(strTable.GetData(118) , initValue, addValue, plusColor, minusColor);
				break;
			}
		}
		else
		{
			switch(valueType)
			{
			case eAttributeType.AbilityPower:
				msg = MakeInfoString(strTable.GetData(102), initValue, addValue, plusColor, minusColor); //"Magic : "
				break;
			case eAttributeType.AttackDamage:
				msg = MakeInfoString(strTable.GetData(101), initValue, addValue, plusColor, minusColor); //AttackDamage
				break;
			case eAttributeType.CriticalHitRate:
				msg = MakeInfoPercentString(strTable.GetData(103), initValue, addValue, plusColor, minusColor); //"CriticalRate : " + Value;
				break;
			case eAttributeType.CriticalDamageRate:
				msg = MakeInfoPercentString(strTable.GetData(104), initValue, addValue, plusColor, minusColor); //"CriticalDamage : " + Value;
				break;
			case eAttributeType.Health:
				msg = MakeInfoString(strTable.GetData(105), initValue, addValue, plusColor, minusColor); //"Health : " + Value;
				break;
			case eAttributeType.HealthRegen:
				msg = MakeInfoString(strTable.GetData(106), initValue, addValue, plusColor, minusColor); //"HealthRegen : " + Value;
				break;
			case eAttributeType.HealthMax:
				msg = MakeInfoString(strTable.GetData(105), initValue, addValue, plusColor, minusColor); //"MaxHealth : " + Value;
				break;
			case eAttributeType.Armor:
				msg = MakeInfoString(strTable.GetData(113), initValue, addValue, plusColor, minusColor); //"Armor : " + Value;
				break;
			case eAttributeType.MagicResist:
				msg = MakeInfoString(strTable.GetData(114), initValue, addValue, plusColor, minusColor); //"MagicResist : " + Value;
				break;
			case eAttributeType.ArmorPenetration:
				msg = MakeInfoString(strTable.GetData(115), initValue, addValue, plusColor, minusColor); //"ArmorPenetration : " + Value;
				break;
			case eAttributeType.MagicPenetration:
				msg = MakeInfoString(strTable.GetData(116), initValue, addValue, plusColor, minusColor); //"MagicPenetration : " + Value;
				break;
			case eAttributeType.Rage:
				msg = MakeInfoString(strTable.GetData(107), initValue, addValue, plusColor, minusColor); //"Rage : " + Value;
				break;
			case eAttributeType.RageMax:
				msg = MakeInfoString(strTable.GetData(107), initValue, addValue, plusColor, minusColor); //"MaxRage : " + Value;
				break;
			case eAttributeType.RageRegen:
				msg = MakeInfoString(strTable.GetData(108), initValue, addValue, plusColor, minusColor); //"RageRegen : " + Value;
				break;
			case eAttributeType.Vital:
				msg = MakeInfoString(strTable.GetData(109), initValue, addValue, plusColor, minusColor); //"Vital : " + Value;
				break;
			case eAttributeType.VitalMax:
				msg = MakeInfoString(strTable.GetData(109), initValue, addValue, plusColor, minusColor); //"MaxVital : " + Value;
				break;
			case eAttributeType.VitalRegen:
				msg = MakeInfoString(strTable.GetData(110), initValue, addValue, plusColor, minusColor); //"VitalRegen : " + Value;
				break;
			case eAttributeType.Mana:
				msg = MakeInfoString(strTable.GetData(111), initValue, addValue, plusColor, minusColor); //"Mana : " + Value;
				break;
			case eAttributeType.ManaMax:
				msg = MakeInfoString(strTable.GetData(111), initValue, addValue, plusColor, minusColor); //"MaxMana : " + Value;
				break;
			case eAttributeType.ManaRegen:
				msg = MakeInfoString(strTable.GetData(112), initValue, addValue, plusColor, minusColor); //"ManaRegen : " + Value;
				break;
			case eAttributeType.LifeSteal:
				msg = MakeInfoPercentString(strTable.GetData(118) , initValue, addValue, plusColor, minusColor);
				break;
			}
		}
		
		return msg;
	}
	
	public string GetTemplateItemInfoStr()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable strTable = null;
		if (tableManager != null)
			strTable = tableManager.stringTable;
		
		Vector2 resultValue = GetMinMaxValue(0.8f, 1.2f);
		
		string msg = "";
		switch(valueType)
		{
		case eAttributeType.AbilityPower:
			msg = MakeInfoString(strTable.GetData(102), resultValue); //"Magic : "
			break;
		case eAttributeType.AttackDamage:
			msg = MakeInfoString(strTable.GetData(101), resultValue); //AttackDamage
			break;
		case eAttributeType.CriticalHitRate:
			msg = MakeInfoPercentString(strTable.GetData(103), resultValue); //"CriticalRate : " + Value;
			break;
		case eAttributeType.CriticalDamageRate:
			msg = MakeInfoPercentString(strTable.GetData(104), resultValue); //"CriticalDamage : " + Value;
			break;
		case eAttributeType.Health:
			msg = MakeInfoString(strTable.GetData(105), resultValue); //"Health : " + Value;
			break;
		case eAttributeType.HealthRegen:
			msg = MakeInfoString(strTable.GetData(106), resultValue); //"HealthRegen : " + Value;
			break;
		case eAttributeType.HealthMax:
			msg = MakeInfoString(strTable.GetData(105), resultValue); //"MaxHealth : " + Value;
			break;
		case eAttributeType.Armor:
			msg = MakeInfoString(strTable.GetData(113), resultValue); //"Armor : " + Value;
			break;
		case eAttributeType.MagicResist:
			msg = MakeInfoString(strTable.GetData(114), resultValue); //"MagicResist : " + Value;
			break;
		case eAttributeType.ArmorPenetration:
			msg = MakeInfoString(strTable.GetData(115), resultValue); //"ArmorPenetration : " + Value;
			break;
		case eAttributeType.MagicPenetration:
			msg = MakeInfoString(strTable.GetData(116), resultValue); //"MagicPenetration : " + Value;
			break;
		case eAttributeType.Rage:
			msg = MakeInfoString(strTable.GetData(107), resultValue); //"Rage : " + Value;
			break;
		case eAttributeType.RageMax:
			msg = MakeInfoString(strTable.GetData(107), resultValue); //"MaxRage : " + Value;
			break;
		case eAttributeType.RageRegen:
			msg = MakeInfoString(strTable.GetData(108), resultValue); //"RageRegen : " + Value;
			break;
		case eAttributeType.Vital:
			msg = MakeInfoString(strTable.GetData(109), resultValue); //"Vital : " + Value;
			break;
		case eAttributeType.VitalMax:
			msg = MakeInfoString(strTable.GetData(109), resultValue); //"MaxVital : " + Value;
			break;
		case eAttributeType.VitalRegen:
			msg = MakeInfoString(strTable.GetData(110), resultValue); //"VitalRegen : " + Value;
			break;
		case eAttributeType.Mana:
			msg = MakeInfoString(strTable.GetData(111), resultValue); //"Mana : " + Value;
			break;
		case eAttributeType.ManaMax:
			msg = MakeInfoString(strTable.GetData(111), resultValue); //"MaxMana : " + Value;
			break;
		case eAttributeType.ManaRegen:
			msg = MakeInfoString(strTable.GetData(112), resultValue); //"ManaRegen : " + Value;
			break;
		case eAttributeType.LifeSteal:
			msg = MakeInfoPercentString(strTable.GetData(118) , resultValue);
			break;
		}
		
		return msg;
	}
}

[System.Serializable]
public class AttributeManager {

	//public List<AttributeValue> attributeValues = new List<AttributeValue>();
	public Dictionary<AttributeValue.eAttributeType, AttributeValue> attributeValues = new Dictionary<AttributeValue.eAttributeType, AttributeValue>();
	//public MasteryManager masteryManager = new MasteryManager();
	
	
	public delegate void OnChangeAttributeManager();
	public OnChangeAttributeManager onChangeAttributeManager = null;
	
	public AttributeManager()
	{

	}
	
	public List<AttributeValue.eAttributeType> basicAttributeTypeList = new List<AttributeValue.eAttributeType>();
	public void AddAttributeValue(AttributeValue newInfo)
	{
		if (newInfo == null)
			return;
		
		if (attributeValues.ContainsKey(newInfo.valueType) == true)
			attributeValues.Remove(newInfo.valueType);
		
		attributeValues.Add(newInfo.valueType, newInfo);
	}
	
	public AttributeValue GetAttribute(AttributeValue.eAttributeType type)
	{
		AttributeValue aValue = null;
		if (attributeValues.ContainsKey(type) == true)
			aValue = attributeValues[type];
		
		return aValue;
	}
	
	public void SubValue(AttributeValue _value)
	{
		if (_value == null)
			return;
		
		AttributeValue origValue = GetAttribute(_value.valueType);
		if (origValue != null)
		{
			origValue.addValue -= _value.Value;
			
			UpdateValue(origValue);
			
			origValue.DebugInfo();
		}
	}
	
	public void AddValue(AttributeValue _value)
	{
		if (_value == null)
			return;
		
		AttributeValue origValue = GetAttribute(_value.valueType);
		if (origValue == null)
		{
			TableManager tableManager = TableManager.Instance;
			AttributeInitTable attributeIncTable = null;
			if (tableManager != null)
				attributeIncTable = tableManager.attributeIncTable;
			
			AttributeInitData incData = null;
			if (attributeIncTable != null)
				incData = attributeIncTable.GetData((int)(Game.Instance.playerClass) + 1);
			
			float incValue = incData != null ? incData.GetValue(_value.valueType) : 0.0f;

			origValue = new AttributeValue(_value.valueType, 0.0f, incValue, 1.0f);
			AddAttributeValue(origValue);
		}
				
		if (origValue != null)
		{
			origValue.addValue += _value.Value;
			
			UpdateValue(origValue);
			
			origValue.DebugInfo();
		}
	}
	
	public void SubValue(AttributeValue.eAttributeType _type, float _value)
	{
		AttributeValue origValue = GetAttribute(_type);
		if (origValue != null)
		{
			origValue.addValue -= _value;
			
			UpdateValue(origValue);
			
			origValue.DebugInfo();
		}
	}
	
	public void AddValue(AttributeValue.eAttributeType _type, float _value)
	{
		AttributeValue origValue = GetAttribute(_type);
		if (origValue == null)
		{
			TableManager tableManager = TableManager.Instance;
			AttributeInitTable attributeIncTable = null;
			if (tableManager != null)
				attributeIncTable = tableManager.attributeIncTable;
			
			AttributeInitData incData = null;
			if (attributeIncTable != null)
				incData = attributeIncTable.GetData((int)(Game.Instance.playerClass) + 1);
			
			float incValue = incData != null ? incData.GetValue(_type) : 0.0f;

			origValue = new AttributeValue(_type, 0.0f, incValue, 1.0f);
			AddAttributeValue(origValue);
		}
		
		if (origValue != null)
		{
			origValue.addValue += _value;
			
			UpdateValue(origValue);
			
			origValue.DebugInfo();
		}
	}
	
	public void SetMasteryValue(AttributeValue.eAttributeType _type, float _value)
	{
		AttributeValue origValue = GetAttribute(_type);
		if (origValue != null)
		{
			origValue.addValueByMastery = _value;
			
			UpdateValue(origValue);
		}
	}
	
	public void SetMasteryValueRate(AttributeValue.eAttributeType _type, float _value)
	{
		AttributeValue origValue = GetAttribute(_type);
		if (origValue != null)
		{
			if (_type == AttributeValue.eAttributeType.IncGainExp)
				origValue.baseValue = _value;
			else
				origValue.addRateByMastery = _value;
			
			UpdateValue(origValue);
		}
	}
	
	public void AddValueRate(AttributeValue.eAttributeType _type, float _value)
	{
		AttributeValue origValue = GetAttribute(_type);
		if (origValue == null)
		{
			TableManager tableManager = TableManager.Instance;
			AttributeInitTable attributeIncTable = null;
			if (tableManager != null)
				attributeIncTable = tableManager.attributeIncTable;
			
			AttributeInitData incData = null;
			if (attributeIncTable != null)
				incData = attributeIncTable.GetData((int)(Game.Instance.playerClass) + 1);
			
			float incValue = incData != null ? incData.GetValue(_type) : 0.0f;

			origValue = new AttributeValue(_type, 0.0f, incValue, 1.0f);
			AddAttributeValue(origValue);
		}
		
		if (origValue != null)
		{
			if (_type == AttributeValue.eAttributeType.IncGainExp)
				origValue.baseValue += _value;
			else
				origValue.addRate += _value;
			
			UpdateValue(origValue);
			
			//origValue.DebugInfo();
		}
	}
	
	public void SubValueRate(AttributeValue.eAttributeType _type, float _value)
	{
		AttributeValue origValue = GetAttribute(_type);
		if (origValue != null)
		{
			if (_type == AttributeValue.eAttributeType.IncGainExp)
				origValue.baseValue -= _value;
			else
				origValue.addRate -= _value;
			//origValue.addRate = Mathf.Max(0.0f, origValue.addRate);
			
			UpdateValue(origValue);
			
			origValue.DebugInfo();
		}
	}
	
	public float GetAttributeValue(AttributeValue.eAttributeType type)
	{
		float attributeValue = 0.0f;
		
		AttributeValue aValue = GetAttribute(type);
		if (aValue != null)
			attributeValue = aValue.Value;
		
		return attributeValue;
	}
	
	public void UpdateValue(AttributeValue updateValue)
	{
		if (updateValue == null)
			return;
		
		/*
		if (attributeValues.ContainsKey(updateValue.valueType) == true)
			attributeValues.Remove(updateValue.valueType);
		
		attributeValues.Add(updateValue.valueType, updateValue);
		*/
		
		switch(updateValue.valueType)
		{
		case AttributeValue.eAttributeType.Health:
		case AttributeValue.eAttributeType.HealthMax:
			UpdateHPUI();
			break;
		case AttributeValue.eAttributeType.Mana:
		case AttributeValue.eAttributeType.Vital:
		case AttributeValue.eAttributeType.Rage:
			UpdateAbilityUI(updateValue.valueType);
			break;
		}
		
		if (onChangeAttributeManager != null)
			onChangeAttributeManager();
	}
	
	public UISlider abilityUI = null;
	public UILabel abilityInfoLabel = null;
	public void UpdateAbilityUI(AttributeValue.eAttributeType type)
	{
		if (abilityUI != null)
		{
			AttributeValue baseAttributeValue = null;
			AttributeValue maxAttributeValue = null;
			
			switch(type)
			{
			case AttributeValue.eAttributeType.Mana:
				baseAttributeValue = GetAttribute(AttributeValue.eAttributeType.Mana);
				maxAttributeValue = GetAttribute(AttributeValue.eAttributeType.ManaMax);
				break;
			case AttributeValue.eAttributeType.Rage:
				baseAttributeValue = GetAttribute(AttributeValue.eAttributeType.Rage);
				maxAttributeValue = GetAttribute(AttributeValue.eAttributeType.RageMax);
				break;
			case AttributeValue.eAttributeType.Vital:
				baseAttributeValue = GetAttribute(AttributeValue.eAttributeType.Vital);
				maxAttributeValue = GetAttribute(AttributeValue.eAttributeType.VitalMax);
				break;
			}
			
			if (baseAttributeValue != null && maxAttributeValue != null)
			{
				float curValue = baseAttributeValue.Value;
				float maxValue = maxAttributeValue.Value;
				
				if (abilityInfoLabel != null)
					abilityInfoLabel.text = string.Format("{0}/{1}", (int)curValue, (int)maxValue);
				
				float rateValue = curValue / maxValue;
				
				
				abilityUI.sliderValue = rateValue;
			}
		}
	}
	
	public UISlider hpUI = null;
	public UILabel hpInfoLabel = null;
	public void UpdateHPUI()
	{
		if (hpUI != null)
		{
			float maxValue = GetAttributeValue(AttributeValue.eAttributeType.HealthMax);
			float curValue = GetAttributeValue(AttributeValue.eAttributeType.Health);
			
			if (hpInfoLabel != null)
				hpInfoLabel.text = string.Format("{0}/{1}", (int)curValue, (int)maxValue);
			
			float rateValue = curValue / maxValue;
			
			hpUI.sliderValue = rateValue;
		}
	}
	
	/*
	public void UpdateValue()
	{
		addMaxHP = 0.0f;
		maxHP = hp = baseHP;
		
		addMaxAbility = 0.0f;
		maxAbility = abilityValue = baseAbility;
		
		attackPower = baseAttackPower;
	}
	*/
	
	public void UpdateLevel(int level)
	{
		AttributeValue _value = null;
		foreach(AttributeValue.eAttributeType type in basicAttributeTypeList)
		{
			_value = GetAttribute(type);
			if (_value != null)
			{
				_value.SetLevel(level);
				UpdateValue(_value);
			}
		}
		
		HpFull();
		
		DebugInfo();
	}
	
	public void HpFull()
	{
		float maxValue = GetAttributeValue(AttributeValue.eAttributeType.HealthMax);
		AttributeValue hpValue = GetAttribute(AttributeValue.eAttributeType.Health);
		
		if (hpValue != null)
			hpValue.baseValue = maxValue;
		
		if (hpUI != null)
			hpUI.sliderValue = 1.0f;
		
	}
	
	public void AbilityFull(AttributeValue.eAttributeType abilityType, AttributeValue.eAttributeType maxValueType)
	{
		float maxValue = GetAttributeValue(maxValueType);
		AttributeValue abilityValue = GetAttribute(abilityType);
		
		if (abilityValue != null)
			abilityValue.baseValue = maxValue;
		
		if (abilityUI != null)
			abilityUI.sliderValue = 1.0f;
	}
	
	public void DebugInfo()
	{
		Debug.Log("Start Attribute Manager Value Infos.......");
		AttributeValue _value = null;
		foreach(var temp in attributeValues)
		{
			_value = temp.Value;
			
			_value.DebugInfo();
		}
		
		Debug.Log("End Attribute Manager Value Infos.......");
	}
}
