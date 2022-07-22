using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {
	public StageEndEvent stageEndEvent = null;
	
	public List<MercenaryGenerator> waveNPCList = new List<MercenaryGenerator>();
	
	public List<WaveStep> waveList = new List<WaveStep>();
	private WaveStep currentWave = null;
	private int currentWaveStep = -1;
	public int CurWaveStep
	{
		get { return currentWaveStep; }
	}
	
	public int testStartWave = -1;
	
	public static int continueWaveStep = -1;
	public static int continueWaveTime = 0;
	
	public Transform catapultStartPos = null;
	public string catapultPrefabPath = "NewAsset/Wave/Catapult";
	public Catapult catapult = null;
	public float catapultRegenTime = 180.0f;
	public float catapultDelayTime = 0.0f;
	
	public string waveModeUIPrefabPath = "UI/WaveMode/WaveMode";
	public enum eWaveState
	{
		None = -1,
		Activate,
		Complete,
		Deactivate,
	}
	private eWaveState currentStep = eWaveState.None;
	
	private float currentWaveTime = 0.0f;
	public float WaveTime
	{
		get { return currentWaveTime; }	
	}
	
	private float clearWaveTime = float.MaxValue;
	public float WaveClearTime
	{
		get { return clearWaveTime; }	
	}
	
	// Use this for initialization
	void Start () {
		GameUI.Instance.waveManager = this;
		
		NPCGenActivate(false);
		
		UIRootPanel uiRootPanel = GameUI.Instance.uiRootPanel;
		if (uiRootPanel != null)
		{
			Transform root = uiRootPanel.gameObject.transform;
			
			GameObject prefab = ResourceManager.LoadPrefab(waveModeUIPrefabPath);
			if (prefab != null)
			{
				GameObject waveModeUI =  (GameObject)Instantiate(prefab);
				if (waveModeUI != null)
				{
					waveModeUI.transform.parent = root;
					
					waveModeUI.transform.localPosition = Vector3.zero;
					waveModeUI.transform.localScale = Vector3.one;
					waveModeUI.transform.localRotation = Quaternion.identity;
				}
			}
		}
		
		//공성차 생성에서 버벅인다고.. 일단 로딩만 해 놓도록...
		catapultPrefab = ResourceManager.LoadPrefab(catapultPrefabPath);
		
		stageEndEvent = GameObject.FindObjectOfType(typeof(StageEndEvent)) as StageEndEvent;
		if (stageEndEvent != null)
		{
			stageEndEvent.onStageFailed = new StageEndEvent.OnEvent(OnStageFailed);
		}
		
		catapultDelayTime = catapultRegenTime;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.waveManager = null;	
	}
	
	void Awake()
	{
		waveList.Clear();

		WaveStep obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(WaveStep), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			obj = (WaveStep)objs[i];
			
			waveList.Add(obj);
		}
		
		waveList.Sort(WaveStep.CompareIndices);
	}
	
	// Update is called once per frame
	void Update () {
		switch(currentStep)
		{
		case eWaveState.Activate:
			if (currentWave != null && currentWave.IsComplete == true)
			{
				if (currentWaveStep >= 0)
				{
					clearWaveTime = currentWaveTime;
				}
				
				NextStep();
				
				if (currentWaveStep >= waveList.Count)
				{
					currentStep = eWaveState.Complete;
					currentWaveStep = waveList.Count - 1;
				}
			}
			
			currentWaveTime += Time.deltaTime;
			
			UpdateRegenCatapult();
			break;
		case eWaveState.Complete:
			if (stageEndEvent != null)
			{
				NPCGenActivate(false);
				
				//stageEndEvent.OnActivate();
				//stageEndEvent.OnEventAreaEnter();
				
				OnWaveEnd();
			}
			
			currentStep = eWaveState.None;
			break;
		}
	}
	
	public void UpdateRegenCatapult()
	{
		if (catapult != null)
			return;
		
		if (catapultDelayTime <= 0.0f && catapult == null)
		{
			catapultDelayTime = catapultRegenTime;
			RegenCatapult(catapultPrefabPath);
		}
		
		catapultDelayTime -= Time.deltaTime;
	}
	
	public string warningCatapultPrefab = "UI/WaveMode/Warning_Catapult";
	public float warningLifeTime = 3.0f;
	public void WarningCatapult()
	{
		UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
		
		GameObject warning = ResourceManager.CreatePrefab(warningCatapultPrefab, uiRoot.transform, Vector3.zero);
		if (warning != null)
		{
			if (warning.audio != null)
				warning.audio.mute = !GameOption.effectToggle;
			
			DestroyObject(warning, warningLifeTime);
		}
	}
	
	private GameObject catapultPrefab = null;
	public void RegenCatapult(string prefabPath)
	{
		if (catapultPrefab != null)
		{
			GameObject catapultObj = (GameObject)Instantiate(catapultPrefab);
			
			RaycastHit hitInfo;
			var layerMask = 1 << LayerMask.NameToLayer("Ground");
			Vector3 vStart = this.catapultStartPos.position + (Vector3.up * 2.0f);
			
			Vector3 createPos = vStart;
			if (Physics.Raycast(vStart, Vector3.down, out hitInfo, float.MaxValue, layerMask) == true)
			{
				createPos = hitInfo.point;
			}
			
			if (catapultObj != null)
			{
				catapultObj.transform.position = createPos;
				catapult = catapultObj.GetComponent<Catapult>();
				
				catapult.waveManager = this;
				
				WarningCatapult();
			}
		}
	}
	
	public void Activate()
	{
		NPCGenActivate(true);
		
		if (continueWaveStep != -1)
			testStartWave = continueWaveStep;
		
		NextStep();
		currentStep = eWaveState.Activate;
		
		if (continueWaveTime != 0)
			currentWaveTime = continueWaveTime;
		else
			currentWaveTime = 0.0f;
	}
	
	public void NextStep()
	{
		int nCount = waveList.Count;
		if (nCount <= 0)
			return;
		
		if (testStartWave != -1)
		{
			currentWaveStep = testStartWave - 1;
			testStartWave = -1;
		}
		
		int index = currentWaveStep + 1;
		currentWaveStep = index;
		
		if (index >= 0 && index < nCount)
		{
			currentWave = waveList[index];
			if (currentWave == null)
				return;
			
			HideBossHP();
			currentWave.Activate();
			
			SetNPCGenWaveStep(currentWaveStep);
		}
	}
	
	public void HideBossHP()
	{
		UIMyStatusInfo myStatusInfo = GameUI.Instance.myStatusInfo;
		if (myStatusInfo != null)
			myStatusInfo.Invoke("DisableBossHP", 0.2f);
	}
	
	private void NPCGenActivate(bool bActivate)
	{
		foreach(MercenaryGenerator mercenaryGen in waveNPCList)
		{
			if (mercenaryGen == null)
				continue;
			
			if (bActivate == true)
				mercenaryGen.OnActivate();
			else
				mercenaryGen.OnDeactivate();
		}
	}
	
	private void SetNPCGenWaveStep(int step)
	{
		foreach(MercenaryGenerator mercenaryGen in waveNPCList)
		{
			if (mercenaryGen == null)
				continue;
			
			mercenaryGen.ResetCurrentCount();
			mercenaryGen.waveStep = step;
		}
	}
	
	public int GetWaveMaxCount()
	{
		int nCount = waveList.Count;
		return nCount;
	}
	
	public float delayFailCallTime = 2.0f;
	public bool bStageFailedCall = false;
	public void OnStageFailed()
	{
		if (bStageFailedCall == true)
			return;
		
		Game.Instance.Pause = true;
		
		bStageFailedCall = true;
		
		UIMyStatusInfo myStatusInfo = GameUI.Instance.myStatusInfo;
		if (myStatusInfo != null && myStatusInfo.goBackButton != null)
		{
			myStatusInfo.goBackButton.isEnabled = false;
		}
		
		Invoke("OnStageFailedDelay", delayFailCallTime);
	}
	
	public void OnStageFailedDelay()
	{
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			int charIndex = Game.Instance.connector.charIndex;
			//CharPrivateData privateData = Game.Instance.charInfoData.privateDatas[charIndex];
			
			int curWaveStep = this.currentWaveStep;
			
			//GainItemInfo[] useItems = Game.Instance.charInfoData.useItems.ToArray();
			CharInfoData charData = Game.Instance.charInfoData;
			int usedPotion1 = 0;
			int usedPotion2 = 0;
			if (charData != null)
			{
				usedPotion1 = charData.usedPotion1;
				usedPotion2 = charData.usedPotion2;
			}
			
			packetSender.SendWaveEnd(charIndex, curWaveStep, (int)this.currentWaveTime, 0, usedPotion1, usedPotion2);
		}
	}
	
	public void OnWaveEnd()
	{
		PlayerController player = Game.Instance.player;
		if (player != null && player.stateController != null)
		{
			player.stateController.ChangeState(BaseState.eState.Stage_clear1);
		}
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
		{
			int charIndex = Game.Instance.connector.charIndex;
			//CharPrivateData privateData = Game.Instance.charInfoData.privateDatas[charIndex];
			
			//int curWaveStep = this.currentWaveStep;
			int curWaveStep = this.waveList.Count;
			
			int isClear = 1;
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eDefenceComplete, 0);
			
			//GainItemInfo[] useItmes = Game.Instance.charInfoData.useItems.ToArray();
			CharInfoData charData = Game.Instance.charInfoData;
			int usedPotion1 = 0;
			int usedPotion2 = 0;
			if (charData != null)
			{
				usedPotion1 = charData.usedPotion1;
				usedPotion2 = charData.usedPotion2;
			}
			packetSender.SendWaveEnd(charIndex, curWaveStep, (int)this.currentWaveTime, isClear, usedPotion1, usedPotion2);
		}
	}
	
	
	
	public string waveEndWindowPrefabPath = "UI/WaveMode/WaveEndBlack";
	public void ShowWaveEndWindow(bool isClear, Item rewardItem, int curStep, int curTime, int maxStep, int maxTime)
	{
		UIMyStatusInfo myStatusInfo = GameUI.Instance.myStatusInfo;
		if (myStatusInfo != null && myStatusInfo.goBackButton != null)
		{
			myStatusInfo.goBackButton.isEnabled = true;
		}
		
		Transform root = null;
		if (GameUI.Instance.uiRootPanel != null)
			root = GameUI.Instance.uiRootPanel.transform;
		
		WaveRewardWindow window = ResourceManager.CreatePrefab<WaveRewardWindow>(waveEndWindowPrefabPath, root);
		if (window != null)
		{			
			window.SetStageRewardItems(rewardItem);
			
			window.SetWaveClearTimeInfo(isClear, curStep, curTime, maxStep, maxTime);
		}
	}
	
	public void ResetMonsterGeneratorByBossRecall()
	{
		if (currentWave != null)
			currentWave.ClearMonsters();
	}
}
