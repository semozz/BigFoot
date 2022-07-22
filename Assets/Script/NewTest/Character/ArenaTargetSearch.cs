using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaTargetSearch : MonoBehaviour {
	public float searchMinLegnth = 0.0f;
	public float searchLimitLength = 15.0f;
	public bool bLimitLength = true;
	
	public ArenaPlayerInput arenaInput = null;
	public PlayerController player = null;
	
	public float searchDelay = 1.5f;
	public float delayTime = 0.0f;
	
	public ActorInfo myActorInfo = null;
	StageManager.eStageType stageType = StageManager.eStageType.ST_FIELD;
	
	void Start()
	{
		myActorInfo = gameObject.GetComponent<ActorInfo>();
		
		delayTime = searchDelay;
		
		StageManager stageManager = GameObject.FindObjectOfType(typeof(StageManager)) as StageManager;
		if (stageManager != null)
		{
			stageType = stageManager.StageType;
		}
	}
	
	public void Update()
	{
		if (stageType != StageManager.eStageType.ST_ARENA)
			return;
		
		delayTime -= Time.deltaTime;
		
		if (delayTime <= 0.0f)
		{
			ActorInfo targetActor = SelectTargetActor();
			
			player.ChangeTarget(targetActor);
			
			if (targetActor != null)
				bLimitLength = false;
			
			delayTime = searchDelay;
		}
	}
	
	public ActorInfo SelectTargetActor()
	{
		ActorInfo targetActor = null;
		
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> actorList = null;	
		if (actorManager != null)
			actorList = actorManager.GetActorList(myActorInfo.enemyTeam);
		
		Vector3 myPos = this.transform.position;
		Vector3 targetPos = myPos;
		
		
		List<ActorInfo> availableList = new List<ActorInfo>();
		
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
				float diffX = Mathf.Max(0.0f, (Mathf.Abs(vDiff.x) - (info.colliderRadius + this.myActorInfo.colliderRadius)));
				if (bLimitLength == true &&
					diffX > searchLimitLength || diffX < searchMinLegnth)
					continue;
				
				availableList.Add(info);
			}
			
			targetActor = GetNearestActor(availableList);
		}
		
		return targetActor;
	}
	
	public ActorInfo GetNearestActor(List<ActorInfo> actorList)
	{
		Vector3 vDiff = Vector3.zero;
		float diffLength = float.MaxValue;
		Vector3 myPos = this.transform.position;
		Vector3 targetPos = myPos;
		
		ActorInfo selectedActor = null;
		float tempLength = 0.0f;
		
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
		
		return selectedActor;
	}
}
