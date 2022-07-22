using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScoreWindow : MonoBehaviour {
	public enum eScoreType
	{
		Arena,
		Wave,
	}
	public eScoreType scoreWindowType = eScoreType.Arena;
	
	public GameObject parentWindow = null;
	
	public string scorePanelPrefabPath = "";
	public UIGrid grid = null;
	public UIDraggablePanel dragablePanel = null;
	public UIPanel scrollPanel = null;
	
	public UILabel timeTitleLabel = null;
	public UILabel timeInfoLabel = null;
	
	public int timeTitleStringID = -1;
	
	public StringTable stringTable = null;
	public int unavailableTimeFormatStringID = -1;
	public int availableTimeFormatStringID = -1;
	
	public ArenaWindow arenaWindow = null;
	
	public Transform firstItemPos = null;
	public Transform lastItemPos = null;
	
	public ScoreInfoPanel firstItem = null;
	public ScoreInfoPanel lastItem = null;
	
	public void Start()
	{
		TableManager tableManager = TableManager.Instance;
		stringTable = tableManager != null ? tableManager.stringTable : null;
		
		if (stringTable != null)
		{
			if (timeTitleLabel != null && timeTitleStringID != -1)
				timeTitleLabel.text = stringTable.GetData(timeTitleStringID);
			
			if (timeInfoLabel != null && unavailableTimeFormatStringID != -1)
				timeInfoLabel.text = stringTable.GetData(unavailableTimeFormatStringID);
		}
		/*
		if (stringTable != null && timeTitleLabel != null && timeTitleStringID != -1)
			timeTitleLabel.text = stringTable.GetData(timeTitleStringID);
		
		
		List<ScoreInfo> infos = new List<ScoreInfo>();
		int nCount = 10;
		for (int index = 0; index < nCount; ++index)
		{
			ScoreInfo info = new ScoreInfo();
			info.charName = string.Format("Name_{0}", index);
			info.score = Random.Range(0, 1000);
			info.profileName = string.Format("Profile_{0}", index);
			info.classType = (GameDef.ePlayerClass)Random.Range(0, 4);
			
			infos.Add(info);
		}
		
		SetScoreInfos(infos);
		*/
		
		if (dragablePanel != null)
			dragablePanel.onDragFinished = new UIDraggablePanel.OnDragFinished(OnDragFinished);
	}
	
	
	private bool isOpen = false;
	void Update()
	{
		System.TimeSpan restTime = new System.TimeSpan(0, 0, 0);
		UpdateRefreshTime(out restTime);
		double totalSeconds = restTime.TotalSeconds;
		
		string timeText = "--:--:--";
		if (stringTable != null)
		{
			if (isOpen == true && totalSeconds > 0)
			{
				string timeFormatString = stringTable.GetData(availableTimeFormatStringID);
				timeText = string.Format(timeFormatString, restTime.Days, restTime.Hours, restTime.Minutes);
			}
		}
		
		if (isOpen == true && totalSeconds <= 0)
			parentWindow.SendMessage("CloseStartButton", SendMessageOptions.DontRequireReceiver);
		
		timeInfoLabel.text = timeText;
	}
	
	public static System.DateTime refreshExpireTime = System.DateTime.Now;
	public void UpdateRefreshTime(out System.TimeSpan restTime)
	{
		System.DateTime nowTime = System.DateTime.Now;
		restTime = refreshExpireTime - nowTime;
		
		if (restTime.TotalSeconds < 0)
			restTime = System.TimeSpan.Zero;
	}
	
	/*
	public void SetScoreInfos(List<ScoreInfo> infos, int leftRefreshTime)
	{
		refreshExpireTime = System.DateTime.Now;
		System.TimeSpan addSpan = Game.ToTimeSpan(leftRefreshTime);
		refreshExpireTime += addSpan;
		
		infos.Sort(ScoreInfo.Sort);
		
		Vector3 vPos = Vector3.zero;
		foreach(ScoreInfo info in infos)
		{
			ScoreInfoPanel infoPanel = PopupBaseWindow.CreatePrefab<ScoreInfoPanel>(scorePanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetInfo(info);
				
				SetDetailButtonMessage(infoPanel.buttonMessage);
				
				vPos.y += grid.cellHeight;
			}
		}
		
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
	}
	*/
	
	public void SetDetailButtonMessage(UIButtonMessage buttonMsg)
	{
		if (buttonMsg != null)
		{
			buttonMsg.target = this.parentWindow;
			buttonMsg.functionName = "OnTargetDetailWindow";
		}
	}
	
	private List<ScoreInfoPanel> scoreInfoPanels = new List<ScoreInfoPanel>();
	public void SetScoreInfos(ArenaRankingInfo[] rankingInfos, int leftRefreshTime, int isOpen)
	{
		this.isOpen = (isOpen == 1);
		
		refreshExpireTime = System.DateTime.Now;
		System.TimeSpan addSpan = Game.ToTimeSpan(leftRefreshTime);
		refreshExpireTime += addSpan;
		
		SetScoreInfos(rankingInfos);
	}
	
	public void SetScoreInfos(ArenaRankingInfo[] rankingInfos)
	{
		if (rankingInfos == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		int nCount = rankingInfos.Length;
		for (int index = 0; index < nCount; ++index)
		{
			ArenaRankingInfo info = rankingInfos[index];
			
			ScoreInfoPanel infoPanel = ResourceManager.CreatePrefab<ScoreInfoPanel>(scorePanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetInfo(info);
				
				SetDetailButtonMessage(infoPanel.buttonMessage);
				
				infoPanel.gameObject.name = string.Format("{0:D8}", info.ranking);
				
				if (lastItem == null || lastItem.arenaRankingInfo.ranking < info.ranking)
					lastItem = infoPanel;
				
				if (firstItem == null || firstItem.arenaRankingInfo.ranking > info.ranking)
					firstItem = infoPanel;
				
				scoreInfoPanels.Add(infoPanel);
				
				//vPos.y += grid.cellHeight;
			}
		}
		
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
	}
	
	public void AddDownList(ArenaRankingInfo[] rankingInfos)
	{
		if (rankingInfos == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		if (lastItem != null)
			vPos = lastItem.transform.localPosition;
		
		foreach(ArenaRankingInfo info in rankingInfos)
		{
			ScoreInfoPanel infoPanel = ResourceManager.CreatePrefab<ScoreInfoPanel>(scorePanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetInfo(info);
				
				SetDetailButtonMessage(infoPanel.buttonMessage);
				
				infoPanel.gameObject.name = string.Format("{0:D8}", info.ranking);
				
				scoreInfoPanels.Add(infoPanel);
				
				if (lastItem == null || lastItem.arenaRankingInfo.ranking < info.ranking)
					lastItem = infoPanel;
				
				if (firstItem == null || firstItem.arenaRankingInfo.ranking > info.ranking)
					firstItem = infoPanel;
				
				//vPos.y += grid.cellHeight;
			}
		}
		
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
		
		if (dragablePanel != null && dragablePanel.verticalScrollBar != null)
			dragablePanel.verticalScrollBar.scrollValue = 1.0f;
	}
	
	public void AddUpList(ArenaRankingInfo[] rankingInfos)
	{
		if (rankingInfos == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		if (firstItem != null)
			vPos = firstItem.transform.localPosition;
		
		int nCount = rankingInfos.Length;
		for (int index = nCount - 1; index >= 0; --index)
		{
			ArenaRankingInfo info = rankingInfos[index];
			
			ScoreInfoPanel infoPanel = ResourceManager.CreatePrefab<ScoreInfoPanel>(scorePanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetInfo(info);
				
				SetDetailButtonMessage(infoPanel.buttonMessage);
				
				infoPanel.gameObject.name = string.Format("{0:D8}", info.ranking);
				
				scoreInfoPanels.Insert(0, infoPanel);
				
				if (lastItem == null || lastItem.arenaRankingInfo.ranking < info.ranking)
					lastItem = infoPanel;
				
				if (firstItem == null || firstItem.arenaRankingInfo.ranking > info.ranking)
					firstItem = infoPanel;
				
				//vPos.y -= grid.cellHeight;
			}
		}
		
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
		
		if (dragablePanel != null && dragablePanel.verticalScrollBar != null)
			dragablePanel.verticalScrollBar.scrollValue = 0.0f;
	}
	
	
	
	//
	public void SetScoreInfos(WaveRankingInfo[] rankingInfos, int leftRefreshTime, int isOpen)
	{
		this.isOpen = (isOpen == 1);
		
		refreshExpireTime = System.DateTime.Now;
		System.TimeSpan addSpan = Game.ToTimeSpan(leftRefreshTime);
		refreshExpireTime += addSpan;
		
		SetScoreInfos(rankingInfos);
	}
	
	public void SetScoreInfos(WaveRankingInfo[] rankingInfos)
	{
		if (rankingInfos == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		int nCount = rankingInfos.Length;
		for (int index = 0; index < nCount; ++index)
		{
			WaveRankingInfo info = rankingInfos[index];
			
			ScoreInfoPanel infoPanel = ResourceManager.CreatePrefab<ScoreInfoPanel>(scorePanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetInfo(info);
				
				SetDetailButtonMessage(infoPanel.buttonMessage);
				
				infoPanel.gameObject.name = string.Format("{0:D8}", info.ranking);
				
				if (lastItem == null || lastItem.waveRankingInfo.ranking < info.ranking)
					lastItem = infoPanel;
				
				if (firstItem == null || firstItem.waveRankingInfo.ranking > info.ranking)
					firstItem = infoPanel;
				
				scoreInfoPanels.Add(infoPanel);
				
				//vPos.y += grid.cellHeight;
			}
		}
		
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
	}
	
	public void AddDownList(WaveRankingInfo[] rankingInfos)
	{
		if (rankingInfos == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		if (lastItem != null)
			vPos = lastItem.transform.localPosition;
		
		foreach(WaveRankingInfo info in rankingInfos)
		{
			ScoreInfoPanel infoPanel = ResourceManager.CreatePrefab<ScoreInfoPanel>(scorePanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetInfo(info);
				
				SetDetailButtonMessage(infoPanel.buttonMessage);
				
				infoPanel.gameObject.name = string.Format("{0:D8}", info.ranking);
				
				scoreInfoPanels.Add(infoPanel);
				
				if (lastItem == null || lastItem.waveRankingInfo.ranking < info.ranking)
					lastItem = infoPanel;
				
				if (firstItem == null || firstItem.waveRankingInfo.ranking > info.ranking)
					firstItem = infoPanel;
				
				//vPos.y += grid.cellHeight;
			}
		}
		
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
		
		if (dragablePanel != null && dragablePanel.verticalScrollBar != null)
			dragablePanel.verticalScrollBar.scrollValue = 1.0f;
	}
	
	public void AddUpList(WaveRankingInfo[] rankingInfos)
	{
		if (rankingInfos == null)
			return;
		
		Vector3 vPos = Vector3.zero;
		if (firstItem != null)
			vPos = firstItem.transform.localPosition;
		
		int nCount = rankingInfos.Length;
		for (int index = nCount - 1; index >= 0; --index)
		{
			WaveRankingInfo info = rankingInfos[index];
			
			ScoreInfoPanel infoPanel = ResourceManager.CreatePrefab<ScoreInfoPanel>(scorePanelPrefabPath, grid.transform, vPos);
			if (infoPanel != null)
			{
				infoPanel.SetInfo(info);
				
				SetDetailButtonMessage(infoPanel.buttonMessage);
				
				infoPanel.gameObject.name = string.Format("{0:D8}", info.ranking);
				
				scoreInfoPanels.Insert(0, infoPanel);
				
				if (lastItem == null || lastItem.waveRankingInfo.ranking < info.ranking)
					lastItem = infoPanel;
				
				if (firstItem == null || firstItem.waveRankingInfo.ranking > info.ranking)
					firstItem = infoPanel;
				
				//vPos.y -= grid.cellHeight;
			}
		}
		
		if (grid != null)
			grid.Reposition();
		
		if (dragablePanel != null)
			dragablePanel.ResetPosition();
		
		if (scrollPanel != null)
			scrollPanel.Refresh();
		
		if (dragablePanel != null && dragablePanel.verticalScrollBar != null)
			dragablePanel.verticalScrollBar.scrollValue = 0.0f;
	}
	
	public void ResetScrollViewData()
	{
		foreach(ScoreInfoPanel panel in scoreInfoPanels)
		{
			DestroyObject(panel.gameObject, 0.0f);
		}
		scoreInfoPanels.Clear();
		
		if (grid != null)
			grid.Reposition();
	}
	
	public float refreshLimitLength = 0.1f;
	public void OnDragFinished()
	{
		if (firstItem == null || firstItemPos == null || lastItem == null || lastItemPos == null)
			return;
		
		Vector3 limitPos = firstItemPos.position;
		Vector3 lastPos = firstItem.transform.position;
		
		Vector3 firstDiff = limitPos - lastPos;
		if (limitPos.y > lastPos.y &&
			Mathf.Abs(firstDiff.y) > refreshLimitLength)
		{
			Debug.Log("Upper List Request........");
			
			int ranking = -1;
			if (scoreWindowType == eScoreType.Arena)
				ranking = firstItem.arenaRankingInfo.ranking;
			else
				ranking = firstItem.waveRankingInfo.ranking;
				
			if (parentWindow != null)
				parentWindow.SendMessage("RequestUpperList", ranking, SendMessageOptions.DontRequireReceiver);

			return;
		}
		
		limitPos = lastItemPos.position;
		lastPos = lastItem.transform.position;
		
		Vector3 lastDiff = limitPos - lastPos;
		if (limitPos.y < lastPos.y &&
			Mathf.Abs(lastDiff.y) > refreshLimitLength)
		{
			Debug.Log("Lower List Request......");
			
			int ranking = -1;
			if (scoreWindowType == eScoreType.Arena)
				ranking = lastItem.arenaRankingInfo.ranking;
			else
				ranking = lastItem.waveRankingInfo.ranking;
			
			if (parentWindow != null)
				parentWindow.SendMessage("RequestLowerList", ranking, SendMessageOptions.DontRequireReceiver);
			
			return;
		}
	}
	
}
