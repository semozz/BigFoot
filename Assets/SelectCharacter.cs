using UnityEngine;
using System.Collections;

public class SelectCharacter : MonoBehaviour {
	public string warriorSceneName = "131219_Warrior";
	public string assassinSceneName = "131219_Assassin";
	public string wizardSceneName = "131219_Wizard";
	
	public string selectCharacterScene = "SelectCharacter";
	
	public GameObject avatarPanel = null;
	
	public Transform popupNode = null;
	
	public void OnLoadWarrior()
	{
		Game.Instance.playerClass = GameDef.ePlayerClass.CLASS_WARRIOR;
		LoadScene("TownTest");
	}
	
	public void OnLoadAssassin()
	{
		Game.Instance.playerClass = GameDef.ePlayerClass.CLASS_ASSASSIN;
		LoadScene("TownTest");
	}
	
	public void OnLoadWizard()
	{
		Game.Instance.playerClass = GameDef.ePlayerClass.CLASS_WIZARD;
		LoadScene("TownTest");
	}
	
	public void OnSelectCharacter()
	{
		LoadScene(selectCharacterScene);
	}
	
	public void LoadScene(string sceneName)
	{
		Game.Instance.ResetPause();
		if (avatarPanel != null)
			avatarPanel.SetActive(false);
		
		//Application.LoadLevelAsync(sceneName);
		CreateLoadingPanel(sceneName);
	}
	
	public void OnGoTown()
	{
		//Game.Instance.playerClass = 
		LoadScene("TownTest");
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
			//reinforceWindow.InitMap();
		}
		
		if (loadingPanel != null)
			loadingPanel.LoadScene(stageName, null);
	}
}
