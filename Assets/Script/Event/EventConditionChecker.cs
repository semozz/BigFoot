using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventConditionChecker
{
	private static EventConditionChecker sInstance = null;

    public static EventConditionChecker Instance
    {
        get
        {
            if (sInstance == null)
                sInstance = new EventConditionChecker();

			return sInstance;
        }
    }
	
	private List<MonsterKillCounter> monsterKillCounterList = new List<MonsterKillCounter>();
	private List<MonsterHitCounter> monsterHitCounterList = new List<MonsterHitCounter>();
	
	private List<RescureCounter> rescureCounterList = new List<RescureCounter>();
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void InitList()
	{
		monsterKillCounterList.Clear();
		monsterHitCounterList.Clear();
		
		rescureCounterList.Clear();
	}
	
	public void AddMonsterKillCounter(MonsterKillCounter counter)
	{
		if (monsterKillCounterList.Contains(counter) == false)
			monsterKillCounterList.Add(counter);
	}
	
	public void RemoveMonsterKillCounter(MonsterKillCounter counter)
	{
		int nCount = monsterKillCounterList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			if (counter != null && monsterKillCounterList[index] == counter)
			{
				monsterKillCounterList.RemoveAt(index);
				break;
			}
		}
	}
	
	public void ApplyMonsterKillCount(ActorInfo.ActorType actorType)
	{
		foreach(MonsterKillCounter counter in monsterKillCounterList)
		{
			if (counter != null &&
			    counter.IsActivate == true && 
			    counter.IsCounteralbe(actorType) == true)
			{
				counter.OnChangeValue(1);
			}	
		}
	}	
	
	public void AddMonsterHitCounter(MonsterHitCounter counter)
	{
		if (monsterHitCounterList.Contains(counter) == false)
			monsterHitCounterList.Add(counter);
	}
	
	public void RemoveMonsterHitCounter(MonsterHitCounter counter)
	{
		int nCount = monsterHitCounterList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			if (counter != null && monsterHitCounterList[index] == counter)
			{
				monsterHitCounterList.RemoveAt(index);
				break;
			}
		}
	}
	
	public void ApplyHitCount(LifeManager attacker, LifeManager hitted, StateInfo.eAttackCategory category)
	{
		foreach (MonsterHitCounter counter in monsterHitCounterList)
		{
			if (counter != null &&
			    counter.IsActivate == true && 
			    counter.IsCounterable(category) == true)
			{
				//Debug.Log("Attack Count >>>>>>>>>>>>>>>>");
				counter.OnChangeValue(1);
			}
		}
	}
	
	public void AddRescureCounter(RescureCounter counter)
	{
		if (rescureCounterList.Contains(counter) == false)
			rescureCounterList.Add(counter);
	}
	
	public void RemoveRescureCounter(RescureCounter counter)
	{
		int nCount = rescureCounterList.Count;
		for (int index = 0; index < nCount; ++index)
		{
			if (counter != null && rescureCounterList[index] == counter)
			{
				rescureCounterList.RemoveAt(index);
				break;
			}
		}
	}
	
	public void ApplyRescureCount()
	{
		foreach(RescureCounter counter in rescureCounterList)
		{
			if (counter != null &&
			    counter.IsActivate == true)
			{
				counter.OnChangeValue(1);
			}	
		}
	}	
}
