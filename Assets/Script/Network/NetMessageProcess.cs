using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using Hive5;

public class NetMessageProcess : INetMessageProcess
{


    ClientConnector connector;

    public ClientConnector Connector
    {
        get { return this.connector; }
        set { this.connector = value; }
    }

    public void ServerCheckingProcess(string packet)
    {
        PacketServerChecking pd = LitJson.JsonMapper.ToObject<PacketServerChecking>(packet);

        Logger.DebugLog("Recv ServerChecking " + packet);

        // 점검없음.
        if (pd.errorCode == NetErrorCode.OK)
        {
            EmptyLoadingPage loadingPage = Game.Instance.loadingPage;
            if (loadingPage != null)
                loadingPage.LoadBundleVersion();

            //connector.SendPreLogin();
        }
        // 있음 공지띄우고 닫기누르면 종료한다.
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
            //Game.Instance.networkManager.androidManager.CallUnityExit("serverChecking");
        }
    }

    public void RequestReconnectProcess(string packet)
    {
        // 서버와 의 연결이 끊겼음.
        // 위의 패킷으로 재시도한다.
        UIRootPanel uiRoot = GameUI.Instance.uiRootPanel;
        Transform rootTrans = null;

        if (uiRoot != null)
        {
            if (uiRoot.popUpNode != null)
                rootTrans = uiRoot.popUpNode;
            else
                rootTrans = uiRoot.transform;
        }

        PacketRequestReconnect msg = LitJson.JsonMapper.ToObject<PacketRequestReconnect>(packet);

        ReconnectPopup popup = ResourceManager.CreatePrefabByResource<ReconnectPopup>("SystemPopup/ReconnectPopup", rootTrans, Vector3.zero);
        if (popup != null)
        {
            popup.SetInfo(msg.packet, msg.url);
        }
    }

    public void NeedUpdateAppProcess(string packet)
    {
        // 소프트 웨어 업데이트가 필요합니다. 팝업 띄우고 확인누르면 웹페이지로 아래의 이동.		
        PacketNeedUpdateApp msg = LitJson.JsonMapper.ToObject<PacketNeedUpdateApp>(packet);

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
        {
            popup.UpdateURL = msg.url;
        }

        //Application.OpenURL(msg.url);
    }

    public void PreLoginProcess(string packet)
    {
        PacketPreLoginDone pd = LitJson.JsonMapper.ToObject<PacketPreLoginDone>(packet);
        connector.SetEncrypt(pd.bEncrpyt == 1);
        connector.SetToken(pd.token);
        Logger.DebugLog("Recv PreLoginProcess bEncrpyt:" + pd.bEncrpyt);

        GameUI.Instance.loginPage.OnPreLoinOK();
    }

    public void ErrorProcess(string packet)
    {
        PacketError pd = LitJson.JsonMapper.ToObject<PacketError>(packet);

        Logger.DebugLog("Recv Error[" + pd.ErrorCode.ToString() + "] " + pd.ErrorMessage);

        switch ((NetErrorCode)pd.ErrorCode)
        {
            case NetErrorCode.XignCodeInvalid:
                Game.Instance.AndroidManager.CallUnityExitWindow(AlertDialogType.HackDetected);
                return;
            case NetErrorCode.DuplicateConnection:
                // 중복로그인을 알리고 프로그램 종료.
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
                    popup.SetMessage(pd.ErrorMessage);

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

        // 재시작이나 로그인단으로 돌아간다.
        if (pd.ErrorCode < (int)NetErrorCode.Unknown)
        {
            // 별다른 메세지 없이  로그인 부터.
            Game.Instance.ResetLoginInfo(false);

            Resources.UnloadUnusedAssets();
            Application.LoadLevelAsync(0);
        }
        else
        {

            //if (pd.ErrorCode > (int)NetErrorCode.Unknown)
            if (pd.ErrorCode != (int)NetErrorCode.None)
            {
                if (GameUI.Instance.MessageBox)
                    GameUI.Instance.MessageBox.SetMessage(pd.ErrorMessage);
            }

        }
    }

    public void UserInfoProcess(string packet)
    {
        Logger.DebugLog("Recv PacketUserInfo:" + packet);

        PacketUserInfo pd = LitJson.JsonMapper.ToObject<PacketUserInfo>(packet);

        LoginInfo loginInfo = Game.Instance.loginInfo;

        if (loginInfo != null)
            loginInfo.userID = pd.UserIndexID;

        connector.UserIndexID = pd.UserIndexID;
        connector.Nick = pd.NickName;

        Game.Instance.androidManager.CallReadyGoogleRegID(connector.UserIndexID.ToString());

        if (Game.Instance.charInfoData == null)
            Game.Instance.InitCharInfoData();

        CharInfoData charData = connector.charInfo = Game.Instance.charInfoData;

        charData.SetNickName(pd.NickName);
        charData.SetGold(pd.Gold, pd.Cash, pd.Medal);
        charData.gambleCoupon = pd.Coupon;
        charData.isTutorialComplete = pd.tutorial != 0;
        charData.potion1 = pd.potion1;
        charData.potion1Present = pd.potion1Present;
        charData.potion2 = pd.potion2;
        charData.potion2Present = pd.potion2Present;


        foreach (int packageID in pd.buyLimitedItems)
            connector.charInfo.packageItems.Add(packageID);

        LoginInfo info = Game.Instance.loginInfo;

        if (info != null && string.Compare(info.loginID, connector.tempAccountID) != 0 && info.acctountType != connector.tempAccountType)
        {
            info.loginID = connector.tempAccountID;
            info.pass = connector.tempPass;
            info.acctountType = connector.tempAccountType;
            info.loginDate = System.DateTime.Now;

            Game.Instance.SaveLoginData();
        }

        connector.tempAccountID = "";
        connector.tempPass = "";
        connector.tempAccountType = AccountType.MonsterSide;
    }

    public void PopupNoticeProcess(string packet)
    {
        PacketPopupNotice pd = LitJson.JsonMapper.ToObject<PacketPopupNotice>(packet);

        Game.Instance.noticeItems.Clear();

        for (int i = 0; i < pd.IDs.Length; ++i)
        {
            NoticeItem.eNoticeType type = NoticeItem.eNoticeType.None;

            string infoStr = "";
            int order = 0;
            long noticeID = pd.IDs[i];
            string linkURL = "";

            if (!String.IsNullOrEmpty(pd.ImageUrls[i]))
            {
                Logger.DebugLog("Recv Notice: " + pd.ImageUrls[i]);

                type = NoticeItem.eNoticeType.ImageURL;
                infoStr = pd.ImageUrls[i];
                linkURL = pd.LinkUrls[i];
                order = pd.Orders[i];
            }
            else if (!String.IsNullOrEmpty(pd.Contents[i]))
            {
                Logger.DebugLog("Recv Notice: " + pd.Contents[i]);

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
                case NoticeItem.eNoticeType.Message:
                    notice = new NoticeItem();
                    notice.message = infoStr;
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

        if (Game.Instance.noticeItems.Count > 0)
            Game.Instance.noticeItems.Sort(NoticeItem.SortByOrder);
    }

    public void CreateNickNameProcess(string packet)
    {
        PacketCreateNickName msg = LitJson.JsonMapper.ToObject<PacketCreateNickName>(packet);

        Logger.DebugLog("Recv Request CreateNickName " + msg.UserIndexID);

        LoginPage loginPage = GameUI.Instance.loginPage;

        if (loginPage != null)
            loginPage.OnCreateNickResult(msg.errorCode);
    }

    public void RequestCreateNickNameProcess(string packet)
    {
        PacketCreateNickName msg = LitJson.JsonMapper.ToObject<PacketCreateNickName>(packet);
        Logger.DebugLog("Recv Request CreateNickName " + msg.UserIndexID);

        connector.UserIndexID = msg.UserIndexID;

        LoginPage loginPage = GameUI.Instance.loginPage;

        if (loginPage != null)
            loginPage.OnCreateNickName();
    }

    public void CheckNickNameProcess(string packet)
    {
        PacketCheckNickName checkNick = LitJson.JsonMapper.ToObject<PacketCheckNickName>(packet);
        LoginPage loginPage = GameUI.Instance.loginPage;
        if (loginPage != null)
        {
            loginPage.OnCheckNickResult(checkNick.errorCode);
        }
    }

    public void CharacterInfoProcess(string packet)
    {
        Logger.DebugLog("Recv CharacterInfo:" + packet);

        PacketCharacterInfo pd = LitJson.JsonMapper.ToObject<PacketCharacterInfo>(packet);

        for (int i = 0; i < pd.Count; ++i)
        {
            CharInfoData charData = Game.Instance.charInfoData;

            int charIndex = pd.Infos[i].CharacterIndex;
            CharacterDBInfo baseInfo = pd.Infos[i];

            baseInfo.ExpValue = long.Parse(baseInfo.ExpValueStr);
            baseInfo.AExp = long.Parse(baseInfo.AExpStr);

            if (baseInfo.ExpValue < 0L)
                baseInfo.ExpValue = 0L;
            if (baseInfo.AExp < 0L)
                baseInfo.AExp = 0L;

            CharPrivateData privateData = null;
            if (charData != null)
                privateData = charData.GetPrivateData(charIndex);

            if (privateData != null)
            {
                privateData.baseInfo = baseInfo;
                //privateData.baseInfo.tutorial = 1;

                privateData.SetGambleTime(baseInfo.gambleLeftSec);
                privateData.SetStaminaRefreshTime(baseInfo.StaminaLeftTimeSec);

                privateData.InitMapInfo();
            }
        }
    }


    public void SkillInfoProcess(string packet)
    {
        PacketSkillInfo pd = LitJson.JsonMapper.ToObject<PacketSkillInfo>(packet);

        CharPrivateData privateData = connector.charInfo.GetPrivateData(pd.CharacterIndex);
        if (privateData != null && pd.Info != null)
        {
            int nCount = Math.Min(pd.Info.IDs.Length, pd.Info.Lvs.Length);
            int skillID = 0;
            int skillLv = 0;

            for (int index = 0; index < nCount; ++index)
            {
                skillID = pd.Info.IDs[index];
                skillLv = pd.Info.Lvs[index];

                privateData.SetMasteryData(skillID, skillLv);
            }
        }

        Logger.DebugLog("Recv SkillInfo:" + pd.CharacterIndex);
    }

    public void CostumeProcess(string packet)
    {
        Logger.DebugLog("Recv Costume:" + packet);
        PacketInventory pd = LitJson.JsonMapper.ToObject<PacketInventory>(packet);
        //Game.Instance.charInfoData.inventoryCostumeData.Clear();
        connector.charInfo.inventoryCostumeData.Clear();

        for (int index = 0; index < pd.Count; ++index)
        {
            ItemDBInfo dbInfo = pd.Infos[index];

            Item invenItem = Item.CreateItem(dbInfo);
            if (invenItem != null)
                connector.charInfo.AddCostume(invenItem);
        }

        Game.RerangeItemList(connector.charInfo.inventoryCostumeData);
    }

    public void EquipItemProcess(string packet)
    {
        PacketEquipItem pd = LitJson.JsonMapper.ToObject<PacketEquipItem>(packet);

        Logger.DebugLog("Recv EquipItem:" + pd.CharacterIndex);

        //connector.charInfolist[pd.CharacterIndex].SetEquipItemList(pd.Count, pd.Infos);

        connector.charInfo.privateDatas[pd.CharacterIndex].SetEquipItemList(pd.Count, pd.Infos, pd.SetItem);

    }

    public void InventoryProcess(string packet)
    {
        Logger.DebugLog("Recv Inven:" + packet);

        PacketInventory pd = LitJson.JsonMapper.ToObject<PacketInventory>(packet);
        Game.Instance.charInfoData.inventoryNormalData.Clear();

        for (int index = 0; index < pd.Count; ++index)
        {
            ItemDBInfo dbInfo = pd.Infos[index];

            Item invenItem = Item.CreateItem(dbInfo);
            if (invenItem != null)
                connector.charInfo.AddItem(invenItem);
        }

        Game.RerangeItemList(connector.charInfo.inventoryNormalData);
    }

    public void InventoryNormalItemProcess(string packet)
    {
        PacketInvenNormalInfo pd = LitJson.JsonMapper.ToObject<PacketInvenNormalInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;

        if (charData == null)
            return;

        string UID = "";
        int ID = 0;
        int Grade = 0;
        int Count = 0;
        int Reinforce = 0;
        int Rate = 0;
        int itemExp = 0;

        int nCount = pd.IDs.Length;
        for (int index = 0; index < nCount; ++index)
        {
            ID = pd.IDs[index];
            UID = pd.UIDs[index];
            Grade = pd.Grades[index];
            Reinforce = pd.Reinforces[index];
            Rate = pd.Rates[index];
            Count = pd.Counts[index];
            //itemExp = pd.Exps[index];

            Item invenItem = Item.CreateItem(ID, UID, Grade, Reinforce, Count, Rate, itemExp);
            if (invenItem != null)
            {
                charData.AddItem(invenItem);
            }
        }
    }

    public void InventoryCostumeItemProcess(string packet)
    {
        PacketInvenCostumeInfo pd = LitJson.JsonMapper.ToObject<PacketInvenCostumeInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;

        if (charData == null)
            return;

        string UID = "";
        int ID = 0;
        int Grade = 0;
        int Reinforce = 0;
        int Rate = -1;
        int Count = 1;

        int nCount = pd.IDs.Length;
        for (int index = 0; index < nCount; ++index)
        {
            ID = pd.IDs[index];
            UID = pd.UIDs[index];

            Item invenItem = Item.CreateItem(ID, UID, Grade, Reinforce, Count, Rate, 0);
            if (invenItem != null)
                charData.AddCostume(invenItem);
        }
    }
    public void InventoryMaterialItemProcess(string packet)
    {
        PacketMaterialInfo pd = LitJson.JsonMapper.ToObject<PacketMaterialInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;

        if (charData == null)
            return;

        string UID = "";
        int ID = 0;
        int Grade = 0;
        int Reinforce = 0;
        int Rate = -1;
        int Count = 1;

        int nCount = pd.IDs.Length;
        for (int index = 0; index < nCount; ++index)
        {
            ID = pd.IDs[index];
            UID = pd.UIDs[index];
            Count = pd.Counts[index];

            Item invenItem = Item.CreateItem(ID, UID, Grade, Reinforce, Count, Rate, 0);
            if (invenItem != null)
                charData.AddMaterial(invenItem);
        }
    }
    public void InventoryCostumeSetItemProcess(string packet)
    {
        PacketInvenCostumeSetInfo pd = LitJson.JsonMapper.ToObject<PacketInvenCostumeSetInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;

        if (charData == null)
            return;

        string UID = "";
        int ID = 0;

        int nCount = pd.IDs.Length;
        for (int index = 0; index < nCount; ++index)
        {
            ID = pd.IDs[index];
            UID = pd.UIDs[index];

            CostumeSetItem costumeSet = CostumeSetItem.Create(ID, UID);
            if (costumeSet != null)
                charData.AddCostumeSetItem(costumeSet);
        }
    }

    public void ExpandSlotsInfoProcess(string strPacket)
    {
        PacketInvenExtendInfo packet = LitJson.JsonMapper.ToObject<PacketInvenExtendInfo>(strPacket);

        if (packet.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = Game.Instance.charInfoData;

            //StorageWindow storeWindow = GameUI.Instance.storageWindow;

            if (charData != null)
            {
                int nCount = packet.bNormalInven.Length;
                for (int index = 0; index < nCount; ++index)
                {
                    int invenType = packet.bNormalInven[index];
                    int slotCount = packet.Count[index];

                    if (invenType == 1)
                        charData.expandNormalItemSlotCount = slotCount;
                    else if (invenType == 0)
                        charData.expandMaterialItemSlotCount = slotCount;
                }
            }
        }
    }

    public void ExpandSlotsProcess(string strPacket)
    {
        PacketInvenExtend packet = LitJson.JsonMapper.ToObject<PacketInvenExtend>(strPacket);

        if (packet.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = Game.Instance.charInfoData;

            StorageWindow storeWindow = GameUI.Instance.storageWindow;
            GameDef.eItemSlotWindow slotWindowType = GameDef.eItemSlotWindow.Inventory;

            if (charData != null)
            {
                charData.SetGold(packet.totalGold, packet.totalJewel);

                if (packet.bNormalInven == 1)
                {
                    slotWindowType = GameDef.eItemSlotWindow.Inventory;
                    charData.expandNormalItemSlotCount = packet.Count;
                }
                else if (packet.bNormalInven == 0)
                {
                    slotWindowType = GameDef.eItemSlotWindow.MaterialItem;
                    charData.expandMaterialItemSlotCount = packet.Count;
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
        }
    }

    public void LoginDoneProcess(string packet)
    {
        Logger.DebugLog("PacketLoginDone");

        // gcm regid Register.
        if (!String.IsNullOrEmpty(connector.gcmRegID))
        {
            PacketRegisterGCMID msg = new PacketRegisterGCMID();

            msg.UserIndexID = connector.UserIndexID;
            msg.regID = connector.gcmRegID;

            connector.SendPacket("RegisterGCMID.php", msg);
        }

        if (GameUI.Instance.loginPage != null)
            GameUI.Instance.loginPage.OnLoginOK();
    }

    public void CreateAccountProcess(string strPacket)
    {
        PacketCreateAccount packet = LitJson.JsonMapper.ToObject<PacketCreateAccount>(strPacket);

        if (packet.errorCode == (int)NetErrorCode.OK)
        {
            LoginInfo info = Game.Instance.loginInfo;
            if (info != null)
            {
                info.loginID = packet.Account;
                info.pass = packet.Password;
                info.acctountType = (AccountType)packet.EmailType;
                info.loginDate = System.DateTime.Now;

                Game.Instance.SaveLoginData();
            }
        }
        else
        {
            Game.Instance.loginInfo = null;
            Game.Instance.SaveLoginData();

            GameUI.Instance.signupWindow.OnErrorSignUp(packet.errorCode);
        }
        //		if (GameUI.Instance.loginPage != null)
        //			GameUI.Instance.loginPage.OnLoginOK();
    }

    public void BuyNormalItemProcess(string strPacket)
    {
        PacketBuyNormalItem packet = LitJson.JsonMapper.ToObject<PacketBuyNormalItem>(strPacket);

        if (packet.result == NetErrorCode.OK)
        {
            int nCount = Mathf.Min(packet.UIDs.Length, packet.counts.Length);
            string UID = "";
            int itemCount = 0;

            for (int index = 0; index < nCount; ++index)
            {
                UID = packet.UIDs[index];
                itemCount = packet.counts[index];

                //상점 구입 아이템 하급..
                Item newItem = Item.CreateItem(packet.ItemID, UID, 0, 0, itemCount, packet.rate, packet.exp);
                if (newItem != null)
                {
                    newItem.IsNewItem = true;

                    if (newItem.itemInfo != null && newItem.itemInfo.buyPrice.z > 0.0f)
                        Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyArenaItem, 0);
                }

                AddNewItem(connector.charInfo, newItem);
            }

            connector.charInfo.medal_Value = packet.Medal;
			
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyItem, 0);
        }

        UpdateItemInfos(null, true);

        ShopWindow shopWindow = GameUI.Instance.shopWindow;
        if (shopWindow != null)
        {
            shopWindow.OnBuyNormalItemResult(packet.slotIndex, packet.result);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void BuyCostumeItemProcess(string strPacket)
    {
        PacketBuyCostumeItem packet = LitJson.JsonMapper.ToObject<PacketBuyCostumeItem>(strPacket);

        if (packet.result == NetErrorCode.OK)
        {
            Item newItem = Item.CreateItem(packet.ItemID, packet.UID, 0, 0, 1, -1, 0);
            if (newItem != null)
                newItem.IsNewItem = true;

            connector.charInfo.AddCostume(newItem);
            connector.charInfo.SetGold(packet.Gold, packet.Cash);

            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyItem, 0);
        }

        UpdateItemInfos(null, true);

        ShopWindow shopWindow = GameUI.Instance.shopWindow;
        if (shopWindow != null)
        {
            shopWindow.OnBuyCostumeItemResult(packet.slotIndex, packet.result);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void BuyCostumeSetItemProcess(string strPacket)
    {
        PacketBuyCostumeSetItem packet = LitJson.JsonMapper.ToObject<PacketBuyCostumeSetItem>(strPacket);

        if (packet.result == NetErrorCode.OK)
        {
            CostumeSetItem costumeSet = CostumeSetItem.Create(packet.ItemID, packet.UID);

            connector.charInfo.AddCostumeSetItem(costumeSet);
            connector.charInfo.SetGold(packet.Gold, packet.Cash);

            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eBuyItem, 0);
        }

        UpdateItemInfos(null, true);

        ShopWindow shopWindow = GameUI.Instance.shopWindow;
        if (shopWindow != null)
        {
            shopWindow.OnBuyCostumeSetItemResult(packet.slotIndex, packet.result);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void SellItemProcess(string strPacket)
    {
        PacketSellItem packet = LitJson.JsonMapper.ToObject<PacketSellItem>(strPacket);

        CharPrivateData privateData = connector.charInfo.privateDatas[packet.CharacterIndex];

        if (packet.result == NetErrorCode.OK)
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eSellItem, 0);

        if (packet.Shop)
        {
            ShopWindow shopWindow = GameUI.Instance.shopWindow;

            if (shopWindow == null)
                Logger.DebugLog("SellItemProcess shopWindow null");

            switch (packet.windowType)
            {
                case GameDef.eItemSlotWindow.Equip:
                    if (packet.result == NetErrorCode.OK)
                    {
                        connector.charInfo.RemoveEquipItem(packet.CharacterIndex, packet.slotIndex);
                        connector.charInfo.SetGold(packet.Gold, packet.Cash);
                    }
                    UpdateItemInfos(privateData, true);

                    if (shopWindow != null)
                        shopWindow.OnSellEquipItemResult(packet.slotIndex, packet.result);
                    break;
                default:
                    Logger.DebugLog("SellItemProcess windowType:" + packet.windowType.ToString());
                    break;
            }
        }
        else
        {
            StorageWindow storageWindow = GameUI.Instance.storageWindow;

            if (storageWindow == null)
                Logger.DebugLog("SellItemProcess StorageWindow null");

            switch (packet.windowType)
            {
                case GameDef.eItemSlotWindow.Equip:
                    if (packet.result == NetErrorCode.OK)
                    {
                        connector.charInfo.RemoveEquipItem(packet.CharacterIndex, packet.slotIndex);

                        connector.charInfo.SetGold(packet.Gold, packet.Cash);
                    }

                    UpdateItemInfos(privateData, true);

                    if (storageWindow != null)
                        storageWindow.OnSellEquipItemResult(packet.slotIndex, packet.result);
                    break;
                case GameDef.eItemSlotWindow.Inventory:
                    if (packet.result == NetErrorCode.OK)
                    {
                        //connector.userInfo.RemoveItem(packet.slotIndex);
                        //connector.userInfo.SetMoney(packet.Cash, packet.Gold);

                        connector.charInfo.RemoveItemByIndex(packet.slotIndex, packet.UID);
                        connector.charInfo.SetGold(packet.Gold, packet.Cash);
                    }

                    UpdateItemInfos(privateData, true);

                    if (storageWindow != null)
                        storageWindow.OnSellNormalItemResult(packet.slotIndex, packet.result);
                    break;
                case GameDef.eItemSlotWindow.Costume:
                    if (packet.result == NetErrorCode.OK)
                    {
                        //connector.userInfo.RemoveCostume(packet.slotIndex);
                        //connector.userInfo.SetMoney(packet.Cash, packet.Gold);

                        connector.charInfo.RemoveCostumeByIndex(packet.slotIndex, packet.UID);
                        connector.charInfo.SetGold(packet.Gold, packet.Cash);
                    }

                    UpdateItemInfos(privateData, true);

                    if (storageWindow != null)
                        storageWindow.OnSellCostumeItemResult(packet.slotIndex, packet.result);
                    break;
                case GameDef.eItemSlotWindow.MaterialItem:
                    if (packet.result == NetErrorCode.OK)
                    {
                        connector.charInfo.RemovMaterialItemByIndex(packet.slotIndex, packet.UID);
                        connector.charInfo.SetGold(packet.Gold, packet.Cash);
                    }

                    UpdateItemInfos(privateData, true);

                    if (storageWindow != null)
                        storageWindow.OnSellMaterialItemResult(packet.slotIndex, packet.result);
                    break;
                case GameDef.eItemSlotWindow.CostumeSet:
                    if (packet.result == NetErrorCode.OK)
                    {
                        connector.charInfo.RemoveCostumeSetByIndex(packet.slotIndex, packet.UID);
                        connector.charInfo.SetGold(packet.Gold, packet.Cash);
                    }

                    UpdateItemInfos(privateData, true);

                    if (storageWindow != null)
                        storageWindow.OnSellCostumeSetItemResult(packet.slotIndex, packet.result);
                    break;
                default:
                    Logger.DebugLog("SellItemProcess windowType:" + packet.windowType.ToString());
                    break;
            }
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void DoEquipItemResponeProcess(string strPacket)
    {
        PacketDoEquipItemRespone pd = LitJson.JsonMapper.ToObject<PacketDoEquipItemRespone>(strPacket);

        if (pd.errorCode == NetErrorCode.OK)
        {
            //장착..
            CharPrivateData privateData = connector.charInfo.privateDatas[pd.CharacterIndex];
            if (pd.Equip != null)
                privateData.AddEquipItem(pd.equipSlotIndex, pd.Equip);

            //인벤에서 제거.
            switch (pd.windowType)
            {
                case GameDef.eItemSlotWindow.Inventory:
                    connector.charInfo.RemoveItemByIndex(pd.invenSlotIndex);
                    break;
                case GameDef.eItemSlotWindow.Costume:
                    connector.charInfo.RemoveCostumeByIndex(pd.invenSlotIndex);
                    break;
                case GameDef.eItemSlotWindow.CostumeSet:
                    connector.charInfo.RemoveCostumeSetByIndex(pd.invenSlotIndex);
                    break;
            }

            Item unEquipItem = null;
            if (pd.Unequip != null)
                unEquipItem = Item.CreateItem(pd.Unequip);

            if (unEquipItem != null && unEquipItem.itemInfo != null)
            {
                switch (unEquipItem.itemInfo.itemType)
                {
                    case ItemInfo.eItemType.Costume_Back:
                    case ItemInfo.eItemType.Costume_Body:
                    case ItemInfo.eItemType.Costume_Head:
                        if (unEquipItem.itemCount > 0)
                            connector.charInfo.AddCostume(unEquipItem);
                        else
                            connector.charInfo.RemoveItemByUID(pd.Unequip.UID, connector.charInfo.inventoryCostumeData);
                        break;
                    default:
                        if (unEquipItem.itemCount > 0)
                            connector.charInfo.AddItem(unEquipItem);
                        else
                            connector.charInfo.RemoveItemByUID(pd.Unequip.UID, connector.charInfo.inventoryNormalData);
                        break;
                }
            }

            UpdateItemInfos(privateData, true);
        }

        StorageWindow storageWindow = GameUI.Instance.storageWindow;
        if (storageWindow != null)
        {
            //storageWindow.OnEquipResult(pd.invenSlotIndex, pd.windowType, pd.errorCode);
            storageWindow.OnEquipResult(pd.equipSlotIndex, GameDef.eItemSlotWindow.Equip, pd.errorCode);
        }
    }

    public void DoEquipCostumeSetItemProcess(string strPacket)
    {
        PacketWearCostumeSetItem pd = LitJson.JsonMapper.ToObject<PacketWearCostumeSetItem>(strPacket);
        if (pd.errorCode == NetErrorCode.OK)
        {
            CharPrivateData privateData = null;
            CharInfoData charData = connector.charInfo;
            int charIndex = connector.charIndex;
            if (charData != null)
                privateData = charData.GetPrivateData(charIndex);

            if (privateData != null)
            {
                //1. equip...
                CostumeSetItem equipCostumeSetItem = CostumeSetItem.Create(pd.ItemID, pd.UID);
                privateData.SetCostumeSetItem(equipCostumeSetItem);
            }

            //2. removeItem frome costumeSetItemList..
            if (charData != null)
                charData.RemoveCostumeSetByIndex(pd.slotIndex);

            //3.unEquip item add..
            if (pd.UnwearItem == 0)
            {
                CostumeSetItem unEquipCostumeSetItem = null;
                foreach (CostumeItemDBInfo dbInfo in pd.UnwearItems)
                {
                    unEquipCostumeSetItem = CostumeSetItem.Create(dbInfo.ID, dbInfo.UID);

                    charData.AddCostumeSetItem(unEquipCostumeSetItem);
                }
            }
            else if (pd.UnwearItem == 1)
            {
                Item unEquipCostumeItem = null;
                foreach (CostumeItemDBInfo dbInfo in pd.UnwearItems)
                {
                    unEquipCostumeItem = Item.CreateItem(dbInfo.ID, dbInfo.UID, 0, 0, 1, -1, 0);

                    int equipSlotIndex = EquipInfo.ItemTypeToEquipSlotIndex(null, unEquipCostumeItem.itemInfo.itemType);
                    privateData.RemoveEquipItem(equipSlotIndex, null);

                    charData.AddCostume(unEquipCostumeItem);
                }
            }

            UpdateItemInfos(privateData, true);
        }

        StorageWindow storageWindow = GameUI.Instance.storageWindow;
        if (storageWindow != null)
        {
            storageWindow.OnEquipResult(pd.slotIndex, GameDef.eItemSlotWindow.CostumeSet, pd.errorCode);
        }
    }

    public void DoUnEquipCostumeSetItemProcess(string strPacket)
    {
        PacketUnwearCostumeSetItem packet = LitJson.JsonMapper.ToObject<PacketUnwearCostumeSetItem>(strPacket);

        if (packet.errorCode == NetErrorCode.OK)
        {
            int charIndex = connector.charIndex;
            CharInfoData charData = connector.charInfo;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

            if (privateData != null)
                privateData.SetCostumeSetItem(null);

            if (packet.UnwearItem != null)
            {
                CostumeSetItem unEquipCostumeSetItem = CostumeSetItem.Create(packet.UnwearItem.ID, packet.UnwearItem.UID);

                charData.AddCostumeSetItem(unEquipCostumeSetItem);
            }

            UpdateItemInfos(privateData, true);
        }

        StorageWindow storageWindow = GameUI.Instance.storageWindow;
        if (storageWindow != null)
        {
            storageWindow.OnUnEquipResult(0, packet.errorCode);
        }
    }

    public void DoEquipCostumeItemProcess(string strPacket)
    {
        PacketWearCostumeItem packet = LitJson.JsonMapper.ToObject<PacketWearCostumeItem>(strPacket);

        if (packet.errorCode == NetErrorCode.OK)
        {
            int charIndex = connector.charIndex;
            CharInfoData charData = connector.charInfo;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

            if (privateData != null)
            {
                Item equipItem = Item.CreateItem(packet.ItemID, packet.UID, 0, 0, 1, -1, 0);
                privateData.AddEquipItem(packet.equipSlotIndex, equipItem);
            }

            if (charData != null)
                charData.RemoveCostumeByIndex(packet.slotIndex, packet.UID);

            if (packet.UnwearItemType == 0)
            {
                CostumeSetItem unEquipCostumeSet = null;
                if (packet.UnwearItem != null && packet.UnwearItem.ID > 0 && packet.UnwearItem.UID != "")
                    unEquipCostumeSet = CostumeSetItem.Create(packet.UnwearItem.ID, packet.UnwearItem.UID);

                if (unEquipCostumeSet != null)
                    charData.AddCostumeSetItem(unEquipCostumeSet);

                if (privateData != null)
                    privateData.SetCostumeSetItem(null);
            }
            else if (packet.UnwearItemType == 1)
            {
                Item unEquipCostume = null;
                if (packet.UnwearItem != null && packet.UnwearItem.ID > 0 && packet.UnwearItem.UID != "")
                    unEquipCostume = Item.CreateItem(packet.UnwearItem.ID, packet.UnwearItem.UID, 0, 0, 1, -1, 0);

                if (unEquipCostume != null)
                    charData.AddCostume(unEquipCostume);
            }

            UpdateItemInfos(privateData, true);
        }

        StorageWindow storageWindow = GameUI.Instance.storageWindow;
        if (storageWindow != null)
        {
            storageWindow.OnEquipResult(packet.slotIndex, GameDef.eItemSlotWindow.Costume, packet.errorCode);
        }
    }

    public void DoUnEquipCostumeItemProcess(string strPacket)
    {
        PacketUnwearCostumeItem packet = LitJson.JsonMapper.ToObject<PacketUnwearCostumeItem>(strPacket);

        if (packet.errorCode == NetErrorCode.OK)
        {
            int charIndex = connector.charIndex;
            CharInfoData charData = connector.charInfo;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

            if (privateData != null)
                privateData.RemoveEquipItem(packet.slotIndex, null);

            Item unEquipCostume = null;
            if (packet.UnwearItem != null && packet.UnwearItem.ID > 0 && packet.UnwearItem.UID != "")
                unEquipCostume = Item.CreateItem(packet.UnwearItem.ID, packet.UnwearItem.UID, 0, 0, 1, -1, 0);

            if (unEquipCostume != null)
                charData.AddCostume(unEquipCostume);

            UpdateItemInfos(privateData, true);
        }

        StorageWindow storageWindow = GameUI.Instance.storageWindow;
        if (storageWindow != null)
        {
            storageWindow.OnUnEquipResult(packet.slotIndex, packet.errorCode);
        }
    }

    public void DoUnEquipItemResponeProcess(string strPacket)
    {
        PacketDoUnEquipItemRespone packet = LitJson.JsonMapper.ToObject<PacketDoUnEquipItemRespone>(strPacket);

        if (packet.errorCode == NetErrorCode.OK)
        {
            CharPrivateData privateData = connector.charInfo.privateDatas[packet.CharacterIndex];
            if (privateData != null)
                privateData.RemoveEquipItem(packet.slotIndex, null);

            if (packet.Unequip != null)
            {
                foreach (ItemDBInfo dbInfo in packet.Unequip)
                {
                    Item unEquipItem = null;
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
            }

            UpdateItemInfos(privateData, true);
        }

        StorageWindow storageWindow = GameUI.Instance.storageWindow;
        if (storageWindow != null)
        {
            storageWindow.OnUnEquipResult(packet.slotIndex, packet.errorCode);
        }
    }

    public void ReinforceItemProcess(string strPacket)
    {
        PacketReinforceItem packet = LitJson.JsonMapper.ToObject<PacketReinforceItem>(strPacket);

        StorageWindow storageWindow = GameUI.Instance.storageWindow;

        if (packet.errorCode == NetErrorCode.OK)
        {
            connector.charInfo.SetGold(packet.totalGold, packet.totalCash);

            CharPrivateData privateData = connector.charInfo.privateDatas[packet.CharacterIndex];
            Item resultItem = connector.charInfo.ReinforceItem(privateData, packet.DelItemUIDs, packet.tradeInfo, packet.reinforceStep);
            int newSlotIndex = connector.charInfo.FindSlotIndex(resultItem, privateData, packet.tradeInfo.windowType);

            UpdateItemInfos(privateData, false);

            if (storageWindow != null)
                storageWindow.UpdateWindow();

            if (storageWindow != null && storageWindow.reinforceWindow != null)
                storageWindow.reinforceWindow.UpdateReinforceItem(resultItem, newSlotIndex, packet.tradeInfo.windowType);

            //아이템 강화 업적.
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eReinforceItem, 0);

            if (resultItem.itemGrade == Item.limitCompositionStep && packet.reinforceStep == Item.limitReinforceStep)
            {
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUpgradeToLimit, 0);
                Game.Instance.SendUpdateAchievmentInfo();
            }
        }


        if (storageWindow != null && storageWindow.reinforceWindow != null)
        {
            storageWindow.reinforceWindow.OnReinforceResult(packet.errorCode, ref packet.tradeInfo, packet.reinforceStep);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void ReinforceItemExProcess(string strPacket)
    {
        PacketReinforceItemEx packet = LitJson.JsonMapper.ToObject<PacketReinforceItemEx>(strPacket);

        StorageWindow storageWindow = GameUI.Instance.storageWindow;

        BaseTradeItemInfo baseTradeInfo = null;
        if (packet.errorCode == NetErrorCode.OK)
        {
            connector.charInfo.SetGold(packet.totalGold, packet.totalCash);

            baseTradeInfo = new BaseTradeItemInfo();
            baseTradeInfo.UID = packet.UID;
            baseTradeInfo.ItemID = packet.ItemID;
            baseTradeInfo.slotIndex = packet.slotIndex;
            baseTradeInfo.windowType = packet.windowType;

            CharPrivateData privateData = connector.charInfo.privateDatas[packet.CharacterIndex];
            Item resultItem = connector.charInfo.ReinforceItem(privateData, packet.DelItemUIDs, baseTradeInfo, packet.reinforceStep);
            int newSlotIndex = connector.charInfo.FindSlotIndex(resultItem, privateData, packet.windowType);

            UpdateItemInfos(privateData, false);

            if (storageWindow != null)
                storageWindow.UpdateWindow();

            if (storageWindow != null && storageWindow.reinforceWindow != null)
                storageWindow.reinforceWindow.UpdateReinforceItem(resultItem, newSlotIndex, packet.windowType);

            //아이템 강화 업적.
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eReinforceItem, 0);

            if (resultItem.itemGrade == Item.limitCompositionStep && packet.reinforceStep == Item.limitReinforceStep)
            {
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUpgradeToLimit, 0);
                Game.Instance.SendUpdateAchievmentInfo();
            }
        }


        if (storageWindow != null && storageWindow.reinforceWindow != null)
        {
            storageWindow.reinforceWindow.resultItemExp = (uint)packet.exp;
            storageWindow.reinforceWindow.OnReinforceResult(packet.errorCode, ref baseTradeInfo, packet.reinforceStep);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void CompositionItemProcess(string strPacket)
    {
        PacketCompositionItem packet = LitJson.JsonMapper.ToObject<PacketCompositionItem>(strPacket);

        StorageWindow storageWindow = GameUI.Instance.storageWindow;

        if (packet.errorCode == NetErrorCode.OK ||
            packet.errorCode == NetErrorCode.CompositionFailed)
        {
            connector.charInfo.SetGold(packet.totalGold, packet.totalCash);

            CharPrivateData privateData = connector.charInfo.privateDatas[packet.CharacterIndex];
            Item resultItem = connector.charInfo.CompositionItem(privateData, packet.DelItemUID, packet.materialUID, packet.materialCount, packet.Info, packet.Grade);
            int newSlotIndex = connector.charInfo.FindSlotIndex(resultItem, privateData, packet.Info.windowType);

            UpdateItemInfos(privateData, false);

            if (storageWindow != null)
                storageWindow.UpdateWindow();

            if (storageWindow != null && storageWindow.compositionWindow != null)
                storageWindow.compositionWindow.UpdateCompositionItem(resultItem, newSlotIndex, packet.Info.windowType);
        }

        if (packet.errorCode == NetErrorCode.OK)
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eCompositionSuccess, 0);
        else
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eCompositionFail, 0);

        if (storageWindow != null && storageWindow.compositionWindow != null)
        {
            storageWindow.compositionWindow.OnCompositionResult(packet.Info.slotIndex, packet.errorCode, packet.Grade);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void CompositionItemExProcess(string strPacket)
    {
        PacketCompositionItemEx packet = LitJson.JsonMapper.ToObject<PacketCompositionItemEx>(strPacket);

        StorageWindow storageWindow = GameUI.Instance.storageWindow;

        if (packet.errorCode == NetErrorCode.OK ||
            packet.errorCode == NetErrorCode.CompositionFailed)
        {
            connector.charInfo.SetGold(packet.totalGold, packet.totalCash);

            CharPrivateData privateData = connector.charInfo.privateDatas[packet.CharacterIndex];
            BaseTradeItemInfo baseTradeInfo = new BaseTradeItemInfo();
            baseTradeInfo.UID = packet.UID;
            baseTradeInfo.ItemID = packet.ItemID;
            baseTradeInfo.slotIndex = packet.slotIndex;
            baseTradeInfo.windowType = packet.windowType;

            Item resultItem = connector.charInfo.CompositionItem(privateData, packet.DelItemUID, packet.materialUID, packet.materialCount, baseTradeInfo, packet.Grade);
            //resultItem.SetExp(packet.Exp);

            int newSlotIndex = connector.charInfo.FindSlotIndex(resultItem, privateData, packet.windowType);

            UpdateItemInfos(privateData, false);

            if (storageWindow != null)
                storageWindow.UpdateWindow();

            if (storageWindow != null && storageWindow.compositionWindow != null)
            {
                storageWindow.compositionWindow.resultItemExp = (uint)packet.Exp;
                storageWindow.compositionWindow.UpdateCompositionItem(resultItem, newSlotIndex, packet.windowType);
            }
        }

        if (packet.errorCode == NetErrorCode.OK)
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eCompositionSuccess, 0);
        else
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eCompositionFail, 0);

        if (storageWindow != null && storageWindow.compositionWindow != null)
        {
            storageWindow.compositionWindow.OnCompositionResult(packet.slotIndex, packet.errorCode, packet.Grade);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    void BuyCashItemProcess(string strPacket)
    {
        PacketBuyCashItem packet = LitJson.JsonMapper.ToObject<PacketBuyCashItem>(strPacket);

        if (packet.errorCode == NetErrorCode.OK)
        {
            connector.charInfo.SetGold(packet.Gold, packet.Cash);

            int nCount = Mathf.Min(packet.UIDs.Length, packet.counts.Length);
            string UID = "";
            int itemCount = 0;
            for (int index = 0; index < nCount; ++index)
            {
                UID = packet.UIDs[index];
                itemCount = packet.counts[index];

                Item newItem = Item.CreateItem(packet.itemID, UID, packet.grade, packet.reinforce, itemCount, packet.rate, packet.exp);
                if (newItem != null)
                    newItem.IsNewItem = true;

                connector.charInfo.AddItem(newItem);
            }

            if (GameUI.Instance.myCharInfos != null)
                GameUI.Instance.myCharInfos.UpdateCoinInfo();
        }

        CashShopWindow cashWindow = GameUI.Instance.cashShopWindow;

        if (cashWindow != null)
        {
            cashWindow.OnResult(packet.errorCode, packet.cashID);
        }
    }

    void RequestBuyCashItemProcess(string strPacket)
    {
        PacketRequestBuyCashItem info = LitJson.JsonMapper.ToObject<PacketRequestBuyCashItem>(strPacket);

        if (info.errorCode == NetErrorCode.OK)
        {
            TStoreCashItemInfo item = new TStoreCashItemInfo();

            item.ItemID = info.ItemID;
            item.TStoreProductCode = info.TStoreProductCode;
            item.TStoreTID = info.TStoreTID;
            item.Price = info.Price;
            item.publisherType = (int)info.Store;

            item.itemName = info.itemName;

            // 안드로이드에 아이템 구매요청한다.
            Game.Instance.androidManager.OnClickBuyCashItem(item);

            return;
        }

        BaseCashShopWindow baseCashShopWindow = null;

        TableManager tableManager = TableManager.Instance;
        CashShopInfoTable cashInfoTable = tableManager != null ? tableManager.cashShopInfoTable : null;
        CashItemInfo cashItemInfo = null;
        if (cashInfoTable != null)
            cashItemInfo = cashInfoTable.GetItemInfo(info.ItemID);


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
            baseCashShopWindow.OnResult(info.errorCode, info.ItemID);
        else
        {
            if (GameUI.Instance.MessageBox != null)
            {
                string errorMsgStr = "Failed Buy CashItem!!";
                StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
                if (stringTable != null)
                    errorMsgStr = stringTable.GetData((int)info.errorCode);

                GameUI.Instance.MessageBox.SetMessage(errorMsgStr);
            }
        }
        // todo.
    }

    public void ResponeBuyCashItemProcess(string packet)
    {
        //T스토어 결제는 성공했는데 우리게임에 반영이 실패했음.
        // 캐쉬창닫고 오류를 보여주자.
        // todo.
    }
    public void GambleInfoProcess(string packet)
    {
        PacketGambleInfo pd = LitJson.JsonMapper.ToObject<PacketGambleInfo>(packet);

        if (pd.errorCode == NetErrorCode.OK)
        {
            System.DateTime newExpireTime = GambleWindow.refreshExpireTime;

            CharPrivateData privateData = null;
            CharInfoData charData = connector.charInfo;
            if (charData != null)
                privateData = charData.GetPrivateData(connector.charIndex);

            if (privateData != null)
            {
                if (pd.Info.LeftTimeSec >= 0)
                    privateData.SetGambleTime(pd.Info.LeftTimeSec);

                privateData.SetGambleInfo(pd.Info.Items, pd.LeftTimeSec, pd.eventItemIDs);

                Logger.DebugLog("Recv GambleInfo LeftTimeSec:" + pd.Info.LeftTimeSec.ToString());

                newExpireTime = privateData.refreshGambleExpireTime;

                GambleWindow.refreshExpireTime = newExpireTime;
            }

            GameUI.Instance.townUI.OnGambleWindowOpen();
        }
    }

    public void RequestGambleRefresh(string packet)
    {
        PacketRequestGambleRefresh pd = LitJson.JsonMapper.ToObject<PacketRequestGambleRefresh>(packet);

        if (pd.errorCode == NetErrorCode.OK)
        {
            System.DateTime newExpireTime = GambleWindow.refreshExpireTime;

            CharPrivateData privateData = null;
            CharInfoData charData = connector.charInfo;
            if (charData != null)
                privateData = charData.GetPrivateData(connector.charIndex);

            if (privateData != null)
            {
                if (pd.LeftTimeSec >= 0)
                    privateData.SetGambleTime(pd.LeftTimeSec);

                Logger.DebugLog("Recv GambleRefresh LeftTimeSec:" + pd.LeftTimeSec.ToString());

                newExpireTime = privateData.refreshGambleExpireTime;

                GambleWindow.refreshExpireTime = newExpireTime;
                GambleWindow.isRequestedByServer = true;
            }
        }
    }

    public void GambleRefreshProcess(string packet)
    {
        PacketGambleRefreshRespone pd = LitJson.JsonMapper.ToObject<PacketGambleRefreshRespone>(packet);

        if (pd.errorCode == NetErrorCode.OK)
        {
            CharPrivateData privateData = null;
            CharInfoData charData = connector.charInfo;
            if (charData != null)
            {
                charData.SetGold(pd.Gold, pd.Cash);
                privateData = charData.GetPrivateData(connector.charIndex);
            }

            if (privateData != null)
            {
                if (pd.LeftTimeSec >= 0)
                    privateData.SetGambleTime(pd.LeftTimeSec);

                if (pd.eventLeftTimeSec > 0)
                {
                    privateData.gambleEventEndTime = System.DateTime.Now + Game.ToTimeSpan(pd.eventLeftTimeSec);

                    privateData.gambleEventItemIDs.Clear();
                    foreach (int itemID in pd.eventItemIDs)
                        privateData.gambleEventItemIDs.Add(itemID);
                }

                GambleWindow gambleWindow = GameUI.Instance.gambleWindow;
                if (gambleWindow != null)
                    gambleWindow.UpdateCoinInfo(true);

                GambleWindow.refreshExpireTime = privateData.refreshGambleExpireTime;
            }
        }

        GambleWindow.bSendRefresh = false;
    }

    public void SupplyGambleItemProcess(string packet)
    {
        PacketSupplyGambleItem pd = LitJson.JsonMapper.ToObject<PacketSupplyGambleItem>(packet);

        Item newItem = null;

        GambleWindow gambleWindow = GameUI.Instance.gambleWindow;

        if (pd.errorCode == NetErrorCode.OK)
        {
            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUseGamble, 0);

            newItem = Item.CreateItem(pd.ItemID, pd.UID, pd.Grade, 0, 1, pd.itemRate, pd.exp);
            if (newItem != null)
                newItem.IsNewItem = true;

            if (pd.CostumeItem)
            {
                connector.charInfo.AddCostume(newItem);
            }
            else
            {
                connector.charInfo.AddItem(newItem);
            }

            UpdateItemInfos(null, true);

            connector.charInfo.SetGold(pd.Gold, pd.Cash);
            connector.charInfo.gambleCoupon = pd.coupon;

            if (gambleWindow != null)
            {
                gambleWindow.UpdateCoinInfo(true);
                gambleWindow.UpdateCouponCount();
            }
        }

        if (gambleWindow != null)
            gambleWindow.StartGambleProgress(pd.errorCode, newItem, pd.GambleIndex);

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void OnChangeGambleItem(string packet)
    {
        PacketChangeGambleItem pd = LitJson.JsonMapper.ToObject<PacketChangeGambleItem>(packet);

        if (pd.errorCode == NetErrorCode.OK)
        {
            GambleWindow gambleWindow = GameUI.Instance.gambleWindow;
            if (gambleWindow != null)
            {

                Item newItem = Item.CreateItem(pd.Item.ID, "", pd.Item.Grade, 0, 1, pd.Item.itemRate, 0);

                //////////////////////////////////////////////////////////////////
                //GambleData Update....
                CharPrivateData privateData = null;
                CharInfoData charData = connector.charInfo;
                if (charData != null)
                    privateData = charData.GetPrivateData(connector.charIndex);

                int nCount = 0;
                if (privateData != null && privateData.gambleItemList != null)
                    nCount = privateData.gambleItemList.Count;

                if (pd.GambleIndex >= 0 && pd.GambleIndex < nCount)
                    privateData.gambleItemList[pd.GambleIndex] = pd.Item;
                ///////////////////////////////////////////////////////////////////

                gambleWindow.ChangeGambleItem(pd.GambleIndex, newItem);
            }
        }
    }

    public void RequestGambleOpenProcess(string strPacket)
    {
        //PacketRequestGambleInfo packet = LitJson.JsonMapper.ToObject<PacketRequestGambleInfo>(strPacket);

        TownUI townUI = GameUI.Instance.townUI;

        if (townUI != null)
        {
            townUI.OnGambleWindowOpen();
        }
    }

    public void WaveInfoProcess(string packet)
    {
        PacketWaveInfo pd = LitJson.JsonMapper.ToObject<PacketWaveInfo>(packet);
        TownUI townUI = GameUI.Instance.townUI;

        if (pd.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = Game.Instance.charInfoData;
            int charIndex = -1;
            if (Game.Instance.connector != null)
                charIndex = Game.Instance.connector.charIndex;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

            WaveRankingInfo waveInfo = null;
            if (privateData != null)
            {
                privateData.SetWaveRankInfo(pd.Ranking, pd.RecordStep, pd.RecordSec);
                waveInfo = privateData.waveInfo;
            }

            if (waveInfo != null && pd.Clear == 1)
            {
                TableManager tableManager = TableManager.Instance;
                StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
                int maxWaveCount = 64;
                if (stringValueTable != null)
                    maxWaveCount = stringValueTable.GetData("MaxWaveStep");

                waveInfo.RecordStep = maxWaveCount;
            }

            townUI.OnWaveWindow(waveInfo, pd.rankingList, pd.RewardLeftTimeSec, pd.Clear, pd.Open);
        }
        else
        {
            townUI.requestCount = 0;
        }
    }

    public void WaveStartProcess(string packet)
    {
        PacketWaveStartRespone pd = LitJson.JsonMapper.ToObject<PacketWaveStartRespone>(packet);

        if (pd.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = connector.charInfo;
            CharPrivateData privateData = null;

            if (charData != null)
            {
                charData.SetGold(pd.TotalGold, pd.TotalCash);

                charData.potion1 = pd.potion1;
                charData.potion2 = pd.potion2;

                privateData = charData.GetPrivateData(pd.CharacterIndex);
            }

            if (privateData != null)
            {
                privateData.SetStamina(pd.StaminaLeftTimeSec, pd.StaminaCur, pd.StaminaPresent);
            }

            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eDefenceEnter, 0);
        }

        WaveStartWindow waveStartWindow = GameUI.Instance.waveStartWindow;
        if (waveStartWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
                waveStartWindow.OnStart(pd.SelectedBuffs, pd.SelectedTower, true);
            else
                waveStartWindow.OnErrorMessage(pd.errorCode, waveStartWindow);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void WaveEndProcess(string packet)
    {
        PacketWaveEndRespone pd = LitJson.JsonMapper.ToObject<PacketWaveEndRespone>(packet);

        Item newItem = null;

        int maxStep = pd.WaveStep;
        int clearTime = pd.DurationSec;

        if (pd.errorCode == NetErrorCode.OK)
        {
            switch (pd.invenType)
            {
                case InvenType.Normal:
                case InvenType.Costume:
                case InvenType.Material:
                    newItem = Item.CreateItem(pd.ItemID, pd.UID, pd.Grade, 0, pd.Count, pd.Rate, pd.exp);
                    if (newItem != null)
                        newItem.IsNewItem = true;

                    switch (pd.invenType)
                    {
                        case InvenType.Normal:
                            connector.charInfo.AddItem(newItem);
                            break;
                        case InvenType.Costume:
                            connector.charInfo.AddCostume(newItem);
                            break;
                        case InvenType.Material:
                            connector.charInfo.AddMaterial(newItem);
                            break;
                    }
                    break;
                case InvenType.CostumeSet:
                    CostumeSetItem setItem = CostumeSetItem.Create(pd.ItemID, pd.UID);
                    connector.charInfo.AddCostumeSetItem(setItem);
                    break;
            }

            int charIndex = pd.CharacterIndex;

            CharInfoData charData = connector != null ? connector.charInfo : null;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

            if (privateData != null)
            {
                privateData.SetWaveClearInfo(pd.WaveStep, pd.DurationSec);

                maxStep = privateData.waveInfo.RecordStep;
                clearTime = privateData.waveInfo.RecordSec;

                if (pd.updateItems != null)
                {
                    int nCount = pd.updateItems.Length;
                    privateData.SetEquipItemList(nCount, pd.updateItems);//, pd.costumeSetItem);
                }
            }
        }

        if (GameUI.Instance.waveManager != null)
            GameUI.Instance.waveManager.ShowWaveEndWindow(true, newItem, pd.WaveStep, pd.DurationSec, maxStep, clearTime);
    }

    public void WaveContinueProcess(string packet)
    {
        PacketWaveContinue pd = LitJson.JsonMapper.ToObject<PacketWaveContinue>(packet);

        WaveWindow waveWindow = GameUI.Instance.waveWindow;

        if (waveWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                Game.Instance.ApplyAchievement(Achievement.eAchievementType.eDefenceEnter, 0);

                connector.charInfo.SetGold(pd.TotalGold, pd.TotalCash);

                waveWindow.OnWaveContinue(pd.StartStep, pd.StartSec, pd.SelectedBuffs, pd.SelectedTower);
            }
            else
                waveWindow.OnErrorMessage(pd.errorCode, null);
        }
    }

    public void WaveRankingProcess(string packet)
    {
        PacketWaveRanking pd = LitJson.JsonMapper.ToObject<PacketWaveRanking>(packet);
        WaveWindow waveWindow = GameUI.Instance.waveWindow;

        if (waveWindow != null)
        {
            waveWindow.RefreshRankList(pd.errorCode, pd.rankingList, pd.bDown);
        }
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

    void AddNewItem(CharInfoData charInfo, MaterialItemDBInfo dbInfo)
    {
        AddNewItem(charInfo, dbInfo, false);
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

    void AddNewItem(CharInfoData charInfo, CostumeItemDBInfo dbInfo, bool isNewItem)
    {
        if (charInfo == null || dbInfo == null)
            return;

        Item newItem = Item.CreateItem(dbInfo.ID, dbInfo.UID, 0, 0, 1);
        if (newItem != null)
            newItem.IsNewItem = isNewItem;

        AddNewItem(charInfo, newItem);
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
                //case ItemInfo.eItemType.Material:
                //case ItemInfo.eItemType.Material_Compose:
                //	connector.charInfo.AddMaterial(newItem);
                //	break;
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

    void StageResultProcess(string packet)
    {
        PacketStageResult pd = LitJson.JsonMapper.ToObject<PacketStageResult>(packet);

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

                ItemDBInfo dbInfo = null;

                int nCount = pd.gainNormalItems.Length;
                for (int index = 0; index < nCount; ++index)
                {
                    dbInfo = pd.gainNormalItems[index];
                    AddNewItem(charData, dbInfo, true);
                }

                nCount = pd.gainMaterialItems.Length;
                MaterialItemDBInfo materialItemDBInfo = null;
                for (int index = 0; index < nCount; ++index)
                {
                    materialItemDBInfo = pd.gainMaterialItems[index];
                    AddNewItem(charData, materialItemDBInfo, true);
                }

                dbInfo = pd.rewardItemInfo;
                if (dbInfo != null)
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
                    privateData.baseInfo.StaminaCur = privateData.baseInfo.StaminaMax;

                if (pd.updateItems != null)
                {
                    int nCount = pd.updateItems.Length;
                    privateData.SetEquipItemList(nCount, pd.updateItems);//, pd.costumeSetItem);
                }
            }
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
			int[] rewardPrices = {2,4,6,8,10};
            stageEndEvent.OnStageEndResult(pd.errorCode,
                                            pd.CharacterIndex, pd.ClearStageIndex, pd.StageType,
                                            pd.rewardItemInfo, (long)pd.rewardEXP, pd.totalEXP,
                                            pd.rewardIndex, items, pd.rewardMeat, pd.rewardGold,
                                            pd.rewardMaterialItemID, rewardPrices);
        }
    }

    void StageRewardProcess(string packet)
    {
        PacketStageReward pd = LitJson.JsonMapper.ToObject<PacketStageReward>(packet);
        if (pd.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = connector != null ? connector.charInfo : null;

            if (charData != null)
            {
                charData.SetGold(pd.totalGold, pd.totalCash);

                ItemDBInfo dbInfo = pd.rewardItemInfo;
                if (dbInfo != null)
                    AddNewItem(charData, dbInfo, true);
            }
        }

        StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;
        if (stageEndEvent != null)
        {
            stageEndEvent.RewardAgain(pd.rewardIndex, pd.totalCash);
        }
    }

    public void StageTutorialProcess(string packetStr)
    {
        PacketStageTutorial pd = LitJson.JsonMapper.ToObject<PacketStageTutorial>(packetStr);


        long totalExp = 0L;

        if (pd.errorCode == NetErrorCode.OK)
        {
            int charIndex = pd.CharacterIndex;

            CharInfoData charData = connector != null ? connector.charInfo : null;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

            if (charData != null)
            {
                AddNewItem(charData, pd.item, true);

                if (pd.gainMaterialItems != null)
                {
                    foreach (MaterialItemDBInfo dbInfo in pd.gainMaterialItems)
                    {
                        AddNewItem(charData, dbInfo, true);
                    }
                }

                if (pd.gainNormalItems != null)
                {
                    foreach (ItemDBInfo dbInfo in pd.gainNormalItems)
                    {
                        AddNewItem(charData, dbInfo, true);
                    }
                }
            }

            if (privateData != null)
            {
                privateData.baseInfo.ExpValue += pd.rewardEXP;

                totalExp = privateData.baseInfo.ExpValue;
            }
        }

        StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;
        //if (stageEndEvent != null)
        //	stageEndEvent.OnStageEndResult(pd.errorCode, pd.CharacterIndex, -1, -1, pd.item, pd.rewardEXP, totalExp, 0, rewardList);
    }

    public void TownTutorialDoneProcess(string packetStr)
    {
        PacketTutorialDone pd = LitJson.JsonMapper.ToObject<PacketTutorialDone>(packetStr);

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
    }

    public void StageEndFailedProcess(string packet)
    {
        PacketStageEndFailed pd = LitJson.JsonMapper.ToObject<PacketStageEndFailed>(packet);

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
    }

    void StageStartProcess(string packet)
    {
        PacketStageStart pd = LitJson.JsonMapper.ToObject<PacketStageStart>(packet);

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
    }

    void RecoveryStaminaByStage(string packet)
    {
        PacketStageStartRecoveryStamina pd = LitJson.JsonMapper.ToObject<PacketStageStartRecoveryStamina>(packet);

        MapStartWindow mapStartWindow = GameUI.Instance.mapStartWindow;
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
            {
                privateData.SetStamina(pd.LeftTimeSec, pd.curStamina, pd.presentStamina);
            }

            if (mapStartWindow != null)
                mapStartWindow.OnStart(pd.StageIndex, pd.StageType, pd.SelectedBuffs);
        }
        else
        {
            if (mapStartWindow != null)
                mapStartWindow.OnErrorMessage(pd.errorCode, mapStartWindow);
        }
    }

    void RecoveryStaminaByWave(string packet)
    {
        PacketWaveStartRecoveryStamina pd = LitJson.JsonMapper.ToObject<PacketWaveStartRecoveryStamina>(packet);

        if (pd.errorCode == NetErrorCode.OK)
        {
            connector.charInfo.SetGold(pd.Gold, pd.Cash);
            CharPrivateData privateData = connector.charInfo.GetPrivateData(pd.CharacterIndex);
            if (privateData != null)
                privateData.SetStamina(0, pd.CurStamina, pd.PresentStamina);
        }

        if (pd.bWave == true)
        {
            WaveStartWindow waveStartWindow = GameUI.Instance.waveStartWindow;
            if (waveStartWindow != null)
            {
                if (pd.errorCode == NetErrorCode.OK)
                    waveStartWindow.OnStart(pd.SelectedBuffs, pd.SelectedTower, pd.bStart);
                else
                    waveStartWindow.OnErrorMessage(pd.errorCode, waveStartWindow);
            }
        }
    }

    public void SkillUpgrade(string packet)
    {
        PacketUpgradeSkill pd = LitJson.JsonMapper.ToObject<PacketUpgradeSkill>(packet);

        MasteryWindow_New masteryWindow = GameUI.Instance.masteryWindow;
        if (pd.errorCode == NetErrorCode.OK)
        {
            CharPrivateData privateData = connector.charInfo.GetPrivateData(pd.CharacterIndex);
            if (privateData != null)
            {
                int nCount = Mathf.Min(pd.Info.SkillIDs.Length, pd.Info.Levels.Length);
                int skillID = 0;
                int skillLv = 0;
                for (int index = 0; index < nCount; ++index)
                {
                    skillID = pd.Info.SkillIDs[index];
                    skillLv = pd.Info.Levels[index];

                    privateData.SetMasteryData(skillID, skillLv);
                }

                privateData.baseInfo.SkillPoint = pd.SkillPoint;
            }

            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eUseSkillPoint, 0);

            if (masteryWindow != null)
                masteryWindow.OnResultApplyMastery(pd.Info);
        }
        else
        {
            if (masteryWindow != null)
                masteryWindow.OnErrorMessage(pd.errorCode, masteryWindow);
        }

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void ResetSkill(string packet)
    {
        PacketResetSkill pd = LitJson.JsonMapper.ToObject<PacketResetSkill>(packet);
        MasteryWindow_New masteryWindow = GameUI.Instance.masteryWindow;
        if (pd.errorCode == NetErrorCode.OK)
        {
            CharPrivateData privateData = connector.charInfo.GetPrivateData(pd.CharacterIndex);
            if (privateData != null)
            {
                privateData.baseInfo.SkillPoint = pd.SkillPoint;

                privateData.ResetMastery();
            }

            Game.Instance.ApplyAchievement(Achievement.eAchievementType.eResetSkillPoint, 0);

            connector.charInfo.SetGold(pd.TotalGold, pd.TotalCash);
        }

        if (masteryWindow != null)
            masteryWindow.OnResultResetMastery(pd.errorCode);

        Game.Instance.SendUpdateAchievmentInfo();
    }

    public void ArenaInfoProcess(string packet)
    {
        PacketArenaInfo pd = LitJson.JsonMapper.ToObject<PacketArenaInfo>(packet);
        TownUI townUI = GameUI.Instance.townUI;

        if (townUI != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = Game.Instance.charInfoData;
                int charIndex = -1;
                if (Game.Instance.connector != null)
                    charIndex = Game.Instance.connector.charIndex;
                CharPrivateData privateData = charData != null ? charData.GetPrivateData(charIndex) : null;

                if (privateData != null)
                    privateData.SetArenaInfo(pd.Info);

                townUI.OnArenaWindow(pd.Info, pd.rankingList, pd.RewardLeftTimeSec, pd.Open);
            }
            else
            {
                townUI.requestCount = 0;
            }
        }
    }

    public void ArenaMatchingTargetProcess(string packet)
    {
        PacketArenaMatchingTarget pd = LitJson.JsonMapper.ToObject<PacketArenaMatchingTarget>(packet);
        ArenaWindow arenaWindow = GameUI.Instance.arenaWindow;

        if (pd.errorCode == NetErrorCode.OK)
        {
            pd.TargetExp = long.Parse(pd.targetExpStr);
            pd.targetAwakenExp = long.Parse(pd.targetAwakenExpStr);

            if (pd.TargetExp < 0L)
                pd.TargetExp = 0L;
            if (pd.targetAwakenExp < 0L)
                pd.targetAwakenExp = 0L;

            CharInfoData charData = Game.Instance.charInfoData;
            if (charData != null)
            {
                charData.SetGold(pd.TotalGold, pd.TotalCash);
                charData.ticket = pd.Ticket;
            }
                
            int charIndex = -1;
            if (Game.Instance.connector != null)
                charIndex = Game.Instance.connector.charIndex;
        }

        if (arenaWindow != null)
            arenaWindow.LoadArena(pd);
    }

    public void ArenaRankingProcess(string packet)
    {
        PacketArenaRanking pd = LitJson.JsonMapper.ToObject<PacketArenaRanking>(packet);
        ArenaWindow arenaWindow = GameUI.Instance.arenaWindow;

        if (arenaWindow != null)
        {
            arenaWindow.RefreshRankList(pd.errorCode, pd.RankType, pd.rankingList, pd.bDown);
        }
    }

    public void ArenaResultProcess(string packet)
    {
        PacketArenaResult pd = LitJson.JsonMapper.ToObject<PacketArenaResult>(packet);
        StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;

        if (stageEndEvent != null)
        {
            CharPrivateData enemyPrivateData = Game.Instance.arenaTargetInfo;
            if (enemyPrivateData != null)
            {
                enemyPrivateData.arenaInfo.rankType = pd.TargetRankType;
                enemyPrivateData.arenaInfo.groupRanking = pd.TargetGroupRanking;
            }

            stageEndEvent.OnArenaResult(pd.CharacterIndex, pd.RewardLeftTimeSec, pd.ArenaInfo);
        }
    }

    public void ArenaRewardProcess(string packet)
    {
        PacketArenaReward pd = LitJson.JsonMapper.ToObject<PacketArenaReward>(packet);

        Game.Instance.AddReward(pd);
    }

    public void WaveRewardProcess(string packet)
    {
        PacketWaveReward pd = LitJson.JsonMapper.ToObject<PacketWaveReward>(packet);

        Game.Instance.AddReward(pd);
    }

    public void EnterTownEndProcess(string packet)
    {
        //PacketEnterTown pd = LitJson.JsonMapper.ToObject<PacketEnterTown>(packet);
        TownUI townUI = GameUI.Instance.townUI;

        if (townUI != null)
            townUI.OnEnterTown();

    }

    public void TargetEquipItemProcess(string packet)
    {
        PacketTargetEquipItem pd = LitJson.JsonMapper.ToObject<PacketTargetEquipItem>(packet);

        foreach (TargetInfoAll info in pd.Infos)
            info.Exp = int.Parse(info.expStr);

        TownUI.TargetDetailWindow(pd);
    }

    public void SupplyTicketProcess(string packet)
    {
        PacketSupplyTicket pd = LitJson.JsonMapper.ToObject<PacketSupplyTicket>(packet);

        CharInfoData charData = Game.Instance.charInfoData;
        CharPrivateData privateData = null;
        if (charData != null)
            charData.ticket = pd.Ticket;
    }

    public void MailListProcess(string packet)
    {
        PacketMailList pd = LitJson.JsonMapper.ToObject<PacketMailList>(packet);
        TownUI townUI = GameUI.Instance.townUI;

        if (townUI != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                Game.Instance.AddMailInfos(pd.Infos);

                if (pd.bContinue == 0)
                    townUI.OnPostWindow();
            }
            else
            {
                townUI.requestCount = 0;
            }
        }
    }

    public void PostItemProcess(string packet)
    {
        PacketPostItem pd = LitJson.JsonMapper.ToObject<PacketPostItem>(packet);
        PostWindow postWindow = GameUI.Instance.postWindow;
        if (postWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = Game.Instance.charInfoData;
                CharPrivateData privateData = null;
                if (charData != null)
                    privateData = charData.GetPrivateData(pd.CharacterIndex);

                if (charData != null)
                {
                    charData.SetGold(pd.totalGold, pd.totalJewel);
                    if (pd.totalCoupon >= 0)
                        charData.gambleCoupon = pd.totalCoupon;

                    if (pd.potion1Present >= 0)
                        charData.potion1Present = pd.potion1Present;
                    if (pd.potion2Present >= 0)
                        charData.potion2Present = pd.potion2Present;

                    if (pd.totalTicket >= 0)
                        charData.ticket = pd.totalTicket;
                }

                if (privateData != null && privateData.baseInfo != null)
                {
                    privateData.AddPresendStamina(pd.Stamina);
                }

                if (pd.Infos != null)
                {
                    foreach (ItemDBInfo dbInfo in pd.Infos)
                        AddNewItem(charData, dbInfo, true);
                }

                postWindow.SetRead(pd.mailIndex);

                postWindow.requestCount = 0;
            }
            else
            {
                postWindow.requestCount = 0;
                postWindow.OnErrorMessage(pd.errorCode, postWindow);
            }
        }
    }
    public void PostMsgProcess(string packet)
    {
        PacketPostMsg pd = LitJson.JsonMapper.ToObject<PacketPostMsg>(packet);
        PostWindow postWindow = GameUI.Instance.postWindow;
        if (postWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                postWindow.OnReadMessage(pd.title, pd.Msg);

                postWindow.SetRead(pd.mailIndex);

                postWindow.requestCount = 0;
            }
            else
            {
                postWindow.requestCount = 0;
                postWindow.OnErrorMessage(pd.errorCode, postWindow);
            }
        }
    }
    public void PostItemAllProcess(string packet)
    {
        PacketPostItemAll pd = LitJson.JsonMapper.ToObject<PacketPostItemAll>(packet);
        PostWindow postWindow = GameUI.Instance.postWindow;
        if (postWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                CharInfoData charData = Game.Instance.charInfoData;
                CharPrivateData privateData = null;
                if (charData != null)
                    privateData = charData.GetPrivateData(pd.CharacterIndex);

                if (charData != null)
                {
                    charData.SetGold(pd.totalGold, pd.totalJewel);

                    if (pd.totalCoupon >= 0)
                        charData.gambleCoupon = pd.totalCoupon;

                    if (pd.potion1Present >= 0)
                        charData.potion1Present = pd.potion1Present;
                    if (pd.potion2Present >= 0)
                        charData.potion2Present = pd.potion2Present;

                    if (pd.totalTicket >= 0)
                        charData.ticket = pd.totalTicket;

                }

                if (privateData != null && privateData.baseInfo != null)
                {
                    privateData.AddPresendStamina(pd.Stamina);
                }


                if (pd.normalItems != null)
                {
                    foreach (ItemDBInfo dbInfo in pd.normalItems)
                    {
                        AddNewItem(charData, dbInfo, true);
                    }
                }

                if (pd.costumeItems != null)
                {
                    foreach (CostumeItemDBInfo dbInfo in pd.costumeItems)
                    {
                        AddNewItem(charData, dbInfo, true);
                    }
                }

                if (pd.costumeSetItems != null)
                {
                    foreach (CostumeItemDBInfo dbInfo in pd.costumeSetItems)
                    {
                        CostumeSetItem addItem = CostumeSetItem.Create(dbInfo.ID, dbInfo.UID);
                        if (addItem != null)
                            charData.AddCostumeSetItem(addItem);
                    }
                }

                if (pd.materialItems != null)
                {
                    foreach (MaterialItemDBInfo dbInfo in pd.materialItems)
                    {
                        AddNewItem(charData, dbInfo, true);
                    }
                }

                postWindow.SetRead(pd.mailIndexs);
            }
            else
            {
                postWindow.requestCount = 0;
                postWindow.OnErrorMessage(pd.errorCode, postWindow);
            }
        }
    }

    public void RecommandFriendListProcess(string packet)
    {
        PacketRecommandFriendList pd = LitJson.JsonMapper.ToObject<PacketRecommandFriendList>(packet);
        FriendWindow friendWindow = GameUI.Instance.friendWindow;
        if (friendWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                Game.Instance.AddRecommandFirendList(pd.Friends);

                BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.InviteList);
                if (friendListWindow != null)
                    friendListWindow.SetInfos(pd.Friends);
            }
            else
            {
                friendWindow.OnErrorMessage(pd.errorCode, null);
            }
        }
    }

    public void FriendListProcess(string packet)
    {
        PacketFriendList pd = LitJson.JsonMapper.ToObject<PacketFriendList>(packet);
        FriendWindow friendWindow = GameUI.Instance.friendWindow;
        if (friendWindow != null)
        {
            int curCount = 0;

            if (pd.errorCode == NetErrorCode.OK)
            {
                Game.Instance.AddMyFriendList(pd.Friends);

                BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.FriendList);
                if (friendListWindow != null)
                    friendListWindow.SetInfos(pd.Friends);

                curCount = pd.Friends.Length;
            }
            else
            {
                friendWindow.OnErrorMessage(pd.errorCode, null);
            }

            friendWindow.SetMaxInfo(curCount);
        }
    }

    public void InvitedUserListProcess(string packet)
    {
        PacketInvitedUserList pd = LitJson.JsonMapper.ToObject<PacketInvitedUserList>(packet);
        FriendWindow friendWindow = GameUI.Instance.friendWindow;
        if (friendWindow != null)
        {
            friendWindow.requestCount = 0;

            if (pd.errorCode == NetErrorCode.OK)
            {
                Game.Instance.AddAcceptFriendList(pd.Friends);

                BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.AcceptList);
                if (friendListWindow != null)
                    friendListWindow.SetInfos(pd.Friends);
            }
            else
            {
                friendWindow.OnErrorMessage(pd.errorCode, null);
            }
        }
    }

    public void FriendInviteProcess(string packet)
    {
        PacketFriendInvite pd = LitJson.JsonMapper.ToObject<PacketFriendInvite>(packet);
        FriendWindow friendWindow = GameUI.Instance.friendWindow;
        if (friendWindow != null)
        {
            friendWindow.requestCount = 0;

            if (pd.errorCode == NetErrorCode.OK)
            {
                Game.Instance.RemoveRecommandFriend(pd.InvitedUserID);
            }
            else
            {
                friendWindow.OnErrorMessage(pd.errorCode, null);
            }
        }
    }

    public void FriendInviteAcceptProcess(string packet)
    {
        PacketFriendInviteAccept pd = LitJson.JsonMapper.ToObject<PacketFriendInviteAccept>(packet);
        FriendWindow friendWindow = GameUI.Instance.friendWindow;
        if (friendWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                Game.Instance.RemoveAcceptFriend(pd.Friend);
                Game.Instance.AddMyFriendList(pd.Info);
            }
            else
            {
                friendWindow.OnErrorMessage(pd.errorCode, null);
            }

            friendWindow.requestCount = 0;
        }
    }

    public void FriendInviteByNickNameProcess(string packet)
    {
        PacketFriendInviteByNickName pd = LitJson.JsonMapper.ToObject<PacketFriendInviteByNickName>(packet);
        FriendWindow friendWindow = GameUI.Instance.friendWindow;
        if (friendWindow != null)
        {
            FriendInviteWindow inviteWindow = friendWindow.GetInviteWindow();
            if (inviteWindow != null)
            {
                inviteWindow.OnErrorMessage(pd.errorCode);
            }

            friendWindow.requestCount = 0;
        }
    }

    public void SendStaminaToFriendProcess(string packet)
    {
        PacketSendStaminaToFriend pd = LitJson.JsonMapper.ToObject<PacketSendStaminaToFriend>(packet);
        FriendWindow friendWindow = GameUI.Instance.friendWindow;
        if (friendWindow != null)
        {
            if (pd.errorCode == NetErrorCode.OK)
            {
                BaseFriendListWindow friendListWindow = friendWindow.GetTabWindow(BaseFriendListWindow.eFriendListType.FriendList);
                if (friendListWindow != null)
                    friendListWindow.UpdateInfo(pd.FriendID, pd.coolTimeSec);
            }
            else
            {
                friendWindow.OnErrorMessage(pd.errorCode, friendWindow);
            }

            friendWindow.requestCount = 0;
        }
    }

    public void AchievementCompleteProcess(string packet)
    {
        Logger.DebugLog("Recv AchievementComplete:" + packet);
        PacketAchievementCompleteInfo pd = LitJson.JsonMapper.ToObject<PacketAchievementCompleteInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
        {
            foreach (AchievementClearInfo info in pd.Info)
            {
                achieveMgr.SetClearInfo(info);
            }
        }
    }
    public void AchievementInfoProcess(string packet)
    {
        PacketAchievementInfo pd = LitJson.JsonMapper.ToObject<PacketAchievementInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
        {
            foreach (AchievementDBInfo info in pd.Info)
            {
                achieveMgr.SetAchieveInfo(info);
            }
        }
    }
    public void AchievementProcess(string packet)
    {
        PacketAchievementProgress pd = LitJson.JsonMapper.ToObject<PacketAchievementProgress>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
            achieveMgr.SetAchieveInfo(pd.characterIndex, pd.groupIDs, pd.counts);

        AchievementWindow achieveWindow = GameUI.Instance.achievementWindow;
        if (achieveWindow != null)
        {
            achieveWindow.requestCount = 0;
            achieveWindow.UpdateInfo();
        }

    }
    public void AchievementRewardProcess(string packet)
    {
        PacketAchievementReward pd = LitJson.JsonMapper.ToObject<PacketAchievementReward>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
            achieveMgr.SetCompleteReward(pd.characterIndex, pd.groupID, pd.stepID);

        AchievementWindow achieveWindow = GameUI.Instance.achievementWindow;
        if (achieveWindow != null)
        {
            achieveWindow.requestCount = 0;
            achieveWindow.UpdateInfo();
        }
    }

    public void DailyMissionInfoProcess(string packet)
    {
        Logger.DebugLog("Recv DailyMission:" + packet);
        PacketDailyMissionInfo pd = LitJson.JsonMapper.ToObject<PacketDailyMissionInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        TableManager tableManager = TableManager.Instance;
        AchievementTable dailyAchievementTable = tableManager != null ? tableManager.dailyAchievementTable : null;

        if (achieveMgr != null)
        {
            achieveMgr.dailyAchivements.Clear();

            achieveMgr.dailyAchievementExpireTime = pd.expiredTime;

            int nCount = pd.Infos.Length;
            for (int index = 0; index < nCount; ++index)
            {
                DailyMission mission = pd.Infos[index];

                Achievement dailyAchieve = dailyAchievementTable.GetTempData(mission.id);

                if (dailyAchieve != null)
                {
                    dailyAchieve.curCount = mission.count;
                    dailyAchieve.isComplete = mission.bReward == 1;
                    achieveMgr.AddDailyAchievement(mission.id, dailyAchieve);
                }
            }
        }
    }
    public void DailyMissonProcess(string packet)
    {
        PacketDailyMissionProgress pd = LitJson.JsonMapper.ToObject<PacketDailyMissionProgress>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
            achieveMgr.SetDailyAchieve(pd.ids, pd.counts);

    }
    public void DailyMissionRewardProcess(string packet)
    {
        PacketDailyMissionReward pd = LitJson.JsonMapper.ToObject<PacketDailyMissionReward>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
            achieveMgr.SetCompleteDailyAchieve(pd.id);

        AchievementWindow achieveWindow = GameUI.Instance.achievementWindow;
        if (achieveWindow != null)
        {
            achieveWindow.requestCount = 0;
            achieveWindow.UpdateInfo();
        }
    }

    public void BossAppearProcess(string packet)
    {
        Logger.DebugLog("Recv BossAppear:" + packet);
        PacketBossAppear pd = LitJson.JsonMapper.ToObject<PacketBossAppear>(packet);
        Game.Instance.SetBossAppear(pd.id);
    }

    public void BossRaidInfoProcess(string packet)
    {
        PacketBossRaidInfo pd = LitJson.JsonMapper.ToObject<PacketBossRaidInfo>(packet);
        if (pd != null)
        {
            int nCount = pd.bossID.Length;
            List<BossRaidInfo> bossRaidInfos = new List<BossRaidInfo>();
            for (int index = 0; index < nCount; ++index)
            {
                BossRaidInfo newInfo = new BossRaidInfo();

                newInfo.index = pd.index[index];
                newInfo.bossID = pd.bossID[index];
                newInfo.leftSec = pd.destroyLeftSec[index];

                newInfo.isPhase2 = pd.transform[index] == 1;

                newInfo.finderName = pd.finder[index];

                newInfo.myDamage = pd.totalDamage[index];

                newInfo.topCharName = pd.topNick[index];
                newInfo.topCharDamage = pd.topDamage[index];

                newInfo.curHP = pd.HP[index];

                newInfo.isCleared = pd.die[index] == 1;
                newInfo.lastAttackerName = pd.hunter[index];

                bossRaidInfos.Add(newInfo);
            }

            TownUI townUI = GameUI.Instance.townUI;
            if (townUI != null)
                townUI.OnBossRaidWindow(bossRaidInfos);
        }
    }

    public void BossRaidStartProcess(string packet)
    {
        PacketBossRaidStart pd = LitJson.JsonMapper.ToObject<PacketBossRaidStart>(packet);
        if (pd != null)
        {
            BossRaidWindow bossRaidWindow = GameUI.Instance.bossRaidWindow;

            switch (pd.errorCode)
            {
                case NetErrorCode.OK:
                    CharInfoData charData = Game.Instance.charInfoData;
                    CharPrivateData privateData = null;
                    if (charData != null)
                        privateData = charData.GetPrivateData(pd.CharacterIndex);

                    if (privateData != null)
                    {
                        privateData.baseInfo.StaminaCur = pd.curStamina;
                        privateData.baseInfo.StaminaPresent = pd.presentStamina;
                    }

                    if (bossRaidWindow != null)
                        bossRaidWindow.OnBossRaidStart(pd.index, pd.transform == 1);
                    break;
                default:
                    if (bossRaidWindow != null)
                        bossRaidWindow.OnErrorMessage(pd.errorCode, bossRaidWindow);
                    break;
            }
        }
    }

    public void BossRaidStartRecoveryStaminaProcess(string packet)
    {
        PacketBossRaidStartRecoveryStamina pd = LitJson.JsonMapper.ToObject<PacketBossRaidStartRecoveryStamina>(packet);
        if (pd != null)
        {
            BossRaidWindow bossRaidWindow = GameUI.Instance.bossRaidWindow;

            switch (pd.errorCode)
            {
                case NetErrorCode.OK:
                    CharInfoData charData = Game.Instance.charInfoData;
                    CharPrivateData privateData = null;
                    if (charData != null)
                    {
                        charData.SetGold(pd.totalGold, pd.totalJewel);

                        privateData = charData.GetPrivateData(pd.CharacterIndex);
                    }

                    if (privateData != null)
                    {
                        privateData.baseInfo.StaminaCur = pd.curStamina;
                        privateData.baseInfo.StaminaPresent = pd.presentStamina;
                    }

                    if (bossRaidWindow != null)
                        bossRaidWindow.OnBossRaidStart(pd.index, pd.transform == 1);
                    break;
                default:
                    if (bossRaidWindow != null)
                        bossRaidWindow.OnErrorMessage(pd.errorCode, bossRaidWindow);
                    break;
            }
        }
    }

    public void BossRaidEndProcess(string packet)
    {
        PacketBossRaidEnd pd = LitJson.JsonMapper.ToObject<PacketBossRaidEnd>(packet);
        if (pd != null)
        {
            Game.Instance.bossRaidEnd = pd;

            StageEndEvent stageEndEvent = Game.Instance.stageManager.stageEndEvent;
            if (stageEndEvent != null)
            {
                stageEndEvent.CreateLoadingPanel();
            }
        }
    }

    public void UpdateStaminaProcess(string packet)
    {
        PacketUpdateStamina pd = LitJson.JsonMapper.ToObject<PacketUpdateStamina>(packet);
        if (pd.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = Game.Instance.charInfoData;
            CharPrivateData privateData = null;
            if (charData != null)
                privateData = charData.GetPrivateData(pd.CharacterIndex);

            if (privateData != null)
                privateData.baseInfo.StaminaCur = pd.curStamina;
        }
    }

    public void RevivalProcess(string packet)
    {
        PlayerController player = Game.Instance.player;
        RevivalController revivalController = player != null ? player.gameObject.GetComponent<RevivalController>() : null;

        PacketRevival pd = LitJson.JsonMapper.ToObject<PacketRevival>(packet);
        if (pd.errorCode == 0)
        {
            CharInfoData charData = Game.Instance.charInfoData;
            if (charData != null)
                charData.SetGold(pd.TotalGold, pd.TotalCash);

            if (revivalController != null)
                revivalController.OnRevivalSuccess();
        }
        else
        {
            if (revivalController != null)
                revivalController.OnRevivalFailed();
        }
    }

    public void TownBadgeNotifyProcess(string packetStr)
    {
        PacketBadgeNotify packet = LitJson.JsonMapper.ToObject<PacketBadgeNotify>(packetStr);
        int nCount = packet.windowTypes.Length;

        TownUI townUI = GameUI.Instance.townUI;
        if (townUI != null)
        {
            for (int index = 0; index < nCount; ++index)
            {
                int windowType = packet.windowTypes[index];
                int tabIndex = packet.Tab[index];

                townUI.SetBudgeNotify(windowType, tabIndex);
            }
        }
    }

    public void CouponProcess(string packetStr)
    {
        PacketCoupon packet = LitJson.JsonMapper.ToObject<PacketCoupon>(packetStr);
        CouponWindow couponWindow = GameUI.Instance.couponWindow;
        if (couponWindow != null)
            couponWindow.OnResult(packet.errorCode);
    }

    public void AttandanceProcess(string packetStr)
    {
        PacketAttandanceCheck packet = LitJson.JsonMapper.ToObject<PacketAttandanceCheck>(packetStr);

        CharInfoData charData = Game.Instance.charInfoData;
        if (charData != null)
            charData.attandanceCheck = packet.checkday;
    }

    public void ShowEventProcess(string packetStr)
    {
        PacketShowEvent packet = LitJson.JsonMapper.ToObject<PacketShowEvent>(packetStr);

        CharInfoData charData = Game.Instance.charInfoData;
        CharPrivateData privateData = null;
        if (charData != null)
            privateData = charData.GetPrivateData(packet.CharacterIndex);

        if (privateData != null)
            privateData.levelupRewardEventCheck = packet.Step;
    }

    public void GameReviewProcess(string packetStr)
    {
        PacketReviewPlease packet = LitJson.JsonMapper.ToObject<PacketReviewPlease>(packetStr);

        CharInfoData charData = Game.Instance.charInfoData;
        if (charData != null)
            charData.gameReviewURL = packet.url;
    }

    public void GameReviewRewardProcess(string packetStr)
    {
        PacketRequestReviewReward packet = LitJson.JsonMapper.ToObject<PacketRequestReviewReward>(packetStr);

        TownUI townUI = GameUI.Instance.townUI;
        if (townUI != null)
        {
            if (packet.errorCode != NetErrorCode.OK)
            {
                NoticePopupWindow warningPopup = GameUI.Instance.MessageBox;
                if (warningPopup != null)
                {
                    TableManager tableManager = TableManager.Instance;
                    StringTable stringTable = tableManager != null ? tableManager.stringTable : null;

                    string errorMsg = "ReviewReward Error";
                    if (stringTable != null)
                        errorMsg = stringTable.GetData((int)packet.errorCode);

                    warningPopup.SetMessage(errorMsg);
                }
            }
        }
    }

    public void MemberSecessionProcess(string packetStr)
    {
        PacketDropout packet = LitJson.JsonMapper.ToObject<PacketDropout>(packetStr);

        OptionWindow optionWindow = GameUI.Instance.optionWindow;
        if (optionWindow != null)
        {
            if (packet.errorCode == NetErrorCode.OK)
                optionWindow.OnLogoutOK(null);
            else
                optionWindow.OnErrorMessage(packet.errorCode, null);
        }
    }

    public void IgnorePushProcess(string packetStr)
    {
        PacketIgnorePush packet = LitJson.JsonMapper.ToObject<PacketIgnorePush>(packetStr);

        GameOption.noticeToggle = packet.off == 1;
    }

    void EventShopInfoProcess(string packetStr)
    {
        PacketEventShopInfo packet = LitJson.JsonMapper.ToObject<PacketEventShopInfo>(packetStr);

        if (packet != null)
        {
            CharInfoData charData = Game.Instance.charInfoData;

            if (charData != null)
            {
                charData.eventShopInfos.Clear();

                System.DateTime nowTime = packet.Now;

                int nCount = packet.eventTypes.Length;
                for (int index = 0; index < nCount; ++index)
                {
                    int eventID = packet.eventTypes[index];

                    eCashEvent eventType = eCashEvent.None;
                    switch (eventID)
                    {
                        case 9:
                            eventType = eCashEvent.CashBonus;
                            break;
                        case 6:
                        case 7:
                        case 8:
                            eventType = eCashEvent.RandomBox;
                            break;
                        default:
                            break;
                    }
                    if (eventType == eCashEvent.None)
                        continue;

                    int buyCount = packet.buyCount[index];
                    int limitCount = packet.limitCount[index];
                    int leftTime = packet.leftTimes[index];

                    EventShopInfoData newInfo = new EventShopInfoData();
                    newInfo.SetCountInfo(buyCount, limitCount);
                    newInfo.SetLimitTimeInfo(leftTime, nowTime);
                    newInfo.eventType = eventType;
                    newInfo.eventID = eventID;

                    charData.SetEventShopInfo(eventID, newInfo);
                }
            }
        }
    }

    void BuyEventCashItemProcess(string packetStr)
    {
        PacketBuyCashLimitItem packet = LitJson.JsonMapper.ToObject<PacketBuyCashLimitItem>(packetStr);

        EventShopWindow eventShopWindow = GameUI.Instance.eventShopWindow;
        PackageItemShopWindow packageShopWindow = GameUI.Instance.packageItemShopWindow;

        if (packet.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = Game.Instance.charInfoData;

            if (charData != null)
            {
                charData.SetGold(packet.Gold, packet.Cash);

                int nCount = Mathf.Min(packet.UIDs.Length, packet.counts.Length);
                string UID = "";
                int itemCount = 0;
                for (int index = 0; index < nCount; ++index)
                {
                    Item newItem = Item.CreateItem(packet.itemID, UID, packet.grade, packet.reinforce, itemCount, packet.rate, packet.exp);
                    if (newItem != null)
                        newItem.IsNewItem = true;

                    charData.AddItem(newItem);
                }

                if (GameUI.Instance.myCharInfos != null)
                    GameUI.Instance.myCharInfos.UpdateCoinInfo();

                switch (packet.cashEventType)
                {
                    case 1:
                        EventShopInfoData eventInfo = charData.GetEventShopInfo(eCashEvent.CashBonus);
                        if (eventInfo != null)
                            eventInfo.SetCountInfo(packet.buyCount, packet.limitCount);
                        break;
                    case 2:
                        charData.SetPackageItemLimit(packet.limitCount);
                        charData.SetPackageItem(packet.cashID, packet.buyCount);
                        break;
                }

            }
        }

        BaseCashShopWindow baseCashShopWindow = null;
        switch (packet.cashEventType)
        {
            case 1:
                if (eventShopWindow != null)
                    eventShopWindow.SetLimitInfo(packet.buyCount, packet.limitCount);

                baseCashShopWindow = eventShopWindow;
                break;
            case 2:
                baseCashShopWindow = packageShopWindow;
                break;
        }

        if (baseCashShopWindow != null)
        {
            baseCashShopWindow.OnResult(packet.errorCode, packet.cashID);
        }
    }


    public void RecvEmptyProcess(string packetStr)
    {
        PacketRecvEmpty packet = LitJson.JsonMapper.ToObject<PacketRecvEmpty>(packetStr);

        //아무것도 안함..
    }

    public void SpecialMissionInfoProcess(string packet)
    {
        PacketSpecialMissionInfo pd = LitJson.JsonMapper.ToObject<PacketSpecialMissionInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        TableManager tableManager = TableManager.Instance;
        AchievementTable specialEventTable = tableManager != null ? tableManager.specialEventAchievementTable : null;

        if (achieveMgr != null)
        {
            achieveMgr.ClearSpecialAchievements();

            SpecialEventInfo newEventInfo = new SpecialEventInfo();
            System.TimeSpan eventTimeSpan = System.TimeSpan.MinValue;
            if (newEventInfo != null)
            {
                newEventInfo.eventStartTime = pd.startTime;
                newEventInfo.eventEndTime = pd.endTime;
                newEventInfo.eventID = pd.eventType;

                newEventInfo.eventBannerURL = pd.url;

                eventTimeSpan = pd.endTime - pd.now;
            }

            if (eventTimeSpan.TotalSeconds > 0)
                achieveMgr.InitSpecialAchievements();

            if (charData != null)
                charData.specialEventInfo = newEventInfo;

            if (pd.Info != null)
                achieveMgr.SetSpeicalAchieveInfo(pd.Info);
        }
    }

    public void SpecialMissionCompleteProcess(string packet)
    {
        PacketSpecialMissionCompleteInfo pd = LitJson.JsonMapper.ToObject<PacketSpecialMissionCompleteInfo>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
        {
            int nCount = Mathf.Min(pd.groupIDs.Length, pd.stepIDs.Length);
            for (int index = 0; index < nCount; ++index)
            {
                int groupID = pd.groupIDs[index];
                int step = pd.stepIDs[index];
                int charIndex = pd.characterIndexs[index];

                achieveMgr.SpecialAchievementComplete(charIndex, groupID, step);
            }
        }
    }
    public void SpecialMissionProgress(string packet)
    {
        PacketSpecialMissionProgress pd = LitJson.JsonMapper.ToObject<PacketSpecialMissionProgress>(packet);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        if (achieveMgr != null)
        {
            int nCount = Mathf.Min(pd.groupIDs.Length, pd.counts.Length);
            for (int index = 0; index < nCount; ++index)
            {
                int groupID = pd.groupIDs[index];
                int count = pd.counts[index];
                int charIndex = pd.characterIndex;

                achieveMgr.SpecialAchievementUpdate(charIndex, groupID, count);
            }
        }
    }

    public void SpecialMissionRewardProgress(string packetStr)
    {
        PacketSpecialMissionReward packet = LitJson.JsonMapper.ToObject<PacketSpecialMissionReward>(packetStr);
        CharInfoData charData = Game.Instance.charInfoData;
        AchievementManager achieveMgr = charData != null ? charData.achievementManager : null;

        AchievementWindow achieveWindow = GameUI.Instance.achievementWindow;

        if (packet.errorCode == NetErrorCode.OK)
        {
            if (achieveMgr != null)
                achieveMgr.SetCompleteSpecialAchive(packet.characterIndex, packet.groupID, packet.stepID);

            if (achieveWindow != null)
            {
                achieveWindow.requestCount = 0;
                achieveWindow.UpdateInfo();
            }
        }
        else
        {
            if (achieveWindow != null)
                achieveWindow.OnErrorMessage(packet.errorCode, null);
        }
    }

    public void TimeLimitItemInfoProcess(string packetStr)
    {
        PacketTimeLimitItemInfo packet = LitJson.JsonMapper.ToObject<PacketTimeLimitItemInfo>(packetStr);

        CharInfoData charData = Game.Instance.charInfoData;
        if (charData != null)
        {
            if (charData.timeLimitBuffList == null)
                charData.CreateTimeLimitBuffList();
            else
                charData.InitTimeLimitBuffList();

            int nCount = Mathf.Min(packet.leftTimes.Length, packet.ItemIDs.Length);
            if (nCount > 0)
            {
                System.DateTime nowTime = System.DateTime.Now;
                System.DateTime endTime;

                System.TimeSpan timeSpan;

                int itemID = -1;

                int leftSec = 0;
                for (int index = 0; index < nCount; ++index)
                {
                    leftSec = packet.leftTimes[index];
                    if (leftSec <= 0)
                        continue;

                    timeSpan = Game.ToTimeSpan(leftSec);
                    endTime = nowTime + timeSpan;

                    itemID = packet.ItemIDs[index];

                    AddTimeLimitBuffItem(charData, itemID, endTime);
                }
            }
        }
    }

    public void InvokeTimeLimitItemProcess(string packetStr)
    {
        PacketInvokeTimeLimitItem packet = LitJson.JsonMapper.ToObject<PacketInvokeTimeLimitItem>(packetStr);

        CharInfoData charData = Game.Instance.charInfoData;
        if (charData != null)
        {
            if (packet.leftTime <= 0)
                return;

            System.TimeSpan timeSpan = Game.ToTimeSpan(packet.leftTime);
            System.DateTime endTime = System.DateTime.Now + timeSpan;

            if (charData.timeLimitBuffList == null)
                charData.CreateTimeLimitBuffList();

            AddTimeLimitBuffItem(charData, packet.ItemID, endTime);

            TownUI townUI = GameUI.Instance.townUI;
            if (townUI != null)
                townUI.UpdateLimitTimeBuff(charData);
        }
    }

    public void AddTimeLimitBuffItem(CharInfoData charData, int itemID, System.DateTime endTime)
    {
        TableManager tableManager = TableManager.Instance;
        TimeLimitItemTable timeLimitItemTable = tableManager != null ? tableManager.timeLimitItemTable : null;

        if (charData == null || timeLimitItemTable == null)
            return;

        TimeLimitBuffItemInfo buffItemInfo = timeLimitItemTable.GetData(itemID);
        if (buffItemInfo == null)
            return;

        foreach (TimeLimitBuffInfo buffInfo in buffItemInfo.buffList)
        {
            charData.AddTimeLimitBuff(endTime, buffInfo.buffType, buffItemInfo.type, buffInfo.buffValue);
        }
    }

    public void AwakeningSkillInfoProcess(string packetStr)
    {
        PacketAwakeningInfo packet = LitJson.JsonMapper.ToObject<PacketAwakeningInfo>(packetStr);

        CharInfoData charData = connector.charInfo;
        CharPrivateData privateData = charData != null ? charData.GetPrivateData(packet.CharacterIndex) : null;

        if (privateData != null && packet.Info != null)
        {
            int nCount = Math.Min(packet.Info.IDs.Length, packet.Info.Lvs.Length);
            int skillID = 0;
            int skillLv = 0;

            for (int index = 0; index < nCount; ++index)
            {
                skillID = packet.Info.IDs[index];
                skillLv = packet.Info.Lvs[index];

                privateData.SetAwakeningSkillData(skillID, skillLv);
            }
        }
    }

    public void AwakeningUpgradeSkillProcess(string packetStr)
    {
        PacketAwakningUpgradeSkill packet = LitJson.JsonMapper.ToObject<PacketAwakningUpgradeSkill>(packetStr);

        AwakeningLevelWindow awakeningWindow = GameUI.Instance.awakeningWindow;
        if (packet.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = connector.charInfo;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(packet.CharacterIndex) : null;
            if (privateData != null)
            {
                int nCount = Math.Min(packet.Info.SkillIDs.Length, packet.Info.Levels.Length);
                int skillID = 0;
                int skillLv = 0;

                for (int index = 0; index < nCount; ++index)
                {
                    skillID = packet.Info.SkillIDs[index];
                    skillLv = packet.Info.Levels[index];

                    privateData.SetAwakeningSkillData(skillID, skillLv);
                }

                privateData.baseInfo.APoint = packet.APoint;
                privateData.baseInfo.ABuyCount = packet.ABuyPoint;
            }

            if (charData != null)
                charData.SetGold(packet.TotalGold, packet.TotalCash);

            if (awakeningWindow != null)
                awakeningWindow.OnResultApply(packet.Info, packet.APoint, 0, 0, packet.ABuyPoint);
        }
        else
        {
            if (awakeningWindow != null)
                awakeningWindow.OnErrorMessage(packet.errorCode, null);
        }
    }
    public void AwakeningResetSkillProcess(string packetStr)
    {
        PacketAwakeningResetSkill packet = LitJson.JsonMapper.ToObject<PacketAwakeningResetSkill>(packetStr);

        AwakeningLevelWindow awakeningWindow = GameUI.Instance.awakeningWindow;
        if (packet.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = connector.charInfo;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(packet.CharacterIndex) : null;

            if (privateData != null)
            {
                privateData.baseInfo.APoint = packet.APoint;
                privateData.baseInfo.ABuyCount = packet.ABuyPoint;

                privateData.ResetAwakeingSkill();
            }

            if (charData != null)
                charData.SetGold(packet.TotalGold, packet.TotalCash);
        }

        if (awakeningWindow != null)
            awakeningWindow.OnResultReset(packet.errorCode, packet.APoint, 0, 0, packet.ABuyPoint);
    }

    public void AwakeningBuyPointProcess(string packetStr)
    {
        PacketAwakeningBuyPoint packet = LitJson.JsonMapper.ToObject<PacketAwakeningBuyPoint>(packetStr);

        AwakeningLevelWindow awakeningWindow = GameUI.Instance.awakeningWindow;

        if (packet.errorCode == NetErrorCode.OK)
        {
            CharInfoData charData = connector.charInfo;
            CharPrivateData privateData = charData != null ? charData.GetPrivateData(packet.CharacterIndex) : null;

            if (privateData != null)
            {
                privateData.baseInfo.APoint = packet.APoint;
                privateData.baseInfo.ABuyCount = packet.ABuyPoint;
            }

            if (charData != null)
                charData.SetGold(packet.TotalGold, packet.TotalCash);
        }

        if (awakeningWindow != null)
            awakeningWindow.OnResultBuyPoint(packet.errorCode, packet.APoint, 0, 0, packet.ABuyPoint);
    }

    public void EventListInfoProcess(string packetStr)
    {
        PacketEventList packet = LitJson.JsonMapper.ToObject<PacketEventList>(packetStr);

        Game.Instance.eventList.Clear();

        int nCount = Mathf.Min(packet.leftTimes.Length, packet.values.Length);
        int typeValue = 0;
        int eventValue = 0;
        int eventLeftTime = 0;

        for (int index = 0; index < nCount; ++index)
        {
            typeValue = packet.eventTypes[index];
            eventLeftTime = packet.leftTimes[index];
            eventValue = packet.values[index];

            if (eventLeftTime <= 0)
                continue;

            Game.Instance.AddEvent((CMSEventType)typeValue, eventLeftTime, eventValue);
        }
    }

    public void RandomBoxProcess(string packetStr)
    {
        PacketRandombox packet = LitJson.JsonMapper.ToObject<PacketRandombox>(packetStr);

        RandomBoxEventWindow randomBoxWindow = GameUI.Instance.randomBoxWindow;
        if (randomBoxWindow != null)
            randomBoxWindow.SetRandomBox(packet);
    }

    public void SpecialStageProcess(string packetStr)
    {
        PacketSpecialStage packet = LitJson.JsonMapper.ToObject<PacketSpecialStage>(packetStr);

        CharInfoData charData = Game.Instance.charInfoData;
        if (charData != null)
        {
            charData.specialStageInfo.Clear();

            foreach (int id in packet.open)
                charData.specialStageInfo.Add(id);
        }
    }

    public void RegisterHandler(ref NetMessageDispatcher dispatcher)
    {
        dispatcher.RegisterHandler(NetID.ServerChecking, ServerCheckingProcess);
        dispatcher.RegisterHandler(NetID.PreLoginDone, PreLoginProcess);
        dispatcher.RegisterHandler(NetID.RequestReconnect, RequestReconnectProcess);
        dispatcher.RegisterHandler(NetID.NeedUpdateApp, NeedUpdateAppProcess);
        dispatcher.RegisterHandler(NetID.Error, ErrorProcess);
        dispatcher.RegisterHandler(NetID.UserInfo, UserInfoProcess);
        dispatcher.RegisterHandler(NetID.RequestCreateNickName, RequestCreateNickNameProcess);
        dispatcher.RegisterHandler(NetID.CreateNickName, CreateNickNameProcess);
        dispatcher.RegisterHandler(NetID.CharacterInfo, CharacterInfoProcess);
        dispatcher.RegisterHandler(NetID.EquipItem, EquipItemProcess);
        dispatcher.RegisterHandler(NetID.SkillInfo, SkillInfoProcess);
        //dispatcher.RegisterHandler(NetID.Inventory, InventoryProcess);				
        //dispatcher.RegisterHandler(NetID.CostumeInfo, CostumeProcess);				
        dispatcher.RegisterHandler(NetID.PopupNotice, PopupNoticeProcess);
        dispatcher.RegisterHandler(NetID.LoginDone, LoginDoneProcess);
        dispatcher.RegisterHandler(NetID.CheckNickName, CheckNickNameProcess);

        dispatcher.RegisterHandler(NetID.CreateAccount, CreateAccountProcess);

        dispatcher.RegisterHandler(NetID.InvenExtendInfo, ExpandSlotsInfoProcess);
        dispatcher.RegisterHandler(NetID.InvenExtend, ExpandSlotsProcess);

        dispatcher.RegisterHandler(NetID.InvenNormalInfo, InventoryNormalItemProcess);
        dispatcher.RegisterHandler(NetID.InvenCostumeInfo, InventoryCostumeItemProcess);
        dispatcher.RegisterHandler(NetID.InvenMaterialInfo, InventoryMaterialItemProcess);
        dispatcher.RegisterHandler(NetID.InvenCostumeSetInfo, InventoryCostumeSetItemProcess);

        dispatcher.RegisterHandler(NetID.BuyNormalItem, BuyNormalItemProcess);
        dispatcher.RegisterHandler(NetID.BuyCostumeItem, BuyCostumeItemProcess);
        dispatcher.RegisterHandler(NetID.BuyCostumeSetItem, BuyCostumeSetItemProcess);

        dispatcher.RegisterHandler(NetID.SellEquipItem, SellItemProcess);
        dispatcher.RegisterHandler(NetID.SellInvenItem, SellItemProcess);
        dispatcher.RegisterHandler(NetID.SellCostumeItem, SellItemProcess);
        dispatcher.RegisterHandler(NetID.SellCostumeSetItem, SellItemProcess);
        dispatcher.RegisterHandler(NetID.SellMaterialItem, SellItemProcess);

        dispatcher.RegisterHandler(NetID.ReinforceItem, ReinforceItemProcess);
        dispatcher.RegisterHandler(NetID.ReinforceItemEx, ReinforceItemExProcess);
        dispatcher.RegisterHandler(NetID.CompositionItem, CompositionItemProcess);
        dispatcher.RegisterHandler(NetID.CompositionItemEx, CompositionItemExProcess);
        dispatcher.RegisterHandler(NetID.GambleInfo, GambleInfoProcess);
        dispatcher.RegisterHandler(NetID.GambleRefreshRespone, GambleRefreshProcess);
        dispatcher.RegisterHandler(NetID.SupplyGambleItem, SupplyGambleItemProcess);
        dispatcher.RegisterHandler(NetID.ChangeGambleItem, OnChangeGambleItem);
        dispatcher.RegisterHandler(NetID.DoEquipItemRespone, DoEquipItemResponeProcess);
        dispatcher.RegisterHandler(NetID.DoUnEquipItemRespone, DoUnEquipItemResponeProcess);

        //dispatcher.RegisterHandler(NetID.RequestGambleInfo, RequestGambleInfoProcess);
        dispatcher.RegisterHandler(NetID.RequestGambleOpen, RequestGambleOpenProcess);

        dispatcher.RegisterHandler(NetID.WearCostumeSetItem, DoEquipCostumeSetItemProcess);
        dispatcher.RegisterHandler(NetID.UnwearCostumeSetItem, DoUnEquipCostumeSetItemProcess);
        dispatcher.RegisterHandler(NetID.WearCostumeItem, DoEquipCostumeItemProcess);
        dispatcher.RegisterHandler(NetID.UnwearCostumeItem, DoUnEquipCostumeItemProcess);

        dispatcher.RegisterHandler(NetID.WaveInfo, WaveInfoProcess);
        dispatcher.RegisterHandler(NetID.WaveStartRespone, WaveStartProcess);
        dispatcher.RegisterHandler(NetID.WaveEndRespone, WaveEndProcess);
        dispatcher.RegisterHandler(NetID.WaveContinue, WaveContinueProcess);
        dispatcher.RegisterHandler(NetID.WaveRanking, WaveRankingProcess);
        dispatcher.RegisterHandler(NetID.BuyCashItem, BuyCashItemProcess);
        dispatcher.RegisterHandler(NetID.RequestBuyCashItem, RequestBuyCashItemProcess);
        dispatcher.RegisterHandler(NetID.ResponeBuyCashItem, ResponeBuyCashItemProcess);
        dispatcher.RegisterHandler(NetID.StageResult, StageResultProcess);
        dispatcher.RegisterHandler(NetID.StageEndFailed, StageEndFailedProcess);
        dispatcher.RegisterHandler(NetID.StageReward, StageRewardProcess);

        dispatcher.RegisterHandler(NetID.StageStart, StageStartProcess);
        dispatcher.RegisterHandler(NetID.RecoveryStaminaByStage, RecoveryStaminaByStage);

        dispatcher.RegisterHandler(NetID.RecoveryStamina, RecoveryStaminaByWave);

        dispatcher.RegisterHandler(NetID.UpgradeSkill, SkillUpgrade);
        dispatcher.RegisterHandler(NetID.ResetSkill, ResetSkill);
        dispatcher.RegisterHandler(NetID.RequestGambleRefresh, RequestGambleRefresh);

        dispatcher.RegisterHandler(NetID.ArenaInfo, ArenaInfoProcess);
        dispatcher.RegisterHandler(NetID.ArenaMatchingTarget, ArenaMatchingTargetProcess);
        dispatcher.RegisterHandler(NetID.ArenaRanking, ArenaRankingProcess);
        dispatcher.RegisterHandler(NetID.ArenaResult, ArenaResultProcess);
        dispatcher.RegisterHandler(NetID.ArenaReward, ArenaRewardProcess);
        dispatcher.RegisterHandler(NetID.WaveReward, WaveRewardProcess);
        dispatcher.RegisterHandler(NetID.EnterTown, EnterTownEndProcess);
        dispatcher.RegisterHandler(NetID.SupplyTicket, SupplyTicketProcess);

        dispatcher.RegisterHandler(NetID.TargetEquipItem, TargetEquipItemProcess);

        dispatcher.RegisterHandler(NetID.MailList, MailListProcess);
        dispatcher.RegisterHandler(NetID.PostItem, PostItemProcess);
        dispatcher.RegisterHandler(NetID.PostMsg, PostMsgProcess);
        dispatcher.RegisterHandler(NetID.PostItemAll, PostItemAllProcess);

        dispatcher.RegisterHandler(NetID.RecommandFriendList, RecommandFriendListProcess);
        dispatcher.RegisterHandler(NetID.FriendList, FriendListProcess);
        dispatcher.RegisterHandler(NetID.InvitedUserList, InvitedUserListProcess);
        dispatcher.RegisterHandler(NetID.SendStaminaToFriend, SendStaminaToFriendProcess);
        dispatcher.RegisterHandler(NetID.FriendInvite, FriendInviteProcess);
        dispatcher.RegisterHandler(NetID.FriendInviteAccept, FriendInviteAcceptProcess);
        dispatcher.RegisterHandler(NetID.FriendInviteByNickName, FriendInviteByNickNameProcess);

        dispatcher.RegisterHandler(NetID.AchievementCompleteInfo, AchievementCompleteProcess);
        dispatcher.RegisterHandler(NetID.AchievementInfo, AchievementInfoProcess);
        dispatcher.RegisterHandler(NetID.AchievementProgress, AchievementProcess);
        dispatcher.RegisterHandler(NetID.AchievementReward, AchievementRewardProcess);

        dispatcher.RegisterHandler(NetID.DailyMissionInfo, DailyMissionInfoProcess);
        dispatcher.RegisterHandler(NetID.DailyMissionProgress, DailyMissonProcess);
        dispatcher.RegisterHandler(NetID.DailyMissionReward, DailyMissionRewardProcess);
        dispatcher.RegisterHandler(NetID.BossAppear, BossAppearProcess);
        dispatcher.RegisterHandler(NetID.BossRaidInfo, BossRaidInfoProcess);
        dispatcher.RegisterHandler(NetID.BossRaidStart, BossRaidStartProcess);
        dispatcher.RegisterHandler(NetID.BossRaidEnd, BossRaidEndProcess);
        dispatcher.RegisterHandler(NetID.BossRaidStartRecoveryStamina, BossRaidStartRecoveryStaminaProcess);

        dispatcher.RegisterHandler(NetID.UpdateStamina, UpdateStaminaProcess);
        dispatcher.RegisterHandler(NetID.Revival, RevivalProcess);

        dispatcher.RegisterHandler(NetID.BadgeNotify, TownBadgeNotifyProcess);
        dispatcher.RegisterHandler(NetID.Coupon, CouponProcess);

        dispatcher.RegisterHandler(NetID.AttandnceCheck, AttandanceProcess);
        dispatcher.RegisterHandler(NetID.ShowEvent, ShowEventProcess);
        dispatcher.RegisterHandler(NetID.ReviewPlease, GameReviewProcess);
        dispatcher.RegisterHandler(NetID.RequestReviewReward, GameReviewRewardProcess);

        dispatcher.RegisterHandler(NetID.Dropout, MemberSecessionProcess);
        dispatcher.RegisterHandler(NetID.IgnorePush, IgnorePushProcess);

        dispatcher.RegisterHandler(NetID.StageTutorial, StageTutorialProcess);
        dispatcher.RegisterHandler(NetID.TutorialDone, TownTutorialDoneProcess);

        dispatcher.RegisterHandler(NetID.EventShopInfo, EventShopInfoProcess);
        dispatcher.RegisterHandler(NetID.BuyCashLimitItem, BuyEventCashItemProcess);

        dispatcher.RegisterHandler(NetID.RecvEmpty, RecvEmptyProcess);

        dispatcher.RegisterHandler(NetID.SpecialMissionInfo, SpecialMissionInfoProcess);
        dispatcher.RegisterHandler(NetID.SpecialMissionCompleteInfo, SpecialMissionCompleteProcess);
        dispatcher.RegisterHandler(NetID.SpecialMissionProgress, SpecialMissionProgress);
        dispatcher.RegisterHandler(NetID.SpecialMissionReward, SpecialMissionRewardProgress);

        dispatcher.RegisterHandler(NetID.TimeLimitItemInfo, TimeLimitItemInfoProcess);
        dispatcher.RegisterHandler(NetID.InvokeTimeLimitItem, InvokeTimeLimitItemProcess);

        dispatcher.RegisterHandler(NetID.AwakeningSkillInfo, AwakeningSkillInfoProcess);
        dispatcher.RegisterHandler(NetID.AwakeningUpgreadeSkill, AwakeningUpgradeSkillProcess);
        dispatcher.RegisterHandler(NetID.AwakeningResetSkill, AwakeningResetSkillProcess);
        dispatcher.RegisterHandler(NetID.AwakeningBuyPoint, AwakeningBuyPointProcess);

        dispatcher.RegisterHandler(NetID.EventListInfo, EventListInfoProcess);

        dispatcher.RegisterHandler(NetID.RandomBox, RandomBoxProcess);

        dispatcher.RegisterHandler(NetID.SpecialStage, SpecialStageProcess);
    }

}

