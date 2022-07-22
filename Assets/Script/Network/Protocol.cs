using System;

// 변수명은 변경되면 안됨. php/protocol.php의 클래스와 동일해야함.
public enum NetID
{
    None = 0,
    Error = 1,
    CreateAccount = 2,	
	Login = 3,
    CreateNickName = 4,
    UserInfo = 5,
	CharacterInfo = 6,
	BuyCostumeSetItem = 7,	
	BuyNormalItem = 8,
	BuyCostumeItem = 9,
	SellEquipItem = 10,
	SellCostumeItem = 11,
	SellInvenItem = 12,
	RegisterGCMID = 13,
	SellCostumeSetItem = 14,
	SellMaterialItem = 15,
	EquipItem = 16,
	SkillInfo =17,
	LoginDone = 18,
	DoEquipItem = 19,
	DoEquipItemRespone = 20,
	ReinforceItem = 21,
	CompositionItem = 22,
	BuyCashItem = 23,
	GambleInfo = 24,
	GambleRefresh = 25,
	SelectGambleItem = 26,
	SupplyGambleItem = 27,
	GambleRefreshRespone = 28,
	WaveStart = 29,
	WaveStartRespone = 30,
	WaveEnd = 31,
	WaveEndRespone = 32,
	WaveContinue = 33,
	WaveStartBuyStamina = 34,
	WaveRanking = 35,
	DoUnEquipItem = 36,
	DoUnEquipItemRespone = 37,
	GMCheat = 38,
	StageStart = 39,
	StageEnd = 40,
	StageResult = 41,
	UpdateStamina = 42,
	PopupNotice =  43,
	PopupNoticeIgnore = 44,
	Revival = 45,
	StageEndFailed = 46,
	CheckNickName = 47,
	RequestCreateNickName = 48,
	ChangeGambleItem = 49,
	
	ArenaInfo 				= 50,
	ArenaStart,
	ArenaStartBuyTicket,
	ArenaEnd,
	ArenaMatchingTarget,
	ArenaResult,
	RequestArenaRanking,
	TargetEquipItem,
	ArenaRanking,
	ArenaReward,
	
	RecoveryStamina = 60,
	RecoveryStaminaByStage = 61,	
	EnterTown = 62,	
	EnterArena = 63,	
	EnterWave = 64,	
	WaveInfo = 65,	
	WaveReward = 66,	
	UpgradeSkill = 67,	
	ResetSkill = 68,	
	RequestGambleRefresh = 69,	
	
	EnterPost = 70,
	MailList= 71,
	RequestPostItem= 72,		// 우편아이템받기(보석,골드,스태미너)
	RequestPostItemAll= 73,		// 우편아이템받기(보석,골드,스태미너)
	RequestPostMsg= 74,		// 우편메세지요청
	PostMsg= 75,					// 우편메세지수신.
	PostItem= 76,					// 우편아이템수신.
	PostItemAll= 77,				// 우편아이템모두수신.	
	SupplyTicket = 78,			// 투기장티켓지급. 더하지않고 SET한다.
	RequestGambleInfo = 79,
	
	EnterMessenger =  80,
	SNSFriendList =  81,			
	RecommandFriendList =  82,	// 추천친구목록.
	InvitedUserList =  83,			// 나를초대한 유저리스트.
	FriendList =  84,					// 친구목록.
	FriendInvite =  85,				// 친구초대.
	FriendInviteAccept =  86,		// 친구초대수락.
	DeleteFriend =  87,				// 친구삭제
	SendStaminaToFriend =  88,	// 친구에게 스태미너보내기.
	FriendInviteByNickName = 89,	// 닉네임으로 친구초대.
	
	Achievement =  90,				// 업적.
	AchievementCompleteInfo =  91,		// 업적완료목록.
	AchievementInfo =  92,				// 업적목록.
	AchievementProgress =  93,			// 진행상황.
	AchievementReward =  94,
	
	DailyMissionInfo =  95,			// 일일임무.
	DailyMissionProgress =  96,
	DailyMissionReward =  97,
	
	
	BossRaidEnter = 100,
	BossAppear = 101,
	BossRaidStart = 102,
	BossRaidStartRecoveryStamina = 103,
	BossRaidEnd = 104,
	BossRaidInfo = 105,
	RandomBox = 106,
	SpecialStage = 107,
	
	// add.
	RequestInventory = 110,
	
	InvenNormalInfo= 111,			// 일반아이템 인벤토리정보.
	InvenCostumeInfo= 112,		// 코스튬 인벤토리.
	InvenCostumeSetInfo= 113,	// 쿄스튬세트 인벤토리.
	InvenMaterialInfo= 114,			// 재료 인벤토리.
	
	InvenExtendInfo= 115,			// 인벤 확장정보.
	
	InvenExtend= 116,				// 인벤 확장요청.	
	
	WearCostumeSetItem = 117,
	UnwearCostumeSetItem = 118,
	WearCostumeItem = 119,
	UnwearCostumeItem = 120,
	
	RequestGambleOpen = 121,
	
	RequestBuyCashItem = 122,
	ResponeBuyCashItem = 123,
	ResponeBuyCashItemFailed = 124,
	Coupon =  125,
	BadgeNotify = 126,
	AttandnceCheck = 127,
	ReviewPlease = 128,
	RequestReviewReward = 129,
	IgnorePush = 130,
	ShowEvent = 131,
	StageTutorial = 132,
	TutorialDone = 133,
	EventShopInfo = 134,
	BuyCashLimitItem = 135,
	SpecialMissionInfo = 136,
	SpecialMissionProgress = 137,
	SpecialMissionReward = 138,
	SpecialMissionCompleteInfo = 139,
	
	TimeLimitItemInfo = 140,	//기간제 아이템 정보.
	InvokeTimeLimitItem = 141,	//기간제 아이템 구입시 활성화?
	
	AwakeningSkillInfo = 142,		//각성 스킬.
	AwakeningUpgreadeSkill = 143,	//각성 스킬 레벌업.
	AwakeningResetSkill = 144,
	AwakeningBuyPoint = 145,
	
	EventListInfo = 146,		//이벤트 정보?.
	
	ReinforceItemEx = 147,		//강화.
	CompositionItemEx = 148,	//합성..
	StageReward = 149,			//result reward
	
	PreLoginDone = 1000,
	PublicKey = 1001,
	AESKey = 1002,
	EncrpytEcho= 1003,
	RequestReconnect = 1004,
	ServerChecking = 1005, // 서버점검중.
	NeedUpdateApp = 1006,
	
	Dropout = 1008, //회원 탈퇴.
	RecvEmpty = 1009, //아무것도 안하는 패킷..
}

public enum GMCMD
{
	None = 0,
	AddGold,
}

public enum Publisher
{
	None = 0,
	Google,
	Apple,
	TStore,
	OllehMarket,
	UPlus,
	Naver,
}

public enum AccountType
{
	MonsterSide = 0,
	GooglePlus = 1,
	Facebook = 2,
	Kakao = 3,
	Guest = 4,
}

public enum CashItemType
{
	None = 0,
	CashToJewel,			// 현찰로 보석을 산다.
	JewelToGold,			// 보석으로 골드를 산다.
	CashToMeat,			//체력 회복 고기.
	Medal,
	PackageItem,		//패키지 아이템.
}

public enum MailType
{
	Item = 0,						// 운영자가 아이템.
	Msg = 1,                   	 	// 운영자가 메세지.	
	Stamina = 2,                 	// 스태미나 선물. XXXX님이 행동력10을 선물했습니다.						
	StaminaReward = 3,         	// 스태미나 선물에 대한 보상. XXXX님에게 행동력을 보낸 답례로 재료아이템이름을(를) 받았습니다.						
	AchievementReward = 4,    // 업적보상	Sender에 업적이름이 들어있다. 'XXXX'업적의 보상으로 아이템이름을(를) 받았습니다.					
	MissionReward = 5,       	 // 미션보상	Sender에 미션이름이 들어있다. 'XXXX'일일 임무의 보상으로 보석XX개를 받았습니다.					
	BossRaid01 = 6,						//'보스이름'레이드 피해량 1위 보상으로 아이템이름을(를) 받았습니다.						
	BossRaid02 = 7,						//'보스이름'레이드에 참여한 보상으로 아이템이름을(를) 받았습니다.						
	BossRaid03 = 8,						//'보스이름'에게 치명상을 입힌 보상으로 뽑기표을(를) 받았습니다.						
	System = 9,							// 운영자가 선물.(골드, 보석,스태미나)
	CouponReward = 10,					//쿠폰입력으로..
	PackageItem = 11,				//패키지 아이템.
	BossFinder = 12,				//'보스이름' 발견 보상으로 ###을(를) 받았습니다.
	EventReward = 13,				//이벤트 보상.
	JewelPack = 14,					//보석 패키지 아이템
	InventoryFull = 15,				//인벤토리 부족 스테이지 보상으로
}

