using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerWizard : PlayerController {
	public string iceArrowPrefabPath = "NewAsset/Others/IceArrow";
	public string rubblePrefabPath = "NewAsset/Others/Rubble";
	public string iceTornadoPrefabPath = "NewAsset/Others/IceTornado";
	
	public Transform projectileStart = null;
	
	public float teleportLength = 4.0f;
	
	public float slowBuffRate = 1.0f;
	public float baseSlowRate = 0.8f;
	public float baseSlowTime = 1.0f;
	public float addSlowTime = 0.0f;
	
	public float addHeavyAttackRate = 0.0f;
	
	public float manaShieldBuffTime = 2.0f;
	public float baseManaShieldRate = 0.5f;
	public float addManaShieldRate = 0.0f;
	
	public float addAbilityRateOnManaShield = 0.0f;
	
	public float startAbilityValue = 600.0f;
	
	public float incAbilityValue = 10.0f;
	public float incAbilityTime = 5.0f;
	
	public float incAbilityCoolTime = 1.0f;
	public float incAbilityDelayTime = 0.0f;
	public float incAbilityDeltaValue = 0.0f;
	
	public float baseRubbleRate = 0.0f;
	public float addRubbleRate = 0.0f;
	public float rubbleAttackRate = 2.5f;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		incAbilityDelayTime = incAbilityCoolTime;
		incAbilityDeltaValue = incAbilityValue / incAbilityTime * incAbilityCoolTime;
		
		if (this.lifeManager != null)
		{
			//lifeManager.onTargetHit = new LifeManager.OnTargetHit(OnTargetHit);
			
			lifeManager.onMansShieldBroken = new LifeManager.OnManaShieldBroken(OnManaShieldBroken);
			
			lifeManager.onApplyReflectDamage = new LifeManager.OnApplyReflectDamage(OnApplyReflectDamage);
		}
		
		Transform[] bones = GetComponentsInChildren<Transform>();
        foreach (Transform bone in bones)
        {
            if (bone.name == "Shot")
			{
				projectileStart = bone;
				break;
			}
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
		
		AttributeManager attributeManager = lifeManager.attributeManager;
		
		if (initData != null)
		{
			AttributeValue[] attributes = {
				new AttributeValue(AttributeValue.eAttributeType.Mana, initData.manaMax, 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.ManaMax, initData.manaMax, incData != null ? incData.manaMax : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.ManaRegen, initData.manaRegen, incData != null ? incData.manaRegen : 0.0f, 0.0f),
			};
			
			foreach(AttributeValue initValue in attributes)
			{
				attributeManager.basicAttributeTypeList.Add(initValue.valueType);
				attributeManager.AddAttributeValue(initValue);
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
			
			MasteryInfo[] initValues = { 
				new MasteryInfo(MasteryInfo.eMasteries.Teleport, -2, 0),
				new MasteryInfo(MasteryInfo.eMasteries.BrokenPieceOfIce, 0.025f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.Cohesive, 0.03f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.Protection, 0.03f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.ColdWave, 0.02f, 0), 
				new MasteryInfo(MasteryInfo.eMasteries.Frostbite, 0.05f, 0), 
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
				if (hpRate <= 0.5f)
					addRate = attributeMgr.GetAttributeValue(AttributeValue.eAttributeType.IncAbilityGainRateUnderHP50);
				
				//5초동안 채워지는 양이라 1초 간격으로 추가되어 0.2를 곱한다..
				float addAbilityValue = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.ManaRegen) * 0.2f;
				float addValue = addAbilityValue + (addAbilityValue * addRate);
				OnChangeAbilityValue(addValue);
			}
		}
	}
	
	public void DefaultIceArrow()
	{
		Debug.Log("Wizard.. fire Projectile....");
		
		IceArrow arrow = ResourceManager.CreatePrefab<IceArrow>(iceArrowPrefabPath);
        if (arrow == null) return;

        Vector3 vCreatePos = Vector3.zero;
        vCreatePos = projectileStart.position;
        
        arrow.transform.position = vCreatePos;

        Vector3 vMoveDir = this.moveController.moveDir;
		
		arrow.MoveDir = vMoveDir;
        //arrow.lookDir = vMoveDir;
		
		if (stateController.currentState == BaseState.eState.Heavyattack)
			arrow.IceArrowType = IceArrow.eIceArrowType.SuperArrow;
		
		arrow.SetOwnerActor(lifeManager);
		arrow.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
        arrow.SetFired();
		
		arrow.AddBuff(slowBuffRate, GameDef.eBuffType.BT_SLOW, baseSlowRate, baseSlowTime + addSlowTime);
	}
	
	public StateInfo iceTornadoStateInfo = new StateInfo();
	public string iceTornadoFXName = "";
	public float iceTornadoFXScale = 1.0f;
	public eFXEffectType iceTornadoFXType = eFXEffectType.ScaleNode;
	public float iceTornadoFXDelaytime = 0.5f;
	public void MakeIceTornado()
	{
		IceTornado iceTornado = ResourceManager.CreatePrefab<IceTornado>(iceTornadoPrefabPath);
		if (iceTornado != null)
		{
			if (moveController != null && moveController.root != null)
				iceTornado.gameObject.transform.parent = moveController.root.transform;
			else
				iceTornado.gameObject.transform.parent = this.gameObject.transform;
			
			iceTornado.gameObject.transform.localPosition = Vector3.zero;
			
			iceTornado.SetMoveDir(moveController.moveDir);
			
			AttackStateInfo newAttackInfo = new AttackStateInfo();
			
			iceTornadoStateInfo.attackRate = 1.0f;
			
			newAttackInfo.attackDamage = 0.0f;
			newAttackInfo.abilityPower = lifeManager.GetAbilityPower();
			
			newAttackInfo.SetOwnerActor(lifeManager);
			newAttackInfo.SetState(iceTornadoStateInfo);
			
			iceTornado.SetAttackInfo(newAttackInfo);
			iceTornado.SetOwnerActor(lifeManager);
			
			iceTornado.AddBuff(1.0f, GameDef.eBuffType.BT_SLOW, baseSlowRate, baseSlowTime + addSlowTime);
			
			if (lifeManager != null && lifeManager.attributeManager != null)
			{
				float incTempValue = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.DecDamageOnBuff);
				if (incTempValue != 0.0f)
					lifeManager.buffManager.AddBuff(GameDef.eBuffType.BT_DEC_DAMAGE, 0.0f, iceTornado.lifeTime, this.lifeManager, 1);
			}
					
			lifeManager.AddFXDelayInfo(iceTornadoFXName, iceTornadoFXType, iceTornadoFXScale, iceTornadoFXDelaytime);
		}
	}
	
	public override void FireProjectile()
	{
		BaseState.eState curState = stateController.currentState;
		switch(curState)
		{
		case BaseState.eState.AttackB_2:
			MakeIceTornado();
			break;
		case BaseState.eState.AttackB_3:
			int buffIndex = lifeManager.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_REFLECTDAMAGE, lifeManager);
			if (buffIndex != -1)
				lifeManager.buffManager.RemoveBuff(buffIndex);
			
			lifeManager.buffManager.AddBuff(GameDef.eBuffType.BT_REFLECTDAMAGE, 0.0f, 2.0f, lifeManager, 1);
			break;
		default:
			DefaultIceArrow();
			break;
		}
	}
	
	public override void OnCollisionStop()
	{
		base.OnCollisionStop();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.AttackB_1:
			DoTeleport();
			break;
		case BaseState.eState.Skill02:
			ApplyManaShield();
			break;
		}
	}
	
	public override void OnTargetHit(LifeManager hitActor, float damage, bool isCritical, AttackStateInfo attackInfo)
	{
		switch(attackInfo.attackState)
		{
		case BaseState.eState.Dashattack:
		case BaseState.eState.AttackB_1:
		case BaseState.eState.Skill02:
			int rateValue = Mathf.RoundToInt(slowBuffRate * 100.0f);
			int buffRandValue = Random.Range(0, 100);
			
			if (rateValue < buffRandValue)
				return;
			
			ApplyTargetSlowBuff(hitActor, baseSlowRate, baseSlowTime + addSlowTime);
			break;
		case BaseState.eState.Skill01:
			
			ApplyTargetSlowBuff(hitActor, baseSlowRate, baseSlowTime + addSlowTime);
			break;
		}
		
		base.OnTargetHit(hitActor, damage, isCritical, attackInfo);
	}
	
	public void ApplyTargetSlowBuff(LifeManager hitActor, float slowRate, float slowTime)
	{
		BuffManager buffMgr = null;
		if (hitActor != null)
			buffMgr = hitActor.buffManager;
		
		if (buffMgr != null)
		{
			int index = buffMgr.GetAppliedBuffIndex(GameDef.eBuffType.BT_SLOW, lifeManager);
			if (index != -1)
				buffMgr.RemoveBuff(index);
			
			buffMgr.AddBuff(GameDef.eBuffType.BT_SLOW, slowRate, slowTime, lifeManager, 1);
		}
	}
	
	private void DoTeleport()
	{
		float teleportDist = teleportLength;
			
		Vector3 moveDir = moveController.moveDir;
		
		int groundMask = 1 << LayerMask.NameToLayer("Ground");
		int obstacleMask = 1 << LayerMask.NameToLayer("Obstacle");
		
		RaycastHit hit;
		Vector3 vStartPos = gameObject.transform.position;
		if (Physics.Raycast(vStartPos, -moveDir, out hit, teleportLength + moveController.colliderRadius, groundMask) == true)
		{
			teleportDist = hit.distance - moveController.colliderRadius;
		}
		
		Vector3 targetPos = this.transform.position - (moveDir * teleportDist);
		
		Vector3 testPos1 = targetPos - (moveDir * moveController.colliderRadius);
		Vector3 testPos2 = targetPos + (moveDir * moveController.colliderRadius);
		if (Physics.Raycast(testPos1 + (Vector3.up * 7.0f), Vector3.down, out hit, float.MaxValue, obstacleMask) == true ||
			Physics.Raycast(testPos2 + (Vector3.up * 7.0f), Vector3.down, out hit, float.MaxValue, obstacleMask) == true)
		{
			Bounds bound = hit.collider.bounds;
			
			if (targetPos.x <= bound.center.x)
			{
				targetPos.x = bound.center.x - (bound.extents.x + moveController.colliderRadius + moveController.skinWidth);
			}
			else
			{
				targetPos.x = bound.center.x + (bound.extents.x + moveController.colliderRadius + moveController.skinWidth);
			}
		}
		
		this.transform.position = targetPos;
	}
	
	private void ApplyManaShield()
	{
		BuffManager buffManager = null;
		if (lifeManager != null)
			buffManager = lifeManager.buffManager;
		
		if (buffManager == null)
			return;
		
		int index = buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_MANASHIELD, lifeManager);
		if (index != -1)
			buffManager.RemoveBuff(index);
		
		float manaShieldValue = 0.0f;
		if (lifeManager != null && lifeManager.attributeManager != null)
		{
			float abilityPower = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
			float addRate = baseManaShieldRate + addManaShieldRate;
			manaShieldValue = abilityPower * addRate;
		}
		
		buffManager.AddBuff(GameDef.eBuffType.BT_MANASHIELD, manaShieldValue, manaShieldBuffTime, lifeManager, 1);

		float incAbilityPowerRate = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAbilityPowerWhenManaShield);
		if (incAbilityPowerRate != 0.0f)
			lifeManager.attributeManager.AddValueRate(AttributeValue.eAttributeType.AbilityPower, incAbilityPowerRate);
	}
	
	public StateInfo rubbleStateInfo = new StateInfo();
	public string rubbleFXName = "";
	public float rubbleFXScale = 1.0f;
	public eFXEffectType rubbleFXType = eFXEffectType.ScaleNode;
	public float rubbleFXDelaytime = 0.5f;
	
	public void OnManaShieldBroken()
	{
		CharStateInfo skill2Info = stateController.stateList.GetState(BaseState.eState.Skill02);
		if (skill2Info != null && skill2Info.stateInfo != null)
		{
			float useManaValue =GetRequireAbilityValue(skill2Info.stateInfo);
			float recoverValue = 0.0f;
			
			float recoverRate = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.RecoverUseAbiliyValue);
			if (recoverRate != 0.0f)
				recoverValue = useManaValue * recoverRate;
			
			OnChangeAbilityValue(recoverValue);
		}
		
		Debug.Log("Rubble...................");
		
		float rubbleRate = baseRubbleRate + addRubbleRate;
		if (rubbleRate == 0.0f)
			return;
		
		int randValue = Random.Range(0, 100);
		int rateValue = Mathf.RoundToInt(rubbleRate * 100.0f);
		if (rateValue < randValue)
			return;
		
		Rubble rubble = ResourceManager.CreatePrefab<Rubble>(rubblePrefabPath);
		if (rubble != null)
		{
			AttackStateInfo newAttackInfo = new AttackStateInfo();
			/*
			StateInfo tempSateInfo = new StateInfo();
			tempSateInfo.attackRate = rubbleAttackRate;
			tempSateInfo.attackState = StateInfo.eAttackState.AS_DAMAGE;
			tempSateInfo.attackType = StateInfo.eAttackType.AT_DISABLEAVOID;
			tempSateInfo.painValue = 900.0f;
			*/
			
			rubbleStateInfo.attackRate = rubbleAttackRate;
			
			newAttackInfo.attackDamage = 0.0f;
			newAttackInfo.abilityPower = lifeManager.GetAbilityPower();
			
			newAttackInfo.SetOwnerActor(lifeManager);
			newAttackInfo.SetState(rubbleStateInfo);
			
			rubble.SetAttackInfo(newAttackInfo);
			rubble.SetOwnerActor(lifeManager);
			
			rubble.AddBuff(1.0f, GameDef.eBuffType.BT_SLOW, baseSlowRate, baseSlowTime + addSlowTime);
			
			rubble.gameObject.transform.position = this.gameObject.transform.position;
			
			lifeManager.AddFXDelayInfo(rubbleFXName, rubbleFXType, rubbleFXScale, rubbleFXDelaytime);
		}
	}
	
	public override void OnResetMastery_New ()
	{
		base.OnResetMastery_New ();
		
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
				case MasteryInfo_New.eMethodType.Wizard_01:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.AbilityPower, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_02:
				case MasteryInfo_New.eMethodType.Wizard_03:
				case MasteryInfo_New.eMethodType.Wizard_04:
				case MasteryInfo_New.eMethodType.Wizard_06:
				case MasteryInfo_New.eMethodType.Wizard_07:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
						{
							foreach(CollisionInfo info in state.collisionInfoList)
								info.stateInfo.addAttackRate -= masteryValue;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_05:
				case MasteryInfo_New.eMethodType.Wizard_15:
				case MasteryInfo_New.eMethodType.Wizard_25:
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
							
							this.actionBStartState = this.actionBState = targetState;
							this.actionBCoolTime = coolTime;
							this.actionBDelayTime = 0.0f;
							
							if (stateController != null && stateController.stateList != null)
								state = stateController.stateList.GetState(actionBStartState);
							if (state != null && state.stateInfo != null)
								state.stateInfo.requireAbilityValue = requireValue;
							
						}
						else
						{
							this.actionBStartState = this.actionBState = BaseState.eState.None;
							this.actionBCoolTime = 0.0f;
							this.actionBDelayTime = 0.0f;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_08:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
						{
							//state.stateInfo.addCriticalHitRate += masteryValue;
							foreach(CollisionInfo info in state.collisionInfoList)
								info.stateInfo.addCriticalHitRate -= masteryValue;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_09:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncDamageOnLowMana, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_10:
					if (attributeManager != null)
						attributeManager.SubValueRate(AttributeValue.eAttributeType.MagicPenetration, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_11:
					if (attributeManager != null)
						attributeManager.SubValueRate(this.abilityRegenType, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_12:
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
				case MasteryInfo_New.eMethodType.Wizard_13:
					actionBCoolTimeAdjust += masteryValue;
					skill1CoolTimeAdjust += masteryValue;
					skill2CoolTimeAdjust += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_14:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
							state.stateInfo.addRequireAbilityRate += masteryValue;
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_16:
					actionBCoolTimeAdjust += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_17:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.RecoverUseAbiliyValue, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_18:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.DecDamageOnBuff, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_19:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAbilityGainRateUnderHP50, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_20:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.RegenHPWhenRecoverAbility, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_21:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncGainExp, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_22:
					if (attributeManager != null)
					{

						float abilityPower = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
						float addValue = abilityPower * masteryValue;
						
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.Armor, 0.0f);
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.MagicResist, 0.0f);
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_23:
					if (attributeManager != null)
					{
						float abilityPower = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
						float addMaxHP = abilityPower * masteryValue;
						
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.HealthMax, 0.0f);
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_24:
					addSlowTime -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_26:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncDamageOnSlow, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_27:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncReflectDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_28:
					addManaShieldRate -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_29:
					addRubbleRate -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_30:
					if (attributeManager != null)
						attributeManager.SubValue(AttributeValue.eAttributeType.IncAbilityPowerWhenManaShield, masteryValue);
					break;
				}
			}
		}
		
		attributeManager.UpdateAbilityUI(AttributeValue.eAttributeType.Mana);
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
				float masteryValue = temp.incValue * temp.Point;
				state = null;
				
				switch(temp.method)
				{
				case MasteryInfo_New.eMethodType.Wizard_01:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.AbilityPower, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_02:
				case MasteryInfo_New.eMethodType.Wizard_03:
				case MasteryInfo_New.eMethodType.Wizard_04:
				case MasteryInfo_New.eMethodType.Wizard_06:
				case MasteryInfo_New.eMethodType.Wizard_07:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
						{
							foreach(CollisionInfo info in state.collisionInfoList)
								info.stateInfo.addAttackRate += masteryValue;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_05:
				case MasteryInfo_New.eMethodType.Wizard_15:
				case MasteryInfo_New.eMethodType.Wizard_25:
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
							
							this.actionBStartState = this.actionBState = targetState;
							this.actionBCoolTime = coolTime;
							this.actionBDelayTime = 0.0f;
							
							if (stateController != null && stateController.stateList != null)
								state = stateController.stateList.GetState(actionBStartState);
							if (state != null && state.stateInfo != null)
								state.stateInfo.requireAbilityValue = requireValue;
							
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
				case MasteryInfo_New.eMethodType.Wizard_08:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
						{
							//state.stateInfo.addCriticalHitRate += masteryValue;
							foreach(CollisionInfo info in state.collisionInfoList)
								info.stateInfo.addCriticalHitRate += masteryValue;
						}
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_09:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageOnLowMana, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_10:
					if (attributeManager != null)
						attributeManager.AddValueRate(AttributeValue.eAttributeType.MagicPenetration, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_11:
					if (attributeManager != null)
						attributeManager.AddValueRate(this.abilityRegenType, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_12:
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
				case MasteryInfo_New.eMethodType.Wizard_13:
					actionBCoolTimeAdjust -= masteryValue;
					skill1CoolTimeAdjust -= masteryValue;
					skill2CoolTimeAdjust -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_14:
					args = temp.methodArg.Split(';');
					foreach(string str in args)
					{
						targetState = BaseState.ToState(str);
						
						if (stateController != null && stateController.stateList != null)
							state = stateController.stateList.GetState(targetState);
						if (state != null && state.stateInfo != null)
							state.stateInfo.addRequireAbilityRate -= masteryValue;
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_16:
					actionBCoolTimeAdjust -= masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_17:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.RecoverUseAbiliyValue, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_18:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.DecDamageOnBuff, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_19:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAbilityGainRateUnderHP50, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_20:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.RegenHPWhenRecoverAbility, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_21:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncGainExp, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_22:
					if (attributeManager != null)
					{

						float abilityPower = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
						float addValue = abilityPower * masteryValue;
						
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.Armor, addValue);
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.MagicResist, addValue);
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_23:
					if (attributeManager != null)
					{
						float abilityPower = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
						float addMaxHP = abilityPower * masteryValue;
						
						attributeManager.SetMasteryValue(AttributeValue.eAttributeType.HealthMax, addMaxHP);
					}
					break;
				case MasteryInfo_New.eMethodType.Wizard_24:
					addSlowTime += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_26:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncDamageOnSlow, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_27:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncReflectDamage, masteryValue);
					break;
				case MasteryInfo_New.eMethodType.Wizard_28:
					addManaShieldRate += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_29:
					addRubbleRate += masteryValue;
					break;
				case MasteryInfo_New.eMethodType.Wizard_30:
					if (attributeManager != null)
						attributeManager.AddValue(AttributeValue.eAttributeType.IncAbilityPowerWhenManaShield, masteryValue);
					break;
				}
			}
		}
		
		attributeManager.UpdateAbilityUI(AttributeValue.eAttributeType.Mana);
	}
	
	public override void OnChangeState(CharStateInfo info)
	{
		base.OnChangeState(info);
		
		switch(info.baseState.state)
		{
		case BaseState.eState.Heavyattack:
			//lifeManager.attackStateInfo.addAttackPower = 0.0f;
			lifeManager.attackStateInfo.addAttackRate = addHeavyAttackRate;
			break;
		}
	}
	
	public override void OnChangeAbilityValue(float addValue)
	{
		base.OnChangeAbilityValue(addValue);
	}
	
	public string reflectPrefabPath = "NewAsset/Others/Reflect";
	public StateInfo reflectStateInfo = new StateInfo();
	public string reflectFXName = "";
	public float reflectFXScale = 1.0f;
	public eFXEffectType reflectFXType = eFXEffectType.ScaleNode;
	public float reflectFXDelaytime = 0.5f;
	public float reflectDamageRate = 1.0f;
	public float reflectMultiplyRate1 = 1.5f;
	public float reflectMultiplyRate2 = 0.8f;
	public void OnApplyReflectDamage(float reflectDamage)
	{
		Rubble rubble = ResourceManager.CreatePrefab<Rubble>(reflectPrefabPath);
		if (rubble != null)
		{
			AttackStateInfo newAttackInfo = new AttackStateInfo();
			
			rubbleStateInfo.attackRate = reflectDamageRate;
			newAttackInfo.attackDamage = 0.0f;
			newAttackInfo.abilityPower = lifeManager.GetAbilityPower() * reflectMultiplyRate1 + (reflectDamage * reflectMultiplyRate2);
			
			newAttackInfo.SetOwnerActor(lifeManager);
			newAttackInfo.SetState(reflectStateInfo);
			
			rubble.SetAttackInfo(newAttackInfo);
			rubble.SetOwnerActor(lifeManager);
			
			rubble.AddBuff(1.0f, GameDef.eBuffType.BT_SLOW, baseSlowRate, baseSlowTime + addSlowTime);
			
			rubble.gameObject.transform.position = this.gameObject.transform.position;
			
			lifeManager.AddFXDelayInfo(reflectFXName, reflectFXType, reflectFXScale, reflectFXDelaytime);
		}
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
		case MasteryInfo_New.eMethodType.Wizard_01:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_02:
		case MasteryInfo_New.eMethodType.Wizard_03:
		case MasteryInfo_New.eMethodType.Wizard_04:
		case MasteryInfo_New.eMethodType.Wizard_06:
		case MasteryInfo_New.eMethodType.Wizard_07:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_05:
		case MasteryInfo_New.eMethodType.Wizard_15:
		case MasteryInfo_New.eMethodType.Wizard_25:
			infoStr = info.formatString;
			break;
		case MasteryInfo_New.eMethodType.Wizard_08:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_09:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_10:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_11:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_12:
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
		case MasteryInfo_New.eMethodType.Wizard_13:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_14:
			masteryValue = 100.0f - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_16:
			addMastery = masteryManager.GetMastery(75);
			baseValue = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseValue = float.Parse(args[3]);
			}
			
			masteryValue = baseValue - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_17:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_18:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_19:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_20:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_21:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_22:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_23:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_24:
			masteryValue += baseSlowTime;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_26:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_27:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_28:
			masteryValue += 100.0f;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_29:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_30:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		}
		
		return infoStr;
	}
	
	public override string GetNextMasteryInfo_New(MasteryInfo_New info)
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
		case MasteryInfo_New.eMethodType.Wizard_01:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_02:
		case MasteryInfo_New.eMethodType.Wizard_03:
		case MasteryInfo_New.eMethodType.Wizard_04:
		case MasteryInfo_New.eMethodType.Wizard_06:
		case MasteryInfo_New.eMethodType.Wizard_07:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_05:
		case MasteryInfo_New.eMethodType.Wizard_15:
		case MasteryInfo_New.eMethodType.Wizard_25:
			infoStr = "";
			break;
		case MasteryInfo_New.eMethodType.Wizard_08:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_09:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_10:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_11:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_12:
			args = info.methodArg.Split(';');
			foreach(string str in args)
			{
				targetState = BaseState.ToState(str);
				
				if (stateController != null && stateController.stateList != null)
					state = stateController.stateList.GetState(targetState);
				if (state != null && state.stateInfo != null)
					masteryValue = this.GetRequireAbilityValue(state.stateInfo) - masteryValue;
			}
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_13:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_14:
			masteryValue = 100.0f - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_16:
			MasteryInfo_New addMastery = masteryManager.GetMastery(75);
			float baseCoolTime = 0.0f;
			if (addMastery != null)
			{
				args = addMastery.methodArg.Split(';');
				baseCoolTime = float.Parse(args[3]);
			}
			
			masteryValue = baseCoolTime - masteryValue;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_17:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_18:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_19:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_20:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_21:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_22:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_23:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_24:
			masteryValue += baseSlowTime;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_26:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_27:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_28:
			masteryValue += 100.0f;
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_29:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		case MasteryInfo_New.eMethodType.Wizard_30:
			infoStr = string.Format(info.formatString, masteryValue, info.unitString);
			break;
		}
		
		return infoStr;
	}
	
	public override void OnRevivalSub()
	{
		AttributeValue manaValue = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Mana);
		AttributeValue manMaxValue = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.ManaMax);
		
		manaValue.baseValue = manMaxValue.Value;
		lifeManager.attributeManager.UpdateValue(manaValue);
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
			case MasteryInfo_New.eMethodType.Wizard_22:
				if (attributeManager != null)
				{
					float addValue = abilityPower * masteryValue;
					
					attributeManager.SetMasteryValue(AttributeValue.eAttributeType.Armor, addValue);
					attributeManager.SetMasteryValue(AttributeValue.eAttributeType.MagicResist, addValue);
				}
				break;
			case MasteryInfo_New.eMethodType.Wizard_23:
				if (attributeManager != null)
				{
					float addMaxHP = abilityPower * masteryValue;
					
					attributeManager.SetMasteryValue(AttributeValue.eAttributeType.HealthMax, addMaxHP);
				}
				break;
			}
		}
	}
}
