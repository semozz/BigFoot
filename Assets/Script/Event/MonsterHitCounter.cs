using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterHitCounter : EventConditionTrigger {
	public List<StateInfo.eAttackCategory> acceptableList = new List<StateInfo.eAttackCategory>();
	public MonsterGenerator monsterGenerator = null;
	
	//public BaseMission mission = null;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public bool IsCounterable(StateInfo.eAttackCategory hitAttackCategory)
	{
		foreach(StateInfo.eAttackCategory category in acceptableList)
		{
			if (category == StateInfo.eAttackCategory.None || category == hitAttackCategory)
				return true;
		}
		
		return false;
	}
	
	public override void OnActivate()
	{
		base.OnActivate();
		
		if (monsterGenerator != null)
			monsterGenerator.ActivateTrigger(true);
		
		EventConditionChecker checker = EventConditionChecker.Instance;
		if (checker != null)
			checker.AddMonsterHitCounter(this);
		
		//Debug.Log(gameObject.name + "OnActivate.....");
	}
	
	public override void OnComplete()
	{
		this.isActivate = false;
		
		if (monsterGenerator != null)
			monsterGenerator.ActivateTrigger(false);
	}
	
	public override void UnActivate()
	{
		base.UnActivate();
		
		if (monsterGenerator != null)
			monsterGenerator.ActivateTrigger(false);
		
		EventConditionChecker checker = EventConditionChecker.Instance;
		if (checker != null)
			checker.RemoveMonsterHitCounter(this);
		
		//Debug.Log(gameObject.name + "UnActivate.....");
	}
	
	public override void OnChangeValue(int addValue)
	{
		base.OnChangeValue(addValue);
		
		/*
		if (mission != null)
		{
			if (isActivate == false)
				return;
			
			mission.AddCondtionValue(addValue);
		}
		else
		{
			base.OnChangeValue(addValue);
		}
		*/
	}
}
