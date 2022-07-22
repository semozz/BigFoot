using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveStep : MonoBehaviour {
	
	public int waveStep = -1;
	
	public List<MonsterGenerator> waveMonsterList = new List<MonsterGenerator>();
	
	public MonsterKillCounter killCounter = null;
	public EventCondition waveCondition = null;
	
	public List<MonsterGenerator> waveBossMonsterList = new List<MonsterGenerator>();
	public MonsterKillCounter bossKillCounter = null;
	public EventCondition waveBossCondition = null;
	
	public EventDialogTrigger eventDialog = null;
	
	public float msgDelayTime = 4.5f;
	public float waveCompleteDelayTime = 3.0f;
	public float bossModeDelayTime = 2.0f;
	
	private float delayTime = 0.0f;
	
	private bool isComplete = false;
	public bool IsComplete
	{
		get { return isComplete; }	
	}
	
	public enum eWaveStep
	{
		None = -1,
		WaveStart,
		Activate,
		BossModeReady,
		BossMode,
		WaveComplete,
		Deactivate,
	}
	private eWaveStep currentStep = eWaveStep.None;
	
	public static int CompareIndices(WaveStep a, WaveStep b)
	{
		return a.waveStep - b.waveStep;
	}
	
	// Use this for initialization
	void Start () {
		if (killCounter != null)
			killCounter.UnActivate();
		
		foreach(MonsterGenerator mg in waveMonsterList)
		{
			if (mg == null)
				continue;
			
			mg.OnDeactivate();
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentStep)
		{
		case eWaveStep.Activate:
			if (waveCondition != null && waveCondition.IsComplete == true)
			{
				//CreateTitle(MakeWaveStepName(), "BossReady", msgDelayTime);
				delayTime = bossModeDelayTime;
				
				//ClearMonsters();
				
				currentStep = eWaveStep.BossModeReady;
			}
			break;
		case eWaveStep.BossModeReady:
			delayTime -= Time.deltaTime;
			
			if (delayTime <= 0.0f)
			{
				currentStep = eWaveStep.BossMode;
				
				SetBossMode();
			}
			break;
		case eWaveStep.BossMode:
			if (waveBossCondition == null || waveBossCondition.IsComplete == true)
			{
				//CreateTitle(MakeWaveStepName(), "Complete", msgDelayTime);
				delayTime = waveCompleteDelayTime;
								
				currentStep = eWaveStep.WaveComplete;
				
				SurrendMonsters();
				
				if (eventDialog != null)
					eventDialog.OnActivate();
			}
			break;
		case eWaveStep.WaveStart:
			delayTime -= Time.deltaTime;
			
			if (delayTime <= 0.0f)
			{
				StepInit();
				currentStep = eWaveStep.Activate;
			}
			break;
		case eWaveStep.WaveComplete:
			delayTime -= Time.deltaTime;
						
			if (delayTime <= 0.0f)
			{
				currentStep = eWaveStep.Deactivate;
				
				ClearBossMonsters();
				ClearMonsters();
				
				isComplete = true;
			}
			break;
		}
	}
	
	public void Activate()
	{
		//CreateTitle(MakeWaveStepName(), "Start", msgDelayTime);
		delayTime = msgDelayTime;
		
		currentStep = eWaveStep.WaveStart;
		
		isComplete = false;
	}
	
	public void StepInit()
	{
		if (killCounter != null)
			killCounter.OnActivate();
		
		foreach(MonsterGenerator mg in waveMonsterList)
		{
			if (mg == null)
				continue;
			
			mg.OnActivate();
		}
	}
	
	public void SetBossMode()
	{
		if (bossKillCounter != null)
			bossKillCounter.OnActivate();
		
		foreach(MonsterGenerator mg in waveBossMonsterList)
		{
			if (mg == null)
				continue;
			
			mg.OnActivate();
		}
	}
	
	public string MakeWaveStepName()
	{
		string msg = string.Format("Wave {0:##}", this.waveStep + 1);
		return msg;
	}
	
	public void CreateTitle(string title, string msg, float delayTime)
	{
		/*
		GameObject newObject = (GameObject)Instantiate(Resources.Load("UI_FX/Area/Area"));
		EventMessage eventMsg = newObject.GetComponent<EventMessage>();
		*/
		
		EventMessage eventMsg = ResourceManager.CreatePrefab<EventMessage>("UI_FX/Area/Area");
		
		/*
		ScrollCamera scrollCamera = Game.Instance.camera;
		if (scrollCamera != null)
		{
			eventMsg.transform.parent = scrollCamera.transform;
			eventMsg.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);
		}
		*/
		
		if (eventMsg != null)
			eventMsg.SetMessage(title, msg, delayTime);
	}
	
	public void ClearMonsters()
	{
		if (killCounter != null)
			killCounter.UnActivate();
		
		foreach(MonsterGenerator mg in waveMonsterList)
		{
			if (mg != null)
			{
				mg.OnDeactivate();
				mg.ResetMonsters();
			}
		}
		
		MonsterGenerator.isSurrendMode = false;
	}
	
	public void SurrendMonsters()
	{
		if (killCounter != null)
			killCounter.UnActivate();
		
		foreach(MonsterGenerator mg in waveMonsterList)
		{
			if (mg != null)
			{
				mg.SurrendMonsters();
				mg.OnDeactivate();
			}
		}
		
		MonsterGenerator.isSurrendMode = true;
	}
	
	public void ClearBossMonsters()
	{
		if (bossKillCounter != null)
			bossKillCounter.UnActivate();
		
		foreach(MonsterGenerator mg in waveBossMonsterList)
		{
			if (mg != null)
			{
				mg.OnDeactivate();
				mg.ResetMonsters();
			}
		}
	}
}
