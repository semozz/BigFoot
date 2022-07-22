using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MercenaryGenerateInfo
{
	public enum eMercenaryType
	{
		GUARDIAN,
		HUNTER,
	}
	public eMercenaryType type = eMercenaryType.GUARDIAN;
}

[System.Serializable]
public class MercenaryGenerator : MonoBehaviour 
{
	public GeneratorTarget regenTargetPos = null;
	public float TriggerLength = 1.0f;
	public List<MercenaryGenerateInfo> GenerateInfos = new List<MercenaryGenerateInfo>();

	private bool isActivate = false; 			//활성화 여부
	private bool isFirstGeneator = true;	//최초 몬스터 생성 여부(최최는 생성 정보에 있는 모든 몬스터들을 한꺼번에 생성한다.)
	
	public float coolTime = 5.0f;				//기본 쿨타임.
	
	private float genDelayTime = 0.0f;	//남은 쿨타임.
	
	private int currentGenIndex = 0;	//생성되어야할 몬스터 정보 Index.
	public int totalLimitCount = 5;			//최대 생성될 몬스터 제한 갯수.
	public int subLimitCount = 3;			//한번에(?) 생성될 몬스터 제한 갯수.
	private int currentGenCount = 0;	//지금까지 생성한 몬스터 갯수.
	public void ResetCurrentCount()
	{
		if (isActivate == false || isEnable == false)
			OnActivate();
		else
			currentGenCount = 0;
	}
	
	public bool IsSelfBoxCollider = true;
	
	private List<GameObject> GenMercenaryList = new List<GameObject>();	//생성된 몬스터 리스트.
	
	public bool isEnable = true;
	
	private bool bObjectEntered = false;
	public int waveStep = -1;

	public void Update ()
	{
		if (isActivate == false || isEnable == false)
			return;
		
		if (regenTargetPos == null)
			return;
		
		if (GenerateInfos.Count < subLimitCount)
			genDelayTime -= Time.deltaTime;
		
		if (genDelayTime <= 0.0f)
		{
			genDelayTime = coolTime;
			
			if (isFirstGeneator == true)
			{
				isFirstGeneator = false;
				
				GenerateFirstTime();
			}
			else
			{
				Generate();
			}
			
			if (totalLimitCount != -1 && currentGenCount >= totalLimitCount)
				OnDeactivate();
		}
	}

    public void OnActivate()
    {
		//제너레이터 활성화 시킴.
        isActivate = true;
		isEnable = true;
		
		isFirstGeneator = true;
		
		//쿨타임 초기화 시킴.
		genDelayTime = coolTime;
		
		currentGenIndex = 0;
		currentGenCount = 0;
    }
	
