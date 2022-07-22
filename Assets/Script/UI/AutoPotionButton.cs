using UnityEngine;
using System.Collections;

public class AutoPotionButton : MonoBehaviour {
	
	public UICheckbox autoToggle = null;
	public PlayerController player = null;
	public GameObject autoOn = null;
	private bool potionEnableStage = false;
	
	void Start () {
		Game game = Game.Instance;
		player = game.player;
		StageManager stageManager = GameObject.FindObjectOfType(typeof(StageManager)) as StageManager;
		PlayerControlButtonPanel buttonPanel = GameObject.FindObjectOfType(typeof(PlayerControlButtonPanel)) as PlayerControlButtonPanel;
		
		potionEnableStage = false;
		if (stageManager != null && buttonPanel != null)
			potionEnableStage = buttonPanel.IsPotionEnableStage(stageManager.StageType);
		
		gameObject.SetActive(potionEnableStage);
		
		if (autoToggle != null)
		{
			autoToggle.onStageChangeArg2 = new UICheckbox.OnStateChangeArg2(OnAutoToggle);
			autoToggle.isChecked = false;
		}
	}
	
	void Update () {
	
	}
	
	public void OnAutoToggle(UICheckbox checkBox, bool bCheck)
	{
		if (player != null)
		{
			player.isAutoPotionMode = bCheck;
		 	autoOn.SetActive(bCheck);
		}
	}
}
