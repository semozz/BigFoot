using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDef 
{
    public enum ePlayerClass
    {
		CLASS_NONE = -1,
        CLASS_WARRIOR, 
        CLASS_ASSASSIN,
        CLASS_WIZARD,
        CLASS_MAX
    };

    public enum eItemType
    {
        NORMAL,
        COSTUME,
        REDPOTION,
        YELLOWPOTION,
        GOLD,
		MOJO,
        REVIVAL,
        RESETSPECIALITY,
        EXPANDSLOT,
		MATERIAL,
        JEWEL,
		EXPANDSLOT_MAX,
        MAX_COUNT
    };
	
	// Don't Change!!! Server Share Value
    public enum eItemSlotWindow
    {
        Equip,
        Inventory,
        Costume,
        Shop_Normal,
        Shop_Costume,
        Shop_Cash,
		Reinforce_Item,
		Reinforce_Material,
		Composition_Item,
		Composition_Material,
		StageReward,
		StageReward_List,
		Gamble,
		Special,
		Shop_CostumeSet,
		CostumeSet,
		MaterialItem,
		Cash_Jewel,
		Cash_Gold,
		Cash_Buff,
		Cash_Potion,
    };

	public enum eSlotType
	{
		Common,
		Head,
		Hand,
		Pants,
		Boots,
		Armor,
		Weapon,
		Accessories,
		Ring,
		Potion_1,
		Potion_2,
		Costume_Body,
		Costume_Back,
		Costume_Head,
	}
	
	public static eSlotType ItemTypeToSlotType(ItemInfo.eItemType itemType)
	{
		GameDef.eSlotType equipSlotType = GameDef.eSlotType.Common;
		switch(itemType)
		{
		case ItemInfo.eItemType.Head:
			equipSlotType = GameDef.eSlotType.Head;
			break;
		case ItemInfo.eItemType.Hand:
			equipSlotType = GameDef.eSlotType.Hand;
			break;
		case ItemInfo.eItemType.Pants:
			equipSlotType = GameDef.eSlotType.Pants;
			break;
		case ItemInfo.eItemType.Boots:
			equipSlotType = GameDef.eSlotType.Boots;
			break;
		case ItemInfo.eItemType.Armor:
			equipSlotType = GameDef.eSlotType.Armor;
			break;
		case ItemInfo.eItemType.Weapon:
			equipSlotType = GameDef.eSlotType.Weapon;
			break;
		case ItemInfo.eItemType.Accessories:
			equipSlotType = GameDef.eSlotType.Accessories;
			break;
		case ItemInfo.eItemType.Ring:
			equipSlotType = GameDef.eSlotType.Ring;
			break;
		case ItemInfo.eItemType.Costume_Body:
			equipSlotType = GameDef.eSlotType.Costume_Body;
			break;
		case ItemInfo.eItemType.Costume_Back:
			equipSlotType = GameDef.eSlotType.Costume_Back;
			break;
		case ItemInfo.eItemType.Costume_Head:
			equipSlotType = GameDef.eSlotType.Costume_Head;
			break;
		case ItemInfo.eItemType.Potion_1:
			equipSlotType = GameDef.eSlotType.Potion_1;
			break;
		case ItemInfo.eItemType.Potion_2:
			equipSlotType = GameDef.eSlotType.Potion_2;
			break;
		default:
			equipSlotType = GameDef.eSlotType.Common;
			break;
		}
		
		return equipSlotType;
	}

    public enum eNPCKind
    {
        NPC_NORMAL,
        NPC_SHOP,
        NPC_STORAGE,
        NPC_GATE,
        NPC_STATUS,
		NPC_GAMBLER,
		NPC_POST,
		NPC_GHOST,
        NPC_MAX
    };
	
	public enum ePotionUseResult
	{
		GAUGE_FULL,
		COOLTIME,
		ITEM_COUNT,
		OK,
		CAN_NOT,
	}
	

    public static ePlayerClass GetAvatarClass(int index)
    {
        switch (index)
        {
            case 0: return ePlayerClass.CLASS_WARRIOR;
            case 1: return ePlayerClass.CLASS_ASSASSIN;
            case 2: return ePlayerClass.CLASS_WIZARD;
        }
        return ePlayerClass.CLASS_MAX;
    }
	
	/*
    public static string ToString (ePlayerClass e)
    {
        switch (e)
        {
            case ePlayerClass.CLASS_WARRIOR : return Game.instance.GetString(30);
            case ePlayerClass.CLASS_ASSASSIN: return Game.instance.GetString(31);
            case ePlayerClass.CLASS_WIZARD: return Game.instance.GetString(32);
        }

        return Game.instance.GetString(39);
    }
	*/
	
	public enum ePoisonLevel
	{
		POISON_LEVEL_01,
		POISON_LEVEL_02,
		POISON_LEVEL_03,
		POISON_LEVEL_04,
		POISON_LEVEL_05,
		MAX_COUNT,
	}
	
	public enum eItemAbilityType 
	{
		NONE = -1,
		STR, 
		DEX, 
		INT,
		ATTACK,
		HP_MAX,
		HP_RECOVER,
		CRITICAL_RATE, 
		SKILLGAUGE_MAX,
		SKILLGAUGE_RECOVER,
		POISION_DAMAGE,
		USESKILLGAUGE1,
		USESKILLGAUGE2,
		KNOCKDOWN_CANCEL,
		DASHSPEED,
		MAX_COUNT,
	}
	
	public enum eDefenceState
	{
		Absorb,
		Block,
		Avoid,
		MAX_COUNT,
	}
	
	public static Color GetColor(int r, int g, int b)
	{
		return GetColor(r, g, b, 255);	
	}
	public static Color GetColor(int r, int g, int b, int a)
	{
		float fValue = 1 / 255.0f;
		float fR = (float)r * fValue;
		float fG = (float)g * fValue;
		float fB = (float)b * fValue;
		float fA = (float)a * fValue;
		
		return new Color(fR, fG, fB, fA);
	}
	
	private static List<Color> PoisonColorLevel = new List<Color>();
	private static Color _CurseColor = GetColor(165, 65, 205);
	public static Color CurseColor
	{
		get { return _CurseColor; }		
	}
	private static Color _BerserkColor = GetColor(230, 80, 75);
	public static Color BerserkColor
	{
		get { return _BerserkColor; }		
	}
	private static Color _SlowColor = GetColor(70, 160, 200);
	public static Color SlowColor
	{
		get { return _SlowColor; }		
	}
	
	private static Color _reflectColor = GetColor(100, 150, 255);
	public static Color ReflectColor
	{
		get { return _reflectColor; }
	}
	
	public static void InitColorValue()
	{
		SetPoisonLevelColors();	
	}
	
	private static void SetPoisonLevelColors()
	{
		PoisonColorLevel.Clear();
		
		PoisonColorLevel.Add(GetColor(110, 120, 80, 255));
		PoisonColorLevel.Add(GetColor(100, 115, 50, 255));
		PoisonColorLevel.Add(GetColor(80, 110,  40, 255));
		PoisonColorLevel.Add(GetColor( 65, 105,  20, 255));
		PoisonColorLevel.Add(GetColor( 50, 100,  0, 255));
	}
	
	public static Color GetPoisonLevelColor(int stackCount)
	{
		Color selectedColor = Color.green;
		
		if (stackCount < 0 || stackCount >= PoisonColorLevel.Count)
			selectedColor = Color.green;
		else
			selectedColor = PoisonColorLevel[stackCount];
		
		return selectedColor;
	}
	
	
	public enum eBuffType 
	{ 
		BT_NONE,
		BT_CURSE,
		BT_MANASHIELD,
		BT_RED_POTION,
		BT_YELLOW_POTION,
		BT_POISION,
		BT_BERSERK,
		BT_SLOW,
		BT_INVINCIBLE,
		BT_REGENHP,
		BT_REFLECTDAMAGE,
		BT_DEC_ATTACK_RATE,
		BT_BOSSRAID_PHASE2,
		BT_DEC_DAMAGE,
	}
	
	public static int MaxWaveCount = 20;
	
	public static int EquipNormalItemSlotIndex = 5;
	public static int EquipMaxCount = 11;
	
	public static float DecDurabilityDamageRate = 0.06f;
	
	public static float IncDurabilityValue = 5.0f;
	
	
	public static string GetHex(int num) {
        const string alpha = "0123456789ABCDEF";
        string ret = "" + alpha[num];
        return ret;
    }
 
    public static string RGBToHex(Color color) {
        float red = color.r * 255;
        float green = color.g * 255;
        float blue = color.b * 255;
 
        string a = GetHex(Mathf.FloorToInt(red / 16));
        string b = GetHex(Mathf.RoundToInt(red) % 16);
        string c = GetHex(Mathf.FloorToInt(green / 16));
        string d = GetHex(Mathf.RoundToInt(green) % 16);
        string e = GetHex(Mathf.FloorToInt(blue / 16));
        string f = GetHex(Mathf.RoundToInt(blue) % 16);
 
        return "[" + a + b + c + d + e + f + "]";
    }
	
	public static string MakeItemGradeToString(int grade)
	{
		int stringID = -1;
		
		if (grade >= 0 && grade <= Item.limitCompositionStep)
			stringID = 1000 + grade;
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string msg = "";
		if (stringTable != null)
			msg = stringTable.GetData(stringID);
		
		return msg;
	}
	
	public static string MakeCharClassToString(GameDef.ePlayerClass classType)
	{
		int stringID = -1;
		switch(classType)
		{
		case ePlayerClass.CLASS_WARRIOR:
			stringID = 1;
			break;
		case ePlayerClass.CLASS_ASSASSIN:
			stringID = 2;
			break;
		case ePlayerClass.CLASS_WIZARD:
			stringID = 3;
			break;
		}
		
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string msg = "";
		if (stringTable != null)
			msg = stringTable.GetData(stringID);
		
		return msg;
	}
}

public class BuffInfo
{
	public float Rate = 1.0f;
	public float DelayTime = 1.0f;
	public float Value = 1.0f;
	public GameDef.eBuffType Type = GameDef.eBuffType.BT_NONE;
}

public class EventBoxParam
{
	public Collider other;
	public Collider self;
}

[System.Serializable]
public class AbilityValue
{
	public float defaultValue = 0.0f;
	public float randValue = 0.0f;
	
	public GameDef.eItemAbilityType type = GameDef.eItemAbilityType.NONE;
	public float abilityValue = 0.0f;
	public float nextAddValue = 0.0f;
	public bool isRelative = false;
}
