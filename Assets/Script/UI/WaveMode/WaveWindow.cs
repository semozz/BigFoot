using UnityEngine;
using System.Collections;

public class WaveWindow : PopupBaseWindow
{
    public TownUI townUI = null;

    public UILabel titleLabel = null;
    public int titleStringID = -1;

    public UILabel continuePriceLabel = null;
    public UILabel continueButtonLabel = null;
    public int continuButtonStringID = -1;

    public UILabel startButtonLabel = null;
    public int startButtonStringID = -1;

    public UILabel candidateRewardTitle = null;
    public int candidateTitleStringID = -1;
    public UILabel candiateAmountLabel = null;

    public ScoreWindow scoreWindow = null;

    public TabButtonController tabButtonController = null;

    public WaveStartInfo waveStartInfo = null;
    public WaveRewardInfoTable waveRewardInfoTable = null;
    Vector3 continueWavePrice = Vector3.zero;
    public override void Awake()
    {
        this.windowType = TownUI.eTOWN_UI_TYPE.WAVE;

        base.Awake();

        TableManager tableManager = TableManager.Instance;
        StringTable stringTable = null;
        StringValueTable stringValueTable = null;
        if (tableManager != null)
        {
            waveRewardInfoTable = tableManager.waveRewardInfo;
            stringTable = tableManager.stringTable;
            stringValueTable = tableManager.stringValueTable;

            waveStartInfo = tableManager.waveStartInfo;
        }

        if (stringValueTable != null)
            continueWavePrice.y = (float)stringValueTable.GetData("WaveContinuePrice");

        if (continuePriceLabel != null)
            continuePriceLabel.text = string.Format("{0}", (int)continueWavePrice.y);

        if (stringTable != null)
        {
            if (titleLabel != null && titleStringID != -1)
                titleLabel.text = stringTable.GetData(titleStringID);

            if (continueButtonLabel != null && continuButtonStringID != -1)
                continueButtonLabel.text = stringTable.GetData(continuButtonStringID);

            if (startButtonLabel != null && startButtonStringID != -1)
                startButtonLabel.text = stringTable.GetData(startButtonStringID);

            if (candidateRewardTitle != null && candidateTitleStringID != -1)
                candidateRewardTitle.text = stringTable.GetData(candidateTitleStringID);
        }
    }
    // Use this for initialization
    void Start()
    {
        GameUI.Instance.waveWindow = this;
    }

    void OnDestory()
    {
        GameUI.Instance.waveWindow = null;
    }

    public bool bListEnd = false;
    public void InitWindow(WaveRankingInfo myWaveInfo, WaveRankingInfo[] rankingInfos, int leftRefreshTime, int isClear, int isOpen)
    {
        WaveMyInfo myInfo = tabButtonController != null ? tabButtonController.GetViewWindow<WaveMyInfo>(0) : null;
        if (myInfo != null)
            myInfo.SetMyInfo(myWaveInfo);

        SetCandidateReward(myWaveInfo);

        if (scoreWindow != null)
            scoreWindow.SetScoreInfos(rankingInfos, leftRefreshTime, isOpen);

        bool bCanContinue = false;
        if (myWaveInfo.RecordStep >= 0 && myWaveInfo.RecordSec > 0 && isClear == 0)
            bCanContinue = true;
        SetContinueButton(bCanContinue);

        SetAdjustTimeMode(isOpen == 1);

        UpdateCoinInfo();
		
		if (TownUI.notifyOpen)
		{
			NotifyOpenPopup();
			TownUI.notifyOpen = false;
		}
    }

    public void SetCandidateReward(WaveRankingInfo myInfo)
    {
        int rewardAmount = 0;
        WaveRewardData data = null;
        if (waveRewardInfoTable != null && myInfo != null)
            data = waveRewardInfoTable.GetRewardInfoData(myInfo.ranking);

        if (data != null)
            rewardAmount = data.rewardJewel;

        string rewardStr = string.Format("{0}", rewardAmount);

        if (candiateAmountLabel != null)
            candiateAmountLabel.text = rewardStr;
    }

    public Collider continueButtonCollider = null;

    public void SetContinueButton(bool bCanContinue)
    {
        if (continueButtonCollider != null)
            continueButtonCollider.enabled = bCanContinue;

        Color labelColor = Color.white;
        if (bCanContinue == false)
            labelColor = Color.gray;
        if (continueButtonLabel != null)
            continueButtonLabel.color = labelColor;
    }

