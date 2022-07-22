using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerWarrior : PlayerController {
	public GameObject fxBerserk = null;
	
	public float abilityResetCoolTime = 2.0f;
	public float abilityResetDelay = 0.0f;
	
	public bool decAbilityMode = false;
	public float decAbilityValue = 5.0f;
	public float decAbilityCoolTime = 1.0f;
	public float decAbilityDelayTime = 0.0f;
	
	public float limitAbilityValue = 50.0f;
	public float berserkDeleteTime = 2.0f;
	
	public float startAbilitValue = 0.0f;
	public float startMaxAbilityValue = 100.0f;
	
	public float addAbilityByDefault = 3.0f;
	public float addAbilityByDamage = 9.0f;
	public float addAbilityByKnockDown = 15.0f;
	
	public float baseBerserkAttackRate = 0.1f;
	public float addBerserkAttackRate = 0.0f;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		if (this.lifeManager != null)
		{
			//lifeManager.onTargetHit = new LifeManager.OnTargetHit(OnTargetHit);
			//lifeManager.onDamage = new LifeManager.OnDamageDelegate(OnDamage);
		}
		
		if (fxBerserk != null)
			fxBerserk.SetActive(false);
		
		abilityResetDelay = abilityResetCoolTime;
		
		//ApplyMastery();
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
		
		AttributeManager attributeManager = lifeManager.attributeManager;
		
		if (initData != null)
		{
			AttributeValue[] attributes = {
				new AttributeValue(AttributeValue.eAttributeType.Rage, 0.0f, 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.RageMax, initData.rageMax, incData != null ? incData.rageMax : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.RageRegen, initData.rageRegen, incData != null ? incData.rageRegen : 0.0f, 0.0f),
			};
			
			foreach(AttributeValue initValue in attributes)
			{
				attributeManager.basicAttributeTypeList.Add(initValue.valueType);
				attributeManager.AddAttributeValue(initValue);
			}
		}
		
		return initData;
	}
	
	public override void Awake()
	{
		base.Awake();
		
		if (stateController != null)
		{
			CharStateInfo stateInfo =  stateController.stateList.GetState(BaseState.eState.Attack21);
			if (stateInfo != null)
			{
				CollisionInfo colInfo = stateInfo.collisionInfoList[0];
				if (colInfo != null)
					baseAttack21Rate = colInfo.stateInfo.attackRate + colInfo.stateInfo.addAttackRate;
			}
			
			stateInfo =  stateController.stateList.GetState(BaseState.eState.Skill02);
			if (stateInfo != null)
			{
				baseRequireSkill02 = stateInfo.stateInfo.requireAbilityValue;
			}
		}
	}
	
	// Update is called once per frame
	public override void Update () {
		base.Update();
		
		bool isDie = false;
		switch(stateController.currentState)
		{
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			isDie = true;
			break;
		default:
			isDie = false;
			break;
		}
		
		if (isDie == false)
			UpdateAbility();
	}
	
	public void UpdateAbility()
	{
		if (decAbilityMode == false)
		{
			abilityResetDelay -= Time.deltaTime;
			
			if (abilityResetDelay <= 0.0f)
			{
				abilityResetDelay = abilityResetCoolTime;
				
				decAbilityMode = true;
				decAbilityDelayTime = decAbilityCoolTime;
			}
		}
		else
		{
			decAbilityDelayTime -= Time.deltaTime;
			
			if (decAbilityDelayTime <= 0.0f)
			{
				decAbilityDelayTime = decAbilityCoolTime;
				
				float addAbilityValue = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.RageRegen);
				OnChangeAbilityValue(addAbilityValue * 0.2f);
			}
		}
	}
	
	public override void OnChangeAbilityValue(float addValue)
	{
		AttributeValue rageValue = lifeManager.attributeManager.GetAttribute(this.abilityValueType);
		AttributeValue rageMaxValue = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.RageMax);
		
		if (rageValue != null && rageMaxValue != null)
		{
			//float maxValue = rageMaxValue.Value;
			
			float beforeAbility = rageValue.Value;
			
			base.OnChangeAbilityValue(addValue);
			
			float afterAbility = rageValue.Value;
			
			//lifeManager.attributeManager.UpdateAbilityUI(this.abilityValueType);
			
			if (beforeAbility >= limitAbilityValue &&
				afterAbility < limitAbilityValue)
			{
				AddBerserkBuff(berserkDeleteTime);
			}
			else if (beforeAbility < limitAbilityValue &&
				afterAbility >= limitAbilityValue)
			{
				AddBerserkBuff(-1.0f);
			}
		}
	}
	
	public override void OnTargetHit(LifeManager hitActor, float damage, bool isCritical, AttackStateInfo attackInfo)
	{
		decAbilityMode = false;
		
		abilityResetDelay = abilityResetCoolTime;
		
		if (attackInfo != null && attackInfo.stateInfo != null)
		{
			float baseValue = attackInfo.stateInfo.acquireAbility + attackInfo.stateInfo.addAcquireAbility;
			float addRateValue = baseValue * attackInfo.stateInfo.addAcquireAbilityRate;
			
			float addAbilityValue = baseValue + addRateValue;
			
			if (isCritical == true)
				addAbilityValue *= 2.0f;
			
			OnChangeAbilityValue(addAbilityValue);
		}
		else
		{
			Debug.LogWarning("attackInfo is null......");
		}
		
		base.OnTargetHit(hitActor, damage, isCritical, attackInfo);
	}
	
	public override void OnDamage(AttackStateInfo attackInfo, Transform hitPos, LifeManager.eDamageType damageType)
	{
		if (lifeManager.GetHPRate() <= 0.0f)
			return;
		
		decAbilityMode = false;
		
		abilityResetDelay = abilityResetCoolTime;
		
		float addValue = 0.0f;
		switch(damageType)
		{
		case LifeManager.eDamageType.None:
			addValue = addAbilityByDefault;
			break;
		case LifeManager.eDamageType.Damge:
			addValue = addAbilityByDamage;
			break;
		case LifeManager.eDamageType.KnockDown:
			addValue = addAbilityByKnockDown;
			break;
		}
		
		float incRate = 0.0f;
		AttributeManager attributeManager = lifeManager != null ? lifeManager.attributeManager : null;
		if (attributeManager != null)
			incRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAbilityGainRateByDamage);
		
		if (incRate != 0.0f)
		{
			Debug.Log("OnDamage.. abilityValue incRate : " + incRate);
			addValue += addValue * incRate;
		}
		
		OnChangeAbilityValue(addValue);
		
		base.OnDamage(attackInfo, hitPos, damageType);
	}
	
	public void AddBerserkBuff(float buffTime)
	{
		BuffManager buffManager = null;
		
		if (lifeManager != null)
			buffManager = lifeManager.buffManager;
		
		if (buffManager != null)
		{
			int index = buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_BERSERK, lifeManager);
			if (index != -1)
				buffManager.RemoveBuff(index);
		
			float bererkAttackRate = baseBerserkAttackRate + addBerserkAttackRate;
			buffManager.AddBuff(GameDef.eBuffType.BT_BERSERK, bererkAttackRate, buffTime, lifeManager, 1);
		}
	}
	
	public override FXInfo SelectBuffFXObject(GameDef.eBuffType e)
	{
		FXInfo fxInfo = base.SelectBuffFXObject(e);
		
		if (fxInfo == null)
		{
			switch(e)
			{
			case GameDef.eBuffType.BT_BERSERK:
				fxInfo = new FXInfo();
				fxInfo.fxObject = fxBerserk;
				fxInfo.effectType = eFXEffectType.Toggle;
				break;
			}
		}
		
		return fxInfo;
	}
	
	public override void OnResetMastery_New()
	{
		base.OnResetMastery_New();
		
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
				case MasteryInfo_New.eMethodType.Warrior_01:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.CriticalHitRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_02:
				case MasteryInfo_New.eMethodType.Warrior_27:
				case MasteryInfo_New.eMethodType.Warrior_28:
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
				case MasteryInfo_New.eMethodType.Warrior_03:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncDamageWhenUnderHP50,  masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_04:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAbilityGainRateByDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_05:
				case MasteryInfo_New.eMethodType.Warrior_15:
				case MasteryInfo_New.eMethodType.Warrior_25:
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
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
							state.stateInfo.requireAbilityValue = requireValue;
						
						this.actionBStartState = this.actionBState = targetState;
						this.actionBCoolTime = coolTime;
						this.actionBDelayTime = 0.0f;
					}
					else
					{
						this.actionBStartState = this.actionBState = BaseState.eState.None;
						this.actionBCoolTime = 0.0f;
						this.actionBDelayTime = 0.0f;
					}
					break;
				case MasteryInfo_New.eMethodType.Warrior_06:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAttackDamageByAxe, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_07:
					actionBCoolTimeAdjust += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Warrior_08:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAttackDamageUnderHP35, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_09:
					addBerserkAttackRate -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Warrior_10:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.RageMax, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_11:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.AttackDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_12:
				case MasteryInfo_New.eMethodType.Warrior_16:
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
				case MasteryInfo_New.eMethodType.Warrior_13:
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
								info.stateInfo.addAttackRate = 0.0f;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Warrior_14:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
							state.stateInfo.addStunRate -= masteryValue;
					}
					break;
				case MasteryInfo_New.eMethodType.Warrior_17:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAttackDamageByHammer, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_18:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAttackDamageWhenStun, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_19:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncStunRateByHammer, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_20:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.ArmorPenetration, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_21:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncGainExp, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_22:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.HealthMax, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_23:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.HealthRegen, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_24:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.RegenHPWhenKnockdown, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_26:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.DecDamageWhenBerserk, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_29:
					if (attributeManager != null)
					{
						attributeManager.SubValueRate(AttributeValue.eAttributeType.Armor, masteryValue);
						attributeManager.SubValueRate(AttributeValue.eAttributeType.MagicResist, masteryValue);
					}
					break;
				case MasteryInfo_New.eMethodType.Warrior_30:
					if (attributeManager != null)
					{
						//attributeManager.SubValue(AttributeValue.eAttributeType.IncAttackDamageByArmor, masteryValue);
						float armorValue = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Armor);
						float addValue = armorValue * masteryValue;
						
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.AttackDamage, 0.0f);
					}
					break;
				}
			}
		}
		
		this.lifeManager.attributeManager.UpdateAbilityUI(AttributeValue.eAttributeType.Rage);
	}
	
	public override void OnUpdateMastery_New()
	{
		base.OnUpdateMastery_New();
		
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
				float masteryValue = temp.incValue * temp.Point;
				state = null;
				
				switch(temp.method)
				{
				case MasteryInfo_New.eMethodType.Warrior_01:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.CriticalHitRate, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_02:
				case MasteryInfo_New.eMethodType.Warrior_27:
				case MasteryInfo_New.eMethodType.Warrior_28:
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
				case MasteryInfo_New.eMethodType.Warrior_03:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageWhenUnderHP50,  masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_04:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAbilityGainRateByDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_05:
				case MasteryInfo_New.eMethodType.Warrior_15:
				case MasteryInfo_New.eMethodType.Warrior_25:
					if (masteryManager.activeMastery != null && masteryManager.activeMastery.id == temp.id)
					{
						if (temp.curPoint > 0)
						{
							args = temp.methodArg.Split(';');
							//BaseState.eState startState = BaseState.ToState(args[0]);
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
						
							if (stateController != null && stateController.stateList != null)
								state = stateController.stateList.GetState(targetState);
							if (state != null && state.stateInfo != null)
								state.stateInfo.requireAbilityValue = requireValue;
							
							this.actionBStartState = this.actionBState = targetState;
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
				case MasteryInfo_New.eMethodType.Warrior_06:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAttackDamageByAxe, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_07:
					actionBCoolTimeAdjust -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Warrior_08:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAttackDamageUnderHP35, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_09:
					addBerserkAttackRate += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Warrior_10:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.RageMax, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_11:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.AttackDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_12:
				case MasteryInfo_New.eMethodType.Warrior_16:
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
				case MasteryInfo_New.eMethodType.Warrior_13:
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
							{
								float addRate = info.stateInfo.attackRate * masteryValue;
								info.stateInfo.addAttackRate = addRate;
							}
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Warrior_14:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
							state.stateInfo.addStunRate += masteryValue;
					}
					break;
				case MasteryInfo_New.eMethodType.Warrior_17:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAttackDamageByHammer, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_18:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAttackDamageWhenStun, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_19:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncStunRateByHammer, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_20:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.ArmorPenetration, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_21:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncGainExp, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_22:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.HealthMax, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_23:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.HealthRegen, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_24:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.RegenHPWhenKnockdown, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_26:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.DecDamageWhenBerserk, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Warrior_29:
					if (attributeManager != null)
					{
						attributeManager.AddValueRate(AttributeValue.eAttributeType.Armor, masteryValue);
						attributeManager.AddValueRate(AttributeValue.eAttributeType.MagicResist, masteryValue);
					}
					break;
				case MasteryInfo_New.eMethodType.Warrior_30:
					if (attributeManager != null)
					{
						//attributeManager.AddValue(AttributeValue.eAttributeType.IncAttackDamageByArmor, masteryValue);
						float armorValue = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Armor);
						float addValue = armorValue * masteryValue;
						
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.AttackDamage, addValue);
					}
					break;
				}
			}
		}
		
		attributeManager.UpdateAbilityUI(AttributeValue.eAttributeType.Rage);
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		switch(info.baseState.state)
		{
		case BaseState.eState.Attack21:
			lifeManager.attackStateInfo.addAttackPower = 0.0f;
			lifeManager.attackStateInfo.addAttackRate = addAttack21Rate;
			break;
		}
	}
	
	public override string GetCurMasteryInfo_New(MasteryInfo_New info)
	{
		string infoStr = base.GetCurMasteryInfo_New(info);
		
		AttributeManager attributeManager = null;
		MasteryManager_New masteryManager = null;
		if (lifeManager != null)
		{
			attributeManager = lifeManager.attributeManager;
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
		case MasteryInfo_New.eMethodType.Warrior_01:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_02:
		case MasteryInfo_New.eMethodType.Warrior_27:
		case MasteryInfo_New.eMethodType.Warrior_28:
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
		case MasteryInfo_New.eMethodType.Warrior_03:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_04:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_05:
		case MasteryInfo_New.eMethodType.Warrior_15:
		case MasteryInfo_New.eMethodType.Warrior_25:
			infoStr = info.formatString;
			break;
		case MasteryInfo_New.eMethodType.Warrior_06:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_07:
			addMastery = masteryManager.GetMastery(5);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_08:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_09:
			masteryValue += 110.0f;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_10:
			AttributeValue rageMax = attributeManager.GetAttribute(AttributeValue.eAttributeType.RageMax);
			float fValue = 100.0f;
			if (rageMax != null)
				fValue = rageMax.baseValue;
			
			fValue += masteryValue;
			infoStr = string.Format(info.formatString, fValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_11:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_12:
		case MasteryInfo_New.eMethodType.Warrior_13:
		case MasteryInfo_New.eMethodType.Warrior_16:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_14:
			args = info.methodArg.Split(';');
			float fRateValue = 0.0f;
			foreach(string str in args)
			{
				targetState = BaseState.ToState(str);
				if (stateController != null && stateController.stateList != null)
					state = stateController.stateList.GetState(targetState);
				if (state != null && state.stateInfo != null)
					fRateValue = (state.stateInfo.stunRate * 100.0f) + masteryValue;
			}
			
			infoStr = string.Format(info.formatString, fRateValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_17:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_18:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_19:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_20:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_21:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_22:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_23:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_24:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_26:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_29:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_30:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		}

		return infoStr;
	}
	
	public override string GetNextMasteryInfo_New(MasteryInfo_New info)
	{
		string infoStr = base.GetNextMasteryInfo_New(info);
		
		AttributeManager attributeManager = null;
		MasteryManager_New masteryManager = null;
		if (lifeManager != null)
		{
			attributeManager = lifeManager.attributeManager;
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
		
		switch(info.method)
		{
		case MasteryInfo_New.eMethodType.Warrior_01:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_02:
		case MasteryInfo_New.eMethodType.Warrior_27:
		case MasteryInfo_New.eMethodType.Warrior_28:
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
		case MasteryInfo_New.eMethodType.Warrior_03:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_04:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_05:
		case MasteryInfo_New.eMethodType.Warrior_15:
		case MasteryInfo_New.eMethodType.Warrior_25:
			infoStr = "";//string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_06:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_07:
			MasteryInfo_New addMastery = masteryManager.GetMastery(5);
			float addValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				addValue = float.Parse(args[3]);
			}
			
			masteryValue = addValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_08:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_09:
			masteryValue += 110.0f;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_10:
			AttributeValue rageMax = attributeManager.GetAttribute(AttributeValue.eAttributeType.RageMax);
			float fValue = 100.0f;
			if (rageMax != null)
				fValue = rageMax.baseValue;
			
			fValue += masteryValue;
			infoStr = string.Format(info.formatString, fValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_11:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_12:
		case MasteryInfo_New.eMethodType.Warrior_13:
		case MasteryInfo_New.eMethodType.Warrior_16:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_14:
			args = info.methodArg.Split(';');
			float fRateValue = 0.0f;
			foreach(string str in args)
			{
				targetState = BaseState.ToState(str);
				if (stateController != null && stateController.stateList != null)
					state = stateController.stateList.GetState(targetState);
				if (state != null && state.stateInfo != null)
					fRateValue = (state.stateInfo.stunRate * 100.0f) + masteryValue;
			}
			
			infoStr = string.Format(info.formatString, fRateValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_17:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_18:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_19:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_20:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_21:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_22:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_23:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_24:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_26:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_29:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Warrior_30:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		}

		return infoStr;
	}
	
	public override void FireProjectile()
	{
		Debug.Log("Warrior.. fire Projectile....");
		
		BaseState.eState curState = stateController.currentState;
		switch(curState)
		{
		case BaseState.eState.AttackB_3:
			ShockWave();
			break;
		}
	}
	
	public string shockWavePrefabPath = "NewAsset/Others/ShockWave";
	public StateInfo shockWaveStateInfo = new StateInfo();
	public void ShockWave()
	{
		ShockWave shockWave = ResourceManager.CreatePrefab<ShockWave>(shockWavePrefabPath);
        if (shockWave == null) return;

        Vector3 vCreatePos = Vector3.zero;
        vCreatePos = transform.position;
        vCreatePos.z = 0.0f;
		
		shockWave.transform.position = vCreatePos;

        Vector3 vMoveDir = this.moveController.moveDir;
		
		shockWave.moveDir = vMoveDir;
		shockWaveStateInfo.attackRate = 1.0f;
		
		AttackStateInfo newAttackInfo = new AttackStateInfo();
		newAttackInfo.attackState = BaseState.eState.AttackB_3;
		newAttackInfo.attackDamage = lifeManager.GetAttackDamage();
		newAttackInfo.abilityPower = lifeManager.GetAbilityPower();
		
		newAttackInfo.SetOwnerActor(lifeManager);
		newAttackInfo.SetState(shockWaveStateInfo);
		
		shockWave.SetAttackInfo(newAttackInfo);
		shockWave.SetOwnerActor(lifeManager);
		
		shockWave.InitAttack();
	}
	
	public override void OnRevivalSub()
	{
		AttributeValue rageValue = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Rage);
		AttributeValue rageMaxValue = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.RageMax);
		
		rageValue.baseValue = rageMaxValue.Value * 0.5f;
		lifeManager.attributeManager.UpdateValue(rageValue);
	}
	
	public override void Post_UpdateAbilityData()
	{
		AttributeManager attributeManager = null;
		MasteryManager_New masteryManager = null;
		if (lifeManager != null)
		{
			attributeManager = lifeManager.attributeManager;
			masteryManager = lifeManager.masteryManager_New;
		}
		
		float abilityPower = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
		List<MasteryInfo_New> masteryList = masteryManager != null ? masteryManager.totalList : null;
		
		foreach(MasteryInfo_New temp in masteryList)
		{
			float masteryValue = temp.incValue * temp.Point;
			
			switch(temp.method)
			{
			case MasteryInfo_New.eMethodType.Warrior_30:
				if (attributeManager != null)
				{
					float armorValue = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Armor);
					float addValue = armorValue * masteryValue;
					
					attributeManager.SetMasteryValue(AttributeValue.eAttributeType.AttackDamage, addValue);
				}
				break;
			}
		}
	}
}
