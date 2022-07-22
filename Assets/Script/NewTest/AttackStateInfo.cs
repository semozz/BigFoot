using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackStateInfo {
	public LifeManager ownerActor = null;
	public BaseState.eState attackState = BaseState.eState.None;
	
	public StateInfo stateInfo = new StateInfo();
	
	public List<BuffInfo> buffList = new List<BuffInfo>();
	
	public float attackDamage = 0.0f;
	public float abilityPower = 0.0f;
	
	public float addAttackPower = 0.0f;
	public float addAttackRate = 0.0f;
	
	public void SetOwnerActor(LifeManager actor)
	{
		ownerActor = actor;
	}
	
	public void SetState(StateInfo info)
	{
		stateInfo.SetInfo(info);
		
		buffList.Clear();
	}
	
	public void AddBuff(float hitRate, GameDef.eBuffType type, float buffValue, float buffTime)
	{
		BuffInfo newBuffInfo = new BuffInfo();
		
		newBuffInfo.Rate = hitRate;
		newBuffInfo.Type = type;
		newBuffInfo.Value = buffValue;
		newBuffInfo.DelayTime = buffTime;
		
		buffList.Add(newBuffInfo);
	}
	
	public float CalcValue(float baseValue, float optionRate)
	{
		float baseAttack = baseValue + addAttackPower;
		float attackRate = (stateInfo.attackRate * (1.0f + stateInfo.addAttackRate + addAttackRate + optionRate));
		float attackDamage = baseAttack * attackRate;
		
		return attackDamage;
	}
	
	public float GetAttackDamage()
	{
		float optionAddAttackRate = 0.0f;
		
		if (ownerActor != null &&
			ownerActor.buffManager != null)
		{
			int buffIndex = ownerActor.buffManager.GetBuff(GameDef.eBuffType.BT_BERSERK);
			if (buffIndex != -1)
			{
				BuffManager.stBuff buffInfo = ownerActor.buffManager.mHaveBuff[buffIndex];
				optionAddAttackRate = buffInfo.AbilityValue;
			}
		}
		
		return GetAttackDamage(optionAddAttackRate);
	}
	
	public float GetAbilityPower()
	{
		float optionAddAttackRate = 0.0f;
		
		if (ownerActor != null &&
			ownerActor.buffManager != null)
		{
			int buffIndex = ownerActor.buffManager.GetBuff(GameDef.eBuffType.BT_BERSERK);
			if (buffIndex != -1)
			{
				BuffManager.stBuff buffInfo = ownerActor.buffManager.mHaveBuff[buffIndex];
				optionAddAttackRate = buffInfo.AbilityValue;
			}
		}
		
		return GetAbilityPower(optionAddAttackRate);
	}
	
	public float GetAttackDamage(float optionRate)
	{
		if (attackDamage == 0.0f)
			return 0.0f;
		
		return CalcValue(attackDamage, optionRate);
	}
	
	public float GetAbilityPower(float optionRate)
	{
		if (abilityPower == 0.0f)
			return 0.0f;
		
		return CalcValue(abilityPower, optionRate);
	}
}
