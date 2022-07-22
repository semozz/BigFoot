using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BossDialog : MonoBehaviour {
	public enum eDialogState
	{
		None,
		BossIn,
		BossDialog,
		BossDialogEnd,
		BossOut,
		CharIn,
		CharDialog,
		CharDialogEnd,
		CharOut,
		End,
	}
	public eDialogState currentState = eDialogState.None;
	
	public Animation bossAnim = null;
	public string bossInAnimation = "";
	public string bossOutAnimation = "";
	
	public UITexture bossUpperBody = null;
	public List<ChatBubble> bossDlgInfos = new List<ChatBubble>();
	
	public List<BossDialogStringInfo> dlgStringInfos = new List<BossDialogStringInfo>();
	
	//public List<BossDialogStringInfo> bossDlgStringInfos = new List<BossDialogStringInfo>();
	
	public Animation charAnim = null;
	public string charInAnimation = "";
	public string charOutAnimation = "";
	
	public UITexture charUpperBody = null;
	public List<ChatBubble> charDlgInfos = new List<ChatBubble>();
	//public List<BossDialogStringInfo> charDlgStringInfos = new List<BossDialogStringInfo>();
	
	/*
	public float bossInDelayTime = 1.0f;
	public float bossOutDelayTime = 1.0f;
	public float bossDialogEndDelay = 0.5f;
	public float charInDelayTime = 1.0f;
	public float charOutDelayTime = 1.0f;
	public float dialogEndDelayTime = 1.0f;
	*/
	
	public string texturePath = "IMG/UI/Speak_UpperBody/";
	public void SetTexture(string bossUpper, string charUpper)
	{
		if (bossUpperBody != null)
			bossUpperBody.mainTexture = LoadTexture(bossUpper);
		
		if (charUpperBody != null)
			charUpperBody.mainTexture = LoadTexture(charUpper);
	}
	
	public Texture LoadTexture(string textureName)
	{
		string pathStr = string.Format("{0}{1}", texturePath, textureName);
		//Texture texture = (Texture2D)Resources.Load(pathStr);
		Texture texture = ResourceManager.LoadTexture(pathStr);
		
		return texture;
	}
	
	float delayTime = 0.0f;
	/*
	void Update()
	{
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0f)
		{
			switch(currentState)
			{
			case eDialogState.BossIn:
				currentState = eDialogState.BossDialog;
				UpdateBossDialog();
				break;
			case eDialogState.BossDialog:
				UpdateBossDialog();
				break;
			case eDialogState.BossDialogEnd:
				if (bossAnim != null)
					bossAnim.Play(bossOutAnimation);
				
				currentState = eDialogState.BossOut;
				delayTime = bossOutDelayTime;
				break;
			case eDialogState.BossOut:
				if (charDlgStringInfos.Count > 0)
				{
					if (charAnim != null)
						charAnim.Play(charInAnimation);
					
					currentState = eDialogState.CharIn;
					delayTime = charInDelayTime;
				}
				else
				{
					currentState = eDialogState.End;
					delayTime = dialogEndDelayTime;
				}
				break;
			case eDialogState.CharIn:
				currentState = eDialogState.CharDialog;
				UpdateCharDialog();
				break;
			case eDialogState.CharDialog:
				UpdateCharDialog();
				break;
			case eDialogState.CharDialogEnd:
				if (charAnim != null)
					charAnim.Play(charOutAnimation);
				
				currentState = eDialogState.CharOut;
				delayTime = charOutDelayTime;
				break;
			case eDialogState.CharOut:
				currentState = eDialogState.End;
				delayTime = dialogEndDelayTime;
				break;
			case eDialogState.End:
				PlayerController player = Game.Instance.player;
				if (player != null && charDialogInfos.Count > 0)
				{
					player.SetCharDialog(charDialogInfos);
					player.Invoke("OnCharDialog", charDialogDelayTime);
				}
				
				DestroyObject(this.gameObject, 0.0f);
				Game.Instance.Pause = false;
				break;
			}
		}
	}
	*/
	
	public void Update()
	{
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0f)
		{
			int nCount = dlgStringInfos.Count;
			if (nCount > 0)
			{
				BossDialogStringInfo info = dlgStringInfos[0];
				switch(info.dialogState)
				{
				case BossDialog.eDialogState.BossIn:
					if (bossAnim != null)
						bossAnim.Play(bossInAnimation);
					
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.BossDialog:
					UpdateBossDialog(info);
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.BossDialogEnd:
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.BossOut:
					if (bossAnim != null)
						bossAnim.Play(bossOutAnimation);
					
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.CharIn:
					if (charAnim != null)
						charAnim.Play(charInAnimation);
					
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.CharDialog:
					UpdateCharDialog(info);
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.CharDialogEnd:
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.CharOut:
					if (charAnim != null)
						charAnim.Play(charOutAnimation);
					
					delayTime = info.delayTime;
					break;
				case BossDialog.eDialogState.End:
					delayTime = info.delayTime;
					break;
				}
				
				dlgStringInfos.RemoveAt(0);
			}
			else
			{
				PlayerController player = Game.Instance.player;
				if (player != null && charDialogInfos.Count > 0)
				{
					player.SetCharDialog(charDialogInfos);
					player.Invoke("OnCharDialog", charDialogDelayTime);
				}
				
				ResetMonsters();
				
				DestroyObject(this.gameObject, 0.0f);
				Game.Instance.Pause = false;
				Game.Instance.InputPause = false;
				
				BossDialogController.isBossDialogStart = false;
			}
		}
	}
	
	/*
	public void UpdateBossDialog()
	{
		int nCount = bossDlgStringInfos.Count;
		if (nCount > 0)
		{
			BossDialogStringInfo info = bossDlgStringInfos[0];
			
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			
			string bossDlgString = "";
			if (stringTable != null)
				bossDlgString = stringTable.GetData(info.dlgStringID);
			
			UpdateBossDialog(info.dlgIndex, bossDlgString, info.delayTime);
			delayTime = info.delayTime;
			
			bossDlgStringInfos.RemoveAt(0);
		}
		else
		{
			currentState = eDialogState.BossDialogEnd;
			delayTime = bossDialogEndDelay;
		}
	}
	*/
	
	public void UpdateBossDialog(BossDialogStringInfo info)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string bossDlgString = "";
		if (stringTable != null)
			bossDlgString = stringTable.GetData(info.dlgStringID);
		
		UpdateBossDialog(info.dlgIndex, bossDlgString, info.delayTime);
	}
	
	public void UpdateBossDialog(int index, string dlgStr, float lifeTime)
	{
		ChatBubble chatBubble = null;
		
		int nCount = bossDlgInfos.Count;
		for (int idx = 0; idx < nCount; ++idx)
		{
			chatBubble = bossDlgInfos[idx];
			if (chatBubble == null)
				continue;
			
			bool isActive = idx == index;
			
			chatBubble.gameObject.SetActive(isActive);
			if (isActive == true)
				chatBubble.SetMsg(dlgStr, lifeTime);
		}
	}
	
	/*
	public void UpdateCharDialog()
	{
		int nCount = charDlgStringInfos.Count;
		if (nCount > 0)
		{
			BossDialogStringInfo info = charDlgStringInfos[0];
			
			TableManager tableManager = TableManager.Instance;
			StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
			
			PlayerController player = Game.Instance.player;
			int classIndex = 0;
			switch(player.classType)
			{
			case GameDef.ePlayerClass.CLASS_WARRIOR:
				classIndex = 500;
				break;
			case GameDef.ePlayerClass.CLASS_ASSASSIN:
				classIndex = 600;
				break;
			case GameDef.ePlayerClass.CLASS_WIZARD:
				classIndex = 700;
				break;
			}
			
			int talkID = classIndex + info.dlgStringID;
			
			string bossDlgString = "";
			if (stringTable != null)
				bossDlgString = stringTable.GetData(talkID);
			
			UpdateCharDialog(info.dlgIndex, bossDlgString, info.delayTime);
			delayTime = info.delayTime;
			
			charDlgStringInfos.RemoveAt(0);
		}
		else
		{
			currentState = eDialogState.CharDialogEnd;
			delayTime = bossDialogEndDelay;
		}
	}
	*/
	
	public void UpdateCharDialog(BossDialogStringInfo info)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		PlayerController player = Game.Instance.player;
		int classIndex = 0;
		switch(player.classType)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			classIndex = 500;
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			classIndex = 600;
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			classIndex = 700;
			break;
		}
		
		int talkID = classIndex + info.dlgStringID;
		
		string bossDlgString = "";
		if (stringTable != null)
			bossDlgString = stringTable.GetData(talkID);
		
		UpdateCharDialog(info.dlgIndex, bossDlgString, info.delayTime);
	}
	
	public void UpdateCharDialog(int index, string dlgStr, float lifeTime)
	{
		ChatBubble chatBubble = null;
		
		int nCount = charDlgInfos.Count;
		for (int idx = 0; idx < nCount; ++idx)
		{
			chatBubble = charDlgInfos[idx];
			if (chatBubble == null)
				continue;
			
			bool isActive = idx == index;
			
			chatBubble.gameObject.SetActive(isActive);
			chatBubble.SetMsg(dlgStr, lifeTime);
		}
	}
	
	/*
	public void SetBossDialog(List<BossDialogStringInfo> dlgList1, List<BossDialogStringInfo> dlgList2)
	{
		Game.Instance.Pause = true;
		
		bossDlgStringInfos.Clear();
		charDlgStringInfos.Clear();
		
		bossDlgStringInfos.AddRange(dlgList1);
		charDlgStringInfos.AddRange(dlgList2);
		
		if (bossAnim != null)
			bossAnim.Play(bossInAnimation);
		
		currentState = eDialogState.BossIn;
		delayTime = bossInDelayTime;
	}
	*/
	
	public float charDialogDelayTime = 1.5f;
	public List<DialogInfo> charDialogInfos = new List<DialogInfo>();
	public void SetCharDialog(List<DialogInfo> dlgList, float delayTime)
	{
		charDialogDelayTime = delayTime;
		charDialogInfos.Clear();
		charDialogInfos.AddRange(dlgList);
	}
	
	public void SetDialogInfos(List<BossDialogStringInfo> dlgList)
	{
		Game.Instance.Pause = true;
		Game.Instance.InputPause = true;
		
		ActorManager actorManager = ActorManager.Instance;
		foreach(var temp in actorManager.teamActorList)
		{
			foreach(ActorInfo info in temp.Value)
			{
				BaseMonster monster = info.gameObject.GetComponent<BaseMonster>();
				LifeManager lifeManager = info.gameObject.GetComponent<LifeManager>();
				float hpValue = lifeManager != null ? lifeManager.GetHP() : 0.0f;
				
				StateController stateController = info.gameObject.GetComponent<StateController>();
				BaseState.eState curState = stateController != null ? stateController.currentState : BaseState.eState.Stand;
				bool isDiestate = false;
				switch(curState)
				{
				case BaseState.eState.Die:
				case BaseState.eState.Knockdown_Die:
					isDiestate = true;
					break;
				}
				
				if (hpValue > 0.0f && isDiestate == false)
				{
					if (monster != null)
						monster.isEnableUpdate = false;
					
					if (stateController != null)
						stateController.ChangeState(BaseState.eState.Stand);
				}
			}
		}
		
		dlgStringInfos.Clear();
		
		dlgStringInfos.AddRange(dlgList);
		
		if (bossAnim != null)
			bossAnim.Play(bossInAnimation);
		
		delayTime = 0.0f;
		
		//currentState = eDialogState.BossIn;
		//delayTime = bossInDelayTime;
	}
	
	public void InitMonsters()
	{
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
						monster.stateController.ChangeState(BaseState.eState.Stand);
						monster.isEnableUpdate = false;
					}
				}
			}
		}
	}
	
	public void ResetMonsters()
	{
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
						monster.stateController.ChangeState(BaseState.eState.Stand);
						monster.isEnableUpdate = true;
					}
				}
			}
		}
	}
}
