using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetSearch : MonoBehaviour {
	public enum eSearchType
	{
		TowerDefence,
		Normal,
		Catapult,
	}
	public eSearchType searchType = eSearchType.Normal;
	
	public BaseMonster ownerActor = null;
	
	public ActorInfo actorInfo = null;
	
	public float searchDelay = 1.5f;
	public float delayTime = 0.0f;
	
	
	public float searchWeight = 0.0f; // -1.0f -> Left , 1.0f -> Right, 0.0f -> Both..
	
	
	void Awake()
	{
		actorInfo = GetComponent<ActorInfo>();
		ownerActor = gameObject.GetComponent<BaseMonster>();
		
		CheckTargetType();
	}
	
	public void CheckTargetType()
	{
		StageManager stageManager = null;
		GameObject obj = GameObject.Find("StageManager");
		if (obj != null)
			stageManager = obj.GetComponent<StageManager>();
		
		if (stageManager != null)
		{
			ActorInfo.ActorType actorType = actorInfo != null ? actorInfo.actorType : ActorInfo.ActorType.Monster;
			switch(actorType)
			{
			case ActorInfo.ActorType.Catapult:
				this.searchType = eSearchType.Catapult;
				break;
			case ActorInfo.ActorType.Monster:
			case ActorInfo.ActorType.BossMonster:
				if (stageManager.StageType == StageManager.eStageType.ST_WAVE)
					this.searchType = eSearchType.TowerDefence;
				else
					this.searchType = eSearchType.Normal;
				break;
			}
		}
	}
	
	// Use this for initialization
	public virtual void Start () {
		//delayTime = searchDelay;
		
		searchWeight = Mathf.Clamp(searchWeight, -1.0f, 1.0f);
	}
	
	// Update is called once per frame
	public virtual void Update () {
		delayTime -= Time.deltaTime;
		
		if (delayTime <= 0.0f)
		{
			DoSearch();
			
			delayTime = searchDelay;
		}
	}
	
	public void ResetTime()
	{
		delayTime = searchDelay;
	}
	
	public int SelectSearchDir()
	{
		int dir = 0;
		int randValue = Random.Range(0, 100);
		
		int leftRatio = 0;
		int rightRatio = 0;
		
		if (searchWeight < 0.0f)
		{
			leftRatio = Mathf.RoundToInt(-searchWeight * 100.0f);
			rightRatio = 100 - leftRatio;
		}
		else if (searchWeight > 0.0f)
		{
			rightRatio = Mathf.RoundToInt(searchWeight * 100.0f);
			leftRatio = 100 - rightRatio;
		}
		else
		{
			leftRatio = rightRatio = 0;
		}
		
		if (leftRatio == 0 && rightRatio == 0)
			dir = 0;
		else if (0 <= randValue && randValue < leftRatio)
			dir = -1;
		else
			dir = 1;
		
		return dir;
	}
	
	public void GetTargetActorList(int searchDir, List<ActorInfo> searchList)
	{
		ActorManager actorManager = ActorManager.Instance;
		List<ActorInfo> actorList = null;	
		if (actorManager != null)
			actorList = actorManager.GetActorList(actorInfo.enemyTeam);
		
		Vector3 myPos = this.transform.position;
		
		if (actorList != null)
		{
			Vector3 tempPos = Vector3.zero;
			Vector3 diff = Vector3.zero;
			
			foreach(ActorInfo info in actorList)
			{
				tempPos = info.transform.position;
				
				diff = tempPos - myPos;
				
				LifeManager life = info.gameObject.GetComponent<LifeManager>();
				if (life == null || life.GetHPRate() <= 0.0f)
					continue;
				
				switch(searchDir)
				{
				case -1:
					if (diff.x <= 0.0f)
						searchList.Add(info);
					break;
				case 1:
					if (diff.x >= 0.0f)
						searchList.Add(info);
					break;
				default:
					searchList.Add(info);
					break;
				}
			}
		}
	}
	
	public void DoSearch()
	{
		if (actorInfo != null)
		{
			int searchDir = SelectSearchDir();
		
			//Debug.Log("DoSearch.. dir : " + searchDir + " time : " + Time.time);
			
			List<ActorInfo> searchList = new List<ActorInfo>();
			GetTargetActorList(searchDir, searchList);
			
			Vector3 myPos = this.transform.position;
			
			ActorInfo targetActor = null;
			float diff = float.MaxValue;
			
			foreach(ActorInfo info in searchList)
			{
				if (actorInfo == info)
					continue;
				
				Vector3 tempPos = info.transform.position;
				float tempDiff = Mathf.Abs(tempPos.x - myPos.x);
				
				if (tempDiff < diff)
				{
					targetActor = info;
					diff = tempDiff;
				}
			}
			
			if (ownerActor != null)
				ownerActor.ChangeTarget(targetActor);
		}
	}
	
	public void DoChangeTarget(ActorInfo target)
	{
		if (ownerActor != null)
			ownerActor.ChangeTarget(target);
	}
}