public class InvenType
{
    public const int None = 0;
	public const int Material = 1;
	public const int Costume = 2;
	public const int CostumeSet = 3;
	public const int Normal = 4;
}


public class Header
{
    public NetID MsgID;
	public int UserIndexID = -1;
	public string uniqueID;
	public string token;
}

public class PacketError : Header
{
	public int ErrorCode;
    public string ErrorMessage;
    
    public PacketError() { this.MsgID = NetID.Error;  }
}

public class PacketRequestReconnect : Header
{
	public NetErrorCode errorCode;
	public string packet;
	public string url;
	
	public PacketRequestReconnect() { this.MsgID = NetID.RequestReconnect; }
}

public class PacketCreateAccount : Header
{
	public int errorCode;
	
	public string Account;
	public string Password;
	public int EmailType;			//0:MonsterSide, 1:Google, 2:Facebook
	
	public PacketCreateAccount() { this.MsgID = NetID.CreateAccount; }
}

public class PacketRegisterGCMID : Header
{
	public string regID;		// max:250.
	
	public PacketRegisterGCMID() { this.MsgID = NetID.RegisterGCMID; }
}

public class PacketLogin : Header
{
    public string AccountID;		// email.
	public string Password;
	public NetConfig.PublisherType publisherID;
	public AccountType EmailType;	
	public int version;				// Version.NetVersion
	public string deviceModel;
	public string OS;
    
    public PacketLogin() { this.MsgID = NetID.Login; }
}

public class StageType
{
	public static int NormalHP = 0;
	public static int Hard = 1;
	public static int Hell = 2;
}

public class CharacterDBInfo
{
	public int CharacterIndex;				// 전사.도적.마법사.
    
	// server가 채워준다.
	public int Level;
	public int SkillPoint;
	public long ExpValue;
	public string ExpValueStr;
	
	public long AExp;			//각성경험치
	public string AExpStr;
	public int ALevel;			//각상 레벨.
	
	public int APoint;			//각성포인트.
	public int APointGift;		//각성 선물 포인트.
	public int ALimitBuyCount;	//각성 구매 가능 횟수
	public int ABuyCount;		//각성 포인트 구매 횟수.
	
	public int[] StageIndex;		//index 0 : normal, 1 : hard, 2 : hell
	public int StaminaLeftTimeSec; 	// 5가 충전될 때까지의 남은 초. 충전 될 때 5의 스테미너가 회복되고, 충전시간은 25분이다.	0이되면 5가 찬다.				
	public int StaminaCur;		
	public int StaminaMax;
	public int StaminaPresent;
	//public int Ticket;
	public int gambleLeftSec;
	
	public int tutorial;
}

public class PacketCharacterInfo : Header
{
	public int Count;
	
	public CharacterDBInfo[] Infos;
	
	public PacketCharacterInfo() { this.MsgID = NetID.CharacterInfo; }
}

public class PacketSelectCharacter : Header
{
	public int index;
	
	public PacketSelectCharacter() { /*this.MsgID = NetID.SelectCharacter;*/ }
}

public class PacketUserInfo : Header
{
    
    //public string AccountID;
	public string NickName;
	public DateTime CreateTime;
	public DateTime ShutdownTime;
    public int Gold;
    public int Cash;
	public int Medal;
	public int Coupon;	// 겜블쿠폰.
    public int tutorial;
	
	public int potion1;	//물약
	public int potion2; //고기
	public int potion1Present;	//물약 선물 받음. 선물 받은게 먼저 까이고, 0이 되면 potion1이 까임..
	public int potion2Present;	//고기 선물 받음.
	
	public int[] buyLimitedItems;	//패키지 아이템구입 정보.
	
    public PacketUserInfo() { this.MsgID = NetID.UserInfo; }
}

public class ItemDBInfo
{
	public string UID = "";
	public int ID = 0;
	public int Grade = 0;
	public int Count = 0;
	public int Reinforce = 0;
	public int Rate = 0;
	public int Exp = 0;
}

public class EquipItemDBInfo
{
	public int SlotIndex = 0;
	public string UID = "";
	public int ID = 0;
	public int Grade = 0;
	public int Count = 0;
	public int Reinforce = 0;
	public int Rate = 0;
	public int Exp = 0;
}

public class PacketEquipItem : Header
{
	public int CharacterIndex;
	public int Count;
	
	public CostumeItemDBInfo SetItem;
	public EquipItemDBInfo[] Infos;
	
	public PacketEquipItem() { this.MsgID = NetID.EquipItem; }
}

public class PacketInventory : Header
{
	public int Count;
	
	public ItemDBInfo[] Infos;
	
	public PacketInventory() { /*this.MsgID = NetID.Inventory;*/ }
}

public class PacketCostume : Header
{
	public int Count;
	
	public ItemDBInfo[] Infos;
	
	public PacketCostume() { /*this.MsgID = NetID.CostumeInfo;*/ }
}

public class SkillDBInfo
{
	public int[] IDs;
	public int[] Lvs;
}

public class SkillUpgradeDBInfo
{
	public int[] SkillIDs;
	public int[] Levels;
	public int[] Adds;
}

public class PacketSkillInfo : Header
{
	public int CharacterIndex;
	public int Count;
	
	public SkillDBInfo Info;
	
	public PacketSkillInfo() { this.MsgID = NetID.SkillInfo; }
}

public class PacketUpgradeSkill : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	
	public SkillUpgradeDBInfo Info;
	
	public int SkillPoint;
	
	public PacketUpgradeSkill() {this.MsgID = NetID.UpgradeSkill; }
}

public class PacketResetSkill : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	public int SkillPoint;
	
	public int TotalGold;
	public int TotalCash;
	
	public PacketResetSkill() {this.MsgID = NetID.ResetSkill; }
}

public class PacketBuyItem : Header
{
	public int CharacterIndex;		// xgreen add.
	public int ItemID;	// 사려는 아이템 아이디.
	public int Grade; // 사려는 아이템 등급.
	public int ReinforceStep;
	public int itemCount;			// 아이템 총갯수. 스택되면 서버가 갯수를 알려준다.
	
	public int slotIndex = -1;
	public GameDef.eItemSlotWindow windowType = GameDef.eItemSlotWindow.Inventory;
	
	// server Result
	public NetErrorCode result = NetErrorCode.None;			// 0:OK errorCode 참고.
	public string UID;				// 아이템 유니크아이디.
	public int Gold;					// 유저총골드. (-1이면 갱신하지않는다.)
	public int Cash;				// 유저총캐쉬. (-1이면 갱신하지않는다.)
	public int Medal;				// 유저총메달. (-1이면 갱신하지않는다.)

	public PacketBuyItem(NetID msgID) {this.MsgID = msgID; }
}

// 투기장 아이템.
public class PacketBuyNormalItem : Header
{
	public int CharacterIndex;
	public int ItemID;
	public int slotIndex;
	public GameDef.eItemSlotWindow windowType;
	public int buyCount;
	
	public NetErrorCode result = NetErrorCode.None;
	public int Medal;
	
	public int rate;
	public int grade;
	public int reinforce;
	public int exp;
	
	public string[] UIDs;
	public int[] counts;
	
	public PacketBuyNormalItem() { this.MsgID = NetID.BuyNormalItem; }
}
// 코스튬 파츠아이템.
public class PacketBuyCostumeItem : Header
{
	public int CharacterIndex;
	public int ItemID;
	public int slotIndex;
	public GameDef.eItemSlotWindow windowType;
	
	public NetErrorCode result = NetErrorCode.None;
	public int Gold;
	public int Cash;
	public string UID;
	
	public PacketBuyCostumeItem() { this.MsgID = NetID.BuyCostumeItem; }
}
// 코스튬 세트아이템.
public class PacketBuyCostumeSetItem : Header
{
	public int CharacterIndex;
	public int ItemID;
	public int slotIndex;
	public GameDef.eItemSlotWindow windowType;
	
	public NetErrorCode result;
	public int Gold;
	public int Cash;
	public string UID;
	
	public PacketBuyCostumeSetItem() { this.MsgID = NetID.BuyCostumeSetItem; }
}

public class PacketSellItem : Header
{
	
	public int CharacterIndex;
	public string UID;	// 팔려는 아이템.
	public int ItemID;	// 팔려는 아이템.
	
	public int slotIndex = -1;	// for client;
	public GameDef.eItemSlotWindow windowType; // for client;
	public bool Shop; // 상점에서 팔거나 창고에서 팔수 있음.
	
