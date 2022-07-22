using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EndInfo
{
	public MonsterEndController.eEndState endState = MonsterEndController.eEndState.None;
	public float delayTime = 1.0f;
	
	public float targetDistant = 3.5f;
	public int dlgStringID = -1;
}

public class MonsterEndController : MonoBehaviour {
	public enum eEndState
	{
		None,
		TargetMove,
		BossDialog,
		CharDialog,
		End,
	}
	
	public bool isActivate = false;
	
	public List<EndInfo> infoList = new List<EndInfo>();
	
	public float delayTime = 0.0f;
	public bool isEndCall = false;
	
	public void Update()
	{
		if (isActivate == false)
			return;
		
		delayTime -= Time.deltaTime;
		if (delayTime <= 0.0f)
		{
			int nCount = infoList.Count;
			if (nCount > 0)
			{
				EndInfo info = infoList[0];
				bool bRemove = false;
				switch(info.endState)
				{
				case MonsterEndController.eEndState.TargetMove:
					bRemove = UpdateTargetMove(info);
					break;
				case MonsterEndController.eEndState.BossDialog:
					UpdateBossDialog(info);
					delayTime = info.delayTime;
					bRemove = true;
					break;
				case MonsterEndController.eEndState.CharDialog:
					UpdateCharDialog(info);
					delayTime = info.delayTime;
					bRemove = true;
					break;
				case MonsterEndController.eEndState.End:
					StateController stateController = this.gameObject.GetComponent<StateController>();
					if (stateController != null)
						stateController.ChangeState(BaseState.eState.StageEndStart);
					
					bRemove = true;
					delayTime = info.delayTime;
					break;
				}
				
				if (bRemove == true)
				{
					isFirstMove = true;
					infoList.RemoveAt(0);
				}
			}
			else
			{
				if (isEndCall == false)
				{
					LifeManager lifeManager = this.gameObject.GetComponent<LifeManager>();
					lifeManager.ApplyMonsterKillCount(lifeManager);
					
					//업적...
					BaseMonster monster = this.gameObject.GetComponent<BaseMonster>();
					if (monster != null)
						Game.Instance.ApplyAchievement(Achievement.eAchievementType.eKillMonster, monster.stageType, monster.attributeTableID);
					
					isEndCall = true;
					
					isActivate = false;
				}
			}
		}
	}
	
	private bool isFirstMove = true;
	public bool UpdateTargetMove(EndInfo info)
	{
		StateController stateController = this.gameObject.GetComponent<StateController>();
		
		if (isFirstMove == true)
		{
			if (stateController.currentState == BaseState.eState.Stand)
			{
				SetFirstMoveStep(info);
				isFirstMove = false;
			}
			return false;
		}
		
		PlayerController player = Game.Instance.player;
		
		Vector3 vMyPos = this.transform.position;
		Vector3 vPlayerPos = vMyPos;
		if (player != null)
			vPlayerPos = player.transform.position;
		
		Vector3 vDiff = vPlayerPos - vMyPos;
		float fDistance = Mathf.Abs(vDiff.x);
		
		
		bool bResult = fDistance > info.targetDistant;
		
		info.delayTime -= Time.deltaTime;
		if (info.delayTime <= 0.0f)
			bResult = true;
		
		if (bResult == true)
		{
			BaseMonster monster = this.gameObject.GetComponent<BaseMonster>();
			if (monster != null)
				monster.ChangeMoveDir(vPlayerPos, true);
			
			if (stateController != null)
				stateController.ChangeState(BaseState.eState.Die);
		}
		else
		{
			if (stateController != null)
				stateController.ChangeState(BaseState.eState.Run);
		}
		
		return bResult;
	}
	
	public void UpdateBossDialog(EndInfo info)
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string bossDlgString = "";
		if (stringTable != null)
			bossDlgString = stringTable.GetData(info.dlgStringID);
		
		UpdateBossDialog(bossDlgString, info.delayTime);
	}
	
	public void UpdateBossDialog(string dlgStr, float lifeTime)
	{
		LifeManager lifeManager = this.gameObject.GetComponent<LifeManager>();
		if (lifeManager != null)
		{
			lifeManager.bHPCheck = false;
			lifeManager.DoTalk(dlgStr, lifeTime, DialogInfo.eDialogType.Normal, false);
		}
	}
	
	public void UpdateCharDialog(EndInfo info)
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
		
		UpdateCharDialog(bossDlgString, info.delayTime);
	}
	
	public void UpdateCharDialog(string dlgStr, float lifeTime)
	{
		PlayerController player = Game.Instance.player;
		LifeManager lifeManager = player != null ? player.lifeManager : null;
		
		if (lifeManager != null)
			lifeManager.DoTalk(dlgStr, lifeTime, DialogInfo.eDialogType.Normal, false);
	}
	
	public void SetActivate()
	{
		this.isActivate = true;
		
		//Game.Instance.Pause = true;
		//Game.Instance.InputPause = true;
	}
	
	public void SetFirstMoveStep(EndInfo info)
	{
		gameObject.layer = LayerMask.NameToLayer("HideBody");
		BaseMoveController moveController = this.gameObject.GetComponent<BaseMoveController>();
		if (moveController != null)
			moveController.ignoreMonsterBody = true;
		
		PlayerController player = Game.Instance.player;
		
		Vector3 vMyPos = this.transform.position;
		Vector3 vPlayerPos = vMyPos;
		if (player != null)
			vPlayerPos = player.transform.position;
		
		Vector3 targetPos = vPlayerPos;
		Vector3 addPos = Vector3.zero;
		if (player.moveController.moveDir == Vector3.right)
			addPos.x = info.targetDistant;
		else
			addPos.x = -info.targetDistant;
		
		RaycastHit hitInfo;
		int groundMask = 1 << LayerMask.NameToLayer("Ground");
		bool bCheck = false;
		if (Physics.Raycast(targetPos + addPos + Vector3.up, Vector3.down, out hitInfo, float.MaxValue, groundMask) == true)
		{
			if (hitInfo.collider.name.Contains("ground") == true)
				bCheck = true;
			else
				bCheck = false;
		}
		
		if (bCheck == false)
			addPos = -addPos;
		
		targetPos += addPos;
		
		BaseMonster monster = this.gameObject.GetComponent<BaseMonster>();
		if (monster != null)
		{
			monster.isEnableUpdate = false;
			monster.ChangeMoveDir(targetPos, true);
		}
		
		if (player != null)
			player.ChangeMoveDir(targetPos, true);
		
		StateController stateController = this.gameObject.GetComponent<StateController>();
		if (stateController != null)
			stateController.ChangeState(BaseState.eState.Run);
	}
}
