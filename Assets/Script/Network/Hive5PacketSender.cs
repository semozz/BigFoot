using Hive5;
using Hive5.Model;
using Hive5.Util;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

public class Hive5PacketSender : IPacketSender
{
    private ClientConnector connector;

    public ClientConnector Connector
    {
        get
        {
            return this.connector;
        }
        set
        {
            this.connector = value;

            if (hive5Process != null)
                hive5Process.Connector = this.connector;
        }
    }

    Hive5Client hive5 { get { return Hive5Client.Instance; } }
    Hive5Process hive5Process;

    NetErrorCode result;

    public Hive5PacketSender()
    {
		Logger.DebugLog(string.Format("Hive5Packet Sender Create"));
		
        hive5Process = new Hive5Process() { Connector = this.Connector };
        hive5.Init(Hive5Config.AppKey, SystemInfo.deviceUniqueIdentifier, Hive5APIZone.Production);

#if UNITY_EDITOR
        hive5.SetDebug();
#else
        if (hive5.Zone != Hive5APIZone.Production)
        {
            hive5.SetDebug();
        }
#endif
    }

    public void RequestCheckNickName(string nickName)
    {
        hive5.CheckNicknameAvailability(nickName, (response) =>
        {
            var loginPage = GameUI.Instance.loginPage;
            if (loginPage != null)
            {
                loginPage.OnCheckNickResult(ErrorCodeConverter.Convert(response.ResultCode));
            }

            if (response.ResultCode == Hive5ResultCode.Success)
            {
                connector.Nick = nickName;
            }
        });
    }

    public void RequestCreateNickName(string nickName)
    {
        switch (connector.tempAccountType)
        {
            case AccountType.Kakao:
            case AccountType.MonsterSide:
                {
                    hive5.SetNickname(nickName, (response) =>
                        {
                            LoginPage loginPage = GameUI.Instance.loginPage;
                            if (response.ResultCode != Hive5ResultCode.Success)
                            {
                                Logger.DebugLog("Recv Request CreateNickName " + connector.UserIndexID);
                                if (loginPage != null)
                                    loginPage.OnCreateNickName();
                            }
                            else
                            {
                                if (loginPage != null)
                                    loginPage.OnCheckNickResult(ErrorCodeConverter.Convert(response.ResultCode));
								
								if (Game.Instance.AndroidManager != null)
								{
									Game.Instance.AndroidManager.SendPartyTrackEvent("Nickname");
								}
					
								var parameters = new Hive5.Util.TupleList<string, string>();
                                parameters.Add("name", nickName);
                                hive5.CallProcedure("get_user", parameters, (callProcedureResponse) =>
                                {
                                    if (callProcedureResponse.ResultCode == Hive5ResultCode.Success)
                                    {
                                        var callProcedureResult = callProcedureResponse.ResultData as CallProcedureResponseBody;
                                        if (callProcedureResult == null)
                                            return;

                                        try {
                                        	this.hive5Process.GetUserProcess(callProcedureResult.CallReturn);
										}
										catch(Exception ex)
										{
											if (GameUI.Instance.MessageBox != null)
												GameUI.Instance.MessageBox.SetFatalError(NoticePopupWindow.Fatal_Error.GetUserError, getUserErrorStringID);
										}
                                    }
                                });
                            }
                        });
                }
                break;
            case AccountType.GooglePlus:
                break;
            case AccountType.Facebook:
            default:
                break;
        }

    }

    public void RequestInviteFriendByKatalk(string Nick)
    {
        //throw new NotImplementedException();
    }

