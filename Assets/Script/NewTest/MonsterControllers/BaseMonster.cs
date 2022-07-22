using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BaseAttackInfo
{
	public const float rangeDelta = 0.6f;
	
	public Vector2 attackHorizontalRange = Vector2.zero;
	public Vector2 attackVerticalRange = Vector2.zero;
	
	public float attackDelayTime = 0.0f;
	public float attackCoolTime = 0.0f;
	
	public int attackProbability = 100;
	
	public int priority = 1;
	
	public bool availableSameGround = true;
	
	public bool bRankAdjust = false;
	public float rankAdjustRate = 0.0f;
	
	public BaseState.eState attackState = BaseState.eState.Attack1;
	
	//데미지 상태 무시 하고 공격 가능 하도록..
	public bool ignoreDamageState = false;
	
	public void ResetCoolTime()
	{
		attackDelayTime = attackCoolTime;
	}
	
	public void UpdateCoolTime()
	{
		if (attackDelayTime <= 0.0f)
			attackDelayTime = 0.0f;
		else
			attackDelayTime -= Time.deltaTime;
	}
	
	public bool IsAvailableAttack(int randValue)
	{
		if (attackDelayTime > 0.0f)
			return false;
		
		//if (attackProbability < randValue)
		//	available = false;
		
		return true;
	}
	
	public bool IsAvailableAttack(float diffX, float diffY, bool bSameGround)
	{
		if (availableSameGround == true && bSameGround == false)
			return false;
		
		if (attackDelayTime > 0.0f)
			return false;
		
		if (attackHorizontalRange.x - rangeDelta > diffX || diffX > attackHorizontalRange.y + rangeDelta)
			return false;
		
		/*
		if (attackVerticalRange.x > diffY || diffY > attackVerticalRange.y)
			available = false;
		*/

		return true;
	}
	
	public bool IsAvailableRange(float diffX, bool bSameGround)
	{
		if (availableSameGround == true && bSameGround == false)
			return false;
		
		if (attackHorizontalRange.x - rangeDelta > diffX || diffX > attackHorizontalRange.y + rangeDelta)
			return false;
		
		return true;
	}
	
	public bool IsAvailableAttack(int randValue, float diffX, float diffY, bool bSameGround)
	{
		if (attackDelayTime > 0.0f)
			return false;
		
		if (attackProbability < randValue)
			return false;
		
		if (availableSameGround == true && bSameGround == false)
			return false;
		
		if (attackHorizontalRange != Vector2.zero)
		{
			if (attackHorizontalRange.x - rangeDelta > diffX || diffX > attackHorizontalRange.y + rangeDelta)
				return false;
		}
		
		if (attackVerticalRange != Vector2.zero)
		{
			if (attackVerticalRange.x > diffY || diffY > attackVerticalRange.y + rangeDelta)
				return false;
		}
		
		return true;
	}
	
	public bool IsAvailableAttack(int randValue, float diffX, float diffY, bool bSameGround, int rankValue)
	{
		if (attackDelayTime > 0.0f)
			return false;
		
		int resultRate = attackProbability;
		if (this.bRankAdjust == true && this.rankAdjustRate != 0.0f)
			resultRate -= Mathf.RoundToInt(this.rankAdjustRate * (float)rankValue);
		
		if (resultRate < randValue)
			return false;
		
		if (availableSameGround == true && bSameGround == false)
			return false;
		
		if (attackHorizontalRange != Vector2.zero)
		{
			if (attackHorizontalRange.x - rangeDelta > diffX || diffX > attackHorizontalRange.y + rangeDelta)
				return false;
		}
		
		if (attackVerticalRange != Vector2.zero)
		{
			if (attackVerticalRange.x > diffY || diffY > attackVerticalRange.y + rangeDelta)
				return false;
		}
		
		return true;
	}
	
	public static int SortFunc(BaseAttackInfo x, BaseAttackInfo y)
	{
		int xValue = x == null ? int.MinValue : x.priority;
		int yValue = y == null ? int.MinValue : y.priority;
		return yValue - xValue;
	}
}

[RequireComponent(typeof(AudioSource))]
public class BaseMonster : MonoBehaviour {
	public int stageType = 0;
	//public int monsterID = -1;
	
	public ActorInfo myInfo = null;
	
	public StateController stateController = null;
	public AnimationEventTrigger animEventTrigger = null;
	
	public BaseMoveController moveController = null;
	
	public BuffManager buffManager = null;
	public LifeManager lifeManager = null;
	public ActorInfo targetInfo = null;
	
	public List<BaseAttackInfo> attackList = new List<BaseAttackInfo>();
	[HideInInspector]
	public int curAttackIndex = -1;
	
	public BaseAttackInfo counterAttackInfo = new BaseAttackInfo();
	//
	public float evasiveRate = 0.0f;
	
	public BaseAttackInfo blockAttackInfo = new BaseAttackInfo();
	public float blockRate = 0.0f;
	
	[HideInInspector]
	public Vector2 attackRange = Vector2.zero;
	[HideInInspector]
	public bool ignoreAttackRange = false;
	
	public float limitObstacleLength = 7.0f;
	public float limitTargetLength = 5.0f;
	
	public Vector2 limitFollowRate = new Vector2(0.6f, 0.8f);
	public float limitFollowLength = 2.0f;
	[HideInInspector]
	public float followDelayTime = 0.5f;
	public float followCoolTime = 0.5f;
	
	//public float ignoreAttackRangeRate = 0.5f;
	//private bool doBackWalk = false;
	//private float attackCheckDelayTime = 0.0f;
	//private float attackCheckCoolTime = 0.5f;
	
	public string fxManaShield = "FX_mana_shield_01";
	public string fxCurseTarget = "FX_curse_target_particle";
	public string fxPoisonTarget = "FX_poison_target_particle";
	
	
	public Path targetPath = new Path();
	
	protected GameObject MeshNode = null;
	protected Renderer[] meshRenderers = null;
	
	public int attributeTableID = 1000;
	
	public float destroyDelayTime = 1.5f;
	
	public GameObject hpProgressPrefab = null;
	public Transform hpUIPos = null;
	public NormalHP normalHP = null;
	
