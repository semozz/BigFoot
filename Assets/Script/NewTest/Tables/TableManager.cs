using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Text;

public class BaseTable
{
	public void LoadTable(TableManager tableMgr, string tableName)
	{
		CSVDB db = null;
		if (tableMgr != null)
			db = tableMgr.GetDB(tableName);
		
		LoadTable(db);
	}
	
	public void LoadTable(TableManager tableMgr, string tableName, AssetBundle assetBundle)
	{
		CSVDB db = null;
		if (tableMgr != null)
			db = tableMgr.GetDB(tableName, assetBundle);
		
		LoadTable(db);
	}
	
	public virtual void LoadTable(CSVDB db)
	{
		
	}
	
	
	public void LoadTableFromXML(TextAsset textAsset)
	{
		if (textAsset != null)
		{
			XmlTextReader textReader = new XmlTextReader(new System.IO.StringReader(textAsset.text));
			XmlDocument xmlDoc = new XmlDocument();
			
			if (xmlDoc != null && textReader != null)
			{
				xmlDoc.Load(textReader);
			
				LoadTableFromXML (xmlDoc);
			}
			
			if (textReader != null)
				textReader.Close();
		}
	}
	
	public void LoadTableFromXML(TableManager tableMgr, string filePath)
	{
		TextAsset textAsset = (TextAsset)Resources.Load(filePath, typeof(TextAsset));
		
		if (textAsset != null)
		{
			LoadTableFromXML(textAsset);
		}
		else
		{
			string infoStr = string.Format("{0} File not Found!!!", filePath);
			Debug.LogWarning(infoStr);
		}
		
	}
	
	public void LoadTableFromXML(TableManager tableMgr, string xmlName, AssetBundle assetBundle)
	{
		TextAsset textAsset = null;
		
		if (assetBundle != null)
			textAsset = (TextAsset)assetBundle.Load(xmlName, typeof(TextAsset));
		
		if (textAsset != null)
		{
			LoadTableFromXML(textAsset);
		}
		else
		{
			string infoStr = string.Format("AssetBundle {0} doesn't contain asset {1}", assetBundle.name, xmlName);
			Debug.LogWarning(infoStr);
		}
	}
	
	public virtual void LoadTableFromXML(XmlDocument xmlDoc)
	{
		
	}
}

public class TableManager  {
	private static TableManager ms_Instance = null;
	
	public bool bLoaded = false;
	
#if UNITY_EDITOR
	public static bool UseAssetBundle = false;
#else
	public static bool UseAssetBundle = true;
#endif
	
	public static TableManager Instance
	{
		get
		{
			if (ms_Instance == null)
			{
				ms_Instance = new TableManager();
				
				if (UseAssetBundle == false)
					ms_Instance.LoadTables();
			}
			
			return ms_Instance;
		}
	}
	
	public Dictionary<string, CSVDB> tableList = new Dictionary<string, CSVDB>();
	
	public CSVDB GetDB(string tableName)
	{
		CSVDB selectDB = null;
		
		if (tableList.ContainsKey(tableName) == false)
		{
			string path = string.Format("NewAsset/Tables/{0}.txt", tableName);
			TextAsset text = ResourceManager.LoadTextAsset(path);
			
			CSVDB db = new CSVDB();
			db.ReadCSVFile(text.text);
			
			tableList.Add(tableName, db);
			
			selectDB = db;
		}
		else
			selectDB = tableList[tableName];
		
		return selectDB;
	}
	
	public CSVDB GetDB(string tableName, AssetBundle assetBundle)
	{
		CSVDB selectDB = null;
		
		if (tableList.ContainsKey(tableName) == false)
		{
			//string path = "Tables/" + tableName;
			TextAsset text = (TextAsset)assetBundle.Load(tableName, typeof(TextAsset));
			if (text != null)
			{
				CSVDB db = new CSVDB();
				db.ReadCSVFile(text.text);
				
				tableList.Add(tableName, db);
				
				selectDB = db;
			}
			else
			{
				string infoStr = string.Format("AssetBundle {0} doesn't contain asset {1}", assetBundle.name, tableName);
				Debug.LogWarning(infoStr);
			}
		}
		else
			selectDB = tableList[tableName];
		
		return selectDB;
	}
	
