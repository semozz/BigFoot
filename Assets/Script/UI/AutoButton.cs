using UnityEngine;
using System.Collections;

public class AutoButton : MonoBehaviour {
	
	public UICheckbox autoToggle = null;
	public PlayerController player = null;
	public GameObject autoOn = null;
	
	// Use this for initialization
	void Start () {
		Game game = Game.Instance;
		player = game.player;
		CharPrivateData charPrivateData = null;
		int charIndex = 0;
		if (game.connector != null)
			charIndex = game.connector.charIndex;
		
		if (game.charInfoData != null)
			charPrivateData = game.charInfoData.GetPrivateData(charIndex);
		
		StageInfo info = null;
		if (charPrivateData != null)
			info = charPrivateData.GetStageInfo( charPrivateData.GetModeStageInfos(game.lastSelectStageType), game.stageIndex );
		
		gameObject.SetActive( isAutoStage(game, info) );
		
		if (autoToggle != null)
		{
			autoToggle.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnAutoToggle);
			autoToggle.isChecked = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private bool isAutoStage(Game game, StageInfo info)
	{
		bool result = false;
		if(game.stageManager.StageType == StageManager.eStageType.ST_WAVE)
			result = true;
		else if(game.stageManager.StageType == StageManager.eStageType.ST_FIELD)
		{
			if(info != null && info.stageInfo == StageButton.eStageButton.Clear)
				result = true;
		}
		else if(game.stageManager.StageType == StageManager.eStageType.ST_EVENT)
			result = true;
		return result;
	}
	
	public void OnAutoToggle(UICheckbox checkBox, bool bCheck)
	{
		if (player != null)
		{
			player.isAIMode = bCheck;
			player.isAutoMode = bCheck;
		 	autoOn.SetActive(bCheck);
		}
	}
}
