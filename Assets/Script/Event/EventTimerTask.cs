using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventTimerTask : EventTask {
	public float lifeTime = 8.0f;
	
	public string titlePrefabPath = "UI/Area/EndMessage";
	public ScrollCamera mainCamera = null;
	
	public bool isStageEnd = true;
	
	void Start()
	{
		mainCamera = GameObject.FindObjectOfType(typeof(ScrollCamera)) as ScrollCamera;
	}
	
	public override void Update()
	{
		if (isActivate == false)
			return;
		if (lifeTime <= 0.0f)
		{
			isComplete = true;
		}
		
		lifeTime -= Time.deltaTime;
	}
	
	EventMessage eventMsg = null;
	public void CreateMessage()
	{
		if (eventMsg != null)
		{
			DestroyObject(eventMsg.gameObject, 0.2f);
			eventMsg = null;
		}
		
		Transform uiRoot = null;
		if (mainCamera != null)
			uiRoot = mainCamera.uiRoot;
		
		eventMsg = ResourceManager.CreatePrefab<EventMessage>(titlePrefabPath, uiRoot);
	}
	
	public override void DoStart()
	{
		base.DoStart();
		
		if (isStageEnd == true)
		{
			if (Game.Instance != null)
				Game.Instance.SetPlayerSuperArmorMode(Game.Instance.player);
			
			MonsterGenerator.isSurrendMode = true;
			
			ActorManager actorManager = ActorManager.Instance;
			if (actorManager != null)
			{
				List<ActorInfo> monsterList = actorManager.GetActorList(actorManager.playerInfo.enemyTeam);
				if (monsterList != null)
				{
					foreach(ActorInfo info in monsterList)
					{
						BaseMonster monster = info.gameObject.GetComponent<BaseMonster>();
						if (monster != null && monster.lifeManager.GetHPRate() > 0.0f)
						{
							monster.SetSurrend();
						}
					}
				}
			}
			
			StageManager stageManager = Game.Instance.stageManager;
			if (stageManager != null)
				stageManager.StopBGM();
		}
		
		CreateMessage();
	}
	
	public override void DoEnd()
	{
		base.DoEnd();
		
		if (eventMsg != null)
		{
			DestroyImmediate(eventMsg.gameObject);
			eventMsg = null;
		}
	}
}
