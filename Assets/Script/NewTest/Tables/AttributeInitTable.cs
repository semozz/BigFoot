using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttributeInitData
{
	public float abilityPower = 0.0f;
	public float attackDamage = 0.0f;
	public float criticalHitRate = 0.0f;
	public float criticalDamageRate = 0.0f;
	
	public float healthMax = 0.0f;
	public float healthRegen = 0.0f;
	
	public float rageMax = 0.0f;
	public float rageRegen = 0.0f;
	
	public float vitalMax = 0.0f;
	public float vitalRegen = 0.0f;
	
	public float manaMax = 0.0f;
	public float manaRegen = 0.0f;
	
	public float armor = 0.0f;
	public float magicResist = 0.0f;
	
	public float armorPenetration = 0.0f;
	public float magicPenetration = 0.0f;
	
	public float stamina = 0.0f;
	public float lifeSteal = 0.0f;
	public float incGainGold = 0.0f;
	
	public float GetValue(AttributeValue.eAttributeType valueType)
	{
		float defValue = 0.0f;
		switch(valueType)
		{
		case AttributeValue.eAttributeType.AbilityPower:
			defValue = abilityPower;
			break;
		case AttributeValue.eAttributeType.AttackDamage:
			defValue = attackDamage;
			break;
		case AttributeValue.eAttributeType.CriticalHitRate:
			defValue = criticalHitRate;
			break;
		case AttributeValue.eAttributeType.CriticalDamageRate:
			defValue = criticalDamageRate;
			break;
		case AttributeValue.eAttributeType.Health:
			defValue = 0.0f;
			break;
		case AttributeValue.eAttributeType.HealthRegen:
			defValue = healthRegen;
			break;
		case AttributeValue.eAttributeType.HealthMax:
			defValue = healthMax;
			break;
		case AttributeValue.eAttributeType.Armor:
			defValue = armor;
			break;
		case AttributeValue.eAttributeType.MagicResist:
			defValue = magicResist;
			break;
		case AttributeValue.eAttributeType.ArmorPenetration:
			defValue = armorPenetration;
			break;
		case AttributeValue.eAttributeType.MagicPenetration:
			defValue = magicPenetration;
			break;
		case AttributeValue.eAttributeType.Rage:
			defValue = 0.0f;
			break;
		case AttributeValue.eAttributeType.RageMax:
			defValue = rageMax;
			break;
		case AttributeValue.eAttributeType.RageRegen:
			defValue = rageRegen;
			break;
		case AttributeValue.eAttributeType.Vital:
			defValue = 0.0f;
			break;
		case AttributeValue.eAttributeType.VitalMax:
			defValue = vitalMax;
			break;
		case AttributeValue.eAttributeType.VitalRegen:
			defValue = vitalRegen;
			break;
		case AttributeValue.eAttributeType.Mana:
			defValue = 0.0f;
			break;
		case AttributeValue.eAttributeType.ManaMax:
			defValue = manaMax;
			break;
		case AttributeValue.eAttributeType.ManaRegen:
			defValue = manaRegen;
			break;
		case AttributeValue.eAttributeType.LifeSteal:
			defValue = lifeSteal;
			break;
		case AttributeValue.eAttributeType.IncGainGold:
			defValue = incGainGold;
			break;
		}
		
		return defValue;
	}
}

public class AttributeInitTable : BaseTable
{
	public Dictionary<int, AttributeInitData> dataList = new Dictionary<int, AttributeInitData>();
	
	public AttributeInitData GetData(int id)
	{
		AttributeInitData data = null;
		if (dataList != null &&
			dataList.ContainsKey(id) == true)
			data = dataList[id];
		
		return data;
	}
	
	public override void LoadTable(CSVDB db)
	{
		if (db != null)
		{
			int id = 0;
			AttributeInitData initData = null;
			
			foreach(var data in db.data)
			{
				id = int.Parse(data.Key);
				
				initData = new AttributeInitData();
				
				initData.attackDamage = data.Value.GetValue("AttackDamage").ToFloat();
				initData.abilityPower = data.Value.GetValue("AbilityPower").ToFloat();
				
				initData.criticalHitRate = data.Value.GetValue("CriticalHitRate").ToFloat();
				initData.criticalDamageRate = data.Value.GetValue("CriticalDamageRate").ToFloat();
				
				initData.healthMax = data.Value.GetValue("HealthMax").ToFloat();
				initData.healthRegen = data.Value.GetValue("HealthRegen").ToFloat();
				
				initData.rageMax = data.Value.GetValue("RageMax").ToFloat();
				initData.rageRegen = data.Value.GetValue("RageRegen").ToFloat();
				initData.vitalMax = data.Value.GetValue("VitalMax").ToFloat();
				initData.vitalRegen = data.Value.GetValue("VitalRegen").ToFloat();
				initData.manaMax = data.Value.GetValue("ManaMax").ToFloat();
				initData.manaRegen = data.Value.GetValue("ManaRegen").ToFloat();
				
				initData.armor = data.Value.GetValue("Armor").ToFloat();
				initData.magicResist = data.Value.GetValue("MagicResist").ToFloat();
				initData.armorPenetration = data.Value.GetValue("ArmorPenetration").ToFloat();
				initData.magicPenetration = data.Value.GetValue("MagicPenetration").ToFloat();
				
				initData.stamina = data.Value.GetValue("Stamina").ToFloat();
				
				ValueData tempData = null;
				tempData = data.Value.GetValue("LifeSteal");
				if (tempData != null)
					initData.lifeSteal = tempData.ToFloat();
				
				tempData = data.Value.GetValue("GainGold");
				if (tempData != null)
					initData.incGainGold = tempData.ToFloat();
				
				this.dataList.Add(id, initData);
			}
		}
	}
}
