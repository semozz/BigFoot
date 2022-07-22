using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BossDialogStringInfo
{
	public BossDialog.eDialogState dialogState = BossDialog.eDialogState.None;
	public float delayTime = 1.5f;
	
	public int dlgIndex = 0;
	public int dlgStringID = -1;
}

public class BossDialogController : MonoBehaviour {
	
	public string bossUpperTexture = "";
	public List<BossDialogStringInfo> dialogList = new List<BossDialogStringInfo>();
	
	//public List<BossDialogStringInfo> bossMonsterDialogList = new List<BossDialogStringInfo>();
	
	public string charAssassin = "SpeakIMG_C_Assassin";
	public string charWarrior = "SpeakIMG_C_Warrior";
	public string charWizard = "SpeakIMG_C_Wizard";
	//public List<BossDialogStringInfo> charDialogList = new List<BossDialogStringInfo>();
	
	
	public string bossDialogPrefab = "";
	public BossDialog bossDialog = null;
	
	public Vector3 dialogPos = new Vector3(0.0f, 0.0f, -100.0f);
	public static bool isBossDialogStart = false;
	public void BossDialogStart()
	{
		if (bossDialog == null)
		{
			Transform uiRoot = GameUI.Instance.uiRootPanel.transform;
			bossDialog = ResourceManager.CreatePrefab<BossDialog>(bossDialogPrefab, uiRoot, dialogPos);
		}
		
		if (bossDialog != null)
		{
			PlayerController player = Game.Instance.player;
			string charTexture = "";
			if (player != null)
			{
				switch(player.classType)
				{
				case GameDef.ePlayerClass.CLASS_WARRIOR:
					charTexture = charWarrior;
					break;
				case GameDef.ePlayerClass.CLASS_ASSASSIN:
					charTexture = charAssassin;
					break;
				case GameDef.ePlayerClass.CLASS_WIZARD:
					charTexture = charWizard;
					break;
				}
			}
			
			
			bossDialog.SetTexture(bossUpperTexture, charTexture);
			
			//bossDialog.SetBossDialog(bossMonsterDialogList, charDialogList);
			bossDialog.SetDialogInfos(dialogList);
			
			bossDialog.SetCharDialog(charDialogInfos, charDialogDelayTime);
			
			bossDialog.InitMonsters();
			
			isBossDialogStart = true;
		}
	}
	
	public float charDialogDelayTime = 1.5f;
	public List<DialogInfo> charDialogInfos = new List<DialogInfo>();
}
