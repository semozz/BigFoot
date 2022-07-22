using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterPaladin : BaseMonster {
	public BaseState.eState healAttack = BaseState.eState.Attack2;
	public LifeManager healTarget = null;
	
	public string fxHealTarget = "FX_Heal_target";
	public float fxHealTime = 1.0f;
	
	public override BaseAttackInfo ChooseAttackIndex(Vector3 targetPos, float targetRadius, bool bSameGround)
	{
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
				
				if (healAttack == info.attackState)
				{
					if (CanAttackState(info) == false || CheckHealAttack(randValue, info) == false)
						continue;
				}
				else if (info.attackState == BaseState.eState.Dashattack)
				{
					if (info.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
						continue;
				}
				else 
				{
					if (CanAttackState(info) == false ||
						info.IsAvailableAttack(randValue, diffX, diffY, bSameGround) == false)
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
				
				float tempRate = actor.GetHPRate();
				
				if (tempRate <= 0.0f || tempRate >= 1.0f)
					continue;
				
				if (tempRate < hpRate)
				{
					hpRate = tempRate;
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
		
		healTarget = newTarget;
		
		if (newTarget == null)
			return false;
		else
			return true;
	}
	
	public override bool CheckBlock(LifeManager attacker)
    {
		if (stateController.currentState == healAttack)
			return false;
		
		return base.CheckBlock(attacker);
    }
	
	public override void FireProjectile()
	{
		//LifeManager target = null;
		
		if (healAttack == stateController.currentState)
        {
			//Heal... myTeam
			if (healTarget != null)
			{
				healTarget.AddFXDelayInfo(fxHealTarget, fxHealTime);
				healTarget.IncHP(lifeManager.GetAttackDamage(), false, GameDef.eBuffType.BT_REGENHP);
				
				if (healTarget != this.lifeManager && this.lifeManager.moveController != null)
				{
					Vector3 vDiff = healTarget.transform.position - this.transform.position;
					Vector3 vMoveDir = moveController.moveDir;
					if (vDiff.x < 0.0f)
						vMoveDir = Vector3.left;
					else
						vMoveDir = Vector3.right;
					
					this.lifeManager.moveController.ChangeMoveDir(vMoveDir);
				}
			}
        }
	}
}