	// server Result
	public NetErrorCode result = NetErrorCode.None;			// 0:OK errorCode 참고.
	public int Gold;				// 유저골드.
	public int Cash;				// 유저캐쉬.	
	
	//public PacketSellItem()	{this.MsgID = NetID.SellItem; }
}

public class BaseTradeItemInfo
{
	public string UID = "";
	public int ItemID = -1;
	public int slotIndex = -1;
	public GameDef.eItemSlotWindow windowType = GameDef.eItemSlotWindow.Inventory;
}


public class PacketDoEquipItem : Header
{
	public int CharacterIndex;
	
	public int eEquipSlot;		// 장착할 위치.
	public BaseTradeItemInfo Info;
	
//	public string UID = "";				// 장착할 아이템.
//	public int ItemID = -1;				// 장착할 아이템 아이디.
//	public int slotIndex = -1;			// 장착할 위치. eSlotIndex enum값 참조.

//	public GameDef.eItemSlotWindow windowType = GameDef.eItemSlotWindow.Inventory;	// 현재 아이템 위치.
	
	public PacketDoEquipItem() { this.MsgID = NetID.DoEquipItem; }
}

public class PacketDoEquipItemRespone : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	
	public int equipSlotIndex = -1;	// 장착슬롯 위치.																					
	public int invenSlotIndex = -1;	// 인벤슬롯 위치.
	public GameDef.eItemSlotWindow windowType = GameDef.eItemSlotWindow.Inventory;	// 현재 아이템 위치. Inventory or Costume
	
	public ItemDBInfo Equip;			// 장착한아이템. 이미장착된 아이템과 UID가 같으면 갱신. 없으면 생성한다.
	
	public int UnequipItemType;	//0: CostumeSetIte, 1: Costume
	public ItemDBInfo Unequip;			// 탈착된 아이템. UID기준으로 인벤토리에 있으면 갱신해주고.  없으면 생성한다. (Count=0 이면 삭제한다.)
	
	public PacketDoEquipItemRespone() { this.MsgID = NetID.DoEquipItemRespone; }
}

public class PacketDoUnEquipItem : Header
{
	public int CharacterIndex;
	
	public BaseTradeItemInfo tradeInfo = new BaseTradeItemInfo();		//slotIndex == EquipSlotIndex
	
	public PacketDoUnEquipItem() { this.MsgID = NetID.DoUnEquipItem; }
}

public class PacketDoUnEquipItemRespone : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public int slotIndex = -1;	
	public ItemDBInfo[] Unequip;
	
	public PacketDoUnEquipItemRespone() { this.MsgID = NetID.DoUnEquipItemRespone; }
}

public class MaterialItemInfo
{
	public string UID;
	public int Count;	// 총갯수. 0 개면 삭제한다.
}

public class PacketReinforceItem : Header
{
	
	public int CharacterIndex;
	public NetErrorCode errorCode;
	public BaseTradeItemInfo tradeInfo;
	public int reinforceStep;		// 강화단계
	public int totalGold;				// 
	public int totalCash;
	public MaterialItemInfo[] DelItemUIDs; // need null check
	
	public PacketReinforceItem() { this.MsgID = NetID.ReinforceItem; }
}

public class PacketReinforceItemEx : Header
{
	public int CharacterIndex;
	public NetErrorCode errorCode;
	
	public string UID= "";
	public int ItemID = -1;
	public int slotIndex = -1;
	public GameDef.eItemSlotWindow windowType = GameDef.eItemSlotWindow.Inventory;
	
	public int exp = 0;
	public int reinforceStep;	// 강화단계
	public int totalGold;		// 
	public int totalCash;
	public string[] DelItemUIDs; // need null check
	
	public PacketReinforceItemEx() { this.MsgID = NetID.ReinforceItemEx; }
}

public class PacketCompositionItem : Header
{
	
	public int CharacterIndex;
	public BaseTradeItemInfo Info = new BaseTradeItemInfo();			// 합성될 아이템. 메인템. (장착창에 있는 아이템일수 있다. window를 Equip으로 보내준다.)
	
	public string materialUID;			// 합성재료.
	public int ByJewel;					//1.보석, 0: 골드.
	
	public NetErrorCode errorCode;	
	
	public string DelItemUID;	// 합성될 아이템. 삭제됨.
	public int Grade;
	public int totalGold;
	public int totalCash;
	
	public int materialCount;			// 합성재료 남은 갯수. 0개면 삭제한다.
	
	public int tutorial;
	
	public PacketCompositionItem() { this.MsgID = NetID.CompositionItem; }
}

public class PacketCompositionItemEx : Header
{
	public NetErrorCode errorCode;	
	
	public int CharacterIndex;
	public int ByJewel;					//1.보석, 0: 골드.
	public string UID = "";
	public int ItemID = -1;
	public int slotIndex = -1;
	public GameDef.eItemSlotWindow windowType = GameDef.eItemSlotWindow.Inventory;
	public string materialUID;			// 합성재료.
		
	public string DelItemUID;	// 합성될 아이템. 삭제됨.
	public int Grade;
	public int reinforce;
	
	public uint Exp;
	
	public int totalGold;
	public int totalCash;
	
	public int materialCount;			// 합성재료 남은 갯수. 0개면 삭제한다.
	
	public int tutorial;
	
	public PacketCompositionItemEx() { this.MsgID = NetID.CompositionItemEx; }
}

public class GambleItem
{
	public int ID;	
	public int Grade;
	public int itemRate;
}

public class GambleInfo
{
	public int CharacterIndex;
	public int LeftTimeSec;				// 갱신 남은시간(초).
	
	public GambleItem[] Items;		// 12개.
}

public class PacketGambleInfo : Header
{
	public NetErrorCode errorCode;
	public GambleInfo Info;
	public System.DateTime now;
	public int LeftTimeSec;
	public int[] eventItemIDs;
	
	public PacketGambleInfo() { this.MsgID = NetID.GambleInfo; }
}

public class PacketRequestGambleRefresh : Header
{
	public NetErrorCode errorCode;
	public int LeftTimeSec;				// 갱신 남은시간(초).
	
	public PacketRequestGambleRefresh()	{ this.MsgID = NetID.RequestGambleRefresh; }
}

public class PacketGambleRefresh : Header
{
	public int CharacterIndex;
	public bool ByCash;			// true:캐쉬로.
	public GambleItem[] Infos;		// 12개.
	
	
	public PacketGambleRefresh() { this.MsgID = NetID.GambleRefresh; }
}

public class PacketGambleRefreshRespone : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;		// 유효성체크를 위해 나중에구현.
	public int LeftTimeSec;				// 갱신 남은시간(초).
	public int Cash;					// -1이면 갱신하지 않는다.
	public int Gold;
	
	public int eventLeftTimeSec;
	public int[] eventItemIDs;
	
	public PacketGambleRefreshRespone() { this.MsgID = NetID.GambleRefreshRespone; }
}

public class PacketSelectGambleItem : Header
{
	public int CharacterIndex;
	public int what; 	// 0:골드 1:보석 2:쿠폰.	
	public int bAgain;	// 0:처음 1:한번더.

	
	public PacketSelectGambleItem() { this.MsgID = NetID.SelectGambleItem; }
}	

public class PacketChangeGambleItem : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public int GambleIndex;
	public GambleItem Item;
	
	public PacketChangeGambleItem() { this.MsgID = NetID.ChangeGambleItem; }
}

public class PacketSupplyGambleItem : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;		// 
	public int GambleIndex;		// 겜블의 위치인덱스.(0base).
	public string UID;				// 아이템 유니크아이디.
	public int ItemID;
	public int Grade;
	public int itemRate;
	public int Count;
	public bool CostumeItem;		// true:쿄스튬탭에 false:인벤토리탭에 들어간다.
	public int Cash;
	public int Gold;
	public int coupon;					// 총량.
	public int exp;
	
	public PacketSupplyGambleItem() { this.MsgID = NetID.SupplyGambleItem; }
}

public class PacketGMCheat : Header
{
	public GMCMD cmd;
	public int intValue;
	
	public PacketGMCheat() { this.MsgID = NetID.GMCheat; }
}

public class PacketWaveStart : Header
{
			 
	public int CharacterIndex;	
	
	public int CurStamina;				// 현재 스테미나. 클라가 채워준다. 서버와의 오차가 생기면 클라값을 우선시해준다.
	public int SelectedTower;			// 나무, 벽돌, 강철 성문.
	public int [] SelectedBuffs;			// 버프타입.1:공경력강화 2.방어력강화. 
	public int buyPotion1;
	public int buyPotion2;
	
