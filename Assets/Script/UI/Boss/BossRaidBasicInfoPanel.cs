using UnityEngine;
using System.Collections;

public class BossRaidBasicInfoPanel : MonoBehaviour {

	public UILabel bossNameLabel = null;
	public int bossNamePostfixStringID = -1;
	
	public UILabel finderTitleLabel = null;
	public UILabel finderNameLabel = null;
	public int finderTitleStringID = 184;

	public UILabel timeTitleLabel = null;
	public UILabel timeInfoLabel = null;
	public int timeTitleStringID = 185;
	public int timeFormatStringID = 182;
	public int nonTimeFormatStringID = 183;

	public UILabel myDamageTitle = null;
	public UILabel myDamageLabel = null;
	public int myDamageTitleStringID = 186;

	public UILabel topCharTitleLabel = null;
	public UILabel topCharName = null;
	public UILabel topCharDamage = null;
	public int topCharTitleStringID = 187;
	
	public UILabel lastAttackerTitleLabel = null;
	public UILabel lastAttackerName = null;
	public int lastAttackerTitleStringID = 255;

	public int needStamina = 0;
	public UILabel needStaminaLabel = null;
	public UILabel startButtonLabel = null;
	public int startButtonStringID = 188;
	
	public UIButtonMessage buttonMessage = null;
	
	
	public BossRaidInfo bossRaidInfo = null;
	private string bossNamePostfixStr = "";
	
	private System.DateTime expireTime = System.DateTime.Now;
	string leftTimeFormat = "";
	
	public void Start()
	{
		TableManager tableManager = TableManager.Instance;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		StringValueTable stringValueTable = tableManager != null ? tableManager.stringValueTable : null;
		needStamina = stringValueTable != null ? stringValueTable.GetData("BossRaidNeedStamina") : 10;	
		
		if (finderTitleLabel != null && finderTitleStringID != -1)
			finderTitleLabel.text = stringTable.GetData(finderTitleStringID);
		if (timeTitleLabel != null && timeTitleStringID != -1)
			timeTitleLabel.text = stringTable.GetData(timeTitleStringID);
		if (myDamageTitle != null && myDamageTitleStringID != -1)
			myDamageTitle.text = stringTable.GetData(myDamageTitleStringID);
		if (topCharTitleLabel != null && topCharTitleStringID != -1)
			topCharTitleLabel.text = stringTable.GetData(topCharTitleStringID);
		if (startButtonLabel != null && startButtonStringID != -1)
			startButtonLabel.text = stringTable.GetData(startButtonStringID);
		if (needStaminaLabel != null)
			needStaminaLabel.text = string.Format("{0}", needStamina);
	}
	
	public void SetBossInfo(BossRaidInfo info)
	{
		bossRaidInfo = info;
		
		TableManager tableManager = TableManager.Instance;
		BossRaidTable bossRaidTable = tableManager != null ? tableManager.bossRaidTable : null;
		StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
		
		string bossNameStr = "";
		string leftTimeStr = "";
		string finderNameStr = "";
		string myDamageInfoStr = "";
		string topDamageInfoStr = "";
		string topCharNameStr = "";
		string lastAttackerStr = "";
		
		if (bossNamePostfixStringID != -1)
			bossNamePostfixStr = stringTable.GetData(bossNamePostfixStringID);
		
		if (bossRaidInfo != null)
		{
			BossRaidData bossRaidData = bossRaidTable != null ? bossRaidTable.GetData(bossRaidInfo.bossID) : null;
			
			if (bossRaidData != null)
				bossNameStr = bossRaidData.bossName;

			if (bossRaidInfo.leftSec > 0)
			{
				System.DateTime nowTime = System.DateTime.Now;
				System.TimeSpan leftTime = Game.ToTimeSpan(bossRaidInfo.leftSec);
				expireTime = nowTime + leftTime;
				
				leftTimeFormat = stringTable.GetData(timeFormatStringID);
				leftTimeStr = string.Format(leftTimeFormat, leftTime.Hours, leftTime.Seconds);
			}
			else
			{
				leftTimeStr = stringTable.GetData(nonTimeFormatStringID);
			}
			
			finderNameStr = bossRaidInfo.finderName;
			
			myDamageInfoStr = string.Format("{0:###,###,##0}", bossRaidInfo.myDamage);
			
			topDamageInfoStr = string.Format("{0:###,###,##0}", bossRaidInfo.topCharDamage);
			topCharNameStr = bossRaidInfo.topCharName;
			
			lastAttackerStr = bossRaidInfo.lastAttackerName;
		}
		
		if (finderNameLabel != null)
			finderNameLabel.text = finderNameStr;
		
		if (bossNameLabel != null)
			bossNameLabel.text = string.Format("{0} {1}", bossNameStr, bossNamePostfixStr);
		
		if (timeInfoLabel != null)
			timeInfoLabel.text = leftTimeStr;
		
		if (myDamageLabel != null)
			myDamageLabel.text = myDamageInfoStr;
		
		if (topCharName != null)
			topCharName.text = topCharNameStr;
		if (topCharDamage != null)
			topCharDamage.text = topDamageInfoStr;
		
		if (lastAttackerName != null)
			lastAttackerName.text = lastAttackerStr;
	}
	
	public void Update()
	{
		if (bossRaidInfo == null)
			return;
		
		System.TimeSpan leftTime = expireTime - System.DateTime.Now;
		string leftTimeStr = "";
		
		if (leftTime.TotalSeconds <= 0)
			leftTimeStr = string.Format(leftTimeFormat, "--", "--");
		else
			leftTimeStr = string.Format(leftTimeFormat, leftTime.Hours, leftTime.Minutes);
		
		if (timeInfoLabel != null)
			timeInfoLabel.text = leftTimeStr;
	}
}
