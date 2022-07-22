using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour {
	public Transform popupNode = null;
	
	public string pausePopupPrefab = "";
	BaseConfirmPopup pausePopup = null;
	public bool IsPauseMode
	{
		get { return pausePopup != null; }
	}
	
	public int pauseMessageStringID_Field = 219;
	public int pauseMessageStringID_Wave = 220;
	public int pauseMessageStringID_Tutorial = 221;
	public void OnOk(GameObject obj)
	{
		Game.Instance.Pause = true;
		Time.timeScale = 0.0f;
		
		if (pausePopup == null)
			pausePopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(pausePopupPrefab, popupNode, Vector3.zero);
		
		if (pausePopup != null)
		{
			int messageStringID = -1;
			StageManager stageManager = Game.Instance.stageManager;
			if (stageManager != null)
			{
				switch(stageManager.StageType)
				{
				case StageManager.eStageType.ST_TUTORIAL:
					messageStringID = pauseMessageStringID_Tutorial;
					break;
				case StageManager.eStageType.ST_WAVE:
					messageStringID = pauseMessageStringID_Wave;
					break;
				default:
					messageStringID = pauseMessageStringID_Field;
					break;
				}
			}
			
			if (messageStringID != -1)
				pausePopup.SetMessage(messageStringID);
			
			pausePopup.cancelButtonMessage.target = this.gameObject;
			pausePopup.cancelButtonMessage.functionName = "OnClosePopup";
			
			pausePopup.okButtonMessage.target = this.gameObject;
			pausePopup.okButtonMessage.functionName = "OnGoTown";
		}
	}
	
	public void OnClosePopup(GameObject obj)
	{
		Game.Instance.Pause = false;
		Time.timeScale = 1.0f;
		
		if (pausePopup != null)
		{
			DestroyObject(pausePopup.gameObject, 0.0f);
			pausePopup = null;
		}
	}
	
	public void LoadScene(string sceneName)
	{
		CreateLoadingPanel(sceneName);
	}
	
	public string townStage = "";
	public void OnGoTown(GameObject obj)
	{
		System.Collections.Generic.List<ActorInfo> monsterList = null;
		ActorManager actorManager = ActorManager.Instance;
		if (actorManager != null)
			monsterList = actorManager.GetActorList(ActorInfo.TeamNo.Team_Two);
		
		MonsterGenerator.isSurrendMode = true;
		Game.Instance.Pause = true;
		
		if (monsterList != null)
		{
			LifeManager lifeManager = null;
			float hpValue = 0.0f;
			
			foreach(ActorInfo info in monsterList)
			{
				lifeManager = info.GetLifeManager();
				hpValue = lifeManager != null ? lifeManager.GetHP() : 0.0f;
				
				if (hpValue <= 0.01f)
					continue;
				
				lifeManager.stateController.ChangeState(BaseState.eState.Stand);
			}
		}
		
		Time.timeScale = 1.0f;
		
		StageManager stageManager = Game.Instance.stageManager;
		if (stageManager != null)
		{
			int charIndex = 0;
			if (Game.Instance.connector != null)
				charIndex = Game.Instance.connector.charIndex;
			
			switch(stageManager.StageType)
			{
			case StageManager.eStageType.ST_TUTORIAL:
				IPacketSender sender = Game.Instance.PacketSender;
				if (sender != null)
					sender.SendTownTutorialEnd(charIndex);
				break;
			default:
				int usedPotion1 = 0;
				int usedPotion2 = 0;
				CharInfoData charData = Game.Instance.charInfoData;
				if (charData != null)
				{
					usedPotion1 = charData.usedPotion1;
					usedPotion2 = charData.usedPotion2;
				}
				
				IPacketSender packetSender = Game.Instance.packetSender;
				if (packetSender != null)
					packetSender.SendStageEndFailed(charIndex, usedPotion1, usedPotion2);
				break;
			}
		}
		
		LoadScene(townStage);
		
		//ClosePopup(null);
	}
	
	public string loadingPanelPrefabPath = "";
	public LoadingPanel loadingPanel = null;
	public void CreateLoadingPanel(string stageName)
	{
		if (loadingPanel == null)
		{
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, popupNode, Vector3.zero);
		}
		else
		{
			loadingPanel.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		if (loadingPanel != null)
			loadingPanel.LoadScene(stageName, null);
	}
}