	public PacketWaveStart() { this.MsgID = NetID.WaveStart; }
}

public class PacketWaveStartRespone : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public int StaminaLeftTimeSec;	// 스테미나 5가 찰 남은 시간(초)
	public int StaminaCur;
	public int StaminaPresent;
	public int StartStep;					// 시작할 웨이브 단계;
	public int TotalCash;
	public int TotalGold;
	public int SelectedTower;			// 나무, 벽돌, 강철 성문.
	public int [] SelectedBuffs;			// 버프타입.1:공경력강화 2.방어력강화. 
	public int potion1;
	public int potion2;
	
	public PacketWaveStartRespone() { this.MsgID = NetID.WaveStartRespone; }
}

public class PacketWaveContinue : Header // 보석만 든다. 골드,스태미너 소비없음.
{
	public NetErrorCode errorCode;
	public int CharacterIndex;	
	
	// 서버가 채워준다	
	public int StartStep;					// 시작할 웨이브 단계;
	public int StartSec;
	public int TotalCash;
	public int TotalGold;
	public int SelectedTower;			// 나무, 벽돌, 강철 성문.
	public int [] SelectedBuffs;			// 버프타입.1:공경력강화 2.방어력강화. 
	
	public PacketWaveContinue() { this.MsgID = NetID.WaveContinue; }
}

public class PacketWavePause : Header
{
	public PacketWavePause() { this.MsgID = NetID.WaveStart; }
}

public class PacketWaveEnd : Header
{
	
	public int CharacterIndex;
	public int WaveStep;			// 웨이브단계 (0 - 80).
	public int DurationSec;		// 걸린시간.(초)
	public int Clear;				// 64스테이지까지 완료했으면 1 아니면 0
	public GainItemInfo [] usedItems;		// 플레이하면서 사용한 아이템.
	public int usedPotion1;
	public int usedPotion2;
	
	public PacketWaveEnd() { this.MsgID = NetID.WaveEnd; }
}

public class PacketWaveEndRespone : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	
	public int Cash;
	public int Gold;
	public int WaveStep;	
	public int DurationSec;
	
	public string UID;				// 아이템 유니크아이디.
	public int ItemID;
	public int Grade; 
	public int Count;
	public int Rate;				// 추가.
	public int invenType;
	public int exp;
	
	//public bool CostumeItem;		// true:쿄스튬탭에 false:인벤토리탭에 들어간다.
	public EquipItemDBInfo [] updateItems;// Count == 0 이면 삭제해주.
	public CostumeItemDBInfo costumeSetItem;
	
	public PacketWaveEndRespone() { this.MsgID = NetID.WaveEndRespone; }
}

public class PacketWaveRanking : Header
{
	public NetErrorCode errorCode;
	public int ranking;
	public bool bDown;					// int ranking 아래등수 10개 요청.
	public WaveRankingInfo [] rankingList;	// 10개.
	
	public PacketWaveRanking() { this.MsgID = NetID.WaveRanking; }
}

public class PacketRequestBuyCashItem : Header
{
	public int characterIndex;
	
	public NetErrorCode errorCode;
	public NetConfig.PublisherType Store;
	
	public int ItemID;
	public string TStoreProductCode;
	public string TStoreTID;
	public int Price;
	public int type; //eCashType 3:고기추가.
	public string addInfo;
	public string itemName;
	public int amount;
	public string spriteName;
	
	
	public PacketRequestBuyCashItem() { this.MsgID = NetID.RequestBuyCashItem; }	
}

public class PacketResponeBuyCashItem : Header
{
	public int characterIndex;
	
	public NetErrorCode errorCode;
	public NetConfig.PublisherType Store;
	public int ItemID;	
	public string TStoreProductCode;
	public string TStoreTID;
	public string OriginalJson;
	public string Siginature;
	
	public int Cash;
	public int Gold;
	
	public PacketResponeBuyCashItem() { this.MsgID = NetID.ResponeBuyCashItem; }	
}

public class PacketResponeBuyCashItemFailed : Header
{
	public int characterIndex;
	
	public string errorString;
	public NetConfig.PublisherType Store;
	public int ItemID;	
	public string TStoreProductCode;
	public string TStoreTID;	
	public PacketResponeBuyCashItemFailed() {this.MsgID = NetID.ResponeBuyCashItemFailed; }
}

public class PacketBuyCashItem : Header
{
	public int characterIndex;
	
	public NetErrorCode errorCode;
	public int cashID;	
	
	public int Cash;
	public int Gold;
	
	public int rate;
	public int grade;
	public int reinforce;
	public int itemID;
	public int exp;
	
	public string[] UIDs;
	public int[] counts;
	
	public PacketBuyCashItem()  { this.MsgID = NetID.BuyCashItem; }
}

public class PacketWaveStartRecoveryStamina : Header
{
	public NetErrorCode errorCode;	
	
	public int CharacterIndex;
	public int Cash;
	public int Gold;
	public int CurStamina;				// 현재 스테미나. 받으면 이값으로 갱신.
	public int PresentStamina;			// 서버에서 채워준다.
	public bool bWave;
	public bool bStart;
	public int SelectedTower;			// 나무, 벽돌, 강철 성문.
	public int [] SelectedBuffs;			// 버프타입.1:공경력강화 2.방어력강화. 
	
	public int buyPotion1;
	public int buyPotion2;
	
	public PacketWaveStartRecoveryStamina() { this.MsgID = NetID.RecoveryStamina; }
}

public class PacketStageStartRecoveryStamina : Header
{
	public NetErrorCode errorCode;
		
	public int CharacterIndex;	
	public int StageType;
	public int StageIndex;
	public int[] SelectedBuffs; 		// 0: None, 1:공격력강화 2:방어력강화.
	public int buyPotion1;
	public int buyPotion2;
	
	// server result;
	public int curStamina;			// 현재 스테미나. 받으면 이값으로 갱신.
	public int presentStamina;		// 받으면 이값으로 갱신.
	public int LeftTimeSec = 0;	// 스태미나 남은 시간. 서버가 채워준다.
	public int Cash = -1;				// 서버가 채워준다.
	public int Gold = -1;				// 서버가 채워준다.
	public int potion1;
	public int potion2;
	
	public PacketStageStartRecoveryStamina() { this.MsgID = NetID.RecoveryStaminaByStage; }
}

public class PacketStageStart : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	
	public int StageType;
	public int StageIndex;
	public int[] SelectedBuffs; 		// 0: None, 1:공격력강화 2:방어력강화.
	public int buyPotion1;	//물약 구매 갯수.
	public int buyPotion2;	//고기 구매 갯수.
	
	// server result;
	public int curStamina;			// 현재 스테미나. 클라가 채워준다. 서버가 다시 채워준다. 받으면 이값으로 갱신.
	public int presentStamina;
	public int LeftTimeSec = 0;	// 스태미나 남은 시간. 서버가 채워준다.
	public int Cash = -1;				// 서버가 채워준다.
	public int Gold = -1;				// 서버가 채워준다.
	
	public int potion1;	//총 물약 갯수 charDBInfo.potion1에 set.
	public int potion2; //총 고기 갯수 charDBInfo.potion2에 set.
	
	public PacketStageStart() { this.MsgID = NetID.StageStart; }
}

public class GainItemInfo 
{
	public int ID;
	public int Count;
}

public class PacketStageEnd :Header
{
	public int CharacterIndex;
	public int curLevel;
	public int gainGold;						// 스테이지 플레이하면서 얻은 골드.
	public int gainCash;
	
	public GainItemInfo[] gainNormalItems;		// 스테이지 플레이하면서 얻은 아이템.
	public GainItemInfo[] gainMaterialItems;		//재료 아이템.
	
	//public GainItemInfo[] usedItems;		// 스테이지 플레이하면서 사용한 아이템.
	public int usedPotion1;
	public int usedPotion2;
	
	public PacketStageEnd() { this.MsgID = NetID.StageEnd; }
}

public class PacketStageEndFailed : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	//public GainItemInfo [] usedItems;		// 스테이지 플레이하면서 사용한 아이템.
	public int usedPotion1;
	public int usedPotion2;
	
	public int potion1;
	public int potion2;
	public int potion1Present;
	public int potion2Present;
	
	public PacketStageEndFailed() { this.MsgID = NetID.StageEndFailed; }
	
}

