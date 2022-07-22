using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour {
	public ActorInfo myInfo = null;
	
	public GameDef.ePlayerClass classType = GameDef.ePlayerClass.CLASS_WARRIOR;
	
	public StateController stateController = null;
	public AnimationEventTrigger animEventTrigger = null;
	
	public BaseMoveController moveController = null;
	
	public BuffManager buffManager = null;
	public LifeManager lifeManager = null;
	
	public BaseState.eState attack3StartState = BaseState.eState.Attack3;
	public BaseState.eState actionBStartState = BaseState.eState.None;
	public BaseState.eState actionBState = BaseState.eState.None;
	public string addFXActionBState = "FX_character_s_attack_";
	
	public PlayerInput input = null;
	public bool chainAttack = false;
	public bool strongAttack = false;
	
	public float actionBCoolTime = 1.0f;
	public float actionBCoolTimeAdjust = 0.0f;
	
	public float actionBDelayTime = 0.0f;
	public float GetActionBCoolTimeRate()
	{
		return GetRate(actionBDelayTime, actionBCoolTime + actionBCoolTimeAdjust); 
	}
	
	public float skill1CoolTime = 10.0f;
	public float skill1CoolTimeAdjust = 0.0f;
	public float skill1DelayTime = 0.0f;
	public float GetSkill1CoolTimeRate()
	{
		return GetRate(skill1DelayTime, skill1CoolTime + skill1CoolTimeAdjust);
	}
	
	public float potion1CoolTime = 10.0f;
	public float potion2CoolTime = 10.0f;
	public float potion1DelayTime = 0.0f;
	public float potion2DelayTime = 0.0f;
	public float GetPotion1CoolTimeRate()
	{
		return GetRate(potion1DelayTime, potion1CoolTime);
	}
	
	public float GetPotion2CoolTimeRate()
	{
		return GetRate (potion2DelayTime, potion2CoolTime);
	}
	
	public float GetRate(float curValue, float maxValue)
	{
		float rate = 0.0f;
		if (maxValue > 0.0f)
			rate = curValue / maxValue;
		
		return rate;
	}
	
	public float skill2CoolTime = 1.0f;
	public float skill2CoolTimeAdjust = 0.0f;
	public float skill2DelayTime = 0.0f;
	public float GetSkill2CoolTimeRate()
	{
		return GetRate(skill2DelayTime, skill2CoolTime + skill2CoolTimeAdjust);
	}
	
	public ActorInfo targetInfo = null;
	public BaseAttackInfo counterAttackInfo = new BaseAttackInfo();
	public bool enableCounterAttack = false;
	
	public string fxManaShield = "FX_mana_shield_01";
	public string fxCurseTarget = "FX_curse_target_particle";
	public string fxPoisonTarget = "FX_poison_target_particle";
	public string fxDamage = "FX_character_damage";
	public string fxReflect = "FX_Reaction";
	
	public string defaultCameraAnim = "Camera_shake_De";
	
	public float baseAttack21Rate = 0.0f;
	public float addAttack21Rate = 0.0f;
	
	public float addRequireAbilityRate = 0.0f;
	public float baseRequireSkill02 = 0.0f;
	
	public UISlider abilityUI = null;
	public UILabel abilityInfoLabel = null;
	public UISlider hpUI = null;
	public UILabel hpInfoLabel = null;
	
	public UIMyStatusInfo myStatusInfo = null;
	public UILabel charLevelLabel = null;
	
	public AttributeValue.eAttributeType abilityValueType = AttributeValue.eAttributeType.None;
	public AttributeValue.eAttributeType abilityRegenType = AttributeValue.eAttributeType.None;
	
	public int attributeTableID = 1000;
	
	public float destroyDelayTime = 1.5f;
	
	public GameObject ActionA_Progress = null;
	public GameObject ActionB_Progress = null;
	public GameObject Skill1_Progress = null;
	public GameObject Skill2_Progress = null;
	
	
	public bool isAIMode = false;
	[HideInInspector]
	public bool isAutoMode = false;
	[HideInInspector]
	public bool isAutoPotionMode = false;
	
	ComboCounter comboCounter = null;
	
	
	int potion1ItemID = 0;
	int potion2ItemID = 0;
	
	// Use this for initialization
	public virtual void Start () {
		GameDef.InitColorValue();
		
		isAutoMode = false;
		if (isAIMode == false )
			SetPlayerControllsNormalMode();
		else
			SetPlayerControlsAIMode();
		
		if (stateController != null)
		{
			stateController.onChangeState = new StateController.OnChangeState(OnChangeState);
			stateController.onEndState = new StateController.OnEndSate(OnEndState);
		}
		
		if (animEventTrigger != null)
		{
			animEventTrigger.onAnimationBegin = new AnimationEventTrigger.OnAnimationEvent(OnAnimationBegin);
			animEventTrigger.onAnimationEnd = new AnimationEventTrigger.OnAnimationEvent(OnAnimationEnd);
			animEventTrigger.onCollisionStart = new AnimationEventTrigger.OnAnimationEvent(OnCollisionStart);
			animEventTrigger.onCollisionStop = new AnimationEventTrigger.OnAnimationEvent(OnCollisionStop);
			animEventTrigger.onWalkingStart = new AnimationEventTrigger.OnAnimationEvent(OnWalkingStart);
			animEventTrigger.onWalkingStop = new AnimationEventTrigger.OnAnimationEvent(OnWalkingStop);
			animEventTrigger.onStrongAttackCheck = new AnimationEventTrigger.OnAnimationEvent(OnStrongAttackCheck);
			animEventTrigger.onFire = new AnimationEventTrigger.OnAnimationEvent(OnFire);
			
			animEventTrigger.onDisableAttackInput = new AnimationEventTrigger.OnAnimationEvent(OnDisableAttackInput);
			animEventTrigger.onEnableAttackInput = new AnimationEventTrigger.OnAnimationEvent(OnEnableAttackInput);
			
			animEventTrigger.onBreakState = new AnimationEventTrigger.OnAnimationEvent(OnBreakState);
			animEventTrigger.onCanChangeDir = new AnimationEventTrigger.OnAnimationEvent(OnCanChangeDir);
			
			animEventTrigger.onIgnoreActionKey = new AnimationEventTrigger.OnAnimationEvent(OnIgnoreActionKey);
			
			animEventTrigger.onDisableJump = new AnimationEventTrigger.OnAnimationEvent(OnDisableJump);
			animEventTrigger.onEnableJump = new AnimationEventTrigger.OnAnimationEvent(OnEnableJump);
			
			animEventTrigger.onJumpBlowAttack = new AnimationEventTrigger.OnAnimationEvent(OnJumpBlowAttack);
			
			animEventTrigger.onCameraShake = new AnimationEventTrigger.OnAnimationEventByString(OnCameraShake);
			
			animEventTrigger.onPlaySoundA = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundA);
			animEventTrigger.onPlaySoundB = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundB);
			animEventTrigger.onPlaySoundC = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundC);
			
			animEventTrigger.onStopSound = new AnimationEventTrigger.OnAnimationEvent(OnStopSound);
			
		}
		
		if (moveController != null)
			moveController.onCollision = new BaseMoveController.OnCollision(OnCollision);
		
		if (lifeManager != null)
		{
			lifeManager.onTargetHit = new LifeManager.OnTargetHit(OnTargetHit);
			lifeManager.onDamage = new LifeManager.OnDamageDelegate(OnDamage);
			
			lifeManager.checkAvoid = new LifeManager.CheckFunc(CheckAvoid);
			//lifeManager.checkBlock = new LifeManager.CheckFunc(CheckBlock);
			
			lifeManager.onDamageFX = new LifeManager.CheckFunc(OnDamageFX);
			
			lifeManager.attributeManager.abilityUI = abilityUI;
			lifeManager.attributeManager.hpUI = hpUI;
			
			lifeManager.attributeManager.hpInfoLabel = hpInfoLabel;
			lifeManager.attributeManager.abilityInfoLabel = abilityInfoLabel;
			
			lifeManager.charLevelLabel = this.charLevelLabel;
			if (lifeManager.charLevelLabel != null)
				lifeManager.charLevelLabel.text = lifeManager.charLevel.ToString();
			
			lifeManager.onDie = new LifeManager.OnDieDelegate(OnDie);
			
			if (lifeManager.equipManager != null)
			{
				lifeManager.equipManager.onChangeEquip = new EquipManager.OnEquipChanged(OnEquipChanged);
			}
		}
		
		myWayTypeMask = 1;
		if (stateController.IsContainState(BaseState.eState.JumpStart) == true)
			myWayTypeMask |= 1 << (int)WayPoint.eWayType.Jump;
		
		if (buffManager != null)
		{
			buffManager.onSelectBuffFX = new BuffManager.SelectBuffFX(SelectBuffFXObject);
		}
		
		if (myInfo.actorType == ActorInfo.ActorType.Player)
		{
			AudioListener listener = gameObject.GetComponent<AudioListener>();
			if (listener == null)
				listener = gameObject.AddComponent<AudioListener>();
		}
		
		audioSource = gameObject.GetComponent<AudioSource>();
		if (audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();
		
		if (audioSource != null)
			audioSource.bypassEffects = true;
		
		comboCounter = this.gameObject.GetComponent<ComboCounter>();
		
		Game.Instance.onInputPause += new Game.OnEvent(OnInputPause);
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		if (stringValueTable != null)
		{
			potion1ItemID = stringValueTable.GetData("Potion1ItemID");
			potion2ItemID = stringValueTable.GetData("Potion2ItemID");
		}
	}
	
	public PlayerControlButtonPanel playerControls = null;
	public virtual void Awake()
	{
		ActorManager actorManager = ActorManager.Instance;
		myInfo = GetComponent<ActorInfo>();
		if (myInfo != null)
		{
			if (actorManager != null)
				actorManager.AddActor(myInfo.myTeam, myInfo);
		}
		
		lifeManager = gameObject.GetComponent<LifeManager>();
		
		InitAttributeData();
		
		InitMasteryData();
		
		InitConquerorSkill();
		
		revivalController = gameObject.GetComponent<RevivalController>();
	}
	
	public void SetPlayerControlsAIMode()
	{
		hpCheckDelayTime = hpCheckCoolTime;
	}
	
	public void SetPlayerControllsNormalMode()
	{
		if (playerControls != null || myInfo.actorType != ActorInfo.ActorType.Player)
			return;
		
		playerControls = GameObject.FindObjectOfType(typeof(PlayerControlButtonPanel)) as PlayerControlButtonPanel;
		if (playerControls != null)
		{
			GameObject charButton = (GameObject)GameObject.Instantiate(ActionA_Progress);
			CharacterButton actionButton = null;
			if (charButton != null)
			{
				actionButton = charButton.GetComponent<CharacterButton>();
				playerControls.SetButton(playerControls.actionAButton, actionButton);
			}
			
			charButton = (GameObject)GameObject.Instantiate(ActionB_Progress);
			actionButton = null;
			if (charButton != null)
			{
				actionButton = charButton.GetComponent<CharacterButton>();
				playerControls.SetButton(playerControls.actionBButton, actionButton);
				playerControls.SetEnableActionBButton(false);
			}
			
			charButton = (GameObject)GameObject.Instantiate(Skill1_Progress);
			actionButton = null;
			if (charButton != null)
			{
				actionButton = charButton.GetComponent<CharacterButton>();
				playerControls.SetButton(playerControls.skill1Button, actionButton);
			}
			
			charButton = (GameObject)GameObject.Instantiate(Skill2_Progress);
			actionButton = null;
			if (charButton != null)
			{
				actionButton = charButton.GetComponent<CharacterButton>();
				playerControls.SetButton(playerControls.skill2Button, actionButton);
			}
			
			playerControls.player = this;
			
			if (playerControls.joystic != null)
			{
				playerControls.joystic.player = this;
			}
		}
		
		this.myStatusInfo = GameObject.FindObjectOfType(typeof(UIMyStatusInfo)) as UIMyStatusInfo;
		if (myStatusInfo != null)
		{
			switch(classType)
			{
			case GameDef.ePlayerClass.CLASS_WARRIOR:
				this.abilityUI = myStatusInfo.abilityRage;
				break;
			case GameDef.ePlayerClass.CLASS_ASSASSIN:
				this.abilityUI = myStatusInfo.abilityVital;
				break;
			case GameDef.ePlayerClass.CLASS_WIZARD:
				this.abilityUI = myStatusInfo.abilityMana;
				break;
			}
			
			myStatusInfo.SetAbilityType(this.abilityValueType);
			
			this.hpUI = myStatusInfo.hp;
			
			this.hpInfoLabel = myStatusInfo.hpInfoLabel;
			this.abilityInfoLabel = myStatusInfo.abilityInfoLabel;
			
			this.charLevelLabel = myStatusInfo.charLevel;
			
			if (myStatusInfo.battleFace != null)
				myStatusInfo.battleFace.SetClass(this.classType);
			
			if (lifeManager != null)
				this.myStatusInfo.SetAwakeningInfo(lifeManager.awakeningLevelManager);
		}
		
		Game.Instance.player = this;
	}
	
	public void UpdateAwakenLevel(AwakeningLevelManager manager)
	{
		if (myStatusInfo != null)
			myStatusInfo.SetAwakeningInfo(manager);
	}
	
	public void UpdateAIAwakenLevel(AwakeningLevelManager manager)
	{
		if (myStatusInfo != null)
			myStatusInfo.SetAIAwakeningInfo(manager);
	}
	
	public virtual void OnInputPause()
	{
		if (isAIMode == false || isAutoMode == true)
		{
			if (input != null)
			{
				input.AddInputs(PlayerInput.eControlKey.CK_LEFT, false);
				input.AddInputs(PlayerInput.eControlKey.CK_RIGHT, false);
			}
		}
	}
	
	public virtual void InitMasteryData()
	{
		MasteryManager_New masteryManager = lifeManager != null ? lifeManager.masteryManager_New : null;
		TableManager tableManager = TableManager.Instance;
		MasteryInfoTable masteryInfoTable = null;
		CharacterMasteryTable charMasteryTable = null;
		if (tableManager != null)
		{
			masteryInfoTable = tableManager.masteryInfoTable;
			charMasteryTable = tableManager.charMasteryTable;
		}
		
		if (masteryManager != null && masteryInfoTable != null && charMasteryTable != null)
		{
			masteryManager.onResetMastery = new MasteryManager_New.OnUpdateMastery(OnResetMastery_New);
			masteryManager.onUpdateMastery = new MasteryManager_New.OnUpdateMastery(OnUpdateMastery_New);
			
			MasteryIDs masteryIDs = charMasteryTable.GetData(this.classType);
			List<int> idList = masteryIDs != null ? masteryIDs.masteryIDs : null;
			if (idList != null)
			{
				MasteryInfo_New info = null;
				MasteryTableInfo_New tableInfo = null;
				foreach(int id in idList)
				{
					tableInfo = masteryInfoTable.GetData(id);
					if (tableInfo != null)
						info = new MasteryInfo_New(tableInfo);
					
					if (info != null)
						masteryManager.AddMastery(info);
				}
			}
		}
	}
	
	public virtual AttributeInitData InitAttributeData()
	{
		TableManager tableManager = TableManager.Instance;
		AttributeInitTable attributeTable = null;
		AttributeInitTable attributeIncTable = null;
		
		if (tableManager != null)
		{
			attributeTable = tableManager.attributeInitTable;
			attributeIncTable = tableManager.attributeIncTable;
		}
		
		AttributeInitData initData = null;
		AttributeInitData incData = null;
		if (attributeTable != null)
			initData = attributeTable.GetData(attributeTableID);
		
		if (attributeIncTable != null)
			incData = attributeIncTable.GetData((int)(this.classType) + 1);
		
		AttributeManager attributeManager = lifeManager.attributeManager;
		
		if (initData != null)
		{
			AttributeValue[] attributes = {
				new AttributeValue(AttributeValue.eAttributeType.AbilityPower, initData.abilityPower, incData != null ? incData.abilityPower : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.AttackDamage, initData.attackDamage, incData != null ? incData.attackDamage : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.CriticalHitRate, initData.criticalHitRate, incData != null ? incData.criticalHitRate : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.CriticalDamageRate, initData.criticalDamageRate, incData != null ? incData.criticalDamageRate : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.Health, initData.healthMax, 0.0f, 1.0f),
				new AttributeValue(AttributeValue.eAttributeType.HealthMax, initData.healthMax, incData != null ? incData.healthMax : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.HealthRegen, initData.healthRegen, incData != null ? incData.healthRegen : 0.0f, 0.0f),
				
				new AttributeValue(AttributeValue.eAttributeType.Armor, initData.armor, incData != null ? incData.armor : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.MagicResist, initData.magicResist, incData != null ? incData.magicResist : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.ArmorPenetration, initData.armorPenetration, incData != null ? incData.armorPenetration : 0.0f, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.MagicPenetration, initData.magicPenetration, incData != null ? incData.magicPenetration : 0.0f, 0.0f),
				
				new AttributeValue(AttributeValue.eAttributeType.Stamina, initData.stamina, incData != null ? incData.stamina : 0.0f, 0.0f),
			};
			
			
			foreach(AttributeValue initValue in attributes)
			{
				attributeManager.basicAttributeTypeList.Add(initValue.valueType);
				attributeManager.AddAttributeValue(initValue);
			}
			
		}
		else
		{
			AttributeValue[] attributes = {
				new AttributeValue(AttributeValue.eAttributeType.AbilityPower, 200.0f),
				new AttributeValue(AttributeValue.eAttributeType.AttackDamage, 200.0f),
				new AttributeValue(AttributeValue.eAttributeType.CriticalHitRate, 0.05f),
				new AttributeValue(AttributeValue.eAttributeType.CriticalDamageRate, 2.0f),
				new AttributeValue(AttributeValue.eAttributeType.Health, 12000.0f),
				new AttributeValue(AttributeValue.eAttributeType.HealthMax, 12000.0f),
				new AttributeValue(AttributeValue.eAttributeType.HealthRegen, 40.0f),
				new AttributeValue(AttributeValue.eAttributeType.Vital, 100.0f),
				new AttributeValue(AttributeValue.eAttributeType.VitalMax, 100.0f),
				new AttributeValue(AttributeValue.eAttributeType.VitalRegen, 25.0f),
				
				new AttributeValue(AttributeValue.eAttributeType.Armor, 100.0f),
				new AttributeValue(AttributeValue.eAttributeType.MagicResist, 100.0f),
				new AttributeValue(AttributeValue.eAttributeType.ArmorPenetration, 0.0f),
				new AttributeValue(AttributeValue.eAttributeType.MagicPenetration, 0.0f),
				
				new AttributeValue(AttributeValue.eAttributeType.Stamina, 55.0f),
			};
			
			foreach(AttributeValue initValue in attributes)
			{
				attributeManager.basicAttributeTypeList.Add(initValue.valueType);
				attributeManager.AddAttributeValue(initValue);
			}
		}
		
		return initData;
	}
	
	public virtual void InitConquerorSkill()
	{
		AwakeningLevelManager awakeningLevelManager = lifeManager.awakeningLevelManager;
		
		TableManager tableManager = TableManager.Instance;
		AwakeningLevelInfoTable awakeningLevelInfoTable = tableManager != null ? tableManager.awakeningLevelInfoTable : null;
		if (awakeningLevelManager != null && awakeningLevelInfoTable != null)
		{
			foreach(var temp in awakeningLevelInfoTable.dataList)
				awakeningLevelManager.SetSkillLevel(temp.Value, 0);
		}
	}
	
	public virtual void OnDestroy()
	{
		Debug.Log(this.gameObject.name + " OnDestroy......");
		
		ActorManager actorManager = ActorManager.Instance;
		if (myInfo != null)
		{
			if (actorManager != null)
			{
				actorManager.RemoveActor(myInfo.myTeam, myInfo);
				
				if (this.myInfo.actorType == ActorInfo.ActorType.Player)
					actorManager.playerInfo = null;
			}
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (Game.Instance.Pause == true)
			return;
		
		float hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		UpdateCoolTimes();
		
		if (isAIMode == false)
			UpdateNormalMode();
		else
			UpdateAIMode();
		
		if (isAutoPotionMode)
			UpdateAutoPotion(hpRate);
	}
	
	public void UpdateCoolTimes()
	{
		actionBDelayTime -= Time.deltaTime;
		if (actionBDelayTime <= 0.0)
			actionBDelayTime = 0.0f;
		
		skill1DelayTime -= Time.deltaTime;
		if (skill1DelayTime <= 0.0f)
			skill1DelayTime = 0.0f;
		
		skill2DelayTime -= Time.deltaTime;
		if (skill2DelayTime <= 0.0f)
			skill2DelayTime = 0.0f;
		
		if (idleTargetTime > 0.0f)
			idleTargetTime -= Time.deltaTime;
		
		potion1DelayTime -= Time.deltaTime;
		if (potion1DelayTime <= 0.0f)
			potion1DelayTime = 0.0f;
		
		potion2DelayTime -= Time.deltaTime;
		if (potion2DelayTime <= 0.0f)
			potion2DelayTime = 0.0f;
	}
	
	public void UpdateNormalMode()
	{
		if (CheckCharSkill() == true)
			return;
		
		if (CheckSpecialAttack() == true)
			return;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Jumpland:
		case BaseState.eState.Blowattack:
			if (input.bCanBreakState == true)
			{
				if (input.mActionAKey == true)
					stateController.ChangeState(BaseState.eState.Attack1);
			}
			break;
		case BaseState.eState.Stand:
		case BaseState.eState.Run:
			if (stateController.preState == BaseState.eState.Attack21)
			{
				if (input.bActionAKeyChanged == true && input.mActionAKey == true)
					stateController.ChangeState(BaseState.eState.Attack1);
			}
			else
			{
				if (input.mActionAKey == true)
					stateController.ChangeState(BaseState.eState.Attack1);
			}
			break;
		case BaseState.eState.Dash:
			if (input.mActionAKey == true)
				stateController.ChangeState(BaseState.eState.Dashattack);
			break;
		case BaseState.eState.Attack1:
		case BaseState.eState.Attack2:
			break;
		case BaseState.eState.Attack3_Base:
		case BaseState.eState.Attack3_Focus:
			if (input.mActionAKey == false)
				stateController.ChangeState(BaseState.eState.Attack3);
			break;
		case BaseState.eState.Attack3_1_Ready:
			if (input.mActionAKey == false)
				stateController.ChangeState(BaseState.eState.Heavyattack);
			break;
		case BaseState.eState.JumpStart:
		case BaseState.eState.JumpFall:
			if (input.mActionAKey == true)
				stateController.ChangeState(BaseState.eState.JumpAttack);
			break;
		case BaseState.eState.Evadestart:
		case BaseState.eState.Evadeend:
			UpdateEvadeState();
			break;
		case BaseState.eState.Stun:
			UpdateStun();
			break;
		}
	}
	
	protected bool CheckSpecialAttack ()
    {
        if (input.mActionBKey == true && actionBDelayTime <= 0.0f)
        {
			bool availableState = true;
			switch(stateController.currentState)
			{
			case BaseState.eState.JumpStart:
			case BaseState.eState.JumpFall:
			case BaseState.eState.JumpAttack:
			case BaseState.eState.Knockdownstart:
			case BaseState.eState.Knockdownfall:
			case BaseState.eState.Knockdownland:
				availableState = false;
				break;
			case BaseState.eState.Skill01:
			case BaseState.eState.Skill02:
				availableState = false;
				break;
			}
			
			if (availableState == false)
				return false;
			
			if (actionBStartState != BaseState.eState.None &&
				CheckStateRequireAbilityValue(actionBStartState) == true)
			{
				if (lifeManager != null)
					lifeManager.stunDelayTime = 0.0f;
				
				stateController.ChangeState(actionBStartState);
				actionBDelayTime = GetActionBCoolTime();
				
				input.mActionBKey = false;
				return true;
			}
        }
		
		return false;
        //return base.UpdateSpecialAttack(EnableActionB, EnableSkill01, EnableSkill02);
    }
	
	public bool CheckStateRequireAbilityValue(BaseState.eState state)
	{
		bool bCheckAction = false;
				
		float requireAbilityValue = 0.0f;
		CharStateInfo charStateInfo = stateController.stateList.GetState(state);
		if (charStateInfo != null && charStateInfo.stateInfo != null)
		{
			requireAbilityValue = GetRequireAbilityValue(charStateInfo.stateInfo);
			
			float curAbilityValue = lifeManager.attributeManager.GetAttributeValue(this.abilityValueType);
			
			bCheckAction = curAbilityValue >= requireAbilityValue;
		}
		
		return bCheckAction;
	}
	
	protected bool CheckCharSkill()
    {
		bool availableState = true;
		switch(stateController.currentState)
		{
		case BaseState.eState.JumpStart:
		case BaseState.eState.JumpFall:
		case BaseState.eState.JumpAttack:
		case BaseState.eState.Knockdownstart:
		case BaseState.eState.Knockdownfall:
		case BaseState.eState.Knockdownland:
			availableState = false;
			break;
		}
		
	   if (availableState == true)
        {
            if (input.mSkill1Button == true)
			{
				if (CheckStateRequireAbilityValue(BaseState.eState.Skill01) == true)
				{
					stateController.ChangeState(BaseState.eState.Skill01);
					skill1DelayTime = skill1CoolTime;
					
					input.mSkill1Button = false;
					
					 return true;
				}
			}
            else if (input.mSkill2Button == true)
			{
				if (CheckStateRequireAbilityValue(BaseState.eState.Skill02) == true)
				{
					stateController.ChangeState(BaseState.eState.Skill02);
					skill2DelayTime = skill2CoolTime;
					
					input.mSkill2Button = false;
					
					 return true;
				}
			}
        }
		
		return false;
    }
	
	public virtual void OnEndState()
	{
		if (stateController.currentState == actionBState && addFXActionBState != "")
		{
			if (myInfo.actorType == ActorInfo.ActorType.Player)
				stateController.RemoveFXObject(addFXActionBState, eFXEffectType.CameraNode);
		}
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Stun:
			lifeManager.stunDelayTime = 0.0f;
			break;
		case BaseState.eState.Blowattack:
			input.jumpBlowAttackCount = 0;
			break;
		}
		
		lifeManager.attackStateInfo.addAttackPower = 0.0f;
		lifeManager.attackStateInfo.addAttackRate = 0.0f;
		
		OnCameraShake(defaultCameraAnim);
	}
	
	public virtual void OnChangeState(CharStateInfo info)
	{
		strongAttack = false;
		if (info != null)
		{
			chainAttack = info.chainAttack;
			
			if (info.baseState.state == BaseState.eState.Skill02)
				lifeManager.stunDelayTime = 0.0f;
		}
		else
		{
			chainAttack = false;
		}
		
		if (input != null)
		{
			input.mActionBKey = false;
			input.mActionBKey = false;
			input.mSkill1Button = false;
			input.mSkill2Button = false;
			input.mJumpKey = false;
			
			//Attack Input enable.....
			input.enableAttackInput = true;
			input.ResetActionKeyInfo();
			
			input.ignoreActionKeyChange = false;
			
			input.bCanBreakState = false;
			
			input.bCanChangeDir = info.cantChangeDir == false;
			
			input.enableJump = true;
		}
		
		enableCounterAttack = false;
		
		if (moveController != null)
		{
			float moveSpeed = 0.0f;
			switch(info.moveType)
			{
			case CharStateInfo.eMoveType.Dash:
				moveSpeed = moveController.dashMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Run:
				moveSpeed = moveController.defaultMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Keep:
				moveController.prevMoveSpeed = moveController.moveSpeed;
				moveSpeed = moveController.moveSpeed;
				break;
			case CharStateInfo.eMoveType.Stop:
				moveSpeed = 0.0f;
				break;
			}
			
			moveController.moveSpeed = moveSpeed;
		}
		
		if (lifeManager != null)
		{
			lifeManager.SetAttackInfo(stateController.curStateInfo.stateInfo);
			
			lifeManager.ClearHitObject();
		}
		
		if (stateController.currentState == BaseState.eState.Stand)
		{
			if (moveController != null)
			{
				moveController.ChangeDefaultLayer(true);
				
				moveController.ModifyGroundPos();
				
				if (isAIMode == true && this.targetInfo != null)
					UpdateTargetPath(this.targetInfo.gameObject);
			}
			
			rigidbody.velocity = Vector3.zero;
			rigidbody.angularVelocity = Vector3.zero;
		}
		
		if (input != null)
		{
			switch(this.stateController.currentState)
			{
			case BaseState.eState.JumpFall:
			case BaseState.eState.JumpAttack:
			case BaseState.eState.Knockdownfall:
				if (this.lifeManager.GetHP() > 0.0f)
					input.jumpBlowAttack = true;
				else
					input.jumpBlowAttack = false;
				break;
			case BaseState.eState.JumpStart:
				input.jumpBlowAttack = false;
				input.jumpBlowAttackCount = 0;
				break;
			case BaseState.eState.Blowattack:
				input.jumpBlowAttackCount += 1;
				break;
			default:
				input.jumpBlowAttack = false;
				break;
			}
		}
		
		if (stateController.currentState == actionBState && addFXActionBState != "")
		{
			if (myInfo.actorType == ActorInfo.ActorType.Player)
				stateController.AddFXObject(addFXActionBState, eFXEffectType.CameraNode, 1.0f);
		}
		
		if (stateController.currentState == BaseState.eState.Knockdownstart)
		{
			AttributeManager attributeManager = lifeManager != null ? lifeManager.attributeManager : null;
			float regenHPRate = 0.0f;
			if (attributeManager != null)
				regenHPRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.RegenHPWhenKnockdown);
			
			if (regenHPRate != 0.0f)
			{
				float regenHP = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.HealthMax) * regenHPRate;
				
				int oldIndex = -1;
				if (this.buffManager != null)
				{
					oldIndex = this.buffManager.GetBuff(GameDef.eBuffType.BT_REGENHP);
				
					if (oldIndex != -1)
						this.buffManager.RemoveBuff(oldIndex);
				
					this.buffManager.AddBuff(GameDef.eBuffType.BT_REGENHP, regenHP, 5.0f, null, 1);
				}
			}
		}
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Run:
		case BaseState.eState.Dash:
			jumpDelayTime = jumpCoolTime;
			break;
		}
		
		bJumpTriggerByTargetHit = false;
		targetHitInfo = null;
		targetHitActor = null;
		
		float useAbilityValue = GetRequireAbilityValue(stateController.curStateInfo.stateInfo);
		OnChangeAbilityValue(-useAbilityValue);
	}
	
	public float GetRequireAbilityValue(StateInfo stateInfo)
	{
		float baseAbilityValue = stateInfo.requireAbilityValue + stateInfo.addRequireAbility;
		float addAbilityValue = baseAbilityValue * stateInfo.addRequireAbilityRate;
		
		float useAbilityValue = baseAbilityValue + addAbilityValue;
		
		return useAbilityValue;
	}
	
	public virtual void OnChangeAbilityValue(float addValue)
	{
		//lifeManager.attributeManager.AddAbility(addValue);
		AttributeValue curValue = null;
		AttributeValue maxValue = null;
		
		AttributeManager attributeManager = lifeManager != null ? lifeManager.attributeManager : null;
		
		if (attributeManager == null)
			return;
		
		switch(this.abilityValueType)
		{
		case AttributeValue.eAttributeType.Vital:
			curValue = attributeManager.GetAttribute(AttributeValue.eAttributeType.Vital);
			maxValue = attributeManager.GetAttribute(AttributeValue.eAttributeType.VitalMax);
			break;
		case AttributeValue.eAttributeType.Rage:
			curValue = attributeManager.GetAttribute(AttributeValue.eAttributeType.Rage);
			maxValue = attributeManager.GetAttribute(AttributeValue.eAttributeType.RageMax);
			break;
		case AttributeValue.eAttributeType.Mana:
			curValue = attributeManager.GetAttribute(AttributeValue.eAttributeType.Mana);
			maxValue = attributeManager.GetAttribute(AttributeValue.eAttributeType.ManaMax);
			break;
		}
		
		if (curValue != null && maxValue != null)
		{
			float resultValue = curValue.baseValue + addValue;
			
			curValue.baseValue = Mathf.Clamp(resultValue, 0.0f, maxValue.Value);
			
			lifeManager.attributeManager.UpdateValue(curValue);
		}
		
		float recoveryHPRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.RegenHPWhenRecoverAbility);
		float addHP = addValue * recoveryHPRate;
		lifeManager.IncHP(addHP, false, GameDef.eBuffType.BT_REGENHP);
	}
	
	public void OnAnimationBegin()
	{
		
	}
	
	public BaseAttackInfo GetAttack(BaseState.eState attackState, bool skillCheck)
	{
		if (targetInfo == null || isRunawayMode == true)
			return null;
		
		WayPointManager targetWayMgr = BaseMonster.GetWayPointManager(targetInfo);
		WayPointManager curWayMgr = BaseMonster.GetWayPointManager(this.moveController.groundCollider);
		
		bool bSameGround = targetWayMgr == curWayMgr;
		
		Vector3 targetPos = this.transform.position;
		float targetRadius = 0.0f;
		
		targetPos = targetInfo.transform.position;
		targetRadius = targetInfo.colliderRadius;
		
		BaseAttackInfo attackInfo = null;
		
		LifeManager targetLifeMgr = null;
		if (targetInfo != null)
			targetLifeMgr = targetInfo.GetComponent<LifeManager>();
		
		if (targetLifeMgr != null && targetLifeMgr.GetHP() <= 0.0f)
			return null;
		
		int randValue = Random.Range(0, 100);
		Vector3 vDiff = targetPos - this.transform.position;
		float diffX = Mathf.Max(0.0f, Mathf.Abs(vDiff.x) - (myInfo.colliderRadius + targetRadius));
		float diffY = vDiff.y;
		
		if (Mathf.Abs(diffY) < 0.1f)
			diffY = 0.0f;
		
		int nCount = attackList.Count;
		for (int i = 0; i < nCount; ++i)
		{
			BaseAttackInfo info = attackList[i];
			
			if (info.attackState == attackState)
			{
				if (skillCheck == true)
				{
					if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround, myRankValue) == true)
					{
						attackInfo = info;
						break;
					}
				}
				else
				{
					attackInfo = info;
					break;
				}
			}
		}

		return attackInfo;
	}
	
	public void OnAnimationEnd()
	{
		//Debug.Log("OnAnimationEnd.....");
		BaseState.eState nextState = BaseState.eState.Stand;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Stage_clear1:
			nextState = BaseState.eState.Stage_clear2;
			break;
		case BaseState.eState.Stage_clear2:
			nextState = BaseState.eState.Stage_clear2;
			break;
		case BaseState.eState.Knockdownstart:
			nextState = BaseState.eState.Knockdownstart;//BaseState.eState.Knockdownfall;
			if (this.lifeManager.GetHP() <= 0.0f)
				nextState = BaseState.eState.Knockdownfall;
			break;
		case BaseState.eState.Knockdownfall:
			nextState = BaseState.eState.Knockdownfall;//BaseState.eState.Knockdownland;
			if (this.lifeManager.GetHP() <= 0.0f)
				nextState = BaseState.eState.Knockdownland;
			break;
		case BaseState.eState.Die:
			nextState = BaseState.eState.Die;
			if (destroyDelayTime >= 0.0f)
				DestroyObject(this.gameObject, destroyDelayTime);
			break;
		case BaseState.eState.Knockdown_Die:
			nextState = BaseState.eState.Knockdown_Die;
			if (destroyDelayTime >= 0.0f)
				DestroyObject(this.gameObject, destroyDelayTime);
			break;
		case BaseState.eState.JumpStart:
			nextState = BaseState.eState.JumpStart;//BaseState.eState.JumpFall;
			break;
		case BaseState.eState.JumpFall:
			nextState = BaseState.eState.JumpFall;//BaseState.eState.Jumpland;
			break;
		case BaseState.eState.JumpAttack:
			nextState = BaseState.eState.JumpAttack;//BaseState.eState.Jumpland;
			break;
		case BaseState.eState.Attack1:
			if (isAIMode == true)
			{
				BaseAttackInfo nextAttackInfo = GetAttack(BaseState.eState.Attack21, true);
				if (nextAttackInfo != null)
				{
					nextState = nextAttackInfo.attackState;
				}
				else
				{
					nextAttackInfo = GetAttack(BaseState.eState.Attack2, true);
					if (nextAttackInfo != null)
					{
						nextState = nextAttackInfo.attackState;
					}
				}
			}
			else
			{
				if (chainAttack == true)
				{
					if (input.enableAttackInput == true)
					{
						bool bNextAttack = false;
						if (input.ignoreActionKeyChange == true)
							bNextAttack = (input.bActionAKeyChanged == true || input.mActionAKey == true);
						else
							bNextAttack = (input.bActionAKeyChanged == true);
						
						if (bNextAttack == true)
							nextState = BaseState.eState.Attack2;
						else if (strongAttack == true)
							nextState = BaseState.eState.Attack21;
					}
				}
				else if (input.bActionAKeyChanged == true && strongAttack == true)
					nextState = BaseState.eState.Attack21;
			}
            break;
		case BaseState.eState.Attack2:
			if (isAIMode == true)
			{
				//nextState = attack3StartState;
				BaseAttackInfo nextAttackInfo = GetAttack(attack3StartState, true);
				if (nextAttackInfo != null)
				{
					nextState = nextAttackInfo.attackState;
				}
			}
			else
			{
				if (chainAttack == true)
				{
					if (input.bActionAKeyChanged == true || input.mActionAKey == true)
						nextState = attack3StartState;
				}
			}
			break;
		case BaseState.eState.Attack3_Base:
			if (isAIMode == true)
			{
				nextState = BaseState.eState.Attack3_Focus;
				
				if (targetInfo != null)
				{
					Vector3 targetPos = targetInfo.transform.position;
					Vector3 myPos = this.gameObject.transform.position;
					
					ChangeMoveDir(targetPos, true);
					
					Vector3 diff = targetPos - myPos;
					float diffX = Mathf.Abs(diff.x) - (myInfo.colliderRadius + targetInfo.colliderRadius);
					if (diffX <= 2.0f)
						nextState = BaseState.eState.Attack3;
				}
			}
			else
			{
				if (chainAttack == true && input.mActionAKey == true)
					nextState = BaseState.eState.Attack3_Focus;
				else
					nextState = BaseState.eState.Attack3;
			}
			break;
		case BaseState.eState.Attack3_Focus:
			if (isAIMode == true)
			{
				nextState = BaseState.eState.Attack3_1_Ready;
				
				if (targetInfo != null)
				{
					Vector3 targetPos = targetInfo.transform.position;
					Vector3 myPos = this.gameObject.transform.position;
					
					ChangeMoveDir(targetPos, true);
					
					Vector3 diff = targetPos - myPos;
					float diffX = Mathf.Abs(diff.x) - (myInfo.colliderRadius + targetInfo.colliderRadius);
					if (diffX <= 2.0f)
						nextState = BaseState.eState.Attack3;
				}
			}
			else
			{
				if (chainAttack == true && input.mActionAKey == true)
					nextState = BaseState.eState.Attack3_1_Ready;
				else
					nextState = BaseState.eState.Attack3;
			}
			break;
		case BaseState.eState.Attack3_1_Ready:
			if (isAIMode == true)
			{
				nextState = BaseState.eState.Attack3_1_Ready;
				
				if (targetInfo != null)
					nextState = BaseState.eState.Heavyattack;
			}
			else
			{
				if (chainAttack == true && input.mActionAKey == true)
					nextState = BaseState.eState.Attack3_1_Ready;
				else
					nextState = BaseState.eState.Heavyattack;
			}
			break;
		case BaseState.eState.Drop:
			nextState = BaseState.eState.Blowattack;
			break;
		case BaseState.eState.Evadestart:
			nextState = BaseState.eState.Evadeend;
			break;
		case BaseState.eState.Evadeend:
		case BaseState.eState.Evadecounterattack:
		case BaseState.eState.Heavyattack:
		case BaseState.eState.Skill01:
		case BaseState.eState.Skill02:
		case BaseState.eState.Dashattack:
		case BaseState.eState.AttackB_1:
		case BaseState.eState.AttackB_2:
		case BaseState.eState.AttackB_3:	
			nextState = BaseState.eState.Stand;
			break;
		case BaseState.eState.Attack21:
			nextState = BaseState.eState.Stand;
			break;
		case BaseState.eState.Jumpland:
		case BaseState.eState.Knockdownland:
		case BaseState.eState.Damage:
			if (lifeManager.stunDelayTime > 0.0f)
				nextState = BaseState.eState.Stun;
			else
				nextState = BaseState.eState.Stand;
			
			if (this.lifeManager.GetHP() <= 0.0f)
			{
				if (stateController.currentState == BaseState.eState.Knockdownland)
					nextState = BaseState.eState.Knockdown_Die;
				else
					nextState = BaseState.eState.Die;
			}
			break;
		default:
			switch(stateController.preState)
			{
			case BaseState.eState.Dash:
			case BaseState.eState.Run:
				nextState = stateController.preState;
				break;
			}
			break;
		}
		
		stateController.ChangeState(nextState);	
	}
	
	public virtual void OnCollisionStart()
	{
		int attackStep = stateController.colliderManager.colliderStep;
		
		CollisionInfo colInfo = null;
		
		int nCount = stateController.curStateInfo.collisionInfoList.Count;
		if (attackStep >= 0 && attackStep < nCount)
			colInfo = stateController.curStateInfo.collisionInfoList[attackStep];
		
		if (colInfo != null)
		{
			stateController.colliderManager.SetupCollider(colInfo.colliderName, true);
			
			if (stateController.currentState == BaseState.eState.Skill01)
			{
				ScrollCamera stageCam = null;
				if (Game.Instance.stageManager != null)
					stageCam = Game.Instance.stageManager.StageCamera;
				
				if (stageCam != null)
				{
					Vector3 vPos = stageCam.gameObject.transform.position;
					stateController.colliderManager.SetColliderPos(colInfo.colliderName, vPos);
				}
			}
			
			colInfo.stateInfo.acquireAbility = stateController.curStateInfo.stateInfo.acquireAbility;
			colInfo.stateInfo.requireAbilityValue = stateController.curStateInfo.stateInfo.requireAbilityValue;
			colInfo.stateInfo.defenseState = stateController.curStateInfo.stateInfo.defenseState;
			
			if (lifeManager != null)
				lifeManager.SetAttackInfo(colInfo.stateInfo);
		}
		
		stateController.colliderManager.IncColliderStep();
	}
	
	public virtual void OnCollisionStop()
	{
		int attackStep = stateController.colliderManager.colliderStep - 1;
		
		CollisionInfo attackInfo = null;
		
		int nCount = stateController.curStateInfo.collisionInfoList.Count;
		if (attackStep >= 0 && attackStep < nCount)
			attackInfo = stateController.curStateInfo.collisionInfoList[attackStep];
		
		if (attackInfo != null)
			stateController.colliderManager.SetupCollider(attackInfo.colliderName, false);
		
		if (lifeManager != null)
		{
			lifeManager.SetAttackInfo(stateController.curStateInfo.stateInfo);
			lifeManager.ClearHitObject();
		}
	}
	
	public virtual void OnWalkingStart()
	{
		if (moveController != null)
		{
			moveController.moveSpeed = stateController.curStateInfo.walkingEventMoveSpeed;
		}
	}
	
	public virtual void OnWalkingStop()
	{
		if (moveController != null)
		{
			float moveSpeed = 0.0f;
			switch(stateController.curStateInfo.moveType)
			{
			case CharStateInfo.eMoveType.Dash:
				moveSpeed = moveController.dashMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Run:
				moveSpeed = moveController.defaultMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Keep:
				moveSpeed = moveController.prevMoveSpeed;
				break;
			case CharStateInfo.eMoveType.Stop:
				moveSpeed = 0.0f;
				break;
			}
			
			moveController.moveSpeed = moveSpeed;
		}
	}
	
	public void OnStrongAttackCheck()
	{
		bool bActionAKey = false;
		if (input != null)
			bActionAKey = input.mActionAKey;
		
		if (/*chainAttack == false && */bActionAKey == true)
		{
			strongAttack = true;
		}
	}
	
	public AudioSource audioSource = null;
	public void OnPlaySoundA(string soundFileName)
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.DontCare);
	}
	
	public void OnPlaySoundB(string soundFileName)
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.TryKeep);
	}
	
	public void OnPlaySoundC(string soundFileName)
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null && GameOption.effectToggle == true)
			soundManager.AddSoundEffect(soundFileName, SoundEffect.eSoundType.CancelByState);
	}
	
	public void OnStopSound()
	{
		SoundManager soundManager = null;
		if (stateController != null)
			soundManager = stateController.soundManager;
		
		if (soundManager != null)
			soundManager.StopSoundEffects(SoundEffect.eSoundType.TryKeep);
	}
	
	public virtual void FireProjectile()
	{
		
	}
	
	public ActorInfo attackTargetInfo = null;
	public void OnFire()
	{
		OnCollisionStart();
		
		FireProjectile();
		
		OnCollisionStop();
	}
	
	public void OnCollision()
	{
		
	}
	
	public void OnDisableAttackInput()
	{
		if (input != null)
			input.enableAttackInput = false;
	}
	
	public void OnEnableAttackInput()
	{
		if (input != null)
			input.enableAttackInput = true;
	}
	
	public void OnBreakState()
	{
		if (input != null)
			input.bCanBreakState = true;
	}
	
	public void OnCanChangeDir()
	{
		if (input != null)
			input.bCanChangeDir = true;
	}
	
	public void OnIgnoreActionKey()
	{
		if (input != null)
			input.ignoreActionKeyChange = true;
	}
	
	public void OnDisableJump()
	{
		if (input != null)
			input.enableJump = false;
	}
	
	public void OnEnableJump()
	{
		if (input != null)
			input.enableJump = true;
		
		if (lifeManager.stunDelayTime > 0.0f)
			return;
		
		if (isAIMode == false)
			return;
		
		if (targetHitInfo == null || targetHitActor == null || bJumpTriggerByTargetHit == false)
			return;
		
		bool isJump = false;
		
		int randValue = Random.Range(0, 100);
		float rateFloatValue = 0.0f;
		int rateValue = 0;
		
		switch(targetHitInfo.attackState)
		{
		case BaseState.eState.Attack2:
			if (targetHitActor.stateController.IsJumpState() == true)
			{
				rateFloatValue = (jumpRateAfterAttack2 - ((float)myRankValue * adjustJumpRateAfterAttack2)) * 100.0f;
				rateValue = Mathf.RoundToInt(rateFloatValue);
				if (randValue < rateValue)
					isJump = true;
			}
			break;
		case BaseState.eState.Dashattack:
			rateFloatValue = (jumpRateAfterDashAttack - ((float)myRankValue * adjustJumpRateAfterDashAttack)) * 100.0f;
			rateValue = Mathf.RoundToInt(rateFloatValue);
			if (randValue < rateValue)
				isJump = true;
			break;
		case BaseState.eState.Skill02:
			rateFloatValue = (jumpRateAfterSkill02 - ((float)myRankValue * adjustJumpRateAfterSkill02)) * 100.0f;
			rateValue = Mathf.RoundToInt(rateFloatValue);
			if (randValue < rateValue)
				isJump = true;
			break;
		}
		
		if (moveController != null && isJump == true)
		{
			stateController.ChangeState(BaseState.eState.JumpStart);
			
			moveController.moveSpeed = moveController.defaultMoveSpeed;
			moveController.DoJump();
		}
		
		bJumpTriggerByTargetHit = false;
	}
	
	public void OnJumpBlowAttack()
	{
		if (input != null)
			input.jumpBlowAttack = true;
	}
	
	public void OnCameraShake(string strValue)
	{
		if (myInfo.actorType != ActorInfo.ActorType.Player)
			return;
		
		StageManager stageManager = null;
		if (moveController != null)
			stageManager = moveController.stageManager;
		
		ScrollCamera mainCamera = null;
		if (stageManager != null)
			mainCamera = stageManager.StageCamera;
		
		if (mainCamera != null)
			mainCamera.ChangeAnimation(strValue);
	}
	
	public void UpdateEvadeState()
	{

	}
	
	public string avoidSoundFile = "";
	public bool CheckAvoid(LifeManager attacker)
	{
		if (lifeManager.stunDelayTime > 0.0f) return false;
		
		if (stateController.IsJumpState() == true) return false;
		
		if (this.actionBState == BaseState.eState.None)
			return false;
		
		if (lifeManager.attackStateInfo.stateInfo.defenseState != StateInfo.eDefenseState.DS_AVOID)
			return false;
		
		float counterAttackLimitLength = counterAttackInfo.attackHorizontalRange.y;
		
		var counterLayer = 0;
		switch(this.myInfo.actorType)
		{
		case ActorInfo.ActorType.Player:
			counterLayer = 1 << LayerMask.NameToLayer("MonsterBody");
			break;
		case ActorInfo.ActorType.BossMonster:
			counterLayer = 1 << LayerMask.NameToLayer("PlayerBody");
			break;
		}
		
		Vector3 vDir = Vector3.right;
		Vector3 vPos = this.transform.position;
		if (moveController != null)
		{
			vDir = moveController.moveDir;
			vPos.y += moveController.colliderRadius;
		}
		
		RaycastHit hitInfo;
		if (Physics.Raycast(vPos, vDir, out hitInfo, counterAttackLimitLength, counterLayer) == true)
		{
			if (counterAttackInfo.attackHorizontalRange.x <= hitInfo.distance &&
				counterAttackInfo.attackHorizontalRange.y >= hitInfo.distance)
			{
				if (attacker != null)
				{
					if (lifeManager != null)
						lifeManager.ChangeMoveDir(attacker.transform);
				}
				
				stateController.ChangeState(this.actionBState);
			}
		}
		else
		{
			float effectVoluem = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(audioSource, avoidSoundFile, effectVoluem);
		}
		
		return true;
	}
	
	/*
	void OnTriggerEnter(Collider other)
	{
		//this.gameObject.layer == LayerMask.NameToLayer("PlayerWeapon");
		if (other.gameObject.layer != LayerMask.NameToLayer("MonsterWeapon") &&
			other.gameObject.layer != LayerMask.NameToLayer("MonsterProjectile"))
			return;
			
		Debug.Log(this + " vs " + other + " Time : " + Time.time);
	}
	*/
	
	public virtual FXInfo SelectBuffFXObject(GameDef.eBuffType e)
	{
		FXInfo fxInfo = null;
		if (stateController != null)
		{
			switch(e)
			{
			case GameDef.eBuffType.BT_MANASHIELD:
				fxInfo = stateController.GetFXObject(fxManaShield, eFXEffectType.ScaleNode);//FXManaShield1;
				break;
			case GameDef.eBuffType.BT_CURSE:
				fxInfo = stateController.GetFXObject(fxCurseTarget, eFXEffectType.ScaleNode);//FXCurse;
				break;
			case GameDef.eBuffType.BT_POISION:
				fxInfo = stateController.GetFXObject(fxPoisonTarget, eFXEffectType.ScaleNode);//FXPoison;
				break;
			case GameDef.eBuffType.BT_REFLECTDAMAGE:
				fxInfo = stateController.GetFXObject(fxReflect, eFXEffectType.ScaleNode);
				break;
			}
		}
		return fxInfo;
	}
	
	public float GetActionBCoolTime()
	{
		float coolTime = actionBCoolTime + actionBCoolTimeAdjust;
		coolTime = Mathf.Max(0.0f, coolTime);
		
		return coolTime;
	}
	
	public virtual void OnUpdateMastery_New ()
	{
		SetPlayerControllsNormalMode();
	}
	
	public virtual void OnResetMastery_New ()
	{
		
	}
	
	public virtual void UpdateStun()
	{
		lifeManager.stunDelayTime -= Time.deltaTime;
		
		if (lifeManager.stunDelayTime <= 0.0f)
		{
			lifeManager.stunDelayTime = 0.0f;
			stateController.ChangeState(BaseState.eState.Stand);
		}
	}
	
	public bool IsAliveState()
	{
		float hp = lifeManager.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Health);
		
		bool isDie = false;
		switch(stateController.currentState)
		{
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			isDie = true;
			break;
		default:
			isDie = hp <= 0.0f;
			break;
		}
		
		return !isDie;
	}
	
	public bool OnDamageFX(LifeManager attacker)
	{
		if (myInfo.actorType == ActorInfo.ActorType.Player)
		{
			stateController.AddFXDelayInfo(fxDamage, eFXEffectType.CameraNode, 1.0f, 0.4f);
		
			return true;
		}
		else
			return false;
	}
	
	
	public bool isEventMode = false;
	//private Vector3 vMoveTargetPos = Vector3.zero;
	
	public virtual void OnCompleteDialogEvent()
	{
		isEventMode = false;
	
	}
	
	
	public RevivalController revivalController = null;
	public float slowTimeRate = 0.2f;
	public float slowDelayTime = 1.0f;
	public virtual void OnDie(LifeManager attacker)
	{
		lifeManager.stunDelayTime = 0.0f;
		
		AttributeValue _value = lifeManager.attributeManager.GetAttribute(this.abilityValueType);
		if (_value != null)
		{
			_value.baseValue = 0.0f;
			lifeManager.attributeManager.UpdateValue(_value);
		}
		
		StageManager stageManager = Game.Instance.stageManager;
		
		StartSlow();
		
		if (stageManager != null)
		{
			switch(stageManager.StageType)
			{
			case StageManager.eStageType.ST_ARENA:
				PlayerController player = null;
				if (myInfo.actorType != ActorInfo.ActorType.Player)
					player = Game.Instance.player;
				else
					player = Game.Instance.arenaPlayer;
				
				if (Game.Instance != null)
					Game.Instance.SetPlayerSuperArmorMode(player);
				break;
			}
		}
		
		if (myInfo.actorType != ActorInfo.ActorType.Player)
			return;
		
		if (stageManager != null)
		{
			StageEndEvent stageEndEvent = stageManager.stageEndEvent;
			
			switch(stageManager.StageType)
			{
			case StageManager.eStageType.ST_FIELD:
			case StageManager.eStageType.ST_EVENT:
				this.Invoke("OnDieField", slowDelayTime);
				break;
			case StageManager.eStageType.ST_ARENA:
				if (stageEndEvent != null)
					stageEndEvent.Invoke("OnStageFailed", slowDelayTime);
				break;
			case StageManager.eStageType.ST_WAVE:
				if (stageEndEvent != null)
					stageEndEvent.Invoke("OnStageFailed", slowDelayTime);
				break;
			case StageManager.eStageType.ST_BOSSRAID:
				if (stageEndEvent != null)
					stageEndEvent.Invoke("OnStageFailed", slowDelayTime);
				break;
			}
		}
	}
	
	public void StartSlow()
	{
		Time.timeScale = slowTimeRate;
		Invoke("ResetSlow", slowDelayTime);
	}
	
	public void ResetSlow()
	{
		Time.timeScale = 1.0f;
	}
	
	public void OnDieField()
	{
		StageManager stageManager = Game.Instance.stageManager;
		StageEndEvent stageEndEvent = null;
		if (stageManager != null)
			stageEndEvent = stageManager.stageEndEvent;
		
		if (revivalController != null)
		{
			if (revivalController.revivalCount == 0)
			{
				revivalController.OnRevivalPopup();
			}
			else
			{
				if (stageEndEvent != null)
					stageEndEvent.OnStageFailed();
			}
		}
	}
	
	public virtual void OnEquipChanged()
	{
		Debug.Log("PlayerController ... OnEquipChanged...");
		
		EquipManager equipManager = null;
		if (lifeManager != null)
			equipManager = lifeManager.equipManager;
		
		List<EquipInfo> equipItems = null;
		if (equipManager != null)
			equipItems = equipManager.equipItems;
		
		if (equipItems != null)
		{
			int weaponID = -1;
			int bodyID = -1;
			int backID = -1;
			int headID = -1;
			
			foreach(EquipInfo info in equipItems)
			{
				switch(info.slotType)
				{
				case GameDef.eSlotType.Weapon:
					if (info.item != null)
						weaponID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Body:
					if (info.item != null)
						bodyID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Back:
					if (info.item != null)
						backID = info.item.itemInfo.itemID;
					break;
				case GameDef.eSlotType.Costume_Head:
					if (info.item != null)
						headID = info.item.itemInfo.itemID;
					break;
				}
			}
			
			if (lifeManager != null)
			{
				lifeManager.ChangeCostume(bodyID, headID, backID);
				lifeManager.ChangeWeapon(weaponID);
			}
		}
	}
	
	public virtual string GetCurMasteryInfo_New(MasteryInfo_New info)
	{
		string infoStr = "";
		
		if (info == null || info.manager == null ||
			info.Point >= info.maxPoint)
			return infoStr;
		
		float masteryValue = (info.incValue * info.curPoint);
		if (info.unitString == "%")
			masteryValue *= 100.0f;
		
		infoStr = string.Format(info.formatString, masteryValue, info.unitString);
		
		return infoStr;
	}
	
	public virtual string GetNextMasteryInfo_New(MasteryInfo_New info)
	{
		string infoStr = "";
		
		if (info == null || info.manager == null ||
			info.Point >= info.maxPoint)
			return infoStr;
		
		float masteryValue = (info.incValue * info.curPoint) + info.incValue;
		if (info.unitString == "%")
			masteryValue *= 100.0f;
		
		infoStr = string.Format(info.formatString, masteryValue, info.unitString);
		
		return infoStr;
	}
	
	/////////////////////////////////////////////////////////////////////////////////////////////////
	public void ChangeMoveDir(Vector3 targetPos)
	{
		ChangeMoveDir(targetPos, false);
	}
	
	public void ChangeMoveDir(Vector3 targetPos, bool bForce)
	{
		if (moveCheckDelayTime > 0.0f)
			return;
		
		Vector3 thisPos = this.transform.position;
		
		float diffX = targetPos.x - thisPos.x;
		
		if (Mathf.Abs(diffX) <= 0.01f)
			return;
		
		Vector3 newDir = Vector3.zero;
		if (this.moveController != null)
		{
			newDir = this.moveController.moveDir;
		
			if (diffX < 0.0f)
				newDir = Vector3.left;
			else
				newDir = Vector3.right;
			
			if (newDir != this.moveController.moveDir)
				moveCheckDelayTime = moveCheckCoolTime;
			
			if (bForce == false)
			{
				switch(stateController.currentState)
				{
				case BaseState.eState.Stand:
				case BaseState.eState.Run:
				case BaseState.eState.Dash:
					this.moveController.ChangeMoveDir(newDir);
					break;
				}
			}
			else
			{
				this.moveController.ChangeMoveDir(newDir);
			}
		}
	}
	
	public void ChangeMoveDir(Transform target)
	{
		if (target == null)
			return;
		
		ChangeMoveDir(target.transform.position, false);
	}
	
	
	public bool isRunawayMode = false;
	public float hpCheckCoolTime = 10.0f;
	public float hpCheckDelayTime = 0.0f;
	public float runAwayRate = 0.96f;
	public float adjustRunAwayRate = 0.06f;
	
	public Vector2 runAwayTime = Vector2.zero;
	public float runAwayDelayTime = 0.0f;
	public void UpdateRunAway()
	{
		if (isRunawayMode == true)
		{
			if (runAwayDelayTime <= 0.0f)
				isRunawayMode = false;
			else
				runAwayDelayTime -= Time.deltaTime;
		}
	}
	
	public void UpdateHPCheck()
	{
		//HP CheckTimer
		if (hpCheckDelayTime <= 0.0f)
		{
			bool isAvailableState = false;
			switch(stateController.currentState)
			{
			case BaseState.eState.Stand:
			case BaseState.eState.Run:
			case BaseState.eState.Dash:
				isAvailableState = true;
				break;
			}
			if (isAvailableState == false)
				return;
			
			if (targetInfo != null)
			{
				float myHPRate = lifeManager != null ? lifeManager.GetHPRate() : 0.0f;
				if (myHPRate <= 0.5f)
				{
					LifeManager targetLifeManager = targetInfo.GetLifeManager();
					float targetHPRate = targetLifeManager != null ? targetLifeManager.GetHPRate() : 0.0f;
					
					//상태방 체력비율이 더 많을때 확률로 도망 모드 설정.
					if (targetHPRate > myHPRate && isAutoMode == false)
					{
						int randValue = Random.Range(0, 100);
						float adjustValue = (float)myRankValue * adjustRunAwayRate;
						int limitRateValue = Mathf.RoundToInt((runAwayRate - adjustValue) * 100.0f);
						if (randValue <= limitRateValue)
						{
							isRunawayMode = true;
							runAwayDelayTime = Random.Range(runAwayTime.x, runAwayTime.y);
							
							runawayTargetWayMgr = FindRunAwayTarget(myInfo, targetInfo);
							
							DoRunOrDash();
						}
						else
						{
							runawayTargetWayMgr = null;
							isRunawayMode = false;
						}
					}
				}
				
				hpCheckDelayTime = hpCheckCoolTime;
			}
		}
		else
			hpCheckDelayTime -= Time.deltaTime;
	}
	
	public float arrowEvadeDistance = 3.0f;
	public float projectileEvadeDistance = 2.5f;
	public float projectileEvadeRateByJump = 0.38f;
	public float adjustProjectileEvadeRateByJump = 0.03f;
	public float projectileEvadeRateBySkill02 = 0.15f;
	public void UpdateEvadeProjectile()
	{
		bool isAvailableState = false;
		switch(stateController.currentState)
		{
		case BaseState.eState.Dash:
		case BaseState.eState.Run:
		case BaseState.eState.Stand:
			isAvailableState = true;
			break;
		}
		
		if (isAvailableState == false)
			return;
		
		WeaponManager weaponManager = WeaponManager.Instance;
		if (weaponManager != null)
		{
			LifeManager weaponOwner = null;
			ActorInfo weaponActor = null;
			
			Vector3 weaponPos = Vector3.zero;
			Vector3 myPos = this.gameObject.transform.position;
			Vector3 vDiff = Vector3.zero;
			Vector3 weaponMoveDir = Vector3.zero;
			
			bool canJump = false;
			bool canSkill02 = false;
			
			if (lifeManager.stunDelayTime > 0.0f)
				return;
			
			int randValue = Random.Range(0, 100);
			float adjustRate = ((float)myRankValue * adjustProjectileEvadeRateByJump);
			int jumpRateValue = Mathf.RoundToInt(projectileEvadeRateByJump * 100.0f - adjustRate);
			int skill02RateValue = Mathf.RoundToInt(projectileEvadeRateBySkill02 * 100.0f);
			if (randValue < jumpRateValue)
			{
				if (IsJumpState() == false)
					canJump = true;
			}
			else if (randValue < skill02RateValue)
			{
				if (stateController.currentState != BaseState.eState.Skill01 && IsJumpState() == false &&
					GetSkill2CoolTimeRate() <= 0.0f && CheckStateRequireAbilityValue(BaseState.eState.Skill02) == true)
				canSkill02 = true;
			}
			
			if (canJump == false && canSkill02 == false)
				return;
			
			foreach(BaseWeapon weapon in weaponManager.projectiles)
			{
				if (weapon == null || weapon.attackInfo == null)
					continue;
				
				weaponOwner = weapon.attackInfo.ownerActor;
				weaponActor = weaponOwner != null ? weaponOwner.myActorInfo : null;
				
				if (weaponActor == null)
					continue;
				
				if (weaponActor.myTeam == myInfo.myTeam)
					continue;
				
				weaponMoveDir = weapon.GetMoveDir();
				if (weaponMoveDir == Vector3.zero)
					continue;
				
				weaponPos = weapon.gameObject.transform.position;
				vDiff = myPos - weaponPos;
				
				if (Mathf.Abs(weaponMoveDir.x) > Mathf.Abs(weaponMoveDir.y))
				{
					if (Mathf.Abs(vDiff.x) > arrowEvadeDistance)
						continue;
				
					weaponMoveDir.y = weaponMoveDir.z = 0.0f;
					vDiff.y = vDiff.z = 0.0f;
					
					weaponMoveDir = weaponMoveDir.normalized;
					vDiff = vDiff.normalized;
					
					if (vDiff.x != weaponMoveDir.x || Random.Range(1, 10) > 3)
						continue;
				}
				else
				{
					if (Mathf.Abs(vDiff.y) > projectileEvadeDistance)
						continue;
				
					weaponMoveDir.x = weaponMoveDir.z = 0.0f;
					vDiff.x = vDiff.z = 0.0f;
					
					weaponMoveDir = weaponMoveDir.normalized;
					vDiff = vDiff.normalized;
					
					if (vDiff.y != weaponMoveDir.y)
						continue;
				}
				
				if (canJump == true)
				{
					moveController.moveSpeed = moveController.defaultMoveSpeed;
					moveController.DoJump();
					stateController.ChangeState(BaseState.eState.JumpStart);
					break;
				}
				else if (canSkill02 == true)
				{
					stateController.ChangeState(BaseState.eState.Skill02);
					break;
				}
			}
		}
	}
	
	public void UpdateAttackInfo()
	{
		foreach(BaseAttackInfo info in attackList)
		{
			info.UpdateCoolTime();
		}
		
		counterAttackInfo.UpdateCoolTime();
		//blockAttackInfo.UpdateCoolTime();
	}
	
	public void UpdateAIMode()
	{
		moveCheckDelayTime -= Time.deltaTime;
		switch(stateController.currentState)
		{
		case BaseState.eState.Stage_clear1:
		case BaseState.eState.Stage_clear2:
			return;
		}
		
		UpdateAttackInfo();
		
		UpdateHPCheck();
		UpdateRunAway();
		
		UpdateEvadeProjectile();
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Stand:
			UpdateIdle();
			break;
		case BaseState.eState.Run:
		case BaseState.eState.Dash:
			UpdateMove();
			break;
		case BaseState.eState.Attack1:
		case BaseState.eState.Attack2:
			break;
		case BaseState.eState.Attack3_Base:
		case BaseState.eState.Attack3_Focus:
			break;
		case BaseState.eState.Attack3_1_Ready:
			break;
		case BaseState.eState.JumpStart:
		case BaseState.eState.JumpFall:
			break;
		case BaseState.eState.Evadestart:
		case BaseState.eState.Evadeend:
			UpdateEvade();
			break;
		case BaseState.eState.Damage:
			break;
		case BaseState.eState.Stun:
			UpdateStun();
			break;
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			UpdateDie();
			break;
		}
		
		UpdateAttackCheck();
	}
	
	public float attackCheckCoolTime = 1.0f;
	private float attackCheckDelayTime = 0.0f;
	public float moveCheckCoolTime = 1.0f;
	private float moveCheckDelayTime = 0.0f;
	public void UpdateAttackCheck()
	{
		if (attackCheckDelayTime <= 0.0f)
		{
			WayPointManager targetWayMgr = BaseMonster.GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = BaseMonster.GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			//Vector3 myPos = this.transform.position;
			Vector3 targetPos = this.transform.position;
			float targetRadius = 0.0f;
			
			if (targetInfo != null)
			{
				targetPos = targetInfo.transform.position;
				targetRadius = targetInfo.colliderRadius;
			}
			
			BaseAttackInfo attackInfo = null;
			if (targetInfo != null && CheckAttackTarget(targetInfo) == true)
				attackInfo = ChooseAttackIndex(targetPos, targetRadius, bSameGround);
			
			if (attackInfo != null)
			{
				ChangeMoveDir(targetPos);
				
				stateController.ChangeState(attackInfo.attackState);
				attackInfo.ResetCoolTime();
				
				switch(attackInfo.attackState)
				{
				case BaseState.eState.Drop:
					if (moveController != null)
						moveController.DoFall();
					break;
				case BaseState.eState.Skill01:
					skill1DelayTime = skill1CoolTime + skill1CoolTimeAdjust;
					break;
				case BaseState.eState.Skill02:
					skill2DelayTime = skill2CoolTime;
					break;
				case BaseState.eState.AttackB_1:
				case BaseState.eState.AttackB_2:
				case BaseState.eState.AttackB_3:
					actionBDelayTime = actionBCoolTime + actionBCoolTimeAdjust;
					break;
				}
				attackTargetInfo = targetInfo;
			}
			
			attackCheckDelayTime = attackCheckCoolTime;
		}
		else
		{
			attackCheckDelayTime -= Time.deltaTime;
		}
	}
	
	public float jumpRateAfterAttack2 = 0.76f;
	public float adjustJumpRateAfterAttack2 = 0.06f;
	public float jumpRateAfterDashAttack = 0.8f;
	public float adjustJumpRateAfterDashAttack = 0.05f;
	public float jumpRateAfterSkill02 = 0.8f;
	public float adjustJumpRateAfterSkill02 = 0.05f;
	
	public bool bJumpTriggerByTargetHit = false;
	public AttackStateInfo targetHitInfo = null;
	public LifeManager targetHitActor = null;
	public virtual void OnTargetHit(LifeManager hitActor, float damage, bool isCritical, AttackStateInfo attackInfo)
	{
		if (comboCounter != null)
			comboCounter.AddComboCount();
		
		if (isAIMode == false)
			return;
		
		switch(attackInfo.attackState)
		{
		case BaseState.eState.Attack2:
		case BaseState.eState.Dashattack:
		case BaseState.eState.Skill02:
			bJumpTriggerByTargetHit = true;
			targetHitInfo = attackInfo;
			targetHitActor = hitActor;
			break;
		}
	}
	
	public Vector2 limitFollowRate = new Vector2(0.6f, 0.8f);
	public float limitFollowLength = 2.0f;
	
	[HideInInspector]
	public float followDelayTime = 0.5f;
	public float followCoolTime = 0.5f;
	
	public float limitObstacleLength = 7.0f;
	public float limitTargetLength = 5.0f;
	
	public Path targetPath = new Path();
	
	public int dashRateValue = 76;
	public float adjustDashRate = 0.06f;
	public int dashRateValueWhenRunawayMode = 96;
	public float adjustDashRateValueWhenRunawayMode = 0.06f;
	public void DoRunOrDash()
	{
		int dashRate = Random.Range(0, 100);
		int adjustRate = 0;
		
		bool isDash = false;
		if (isRunawayMode == true)
		{
			adjustRate = Mathf.RoundToInt(adjustDashRateValueWhenRunawayMode * (float)myRankValue);
			isDash = dashRate < (dashRateValueWhenRunawayMode - adjustRate);
		}
		else
		{
			adjustRate = Mathf.RoundToInt(adjustDashRate * (float)myRankValue);
			isDash = dashRate < (dashRateValue - adjustRate);
		}
		
		if (isDash == true)
			stateController.ChangeState(BaseState.eState.Dash);
		else
			stateController.ChangeState(BaseState.eState.Run);
	}
	
	public float autoPotion1Rate = 0.6f;
	public float autoPotion2Rate = 0.4f;
	
	public void UpdateAutoPotion(float hpRate)
	{
		if (hpRate < autoPotion2Rate && GetPotion2CoolTimeRate() <= 0.0f)
			playerControls.OnPotion2Click();
		else if (hpRate < autoPotion1Rate)
			playerControls.OnPotion1Click();
	}
	
	public void UpdateIdle()
	{
		followDelayTime -= Time.deltaTime;
		
		if (targetInfo != null)
		{
			string animName = GetStandAnimation();
			if (animName != "")
				stateController.animationController.ChangeAnimation(animName);
		}
		else
		{
			stateController.animationController.ChangeAnimation(stateController.curStateInfo.baseState.animationClip);
		}
		
		if (this.moveController != null && targetInfo != null)
		{
			WayPointManager targetWayMgr = BaseMonster.GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = BaseMonster.GetWayPointManager(this.moveController.groundCollider);
			
			Vector3 targetPos = targetInfo.transform.position;
			
			float diffX = BaseMonster.CalcDiffX(myInfo, targetInfo);
			bool isJump = false;
			
			BaseAttackInfo attackInfo = null;//ChooseAttackIndex(targetPos, targetRadiuse, bSameGround);
			
			if (attackInfo != null)
			{
				ChangeMoveDir(targetPos);
				
				stateController.ChangeState(attackInfo.attackState);
				attackInfo.ResetCoolTime();
				
				attackTargetInfo = targetInfo;
				return;
			}
			
			if (moveController != null && isJump == true)
			{
				stateController.ChangeState(BaseState.eState.JumpStart);
				
				moveController.moveSpeed = moveController.defaultMoveSpeed;
				moveController.DoJump();
				return;
			}

			if (targetWayMgr != null && targetPath.target == curWayMgr)
			{
				targetPos = targetInfo.transform.position;
				
				ChangeMoveDir(targetPos);
				
				if (followDelayTime <= 0.0f)
				{
					int randValue = Random.Range(0, 100);
					int rateStart = Mathf.RoundToInt(limitFollowRate.x * 100.0f);
					int rateEnd = Mathf.RoundToInt(limitFollowRate.y * 100.0f);
					
					int rateValue = Random.Range(rateStart, rateEnd);
					
					if (limitFollowRate == Vector2.zero || randValue < rateValue || diffX > this.limitFollowLength)
					{
						DoRunOrDash();
					}
					
					followDelayTime = followCoolTime;
				}
				else
				{
					if (diffX > this.limitFollowLength)
					{
						DoRunOrDash();
					}
				}
			}
			else if (curWayPoint != null)
			{
				Bounds wayPointArea = curWayInfo.area.bounds;
				//Bounds myBound = moveController.collider.bounds;
				
				Vector3 areaHalfSize = curWayInfo.area.size * 0.5f;
				Vector3 myPos = this.transform.position;
				
				targetPos = curWayInfo.area.transform.position;
				
				if (moveController.moveDir == Vector3.right)
					targetPos = wayPointArea.center + wayPointArea.extents;
				else
					targetPos = wayPointArea.center - wayPointArea.extents;
				
				ChangeMoveDir(targetPos, true);
				
				bool bIntersectArea = wayPointArea.Contains(myPos);
				if (bIntersectArea == true)
				{
					Vector3 start = wayPointArea.center - areaHalfSize;
					Vector3 end = wayPointArea.center + areaHalfSize;
					
					float startX = start.x;
					float endX = end.x;
					
					float pos = myPos.x;
					float rate = 0.0f;
					int rateValue = 0;
					
					int randValue = Random.Range(10, 90);
					if (moveController.moveDir == Vector3.right)
					{
						rate = (pos - startX) / (endX - startX);
						rateValue = Mathf.RoundToInt(rate * 100.0f);
					}
					else
					{
						rate = (pos - endX) / (startX - endX);
						rateValue = Mathf.RoundToInt(rate * 100.0f);
					}
					
					bIntersectArea = randValue < rateValue;
				}
				
				if (bIntersectArea == true)
				{
					if (moveController != null)
						moveController.moveSpeed = 0.0f;
					stateController.ChangeState(BaseState.eState.Stand);
					
					if (this.moveController != null)
						this.moveController.ChangeMoveDir(curWayInfo.vDir);
					
					List<BaseState.eState> stateList = BaseMonster.GetWayInfoStateList(curWayInfo.wayTypeMask);
					int nCount = stateList.Count;
					int randIndex = -1;
					if (nCount > 0)
						randIndex = Random.Range(0, nCount);
					
					BaseState.eState nextState = stateController.currentState;
					if (randIndex != -1)
						nextState = stateList[randIndex];
					
					//if (stateController.currentState != nextState)
					{
						Debug.Log("WayPoint Area ChangeState : " + nextState);
						
						if (nextState == BaseState.eState.JumpStart)
						{
							if (curWayInfo.vDir == Vector3.up)
								moveController.moveSpeed = 0.0f;
							else
								moveController.moveSpeed = moveController.defaultMoveSpeed;
							
							moveController.DoJump();
						}
						stateController.ChangeState(nextState);
					}
					
					
					if (targetPath.pathList.Count > 0)
						targetPath.pathList.RemoveAt(0);
					
					if (targetPath.pathList.Count > 0)
					{
						curWayPoint = targetPath.pathList[0];
						curWayInfo = null;
						
						List<WayInfo> availList = new List<WayInfo>();
						
						foreach(WayInfo info in curWayPoint.wayInfoList)
						{
							if ((info.wayTypeMask & myWayTypeMask) != 0)
								availList.Add(info);
						}
						
						int nWayCount = availList.Count;
						curWayIndex = -1;
						if (nWayCount > 0)
							curWayIndex = Random.Range(0, nWayCount);
						
						if (curWayIndex >= 0 && curWayIndex < nWayCount)
							curWayInfo = availList[curWayIndex];
					}
					else
					{
						curWayIndex = -1;
						curWayInfo = null;
						curWayPoint = null;
					}
					
					return;
				}
				else
				{
					ChangeMoveDir(targetPos, true);
					
					if (followDelayTime <= 0.0f)
					{
						int randValue = Random.Range(0, 100);
						int rateStart = Mathf.RoundToInt(limitFollowRate.x * 100.0f);
						int rateEnd = Mathf.RoundToInt(limitFollowRate.y * 100.0f);
						
						int rateValue = Random.Range(rateStart, rateEnd);
						
						if (randValue < rateValue || diffX > this.limitFollowLength)
						{
							int dashRate = Random.Range(0, 100);
							if (dashRate < dashRateValue)
								stateController.ChangeState(BaseState.eState.Dash);
							else
								stateController.ChangeState(BaseState.eState.Run);
						}
						
						followDelayTime = followCoolTime;
					}
				}
			}
			else
			{
				ResetIdleGroundTargetPos();
			}
		}
		else
		{
			ResetIdleGroundTargetPos();
		}
	}
	
	
	public WayPointManager runawayTargetWayMgr = null;
	public Vector3 runAwayPos = Vector3.zero;
	public WayPointManager FindRunAwayTarget(ActorInfo my, ActorInfo target)
	{
		WayPointManager wayPointMgr = null;
		StageManager stageManager = Game.Instance.stageManager;
		
		Vector3 targetPos = Vector3.zero;
		Vector3 myPos = Vector3.zero;
		Vector3 vDiff = Vector3.zero;
		
		if (my != null)
			myPos = myInfo.gameObject.transform.position;
		if (target != null)
			targetPos = target.gameObject.transform.position;
		
		vDiff = targetPos - myPos;
		
		
		if (stageManager != null)
		{
			float maxDist = float.MinValue;
			float xDiff = 0.0f;
			GroundInfo targetGround = null;
			
			foreach(GroundInfo info in stageManager.groundInfos)
			{
				BoxCollider groundBox = info.groundObject.GetComponent<BoxCollider>();
				
				Bounds groundArea = groundBox.bounds;
			
				Vector3 areaHalfSize = groundBox.size * 0.5f;
				
				Vector3 minPos = groundArea.center - areaHalfSize;
				Vector3 maxPos = groundArea.center + areaHalfSize;
				
				if (vDiff.x < 0.0f)
				{
					xDiff = Mathf.Abs(minPos.x - myPos.x);
					if (xDiff > maxDist)
					{
						maxDist = xDiff;
						targetGround = info;
						
						runAwayPos = new Vector3(minPos.x + myInfo.colliderRadius, maxPos.y, 0.0f);
					}
				}
				else
				{
					xDiff = Mathf.Abs(maxPos.x - myPos.x);
					if (xDiff > maxDist)
					{
						maxDist = xDiff;
						targetGround = info;
						
						runAwayPos = new Vector3(maxPos.x - myInfo.colliderRadius, maxPos.y, 0.0f);
					}
				}
			}
			
			if (targetGround != null)
			{
				wayPointMgr = targetGround.groundObject.GetComponent<WayPointManager>();
			}
		}
		
		return wayPointMgr;
	}
	
	public float jumpCoolTime = 1.0f;
	public float jumpDelayTime = 0.0f;
	public int jumpRateOnNormalMode = 32;
	public int jumpRateOnRunawayMode = 30;
	public float adjustJumpRate = 0.02f;
	
	public void UpdateMove()
	{	
		if (this.moveController != null)
		{
			WayPointManager targetWayMgr = null;
			
			if (isRunawayMode == true )
			{
				if (runawayTargetWayMgr == null)
					runawayTargetWayMgr = FindRunAwayTarget(myInfo, targetInfo);
				
				targetWayMgr = runawayTargetWayMgr;
			}
			else
			{
				targetWayMgr = BaseMonster.GetWayPointManager(targetInfo);
			}
			
			WayPointManager curWayMgr = BaseMonster.GetWayPointManager(this.moveController.groundCollider);
			
			//bool bSameGround = targetWayMgr == curWayMgr;
			
			Vector3 myPos = this.transform.position;
			Vector3 targetPos = this.transform.position;
			float targetRadius = 0.0f;
			
			if (isRunawayMode == true)
			{
				
			}
			else
			{
				if (targetInfo != null)
				{
					targetPos = targetInfo.transform.position;
					targetRadius = targetInfo.colliderRadius;
				}
			}
			
			bool isJump = false;
			
			Vector3 obstaclePos = targetPos;
			
			if (isAIMode == true)
			{
				jumpDelayTime -= Time.deltaTime;
				if (jumpDelayTime <= 0.0f)
				{
					int jumpRnadValue = Random.Range(0, 100);
					//int adjustRateValue = 0;
					int jumpRateValue = 0;
					if (isRunawayMode == true)
					{
						//adjustRateValue = Mathf.RoundToInt(adjustJumpRate * 100.0f);
						jumpRateValue = jumpRateOnRunawayMode;
					}
					else
					{
						//adjustRateValue = Mathf.RoundToInt(adjustJumpRate * 100.0f);
						jumpRateValue = jumpRateOnRunawayMode;
					}
					
					if (jumpRnadValue < jumpRateValue)
						isJump = true;
				}
			}
			
			if (isAIMode == true && moveController != null)
			{
				if(isRunawayMode == true)
				{
					if (moveController != null && moveController.CheckObstacle(out obstaclePos, limitObstacleLength) == true)
					{
						targetPos = obstaclePos;
						
						if (stateController.IsContainState(BaseState.eState.JumpStart) == true)
						{
							Vector3 diffOb = obstaclePos - this.transform.position;
							if (Mathf.Abs(diffOb.x) < 1.1f)
								isJump = true;
						}
					}
				}
				else if(targetInfo != null && 
					(stateController.currentState == BaseState.eState.Run || stateController.currentState == BaseState.eState.Dash) )
				{
					if (moveController.CheckObstacleMonster(out obstaclePos, targetInfo.gameObject, limitObstacleLength) == true)
					{
						targetPos = obstaclePos;
						
						if (stateController.IsContainState(BaseState.eState.JumpStart) == true)
						{
							Vector3 diffOb = obstaclePos - this.transform.position;
							if (Mathf.Abs(diffOb.x) < 1.1f)
							{
								float randVal = Random.Range(0.0f, 1.0f);
								if (randVal > 0.5f)
									ChangeTarget(targetInfo);
								else if (randVal > 0.4f)
									isJump = true;
								else
								{
									stateController.ChangeState(BaseState.eState.Dashattack);
									return;
								}
							}
						}
					}
				}					
			}
			
			if (moveController != null && isJump == true)
			{
				jumpDelayTime = jumpCoolTime;
				
				stateController.ChangeState(BaseState.eState.JumpStart);
						
				moveController.moveSpeed = moveController.defaultMoveSpeed;
				moveController.DoJump();
				return;
			}
			
			if (isRunawayMode == true)
			{
				if (targetPath.target != targetWayMgr)
					UpdateTargetPath(targetWayMgr);
			}
			else
			{
				if (targetPath.target != targetWayMgr && targetInfo != null)
					UpdateTargetPath(targetInfo.gameObject);
			}
			
			if (targetWayMgr != null && 
				curWayMgr != null &&
				targetPath.target == curWayMgr)
			{
				if (isRunawayMode == true)
					targetPos = runAwayPos;
				else
					targetPos = targetInfo.transform.position;
				
				ChangeMoveDir(targetPos);
				
				Vector3 diff = targetPos - myPos;
				float diffX = Mathf.Abs(diff.x) - (myInfo.colliderRadius + targetRadius);
				if (diffX <= 0.05f)
				{
					stateController.ChangeState(BaseState.eState.Stand);
				}
			}
			else if (curWayPoint != null)
			{
				Bounds wayPointArea = curWayInfo.area.bounds;
				//Bounds myBound = moveController.collider.bounds;
				
				Vector3 areaHalfSize = curWayInfo.area.size * 0.5f;
				
				targetPos = curWayInfo.area.transform.position;
				
				if (moveController.moveDir == Vector3.right)
					targetPos = wayPointArea.center + areaHalfSize;
				else
					targetPos = wayPointArea.center - areaHalfSize;
				
				ChangeMoveDir(targetPos, true);
				
				bool bIntersectArea = wayPointArea.Contains(myPos);
				if (bIntersectArea == true)
				{
					Vector3 start = wayPointArea.center - areaHalfSize;
					Vector3 end = wayPointArea.center + areaHalfSize;
					
					float startX = start.x;
					float endX = end.x;
					
					float pos = myPos.x;
					float rate = 0.0f;
					int rateValue = 0;
					
					int randValue = Random.Range(10, 90);
					if (moveController.moveDir == Vector3.right)
					{
						rate = (pos - startX) / (endX - startX);
						rateValue = Mathf.RoundToInt(rate * 100.0f);
					}
					else
					{
						rate = (pos - endX) / (startX - endX);
						rateValue = Mathf.RoundToInt(rate * 100.0f);
					}
					
					bIntersectArea = randValue < rateValue;
				}
				
				if (bIntersectArea == true)
				{
					if (this.moveController != null)
					{
						this.moveController.ChangeMoveDir(curWayInfo.vDir);
					
						this.moveController.moveSpeed = 0.0f;
					}
					
					stateController.ChangeState(BaseState.eState.Stand);
					
					if (curWayInfo == null)
						return;
					
					List<BaseState.eState> stateList = BaseMonster.GetWayInfoStateList(curWayInfo.wayTypeMask);
					int nCount = stateList.Count;
					int randIndex = -1;
					if (nCount > 0)
						randIndex = Random.Range(0, nCount);
					
					BaseState.eState nextState = stateController.currentState;
					if (randIndex != -1)
						nextState = stateList[randIndex];
					
					//if (stateController.currentState != nextState)
					{
						Debug.Log("WayPoint Area ChangeState : " + nextState);
						
						if (nextState == BaseState.eState.JumpStart)
						{
							if (curWayInfo.vDir == Vector3.up)
								moveController.moveSpeed = 0.0f;
							else
								moveController.moveSpeed = moveController.defaultMoveSpeed;
							
							moveController.DoJump();
						}
						stateController.ChangeState(nextState);
					}
					
					//targetPath.DebugInfo();
					
					if (targetPath.pathList.Count > 0 && curWayPoint.target == curWayMgr)
					{
						curWayPoint = targetPath.pathList[0];
						curWayInfo = null;
						
						List<WayInfo> availList = new List<WayInfo>();
						bool hasJump = (myWayTypeMask & (1 << (int)WayPoint.eWayType.Jump)) != 0;
						
						if (hasJump == true)
						{
							foreach(WayInfo info in curWayPoint.wayInfoList)
							{
								if ((info.wayTypeMask & myWayTypeMask) != 0)
									availList.Add(info);
							}
						}
						
						int nWayCount = availList.Count;
						curWayIndex = -1;
						if (nWayCount > 0)
							curWayIndex = Random.Range(0, nWayCount);
						
						if (curWayIndex >= 0 && curWayIndex < nWayCount)
							curWayInfo = availList[curWayIndex];
					}
					
					if (targetPath.pathList.Count == 0)
					{
						curWayIndex = -1;
						curWayInfo = null;
						curWayPoint = null;
					}

					return;
				}
			}
			else
			{
				if (bSetIdleTargetPos == true)
				{
					targetPos = idleTargetPos;
					
					float randTimeRate = Random.Range(0.5f, 1.5f);
					idleTargetTime = idleTargetPosResetTime * randTimeRate;
				}
				else
					targetPos = myPos;
				
				ChangeMoveDir(targetPos);
				
				
				Vector3 diff = targetPos - myPos;
				
				float diffX = Mathf.Abs(diff.x);
				
				if (diffX < moveController.colliderRadius)
				{
					ChangeMoveDir(targetPos);
					//Debug.Log("Idle Target Pos complete.....");
					stateController.ChangeState(BaseState.eState.Stand);
				}
			}
		}
	}
	
	public void UpdateEvade()
	{
		
	}
	
	public void UpdateDie()
	{
		
	}
	
	public virtual string GetStandAnimation()
	{
		CharStateInfo stateInfo = null;
		if (stateController != null && stateController.stateList != null)
			stateInfo = stateController.stateList.GetState(BaseState.eState.Stand);
		
		if (stateInfo == null)
			return "";
		
		string origAnimName = stateInfo.baseState.animationClip;
		string newAnimName = origAnimName;
		
		if (targetInfo != null)
		{
			float myHeight = 1.0f;
			Vector3 myPos = this.transform.position;
			if (moveController != null)
				myHeight = moveController.colliderHeight;
			
			float targetHeight = 1.0f;
			Vector3 targetPos = targetInfo.transform.position;
			BaseMoveController targetMoveController = targetInfo.GetMoveController();
			if (targetMoveController != null)
				targetHeight = targetMoveController.colliderHeight;
			
			Vector3 diff = targetPos - myPos;
			if (diff.y > myHeight)
				newAnimName = stateController.standUpAnim;
			else if (diff.y + targetHeight < 0.0f)
				newAnimName = stateController.standDownAnim;
			
			bool checkAnim = false;
			foreach(AnimationState animState in stateController.animationController.anim)
			{
				if (animState.name == newAnimName)
				{
					checkAnim = true;
					break;
				}
			}
			
			if (checkAnim == false)
				newAnimName = origAnimName;
		}
		
		return newAnimName;
	}
	
	public WayPoint curWayPoint = null;
	public int curWayIndex = -1;
	public WayInfo curWayInfo = null;
	
	public int myWayTypeMask = 0;
	public void UpdateTargetPath(GameObject target)
	{
		BaseMoveController targetMoveController = target.GetComponent<BaseMoveController>();
		WayPointManager targetWayMgr = null;
		if (targetMoveController != null)
		{
			if (targetMoveController.groundCollider != null)
				targetWayMgr = targetMoveController.groundCollider.gameObject.GetComponent<WayPointManager>();
		}
		
		UpdateTargetPath(targetWayMgr);
	}
	
	public void UpdateTargetPath(WayPointManager targetWayMgr)
	{
		//Debug.Log("UpdateTargetPath...");
		
		if (this.moveController != null)
		{
			if (targetWayMgr != null)
			{
				WayPointManager curWayPointMgr = BaseMonster.GetWayPointManager(this.moveController.groundCollider);
				if (curWayPointMgr == null)
					return;
				
				bool bNonavailablePath = false;
				if (targetPath.CheckPath(curWayPointMgr) == false)
					bNonavailablePath = true;
				else if (targetPath.target != targetWayMgr)
					bNonavailablePath = true;
				
				if (bNonavailablePath == true)
				{
					targetPath.findPath = false;
					targetPath.pathList.Clear();
		
					WayPointManager curWayMgr = this.moveController.groundCollider.GetComponent<WayPointManager>();
					if (curWayMgr != null)
						curWayMgr.FindPath(targetWayMgr, targetPath, myWayTypeMask);
				}
				
				if (targetPath.pathList.Count == 0)
					targetPath.findPath = false;
				
				if (targetPath.findPath == true)
				{
					//targetPath.DebugInfo();
					
					curWayPoint = targetPath.pathList[0];
					
					
					int nWayCount = curWayPoint.wayInfoList.Count;
					curWayIndex = -1;
					if (nWayCount > 0)
						curWayIndex = Random.Range(0, nWayCount);
					
					if (curWayIndex >= 0 && curWayIndex < nWayCount)
						curWayInfo = this.curWayPoint.wayInfoList[curWayIndex];
				}
				else
				{
					curWayInfo = null;
					curWayPoint = null;
					curWayIndex = -1;
				}
			}
		}
	}
	
	public bool bSetIdleTargetPos = false;
	public Vector3 idleTargetPos = Vector3.zero;
	public float idleTargetPosResetTime = 1.0f;
	private float idleTargetTime = 0.0f;
	
	public float limitTargetFarLength = 10.0f;
	public void ResetIdleGroundTargetPos()
	{
		if (moveController == null || moveController.groundCollider == null)
			return;
		
		if (idleTargetTime <= 0.0f)
		{
			BoxCollider groundBox = (BoxCollider)moveController.groundCollider;
			Bounds groundArea = groundBox.bounds;
			
			Vector3 areaHalfSize = groundBox.size * 0.5f;
			Vector3 myPos = this.transform.position;
			
			Vector3 minPos = groundArea.center - areaHalfSize;
			Vector3 maxPos = groundArea.center + areaHalfSize;
			Vector3 myCenterPos = myPos + (Vector3.up * 0.5f);
			
			RaycastHit hitInfo;
			int layerMaskValue = moveController.layerMask;// ^ moveController.enemyLayerMask;
			if (Physics.Raycast(myCenterPos, Vector3.right, out hitInfo, float.MaxValue, layerMaskValue) == true)
			{
				if (hitInfo.point.x < maxPos.x)
					maxPos.x = hitInfo.point.x - moveController.colliderRadius;
			}
			
			if (Physics.Raycast(myCenterPos, Vector3.left, out hitInfo, float.MaxValue, layerMaskValue) == true)
			{
				if (hitInfo.point.x > minPos.x)
					minPos.x = hitInfo.point.x + moveController.colliderRadius;
			}
			
			float limitMinX = minPos.x;
			float limitMaxX = maxPos.x;
			Vector3 targetPos = myPos;
			
			if (targetInfo != null)
			{
				targetPos = targetInfo.transform.position;
			}
			else
			{
				ActorInfo playerInfo = null;
				ActorManager actorManager = ActorManager.Instance;
				if (actorManager != null)
					playerInfo = actorManager.playerInfo;
				
				if (playerInfo != null && playerInfo.myTeam != this.lifeManager.myActorInfo.myTeam)
					targetPos = playerInfo.transform.position;
				else
					targetPos = this.gameObject.transform.position + (Vector3.right * 3.0f);
			}
			
			limitMinX = targetPos.x - limitTargetFarLength;
			limitMaxX = targetPos.x + limitTargetFarLength;
			
			float minX = Mathf.Clamp(limitMinX, minPos.x, maxPos.x);
			float maxX = Mathf.Clamp(limitMaxX, minPos.x, maxPos.x);
			
			List<float> targetPosList = new List<float>();
			targetPosList.Add(minX);
			targetPosList.Add(maxX);
			
			float targetX = myPos.x;
			if (targetInfo != null)
			{
				BaseMonster.GetTargetGroundPos(targetInfo, targetPosList, minX, maxX);
			
				int nCount = targetPosList.Count;
				int randIndex = Random.Range(0, nCount);
				targetX = targetPosList[randIndex];
			}
			
			targetPos.x = targetX;
			
			ChangeMoveDir(targetPos);
			if (myInfo.myTeam == ActorInfo.TeamNo.Team_Two)
				targetX -= moveController.moveDir.x * moveController.colliderRadius;
			else
				targetX += moveController.moveDir.x * moveController.colliderRadius;
			
			float randTimeRate = 1.0f;
			float diffX = Mathf.Abs(idleTargetPos.x - targetX);
			if (diffX <= 1.0f)
			{
				targetX = idleTargetPos.x;
				
				randTimeRate = Random.Range(0.5f, 1.5f);
			}
			
			diffX = Mathf.Abs(idleTargetPos.x - myPos.x);
			if (diffX > 1.0f)
			{
				stateController.ChangeState(BaseState.eState.Run);
			}
			
			idleTargetPos = new Vector3(targetX, myPos.y, myPos.z);
			idleTargetTime = idleTargetPosResetTime * randTimeRate;
			
			bSetIdleTargetPos = true;
			//Debug.Log("Idle TargetPos Setting...." + idleTargetPos);
		}
	}
	
	public List<BaseAttackInfo> attackList = new List<BaseAttackInfo>();
	public virtual bool CanAttackState(BaseAttackInfo attackInfo)
	{
		bool canAttack = false;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Stand:
		case BaseState.eState.Run:
		case BaseState.eState.Dash:
		case BaseState.eState.JumpStart:
		case BaseState.eState.JumpFall:
		case BaseState.eState.Jumpland:
		case BaseState.eState.Knockdownfall:
			canAttack = true;
			break;
		}
		
		if (attackInfo != null)
		{
			if (attackInfo.ignoreDamageState == true)
			{
				if (stateController.currentState == BaseState.eState.Damage)
					canAttack = true;
			}
		}
		
		return canAttack;
	}
	
	public int myRankValue = 0;
	public float addSpecialRateWhenStunOrDamage = 2.0f;
	public float aiSkill02RateWhenTargetSkill01 = 0.8f;
	public virtual BaseAttackInfo ChooseAttackIndex(Vector3 targetPos, float targetRadius, bool bSameGround)
	{
		BaseAttackInfo attackInfo = null;
		
		LifeManager targetLifeMgr = null;
		if (targetInfo != null)
			targetLifeMgr = targetInfo.GetComponent<LifeManager>();
		
		if (targetLifeMgr != null && targetLifeMgr.GetHP() <= 0.0f)
			return null;
		
		if (isRunawayMode == true)
			return attackInfo;
		
		//if (CanAttackState() == true)
		{
			List<BaseAttackInfo> availableAttackList = new List<BaseAttackInfo>();
			
			int randValue = Random.Range(0, 100);
			Vector3 vDiff = targetPos - this.transform.position;
			float diffX = Mathf.Max(0.0f, Mathf.Abs(vDiff.x) - (myInfo.colliderRadius + targetRadius));
			float diffY = vDiff.y;
			
			if (Mathf.Abs(diffY) < 0.1f)
				diffY = 0.0f;
			
			bool bStunState = lifeManager.stunDelayTime > 0.0f;
		
			BaseState.eState targetState = BaseState.eState.Stand;
			if (targetLifeMgr != null && targetLifeMgr.stateController != null)
				targetState = targetLifeMgr.stateController.currentState;
			
			bool checkAttackState = false;
			if (targetState == BaseState.eState.Skill01)
			{
				int rateValue = (int)(aiSkill02RateWhenTargetSkill01 * 100.0f);
				if (rateValue >= randValue)
				{
					attackInfo = GetAttack(BaseState.eState.Skill02, false);
					
					if (attackInfo != null)
					{
						checkAttackState = false; 
						if (stateController.currentState != BaseState.eState.Skill01 && IsJumpState() == false)
						{
							if (GetSkill2CoolTimeRate() <= 0.0f && CheckStateRequireAbilityValue(attackInfo.attackState) == true)
								checkAttackState = true;
						}
						
						if (checkAttackState == true)
							return attackInfo;
					}
				}
			}
			
			attackInfo = null;
			
			int nCount = attackList.Count;
			for (int i = 0; i < nCount; ++i)
			{
				BaseAttackInfo info = attackList[i];
				
				checkAttackState = false;
				switch(info.attackState)
				{
				case BaseState.eState.Attack1:
					if (bStunState == false &&
						CanAttackState(info) == true &&
						IsAttackState() ==  false && IsJumpState() == false)
						checkAttackState = true;
					break;
				case BaseState.eState.Drop:
					if (bStunState == false)
					{
						switch(stateController.currentState)
						{
						case BaseState.eState.Knockdownfall:
							checkAttackState = true;
							break;
						//case BaseState.eState.JumpStart:
						case BaseState.eState.JumpFall:
						//case BaseState.eState.Jumpland:
							if (diffY < 1.5f)
								checkAttackState = true;
							break;
						case BaseState.eState.JumpAttack:
							checkAttackState = true;
							break;
						}
					}
					break;
				case BaseState.eState.JumpAttack:
					if (bStunState == false)
					{
						switch(stateController.currentState)
						{
						case BaseState.eState.JumpStart:
						case BaseState.eState.JumpFall:
							if (input.jumpBlowAttack == true)
								checkAttackState = true;
							break;
						}
					}
					break;
				case BaseState.eState.Dashattack:
					if (bStunState == false &&
						IsSkillState() == false && 
						stateController.currentState == BaseState.eState.Dash)
						checkAttackState = true;
					break;
				case BaseState.eState.Evadestart:
				case BaseState.eState.AttackB_1:
				case BaseState.eState.AttackB_2:
				case BaseState.eState.AttackB_3:
					if (/*bStunState == false &&*/
						CanAttackState(info) == true &&
						IsSkillState() == false && IsJumpState() == false && GetActionBCoolTimeRate() <= 0.0f &&
						CheckLearnActionB(info.attackState) == true)
						checkAttackState = true;
					break;
				case BaseState.eState.Skill01:
					if (bStunState == false &&
						CanAttackState(info) == true &&
						stateController.currentState != BaseState.eState.Skill02 && IsJumpState() == false &&
						GetSkill1CoolTimeRate() <= 0.0f && CheckStateRequireAbilityValue(info.attackState) == true)
						checkAttackState = true;
					break;
				case BaseState.eState.Skill02:
					//bool isAvailableState = false;
					if ((stateController.currentState == BaseState.eState.Damage || stateController.currentState == BaseState.eState.Stun) ||
						 (stateController.currentState != BaseState.eState.Skill01 && IsJumpState() == false))
					{
						if (GetSkill2CoolTimeRate() <= 0.0f && CheckStateRequireAbilityValue(info.attackState) == true)
							checkAttackState = true;
					}
					break;
				}
				
				if (checkAttackState == false)
					continue;
				
				//경직이나 디버프가 걸린 중에는 스킬02 사용 확률이 2배.
				int TempRate = 0;
				int saveOriginValue = info.attackProbability;
				if (info.ignoreDamageState == true)
				{
					TempRate = saveOriginValue;
					
					bool isAvailableState = false;
					switch(stateController.currentState)
					{
					case BaseState.eState.Damage:
					case BaseState.eState.Stun:
						isAvailableState = true;
						break;
					}
					
					if (isAvailableState == true)
						TempRate = Mathf.RoundToInt((float)info.attackProbability * addSpecialRateWhenStunOrDamage);
					
					info.attackProbability = TempRate;
				}
				
				if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround, myRankValue) == false)
				{
					info.attackProbability = saveOriginValue;
					continue;
				}
				else
				{
					info.attackProbability = saveOriginValue;
					
					availableAttackList.Add(info);
				}
			}
			
			nCount = availableAttackList.Count;
			if (nCount > 1)
				availableAttackList.Sort(BaseAttackInfo.SortFunc);
			
			if (nCount > 0)
				attackInfo = availableAttackList[0];
		}
		
		return attackInfo;
	}
	
	public bool CheckLearnActionB(BaseState.eState attackState)
	{
		bool bLearnActionB = false;
		MasteryManager_New masteryManager = lifeManager != null ? lifeManager.masteryManager_New : null;
		
		if (masteryManager != null)
		{
			if (masteryManager.activeMastery != null && masteryManager.activeMastery.id != 0)
			{
				string[] args = masteryManager.activeMastery.methodArg.Split(';');
				BaseState.eState startState = BaseState.ToState(args[0]);
				BaseState.eState targetState = BaseState.ToState(args[1]);
				
				bool enableAbility = CheckStateRequireAbilityValue(startState);
				if ((startState == attackState || targetState == attackState) && enableAbility == true)
					bLearnActionB = true;
			}
		}
		
		return bLearnActionB;
	}
	
	public bool IsAttackState()
	{
		bool isAttack = false;
		
		BaseState.eState currentState = stateController.currentState;
		
		foreach(BaseAttackInfo info in attackList)
		{
			if (info.attackState == currentState)
			{
				isAttack = true;
				break;
			}
		}
		
		return isAttack;
	}
	
	public bool IsKockDownState()
	{
		bool isState = false;
		switch(stateController.currentState)
		{
		case BaseState.eState.Knockdownstart:
		case BaseState.eState.Knockdownfall:
		//case BaseState.eState.Knockdownland:
			isState = true;
			break;
		}
		
		return isState;
	}
	
	public bool IsJumpState()
	{
		return stateController.IsJumpState();
	}
	
	public bool IsSkillState()
	{
		bool isState = false;
		switch(stateController.currentState)
		{
		case BaseState.eState.Skill01:
		case BaseState.eState.Skill02:
			isState = true;
			break;
		}
		
		return isState;
	}
	
	public virtual void ChangeTarget(ActorInfo newTarget)
	{
		if (IsAttackState() == true)
			return;
		
		if (attackTargetInfo != null && attackTargetInfo.GetLifeManager().GetHP() > 0.0f)
			return;
		
		if (newTarget != null)
		{
			if (newTarget.myTeam == myInfo.myTeam)
				return;
			
			if (targetInfo == null)
				OnFirstTargeting();
		}
		
		if (stateController.currentState == BaseState.eState.Die)
			return;
		
		if (newTarget != null)
		{
			UpdateTargetPath(newTarget.gameObject);
		}
		else if (targetInfo != null && newTarget == null)
		{
			switch(stateController.currentState)
			{
			case BaseState.eState.Run:
			case BaseState.eState.Dash:
				stateController.ChangeState(BaseState.eState.Stand);
				break;
			}
			
			curWayPoint = null;
		}
		
		targetInfo = newTarget;
	}
	
	private bool isFirstTargeting = true;
	public void OnFirstTargeting()
	{
		if (isFirstTargeting == true)
		{
			isFirstTargeting = false;
			
			if (myInfo.actorType == ActorInfo.ActorType.BossMonster)
			{
				UIMyStatusInfo myStatusInfo = GameUI.Instance.myStatusInfo;
				
				if (myStatusInfo != null)
				{
					myStatusInfo.ShowAIInfos(true);
					myStatusInfo.SetAIInfos(this);
				}
				
				StageManager stageManager = Game.Instance.stageManager;
				if (stageManager != null)
					stageManager.StartBossBGM();
			}
		}
	}
	
	public int GetEquipItemCount(ItemInfo.eItemType itemType)
	{
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int itemCount = 0;
		if (privateData != null && privateData.equipData != null)
		{
			foreach(EquipInfo info in privateData.equipData)
			{
				if (info == null || info.item == null || info.item.itemInfo == null)
					continue;
				
				if (info.item.itemInfo.itemType == itemType)
					itemCount += info.item.itemCount;
			}
		}
		
		itemCount = Mathf.Max(0, itemCount);
		
		return itemCount;
	}
	
	public string potionSound1 = "Battle_ItemUse_Potion";
	public string potionSound2 = "Battle_ItemUse_Meat";
	public void UsePotion(ItemInfo.eItemType potionType)
	{
		float hpRate = 0.0f;
		if (lifeManager != null)
			hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f || hpRate >= 1.0f)
			return;
		
		
		int charIndex = -1;
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		int itemCount = 0;
		Item potionItem = null;
		EquipInfo potionEquipInfo = null;
		
		/*
		if (privateData != null && privateData.equipData != null)
		{
			foreach(EquipInfo info in privateData.equipData)
			{
				if (info == null || info.item == null || info.item.itemInfo == null)
					continue;
				
				if (info.item.itemInfo.itemType == potionType)
				{
					potionEquipInfo = info;
					break;
				}
			}
		}
		
		potionItem = potionEquipInfo != null ? potionEquipInfo.item : null;
		itemCount = potionItem != null ? potionItem.itemCount : 0;
		
		if (itemCount <= 0)
			return;
		*/
		
		if (charData != null)
		{
			switch(potionType)
			{
			case ItemInfo.eItemType.Potion_1:
				itemCount = charData.equipPotion1Count;
				break;
			case ItemInfo.eItemType.Potion_2:
				itemCount = charData.equipPotion2Count;
				break;
			}
		}
		
		AttributeManager attributeManager = lifeManager != null ? lifeManager.attributeManager : null;
		AttributeValue hpMax = attributeManager != null ? attributeManager.GetAttribute(AttributeValue.eAttributeType.HealthMax) : null;
		
		float maxHPValue = hpMax != null ? hpMax.Value : 0.0f;
		
		if (maxHPValue == 0.0f)
			return;
		
		float addHPValue = 0.0f;
		string potionSound = "";
		
		int useItemCount = 0;
		switch(potionType)
		{
		case ItemInfo.eItemType.Potion_1:
			if (charData != null && charData.equipPotion1Count > 0)
			{
				potion1DelayTime = potion1CoolTime;
				
				useItemCount = 1;
				charData.equipPotion1Count -= useItemCount;
				charData.usedPotion1 += useItemCount;
				
				addHPValue = maxHPValue * 0.3f;
				if (buffManager != null && useItemCount > 0)
				{
					int buffIndex = buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_RED_POTION, this.lifeManager);
					if (buffIndex != -1)
						buffManager.RemoveBuff(buffIndex);
					
					buffManager.AddBuff(GameDef.eBuffType.BT_RED_POTION, addHPValue, potion1CoolTime, this.lifeManager, 1);
				}
				
				potionSound = potionSound1;
			}
			break;
		case ItemInfo.eItemType.Potion_2:
			if (charData != null && charData.equipPotion2Count > 0)
			{
				potion2DelayTime = potion2CoolTime;
				
				useItemCount = 1;
				charData.equipPotion2Count -= useItemCount;
				charData.usedPotion2 += useItemCount;
				
				AttributeValue curHP = attributeManager != null ? attributeManager.GetAttribute(AttributeValue.eAttributeType.Health) : null;
				if (curHP != null)
					curHP.baseValue = maxHPValue;
				
				potionSound = potionSound2;
			}
			break;
		}
		
		if (string.IsNullOrEmpty(potionSound) == false)
			OnPlaySoundA(potionSound);
		
		Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUsePotion, 0);
	}
	
	public bool CheckAttackTarget(ActorInfo info)
	{
		if (info == null)
			return false;
		
		LifeManager lifeManger = info.gameObject.GetComponent<LifeManager>();
		if (lifeManger == null)
			return false;
		
		if (lifeManger.GetHPRate() <= 0.0f)
			return false;
		
		return true;
	}
	
	public List<DialogInfo> charDialogInfos = new List<DialogInfo>();
	public void SetCharDialog(List<DialogInfo> dlgList)
	{
		charDialogInfos.Clear();
		charDialogInfos.AddRange(dlgList);
	}
	
	public void OnCharDialog()
	{
		int nCount = charDialogInfos.Count;
		if (nCount > 0)
		{
			DialogInfo dlgInfo = charDialogInfos[0];
			
			if (dlgInfo != null)
			{
				int classIndex = 0;
				switch(this.classType)
				{
				case GameDef.ePlayerClass.CLASS_WARRIOR:
					classIndex = 500;
					break;
				case GameDef.ePlayerClass.CLASS_ASSASSIN:
					classIndex = 600;
					break;
				case GameDef.ePlayerClass.CLASS_WIZARD:
					classIndex = 700;
					break;
				}
				
				int talkID = classIndex + dlgInfo.stringTableID;
				
				string msg = "";
				TableManager tableManager = TableManager.Instance;
				StringTable stringTable = null;
				if (tableManager != null)
					stringTable = tableManager.stringTable;
				
				if (stringTable != null)
					msg = stringTable.GetData(talkID);
				
				lifeManager.DoTalk(msg, dlgInfo.delayTime, dlgInfo.dialogType, dlgInfo.preventInput);
				Invoke("OnCharDialog", dlgInfo.delayTime);
				
				charDialogInfos.RemoveAt(0);
			}
		}
	}
	
	
	public BoxCollider revivalCheckCollider = null;
	public Vector3 revivalKnockDir = new Vector3(0.0f, 4.0f, 0.0f);
	public void OnRevival()
	{
		//1. HP Full...
		if (lifeManager != null && lifeManager.attributeManager != null)
		{
			AttributeValue health = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
			AttributeValue healthMax = lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.HealthMax);
			
			health.baseValue = healthMax.Value;
			lifeManager.attributeManager.UpdateValue(health);
			
			lifeManager.stunDelayTime = 0.0f;
		}
		
		//2...
		OnRevivalSub();
		
		//3. 몬스터 넉다운..(데미지 없이..)
		//boxCollider가 회전 되어 있어서 바운드 수정..
		Bounds checkBound = revivalCheckCollider.bounds;
		Vector3 vDir = revivalCheckCollider.extents;
		Vector3 extents = new Vector3(vDir.z, vDir.y, vDir.z);
		checkBound.extents = extents;
		
		ActorManager actorManager = ActorManager.Instance;
		if (actorManager != null)
		{
			List<ActorInfo> monsterList = actorManager.GetActorList(myInfo.enemyTeam);
			if (monsterList != null)
			{
				foreach(ActorInfo info in monsterList)
				{
					BaseMonster monster = info.gameObject.GetComponent<BaseMonster>();
					LifeManager monLifeManager = info.gameObject.GetComponent<LifeManager>();
					
					float hpValue = monLifeManager.GetHP();
					
					if (monster == null || hpValue <= 0.0f)
						continue;
					
					if (checkBound.Intersects(monster.collider.bounds) == false)
						continue;
					
					monster.DoKnockDown(revivalKnockDir);
				}
			}
		}
		
		if (stateController != null)
		{
			stateController.Locked = false;
			stateController.ChangeState(BaseState.eState.Stand);
		}
		
		Game.Instance.ApplyAchievement(Achievement.eAchievementType.eRevival, 0);
	}
	
	public virtual void OnRevivalSub()
	{
		
	}
	
	
	public string dieSoundFile = "";
	public virtual void OnDamage(AttackStateInfo attackInfo, Transform hitPos, LifeManager.eDamageType damageType)
	{
		if (comboCounter != null)
			comboCounter.ResetCombo();
		
		float hpValue = this.lifeManager.GetHP();
		if (hpValue <= 0.0f)
		{
			SoundManager soundManager = null;
			if (stateController != null)
				soundManager = stateController.soundManager;
			
			if (soundManager != null)
				soundManager.AddSoundEffect(dieSoundFile, SoundEffect.eSoundType.TryKeep);
		}
	}
	
	public void AbilityFull()
	{
		AttributeValue.eAttributeType maxValueType = AttributeValue.eAttributeType.None;
		switch(this.abilityValueType)
		{
		case AttributeValue.eAttributeType.Vital:
			maxValueType = AttributeValue.eAttributeType.VitalMax;
			break;
		case AttributeValue.eAttributeType.Mana:
			maxValueType = AttributeValue.eAttributeType.ManaMax;
			break;
		case AttributeValue.eAttributeType.Rage:
			maxValueType = AttributeValue.eAttributeType.RageMax;
			break;
		}
		
		if (lifeManager != null && lifeManager.attributeManager != null)
			lifeManager.attributeManager.AbilityFull(this.abilityValueType, maxValueType);
	}
	
	public virtual void Post_UpdateAbilityData()
	{
		
	}
}
