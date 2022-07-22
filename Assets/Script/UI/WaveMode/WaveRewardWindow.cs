using UnityEngine;
using System.Collections;

public class WaveRewardWindow : MonoBehaviour {
	public string stageName = "TownTest";
	
	public ItemSlot rewardItemSlot = null;
	
	public WaveRecord curRecord = null;
	public WaveRecord maxRecord = null;
	
	public UISprite newRecordSprite = null;
	
	public void Update()
	{
	}
	
	public string loadingPanelPrefabPath = "";
	public LoadingPanel loadingPanel = null;
	public void CreateLoadingPanel(string stageName)
	{
		TownUI.firstWindowType = TownUI.eTOWN_UI_TYPE.WAVE;
		
		if (loadingPanel == null)
		{
			Transform uiRoot = this.transform.parent;
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, uiRoot, Vector3.zero);
		}
		else
		{
			loadingPanel.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		if (loadingPanel != null)
			loadingPanel.LoadScene(stageName, null);
	}
	
	public void LoadingCall()
	{
		CreateLoadingPanel(stageName);
		MonsterGenerator.isSurrendMode = false;
				
		DestroyObject(this.gameObject, 0.0f);
	}
		
	public void SetStageRewardItems(Item rewardItem)
	{
		if (rewardItemSlot != null)
			rewardItemSlot.SetItem(null);
		
		if (rewardItemSlot != null)
			rewardItemSlot.SetItem(rewardItem);
	}
	
	public void SetWaveClearTimeInfo(bool isClear, int curStep, int curTime, int maxStep, int maxTime)
	{
		bool isNewRecord = (curStep == maxStep && curTime == maxTime);
		
		if (curRecord != null)
			curRecord.SetWaveTime(curStep, curTime);
		
		if (maxRecord != null)
			maxRecord.SetWaveTime(maxStep, maxTime);
		
		if (newRecordSprite != null)
			newRecordSprite.enabled = isNewRecord;
	}
}