public class PacketStageResult : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	
	public int curLevel;
	public int curALevel;	//각성 레벨
	
	public long totalEXP;	// 총경험치.
	public string totalEXPStr;
	public long totalAEXP;	//각성 경험치.
	public string totalAEXPStr;
	
	public int rewardEXP;					// 보상 경험치.
	public ItemDBInfo rewardItemInfo;	// 보상. 세개중 하나의 아이템.	
	
	public bool bLevelup;	// 레벨업했는지.
	public bool bALevelup;	//각성 레벨업.
	
	public int AddPoint;
	public int AddAPoint;
	public int AddBuyAblePoint;
	
	public int StageType;
	public int ClearStageIndex;				
	public int Gold;
	public int Cash;
	
	public int LevelupStamina;				// 스테미너 증가분.
	
	public ItemDBInfo[] gainNormalItems;					// array::ItemDBInfo;
	public MaterialItemDBInfo[] gainMaterialItems;
	
	public int potion1;            // 물약
   	public int potion2;            // 고기
  	public int potion1Present;   // 선물받은물약
   	public int potion2Present;   // 선물받은고기
	
	public int rewardIndex = -1;
	public int[] rewardItemIDs;       //  장비 아이템아이디 3 개 ,
	public int[] rewardItemGrades;    //  장비 아이템아이디 3 개 ,
	public int[] rewardItemRates;       //  장비 아이템아이디 3 개 ,
	public int rewardMeat = 0;               // 고기
	public int rewardGold = 0;               // 골드
	public int rewardMaterialItemID = 0;       // 강화재료
	public int[] rewardPrices;
	
	// EquipITem.
	public EquipItemDBInfo [] updateItems;// Count == 0 이면 삭제해주.
	public CostumeItemDBInfo costumeSetItem;
	
	public PacketStageResult() { this.MsgID = NetID.StageResult; }
}

public class PacketStageReward : Header
{
   	public NetErrorCode errorCode;
   	public int CharacterIndex;
   	public int stageType;            
   	public int stageIndex;            
   	
   	public int rewardIndex;         // zerobase;
   	public ItemDBInfo rewardItemInfo;       // ItemDBInfo
   	
   	public int totalGold;
   	public int totalCash;
   	public int potion2Present;   // 선물받은고기

	public PacketStageReward() { this.MsgID = NetID.StageReward; }
}

public class ArenaInfo
{
	public int rankType;					// 현재랭크.
	public int groupRanking;				// 랭크내 순위, 매주 정산될때 리셋됨.
	public int winningStreakCount = 0;		// 연승횟수. 리셋안됨.
	public int totalWinningCount = 0;		// 누적승수. 리셋안됨.
	public int seasonBestRank = 10;			// 시즌최고랭크.
}

public class ArenaRankingInfo
{
	public int ranking;
	public long UserIndexID;
    public long PlatformUserId;
	public int CharacterIndex;
	public string NickName;
    public string platform;
	public int[] charLevels;
}

public class PacketArenaInfo : Header
{
	public NetErrorCode errorCode;
	
	public int Open;					//1: Open, 0: Close.
	public int CharacterIndex;
	public int RewardLeftTimeSec;		// 정산 남은시간. -1이면 정산시간이다.
	public ArenaInfo Info;							// ArenaInfo;
	public ArenaRankingInfo [] rankingList;					// ArenaRankingInfo : array 11개. 내순위 위로 5, 아래로 5. 없을 수도 있다.
	
	public PacketArenaInfo() { this.MsgID = NetID.ArenaInfo; }
}

public class WaveRankingInfo
{
	public long UserIndexID;
    public long PlatformUserId;
    public string Platform;
	public int CharacterIndex;
	public int ranking;	
	public int RecordStep;
	public int RecordSec;
	public string NickName;	
}

public class PacketWaveInfo : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public int Open;				//1:Open 0:Close
	public int RewardLeftTimeSec;
	public int Ranking;				// 현재순위.
	public int RecordStep;			// 현재 기록.웨이브.
	public int RecordSec;			// 현재 기록.초.
	public int BestRecordSec;		// 최고 기록.초.
	public int Clear;					// 0:none, 1:클리어. 64스테이지까지 전부완료함. 
	public WaveRankingInfo [] rankingList;				// WaveRankingInfo array 11개. 내순위 위로 5, 아래로 5. 없을 수도 있다.

	public  PacketWaveInfo() {}
}

public class PacketArenaStart : Header
{
	public int CharacterIndex;
	
	public PacketArenaStart() { this.MsgID = NetID.ArenaStart; }
}

public class PacketArenaEnd : Header
{
	
	public int CharacterIndex;
	public bool bWin;
	public long TargetUserIndexID;
	public int TargetCharacterIndex;
	public PacketArenaEnd() { this.MsgID =NetID.ArenaEnd; }
}

public class PacketArenaMatchingTarget : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	public int Ticket;
	
	public string TargetNickName;
	
	public long TargetExp;
	public string targetExpStr;
	public long targetAwakenExp;
	public string targetAwakenExpStr;
    public string targetUserId;
    public string targetUserPlatform;
	
	public int TargetRankType;
	public int TargetGroupRanking;
	
	public EquipItemDBInfo [] TargetEquipInfos;		//EquipItemDBInfo
	public SkillDBInfo TargetSkillInfo;
	public SkillDBInfo awakenSkillInfo;
	
	public CostumeItemDBInfo costumeSetItem;
	
	public int TotalGold;
	public int TotalCash;
	
	public PacketArenaMatchingTarget() { this.MsgID = NetID.ArenaMatchingTarget; }
}

public class PacketArenaResult : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public int RewardLeftTimeSec;		// 정산 남은시간.
	public ArenaInfo ArenaInfo;					// ArenaInfo
	public ArenaRankingInfo [] rankingList;					// ArenaRankingInfo : array 11개. 내순위 위로 5, 아래로 5. 없을 수도 있다.
	
	public int TargetRankType;
	public int TargetGroupRanking;
	
	public PacketArenaResult() { this.MsgID = NetID.ArenaResult; }
}

public class PacketArenaReward : Header
{
	// 지난주 최고의 용사.
	public ArenaRankingInfo TopInfo;			// ArenaRankingInfo;
	
	public int CharacterIndex;
	public int RankType;
	public int GroupRanking;
	public int RewardMedal;
	
	public PacketArenaReward() { this.MsgID = NetID.ArenaReward; }
}


public class PacketWaveReward : Header
{
	// 지난주 최고의 용사.
	public WaveRankingInfo TopInfo;			// WaveRankingInfo;
	
	public int CharacterIndex;
	public int Ranking;				// 지난주 순위.
	public int RecordStep;			// 지난주 기록.웨이브.
	public int RecordSec;				// 지난주 기록.초.	
	public int RewardJewel;			// 보상보석.
	
	public PacketWaveReward() { this.MsgID = NetID.WaveReward; }
}


public class PacketArenaStartBuyTicket : Header
{
	
	public int CharacterIndex;
	
	public PacketArenaStartBuyTicket() { this.MsgID = NetID.ArenaStartBuyTicket; }
}

public class PacketEnterTown : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	
	public PacketEnterTown() { this.MsgID = NetID.EnterTown; }
}

public class PacketEnterWave : Header
{
	
	public int CharacterIndex;
	
	public PacketEnterWave() { this.MsgID = NetID.EnterWave; }
}

public class PacketEnterArena : Header
{
	
	public int CharacterIndex;
	public PacketEnterArena() { this.MsgID = NetID.EnterArena; }
}

public class PacketRequestArenaRanking : Header
{
	public int RankType;
	public int ranking;					// 
	public bool bDown;					// int ranking 아래등수 10개 요청.	int ranking포함. int ranking = 10, int bDown = true. 10, 12 ... 20위까지 등수 알려준다.
	
	public PacketRequestArenaRanking() { this.MsgID = NetID.RequestArenaRanking; }
}

public class TargetInfoAll
{
	/*
	// arenaInfo
	public int rankType;					// 현재랭크.
	public int groupRanking;				// 랭크내 순위, 매주 정산될때 리셋됨.
	public int winningStreakCount;	// 연승횟수. 리셋안됨.
	public int totalWinningCount;		// 누적승수. 리셋안됨.
	// waveInfo
	public int waveRanking;				// 현재순위.
	public int waveRecordStep;		// 진행웨이브.
	public int waveRecordSec;			// 진행시간.초.
	*/
	
	public long Exp;
	public string expStr;
	
	public EquipItemDBInfo [] equips;
	public SkillDBInfo skills;
	public SkillDBInfo awakenSkills;
	
	public CostumeItemDBInfo costumeSetItem;
}

public class PacketTargetEquipItem : Header
{
	public NetErrorCode errorCode;


    public string TargetUserPlatform;
	public long TargetUserIndexID;
	public int TargetCharacterIndex;
	
	public string Account;
	public int IsFriend;
	public TargetInfoAll [] Infos;	// 0: 1: 2:
	
	public PacketTargetEquipItem() { this.MsgID = NetID.TargetEquipItem; }
}

