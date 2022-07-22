using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GeneratorInfo
{
	public GameObject monster = null;
}

[System.Serializable]
public class MonsterGenerator : MonoBehaviour {
	public static bool isSurrendMode = false;
	
	public GeneratorTarget genTargetPos = null;
	
	public LayerMask triggerLayerMask = 0;
	public bool isTriggerMode = false;
	public float triggerLength = 1.0f;
	public List<GeneratorInfo> generatorInfos = new List<GeneratorInfo>();
	
	public bool isEnable = true;
	private bool isActivate = false;
	private bool isFirstTime = true;
	
	public float genCoolTime = 5.0f;
	public float genDelayTime = 0.0f;
	
	public int curGenIndex = 0;
	public int totalCount = 5;
	public int subLimitCount = 3;
	public int curGenCount = 0;
	
	public LayerMask genPosPickLayerMask = 0;
	private List<GameObject> genMonsterList = new List<GameObject>();
	
	private bool bObjectEntered = false;
	
	public int monsterLinkID = -1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Game.Instance.Pause == true)
			return;
		
		if (isSurrendMode == true)
			return;
		
		if (isActivate == false || isEnable == false)
			return;
		
		if (genTargetPos == null)
			return;
		
		if (genMonsterList.Count < subLimitCount)
			genDelayTime -= Time.deltaTime;
		
		if (genDelayTime <= 0.0f)
		{
			genDelayTime = genCoolTime;
			
			if (isFirstTime == true)
			{
				isFirstTime = false;
				
				GenerateFirstTime();
			}
			else
			{
				Generate();
			}
			
			if (totalCount != -1 && curGenCount >= totalCount)
				OnDeactivate();
		}
	}
	
	
	public void OnActivate()
	{
		isActivate = true;
		isEnable = true;
		
		isFirstTime = true;
		
		genDelayTime = genCoolTime;
		
		curGenIndex = 0;
		curGenCount = 0;
	}
	
	public void OnDeactivate()
	{
		isActivate = false;
		isEnable = false;
	}
	
	public void ActivateTrigger(bool bTrigger)
	{
		if (bTrigger == false)
		{
			isEnable = false;
		}
		else
		{
			if (isEnable == false)
			{
				isEnable = true;
				if (isActivate == false && bObjectEntered == true)
					OnActivate();
			}
		}
	}
	
	public void AddMonster(GameObject monster)
	{
		if (monster != null)
		{
			genMonsterList.Add(monster);
			
			BaseMonster baseMon = monster.GetComponent<BaseMonster>();
			if (baseMon != null)
				baseMon.SetMonsterGenerator(this);
		}
	}
	
	public void RemoveMonster(GameObject monster)
	{
		int nCount = genMonsterList.Count;
		
		GameObject temp = null;
		for (int index = 0; index < nCount; ++index)
		{
			temp = genMonsterList[index];
			if (temp != null && temp == monster)
			{
				genMonsterList.RemoveAt(index);
				break;
			}
		}
	}
	
	public void GenerateFirstTime()
	{
		float genPosDelta = 1.0f;
		Vector3 genOrigPos = genTargetPos.transform.position;
		Vector3 genPos = genOrigPos;
		
		int nCount = generatorInfos.Count;
		
		GeneratorInfo genInfo = null;
		int genStep = 0;
		for (int index = 0; index < nCount; ++index)
		{
			genInfo = generatorInfos[index];
			if (genInfo != null && genInfo.monster != null)
			{
				genPos = genOrigPos;
				
				genStep = index % 3;
				if (genStep == 1)
					genPos.x += genPosDelta;
				else if (genStep == 2)
					genPos.x -= genPosDelta;
				
				GenerateMonster(genInfo, genPos);
			}
		}
		
		curGenIndex = 0;
	}
	
	public void Generate()
	{
		if (isActivate == false || isEnable == false)
			return;
		
		if (genMonsterList.Count >= subLimitCount)
			return;
		
		int nCount = generatorInfos.Count;
		
		Vector3 genOrigPos = genTargetPos.transform.position;
		Vector3 genPos = genOrigPos;
		
		int genStep = 0;
		
		float genPosDelta = 1.0f;
		GeneratorInfo genInfo = null;
		
		if (curGenIndex >= 0 && curGenIndex < nCount)
			genInfo = generatorInfos[curGenIndex];
		
		if (genInfo != null && genInfo.monster != null)
		{
			genStep = curGenIndex % 3;
			
			if (genStep == 1)
				genPos.x += genPosDelta;
			else if (genStep == 2)
				genPos.x -= genPosDelta;
			
			GenerateMonster(genInfo, genPos);
		}
		
		if (nCount > 0)
			curGenIndex = (curGenIndex + 1) % nCount;
	}
	
	
	protected void GenerateMonster(GeneratorInfo info, Vector3 genPos)
	{
		RaycastHit hit;
		
		if (Physics.Raycast(genPos, Vector3.down, out hit, Mathf.Infinity, genPosPickLayerMask) == true)
		{
			GameObject monObj = (GameObject)Instantiate(info.monster);
			
			if (monObj != null)
			{
				monObj.transform.position = genPos;
				
				AddMonster(monObj);
				
				curGenCount++;
				
				ActorManager actorManager = ActorManager.Instance;
				
				BaseMonster baseMon = monObj.GetComponent<BaseMonster>();
				
				if (actorManager != null)
					actorManager.AddActor(baseMon.myInfo.myTeam, baseMon.myInfo);
				
				if (baseMon != null && actorManager != null)
				{
					MonsterTargetSearch search = monObj.GetComponent<MonsterTargetSearch>();
					if (search != null)
						search.bLimitLength = false;
					
					if (BossDialogController.isBossDialogStart == true)
						baseMon.isEnableUpdate = false;
					else
						baseMon.ChangeTarget(actorManager.playerInfo);
					
					Game.Instance.ApplyIgnoreCollision(baseMon);
				}
			}
		}
	}
	
	public void UpdateData()
	{
		Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
		if (rigidBody == null)
			rigidBody = gameObject.AddComponent<Rigidbody>();
		
		if (rigidBody != null)
		{
			rigidBody.isKinematic = true;
			rigidBody.useGravity = false;
		}
		
		BoxCollider collider = gameObject.GetComponent<BoxCollider>();
		if (isTriggerMode == true)
		{
			if (collider == null)
				collider = gameObject.AddComponent<BoxCollider>();
			
			if (collider != null)
			{
				collider.isTrigger = true;
				collider.size = new Vector3(triggerLength, 1.0f, 10.0f);
			}
		}
		else
		{
			if (collider != null)
				DestroyImmediate(collider);
		}
	}
	
	public void ResetMonsters()
	{
		foreach(GameObject mon in genMonsterList)
		{
			Destroy(mon, 0.1f);
		}
	}
	
	public void SurrendMonsters()
	{
		foreach(GameObject mon in genMonsterList)
		{
			BaseMonster baseMon = null;
			if (mon == null)
				continue;
			
			baseMon = mon.GetComponent<BaseMonster>();
			
			if (baseMon != null)
				baseMon.SetSurrend();
		}
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if (isEnable == false || isActivate == true)
			return;
		
		int layerValue = other.gameObject.layer;
		int checkLayerMask = 1 << layerValue;
		
		int maskValue = triggerLayerMask.value;
		
		if ((maskValue & checkLayerMask) == checkLayerMask)
		{
			OnActivate();
			bObjectEntered = true;
		}
	}
	
	public void OnTriggerExit(Collider other)
	{
		int layerValue = other.gameObject.layer;
		int checkLayerMask = 1 << layerValue;
		
		int maskValue = triggerLayerMask.value;
		
		if ((maskValue & checkLayerMask) == checkLayerMask)
		{
			bObjectEntered = false;
		}
	}
}