	public void OnDeactivate()
	{
		//제너레이터 비활성화 시킴.
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
	
	//생성된 몬스터를 리스트에 담아 놓는다.
	public void AddMercenaryActor(GameObject actor)
	{
		if (actor != null)
		{
			GenMercenaryList.Add(actor);
			
			MercenaryActor mercenaryActor = actor.GetComponent<MercenaryActor>();
			if (mercenaryActor != null)
				mercenaryActor.SetMercenaryGenerator(this);
		}
	}

	public void RemoveMercenaryActor(GameObject actor)
	{
		//기존 몬스터 리스트에서 같은 녀석을 찾아서 지운다.
		for (int i = 0; i < GenMercenaryList.Count; i++)
        {
			GameObject tempActor = GenMercenaryList[i];
			if (tempActor != null && tempActor == actor)
			{
				GenMercenaryList.RemoveAt(i);
				break;
			}
		}
	}
	
	private void GenerateFirstTime()
	{
		float genPosDelta = 1.0f;
		Vector3 genOrigPos = regenTargetPos.transform.position;
		Vector3 genPos = genOrigPos;
		
		int nCount = GenerateInfos.Count;
		
		int genStep = 0;
		for (int index = 0; index < nCount; ++index)
		{
			MercenaryGenerateInfo info = GenerateInfos[index];
			
			genPos = genOrigPos;
			
			genStep = index % 3;
			if (genStep == 1)
				genPos.x += genPosDelta;
			else if (genStep == 2)
				genPos.x -= genPosDelta;
			
			GeneratorMercenary(info, genPos);
		}
		
		currentGenIndex = 0;
	}
	
    private void Generate()
    {
		if (isActivate == false || isEnable == false)
			return;
		
		if (GenMercenaryList.Count >= subLimitCount)
			return;
		
		int nCount = this.GenerateInfos.Count;
		
		Vector3 genOrigPos = regenTargetPos.transform.position;
		Vector3 genPos = genOrigPos;
		
		int genStep = 0;
		
		float genPosDelta = 1.0f;
		MercenaryGenerateInfo genInfo = null;
		
		if (currentGenIndex >= 0 && currentGenIndex < nCount)
			genInfo = GenerateInfos[currentGenIndex];
		
		genStep = currentGenIndex % 3;
		
		if (genStep == 1)
			genPos.x += genPosDelta;
		else if (genStep == 2)
			genPos.x -= genPosDelta;
		
		GeneratorMercenary(genInfo, genPos);
		
		if (nCount > 0)
			currentGenIndex = (currentGenIndex + 1) % nCount;
	}
	
	protected void GeneratorMercenary(MercenaryGenerateInfo info, Vector3 vPos)
	{
		RaycastHit hit;
			
		var groundLayer = 1 << LayerMask.NameToLayer("Ground");
		var floorLayer = 1 << LayerMask.NameToLayer("Floor");
		var layerMask = groundLayer | floorLayer;
		
		int mercenerayLevel = 0;
		/*
		if (Game.instance.avatar != null)
		{
			switch(info.type)
			{
			case MercenaryGenerateInfo.eMercenaryType.GUARDIAN:
				mercenerayLevel = Game.instance.avatar.guardianLevel;
				break;
			case MercenaryGenerateInfo.eMercenaryType.HUNTER:
				mercenerayLevel = Game.instance.avatar.hunterLevel;
				break;
			}
		}
		*/
			
		if (Physics.Raycast(vPos, Vector3.down, out hit, Mathf.Infinity, layerMask) == true)
        {			
			string resourceName = GetMercenaryResource(info.type, mercenerayLevel);
			//GameObject actor = (GameObject)Instantiate(Resources.Load(resourceName));
			MercenaryActor mercenaryActor = ResourceManager.CreatePrefab<MercenaryActor>(resourceName);
			
			float waveRate = 0.5f + ((float)(waveStep + 1) / (float)GameDef.MaxWaveCount);
			
			if (mercenaryActor.bSetupWaveRate == false)
				mercenaryActor.SetMercenaryLevel(mercenerayLevel, info.type, waveRate);
			
			//몬스터 생성 위치를 그냥 TargetPos위치로 설정 한다.
			mercenaryActor.transform.position = hit.point + Vector3.up;
			
			currentGenCount++;
		}
	}

    public void UpdateData()
    {
        Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null) rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.useGravity = false;

		BoxCollider collider = gameObject.GetComponent<BoxCollider>();
        if (IsSelfBoxCollider == true)
		{
			if (collider == null) collider = gameObject.AddComponent<BoxCollider>();
			
			collider.isTrigger = true;
			collider.size = new Vector3(TriggerLength, 1.0f, 5.0f);
		}
		else
		{
			if (collider != null)
				DestroyImmediate(collider);
		}
    }

    public void OnTriggerEnter(Collider other)
    {
		if (other.gameObject.layer != LayerMask.NameToLayer("PlayerBody"))
			return;
		
        if (isEnable == true && isActivate == false)
			OnActivate();
    }
	
	private void ResetMonsters()
	{
		//기존 몬스터 리스트에서 같은 녀석을 찾아서 지운다.
		foreach(GameObject actor in this.GenMercenaryList)
		{
			Destroy(actor, 0.1f);	
		}
	}
	
	private string GetMercenaryResource(MercenaryGenerateInfo.eMercenaryType type, int level)
	{
		string path = "NewAsset/";
		string fileName = "";
		
		switch(type)
		{
		case MercenaryGenerateInfo.eMercenaryType.GUARDIAN:
			path = "NewAsset/Mercenary/FatMan/";
			fileName = "Guardian";
			break;
		case MercenaryGenerateInfo.eMercenaryType.HUNTER:
			path = "NewAsset/Mercenary/SpearWoman/";
			fileName = "Hunter";
			break;
		}
		
		string resourceName = string.Format("{0}{1}_{2:0#}", path, fileName, level);
		return resourceName;		
	}
}