    public GameObject adjustModeObj = null;
    public GameObject normalModeObj = null;
    public void SetAdjustTimeMode(bool isOpen)
    {
        if (adjustModeObj != null && normalModeObj != null)
        {
            adjustModeObj.SetActive(!isOpen);
            normalModeObj.SetActive(isOpen);
        }
    }

    public void CloseStartButton()
    {
        SetAdjustTimeMode(false);
    }

    public override void OnBack()
    {
        requestCount = 0;
        base.OnBack();

        if (scoreWindow != null)
            scoreWindow.ResetScrollViewData();

    }

    public int requestCount = 0;
    public string alertArenaPopupPrefabPath = "";
    public void DoAlertArenaPopup()
    {
        if (requestCount > 0)
            return;

        BaseConfirmPopup resetConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(alertArenaPopupPrefabPath, popupNode, Vector3.zero);
        if (resetConfirmPopup != null)
        {
            resetConfirmPopup.cancelButtonMessage.target = this.gameObject;
            resetConfirmPopup.cancelButtonMessage.functionName = "OnClosePopup";

            resetConfirmPopup.okButtonMessage.target = this.gameObject;
            resetConfirmPopup.okButtonMessage.functionName = "OnWaveStartByCheck";

            popupList.Add(resetConfirmPopup);
        }
    }

    public void OnWaveStratButton(GameObject obj)
    {
        Debug.Log("Enter Arena....");

        if (scoreWindow != null)
        {
            System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
            scoreWindow.UpdateRefreshTime(out restTime);
            double totalSeconds = restTime.TotalSeconds;
            System.TimeSpan limitTime = new System.TimeSpan(0, 30, 0);
            if (totalSeconds >= 0 && totalSeconds <= limitTime.TotalSeconds)
            {
                DoAlertArenaPopup();
                return;
            }
            else
                OnWaveStartByCheck(null);
        }
    }

    public string waveCheckPopupPrefabPath = "";
    public void OnWaveStartByCheck(GameObject obj)
    {
        if (requestCount > 0)
            return;

        bool canContinue = false;
        if (continueButtonCollider != null)
            canContinue = continueButtonCollider.enabled;

        if (canContinue == true)
        {
            BaseConfirmPopup resetConfirmPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(waveCheckPopupPrefabPath, popupNode, Vector3.zero);
            if (resetConfirmPopup != null)
            {
                resetConfirmPopup.cancelButtonMessage.target = this.gameObject;
                resetConfirmPopup.cancelButtonMessage.functionName = "OnClosePopup";

                resetConfirmPopup.okButtonMessage.target = this.gameObject;
                resetConfirmPopup.okButtonMessage.functionName = "OnWaveStart";

                popupList.Add(resetConfirmPopup);
            }
        }
        else
            OnWaveStart(null);
    }

    public string waveStartPrefabPath = "";
    public WaveStartWindow waveStartWindow = null;
    public void OnWaveStart(GameObject obj)
    {
        ClosePopup();

        if (waveStartWindow == null)
            waveStartWindow = ResourceManager.CreatePrefab<WaveStartWindow>(waveStartPrefabPath, popupNode, Vector3.zero);
        else
            waveStartWindow.gameObject.SetActive(true);

        if (waveStartWindow != null)
        {
            waveStartWindow.waveWindow = this;
            waveStartWindow.InitWindow();
        }
    }

    public void OnRequestEnterArena()
    {
        IPacketSender packetSender = Game.Instance.packetSender;
        if (packetSender != null)
        {
            packetSender.SendRequestArenaStart();

            requestCount++;
        }
    }

    public void RefreshRankList(NetErrorCode errorCode, WaveRankingInfo[] rankingInfos, bool bDown)
    {
        requestCount--;

        if (errorCode == NetErrorCode.OK)
        {
            if (scoreWindow != null)
            {
                if (bDown == true)
                    scoreWindow.AddDownList(rankingInfos);
                else
                    scoreWindow.AddUpList(rankingInfos);
            }
        }
    }

    public void RequestUpperList(int ranking)
    {
        IPacketSender packetSender = Game.Instance.packetSender;
        if (packetSender != null)
        {
            packetSender.SendRequestWaveRanking(ranking, false);

            requestCount++;
        }
    }

    public void RequestLowerList(int ranking)
    {
        IPacketSender packetSender = Game.Instance.packetSender;
        if (packetSender != null)
        {
            packetSender.SendRequestWaveRanking(ranking, true);

            requestCount++;
        }
    }