    public void RequestInviteFriendByNick(string nickName)
    {
        var data = new
        {
            nickname = nickName
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("request_friend", data, (response) =>
        {
            GameUI.Instance.CancelWait();
            FriendWindow friendWindow = GameUI.Instance.friendWindow;
            if (friendWindow != null)
                friendWindow.requestCount = 0;

            FriendInviteWindow inviteWindow = GameUI.Instance.friendInviteWindow;
            if (inviteWindow != null)
                inviteWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(nickName);

                hive5Process.ErrorProcess(response, "RequestInviteFriendByNick", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (inviteWindow != null)
            {
                inviteWindow.OnErrorMessage(result);
            }
        });
    }

    public void RequestSignup(string id, string pass1, string pass2)
    {
        hive5.CheckPlatformEmailAvailablity(id, (response) =>
        {

            if (response.ResultCode == Hive5ResultCode.Success)
            {
                // save temp
                connector.tempAccountType = AccountType.MonsterSide;
                connector.tempAccountID = id;
                connector.tempPass = pass1;

                // save login info
                LoginInfo info = Game.Instance.loginInfo;
                if (info != null)
                {
                    info.loginID = id;
                    info.pass = pass1;
                    info.acctountType = AccountType.MonsterSide;
                    info.loginDate = System.DateTime.Now;

                    Game.Instance.SaveLoginData();
                }

                hive5.CreatePlatformAccount(id, pass1, (response2) =>
                    {
                        if (response2.ResultCode == Hive5ResultCode.Success)
                        {
                            hive5.AuthenticatePlatformAccount(id, pass1, (response3) =>
                                {
                                    if (response3.ResultCode == Hive5ResultCode.Success)
                                    {
                                        var responseBody = response3.ResultData as AuthenticatePlatformAccountResponseBody;
                                        if (responseBody == null)
                                            return;

                                        connector.Platform = "bigfoot";
                                        connector.UserIndexID = int.Parse(responseBody.Id);
                                        connector.PlatformUserId = responseBody.Id;
                                        var objectKeys = new string[] { "" };
                                        var configKeys = new string[] { "time_event", "last_version" };

                                        Logger.DebugLog("Token1:" + hive5.AccessToken);
                                        hive5.Login(OSType.Android, objectKeys, configKeys, "bigfoot", connector.PlatformUserId, "0", (loginResponse) =>
                                        {
                                            if (loginResponse.ResultCode == Hive5ResultCode.Success)
                                            {
                                                Logger.DebugLog("Token2:" + hive5.AccessToken);
                                                // show login page
                                                LoginPage loginPage = GameUI.Instance.loginPage;
                                                if (loginPage != null)
                                                    loginPage.OnCreateNickName();
                                            }
                                            else
                                            {
                                                GameUI.Instance.signupWindow.OnErrorSignUp(ErrorCodeConverter.Convert(loginResponse.ResultCode));
                                            }
                                        });
                                    }
                                    else
                                    {
                                        Game.Instance.loginInfo = null;
                                        Game.Instance.SaveLoginData();

                                        GameUI.Instance.signupWindow.OnErrorSignUp(ErrorCodeConverter.Convert(response3.ResultCode));
                                    }
                                });
                        }
                        else
                        {
                            Game.Instance.loginInfo = null;
                            Game.Instance.SaveLoginData();

                            GameUI.Instance.signupWindow.OnErrorSignUp(ErrorCodeConverter.Convert(response2.ResultCode));
                        }
                    }, "", id);
            }
            else
            {
                Game.Instance.loginInfo = null;
                Game.Instance.SaveLoginData();

                GameUI.Instance.signupWindow.OnErrorSignUp(ErrorCodeConverter.Convert(response.ResultCode));
            }
        });
    }

    public void SendAchievementProcess(List<Achievement> achievementList)
    {
        if (achievementList.Count <= 0)
            return;

        List<int> groupIDList = new List<int>();
        List<int> countList = new List<int>();

        int groupID = -1;
        int totalCount = 0;
        foreach (Achievement achievement in achievementList)
        {
            groupID = achievement.id;
            totalCount = achievement.curCount + achievement.addCount;

            Game.Instance.AndroidManager.CallUnityAchievement(groupID, achievement.addCount);

            groupIDList.Add(groupID);
            countList.Add(totalCount);
        }

        var data = new
        {
            hero_type = Connector.charIndex,
            group_ids = groupIDList,
            counts = countList
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("process_achievement", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            if ((NetErrorCode)(int)jsonData["result"] == NetErrorCode.OK)
            {

                int hero_type = jsonData["hero_type"].ToInt();

                int[] groupIDs = JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["group_ids"]));
                int[] counts = JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["counts"]));

                CharInfoData charData = Game.Instance.charInfoData;
                AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

                if (achieveMgr != null)
                    achieveMgr.SetAchieveInfo(hero_type, groupIDs, counts);

                AchievementWindow achieveWindow = GameUI.Instance.achievementWindow;
                if (achieveWindow != null)
                {
                    achieveWindow.requestCount = 0;
                    achieveWindow.UpdateInfo();
                }
            }
        });
    }

    public void SendBossRaidEnd(long bossIndex, float damageValue, bool isPhase2, int curHP, string platform, string owner_id, int boss_id)
    {

        var data = new
        {
            hero_type = Connector.charIndex,
            boss_index = bossIndex,
            boss_id = boss_id,
            owner_platform = platform,
            owner_id = owner_id,
            damage = (int)damageValue,
            transform = isPhase2,
            boss_hp = curHP
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("end_boss_raid", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(bossIndex);
                objects.Add(damageValue);
                objects.Add(isPhase2);
                objects.Add(curHP);
                objects.Add(platform);
                objects.Add(owner_id);
                objects.Add(boss_id);

                hive5Process.ErrorProcess(response, "SendBossRaidEnd", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            Game.Instance.bossRaidEnd = new PacketBossRaidEnd()
            {
                errorCode = result,
                index = jsonData["boss_id"].ToLong(),
                bossHP = jsonData["boss_hp"].ToInt(),
                transform = (bool)jsonData["transform"] == true ? 1 : 0,
                damage = jsonData["damage"].ToInt(),
                bClear = (bool)jsonData["clear"] == true ? 1 : 0,
                bTop = (bool)jsonData["top"] == true ? 1 : 0,
            };

            StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;
            if (stageEndEvent != null)
            {
                stageEndEvent.CreateLoadingPanel();
            }
        });
    }


    public void SendBuyCashItem(int ItemID)
    {
        var parameters = new
        {
            item_id = ItemID.ToString()
        };
		
		TableManager tableManager = TableManager.Instance;
        CashShopInfoTable cashInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
        CashItemInfo cashItemInfo = null;
        if (cashInfoTable != null)
            cashItemInfo = cashInfoTable.GetItemInfo(ItemID);

		string procedureName = "";
		if ( cashItemInfo == null)
			return;
		switch(cashItemInfo.paymentType)
		{
		case ePayment.Cash:
			procedureName = "buy_cash_item_by_iap";
			break;
		default:
			procedureName = "buy_cash_item_without_iap";
			break;
		}
		
        GameUI.Instance.DoWait();

        hive5.CallProcedure(procedureName, parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(ItemID);

                hive5Process.ErrorProcess(response, "SendBuyCashItem", objects.ToArray());

                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            BuyCashItemHandler(cashItemInfo, body.CallReturn);
        });
    }
	
	public void BuyCashItemHandler(CashItemInfo cashItemInfo, string jsonString)
	{
		var jsonData = LitJson.JsonMapper.ToObject(jsonString);
		
		NetErrorCode errorCode = (NetErrorCode)(int)jsonData["result"];

        if (errorCode == NetErrorCode.OK)
        {
            if (jsonData["by_mail"].ToInt() == 1)
                SetPostBadge();

            connector.charInfo.SetGold(jsonData["gold"].ToInt(), jsonData["jewel"].ToInt());

            var item_uids = jsonData["item_uids"];
            var item_counts = jsonData["item_counts"];

            int nCount = Mathf.Min(item_uids.Count, item_counts.Count);

            for (int index = 0; index < nCount; ++index)
            {
                string UID = item_uids[index].ToString();
                int itemCount = item_counts[index].ToInt();

                Item newItem = Item.CreateItem(jsonData["item_id"].ToInt(),
                    UID,
                    jsonData["grade"].ToInt(),
                    jsonData["reinforce"].ToInt(),
                    itemCount,
                    jsonData["rate"].ToInt(),
                    jsonData["exp"].ToInt());
                if (newItem != null)
                    newItem.IsNewItem = true;

                connector.charInfo.AddItem(newItem);
            }
			
			for (int i = 0; i < jsonData["buff_left_sec"]["type"].Count; i++)
			{
				string buff_type = jsonData["buff_left_sec"]["type"][i].ToString();
				int buff_left_sec = (int)jsonData["buff_left_sec"]["diff"][i];
				Logger.DebugLog(string.Format("GetUser :: {0} left_sec : {1}", buff_type, buff_left_sec));
    			if (buff_left_sec > 0)
    		   		this.hive5Process.AddTimeLimitBuffItem(buff_type, buff_left_sec);
			}

            var shop_history = jsonData["shop_history"];

            CharInfoData charData = Game.Instance.charInfoData;

            if (shop_history.Keys.Contains<string>("limited_item") == true)
            {
                JsonData limited_item = shop_history["limited_item"];

                int buyCount = limited_item["ids"].Count;

                EventShopInfoData eventInfo = charData.GetEventShopInfo(eCashEvent.CashBonus);

                if (eventInfo != null)
                    eventInfo.SetCountInfo(buyCount, -1);

                EventShopWindow eventShopWindow = GameUI.Instance.eventShopWindow;
                if (eventShopWindow != null)
                    eventShopWindow.SetLimitInfo(buyCount, eventInfo.limitCount);

            }
            
            if (shop_history.Keys.Contains<string>("starter_pack") == true)
            {
                JsonData starter_pack = shop_history["starter_pack"];

                for (int i = 0; i < starter_pack["ids"].Count; ++i)
                {
                    int id = starter_pack["ids"][i].ToInt();
                    int count = starter_pack["counts"][i].ToInt();

                    charData.SetPackageItem(id, count);                    
                }
            }
        }
		
		BaseCashShopWindow baseCashShopWindow = null;
		
		if (cashItemInfo != null)
        {
            switch (cashItemInfo.cashItemType)
            {
            case eCashItemType.Event:
                baseCashShopWindow = GameUI.Instance.eventShopWindow;
                break;
            case eCashItemType.StartPack:
                baseCashShopWindow = GameUI.Instance.packageItemShopWindow;
                break;
            default:
                baseCashShopWindow = GameUI.Instance.cashShopWindow;
                break;
            }
        }
        
		if (baseCashShopWindow != null)
        {
            baseCashShopWindow.OnResult((NetErrorCode)jsonData["result"].ToInt(), jsonData["item_id"].ToInt());
        }
		else
		{
			if (GameUI.Instance.MessageBox != null)
            {
                string errorMsgStr = "Failed Buy CashItem!!";
              	TableManager tableManager = TableManager.Instance;
				StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
                if (stringTable != null)
                    errorMsgStr = stringTable.GetData((int)errorCode);

                GameUI.Instance.MessageBox.SetMessage(errorMsgStr);
            }
		}
	}
	
    public void SendChangeGambleItem(int index, Item newItem)
    {
		GambleItem gambleItem = new GambleItem();
        gambleItem.ID = newItem.itemInfo.itemID;
        gambleItem.Grade = newItem.itemGrade;
        gambleItem.itemRate = newItem.itemRateStep;

		var data = new 
		{
			changeIndex = index,
			hero_type = connector.charIndex,
			
			gambleItem = gambleItem
		};
		
		hive5.CallProcedure("change_gamble_item", data, (response) =>
        {
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(index);
                objects.Add(newItem);

                hive5Process.ErrorProcess(response, "SendChangeGambleItem", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)(int)jsonData["result"];
            int charIndex = jsonData["hero_type"].ToInt();
            int changeIndex = jsonData["changeIndex"].ToInt();
			
			var gambleItemInfo = jsonData["item"];
            GambleItem newInfo = new GambleItem();
            newInfo.ID = gambleItemInfo["ID"].ToInt();
            newInfo.Grade = gambleItemInfo["Grade"].ToInt();
            newInfo.itemRate = gambleItemInfo["itemRate"].ToInt();
            
            if (errorCode == NetErrorCode.OK)
            {
                GambleWindow gambleWindow = GameUI.Instance.gambleWindow;
                if (gambleWindow != null)
                {
                    Item newGambleItem = Item.CreateItem(newInfo.ID, "", newInfo.Grade, 0, 1, newInfo.itemRate, 0);

                    //////////////////////////////////////////////////////////////////
                    //GambleData Update....
                    CharPrivateData privateData = null;
                    CharInfoData charData = connector.charInfo;
                    if (charData != null)
                        privateData = charData.GetPrivateData(charIndex);

                    int nCount = 0;
                    if (privateData != null && privateData.gambleItemList != null)
                        nCount = privateData.gambleItemList.Count;

                    if (changeIndex >= 0 && changeIndex < nCount)
                        privateData.gambleItemList[changeIndex] = newInfo;
                    ///////////////////////////////////////////////////////////////////

                    gambleWindow.ChangeGambleItem(changeIndex, newGambleItem);
                }
            }
        });
    }

    public void SendDailyAchievementProcess(List<Achievement> achievementList)
    {
        List<int> groupIDList = new List<int>();
        List<int> countList = new List<int>();

        int groupID = -1;
        int totalCount = 0;
        foreach (Achievement achievement in achievementList)
        {
            groupID = achievement.id;
            totalCount = achievement.curCount + achievement.addCount;

            groupIDList.Add(groupID);
            countList.Add(totalCount);
        }

        var data = new
        {
            group_ids = groupIDList,
            counts = countList
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("process_daily_mission", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                int expiredtime = jsonData["expired_time"].ToInt();
                //int[] groupIDs = JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["group_ids"]));
                //int[] counts = JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["counts"]));
                int[] groupIDs = jsonData["ids"].ToArray<int>();
                int[] counts = jsonData["counts"].ToArray<int>();

                CharInfoData charData = Game.Instance.charInfoData;
                AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

                if (achieveMgr != null)
                    achieveMgr.SetDailyAchieve(groupIDs, counts);
            }
        });
    }

    public void SendGMCheat(GMCMD cmd, int Value)
    {
        //throw new NotImplementedException();
    }

    public void SendIgnorePush(bool bToggle)
    {
        GameUI.Instance.DoWait();
        hive5.TogglePushAccept(bToggle, (response) =>
            {
                GameUI.Instance.CancelWait();
                if (response.ResultCode != Hive5ResultCode.Success)
                {
                    GameOption.noticeToggle = false; // on
                    Logger.DebugLog(response.ResultMessage);
                    return;
                }

                GameOption.noticeToggle = true; // off
            });
    }

    public void SendKeepAlive()
    {
        //throw new NotImplementedException();
    }

    public void SendLogin(string Account, string password, AccountType accountType)
    {
        switch (accountType)
        {
            case AccountType.MonsterSide:
                {
                    hive5.AuthenticatePlatformAccount(Account, password, (hive5Response) =>
                    {
                        if (hive5Response == null)
                            return;

                        connector.tempAccountID = Account;
                        connector.tempPass = password;
                        connector.tempAccountType = accountType;

                        if (hive5Response.ResultCode == Hive5ResultCode.Success)
                        {
                            var result = hive5Response.ResultData as AuthenticatePlatformAccountResponseBody;
                            if (result == null)
                                return;

                            Logger.DebugLog("PacketLoginDone");


                            connector.UserIndexID = int.Parse(result.Id);
                            connector.PlatformUserId = result.Id;
                            connector.Platform = "bigfoot";

                            this.SendRegisterGCMID();

                            var objectKeys = new string[] { "" };
                            var configKeys = new string[] { "time_event", "last_version" };

                            Logger.DebugLog("Token1:" + hive5.AccessToken);
                            hive5.Login(OSType.Android, objectKeys, configKeys, "bigfoot", connector.PlatformUserId.ToString(), "0", (loginResponse) =>
                            {
                                if (loginResponse.ResultCode == Hive5ResultCode.Success)
                                {
                                    Logger.DebugLog("Token2:" + hive5.AccessToken);

                                    var parameters = new Hive5.Util.TupleList<string, string>();
                                    parameters.Add("name", connector.Nick);
                                    hive5.CallProcedure("get_user", parameters, (callProcedureResponse) =>
                                        {
                                            if (callProcedureResponse.ResultCode == Hive5ResultCode.Success)
                                            {
                                                var callProcedureResult = callProcedureResponse.ResultData as CallProcedureResponseBody;
                                                if (callProcedureResult == null)
                                                    return;
												
												try {
                                                	this.hive5Process.GetUserProcess(callProcedureResult.CallReturn);
												}
												catch(Exception ex)
												{
													if (GameUI.Instance.MessageBox != null)
														GameUI.Instance.MessageBox.SetFatalError(NoticePopupWindow.Fatal_Error.GetUserError, getUserErrorStringID);
												}
                                            }
											else
												hive5Process.ErrorProcess(callProcedureResponse);
                                        });
                                }
                            });
                        }
                        else
                        {
                            this.hive5Process.ErrorProcess(hive5Response);
                        }
                    });
                }
                break;
            case AccountType.Facebook:
            case AccountType.GooglePlus:
                throw new NotImplementedException();
                break;
        }
    }

    public void SendKakaoLogin(string kakaoUserID)
    {
        var objectKeys = new string[] { "" };
        var configKeys = new string[] { "time_event", "last_version" };


        Logger.DebugLog("SendKakaoLogin Token1:" + hive5.AccessToken);
        hive5.Login(OSType.Android, objectKeys, configKeys, PlatformType.Kakao, kakaoUserID, "0", (loginResponse) =>
        {
            Logger.DebugLog("login result : " + loginResponse.ResultCode);
            connector.PlatformUserId = kakaoUserID;
            connector.Platform = "bigfoot";

            LoginPage loginPage = null;
            if (GameUI.Instance != null)
                loginPage = GameUI.Instance.loginPage;

            if (loginResponse.ResultCode != Hive5ResultCode.Success)
            {
                NetErrorCode errorMsg = ErrorCodeConverter.Convert(loginResponse.ResultCode);

                if (loginPage != null)
                    loginPage.OnLoginError((int)errorMsg);
                else
                {
                    if (GameUI.Instance.MessageBox != null)
                        GameUI.Instance.MessageBox.SetMessage(errorMsg.ToString());
                }
                return;
            }

            if (this.pending_friends_user_ids != null)
            {
                Logger.DebugLog("update friends info : " + pending_friends_user_ids.ToString());

                this.UpdateKakaoFriends(this.pending_friends_user_ids);
            }

            Logger.DebugLog("get_nick_calll"); 
			Game.Instance.AddDelayCall("CheckNickName");

        });
    }

    string[] pending_friends_user_ids = null;

    public void UpdateKakaoFriends(string[] friends_user_ids)
    {
        if (string.IsNullOrEmpty(hive5.AccessToken) == true)
        {
            pending_friends_user_ids = friends_user_ids;
            return;
        }

        pending_friends_user_ids = null;
        hive5.UpdateFriends("kakao", PlatformType.Kakao, friends_user_ids, (friendsResponse) =>
        {
            string infoStr = string.Format("ResultCode {0} - Message {1}", friendsResponse.ResultCode, friendsResponse.ResultMessage);
            Debug.Log(infoStr);

            if (friendsResponse.ResultCode == Hive5ResultCode.Success)
            {
                Debug.Log("Kakao Friends UserID Update Complete..");
            }
            else
            {
                Debug.Log("Kakao Friends UserID Update failed.. :" + friendsResponse.ResultCode);
            }
        });
    }

    public void SendPopupNoticeIgnore(long noticeID)
    {

        var data = new
        {
            popup_id = noticeID.ToString(),
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("ignore_popup", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(noticeID);

                hive5Process.ErrorProcess(response, "SendPopupNoticeIgnore", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            // code here

        });
    }

    public void SendPreLogin()
    {
        Connector.SendPreLogin();
    }

    public void SendRecoveryStamina(int CharacterIndex, int[] SelectedBuffs, int SeletedTower, bool Start, bool isWaveMode, int buyPotion1, int buyPotion2)
    {
        SendWaveStartOrContinue(CharacterIndex, SelectedBuffs, SeletedTower, 0, true, buyPotion1, buyPotion2);
    }

    public void SendRecoveryStaminaByBossRaidStart(long bossIndex, string platform, string owner_id)
    {
        SendRequestBossRaidStart(bossIndex, true, platform, owner_id);
    }

    public void SendRecoveryStaminaByStage(int charIndex, int[] selectedBuffs, int stageIndex, int stageType, int buyPotion1, int buyPotion2)
    {
        SendStageStart(charIndex, stageType, stageIndex, selectedBuffs, -1, buyPotion1, buyPotion2);
    }

    public void SendRefreshGambleItems(bool ByCash, int CharacterIndex, ref List<Item> Items)
    {
        //throw new NotImplementedException();
        CharPrivateData privateData = null;
        CharInfoData charData = connector.charInfo;
        if (charData != null)
            privateData = charData.GetPrivateData(CharacterIndex);

        if (privateData != null)
            privateData.gambleItemList.Clear();

        List<GambleItem> tempList = new List<GambleItem>();
        foreach (Item item in Items)
        {
            GambleItem newGambleItem = new GambleItem();
            newGambleItem.ID = item.itemInfo.itemID;
            newGambleItem.Grade = (int)item.itemGrade;
            newGambleItem.itemRate = item.itemRateStep;

            tempList.Add(newGambleItem);
        }

        if (privateData != null)
            privateData.gambleItemList.AddRange(tempList);

        var data = new
        {
            byCash = ByCash == true ? 1 : 0,
            hero_type = CharacterIndex,
            gambleItems = tempList
        };

        hive5.CallProcedure("refresh_gamble_items", data, (response) =>
        {
            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            int charIndex = (int)jsonData["hero_type"];
            int gold = (int)jsonData["gold"];
            int jewel = (int)jsonData["jewel"];
            int leftTime = (int)jsonData["leftTime"];

            // code here
            if (response.ResultCode == Hive5ResultCode.Success)
            {
                if (charData != null)
                {
                    charData.SetGold(gold, jewel);
                    privateData = charData.GetPrivateData(charIndex);
                }

                if (privateData != null)
                {
                    if (leftTime >= 0)
                        privateData.SetGambleTime(leftTime);

                    GambleWindow gambleWindow = GameUI.Instance.gambleWindow;
                    if (gambleWindow != null)
                        gambleWindow.UpdateCoinInfo(true);

                    GambleWindow.refreshExpireTime = privateData.refreshGambleExpireTime;
                }
            }

            GambleWindow.bSendRefresh = false;
        });
    }

    public void SendRegisterGCMID()
    {
        //PacketRegisterGCMID  msg = new PacketRegisterGCMID();

        //msg.UserIndexID = connector.UserIndexID;
        //msg.regID = connector.gcmRegID;

        //SendPacket("RegisterGCMID.php", msg);

        hive5.UpdatePushToken(OSType.Android, connector.gcmRegID, (response) =>
        {
        });
    }

    void SetPostBadge()
    {
        TownUI townUI = GameUI.Instance.townUI;

        if (townUI != null)
            townUI.SetBudgeNotify((int)TownUI.eTOWN_UI_TYPE.POST, 0);        
    }

    public void SendRequestAcceptAchieveReward(int id, int step, int achieveCharIndex)
    {
        var data = new
        {
            hero_type = achieveCharIndex,
            group_id = id,
            step_id = step
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("apply_achievement_reward", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(id);
                objects.Add(step);
                objects.Add(achieveCharIndex);

                hive5Process.ErrorProcess(response, "SendRequestAcceptAchieveReward", objects.ToArray());

                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            if (result == NetErrorCode.OK)
            {
                int hero_type = jsonData["hero_type"].ToInt();
                int group_id = jsonData["group_id"].ToInt();
                int step_id = jsonData["step_id"].ToInt();

                CharInfoData charData = Game.Instance.charInfoData;
                AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

                if (achieveMgr != null)
                    achieveMgr.SetCompleteReward(hero_type, group_id, step_id);

                AchievementWindow achieveWindow = GameUI.Instance.achievementWindow;
                if (achieveWindow != null)
                {
                    achieveWindow.requestCount = 0;
                    achieveWindow.UpdateInfo();
                }

                SetPostBadge();
            }
        });


    }

    public void SendRequestAcceptDailyAchieveReward(int id, int step)
    {
        var data = new
        {
            id = id
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("apply_daily_mission_reward", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
			AchievementWindow achieveWindow = GameUI.Instance.achievementWindow;
            
            if (response.ResultCode != Hive5ResultCode.Success)
            {
				if (achieveWindow != null)
					achieveWindow.requestCount = 0;
				
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
			
			if (achieveWindow != null)
				achieveWindow.requestCount = 0;
			
            if (result == NetErrorCode.OK)
            {
                int respone_id = jsonData["id"].ToInt();

                CharInfoData charData = Game.Instance.charInfoData;
                AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

                if (achieveMgr != null)
				{
					achieveMgr.SetCompleteDailyAchieve(respone_id);
					achieveMgr.GetDailyAchievementList();
				}

                if (achieveWindow != null)
                {
                    achieveWindow.requestCount = 0;
                    achieveWindow.UpdateInfo();
                }

                SetPostBadge();
            }
			else
			{
				if (GameUI.Instance.MessageBox != null)
					GameUI.Instance.MessageBox.SetMessage(result);
			}
        });
    }

    public void SendRequestAcceptFriend(long targetUserID, string platform = "kakao")
    {
        var parameters = new
            {
                platform = platform,
                id = targetUserID.ToString(),
            };

        GameUI.Instance.DoWait();
        hive5.CallProcedure("accept_friend_request", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            FriendWindow friendWindow = GameUI.Instance.friendWindow;
            if (friendWindow != null)
                friendWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            // code here
            PacketFriendInviteAccept pd = new PacketFriendInviteAccept()
            {
                errorCode = (NetErrorCode)jsonData["result"].ToInt(),
                Friend = targetUserID,
                Info = null,
            };

            if (friendWindow != null)
            {
                if (pd.errorCode == NetErrorCode.OK)
                {
                    pd.Info = new FriendInfo()
                    {
                        CharID = jsonData["hero_type"].ToInt(),
                        Lv = jsonData["hero_level"].ToInt(),
                        nick = (string)jsonData["nickname"],
                        UserID = pd.Friend,
                        connTime = TimeHelper.UnixTimeStampToDateTime(jsonData["logged_in_at"].ToInt())
                    };

                    Game.Instance.RemoveAcceptFriend(pd.Friend);
                    Game.Instance.AddMyFriendList(pd.Info);
                }
                else
                {
                    friendWindow.OnErrorMessage(pd.errorCode, null);
                }
            }
        });
    }

    public void SendRequestAcceptSpecialAchieveReward(int id, int step, int charIndex)
    {
        throw new NotImplementedException();
    }

    public void SendRequestArenaEnd(bool bWin, long targetUserIndex, int targetCharIndex, string platform = "kakao")
    {

        var data = new
        {
            hero_type = Connector.charIndex,
            win = bWin,
            target_platform = platform,
            target_platform_user_id = targetUserIndex.ToString(),
            target_hero_type = targetCharIndex,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("end_arena", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            ArenaInfo arenaInfo = null;
            StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;
            if (result == NetErrorCode.OK)
            {
                int rank = jsonData["arena_rank"].ToInt();
                int groupRanking = jsonData["group_ranking"].ToInt();

                arenaInfo = new ArenaInfo()
                {
                    rankType = rank,
                    groupRanking = groupRanking,

                    winningStreakCount = jsonData["arena"]["win_streak_count"].ToInt(),
                    totalWinningCount = jsonData["arena"]["win_count"].ToInt(),
                    seasonBestRank = jsonData["arena"]["best_rank"].ToInt(),
                };
				
				CharPrivateData enemyPrivateData = Game.Instance.arenaTargetInfo;
           		if (enemyPrivateData != null)
            	{
               		enemyPrivateData.arenaInfo.rankType = jsonData["target_arena_rank"].ToInt();
                	enemyPrivateData.arenaInfo.groupRanking = jsonData["target_group_ranking"].ToInt();
            	}
            }

            stageEndEvent.OnArenaResult(Connector.charIndex, 0, arenaInfo);
        });
    }

    public void SendRequestArenaInfo()
    {
        var data = new
        {
            hero_type = Connector.charIndex,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("enter_arena", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "SendRequestArenaInfo");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            TownUI townUI = GameUI.Instance.townUI;

            if (result == NetErrorCode.OK)
            {
                int rank = jsonData["arena_rank"].ToInt();
                int groupRanking = jsonData["group_ranking"].ToInt();
                int winningStreakCount = -1;
                int totalWinningCount = -1;
                int seasonBestRank = 10;
                int reward_left_sec = jsonData["reward_left_sec"].ToInt();
                int open = (bool)jsonData["is_arena_open"] == true ? 1 : 0;

                if (jsonData["arena"] != null)
                {
                    winningStreakCount = jsonData["arena"]["win_streak_count"].ToInt();
                    totalWinningCount = jsonData["arena"]["win_count"].ToInt();
                    seasonBestRank = jsonData["arena"]["best_rank"].ToInt();
                }

                ArenaInfo arenaInfo = new ArenaInfo()
                {
                    rankType = rank,
                    groupRanking = groupRanking,

                    winningStreakCount = winningStreakCount,
                    totalWinningCount = totalWinningCount,
                    seasonBestRank = seasonBestRank
                };

                CharInfoData charData = Game.Instance.charInfoData;
                int charIndex = -1;
                if (Game.Instance.connector != null)
                    charIndex = Game.Instance.connector.charIndex;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;
                charData.ticket = jsonData["ticket"].ToInt();

                if (privateData != null)
                {
                    privateData.SetArenaInfo(arenaInfo);
                }

                List<ArenaRankingInfo> rankingInfos = new List<ArenaRankingInfo>();

                for (int i = 0; i < jsonData["rankings"].Count; ++i)
                {
                    var rankings = jsonData["rankings"][i];
                    ArenaRankingInfo info = new ArenaRankingInfo() { };
                    info.CharacterIndex = rankings["hero_type"].ToInt();
					info.ranking = rankings["ranking"].ToInt();
                    info.NickName = rankings["name"] != null ? rankings["name"].ToString() : "";
                    info.PlatformUserId = rankings["user_id"].ToLong();
                    info.platform = rankings["platform"].ToString();

                    info.charLevels = JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(rankings["hero_levels"]));


                    rankingInfos.Add(info);
                }

                if (open == 0)
                    reward_left_sec = -1;

                townUI.OnArenaWindow(arenaInfo, rankingInfos.ToArray(), reward_left_sec, open);
            }

            if (townUI)
                townUI.requestCount = 0;
        });
    }

    public void SendRequestArenaRanking(int rankType, int rank, bool bDown)
    {
        var parameters = new
        {
            ranking_type = rankType,
            ranking = rank,
            is_downward = bDown,
        };

        GameUI.Instance.DoWait();
        hive5.CallProcedure("get_arena_ranking", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                // handle error here
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            List<ArenaRankingInfo> rankings = new List<ArenaRankingInfo>();
            for (int i = 0; i < jsonData["rankings"].Count; i++)
            {
                var item = jsonData["rankings"][i];
                rankings.Add(new ArenaRankingInfo()
                {
                    CharacterIndex = item["hero_type"].ToInt(),
                    NickName = item["name"] != null ? item["name"].ToString() : "",
                    ranking = item["ranking"].ToInt(),
                    charLevels = item["hero_levels"].ToArray<int>().ToArray(),
                    PlatformUserId = item["user_id"].ToLong(),
					platform = item["platform"].ToString(),
                });
            }


            // code here
            ArenaWindow arenaWindow = GameUI.Instance.arenaWindow;

            if (arenaWindow != null)
            {
                arenaWindow.RefreshRankList(ErrorCodeConverter.Convert((Hive5ResultCode)jsonData["result"].ToInt()),
                    jsonData["ranking_type"].ToInt(),
                    rankings.ToArray(),
                    (bool)jsonData["is_downward"]);
            }
        });
    }


    public void SendRequestArenaStart(bool recovery = false)
    {
        var data = new
        {
            hero_type = Connector.charIndex,
            recovery = recovery,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("start_arena", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            ArenaWindow arenaWindow = GameUI.Instance.arenaWindow;



            if (result == NetErrorCode.OK)
            {
                PacketArenaMatchingTarget target_info = new PacketArenaMatchingTarget();

                target_info.Ticket = jsonData["ticket"].ToInt();
                target_info.TargetRankType = jsonData["target_rank"].ToInt();
                target_info.TargetGroupRanking = jsonData["target_group_ranking"].ToInt();

                target_info.TargetEquipInfos = new EquipItemDBInfo[0];

                target_info.TargetSkillInfo = null;
                target_info.awakenSkillInfo = null;

                if (jsonData["target_info"] != null)
                {
                    var targetData = jsonData["target_info"];
                    target_info.TargetNickName = targetData["name"] != null ? targetData["name"].ToString() : "";
                    target_info.CharacterIndex = targetData["hero_type"].ToInt();
                    //target_info.UserIndexID = targetData["platform_user_id"].ToInt();
                    target_info.targetUserId = (string)targetData["platform_user_id"];
                    target_info.targetUserPlatform = (string)targetData["platform"];
                    target_info.TargetExp = targetData["exp"].ToInt();
                    target_info.targetAwakenExp = targetData["awaken_exp"].ToInt();
                    target_info.TargetEquipInfos = GetEquipItemsFromJson(targetData["wear_items"]);
                    target_info.TargetSkillInfo = GetSkillDBInfoFromJson(targetData["skills"]);
                    target_info.awakenSkillInfo = GetSkillDBInfoFromJson(targetData["awaken_skills"]);
                    target_info.costumeSetItem = GetCostumeSetFromJson(targetData["costumeset_item"]);
                }

                CharInfoData charData = Game.Instance.charInfoData;

                if (charData != null)
                    charData.SetGold(jsonData["gold"].ToInt(), jsonData["jewel"].ToInt());

                if (arenaWindow != null)
                    arenaWindow.LoadArena(target_info);
            }
            else {
                hive5Process.ErrorProcess(new PacketError()
                {
                    ErrorCode = (int)result,
                    ErrorMessage = "",
                });
            }
           

        });
    }

    public void SendRequestArenaStartBuyTicket()
    {
        SendRequestArenaStart(true);
    }

    public void SendRequestAwakeningBuyPoint(int buyCount)
    {
        var data = new
        {
            hero_type = connector.charIndex,
            buy_point = buyCount
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("buy_awaken_point", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();

            int gold = jsonData["gold"].ToInt();
            int jewel = jsonData["jewel"].ToInt();
            int charIndex = jsonData["hero_type"].ToInt();
			
			var pointInfoJson = jsonData["awaken_point_info"];
			int usedPoint = pointInfoJson["used_point"].ToInt();
			int gainPoint = pointInfoJson["gain_point"].ToInt();
			int buyLimitPoint = pointInfoJson["buyable_point"].ToInt();
            int buyPoint = pointInfoJson["buy_point"].ToInt();
			int giftPoint = pointInfoJson["gift_point"].ToInt();
            
            AwakeningLevelWindow awakeningWindow = GameUI.Instance.awakeningWindow;

            if (errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = connector.charInfo;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                if (privateData != null)
                {
                    privateData.baseInfo.APoint = gainPoint;
                    privateData.baseInfo.APointGift = giftPoint;
					privateData.baseInfo.ABuyCount = buyPoint;
					privateData.baseInfo.ALimitBuyCount = buyLimitPoint;
                }

                if (charData != null)
                    charData.SetGold(gold, jewel);
            }

            if (awakeningWindow != null)
                awakeningWindow.OnResultBuyPoint(errorCode, gainPoint, giftPoint, buyLimitPoint, buyPoint);
        });
    }

    public void SendRequestAwakeningReset()
    {
        var data = new
        {
            hero_type = connector.charIndex
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("reset_awaken_skill", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();

            int gold = jsonData["gold"].ToInt();
            int jewel = jsonData["jewel"].ToInt();
            int charIndex = jsonData["hero_type"].ToInt();
			
			var pointInfoJson = jsonData["awaken_point_info"];
			int usedPoint = pointInfoJson["used_point"].ToInt();
			int gainPoint = pointInfoJson["gain_point"].ToInt();
			int buyLimitPoint = pointInfoJson["buyable_point"].ToInt();
            int buyPoint = pointInfoJson["buy_point"].ToInt();
			int giftPoint = pointInfoJson["gift_point"].ToInt();
			
			int availPoint = (gainPoint + Mathf.Min(buyLimitPoint, buyPoint) + giftPoint) - usedPoint;
			
            AwakeningLevelWindow awakeningWindow = GameUI.Instance.awakeningWindow;

            if (errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = connector.charInfo;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                if (privateData != null)
                {
                    privateData.baseInfo.APoint = gainPoint;
					privateData.baseInfo.APointGift = giftPoint;
                    privateData.baseInfo.ABuyCount = buyPoint;
					privateData.baseInfo.ALimitBuyCount = buyLimitPoint;

                    privateData.ResetAwakeingSkill();
                }

                if (charData != null)
                    charData.SetGold(gold, jewel);
            }

            if (awakeningWindow != null)
                awakeningWindow.OnResultReset(errorCode, gainPoint, giftPoint, buyLimitPoint, buyPoint);
        });
    }

    public void SendRequestAwakeningUpgrade(SkillUpgradeDBInfo info)
    {
        var data = new
        {
            hero_type = connector.charIndex,
            skill_ids = info.SkillIDs,
            skill_lvs = info.Levels,
            skill_adds = info.Adds
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("apply_awaken_skill", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();

            AwakeningLevelWindow awakeningWindow = GameUI.Instance.awakeningWindow;
            if (errorCode == NetErrorCode.OK)
            {
				int gold = jsonData["gold"].ToInt();
	            int jewel = jsonData["jewel"].ToInt();
	            int charIndex = jsonData["hero_type"].ToInt();
	
	            int[] skill_ids = LitJson.JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["skill_ids"]));
	            int[] skill_levels = LitJson.JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["skill_levels"]));
	            
				var pointInfoJson = jsonData["awaken_point_info"];
				int usedPoint = pointInfoJson["used_point"].ToInt();
				int gainPoint = pointInfoJson["gain_point"].ToInt();
				int buyLimitPoint = pointInfoJson["buyable_point"].ToInt();
	            int buyPoint = pointInfoJson["buy_point"].ToInt();
				int giftPoint = pointInfoJson["gift_point"].ToInt();
				
                SkillUpgradeDBInfo skillDBInfo = new SkillUpgradeDBInfo();
                skillDBInfo.SkillIDs = skill_ids;
                skillDBInfo.Levels = skill_levels;

                CharInfoData charData = connector.charInfo;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;
                if (privateData != null)
                {
                    int nCount = Math.Min(skill_ids.Length, skill_levels.Length);
                    int skillID = 0;
                    int skillLv = 0;

                    for (int index = 0; index < nCount; ++index)
                    {
                        skillID = skill_ids[index];
                        skillLv = skill_levels[index];

                        privateData.SetAwakeningSkillData(skillID, skillLv);
                    }

                    privateData.baseInfo.APoint = gainPoint;
                    privateData.baseInfo.APointGift = giftPoint;
					privateData.baseInfo.ABuyCount = buyPoint;
					privateData.baseInfo.ALimitBuyCount = buyLimitPoint;
                }

                if (charData != null)
                    charData.SetGold(gold, jewel);

                if (awakeningWindow != null)
                    awakeningWindow.OnResultApply(skillDBInfo, gainPoint, giftPoint, buyLimitPoint, buyPoint);
            }
            else
            {
                if (awakeningWindow != null)
                    awakeningWindow.OnErrorMessage(errorCode, null);
            }
        });
    }

    public void SendRequestBossRaidEnter()
    {

        var data = new
        {

        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("enter_boss_raid", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);


            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();
            long user_id = jsonData["user_id"].ToLong();

            if (errorCode == NetErrorCode.OK)
            {
                int server_time = jsonData["server_time"].ToInt();

                List<BossRaidInfo> bossRaidInfos = new List<BossRaidInfo>();

                for (int i = 0; i < jsonData["bosses"].Count; ++i)
                {
                    var boss = jsonData["bosses"][i];

                    BossRaidInfo info = new BossRaidInfo();

                    info.bossID = boss["table_id"].ToInt();
                    info.index = boss["id"].ToLong();        // ?             
                    info.leftSec = boss["expire_at"].ToInt() - server_time;
                    info.isPhase2 = (bool)boss["transform"];
                    info.finderName = boss["find_user_nick"].ToString();
                    info.ownerPlatform = boss["find_user_platform"].ToString();
                    info.ownerPlatformUserID = boss["find_user_id"].ToString();
                    info.curHP = boss["hp"].ToInt();

                    if (info.curHP <= 0)
                        info.leftSec = 0;

                    List<BossDamage> damages = new List<BossDamage>();

                    for (int j = 0; j < boss["damages"].Count; ++j)
                    {
                        var damage = boss["damages"][j];

                        var bossDamage = new BossDamage()
                        {
                            amount = damage["damage"].ToInt(),
                            nick = damage["nick"].ToString()
                        };

                        damages.Add(bossDamage);

                        if (Connector.Nick.CompareTo(damage["nick"].ToString()) == 0)
                        {
                            info.myDamage = damage["damage"].ToInt();
                        }
                    }

                    damages.Sort(delegate(BossDamage a, BossDamage b)
                    {
                        if (a.amount < b.amount)
                            return 1;
                        if (a.amount > b.amount)
                            return -1;
                        return 0;
                    });

                    if (damages.Count > 0)
                    {
                        if (damages[0].amount > 0)
                            info.topCharName = damages[0].nick;
                        else 
                            info.topCharName = "";

                        info.topCharDamage = damages[0].amount;
                    }

                    info.isCleared = info.curHP <= 0;
                    info.lastAttackerName = boss["last_hit_user_nick"].ToString();

                    bossRaidInfos.Add(info);

                    bossRaidInfos.Sort(delegate(BossRaidInfo a, BossRaidInfo b)
                    {
                        if (a.leftSec < b.leftSec)
                            return 1;

                        if (a.leftSec > b.leftSec)
                            return -1;

                        if (a.bossID < b.bossID)
                            return 1;

                        if (a.bossID > b.bossID)
                            return -1;

                        return 0;
                    });
                }

                TownUI townUI = GameUI.Instance.townUI;
                if (townUI != null)
                    townUI.OnBossRaidWindow(bossRaidInfos);
            }
        });
    }

    public void SendRequestBossRaidStart(long bossIndex, bool recovery, string platform, string owner_id)
    {
        var data = new
        {
            hero_type = Connector.charIndex,
            recovery = recovery,
            boss_index = bossIndex,
            owner_platform = platform,
            owner_id = owner_id,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("start_boss_raid", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            BossRaidWindow bossRaidWindow = GameUI.Instance.bossRaidWindow;

            if (result == NetErrorCode.OK)
            {
                CharInfoData charData = Game.Instance.charInfoData;
                CharPrivateData privateData = null;
                if (charData != null)
                    privateData = charData.GetPrivateData(Connector.charIndex);

                if (privateData != null)
                {
                    privateData.baseInfo.StaminaCur = jsonData["stamina"].ToInt();
                    privateData.baseInfo.StaminaPresent = jsonData["stamina_gift"].ToInt();
                }

                if (bossRaidWindow != null)
                    bossRaidWindow.OnBossRaidStart(bossIndex, (bool)jsonData["transform"]);

                connector.charInfo.SetGold(jsonData["gold"].ToInt(), jsonData["jewel"].ToInt());
            }
            else
            {
                if (bossRaidWindow != null)
                    bossRaidWindow.OnErrorMessage(result, bossRaidWindow);
            }
        });
    }

    public void SendRequestBuyCashItem(CashItemInfo info)
    {
        Logger.DebugLog("SendRequestBuyCashItem - CashItemInfo.ItemID: " + info.ItemID);
        switch (info.paymentType)
        {
		case ePayment.Cash:
			// 현찰사용. 작업중.
			SendRequestBuyCashItemNotify(info);
			break;
		default:
			SendBuyCashItem(info.ItemID);
			break;
        }
    }


    public void SendRequestBuyCashItemNotify(CashItemInfo info)
    {
        //TStoreTID
        string timestr = System.DateTime.Now.ToString("yyMMddHHmmss");
        string TStoreTID = timestr + "_" + connector.UserIndexID.ToString();
        Logger.DebugLog("SendRequestBuyCashItemNotify" + TStoreTID);

        var productCode = info.GetStoreItemCode(Connector.publisher);

        GameUI.Instance.DoWait();
        hive5.CreateGooglePurchase(productCode, (response) =>
            {
                GameUI.Instance.CancelWait();

                if (response.ResultCode != Hive5ResultCode.Success)
                {
                    hive5Process.ErrorProcess(response);
                    return;
                }

                var body = response.ResultData as CreateGooglePurchaseResponseBody;
                connector.UniversalPurchaseId = body.Id;

                // google
                TStoreCashItemInfo item = new TStoreCashItemInfo();
                item.ItemID = info.ItemID;
                item.TStoreProductCode = productCode;
                item.TStoreTID = connector.UniversalPurchaseId.ToString();
                item.Price = (int)info.price.x;
                item.publisherType = (int)connector.publisher;
                item.itemName = info.itemName;

                // 안드로이드에 아이템 구매요청한다.
                Game.Instance.androidManager.OnClickBuyCashItem(item);
            });

    }

    public void SendRequestBuyCostumeItem(int ItemID, int slotIndex)
    {
        var parameters = new TupleList<string, string>();
        parameters.Add("item_id", ItemID.ToString());
        parameters.Add("hero_type", connector.charIndex.ToString());
        parameters.Add("slot_index", slotIndex.ToString());
        GameUI.Instance.DoWait();

        hive5.CallProcedure("buy_costume_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            int result = (int)jsonData["result"];

            int slot_index = -1;

            if (result == (int)NetErrorCode.OK)
            {
                slot_index = (int)jsonData["slot_index"];
                int item_id = (int)jsonData["item_id"];
                string item_uid = jsonData["item_uid"].ToString();
                int gold = (int)jsonData["gold"];
                int jewel = (int)jsonData["jewel"];

                Item newItem = Item.CreateItem(item_id, item_uid, 0, 0, 1, -1, 0);
                if (newItem != null)
                    newItem.IsNewItem = true;

                connector.charInfo.AddCostume(newItem);
                connector.charInfo.SetGold(gold, jewel);

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyItem, 0);
            }

            UpdateItemInfos(null, true);

            ShopWindow shopWindow = GameUI.Instance.shopWindow;
            if (shopWindow != null)
            {
                shopWindow.OnBuyCostumeItemResult(slot_index, response.ResultCode != Hive5ResultCode.Success ? ErrorCodeConverter.Convert(response.ResultCode) : (NetErrorCode)result);
            }

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }

    public void UpdateItemInfos(CharPrivateData privateData, bool bRerangeItem)
    {
        Game.RerangeItemList(connector.charInfo.inventoryMaterialData);

        if (bRerangeItem == true)
        {
            Game.RerangeItemList(connector.charInfo.inventoryCostumeData);
            Game.RerangeItemList(connector.charInfo.inventoryNormalData);
            //Game.RerangeItemList(connector.charInfo.inventoryCostumeSetData);
        }

        PlayerController player = Game.Instance.player;
        LifeManager lifeManager = player != null ? player.lifeManager : null;
        InventoryManager invenManager = null;
        EquipManager equipManager = null;
        if (lifeManager != null)
        {
            invenManager = lifeManager.inventoryManager;
            equipManager = lifeManager.equipManager;
        }

        if (invenManager != null)
        {
            invenManager.SetInvenItemData(connector.charInfo.inventoryNormalData);
            invenManager.SetInvenCostumeData(connector.charInfo.inventoryCostumeData);
            invenManager.SetMaterialItemData(connector.charInfo.inventoryMaterialData);
            invenManager.SetCostumeSetItems(connector.charInfo.inventoryCostumeSetData);
        }

        if (equipManager != null && privateData != null)
            equipManager.SetEquipItemData(privateData.equipData, privateData.costumeSetItem);
    }

    public void SendRequestBuyCostumeSetItem(int itemID, int slotIndex)
    {
        var parameters = new TupleList<string, string>();
        parameters.Add("item_id", itemID.ToString());
        parameters.Add("hero_type", connector.charIndex.ToString());
        parameters.Add("slot_index", slotIndex.ToString());

        GameUI.Instance.DoWait();
        hive5.CallProcedure("buy_costume_set_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            int result = (int)jsonData["result"];
            int slot_index = -1;
            if (result == (int)NetErrorCode.OK)
            {
                slot_index = (int)jsonData["slot_index"];
                int item_id = (int)jsonData["item_id"];
                string item_uid = jsonData["item_uid"].ToString();
                int gold = (int)jsonData["gold"];
                int jewel = (int)jsonData["jewel"];

                CostumeSetItem costumeSet = CostumeSetItem.Create(item_id, item_uid);

                connector.charInfo.AddCostumeSetItem(costumeSet);
                connector.charInfo.SetGold(gold, jewel);

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyItem, 0);
            }

            UpdateItemInfos(null, true);

            ShopWindow shopWindow = GameUI.Instance.shopWindow;
            if (shopWindow != null)
            {
                shopWindow.OnBuyCostumeSetItemResult(slot_index, response.ResultCode != Hive5ResultCode.Success ? ErrorCodeConverter.Convert(response.ResultCode) : (NetErrorCode)result);
            }

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }


    void AddNewItem(CharInfoData charInfo, Item newItem)
    {
        if (newItem != null && newItem.itemInfo != null)
        {
            switch (newItem.itemInfo.itemType)
            {
                case ItemInfo.eItemType.Costume_Back:
                case ItemInfo.eItemType.Costume_Body:
                case ItemInfo.eItemType.Costume_Head:
                    connector.charInfo.AddCostume(newItem);
                    break;
                case ItemInfo.eItemType.Material:
                case ItemInfo.eItemType.Material_Compose:
                	connector.charInfo.AddMaterial(newItem);
                	break;
                default:

                    if (newItem != null && newItem.IsNewItem == true && newItem.itemRateStep == 4)
                    {
                        Game.Instance.ApplyAchievement(Achievement.eAchievementType.eGetBestItem, 1);
                        Game.Instance.SendUpdateAchievmentInfo();
                    }

                    connector.charInfo.AddItem(newItem);
                    break;
            }
        }

    }

    public void SendRequestBuyArtifactItem(int ItemID, int buyCount, int slotIndex, GameDef.eItemSlotWindow window)
    {
        var parameters = new TupleList<string, string>();
        parameters.Add("item_id", ItemID.ToString());
        parameters.Add("item_count", buyCount.ToString());
        parameters.Add("hero_type", connector.charIndex.ToString());
        parameters.Add("slot_index", slotIndex.ToString());
        parameters.Add("window_type", ((int)window).ToString());

        GameUI.Instance.DoWait();

        hive5.CallProcedure("buy_artifact_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);
            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];
            int slot_index = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            if (result == NetErrorCode.OK)
            {
                slot_index = (int)jsonData["slot_index"];
                int item_id = (int)jsonData["item_id"];

                var items = jsonData["items"];

                for (int i = 0; i < items.Count; ++i)
                {
                    ItemDBInfo info = ItemToItemDBInfo(items[i]);

                    //상점 구입 아이템 하급..
                    Item newItem = Item.CreateItem(info);
                    if (newItem != null)
                    {
                        newItem.IsNewItem = true;

                        if (newItem.itemInfo != null && newItem.itemInfo.buyPrice.z > 0.0f)
                            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyArenaItem, 0);
                    }

                    AddNewItem(connector.charInfo, newItem);
                }

                CharInfoData charData = connector.charInfo;
                charData.SetGold(jsonData["gold"].ToInt(), jsonData["jewel"].ToInt());

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyItem, 0);

            }

            UpdateItemInfos(null, true);

            ShopWindow shopWindow = GameUI.Instance.shopWindow;
            if (shopWindow != null)
            {
                shopWindow.OnBuyNormalItemResult(slot_index, result);
            }

            Game.Instance.SendUpdateAchievmentInfo();

        });
    }

    public void SendRequestBuyNormalItem(int ItemID, int buyCount, int slotIndex, GameDef.eItemSlotWindow window)
    {
        var parameters = new TupleList<string, string>();
        parameters.Add("item_id", ItemID.ToString());
        parameters.Add("item_count", buyCount.ToString());
        parameters.Add("hero_type", connector.charIndex.ToString());
        parameters.Add("slot_index", slotIndex.ToString());
        parameters.Add("slot_window", ((int)window).ToString());

        GameUI.Instance.DoWait();

        hive5.CallProcedure("buy_arena_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);
            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];
            int slot_index = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            if (result == NetErrorCode.OK)
            {
                slot_index = (int)jsonData["slot_index"];
                int item_id = (int)jsonData["item_id"];
                int medal = (int)jsonData["medal"];
                var items = jsonData["items"];

                for (int i = 0; i < items.Count; ++i)
                {
                    ItemDBInfo info = ItemToItemDBInfo(items[i]);

                    //상점 구입 아이템 하급..
                    Item newItem = Item.CreateItem(info);
                    if (newItem != null)
                    {
                        newItem.IsNewItem = true;

                        if (newItem.itemInfo != null && newItem.itemInfo.buyPrice.z > 0.0f)
                            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyArenaItem, 0);
                    }

                    AddNewItem(connector.charInfo, newItem);
                }

                connector.charInfo.medal_Value = medal;
				
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyItem, 0);

            }

            UpdateItemInfos(null, true);

            ShopWindow shopWindow = GameUI.Instance.shopWindow;
            if (shopWindow != null)
            {
                shopWindow.OnBuyNormalItemResult(slot_index, result);
            }

            Game.Instance.SendUpdateAchievmentInfo();

        });
    }

    public void SendRequestCompositionItem(int slotIndex, GameDef.eItemSlotWindow slotWindow, string UID, int ItemID, string composMaterialUID, string composAddMaterialUID, bool bCash, bool isTutorial)
    {
        var data = new
        {
            hero_type = connector.charIndex,

            slot_index = slotIndex,
            slot_window = slotWindow,
            item_id = ItemID,
            item_UID = UID,
            material_UID = composMaterialUID,
            material_add_UID = composAddMaterialUID,

            by_cash = bCash,
            tutorial = isTutorial,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("composition_item", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);
            int slot_index = 0;
            int item_grade = 0;

            StorageWindow storageWindow = GameUI.Instance.storageWindow;

            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();

            if (errorCode == NetErrorCode.OK ||
                errorCode == NetErrorCode.CompositionFailed)
            {

                int charIndex = jsonData["hero_type"].ToInt();
                int gold = jsonData["gold"].ToInt();
                int jewel = jsonData["jewel"].ToInt();

                string item_UID = jsonData["item_UID"].ToString();
                int item_id = jsonData["item_id"].ToInt();
                slot_index = jsonData["slot_index"].ToInt();
                GameDef.eItemSlotWindow window_type = (GameDef.eItemSlotWindow)jsonData["slot_window"].ToInt();

                string del_item_UID = jsonData["del_item_UID"].ToString();
                string material_UID = jsonData["material_UID"].ToString();
                int material_count = jsonData["material_count"].ToInt();
                item_grade = jsonData["item_grade"].ToInt();
                int item_exp = jsonData["item_exp"].ToInt();

                connector.charInfo.SetGold(gold, jewel);

                CharPrivateData privateData = connector.charInfo.privateDatas[charIndex];

                BaseTradeItemInfo baseTradeInfo = new BaseTradeItemInfo();
                baseTradeInfo.UID = item_UID;
                baseTradeInfo.ItemID = item_id;
                baseTradeInfo.slotIndex = slot_index;
                baseTradeInfo.windowType = window_type;

                Item resultItem = connector.charInfo.CompositionItem(privateData, del_item_UID, material_UID, material_count, baseTradeInfo, item_grade);
                //resultItem.SetExp(packet.Exp);

                int newSlotIndex = connector.charInfo.FindSlotIndex(resultItem, privateData, window_type);

                UpdateItemInfos(privateData, false);

                if (storageWindow != null)
                    storageWindow.UpdateWindow();

                if (storageWindow != null && storageWindow.compositionWindow != null)
                {
                    storageWindow.compositionWindow.resultItemExp = (uint)item_exp;
                    storageWindow.compositionWindow.UpdateCompositionItem(resultItem, newSlotIndex, window_type);
                }
            }

            if (errorCode == NetErrorCode.OK)
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eCompositionSuccess, 0);
            else
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eCompositionFail, 0);

            if (storageWindow != null && storageWindow.compositionWindow != null)
            {
                storageWindow.compositionWindow.OnCompositionResult(slot_index, errorCode, item_grade);
            }

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }

    public void SendRequestCoupon(string couponNumber)
    {
        var data = new {
			coupon_number = couponNumber
		};
		
		GameUI.Instance.DoWait();
		hive5.CallProcedure("request_coupon", data, (response) =>
		{
			GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }
			
			var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
			string error_message = "";
			if (jsonData.Keys.Contains("error_message") == true)
				error_message = jsonData["error_message"] != null ? jsonData["error_message"].ToString() : "";
			
            CouponWindow couponWindow = GameUI.Instance.couponWindow;
	        if (couponWindow != null)
	            couponWindow.OnResult(result, error_message);
			
		});
    }

    public void SendRequestDeleteFriend(long targetUserID, string platform = "kakao")
    {
        var parameters = new
        {
            platform = platform,
            id = targetUserID.ToString(),
        };

        GameUI.Instance.DoWait();
        hive5.CallProcedure("unfriend", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {

            }
        });
    }

    public void SendRequestDoEquipCostumeItem(int equipSlotIndex, int slotIndex, int ItemID, string UID)
    {
        var parameters = new TupleList<string, string>();

        parameters.Add("hero_type", Connector.charIndex.ToString());
        parameters.Add("item_uid", UID);
        parameters.Add("equip_slot_index", equipSlotIndex.ToString());
        parameters.Add("inven_slot_index", slotIndex.ToString());
        parameters.Add("item_id", ItemID.ToString());

        GameUI.Instance.DoWait();
        hive5.CallProcedure("equip_costume_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            int equip_slot_index = -1;
            int inven_slot_index = -1;

            if (result == NetErrorCode.OK)
            {
                int charIndex = connector.charIndex;
                CharInfoData charData = connector.charInfo;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                equip_slot_index = (int)jsonData["equip_slot_index"];
                inven_slot_index = (int)jsonData["inven_slot_index"];

                if (privateData != null)
                {
                    if (jsonData["equip_item"] != null)
                    {
                        string uid = jsonData["equip_item"]["id"].ToString();
                        int id = jsonData["equip_item"]["table_id"].ToInt();

                        Item equipItem = Item.CreateItem(id, uid, 0, 0, 1, -1, 0);

                        privateData.AddEquipItem(equip_slot_index, equipItem);

                        if (charData != null)
                            charData.RemoveCostumeByIndex(inven_slot_index, uid);
                    }
                }

                for (int i = 0; i < jsonData["inven_items"].Count; ++i)
                {
                    var invenitem = jsonData["inven_items"][i];

                    int type = invenitem["type"].ToInt();
                    string uid = invenitem["id"].ToString();
                    int id = invenitem["table_id"].ToInt();

                    switch (type)
                    {
                        case Item_Type.costume:
                            {
                                Item unEquipCostume = Item.CreateItem(id, uid, 0, 0, 1, -1, 0);

                                if (unEquipCostume != null)
                                    charData.AddCostume(unEquipCostume);
                            }
                            break;
                        case Item_Type.costumeset:
                            {
                                CostumeSetItem unEquipCostumeSet = CostumeSetItem.Create(id, uid);

                                if (unEquipCostumeSet != null)
                                    charData.AddCostumeSetItem(unEquipCostumeSet);

                                if (privateData != null)
                                    privateData.SetCostumeSetItem(null);
                            }
                            break;
                    }
                }

                UpdateItemInfos(privateData, true);
            }


            StorageWindow storageWindow = GameUI.Instance.storageWindow;
            if (storageWindow != null)
            {
                storageWindow.OnEquipResult(inven_slot_index, GameDef.eItemSlotWindow.Costume, result);
            }

        });
    }

    public void SendRequestDoEquipCostumeSetItem(int invenSlotIndex, int itemID, string UID, GameDef.eItemSlotWindow slotWindow)
    {

        var parameters = new TupleList<string, string>();

        parameters.Add("hero_type", Connector.charIndex.ToString());
        parameters.Add("item_uid", UID);
        parameters.Add("inven_slot_index", invenSlotIndex.ToString());
        parameters.Add("item_id", itemID.ToString());

        GameUI.Instance.DoWait();
        hive5.CallProcedure("equip_costume_set_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }
            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];

            int inven_slot_index = -1;

            if (result == NetErrorCode.OK)
            {
                int heroType = (int)jsonData["hero_type"];

                inven_slot_index = (int)jsonData["inven_slot_index"];

                CharInfoData charData = connector.charInfo;
                int charIndex = connector.charIndex;
                CharPrivateData privateData = connector.charInfo.privateDatas[charIndex];


                if (privateData != null)
                {
                    //1. equip...
                    if (jsonData["equip_item"] != null)
                    {
                        int id = jsonData["equip_item"]["table_id"].ToInt();
                        string uid = jsonData["equip_item"]["id"].ToString();
                        CostumeSetItem equipCostumeSetItem = CostumeSetItem.Create(id, uid);
                        privateData.SetCostumeSetItem(equipCostumeSetItem);
                    }
                }

                //2. removeItem frome costumeSetItemList..
                if (charData != null)
                    charData.RemoveCostumeSetByIndex(inven_slot_index);

                //3.unEquip item add..
                if (jsonData["inven_items"] != null)
                {
                    for (int i = 0; i < jsonData["inven_items"].Count; ++i)
                    {
                        var invenitem = jsonData["inven_items"][i];
                        int id = invenitem["table_id"].ToInt();
                        string uid = invenitem["id"].ToString();
                        int type = invenitem["type"].ToInt();

                        if (type == Item_Type.costumeset)
                        {
                            CostumeSetItem unEquipCostumeSetItem = null;
                            unEquipCostumeSetItem = CostumeSetItem.Create(id, uid);

                            charData.AddCostumeSetItem(unEquipCostumeSetItem);
                        }
                        else
                        {
                            Item unEquipCostumeItem = null;

                            unEquipCostumeItem = Item.CreateItem(id, uid, 0, 0, 1, -1, 0);

                            int equipSlotIndex = EquipInfo.ItemTypeToEquipSlotIndex(null, unEquipCostumeItem.itemInfo.itemType);
                            privateData.RemoveEquipItem(equipSlotIndex, null);

                            charData.AddCostume(unEquipCostumeItem);
                        }

                    }
                }

                UpdateItemInfos(privateData, true);
            }

            StorageWindow storageWindow = GameUI.Instance.storageWindow;
            if (storageWindow != null)
            {
                storageWindow.OnEquipResult(inven_slot_index, GameDef.eItemSlotWindow.Costume, result);
            }

        });
    }

    public void SendRequestDoEquipItem(int equipSlotIndex, int InvenSlotIndex, int ItemID, string UID, GameDef.eItemSlotWindow slotWindow)
    {
        var parameters = new TupleList<string, string>();

        parameters.Add("hero_type", Connector.charIndex.ToString());
        parameters.Add("item_uid", UID);
        parameters.Add("equip_slot_index", equipSlotIndex.ToString());
        parameters.Add("inven_slot_index", InvenSlotIndex.ToString());
        parameters.Add("item_id", ItemID.ToString());
        parameters.Add("window_type", ((int)slotWindow).ToString());

        GameUI.Instance.DoWait();
        hive5.CallProcedure("equip_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];

            int equip_slot_index = -1;
            int inven_slot_index = -1;

            if (result == NetErrorCode.OK)
            {
                int heroType = (int)jsonData["hero_type"];
                equip_slot_index = (int)jsonData["equip_slot_index"];
                inven_slot_index = (int)jsonData["inven_slot_index"];
                GameDef.eItemSlotWindow window_type = (GameDef.eItemSlotWindow)(int)jsonData["window_type"];

                int charIndex = connector.charIndex;
                CharPrivateData privateData = connector.charInfo.privateDatas[charIndex];

                if (jsonData["equip_item"] != null)
                {
                    ItemDBInfo equip = ItemToItemDBInfo(jsonData["equip_item"]);

                    privateData.AddEquipItem(equip_slot_index, equip);
                }


                //인벤에서 제거.
                switch (window_type)
                {
                    case GameDef.eItemSlotWindow.Inventory:
                        connector.charInfo.RemoveItemByIndex(inven_slot_index);
                        break;
                    case GameDef.eItemSlotWindow.Costume:
                        connector.charInfo.RemoveCostumeByIndex(inven_slot_index);
                        break;
                    case GameDef.eItemSlotWindow.CostumeSet:
                        connector.charInfo.RemoveCostumeSetByIndex(inven_slot_index);
                        break;
                }

                Item unEquipItem = null;
                if (jsonData["inven_item"] != null)
                {
                    ItemDBInfo info = ItemToItemDBInfo(jsonData["inven_item"]);
                    unEquipItem = Item.CreateItem(info);

                    switch (unEquipItem.itemInfo.itemType)
                    {
                        case ItemInfo.eItemType.Costume_Back:
                        case ItemInfo.eItemType.Costume_Body:
                        case ItemInfo.eItemType.Costume_Head:
                            if (unEquipItem.itemCount > 0)
                                connector.charInfo.AddCostume(unEquipItem);
                            else
                                connector.charInfo.RemoveItemByUID(info.UID, connector.charInfo.inventoryCostumeData);
                            break;
                        default:
                            if (unEquipItem.itemCount > 0)
                                connector.charInfo.AddItem(unEquipItem);
                            else
                                connector.charInfo.RemoveItemByUID(info.UID, connector.charInfo.inventoryNormalData);
                            break;
                    }

                }

                UpdateItemInfos(privateData, true);
            }

            StorageWindow storageWindow = GameUI.Instance.storageWindow;
            if (storageWindow != null)
            {
                //storageWindow.OnEquipResult(pd.invenSlotIndex, pd.windowType, pd.errorCode);
                storageWindow.OnEquipResult(equip_slot_index, GameDef.eItemSlotWindow.Equip, result);
            }

        });
    }

    public void SendRequestDoUnEquipCostume(int slotIndex, string UID, int ItemID)
    {
        var parameters = new TupleList<string, string>();

        parameters.Add("hero_type", Connector.charIndex.ToString());
        parameters.Add("item_uid", UID);
        parameters.Add("item_id", ItemID.ToString());
        parameters.Add("slot_index", slotIndex.ToString());

        GameUI.Instance.DoWait();
        hive5.CallProcedure("unequip_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];
            int slot_index = -1;

            if (result == NetErrorCode.OK)
            {
                int heroType = (int)jsonData["hero_type"];
                slot_index = (int)jsonData["slot_index"];

                int charIndex = connector.charIndex;
                CharInfoData charData = connector.charInfo;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                if (privateData != null)
                    privateData.RemoveEquipItem(slot_index, null);

                Item unEquipCostume = null;
                if (jsonData["item"] != null)
                {
                    string uid = jsonData["item"]["id"].ToString();
                    int id = (int)jsonData["item"]["table_id"];

                    if (id > 0 && !string.IsNullOrEmpty(uid))
                    {
                        unEquipCostume = Item.CreateItem(id, uid, 0, 0, 1, -1, 0);
                    }
                }

                if (unEquipCostume != null)
                    charData.AddCostume(unEquipCostume);

                UpdateItemInfos(privateData, true);
            }

            StorageWindow storageWindow = GameUI.Instance.storageWindow;
            if (storageWindow != null)
            {
                storageWindow.OnUnEquipResult(slot_index, result);
            }

        });
    }

    public void SendRequestDoUnEquipCostumeSetItem(string UID, int ItemID)
    {
        var parameters = new TupleList<string, string>();

        parameters.Add("hero_type", Connector.charIndex.ToString());
        parameters.Add("item_uid", UID);
        parameters.Add("item_id", ItemID.ToString());
        parameters.Add("slot_index", "-1");

        GameUI.Instance.DoWait();
        hive5.CallProcedure("unequip_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];

            if (result == NetErrorCode.OK)
            {
                int heroType = (int)jsonData["hero_type"];
                int slot_index = (int)jsonData["slot_index"];
                int charIndex = connector.charIndex;

                CharInfoData charData = connector.charInfo;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                if (privateData != null)
                    privateData.SetCostumeSetItem(null);

                if (jsonData["item"] != null)
                {
                    string uid = jsonData["item"]["id"].ToString();
                    int id = (int)jsonData["item"]["table_id"];
                    CostumeSetItem unEquipCostumeSetItem = CostumeSetItem.Create(id, uid);

                    charData.AddCostumeSetItem(unEquipCostumeSetItem);
                }

                UpdateItemInfos(privateData, true);
            }

            StorageWindow storageWindow = GameUI.Instance.storageWindow;
            if (storageWindow != null)
            {
                storageWindow.OnUnEquipResult(0, result);
            }

        });
    }

    public void SendRequestDoUnEquipItem(int slotIndex, string UID, int ItemID)
    {
        var parameters = new TupleList<string, string>();

        parameters.Add("hero_type", Connector.charIndex.ToString());
        parameters.Add("item_uid", UID);
        parameters.Add("item_id", ItemID.ToString());
        parameters.Add("slot_index", slotIndex.ToString());

        GameUI.Instance.DoWait();
        hive5.CallProcedure("unequip_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];

            int slot_index = -1;

            if (result == NetErrorCode.OK)
            {
                slot_index = (int)jsonData["slot_index"];
                int hero_type = (int)jsonData["hero_type"];

                CharPrivateData privateData = connector.charInfo.privateDatas[hero_type];
                if (privateData != null)
                    privateData.RemoveEquipItem(slot_index, null);

                if (jsonData["item"] != null)
                {
                    Item unEquipItem = null;
                    ItemDBInfo dbInfo = ItemToItemDBInfo(jsonData["item"]);

                    if (dbInfo != null)
                        unEquipItem = Item.CreateItem(dbInfo);

                    if (unEquipItem != null && unEquipItem.itemInfo != null)
                    {
                        switch (unEquipItem.itemInfo.itemType)
                        {
                            case ItemInfo.eItemType.Costume_Back:
                            case ItemInfo.eItemType.Costume_Body:
                            case ItemInfo.eItemType.Costume_Head:
                                connector.charInfo.AddCostume(unEquipItem);
                                break;
                            default:
                                connector.charInfo.AddItem(unEquipItem);
                                break;
                        }
                    }
                }

                UpdateItemInfos(privateData, true);
            }

            StorageWindow storageWindow = GameUI.Instance.storageWindow;
            if (storageWindow != null)
            {
                storageWindow.OnUnEquipResult(slot_index, result);
            }
        });
    }

    public void RequestCreateDailyMission()
    {

        var data = new
        {

        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("enter_town_daily_mission", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "RequestCreateDailyMission");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            // 갱신할게 없다.
            if ((bool)jsonData["refresh"] == false)
                return;

            var dailyJson = jsonData["daily_mission"];
            if (jsonData["daily_mission"] != null)
            {
                int expired_time = dailyJson["expired_time"].ToInt();

                if (dailyJson["data"] != null)
                {
                    CharInfoData charData = Game.Instance.charInfoData;
                    AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

                    TableManager tableManager = TableManager.Instance;
                    AchievementTable dailyAchievementTable = tableManager != null ? tableManager.dailyAchievementTable : null;

                    if (achieveMgr != null)
                    {
                        achieveMgr.dailyAchivements.Clear();
                        achieveMgr.dailyAchievementExpireTime = DateTime.Now + new TimeSpan(1, 0, 0, 0);
                    }

                    for (int i = 0; i < dailyJson["data"].Count; ++i)
                    {
                        var info = dailyJson["data"][i];

                        int id = info["id"].ToInt();
                        int count = info["count"].ToInt();
                        bool reward = (bool)info["reward"];


                        if (achieveMgr != null)
                        {
                            Achievement dailyAchieve = dailyAchievementTable.GetTempData(id);

                            if (dailyAchieve != null)
                            {
                                dailyAchieve.curCount = count;
                                dailyAchieve.isComplete = reward;
                                achieveMgr.AddDailyAchievement(id, dailyAchieve);
                            }
                        }
                    }
                }
            }
        });
    }

    public void GetSpecialStage()
    {
        var data = new { };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("get_special_stage", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "GetSpecialStage");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result != NetErrorCode.OK)
                return;

            CharInfoData charData = Game.Instance.charInfoData;
            if (charData != null)
            {
                charData.specialStageInfo.Clear();

                if ((bool)jsonData["gold_day"] == true)
                    charData.specialStageInfo.Add(0);
                if ((bool)jsonData["material_day"] == true)
                    charData.specialStageInfo.Add(1);
            }
        });
    }

    public void GetRankingReward()
    {
        var data = new {};

        GameUI.Instance.DoWait();

        hive5.CallProcedure("get_ranking_reward", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);
                        
            CharInfoData charData = Game.Instance.charInfoData;

            if (charData != null)
                charData.SetGold(-1, jsonData["jewel"].ToInt(), jsonData["medal"].ToInt());
            
            var myWave = jsonData["myWave"];
            if (myWave != null)
            {
                PacketWaveReward waveReward = new PacketWaveReward();

                var topWaveHero = jsonData["topWaveHero"];

                if (topWaveHero != null)
                {
                    waveReward.TopInfo = new WaveRankingInfo();
                    waveReward.TopInfo.UserIndexID = topWaveHero["user_id"].ToLong();
                    waveReward.TopInfo.PlatformUserId = topWaveHero["user_id"].ToLong();
                    waveReward.TopInfo.Platform = topWaveHero["platform"].ToString();
                    waveReward.TopInfo.CharacterIndex = topWaveHero["hero_type"].ToInt();
                    waveReward.TopInfo.ranking = topWaveHero["ranking"].ToInt();
                    waveReward.TopInfo.RecordStep = topWaveHero["step"].ToInt();
                    waveReward.TopInfo.RecordSec = topWaveHero["sec"].ToInt();
                    waveReward.TopInfo.NickName = topWaveHero["nickname"].ToString();
                }

                waveReward.CharacterIndex = Connector.charIndex;
                waveReward.Ranking = myWave["ranking"].ToInt();
                waveReward.RecordStep = myWave["step"].ToInt();
                waveReward.RecordSec = myWave["sec"].ToInt();
                waveReward.RewardJewel = myWave["rewardJewel"].ToInt();
                
                Game.Instance.AddReward(waveReward);
            }

            var myArena = jsonData["myArena"];
            if (myArena != null)
            {
                PacketArenaReward arenaReward = new PacketArenaReward();

                var topArenaHero = jsonData["topArenaHero"];

                if (topArenaHero != null)
                {
                    arenaReward.TopInfo = new ArenaRankingInfo();
                    arenaReward.TopInfo.ranking = topArenaHero["ranking"].ToInt();
                    arenaReward.TopInfo.UserIndexID = topArenaHero["user_id"].ToLong();
                    arenaReward.TopInfo.PlatformUserId = topArenaHero["user_id"].ToLong();
                    arenaReward.TopInfo.CharacterIndex = topArenaHero["hero_type"].ToInt();
                    arenaReward.TopInfo.NickName = topArenaHero["nickname"].ToString();
                    arenaReward.TopInfo.platform = topArenaHero["platform"].ToString();
                    //pd.TopInfo.charLevels;
                }

                arenaReward.CharacterIndex = Connector.charIndex;
                arenaReward.RankType = myArena["rank"].ToInt();
                arenaReward.GroupRanking = myArena["group_ranking"].ToInt();
                arenaReward.RewardMedal = myArena["reward_medal"].ToInt();

                Game.Instance.AddReward(arenaReward);
            }
        });
    }

    int GetBuyStarterPackCount(JsonData shop_history, List<int> itemIDs)
    {
        if (shop_history.Keys.Contains<string>("starter_pack") == false)
            return 0;

        JsonData starter_pack = shop_history["starter_pack"];

        int buyCount = 0;

        for (int i = 0; i < starter_pack["ids"].Count; ++i)
        {
            int id = starter_pack["ids"][i].ToInt();
            if (itemIDs.IndexOf(id) >= 0)
                buyCount += 1;
        }

        return buyCount;
    }

    // StartTime-EndTime의 구매갯수를 가져온다.
    int GetBuyItemCount(JsonData shop_history, List<int> itemIDs)
    {
        if (shop_history.Keys.Contains<string>("limited_item") == false)
            return 0;
        
        JsonData limited_item = shop_history["limited_item"];

        int buyCount = 0;

        for (int i = 0; i < limited_item["ids"].Count; ++i)
        {
            if (itemIDs.IndexOf(limited_item["ids"][i].ToInt()) >= 0)
                buyCount += 1;
        }

        return buyCount;
    }

    public void RequestEnterTown()
    {
        var parameters = new
        {
            hero_type = Connector.charIndex
        };

        // enter town 호출
        hive5.CallProcedure("enter_town", parameters, (response) =>
        {
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "RequestEnterTown");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            // code here
            bool is_wave_open = (bool)jsonData["is_wave_open"];

            List<int> windowTypes = new List<int>();
            bool has_friend_request = (bool)jsonData["has_friend_request"];
            if (has_friend_request) windowTypes.Add((int)TownUI.eTOWN_UI_TYPE.FIREND);
            bool has_boss_raid = (bool)jsonData["has_boss_raid"];
            if (has_boss_raid) windowTypes.Add((int)TownUI.eTOWN_UI_TYPE.BOSSRAID);
            bool has_post = (bool)jsonData["has_post"];
            if (has_post) windowTypes.Add((int)TownUI.eTOWN_UI_TYPE.POST);

            List<int> tabs = new List<int>();
            for (int i = 0; i < windowTypes.Count; i++)
            {
                tabs.Add(0);
            }

            PacketBadgeNotify packet = new PacketBadgeNotify()
            {
                //windowTypes = new int[] {  (int)TownUI.eTOWN_UI_TYPE.FIREND, (int)TownUI.eTOWN_UI_TYPE.POST },
                //Tab = new int[] { has_friend_request == true ? 0 : 1, has_post == true ? 0 : 1 },
                windowTypes = windowTypes.ToArray(),
                Tab = tabs.ToArray(),
            };



            int nCount = packet.windowTypes.Length;

            TownUI townUI2 = GameUI.Instance.townUI;
            if (townUI2 != null)
            {
                for (int index = 0; index < nCount; ++index)
                {
                    int windowType = packet.windowTypes[index];
                    int tabIndex = packet.Tab[index];

                    townUI2.SetBudgeNotify(windowType, tabIndex);
                }
            }

            CharInfoData charData = Game.Instance.charInfoData;
            CharPrivateData privateData = charData.GetPrivateData(Connector.charIndex);;

            if (privateData == null)
                return;

            // 티켓리필.
            charData.ticket = jsonData["ticket"].ToInt();

            var shop_history = jsonData["shop_history"];
            // 이벤트리스트.
            
            int limit_count = 0;
            int buy_count = 0;            
            int eventValue = 0;
            int eventLeftTime = 0;
            charData.eventShopInfos.Clear();
            Game.Instance.eventList.Clear();
            int server_time = jsonData["server_time"].ToInt();
            Game.Instance.now = TimeHelper.UnixTimeStampToDateTime(server_time);

            int StaminaCur = (int)jsonData["stamina"]["value"];
            int StaminaMax = (int)jsonData["stamina"]["max"];
            int StaminaPresent = (int)jsonData["stamina_gift"];
            int StaminaLeftTimeSec = (int)jsonData["stamina"]["next_time"] - server_time;

            if (StaminaLeftTimeSec < 0)
                StaminaLeftTimeSec = 0;

            if (privateData != null)
                privateData.SetStamina(StaminaLeftTimeSec, StaminaCur, StaminaPresent);

            for (int i = 0; i < jsonData["events"].Count; ++i)
            {
                var eventJson = jsonData["events"][i];
                string endTime = eventJson["endTime"].ToString();
                eCashEvent eventType = eCashEvent.None;

                CMSEventType id = (CMSEventType)eventJson["id"].ToInt();
                switch (id)
                {
                    case CMSEventType.SpecialItem:
                        {
                            eventType = eCashEvent.CashBonus;
                            limit_count = eventJson["params"].ToInt();
                            
                            List<int> itemIDs = TableManager.Instance.cashShopInfoTable.GetItemIDByEventID((int)CMSEventType.SpecialItem);

                            // itemIDs 구매기록이 있는지.
                            buy_count = GetBuyItemCount(shop_history, itemIDs);
                        }
                        break;
                    case CMSEventType.StarterPack:
                        {
                            //eventType = eCashEvent.StarterPack;
                            limit_count = eventJson["params"].ToInt();
                            
                            charData.SetPackageItemLimit(limit_count);
                            /*
                            if (shop_history.Keys.Contains<string>("starter_pack") == true)
                            {
                                JsonData starter_pack = shop_history["starter_pack"];
                                int count = 0;
                                for (int j = 0; j < starter_pack["counts"].Count; ++j)
                                {
                                    count += starter_pack["counts"][j].ToInt();
                                }

                                for (int j = 0; j < starter_pack["ids"].Count; ++j)
                                {
                                    int id = starter_pack["ids"][j].ToInt();
                                    charData.SetPackageItem(id,count);
                                }
                            }
                            */
                        }
                        break;
                    case CMSEventType.GambleRate:
                    case CMSEventType.StaminaRate:
                        {
                            // 행동력절반, 암거래S확률업
                            int end_time = TimeHelper.DateTimeToUnixTimeStamp(Convert.ToDateTime(endTime));

                            privateData.gambleEventEndTime = Convert.ToDateTime(endTime);

                            eventLeftTime = end_time - server_time;

                            eventValue = eventJson["params"].ToInt();

                            if (eventLeftTime <= 0)
                                continue;

                            Game.Instance.AddEvent(id, eventLeftTime, eventValue);
                        }
                        break;
                }

                if (eventType == eCashEvent.None)
                    continue;

                EventShopInfoData newInfo = new EventShopInfoData();

                newInfo.SetCountInfo(buy_count, limit_count);
                newInfo.SetLimitTimeInfo(endTime);
                newInfo.eventType = eventType;
                newInfo.eventID = eventJson["id"].ToInt();

                charData.SetEventShopInfo(eventJson["id"].ToInt(), newInfo);
            }

            charData.attandanceCheck = jsonData["daily_login_days"].ToInt();

            // ranking reward
            var myWave = jsonData["myWave"];
            if (myWave != null)
            {
                PacketWaveReward waveReward = new PacketWaveReward();

                var topWaveHero = jsonData["topWaveHero"];

                if (topWaveHero != null)
                {
                    waveReward.TopInfo = new WaveRankingInfo();
                    waveReward.TopInfo.UserIndexID = topWaveHero["user_id"].ToLong();
                    waveReward.TopInfo.PlatformUserId = topWaveHero["user_id"].ToLong();
                    waveReward.TopInfo.Platform = topWaveHero["platform"].ToString();
                    waveReward.TopInfo.CharacterIndex = topWaveHero["hero_type"].ToInt();
                    waveReward.TopInfo.ranking = topWaveHero["ranking"].ToInt();
                    waveReward.TopInfo.RecordStep = topWaveHero["step"].ToInt();
                    waveReward.TopInfo.RecordSec = topWaveHero["sec"].ToInt();
                    waveReward.TopInfo.NickName = topWaveHero["nickname"].ToString();
                }

                waveReward.CharacterIndex = Connector.charIndex;
                waveReward.Ranking = myWave["ranking"].ToInt();
                waveReward.RecordStep = myWave["step"].ToInt();
                waveReward.RecordSec = myWave["sec"].ToInt();
                waveReward.RewardJewel = myWave["rewardJewel"].ToInt();

                Game.Instance.AddReward(waveReward);
            }

            var myArena = jsonData["myArena"];
            if (myArena != null && myArena["rank"] != null)
            {
                PacketArenaReward arenaReward = new PacketArenaReward();

                var topArenaHero = jsonData["topArenaHero"];

                if (topArenaHero != null)
                {
                    arenaReward.TopInfo = new ArenaRankingInfo();
                    arenaReward.TopInfo.ranking = topArenaHero["ranking"].ToInt();
                    arenaReward.TopInfo.UserIndexID = topArenaHero["user_id"].ToLong();
                    arenaReward.TopInfo.PlatformUserId = topArenaHero["user_id"].ToLong();
                    arenaReward.TopInfo.CharacterIndex = topArenaHero["hero_type"].ToInt();
                    arenaReward.TopInfo.NickName = topArenaHero["nickname"].ToString();
                    arenaReward.TopInfo.platform = topArenaHero["platform"].ToString();
                    arenaReward.TopInfo.charLevels = topArenaHero["heros_level"].ToArray<int>();
                    //pd.TopInfo.charLevels;
                }

                arenaReward.CharacterIndex = Connector.charIndex;
                arenaReward.RankType = myArena["rank"].ToInt();
                arenaReward.GroupRanking = myArena["groupRanking"].ToInt();
                arenaReward.RewardMedal = myArena["reward_medal"].ToInt();

                Game.Instance.AddReward(arenaReward);
            }
			
			Game.Instance.noticeInTown.Clear();
			
			for (int i = 0; i < jsonData["town_notice"].Count; ++i)
			{
				var notice = jsonData["town_notice"][i];
				Game.Instance.noticeInTown.Add(notice["description"].ToString());
				Game.Instance.noticeInTown.Add(notice["endTime"].ToString());
			}
            TownUI townUI = GameUI.Instance.townUI;

            if (townUI != null)
                townUI.OnEnterTown();
        });
    }

    public void SendRequestEnterTown()
    {
        // 일일임무.
        RequestCreateDailyMission();
        
        // 요일던전체크
        GetSpecialStage();

        RequestEnterTown();

    }

    public void SendRequestExpandSlots(GameDef.eItemSlotWindow slotWindow)
    {
        int inventory_type = -1;
        switch (slotWindow)
        {
            case GameDef.eItemSlotWindow.Inventory:
                inventory_type = 1;
                break;
            case GameDef.eItemSlotWindow.MaterialItem:
                inventory_type = 0;
                break;
            default:
                inventory_type = -1;
                break;
        }

        var data = new
        {
            hero_type = connector.charIndex,
            inventory_type = inventory_type,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("expand_slots", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(slotWindow);

                hive5Process.ErrorProcess(response, "SendRequestExpandSlots", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            // code here
            CharInfoData charData = Game.Instance.charInfoData;

            StorageWindow storeWindow = GameUI.Instance.storageWindow;
            GameDef.eItemSlotWindow slotWindowType = GameDef.eItemSlotWindow.Inventory;

            if (charData != null)
            {
                charData.SetGold(jsonData["gold"].ToInt(), jsonData["jewel"].ToInt());

                inventory_type = jsonData["inventory_type"].ToInt();
                int size_expanded = jsonData["size_expanded"].ToInt(); // 기본 사이즈 이외의 확장된 사이즈 총량
                if (inventory_type == 1)
                {
                    slotWindowType = GameDef.eItemSlotWindow.Inventory;
                    charData.expandNormalItemSlotCount = size_expanded;
                }
                else if (inventory_type == 0)
                {
                    slotWindowType = GameDef.eItemSlotWindow.MaterialItem;
                    charData.expandMaterialItemSlotCount = size_expanded;
                }

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eExpandItemSlot, 0);
                Game.Instance.SendUpdateAchievmentInfo();
            }

            if (storeWindow != null)
            {
                storeWindow.activeTabType = slotWindowType;
                storeWindow.TabButtonActive();

                storeWindow.InitTabWindow(slotWindowType);
                storeWindow.UpdateCoinInfo();
            }

        });
    }

    public void SendRequestFriendFunc(BaseFriendListWindow.eFriendListType listType, FriendSimpleInfo info)
    {
        if (info == null)
            return;

        switch (listType)
        {
            case BaseFriendListWindow.eFriendListType.FriendList:
                SendRequestSendStamina(info.UserID, info.platform, info.nick);
                break;
            case BaseFriendListWindow.eFriendListType.InviteList:
                SendRequestInviteFriend(info.UserID, info.platform);
                break;
            case BaseFriendListWindow.eFriendListType.AcceptList:
                SendRequestAcceptFriend(info.UserID, info.platform);
                break;
        }
    }

    public void SendRequestFriendList(BaseFriendListWindow.eFriendListType listType)
    {
        switch (listType)
        {
            case BaseFriendListWindow.eFriendListType.FriendList:
                RequestFriends();
                break;
            case BaseFriendListWindow.eFriendListType.InviteList:
                RequestRecommandFriends();
                break;
            case BaseFriendListWindow.eFriendListType.AcceptList:
                RequestInvitedList();
                break;
        }
    }


    private FriendInfo ConvertFriendInfo(LitJson.JsonData jsonData)
    {
        FriendInfo newInfo = new FriendInfo
        {
            nick = jsonData["nickname"] == null ? "" : jsonData["nickname"].ToString(),
            platform = (string)jsonData["user"]["platform"],
            UserID = long.Parse((string)jsonData["user"]["id"]),
            CharID = jsonData["hero_type"].ToInt(),
            Lv = jsonData["hero_level"].ToInt(),
            ShowProfileImage = (bool)jsonData["show_profile_image"],
            connTime = TimeHelper.UnixTimeStampToDateTime(jsonData["logged_in_at"].ToInt()) 
        };

        newInfo.coolTimeSec = jsonData["cool_time"].ToInt();

        return newInfo;
    }

    private FriendSimpleInfo ConvertFriendSimpleInfo(LitJson.JsonData jsonData)
    {
        FriendSimpleInfo newInfo = new FriendSimpleInfo
		{
        	nick = jsonData["nickname"] == null ? "" : jsonData["nickname"].ToString(),
            platform = (string)jsonData["user"]["platform"],
            UserID = long.Parse((string)jsonData["user"]["id"]),
            CharID = jsonData["hero_type"].ToInt(),
            Lv = jsonData["hero_level"].ToInt(),
            ShowProfileImage = (bool)jsonData["show_profile_image"],
            connTime = TimeHelper.UnixTimeStampToDateTime(jsonData["logged_in_at"].ToInt()) 
        };
                
        return newInfo;
    }
    private List<FriendInfo> ConvertFriendInfos(LitJson.JsonData jsonData)
    {
        List<FriendInfo> newList = new List<FriendInfo>();

        for (int i = 0; i < jsonData.Count; i++)
        {
            var item = jsonData[i];

            var info = ConvertFriendInfo(item);
            newList.Add(info);
        }

        return newList;
    }

    private List<FriendSimpleInfo> ConvertFriendSimpleInfos(LitJson.JsonData jsonData)
    {
        List<FriendSimpleInfo> newList = new List<FriendSimpleInfo>();

        for (int i = 0; i < jsonData.Count; i++)
        {
            var item = jsonData[i];

            FriendSimpleInfo info = ConvertFriendSimpleInfo(item);
            newList.Add(info);
        }
        return newList;
    }

    public void RequestFriends()
    {
        var data = new
        {

        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("list_friends", data, (response) =>
       {
           GameUI.Instance.CancelWait();

           FriendWindow friendWindow = GameUI.Instance.friendWindow;
           if (friendWindow != null)
               friendWindow.requestCount = 0;

           if (response.ResultCode != Hive5ResultCode.Success)
           {
               hive5Process.ErrorProcess(response, "RequestFriends");
               return;
           }

           var body = response.ResultData as CallProcedureResponseBody;
           string jsonString = body.CallReturn;
           var jsonData = LitJson.JsonMapper.ToObject(jsonString);

           NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

           if (friendWindow != null)
           {
               int curCount = 0;

               if (result == NetErrorCode.OK)
               {
                   LitJson.JsonData friendsData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["friends"]));
                   List<FriendInfo> friends = ConvertFriendInfos(friendsData);

                   FriendInfo[] arrayFriends = friends.ToArray();

                   Game.Instance.AddMyFriendList(arrayFriends);

                   BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.FriendList);
                   if (friendListWindow != null)
                       friendListWindow.SetInfos(arrayFriends);

                   curCount = friends.Count;
               }
               else
               {
                   friendWindow.OnErrorMessage(result, null);
               }

               friendWindow.SetMaxInfo(curCount);
           }
       });
    }

    public void RequestRecommandFriends()
    {
        var data = new
        {

        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("list_friend_candidates", data, (response) =>
        {
            GameUI.Instance.CancelWait();
            FriendWindow friendWindow = GameUI.Instance.friendWindow;
            if (friendWindow != null)
                friendWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "RequestRecommandFriends");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (friendWindow != null)
            {
                if (result == NetErrorCode.OK)
                {
                    LitJson.JsonData friendsData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["friends"]));
                    List<FriendSimpleInfo> friends = ConvertFriendSimpleInfos(friendsData);

                    FriendSimpleInfo[] arrayFriends = friends.ToArray();

                    Game.Instance.AddRecommandFirendList(arrayFriends);

                    BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.InviteList);
                    if (friendListWindow != null)
                        friendListWindow.SetInfos(arrayFriends);
                }
                else
                {
                    friendWindow.OnErrorMessage(result, null);
                }
            }
        });
    }

    public void RequestInvitedList()
    {
        var data = new
        {

        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("list_friend_requests", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            FriendWindow friendWindow = GameUI.Instance.friendWindow;
            if (friendWindow != null)
                friendWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "RequestInvitedList");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (friendWindow != null)
            {
                if (result == NetErrorCode.OK)
                {
                    LitJson.JsonData friendsData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["friends"]));
                    List<FriendSimpleInfo> friends = ConvertFriendSimpleInfos(friendsData);

                    FriendSimpleInfo[] arrayFriends = friends.ToArray();
                    Game.Instance.AddAcceptFriendList(arrayFriends);

                    BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.AcceptList);
                    if (friendListWindow != null)
                        friendListWindow.SetInfos(arrayFriends);
                }
                else
                {
                    friendWindow.OnErrorMessage(result, null);
                }
            }
        });
    }

    public void SendRequestGambleInfo()
    {
        var parameters = new TupleList<string, string>();
        parameters.Add("hero_type", connector.charIndex.ToString());

        GameUI.Instance.DoWait();

        hive5.CallProcedure("get_gamble_info", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "SendRequestGambleInfo");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            int charIndex = (int)jsonData["hero_type"];
            int refresh_leftTime = (int)jsonData["refresh_left_time"];
            int gamble_event_time = (int)jsonData["gamble_event_time"];
            int result = (int)jsonData["result"];

            /*
            List<GambleItem> gambleItems = new List<GambleItem>();
            foreach (var item in jsonData["gambleItems"]) {
                gambleItems.Add(new GambleItem(){
                    ID = item["ID"].ToInt(),
                    Grade = item["Grade"].ToInt(),
                    itemRate = item["itemRate"].ToInt(),
                });
            }
            */

            var gambleItems = LitJson.JsonMapper.ToObject<GambleItem[]>(BigFoot.ConverJson.MakeToJson(jsonData["gambleItems"]));

            // code here
            if (result == (int)NetErrorCode.OK)
            {
                System.DateTime newExpireTime = GambleWindow.refreshExpireTime;

                CharPrivateData privateData = null;
                CharInfoData charData = connector.charInfo;
                if (charData != null)
                    privateData = charData.GetPrivateData(charIndex);

                if (privateData != null)
                {
                    if (refresh_leftTime >= 0)
                        privateData.SetGambleTime(refresh_leftTime);

                    privateData.SetGambleInfo(gambleItems, gamble_event_time, null);

                    Logger.DebugLog("Recv GambleInfo LeftTimeSec:" + refresh_leftTime.ToString());

                    newExpireTime = privateData.refreshGambleExpireTime;

                    GambleWindow.refreshExpireTime = newExpireTime;
                }

                GameUI.Instance.townUI.OnGambleWindowOpen();
            }
        });
    }

    public void SendRequestGameReview()
    {
        GameUI.Instance.DoWait();

        hive5.CallProcedure("get_review_url", null, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "SendRequestGameReview");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            // code here
            CharInfoData charData = Game.Instance.charInfoData;
            if (charData != null)
                charData.gameReviewURL = (string)jsonData["url"];
        });
    }

    public void SendRequestInviteFriend(long targetUserID, string platform = "kakao")
    {
        var data = new
        {
            platform = platform,
            id = targetUserID.ToString(),
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("request_friend", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            FriendWindow friendWindow = GameUI.Instance.friendWindow;
            if (friendWindow != null)
                friendWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(targetUserID);
                objects.Add(platform);

                hive5Process.ErrorProcess(response, "SendRequestInviteFriend");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            if (friendWindow != null)
            {
                if (result == NetErrorCode.OK)
                {
                    Game.Instance.RemoveRecommandFriend(targetUserID.ToString());
                }
                else
                {
                    friendWindow.OnErrorMessage(result, null);
                }
            }
        });
    }

    public void SendRequestMasteryReset()
    {
        //throw new NotImplementedException();
        var parameters = new TupleList<string, string>();
        parameters.Add("hero_type", connector.charIndex.ToString());

        GameUI.Instance.DoWait();

        hive5.CallProcedure("reset_mastery_skill", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "SendRequestMasteryReset");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();

            int charIndex = jsonData["hero_type"].ToInt();
            int skillPoint = jsonData["skill_point"].ToInt();
            int gold = jsonData["gold"].ToInt();
            int jewel = jsonData["jewel"].ToInt();

            // code here
            MasteryWindow_New masteryWindow = GameUI.Instance.masteryWindow;

            if (errorCode == NetErrorCode.OK)
            {
                CharPrivateData privateData = connector.charInfo.GetPrivateData(charIndex);
                if (privateData != null)
                {
                    privateData.baseInfo.SkillPoint = skillPoint;

                    privateData.ResetMastery();
                }

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eResetSkillPoint, 0);

                connector.charInfo.SetGold(gold, jewel);
            }

            if (masteryWindow != null)
                masteryWindow.OnResultResetMastery(errorCode);

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }

    public void SendRequestMasteryUpgrade(SkillUpgradeDBInfo info)
    {
        var data = new
        {
            hero_type = connector.charIndex,
            skill_ids = info.SkillIDs,
            skill_lvs = info.Levels,
            skill_adds = info.Adds
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("apply_mastery_skill", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(info);

                hive5Process.ErrorProcess(response, "SendRequestMasteryUpgrade", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();

            int charIndex = jsonData["hero_type"].ToInt();
            int[] skill_ids = LitJson.JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["skill_ids"]));
            int[] skill_levels = LitJson.JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["skill_levels"]));
            int skill_point = jsonData["skill_point"].ToInt();

            // code here
            MasteryWindow_New masteryWindow = GameUI.Instance.masteryWindow;
            if (errorCode == NetErrorCode.OK)
            {
                SkillUpgradeDBInfo skillDBInfo = new SkillUpgradeDBInfo();
                skillDBInfo.SkillIDs = skill_ids;
                skillDBInfo.Levels = skill_levels;

                CharPrivateData privateData = connector.charInfo.GetPrivateData(charIndex);
                if (privateData != null)
                {
                    int nCount = Mathf.Min(skill_ids.Length, skill_levels.Length);
                    int skillID = 0;
                    int skillLv = 0;
                    for (int index = 0; index < nCount; ++index)
                    {
                        skillID = skill_ids[index];
                        skillLv = skill_levels[index];

                        privateData.SetMasteryData(skillID, skillLv);
                    }

                    privateData.baseInfo.SkillPoint = skill_point;
                }

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUseSkillPoint, 0);

                if (masteryWindow != null)
                    masteryWindow.OnResultApplyMastery(skillDBInfo);
            }
            else
            {
                if (masteryWindow != null)
                    masteryWindow.OnErrorMessage(errorCode, masteryWindow);
            }

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }

    public void SendRequestMemberSecession(string accountStr, string passStr, AccountType type)
    {
        if (type == AccountType.Kakao)
        {
            if (Game.Instance.AndroidManager != null)
                Game.Instance.AndroidManager.CallUnRegisterKakao();
        }

        var data = new
        {
            user_ids = accountStr,
            pass = passStr,
            account_type = type
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("request_member_secession", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(accountStr);
                objects.Add(passStr);
                objects.Add(type);

                hive5Process.ErrorProcess(response, "SendRequestMemberSecession", objects.ToArray());

                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
				hive5.Unregister((unregist) =>
				{
					
				});
            }
            else
            {

            }
        });
    }

    public void SendRequestPostInfo(string id = "-1")
    {
        var data = new {id = id};

        List<string> delete_ids = null;
        List<string> delete_reward_ids = null;

        GameUI.Instance.DoWait();

        hive5.CallProcedure("get_mails", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
			TownUI townUI = GameUI.Instance.townUI;
            if (townUI == null)
                return;

			PostWindow postWindow = GameUI.Instance.postWindow;
			if (postWindow != null)
				postWindow.requestCount = 0;
			if (townUI != null)
				townUI.requestCount = 0;
			
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(id);

                hive5Process.ErrorProcess(response, "SendRequestPostInfo", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            
            int server_time = jsonData["server_time"].ToInt();
            int mail_count = jsonData["count"].ToInt();
            Game.Instance.postUpdateTime = TimeHelper.UnixTimeStampToDateTime(server_time);
            if (result == NetErrorCode.OK)
            {                
                List<MailInfo> posts = new List<MailInfo>();
                
                for (int i = 0; i < jsonData["posts"].Count; ++i)
                {
                    var jsonPost = jsonData["posts"][i];

                    MailInfo mail = new MailInfo();
                    mail.Index = jsonPost["id"].ToString();

                    try
                    {
                        mail.reward_id = jsonPost["reward_id"].ToString();
                        string contentstr = jsonPost["content"].ToString();
                        var content = LitJson.JsonMapper.ToObject(contentstr);
                        mail.Type = (MailType)content["type"].ToInt();
                        mail.Sender = content.Keys.Contains("message") == true ? content["message"].ToString() : "";
                        mail.CreateTime = Convert.ToDateTime(jsonPost["created_at"].ToString());
                        mail.CreateTime = mail.CreateTime.AddDays(15);
                        mail.CreateTime = mail.CreateTime.AddHours(9);

                        TimeSpan delta = mail.CreateTime - Game.Instance.postUpdateTime;
                        mail.LeftDestroySec = (int)delta.TotalSeconds; // todo.
                        mail.rewards = new MailReward[] { };

                        List<MailReward> gifts = new List<MailReward>();
                        for (int j = 0; j < content["gifts"].Count; ++j)
                        {
                            var gift = content["gifts"][j];
                            MailReward reward = new MailReward();
                            switch (gift["kind"].ToString())
                            {
                                case "item": reward.ItemID = gift["id"].ToInt(); break;
                                case "gold": reward.Gold = gift["amount"].ToInt(); break;
                                case "jewel": reward.Jewel = gift["amount"].ToInt(); break;
                                case "stamina": reward.Stamina = gift["amount"].ToInt(); break;
                                case "gamble_coupon": reward.coupon = gift["amount"].ToInt(); break;
                                case "arena_ticket": reward.ticket = gift["amount"].ToInt(); break;
                                case "potion1": reward.potion1 = gift["amount"].ToInt(); break;
                                case "potion2": reward.potion2 = gift["amount"].ToInt(); break;
                                case "buff_package": reward.ItemID = gift["id"].ToInt(); break;
                                case "awaken_point": reward.awaken_point = gift["amount"].ToInt(); break;
                                case "start_package": reward.ItemID = gift["package_item"].ToInt(); break;
                            }

                            gifts.Add(reward);
                        }
                        mail.rewards = gifts.ToArray();
                        posts.Add(mail);
                    }
                    catch (Exception)
                    {
                        if (delete_ids == null)
                            delete_ids = new List<string>();

                        if (delete_reward_ids == null)
                            delete_reward_ids = new List<string>();

                        if (string.IsNullOrEmpty(mail.reward_id) == false)
                            delete_reward_ids.Add(mail.reward_id);
                        else
                            delete_ids.Add(mail.Index);
                    }
                }

				Game.Instance.postItemCount = mail_count;
                if (id == "-1")
                {
                    //MailInfo
                    Game.Instance.postItemList = posts;

                    townUI.OnPostWindow();
                }
                else
                {
                    if (posts.Count > 0)
                    {
                        Game.Instance.postItemList.AddRange(posts);

                        if (postWindow != null)
                            postWindow.AddPostItems(posts);
                    }
                }
            }
            // Json오류 메일 삭제 요청한다.
            if (delete_ids != null || delete_reward_ids != null)
            {
                var param = new { 
                    mail_ids = delete_ids,
                    mail_reward_ids = delete_reward_ids,
                };

                hive5.CallProcedure("delete_mails", param, (delete_response) =>
                {
                    if (delete_response.ResultCode == Hive5ResultCode.Success)
                    {
                        var delete_body = delete_response.ResultData as CallProcedureResponseBody;
                        var json = LitJson.JsonMapper.ToObject(body.CallReturn);

                        int count = json["count"].ToInt();
                    }
                });
            }
        });
    }


    public void SelectHero(int hero_type)
    {
        var data = new
        {
            hero_type = hero_type,
        };

        hive5.CallProcedure("select_hero", data, (response) =>
        {
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(hero_type);

                hive5Process.ErrorProcess(response, "SelectHero", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                //

            }
        });
    }


    public void SendRequestPostItem(MailInfo info)
    {
        Hive5Client hive5 = Hive5Client.Instance;

        GameUI.Instance.DoWait();
        
        hive5.ApplyReward(Convert.ToInt64(info.reward_id), true, (response) => 
        {
            GameUI.Instance.CancelWait();

            PostWindow postWindow = GameUI.Instance.postWindow;

            if (postWindow == null)
                return;
            NetErrorCode result = NetErrorCode.OK;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                if (response.ResultMessage.IndexOf("material_inventory full") > 0)
                {
                    result = NetErrorCode.NotEnoughInvenMaterial;
                }

                else if (response.ResultMessage.IndexOf("inventory full") > 0)
                {
                    result = NetErrorCode.NotEnoughInven;
                }

                if (result != NetErrorCode.OK)
                {
                    postWindow.requestCount = 0;
                    postWindow.OnErrorMessage(result, postWindow);
                }
                else 
                {
                    hive5Process.ErrorProcess(response);
                }

                return;
            }

            var body = response.ResultData as ApplyRewardResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                CharInfoData charData = Game.Instance.charInfoData;

                if (charData == null)
                    return;

                var userJson = jsonData["user"];

                charData.SetGold((int)userJson["gold"], (int)userJson["jewel"], (int)userJson["medal"]);
                charData.gambleCoupon = (int)userJson["coupon"];
                charData.potion1 = (int)userJson["potion1"];
                charData.potion1Present = (int)userJson["potion1_gift"];
                charData.potion2 = (int)userJson["potion2"];
                charData.potion2Present = (int)userJson["potion2_gift"];
                charData.ticket = (int)userJson["ticket"]["value"];
                
                CharPrivateData privateData = connector.charInfo.privateDatas[Connector.charIndex];
                if (privateData != null && privateData.baseInfo != null)
                {
                    privateData.baseInfo.StaminaCur = jsonData["stamina"].ToInt();
					privateData.SetPresentAwakenPoint(jsonData["awaken_point_gift"].ToInt());
                }

                for (int i = 0; i < jsonData["items"].Count; ++i)
                {
                    ItemDBInfo dbinfo = ItemToItemDBInfo(jsonData["items"][i]);

                    AddNewItem(charData, dbinfo, true);
                    
                }

                postWindow.SetRead(info.Index);

                if (GameUI.Instance.myCharInfos != null)
                    GameUI.Instance.myCharInfos.UpdateCoinInfo();

                postWindow.requestCount = 0;
            }
            else
            {
                postWindow.requestCount = 0;
                postWindow.OnErrorMessage(result, postWindow);
            }
        });
    }

    public void GetUserInfoForPost()
    {
        // 인벤토리 풀이라도 받아진게 있을 수있다.
        var data = new
        {
            hero_type = Connector.charIndex
        };

        hive5.CallProcedure("get_user_for_post", data, (itemAllresponse) =>
        {
            if (itemAllresponse.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(itemAllresponse, "GetUserInfoForPost");
				
                return;
            }

            var body = itemAllresponse.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            CharInfoData charData = Game.Instance.charInfoData;


            //costume_sets
            for (int i = 0; i < jsonData["costumesets"].Count; i++)
            {
                var infoJson = jsonData["costumesets"][i];

                string UID = infoJson["id"].ToString();

                int slotIndex = charData.GetItemIndexByUID(UID, charData.inventoryMaterialData);
                if (slotIndex == -1)
                {
                    int itemID = infoJson["table_id"].ToInt();

                    CostumeSetItem newItem = CostumeSetItem.Create(itemID, UID);
                    charData.AddCostumeSetItem(newItem);
                }
            }

            //materials
            for (int i = 0; i < jsonData["materials"].Count; i++)
            {
                var infoJson = jsonData["materials"][i];

                string UID = infoJson["id"].ToString();

                int slotIndex = charData.GetItemIndexByUID(UID, charData.inventoryMaterialData);
                if (slotIndex == -1)
                {
                    int itemCount = infoJson["count"].ToInt();
                    int itemID = infoJson["table_id"].ToInt();

                    MaterialItemDBInfo dbInfo = new MaterialItemDBInfo();
                    dbInfo.UID = UID;
                    dbInfo.ID = itemID;
                    dbInfo.Count = itemCount;

                    Item newItem = Item.CreateItem(dbInfo);
                    newItem.IsNewItem = true;
                    charData.AddMaterial(newItem);
                }
            }

            //items
            for (int i = 0; i < jsonData["items"].Count; i++)
            {
                var infoJson = jsonData["items"][i];

                string UID = infoJson["id"].ToString();

                int slotIndex = charData.GetItemIndexByUID(UID, charData.inventoryMaterialData);
                if (slotIndex == -1)
                {
                    int itemCount = infoJson["count"].ToInt();
                    int itemID = infoJson["table_id"].ToInt();
                    int itemGrade = infoJson["grade"].ToInt();
                    int itemReinforce = infoJson["reinforce"].ToInt();
                    int itemRate = infoJson["rate"].ToInt();

                    ItemDBInfo dbInfo = new ItemDBInfo();
                    dbInfo.UID = UID;
                    dbInfo.ID = itemID;
                    dbInfo.Count = itemCount;
                    dbInfo.Grade = itemGrade;
                    dbInfo.Reinforce = itemReinforce;
                    dbInfo.Rate = itemRate;

                    Item newItem = Item.CreateItem(dbInfo);
                    newItem.IsNewItem = true;
                    charData.AddItem(newItem);
                }
            }

            //costume
            for (int i = 0; i < jsonData["costumes"].Count; i++)
            {
                var infoJson = jsonData["costumes"][i];

                string UID = infoJson["id"].ToString();

                int slotIndex = charData.GetItemIndexByUID(UID, charData.inventoryMaterialData);
                if (slotIndex == -1)
                {
                    int itemID = infoJson["table_id"].ToInt();

                    Item newItem = Item.CreateItem(itemID, UID, 0, 0, 1);
                    newItem.IsNewItem = true;
                    charData.AddCostume(newItem);
                }
            }

            //stamina, 각성포인트(선물)
            int charIndex = 0;
            if (Game.Instance.Connector != null)
                charIndex = Game.Instance.Connector.charIndex;
            CharPrivateData privateData = charData.GetPrivateData(charIndex);
            if (privateData != null)
            {
                privateData.baseInfo.StaminaCur = jsonData["hero_stamina"].ToInt();
                privateData.baseInfo.StaminaPresent = jsonData["hero_stamina_gift"].ToInt();
				privateData.SetPresentAwakenPoint(jsonData["awaken_point_gift"].ToInt());
            }

            //user(돈, 보석, 티켓, 포션1, 포션2, 쿠폰,
            var userInfoJson = jsonData["user"];
            int gold = userInfoJson["gold"].ToInt();
            int jewel = userInfoJson["jewel"].ToInt();
            charData.SetGold(gold, jewel);

            charData.potion1 = userInfoJson["potion1"].ToInt();
            charData.potion1Present = userInfoJson["potion1_gift"].ToInt();
            charData.potion2 = userInfoJson["potion2"].ToInt();
            charData.potion2Present = userInfoJson["potion2_gift"].ToInt();

            charData.gambleCoupon = userInfoJson["coupon"].ToInt();

            var ticketInfoJson = userInfoJson["ticket"];
            charData.ticket = ticketInfoJson["value"].ToInt();

            //todo. 버프 패키지 처리 해야함..

        });
    }

	void ApplyAllRewards(int executeTime)
	{
        ++executeTime;

        Hive5Client hive5 = Hive5Client.Instance;
        GameUI.Instance.DoWait();

        hive5.ApplyAllRewards(true, (response) =>
        {
            switch (response.ResultCode)
            {
                case Hive5ResultCode.Success:
                    {
                        GetUserInfoForPost();

                        PostWindow postWindow = GameUI.Instance.postWindow;

                        postWindow.SetReadAll();

                    }
                    break;
                case Hive5ResultCode.ExecutionTimeout:
                    {
                       // if (executeTime > 10)
                        {
                            ApplyAllRewards(executeTime);
                            return;
                        }
                    }
                    break;
                default:
                    {
                        NetErrorCode result = NetErrorCode.OK;

                        if (response.ResultMessage.IndexOf("material_inventory full") > 0)
                        {
                            result = NetErrorCode.NotEnoughInvenMaterial;

                            GetUserInfoForPost();

                            SendRequestPostInfo();
                        }

                        else if (response.ResultMessage.IndexOf("inventory full") > 0)
                        {
                            result = NetErrorCode.NotEnoughInven;

                            GetUserInfoForPost();

                            SendRequestPostInfo();

                        }
                        else if (response.ResultMessage.IndexOf("Timeout of Controller Action") > 0)
                        {
                           // if (executeTime > 1)
                            {
                                ApplyAllRewards(executeTime);
                                return;
                            }

                            response.ResultCode = Hive5ResultCode.ExecutionTimeout;

                        }

                        if (result != NetErrorCode.OK)
                        {
                            PostWindow postWindow = GameUI.Instance.postWindow;

                            postWindow.requestCount = 0;
                            postWindow.OnErrorMessage(result, postWindow);
                        }
                        else
                        {
                            hive5Process.ErrorProcess(response);
                        }
                    }
                    break;
            }

            GameUI.Instance.CancelWait();

            //ApplyAllRewardsResponseBody body = response.ResultData as ApplyAllRewardsResponseBody;
        });
    }

    public void SendRequestPostItemAll()
    {
        ApplyAllRewards(0);

        //GameUI.Instance.CancelWait();

    }

    public void SendRequestPostMessage(MailInfo info)
    {
        throw new NotImplementedException();
    }

    public void SendRequestReinforceItem(int slotIndex, GameDef.eItemSlotWindow slotWindow, string UID, int ItemID, string[] delItems)
    {
        var data = new
        {
            hero_type = Connector.charIndex,
            slot_index = slotIndex,
            slot_window = (int)slotWindow,
            item_id = UID,

            materials = delItems, // 강화재료. 실제아이템들.
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("reinforce_item", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(slotIndex);
                objects.Add(slotWindow);
                objects.Add(UID);
                objects.Add(ItemID);
                objects.Add(delItems);

                hive5Process.ErrorProcess(response, "SendRequestReinforceItem", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            int reinforce_step = 0;
            int exp = 0;

            StorageWindow storageWindow = GameUI.Instance.storageWindow;

            BaseTradeItemInfo baseTradeInfo = null;
            if (result == NetErrorCode.OK)
            {
                connector.charInfo.SetGold(jsonData["gold"].ToInt(), jsonData["jewel"].ToInt());

                var itemJson = jsonData["item"];

                baseTradeInfo = new BaseTradeItemInfo();
                baseTradeInfo.UID = itemJson["id"].ToString();
                baseTradeInfo.ItemID = itemJson["table_id"].ToInt();
                baseTradeInfo.slotIndex = jsonData["slot_index"].ToInt();
                baseTradeInfo.windowType = (GameDef.eItemSlotWindow)jsonData["slot_window"].ToInt();

                reinforce_step = itemJson["reinforce"].ToInt();
                exp = itemJson["exp"].ToInt();

                CharPrivateData privateData = connector.charInfo.privateDatas[Connector.charIndex];
                Item resultItem = connector.charInfo.ReinforceItem(privateData, delItems, baseTradeInfo, reinforce_step);
                int newSlotIndex = connector.charInfo.FindSlotIndex(resultItem, privateData, baseTradeInfo.windowType);

                UpdateItemInfos(privateData, false);

                if (storageWindow != null)
                    storageWindow.UpdateWindow();

                if (storageWindow != null && storageWindow.reinforceWindow != null)
                    storageWindow.reinforceWindow.UpdateReinforceItem(resultItem, newSlotIndex, baseTradeInfo.windowType);

                //아이템 강화 업적.
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eReinforceItem, 0);

                if (resultItem.itemGrade == Item.limitCompositionStep && reinforce_step == Item.limitReinforceStep)
                {
                    Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUpgradeToLimit, 0);
                    Game.Instance.SendUpdateAchievmentInfo();
                }
            }

            if (storageWindow != null && storageWindow.reinforceWindow != null)
            {
                storageWindow.reinforceWindow.resultItemExp = (uint)exp;
                storageWindow.reinforceWindow.OnReinforceResult(result, ref baseTradeInfo, reinforce_step);
            }

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }

    public void SendRequestRevival(int gold, int jewel)
    {
        var data = new
        {
            hero_type = Connector.charIndex,
            gold = gold,
            jewel = jewel
        };

        hive5.CallProcedure("request_revival", data, (response) =>
        {
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(gold);
                objects.Add(jewel);

                hive5Process.ErrorProcess(response, "SendRequestRevival", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();
            int newGold = jsonData["gold"].ToInt();
            int newJewel = jsonData["jewel"].ToInt();

            // code here
            PlayerController player = Game.Instance.player;
            RevivalController revivalController = player != null ? player.gameObject.GetComponent<RevivalController>() : null;

            if (errorCode == 0)
            {
                CharInfoData charData = Game.Instance.charInfoData;
                if (charData != null)
                    charData.SetGold(newGold, newJewel);

                if (revivalController != null)
                    revivalController.OnRevivalSuccess();
            }
            else
            {
                if (revivalController != null)
                    revivalController.OnRevivalFailed();
            }
        });
    }

    public void SendRequestSellItem(int hero_type, string item_uid, int item_id, int slot_index, int window_type, bool shop, int inven_type)
    {
        var parameters = new TupleList<string, string>();

        parameters.Add("hero_type", hero_type.ToString());
        parameters.Add("item_uid", item_uid);
        parameters.Add("item_id", item_id.ToString());
        parameters.Add("slot_index", slot_index.ToString());
        parameters.Add("window_type", window_type.ToString());
        parameters.Add("shop", shop.ToString());
        parameters.Add("inven_type", ((int)inven_type).ToString());

        GameUI.Instance.DoWait();
        hive5.CallProcedure("sell_item", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            Debug.Log("Procedure return:" + response);

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(hero_type);
                objects.Add(item_uid);
                objects.Add(item_id);
                objects.Add(slot_index);
                objects.Add(window_type);
                objects.Add(shop);
                objects.Add(inven_type);

                hive5Process.ErrorProcess(response, "SendRequestSellItem", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)(int)jsonData["result"];

            int gold = -1;
            int jewel = -1;
            string uid = "";
            GameDef.eItemSlotWindow slot_type = GameDef.eItemSlotWindow.Equip;

            if (result == NetErrorCode.OK)
            {
                slot_type = (GameDef.eItemSlotWindow)jsonData["window_type"].ToInt();
                gold = (int)jsonData["gold"];
                jewel = (int)jsonData["jewel"];
                uid = jsonData["item_uid"].ToString();
            }
            CharPrivateData privateData = Connector.charInfo.privateDatas[hero_type];

            if (result == NetErrorCode.OK)
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eSellItem, 0);

            if (shop)
            {
                ShopWindow shopWindow = GameUI.Instance.shopWindow;

                if (shopWindow == null)
                    Logger.DebugLog("SellItemProcess shopWindow null");

                switch (slot_type)
                {
                    case GameDef.eItemSlotWindow.Equip:
                        if (result == NetErrorCode.OK)
                        {
                            connector.charInfo.RemoveEquipItem(hero_type, slot_index);
                            connector.charInfo.SetGold(gold, jewel);
                        }
                        UpdateItemInfos(privateData, true);

                        if (shopWindow != null)
                            shopWindow.OnSellEquipItemResult(slot_index, result);
                        break;
                    default:
                        Logger.DebugLog("SellItemProcess windowType:" + window_type.ToString());
                        break;
                }
            }
            else
            {
                StorageWindow storageWindow = GameUI.Instance.storageWindow;

                if (storageWindow == null)
                    Logger.DebugLog("SellItemProcess StorageWindow null");

                switch (slot_type)
                {
                    case GameDef.eItemSlotWindow.Equip:
                        if (result == NetErrorCode.OK)
                        {
                            connector.charInfo.RemoveEquipItem(hero_type, slot_index);

                            connector.charInfo.SetGold(gold, jewel);
                        }

                        UpdateItemInfos(privateData, true);

                        if (storageWindow != null)
                            storageWindow.OnSellEquipItemResult(slot_index, result);
                        break;
                    case GameDef.eItemSlotWindow.Inventory:
                        if (result == NetErrorCode.OK)
                        {
                            //connector.userInfo.RemoveItem(slot_index);
                            //connector.userInfo.SetMoney(jewel, gold);

                            connector.charInfo.RemoveItemByIndex(slot_index, uid);
                            connector.charInfo.SetGold(gold, jewel);
                        }

                        UpdateItemInfos(privateData, true);

                        if (storageWindow != null)
                            storageWindow.OnSellNormalItemResult(slot_index, result);
                        break;
                    case GameDef.eItemSlotWindow.Costume:
                        if (result == NetErrorCode.OK)
                        {
                            //connector.userInfo.RemoveCostume(slot_index);
                            //connector.userInfo.SetMoney(jewel, gold);

                            connector.charInfo.RemoveCostumeByIndex(slot_index, uid);
                            connector.charInfo.SetGold(gold, jewel);
                        }

                        UpdateItemInfos(privateData, true);

                        if (storageWindow != null)
                            storageWindow.OnSellCostumeItemResult(slot_index, result);
                        break;
                    case GameDef.eItemSlotWindow.MaterialItem:
                        if (result == NetErrorCode.OK)
                        {
                            connector.charInfo.RemovMaterialItemByIndex(slot_index, uid);
                            connector.charInfo.SetGold(gold, jewel);
                        }

                        UpdateItemInfos(privateData, true);

                        if (storageWindow != null)
                            storageWindow.OnSellMaterialItemResult(slot_index, result);
                        break;
                    case GameDef.eItemSlotWindow.CostumeSet:
                        if (result == NetErrorCode.OK)
                        {
                            connector.charInfo.RemoveCostumeSetByIndex(slot_index, uid);
                            connector.charInfo.SetGold(gold, jewel);
                        }

                        UpdateItemInfos(privateData, true);

                        if (storageWindow != null)
                            storageWindow.OnSellCostumeSetItemResult(slot_index, result);
                        break;
                    default:
                        Logger.DebugLog("SellItemProcess windowType:" + window_type.ToString());
                        break;
                }
            }

            Game.Instance.SendUpdateAchievmentInfo();

        });
    }

    public void SendRequestSellCostumeItem(int charIndex, int slotIndex, int ItemID, string ItemUID)
    {
        SendRequestSellItem(charIndex, ItemUID, ItemID, slotIndex, (int)GameDef.eItemSlotWindow.Costume, true, Inven_Type.costume);
    }

    public void SendRequestSellCostumeItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
    {
        SendRequestSellItem(charIndex, ItemUID, ItemID, slotIndex, (int)GameDef.eItemSlotWindow.Costume, false, Inven_Type.costume);
    }

    public void SendRequestSellCostumeSetItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
    {
        SendRequestSellItem(charIndex, ItemUID, ItemID, slotIndex, (int)GameDef.eItemSlotWindow.CostumeSet, false, Inven_Type.costume);
    }

    public void SendRequestSellEquipItem(int charIndex, int slotIndex, int ItemID, string ItemUID)
    {
        SendRequestSellItem(charIndex, ItemUID, ItemID, slotIndex, (int)GameDef.eItemSlotWindow.Equip, true, Inven_Type.equip);
    }

    public void SendRequestSellEquipItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
    {
        SendRequestSellItem(charIndex, ItemUID, ItemID, slotIndex, (int)GameDef.eItemSlotWindow.Equip, false, Inven_Type.equip);
    }

    public void SendRequestSellMaterialItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
    {
        SendRequestSellItem(charIndex, ItemUID, ItemID, slotIndex, (int)GameDef.eItemSlotWindow.MaterialItem, false, Inven_Type.material);
    }

    public void SendRequestSellNormalItemFromStorage(int charIndex, int slotIndex, int ItemID, string ItemUID)
    {
        SendRequestSellItem(charIndex, ItemUID, ItemID, slotIndex, (int)GameDef.eItemSlotWindow.Inventory, false, Inven_Type.item);
    }

    public void SendRequestSendStamina(long targetUserID, string platform = "kakao", string nick = "")
    {
        var data = new
        {
            platform = platform,
            user_id = targetUserID.ToString(),
            nick = nick,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("send_stamina_to_friend", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            FriendWindow friendWindow = GameUI.Instance.friendWindow;
            if (friendWindow != null)
                friendWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(targetUserID);
                objects.Add(platform);
                objects.Add(nick);
                hive5Process.ErrorProcess(response, "SendRequestSendStamina", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            PacketSendStaminaToFriend pd = new PacketSendStaminaToFriend()
            {

            };

            if (friendWindow != null)
            {
                if (result == NetErrorCode.OK)
                {
                    string friend_platform = (string)jsonData["friend_platform"];
                    long userID = long.Parse(jsonData["friend_id"].ToString());
                    int timeSec = jsonData["cool_time"].ToInt();

                    BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.FriendList);
                    if (friendListWindow != null)
                        friendListWindow.UpdateInfo(userID, timeSec, friend_platform);

                    // 보상으로 스태미너를 나도 받는다.
                    SetPostBadge();
                }
                else
                {
                    friendWindow.OnErrorMessage(result, null);
                }
            }
        });
    }

    public void SendRequestServerChecking(string cookie)
    {
        var data = new
        {
            version = Version.NetVersion,
            cookie = cookie,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedureWithoutAuth("shutdown_notice", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(cookie);

                hive5Process.ErrorProcess(response, "SendRequestServerChecking", objects.ToArray());
				
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);
            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            PacketServerChecking pd = new PacketServerChecking()
            {
                errorCode = result,
                Content = jsonData["notice"] != null ? (string)jsonData["notice"]["message"] : "",
            };
			
			LoginInfo loginInfo = Game.Instance.loginInfo;
			
			if (jsonData.Keys.Contains("eula_info") == true)
			{
				int eula_new_version = jsonData["eula_info"]["version"].ToInt();
				
				if (eula_new_version > loginInfo.eula_version)
				{
					loginInfo.eula_version = eula_new_version;
					loginInfo.eula_Checked = false;
					loginInfo.eula_url = jsonData["eula_info"]["eula_url"].ToString();
					loginInfo.private_url = jsonData["eula_info"]["private_url"].ToString();
				}
			}
			
            // 점검없음.
            if (pd.errorCode == NetErrorCode.OK)
            {
                EmptyLoadingPage loadingPage = Game.Instance.loadingPage;
                if (loadingPage != null)
                    loadingPage.LoadBundleVersion();

                //connector.SendPreLogin();
            }
            // 있음 공지띄우고 닫기누르면 종료한다.
            else if (pd.errorCode == NetErrorCode.NeedUpdateApp)
            {
                UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
                Transform rootTrans = null;

                if (uiRoot != null)
                {
                    if (uiRoot.popUpNode != null)
                        rootTrans = uiRoot.popUpNode;
                    else
                        rootTrans = uiRoot.transform;
                }
                NeedUpdatePopup popup = ResourceManager.CreatePrefabByResource<NeedUpdatePopup>("SystemPopup/NeedUpdatePopup", rootTrans, Vector3.zero);
                if (popup != null)
                    popup.UpdateURL = jsonData["update_url"].ToString();
                
            }
            else
            {
                UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
                Transform rootTrans = null;

                if (uiRoot != null)
                {
                    if (uiRoot.popUpNode != null)
                        rootTrans = uiRoot.popUpNode;
                    else
                        rootTrans = uiRoot.transform;
                }

                ServerCheckPopup serverCheckPopup = ResourceManager.CreatePrefabByResource<ServerCheckPopup>("SystemPopup/ServerNoticePopupWindow", rootTrans, Vector3.zero);

                if (serverCheckPopup != null)
                {
                    NoticeItem notice = null;
                    if (string.IsNullOrEmpty(pd.ImageUrl) == false)
                    {
                        notice = new NoticeItem();
                        notice.imgURL = pd.ImageUrl;
                        notice.type = NoticeItem.eNoticeType.ImageURL;
                    }
                    else
                    {
                        notice = new NoticeItem();
                        notice.message = pd.Content;
                        notice.type = NoticeItem.eNoticeType.Message;
                    }

                    serverCheckPopup.SetNotice(notice);
                }
            }
        });
    }

    public void SendRequestTargetEquipItem(long targetUserIndexID, int targetCharIndex, string platform = "kakao")
    {
        var data = new
        {
            platform_name = platform,
            platform_user_id = targetUserIndexID.ToString(),
            hero_type = targetCharIndex,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("request_target_info", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
			TownUI.detailRequestCount = 0;
			
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(targetUserIndexID);
                objects.Add(targetCharIndex);
                objects.Add(platform);

                hive5Process.ErrorProcess(response, "SendRequestTargetEquipItem", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                PacketTargetEquipItem pd = new PacketTargetEquipItem()
                {
                    TargetUserIndexID = long.Parse((string)jsonData["user_id"]),
                    TargetUserPlatform = (string)jsonData["platform"],
                    TargetCharacterIndex = jsonData["hero_type"].ToInt(),
                    errorCode = result,
                    IsFriend = (bool)jsonData["is_friend"] == true ? 1 : 0,
                    UserIndexID = connector.UserIndexID,
					Account = jsonData["nick"] != null ? jsonData["nick"].ToString() : ""
                };

                List<TargetInfoAll> infos = new List<TargetInfoAll>();
                for (int i = 0; i < jsonData["infos"].Count; i++)
                {
                    var infoJson = jsonData["infos"][i];

                    TargetInfoAll info = new TargetInfoAll()
                    {
                        Exp = infoJson["exp"].ToLong(),
                        expStr = infoJson["exp"].ToLong().ToString(),
                    };


                    info.equips = GetEquipItemsFromJson(infoJson["equips"]);
                    info.skills = GetSkillDBInfoFromJson(infoJson["skills"]);
                    info.awakenSkills = GetSkillDBInfoFromJson(infoJson["awaken_skills"]);
                    info.costumeSetItem = GetCostumeSetFromJson(infoJson["costumeset_item"]);

                    infos.Add(info);
                }

                pd.Infos = infos.ToArray();

                TownUI.TargetDetailWindow(pd);
            }
        });
    }

    private SkillDBInfo GetSkillDBInfoFromJson(JsonData jsonData)
    {
        SkillDBInfo info = new SkillDBInfo();

        info.IDs = jsonData["ids"].ToArray<int>();
        info.Lvs = jsonData["lvs"].ToArray<int>();

        return info;
    }

    private CostumeItemDBInfo GetCostumeSetFromJson(JsonData costumeSetJsonData)
    {
        if (costumeSetJsonData == null)
            return null;

        CostumeItemDBInfo info = null;
        var itemType = costumeSetJsonData["type"].ToInt();
        if (itemType != Item_Type.costumeset)
            return null;

        info = new CostumeItemDBInfo()
        {
            ID = costumeSetJsonData["table_id"].ToInt(),
            UID = costumeSetJsonData["id"].ToLong().ToString(),
        };

        return info;
    }

    private EquipItemDBInfo[] GetEquipItemsFromJson(JsonData equipsJson)
    {
        // equips
        List<EquipItemDBInfo> equips = new List<EquipItemDBInfo>();
        for (int j = 0; j < equipsJson.Count; j++)
        {
            var equipJson = equipsJson[j];
            var equipInfo = GetEquipItemFromJson(equipJson);

            if (equipInfo == null)
                continue;

            equips.Add(equipInfo);
        }
        return equips.ToArray();
    }

    private EquipItemDBInfo GetEquipItemFromJson(JsonData equipJson)
    {
        EquipItemDBInfo info = null;
        var itemType = equipJson["type"].ToInt();

        switch (itemType)
        {
            case Item_Type.material:
                break;
            case Item_Type.normal:

                    info = new EquipItemDBInfo()
                    {
                        Count = equipJson["count"].ToInt(),
                        Exp = equipJson["exp"].ToInt(),
                        Grade = equipJson["grade"].ToInt(),
                        ID = equipJson["table_id"].ToInt(),
                        Rate = equipJson["rate"].ToInt(),
                        Reinforce = equipJson["reinforce"].ToInt(),
                        SlotIndex = equipJson["slot_index"].ToInt(),
                        UID = (string)equipJson["id"],
                    };
                break;
            case Item_Type.costume:
                info = new EquipItemDBInfo()
                {
                    ID = equipJson["table_id"].ToInt(),
                    SlotIndex = equipJson["slot_index"].ToInt(),
                    UID = equipJson["id"].ToLong().ToString(),
					Rate = -1,
                };
                break;
            case Item_Type.costumeset:
                
                break;

            default:
                break;
        }

        return info;
    }

    public void SendRequestWaveContinue()
    {
        throw new NotImplementedException();
    }

    public void SendRequestWaveInfo()
    {
        var data = new
        {
            hero_type = Connector.charIndex
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("enter_wave", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "SendRequestWaveInfo");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            TownUI townUI = GameUI.Instance.townUI;

            if (result == NetErrorCode.OK)
            {
                int wave_reward_left_sec = jsonData["wave_reward_left_sec"].ToInt();
                int wave_step = jsonData["wave_step"].ToInt();
                int wave_sec = jsonData["wave_sec"].ToInt();
                int wave_best_step = jsonData["wave_best_step"].ToInt();
                int wave_best_sec = jsonData["wave_best_sec"].ToInt();
                int hero_ranking = jsonData["hero_ranking"].ToInt();
                bool is_wave_open = (bool)jsonData["is_wave_open"];
                bool clear = (bool)jsonData["clear"];
                int server_time = jsonData["server_time"].ToInt();

                CharInfoData charData = Game.Instance.charInfoData;
                int charIndex = -1;
                if (Game.Instance.connector != null)
                    charIndex = Game.Instance.connector.charIndex;

                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                WaveRankingInfo waveInfo = null;
                if (privateData != null)
                {
                    privateData.SetWaveRankInfo(hero_ranking, wave_step, wave_sec);
                    waveInfo = privateData.waveInfo;
                }

                if (waveInfo != null && clear == true)
                {
                    TableManager tableManager = TableManager.Instance;
                    StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
                    int maxWaveCount = 64;
                    if (stringValueTable != null)
                        maxWaveCount = stringValueTable.GetData("MaxWaveStep");

                    waveInfo.RecordStep = maxWaveCount;
                }

                List<WaveRankingInfo> rankingList = new List<WaveRankingInfo>();

                var rankings = jsonData["rankings"];
                for (int i = 0; i < rankings.Count; ++i)
                {
                    rankingList.Add(new WaveRankingInfo()
                    {
                        UserIndexID = rankings[i]["user_id"].ToLong(),
                        Platform = rankings[i]["platform"].ToString(),
                        PlatformUserId = rankings[i]["user_id"].ToLong(),
                        CharacterIndex = rankings[i]["hero_type"].ToInt(),
                        ranking = rankings[i]["ranking"].ToInt(),
                        RecordStep = rankings[i]["step"].ToInt(),
                        RecordSec = rankings[i]["sec"].ToInt(),
                        NickName = rankings[i]["nickname"] == null ? "" : rankings[i]["nickname"].ToString(),
                    });
                }

                if (is_wave_open == false)
                    wave_reward_left_sec = -1;

                townUI.OnWaveWindow(waveInfo, rankingList.ToArray(), wave_reward_left_sec, clear == true ? 1 : 0, is_wave_open == true ? 1 : 0);
            }
            else
            {
                townUI.requestCount = 0;
            }

        });
    }

    public void SendRequestWaveRanking(int ranking, bool bDown)
    {
        var parameters = new
        {
            ranking = ranking,
            is_downward = bDown,
        };

        GameUI.Instance.DoWait();
        hive5.CallProcedure("get_defense_ranking", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(ranking);
                objects.Add(bDown);
                hive5Process.ErrorProcess(response, "SendRequestWaveRanking");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            List<WaveRankingInfo> rankings = new List<WaveRankingInfo>();
            for (int i = 0; i < jsonData["rankings"].Count; i++)
            {
                var item = jsonData["rankings"][i];
                rankings.Add(new WaveRankingInfo()
                    {
                        CharacterIndex = item["hero_type"].ToInt(),
                        NickName = (string)item["nickname"],
                        ranking = item["ranking"].ToInt(),
                        RecordSec = item["sec"].ToInt(),
                        RecordStep = item["step"].ToInt(),
                        UserIndexID = item["user_id"].ToLong(),
                        PlatformUserId = item["user_id"].ToLong(),
                        Platform = item["platform"].ToString(),
                    });
            }

            // code here
            WaveWindow waveWindow = GameUI.Instance.waveWindow;

            if (waveWindow != null)
            {
                waveWindow.RefreshRankList(ErrorCodeConverter.Convert((Hive5ResultCode)jsonData["result"].ToInt()),
                    rankings.ToArray(),
                    (bool)jsonData["is_downward"]);
            }
        });
    }

    public void SendResponeBuyCashItem(TStoreCashItemInfo info)
    {
        GameUI.Instance.DoWait();
#if UNITY_ANDROID && !UNITY_EDITOR
        hive5.CompleteGooglePurchase(connector.UniversalPurchaseId, (long)info.Price, (long)info.Price, "KRW", info.OriginalJson, info.Siginature, (completePurchaseResponse) =>
        {
            GameUI.Instance.CancelWait();

            if (completePurchaseResponse.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(completePurchaseResponse);
                return;
            }

            var body2 = completePurchaseResponse.ResultData as CompleteGooglePurchaseResponseBody;
            //var json = body2.CallReturn;
			//SendBuyCashItem(info.ItemID);
			
			TableManager tableManager = TableManager.Instance;
	        CashShopInfoTable cashInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
	        CashItemInfo cashItemInfo = null;
	        if (cashInfoTable != null)
	            cashItemInfo = cashInfoTable.GetItemInfo(info.ItemID);
			
			BuyCashItemHandler(cashItemInfo, body2.CallReturn);
        });
#else
        SendBuyCashItem(info.ItemID);
#endif
    }

    public void SendResponeBuyCashItemFailed(TStoreCashItemInfo info)
    {
        // 구매실패. 
        // todo. 구매창을 닫는다.
        // 실패를 서버에 알린다. 응답은 없다.
        var parameters = new
        {
            hero_type = connector.charIndex,
            user_id = connector.PlatformUserId,
            error_message = info.errorString,
            store = connector.publisher,
            item_id = info.ItemID,
            product_code = info.TStoreProductCode,
            store_tid = info.TStoreTID,
            pid = connector.UniversalPurchaseId,
        };

        GameUI.Instance.DoWait();
        hive5.CallProcedure("buy_cash_item_fail", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response);
                return;
            }

            //var body = response.ResultData as CallProcedureResponseBody;
            //string jsonString = body.CallReturn;
            //var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            //// code here
        });
    }

    public void SendSelectGambleItem(int CharacterIndex, int gambleType, int again)
    {
        var parameters = new TupleList<string, string>();
        parameters.Add("gambleType", gambleType.ToString());
        parameters.Add("tryAgain", again.ToString());
        parameters.Add("hero_type", CharacterIndex.ToString());

        GameUI.Instance.DoWait();

        hive5.CallProcedure("start_gamble", parameters, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(CharacterIndex);
                objects.Add(gambleType);
                objects.Add(again);

                hive5Process.ErrorProcess(response, "SendSelectGambleItem", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode errorCode = (NetErrorCode)(int)jsonData["result"];

            int gold = (int)jsonData["gold"];
            int jewel = (int)jsonData["jewel"];
            int coupon = (int)jsonData["coupon"];
            int selectedIndex = (int)jsonData["selectedIndex"];

            ItemDBInfo itemDBInfo = ItemToItemDBInfo(jsonData["item"]);

            // code here
            Item newItem = null;

            GambleWindow gambleWindow = GameUI.Instance.gambleWindow;

            if (errorCode == NetErrorCode.OK)
            {
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUseGamble, 0);

                newItem = Item.CreateItem(itemDBInfo);
                if (newItem != null)
                    newItem.IsNewItem = true;

                if (newItem != null && newItem.itemInfo != null)
                {
                    switch (newItem.itemInfo.itemType)
                    {
                        case ItemInfo.eItemType.Costume_Back:
                        case ItemInfo.eItemType.Costume_Body:
                        case ItemInfo.eItemType.Costume_Head:
                            connector.charInfo.AddCostume(newItem);
                            break;
                        case ItemInfo.eItemType.Material:
                            connector.charInfo.AddMaterial(newItem);
                            break;
                        default:
                            connector.charInfo.AddItem(newItem);
                            break;
                    }
                }

                UpdateItemInfos(null, true);

                connector.charInfo.SetGold(gold, jewel);
                connector.charInfo.gambleCoupon = coupon;

                if (gambleWindow != null)
                {
                    gambleWindow.UpdateCoinInfo(true);
                    gambleWindow.UpdateCouponCount();
                }

                Game.Instance.SendUpdateAchievmentInfo();
            }

            if (gambleWindow != null)
                gambleWindow.StartGambleProgress(errorCode, newItem, selectedIndex);

            
        });

    }

    public void SendSpecialAchievementProcess(List<Achievement> achievementList)
    {
        throw new NotImplementedException();
    }

    public void SendStageEnd(int CharacterIndex, int curLevel, int gainGold, int gainCash, GainItemInfo[] gainItems, GainItemInfo[] gainMaterialItems, int usedPotion1, int usedPotion2)
    {
        var data = new
        {
            hero_type = CharacterIndex,
            current_level = curLevel,
            stage_index = connector.StageIndex,
            stage_type = connector.StageType,
            gain_gold = gainGold,
            gain_jewel = gainCash,
            gain_normal_items = gainItems,
            gain_material_items = gainMaterialItems,
            used_potion1 = usedPotion1,
            used_potion2 = usedPotion2
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("end_stage", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(CharacterIndex);
                objects.Add(curLevel);
                objects.Add(gainGold);
                objects.Add(gainCash);
                objects.Add(gainItems);
                objects.Add(gainMaterialItems);
                objects.Add(usedPotion1);
                objects.Add(usedPotion2);

                hive5Process.ErrorProcess(response, "SendStageEnd", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            PacketStageResult pd = new PacketStageResult();

            pd.AddAPoint = jsonData["add_awaken_point"].ToInt();
            pd.AddBuyAblePoint = jsonData["add_buyable_point"].ToInt();
            pd.AddPoint = jsonData["add_point"].ToInt();
            pd.bALevelup = (bool)jsonData["is_awaken_levelup"];
            pd.bLevelup = (bool)jsonData["is_levelup"];
            pd.Cash = jsonData["jewel"].ToInt();
            pd.CharacterIndex = jsonData["hero_type"].ToInt();
            pd.ClearStageIndex = jsonData["stage_index"].ToInt();
            pd.errorCode = result;
            pd.Gold = jsonData["gold"].ToInt();
            pd.LevelupStamina = jsonData["levelup_stamina"].ToInt();
            pd.potion1 = jsonData["potion1"].ToInt();
            pd.potion1Present = jsonData["potion1_gift"].ToInt();
            pd.potion2 = jsonData["potion2"].ToInt();
            pd.potion2Present = jsonData["potion2_gift"].ToInt();
            pd.StageType = jsonData["stage_type"].ToInt();
            pd.totalAEXP = jsonData["awaken_exp"].ToLong();
            pd.totalAEXPStr = jsonData["awaken_exp"].ToLong().ToString();
            pd.totalEXP = jsonData["exp"].ToLong();
            pd.totalEXPStr = jsonData["exp"].ToLong().ToString();
            pd.curALevel = jsonData["awaken_level"].ToInt();
            pd.curLevel = jsonData["level"].ToInt();
            pd.rewardEXP = jsonData["reward_exp"].ToInt();
            pd.rewardGold = jsonData["reward_gold"].ToInt();
            pd.rewardIndex = jsonData["reward_index"].ToInt();
            pd.rewardItemGrades = jsonData["reward_item_grades"].ToArray<int>();
            pd.rewardItemIDs = jsonData["reward_item_ids"].ToArray<int>();
            pd.rewardMaterialItemID = jsonData["reward_material_id"].ToInt();
            pd.rewardItemRates = jsonData["reward_item_rates"].ToArray<int>();
            pd.rewardItemInfo = GetItemDBInfoFromGatcha(jsonData["reward_item_info"]);
            pd.rewardMeat = jsonData["reward_meat"].ToInt();
			pd.rewardPrices = jsonData["retry_prices"].ToArray<int>();
            pd.costumeSetItem = null; // GetCostumeSetFromJson(jsonData["contumeset_item"]),
            //pd.gainMaterialItems = new List<MaterialItemDBInfo>().ToArray(); // GetMaterialItemsFromJson(jsonData["gain_material_items"]),
            pd.gainNormalItems = new List<ItemDBInfo>().ToArray(); // GetNormalItemsFromJson(jsonData["gain_normal_items"]),
            
			var gain_materials = jsonData["gain_material_items"];

            Game.Instance.appearBossID = jsonData["boss_id"].ToInt();
            if (pd.errorCode == NetErrorCode.OK)
            {
                pd.totalEXP = long.Parse(pd.totalEXPStr);
                pd.totalAEXP = long.Parse(pd.totalAEXPStr);

                if (pd.totalEXP < 0L)
                    pd.totalEXP = 0L;
                if (pd.totalAEXP < 0L)
                    pd.totalAEXP = 0L;

                int charIndex = pd.CharacterIndex;

                CharInfoData charData = connector != null ? connector.charInfo : null;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                if (charData != null)
                {
                    charData.SetGold(pd.Gold, pd.Cash);
					
					charData.potion1 = pd.potion1;
					charData.potion1Present = pd.potion1Present;
					charData.potion2 = pd.potion2;
					charData.potion2Present = pd.potion2Present;
					
                    ItemDBInfo dbInfo = null;

                    int nCount = pd.gainNormalItems.Length;
                    for (int index = 0; index < nCount; ++index)
                    {
                        dbInfo = pd.gainNormalItems[index];
                        AddNewItem(charData, dbInfo, true);
                    }
					
					/*
                    nCount = pd.gainMaterialItems.Length;
                    for (int index = 0; index < nCount; ++index)
                    {
                        materialItemDBInfo = pd.gainMaterialItems[index];
                        AddNewItem(charData, materialItemDBInfo, true);
                    }
					*/
					
					MaterialItemDBInfo materialItemDBInfo = null;
                    for (int i = 0; i < gain_materials.Count; ++i)
					{
						var material = gain_materials[i];
						materialItemDBInfo = new MaterialItemDBInfo();
						materialItemDBInfo.UID = material["UID"].ToString();
						materialItemDBInfo.ID = material["ID"].ToInt();
						materialItemDBInfo.Count = material["Count"].ToInt();
						
						AddNewItem(charData, materialItemDBInfo, true);
					}

                    dbInfo = pd.rewardItemInfo;
                    if (dbInfo != null && !string.IsNullOrEmpty(dbInfo.UID))
                        AddNewItem(charData, dbInfo, true);
                }

                if (privateData != null)
                {
                    privateData.baseInfo.ExpValue = pd.totalEXP;
                    privateData.baseInfo.AExp = pd.totalAEXP;

                    privateData.baseInfo.SkillPoint += pd.AddPoint;
                    privateData.baseInfo.APoint += pd.AddAPoint;
                    privateData.baseInfo.ALimitBuyCount += pd.AddBuyAblePoint;

                    privateData.baseInfo.StaminaMax += pd.LevelupStamina;
                    if (pd.LevelupStamina > 0)
                        privateData.baseInfo.StaminaCur = Math.Max(privateData.baseInfo.StaminaCur, privateData.baseInfo.StaminaMax);

                    if (pd.updateItems != null)
                    {
                        int nCount = pd.updateItems.Length;
                        privateData.SetEquipItemList(nCount, pd.updateItems);//, pd.costumeSetItem);
                    }

                    int levelup_event_step = jsonData["levelup_event_step"].ToInt();

                    if (levelup_event_step >= 0)
                        privateData.levelupRewardEventCheck = levelup_event_step+1;

                }

                StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;
                if (stageEndEvent != null)
                {
                    List<Item> items = new List<Item>();
                    for (int i = 0; i < pd.rewardItemIDs.Length; i++)
                    {
                        items.Add(Item.CreateItem(pd.rewardItemIDs[i], "", pd.rewardItemGrades[i],
                                                0, 1, pd.rewardItemRates[i], 0));
                    }

                    stageEndEvent.OnStageEndResult(pd.errorCode,
                                                    pd.CharacterIndex, pd.ClearStageIndex, pd.StageType,
                                                    pd.rewardItemInfo, (long)pd.rewardEXP, pd.totalEXP,
                                                    pd.rewardIndex, items, pd.rewardMeat, pd.rewardGold,
                                                    pd.rewardMaterialItemID, pd.rewardPrices);
                }
				
				if (Game.Instance.AndroidManager != null)
				{
					if (pd.bLevelup == true)
					{
						TableManager tableManager = TableManager.Instance;
						CharExpTable expTable = tableManager != null ? tableManager.charExpTable : null;
						int charLevel = curLevel + 1;
						if (expTable != null)
							charLevel = expTable.GetLevel(pd.totalEXP);
						Game.Instance.AndroidManager.SendPartyTrackEvent(string.Format("Lv{0}", charLevel));
					}
				}
            }
        });
    }

    private ItemDBInfo GetItemDBInfoFromGatcha(JsonData jsonData)
    {
        if (jsonData == null)
            return null;

        if (jsonData.Keys.Contains<string>("uid") == false)
            return null;

        var info = new ItemDBInfo();
        info.Count = 1;
        info.Exp = 0;
        info.Grade = jsonData["grade"].ToInt();
        info.Rate = jsonData["rate"].ToInt();
        info.ID = jsonData["id"].ToInt();
        info.UID = jsonData["uid"].ToString();
        info.Reinforce = 0;

        return info;
    }


    void AddNewItem(CharInfoData charInfo, ItemDBInfo dbInfo)
    {
        AddNewItem(charInfo, dbInfo, false);
    }

    void AddNewItem(CharInfoData charInfo, ItemDBInfo dbInfo, bool isNewItem)
    {
        if (charInfo == null || dbInfo == null)
            return;

        Item newItem = Item.CreateItem(dbInfo);
        if (newItem != null)
            newItem.IsNewItem = isNewItem;

        AddNewItem(charInfo, newItem);
    }

    void AddNewItem(CharInfoData charInfo, MaterialItemDBInfo dbInfo, bool isNewItem)
    {
        if (charInfo == null || dbInfo == null)
            return;

        Item newItem = Item.CreateItem(dbInfo);
        if (newItem != null)
            newItem.IsNewItem = isNewItem;

        AddNewItem(charInfo, newItem);
    }

    void AddNewItem(CharInfoData charInfo, CostumeItemDBInfo dbInfo, bool isNewItem)
    {
        if (charInfo == null || dbInfo == null)
            return;

        Item newItem = Item.CreateItem(dbInfo.ID, dbInfo.UID, 0, 0, 1);
        if (newItem != null)
            newItem.IsNewItem = isNewItem;

        AddNewItem(charInfo, newItem);
    }


    public void SendStageEndFailed(int CharacterIndex, int usedPotion1, int usedPotion2)
    {
        var data = new
        {
            hero_type = CharacterIndex,
            used_potion1 = usedPotion1,
            used_potion2 = usedPotion2,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("end_stage_fail", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(CharacterIndex);
                objects.Add(usedPotion1);
                objects.Add(usedPotion2);

                hive5Process.ErrorProcess(response, "SendStageEndFailed", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            PacketStageEndFailed pd = new PacketStageEndFailed()
            {
                CharacterIndex = jsonData["hero_type"].ToInt(),
                errorCode = result,
                potion1 = jsonData["potion1"].ToInt(),
                potion1Present = jsonData["potion1_gift"].ToInt(),
                potion2 = jsonData["potion2"].ToInt(),
                potion2Present = jsonData["potion2_gift"].ToInt(),
                usedPotion1 = jsonData["used_potion1"].ToInt(),
                usedPotion2 = jsonData["used_potion2"].ToInt(),
                UserIndexID = connector.UserIndexID,
            };

            if (pd.errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = connector != null ? connector.charInfo : null;

                if (charData != null)
                {
                    charData.potion1 = pd.potion1;
                    charData.potion1Present = pd.potion1Present;

                    charData.potion2 = pd.potion2;
                    charData.potion2Present = pd.potion2Present;
                }

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eStageFailed, 0);
            }
        });
    }

    public void SendStageReward(int CharacterIndex, int stageType, int stageIndex, int price)
    {
        var data = new
        {
            hero_type = CharacterIndex,
            stage_type = stageType,
            stage_index = stageIndex,
			reward_price = price,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("reward_stage", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(CharacterIndex);
                objects.Add(stageType);
                objects.Add(stageIndex);
                objects.Add(price);

                hive5Process.ErrorProcess(response, "SendStageReward", objects.ToArray()); 
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            PacketStageReward pd = new PacketStageReward()
            {
                CharacterIndex = jsonData["hero_type"].ToInt(),
                errorCode = result,
                potion2Present = jsonData["potion2_gift"].ToInt(),
                rewardIndex = jsonData["reward_index"].ToInt(),
                rewardItemInfo = GetItemDBInfoFromGatcha(jsonData["reward_item_info"]),
                stageIndex = jsonData["stage_index"].ToInt(),
                stageType = jsonData["stage_type"].ToInt(),
                totalCash = jsonData["jewel"].ToInt(),
                totalGold = jsonData["gold"].ToInt(),
            };

            if (pd.errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = connector != null ? connector.charInfo : null;

                if (charData != null)
                {
                    charData.SetGold(pd.totalGold, pd.totalCash);

                    ItemDBInfo dbInfo = pd.rewardItemInfo;
                    if (dbInfo != null && !string.IsNullOrEmpty(dbInfo.UID))
                        AddNewItem(charData, dbInfo, true);
                }
            }

            StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;
            if (stageEndEvent != null)
            {
                stageEndEvent.RewardAgain(pd.rewardIndex, pd.totalCash);
            }
        });
    }

    public void SendStageStart(int CharacterIndex, int stageType, int stageIndex, int[] selectedBuffs, int curStamina, int buyPotion1, int buyPotion2)
    {
        bool recovery = false;

        if (curStamina < 0)
            recovery = true;

        var data = new
        {
            hero_type = CharacterIndex,
            stage_type = stageType,
            stage_index = stageIndex,
            buffs = selectedBuffs,
            current_stamina = curStamina,
            buy_potion1 = buyPotion1,
            buy_potion2 = buyPotion2,
            recovery = recovery,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("start_stage", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(CharacterIndex);
                objects.Add(stageType);
                objects.Add(stageIndex);
                objects.Add(selectedBuffs);
                objects.Add(curStamina);
                objects.Add(buyPotion1);
                objects.Add(buyPotion2);

                hive5Process.ErrorProcess(response, "SendStageStart", objects.ToArray());			
				
                return;
            }

            connector.StageIndex = stageIndex;
            connector.StageType = stageType;

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
			
			if (result != NetErrorCode.OK)
			{
				if (GameUI.Instance.MessageBox != null)
					GameUI.Instance.MessageBox.SetMessage(result);
				
				if (GameUI.Instance.mapStartWindow != null)
					GameUI.Instance.mapStartWindow.requestCount = 0;
				if (GameUI.Instance.specialMapStartWindow != null)
					GameUI.Instance.specialMapStartWindow.requestCount = 0;
				
				return;
			}
			
            PacketStageStart pd = new PacketStageStart()
            {
                buyPotion1 = jsonData["buy_potion1"].ToInt(),
                buyPotion2 = jsonData["buy_potion2"].ToInt(),
                Cash = jsonData["jewel"].ToInt(),
                CharacterIndex = jsonData["hero_type"].ToInt(),
                curStamina = jsonData["stamina"]["value"].ToInt(),
                errorCode = result,
                Gold = jsonData["gold"].ToInt(),
                LeftTimeSec = jsonData["stamina"]["next_time"].ToInt() - jsonData["server_time"].ToInt(),
                potion1 = jsonData["potion1"].ToInt(),
                potion2 = jsonData["potion2"].ToInt(),
                presentStamina = jsonData["stamina_gift"].ToInt(),
                SelectedBuffs = jsonData["buffs"].ToArray<int>(),
                StageIndex = jsonData["stage_index"].ToInt(),
                StageType = jsonData["stage_type"].ToInt(),
                UserIndexID = connector.UserIndexID,
            };

            MapStartWindow mapStartWindow = null;
            if (pd.StageIndex > 200)
                mapStartWindow = GameUI.Instance.specialMapStartWindow;
            else
                mapStartWindow = GameUI.Instance.mapStartWindow;

            if (pd.errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = connector != null ? connector.charInfo : null;
                CharPrivateData privateData = null;

                if (charData != null)
                {
                    charData.SetGold(pd.Gold, pd.Cash);
                    charData.potion1 = pd.potion1;
                    charData.potion2 = pd.potion2;

                    privateData = charData.GetPrivateData(pd.CharacterIndex);
                }

                if (privateData != null)
                    privateData.SetStamina(pd.LeftTimeSec, pd.curStamina, pd.presentStamina);

                if (mapStartWindow != null)
                {
                    mapStartWindow.OnStart(pd.StageIndex, pd.StageType, pd.SelectedBuffs);

                    Game.Instance.ApplyAchievement(Achievement.eAchievementType.eStageEnter, 0);
                }
            }
            else
            {
                if (mapStartWindow != null)
                    mapStartWindow.OnErrorMessage(pd.errorCode, mapStartWindow);
            }

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }

    public void SendTownTutorialEnd(int charIndex)
    {
        var data = new
        {
            hero_type = charIndex,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("end_town_tutorial", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(charIndex);

                hive5Process.ErrorProcess(response, "SendTownTutorialEnd", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            PacketTutorialDone pd = new PacketTutorialDone()
            {
                CharacterIndex = jsonData["hero_type"].ToInt(),
                errorCode = result,
            };

            if (pd.errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = Game.Instance.charInfoData;
                CharPrivateData privateData = null;
                if (charData != null)
                    privateData = charData.GetPrivateData(pd.CharacterIndex);

                if (charData != null)
                    charData.isTutorialComplete = true;
                if (privateData != null)
                    privateData.baseInfo.tutorial = 1;
            }
        });
    }

    public void SendTutorialEnd(int charIndex)
    {
        var data = new
        {
            hero_type = charIndex,
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("end_tutorial", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(charIndex);

                hive5Process.ErrorProcess(response, "SendTutorialEnd", objects.ToArray());
                return; 
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            // code here
            NetErrorCode errorCode = (NetErrorCode)jsonData["result"].ToInt();
            ItemDBInfo rewardItem = null;
            if (errorCode == NetErrorCode.OK)
            {
                //int hero_type = jsonData["hero_type"].ToInt();

                CharInfoData charData = connector != null ? connector.charInfo : null;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                rewardItem = GetItemDBInfoFromGatcha(jsonData["picked_item1"]);

                if (charData != null)
                {                    
                    AddNewItem(charData, rewardItem, true);
                    AddNewItem(charData, GetItemDBInfoFromGatcha(jsonData["picked_item2"]), true);
                    AddNewItem(charData, GetItemDBInfoFromGatcha(jsonData["material_item"]), true);
                }
            }

            StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;

            if (stageEndEvent != null)
            	stageEndEvent.OnStageEndResult(result, charIndex, -1, -1, rewardItem, 0, -1, 0, null, 0, 0, 0, null);
        });
    }

    public void SendUpdateStamina(int curStamina)
    {
        var parameters = new Hive5.Util.TupleList<string, string>();
        parameters.Add("current_stamina", curStamina.ToString());

        hive5.CallProcedure("update_stamina", parameters, (response) =>
        {
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                //hive5Process.ErrorProcess(response);
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;

            var jsonData = LitJson.JsonMapper.ToObject(body.CallReturn);
            CharInfoData charData = Game.Instance.charInfoData;
            if (charData == null)
                return;

            CharPrivateData privateData = charData.GetPrivateData((int)jsonData["hero_type"]);
            if (privateData == null)
                return;

            privateData.baseInfo.StaminaCur = (int)jsonData["stamina"]["value"];

        });
    }

    public void SendWaveEnd(int CharacterIndex, int WaveStep, int DurationSec, int isClear, int usedPotion1, int usedPotion2)
    {

        var data = new
        {
            hero_type = CharacterIndex,
            wave_step = WaveStep,
            wave_sec = DurationSec,
            clear = isClear,
            used_potion1 = usedPotion1,
            used_potion2 = usedPotion2
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("end_wave", data, (response) =>
        {
            GameUI.Instance.CancelWait();
			
            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(CharacterIndex);
                objects.Add(WaveStep);
                objects.Add(DurationSec);
                objects.Add(isClear);
                objects.Add(usedPotion1);
                objects.Add(usedPotion2);

                hive5Process.ErrorProcess(response, "SendWaveEnd", objects.ToArray());
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            Item newItem = null;
            int wave_step = -1;
            int wave_sec = -1;
            int bestStep = -1;
            int bestSec = -1;

            if (result == NetErrorCode.OK)
            {
                wave_step = jsonData["wave_step"].ToInt();
                wave_sec = jsonData["wave_sec"].ToInt();

                if (jsonData["reward_item"] != null)
                {
                    ItemDBInfo itemdb = ItemToItemDBInfo(jsonData["reward_item"]);
                    int type = jsonData["reward_item"]["type"].ToInt();
                    switch (type)
                    {
                        case Item_Type.costume:
                        case Item_Type.normal:
                        case Item_Type.material:
                            {
                                newItem = Item.CreateItem(itemdb);

                                if (newItem != null)
                                    newItem.IsNewItem = true;
                                switch (type)
                                {
                                    case Item_Type.costume:
                                        connector.charInfo.AddCostume(newItem);
                                        break;
                                    case Item_Type.normal:
                                        connector.charInfo.AddItem(newItem);
                                        break;
                                    case Item_Type.material:
                                        connector.charInfo.AddMaterial(newItem);
                                        break;
                                }
                            }
                            break;

                        case Item_Type.costumeset:
                            CostumeSetItem setItem = CostumeSetItem.Create(itemdb.ID, itemdb.UID);
                            connector.charInfo.AddCostumeSetItem(setItem);
                            break;
                    }
                }

                int charIndex = Connector.charIndex;

                CharInfoData charData = connector != null ? connector.charInfo : null;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                if (privateData != null)
                {
                    privateData.SetWaveClearInfo(wave_step, wave_sec);

                    bestStep = privateData.waveInfo.RecordStep;
                    bestSec = privateData.waveInfo.RecordSec;
                }

                charData.potion1 = jsonData["potion1"].ToInt();
                charData.potion1Present = jsonData["potion1_gift"].ToInt();
                charData.potion2 = jsonData["potion2"].ToInt();
                charData.potion2Present = jsonData["potion2_gift"].ToInt();
            }

            if (GameUI.Instance.waveManager != null)
                GameUI.Instance.waveManager.ShowWaveEndWindow(isClear == 1, newItem, wave_step, wave_sec, bestStep, bestSec);

        });
    }

    public void SendWaveStartOrContinue(int CharacterIndex, int[] SelectedBuffs, int SeletedTowner, int curStamina, bool recovery, int buyPotion1, int buyPotion2)
    {
        var data = new
        {
            hero_type = Connector.charIndex,
            current_stamina = curStamina,
            tower = SeletedTowner,
            buffs = SelectedBuffs,
            buy_potion1 = buyPotion1,
            buy_potion2 = buyPotion2,
            recovery = recovery
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("start_wave", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(CharacterIndex);
                objects.Add(SelectedBuffs);
                objects.Add(SeletedTowner);
                objects.Add(curStamina);
                objects.Add(recovery);
                objects.Add(buyPotion1);
                objects.Add(buyPotion2);

                hive5Process.ErrorProcess(response, "SendWaveStartOrContinue", objects.ToArray());
				
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();
            WaveStartWindow waveStartWindow = GameUI.Instance.waveStartWindow;

            if (result == NetErrorCode.OK)
            {
                int hero_type = jsonData["hero_type"].ToInt();
                int gold = jsonData["gold"].ToInt();
                int jewel = jsonData["jewel"].ToInt();
                int potion1 = jsonData["potion1"].ToInt();
                int potion2 = jsonData["potion2"].ToInt();
                int tower = jsonData["tower"].ToInt();
                int server_time = jsonData["server_time"].ToInt();

                int StaminaCur = (int)jsonData["stamina"]["value"];
                int StaminaMax = (int)jsonData["stamina"]["max"];
                int StaminaPresent = (int)jsonData["stamina_gift"];
                int StaminaLeftTimeSec = (int)jsonData["stamina"]["next_time"] - server_time;
                int[] buffs = JsonMapper.ToObject<int[]>(BigFoot.ConverJson.MakeToJson(jsonData["buffs"]));

                CharInfoData charData = connector.charInfo;
                CharPrivateData privateData = null;

                if (charData != null)
                {
                    charData.SetGold(gold, jewel);

                    charData.potion1 = potion1;
                    charData.potion2 = potion2;

                    privateData = charData.GetPrivateData(hero_type);
                }

                if (privateData != null)
                {
                    privateData.SetStamina(StaminaLeftTimeSec, StaminaCur, StaminaPresent);
                }

                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eDefenceEnter, 0);

                if (waveStartWindow != null)
                    waveStartWindow.OnStart(buffs, tower, true);
            }


            if (waveStartWindow != null)
                waveStartWindow.OnErrorMessage(result, waveStartWindow);

            Game.Instance.SendUpdateAchievmentInfo();
        });
    }

    ItemDBInfo ItemToItemDBInfo(JsonData item)
    {
        ItemDBInfo dbInfo = new ItemDBInfo();

        dbInfo.UID = item["id"].ToString();
        dbInfo.ID = (int)item["table_id"];


        switch (item["type"].ToInt())
        {
            case Item_Type.costume:
            case Item_Type.costumeset:
                {
                    dbInfo.Grade = 0;
                    dbInfo.Count = 1;
                    dbInfo.Reinforce = 0;
                    dbInfo.Rate = -1;
                    dbInfo.Exp = 0;
                }
                break;

            case Item_Type.material:
            case Item_Type.normal:
                {
                    dbInfo.Grade = (int)item["grade"];
                    dbInfo.Count = (int)item["count"];
                    dbInfo.Reinforce = (int)item["reinforce"];
                    dbInfo.Rate = (int)item["rate"];
                    dbInfo.Exp = (int)item["exp"];
                }
                break;

        }


        return dbInfo;
    }

    public void SendRequsetInviteKakaoFriendByUserID(string user_id)
    {
        var data = new
        {
            user_id = user_id
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("report_kakao_invite", data, (response) =>
        {
            GameUI.Instance.CancelWait();
            FriendWindow friendWindow = GameUI.Instance.friendWindow;
            if (friendWindow != null)
                friendWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                if (friendWindow != null)
                    friendWindow.OnCancelPopup(null);

                List<System.Object> objects = new List<System.Object>();
                objects.Add(user_id);

                hive5Process.ErrorProcess(response, "SendRequsetInviteKakaoFriendByUserID", objects.ToArray());
				

                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                var kakaoJsonData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["kakao"]));

                string[] invited_ids = null;
                //if (kakaoJsonData["invited_ids"] != null)
                invited_ids = kakaoJsonData["invited_ids"].ToArray<string>();
                Game.Instance.todayInvites = kakaoJsonData["today_invites"].ToInt();
                Game.Instance.totalInvites = kakaoJsonData["total_invites"].ToInt();

                Game.Instance.kakaoInvitedIDs.Clear();
                if (invited_ids != null)
                {
                    foreach (string u_id in invited_ids)
                        Game.Instance.kakaoInvitedIDs.Add(u_id);
                }

                if (friendWindow != null)
                    friendWindow.UpdateKakaoInviteInfo();

                var giftData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["gift"]));
                if (giftData != null)
                {
                    string kind = giftData["kind"].ToString();
                    int amount = giftData["amount"].ToInt();
                    int id = giftData["id"].ToInt();
                    string msg = giftData["message"].ToString();

                    string popupMessage = "";
                    TableManager tableManager = TableManager.Instance;
                    StringTable stringTable = tableManager != null ? tableManager.stringTable : null;

                    int formatStringID = 286;
                    int rewardNameID = -1;
                    string formatStr = "";
                    string itemName = "";

                    if (kind == "potion1")
                    {
                        rewardNameID = 240;
                    }
                    else if (kind == "gold")
                    {
                        rewardNameID = 17;
                    }
                    else if (kind == "material")
                    {
                        rewardNameID = 287;
                    }
                    else if (kind == "jewel")
                    {
                        rewardNameID = 16;
                    }
                    else if (kind == "gamble_cuopon")
                    {
                        rewardNameID = 218;
                    }

                    if (stringTable != null)
                    {
                        formatStr = stringTable.GetData(formatStringID);
                        itemName = stringTable.GetData(rewardNameID);
                    }

                    popupMessage = string.Format(formatStr, msg, itemName, amount);

                    if (string.IsNullOrEmpty(popupMessage) == false)
                    {
                        if (GameUI.Instance.MessageBox != null)
                            GameUI.Instance.messageBox.SetMessage(popupMessage);
                    }

                }
				
				if (Game.Instance.AndroidManager != null)
					Game.Instance.AndroidManager.SendPartyTrackEvent("Invitation");
            }
            else
            {
                if (friendWindow != null)
                    friendWindow.OnCancelPopup(null);

                if (GameUI.Instance.MessageBox != null)
                    GameUI.Instance.MessageBox.SetMessage(result);
            }
        });
    }

    public void SendProfileImageOnOff(bool bToggle)
    {
        var data = new
        {

        };

        GameUI.Instance.DoWait();

        string procedureName = string.Format("{0}_kakao_profile_image", bToggle == true ? "on" : "off");

        hive5.CallProcedure(procedureName, data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                List<System.Object> objects = new List<System.Object>();
                objects.Add(bToggle);

                hive5Process.ErrorProcess(response, "SendProfileImageOnOff", objects.ToArray()); 

                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                var kakaoJsonData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["kakao"]));

                GameOption.faceToggle = (bool)kakaoJsonData["show_profile_image"];
            }
        });
    }

    public void GetKakaoInfo()
    {
        var data = new
        {

        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("get_kakao_info", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "GetKakaoInfo");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                var kakaoData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["kakao"]));

                Game.Instance.todayInvites = kakaoData["today_invites"].ToInt();
                Game.Instance.totalInvites = kakaoData["total_invites"].ToInt();

                string[] invited_ids = null;
                if (kakaoData["invited_ids"] != null)
                    invited_ids = kakaoData["invited_ids"].ToArray<string>();

                Game.Instance.kakaoInvitedIDs.Clear();
                if (invited_ids != null)
                {
                    foreach (string user_id in invited_ids)
                        Game.Instance.kakaoInvitedIDs.Add(user_id);
                }

                FriendInviteWindow inviteWindow = GameUI.Instance.friendInviteWindow;
                if (inviteWindow != null)
                    inviteWindow.UpdateInviteInfo();

            }
        });
    }

	public void SendStaminaForAll()
    {
		var data = new
        {
            
        };

        GameUI.Instance.DoWait();

        hive5.CallProcedure("send_stamina_for_all_friends", data, (response) =>
        {
            GameUI.Instance.CancelWait();

            FriendWindow friendWindow = GameUI.Instance.friendWindow;

            if (friendWindow != null)
                friendWindow.requestCount = 0;

            if (response.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(response, "SendStaminaForAll");
                return;
            }

            var body = response.ResultData as CallProcedureResponseBody;
            string jsonString = body.CallReturn;
            var jsonData = LitJson.JsonMapper.ToObject(jsonString);

            NetErrorCode result = (NetErrorCode)jsonData["result"].ToInt();

            if (result == NetErrorCode.OK)
            {
                BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.FriendList);
				
				int timeSec = jsonData["cool_time"].ToInt();
				var infoJsonData = LitJson.JsonMapper.ToObject(BigFoot.ConverJson.MakeToJson(jsonData["friends"]));
                
                for (int i = 0; i < infoJsonData.Count; i++)
                {
                    var item = infoJsonData[i];

                    long userID = long.Parse(item["id"].ToString());
                    string platform = item["platform"].ToString();
					
                    if (friendListWindow != null)
                        friendListWindow.UpdateInfo(userID, timeSec, platform);
                }
				
				if (friendListWindow != null)
				{
					if (friendListWindow.CheckSednStamina() == false)
						SendStaminaForAll();
				}
				
                // 보상으로 스태미너를 나도 받는다.
                if (infoJsonData.Count > 0)
                    SetPostBadge();
            }
            else
            {
                friendWindow.OnErrorMessage(result, null);
            }
        });
    }
    
    public void CheckNickName()
    {
        Logger.DebugLog("get_nick_calll");
        //hive5.CallProcedure("get_nick", null, (getNickResponse) =>
        hive5.CallProcedure("has_nick", null, (getNickResponse) =>
        {
            Logger.DebugLog("get_nick_result : " + getNickResponse.ResultCode);

            if (getNickResponse.ResultCode != Hive5ResultCode.Success)
            {
                hive5Process.ErrorProcess(getNickResponse);
                return;
            }

            Logger.DebugLog("Token2:" + hive5.AccessToken);
            var body = getNickResponse.ResultData as CallProcedureResponseBody;
            var jsonData = LitJson.JsonMapper.ToObject(body.CallReturn);
            Debug.Log(body.CallReturn);

            string infoStr = string.Format("nickName 1 : {0}", BigFoot.ConverJson.MakeToJson(jsonData));
            Debug.Log(infoStr);

            var has_nick = (bool)jsonData["has_nick"];

			if (has_nick == false)
            {
                LoginPage loginPage = GameUI.Instance.loginPage;
                // show login page
                if (loginPage != null)
                    loginPage.OnCreateNickName();
            }
            else
            {
                Game.Instance.AddDelayCall("GetUserInfo");
            }
        });
    }
	
	
	public int getUserErrorStringID = 1105;
    public void GetUserInfo()
    {
        var parameters = new
        {
            name = "gilbokgilbok",
        };

        hive5.CallProcedure("get_user", parameters, (callProcedureResponse) =>
        {
            if (callProcedureResponse.ResultCode == Hive5ResultCode.Success)
            {
                var callProcedureResult = callProcedureResponse.ResultData as CallProcedureResponseBody;
                if (callProcedureResult == null)
                    return;
				
				try
				{
	                if (hive5Process != null)
	                    hive5Process.GetUserProcess(callProcedureResult.CallReturn);
				}
				catch (Exception ex)
				{
					Logger.DebugLog(ex.ToString());
					
					if (GameUI.Instance.MessageBox != null)
						GameUI.Instance.MessageBox.SetFatalError(NoticePopupWindow.Fatal_Error.GetUserError, getUserErrorStringID);
					
					//OptionWindow.OnLogoutOK(null);
				}
            }
        });
    }
	
	public void TimeOutProcess(string functionName, string jsonString)
	{
		var data = LitJson.JsonMapper.ToObject(jsonString);
		
		int charIndex = 0;
		int usedPotion1 = 0;
		int usedPotion2 = 0;
			
		switch(functionName)
		{
		case "SendStageEnd":
			int curLevel = 0;
			int stageIndex = 0;
			int stageType = 0;
			int gainGold = 0;
			int gainCash = 0;
			List<GainItemInfo> gainItemInfos = new List<GainItemInfo>();
			List<GainItemInfo> gainMaterialItems = new List<GainItemInfo>();
			
			charIndex = data["hero_type"].ToInt();
			curLevel = data["current_level"].ToInt();
			stageIndex = data["stage_index"].ToInt();
			stageType = data["stage_type"].ToInt();
			gainGold = data["gain_gold"].ToInt();
			gainCash = data["gain_jewel"].ToInt();
			usedPotion1 = data["used_potion1"].ToInt();
			usedPotion2 = data["used_potion1"].ToInt();
			
			if (data.Keys.Contains("gain_normal_items") == true)
			{
				for (int index = 0; index < data["gain_normal_items"].Count; ++index)
				{
					var item = data["gain_normal_items"][index];
					
					int ID = item["ID"].ToInt();
					int Count = item["Count"].ToInt();
					
					GainItemInfo newInfo = new GainItemInfo();
					newInfo.ID = ID;
					newInfo.Count = Count;
					
					gainItemInfos.Add(newInfo);
				}
			}
			
			if (data.Keys.Contains("gain_material_items") == true)
			{
				for (int index = 0; index < data["gain_material_items"].Count; ++index)
				{
					var item = data["gain_material_items"][index];
					
					int ID = item["ID"].ToInt();
					int Count = item["Count"].ToInt();
					
					GainItemInfo newInfo = new GainItemInfo();
					newInfo.ID = ID;
					newInfo.Count = Count;
					
					gainMaterialItems.Add(newInfo);
				}
			}
			
			SendStageEnd(charIndex, curLevel, gainGold, gainCash, gainItemInfos.ToArray(), gainMaterialItems.ToArray(), usedPotion1, usedPotion2);
			break;
		case "SendBossRaidEnd":
			
			charIndex = data["hero_type"].ToInt();
			long bossIndex = data["boss_index"].ToInt(); 
			float damageValue = float.Parse(data["damage"].ToString());
			bool isPhase2 = (bool)data["transform"];
			int curHP = data["boss_hp"].ToInt();
			string platform = data["owner_platform"].ToString();
			string owner_id = data["owner_id"].ToString();
            int boss_id = data["boss_id"].ToInt();

            SendBossRaidEnd(bossIndex, damageValue, isPhase2, curHP, platform, owner_id, boss_id);
			break;
		case "SendStageEndFailed":
			charIndex = data["hero_type"].ToInt();
			usedPotion1 = data["used_potion1"].ToInt();
			usedPotion2 = data["used_potion1"].ToInt();
			
			SendStageEndFailed(charIndex, usedPotion1, usedPotion2);
			break;
		case "SendTownTutorialEnd":
			charIndex = data["hero_type"].ToInt();
			SendTownTutorialEnd(charIndex);
			break;
		case "SendTutorialEnd":
			charIndex = data["hero_type"].ToInt();
			SendTutorialEnd(charIndex);
			break;
		case "SendWaveEnd":
			charIndex = data["hero_type"].ToInt();
			int waveStep = data["wave_step"].ToInt();
			int waveSec = data["wave_sec"].ToInt();
			int isClear = (bool)data["clear"] == true ? 1 : 0;
			usedPotion1 = data["used_potion1"].ToInt();
			usedPotion2 = data["used_potion1"].ToInt();
			SendWaveEnd(charIndex, waveStep, waveSec, isClear, usedPotion1, usedPotion2);
			break;
		}
	}
	
	public void SendUpdateFailedKakaoFriends(string jsonString)
	{
		var data = new {
			kakao_result = jsonString
		};
		
		hive5.CallProcedure("update_failed_kakao_friends", data, (callResponse) =>
        {
			
		});
	}
}