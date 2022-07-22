using UnityEngine;
using System.Collections;

public class SelectAvatarWindow : MonoBehaviour {

	public GameDef.ePlayerClass playerClass = GameDef.ePlayerClass.CLASS_NONE;
	public GameObject startGame = null;
	public Transform popupNode = null;
	
	public GameObject assassinInfo = null;
	public GameObject warriorInfo = null;
	public GameObject wizardInfo = null;
	
	public string loadingSoundPrefabPath = "Effect/AudioListener";
	
	public void Start()
	{
		if (startGame != null)
			startGame.SetActive(false);
		
		if (GameOption.bgmToggle)
		{
			LoadingSound loadingSound = FindObjectOfType(typeof(LoadingSound)) as LoadingSound;
			if (loadingSound == null)
			{
				loadingSound = ResourceManager.CreatePrefab<LoadingSound>(loadingSoundPrefabPath);
				if (loadingSound != null)
					loadingSound.DestroyStamp();
			}
		}
		
#if UNITY_ANDROID && !UNITY_EDITOR
 		Game.Instance.AndroidManager.CallUnityUpdateKakaoFriends();
#endif	
	}
	
	public void SetSelectedClass(GameDef.ePlayerClass classType)
	{
		if (playerClass == classType)
			return;
		
		setPlayerClass(classType);
		
		bool bSelected = (playerClass != GameDef.ePlayerClass.CLASS_NONE);
		if (startGame != null)
			startGame.SetActive(bSelected);
	}
	
	private void setPlayerClass(GameDef.ePlayerClass classType)
	{
		playerClass = classType;
		if( assassinInfo != null )
			assassinInfo.SetActive(classType == GameDef.ePlayerClass.CLASS_ASSASSIN);
		if( warriorInfo != null )
			warriorInfo.SetActive(classType == GameDef.ePlayerClass.CLASS_WARRIOR);
		if( wizardInfo != null )
			wizardInfo.SetActive(classType == GameDef.ePlayerClass.CLASS_WIZARD);
	}
	
	public string loadingPanelPrefab = "UI/LoadingPanel";
	public string loadStageName = "TownTest";
	public string tutorialStageName = "TutorialMap";
	public void OnStartGame(GameObject obj)
	{		
		LoadingSound loadingSound = FindObjectOfType(typeof(LoadingSound)) as LoadingSound;
		if (loadingSound != null)
			DestroyObject(loadingSound.gameObject);
		
		Game.Instance.playerClass = playerClass;
		
		int charIndex = (int)playerClass;
		LoginInfo loginInfo = Game.Instance.loginInfo;
		if (loginInfo != null)
			loginInfo.charIndex = charIndex;

        Game.Instance.packetSender.SelectHero(charIndex);
		Game.Instance.connector.charIndex = charIndex;
		
		CharInfoData charData = Game.Instance.charInfoData;
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		string stageName = loadStageName;
		
		if (privateData != null && privateData.baseInfo.tutorial == 0)
			stageName = tutorialStageName;
		
		LoadingPanel loadingPanel = ResourceManager.CreatePrefab<LoadingPanel>(loadingPanelPrefab, popupNode, Vector3.zero);
		if (loadingPanel != null)
			loadingPanel.LoadScene(stageName, null);
	}
}
