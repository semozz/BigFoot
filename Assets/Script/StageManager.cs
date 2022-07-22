using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class TeleportTarget
{
    public GameObject target = null;
}

[System.Serializable]
public class TreasureBoxTarget
{
    public GameObject target = null;
}

[System.Serializable]
public class PlayerTargetInfo
{
	public enum ePlayerTargetType
	{
		None,
		Wave,
		Stage
	}
	public ePlayerTargetType type = ePlayerTargetType.None;
    public Transform target = null;
}

[System.Serializable]
public class WallInfo
{
	public float wallHeight = 10.0f;
	
	public GameObject wall = null;
}

[System.Serializable]
public class GroundInfo
{
	public enum eGroundType
	{
		Ground,
		Wall,
		Floor
	}
	public eGroundType type = eGroundType.Ground;
	public float fSize = 5;
	public GameObject groundObject = null;
	public Vector3 vOffset = Vector3.zero;
}

[System.Serializable]
public class StageManager : MonoBehaviour 
{
    public enum eStageType
    {
        ST_FIELD = 0,
        ST_TOWN,
		ST_WAVE,
		ST_TUTORIAL,
		ST_ARENA,
		ST_BOSSRAID,
		ST_EVENT,
    };
	
	public enum eDayMode
	{
		eDay,
		eNight,
	}
	public eDayMode dayMode = eDayMode.eDay;
	public Color dayColor = Color.white;
	public Color nightColor = new Color(200.0f/255.0f, 190.0f/255.0f, 255.0f/255.0f);
	
	public float ScreenHeight = 6.4f;
	public float ScreenWidth = 9.6f;
	
    public float NearStageZ = 5.0f;
    public float FarStageZ = 10.0f;
    public float FrontStageZ = -5.0f;
	public float MiddleStageZ = 7.0f;

    public eStageType StageType = eStageType.ST_FIELD;

    public int NearStageWBlockCount = 6;
    public int NearStageHBlockCount = 2;

    public int FarStageWBlockCount = 2;
    public int FarStageHBlockCount = 1;
	
	public int MiddleStageWBlockCount = 2;
    public int MiddleStageHBlockCount = 1;
	
	
	public float NearStageWidth = 0.0f;
	public float NearStageHeight = 0.0f;

    public float NearStageScrollHeight = 0.0f;

	public float FarStageWidth = 0.0f;
	public float FarStageHeight = 0.0f;
	
	public float MiddleStageWidth = 0.0f;
	public float MiddleStageHeight = 0.0f;

	public List<string> nearStageTextureNames = new List<string>();
	public List<string> farStageTextureNames = new List<string>();
	public List<string> middleStageTextureNames = new List<string>();
	public List<string> frontStageTexturenNames = new List<string>();
	
	public List<Texture> NearStageTextures = new List<Texture>();
	public List<Texture> FarStageTextures = new List<Texture>();
	
	public List<Texture> MiddleStageTextures = new List<Texture>();
    public List<Texture> FrontStageTextures = new List<Texture>();
	
	public float middleStageVScrollRate = 0.1f;
	public float farStageVScrollRate = 1.0f;
	
    public int GroundCount = 1;
    public Vector3 groundOffset = Vector3.zero;
    public List<GroundInfo> groundInfos = new List<GroundInfo>();
	
	public List<GroundInfo> wallInfos = new List<GroundInfo>();
	
	public int floorCount = 0;
	public List<GroundInfo> floorInfos = new List<GroundInfo>();
	
	public GroundInfo leftSideWall = new GroundInfo();
	public GroundInfo rightSideWall = new GroundInfo();
	
    public int GroundLayer = 0;
	public int FloorLayer = 0;
	
    public ScrollCamera StageCamera = null;
	public void SetScrollCamera(ScrollCamera camera)
	{
		this.StageCamera = camera;
		
		if (farStage != null)
			farStage.scrollCamera = camera;
		
		if (middleStage != null)
			middleStage.scrollCamera = camera;
	}
	
	public Transform PlayerTarget = null;
	public Transform ArenaTarget = null;	//보스 레이드용으로 변경..
	public List<PlayerTargetInfo> arenaTargetList = new List<PlayerTargetInfo>();
    public List<PlayerTargetInfo> playerTargetList = new List<PlayerTargetInfo>();
	
	public int stageBlockWidth = 10;
	public int stageBlockHeight = 10;
	
	public int farBlockWidth = 10;
	public int farBlockHeight = 10;
	
	public int middleBlockWidth = 10;
	public int middleBlockHeight = 10;
	
	public Transform towerTarget = null;
	
	public GameObject ghostObject = null;
	public GameObject princessObject = null;
	public GameObject tempNpcObject = null;
	
	
	public StageEndEvent stageEndEvent = null;
	public EventStep startDialogEvent = null;
	
	public int stageRewardID = -1;
	
	public UIRootPanel uiRootPanel = null;
	
	public string uiRootPrefab = "UI/UI Root (2D)";
	public string townMyInfoPrefab = "UI/MyStatusInfo_Town";
	public string townUIPrefab = "UI/TownUI";
	void Awake()
    {
		if (NearStageScrollHeight <= 0.0f)
            NearStageScrollHeight = NearStageHeight;
		
		switch(this.StageType)
		{
		case eStageType.ST_FIELD:
		case eStageType.ST_EVENT:
		case eStageType.ST_ARENA:
		case eStageType.ST_BOSSRAID:
		case eStageType.ST_WAVE:
		case eStageType.ST_TUTORIAL:
			GameObject uiRoot = ResourceManager.CreatePrefab(uiRootPrefab);
			if (uiRoot != null)
				uiRoot.transform.position = new Vector3(-21.0f, 0.0f, 0.0f);
			
			if (this.StageType == eStageType.ST_TUTORIAL)
			{
				PlayerControlButtonPanel playerControls = GameObject.FindObjectOfType(typeof(PlayerControlButtonPanel)) as PlayerControlButtonPanel;
				if (playerControls != null)
					playerControls.SetTutorialMode(-1);
			}
			break;
		}
		
    }
	
