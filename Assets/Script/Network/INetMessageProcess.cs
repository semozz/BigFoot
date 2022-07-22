﻿using System;

public interface INetMessageProcess
{
    void AchievementCompleteProcess(string packet);
    void AchievementInfoProcess(string packet);
    void AchievementProcess(string packet);
    void AchievementRewardProcess(string packet);
    void AddTimeLimitBuffItem(CharInfoData charData, int itemID, DateTime endTime);
    void ArenaInfoProcess(string packet);
    void ArenaMatchingTargetProcess(string packet);
    void ArenaRankingProcess(string packet);
    void ArenaResultProcess(string packet);
    void ArenaRewardProcess(string packet);
    void AttandanceProcess(string packetStr);
    void AwakeningBuyPointProcess(string packetStr);
    void AwakeningResetSkillProcess(string packetStr);
    void AwakeningSkillInfoProcess(string packetStr);
    void AwakeningUpgradeSkillProcess(string packetStr);
    void BossAppearProcess(string packet);
    void BossRaidEndProcess(string packet);
    void BossRaidInfoProcess(string packet);
    void BossRaidStartProcess(string packet);
    void BossRaidStartRecoveryStaminaProcess(string packet);
    void BuyCostumeItemProcess(string strPacket);
    void BuyCostumeSetItemProcess(string strPacket);
    void BuyNormalItemProcess(string strPacket);
    void CharacterInfoProcess(string packet);
    void CheckNickNameProcess(string packet);
    void CompositionItemExProcess(string strPacket);
    void CompositionItemProcess(string strPacket);
    ClientConnector Connector { get; set; }
    void CostumeProcess(string packet);
    void CouponProcess(string packetStr);
    void CreateAccountProcess(string strPacket);
    void CreateNickNameProcess(string packet);
    void DailyMissionInfoProcess(string packet);
    void DailyMissionRewardProcess(string packet);
    void DailyMissonProcess(string packet);
    void DoEquipCostumeItemProcess(string strPacket);
    void DoEquipCostumeSetItemProcess(string strPacket);
    void DoEquipItemResponeProcess(string strPacket);
    void DoUnEquipCostumeItemProcess(string strPacket);
    void DoUnEquipCostumeSetItemProcess(string strPacket);
    void DoUnEquipItemResponeProcess(string strPacket);
    void EnterTownEndProcess(string packet);
    void EquipItemProcess(string packet);
    void ErrorProcess(string packet);
    void EventListInfoProcess(string packetStr);
    void ExpandSlotsInfoProcess(string strPacket);
    void ExpandSlotsProcess(string strPacket);
    void FriendInviteAcceptProcess(string packet);
    void FriendInviteByNickNameProcess(string packet);
    void FriendInviteProcess(string packet);
    void FriendListProcess(string packet);
    void GambleInfoProcess(string packet);
    void GambleRefreshProcess(string packet);
    void GameReviewProcess(string packetStr);
    void GameReviewRewardProcess(string packetStr);
    void IgnorePushProcess(string packetStr);
    void InventoryCostumeItemProcess(string packet);
    void InventoryCostumeSetItemProcess(string packet);
    void InventoryMaterialItemProcess(string packet);
    void InventoryNormalItemProcess(string packet);
    void InventoryProcess(string packet);
    void InvitedUserListProcess(string packet);
    void InvokeTimeLimitItemProcess(string packetStr);
    void LoginDoneProcess(string packet);
    void MailListProcess(string packet);
    void MemberSecessionProcess(string packetStr);
    void NeedUpdateAppProcess(string packet);
    void OnChangeGambleItem(string packet);
    void PopupNoticeProcess(string packet);
    void PostItemAllProcess(string packet);
    void PostItemProcess(string packet);
    void PostMsgProcess(string packet);
    void PreLoginProcess(string packet);
    void RecommandFriendListProcess(string packet);
    void RecvEmptyProcess(string packetStr);
    void RegisterHandler(ref NetMessageDispatcher dispatcher);
    void ReinforceItemExProcess(string strPacket);
    void ReinforceItemProcess(string strPacket);
    void RequestCreateNickNameProcess(string packet);
    void RequestGambleOpenProcess(string strPacket);
    void RequestGambleRefresh(string packet);
    void RequestReconnectProcess(string packet);
    void ResetSkill(string packet);
    void ResponeBuyCashItemProcess(string packet);
    void RevivalProcess(string packet);
    void SellItemProcess(string strPacket);
    void SendStaminaToFriendProcess(string packet);
    void ServerCheckingProcess(string packet);
    void ShowEventProcess(string packetStr);
    void SkillInfoProcess(string packet);
    void SkillUpgrade(string packet);
    void SpecialMissionCompleteProcess(string packet);
    void SpecialMissionInfoProcess(string packet);
    void SpecialMissionProgress(string packet);
    void SpecialMissionRewardProgress(string packetStr);
    void StageEndFailedProcess(string packet);
    void StageTutorialProcess(string packetStr);
    void SupplyGambleItemProcess(string packet);
    void SupplyTicketProcess(string packet);
    void TargetEquipItemProcess(string packet);
    void TimeLimitItemInfoProcess(string packetStr);
    void TownBadgeNotifyProcess(string packetStr);
    void TownTutorialDoneProcess(string packetStr);
    void UpdateItemInfos(CharPrivateData privateData, bool bRerangeItem);
    void UpdateStaminaProcess(string packet);
    void UserInfoProcess(string packet);
    void WaveContinueProcess(string packet);
    void WaveEndProcess(string packet);
    void WaveInfoProcess(string packet);
    void WaveRankingProcess(string packet);
    void WaveRewardProcess(string packet);
    void WaveStartProcess(string packet);
}
