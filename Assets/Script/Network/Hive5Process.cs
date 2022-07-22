using UnityEngine;
using Hive5;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;

public class Hive5Process
{
    public ClientConnector Connector;
    public int retryCount = 0;
    public string retryMethodName = "";

    public Hive5Process()
    {
        this.retryCount = 0;
        this.retryMethodName = "";
    }

    public void AddTimeLimitBuffItem(string buff_type, int buff_left_sec)
    {
        if (buff_left_sec <= 0)
            return;

        CharInfoData charData = Game.Instance.charInfoData;

        System.TimeSpan timeSpan = Game.ToTimeSpan(buff_left_sec);
        // todo. now를 serverTime으로 변경해야함.
        System.DateTime endTime = System.DateTime.Now + timeSpan;

        if (charData.timeLimitBuffList == null)
            charData.CreateTimeLimitBuffList();

        TableManager tableManager = TableManager.Instance;
        TimeLimitItemTable timeLimitItemTable = tableManager != null ? tableManager.timeLimitItemTable : null;

        if (charData == null || timeLimitItemTable == null)
            return;

        TimeLimitBuffItemInfo buffItemInfo = timeLimitItemTable.GetDataByType(buff_type);
        if (buffItemInfo == null)
            return;

        foreach (TimeLimitBuffInfo buffInfo in buffItemInfo.buffList)
        {
            charData.AddTimeLimitBuff(endTime, buffInfo.buffType, buffItemInfo.type, buffInfo.buffValue);
        }

        TownUI townUI = GameUI.Instance.townUI;
        if (townUI != null)
            townUI.UpdateLimitTimeBuff(charData);

    }