	public void UpdateStageTexture()
	{
		/*
		string path = "Theme1_Images/";
		
		BasicSprite[] sprites = this.gameObject.GetComponentsInChildren(typeof(BasicSprite));
		foreach(BasicSprite sprite in sprites)
		{
			sprite.SpriteTexture = 
		}
		*/
	}
	
	public void SetCamera()
	{
		ScrollCamera scrollCamera = GameObject.FindObjectOfType(typeof(ScrollCamera)) as ScrollCamera;
		SetScrollCamera(scrollCamera);
		
		//ActorManager actorManager = ActorManager.Instance;
		
		if (scrollCamera != null)
		{
			scrollCamera.Stage = this;
			
			PlayerController player = Game.Instance.player;
			if  (player != null)
				scrollCamera.player = player.moveController;
		}
	}
	
	private bool isStarted = false;
    void Start()
    {
		if (isStarted == true)
			return;
		
		UpdateStageAreaMinMaxInfo();
		
		Game.Instance.stageManager = this;
		Game.Instance.damageBossRaid = 0.0f;
		
		switch(this.StageType)
		{
		case eStageType.ST_ARENA:
			Transform arenaStartTarget = GetRandomArenaTarget();
			if (arenaStartTarget == null)
				arenaStartTarget = this.ArenaTarget;
			
			Game.Instance.CreateArenarCharacter(arenaStartTarget);
			break;
		case eStageType.ST_BOSSRAID:
			Game.Instance.CreateBossRaid(this.ArenaTarget, this.towerTarget);
			break;
		}
		
		Transform charStartTarget = GetPlayerTarget(PlayerTargetInfo.ePlayerTargetType.None);
		Vector3 charStartPos = Vector3.zero;
		if (charStartTarget != null)
			charStartPos = charStartTarget.position;
		Game.Instance.CreateCharacter(charStartPos);
		
		Component[] objs = this.gameObject.GetComponentsInChildren(typeof(ScrollPanel), false);
		foreach(Component temp in objs)
		{
			if (temp.name == "FarStage")
				this.farStage = (ScrollPanel)temp;
			else if (temp.name == "MiddleStage")
				this.middleStage = (ScrollPanel)temp;
		}
		
		SetCamera();
		
		UICamera uiCamera = GameObject.FindObjectOfType(typeof(UICamera)) as UICamera;
		if (uiCamera != null && uiCamera.camera != null)
		{
			uiCamera.camera.nearClipPlane = -10.0f;
			uiCamera.camera.farClipPlane = 10.0f;
		}
		
		uiRootPanel = GameObject.FindObjectOfType(typeof(UIRootPanel)) as UIRootPanel;
		stageEndEvent = GameObject.FindObjectOfType(typeof(StageEndEvent)) as StageEndEvent;
		
		EventStep[] stageEvents = GameObject.FindObjectsOfType(typeof(EventStep)) as EventStep[];
		foreach(EventStep tempEvent in stageEvents)
		{
			if (tempEvent != null && tempEvent.name == "StartDialogEvent")
			{
				startDialogEvent = tempEvent;
				break;
			}
		}
		
		if (stageEndEvent != null)
		{
			stageEndEvent.onComplete = new StageEndEvent.OnEvent(OnStageEndEventComplete);
			
			if (stageEndEvent.onStageFailed == null)
				stageEndEvent.onStageFailed = new StageEndEvent.OnEvent(OnStageFailed);
		}
		
		switch(this.StageType)
		{
		case eStageType.ST_TOWN:
			TownUI townUI = ResourceManager.CreatePrefab<TownUI>(townUIPrefab, uiRootPanel.transform, Vector3.zero);

			if (townUI != null)
			{
				townUI.popupNode = uiRootPanel.popUpNode;
				townUI.uiRoot = uiRootPanel.transform;

                GameObject myInfo = ResourceManager.CreatePrefab(townMyInfoPrefab, townUI.transform, new Vector3(-25.0f, 285.0f, 0.0f));
			}
			break;
		}
		
		MonsterGenerator.isSurrendMode = false;
		Game.Instance.ResetPause();
		
		if (startDialogEvent != null)
		{
			startDialogEvent.isRequireGamePause = true;
			startDialogEvent.OnActivate();
		}
		
		GroundLayer = LayerMask.NameToLayer("Ground");
		
		if (this.StageCamera == null)
			SetCamera();
		
		if (this.StageType == eStageType.ST_WAVE)
		{
			TowerInfo towerInfo = Game.Instance.selectedTowerInfo;
			CreateTower(towerInfo);
		}
		
		if (this.StageType == eStageType.ST_TOWN)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
				packetSender.SendRequestEnterTown();
		}
		//if (this.StageType != eStageType.ST_TOWN)
		//	Game.Instance.InitStageRewardItemList();
		
		CharInfoData charInfo = Game.Instance.charInfoData;
		if (charInfo != null)
			charInfo.InitDropInfos();
		
		if (bgmClip != null && GameOption.bgmToggle == true)
		{
			float bgmVolume = Game.Instance.bgmVolume;
			AudioManager.PlayBGM(bgmClip, bgmVolume, 1.0f);
		}
		
