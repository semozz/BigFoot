using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MonsterTargetSearch : TargetSearch {
	public float searchMinLegnth = 0.0f;
	public float searchLimitLength = 15.0f;
	public bool bLimitLength = true;
	
	public float sleepMinLength = 0.0f;
	public float sleepLimitLength = 1.2f;
	
	public float rightPlayerTargetRate = 1.0f;
	public float leftPlayerTargetRate = 0.25f;
	
	
	//디벤프 타워의 경우 제일 가까운 녀석만 골라져서 공격이 안되는 경우 발생..
	//그래서 발사체 곡사포?로 쏘는 녀석은 랜덤으로 선택 하도록 변경..
	public bool bRandomSelect = false;
	
	public override void Start()
	{
		base.Start();
	}
	
	public override void Update()
	{
		delayTime -= Time.deltaTime;
		
		if (delayTime <= 0.0f)
		{
			ActorInfo targetActor = null;
			switch(searchType)
			{
			case eSearchType.TowerDefence:
				targetActor = SelectTargetActor();
				break;
			case eSearchType.Normal:
				targetActor = SelectTargetActorNormal();
				break;
			case eSearchType.Catapult:
				targetActor = SelectTowerActor();
				break;
			}
			
			ownerActor.ChangeTarget(targetActor);
			
			if (targetActor != null)
			{
				if (this.actorInfo != null)
				{
					//한번 타겟팅 된 후 거리 제한 없애기???에서 공성차/성문등은 제외...
					//일반 몬스터만...적용..
					switch(this.actorInfo.actorType)
					{
					case ActorInfo.ActorType.BossMonster:
					case ActorInfo.ActorType.Monster:
						bLimitLength = false;
						break;
					}
				}
			}
			
			delayTime = searchDelay;
		}
	}
	
	public ActorInfo SelectTowerActor()
	{
		ActorInfo targetActor = null;
		
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> actorList = null;	
		if (actorManager != null)
			actorList = actorManager.GetActorList(actorInfo.enemyTeam);
		
		if (actorList != null)
		{
			foreach(ActorInfo info in actorList)
			{
				if (info == null)
					continue;
				
				LifeManager life = info.gameObject.GetComponent<LifeManager>();
				if (life == null || life.GetHPRate() <= 0.0f)
					continue;
				
				if (info.actorType != ActorInfo.ActorType.Casttle)
					continue;
				
				targetActor = info;
			}
		}
		
		return targetActor;
	}
	
	public ActorInfo SelectTargetActorNormal()
	{
		ActorInfo targetActor = null;
		
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> actorList = null;	
		if (actorManager != null)
			actorList = actorManager.GetActorList(actorInfo.enemyTeam);
		
		Vector3 myPos = this.transform.position;
		Vector3 targetPos = myPos;
		
		
		List<ActorInfo> leftActorList = new List<ActorInfo>();
		List<ActorInfo> rightActorList = new List<ActorInfo>();
		
		Vector3 vDiff = Vector3.zero;
		if (actorList != null)
		{
			foreach(ActorInfo info in actorList)
			{
				if (info == null)
					continue;
				
				LifeManager life = info.gameObject.GetComponent<LifeManager>();
				if (life == null || life.GetHPRate() <= 0.0f)
					continue;
				
				targetPos = info.transform.position;
				
				vDiff = targetPos - myPos;
				float diff = vDiff.magnitude - (info.colliderRadius + this.actorInfo.colliderRadius);
				Vector2 searchLength = Vector2.zero;
				searchLength = GetSearchLength();
				
				if (bLimitLength == true && 
					(diff > searchLength.y || diff < searchLength.x))
					continue;		
				
				if (vDiff.x < 0.0f)
					leftActorList.Add(info);
				else
					rightActorList.Add(info);
			}
			
			float diffLength = float.MaxValue;
			float tempLength = 0.0f;
			ActorInfo leftActor = GetNearestActor(leftActorList, bRandomSelect);
			ActorInfo rightActor = GetNearestActor(rightActorList, bRandomSelect);
			
			if (leftActor != null)
			{
				targetPos = leftActor.transform.position;
				vDiff = targetPos - myPos;
				
				tempLength = Mathf.Abs(vDiff.x);
				if (diffLength > tempLength)
				{
					diffLength = tempLength;
					targetActor = leftActor;
				}
			}
			
			if (rightActor != null)
			{
				targetPos = rightActor.transform.position;
				vDiff = targetPos - myPos;
				
				tempLength = Mathf.Abs(vDiff.x);
				if (diffLength > tempLength)
				{
					diffLength = tempLength;
					targetActor = rightActor;
				}
			}
		}
		
		return targetActor;
	}
	
	public ActorInfo GetNearestActor(List<ActorInfo> actorList, bool bRandom)
	{
		Vector3 vDiff = Vector3.zero;
		float diffLength = float.MaxValue;
		Vector3 myPos = this.transform.position;
		Vector3 targetPos = myPos;
		
		ActorInfo selectedActor = null;
		float tempLength = 0.0f;
		
		if (bRandom == false)
		{
			foreach(ActorInfo info in actorList)
			{
				targetPos = info.transform.position;
				
				vDiff = targetPos - myPos;
				tempLength = Mathf.Abs(vDiff.x);
				if (tempLength < diffLength)
				{
					selectedActor = info;
					diffLength =tempLength;
				}
			}
		}
		else
		{
			int nCount = actorList.Count;
			int randIndex = nCount > 0 ? Random.Range(0, nCount) : -1;
			
			if (randIndex >= 0)
				selectedActor = actorList[randIndex];
		}
		
		return selectedActor;
	}
	
	public ActorInfo SelectTargetActor()
	{
		ActorInfo targetActor = null;
		
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> actorList = null;	
		if (actorManager != null)
			actorList = actorManager.GetActorList(actorInfo.enemyTeam);
		
		Vector3 playerPos = this.transform.position;
		
		Vector3 myPos = this.transform.position;
		Vector3 targetPos = myPos;
		Vector3 vDiff = Vector3.zero;
		
		List<ActorInfo> leftActorList = new List<ActorInfo>();
		List<ActorInfo> rightActorList = new List<ActorInfo>();
		
		ActorInfo playerActor = null;
		//ActorInfo casttleActor = null;
		if (actorList != null)
		{
			foreach(ActorInfo info in actorList)
			{
				if (info == null)
					continue;
				
				LifeManager life = info.gameObject.GetComponent<LifeManager>();
				if (life == null || life.GetHPRate() <= 0.0f)
					continue;
				
				targetPos = info.transform.position;
				
				vDiff = targetPos - myPos;
				if (vDiff.x < 0.0f)
					leftActorList.Add(info);
				else
					rightActorList.Add(info);
				
				switch(info.actorType)
				{
				case ActorInfo.ActorType.Player:
					playerActor = info;
					break;
				case ActorInfo.ActorType.Casttle:
					//casttleActor = info;
					break;
				}
			}
		}
		
		if (playerActor != null)
			playerPos = playerActor.gameObject.transform.position;
		
		
		Vector3 diff = playerPos - this.transform.position;
		if (diff.x <= 0.0f)
		{
			int randValue = Random.Range(0, 100);
			int rateValue = Mathf.RoundToInt(rightPlayerTargetRate * 100.0f);
			
			if (randValue <= rateValue)
				targetActor = playerActor;
			else
			if (leftActorList.Count > 0)
				targetActor = GetNearestActor(leftActorList, false);
			else
				targetActor = GetNearestActor(rightActorList, false);
		}
		else
		{
			int randValue = Random.Range(0, 100);
			int rateValue = Mathf.RoundToInt(leftPlayerTargetRate * 100.0f);
			
			if (randValue <= rateValue)
			{
				targetActor = playerActor;
			}
			else
			{
				if (Mathf.Abs(diff.x) < ownerActor.limitTargetLength &&
					randValue <= rateValue)
				{
					targetActor = GetNearestActor(rightActorList, false);
				}
				else
				{
					if (leftActorList.Count > 0)
						targetActor = GetNearestActor(leftActorList, false);
					else
						targetActor = GetNearestActor(rightActorList, false);
				}
			}
		}
		
		return targetActor;
	}
	
	public Vector2 GetSearchLength()
	{
		Vector2 vec = new Vector2(searchMinLegnth, searchLimitLength);
		
		if (ownerActor != null && ownerActor.stateController != null)
		{
			if (ownerActor.stateController.currentState == BaseState.eState.Sleep)
			{
				vec.x = sleepMinLength;
				vec.y = sleepLimitLength;
			}
		}
		
		return vec;
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		//Vector3 areaCenter = Vector3.zero;
		//Vector3 targetCenter = Vector3.zero;
		
		Color oricolor = Gizmos.color;
		
		Gizmos.color = Color.red;
		//Vector3 delta = Vector3.zero;
		
		Gizmos.DrawWireSphere(transform.position, searchLimitLength);
		
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, sleepLimitLength);
		
		Gizmos.color = oricolor;
	}
#endif
}
