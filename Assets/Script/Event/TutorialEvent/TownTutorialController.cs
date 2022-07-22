using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TownTutorialInfo
{
	public enum eTownTutorialStep
	{
		ActiveStorageButton,
		ActiveEquipButton,
		ActiveReinforceButton,
		ActiveComposeButton,
		ActiveStorageCloseButton,
		ModeExplaingMode,
		TutorialClose,
	}
	
	public eTownTutorialStep step = eTownTutorialStep.ActiveStorageButton;
	public string alretPrefab = "";
	public GameObject alertObj = null;
}

public class TownTutorialController : MonoBehaviour {
	
	public List<TownTutorialInfo> tutorialStepList = new List<TownTutorialInfo>();
	public Transform popupNode = null;
	
	public TownTutorialInfo currentInfo = null;
	public void NextStep()
	{
		if (currentInfo != null)
		{
			DestroyObject(currentInfo.alertObj, 0.1f);
			currentInfo = null;
		}
		
		int nCount = tutorialStepList.Count;
		if (nCount > 0)
		{
			currentInfo = tutorialStepList[0];
			tutorialStepList.RemoveAt(0);
		}
		
		if (currentInfo != null)
		{
			string prefabPath = string.Format("UI/Tutorial/{0}", currentInfo.alretPrefab);
			switch(currentInfo.step)
			{
			case TownTutorialInfo.eTownTutorialStep.TutorialClose:
				NoticePopupWindow popup = ResourceManager.CreatePrefab<NoticePopupWindow>(prefabPath, popupNode);
				if (popup != null && popup.buttonMessage != null)
				{
					popup.buttonMessage.target = this.gameObject;
					popup.buttonMessage.functionName = "OnTutorialClose";
					
					currentInfo.alertObj = popup.gameObject;
				}
				break;
			case TownTutorialInfo.eTownTutorialStep.ModeExplaingMode:
				currentInfo.alertObj = ResourceManager.CreatePrefab(prefabPath, popupNode);
				
				TweenChecker checker = null;
				if (currentInfo.alertObj != null)
					checker = currentInfo.alertObj.GetComponent<TweenChecker>();
				
				if (checker != null && checker.checkTween != null)
				{
					checker.checkTween.eventReceiver = this.gameObject;
					checker.checkTween.callWhenFinished = "OnTutorialClosed";
				}
				
				break;
			default:
				currentInfo.alertObj = ResourceManager.CreatePrefab(prefabPath, popupNode);
				break;
			}
		}
		else
		{
			CharInfoData charData = Game.Instance.charInfoData;
			CharPrivateData privateData = null;
			int charIndex = 0;
			if (Game.Instance.connector != null)
				charIndex = Game.Instance.connector.charIndex;
			
			if (charData != null)
			{
				charData.isTutorialComplete = true;
				privateData = charData.GetPrivateData(charIndex);
			}
			
			if (privateData != null)
				privateData.baseInfo.tutorial = 1;
			
			IPacketSender packetSender = Game.Instance.PacketSender;
			if (packetSender != null)
				packetSender.SendTownTutorialEnd(charIndex);
						
			TownUI townUI = GameUI.Instance.townUI;
			if (townUI != null)
			{
				townUI.SetTutorialMode(false);
				townUI.tutorialController = null;
			}
			
			DestroyObject(this.gameObject, 0.2f);
		}
	}
	
	public void GoLastStep()
	{
		int nCount = tutorialStepList.Count;
		
		int lastIndex = -1;
		for(int index = 0; index < nCount; ++index)
		{
			TownTutorialInfo info = tutorialStepList[index];
			if (info != null && info.step == TownTutorialInfo.eTownTutorialStep.TutorialClose)
			{
				lastIndex = index;
				break;
			}
		}
		
		if (lastIndex != -1)
		{
			tutorialStepList.RemoveRange(0, lastIndex);
			NextStep();
		}
	}
	
	public void ClearAlret()
	{
		if (currentInfo != null)
		{
			if (currentInfo.alertObj != null)
				DestroyObject(currentInfo.alertObj, 0.1f);
			
			currentInfo.alertObj = null;
		}
	}
	
	public void OnTutorialClose(GameObject obj)
	{
		NextStep();
	}
	
	public void OnTutorialClosed()
	{
		NextStep();
	}
}
