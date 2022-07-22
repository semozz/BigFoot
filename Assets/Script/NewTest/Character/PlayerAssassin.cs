using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAssassin : PlayerController {
	public float basePoisonHitRate = 0.2f;
	public float addPoisonHitRate = 0.0f;
	
	public float poisonAttackRate = 1.0f;
	
	public float poisonBombAttackRate = 0.6f;
	
	public float startAbilityValue = 100.0f;
	
	public float incAbilityValue = 5.0f;
	public float incAbilityDelayTime = 0.0f;
	public float incAbilityCoolTime = 1.0f;
	
	public float incAbilityValueByPoisonHit = 1.0f;
	public float addAbilityValueRateByPoisonHit = 0.0f;
	
	public float addActionBCoolTime = 0.0f;
	
	public float poisionMoveSpeed = 1.0f;
	public float addPoisionMoveSpeed = 0.0f;
	
	public override void Start () {
		base.Start();
		
		incAbilityDelayTime = incAbilityCoolTime;
		
		if (this.lifeManager != null)
		{
			//lifeManager.onTargetHit = new LifeManager.OnTargetHit(OnTargetHit);
		}
	}
	
	public override AttributeInitData InitAttributeData ()
	{
		AttributeInitData initData = base.InitAttributeData ();
		
		AttributeInitTable attributeIncTable = null;
		TableManager tableManager = TableManager.Instance;
		
		if (tableManager != null)
		{
			attributeIncTable = tableManager.attributeIncTable;
		}
		
		AttributeInitData incData = null;
		if (attributeIncTable != null)
			incData = attributeIncTable.GetData((int)(this.classType) + 1);
		
		if (initData != null)
		{
			AttributeValue[] attributes = {
				new AttributeValue(AttributeValue.eAttributeType.Vital, initData.vitalMax, 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.VitalMax, initData.vitalMax, incData != null ? incData.vitalMax : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.VitalRegen, initData.vitalRegen, incData != null ? incData.vitalRegen : 0.0f, 0.0f),
			};
			
			foreach(AttributeValue initValue in attributes)
			{
				lifeManager.attributeManager.AddAttributeValue(initValue);
			}
		}
		
		return initData;
	}
	
	/*
	public override void Awake()
	{
		base.Awake();
		
		if (this.lifeManager != null)
		{
			lifeManager.masteryManager.onUpdateMastery = new MasteryManager.OnUpdateMastery(OnUpdateMastery);
			
			TableManager tableManager = TableManager.Instance;
			MasteryTable masteryTable = tableManager != null ? tableManager.masteryTable : null;
			if (masteryTable != null)
			{
				foreach(int id in masteryIDs)
				{
					MasteryTableInfo tableInfo = masteryTable.GetData(id);
					
					if (tableInfo != null)
					{
						MasteryInfo newInfo = new MasteryInfo();
						newInfo.tableID = id;
						newInfo.name = tableInfo.name;
						newInfo.iconName = tableInfo.iconName;
						newInfo.desc = tableInfo.desc;
						
						newInfo._incValue = tableInfo.incValue;
						newInfo._level = 0;
						
						lifeManager.masteryManager.AddMastery(newInfo);
					}
				}
			}
			
			MasteryInfo[] initValues = { 
				new MasteryInfo(MasteryInfo.eMasteries.AHeadWind, -1.0f, 0),
				new MasteryInfo(MasteryInfo.eMasteries.Sharp, -0.2f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.Sawtooth, 0.025f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.Infection, 0.015f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.Slaughter, 0.1f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.DeadlyPoision, 0.05f, 0), 
			};
			
			foreach(MasteryInfo initInfo in initValues)
			{
				lifeManager.masteryManager.AddMastery(initInfo);
			}
		}
	}
	*/
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
		
		AttributeManager attributeMgr = null;
		float hpRate = 0.0f;
		if (lifeManager != null)
		{
			attributeMgr = lifeManager.attributeManager;
			hpRate = lifeManager.GetHPRate();
		}
		
		if (hpRate <= 0.0f)
			return;
		
		incAbilityDelayTime -= Time.deltaTime;
		if (incAbilityDelayTime <= 0.0f)
		{
			incAbilityDelayTime = incAbilityCoolTime;
			
			if (attributeMgr != null)
			{
				float addRate = 0.0f;
				if (hpRate <= 0.35f)
					addRate = attributeMgr.GetAttributeValue(AttributeValue.eAttributeType.IncAbilityGainRateUnderHP35);
				
				float addAbilityValue = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.VitalRegen);
				float addValue = addAbilityValue + (addAbilityValue * addRate);// * 0.2f;
				OnChangeAbilityValue(addValue);
			}
		}
	}
	
	public override void OnTargetHit(LifeManager hitActor, float damage, bool isCritical, AttackStateInfo attackInfo)
	{
		int buffIndex = -1;
		if (hitActor != null &&  hitActor.buffManager != null)
			buffIndex = hitActor.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_POISION, this.lifeManager);
		
		int poisionStackCount = 0;
		if (buffIndex != -1)
		{
			BuffManager.stBuff buff = hitActor.buffManager.mHaveBuff[buffIndex];
			poisionStackCount = buff.StackCount;
			
			int rateValue = Mathf.RoundToInt((float)poisionStackCount * 0.2f * 100.0f);
			int randValue = Random.Range(0, 100);
			
			if (rateValue >= randValue)
			{
				float addValue = incAbilityValueByPoisonHit;
				addValue += incAbilityValueByPoisonHit * addAbilityValueRateByPoisonHit;
				OnChangeAbilityValue(addValue);
			}
		}
		
		switch (attackInfo.attackState)
		{
		case BaseState.eState.Dashattack:
		case BaseState.eState.Attack1:
		case BaseState.eState.Attack2:
		case BaseState.eState.Attack21:
		case BaseState.eState.Attack3:
		case BaseState.eState.Evadecounterattack:
		case BaseState.eState.JumpAttack:
		case BaseState.eState.Skill02:
		case BaseState.eState.AttackB_1:
		case BaseState.eState.AttackB_2:
		//case BaseState.eState.AttackB_3:
			//독 공격 확률
			int rateValue = Mathf.RoundToInt((basePoisonHitRate + addPoisonHitRate) * 100.0f);
			int randValue = Random.Range(0, 100);
			bool isAttackable = rateValue >= randValue;
			
			//죽음의 칼날은 100% 중독...
			if (attackInfo.attackState == BaseState.eState.AttackB_2)
				isAttackable = true;
			
			if (isAttackable == true)
			{
				//Debug.Log("Poision Damage Add...!!!!!! Time : " + Time.time);
				PoisonAttack(hitActor, 1);
			}
			
		    break;
		//독 데미지 적용 안함..
		case BaseState.eState.Blowattack:
			break;
		case BaseState.eState.Skill01:
			if (stateController.colliderManager.colliderStep == 3 && hitActor != null)
			{
				PoisonAttack(hitActor, 3);
			}
		    break;
		}
		
		base.OnTargetHit(hitActor, damage, isCritical, attackInfo);
	}
	
	public float CalcPoisionAttack()
	{
		float baseAttackValue = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower) * 0.5f;
		float addAttackValue = baseAttackValue * poisonAttackRate;
		
		return addAttackValue;
	}
	
	private void PoisonAttack(LifeManager hitActor, int poisonStackCount)
	{
		if (hitActor == null || hitActor.buffManager == null)
			return;
		
		/*
		if (IsAquiredSkill(PlayerController.eSkillInfo.Buff) == false)
			return;
		*/
		
		
		int buffIndex = hitActor.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_POISION, this.lifeManager);
		if (buffIndex != -1)
		{
			BuffManager.stBuff oldBuff = hitActor.buffManager.mHaveBuff[buffIndex];
		
			poisonStackCount = Mathf.Min(oldBuff.StackCount + poisonStackCount, (int)GameDef.ePoisonLevel.MAX_COUNT);
			hitActor.buffManager.RemoveBuff(buffIndex);
		}
		
		
		float basePoisionAttack = CalcPoisionAttack();
		
		hitActor.buffManager.AddBuff(GameDef.eBuffType.BT_POISION, basePoisionAttack, 9.9f, this.lifeManager, poisonStackCount);	
		
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//중독된 적의 공격력/주문력 감소 버프..
		float decAttackRate = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.DecAttackDamageOnPoison);
		float decRate = decAttackRate * poisonAttackRate;
		if (decRate != 0.0f)
		{
			buffIndex = hitActor.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_DEC_ATTACK_RATE, this.lifeManager);
			if (buffIndex != -1)
				hitActor.buffManager.RemoveBuff(buffIndex);
			
			hitActor.buffManager.AddBuff(GameDef.eBuffType.BT_DEC_ATTACK_RATE, decRate, 9.9f, this.lifeManager, 1);
		}
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		float slowRate = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.DecMoveSpeedWhenPoison);
		if (poisonStackCount > 0 && slowRate != 0.0f)
		{
			buffIndex = hitActor.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_SLOW, this.lifeManager);
			if (buffIndex != -1)
				hitActor.buffManager.RemoveBuff(buffIndex);
			
			float moveSpeed = (1.0f + (slowRate * poisonStackCount));
			hitActor.buffManager.AddBuff(GameDef.eBuffType.BT_SLOW, moveSpeed, 9.9f, this.lifeManager, 1);
		}
	}
	
	public override void OnChangeAbilityValue(float addValue)
	{
		base.OnChangeAbilityValue(addValue);
	}
	
	public override void OnResetMastery_New ()
	{
		base.OnResetMastery_New ();
		
		base.OnUpdateMastery_New ();
		
		AttributeManager attributeManager = null;
		MasteryManager_New masteryManager = null;
		if (lifeManager != null)
		{
			attributeManager = lifeManager.attributeManager;
			masteryManager = lifeManager.masteryManager_New;
		}
		
		List<MasteryInfo_New> masteryList = masteryManager != null ? masteryManager.totalList : null;
		
		if (masteryList != null)
		{
			string[] args = null;
			BaseState.eState targetState = BaseState.eState.None;
			CharStateInfo state = null;
			
			foreach(MasteryInfo_New temp in masteryList)
			{
				float masteryValue = temp.incValue * temp.curPoint;
				state = null;
				
				switch(temp.method)
				{
				case MasteryInfo_New.eMethodType.Assassin_01:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.CriticalHitRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_02:
				case MasteryInfo_New.eMethodType.Assassin_07:
				case MasteryInfo_New.eMethodType.Assassin_18:
				case MasteryInfo_New.eMethodType.Assassin_19:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
							state.stateInfo.addRequireAbility += masteryValue;
					}
					break;
				case MasteryInfo_New.eMethodType.Assassin_03:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncDamageWhenOverHP50,  masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_04:
				case MasteryInfo_New.eMethodType.Assassin_14:
				case MasteryInfo_New.eMethodType.Assassin_17:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
						{
							//state.stateInfo.addAttackRate -= masteryValue;
							foreach(CollisionInfo info in state.collisionInfoList)
								info.stateInfo.addAttackRate -= masteryValue;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Assassin_05:
				case MasteryInfo_New.eMethodType.Assassin_15:
				case MasteryInfo_New.eMethodType.Assassin_25:
					if (masteryManager.activeMastery != null && masteryManager.activeMastery.id == temp.id)
					{
						if (temp.curPoint > 0)
						{
							args = temp.methodArg.Split(';');
							//BaseState.eState startState = BaseState.ToState(args[0]);
							targetState = BaseState.ToState(args[1]);
							float requireValue = float.Parse(args[2]);
							float coolTime = float.Parse(args[3]);
							//string iconName = args[4];
							addFXActionBState = args[5];
							
							if (stateController != null && stateController.stateList != null)
								state = stateController.stateList.GetState(actionBStartState);
							if (state != null && state.stateInfo != null)
								state.stateInfo.requireAbilityValue = requireValue;
							
							if (temp.method == MasteryInfo_New.eMethodType.Assassin_05)
								this.actionBStartState = BaseState.eState.Evadestart;
							else
								this.actionBStartState = targetState;
							
							this.actionBState = targetState;
							this.actionBCoolTime = coolTime;
							this.actionBDelayTime = 0.0f;
						}
						else
						{
							this.actionBStartState = this.actionBState = BaseState.eState.None;
							this.actionBCoolTime = 0.0f;
							this.actionBDelayTime = 0.0f;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Assassin_06:
					if (attributeManager != null)
						attributeManager.SubValueRate(this.abilityRegenType, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_08:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.CriticalDamageRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_09:
					addActionBCoolTime += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_10:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncDamageOnWeek2, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_11:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncGainGold, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_12:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.AttackDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_13:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageWhenHP100, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_16:
					actionBCoolTimeAdjust += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_20:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncDamageOnPoisionByAction, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_21:
					addPoisonHitRate -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_22:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.AbilityPower, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_23:
					addAbilityValueRateByPoisonHit -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_24:
					poisonAttackRate -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_26:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncPoisonInfectRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_27:
					//addActionBCoolTime += masteryValue;
					actionBCoolTimeAdjust += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_28:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.DecAttackDamageOnPoison, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_29:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAbilityGainRateUnderHP35, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_30:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.DecMoveSpeedWhenPoison, masteryValue);
					break;
				}
			}
		}
		
		attributeManager.UpdateAbilityUI(AttributeValue.eAttributeType.Vital);
	}
	
	public override void OnUpdateMastery_New ()
	{
		base.OnUpdateMastery_New ();
		
		AttributeManager attributeManager = null;
		MasteryManager_New masteryManager = null;
		if (lifeManager != null)
		{
			attributeManager = lifeManager.attributeManager;
			masteryManager = lifeManager.masteryManager_New;
		}
		
		List<MasteryInfo_New> masteryList = masteryManager != null ? masteryManager.totalList : null;
		
		if (masteryList != null)
		{
			string[] args = null;
			BaseState.eState targetState = BaseState.eState.None;
			CharStateInfo state = null;
			
			foreach(MasteryInfo_New temp in masteryList)
			{
				float masteryValue = temp.incValue * temp.curPoint;
				state = null;
				
				switch(temp.method)
				{
				case MasteryInfo_New.eMethodType.Assassin_01:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.CriticalHitRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_02:
				case MasteryInfo_New.eMethodType.Assassin_07:
				case MasteryInfo_New.eMethodType.Assassin_18:
				case MasteryInfo_New.eMethodType.Assassin_19:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
							state.stateInfo.addRequireAbility -= masteryValue;
					}
					break;
				case MasteryInfo_New.eMethodType.Assassin_03:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageWhenOverHP50,  masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_04:
				case MasteryInfo_New.eMethodType.Assassin_14:
				case MasteryInfo_New.eMethodType.Assassin_17:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
						{
							//state.stateInfo.addAttackRate += masteryValue;
							foreach(CollisionInfo info in state.collisionInfoList)
								info.stateInfo.addAttackRate += masteryValue;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Assassin_05:
				case MasteryInfo_New.eMethodType.Assassin_15:
				case MasteryInfo_New.eMethodType.Assassin_25:
					if (masteryManager.activeMastery != null && masteryManager.activeMastery.id == temp.id)
					{
						if (temp.curPoint > 0)
						{
							args = temp.methodArg.Split(';');
							BaseState.eState startStage = BaseState.ToState(args[0]);
							targetState = BaseState.ToState(args[1]);
							float requireValue = float.Parse(args[2]);
							float coolTime = float.Parse(args[3]);
							string iconName = args[4];
							addFXActionBState = args[5];
							
							if (this.myInfo.actorType == ActorInfo.ActorType.Player && playerControls != null)
							{
								playerControls.SetEnableActionBButton(true);
								playerControls.SetActionBIcon(iconName);
							}
							
							this.actionBStartState = startStage;
							this.actionBState = targetState;
							
							if (stateController != null && stateController.stateList != null)
								state = stateController.stateList.GetState(actionBStartState);
							if (state != null && state.stateInfo != null)
								state.stateInfo.requireAbilityValue = requireValue;
							
							this.actionBCoolTime = coolTime;
							this.actionBDelayTime = 0.0f;
						}
						else
						{
							if (playerControls != null)
								playerControls.SetEnableActionBButton(false);
							
							addFXActionBState = "";
							
							this.actionBStartState = this.actionBState = BaseState.eState.None;
							this.actionBCoolTime = 0.0f;
							this.actionBDelayTime = 0.0f;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Assassin_06:
					if (attributeManager != null)
						attributeManager.AddValueRate(this.abilityRegenType, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_08:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.CriticalDamageRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_09:
					addActionBCoolTime -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_10:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageOnWeek2, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_11:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncGainGold, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_12:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.AttackDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_13:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageWhenHP100, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_16:
					actionBCoolTimeAdjust -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_20:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageOnPoisionByAction, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_21:
					addPoisonHitRate += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_22:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.AbilityPower, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_23:
					addAbilityValueRateByPoisonHit += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_24:
					poisonAttackRate += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_26:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncPoisonInfectRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_27:
					actionBCoolTimeAdjust -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Assassin_28:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.DecAttackDamageOnPoison, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_29:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAbilityGainRateUnderHP35, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Assassin_30:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.DecMoveSpeedWhenPoison, masteryValue);
					break;
				}
			}
		}
		
		attributeManager.UpdateAbilityUI(AttributeValue.eAttributeType.Vital);
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		switch(info.baseState.state)
		{
		case BaseState.eState.Evadecounterattack:
			actionBDelayTime += addActionBCoolTime;
			break;
		}
		
		lifeManager.attackStateInfo.abilityPower = 0.0f;
	}
	
	public override void OnEndState()
	{
		switch(stateController.currentState)
		{
		case BaseState.eState.AttackB_2:
		case BaseState.eState.AttackB_3:
			if (moveController != null)
			{
				moveController.ignoreMonsterBody = false;
				moveController.SetJumpCollider(false, false);
			}
			break;
		}
		
		base.OnEndState();
	}
	
	public override void OnCollisionStart()
	{
		base.OnCollisionStart();
		
		lifeManager.attackStateInfo.abilityPower = 0.0f;
	}
	
	public override string GetCurMasteryInfo_New(MasteryInfo_New info)
	{
		string infoStr = base.GetCurMasteryInfo_New(info);
		
		//AttributeManager attributeManager = null;
		MasteryManager_New masteryManager = null;
		if (lifeManager != null)
		{
			//attributeManager = lifeManager.attributeManager;
			masteryManager = lifeManager.masteryManager_New;
		}
		
		if (info == null || info.manager == null ||
			info.Point > info.maxPoint)
			return infoStr;
		
		string[] args = null;
		BaseState.eState targetState = BaseState.eState.None;
		CharStateInfo state = null;
		
		float masteryValue = info.incValue * info.Point;
		if (info.unitString == "%")
			masteryValue *= 100.0f;
		
		state = null;
		
		MasteryInfo_New addMastery = null;
		float baseValue = 0.0f;
		
		switch(info.method)
		{
		case MasteryInfo_New.eMethodType.Assassin_01:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_02:
		case MasteryInfo_New.eMethodType.Assassin_07:
			args = info.methodArg.Split(';');
			foreach(string str in args)
			{
				targetState = BaseState.ToState(str);
				
				if (stateController != null && stateController.stateList != null)
					state = stateController.stateList.GetState(targetState);
				if (state != null && state.stateInfo != null)
					masteryValue = state.stateInfo.requireAbilityValue - masteryValue;
			}
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_03:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_04:
		case MasteryInfo_New.eMethodType.Assassin_14:
		case MasteryInfo_New.eMethodType.Assassin_17:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_05:
		case MasteryInfo_New.eMethodType.Assassin_15:
		case MasteryInfo_New.eMethodType.Assassin_25:
			infoStr = info.formatString;
			break;
		case MasteryInfo_New.eMethodType.Assassin_06:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_08:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_09:
			addMastery = masteryManager.GetMastery(35);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_10:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_11:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_12:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_13:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_16:
			addMastery = masteryManager.GetMastery(45);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_18:
			addMastery = masteryManager.GetMastery(49);
			baseValue = 0.0f;
			if (addMastery != null)
				baseValue = addMastery.incValue * addMastery.Point;
			
			args = info.methodArg.Split(';');
			foreach(string str in args)
			{
				targetState = BaseState.ToState(str);
				
				if (stateController != null && stateController.stateList != null)
					state = stateController.stateList.GetState(targetState);
				if (state != null && state.stateInfo != null)
					masteryValue = state.stateInfo.requireAbilityValue - (masteryValue + baseValue);
			}
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_19:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_20:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_21:
			masteryValue += basePoisonHitRate * 100.0f;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_22:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_23:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_24:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_26:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_27:
			addMastery = masteryManager.GetMastery(55);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_28:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_29:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_30:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		}
		
		return infoStr;
	}
	
	public override string GetNextMasteryInfo_New(MasteryInfo_New info)
	{
		string infoStr = base.GetNextMasteryInfo_New(info);
		
		//AttributeManager attributeManager = null;
		MasteryManager_New masteryManager = null;
		if (lifeManager != null)
		{
			//attributeManager = lifeManager.attributeManager;
			masteryManager = lifeManager.masteryManager_New;
		}
		
		if (info == null || info.manager == null ||
			info.Point >= info.maxPoint)
			return infoStr;
		
		string[] args = null;
		BaseState.eState targetState = BaseState.eState.None;
		CharStateInfo state = null;
		
		float masteryValue = (info.incValue * info.Point) + info.incValue;
		if (info.unitString == "%")
			masteryValue *= 100.0f;
		
		state = null;
		
		MasteryInfo_New addMastery = null;
		float baseValue = 0.0f;
		
		switch(info.method)
		{
		case MasteryInfo_New.eMethodType.Assassin_01:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_02:
		case MasteryInfo_New.eMethodType.Assassin_07:
			args = info.methodArg.Split(';');
			foreach(string str in args)
			{
				targetState = BaseState.ToState(str);
				
				if (stateController != null && stateController.stateList != null)
					state = stateController.stateList.GetState(targetState);
				if (state != null && state.stateInfo != null)
					masteryValue = state.stateInfo.requireAbilityValue - masteryValue;
			}
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_03:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_04:
		case MasteryInfo_New.eMethodType.Assassin_14:
		case MasteryInfo_New.eMethodType.Assassin_17:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_05:
		case MasteryInfo_New.eMethodType.Assassin_15:
		case MasteryInfo_New.eMethodType.Assassin_25:
			infoStr = "";
			break;
		case MasteryInfo_New.eMethodType.Assassin_06:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_08:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_09:
			addMastery = masteryManager.GetMastery(35);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_10:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_11:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_12:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_13:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_16:
			addMastery = masteryManager.GetMastery(45);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_18:
			addMastery = masteryManager.GetMastery(49);
			baseValue = 0.0f;
			if (addMastery != null)
				baseValue = addMastery.incValue * addMastery.Point;
			
			args = info.methodArg.Split(';');
			foreach(string str in args)
			{
				targetState = BaseState.ToState(str);
				
				if (stateController != null && stateController.stateList != null)
					state = stateController.stateList.GetState(targetState);
				if (state != null && state.stateInfo != null)
					masteryValue = state.stateInfo.requireAbilityValue - (masteryValue + baseValue);
			}
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_19:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_20:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_21:
			masteryValue += basePoisonHitRate * 100.0f;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_22:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_23:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_24:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_26:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_27:
			addMastery = masteryManager.GetMastery(55);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_28:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_29:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Assassin_30:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		}
		
		return infoStr;
	}
	
	public override void FireProjectile()
	{
		Debug.Log("Assassin.. fire Projectile....");
		
		BaseState.eState curState = stateController.currentState;
		switch(curState)
		{
		case BaseState.eState.AttackB_2:
			ThrowDagger();
			break;
		case BaseState.eState.AttackB_3:
			DropBomb();
			break;
		}
		
	}
	
	public string arrowPrefabPath = "NewAsset/Others/Assassin_Dagger_Arrow";
	public Transform projectileStart = null;
	public void ThrowDagger()
	{
		Arrow arrow = ResourceManager.CreatePrefab<Arrow>(arrowPrefabPath);
        if (arrow == null) return;

        Vector3 vCreatePos = Vector3.zero;
        vCreatePos = projectileStart.position;
        vCreatePos.z = 0.0f;
		vCreatePos.x = this.transform.position.x;
		
		arrow.transform.position = vCreatePos;

        Vector3 vMoveDir = this.moveController.moveDir;
		
		arrow.MoveDir = vMoveDir;
        
		arrow.SetOwnerActor(lifeManager);
		arrow.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
        arrow.SetFired();
		
		/*
		float basePoisionAttack = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower) * 2.0f;
		arrow.AddBuff(1.0f, GameDef.eBuffType.BT_POISION, basePoisionAttack, 9.9f);
		*/
	}
	
	public string bombPrefabPath = "NewAsset/Others/PoisonBomb";
	public StateInfo bombStateInfo = new StateInfo();
	public void DropBomb()
	{
		PoisonBomb bomb = ResourceManager.CreatePrefab<PoisonBomb>(bombPrefabPath);
		if (bomb != null)
		{
			bomb.calcPoisionAttack = new PoisonBomb.CalcPoisionAttack(CalcPoisionAttack);
			
			bomb.SetMoveDir(this.moveController.moveDir);
			
			AttackStateInfo newAttackInfo = new AttackStateInfo();
			
			bombStateInfo.attackRate = 1.0f;
			
			newAttackInfo.attackState = BaseState.eState.AttackB_3;
			newAttackInfo.attackDamage = 0.0f;
			newAttackInfo.abilityPower = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower) * 2.0f;
			
			newAttackInfo.SetOwnerActor(lifeManager);
			newAttackInfo.SetState(bombStateInfo);
			
			bomb.SetAttackInfo(newAttackInfo);
			bomb.SetOwnerActor(lifeManager);
			
			bomb.gameObject.transform.position = this.gameObject.transform.position;
			
			float basePoisionAttack = CalcPoisionAttack();
			bomb.AddBuff(1.0f, GameDef.eBuffType.BT_POISION, basePoisionAttack, 9.9f);
		}
	}
	
	public override void OnWalkingStart()
	{
		base.OnWalkingStart();
		
		BaseState.eState curState = stateController.currentState;
		switch(curState)
		{
		case BaseState.eState.AttackB_2:
		case BaseState.eState.AttackB_3:
			if (moveController != null)
			{
				moveController.ignoreMonsterBody = true;
				moveController.SetJumpCollider(false, true);
			}
			break;
		}
	}
	
	public override void OnWalkingStop()
	{
		base.OnWalkingStop();
		
		BaseState.eState curState = stateController.currentState;
		switch(curState)
		{
		case BaseState.eState.AttackB_2:
		case BaseState.eState.AttackB_3:
			if (moveController != null)
			{
				moveController.ignoreMonsterBody = false;
				moveController.SetJumpCollider(false, false);
			}
			break;
		}
	}
	
	public override void OnRevivalSub()
	{
		AttributeValue vitalValue = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Vital);
		AttributeValue vitalMaxValue = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.VitalMax);
		
		vitalValue.baseValue = vitalMaxValue.Value;
		lifeManager.attributeManager.UpdateValue(vitalValue);
	}
}