	public AttributeInitTable attributeInitTable = null;
	public AttributeInitTable attributeIncTable = null;
	
	public StringTable stringTable = null;
	
	public ItemTable itemTable = null;
	public StageRewardTable stageRewardTable = null;
	
	public WeaponTable weaponTable = null;
	
	public CharExpTable charExpTable = null;
	public CharExpTable awakenExpTable = null;
	
	public ShopTable shopNormalTable = null;
	public ShopTable shopCostumeTable = null;
	public ShopTable shopCashTable = null;
	public ShopTable shopArtifactTable = null;
	
	public ReinforceMaterialTable reinforceMaterialTable = null;
	public MonsterDropTable monsterDropTable = null;
	public CostumeTable costumeTable = null;

	//public GambleItemTable gambleItemTable = null;
	public GambleItemTable gambleItem_A_Grade = null;
	public GambleItemTable gambleItem_B_Grade = null;
	public GambleSGradeList gambleSGradeTable = null;
	
	public GambleItemGradeRateTable gambleItemGradeRateTable = null;
	public GambleSelectRateTable gambleSelectRateTable = null;
	public GambleItemRateTable gambleItemRateTable = null;
	
	public WaveStartInfo waveStartInfo = null;
	//public CashShopInfo cashShopInfoTable = null;
	public CashShopInfoTable cashShopInfoTable = null;
	public StringValueTable stringValueTable = null;
	
	public StageTable stageTable = null;
	
	public MasteryInfoTable masteryInfoTable = null;
	public CharacterMasteryTable charMasteryTable = null;
	public MasteryUITable masteryUITable = null;
	
	public ArenaRewardInfoTable arenaRewardInfo = null;
	public WaveRewardInfoTable waveRewardInfo = null;
	
	public SetItemTable setItemInfo = null;
	
	public AchievementTable normalAchievementTable = null;
	public AchievementTable dailyAchievementTable = null;
	public AchievementTable specialEventAchievementTable = null;
	
	public MonsterPictureBookTable monsterPictureBookTable = null;
	
	public BossRaidTable bossRaidTable = null;
	
	public ToolTipTable tooltipsTable = null;
	
	public GamblePriceTable gamblePriceTable = null;
	
	public CostumeSetItemTable costumeSetItemTable = null;
	public ShopCostumeSetTable shopCostumeSetTable = null;
	
	public AwakeningLevelInfoTable awakeningLevelInfoTable = null;
	
	public TimeLimitItemTable timeLimitItemTable = null;
	
	public ItemReinforceInfoTable itemReinforceInfoTable = null;
	
	public ArenaItemRateTable arenaItemRateTable = null;
	
