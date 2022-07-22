using UnityEngine;
using System.Collections;

public class BuyAwakeningPointPopup : BasePopup {
	public AwakeningLevelWindow parentWindow = null;
	
	public AwakeningLevelManager levelManager = null;
	
	public UILabel availablePointTitleLabel = null;
	public UILabel availablePointLabel = null;
	
	public UILabel buyPointLabel = null;
	
	public UILabel needJewelInfoLabel = null;
	
	public int buyPoint = 0;
	public int needJewel = 0;
	
	public void InitWindow(AwakeningLevelManager levelManager)
	{
		this.levelManager = levelManager;
		this.buyPoint = 1;
		
		UpdatePointInfo();
	}
	
	public void UpdatePointInfo()
	{
		int availableBuyPoint = 0;
		needJewel = 0;
		
		if (this.levelManager != null)
		{
			availableBuyPoint = Mathf.Max(0, this.levelManager.GetAvailableBuyPoint() - buyPoint);
			needJewel = this.levelManager.GetNeedJewel(buyPoint);
		}
		
		string availablePointInfoStr = string.Format("{0:#,###,##0}", availableBuyPoint);
		string buyPointInfoStr = string.Format("{0:#,###,##0}", buyPoint);
		string needJewelInfoStr = string.Format("{0:#,###,##0}", needJewel);
		
		if (availablePointLabel != null)
			availablePointLabel.text = availablePointInfoStr;
		if (buyPointLabel != null)
			buyPointLabel.text = buyPointInfoStr;
		if (needJewelInfoLabel != null)
			needJewelInfoLabel.text = needJewelInfoStr;
	}
	
	public void IncBuyPoint()
	{
		int availableBuyPoint = 0;
		if (levelManager != null)
			availableBuyPoint = this.levelManager.GetAvailableBuyPoint();
		
		buyPoint = Mathf.Min(availableBuyPoint, buyPoint + 1);
		
		UpdatePointInfo();
	}
	
	public void DecBuyPoint()
	{
		buyPoint = Mathf.Max(1, buyPoint - 1);
		
		UpdatePointInfo();
	}
	
	public void MaxBuyPoint()
	{
		int availableBuyPoint = 0;
		if (levelManager != null)
			availableBuyPoint = this.levelManager.GetAvailableBuyPoint();
		
		buyPoint = availableBuyPoint;
		
		UpdatePointInfo();
	}
	
	public void ResetBuyPoint()
	{
		buyPoint = 1;
		
		UpdatePointInfo();
	}
}
