using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterPriest : BaseMonster {
	public float curseBuffDurationTime = 5.0f;
	public float manaShieldBuffDurationTime = 4.0f;
	
	public BaseState.eState healAttack = BaseState.eState.Attack2;
	public BaseState.eState manaShieldAttack = BaseState.eState.Attack3;
	
	public string fxHealTarget = "FX_Heal_target";
	public float fxHealTime = 1.0f;
	
	private LifeManager healTarget = null;
	private LifeManager manaShieldTarget = null;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
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
				
				if (healAttack == info.attackState)
				{
					if (CheckHealAttack(randValue, info) == false)
						continue;
				}
				else if (manaShieldAttack == info.attackState)
				{
					if (CheckManaShieldAttack(randValue, info) == false)
						continue;
				}
				else if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
				{
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
	
	private void SetHealTarget(LifeManager newActor)
	{
		healTarget = newActor;
	}
	
	public virtual bool CheckHealAttack(int randValue, BaseAttackInfo attackInfo)
	{
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> teamList = null;
		if (actorManager != null)
			teamList = actorManager.GetActorList(myInfo.myTeam);
		
		LifeManager newTarget = null;
		
		if (teamList != null)
		{
			LifeManager actor = null;
			float hpRate = float.MaxValue;
			foreach(ActorInfo info in teamList)
			{
				actor = info.gameObject.GetComponent<LifeManager>();
				if (actor.attributeManager == null)
					continue;
				
				float healthRate = actor.GetHPRate();
				
				if (healthRate >= 1.0f || healthRate <= 0.0f)
					continue;
				
				if (healthRate < hpRate)
				{
					hpRate = healthRate;
					newTarget = actor;
				}
			}
			
			if (newTarget != null)
			{
				Vector3 vDiff = newTarget.transform.position - this.transform.position;
				float diffX = Mathf.Max(0.0f, Mathf.Abs(vDiff.x) - (myInfo.colliderRadius + newTarget.myActorInfo.colliderRadius));
				float diffY = vDiff.y;
				
				if (Mathf.Abs(diffY) < 0.1f)
					diffY = 0.0f;
				
				WayPointManager targetWayMgr = GetWayPointManager(newTarget.myActorInfo);
				WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
				
				bool bSameGround = targetWayMgr == curWayMgr;
				
				if (attackInfo.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
					newTarget = null;
			}
		}
		
		SetHealTarget(newTarget);
		
		if (newTarget == null)
			return false;
		else
			return true;
	}
	
	private void SetManaShieldTarget(LifeManager newActor)
	{
		manaShieldTarget = newActor;
	}
	
	public virtual bool CheckManaShieldAttack(int randValue, BaseAttackInfo attackInfo)
	{
		Vector3 playerPos = this.transform.position;
		
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> teamList = null;
		if (actorManager != null)
		{
			teamList = actorManager.GetActorList(myInfo.myTeam);
			
			if (actorManager.playerInfo != null)
				playerPos = actorManager.playerInfo.transform.position;
		}
		
		LifeManager wounderActor = null;
		LifeManager nearActor = null;
		
		if (teamList != null)
		{
			LifeManager actor = null;
			float hpRate = float.MaxValue;
			float distant = float.MaxValue;
			
			foreach(ActorInfo info in teamList)
			{
				actor = info.gameObject.GetComponent<LifeManager>();
				if (actor == null || actor.attributeManager == null)
					continue;
				
				float tempRate = actor.GetHPRate();
				if (tempRate >= 1.0f || tempRate <= 0.0f)
					continue;
				
				if (tempRate < 1.0f && tempRate < hpRate)
				{
					hpRate = tempRate;
					wounderActor = actor;
				}
				
				Vector3 vDiff = playerPos - actor.transform.position;
				float tempDis = Mathf.Abs(vDiff.x);
				if (tempDis < distant)
				{
					distant = tempDis;
					nearActor = actor;
				}
			}
		}
		
		LifeManager newTarget = wounderActor;
		if (wounderActor == null)
			newTarget = nearActor;
		
		if (newTarget != null)
		{
			Vector3 vDiff = newTarget.transform.position - this.transform.position;
			float diffX = Mathf.Max(0.0f, Mathf.Abs(vDiff.x) - (myInfo.colliderRadius + newTarget.myActorInfo.colliderRadius));
			float diffY = vDiff.y;
			
			if (Mathf.Abs(diffY) < 0.1f)
				diffY = 0.0f;
			
			WayPointManager targetWayMgr = GetWayPointManager(newTarget.myActorInfo);
			WayPointManager curWayMgr = GetWayPointManager(this.moveController.groundCollider);
			
			bool bSameGround = targetWayMgr == curWayMgr;
			
			if (attackInfo.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
				newTarget = null;
		}
		
		SetManaShieldTarget(newTarget);
		
		if (newTarget == null)
			return false;
		else
			return true;
	}
	
	public override void FireProjectile()
	{
		LifeManager target = null;
		
		switch (stateController.currentState)
        {
        case BaseState.eState.Attack1:
			//Curze Buff..
			if (targetInfo != null)
			{
				if (attackTargetInfo.myTeam != myInfo.myTeam)
				{
					target = attackTargetInfo.gameObject.GetComponent<LifeManager>();
					
					if (target != null && target.buffManager != null)
					{
						int index = target.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_CURSE, this.lifeManager);
						if (index != -1)
							target.buffManager.RemoveBuff(index);
						
						float curseValue = this.lifeManager.GetAbilityPower();
						Debug.Log("Curser : " + curseValue + " time : " + curseBuffDurationTime);
						target.buffManager.AddBuff(GameDef.eBuffType.BT_CURSE, curseValue, curseBuffDurationTime, this.lifeManager, 1);
					}
				}
			}
			break;
		case BaseState.eState.Attack2:
			//Heal... myTeam
			if (healTarget != null)
			{
				float hpRate = healTarget.GetHPRate();
				if (hpRate <= 0.0f || hpRate >= 1.0f)
				{
					Debug.LogWarning("Heal cancel.....!!!");
					stateController.ChangeState(BaseState.eState.Stand);
					return;
				}
				
				target = healTarget;
				
				float healValue = this.lifeManager.GetAbilityPower();
				Debug.Log("Heal : " + healValue);
				target.AddFXDelayInfo(fxHealTarget, fxHealTime);
				target.IncHP(healValue, false, GameDef.eBuffType.BT_REGENHP);
			}
			else
			{
				stateController.ChangeState(BaseState.eState.Stand);
			}
            break;
        case BaseState.eState.Attack3:
			if (manaShieldTarget != null)
			{
				target = manaShieldTarget;
				
				int index = target.buffManager.GetAppliedBuffIndex(GameDef.eBuffType.BT_MANASHIELD, this.lifeManager);
				if (index != -1)
					target.buffManager.RemoveBuff(index);
				
				float manaShieldValue = this.lifeManager.GetAbilityPower();
				Debug.Log("ManaShield : " + manaShieldValue + " time : " + manaShieldBuffDurationTime);
				target.buffManager.AddBuff(GameDef.eBuffType.BT_MANASHIELD, manaShieldValue, manaShieldBuffDurationTime, lifeManager, 1);
			}
			break;
        }
		
		if (target != null && target != this.lifeManager)
		{
			Vector3 vDiff = target.transform.position - this.transform.position;
			Vector3 vMoveDir = moveController.moveDir;
			if (vDiff.x < 0.0f)
				vMoveDir = Vector3.left;
			else
				vMoveDir = Vector3.right;
			
			this.lifeManager.moveController.ChangeMoveDir(vMoveDir);
		}
	}
}