	// Use this for initialization
	public virtual void Start () {
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
			animEventTrigger.onArrowEquip = new AnimationEventTrigger.OnAnimationEvent(OnArrowEquip);
			
			animEventTrigger.onDialogStart = new AnimationEventTrigger.OnAnimationEvent(OnDialogStart);
			
			animEventTrigger.onPlaySoundA = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundA);
			animEventTrigger.onPlaySoundB = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundB);
			animEventTrigger.onPlaySoundC = new AnimationEventTrigger.OnAnimationEventByString(OnPlaySoundC);
			
			animEventTrigger.onStopSound = new AnimationEventTrigger.OnAnimationEvent(OnStopSound);
		}
		
		if (moveController != null)
			moveController.onCollision = new BaseMoveController.OnCollision(OnCollision);
		
		if (lifeManager != null)
		{
			lifeManager.checkAvoid = new LifeManager.CheckFunc(CheckAvoid);
			lifeManager.checkBlock = new LifeManager.CheckFunc(CheckBlock);
			
			lifeManager.onDamage = new LifeManager.OnDamageDelegate(OnDamage);
			
			lifeManager.onDie = new LifeManager.OnDieDelegate(OnDie);
			
			lifeManager.onActivateMonsterGeneratorByPhase2 = new LifeManager.OnActivateMonsterGeneratorByPhase2(OnActivateMonsterGeneratorByPhase2);
		}
		
		if (buffManager != null)
		{
			buffManager.onSelectBuffFX = new BuffManager.SelectBuffFX(SelectBuffFXObject);
		}
		
		ActorManager actorManager = ActorManager.Instance;
		myInfo = GetComponent<ActorInfo>();
		if (myInfo != null)
		{
			if (actorManager != null)
				actorManager.AddActor(myInfo.myTeam, myInfo);
		}
		
		CalcAttackRange();
		
		myWayTypeMask = 1;
		if (stateController.IsContainState(BaseState.eState.JumpStart) == true)
			myWayTypeMask |= 1 << (int)WayPoint.eWayType.Jump;
		
		
		Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
		FindMeshNode(transforms);
		
		foreach(BaseAttackInfo attackInfo in attackList)
		{
			attackInfo.attackDelayTime = attackInfo.attackCoolTime;
		}
		
		if (hpProgressPrefab != null && hpUIPos != null)
		{
			GameObject hpProgress = (GameObject)Instantiate(hpProgressPrefab);
			if (hpProgress != null)
			{
				hpProgress.transform.parent = hpUIPos;
				hpProgress.transform.localPosition = Vector3.zero;
				hpProgress.transform.localScale = Vector3.one;
				hpProgress.transform.localRotation = Quaternion.identity;
				
				normalHP = hpProgress.GetComponent<NormalHP>();
				if (normalHP != null)
					lifeManager.attributeManager.hpUI = normalHP.hp;
			}
		}
	}
	
	public bool isInitGroundPos = false;
	void Awake()
	{
		if (stateController != null && stateController.beginState == BaseState.eState.Sleep)
			isInitGroundPos = true;
		
		if (isInitGroundPos == true && this.moveController != null)
		{
			Vector3 myPos = this.transform.position;
			
			RaycastHit hitInfo;
			if (Physics.Raycast(myPos, Vector3.down, out hitInfo, float.MaxValue, this.moveController.layerMask) == true)
			{
				this.transform.position = hitInfo.point;
			}
		}
		
		sound = gameObject.GetComponent<AudioSource>();
		if (sound == null)
			sound = gameObject.AddComponent<AudioSource>();
		
		InitAttributeData();
	}
	
	public virtual AttributeInitData InitAttributeData()
	{
		TableManager tableManager = TableManager.Instance;
		AttributeInitTable attributeTable = null;
		
		if (tableManager != null)
			attributeTable = tableManager.attributeInitTable;
		
		AttributeInitData initData = null;
		if (attributeTable != null)
			initData = attributeTable.GetData(attributeTableID);
		
		if (initData != null)
		{
			AttributeValue[] attributes = {
				new AttributeValue(AttributeValue.eAttributeType.AbilityPower, initData.abilityPower),
				new AttributeValue(AttributeValue.eAttributeType.AttackDamage, initData.attackDamage),
				new AttributeValue(AttributeValue.eAttributeType.CriticalHitRate, initData.criticalHitRate),
				new AttributeValue(AttributeValue.eAttributeType.CriticalDamageRate, initData.criticalDamageRate),
				new AttributeValue(AttributeValue.eAttributeType.Health, initData.healthMax),
				new AttributeValue(AttributeValue.eAttributeType.HealthMax, initData.healthMax),
				new AttributeValue(AttributeValue.eAttributeType.HealthRegen, initData.healthRegen),
				
				new AttributeValue(AttributeValue.eAttributeType.Armor, initData.armor),
				new AttributeValue(AttributeValue.eAttributeType.MagicResist, initData.magicResist),
				new AttributeValue(AttributeValue.eAttributeType.ArmorPenetration, initData.armorPenetration),
				new AttributeValue(AttributeValue.eAttributeType.MagicPenetration, initData.magicPenetration),

			};
			
			foreach(AttributeValue initValue in attributes)
			{
				lifeManager.attributeManager.AddAttributeValue(initValue);
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
				lifeManager.attributeManager.AddAttributeValue(initValue);
			}
		}
		
		return initData;
	}
	
	private void FindMeshNode(Transform[] transforms)
	{
		if (transforms == null)
			return;
		
		foreach (Transform trans in transforms)
		{
			if (trans != null && trans.name == "Mesh")
			{
				MeshNode = trans.gameObject;
				meshRenderers = MeshNode.GetComponentsInChildren<Renderer>();
				break;
			}
		}	
	}
	
	void CalcAttackRange()
	{
		foreach(BaseAttackInfo info in attackList)
		{
			if (attackRange.x > info.attackHorizontalRange.x)
				attackRange.x = info.attackHorizontalRange.x;
			
			if (attackRange.y < info.attackHorizontalRange.y)
				attackRange.y = info.attackHorizontalRange.y;
		}
	}
	
	public virtual void OnDestroy()
	{
		//Debug.Log(this.gameObject.name + " OnDestroy......");
		
		ActorManager actorManager = ActorManager.Instance;
		if (myInfo != null)
		{
			if (actorManager != null)
				actorManager.RemoveActor(myInfo.myTeam, myInfo);
		}
		
		if (monGenerator != null)
			monGenerator.RemoveMonster(this.gameObject);
		
		if (normalHP != null)
			normalHP.SetEnable(false);
		
		if (Game.Instance != null && Game.Instance.escortNPC != null)
			Game.Instance.escortNPC.monsterList.Remove(this.collider);
	}
	
	
	public int dropTableID = 101;
	public void MakeDropItem(LifeManager attacker)
	{
		if (dropTableID == -1)
			return;
		
		TableManager tableManager = TableManager.Instance;
		MonsterDropTable monsterDropTable = null;
		if (tableManager != null)
			monsterDropTable = tableManager.monsterDropTable;
		
		MonsterDropInfo dropInfos = null;
		if (monsterDropTable != null)
			dropInfos = monsterDropTable.GetData(dropTableID);
		
		if (dropInfos == null)
			return;
		
		int eventID = Game.Instance.GetEventID();
		MonsterDropItemInfo dropItemInfo = dropInfos.GetRandDropInfo(eventID);
		if (dropItemInfo == null)
			return;
		
		RaycastHit hitInfo;
		int layerMaskValue = LayerMask.NameToLayer("Ground");
		if (moveController != null)
			layerMaskValue = moveController.layerMask;// ^ moveController.enemyLayerMask;
		Vector3 vMyPos = this.transform.position + Vector3.up;
		
		Vector3 dropPos = vMyPos;
		if (Physics.Raycast(vMyPos, Vector3.down, out hitInfo, float.MaxValue, layerMaskValue) == true)
			dropPos = hitInfo.point;
		
		dropPos.z = 0.0f;
		
		string dropPrefabPath = "";
		int dropValue = 0;
		int addValue = 0;
		switch(dropItemInfo.dropType)
		{
		case MonsterDropItemInfo.eDropType.GoldDrop:
			dropPrefabPath = "NewAsset/Others/Drop/Drop_Coin";
			float addRate = 0.0f;
			if (attacker != null && attacker.attributeManager != null)
				addRate = attacker.attributeManager.GetAttributeValue(AttributeValue.eAttributeType.IncGainGold);
			
			dropValue = dropItemInfo.itemID;
			addValue = (int)(dropValue * addRate);
			break;
		case MonsterDropItemInfo.eDropType.JewelDrop:
			dropPrefabPath = "NewAsset/Others/Drop/Drop_Gem";
			dropValue = dropItemInfo.itemID;
			break;
		case MonsterDropItemInfo.eDropType.PotionDrop:
			dropPrefabPath = "NewAsset/Others/Drop/Drop_Potion";
			dropValue = dropItemInfo.itemID;
			break;
		case MonsterDropItemInfo.eDropType.ItemDrop:
			dropPrefabPath = "NewAsset/Others/Drop/Drop_Item";
			dropValue = dropItemInfo.itemID;
			break;
		case MonsterDropItemInfo.eDropType.MaterialItemDrop:
			dropPrefabPath = "NewAsset/Others/Drop/Drop_Material";
			dropValue = dropItemInfo.itemID;
			break;
		case MonsterDropItemInfo.eDropType.EventItemDrop:
			dropPrefabPath = string.Format("NewAsset/Others/Drop/EventDrop_{0}", dropItemInfo.itemID);
			dropValue = dropItemInfo.itemID;
			break;
		}
		
		BaseDropItem dropItem = ResourceManager.CreatePrefab<BaseDropItem>(dropPrefabPath);
		if (dropItem != null)
		{
			dropItem.transform.position = dropPos;
			
			dropItem.dropInfoValue = dropValue;
			dropItem.addValue = addValue;
			
			dropItem.OnActivate();
		}
	}
	
	// Update is called once per frame
	public virtual void Update () {
		
		if (Game.Instance.Pause == true)
			return;
		
		if (isEnableUpdate == false)
			return;
		
		if (idleTargetTime > 0.0f)
			idleTargetTime -= Time.deltaTime;
		
		if (isFirstDialog == true && stateController.currentState != BaseState.eState.StageEnd)
			UpdateFirstDialog();
		
		UpdateAttackInfo();
		
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
			UpdateDamage();
			break;
		case BaseState.eState.Stun:
			UpdateStun();
			break;
		case BaseState.eState.Die:
		case BaseState.eState.Knockdown_Die:
			UpdateDie();
			break;
		}
	}
	
	protected float destroyAlphaDelayTime = 0.0f;
	public virtual void UpdateDie()
	{
		if (bDestroyCalled == true)
		{
			float alpha = destroyAlphaDelayTime / destroyDelayTime;
			
			lifeManager.UpdateBodyAlpha(alpha);
			UpdateShadowAlpha(alpha);
			
			destroyAlphaDelayTime -= Time.deltaTime;
			if (destroyAlphaDelayTime < 0.0f)
				destroyAlphaDelayTime = 0.0f;
		}
	}
	
	public void UpdateShadowAlpha(float alpha)
	{
		if (moveController != null &&
			moveController.shadowObj != null)
		{
			Renderer[] renderers = moveController.shadowObj.GetComponentsInChildren<Renderer>();
			if (renderers != null)
			{
				foreach(Renderer renderer in renderers)
				{
					if (renderer.material != null && renderer.material.HasProperty("_TintColor") == true)
					{
						Color origColor = renderer.material.GetColor("_TintColor");
						origColor.a *= alpha;
						
						renderer.material.SetColor("_TintColor", origColor);
					}
				}
			}
		}
	}
	
	public virtual void UpdateDamage()
	{
		if (this.moveController != null && targetInfo != null)
		{
			WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			Vector3 targetPos = targetInfo.transform.position;
			float targetRadius = targetInfo.colliderRadius;
			
			Vector3 obstaclePos = targetPos;
			BaseAttackInfo attackInfo = null;
			if (targetInfo != null && CheckAttackTarget(targetInfo) == true)
				attackInfo = ChooseAttackIndex(targetPos, targetRadius, bSameGround);
			
			if (attackInfo != null)
			{
				ChangeMoveDir(targetPos);
				
				stateController.ChangeState(attackInfo.attackState);
				attackInfo.ResetCoolTime();
				
				attackTargetInfo = targetInfo;
				return;
			}
		}
	}
	
	public virtual void UpdateStun()
	{
		lifeManager.stunDelayTime -= Time.deltaTime;
		
		if (lifeManager.stunDelayTime <= 0.0f)
		{
			lifeManager.stunDelayTime = 0.0f;
			
			BaseState.eState nextState = BaseState.eState.Stand;
			if (MonsterGenerator.isSurrendMode == true)
				nextState = BaseState.eState.StageEnd;
			
			stateController.ChangeState(nextState);
		}
	}
	
	public void UpdateEvade()
	{
		
	}
	
	public void UpdateAttackInfo()
	{
		foreach(BaseAttackInfo info in attackList)
		{
			info.UpdateCoolTime();
		}
		
		counterAttackInfo.UpdateCoolTime();
		blockAttackInfo.UpdateCoolTime();
	}
	
	/*
	public void UpdateMove()
	{
		if (stateController.currentState == BaseState.eState.Die)
			return;
		
		attackCheckDelayTime -= Time.deltaTime;
		
		if (targetInfo != null)
		{
			Vector3 targetPos = targetInfo.transform.position;
			
			Vector3 obstaclePos = targetPos;
			
			if (moveController != null && moveController.CheckObstacle(out obstaclePos) == true)
				targetPos = obstaclePos;
			
			Vector3 diff = targetPos - this.transform.position;
			float diffX = Mathf.Abs(diff.x);
			
			Vector2 tempAttackRange = attackRange;
			BaseAttackInfo attackInfo = null;
			if (curAttackIndex != -1)
			{
				attackInfo = attackList[curAttackIndex];
				tempAttackRange = attackInfo.attackHorizontalRange;
			}
			
			if ((tempAttackRange.x - 0.5f <= diffX && diffX <= tempAttackRange.y + 0.5f))
			{
				stateController.ChangeState(BaseState.eState.Stand);
				return;
			}
			else if (tempAttackRange.x - 0.5f > diffX)
			{
				diff = this.transform.position - targetPos;
				Vector3 newDir = -moveController.moveDir;
				if (diff.x <= 0.0f)
					newDir = Vector3.left;
				else
					newDir = Vector3.right;
				
				CapsuleCollider capsule = (CapsuleCollider)collider;
				float radius = capsule.radius + moveController.skinWidth;
				
				float distance = moveController.CheckDistance(newDir, tempAttackRange.x);
				if (doBackWalk == false &&
					distance <= radius && distance != -1.0f)
				{
					ignoreAttackRange = true;
					if (DoAttack() == false)
						stateController.ChangeState(BaseState.eState.Stand);
				}
				else
				{
					if (doBackWalk == false)
					{
						doBackWalk = true;
						attackCheckDelayTime = attackCheckCoolTime;
					}
					
					if (attackCheckDelayTime <= 0.0f)
					{
						int randValue = Random.Range(0, 100);
						int randRate = Mathf.RoundToInt(ignoreAttackRangeRate * 100.0f);
						
						if (randRate > randValue)
						{
							ignoreAttackRange = true;
							if (DoAttack() == true)
								return;
						}
						
						attackCheckDelayTime = attackCheckCoolTime;
					}
					
					moveController.ChangeMoveDir(newDir);
				}
			}
		}
		else
			stateController.ChangeState(BaseState.eState.Stand);
	}
	public void UpdateIdle()
	{
		if (targetInfo != null)
		{
			Vector3 targetPos = targetInfo.transform.position;
			
			Vector3 obstaclePos = targetPos;
			
			if (moveController != null && moveController.CheckObstacle(out obstaclePos) == true)
			{
				targetPos = obstaclePos;
				
				if (stateController.IsContainState(BaseState.eState.JumpStart) == true)
				{
					Vector3 diffOb = obstaclePos - this.transform.position;
					if (Mathf.Abs(diffOb.x) < 1.1f)
					{
						stateController.ChangeState(BaseState.eState.JumpStart);
						
						moveController.moveSpeed = moveController.defaultMoveSpeed;
						moveController.DoJump();
						return;
					}
				}
				else if (HasProjectile() == false)
					targetPos = obstaclePos;
			}
			
			Vector3 vDiff = targetPos - this.transform.position;
			float diffX = Mathf.Abs(vDiff.x);
			
			Vector3 newDir = Vector3.zero;
			if (this.moveController != null)
			{
				newDir = this.moveController.moveDir;
			
				if (vDiff.x < 0.0f)
					newDir = Vector3.left;
				else if (vDiff.x > 0.0f)
					newDir = Vector3.right;
				
				switch(stateController.currentState)
				{
				case BaseState.eState.Stand:
				case BaseState.eState.Run:
					this.moveController.ChangeMoveDir(newDir);
					break;
				}
			}
			
			if (attackList.Count > 0 &&
				curAttackIndex != -1)
			{
				BaseAttackInfo attackInfo = attackList[curAttackIndex];
				
				CapsuleCollider capsule = null;
				if (collider.GetType() == typeof(CapsuleCollider))
					capsule = (CapsuleCollider)collider;
				
				if (capsule != null)
				{
					float radius = capsule.radius + moveController.skinWidth;
				
					if (attackInfo.attackHorizontalRange.x > 0.0f)
					{
						float distance = moveController.CheckDistance(newDir, attackInfo.attackHorizontalRange.x);
						if (doBackWalk == false &&
							distance <= radius && distance != -1.0f)
						{
							ignoreAttackRange = true;
						}
					}
				}
				
				if (DoAttack() == false)
					stateController.ChangeState(BaseState.eState.Run);
			}
			else
			{
				if ((attackRange.y - diffX) <= -0.5f)
					stateController.ChangeState(BaseState.eState.Run);
				else
					curAttackIndex = ChooseAttackIndex();
			}
		}
	}
	*/
	
	public bool isEnableUpdate = true;
	public virtual void UpdateIdle()
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
			WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			Vector3 targetPos = targetInfo.transform.position;
			float targetRadius = targetInfo.colliderRadius;
			
			Vector3 obstaclePos = targetPos;
			
			//Vector3 vDiff = targetPos - this.transform.position;
			float diffX = CalcDiffX(myInfo, targetInfo);
			bool isJump = false;
			if (moveController != null && moveController.CheckObstacle(out obstaclePos, limitObstacleLength) == true)
			{
				targetPos = obstaclePos;
				
				if (stateController.IsContainState(BaseState.eState.JumpStart) == true)
				{
					Vector3 diffOb = obstaclePos - this.transform.position;
					if (Mathf.Abs(diffOb.x) < 1.1f)
						isJump = true;
				}
				else if (HasProjectile() == false)
					targetPos = obstaclePos;
			}
			
			BaseAttackInfo attackInfo = null;
			if (targetInfo != null && CheckAttackTarget(targetInfo) == true)
				attackInfo = ChooseAttackIndex(targetPos, targetRadius, bSameGround);
			
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
						stateController.ChangeState(BaseState.eState.Run);
					
					followDelayTime = followCoolTime;
				}
				else
				{
					if (diffX > this.limitFollowLength)
						stateController.ChangeState(BaseState.eState.Run);
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
					
					List<BaseState.eState> stateList = GetWayInfoStateList(curWayInfo.wayTypeMask);
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
						stateController.ChangeState(nextState);
						
						if (nextState == BaseState.eState.JumpStart)
						{
							if (curWayInfo.vDir == Vector3.up)
								moveController.moveSpeed = 0.0f;
							else
								moveController.moveSpeed = moveController.defaultMoveSpeed;
							
							moveController.DoJump();
						}
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
							stateController.ChangeState(BaseState.eState.Run);
						
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
	
	
	public static void GetTargetGroundPos(ActorInfo target, List<float> targetList, float minX, float maxX)
	{
		if (target != null)
		{
			Collider collider = target.GetGroundCollider();
			if (collider == null || collider.GetType() != typeof(BoxCollider))
				return;
			
			BoxCollider groundBox = (BoxCollider)collider;
			Bounds groundArea = groundBox.bounds;
			
			Vector3 areaHalfSize = groundBox.size * 0.5f;
			Vector3 targetPos = target.transform.position;
			
			Vector3 minPos = groundArea.center - areaHalfSize;
			Vector3 maxPos = groundArea.center + areaHalfSize;
			
			float groundMin = Mathf.Min(maxX, Mathf.Max(minX, minPos.x));
			float groundMax = Mathf.Max(minX, Mathf.Min(maxX, maxPos.x));
			
			float targetX = Mathf.Min(maxX, Mathf.Max(minX, targetPos.x));
			
			targetList.Add(targetX);
			targetList.Add(groundMin);
			targetList.Add(groundMax);
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
				GetTargetGroundPos(targetInfo, targetPosList, minX, maxX);
			
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
	
	public static float CalcDiffX(ActorInfo actor1, ActorInfo actor2)
	{
		float diffX = 0.0f;
		
		Vector3 vDiff = actor1.transform.position - actor2.transform.position;
		diffX = Mathf.Abs(vDiff.x) - (actor1.colliderRadius + actor2.colliderRadius);
		if (diffX < 0.0f)
			diffX = 0.0f;
		
		return diffX;
	}
	
	public static WayPointManager GetWayPointManager(ActorInfo targetActor)
	{
		WayPointManager target = null;
		if (targetActor != null)
		{
			BaseMoveController moveCotrol = targetActor.gameObject.GetComponent<BaseMoveController>();
			if (moveCotrol != null)
				target = GetWayPointManager(moveCotrol.groundCollider);
		}
		
		return target;
	}
	
	public static WayPointManager GetWayPointManager(Collider groundCollider)
	{
		WayPointManager target = null;
		if (groundCollider != null)
			target = groundCollider.gameObject.GetComponent<WayPointManager>();
		
		return target;
	}
	
	public static List<BaseState.eState> GetWayInfoStateList(int wayTypeMask)
	{
		List<BaseState.eState> stateList = new List<BaseState.eState>();
		
		int maskValue = 1 << (int)WayPoint.eWayType.Walk;
		if ((wayTypeMask & maskValue) != 0)
			stateList.Add(BaseState.eState.Run);
		
		maskValue = 1 << (int)WayPoint.eWayType.Jump;
		if ((wayTypeMask & maskValue) != 0)
			stateList.Add(BaseState.eState.JumpStart);
		
		return stateList;
	}
	
	public virtual void UpdateMove()
	{
		if (this.moveController != null)
		{
			WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
			WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			Vector3 myPos = this.transform.position;
			Vector3 targetPos = this.transform.position;
			float targetRadius = 0.0f;
			
			if (targetInfo != null)
			{
				targetPos = targetInfo.transform.position;
				targetRadius = targetInfo.colliderRadius;
			}
			
			Vector3 obstaclePos = targetPos;
			bool isJump = false;
			if (moveController != null && moveController.CheckObstacle(out obstaclePos, limitObstacleLength) == true)
			{
				targetPos = obstaclePos;
				
				if (stateController.IsContainState(BaseState.eState.JumpStart) == true)
				{
					Vector3 diffOb = obstaclePos - this.transform.position;
					if (Mathf.Abs(diffOb.x) < 1.1f)
						isJump = true;
				}
				else if (HasProjectile() == false)
					targetPos = obstaclePos;
			}
			
			BaseAttackInfo attackInfo = null;
			if (targetInfo != null && CheckAttackTarget(targetInfo) == true)
				attackInfo = ChooseAttackIndex(targetPos, targetRadius, bSameGround);
			
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
			
			if (targetPath.target != targetWayMgr && targetInfo != null)
				UpdateTargetPath(targetInfo.gameObject);
			
			if (targetWayMgr != null && 
				curWayMgr != null &&
				targetPath.target == curWayMgr)
			{
				targetPos = targetInfo.transform.position;
				
				ChangeMoveDir(targetPos);
				
				Vector3 diff = targetPos - myPos;
				float diffX = Mathf.Abs(diff.x) - (myInfo.colliderRadius + targetInfo.colliderRadius);
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
					
					List<BaseState.eState> stateList = GetWayInfoStateList(curWayInfo.wayTypeMask);
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
	
	public virtual void OnChangeState(CharStateInfo info)
	{
		//doBackWalk = false;
		
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
			lifeManager.SetAttackInfo(info.stateInfo);
			
			lifeManager.ClearHitObject();
		}
		
		if (stateController.currentState == BaseState.eState.Stand)
		{
			if (moveController != null)
			{
				moveController.ChangeDefaultLayer(true);
				
				if (this.targetInfo != null)
					UpdateTargetPath(this.targetInfo.gameObject);
			}
			
			if (rigidbody != null)
			{
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}
		
		if (stateController.colliderManager != null)
			stateController.colliderManager.colliderStep = 0;
		
		if (info != null)
			info.InitWalkingStep();
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
	
	public virtual void OnEndState()
	{
		switch(stateController.currentState)
		{
		case BaseState.eState.Stun:
			lifeManager.stunDelayTime = 0.0f;
			break;
		}
		
		attackTargetInfo = null;
	}
	
	public virtual void OnAnimationBegin()
	{
		
	}
	
	protected bool bDestroyCalled = false;
	public virtual BaseState.eState ChangeNextState()
	{
		BaseState.eState nextState = BaseState.eState.Stand;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.StageEndStart:
			nextState = BaseState.eState.StageEnd;
			break;
		case BaseState.eState.StageEnd:
			nextState = BaseState.eState.StageEnd;
			break;
		case BaseState.eState.Block:
			nextState = BaseState.eState.Stand;
			
			if (targetInfo != null)
			{
				WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
				WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
				
				bool bSameGround = targetWayMgr == curWayMgr;
				
				Vector3 diff = targetInfo.transform.position - this.transform.position;
				float diffX = Mathf.Max(0.0f, (Mathf.Abs(diff.x) - (myInfo.colliderRadius + targetInfo.colliderRadius)));
				float diffY = diff.y;
				
				int randValue = Random.Range(0, 100);
				if (blockAttackInfo.IsAvailableAttack(diffX, diffY, bSameGround) == true &&
					blockAttackInfo.attackProbability > randValue)
					nextState = BaseState.eState.BlockAttack;
			}
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
		case BaseState.eState.Knockdown_Die:
			nextState = stateController.currentState;
			if (destroyDelayTime >= 0.0f)
			{
				bDestroyCalled = true;
				destroyAlphaDelayTime = destroyDelayTime;
				DestroyObject(this.gameObject, destroyDelayTime);
			}
			break;
		case BaseState.eState.JumpStart:
			nextState = BaseState.eState.JumpStart;
			break;
		case BaseState.eState.JumpFall:
			nextState = BaseState.eState.JumpFall;
			break;
		case BaseState.eState.JumpAttack:
			nextState = BaseState.eState.JumpAttack;//BaseState.eState.Jumpland;
			break;
		case BaseState.eState.Drop:
			nextState = BaseState.eState.Blowattack;
			break;
		case BaseState.eState.Evadestart:
			nextState = BaseState.eState.Evadeend;
			
			if (targetInfo != null)
			{
				WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
				WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
				
				bool bSameGround = targetWayMgr == curWayMgr;
				
				Vector3 diff = targetInfo.transform.position - this.transform.position;
				float diffX = Mathf.Max(0.0f, (Mathf.Abs(diff.x) - (myInfo.colliderRadius + targetInfo.colliderRadius)));
				float diffY = diff.y;
				
				int randValue = Random.Range(0, 100);
				if (counterAttackInfo.IsAvailableAttack(diffX, diffY, bSameGround) == true &&
					counterAttackInfo.attackProbability > randValue)
				{
					nextState = BaseState.eState.Evadecounterattack;
					
					counterAttackInfo.ResetCoolTime();
				}
			}
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
		case BaseState.eState.Evadeend:
		case BaseState.eState.Evadecounterattack:
		case BaseState.eState.Heavyattack:
		case BaseState.eState.Skill01:
		case BaseState.eState.Skill02:
		case BaseState.eState.Attack1:
		case BaseState.eState.Attack2:
		case BaseState.eState.Attack3:
			nextState = BaseState.eState.Stand;
			break;
		default:
			switch(stateController.currentState)
			{
			case BaseState.eState.Dash:
			case BaseState.eState.Run:
				nextState = stateController.preState;
				break;
			}
			break;
		}
		
		return nextState;
	}
	
	public int endDialogID = -1;
	public float endDialogDelayTime = 1.5f;
	public virtual void OnAnimationEnd()
	{
		stateController.animationController.isAnimationPlaying = false;
		
		BaseState.eState nextState = ChangeNextState();
		
		if (MonsterGenerator.isSurrendMode == true)
		{
			switch(nextState)
			{
			case BaseState.eState.Stand:
				nextState = BaseState.eState.StageEnd;
				
				if (endDialogID != -1)
					DoTalk(endDialogID, endDialogDelayTime, DialogInfo.eDialogType.Normal, false);
				
				break;
			}
		}
		
		stateController.ChangeState(nextState);
	}
	
	public void DoTalk(int stringID, float delayTime, DialogInfo.eDialogType dlgType, bool inputPause)
	{
		if (stringID == -1)
			return;
		
		string dlgStr = "";
		StringTable stringTable = TableManager.Instance.stringTable;
		if (stringTable != null)
			dlgStr = stringTable.GetData(stringID);
		
		lifeManager.DoTalk(dlgStr, delayTime, DialogInfo.eDialogType.Normal, inputPause);
	}
	
	public void OnCollisionStart()
	{
		if (stateController.colliderManager == null)
			return;
		
		int attackStep = stateController.colliderManager.colliderStep;
		
		CollisionInfo colInfo = null;
		
		int nCount = stateController.curStateInfo.collisionInfoList.Count;
		if (nCount > 0)
		{
			attackStep = attackStep % nCount;
			colInfo = stateController.curStateInfo.collisionInfoList[attackStep];
		}
		
		if (colInfo != null)
		{
			stateController.colliderManager.SetupCollider(colInfo.colliderName, true);
			if (lifeManager != null)
				lifeManager.SetAttackInfo(colInfo.stateInfo);
		}
		
		stateController.colliderManager.IncColliderStep();
	}
	
	public void OnCollisionStop()
	{
		if (stateController.colliderManager == null)
			return;
		
		int attackStep = stateController.colliderManager.colliderStep - 1;
		
		CollisionInfo colInfo = null;
		
		int nCount = stateController.curStateInfo.collisionInfoList.Count;
		if (attackStep >= 0 && nCount > 0)
		{
			attackStep = attackStep % nCount;
			colInfo = stateController.curStateInfo.collisionInfoList[attackStep];
		}
		
		if (colInfo != null)
			stateController.colliderManager.SetupCollider(colInfo.colliderName, false);
		
		if (lifeManager != null)
		{
			lifeManager.SetAttackInfo(stateController.curStateInfo.stateInfo);
			lifeManager.ClearHitObject();
		}
	}
	
	public void OnWalkingStart()
	{
		if (moveController != null)
		{
			stateController.curStateInfo.IncWalkingStep();
			moveController.moveSpeed = stateController.curStateInfo.walkingEventMoveSpeed;
		}
	}
	
	public void OnWalkingStop()
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
		
	}
	
	public AudioSource sound = null;
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
	public virtual void OnFire()
	{
		OnCollisionStart();
		
		FireProjectile();
		
		OnCollisionStop();
		
		attackTargetInfo = null;
	}
	
	public void OnTargetHit(LifeManager hitActor)
	{
		
	}
	
	public virtual void DoArrowEquip()
	{
			
	}
	
	public void OnArrowEquip()
	{
		DoArrowEquip();
	}
	
	
	public WayPoint curWayPoint = null;
	public int curWayIndex = -1;
	public WayInfo curWayInfo = null;
	
	public int myWayTypeMask = 0;
	public void UpdateTargetPath(GameObject target)
	{
		//Debug.Log("UpdateTargetPath...");
		
		if (this.moveController != null)
		{
			BaseMoveController targetMoveController = target.GetComponent<BaseMoveController>();
			WayPointManager targetWayMgr = null;
			if (targetMoveController != null)
			{
				if (targetMoveController.groundCollider != null)
					targetWayMgr = targetMoveController.groundCollider.gameObject.GetComponent<WayPointManager>();
			}
			
			if (targetWayMgr != null)
			{
				WayPointManager curWayPointMgr = GetWayPointManager(this.moveController.groundCollider);
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
	
	public void ChangeMoveDir(Vector3 targetPos)
	{
		ChangeMoveDir(targetPos, false);
	}
	
	public void ChangeMoveDir(Vector3 targetPos, bool bForce)
	{
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
			
			if (bForce == false)
			{
				switch(stateController.currentState)
				{
				case BaseState.eState.Stand:
				case BaseState.eState.Run:
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
	
	private bool isFirstTargeting = true;
	public void OnFirstTargeting()
	{
		if (isFirstTargeting == true)
		{
			isFirstTargeting = false;
			
			if (myInfo.actorType == ActorInfo.ActorType.BossMonster)
			{
				PlayerController player = Game.Instance.player;
				
				ActorManager actorManager = ActorManager.Instance;
				if (actorManager != null && actorManager.playerInfo != null)
				{
					if (player != null && player.myStatusInfo != null)
					{
						player.myStatusInfo.ShowBossHP(true);
						lifeManager.attributeManager.hpUI = player.myStatusInfo.bossHP.hp;
						lifeManager.attributeManager.hpInfoLabel = player.myStatusInfo.bossHPInfoLabel;
						
						lifeManager.attributeManager.UpdateHPUI();
						
						SetBossFace(player.myStatusInfo.bossFaceNode);
					}
				}
				
				StageManager stageManager = Game.Instance.stageManager;
				if (stageManager != null)
					stageManager.StartBossBGM();
				
				BossDialogController bossDialogController = this.gameObject.GetComponent<BossDialogController>();
				if (bossDialogController != null)
					bossDialogController.BossDialogStart();
			}
			
			int nDialogCount = this.firstMeetDialogInfos.Count;
			if (nDialogCount > 0)
			{
				int randValue = Random.Range(0, 100);
				if (randValue <= firstDialogRate)
					isFirstDialog = true;
			}
		}
	}
	
	public GameObject bossFacePrefab = null;
	public GameObject bossFace = null;
	public void SetBossFace(Transform rootNode)
	{
		UIMyStatusInfo myStatusInfo = GameUI.Instance.myStatusInfo;
		if (myStatusInfo != null && myStatusInfo.bossFace == null)
		{
			if (rootNode == null || bossFacePrefab == null)
				return;
			
			bossFace = (GameObject)Instantiate(bossFacePrefab);
			if (bossFace != null)
			{
				Vector3 origScale = bossFace.transform.localScale;
				
				bossFace.transform.parent = rootNode;
				
				bossFace.transform.localPosition = Vector3.zero;
				bossFace.transform.localScale = origScale;
				bossFace.transform.localRotation = Quaternion.identity;
				
				UISprite faceSprite = bossFace.GetComponent<UISprite>();
				if (faceSprite != null)
					faceSprite.depth = 2;
			}
			
			myStatusInfo.bossFace = bossFace;
		}
	}
	
	public bool IsAttackState()
	{
		bool isAttack = false;
		
		BaseState.eState currentState = stateController.currentState;
		foreach(BaseAttackInfo info in attackList)
		{
			if (info.attackState == currentState)
			{
				if (stateController.animationController.isAnimationPlaying == true)
				{
					isAttack = true;
					break;
				}
			}
		}
		
		return isAttack;
	}
	
	public virtual void ChangeTarget(ActorInfo newTarget)
	{
		if (IsAttackState() == true)
			return;
		
		if (attackTargetInfo != null)
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
			
			curAttackIndex = ChooseAttackIndex();
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
		
		if (targetInfo != null && stateController.currentState == BaseState.eState.Sleep)
			stateController.ChangeState(BaseState.eState.Stand);
	}
	
	public int ChooseAttackIndex()
	{
		int index = -1;
		
		if (MonsterGenerator.isSurrendMode == true)
			return index;
		
		//if (CanAttackState() == true)
		{
			List<int> availableAttackList = new List<int>();
			int randValue = Random.Range(0, 100);
			
			int nCount = attackList.Count;
			for (int i = 0; i < nCount; ++i)
			{
				BaseAttackInfo info = attackList[i];
				
				if (CanAttackState(info) == false)
					continue;
				
				if (info.IsAvailableAttack(randValue) == false)
					continue;
				
				availableAttackList.Add(i);
			}
			
			nCount = availableAttackList.Count;
			if (nCount > 0)
			{
				int randIndex = Random.Range(0, nCount);
				index = availableAttackList[randIndex];
				
				//attackList[index].ResetCoolTime();
			}
		}
		
		return index;
	}
	
	public virtual BaseAttackInfo ChooseAttackIndex(Vector3 targetPos, float targetRadius, bool bSameGround)
	{
		LifeManager targetLifeMgr = null;
		if (targetInfo != null)
			targetLifeMgr = targetInfo.GetComponent<LifeManager>();
		
		if (targetLifeMgr != null && targetLifeMgr.GetHP() <= 0.0f)
			return null;
		
		BaseAttackInfo attackInfo = null;
		
		if (MonsterGenerator.isSurrendMode == true)
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
			
			int nCount = attackList.Count;
			for (int i = 0; i < nCount; ++i)
			{
				BaseAttackInfo info = attackList[i];
				
				if (CanAttackState(info) == false)
					continue;
				
				if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
					continue;
				
				availableAttackList.Add(info);
			}
			
			nCount = availableAttackList.Count;
			if (nCount > 1)
				availableAttackList.Sort(BaseAttackInfo.SortFunc);
			
			if (nCount > 0)
				attackInfo = availableAttackList[0];
		}
		
		return attackInfo;
	}
	
	public string avoidSoundFile = "";
	public bool CheckAvoid(LifeManager attacker)
    {
		if (MonsterGenerator.isSurrendMode == true)
			return false;
		
        if (stateController.IsJumpState() == true) return false;
		if (lifeManager.stunDelayTime > 0.0f) return false;
		
		int rateValue = Mathf.RoundToInt(evasiveRate * 10000.0f);
		int randValue = Random.Range(0, 10000);
		
		if (randValue > rateValue)
			return false;
        		
		switch(stateController.currentState)
		{
		case BaseState.eState.Evadecounterattack:
		case BaseState.eState.Evadestart:
			return false;
		}
		
		if (attacker != null)
		{
			if (lifeManager != null)
				lifeManager.ChangeMoveDir(attacker.transform);
		}
		
        stateController.ChangeState(BaseState.eState.Evadestart);
		
		lifeManager.ApplyEtcDamageUI(EtcDamageUI.eEtcDamge.Avoid);
		
		if (GameOption.effectToggle == true)
		{
			float effectVoluem = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(sound, avoidSoundFile, effectVoluem);
		}
		
        return true;
    }
	
	public string blockSoundFile = "";
	public virtual bool CheckBlock(LifeManager attacker)
    {
		if (MonsterGenerator.isSurrendMode == true)
			return false;
		
		if (lifeManager.stunDelayTime > 0.0f) return false;
		
        if (stateController.IsJumpState() == true) return false;
		
		if (stateController.currentState == BaseState.eState.Dashattack)
			return false;
		
		int rateValue = Mathf.RoundToInt(blockRate * 10000.0f);
		int randValue = Random.Range(0, 10000);
		
		if (randValue > rateValue)
			return false;
        
		if (attacker != null)
		{
			if (lifeManager != null)
				lifeManager.ChangeMoveDir(attacker.transform);
		}
		
		stateController.ChangeState(BaseState.eState.Block);
		
		lifeManager.ApplyEtcDamageUI(EtcDamageUI.eEtcDamge.Block);
		
		if (GameOption.effectToggle == true)
		{
			float effectVoluem = Game.Instance.effectSoundScale;
			AudioManager.PlaySound(sound, blockSoundFile, effectVoluem);
		}
		
        return true;
    }
	
	public void OnCollision()
	{
		switch(stateController.currentState)
		{
		case BaseState.eState.Run:
		case BaseState.eState.Dash:
			//stateController.ChangeState(BaseState.eState.Stand);
			break;
		}
	}
	
	public virtual bool HasProjectile()
	{
		return false;
	}
	
	public virtual bool CanAttackState(BaseAttackInfo attackInfo)
	{
		bool canAttack = false;
		
		switch(stateController.currentState)
		{
		case BaseState.eState.Stand:
		case BaseState.eState.Run:
		case BaseState.eState.Dash:
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
			}
		}
		return fxInfo;
	}
	
	public bool DoAttack()
	{
		bool doAttack = false;
		
		BaseAttackInfo attackInfo = null;
		if (curAttackIndex != -1)
			attackInfo = attackList[curAttackIndex];
		
		if (attackInfo == null || targetInfo == null)
			return doAttack;
		
		int randValue = Random.Range(0, 100);
		
		Vector3 vDiff = targetInfo.transform.position - this.transform.position;
		
		Vector3 newDir = Vector3.right;
		if (vDiff.x <= 0.0f)
			newDir = Vector3.left;
		else
			newDir = Vector3.right;
		
		WayPointManager targetWayMgr = GetWayPointManager(targetInfo);
		WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
		
		bool bSameGround = targetWayMgr == curWayMgr;
	
		bool rangeCheck = attackInfo.IsAvailableRange(Mathf.Abs(vDiff.x), bSameGround);
		bool coolTimeCheck = attackInfo.attackDelayTime <= 0.0f;
		bool probabilityCheck = attackInfo.attackProbability >= randValue;
		
		if (ignoreAttackRange == true || rangeCheck == true)
		{
			ignoreAttackRange = false;
			
			if (coolTimeCheck == true && probabilityCheck == true)
			{
				if (moveController != null)
					moveController.ChangeMoveDir(newDir);
				
				stateController.ChangeState(attackInfo.attackState);
				attackInfo.ResetCoolTime();
				
				curAttackIndex = ChooseAttackIndex();
				
				doAttack = true;
				
				attackTargetInfo = targetInfo;
			}
		}
		
		return doAttack;
	}
	
	public virtual void OnDialogStart()
	{
		
	}
	
	public MonsterGenerator monGenerator = null;
	public void SetMonsterGenerator(MonsterGenerator monGen)
	{
		monGenerator = monGen;
	}
	
	
	public bool isEventMode = false;
	//private Vector3 vMoveTargetPos = Vector3.zero;
	//private bool isTargetMoveComplete = false;
	public virtual void OnCompleteDialogEvent()
	{
		isEventMode = false;
		//isTargetMoveComplete = false;
	}
	
	public void SetSurrend()
	{
		if (stateController != null)
			stateController.ChangeState(BaseState.eState.StageEnd);
	}
	
	public virtual void OnDie(LifeManager attacker)
	{
		if (myInfo.actorType == ActorInfo.ActorType.BossMonster)
		{
			UIMyStatusInfo myStatusInfo = GameUI.Instance.myStatusInfo;
			if (myStatusInfo != null)
				myStatusInfo.bossHP.hp.sliderValue = 0.0f;
		}
		
		if (myInfo.actorType == ActorInfo.ActorType.Escort)
			return;
		
		StageManager stageManager = null;
		if (moveController != null && moveController.stageManager != null)
			stageManager = moveController.stageManager;
		
		if (stageManager != null)
		{
			switch(stageManager.StageType)
			{
			case StageManager.eStageType.ST_FIELD:
			case StageManager.eStageType.ST_EVENT:
				MakeDropItem(attacker);
				StageReward();
				
				if (enableSlowEvent == true)
					SlowEventStart();
				break;
			case StageManager.eStageType.ST_WAVE:
				if (enableWaveSlowEvent == true)
					SlowEventStart();
				break;
			}
		}
		
		if (stageManager != null)
		{
			if (stageManager.StageType == StageManager.eStageType.ST_FIELD
				|| stageManager.StageType == StageManager.eStageType.ST_EVENT)
			{
				Game.Instance.ApplyAchievement(Achievement.eAchievementType.eKillMonster, this.stageType, this.attributeTableID);
			}
		}
	}
	
	public float slowEventTime = 1.0f;
	public float slowEventTimeRateValue = 0.3f;
	public bool enableSlowEvent = false;
	public bool enableWaveSlowEvent = false;
	public void SlowEventStart()
	{
		Time.timeScale = slowEventTimeRateValue;
		Invoke("SlowEventStop", slowEventTime);
	}
	
	public void SlowEventStop()
	{
		Time.timeScale = 1.0f;
	}
	
	public virtual void StageReward()
	{
		StageManager stageManager = Game.Instance.stageManager;
		if (stageManager != null)
		{
			
		}
	}
	
	public int firstDialogRate = 50;
	public bool isFirstDialog = false;
	public float firstMeetDialogLimitLength = 10.0f;
	public List<DialogInfo> firstMeetDialogInfos = new List<DialogInfo>();
	public void UpdateFirstDialog()
	{
		PlayerController player = Game.Instance.player;
		
		float hpRate = lifeManager.GetHPRate();
		if (hpRate <= 0.0f)
			return;
		
		if (player == null || isFirstDialog == false)
			return;
		
		Vector3 vDiff = player.transform.position - this.transform.position;
		if (vDiff.magnitude <= firstMeetDialogLimitLength)
		{
			int nCount = firstMeetDialogInfos.Count;
			int nRandIndex = Random.Range(0, nCount);
			DialogInfo dlgInfo = null;
			if (nCount > 0)
				dlgInfo = firstMeetDialogInfos[nRandIndex];
			
			if (dlgInfo != null)
			{
				string msg = "";
				TableManager tableManager = TableManager.Instance;
				StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
				
				if (stringTable != null)
					msg = stringTable.GetData(dlgInfo.stringTableID);
				
				if (msg != "")
					lifeManager.DoTalk(msg, dlgInfo.delayTime, dlgInfo.dialogType, dlgInfo.preventInput);
			}	
			
			isFirstDialog = false;
		}
	}
	
	public void OnActivateMonsterGeneratorByPhase2()
	{
		StageManager stageManager = Game.Instance.stageManager;
		if (stageManager != null)
			stageManager.ActivateMonsterGeneratorByPhase2(this.attributeTableID);
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
	
	public void DoKnockDown(Vector3 vDir)
	{
		float knockdownPower = 1.0f;
		
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
			
			moveController.moveSpeed = -vDir.x * knockdownPower;
			
			Vector3 UpDir = Vector3.up * (vDir.y * 100.0f * knockdownPower);
			moveController.DoKnockDown(UpDir);
			
			stateController.ChangeState(BaseState.eState.Knockdownstart);
		}
	}
	
	public string dieSoundFile = "";
	public virtual void OnDamage(AttackStateInfo attackInfo, Transform hitPos, LifeManager.eDamageType damageType)
	{
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
}
