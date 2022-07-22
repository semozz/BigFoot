public enum NetErrorCode
{
	OK = 0,   
	None =  	0,   
	
	// System Error
	PacketCodeInvalid = 			8001,
	DataNotFound =  					8002,   				// 데이터가 없습니다.
    ExceptionOnProcedure = 8003,   				// exception에러.
	
	NotFoundSellItem = 				8005,   				// 아이템테이블에 존재하지 않는다.
	ItemTablePriceZero = 			8006,					// 아이템가격이 0원이다.
	DBTableInvalid =					8007,					// DBCSVTable 값이 Invalid.
	Sync =								8008,					// 서버와 값이 상이하다. 
	GambleInvalid =	 				8009,					// 유저가 보내온 겜블데이터가 이상하다.
	FakeData =							8010,					// 조작의심.	                                                          
	MailType =							8011,					// 메일타입과 패킷이 맞지않다.
	EncryptKeyNotFound	=			8012,					// 암호화키 검색실패
	TokenCheckFailed	= 				8013,					// 토큰인증실패	
	JsonException	=					8014,					// 에러코드 보이면서 재시작.
	               
	// UserError
	Unknown = 							9000,
	LoginAccount_Duplicate =  	9001,   				// 계정생성시 계정아이디중복.
	LoginPassword_Wrong =  		9002,   				// 계정선택시 비번틀렸음.
	NotEnoughGold = 		 			9003,   				// 골드가 부족합니다.
	NotEnoughCash = 		 		9004,   				// 캐쉬가 부족합니다.
	NotEnoughInven = 		 		9005,   				// 창고 공간이 부족합니다.
	NotEnoughCostumeInven = 	9006,   				// 쿄스튬 창고가 부족합니다.
	NickNameInvalid = 		 		9007,					// 사용할 수 없는 문자가 들어 있습니다.
	NickNameInvalidLength =  		9008,					// 닉네임은 2~8 자로 만들어 주세요.
	ReinforceAlreadyMax=	 		9009,
	CompositionNotReinforceMax=	9010,					// 합성할 아이템 강화가 맥스가 아이다.
	CompositionGradeMax=	 		9011,					// 합성할 아이템 등급이 맥스다.
	CompositionNotExistItem=		9012,					// 합성될 아이템이 없다.	
	NotEnoughStamina = 			9013,					// 행동력이 부족합니다.
	NotFoundTarget = 				9014,					// 타겟을 찾을 수 없음.
	NotOpenArea =					9015,					// 오픈하지 않았음.				
	NotEnoughTicket =				9016,					// 티켓이 부족하다.
	NoMoreRankingData=				9017,					// 랭킹데이터끝.
	NotEnoughSkillPoint =			9018,					// 스킬포인트가 모자라다.
	NotOpenWave	=					9019,					// 안열었다.	
	BlockedID = 						9020,					// 이용정지된 계정입니다.
	AlreadyOpenedMail =				9021,					// 이미 열어본 메일입니다.
	NotEnoughFriend =				9022,					// 친구제한수에 걸렸음.
	NotEnoughFriendFriend = 		9023,					// 친구의 친구제한수가 결렸음.
	NotCoolTime =						9024,					// 스태미너 제한시간이 걸렸음.	
	NotEnoughFriendInvite =		9025,					// 친구요청제한수에 걸렸음
	NotEnoughTarget =				9026,					// 조건이 충족되지 않았음.
	AlreadyAchievementReward = 9027,					// 이미 업적보상받았다.
	AlreadyDailyMissionReward =	9028,					// 이미 일일임무보상받았다.
	AlreadyClearWave	=				9029,					// 디펜스를 모두 클리어 하였습니다. 이어하기 할 수 없습니다. 
	NotConnected	=					9030,					// 서버와 연결 할 수 없습니다.
	DuplicateConnection =			9031,					// 다른기기에서 연결 되였습니다.
	BossAlreadyDeath = 				9032,					// 팝업 제압실패.이미사망.
	BossRunAway = 					9033,					// 팝업 제압실패.이미도망.
	AccountIDNotEmail =				9034,					// 계정이 이메일이 아니다.
	NickNameDuplicate =				9035,					// 닉네임중복.
	AccountIDInvalid = 				9036,					// 유효하지 않은 계정입니다.	
	NickNameNotFound = 			9037,					// 닉네임을 찾을 수 없습니다.
	NotEnoughMedal = 				9038,					// 메달이 모자르다.
	InvenExtendMax =				9039,					// 인벤토리를 더이상 확장 할 수 없습니다.
	CompositionFailed=				9040,					// 합성실패.
	NotEnoughInvenMaterial = 		9041,					// 재료인벤이 충분하지 않습니다.
	WebTimeOut = 					9042,					// 웹요청 타임아웃.
	ServerChecking = 				9043,					// 서버점검중.
	NotForSale = 						9044,   				// 아이템테이블에 존재하나 팔지않는아이템.
	NotEnoughGambleCoupon = 	9045,					// 겜블 쿠폰 부족.
	CouponInvalidDate =                                9046,                                   // 유효기간이 지난 쿠폰이다.
	UsedCoupon =                                        9047,                                   // 이미 사용된 쿠폰입니다.
	UseCountOverCoupon =                        9048,                                   // 쿠폰은 계정당 하루 1회만 사용 가능해요.
	CountOverCoupon =                                9049,                                   // 쿠폰입력횟수는 하루 최대 5회 입니다.
	CouponInvalid        =                                        9050,                                   // 유효하지 않은 쿠폰입니다.
	AlreadyReviewReward = 9051,
	XignCodeInvalid = 				9052,					// 비정상적인 클라이언트입니다.
    AlreadyTutorialDone =           9053,                   // 투토리얼 보상을 이미 받았습니다.
	DropoutAccount = 				9056,					//탈퇴된 계정입니다.
	NotFoundAccountID = 			9057,					//없는 계정입니다.
	LimitedInviteFriend =			9058,					//하루에 요청가능한 친구수를 넘겼습니다.
    RECIVE_SPECIAL_MISSION_REWARD = 9059,                  //특수임무 보상을 받았습니다.
    CLOSE_EVENT =                  9060,                   //이벤트 기간이 종료 되었습니다.
    NOT_ENOUGH_AWAKEN_POINT =      9061,                   //각성 포인트가 부족합니다.
    LIMIT_AWAKEN_POINT =           9062,                   //능력치 최대 포인트는 1000입니다.
    NOT_A_FRIEND =                 9063,                   //친구가 아니다.
	//NomoreReward =            		9063,                   // 더이상 뽑을 아이템이 없다.
    NeedUpdateApp = 9065,                   // 앱업데이트가 필요합니다.
}