	public void LoadTables()
	{
		if (this.bLoaded == true)
			return;
		
		this.bLoaded = true;
		
		stringValueTable = new StringValueTable();
		stringValueTable.LoadTable(this, "StringValueTable");

		stringTable = new StringTable();
		stringTable.LoadTable(this, "StringTable");
		
		normalAchievementTable = new AchievementTable();
		normalAchievementTable.LoadTable(this, "AchievementTable");
		
		dailyAchievementTable = new AchievementTable();
		dailyAchievementTable.LoadTable(this, "DailyMissionTable");
		
		specialEventAchievementTable = new AchievementTable();
		specialEventAchievementTable.LoadTable(this, "SpecialMissionTable");
		
		attributeInitTable = new AttributeInitTable();
		attributeInitTable.LoadTable(this, "AttributeInitTable");
		
		attributeIncTable = new AttributeInitTable();
		attributeIncTable.LoadTable(this, "AttributeIncTable");
		
		itemTable = new ItemTable();
		itemTable.LoadTable(this, "ItemTable");
		
		stageRewardTable = new StageRewardTable();
		stageRewardTable.LoadTable(this, "StageCompensationTable");
		
		weaponTable = new WeaponTable();
		weaponTable.LoadTable(this, "WeaponTable");
		
		charExpTable = new CharExpTable();
		charExpTable.LoadTable(this, "ExpTable");
		
		awakenExpTable = new CharExpTable();
		awakenExpTable.LoadTable(this, "AwakenExpTable");
		
		reinforceMaterialTable = new ReinforceMaterialTable();
		reinforceMaterialTable.LoadTable(this, "ReinforceMaterialTable");
		
		monsterDropTable = new MonsterDropTable();
		monsterDropTable.LoadTable(this, "MonsterDropTable");
		
		costumeTable = new CostumeTable();
		costumeTable.LoadTable(this, "CostumeTable");
		
		//gambleItemTable = new GambleItemTable();
		//gambleItemTable.LoadTable(this, "GambleTable");
		
		gambleItem_A_Grade = new GambleItemTable();
		gambleItem_A_Grade.LoadTable(this, "GambleTable_A");
		
		gambleItem_B_Grade = new GambleItemTable();
		gambleItem_B_Grade.LoadTable(this, "GambleTable_B");
		
		gambleSGradeTable = new GambleSGradeList();
		gambleSGradeTable.LoadTable(this, "GambleTable_S");
		
		gambleItemGradeRateTable = new GambleItemGradeRateTable();
		gambleItemGradeRateTable.LoadTable(this, "GambleItemGradeRateTable");
		
		gambleItemRateTable = new GambleItemRateTable();
		gambleItemRateTable.LoadTable(this, "GambleTable_Rate");
		/*
		gambleSelectRateTable = new GambleSelectRateTable();
		gambleSelectRateTable.LoadTable(this, "GambleSelectRateTable");
		*/
		
		waveStartInfo = new WaveStartInfo();
		waveStartInfo.Load("WaveStartInfo");
		
		//cashShopInfoTable = new CashShopInfo();
		//cashShopInfoTable.Load("Tables/CashShopInfo");
				
		cashShopInfoTable = new CashShopInfoTable();
		cashShopInfoTable.LoadTable(this, "CashShopInfoTable");		
		
		stageTable = new StageTable();
		stageTable.LoadTable(this, "StageTable");
		
		masteryInfoTable = new MasteryInfoTable();
		masteryInfoTable.LoadTable(this, "MasteryInfoTable");
		
		charMasteryTable = new CharacterMasteryTable();
		charMasteryTable.LoadTable(this, "CharMasteryTable");
		
		masteryUITable = new MasteryUITable();
		masteryUITable.Load("MateryUIInfo");
		
		arenaRewardInfo = new ArenaRewardInfoTable();
		arenaRewardInfo.LoadTable(this, "ArenaWeeklyReward");
		
		waveRewardInfo = new WaveRewardInfoTable();
		waveRewardInfo.Load("WaveRewardInfo");
		
		setItemInfo = new SetItemTable();
		setItemInfo.Load("SetItemTable");
		
		monsterPictureBookTable = new MonsterPictureBookTable();
		monsterPictureBookTable.LoadTable(this, "MonsterPictureBookTable");
		
		bossRaidTable = new BossRaidTable();
		bossRaidTable.LoadTable(this, "BossRaidTable");
		
		tooltipsTable = new ToolTipTable();
		tooltipsTable.LoadTable(this, "ToolTips");
		
		gamblePriceTable = new GamblePriceTable();
		gamblePriceTable.LoadTable(this, "GamblePriceTable");
		
		costumeSetItemTable = new CostumeSetItemTable();
		costumeSetItemTable.LoadTable(this, "CostumeSetItemTable");
		
		shopCostumeSetTable = new ShopCostumeSetTable();
		shopCostumeSetTable.LoadTable(this, "ShopCostumeSetItemTable");
		
		shopNormalTable = new ShopTable();
		shopNormalTable.itemRateStep = 2;
		shopNormalTable.LoadTable(this, "ShopNormalTable");
		
		shopCostumeTable = new ShopTable();
		shopCostumeTable.LoadTable(this, "ShopCostumeTable");
		
		shopArtifactTable = new ShopTable();
		shopArtifactTable.itemRateStep = 3;
		shopArtifactTable.LoadTable(this, "ShopArtifactTable");
		
		awakeningLevelInfoTable = new AwakeningLevelInfoTable();
		awakeningLevelInfoTable.LoadTable(this, "AwakenSkillInfoTable");
		
		timeLimitItemTable = new TimeLimitItemTable();
		timeLimitItemTable.LoadTable(this, "TimeLimitItemTable");
		
		itemReinforceInfoTable = new ItemReinforceInfoTable();
		itemReinforceInfoTable.LoadTable(this, "ItemReinforceTable");
		
		arenaItemRateTable = new ArenaItemRateTable();
		arenaItemRateTable.LoadTable(this, "ArenaItemRateTable");
	}
	
