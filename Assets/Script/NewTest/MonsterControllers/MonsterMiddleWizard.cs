using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterMiddleWizard : MonsterWizard {
	public static List<MonsterMiddleWizard> middleWizardList = new List<MonsterMiddleWizard>();
	
	public MonsterMiddleWizard twinWizard = null;
	public Transform teleportTarget = null;
	
	public float teleportCoolTime = 10.0f;
	private float teleportDelayTime = 0.0f;
	public BaseState.eState teleportAction = BaseState.eState.Attack3;
	
	public Transform shotStart = null;
	
	public override void Start () {
		base.Start();
	
		teleportDelayTime = teleportCoolTime;
		Transform[] transforms = this.GetComponentsInChildren<Transform>();
		foreach(Transform transform in transforms)
		{
			if (transform.name == "shot")
			{
				shotStart = transform;
				break;
			}
		}
		
		if (this.moveController != null && this.moveController.stageManager != null)
		{
			GameObject target = this.moveController.stageManager.GetTeleportTarget();
			if (target != null)
				this.teleportTarget = target.transform;
		}
		
		middleWizardList.Add(this);
		int nActorCount = middleWizardList.Count;
		if (nActorCount > 0 && nActorCount % 2 == 0)
		{
			MonsterMiddleWizard actor1 = middleWizardList[nActorCount - 1];
			MonsterMiddleWizard actor2 = middleWizardList[nActorCount - 2];
			
			if (actor1 != null && actor2 != null)
			{
				actor1.twinWizard = actor2;
				actor2.twinWizard = actor1;
				
				//actor2.eventDialogList.Clear();
			}
		}
		
		if (lifeManager != null)
		{
			lifeManager.syncHPValue = new LifeManager.SyncHPValue(OnSyncHPValue);
		}
		
	}
	
	public override void OnDestroy()
	{
		base.OnDestroy();
		
		middleWizardList.Remove(this);
	}
	
	public override void Update ()
	{
		if (this.targetInfo != null)
		{
			teleportDelayTime -= Time.deltaTime;
		}
		
		base.Update();
	}
	
	private Vector3 FindTeleportPos()
	{
		Vector3 findPos = teleportTarget.position;
		
		RaycastHit hit;
		Vector3 vStartPos = teleportTarget.position + (Vector3.up * 1.0f);
		int layerMask = 1 << LayerMask.NameToLayer("Ground");
		
		if (Physics.Raycast(vStartPos, Vector3.down, out hit, Mathf.Infinity, layerMask) == true)
		{
			findPos = hit.point;// + Vector3.up;
		}
		
		return findPos;
	}
	
	public void Teleport()
	{
		if (teleportTarget != null)
		{
			teleportDelayTime = teleportCoolTime;
			
			this.gameObject.transform.position = FindTeleportPos();
		}
	}
	
	public void TeleportByOther()
	{
		if (teleportTarget != null)
		{
			this.gameObject.transform.position = FindTeleportPos();
		}
	}
	
	/*
	public override bool DecHP(float fValue, bool isCritical)
	{
		bool bResult = base.DecHP(fValue, isCritical);
		
		if (twinWizard != null)
		{
			//twinWizard.mAbility.HP = this.mAbility.HP;
			//twinWizard.Update();
		}
		
		return bResult;
	}
	*/
	
	public override void FireProjectile()
	{
		GameObject go = (GameObject)Instantiate(fireBallPrefab);
        if (go == null) return;
        FireBall fireBall = go.GetComponent<FireBall>();
        if (fireBall == null) return;

        Vector3 targetActorPos = attackTargetInfo.transform.position;
		Vector3 vCreatePos = targetActorPos;
		
		Vector3 targetPos = GetTargetPos(attackTargetInfo);

		Vector3 vMoveDir = moveController.moveDir;
		
		switch(this.stateController.currentState)
		{
		case BaseState.eState.Attack1:
			vCreatePos = targetActorPos + fireBallDeltaPos;
			
			vMoveDir = targetPos - vCreatePos;
			vMoveDir.Normalize();
			
			fireBall.bShowDropIndicator = true;
			break;
		case BaseState.eState.Attack2:
			if (shotStart != null)
				vCreatePos = shotStart.position;
			
			//방향 전환 안되도록..
			/*
			Vector3 newTargetDir = targetPos - this.transform.position;
			moveController.ChangeMoveDir(newTargetDir);
			*/
			
			vMoveDir = moveController.moveDir;
			
			//autoDestroy = true;
			fireBall.bShowDropIndicator = false;
			break;
		}
		
		go.transform.position = vCreatePos;
		
        fireBall.MoveDir = vMoveDir;
        fireBall.lookDir = vMoveDir;
		fireBall.dropPos = targetPos;
		
		fireBall.SetOwnerActor(lifeManager);
		fireBall.SetAttackInfo(lifeManager.GetCurrentAttackInfo());
        fireBall.SetFired();
		
		if (this.moveController != null)
			this.moveController.SetProjectileCollider(fireBall.detectCollider, attackTargetInfo);
	}
	
	public override BaseAttackInfo ChooseAttackIndex(Vector3 targetPos, float targetRadius, bool bSameGround)
	{
		LifeManager targetLifeMgr = null;
		if (targetInfo != null)
			targetLifeMgr = targetInfo.GetComponent<LifeManager>();
		
		if (targetLifeMgr != null && targetLifeMgr.GetHP() <= 0.0f)
			return null;
		
		BaseAttackInfo attackInfo = null;
		
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
				
				if (teleportAction == info.attackState)
				{
					if (teleportTarget == null)
						continue;
					
					Vector3 teleportDiff = teleportTarget.position - this.transform.position;
					if (teleportDiff.magnitude < 4.0f)
						continue;
					
					if (teleportDelayTime > 0.0f)
						continue;
				}
				
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
	
	public override void OnEndState()
	{
		if (stateController.currentState == teleportAction)
		{
			if (twinWizard != null)
				twinWizard.TeleportByOther();
			
			Teleport();
		}
	}
	
	public void OnSyncHPValue()
	{
		if (twinWizard != null)
		{
			AttributeValue thisHealth = this.lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
			AttributeValue twinHealth = twinWizard.lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
			
			float syncValue = Mathf.Min(thisHealth.Value, twinHealth.Value);
			
			thisHealth.baseValue = syncValue;
			twinHealth.baseValue = syncValue;
			
			this.lifeManager.attributeManager.UpdateValue(thisHealth);
			twinWizard.lifeManager.attributeManager.UpdateValue(twinHealth);
			
			if (this.lifeManager.isBossRaidMonster == true)
			{
				if (twinWizard.lifeManager.isPhase2 == false && this.lifeManager.isPhase2 == true)
				{
					twinWizard.lifeManager.isPhase2 = true;
					twinWizard.lifeManager.ActivatePhase2();
				}
			}
		}
	}
	
	public override void OnDie(LifeManager attacker)
	{
		base.OnDie(attacker);
		
		if (twinWizard != null)
			twinWizard.DoDie(attacker);
	}
	
	public void DoDie(LifeManager attacker)
	{
		AttributeValue thisHealth = this.lifeManager.attributeManager.GetAttribute(AttributeValue.eAttributeType.Health);
		thisHealth.baseValue = 0.0f;
		this.lifeManager.attributeManager.UpdateValue(thisHealth);
		
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
		}
	}
}