    public void OnRequestWaveContinue(GameObject obj)
    {
        CashItemType checkType = CheckNeedGold(continueWavePrice);
        if (checkType != CashItemType.None)
        {
            ClosePopup();
            OnNeedMoneyPopup(checkType, this);
            return;
        }
        else
        {
            IPacketSender packetSender = Game.Instance.packetSender;
            if (packetSender != null)
            {
                packetSender.SendRequestWaveContinue();

                requestCount++;
            }
        }
    }

    public string waveStageName = "";
    public void OnWaveContinue(int startStep, int startSec, int[] buffs, int towerIndex)
    {
        requestCount--;

        Debug.Log("WaveWindow Active !!!!!!!");
        this.gameObject.SetActive(true);

        Game.Instance.selectedReinforceInfos.Clear();
        Game.Instance.selectedTowerInfo = null;

        WaveManager.continueWaveStep = -1;
        WaveManager.continueWaveTime = 0;

        foreach (int buffIndex in buffs)
        {
            ReinforceInfo info = waveStartInfo.GetBuffInfo(buffIndex);
            if (info != null)
                Game.Instance.selectedReinforceInfos.Add(info);
        }

        Game.Instance.selectedTowerInfo = waveStartInfo.towerInfoList[towerIndex];

        WaveManager.continueWaveStep = startStep;
        WaveManager.continueWaveTime = startSec;

        CreateLoadingPanel(waveStageName);
    }

    public string loadingPanelPrefabPath = "";
    public LoadingPanel loadingPanel = null;
    public void CreateLoadingPanel(string stageName)
    {
        if (loadingPanel == null)
        {
            loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, popupNode, Vector3.zero);
        }
        else
        {
            loadingPanel.gameObject.SetActive(true);
        }

        if (loadingPanel != null)
        {
            loadingPanel.ChangeLoadingBackgroundImage(stageName);
            loadingPanel.LoadScene(stageName, this);
        }
    }

    public void OnTargetDetailWindow(GameObject obj)
	{
		ScoreInfoPanel scorePanel = null;
		GameObject parentObj = obj != null ? obj.transform.parent.gameObject : null;
		
		if (parentObj != null)
			scorePanel = parentObj.GetComponent<ScoreInfoPanel>();
		
		if (scorePanel != null)
		{
			long targetUserIndexID = -1;
			int targetCharIndex = -1;
            string platform = "kakao";
			switch(scorePanel.dataType)
			{
			case ScoreInfoPanel.eScoreType.eScoreInfo:
				break;
			case ScoreInfoPanel.eScoreType.eArenaRankingInfo:
				targetUserIndexID = scorePanel.arenaRankingInfo.UserIndexID;
				targetCharIndex = scorePanel.arenaRankingInfo.CharacterIndex;
                platform = scorePanel.arenaRankingInfo.platform;
				break;
			case ScoreInfoPanel.eScoreType.eWaveRankingInfo:
				targetUserIndexID = scorePanel.waveRankingInfo.UserIndexID;
				targetCharIndex = scorePanel.waveRankingInfo.CharacterIndex;
                platform = scorePanel.waveRankingInfo.Platform;
				break;
			}
			
			IPacketSender sender = Game.Instance.packetSender;
			if (sender != null && targetUserIndexID != -1 && targetCharIndex != -1)
			{
				if (TownUI.detailRequestCount > 0)
					return;
				
				TownUI.detailRequestCount++;
				TownUI.detailWindowRoot = this.popupNode;

                sender.SendRequestTargetEquipItem(targetUserIndexID, targetCharIndex, platform);
			}
		}
	}
	
	public string waveOpenPrefabPath = "UI/DefenceOpenPopup";
	public void NotifyOpenPopup()
	{		
		BaseConfirmPopup notifyOpenPopup = null;
		notifyOpenPopup = ResourceManager.CreatePrefab<BaseConfirmPopup>(waveOpenPrefabPath, popupNode, Vector3.one);
		if (notifyOpenPopup != null)
		{
			notifyOpenPopup.okButtonMessage.target = this.gameObject;
			notifyOpenPopup.okButtonMessage.functionName = "OnCancelPopup";
			popupList.Add(notifyOpenPopup);
		}
	}
	
	public void OnCancelPopup(GameObject obj)
	{
		ClosePopup();
	}
}
