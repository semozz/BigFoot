using UnityEngine;
using System.Collections;

public class MonsterKillCounter : EventConditionTrigger {
	public MonsterGenerator monsterGenerator = null;
	
	public ActorInfo.ActorType targetType = ActorInfo.ActorType.Monster;
	
	// Use this for initialization
	void Start () {
		if (monsterGenerator != null)
			monsterGenerator.ActivateTrigger(false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public override void OnActivate()
	{
		if (this.isActivate == true)
			return;
		
		base.OnActivate();
		
		if (monsterGenerator != null)
			monsterGenerator.ActivateTrigger(true);
		
		EventConditionChecker checker = EventConditionChecker.Instance;
		if (checker != null)
			checker.AddMonsterKillCounter(this);
	}
	
	public override void OnComplete()
	{
		this.isActivate = false;
		
		if (monsterGenerator != null)
			monsterGenerator.ActivateTrigger(false);
		
		EventConditionChecker checker = EventConditionChecker.Instance;
		if (checker != null)
			checker.RemoveMonsterKillCounter(this);
	}
	
	public override void UnActivate()
	{
		base.UnActivate();
		
		if (monsterGenerator != null)
			monsterGenerator.ActivateTrigger(false);
	}
	
	public bool IsCounteralbe(ActorInfo.ActorType actorType)
	{
		bool bResult = false;
		
		switch(targetType)
		{
		case ActorInfo.ActorType.Monster:
			if  (actorType == ActorInfo.ActorType.Monster ||
				actorType == ActorInfo.ActorType.BossMonster)
				bResult = true;
			break;
		default:
			bResult = targetType == actorType;
			break;
		}
		
		return bResult;
	}
}
