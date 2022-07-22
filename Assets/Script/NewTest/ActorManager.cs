using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActorManager {
	private static ActorManager mInstance = null;
	
	public Dictionary<ActorInfo.TeamNo, List<ActorInfo>> teamActorList = new Dictionary<ActorInfo.TeamNo, List<ActorInfo>>();
	
	public ActorInfo playerInfo = null;
	
	public static ActorManager Instance
	{
		get
		{
			if (mInstance == null)
				mInstance = new ActorManager();
			
			return mInstance;
		}	
	}
	
	public void RemoveActor(ActorInfo.TeamNo teamNo, ActorInfo info)
	{
		if (teamActorList.ContainsKey(teamNo) == true)
		{
			List<ActorInfo> oldList = teamActorList[teamNo];
			if (oldList != null)
				oldList.Remove(info);
		}
	}
	
	public void AddActor(ActorInfo.TeamNo teamNo, ActorInfo info)
	{
		if (teamActorList.ContainsKey(teamNo) == false)
		{
			List<ActorInfo> newList = new List<ActorInfo>();
			newList.Add(info);
			
			teamActorList.Add(teamNo, newList);
		}
		else
		{
			List<ActorInfo> oldList = teamActorList[teamNo];
			
			if (oldList.Contains(info) == false)
				oldList.Add(info);
		}
		
		if (info != null &&  info.actorType == ActorInfo.ActorType.Player)
			playerInfo = info;
	}
	
	public List<ActorInfo> GetActorList(ActorInfo.TeamNo teamNo)
	{
		List<ActorInfo> actorList = null;
		if (teamActorList.ContainsKey(teamNo) == true)
			actorList = teamActorList[teamNo];
		
		return actorList;
	}
	
	public void SetJumpCollider(ActorInfo actorInfo, Collider myCollider, bool bIgnore)
	{
		List<ActorInfo> actorList = GetActorList(actorInfo.enemyTeam);
		
		if (actorList == null)
			return;
		
		Collider collider2 = null;
		
		BaseMoveController moveControl = null;
		foreach(ActorInfo info in actorList)
		{
			moveControl = info.gameObject.GetComponent<BaseMoveController>();
			
			if (moveControl != null)
				collider2 = moveControl.collider;
			
			if (myCollider != null && collider2 != null && myCollider != collider2)
			{
				//Debug.Log("ActorBody " + myCollider + " vs " + collider2 + " " + bIgnore);
				Physics.IgnoreCollision(myCollider, collider2, bIgnore);
			}
		}
	}
	
	public ActorInfo GetActorInfo(ActorInfo.ActorType type)
	{
		ActorInfo actorInfo = null;
		
		foreach(var temp in teamActorList)
		{
			foreach(ActorInfo info in temp.Value)
			{
				if (info.actorType == type)
				{
					actorInfo = info;
					break;
				}
			}
		}
		
		return actorInfo;
	}
}
