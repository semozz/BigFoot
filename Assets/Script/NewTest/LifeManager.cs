using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent (typeof (BuffManager))]
public class LifeManager : MonoBehaviour {
	public static float defenceCalcValue = 3000.0f;
	public static float limitDamageRate = 0.2f;
	
	[HideInInspector]
	public StateController stateController = null;
	[HideInInspector]
	public BaseMoveController moveController = null;
	
	public enum eDamageType
	{
		None,
		Damge,
		KnockDown
	}
	
	[HideInInspector]
	public string defaultWeaponLayerName = "Weapon";
	
	public string weaponLayerName = "Weapon";
	public string projectileLayerName = "Projectile";
	
	[HideInInspector]
	public int defaultWeaponLayer = 0;
	[HideInInspector]
	public int weaponLayer = 0;
	[HideInInspector]
	public int projectileLayer = 0;
	
	public AttributeManager attributeManager = new AttributeManager();
	public MasteryManager_New masteryManager_New = null;
	[HideInInspector]
	public BuffManager buffManager = null;
	
	public delegate bool CheckFunc(LifeManager attacker);
	public CheckFunc checkAvoid = null;
	public CheckFunc checkBlock = null;
	public CheckFunc onDamageFX = null;
	
	public delegate void OnTargetHit(LifeManager hitActor, float damage, bool isCritical, AttackStateInfo stateInfo);
	public OnTargetHit onTargetHit = null;
	
	public delegate void OnDamageDelegate(AttackStateInfo attackInfo, Transform hitPos, eDamageType damageType);
	public OnDamageDelegate onDamage = null;
	
	public delegate void OnManaShieldBroken();
	public OnManaShieldBroken onMansShieldBroken = null;
	
	[HideInInspector]
	public LifeManager targetActor = null;
	
	public AttackStateInfo attackStateInfo = new AttackStateInfo();
	
	public float patience_Normal = 100.0f;
	public float patience_Weak = 50.0f;
	public float patience_SuperAmmor = 400.0f;
	public float patience_BerserkRate = 2.0f;
	
	public float patienceFactor = 1.0f;
	public float receivePainValue = 0.0f;
	public float painResetDelayTime = 0.0f;
	public float painResetCoolTime = 1.0f;
	
	public float stunDelayTime = 0.0f;
	
	//public float knockDownPowerRate = 0.75f;
	
	public List<LifeManager> hitObjectList = new List<LifeManager>();
	
	protected TargetSearch targetSearch = null;
	
	public List<string> damageEffect = new List<string>();
	public string fxDamageName = "";
	public string fxPoisonBomb = "FX_Poison_Bomb";
	
	[HideInInspector]
	public EquipManager equipManager = null;
	[HideInInspector]
	public InventoryManager inventoryManager = null;
	[HideInInspector]
	public AwakeningLevelManager awakeningLevelManager = null;
		
	public long expValue = 0L;
	public int skillPoint = 0;
	//public Vector3 ownGoldValue = Vector3.zero;
	public Vector2 staminaValue = new Vector2(100.0f, 100.0f);
	
	[HideInInspector]
	public ActorInfo myActorInfo = null;
	
	public GameObject meshNode = null;
	protected Renderer[] meshRenderers = null;
	
	public bool ignoreHitActor = false;
	
	// xgreen for test.
	public bool bSuperAmmor = false;
	
	//BossRaid....
	public bool isBossRaidMonster = false;
	public bool isPhase2 = false;
	public float phase2Value = 0.3f;
	
	void Awake()
	{
		stateController = GetComponent<StateController>();
		moveController = GetComponent<BaseMoveController>();
		buffManager = GetComponent<BuffManager>();
		equipManager = GetComponent<EquipManager>();
		inventoryManager = GetComponent<InventoryManager>();
		awakeningLevelManager = GetComponent<AwakeningLevelManager>();
		myActorInfo = GetComponent<ActorInfo>();
		
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		FindMeshNode(transforms);
		
		this.damageUIRoot = FindNode(damageUIRootName, transforms);
		this.damageUIRoot2 = FindNode(uiRootName2, transforms);		
		
		if (awakeningLevelManager != null)
			awakeningLevelManager.lifeManager = this;
		
		if (masteryManager_New == null)
			masteryManager_New = new MasteryManager_New(this);
	}
	
	void OnDestroy()
	{
		
	}
	
	private void FindMeshNode(Transform[] transforms)
	{
		if (transforms == null)
			return;
		
		foreach (Transform trans in transforms)
		{
			if (trans != null && trans.name == "Mesh")
			{
				meshNode = trans.gameObject;
				meshRenderers = meshNode.GetComponentsInChildren<Renderer>();
				break;
			}
		}	
	}
	
	public Transform FindNode(string name, Transform[] transforms)
	{
		Transform transInfo = null;
		
		if (transforms != null)
		{
			foreach (Transform trans in transforms)
			{
				if (trans != null && trans.name == name)
				{
					transInfo = trans;
					break;
				}
			}
		}
		
		return transInfo;
	}
	
	// Use this for initialization
	public AudioSource audioSource = null;
	void Start () {
		weaponLayer = LayerMask.NameToLayer(weaponLayerName);
		projectileLayer = LayerMask.NameToLayer(projectileLayerName);
		
		defaultWeaponLayer = LayerMask.NameToLayer(defaultWeaponLayerName);
		
		stateController = GetComponent<StateController>();
		
		targetSearch = GetComponent<TargetSearch>();
		
		attackStateInfo.ownerActor = this;
		
		ApplyDayColor();
		
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	public void ApplyDayColor()
	{
		if (stateController != null && stateController.stageManager != null)
		{
			Color dayColor = stateController.stageManager.dayColor;
			switch(stateController.stageManager.dayMode)
			{
			case StageManager.eDayMode.eDay:
				dayColor = stateController.stageManager.dayColor;
				break;
			case StageManager.eDayMode.eNight:
				dayColor = stateController.stageManager.nightColor;
				break;
			}
			
			ApplyDayColor(dayColor);
		}
	}
	
	public void ApplyDayColor(Color dayColor)
	{
		if (meshRenderers == null)
			return;
		
		foreach(Renderer renderer in meshRenderers)
		{
			if (renderer.material != null && renderer.material.HasProperty("_DayLight") == true)
			{
				Color origColor = renderer.material.GetColor("_DayLight");
				float origAlaha = origColor.a;
				Color newColor = dayColor;
				newColor.a = origAlaha;
				
				renderer.material.SetColor("_DayLight", newColor);
			}
		}
		
		UpdateWeaponAndCostumeDayColor(dayColor);
	}
	
	public void UpdateWeaponAndCostumeDayColor(Color dayColor)
	{
		List<GameObject> objList = new List<GameObject>();
		if (costumeBack != null)
			objList.Add(costumeBack);
		if (costumeHead != null)
			objList.Add(costumeHead);
		
		foreach(GameObject obj in weaponList)
		{
			if (obj != null)
				objList.Add(obj);
		}
		
		UpdateObjectDayColor(objList, dayColor);
	}
	
	public void UpdateObjectDayColor(List<GameObject> objList, Color dayColor)
	{
		foreach(GameObject obj in objList)
		{
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
			
			if (renderers != null)
			{
				foreach(Renderer renderer in renderers)
				{
					if (renderer.material != null && renderer.material.HasProperty("_DayLight") == true)
					{
						Color origColor = renderer.material.GetColor("_DayLight");
						float origAlaha = origColor.a;
						Color newColor = dayColor;
						newColor.a = origAlaha;
						
						renderer.material.SetColor("_DayLight", newColor);
					}
				}
			}
		}
	}
	
	public float hpRegenCoolTime = 1.0f;
	public float hpRegenDelayTime = 1.0f;
	// Update is called once per frame
	void Update () {
		switch(stateController.currentState)
		{
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			return;
		}
		
		if (painResetDelayTime <= 0.0f)
		{
			ResetReceivePainValue();
			painResetDelayTime = painResetCoolTime;
		}
		else
			painResetDelayTime -= Time.deltaTime;
		
		if (GetHP() <= 0.0f)
			return;
		
		if (hpRegenDelayTime <= 0.0f)
		{
			RegenHP();
			hpRegenDelayTime = hpRegenCoolTime;
		}
		else
			hpRegenDelayTime -= Time.deltaTime;
	}
	
	public void RegenHP()
	{
		AttributeValue hpRegenValue = null;
		if (attributeManager != null)
			hpRegenValue = attributeManager.GetAttribute(AttributeValue.eAttributeType.HealthRegen);
		
		if (hpRegenValue != null)
		{
			float hpRegen = hpRegenValue.Value * 0.2f;
			
			if (hpRegen > 0.0f)
			{
				//Debug.Log("HP Regen : " + hpRegen);
				IncHP(hpRegen, false, GameDef.eBuffType.BT_REGENHP);
			}
		}
	}
	
	public virtual BaseWeapon.eAddResult AddHitObject(LifeManager actor)
    {
        if (hitObjectList.Contains(actor) == true)
            return BaseWeapon.eAddResult.AlreadyAdd;
		
		hitObjectList.Add(actor);

        return BaseWeapon.eAddResult.AddOK;
    }

    public void ClearHitObject()
    {
        hitObjectList.Clear();
    }
	
	public void SetAttackInfo(StateInfo info)
	{
		if (attributeManager != null)
		{
			float baseDamage = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AttackDamage);
			
			float addDamage = 0.0f;
			
			float addTempValue = 0.0f;
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//hp 35%이하 공격력 증가.
			float addRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAttackDamageUnderHP35);
			float hpRate = GetHPRate();
			if (hpRate <= 0.35f && addRate > 0.0f)
			{
				addTempValue = baseDamage * addRate;
				
				Debug.Log("hp 35% .. addAttackDamage : " + addTempValue);
				addDamage += addTempValue;
			}
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			/*
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//방어력 %만큼 공격력 증가.
			addRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAttackDamageByArmor);
			float armorValue = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Armor);
			if (addRate > 0.0f)
			{
				addTempValue = armorValue * addRate;
				
				Debug.Log("Armor .. addAttackDamage : " + addTempValue);
				addDamage += addTempValue;
			}
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			*/
			
			TableManager tableManager = TableManager.Instance;
			StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
			
			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//도끼 추가 데미지.
			bool isEquipItem = false;
			int equipID = 0;
			addRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAttackDamageByAxe);
			if (stringValueTable != null)
				equipID = stringValueTable.GetData("EquipAxeItemID");
			
