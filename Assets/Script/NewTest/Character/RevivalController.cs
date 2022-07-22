using UnityEngine;
using System.Collections;

public class RevivalController : MonoBehaviour {
	public PlayerController player = null;
	
	public string revivalPopupPrefab = "";
	public int revivalCount = 0;
	public float revivalDelayTime = 1.0f;
	
	public int warningStringID_1 = 16;
	public int warningStringID_2 = 22;
	
	public RevivalConfirmPopup revivalPopup = null;
	public void OnRevivalPopup()
	{
		Transform uiRoot = GameUI.Instance.uiRootPanel.transform;
		
		//Vector3 revivalMoney = Vector3.zero;
		//Vector3 ownMoney = Vector3.zero;
		int revivalJewel = 0;
		int ownJewel = 0;
		
		StringValueTable stringValueTable = null;
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
			stringValueTable = tableManager.stringValueTable;
		
		revivalJewel = stringValueTable.GetData("Revival_Jewel");
		
		ownJewel = Game.Instance.charInfoData.jewel_Value;
		
		if (revivalPopup == null)
			revivalPopup = ResourceManager.CreatePrefab<RevivalConfirmPopup>(revivalPopupPrefab, uiRoot, Vector3.zero);
		
		if (revivalPopup != null)
		{
			revivalCount++;
			
			Game.Instance.ResetPause();
			Game.Instance.Pause = true;
			
			revivalPopup.cancelButtonMessage.target = this.gameObject;
			revivalPopup.cancelButtonMessage.functionName = "OnGoTown";
			
			revivalPopup.okButtonMessage.target = this.gameObject;
			revivalPopup.okButtonMessage.functionName = "OnRequestRevival";
			
			revivalPopup.SetRevivalMoeny(ownJewel, revivalJewel);
		}
	}
	
	public string warningPopupPrefab = "SystemPopup/WarningPopup";
	public void OnRequestRevival(GameObject obj)
	{
		Transform popupNode = null;
		//Vector3 needMoney = Vector3.zero;
		int needJewel = 0;
		if (revivalPopup != null)
		{
			needJewel = revivalPopup.revivalJewel;
			popupNode = revivalPopup.popupNode;
		}
		
		CashItemType checkType = PopupBaseWindow.CheckNeedGold(0, needJewel, 0);
		if (checkType == CashItemType.None)
		{
			IPacketSender packetSender = Game.Instance.packetSender;
			if (packetSender != null)
			{
				CharInfoData charData = Game.Instance.charInfoData;
				int gold = 0;
				int cash = 0;
				if (charData != null)
				{
					gold = charData.gold_Value;
					cash = charData.jewel_Value;
				}
				
				packetSender.SendRequestRevival(gold, cash);
			}
		}
		else
		{
			if (revivalPopup != null)
				revivalPopup.OpenCashShop(checkType);

			/*
			NoticePopupWindow warningPopup = ResourceManager.CreatePrefabByResource<NoticePopupWindow>(warningPopupPrefab, popupNode, Vector3.zero);
			if (warningPopup != null)
			{
				TableManager tableManager = TableManager.Instance;
				StringTable stringTable = tableManager != null ? tableManager.stringTable : null;
				
				string infoStr = "Not Engough Jewel.";
				if (stringTable != null)
					infoStr = string.Format("{0}{1}", stringTable.GetData(warningStringID_1), stringTable.GetData(warningStringID_2));
				
				warningPopup.SetMessage(infoStr);
			}
			*/
		}
	}
	
	public void OnRevivalSuccess()
	{
		Game.Instance.Pause = false;
		
		OnCloseRevivalPopup();
		
		if (player != null)
			player.OnRevival();
	}
	
	public void OnRevivalFailed()
	{
		OnGoTown(null);
	}
	
	public void OnCloseRevivalPopup()
	{
		if (revivalPopup != null)
		{
			DestroyObject(revivalPopup.gameObject, 0.0f);
			revivalPopup = null;
		}
	}
	
	public string townStage = "";
	public void OnGoTown(GameObject obj)
	{
		CharInfoData charInfo = Game.Instance.charInfoData;
		int charIndex = Game.Instance.connector.charIndex;
		//int curLevel = 0;
		
		//GainItemInfo[] useItems = charInfo.useItems.ToArray();
		int usedPotion1 = 0;
		int usedPotion2 = 0;
		if (charInfo != null)
		{
			usedPotion1 = charInfo.usedPotion1;
			usedPotion2 = charInfo.usedPotion2;
		}
		
		IPacketSender packetSender = Game.Instance.packetSender;
		if (packetSender != null)
			packetSender.SendStageEndFailed(charIndex, usedPotion1, usedPotion2);
		
		Game.Instance.Pause = false;
		
		TownUI.openInducePopup = true;
		OnCloseRevivalPopup();
		
		LoadScene(townStage);
	}
	
	public void LoadScene(string sceneName)
	{
		Game.Instance.ResetPause();
		
		CreateLoadingPanel(sceneName);
	}
	
	public string loadingPanelPrefabPath = "";
	public LoadingPanel loadingPanel = null;
	public void CreateLoadingPanel(string stageName)
	{
		if (loadingPanel == null)
		{
			Transform uiRoot = GameUI.Instance.uiRootPanel.transform;
			loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefabPath, uiRoot, Vector3.zero);
		}
		else
		{
			loadingPanel.gameObject.SetActive(true);
			//reinforceWindow.InitMap();
		}
		
		if (loadingPanel != null)
			loadingPanel.LoadScene(stageName, null);
	}
}
