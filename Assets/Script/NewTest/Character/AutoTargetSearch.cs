using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoTargetSearch : MonoBehaviour {
	
	private PlayerController player = null;
	
	public float searchDelay = 0.5f;
	private float delayTime = 0.0f;
	
	public float minTargetDistance = 3.0f;
	
	private ActorInfo.TeamNo enemyTeam = ActorInfo.TeamNo.None;
	StageManager.eStageType stageType = StageManager.eStageType.ST_FIELD;
	
	void Awake()
	{
		enemyTeam = GetComponent<ActorInfo>().enemyTeam;
		player = GetComponent<PlayerController>();
	}
	
	void Start()
	{
		delayTime = searchDelay;
		
		StageManager stageManager = GameObject.FindObjectOfType(typeof(StageManager)) as StageManager;
		if (stageManager != null)
		{
			stageType = stageManager.StageType;
		}
	}
	
	public void Update()
	{
		if (player.isAutoMode == false)
			return;
		
		delayTime -= Time.deltaTime;
		
		if (!isJumpNow() &&
			(delayTime <= 0.0f || player.targetInfo == null ||
			checkTargetInfoState()))
		{
			ActorInfo targetActor = SelectTargetActor();
			
			player.ChangeTarget(targetActor);
			
			delayTime = searchDelay;
		}
	}
	
	private bool isJumpNow()
	{
		switch( player.stateController.currentState )
		{
			case BaseState.eState.JumpAttack:
			case BaseState.eState.JumpFall:
			case BaseState.eState.Jumpland:
			case BaseState.eState.JumpStart:
				return true;
		}
		return false;
	}
	
	private bool checkTargetInfoState()
	{
		if(player.targetInfo != null)
		{		
			if(player.stateController.currentState == BaseState.eState.Damage 
			|| player.stateController.currentState == BaseState.eState.Stun)
			{
				Vector3 vDiff = player.targetInfo.transform.position - this.transform.position;
				if(Mathf.Sqrt(vDiff.x*vDiff.x + vDiff.y*vDiff.y) > minTargetDistance)
				{
					player.ChangeTarget(null);
					return true;
				}
			}
			
			if(player.targetInfo.GetLifeManager().GetHP() <= 0.0f)
				return true;
		}
		return false;
	}
	
	public ActorInfo SelectTargetActor()
	{
		ActorInfo targetActor = null;
		
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> actorList = null;	
		if (actorManager != null)
			actorList = actorManager.GetActorList(enemyTeam);		
		
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
				
				availableList.Add(info);
			}
			
			targetActor = GetNearestActor(availableList);
		}
		
		return targetActor;
	}
	
	public ActorInfo GetNearestActor(List<ActorInfo> actorList)
	{
		Vector3 vDiff = Vector3.zero;
		float diffWeight = float.MaxValue;
		Vector3 myPos = this.transform.position;
		Vector3 targetPos = myPos;
		
		ActorInfo selectedActor = null;
		float tempWeight = 0.0f;
		
		foreach(ActorInfo info in actorList)
		{
			targetPos = info.transform.position;
			
			vDiff = targetPos - myPos;
			tempWeight = Mathf.Abs(vDiff.x)+Mathf.Abs(vDiff.y)*2.4f + getMonsterWeight(info.monsterType);
			if (tempWeight < diffWeight)
			{
				selectedActor = info;
				diffWeight = tempWeight;
			}
			
		}
		
		return selectedActor;
	}
	
	private float getMonsterWeight(ActorInfo.MonsterType type)
	{
		float weight = 1.0f;
		switch( type )
		{
			case ActorInfo.MonsterType.Archer:		weight = 0.6f; break;
			case ActorInfo.MonsterType.Assassin:	weight = 0.9f;	break;
			case ActorInfo.MonsterType.Berserk:		weight = 1.8f;	break;
			case ActorInfo.MonsterType.Butcher:		weight = 10.0f;	break;
			case ActorInfo.MonsterType.Defender:	weight = 1.5f;	break;
			case ActorInfo.MonsterType.Devil:		weight = 0.3f;	break;
			case ActorInfo.MonsterType.Lancer:		weight = 0.9f;	break;
			case ActorInfo.MonsterType.Paladin:		weight = 1.2f;	break;
			case ActorInfo.MonsterType.Priest:		weight = 0.0f;	break;
			case ActorInfo.MonsterType.Soldier:		weight = 1.2f;	break;
			case ActorInfo.MonsterType.Wizard:		weight = 0.3f;	break;
		}
		return weight;
	}
}