			if (equipManager != null)
				isEquipItem = equipManager.HasEquped(equipID);
			if (addRate != 0.0f && isEquipItem == true)
			{
				addTempValue = baseDamage * addRate;
				
				Debug.Log("Axe Equip addAttackDamage : " + addTempValue);
				addDamage += addTempValue;
			}
			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//둔기 추가 데미지.
			isEquipItem = false;
			equipID = 0;
			addRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAttackDamageByHammer);
			if (stringValueTable != null)
				equipID = stringValueTable.GetData("EquipHammerItemID");
			
			if (equipManager != null)
				isEquipItem = equipManager.HasEquped(equipID);
			if (addRate != 0.0f && isEquipItem == true)
			{
				addTempValue = baseDamage * addRate;
				
				Debug.Log("Hammer Equip addAttackDamage : " + addTempValue);
				addDamage += addTempValue;
			}
			////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			attackStateInfo.attackDamage = baseDamage + addDamage;
			
			attackStateInfo.abilityPower = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AbilityPower);
		}
		
		attackStateInfo.SetState(info);
		
		attackStateInfo.addAttackPower = 0.0f;
		attackStateInfo.addAttackRate = 0.0f;
	}
	
	public float GetAddAttackDamageByEquipWeapon()
	{
		float baseDamage = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.AttackDamage);
		
		float addDamage = 0.0f;
		float addTempValue = 0.0f;
		float addRate = 0.0f;
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//도끼 추가 데미지.
		bool isEquipItem = false;
		int equipID = 0;
		addRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAttackDamageByAxe);
		if (stringValueTable != null)
			equipID = stringValueTable.GetData("EquipAxeItemID");
		
		if (equipManager != null)
			isEquipItem = equipManager.HasEquped(equipID);
		if (addRate != 0.0f && isEquipItem == true)
		{
			addTempValue = baseDamage * addRate;
			
			Debug.Log("Axe Equip addAttackDamage : " + addTempValue);
			addDamage += addTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//둔기 추가 데미지.
		isEquipItem = false;
		equipID = 0;
		addRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAttackDamageByHammer);
		if (stringValueTable != null)
			equipID = stringValueTable.GetData("EquipHammerItemID");
		
		if (equipManager != null)
			isEquipItem = equipManager.HasEquped(equipID);
		if (addRate != 0.0f && isEquipItem == true)
		{
			addTempValue = baseDamage * addRate;
			
			Debug.Log("Hammer Equip addAttackDamage : " + addTempValue);
			addDamage += addTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		return addDamage;
	}
	
	public AttackStateInfo GetCurrentAttackInfo()
	{
		attackStateInfo.attackState = stateController != null ? stateController.currentState : BaseState.eState.None;
		return attackStateInfo;
	}
	
	public void AddPainValue(float addValue)
	{
		
		switch(attackStateInfo.stateInfo.defenseState)
		{
		case StateInfo.eDefenseState.DS_PROTECT:
		case StateInfo.eDefenseState.DS_INVINCIBLE:
			return;
		}
		
		this.receivePainValue += addValue;
		
		//Debug.Log("Add Pain Value.. " + addValue + ",  " + this.receivePainValue + " " + this.ActorType);
		
		if (addValue > 0.0f)
			ResetPainDelayTime();
	}
	
	public float GetPainValue()
	{
		return attackStateInfo.stateInfo.painValue;
	}
	
	public void ResetPainDelayTime()
	{
		//Debug.Log("ResetPainDelayTime ....." + this.ActorType);
		this.painResetDelayTime = this.painResetCoolTime;	
	}
	
	public void ResetReceivePainValue()
	{
		//Debug.Log("ResetPainValue ....." + this.ActorType);
		
		this.receivePainValue = 0.0f;	
	}
	
	public float GetPatienceValue()
	{
		float _value = 0.0f;
		switch(attackStateInfo.stateInfo.defenseState)
		{
		case StateInfo.eDefenseState.DS_WEAK1:
		case StateInfo.eDefenseState.DS_WEAK2:
			_value = this.patience_Weak;
			break;
		case StateInfo.eDefenseState.DS_NORMAL:
			_value = this.patience_Normal;
			break;
		case StateInfo.eDefenseState.DS_SUPERARMOR:
			_value = this.patience_SuperAmmor;
			break;
		case StateInfo.eDefenseState.DS_PROTECT:
		case StateInfo.eDefenseState.DS_INVINCIBLE:
			_value = float.MaxValue;
			break;
		case StateInfo.eDefenseState.DS_BLOCK:
		case StateInfo.eDefenseState.DS_AVOID:
			_value = this.patience_Normal;
			break;
		}
		
		_value *= this.patienceFactor;
		
		int index = -1;
		if (buffManager != null)
			index = buffManager.GetBuff(GameDef.eBuffType.BT_BERSERK);
		if (index != -1)
			_value *= this.patience_BerserkRate;
		
		return _value;
	}
	
	public bool CheckPatienceVSPain()
	{
		float patienceValue = GetPatienceValue();
		
		return this.receivePainValue >= patienceValue;
	}

	void OnTriggerEnter(Collider other)
	{
		Debug.Log(this + " OnTriggerEnter : " + other);
		
		if (other.gameObject.layer != defaultWeaponLayer && 
			other.gameObject.layer != weaponLayer &&
			other.gameObject.layer != projectileLayer)
			return;
		
		Transform parentObj = other.gameObject.transform.parent;
		ColliderManager colliderManager = null;
		if (parentObj != null)
			colliderManager = parentObj.gameObject.GetComponent<ColliderManager>();
		
		GameObject owner = null;
		if (colliderManager != null)
			owner = colliderManager.ownerActor;
		
		if (owner == null)
			return;
		
		AttackStateInfo attackInfo = null;
		
		BaseWeapon.eAddResult addResult = BaseWeapon.eAddResult.AddOK;
		
		BaseWeapon baseWeapon = owner.GetComponent<BaseWeapon>();
		if (baseWeapon != null)
		{
			addResult = baseWeapon.AddHitObject(this);
			
			attackInfo = baseWeapon.GetAttackInfo();
		}
		else
		{
			LifeManager actor = owner.GetComponent<LifeManager>();
			addResult = BaseWeapon.eAddResult.AlreadyAdd;
			if (actor != null)
				addResult = actor.AddHitObject(this);
			
			attackInfo = actor.GetCurrentAttackInfo();
		}
		
		bool isEvadedAttack = false;
		if (attackInfo.stateInfo.attackType == StateInfo.eAttackType.AT_ENABLEAVOID &&
			this.attackStateInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_AVOID)
			isEvadedAttack = true;
		
		if (addResult != BaseWeapon.eAddResult.AlreadyAdd)
			OnDamage(attackInfo, other.transform, addResult == BaseWeapon.eAddResult.Evade);
		
		if (attackInfo.stateInfo.attackType == StateInfo.eAttackType.AT_ENABLEAVOID &&
			this.attackStateInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_AVOID)
			isEvadedAttack = true;
		
		if (addResult != BaseWeapon.eAddResult.Evade &&
			isEvadedAttack == false && baseWeapon != null)
			baseWeapon.SetDestroy(this);
	}
	
	public void OnDamage(AttackStateInfo attackInfo, Transform hitPos, bool isEvadedAttack)
	{
		float hpValue = GetHP();
		if (hpValue < 0.01f)
			return;
		
		if (GetHPRate() <= 0.0f)
			return;
		
		if (attackInfo == null)
			return;
		
		if (attackInfo.ownerActor == this)
			return;
		
		int buffIndex = -1;
		if (buffManager != null)
			buffIndex = buffManager.GetBuff(GameDef.eBuffType.BT_INVINCIBLE);
		if (buffIndex != -1)
			return;
		
		LifeManager attacker = attackInfo.ownerActor;
		/*
		if (attacker == null)
			return;
		*/
		
		if (this.ignoreHitActor == false)
			ChangeTarget(attacker);
		
		AttackStateInfo hitActorInfo = this.attackStateInfo;
		if (hitActorInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_INVINCIBLE)
			return;
		
		TableManager tableManager = TableManager.Instance;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		int equipID = 0;
		bool isEquipItem = false;
		EquipManager equipManager = attacker != null ? attacker.equipManager : null;
		
		string attackSound = attackInfo.stateInfo.soundFile;
		if (attacker != null && attacker.myActorInfo.actorType == ActorInfo.ActorType.Player)
		{
			PlayerController player = Game.Instance.player;
			if (player.classType == GameDef.ePlayerClass.CLASS_WARRIOR &&
				attackInfo.stateInfo.isWeaponAttack == true)
			{
				if (stringValueTable != null)
					equipID = stringValueTable.GetData("EquipHammerItemID");
			
				if (equipManager != null)
					isEquipItem = equipManager.HasEquped(equipID);
				
				if (isEquipItem == true)
					attackSound += "_hammer";
				else
					attackSound += "_axe";
			}
		}
		
		float damage1 = 0.0f;
		float damage2 = 0.0f;
		
		float attackDamage = attackInfo.GetAttackDamage();
		if (attackDamage > 0.0f)
		{
			float armor = this.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Armor);
			float armorPenetration = 0.0f;
			if (attacker != null)
				armorPenetration = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.ArmorPenetration);
		
			float defenceValue = Mathf.Max(0.0f, (armor - armorPenetration));
			float reduceDamageRate = defenceValue / (defenceValue + defenceCalcValue);
			
			float damageRate = Mathf.Max(limitDamageRate, (1.0f - reduceDamageRate));
			damage1 = attackDamage * damageRate;
		}
		
		float abilityPower = attackInfo.GetAbilityPower();
		if (abilityPower > 0.0f)
		{
			float magicResist = this.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.MagicResist);
			float magicPenetration = 0.0f;
			if (attacker != null)
				magicPenetration = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.MagicPenetration);
		
			float defenceValue = Mathf.Max(0.0f, (magicResist - magicPenetration));
			float reduceDamageRate = defenceValue / (defenceValue + defenceCalcValue);
			
			float damageRate = Mathf.Max(limitDamageRate, (1.0f - reduceDamageRate));
			damage2 = abilityPower * damageRate;
		}

		//if (bSuperAmmor && myActorInfo.actorType == ActorInfo.ActorType.Player)
		//	damage2 = damage1 = 0.0f;
		
		//damage2 = damage1 = 0.0f;
		
		float stunTime = attackInfo.stateInfo.stunTime;
        float stunRate = attackInfo.stateInfo.stunRate + attackInfo.stateInfo.addStunRate;
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// add stunRate
		float incStunRate = 0.0f;
		if (attacker != null)
			incStunRate = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncStunRateByHammer);
		if (stringValueTable != null)
			equipID = stringValueTable.GetData("EquipAxeItemID");
		
		if (equipManager != null)
			isEquipItem = equipManager.HasEquped(equipID);
		if (incStunRate != 0.0f && isEquipItem == true)
		{
			Debug.Log("Hammer Equip addStunRate : " + incStunRate);
			stunRate += incStunRate;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		if (attackInfo == null)
		{
			Debug.LogWarning(attacker + " Attack info is null...... ");
			return;
		}
		
		if (attackInfo.stateInfo.attackType == StateInfo.eAttackType.AT_ENABLEAVOID)
		{
			if (checkAvoid != null && checkAvoid(attacker) == true)
				return;
			
			if (checkBlock != null && checkBlock(attacker) == true)
				return;
			
			if (isEvadedAttack == true)
				return;
		}
		
		BaseState.eState damageState = stateController.currentState;
		
		float knockdownPower = 0.0f;
		
		float curStunDelayTime = 0.0f;
		int stunValue = Mathf.RoundToInt(stunRate * 100.0f);
		int randValue = Random.Range(0, 100);
        
		AddPainValue(attackInfo.stateInfo.painValue);
		bool overPatience = CheckPatienceVSPain();
		
		//?€íŽ??ê²œì° ?žëŽê°?Over?ê±žë¡?ì²ëŠ¬??
		if (this.stateController.IsContainState(BaseState.eState.Stun) == true && stunValue >= randValue)
		{
			curStunDelayTime = stunTime;
			
			overPatience = true;
		}
		
		float incDamageRate = 0.0f;
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 50% 이하 체력에 추가 데미지.
		float incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageWhenUnderHP50);
		float curHPRate = GetHPRate();
		if (curHPRate > 0.0f && curHPRate <= 0.5f)
		{
			Debug.Log("Under HP 50 : addDamageRate : " +  incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 50% 이상 체력에 추가 데미지.
		incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageWhenOverHP50);
		curHPRate = GetHPRate();
		if (curHPRate >= 0.5f)
		{
			Debug.Log("Over HP 50 : addDamageRate : " +  incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//마법사 얼음 안개? 데미지 감소.
		incTempValue = 0.0f;
		if (this.attributeManager != null)
			incTempValue = this.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.DecDamageOnBuff);
		
		int decDamageBuffIndex = -1;
		if (this.buffManager != null)
			decDamageBuffIndex = this.buffManager.GetBuff(GameDef.eBuffType.BT_DEC_DAMAGE);
		
		if (incTempValue != 0.0f && decDamageBuffIndex != -1)
		{
			Debug.Log("Wizard IceTornado DecDamageRate : " + incTempValue);
			incDamageRate -= incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//버서커 데미지 감소.
		incTempValue = 0.0f;
		if (this.attributeManager != null)
			incTempValue = this.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.DecDamageWhenBerserk);
		
		int berserkBuffIndex = -1;
		if (this.buffManager != null)
			berserkBuffIndex = this.buffManager.GetBuff(GameDef.eBuffType.BT_BERSERK);
		
		if (incTempValue != 0.0f && berserkBuffIndex != -1)
		{
			Debug.Log("Berserk DecDamageRate : " + incTempValue);
			incDamageRate -= incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//스턴 데미지 증가
		incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncAttackDamageWhenStun);
		if (incTempValue != 0.0f &&   this.stateController.currentState == BaseState.eState.Stun)
		{
			Debug.Log("Week2 Inc Damage : " + incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//WEEK2 피해 증가
		incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageOnWeek2);
		if (incTempValue != 0.0f &&   this.stateController.curStateInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_WEAK2)
		{
			Debug.Log("Week2 Inc Damage : " + incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//공격자 hp100%인 경우 피해량 증가.
		incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageWhenHP100);
		if (incTempValue != 0.0f && attacker != null && attacker.GetHPRate() >= 1.0f)
		{
			Debug.Log("Attacker HP100 Inc Damage : " + incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//Slow 데미지 증가.
		incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageOnSlow);
		int slowBuffIndex = -1;
		if (this.buffManager != null)
			slowBuffIndex = this.buffManager.GetBuff(GameDef.eBuffType.BT_SLOW);
		
		if (incTempValue != 0.0f && slowBuffIndex != -1)
		{
			Debug.Log("Slow IncDamageRate : " + incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//죽음의 칼날 데미지 증가.
		incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageOnPoisionByAction);
		int poisionBuffIndex = -1;
		if (this.buffManager != null)
			poisionBuffIndex = this.buffManager.GetBuff(GameDef.eBuffType.BT_POISION);
		
		if (incTempValue != 0.0f && poisionBuffIndex != -1 && attackInfo.attackState == BaseState.eState.AttackB_2)
		{
			Debug.Log("Poision on AttackState IncDamageRate : " + incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//마나가 적을 수록 데미지 증가.
		incTempValue = 0.0f;
		float manaRate = 0.0f;
		float curMana = 0.0f;
		float maxMana = 0.0f;
		if (attacker != null)
		{
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageOnLowMana);
			curMana = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.Mana);
			maxMana = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.ManaMax);
		}
		
		if (maxMana > 0.0f)
			manaRate = (1.0f - (curMana / maxMana));
		
		incTempValue *= manaRate;
		
		if (incTempValue != 0.0f)
		{
			Debug.Log("Low Mana IncDamageRate : " + incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		if (incDamageRate != 0.0f)
		{
			damage1 += damage1 * incDamageRate;
			damage2 += damage2 * incDamageRate;
		}
		
		float criticalHitRate = 0.0f;
		if (attacker != null)
			criticalHitRate = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.CriticalHitRate);
		float addCriticalHitRate = attackInfo.stateInfo.addCriticalHitRate;
		
		int criticalRandValue = Random.Range(0, 100);
		int criticalValue = Mathf.RoundToInt((criticalHitRate + addCriticalHitRate) * 100.0f);
		bool isCritical = criticalValue >= criticalRandValue;
		if (isCritical == true && attacker != null)
		{
			float criticalRate = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.CriticalDamageRate);
			Debug.Log("Critical : addDamageRate : " +  criticalRate);
			
			damage1 = damage1 * criticalRate;
			damage2 = damage2 * criticalRate;
		}
		
		
		float effectScale = 1.0f;
		if (attackInfo != null && attackInfo.stateInfo != null)
			effectScale = attackInfo.stateInfo.effectScale;
		
		if (isCritical == true)
			effectScale *= 1.2f;
		
		ApplyAttackEffect(attackInfo.stateInfo.fxObjectName, effectScale);
		
		float resultDamage = damage1 + damage2;
		
		/*
		if (attacker != null && attacker.myActorInfo != null &&
			attacker.myActorInfo.actorType == ActorInfo.ActorType.Player)
			resultDamage *= 990.0f;
		*/
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//데미지 쌓아 놓기..
		int reflectBuffIndex = buffManager != null ? buffManager.GetBuff(GameDef.eBuffType.BT_REFLECTDAMAGE) : -1;
		float reflectRate = attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncReflectDamage);
		if (reflectBuffIndex != -1 && buffManager != null)
		{
			BuffManager.stBuff buff = buffManager.mHaveBuff[reflectBuffIndex];
			buff.AbilityValue += resultDamage + (resultDamage * reflectRate);
			
			buffManager.mHaveBuff[reflectBuffIndex] = buff;
			
			resultDamage = 0.0f;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//흡혈..
		float lifeSteal = 0.0f;
		float lifeStealRate = 0.0f;
		if (attacker != null)
			lifeStealRate = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.LifeSteal);
		lifeSteal = resultDamage * lifeStealRate;
		if (attacker != null && lifeSteal != 0.0f)
			attacker.IncHP(lifeSteal, false, GameDef.eBuffType.BT_REGENHP);
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		if (resultDamage > 0.0f)
			ApplyDamageEffect();
		
		Debug.Log("Damage : " + resultDamage);
		float afterHP = DecHP(resultDamage, attacker, isCritical, GameDef.eBuffType.BT_NONE);
		
		if (resultDamage > 0.0f && onDamageFX != null)
			onDamageFX(attacker);
		Debug.Log("result HP : " + afterHP);
		
		if (buffManager != null && buffManager.GetBuff(GameDef.eBuffType.BT_MANASHIELD) != -1)
			return;
		
		switch(hitActorInfo.stateInfo.defenseState)
		{
		case StateInfo.eDefenseState.DS_AVOID:
		case StateInfo.eDefenseState.DS_BLOCK:
			if (overPatience == true)
			{
				switch(attackInfo.stateInfo.attackType)
				{
				case StateInfo.eAttackType.AT_DISABLEAVOID:
					switch(attackInfo.stateInfo.attackState)
					{
					case StateInfo.eAttackState.AS_DAMAGE:
						damageState = BaseState.eState.Damage;
						if (stateController.IsContainState(damageState) == false)
						{
							knockdownPower = 1.0f;
							damageState = BaseState.eState.Knockdownstart;
						}
						break;
					case StateInfo.eAttackState.AS_KNOCKDOWN:
						knockdownPower = 1.0f;
						damageState = BaseState.eState.Knockdownstart;
						break;
					}
					break;
				default:
					break;
				}
			}
			else
				return;
			break;
		case StateInfo.eDefenseState.DS_NORMAL:
			if (overPatience == true)
			{
				switch(attackInfo.stateInfo.attackState)
				{
				case StateInfo.eAttackState.AS_KNOCKDOWN:
					knockdownPower = 1.0f;
					damageState = BaseState.eState.Knockdownstart;
					break;
				case StateInfo.eAttackState.AS_DAMAGE:
					damageState = BaseState.eState.Damage;
					break;
				default:
					break;
				}
			}
			break;
		case StateInfo.eDefenseState.DS_PROTECT:
			break;
		case StateInfo.eDefenseState.DS_SUPERARMOR:
			if (/*stunDelayTime != 0.0f || */overPatience == true)
			{
				overPatience = true;
				
				switch(attackInfo.stateInfo.attackState)
				{
				case StateInfo.eAttackState.AS_DAMAGE:
					damageState = BaseState.eState.Damage;
					break;
				case StateInfo.eAttackState.AS_KNOCKDOWN:
					knockdownPower = 1.0f;
					damageState = BaseState.eState.Knockdownstart;
					break;
				}
			}
			break;
		case StateInfo.eDefenseState.DS_WEAK1:
			if (overPatience == true)
			{
				switch(attackInfo.stateInfo.attackState)
				{
				case StateInfo.eAttackState.AS_NONE:
					break;
				default:
					damageState = BaseState.eState.Knockdownstart;
					knockdownPower = 1.0f;
					break;
				}
			}
			break;
		case StateInfo.eDefenseState.DS_WEAK2:
			if (overPatience == true)
			{
				switch(attackInfo.stateInfo.attackState)
				{
				case StateInfo.eAttackState.AS_NONE:
					break;
				default:
					damageState = BaseState.eState.Knockdownstart;
					knockdownPower = 0.75f;//knockDownPowerRate;
					break;
				}
			}
			break;
		}
		
		if (overPatience == true)
		{
			ResetPainDelayTime();
			ResetReceivePainValue();
		}
		
		if (knockdownPower != 0.0f)
		{
			damageState = BaseState.eState.Knockdownstart;
		}
		
		if (stateController.currentState != damageState)
			ChangeMoveDir(hitPos);
		
		if (targetSearch != null)
			targetSearch.ResetTime();
		
		eDamageType damageType = eDamageType.None;
		switch(damageState)
		{
		case BaseState.eState.Damage:
			damageType = eDamageType.Damge;
			break;
		case BaseState.eState.Knockdownstart:
			damageType = eDamageType.KnockDown;
			break;
		}
		
		if (GameOption.effectToggle == true)
		{
			float effectSoundScale = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(audioSource, attackSound, effectSoundScale);
		}
		
		if (stateController != null)
		{
			stateController.ChangeState(damageState);
			
			if (damageState == BaseState.eState.Knockdownstart)
			{
				if (moveController != null && moveController.isMovable == true)
				{
					float weightRate = 1.0f;
					switch(moveController.weightType)
					{
					case BaseMoveController.eWeightType.Normal:
						weightRate = 1.0f;
						break;
					case BaseMoveController.eWeightType.Heavy:
						weightRate = 0.85f;
						break;
					case BaseMoveController.eWeightType.Slight:
						weightRate = 1.1f;
						break;
					}
					knockdownPower *= weightRate;
					
					//KnockDown diff value on JumpState / normalState
					Vector3 knockDir = attackInfo.stateInfo.knockDir;
					if (this.stateController.IsJumpState() == true)
						knockDir = attackInfo.stateInfo.knockDir_Air;
					
					moveController.moveSpeed = -knockDir.x * knockdownPower;
					
					Vector3 UpDir = Vector3.up * (knockDir.y * 100.0f * knockdownPower);
					moveController.DoKnockDown(UpDir);
				}
			}
		}
		
		if (afterHP > 0.0f)
		{
			int buffRandValue = Random.Range(0, 100);
			List<int> removeIndex = new List<int>();
			
			foreach(BuffInfo info in attackInfo.buffList)
			{
				if (info == null)
					continue;
				
				int rateValue = Mathf.RoundToInt(info.Rate * 100.0f);
				if (rateValue < buffRandValue)
					continue;
				
				if (this.buffManager != null)
				{
					int index = this.buffManager.GetAppliedBuffIndex(info.Type, attacker);
					int stackCount = 1;
					if (index != -1)
					{
						if (info.Type == GameDef.eBuffType.BT_POISION)
						{
							BuffManager.stBuff oldBuff = this.buffManager.mHaveBuff[index];
							stackCount = Mathf.Min(oldBuff.StackCount + 1, (int)GameDef.ePoisonLevel.MAX_COUNT);
						}
						//this.buffManager.RemoveBuff(index);
						removeIndex.Add(index);
					}
					
					this.buffManager.AddBuff(info.Type, info.Value, info.DelayTime, attacker, stackCount);
				}
			}
			
			int nCount = removeIndex.Count;
			for (int idx = nCount - 1; idx >= 0; --idx)
			{
				this.buffManager.RemoveBuff(idx);
			}
		}
		
		Debug.Log("DamageState : " + damageState);
		
		if (attacker != null && attacker.onTargetHit != null)
			attacker.onTargetHit(this, attackDamage, isCritical, attackInfo);
		
		if (onDamage != null)
			onDamage(attackInfo, hitPos, damageType);
		
		//Debug.Log("StunTime : " + curStunDelayTime + " time : " + Time.time);
		if (curStunDelayTime > 0.0f)
			this.stunDelayTime = curStunDelayTime;
	}
	
	public void ApplyPoisonBombEffect()
	{
		if (stateController != null)
			stateController.AddFXDelayInfo(fxPoisonBomb, eFXEffectType.ScaleNode, 1.0f, 0.4f);
	}
	
	public void ApplyAttackEffect(string fxObject, float scale)
	{
		if (stateController != null)
			stateController.AddFXDelayInfo(fxObject, eFXEffectType.ScaleNode, scale, 0.8f);
	}
	
	public void ApplyDamageEffect()
	{
		int nCount = damageEffect.Count;
		int index = -1;
		if (nCount > 0)
			index = Random.Range(0, nCount);
		
		string fxObjectName = "";
		if (index != -1)
			fxObjectName = damageEffect[index];
		
		if (stateController != null)
			stateController.AddFXDelayInfo(fxObjectName, eFXEffectType.ScaleNode, 1.0f, 0.8f);
		
		
		if (myActorInfo.actorType == ActorInfo.ActorType.Player)
		{
			if (stateController != null)
				stateController.AddFXDelayInfo(fxDamageName, eFXEffectType.CameraNode, 1.0f, 1.0f);
		}
	}
	
	public void ChangeMoveDir(Transform attacker)
	{
		if (attacker == null)
			return;
		
		Vector3 diff = this.transform.position - attacker.position;
		Vector3 moveDir = Vector3.right;
		if (diff.x < 0.0f)
			moveDir = Vector3.right;
		else
			moveDir = Vector3.left;
		
		if (moveController != null)
			moveController.ChangeMoveDir(moveDir);	
	}
	
	public delegate void SyncHPValue();
	public SyncHPValue syncHPValue = null;
	
	public string absorbSoundFile = "";
	public float DecHP(float attackValue, LifeManager attacker, bool isCritical, GameDef.eBuffType buffType)
	{
		//Debug.Log("AttackValue : " + attackValue);
		if (buffType == GameDef.eBuffType.BT_POISION)
		{
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//흡혈..
			float lifeSteal = 0.0f;
			float lifeStealRate = 0.0f;
			if (attacker != null)
				lifeStealRate = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.LifeSteal);
			lifeSteal = attackValue * lifeStealRate;
			if (attacker != null && lifeSteal != 0.0f)
				attacker.IncHP(lifeSteal, false, GameDef.eBuffType.BT_NONE);
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////	
		}
		
		int buffIndex = -1;
		if (buffManager != null)
			buffIndex = buffManager.GetBuff(GameDef.eBuffType.BT_MANASHIELD);
		
        if (buffIndex >= 0)
        {
            BuffManager.stBuff buff = buffManager.mHaveBuff[buffIndex];
            if (attackValue <= buff.AbilityValue)
            {
                buff.AbilityValue = buff.AbilityValue - attackValue;
                attackValue = 0.0f;
                buffManager.mHaveBuff[buffIndex] = buff;
				
				if (GameOption.effectToggle == true)
				{
					this.ApplyEtcDamageUI(EtcDamageUI.eEtcDamge.Absorption);
					
					float effectVolume = Game.Instance.effectSoundScale;
					AudioManager.PlaySound(audioSource, absorbSoundFile, effectVolume);
				}
            }
            else
            {
                attackValue = attackValue - buff.AbilityValue;
				Debug.Log("Damage reduce... : " + attackValue);
                buff.AbilityValue = 0.0f;
                
                buff.bWillDelete = true;
				buffManager.mHaveBuff[buffIndex] = buff;
				
				stateController.RemoveFXDelayInfo(buff.fxInfo);
				
				if (onMansShieldBroken != null)
					onMansShieldBroken();
            }
        }
		
		AttributeValue health = this.attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
		AttributeValue healthMax = this.attributeManager.GetAttribute(AttributeValue.eAttributeType.HealthMax);
		float curHP = health.Value;
		float afterHP = curHP;
		float maxHP = healthMax.Value;
		
		bool isCharDamage = (myActorInfo.actorType == ActorInfo.ActorType.Player || myActorInfo.actorType == ActorInfo.ActorType.Escort);
		if (attackValue > 0)
		{
			ApplyDamageDigitUI(attackValue, isCritical, isCharDamage, buffType);
			
			if (onHPValueChange != null)
				onHPValueChange(attackValue);
		}
		
		if (bSuperAmmor == false)
		{
			if (attacker != null && attacker.myActorInfo.actorType == ActorInfo.ActorType.Player)
				Game.Instance.ApplyBossRaidDamage(attackValue, this);
			
			float beforeHPRate = curHP / maxHP;
			
			afterHP = curHP - attackValue;
			
			afterHP = Mathf.Clamp(afterHP, 0.0f, healthMax.Value);
			if (afterHP <= 0.01f)
				afterHP = 0.0f;
			
			float afterHPRate = afterHP / maxHP;
			
			if (isBossRaidMonster == true && isPhase2 == false)
			{
				if (beforeHPRate > 0.5f && afterHPRate <= 0.5f)
				{
					isPhase2 = true;
					ActivatePhase2();
				}
			}
			
			health.baseValue = afterHP;
			this.attributeManager.UpdateValue(health);
			
			if (syncHPValue != null)
				syncHPValue();
			
			if (curHP > 0.0f && afterHP <= 0.01f)
				OnDie(attacker);
		}
		
		//Debug.Log("CurHP : " + curHP);
		
		return afterHP;
	}
	
	public string damageUIPrefabPath = "UI/DamageUI/DamageUI";
	public string damageUIRootName = "DamageUI_pos";
	public Transform damageUIRoot = null;
	public string uiRootName2 = "Add_UI";
	public Transform damageUIRoot2 = null;
	public virtual void ApplyDamageDigitUI(float fValue, bool isCritical, bool isCharacterDamage, GameDef.eBuffType buffType)
	{
		if (this.damageUIRoot == null)
			return;
		
		DamageUI damageUI = ResourceManager.CreatePrefab<DamageUI>(damageUIPrefabPath, this.damageUIRoot, Vector3.zero);
		if (damageUI != null)
		{
			damageUI.TraceObject = transform;
			
			damageUI.SetDamage((int)fValue, isCritical, isCharacterDamage, buffType);
			
			DestroyObject(damageUI.gameObject, damageUI.TotalTime);
		}
	}
	
	public string dropGoldUIPrefab = "UI/DamageUI/DropGoldUI";
	public void ApplyDropGold(int gold)
	{
		if (this.damageUIRoot == null)
			return;
		
		DropGoldUI dropGoldUI = ResourceManager.CreatePrefab<DropGoldUI>(dropGoldUIPrefab, this.damageUIRoot, Vector3.zero);
		if (dropGoldUI != null)
		{
			dropGoldUI.TraceObject = transform;
			
			dropGoldUI.SetGold(gold);
			
			DestroyObject(dropGoldUI.gameObject, dropGoldUI.TotalTime);
		}
	}
	
	public UISlider.OnValueChange onHPValueChange = null;
	
	public string etcDamageUIPrefabPath = "UI/DamageUI/EtcDamageUI";
	public virtual void ApplyEtcDamageUI(EtcDamageUI.eEtcDamge type)
	{
		EtcDamageUI etcDamageUI = ResourceManager.CreatePrefab<EtcDamageUI>(etcDamageUIPrefabPath, this.damageUIRoot2, Vector3.zero);
		if (etcDamageUI != null)
		{
			etcDamageUI.TraceObject = transform;
			
			etcDamageUI.SetDamageType(type);
		}
	}
	
	public float IncHP(float incValue, bool bShow, GameDef.eBuffType buffType)
	{
		AttributeValue health = attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
		AttributeValue healthMax = attributeManager.GetAttribute(AttributeValue.eAttributeType.HealthMax);
		
		if (health == null || healthMax == null)
			return 0.0f;
		
		float beforHP = health.Value;
		if (beforHP <= 0.01f)
		{
			string infoStr = string.Format("Hp is zero .. but IncHP called!! {0} - {1}", System.DateTime.Now.ToString("HH:mm:ss"), this);
			Debug.LogWarning(infoStr);
			return 0.0f;
		}
		
		float afterHP = beforHP + incValue;
		
		afterHP = Mathf.Clamp(afterHP, 0.0f, healthMax.Value);
		health.baseValue = afterHP;
		
		float applyHP = afterHP - beforHP;
		
		bool isChar = (myActorInfo.actorType == ActorInfo.ActorType.Player || myActorInfo.actorType == ActorInfo.ActorType.Escort);
		if (bShow == true && applyHP > 0.0f)
			ApplyDamageDigitUI(applyHP, false, isChar, buffType);
		
		attributeManager.UpdateValue(health);
		
		return afterHP;
	}
	
	public delegate void OnDieDelegate(LifeManager attacker);
	public OnDieDelegate onDie = null;
	public bool isSkipApplyMonsterSkill = false;
	
	static int killCount = 0;
	public void OnDie(LifeManager attacker)
	{
		gameObject.layer = LayerMask.NameToLayer("HideBody");
		
		//moveController.OnDie();
		
		if (buffManager != null)
			buffManager.Init();	
		
		if (onDie == null)
			return;
		
		if (isSkipApplyMonsterSkill == false)
		{
			string infoStr = string.Format("OnDie Called {0}, {1} - {2}", System.DateTime.Now.ToString("HH:mm:ss"), killCount++, this); 
			Debug.LogWarning(infoStr);
			
			ApplyMonsterKillCount(this);
		}
		
		if (stateController != null)
		{
			switch(stateController.currentState)
			{
			case BaseState.eState.Knockdownstart:
			case BaseState.eState.Knockdownfall:
			case BaseState.eState.Knockdownland:
				stateController.ChangeState(BaseState.eState.Knockdown_Die);
				break;
			default:
				stateController.ChangeState(BaseState.eState.Die);
				break;
			}
			
			//죽으면 더이상 상태 변경 안되도록...
			stateController.Locked = true;
		}
		
		if (onDie != null)
			onDie(attacker);
	}
	
	public void ChangeTarget(LifeManager target)
	{
		if (target != null &&
			this.myActorInfo.myTeam == target.myActorInfo.myTeam)
			return;
		
		targetActor = target;
		
		TargetSearch searcher = this.gameObject.GetComponent<TargetSearch>();
		if (searcher != null && target != null)
			searcher.DoChangeTarget(target.myActorInfo);
	}
	
	public void AddFXDelayInfo(FXInfo fxInfo, float periodTime)
	{
		if (stateController != null)
			stateController.AddFXDelayInfo(fxInfo, periodTime);
	}
	
	public void AddFXDelayInfo(string fxInfo, float periodTime)
	{
		if (stateController != null)
			stateController.AddFXDelayInfo(fxInfo, eFXEffectType.ScaleNode, 1.0f, periodTime);
	}
	
	public void AddFXDelayInfo(string fxInfo, eFXEffectType type, float scale, float periodTime)
	{
		if (stateController != null)
			stateController.AddFXDelayInfo(fxInfo, type, scale, periodTime);
	}
	
	public void RemoveFXDelayInfo(FXInfo fxInfo)
	{
		if (stateController != null)
			stateController.RemoveFXDelayInfo(fxInfo);
	}
	
	public bool IsInvincibleState()
	{
		return attackStateInfo.stateInfo.defenseState == StateInfo.eDefenseState.DS_INVINCIBLE;
	}
	
	public float GetAttackDamage()
	{
		float attackDamage = this.attackStateInfo.GetAttackDamage();
		return attackDamage;
	}
	
	public float GetAbilityPower()
	{
		float abilityPower = this.attackStateInfo.GetAbilityPower();
		return abilityPower;
	}
	
	public void ChangeAnimationSpeed(float speedRate)
	{
		if (stateController != null)
			stateController.ChangeAnimationSpeed(speedRate);
	}
	
	public float GetHP()
	{
		float curHP = 0.0f;
		AttributeValue health = attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
		if (health != null)
			curHP = health.Value;
		
		return curHP;
	}
	
	public float GetHPRate()
	{
		AttributeValue health = attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
		AttributeValue healthMax = attributeManager.GetAttribute(AttributeValue.eAttributeType.HealthMax);
		
		float curHP = 1.0f;
		float maxHP = 1.0f;
		if (health != null)
		{
			curHP = health.Value;
			if (curHP <= 0.01f)
				curHP = 0.0f;
		}
		
		if (healthMax != null)
			maxHP = healthMax.Value;
		
		float hpRate = 0.0f;
		if (maxHP != 0.0f && curHP != 0.0f)
			hpRate = curHP / maxHP;
		
		return hpRate;
	}
	
	public Transform uiRoot = null;
	[HideInInspector]
	public GameObject chatDialog = null;
	public bool bHPCheck = true;
	public virtual void DoTalk(string msg, float delayTime, DialogInfo.eDialogType bubbleType, bool isInputPause)
	{
		if (bHPCheck == true && GetHPRate() <= 0.0f)
			return;
		
		string path = "UI/ChatBubble/";
		string fileName = "DialogBox_Normal";
		switch(bubbleType)
		{
		case DialogInfo.eDialogType.Big:
			fileName = "DialogBox_Big";
			break;
		case DialogInfo.eDialogType.Normal:
			fileName = "DialogBox_Normal";
			break;
		case DialogInfo.eDialogType.Small:
			fileName = "DialogBox_Small";
			break;
		}
		
		if (chatDialog != null)
		{
			DestroyObject(chatDialog.gameObject, 0.1f);
			chatDialog = null;
		}
		
		//chatDialog = (GameObject)Instantiate(Resources.Load(path + fileName));
		chatDialog = ResourceManager.CreatePrefab(path + fileName);
		
		ChatBubble chatBubble = null;
		if (chatDialog != null)
		{
			chatDialog.transform.parent = this.damageUIRoot2;
			chatDialog.transform.localPosition = Vector3.zero;
			chatDialog.transform.localScale = Vector3.one;
			chatDialog.transform.localRotation = Quaternion.identity;
			
			chatBubble = chatDialog.GetComponent<ChatBubble>();
		}
		
		if (chatBubble != null)
		{
			chatBubble.SetMsg(msg, delayTime);
		}
	}
	
	public void ApplyMonsterKillCount(LifeManager actor)
	{
		if (myActorInfo == null)
			return;
		
		ActorManager actorManager = ActorManager.Instance;
		if (actorManager != null && actorManager.playerInfo != null)
		{
			if (myActorInfo.myTeam == actorManager.playerInfo.myTeam)
				return;
		}
		
		EventConditionChecker checker = EventConditionChecker.Instance;
		if (checker != null)
			checker.ApplyMonsterKillCount(myActorInfo.actorType);
		else
		{
			Debug.LogWarning("checker is null.......");
		}
	}
	
	
	public int charLevel = 0;
	public void SetExp(long expValue)
	{
		this.expValue = expValue;
		
		int nLevel = 1;
		
		CharExpTable expTable = null;
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
			expTable = tableManager.charExpTable;
		
		if (expTable != null)
			nLevel = expTable.GetLevel(expValue);
		
		SetLevel(nLevel);
	}
	
	public UILabel charLevelLabel = null;
	public void SetLevel(int level)
	{
		//int incLevel = level - charLevel;
		
		if (attributeManager != null)
			attributeManager.UpdateLevel(level - 1);
		
		charLevel = level;
		
		if (charLevelLabel != null)
			charLevelLabel.text = charLevel.ToString();
	}
	
	public void ChangeCostumeTexture(Texture costumeTexture)
	{
		if (meshRenderers == null)
			return;
		
		foreach(Renderer renderer in meshRenderers)
		{
			if (renderer.material != null && renderer.material.mainTexture != null)
			{
				renderer.material.mainTexture = costumeTexture;
			}
		}
	}
	
	public void UpdateBodyColor(Color buffColor, float changeRate)
	{
		if (meshRenderers == null)
			return;
		
		foreach(Renderer renderer in meshRenderers)
		{
			if (renderer.material != null && renderer.material.HasProperty("_BuffColor") == true)
			{
				Color origColor = renderer.material.GetColor("_BuffColor");
				float origAlaha = origColor.a;
				Color newColor = Color.Lerp(origColor, buffColor, changeRate);
				newColor.a = origAlaha;
				
				renderer.material.SetColor("_BuffColor", newColor);
			}
		}
		
		UpdateWeaponAndCostumeColor(buffColor, changeRate);
	}
	
	public void UpdateBodyAlpha(float alpha)
	{
		if (meshRenderers == null)
			return;
		
		foreach(Renderer renderer in meshRenderers)
		{
			if (renderer.material != null && renderer.material.HasProperty("_Alpha") == true)
			{
				/*
				float origAlpha = renderer.material.GetFloat("_Alpha");
				origAlpha = alpha;
				
				renderer.material.SetFloat("_Alpha", origAlpha);
				*/
				
				Color origAlpha = renderer.material.GetColor("_Alpha");
				origAlpha.a = alpha;
				
				renderer.material.SetColor("_Alpha", origAlpha);
			}
		}
		
		UpdateWeaponAndCostumeAlpha(alpha);
	}
	
	public void UpdateWeaponAndCostumeAlpha(float alpha)
	{
		List<GameObject> objList = new List<GameObject>();
		if (costumeBack != null)
			objList.Add(costumeBack);
		if (costumeHead != null)
			objList.Add(costumeHead);
		
		foreach(GameObject obj in weaponList)
		{
			if (obj != null)
				objList.Add(obj);
		}
		
		UpdateObjectAlpha(objList, alpha);
	}
	
	public void UpdateWeaponAndCostumeColor(Color buffColor, float changeRate)
	{
		List<GameObject> objList = new List<GameObject>();
		if (costumeBack != null)
			objList.Add(costumeBack);
		if (costumeHead != null)
			objList.Add(costumeHead);
		
		foreach(GameObject obj in weaponList)
		{
			if (obj != null)
				objList.Add(obj);
		}
		
		UpdateObjectColor(objList, buffColor, changeRate);
	}
	
	public void UpdateObjectAlpha(List<GameObject> objList, float alpha)
	{
		foreach(GameObject obj in objList)
		{
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
			
			if (renderers != null)
			{
				foreach(Renderer renderer in renderers)
				{
					if (renderer.material != null && renderer.material.HasProperty("_Alpha") == true)
					{
						/*
						float origAlpha = renderer.material.GetFloat("_Alpha");
						origAlpha = alpha;
						
						renderer.material.SetFloat("_Alpha", origAlpha);
						*/
						
						Color origAlpha = renderer.material.GetColor("_Alpha");
						origAlpha.a = alpha;
						
						renderer.material.SetColor("_Alpha", origAlpha);
					}
				}
			}
		}
	}
	
	public void UpdateObjectColor(List<GameObject> objList, Color buffColor, float changeRate)
	{
		foreach(GameObject obj in objList)
		{
			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
			
			if (renderers != null)
			{
				foreach(Renderer renderer in renderers)
				{
					if (renderer.material != null && renderer.material.HasProperty("_BuffColor") == true)
					{
						Color origColor = renderer.material.GetColor("_BuffColor");
						float origAlaha = origColor.a;
						Color newColor = Color.Lerp(origColor, buffColor, changeRate);
						newColor.a = origAlaha;
						
						renderer.material.SetColor("_BuffColor", newColor);
					}
				}
			}
		}
	}
	
	public GameObject defaultWeapon = null;
	public List<Transform> weaponNodes = new List<Transform>();
	public List<GameObject> weaponList = new List<GameObject>();
	
	public int equipWeaponID = -1;
	public void ChangeWeapon(int weaponID)
	{
		foreach(GameObject wpObj in weaponList)
			DestroyObject(wpObj);
		
		weaponList.Clear();
		
		string path = "WP/";
		string fileName = "";
		
		GameObject prefab = null;
		
		if (weaponID != -1)
		{
			WeaponTable weaponTable = null;
			TableManager tableManager = TableManager.Instance;
			if (tableManager != null)
				weaponTable = tableManager.weaponTable;
			
			if (weaponTable != null)
				fileName = weaponTable.GetData(weaponID);
			
			if (fileName.Contains("not Found!") == true)
				fileName = "";
		}
		
		if (fileName != "")
			prefab = ResourceManager.LoadPrefab(path + fileName);
		
		if (prefab == null)
			prefab = defaultWeapon;
		
		foreach(Transform trans in weaponNodes)
		{
			GameObject newWeapon = CreateObjectByPrefab(prefab, trans);
			weaponList.Add(newWeapon);
		}
	}
	
	[HideInInspector]
	public GameObject costumeBack = null;
	[HideInInspector]
	public GameObject costumeHead = null;
	
	[HideInInspector]
	public int costumeBackID = -1;
	[HideInInspector]
	public int costumeHeadID = -1;
	[HideInInspector]
	public int costumeBodyID = -1;
	
	public Texture defaultCostumeTexture = null;
	[HideInInspector]
	public Texture costumeTexture = null;
	public void ChangeCostume(int bodyID, int headID, int backID)
	{
		CostumeTable costumeTable = null;
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
			costumeTable = tableManager.costumeTable;
		
		if (costumeTable == null)
			return;
		
		string path = "NewAsset/Costume/";
		
		CostumeInfo info = null;
		Transform prefabNode = null;
		GameObject prefab = null;
		if (costumeBackID != backID)
		{
			costumeBackID = backID;
			
			if (costumeBack != null)
			{
				DestroyObject(costumeBack);
				costumeBack = null;
			}
			
			if (backID != -1)
			{
				info = costumeTable.GetData(backID);
				
				if (info != null)
				{
					prefab = ResourceManager.LoadPrefab(path + info.prefabFileName);
					prefabNode = FindNode(info.option);
				}
				
				if (prefab != null)
					costumeBack = CreateObjectByPrefab(prefab, prefabNode);
			}
		}
		
		prefab = null;
		if (costumeHeadID != headID)
		{
			costumeHeadID = headID;
			
			if (costumeHead != null)
			{
				DestroyObject(costumeHead);
				costumeHead = null;
			}
			
			if (headID != -1)
			{
				info = costumeTable.GetData(headID);
				
				if (info != null)
				{
					prefab = ResourceManager.LoadPrefab(path + info.prefabFileName);
					prefabNode = FindNode(info.option);
				}
				
				if (prefab != null)
					costumeHead = CreateObjectByPrefab(prefab, prefabNode);
			}
		}
		
		if (costumeBodyID != bodyID)
		{
			costumeBodyID = bodyID;
			
			info = costumeTable.GetData(bodyID);
			if (info != null)
				costumeTexture = ResourceManager.LoadTexture(path + info.prefabFileName);//Resources.Load(path + info.prefabFileName) as Texture;
			else
				costumeTexture = null;
		}
		if (costumeTexture == null)
			costumeTexture = defaultCostumeTexture;
		
		ChangeCostumeTexture(costumeTexture);
	}
	
	public GameObject CreateObjectByPrefab(GameObject prefab, Transform root)
	{
		GameObject newObj = null;
		if (prefab != null)
		{
			newObj = (GameObject)GameObject.Instantiate(prefab);
			
			if (newObj != null)
			{
				newObj.transform.parent = root;
				
				Transform objTrans = newObj.transform;
				objTrans.localPosition = Vector3.zero;
				objTrans.localRotation = Quaternion.identity;
				objTrans.localScale = Vector3.one;
			}
		}
		
		return newObj;
	}
	
	public Transform FindNode(string nodeName)
	{
		Transform findNode = null;
		
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		if (transforms != null)
		{
			foreach (Transform trans in transforms)
			{
				if (trans != null && trans.name == nodeName)
				{
					findNode = trans;
					break;
				}
			}
		}
		
		return findNode;
	}
	
	public delegate void OnApplyReflectDamage(float reflectDamage);
	public OnApplyReflectDamage onApplyReflectDamage = null;
	public void ApplyReflectDamage(float reflectDamage)
	{
		if (onApplyReflectDamage != null)
			onApplyReflectDamage(reflectDamage);
	}
	
	public delegate void OnActivateMonsterGeneratorByPhase2();
	public OnActivateMonsterGeneratorByPhase2 onActivateMonsterGeneratorByPhase2 = null;
	public void ActivatePhase2()
	{
		int buffIndex = buffManager.GetBuff(GameDef.eBuffType.BT_BOSSRAID_PHASE2);
		if (buffIndex != -1)
			buffManager.RemoveBuff(buffIndex);
		
		buffManager.AddBuff(GameDef.eBuffType.BT_BOSSRAID_PHASE2, phase2Value, -1.0f, this, 1);
		
		if (onActivateMonsterGeneratorByPhase2 != null)
			onActivateMonsterGeneratorByPhase2();
		
		WarningPhase2();
	}
	
	public string warningPhase2Prefab = "UI/Boss/Warning_Boss_Rage";
	public float warningLifeTime = 3.0f;
	public void WarningPhase2()
	{
		UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
		
		GameObject warning = ResourceManager.CreatePrefab(warningPhase2Prefab, uiRoot.transform, Vector3.zero);
		if (warning != null)
		{
			DestroyObject(warning, warningLifeTime);
		}
	}
	
	
	public string comboUIPrefabPath = "UI/DamageUI/ComboCountUI";
	private ComboCountUI comboCountUI = null;
	private float lifeTime = 4.0f;
	public virtual void ApplyComboCount(int nCount)
	{
		Transform uiRoot = null;
		if (GameUI.Instance.uiRootPanel != null)
			uiRoot = GameUI.Instance.uiRootPanel.transform;
		
		if (comboCountUI == null)
		{
			comboCountUI = ResourceManager.CreatePrefab<ComboCountUI>(comboUIPrefabPath, uiRoot, Vector3.zero);
			lifeTime = comboCountUI.TotalTime;
		}
		
		if (comboCountUI != null)
		{
			comboCountUI.TraceObject = transform;
			comboCountUI.owner = this;
			comboCountUI.SetComboCount(nCount);
			comboCountUI.TotalTime = lifeTime;
		}
	}
	
	public void DeleteComboUI()
	{
		if (comboCountUI != null)
		{
			comboCountUI.DoDestory();
			comboCountUI = null;
		}
	}
	
	public float CalcMagicDamage(LifeManager attacker, float origDamage)
	{
		float magicResist = this.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.MagicResist);
		float magicPenetration = 0.0f;
		if (attacker != null)
			magicPenetration = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.MagicPenetration);
	
		float defenceValue = Mathf.Max(0.0f, (magicResist - magicPenetration));
		float reduceDamageRate = defenceValue / (defenceValue + defenceCalcValue);
		
		float damageRate = Mathf.Max(limitDamageRate, (1.0f - reduceDamageRate));
		
		float resultDamage = origDamage * damageRate;
		
		float incDamageRate = 0.0f;
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 50% 이상 체력에 추가 데미지.
		float incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageWhenOverHP50);
		float curHPRate = GetHPRate();
		if (curHPRate >= 0.5f)
		{
			Debug.Log("Over HP 50 : addDamageRate : " +  incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//공격자 hp100%인 경우 피해량 증가.
		incTempValue = 0.0f;
		if (attacker != null)
			incTempValue = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncDamageWhenHP100);
		if (incTempValue != 0.0f && attacker != null && attacker.GetHPRate() >= 1.0f)
		{
			Debug.Log("Attacker HP100 Inc Damage : " + incTempValue);
			incDamageRate += incTempValue;
		}
		////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
		if (incDamageRate != 0.0f)
			resultDamage += resultDamage * incDamageRate;
		
		return resultDamage;
	}
	
	public void ApplyTimeLimitBuff(List<TimeLimitBuffInfo> timeLimitBuffs)
	{
		if (timeLimitBuffs == null || timeLimitBuffs.Count == 0)
			return;
		
		List<TimeLimitBuffInfo> availableList = new List<TimeLimitBuffInfo>();
		System.DateTime nowTime = System.DateTime.Now;
		
		System.TimeSpan timeSpan;
		foreach(TimeLimitBuffInfo info in timeLimitBuffs)
		{
			timeSpan = info.endTime - nowTime;
			if (timeSpan.TotalSeconds <= 0)
				continue;
			
			availableList.Add(info);
		}
		
		if (availableList.Count == 0)
			return;
		
		Dictionary<AttributeValue.eAttributeType, float> applyBuffList = new Dictionary<AttributeValue.eAttributeType, float>();
		foreach(TimeLimitBuffInfo info in availableList)
		{
			if (applyBuffList.ContainsKey(info.buffType) == false)
				applyBuffList.Add(info.buffType, info.buffValue);
			else
			{
				float buffValue = applyBuffList[info.buffType];
				//buffValue += info.buffValue;
				buffValue = info.buffValue;
				
				applyBuffList[info.buffType] = buffValue;
			}
		}
		
		foreach(var temp in applyBuffList)
			attributeManager.AddValue(temp.Key, temp.Value);
	}
}