    public void GetUserProcess(string callReturn)
    {
        if (string.IsNullOrEmpty(callReturn) == true)
            return;

        var rootJson = JsonMapper.ToObject(callReturn);
        Debug.Log("Procedure return:" + callReturn);

        if (rootJson["user"].Keys.Contains("name") == true)
        {
            if (rootJson["user"]["name"] != null)
                Connector.Nick = rootJson["user"]["name"].ToString();
        }
		else
			Connector.Nick = "";
		
		Logger.DebugLog(string.Format("GetUser :: Nick {0}", Connector.Nick));
        
        Game.Instance.androidManager.CallReadyGoogleRegID(Connector.UserIndexID.ToString());

        if (Game.Instance.charInfoData == null)
            Game.Instance.InitCharInfoData();

        CharInfoData charData = Connector.charInfo = Game.Instance.charInfoData;
        if (charData == null)
        {
            Logger.DebugLog("charData = null");
            return;
        }
        // 서버에서 전체값을 준다.
        charData.expandNormalItemSlotCount = (int)rootJson["inv_size"] - charData.baseItemSlotCount;
        charData.expandMaterialItemSlotCount = (int)rootJson["inv_size_material"] - charData.baseItemSlotCount;
		
		Logger.DebugLog(string.Format("GetUser :: slotCount {0}, material_slotCount{1}", charData.expandNormalItemSlotCount, charData.expandMaterialItemSlotCount));
        
		
        int serverTime = (int)rootJson["server_time"];
		Logger.DebugLog(string.Format("GetUser :: server_time : {0}", serverTime));
        

        for (int i = 0; i < rootJson["buff_left_sec"]["type"].Count; i++)
		{
			string buff_type = rootJson["buff_left_sec"]["type"][i].ToString();
			int buff_left_sec = (int)rootJson["buff_left_sec"]["diff"][i];
			Logger.DebugLog(string.Format("GetUser :: buff_left_sec type : {0} {1}", buff_type, buff_left_sec));
        	if (buff_left_sec > 0)
           		AddTimeLimitBuffItem(buff_type, buff_left_sec);
		}
		

        // user
        var userJson = rootJson["user"];
        charData.SetNickName(Connector.Nick);
        charData.SetGold((int)userJson["gold"], (int)userJson["jewel"], (int)userJson["medal"]);
        charData.gambleCoupon = (int)userJson["coupon"];
        charData.isTutorialComplete = userJson["tutorial"].ToInt() == 1 ? true : false;
        charData.potion1 = (int)userJson["potion1"];
        charData.potion1Present = (int)userJson["potion1_gift"];
        charData.potion2 = (int)userJson["potion2"];
        charData.potion2Present = (int)userJson["potion2_gift"];
        charData.ticket = (int)userJson["ticket"]["value"];
		
		Logger.DebugLog(string.Format("GetUser :: user"));
		
        // heros
        bool isArray = rootJson["heros"].IsArray;
		
		Logger.DebugLog(string.Format("GetUser :: hero start...."));
		
        for (int i = 0; i < rootJson["heros"].Count; i++)
        {
            var heroJson = rootJson["heros"][i];
            int heroIndex = (int)heroJson["hero_type"];
            CharPrivateData privateData = charData.GetPrivateData(heroIndex);
            if (privateData == null)
            {
                Logger.DebugLog("privateData = null");
                continue;
            }

            int gambleLeftSec = heroJson["gamble_start_at"] == null ? 0 : heroJson["gamble_start_at"].ToDateTime() - serverTime;
            int staminaLeftTimeSec = (int)heroJson["stamina"]["next_time"] - serverTime;
            if (staminaLeftTimeSec < 0)
                staminaLeftTimeSec = 0;

            //var AExp = heroJson["awaken_exp"].ToLong();
            //var ABuyCount = 0;
            //var AExpStr = heroJson["awaken_exp"].ToString();
            //var ALevel = (int)heroJson["awaken_level"];
            //var ALimitBuyCount = 0;
            //var APoint = (int)heroJson["awaken_point"];
            //var CharacterIndex = (int)heroJson["hero_type"];
            //var ExpValue = heroJson["exp"].ToLong();
            //var ExpValueStr = heroJson["exp"].ToString();
            //var GambleLeftSec = gambleLeftSec;
            //var Level = (int)heroJson["level"];
            //var SkillPoint = (int)heroJson["skill_pt"];
            //var StageIndex = new int[] { 0, 0, 0, 0, 0 };
            //var StaminaCur = (int)heroJson["stamina"]["value"];
            //var StaminaMax = (int)heroJson["stamina"]["max"];
            //var Ticket = (int)heroJson["ticket"]["value"];
            //var StaminaPresent = (int)heroJson["stamina_gift"];
            //var tutorial = (int)heroJson["tutorial"];
            //var StaminaLeftTimeSec = 0;

            var buyLimitPoint = heroJson["awaken_point_info"]["buyable_point"].ToInt();
            var buyPoint = Mathf.Min(buyLimitPoint, heroJson["awaken_point_info"]["buy_point"].ToInt());
            var giftPoint = heroJson["awaken_point_info"]["gift_point"].ToInt();
            var gainPoint = heroJson["awaken_point_info"]["gain_point"].ToInt();
            var usedPoint = heroJson["awaken_point_info"]["used_point"].ToInt();
            //var availPoint = (gainPoint + buyPoint + giftPoint) - usedPoint;

            var baseInfo = new CharacterDBInfo();
                {
                    baseInfo.AExp = heroJson["awaken_exp"].ToLong();
                    baseInfo.AExpStr = heroJson["awaken_exp"].ToString();
                    baseInfo.ALevel = (int)heroJson["awaken_level"];

                    baseInfo.ALimitBuyCount = buyLimitPoint;
                    baseInfo.ABuyCount = buyPoint;
                    baseInfo.APointGift = giftPoint;
                    baseInfo.APoint = gainPoint;

                    baseInfo.CharacterIndex = (int)heroJson["hero_type"];
                    baseInfo.ExpValue = heroJson["exp"].ToLong();
                    baseInfo.ExpValueStr = heroJson["exp"].ToString();
                    baseInfo.gambleLeftSec = gambleLeftSec;
                    baseInfo.Level = (int)heroJson["level"];
                    baseInfo.SkillPoint = (int)heroJson["skill_pt"];
                    baseInfo.StageIndex = new int[] { heroJson["normal_stage"].ToInt(), heroJson["hard_stage"].ToInt(), heroJson["hell_stage"].ToInt() };
                    baseInfo.StaminaCur = (int)heroJson["stamina"]["value"];
                    baseInfo.StaminaMax = (int)heroJson["stamina"]["max"];
                    baseInfo.StaminaPresent = (int)heroJson["stamina_gift"];
                    baseInfo.tutorial = (int)heroJson["tutorial"];
                    baseInfo.StaminaLeftTimeSec = staminaLeftTimeSec;
                }
            /*
            if (Hive5Client.Instance.Zone != Hive5APIZone.Production)
            {
                baseInfo.tutorial = 1;
            }*/
			
			Logger.DebugLog(string.Format("GetUser :: baseInfo"));

            //////////////////////////////////////////////////////////////////////////////////////////
            //특성 스킬..
            int[] skill_ids = LitJson.JsonMapper.ToObject<int[]>(heroJson["skill_ids"].ToJson());
            int[] skill_levels = LitJson.JsonMapper.ToObject<int[]>(heroJson["skill_levels"].ToJson());

            int skill_count = Mathf.Min(skill_ids.Length, skill_levels.Length);
            for (int index = 0; index < skill_count; ++index)
            {
                privateData.SetSkillData(privateData.masteryDatas, skill_ids[index], skill_levels[index]);
            }
			
			Logger.DebugLog(string.Format("GetUser :: skill Info"));
            ////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////////////////////////////
            //각성 스킬..
            skill_ids = LitJson.JsonMapper.ToObject<int[]>(heroJson["awaken_skill_ids"].ToJson());
            skill_levels = LitJson.JsonMapper.ToObject<int[]>(heroJson["awaken_skill_levels"].ToJson());

            skill_count = Mathf.Min(skill_ids.Length, skill_levels.Length);
            for (int index = 0; index < skill_count; ++index)
            {
                privateData.SetAwakeningSkillData(skill_ids[index], skill_levels[index]);
            }
			Logger.DebugLog(string.Format("GetUser :: awaken Info"));
            /////////////////////////////////////////////////////////////////////////////////////////////

            privateData.baseInfo = baseInfo;
            privateData.SetGambleTime(baseInfo.gambleLeftSec);
            privateData.SetStaminaRefreshTime(baseInfo.StaminaLeftTimeSec);

            privateData.InitMapInfo();
        }

        var herosWearItems = rootJson["heros_wear_items"];
        if (herosWearItems != null)
        {
            for (int i = 0; i < herosWearItems.Count; ++i)
            {
                CharPrivateData privateData = charData.GetPrivateData(i);
                if (privateData == null)
                {
                    Logger.DebugLog("privateData = null");
                    continue;
                }

                var wearItems = herosWearItems[i];

                for (int j = 0; j < wearItems.Count; ++j)
                {
                    var heroWearItems = wearItems[j];

                    var info = new EquipItemDBInfo();

                    info.UID = heroWearItems["id"].ToString();
                    info.ID = (int)heroWearItems["table_id"];
                    info.SlotIndex = (int)heroWearItems["slot_index"];

                    int itemtype = (int)heroWearItems["type"];

                    switch (itemtype)
                    {
                        case Item_Type.costume:
                            {
                                info.Count = 1;
                                info.Rate = -1;
                                privateData.SetEquipData(info);
                            }
                            break;
                        case Item_Type.costumeset:
                            {
                                privateData.SetCostumeSetData(info.UID, info.ID);
                            }
                            break;
                        case Item_Type.material:
                        case Item_Type.normal:
                            {
                                info.Grade = (int)heroWearItems["grade"];
                                info.Count = (int)heroWearItems["count"];
                                info.Reinforce = (int)heroWearItems["reinforce"];
                                info.Rate = (int)heroWearItems["rate"];
                                info.Exp = (int)heroWearItems["exp"];

                                privateData.SetEquipData(info);
                            }
                            break;
                        default:
                            { }
                            break;
                    }
                }
            }
        }

        Logger.DebugLog(string.Format("GetUser :: shop_history"));
        
        JsonData shop_history = rootJson["shop_history"];

        if (rootJson["shop_history"] != null && shop_history.Keys.Contains("ids") == true)
        {
            for (int i = 0; i < shop_history["ids"].Count; ++i)
            {
                int id = shop_history["ids"][i].ToInt();

                Connector.charInfo.packageItems.Add(id);
            }               
        }

		Logger.DebugLog(string.Format("GetUser :: equip Info"));

        var invenItems = rootJson["items"];
        if (invenItems != null)
        {
            for (int i = 0; i < invenItems.Count; ++i)
            {
                var info = invenItems[i];
                int ID = (int)info["table_id"];
                string UID = info["id"].ToString();
                int Grade = (int)info["grade"];
                int Reinforce = (int)info["reinforce"];
                int Rate = (int)info["rate"];
                int Count = (int)info["count"];
                int itemExp = (int)info["exp"];

                Item invenItem = Item.CreateItem(ID, UID, Grade, Reinforce, Count, Rate, itemExp);
                if (invenItem != null)
                {
                    charData.AddItem(invenItem);
                }
            }
        }
		Logger.DebugLog(string.Format("GetUser :: inventory Info"));

        var invenMaterials = rootJson["materials"];
        if (invenMaterials != null)
        {
            for (int i = 0; i < invenMaterials.Count; ++i)
            {
                var info = invenMaterials[i];
                int ID = (int)info["table_id"];
                string UID = info["id"].ToString();
                int Grade = (int)info["grade"];
                int Reinforce = (int)info["reinforce"];
                int Rate = (int)info["rate"];
                int Count = (int)info["count"];
                int itemExp = (int)info["exp"];

                Item invenItem = Item.CreateItem(ID, UID, Grade, Reinforce, Count, Rate, itemExp);
                if (invenItem != null)
                {
                    charData.AddMaterial(invenItem);
                }
            }
        }
		Logger.DebugLog(string.Format("GetUser :: inventory Material Info"));

        var invenCostumes = rootJson["costumes"];
        if (invenCostumes != null)
        {
            for (int index = 0; index < invenCostumes.Count; ++index)
            {
                int ID = (int)invenCostumes[index]["table_id"];
                string UID = invenCostumes[index]["id"].ToString();

                Item invenItem = Item.CreateItem(ID, UID, 0, 0, 1, -1, 0);

                if (invenItem != null)
                    charData.AddCostume(invenItem);
            }
        }
		Logger.DebugLog(string.Format("GetUser :: inventory Costume Info"));

        var invenCostumeSets = rootJson["costumesets"];
        if (invenCostumeSets != null)
        {
            for (int index = 0; index < invenCostumeSets.Count; ++index)
            {
                int ID = (int)invenCostumeSets[index]["table_id"];
                string UID = invenCostumeSets[index]["id"].ToString();

                CostumeSetItem costumeSet = CostumeSetItem.Create(ID, UID);
                if (costumeSet != null)
                    charData.AddCostumeSetItem(costumeSet);
            }
        }
		Logger.DebugLog(string.Format("GetUser :: inventory CostumeSet Info"));

        var achieveclearJson = rootJson["achievement_clear"];
        if (achieveclearJson != null)
        {
            for (int i = 0; i < achieveclearJson.Count; ++i)
            {
                var info = achieveclearJson[i];

                int heroType = achieveclearJson[i]["hero_type"].ToInt();
                int stepID = achieveclearJson[i]["step_id"].ToInt();
                int groupID = achieveclearJson[i]["group_id"].ToInt();

                AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

                if (achieveMgr != null)
                {
                    achieveMgr.SetClearInfo(heroType, stepID, groupID);
                }
            }
        }
		Logger.DebugLog(string.Format("GetUser :: achievement_clear Info"));

        var achieveJson = rootJson["achievement"];
        if (achieveJson != null)
        {
            for (int i = 0; i < achieveJson.Count; ++i)
            {
                var info = achieveJson[i];

                int heroType = achieveJson[i]["hero_type"].ToInt();
                int groupID = achieveJson[i]["group_id"].ToInt();
                int count = achieveJson[i]["count"].ToInt();
                AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

                if (achieveMgr != null)
                {
                    achieveMgr.SetAchieveInfo(heroType, groupID, count);
                }
            }
        }
		Logger.DebugLog(string.Format("GetUser :: achievement Info"));

        var dailyJson = rootJson["daily_mission"];
        if (rootJson["daily_mission"] != null)
        {
            int expired_time = dailyJson["expired_time"].ToInt();

            if (dailyJson["data"] != null)
            {
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
		Logger.DebugLog(string.Format("GetUser :: daily_mission Info"));

        // popup
        var popupsJson = rootJson["popups"];

        List<string> contents = new List<string>();
        List<long> ids = new List<long>();
        List<string> image_urls = new List<string>();	// 이미지공지.
        List<string> link_urls = new List<string>();		// 이미지를 누르면 웹페이지를 띄워준다. 
        List<int> orders = new List<int>();

        for (int k = 0; k < popupsJson.Count; k++)
        {
            var popupJson = popupsJson[k];
            contents.Add((string)popupJson["description"]);
            ids.Add(popupJson["id"].ToLong());
            orders.Add(popupJson["order"].ToInt());
            image_urls.Add((string)popupJson["image_url"]);
            link_urls.Add((string)popupJson["link_url"]);
        }

        PacketPopupNotice pd = new PacketPopupNotice()
        {
            Contents = contents.ToArray(),
            IDs = ids.ToArray(),
            ImageUrls = image_urls.ToArray(),
            LinkUrls = link_urls.ToArray(),
            Orders = orders.ToArray(),
        };


        Game.Instance.noticeItems.Clear();

        for (int i = 0; i < pd.IDs.Length; ++i)
        {
            NoticeItem.eNoticeType type = NoticeItem.eNoticeType.None;

            string infoStr = "";
            int order = 0;
            long noticeID = pd.IDs[i];
            string linkURL = "";

            if (!String.IsNullOrEmpty(pd.ImageUrls[i]) && pd.ImageUrls[i].CompareTo("null") != 0)
            {
                Logger.DebugLog("Recv Notice: " + pd.ImageUrls[i]);

                type = NoticeItem.eNoticeType.ImageURL;
                infoStr = pd.ImageUrls[i];
                linkURL = pd.LinkUrls[i];
                order = pd.Orders[i];
            }
            else if (!String.IsNullOrEmpty(pd.Contents[i]))
            {
                Logger.DebugLog("Recv Town Notice: " + pd.Contents[i]);

                type = NoticeItem.eNoticeType.Message;
                infoStr = pd.Contents[i];
                order = pd.Orders[i];
            }

            NoticeItem notice = null;
            switch (type)
            {
                case NoticeItem.eNoticeType.ImageURL:
                    notice = new NoticeItem();
                    notice.imgURL = infoStr;
                    notice.linkURL = linkURL;
                    notice.type = type;
                    notice.order = order;
                    break;
            }

            if (notice != null)
            {
                notice.noticeID = noticeID;
                Game.Instance.noticeItems.Add(notice);
            }
        }
		
		Logger.DebugLog(string.Format("GetUser :: popups Info"));

        if (Game.Instance.noticeItems.Count > 0)
            Game.Instance.noticeItems.Sort(NoticeItem.SortByOrder);

        if (GameUI.Instance.loginPage != null)
            GameUI.Instance.loginPage.OnLoginOK();
		
		if (Game.Instance.AndroidManager != null)
		{
			Game.Instance.AndroidManager.SendPartyTrackEvent("Signup");
		}
    }

    public void ErrorProcess(Hive5Response response, string methodName = null, System.Object[] parameters = null)
    {
        ErrorProcess(new PacketError()
        {
            ErrorCode = (int)ErrorCodeConverter.Convert(response.ResultCode, response.ResultMessage),
            ErrorMessage = response.ResultMessage,
        }, 
        methodName, parameters);
    }

    void CreateRetryPopup(string methodName, System.Object[] parameters)
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

        ReconnectPopup popup = ResourceManager.CreatePrefabByResource<ReconnectPopup>("SystemPopup/ReconnectPopup", rootTrans, Vector3.zero);

        
        if (popup)
            popup.SetInfo(Game.Instance.PacketSender, methodName, parameters);
    }

    void CreateRestartPopup()
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
        RestartPopup popup = ResourceManager.CreatePrefabByResource<RestartPopup>("SystemPopup/RestartPopup", rootTrans, Vector3.zero);
    }