public class PacketArenaRanking : Header
{
	public NetErrorCode errorCode;
	public int RankType;
	public bool bDown;					// int ranking 아래등수 10개 요청.
	public ArenaRankingInfo [] rankingList;	// 10개.
	
	public PacketArenaRanking() { this.MsgID = NetID.ArenaRanking; }
}

		
public class PacketEnterPost : Header
{
	public PacketEnterPost() { this.MsgID = NetID.EnterPost; }
}

public class MailReward
{
    public int ItemID;				// 첨부아이템.
    public int Gold;				// 첨부골드.
    public int Jewel;				// 첨부보석.
    public int Stamina;				// 첨부스태미너.
    public int coupon;				// Gamble 쿠폰.
    public int ticket;				// 투기장 티켓.
    public int potion1;				// 포션1
    public int potion2;				// 포션2
    public int awaken_point;
}

public class MailInfo 
{
    public string reward_id;        // hive5.
    public string Index;			// 우편인덱스.
	public MailType Type;			// 메일타입 0:받기 1:보기.
	public int bOpened;				// 본메세지인지.
    public MailReward[] rewards;    // 첨부아이템
	public string Title;			// 제목.
	public string Sender;			// 보낸사람.
	public int bMsg;				// 내용있는지.
	public DateTime CreateTime;		// 받은날짜.
	public int LeftDestroySec;			// 삭제될남은초.
}

// 이패킷은 여러번 올수 있다. 
// 메일을 일정 수로 나누어 여러번 보낸다. bContinue:false이면 메일리스트를 다 받았음.
public class PacketMailList : Header
{
	public NetErrorCode errorCode;
	public int bContinue;		// 1:받을 메일이 더있음. 0:다받았음.
	public MailInfo [] Infos;		// MailInfo
			
	public PacketMailList() { this.MsgID = NetID.MailList; }
}

public class PacketRequestPostItem : Header
{
	
	public int CharacterIndex;
	public string mailIndex;
			
	public PacketRequestPostItem() {this.MsgID = NetID.RequestPostItem; }
}

public class PacketRequestPostItemAll : Header
{
	
	public int CharacterIndex;
			
	public PacketRequestPostItemAll() {this.MsgID = NetID.RequestPostItemAll; }
	
}

public class PacketRequestPostMsg : Header
{

    public string mailIndex;
	
	public PacketRequestPostMsg() { this.MsgID = NetID.RequestPostMsg; }
}

public class PacketPostMsg : Header
{
	public NetErrorCode errorCode;
	public string mailIndex;
	public string title;
	public string Msg;
	
	public PacketPostMsg() { this.MsgID = NetID.PostMsg; }

}

public class PacketPostItem : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public string mailIndex;
	public int totalJewel;				// 총액. Set해준다. -1이면 SET하지 않는다.
	public int totalGold;				// 총액. Set해준다. -1이면 SET하지 않는다.
	public int Stamina;					// 증가분. Add해준다.
	public int totalCoupon;				// 총액. Set해준다. -1이면 SET하지 않는다.
	public int totalTicket;				// 투기장 입장권..
	public int potion1Present;			// 포션1 선물?
	public int potion2Present;			// 포션2 선물
	public ItemDBInfo[] Infos;			// 인벤토리나 쿄스춤에 넣어준다.
	
	public PacketPostItem() { this.MsgID = NetID.PostItem; }
}

public class PacketPostItemAll : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public string [] mailIndexs;		// 보기,받기버튼 비활성화.
	public int totalJewel;
	public int totalGold;
	public int Stamina;
	public int totalCoupon;			// 총액. Set해준다. -1이면 SET하지 않는다.
	public int totalTicket;			// 투기장 입장권..
	public int potion1Present;			// 포션1 선물?
	public int potion2Present;			// 포션2 선물
	
	public ItemDBInfo [] normalItems;
	public CostumeItemDBInfo[] costumeItems;
	public CostumeItemDBInfo[] costumeSetItems;
	public MaterialItemDBInfo[] materialItems;
	
	public PacketPostItemAll(){this.MsgID = NetID.PostItemAll;}
}



public class PacketSupplyTicket : Header
{
	public int CharacterIndex;
	public int Ticket;			// SET한다.
	
	public PacketSupplyTicket() { this.MsgID = NetID.SupplyTicket; }
}


public class PacketEnterMessenger : Header
{	
	
	
	public PacketEnterMessenger() { this.MsgID = NetID.EnterMessenger; }
}

public class FriendInfo : FriendSimpleInfo
{
	public int coolTimeSec = -1;	// 스태미나남은시간.(24시간)
}

public class FriendSimpleInfo
{
	public long UserID = -1;
	public int CharID = 0;
	public int Lv = -1;				// 레벨.
	public DateTime connTime; 	// 최근접속시간.
	public string nick;
    public string platform; // kakao 따위
	public bool ShowProfileImage;
}

public class PacketRecommandFriendList : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public FriendSimpleInfo [] Friends;
			
	public PacketRecommandFriendList() { this.MsgID = NetID.RecommandFriendList; }
}

public class PacketInvitedUserList : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public FriendSimpleInfo [] Friends;
	
	public PacketInvitedUserList() { this.MsgID = NetID.InvitedUserList; }
}

public class PacketFriendList : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public FriendInfo [] Friends;
	
	public PacketFriendList() { this.MsgID = NetID.FriendList; }
}

public class PacketFriendInviteByNickName : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public string InvitedNickName;		// 초대할 유저.
			
	public PacketFriendInviteByNickName() { this.MsgID = NetID.FriendInviteByNickName; }
}

public class PacketFriendInvite : Header
{
	public NetErrorCode errorCode;
	
	public long InvitedUserID =-1;		// 초대할 유저.
			
	public PacketFriendInvite() { this.MsgID = NetID.FriendInvite; }
}

public class PacketFriendInviteAccept : Header
{
	public NetErrorCode errorCode;
	
	public long Friend = -1;		// 초대한유저아이디.

	public FriendInfo Info;			// FriendInfo
			
	public PacketFriendInviteAccept() { this.MsgID = NetID.FriendInviteAccept; }			
}

public class PacketFriendDelete : Header
{
	public NetErrorCode errorCode;
	
	public long FriendID = -1;		// 초대한유저아이디.
	
	public PacketFriendDelete() { this.MsgID = NetID.DeleteFriend; }			

}

public class PacketSendStaminaToFriend : Header
{
	public NetErrorCode errorCode;
	
	public long FriendID;
	
	// 서버가 채워준다.
	public int coolTimeSec = -1;	// 스태미나남은시간.(초), 0 이면 보낼수 있다. 자정이 되면 쿨타임리셋. 
			
	public PacketSendStaminaToFriend() { this.MsgID = NetID.SendStaminaToFriend; }						
}

public class AchievementDBInfo
{
	public int characterIndex = -1;		// 캐릭터 공통업적은 -1로 세팅된다.
	
	public int [] groupIDs;
	public int [] counts; 	
}

public class AchievementClearInfo
{
	public int characterIndex = -1;		// 캐릭터 공통업적은 -1로 세팅된다.
	
	public int [] groupIDs;
	public int [] stepIDs; 	// groupIDs:[1,2], stepIDs[5,5] 이면 1,2,3,4 스텝도 보상받은것.
	
}

public class PacketAchievementCompleteInfo : Header
{
	public AchievementClearInfo[] Info;
	public PacketAchievementCompleteInfo() { this.MsgID = NetID.AchievementCompleteInfo; }
}

public class PacketAchievementInfo : Header		// 최대Step까지 완료하고 보상받았다면 보내지않음.
{
	public AchievementDBInfo[] Info;
	public PacketAchievementInfo() { this.MsgID = NetID.AchievementInfo; }
	
}

public class PacketAchievementProgress : Header	// 완료목록에서 맥스치까지 클리어했다면 다시 보내지말것.
{
	public NetErrorCode errorCode;
	public int characterIndex = -1;
	public int [] groupIDs;
	public int [] counts;
		
	public PacketAchievementProgress() { this.MsgID = NetID.AchievementProgress; }
}

public class PacketAchievementReward : Header
{
	public NetErrorCode errorCode;
	public int characterIndex = -1;
	
	public int groupID =-1;
	public int stepID = -1;
	
	public PacketAchievementReward() { this.MsgID = NetID.AchievementReward; }
}

public class DailyMission
{
	public int id = 0;
	public int count = 0;
	public int bReward = 0;	// 1이면 보상받았음.
}

public class PacketDailyMissionInfo : Header
{
	public DateTime expiredTime;			// 만료날짜. 다음날이되면 새로운 미션을 받는다.
	public DailyMission [] Infos;
	
