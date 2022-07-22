using UnityEngine;
using System.Collections;

public class StageStartEvent : EventStep {
	public float msgDisplayTime = 2.5f;
	
	public string StartTile = "";
	public string StartMsg = "";
	
	public string titlePrefabPath = "UI/Area/EventMessage";
	public ScrollCamera mainCamera = null;
	
	// Use this for initialization
	public override void Start () {
		base.Start();
		
		mainCamera = GameObject.FindObjectOfType(typeof(ScrollCamera)) as ScrollCamera;
	}
	
	public void Awake()
	{
		//mainCamera = GameObject.FindObjectOfType(typeof(ScrollCamera)) as ScrollCamera;
	}
		
	public void CreateTitle()
	{
		Transform uiRoot = null;
		if (mainCamera != null)
			uiRoot = mainCamera.uiRoot;
		
		EventMessage eventMsg = ResourceManager.CreatePrefab<EventMessage>(titlePrefabPath, mainCamera.uiRoot);
		if (eventMsg != null)
		{
			if (StartTile.Length <= 0 || StartMsg.Length <= 0)
			{
				StartTile = "StartTitle";
				StartMsg = "StartMessage";
			}
			eventMsg.SetMessage(StartTile, StartMsg, msgDisplayTime);
		}
	}
	
	public void OnStageStart()
	{
		/*
		if (Game.instance != null)
		{
			Game.instance.IsPause = false;
			Game.instance.SetStageClearMode(false);
			
			UIBattleControl uiBattle = Game.instance.uiBattleControl;
			if (uiBattle != null)
			{
				if (Game.instance.isTutorialMode == false)
				{
					uiBattle.ShowButtonLimit(TutorialChapter.eChapterInputLimit.None);
				}
						
				uiBattle.animation.Play(uiBattle.initAnimation);
			}
			
			if (Game.instance.isTutorialMode == false)
			{
				Game.instance.IsTutorialMode = false;
		
				if (Game.instance.player != null)
					Game.instance.player.SetTutorialMode(false);
			}
		}
		*/
		
		Debug.Log("OnStageStart......");
	}
	
	public void OnReady()
	{
		/*
		if (Game.instance != null)
			Game.instance.IsPause = true;
		*/
		
		Debug.Log("OnReady......");
	}
}