    public void ErrorProcess(PacketError pd, string MethodName = null, System.Object[] parameters = null)
    {
        Logger.DebugLog("Recv Error[" + pd.ErrorCode.ToString() + "] " + pd.ErrorMessage);

        TableManager tableManager = TableManager.Instance;
        StringTable stringTable = tableManager != null ? tableManager.stringTable : null;

        string errorMsg = "Interal Error";
        if (stringTable != null)
            errorMsg = stringTable.GetData((int)pd.ErrorCode);

        switch ((NetErrorCode)pd.ErrorCode)
        {
            case NetErrorCode.WebTimeOut:
                if (MethodName == null)
                {
                    // CreateRestartPopup();
                }
                else
                {
                    if (this.retryMethodName != MethodName)
                        retryCount = 0;
                    else
                        ++retryCount;

                    if (retryCount > 2)
                        CreateRestartPopup();
                    else
                        CreateRetryPopup(MethodName, parameters);

                    this.retryMethodName = MethodName;

                    return;
                }
                break;
            case NetErrorCode.NotConnected:
                Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.NetworkError);
                return;
            case NetErrorCode.XignCodeInvalid:
                Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.HackDetected);
                return;
            case NetErrorCode.DuplicateConnection:
                UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
                Transform rootTrans = null;

                if (uiRoot != null)
                {
                    if (uiRoot.popUpNode != null)
                        rootTrans = uiRoot.popUpNode;
                    else
                        rootTrans = uiRoot.transform;
                }

                DuplicateConnectionPopup popup = ResourceManager.CreatePrefabByResource<DuplicateConnectionPopup>("SystemPopup/DuplicateConnectionPopup", rootTrans, Vector3.zero);
                if (popup != null)
                    popup.SetMessage(errorMsg);

                Game.Instance.ResetLoginInfo(false);
                return;

            case NetErrorCode.LoginAccount_Duplicate:
            case NetErrorCode.LoginPassword_Wrong:
            case NetErrorCode.AccountIDNotEmail:
            case NetErrorCode.AccountIDInvalid:
            case NetErrorCode.DropoutAccount:
            case NetErrorCode.NotFoundAccountID:
                GameUI.Instance.loginPage.OnLoginError(pd.ErrorCode);

                Game.Instance.ResetLoginInfo(false);
                break;
        }

        if (pd.ErrorCode < (int)NetErrorCode.Unknown)
        {
            CreateRestartPopup();
            return;
        }
        
        if (pd.ErrorCode != (int)NetErrorCode.None)
        {
            if (GameUI.Instance.MessageBox)
                GameUI.Instance.MessageBox.SetMessage(errorMsg);
        }
    }
}