	public PacketDailyMissionInfo() { this.MsgID = NetID.DailyMissionInfo; }
}


public class PacketDailyMissionProgress : Header
{
	public NetErrorCode errorCode;
	
	public int [] ids;
	public int [] counts;
	
	public PacketDailyMissionProgress() { this.MsgID = NetID.DailyMissionProgress; }
}

public class PacketDailyMissionReward : Header
{
	public NetErrorCode errorCode;

	public int id =-1;
	
	public PacketDailyMissionReward() { this.MsgID = NetID.DailyMissionReward; }
}

public class PacketPreLoginDone : Header
{
	public NetErrorCode errorCode;
	public int bEncrpyt;	// 1:enable;
	
	public PacketPreLoginDone() { this.MsgID = NetID.PreLoginDone; }
}


public class PacketBossRaidEnter : Header
{
	public PacketBossRaidEnter() { this.MsgID = NetID.BossRaidEnter; }
}

public class PacketBossAppear : Header
{
	public int id;		// BossRaidTable::id	
	public PacketBossAppear() { this.MsgID = NetID.BossAppear; }
}

public class PacketBossRaidStart : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public long index;
	
	// 서버에서 채워준다.
	public int curStamina;
	public int presentStamina;
	public int transform;
	
	public PacketBossRaidStart() { this.MsgID = NetID.BossRaidStart; }
}

public class PacketBossRaidStartRecoveryStamina : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
    public long index;
	// 서버에서 채워준다.
	public int curStamina;
	public int presentStamina;
	public int transform;
	
	public int totalGold;
	public int totalJewel;
	
	public PacketBossRaidStartRecoveryStamina() { this.MsgID = NetID.BossRaidStartRecoveryStamina; }
}

public class PacketBossRaidEnd : Header
{
	public NetErrorCode errorCode;
	public long index;
	public int damage;		// 내가준 데미지량.
	public int transform;		// 1:광포화.
	public int bossHP;			// 현재 보스HP량
	// 서버에서 채워준다.
	//public intbossKilled;		// 1:죽었음.
	public int bClear;			// 1:클리어.
	public int bTop;			// 1:내가 데미지량 탑.
		
	public PacketBossRaidEnd() { this.MsgID = NetID.BossRaidEnd; }
}

public class PacketBossRaidInfo : Header
{
	public int [] index;
	public int [] bossID;
	public int [] die;					// die 1, 0 살아 있음.
	public int [] destroyLeftSec;		// 남은시간 (초).
	public string [] finder;			// 발견자.
	public int [] HP;					// 현재 HP. Max는 BossRaidTable.txt.
	public string [] topNick;			// 피해량 1위 닉네임.
	public int [] topDamage;			// 1위 피해량.
	public int [] totalDamage;			// 내가입힌 피해량.
	public int [] transform;			// 광포화.	
	public string [] hunter;			// 최후의 일격.
	public PacketBossRaidInfo() { this.MsgID = NetID.BossRaidInfo; }
}

// PageName: Stamina.php
public class PacketUpdateStamina : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public int curStamina;
	
	public PacketUpdateStamina() { this.MsgID = NetID.UpdateStamina; }
}


public class PacketPopupNotice : Header
{
	public long [] IDs;			
	public string [] ImageUrls;	// 이미지공지.
	public string [] LinkUrls;		// 이미지를 누르면 웹페이지를 띄워준다.
	public string [] Contents;	// 텍스트공지 (둘중하나이다).
	public int [] Orders;			// 팝업순서(작은순부터 위로).
	
	public PacketPopupNotice() { this.MsgID = NetID.PopupNotice;}
}

// 오늘은 그만보기.
public class PacketPopupNoticeIgnore : Header
{
	public long ID;
	
	public PacketPopupNoticeIgnore() { this.MsgID = NetID.PopupNoticeIgnore;}
}

public class PacketRequestGambleInfo : Header
{
	public int CharacterIndex;

	public PacketRequestGambleInfo() { this.MsgID = NetID.RequestGambleInfo;}
}
// 스테이지중 부활하기. Revival.php
public class PacketRevival : Header
{
	public int errorCode;
	public int CharacterIndex;
	
	public int TotalGold;
	public int TotalCash;
	
	public PacketRevival() { this.MsgID = NetID.Revival; }
}

public class PacketCreateNickName : Header
{
	public NetErrorCode errorCode;		// 에러가 발생하면 이패킷을 다시 돌려준다.
	public string NickName;
	public NetConfig.PublisherType publisherID;
	
	public PacketCreateNickName() { this.MsgID = NetID.CreateNickName; }
}

public class PacketCheckNickName : Header
{
	public NetErrorCode errorCode;		
	public string NickName;
	
	public PacketCheckNickName() { this.MsgID = NetID.CheckNickName; }
}


// 인벤토리 리뉴얼. 추가되는 패킷.

public class PacketInvenNormalInfo : Header
{
	public string [] UIDs;
	public int [] IDs;
	public int [] Grades;
	public int [] Counts;
	public int [] Reinforces;
	public int [] Rates;
	public int [] Exps;
	
	public PacketInvenNormalInfo() { this.MsgID = NetID.InvenNormalInfo; } 
}

public class PacketInvenCostumeInfo : Header
{
	public string [] UIDs;
	public int [] IDs;

	public PacketInvenCostumeInfo()	{  this.MsgID = NetID.InvenNormalInfo;	}
}

public class PacketInvenCostumeSetInfo : Header
{
	public string [] UIDs;
	public int [] IDs;

	public PacketInvenCostumeSetInfo()	{  this.MsgID = NetID.InvenCostumeSetInfo;	}
}

public class PacketMaterialInfo : Header
{
	public string [] UIDs;
	public int [] IDs;
	public int [] Counts;

	public PacketMaterialInfo()	{  this.MsgID = NetID.InvenMaterialInfo;	}
}

public class PacketInvenExtendInfo : Header
{
	public NetErrorCode errorCode;			//
	public int []bNormalInven; 	// 1:아이템탭, 0:재료탭.
	public int []Count;				// server side;

	public PacketInvenExtendInfo()	{  this.MsgID = NetID.InvenExtendInfo;	}
}


public class PacketInvenExtend : Header
{
	public NetErrorCode errorCode;			//
	public int CharacterIndex;
	public int bNormalInven; 	// 1:아이템탭, 0:재료탭.
	
	public int Count;				// server side;
	public int totalJewel;			// 총 보석.			
	public int totalGold;			// 총 골드.
	
	public PacketInvenExtend()	{  this.MsgID = NetID.InvenExtend;	}
}

public class MaterialItemDBInfo
{
	public string UID;
	public int ID;
	public int Count;
}

public class EtcItemDBInfo
{
	public string UID;
	public int ID;
}

public class CostumeItemDBInfo
{
	public string UID;
	public int ID;
}

public class PacketUnwearCostumeSetItem : Header
{
	public int CharacterIndex;
	public string UID;
	
	public NetErrorCode errorCode;
	public CostumeItemDBInfo UnwearItem;
	
	public PacketUnwearCostumeSetItem()	{  this.MsgID = NetID.UnwearCostumeSetItem;	}
}

public class PacketWearCostumeSetItem : Header
{
	public int CharacterIndex;
	public int ItemID;
	public string UID;
	public int slotIndex;
	
	public NetErrorCode errorCode;
	public CostumeItemDBInfo WearItem;
	public int UnwearItem;			//0 : setItem, 1: costume
	public CostumeItemDBInfo[] UnwearItems;
	
	public PacketWearCostumeSetItem()	{  this.MsgID = NetID.WearCostumeSetItem;	}
}

public class PacketUnwearCostumeItem : Header
{
	public int CharacterIndex;
	
	public int ItemID;
	public string UID;
	public int slotIndex;
	
	public NetErrorCode errorCode;
	public CostumeItemDBInfo UnwearItem;
	
	public PacketUnwearCostumeItem()	{  this.MsgID = NetID.UnwearCostumeItem;	}
}

public class PacketWearCostumeItem : Header
{
	public int CharacterIndex;
	
	public int ItemID;
	public string UID;
	public int slotIndex;
	
	public int equipSlotIndex;
	
	public NetErrorCode errorCode;
	public CostumeItemDBInfo WearItem;
	
	public int UnwearItemType;
	public CostumeItemDBInfo UnwearItem;
	
	public PacketWearCostumeItem() { this.MsgID = NetID.WearCostumeItem; }
}

public class PacketRequestServerChecking : Header
{
	public int version;				// Version.NetVersion
	public string cookie;			// XignCode.cookie
	
	public PacketRequestServerChecking() { this.MsgID = NetID.ServerChecking;}
}