		isStarted = true;
    }
	
	public void SetCharacter(PlayerController player)
	{
		if (player != null)
		{
			Transform trans = GetPlayerTarget(PlayerTargetInfo.ePlayerTargetType.None);
			if (trans != null)
			{
				player.gameObject.transform.position = trans.position;
			}
			
			if (StageCamera != null)
			{
				StageCamera.player = player.moveController;
			}
		}
	}
	
	public List<string> gatePrefabPath = new List<string>();
	public void CreateTower(TowerInfo info)
	{
		if (info == null)
			return;
		
		int gateIndex = (int)info.type;
		string prefabPath = gatePrefabPath[gateIndex];
		
		GameObject towerPrefab = ResourceManager.LoadPrefab(prefabPath);
		if (towerPrefab == null)
			return;
		
		GameObject towerObj = (GameObject)Instantiate(towerPrefab);
		
		RaycastHit hitInfo;
		var layerMask = 1 << LayerMask.NameToLayer("Ground");
		Vector3 vStart = this.towerTarget.position + (Vector3.up * 2.0f);
		
		Vector3 createPos = vStart;
		if (Physics.Raycast(vStart, Vector3.down, out hitInfo, float.MaxValue, layerMask) == true)
		{
			createPos = hitInfo.point;
		}
		
		if (towerObj != null)
			towerObj.transform.position = createPos;
	}

	public void SetNearStageTextures (int count)
	{
		while (count > NearStageTextures.Count) NearStageTextures.Add (null);
		while (count < NearStageTextures.Count) NearStageTextures.RemoveAt (NearStageTextures.Count - 1);

        while (count > FrontStageTextures.Count) FrontStageTextures.Add(null);
        while (count < FrontStageTextures.Count) FrontStageTextures.RemoveAt(FrontStageTextures.Count - 1);
	}

    public void SetFarStageTextures(int count)
    {
        while (count > FarStageTextures.Count) FarStageTextures.Add(null);
        while (count < FarStageTextures.Count) FarStageTextures.RemoveAt(FarStageTextures.Count - 1);
    }
	
	public void SetMiddleStageTextures(int count)
    {
        while (count > MiddleStageTextures.Count) MiddleStageTextures.Add(null);
        while (count < MiddleStageTextures.Count) MiddleStageTextures.RemoveAt(MiddleStageTextures.Count - 1);
    }
	
	public void SetGroundInfos(int count)
    {
        while (count > groundInfos.Count) groundInfos.Add(new GroundInfo());
        while (count < groundInfos.Count) groundInfos.RemoveAt(groundInfos.Count - 1);
    }

    public void SetFloorInfos(int count)
    {
        while (count > floorInfos.Count) floorInfos.Add(new GroundInfo());
        while (count < floorInfos.Count) floorInfos.RemoveAt(floorInfos.Count - 1);
    }
	
    Texture GetNearStageTexture (int x, int y)
    {
        int npos = (y * NearStageWBlockCount) + x;
        if (npos < NearStageTextures.Count)
            return NearStageTextures[npos];

        return null;
    }

    Texture GetFrontStageTexture(int x, int y)
    {
        int npos = (y * NearStageWBlockCount) + x;
        if (npos < FrontStageTextures.Count)
            return FrontStageTextures[npos];

        return null;
    }

    Texture GetFarStageTexture(int x, int y)
    {
        int npos = (y * FarStageWBlockCount) + x;
        if (npos < FarStageTextures.Count)
            return FarStageTextures[npos];

        return null;
    }
	
	Texture GetMiddleStageTexture(int x, int y)
    {
        int npos = (y * MiddleStageWBlockCount) + x;
        if (npos < MiddleStageTextures.Count)
            return MiddleStageTextures[npos];

        return null;
    }
	
	public void ChangeFloorLayer(int layerValue)
	{
		foreach(GroundInfo info in floorInfos)
		{
			info.groundObject.layer = layerValue;
		}
	}
	
	public void ChangeGroundLayer(int layerValue)
	{
		foreach(GroundInfo info in groundInfos)
		{
			info.groundObject.layer = layerValue;
		}
	}
	
	public float tutorialDelayTime = 0.5f;
	public float tutorialTime = 0.5f;
	public void Update()
	{
		AudioManager.Update();
		
		if (StageType == eStageType.ST_TUTORIAL)
		{
			tutorialDelayTime -= Time.deltaTime;
			if (tutorialDelayTime <= 0.0f)
			{
				PlayerController player = Game.Instance.player;
				
				if (player != null)
				{
					player.AbilityFull();
					player.lifeManager.attributeManager.HpFull();
				}
				
				tutorialDelayTime = tutorialTime;
			}
		}
	}
	
	public void UpdateData ()
	{
        UpdateNearStages();
		
		UpdateMiddleStages();
        UpdateFarStages();
        UpdateGrounds();
		
		UpdateFloors();
		
		/*
        Vector3 vCamera = Vector3.zero;
        if (Camera.mainCamera != null)
            vCamera = Camera.mainCamera.transform.position;
		*/
		
        transform.position = Vector3.zero; //new Vector3(vCamera.x - ScreenWidth * 0.5f, vCamera.y - ScreenHeight * 0.5f, NearStageZ);

        if (NearStageScrollHeight <= 0.0f)
            NearStageScrollHeight = NearStageHeight;
	}

    void UpdateNearStages()
    {
        DestroyChildNode("NearStage");

        GameObject neargo = new GameObject("NearStage");
        neargo.layer = gameObject.layer;
        neargo.transform.Translate(gameObject.transform.position + new Vector3 (0.0f, 0.0f, NearStageZ));
        neargo.transform.parent = gameObject.transform;

        DestroyChildNode("FrontStage");

        GameObject frontgo = new GameObject("FrontStage");
        frontgo.layer = gameObject.layer;
        frontgo.transform.Translate(gameObject.transform.position + new Vector3 (0.0f, 0.0f, FrontStageZ));
        frontgo.transform.parent = gameObject.transform;

        Vector3 vOffset = Vector3.zero;
        for (int i = NearStageHBlockCount - 1; i >= 0; i--)
        {
            int BlockHeight = 0;
            for (int j = 0; j < NearStageWBlockCount; j++)
            {
                Texture tex = GetNearStageTexture(j, i);
                if (tex)
                {
                    BlockHeight = this.stageBlockHeight;//tex.height;
                    break;
                }
            }

            for (int j = 0; j < NearStageWBlockCount; j++)
            {
                Texture tex = null;
                int BlockWidth = 0;
                for (int k = 0; k < NearStageHBlockCount; k++)
                {
                    tex = GetNearStageTexture(j, k);
                    if (tex)
                    {
                        BlockWidth = this.stageBlockWidth;//tex.width;
                        break;
                    }
                }

                tex = GetNearStageTexture(j, i);
                if (tex)
                {
                    string texname = j + "X" + i;

                    GameObject go = new GameObject(texname);
                    go.layer = neargo.layer;
                    BasicSprite sprite = go.AddComponent<BasicSprite>();
                    sprite.SetData(tex, BlockWidth, BlockHeight, vOffset, Vector3.back);
                    go.transform.Translate(neargo.transform.position);
                    go.transform.parent = neargo.transform;
                }

                tex = GetFrontStageTexture(j, i);
                if (tex)
                {
                    string texname = j + "X" + i;

                    GameObject go = new GameObject(texname);
                    go.layer = frontgo.layer;
                    BasicSprite sprite = go.AddComponent<BasicSprite>();
                    sprite.SetData(tex, BlockWidth, BlockHeight, vOffset, Vector3.back);
                    go.transform.Translate(frontgo.transform.position);
                    go.transform.parent = frontgo.transform;
                }

                vOffset.x += BlockWidth;
            }
            NearStageWidth = vOffset.x;

            vOffset.x = 0.0f;
            vOffset.y += BlockHeight;
        }

        NearStageHeight = vOffset.y;
    }
	
	ScrollPanel farStage = null;
    void UpdateFarStages()
    {
        DestroyChildNode("FarStage");
		farStage = null;
		
        GameObject fargo = new GameObject("FarStage");

        farStage = fargo.AddComponent<ScrollPanel>();
		farStage.stageType = ScrollPanel.eStageType.FarStage;
        farStage.stageManager = this;
        farStage.scrollCamera = this.StageCamera;// (ScrollCamera)GameObject.FindObjectOfType(typeof(ScrollCamera));
		farStage.scrollVWeight = farStageVScrollRate;
		
        fargo.layer = gameObject.layer;
        fargo.transform.Translate(gameObject.transform.position + new Vector3 (0.0f, 0.0f, FarStageZ));
        fargo.transform.parent = gameObject.transform;

        Vector3 vOffset = Vector3.zero;
        for (int i = FarStageHBlockCount - 1; i >= 0; i--)
        {
            int BlockHeight = 0;
            for (int j = 0; j < FarStageWBlockCount; j++)
            {
                Texture tex = GetFarStageTexture(j, i);
                if (tex)
                {
                    BlockHeight = this.farBlockHeight;//tex.height;
                    break;
                }
            }

            for (int j = 0; j < FarStageWBlockCount; j++)
            {
                Texture tex = null;
                int BlockWidth = 0;
                for (int k = 0; k < FarStageHBlockCount; k++)
                {
                    tex = GetFarStageTexture(j, k);
                    if (tex)
                    {
                        BlockWidth = this.farBlockWidth;//tex.width;
                        break;
                    }
                }

                tex = GetFarStageTexture(j, i);
                if (tex)
                {
                    string texname = j + "X" + i;

                    GameObject go = new GameObject(texname);
                    go.layer = fargo.layer;
                    BasicSprite sprite = go.AddComponent<BasicSprite>();
                    sprite.SetData(tex, BlockWidth, BlockHeight, vOffset, Vector3.back);
                    go.transform.Translate(fargo.transform.position);
                    go.transform.parent = fargo.transform;
                }

                vOffset.x += BlockWidth;
            }
            FarStageWidth = vOffset.x;

            vOffset.x = 0.0f;
            vOffset.y += BlockHeight;
        }

        FarStageHeight = vOffset.y;
    }
	
	ScrollPanel middleStage = null;
	void UpdateMiddleStages()
    {
        DestroyChildNode("MiddleStage");
		middleStage = null;
		
        GameObject stageGameObject = new GameObject("MiddleStage");

        middleStage = stageGameObject.AddComponent<ScrollPanel>();
		middleStage.stageType = ScrollPanel.eStageType.MiddleStage;
        middleStage.stageManager = this;
        middleStage.scrollCamera = this.StageCamera;// (ScrollCamera)GameObject.FindObjectOfType(typeof(ScrollCamera));
		middleStage.scrollVWeight = middleStageVScrollRate;
		
        stageGameObject.layer = gameObject.layer;
        stageGameObject.transform.Translate(gameObject.transform.position + new Vector3 (0.0f, 0.0f, MiddleStageZ));
        stageGameObject.transform.parent = gameObject.transform;

        Vector3 vOffset = Vector3.zero;
        for (int i = MiddleStageHBlockCount - 1; i >= 0; i--)
        {
            int BlockHeight = 0;
            for (int j = 0; j < MiddleStageWBlockCount; j++)
            {
                Texture tex = GetMiddleStageTexture(j, i);
                if (tex)
                {
                    BlockHeight = this.middleBlockHeight;//tex.height;
                    break;
                }
            }

            for (int j = 0; j < MiddleStageWBlockCount; j++)
            {
                Texture tex = null;
                int BlockWidth = 0;
                for (int k = 0; k < MiddleStageHBlockCount; k++)
                {
                    tex = GetMiddleStageTexture(j, k);
                    if (tex)
                    {
                        BlockWidth = this.middleBlockWidth;//tex.width;
                        break;
                    }
                }

                tex = GetMiddleStageTexture(j, i);
                if (tex)
                {
                    string texname = j + "X" + i;

                    GameObject go = new GameObject(texname);
                    go.layer = stageGameObject.layer;
                    BasicSprite sprite = go.AddComponent<BasicSprite>();
                    sprite.SetData(tex, BlockWidth, BlockHeight, vOffset, Vector3.back);
                    go.transform.Translate(stageGameObject.transform.position);
                    go.transform.parent = stageGameObject.transform;
                }

                vOffset.x += BlockWidth;
            }
            MiddleStageWidth = vOffset.x;

            vOffset.x = 0.0f;
            vOffset.y += BlockHeight;
        }

        MiddleStageHeight = vOffset.y;
    }
	
	
	public void UpdateGroundOffset()
	{
		Transform ground = transform.FindChild ("Grounds");
		if (ground != null)
		{
			if (ground != null)
				ground.localPosition = this.groundOffset;	
		}
	}
    void UpdateGrounds()
    {
        GameObject GroundRoot = null;
        Transform t = transform.FindChild ("Grounds");
        if (t) GroundRoot = t.gameObject;
        else
        {
            GroundRoot = new GameObject("Grounds");
            GroundRoot.transform.parent = transform;
        }
		
		GroundRoot.transform.localPosition = groundOffset;
		
		GameObject groundListRoot = null;
		Transform root = GroundRoot.transform.FindChild("GroundList");
		if (root != null)
		{
			groundListRoot = root.gameObject;
		}
		else
		{
			groundListRoot = new GameObject("GroundList");
			groundListRoot.transform.parent = GroundRoot.transform;
		}
        
        BuildGrounds(groundListRoot, groundInfos);
		
		UpdateWalls();
		
		BuildEndWall(groundListRoot);
    }

    void BuildGrounds(GameObject GroupRoot, List<GroundInfo> infos)
    {
        if (GroupRoot == null || infos == null) return;

		int nCount = infos.Count;
		Vector3 vOffset = Vector3.zero;
		
		List<WayPointManager> listChilds = new List<WayPointManager>();
        listChilds.AddRange(GroupRoot.GetComponentsInChildren<WayPointManager>(true));
        
		for (int i = 0; i < nCount; ++i)
        {
			GroundInfo info = infos[i];
			
			WayPointManager wayPointManager = null;
			
            GameObject go = null;
			BoxCollider boxCollider = null;
			
			float groundLength = info.fSize;
			if (i == 0 || i == (nCount - 1))
				groundLength += 0.5f;
			else
				groundLength += (0.5f + 0.5f);
			
			Vector3 vCenter = Vector3.zero;
			Vector3 vSize = new Vector3(groundLength, 1.0f, 10.0f);
			
            if (info.groundObject)
			{
				go = info.groundObject;
				
				boxCollider = go.GetComponent<BoxCollider>();
				
				wayPointManager = go.GetComponent<WayPointManager>();
			}
            else
            {
				go = new GameObject();
                go.name = "ground " + i;
                info.groundObject = go;
				
				boxCollider = go.AddComponent<BoxCollider>();
				
				wayPointManager = go.AddComponent<WayPointManager>();
				wayPointManager.ownerGround = boxCollider;
            }
			
			if (boxCollider != null)
			{
				boxCollider.isTrigger = false;
				
				if (i == 0)
					vCenter.x = 0.5f * 0.5f;
				else if (i == (nCount - 1))
					vCenter.x = -(0.5f * 0.5f);
				
				boxCollider.center = vCenter;
				
				boxCollider.size = vSize;
			}
			
            go.layer = GroundLayer;
            go.transform.parent = GroupRoot.transform;
            
            go.transform.localPosition = new Vector3(info.fSize * 0.5f, -0.5f, 0.0f) + info.vOffset + vOffset;
			
			vOffset.x += info.fSize;
			
			if (wayPointManager != null)
				listChilds.Remove(wayPointManager);
        }
		
		foreach (WayPointManager child in listChilds)
            DestroyImmediate(child.gameObject);
    }
	
	public void UpdateWalls()
	{
		GameObject GroundRoot = null;
        Transform t = transform.FindChild ("Grounds");
        if (t) GroundRoot = t.gameObject;
        else
        {
            GroundRoot = new GameObject("Grounds");
            GroundRoot.transform.parent = transform;
        }
		
		GameObject listRoot = null;
		Transform root = GroundRoot.transform.FindChild("WallList");
		if (root != null)
		{
			DestroyImmediate(root.gameObject);
		}
		
		listRoot = new GameObject("WallList");
		listRoot.transform.parent = GroundRoot.transform;
		listRoot.transform.localPosition = Vector3.zero;
		
		wallInfos.Clear();
        BuildWalls(listRoot, groundInfos);
	}
	
	public void BuildWalls(GameObject root, List<GroundInfo> infos)
	{
		if (root == null || infos == null) return;

		int nCount = infos.Count;
		
		Vector3 vOffset =  Vector3.zero;
		
		float groundSize = 0.0f;
		
		for (int i = 0; i < nCount - 1; ++i)
        {
			GroundInfo left = infos[i];
			GroundInfo right = infos[i+1];
			
			bool leftUp = false;
			Vector3 leftGroundPos = left.groundObject.transform.localPosition;
			Vector3 rightGroundPos = right.groundObject.transform.localPosition;
			
			float diffY = leftGroundPos.y - rightGroundPos.y;
			
			float size = Mathf.Abs(diffY) - (0.5f + 0.5f);
			if (size <= 0.0f)
				size = 0.0f;
			
			groundSize += left.fSize;
			
			if (size == 0.0f)
				continue;
			
			if (diffY > 0.0f)
				leftUp = true;
			else
				leftUp = false;
			
			
            GameObject go = null;
			BoxCollider boxCollider = null;
			
			//size = size * 0.95f;
			Vector3 vSize = new Vector3(1.0f, Mathf.Abs(size), 10.0f);
			
        	go = new GameObject();
            go.name = "wall " + i;
			
			if (leftUp == true)
			{
				vOffset.x = /*this.groundOffset.x + */groundSize;// - 0.5f;
				vOffset.y = /*this.groundOffset.y + */leftGroundPos.y - (size * 0.5f) - 0.5f;
			}
			else
			{
				vOffset.x = /*this.groundOffset.x + */groundSize;// + 0.5f;
				vOffset.y = /*this.groundOffset.y + */leftGroundPos.y + (size * 0.5f) + 0.5f;
			}
			
			boxCollider = go.AddComponent<BoxCollider>();
            
			if (boxCollider != null)
			{
				boxCollider.isTrigger = false;
				
				boxCollider.size = vSize;
			}
			
            go.layer = GroundLayer;
            go.transform.parent = root.transform;
            
            go.transform.localPosition = vOffset;
			
			GroundInfo newWallInfo = new GroundInfo();
			newWallInfo.fSize = size;
			newWallInfo.groundObject = go;
			
			wallInfos.Add(newWallInfo);
        }
	}
	
	public void BuildEndWall(GameObject groupRoot)
	{
		if (groupRoot == null || leftSideWall == null || rightSideWall == null) return;
		
		GameObject go = null;
		BoxCollider boxCollider = null;
		
		
		GameObject firstGround = null;
		GameObject lastGround = null;
		int groundCount = groundInfos.Count;
		float stageSize = 0.0f;
		for (int i = 0; i < groundCount; ++i)
		{
			GroundInfo info = groundInfos[i];
			if (i == 0)
				firstGround = info.groundObject;
			
			if (i == groundCount - 1)
				lastGround = info.groundObject;
			
			stageSize += info.fSize;
		}
		
		
		Vector3 vSize = new Vector3(1.0f, leftSideWall.fSize, 10.0f);
		
        if (leftSideWall.groundObject)
		{
			go = leftSideWall.groundObject;
			
			boxCollider = go.GetComponent<BoxCollider>();
		}
        else
        {
			go = new GameObject();
            go.name = "LeftWall";
            leftSideWall.groundObject = go;
			
			boxCollider = go.AddComponent<BoxCollider>();
        }
		
		if (boxCollider != null)
		{
			boxCollider.isTrigger = false;
			
			boxCollider.size = vSize;
		}
		
		go.layer = GroundLayer;
		go.transform.parent = groupRoot.transform;
		
		float xPos = leftSideWall.vOffset.x - 0.5f;
		float yPos = leftSideWall.fSize * 0.5f + firstGround.transform.localPosition.y + 0.5f;
		go.transform.localPosition = new Vector3(xPos, yPos, 0.0f);
		
		
		
		vSize = new Vector3(1.0f, rightSideWall.fSize, 10.0f);
		if (rightSideWall.groundObject)
		{
			go = rightSideWall.groundObject;
			
			boxCollider = go.GetComponent<BoxCollider>();
		}
        else
        {
			go = new GameObject();
            go.name = "RightWall";
            rightSideWall.groundObject = go;
			
			boxCollider = go.AddComponent<BoxCollider>();
        }
		
		if (boxCollider != null)
		{
			boxCollider.isTrigger = false;
			
			boxCollider.size = vSize;
		}
		
		go.layer = GroundLayer;
		go.transform.parent = groupRoot.transform;
		
		xPos = stageSize + rightSideWall.vOffset.x + 0.5f;
		yPos = rightSideWall.fSize * 0.5f + lastGround.transform.localPosition.y + 0.5f;
		go.transform.localPosition = new Vector3(xPos, yPos, 0.0f);
	}

    Vector3 GetMeshSize(Transform t, Mesh m)
    {
        if (t == null || m == null) return Vector3.zero;

        Bounds b = m.bounds;

        Vector3 v1 = b.center - b.extents;
        Vector3 v2 = b.center + b.extents;

        v1 = t.TransformPoint(v1);
        v2 = t.TransformPoint(v2);

        Vector3 v = Vector3.zero;

        v.x = Mathf.Abs(v2.x - v1.x);
        v.y = Mathf.Abs(v2.y - v1.y);
        v.z = Mathf.Abs(v2.z - v1.z);

        return v;
    }

	void UpdateFloors()
    {
		GameObject GroundRoot = null;
        Transform t = transform.FindChild ("Grounds");
        if (t) GroundRoot = t.gameObject;
        else
        {
            GroundRoot = new GameObject("Grounds");
            GroundRoot.transform.parent = transform;
        }
		
        GameObject FloorRoot = null;

        Transform temp = GroundRoot.transform.Find("Floors");
        if (temp) FloorRoot = temp.gameObject;
        else
        {
            FloorRoot = new GameObject("Floors");
            FloorRoot.transform.parent = GroundRoot.transform;
        }
		
		//FloorRoot.transform.localPosition = groundOffset;

        List<WayPointManager> listChilds = new List<WayPointManager>();
        listChilds.AddRange(FloorRoot.GetComponentsInChildren<WayPointManager>(true));
        
        //Vector3 vOffset = Vector3.zero;
        for (int i = 0; i < floorInfos.Count; i++)
        {
            GroundInfo info = floorInfos[i];
			
			Vector3 vSize = new Vector3(info.fSize, 1.0f, 10.0f);
			
			BoxCollider boxCollider = null;
			
            GameObject go = null;
			
			WayPointManager wayPointManager = null;
			
            if (info.groundObject) 
			{
				go = info.groundObject;
				
				boxCollider = go.GetComponent<BoxCollider>();
				
				wayPointManager = go.GetComponent<WayPointManager>();
			}
            else
            {
                go = new GameObject("floor " + i);
                info.groundObject = go;
				
				go.transform.parent = FloorRoot.transform;
				
				//Box Collider의 isTrigger설정.
				boxCollider = go.AddComponent<BoxCollider>();
				
				wayPointManager = go.AddComponent<WayPointManager>();
				wayPointManager.ownerGround = boxCollider;
            }
			
			if (boxCollider != null)
			{
				boxCollider.isTrigger = false;
				boxCollider.size = vSize;
			}
		
            go.layer = this.FloorLayer;
            go.transform.localPosition = info.vOffset;
			
			if (wayPointManager != null)
            	listChilds.Remove(wayPointManager);
        }

        foreach (WayPointManager child in listChilds)
            DestroyImmediate(child.gameObject);
    }
	
    bool DestroyChildNode(string nodename)
    {
        Transform t = transform.Find(nodename);
        if (t)
        {
            DestroyImmediate(t.gameObject);
            return true;
        }

        return false;
    }
	
	public List<TeleportTarget> teleportTargetList = new List<TeleportTarget>();
	private int teleportIndex = 0;
	public GameObject GetTeleportTarget()
	{
		int nCount = teleportTargetList.Count;
		if (nCount < 1 || nCount <= teleportIndex)
			return null;
		
		TeleportTarget teleport = teleportTargetList[teleportIndex];
		teleportIndex++;
		
		return teleport.target;
	}
	
	public void AddEmptyTeleportTarget()
	{
		teleportTargetList.Add(new TeleportTarget());	
	}
	
	public List<TreasureBoxTarget> treasureBoxTargetList = new List<TreasureBoxTarget>();
	public float treasureBoxRate = 0.1f;
	public void AddEmptyTreasureBoxTarget()
	{
		treasureBoxTargetList.Add(new TreasureBoxTarget());	
	}
	
	public void CreateTreasureBoxs()
	{
		int rate = (int)Mathf.Round(this.treasureBoxRate * 100.0f);
		
		foreach(TreasureBoxTarget info in treasureBoxTargetList)
		{
			if (info.target == null)
				continue;
			
			int randValue = Random.Range(0, 100);
			if (randValue <= rate)
			{
				//GameObject obj = (GameObject)Instantiate(Resources.Load("NPC/Gold_Box/Gold_Box_prefab"));
				GameObject obj = ResourceManager.CreatePrefab("NPC/Gold_Box/Gold_Box_prefab");
				if (obj != null)
					obj.transform.position = info.target.transform.position;
			}
		}
	}
	
	
	public void AddEmptyPlayerTarget()
	{
		this.playerTargetList.Add(new PlayerTargetInfo());	
	}
	public Transform GetPlayerTarget(PlayerTargetInfo.ePlayerTargetType type)
	{
		Transform trans = null;
		
		switch(this.StageType)
		{
		case eStageType.ST_TOWN:
			foreach(PlayerTargetInfo info in playerTargetList)
			{
				if (info != null && info.type == type)
				{
					trans = info.target;
					break;
				}
			}
			
			if (trans == null)
				trans = this.PlayerTarget;
			
			break;
		default:
			trans = this.PlayerTarget;
			break;
		}
		
		return trans;
	}
	
	public void AddEmptyArenaTarget()
	{
		this.arenaTargetList.Add(new PlayerTargetInfo());	
	}
	public Transform GetRandomArenaTarget()
	{
		Transform trans = null;
		int nCount = arenaTargetList.Count;
		int randIndex = -1;
		if (nCount > 0)
			randIndex = Random.Range(0, nCount);
		
		PlayerTargetInfo tempInfo = null;
		if (randIndex >= 0 && randIndex < nCount)
			tempInfo = arenaTargetList[randIndex];
		
		if (tempInfo != null)
			trans = tempInfo.target;
		
		return trans;
	}
	
	public void SetJumpCollider(Collider collider, bool bIgnore)
	{
		Collider collider2 = null;
		Vector3 floorPos = Vector3.zero;
		Vector3 colliderPos = collider.gameObject.transform.position;
		
		foreach(GroundInfo info in floorInfos)
		{
			if (info.groundObject != null)
			{
				collider2 = info.groundObject.GetComponent<Collider>();
				
				floorPos = info.groundObject.transform.position;
			}
			
			if (colliderPos.y < floorPos.y && bIgnore == false)
			{
				//Debug.Log("Collider Pos : " + colliderPos + "  floor Pos : " + floorPos);
				continue;
			}
			
			Physics.IgnoreCollision(collider, collider2, bIgnore);
		}
	}
	
	public void SetProjectileCollider(Collider projectile, Collider myGround, Collider targetGround)
	{
		Collider tempCollider = null;
		
		//내가 있는 Ground와는 발사체 충돌 안함..
		if (myGround != null && projectile != null)
			Physics.IgnoreCollision(projectile, myGround, true);
		
		foreach(GroundInfo info1 in floorInfos)
		{
			if (info1.groundObject != null)
				tempCollider = info1.groundObject.GetComponent<Collider>();
			
			Physics.IgnoreCollision(projectile, tempCollider, true);
		}
		
		/*
		foreach(GroundInfo info2 in groundInfos)
		{
			if (info2.groundObject != null)
				tempCollider = info2.groundObject.GetComponent<Collider>();
			
			Physics.IgnoreCollision(projectile, tempCollider, true);
		}
		*/
		
		/*
		foreach(GroundInfo info3 in wallInfos)
		{
			if (info3.groundObject != null)
				tempCollider = info3.groundObject.GetComponent<Collider>();
			
			Physics.IgnoreCollision(projectile, tempCollider, true);
		}
		*/
		//목표가 있는 Ground와는 발사체 충돌 안함..
		Physics.IgnoreCollision(projectile, targetGround, false);
	}
	
	public void FindGrounds(List<GameObject> groundList)
	{
		foreach(GroundInfo info1 in groundInfos)
		{
			groundList.Add(info1.groundObject);
		}
		
		foreach(GroundInfo info2 in floorInfos)
		{
			groundList.Add(info2.groundObject);
		}
	}
	
	public void OnStageEndEventComplete()
	{
		if (Game.Instance != null)
			Game.Instance.SetPlayerSuperArmorMode(Game.Instance.player);
		
		switch(this.StageType)
		{
		case eStageType.ST_TUTORIAL:
		case eStageType.ST_FIELD:	
		case eStageType.ST_EVENT:
			if (stageEndEvent != null && stageRewardID != -1)
				stageEndEvent.MakeRewardItems(stageRewardID);
			break;
		case eStageType.ST_WAVE:
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eDefenceComplete, 0);
			
			if (stageEndEvent != null && stageRewardID != -1)
				stageEndEvent.MakeRewardItems(stageRewardID);
			break;
		case eStageType.ST_ARENA:
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eArenaWin, 0);
			
			if (stageEndEvent != null)
				stageEndEvent.OnArenaEnd(true);
			break;
		case eStageType.ST_BOSSRAID:
			if (stageEndEvent != null)
				stageEndEvent.OnBossRaidEnd();
			break;
		}
	}
	
	public void OnStageFailed()
	{
		Game.Instance.Pause = true;
		
		switch(this.StageType)
		{
		case eStageType.ST_FIELD:	
		case eStageType.ST_EVENT:
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eStageFailed, 0);
			if (stageEndEvent != null)
				stageEndEvent.OnStageFaileUI();
			break;
		case eStageType.ST_WAVE:
			break;
		case eStageType.ST_ARENA:
			Game.Instance.ApplyAchievement(Achievement.eAchievementType.eArenaFail, 0);
			
			if (stageEndEvent != null)
				stageEndEvent.OnArenaEnd(false);
			break;
		case eStageType.ST_BOSSRAID:
			if (stageEndEvent != null)
			{
				stageEndEvent.OnBossRaidEnd();
				
				stageEndEvent.CreateLoadingPanel();
			}
			break;
		}
	}
	
	public List<MonsterGenerator> bossRaidMonsterGenerators = new List<MonsterGenerator>();
	public void ActivateMonsterGenerator(int attributeID)
	{
		foreach(MonsterGenerator temp in bossRaidMonsterGenerators)
		{
			if (temp.monsterLinkID == attributeID)
				temp.OnActivate();
		}
	}
	
	public void DeactivateMonsterGenerator()
	{
		foreach(MonsterGenerator temp in bossRaidMonsterGenerators)
		{
			temp.OnDeactivate();
		}
	}
	
	public void AddEmptyMonsterGenerator()
	{
		bossRaidMonsterGenerators.Add(null);	
	}
	
	public List<MonsterGenerator> bossRaidPhase2MonsterGenerators = new List<MonsterGenerator>();
	public void ActivateMonsterGeneratorByPhase2(int attributeID)
	{
		DeactivateMonsterGenerator();
		
		foreach(MonsterGenerator temp in bossRaidPhase2MonsterGenerators)
		{
			if (temp.monsterLinkID == attributeID)
				temp.OnActivate();
		}
	}
	
	public void AddEmptyMonsterGeneratorByPhase2()
	{
		bossRaidPhase2MonsterGenerators.Add(null);	
	}
	
	public AudioClip bgmClip = null;
	public AudioClip bossBGMClip = null;
	public void OnDestroy()
	{
		StopBGM();
	}
	
	public void StartBossBGM()
	{
		if (this.StageType == eStageType.ST_FIELD
			|| this.StageType == eStageType.ST_EVENT)
		{
			if (bossBGMClip != null && GameOption.bgmToggle == true)
			{
				float bgmVolume = Game.Instance.bgmVolume;
				
				AudioManager.StopBGM();
				AudioManager.PlayBGM(bossBGMClip, bgmVolume, 1.0f);
			}
		}
	}
	
	public void StopBGM()
	{
		AudioManager.StopBGM();
		//AudioManager.StopBGM(bossBGMClip);
	}
	
	public float stageAreaMinX = 0.0f;
	public float stageAreaMaxX = 0.0f;
	public float stageAreaMinY = 0.0f;
	public float stageAreaMaxY = 0.0f;
	
	public void UpdateStageAreaMinMaxInfo()
	{
		Vector3 position = this.transform.position;
		
		if (this.StageType == eStageType.ST_TOWN)
		{
			this.NearStageWidth = 5.0f;
			this.NearStageHeight = 5.0f;
		}
		
		stageAreaMinX = position.x;
		stageAreaMaxX = stageAreaMinX + this.NearStageWidth;
		
		stageAreaMinY = position.y;
		stageAreaMaxY = stageAreaMinY + this.NearStageHeight;
	}
}