	public void LoadTables(AssetBundle assetBundle)
	{
		if (this.bLoaded == false)
		{
			this.bLoaded = true;
			
			stringValueTable = new StringValueTable();
			stringValueTable.LoadTable(this, "StringValueTable", assetBundle);
	
			stringTable = new StringTable();
			stringTable.LoadTable(this, "StringTable", assetBundle);
			
			normalAchievementTable = new AchievementTable();
			normalAchievementTable.LoadTable(this, "AchievementTable", assetBundle);
			
			dailyAchievementTable = new AchievementTable();
			dailyAchievementTable.LoadTable(this, "DailyMissionTable", assetBundle);
			
			specialEventAchievementTable = new AchievementTable();
			specialEventAchievementTable.LoadTable(this, "SpecialMissionTable", assetBundle);
			
			attributeInitTable = new AttributeInitTable();
			attributeInitTable.LoadTable(this, "AttributeInitTable", assetBundle);
			
			attributeIncTable = new AttributeInitTable();
			attributeIncTable.LoadTable(this, "AttributeIncTable", assetBundle);
			
			
			itemTable = new ItemTable();
			itemTable.LoadTable(this, "ItemTable", assetBundle);
			
			stageRewardTable = new StageRewardTable();
			stageRewardTable.LoadTable(this, "StageCompensationTable", assetBundle);
			
			weaponTable = new WeaponTable();
			weaponTable.LoadTable(this, "WeaponTable", assetBundle);
			
			charExpTable = new CharExpTable();
			charExpTable.LoadTable(this, "ExpTable", assetBundle);
			
			awakenExpTable = new CharExpTable();
			awakenExpTable.LoadTable(this, "AwakenExpTable", assetBundle);
			
			reinforceMaterialTable = new ReinforceMaterialTable();
			reinforceMaterialTable.LoadTable(this, "ReinforceMaterialTable", assetBundle);
			
			monsterDropTable = new MonsterDropTable();
			monsterDropTable.LoadTable(this, "MonsterDropTable", assetBundle);
			
			costumeTable = new CostumeTable();
			costumeTable.LoadTable(this, "CostumeTable", assetBundle);
			
			//gambleItemTable = new GambleItemTable();
			//gambleItemTable.LoadTable(this, "GambleTable", assetBundle);
			gambleItem_A_Grade = new GambleItemTable();
			gambleItem_A_Grade.LoadTable(this, "GambleTable_A", assetBundle);
			
			gambleItem_B_Grade = new GambleItemTable();
			gambleItem_B_Grade.LoadTable(this, "GambleTable_B", assetBundle);
			
			gambleSGradeTable = new GambleSGradeList();
			gambleSGradeTable.LoadTable(this, "GambleTable_S", assetBundle);
			
			gambleItemGradeRateTable = new GambleItemGradeRateTable();
			gambleItemGradeRateTable.LoadTable(this, "GambleItemGradeRateTable", assetBundle);
			
			gambleItemRateTable = new GambleItemRateTable();
			gambleItemRateTable.LoadTable(this, "GambleTable_Rate", assetBundle);
			
			waveStartInfo = new WaveStartInfo();
			waveStartInfo.LoadTableFromXML(this, "WaveStartInfo", assetBundle);
			
			cashShopInfoTable = new CashShopInfoTable();
			cashShopInfoTable.LoadTable(this, "CashShopInfoTable", assetBundle);		
			
			stageTable = new StageTable();
			stageTable.LoadTable(this, "StageTable", assetBundle);
			
			masteryInfoTable = new MasteryInfoTable();
			masteryInfoTable.LoadTable(this, "MasteryInfoTable", assetBundle);
			
			charMasteryTable = new CharacterMasteryTable();
			charMasteryTable.LoadTable(this, "CharMasteryTable", assetBundle);
			
			masteryUITable = new MasteryUITable();
			masteryUITable.LoadTableFromXML(this, "MateryUIInfo", assetBundle);
			
			arenaRewardInfo = new ArenaRewardInfoTable();
			arenaRewardInfo.LoadTable(this, "ArenaWeeklyReward", assetBundle);
			
			waveRewardInfo = new WaveRewardInfoTable();
			waveRewardInfo.LoadTableFromXML(this, "WaveRewardInfo", assetBundle);
			
			setItemInfo = new SetItemTable();
			setItemInfo.LoadTableFromXML(this, "SetItemTable", assetBundle);
			
			monsterPictureBookTable = new MonsterPictureBookTable();
			monsterPictureBookTable.LoadTable(this, "MonsterPictureBookTable", assetBundle);
			
			bossRaidTable = new BossRaidTable();
			bossRaidTable.LoadTable(this, "BossRaidTable", assetBundle);
			
			tooltipsTable = new ToolTipTable();
			tooltipsTable.LoadTable(this, "ToolTips", assetBundle);
			
			gamblePriceTable = new GamblePriceTable();
			gamblePriceTable.LoadTable(this, "GamblePriceTable", assetBundle);
			
			costumeSetItemTable = new CostumeSetItemTable();
			costumeSetItemTable.LoadTable(this, "CostumeSetItemTable", assetBundle);
			
			shopCostumeSetTable = new ShopCostumeSetTable();
			shopCostumeSetTable.LoadTable(this, "ShopCostumeSetItemTable", assetBundle);
			
			shopNormalTable = new ShopTable();
			shopNormalTable.itemRateStep = 2;
			shopNormalTable.LoadTable(this, "ShopNormalTable", assetBundle);
			
			shopCostumeTable = new ShopTable();
			shopCostumeTable.LoadTable(this, "ShopCostumeTable", assetBundle);
			
			shopArtifactTable = new ShopTable();
			shopArtifactTable.itemRateStep = 3;
			shopArtifactTable.LoadTable(this, "ShopArtifactTable", assetBundle);
			
			awakeningLevelInfoTable = new AwakeningLevelInfoTable();
			awakeningLevelInfoTable.LoadTable(this, "AwakenSkillInfoTable", assetBundle);
			
			timeLimitItemTable = new TimeLimitItemTable();
			timeLimitItemTable.LoadTable(this, "TimeLimitItemTable", assetBundle);
			
			itemReinforceInfoTable = new ItemReinforceInfoTable();
			itemReinforceInfoTable.LoadTable(this, "ItemReinforceTable", assetBundle);
			
			arenaItemRateTable = new ArenaItemRateTable();
			arenaItemRateTable.LoadTable(this, "ArenaItemRateTable", assetBundle);
		}
		
		Game.Instance.InitCharInfoData();
	}
	
	public void GetGamblePriceValue(out Vector3 Gamble, out Vector3 Refresh, out Vector3 oneMore)
	{
		Gamble = Vector3.zero;
		Refresh = Vector3.zero;
		oneMore = Vector3.zero;
		
		Gamble.y = (float)stringValueTable.GetData(StringValueKey.GamblePriceJewel);
		Refresh.y = (float)stringValueTable.GetData(StringValueKey.GambleRefreshPrice);
		oneMore.y = (float)stringValueTable.GetData("OneMoreGamblePriceJewel");
	}
}