public class PacketServerChecking : Header
{
	public NetErrorCode errorCode; // OK:이면 점검중아님. ServerChecking;
	public string ImageUrl;	// 이미지공지.
	public string Content;	// 텍스트공지 (둘중하나이다).
	
	public PacketServerChecking() { this.MsgID = NetID.ServerChecking;}
}

public class PacketNeedUpdateApp : Header
{
	public string url;
	
	public PacketNeedUpdateApp() { this.MsgID = NetID.NeedUpdateApp;}
}

public class AndroidExitWindow
{
	public int windowType;
	public string Title;
	public string Message;
	
}

public class PacketCoupon : Header
{
	public NetErrorCode errorCode;	// [9046 ~ 9050]
	
	public int CharacterIndex;
	public string serialno;	// 하이픈빼고 16자리.

	public PacketCoupon() { this.MsgID =  NetID.Coupon; }
}

public class PacketAttandanceCheck : Header
{
	public int checkday;	// 여기까지 보상받았음.보상은 우편에. 1부터시작.
	
	public PacketAttandanceCheck() { this.MsgID = NetID.AttandnceCheck; }
}

public class PacketBadgeNotify : Header
{
	public int [] windowTypes;		// eTOWN_UI_TYPE
	public int [] Tab;					// 0,1,2,3 탭순서.
	
	public PacketBadgeNotify() { this.MsgID = NetID.BadgeNotify; }
}

public class PacketReviewPlease : Header
{
	public string url;	// 여기까지 보상받았음.보상은 우편에. 1부터시작.
	
	public  PacketReviewPlease() { this.MsgID = NetID.ReviewPlease; }
}

public class PacketRequestReviewReward : Header
{
	public NetErrorCode errorCode;
	
	public  PacketRequestReviewReward() { this.MsgID = NetID.RequestReviewReward; }
}

public class PacketIgnorePush : Header
{
	public int off;
	
	public  PacketIgnorePush() 	{ this.MsgID = NetID.IgnorePush; }
}

public class PacketShowEvent : Header		
{
	public int CharacterIndex;
	
	public int eventID;	// 뭔가 이벤트 공용으로 쓰려는데 가능하려나... 1.레벨업이벤트. 2
	public int Step;		// 0:레벨 5보상. 1:레벨10 보상. 2:레벨 15보상.
	public PacketShowEvent() { this.MsgID = NetID.ShowEvent; }
}

public class PacketDropout : Header
{
	public NetErrorCode errorCode;
	
	public string AccountID;
	public string Password;
	public string publisherID;
	
	public AccountType EmailType;
	
	public PacketDropout() { this.MsgID = NetID.Dropout; }
}

public class PacketStageTutorial : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	
	public ItemDBInfo item;
	public MaterialItemDBInfo[] gainMaterialItems;
	public ItemDBInfo[] gainNormalItems;
	
	public int rewardEXP;
	
	public PacketStageTutorial() { this.MsgID = NetID.StageTutorial; }
}

public class PacketTutorialDone : Header
{
	public NetErrorCode errorCode;
	
	public int CharacterIndex;
	
	public PacketTutorialDone() { this.MsgID = NetID.TutorialDone; }
}

public class PacketEventShopInfo : Header
{
	public DateTime Now;		//현재시간.
	
	public int[] eventTypes;
	public int[] leftTimes;		//남은 시간.
	
	public int[] limitCount; 	//최대구매횟수.
	public int[] buyCount;		//현재 구매 횟수.
	
	public PacketEventShopInfo() { this.MsgID = NetID.EventShopInfo; }
}

public class PacketBuyCashLimitItem : PacketBuyCashItem
{
	public int limitCount;
	public int buyCount;
	
	public int cashEventType;	//1: 특가 상품, 2:패키지 상품.
	
	public PacketBuyCashLimitItem() { this.MsgID = NetID.BuyCashLimitItem; }
}

public class PacketRecvEmpty : Header
{
	public PacketRecvEmpty() { this.MsgID = NetID.RecvEmpty; }
}

public class SpecialMissionDBInfo
{
	public int[] groupIDs;
	public int[] characterIndexs;
	public int[] counts;
}

public class PacketSpecialMissionInfo : Header
{
	public System.DateTime now;
	public System.DateTime startTime;
	public System.DateTime endTime;
	
	public int eventType;
	public string url;
	
	public SpecialMissionDBInfo Info;
	
	public PacketSpecialMissionInfo() { this.MsgID = NetID.SpecialMissionInfo; }
}

public class PacketSpecialMissionProgress : Header
{
	public NetErrorCode errorCode;
	public int characterIndex;
	public int[] groupIDs;
	public int[] counts;
	
	public PacketSpecialMissionProgress() { this.MsgID = NetID.SpecialMissionProgress; }
}

public class PacketSpecialMissionReward : Header
{
	public NetErrorCode errorCode;
	public int characterIndex;
	public int groupID;
	public int stepID;
	
	public PacketSpecialMissionReward() { this.MsgID = NetID.SpecialMissionReward; }
}

public class PacketSpecialMissionCompleteInfo : Header
{
	public int[] groupIDs;
	public int[] characterIndexs;
	public int[] stepIDs;
	
	public PacketSpecialMissionCompleteInfo() { this.MsgID = NetID.SpecialMissionCompleteInfo; }
}

public class PacketTimeLimitItemInfo : Header
{
	public int[] leftTimes;	//종료 날짜
	public int[] ItemIDs;				//사용아아템.TimeLimitItemTable.id
	
	public PacketTimeLimitItemInfo() { this.MsgID = NetID.TimeLimitItemInfo; }
}

public class PacketInvokeTimeLimitItem : Header
{
	public int leftTime;
	public int ItemID;
	
	public PacketInvokeTimeLimitItem() { this.MsgID = NetID.InvokeTimeLimitItem; }
}

public class PacketAwakningUpgradeSkill : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	
	public SkillUpgradeDBInfo Info;
	
	public int APoint;
	public int ABuyPoint;
	
	public int TotalCash;
	public int TotalGold;
	
	public PacketAwakningUpgradeSkill() { this.MsgID = NetID.AwakeningUpgreadeSkill; }
}

public class PacketAwakeningBuyPoint : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	public int Count;
	
	public int APoint;			//구매포인트 수
	public int ABuyPoint;
	
	public int TotalGold;
	public int TotalCash;
	
	public PacketAwakeningBuyPoint() { this.MsgID = NetID.AwakeningBuyPoint; }
}

public class PacketAwakeningResetSkill : Header
{
	public NetErrorCode errorCode;
	public int CharacterIndex;
	
	public int APoint;		//각성 포인트.
	public int ABuyPoint;	//구매 포인트.
	public int TotalGold;
	public int TotalCash;
	
	public PacketAwakeningResetSkill() { this.MsgID = NetID.AwakeningResetSkill; }
}

public class PacketAwakeningInfo : Header
{
	public int CharacterIndex;
	public SkillDBInfo Info;
	
	public PacketAwakeningInfo() { this.MsgID = NetID.AwakeningSkillInfo; }
}

public class PacketEventList : Header
{
	public int[] eventTypes;	//1레벨업. 2: 출석 3:특수임무 4; 겜블확률 5: 스태미너 반값.
	public int[] leftTimes;
	public int[] values;		//4일때 확률업%, 5일대 할일%.
	
	public PacketEventList() { this.MsgID = NetID.EventListInfo; }
}

public class PacketRandombox : Header
{
	public NetErrorCode errorCode;
	public int itemID;
	public int itemGrade;
	public int itemRate;
	
	public int gold;
	public int meat;
	public int buffPackDay;
	public int coupon;
	
	public PacketRandombox() { this.MsgID = NetID.RandomBox; }
}

public class PacketSpecialStage : Header
{
	public int[] open;
	
	public PacketSpecialStage() { this.MsgID = NetID.SpecialStage; }
}

public class KakaoFriendInfo
{
	public string user_id = "";
	public string nickname = "";
	public string friend_nickname = "";
	public string profile_image_url = "";
	public string message_blocked = "";
	public string hashed_talk_user_id = "";
	public string supported_device = "";
	
	public bool isInvited = false;
	
	public KakaoFriendInfo()
	{
		
	}
	
	public KakaoFriendInfo(string user_id, string nickname, string friend_nickname, string profile_image_url, 
							string message_blocked, string hashed_talk_user_id, string supported_device)
	{
		this.user_id = user_id;
		this.nickname = nickname;
		this.friend_nickname = friend_nickname;
		this.profile_image_url = profile_image_url;
		this.profile_image_url = profile_image_url;
		this.message_blocked = message_blocked;
		this.hashed_talk_user_id = hashed_talk_user_id;
		this.supported_device = supported_device;
	}
}