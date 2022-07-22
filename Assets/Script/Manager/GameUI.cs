using UnityEngine;
using System.Collections;

public class GameUI : Singleton<GameUI> 
{
	public UIRootPanel uiRootPanel = null;
	public UIPanel root = null;
	
	public TownUI townUI = null;
	public ShopWindow shopWindow = null;
	public StorageWindow storageWindow = null;
	public CashShopWindow cashShopWindow = null;
	public LoginPage loginPage = null;
	public GambleProgressWindow gambleProgressWindow = null;
	public GambleWindow gambleWindow = null;
	public WaveManager waveManager = null;
	public NoticePopupWindow messageBox = null;
	
	public WaveWindow waveWindow = null;
	public WaveStartWindow waveStartWindow = null;
	public WaveModeUI  waveModeUI = null;
	
	public UIMyStatusInfo myStatusInfo = null;
	
	//public MapSelect mapSelect = null;
	public MapStartWindow mapStartWindow = null;
	public SpecialMapStartWindow specialMapStartWindow = null;
	
	public MasteryWindow_New masteryWindow = null;
	
	public ArenaWindow arenaWindow = null;
	
	public MyCharInfos myCharInfos = null;
	
	public PostWindow postWindow = null;
	
	public FriendWindow friendWindow = null;
	
	public AchievementWindow achievementWindow = null;
	
	public BossRaidWindow bossRaidWindow = null;
	
	public FriendInviteWindow friendInviteWindow = null;
	
	public SignUpWindow signupWindow = null;
	
	public CouponWindow couponWindow = null;
	
	public OptionWindow optionWindow = null;
	
	public EventShopWindow eventShopWindow = null;
	
	public PackageItemShopWindow packageItemShopWindow = null;
	
	public AwakeningLevelWindow awakeningWindow = null;
	
	public RandomBoxEventWindow randomBoxWindow = null;
	
	public string warningPopupPrefab = "SystemPopup/WarningPopup";
	public NoticePopupWindow MessageBox
    {
        get { 
			
			if (!this.messageBox)
			{
				if (!uiRootPanel)
				{
					uiRootPanel = GameObject.FindObjectOfType(typeof(UIRootPanel)) as UIRootPanel;
					
					if (!uiRootPanel)
						return null;
				}					

				this.messageBox = ResourceManager.CreatePrefabByResource<NoticePopupWindow>(warningPopupPrefab, uiRootPanel.popUpNode, Vector3.zero);
			}
			
			return this.messageBox; 
		}

    }
	
	public GameObject waitPanel = null;
	public void DoWait()
	{
		if (waitPanel == null)
		{
			Transform popupNode = null;
			if (uiRootPanel != null)
				popupNode =uiRootPanel.popUpNode;
			
			waitPanel = ResourceManager.CreatePrefab("UI/Wait", popupNode);
		}
	}
	
	public void CancelWait()
	{
		if (waitPanel != null)
		{
			GameObject.DestroyObject(waitPanel.gameObject, 0.0f);
			waitPanel = null;
		}
	}
	
	public PopupBaseWindow currentWindow = null;
	public void SetCurrentWindow(PopupBaseWindow window)
	{
		currentWindow = window;
	}
}
