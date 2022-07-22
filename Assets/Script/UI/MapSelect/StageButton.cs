using UnityEngine;
using System.Collections;

public class StageButton : MonoBehaviour 
{
	public UISprite clear = null;
	public UISprite locked = null;
	public UISprite normal = null;	
	public UIButton button = null;	
	public string stageName = "";
	public int stageIndex = 0;
	public bool isHardMode = false;
	public MapSelect mapSelect = null;
	
	public bool specialStage = false;
	
	public enum eStageButton
	{
		Clear,
		Locked,
		Normal
	}
	public eStageButton state = eStageButton.Normal;
	
	
	void Awake()
	{
		ChangeState(state);
	}
	
	public void ChangeState(eStageButton _state)
	{
		state = _state;
		
		
		bool isEnable = false;
		
		switch(state)
		{
		case eStageButton.Clear:
			SetToggle(normal, false);
			SetToggle(locked, false);
			SetToggle(clear, true);
			
			isEnable = true;
			break;
		case eStageButton.Locked:
			SetToggle(normal, false);
			SetToggle(locked, true);
			SetToggle(clear, false);
			
			isEnable = false;
			break;
		case eStageButton.Normal:
			SetToggle(normal, true);
			SetToggle(locked, false);
			SetToggle(clear, false);
			
			isEnable = true;
			break;
		}
		
		if (button != null)
			button.isEnabled = isEnable;
	}
	
	public void SetToggle(UISprite sprite, bool bShow)
	{
		if (sprite != null)
			sprite.gameObject.SetActive(bShow);
	}
	
	public void OnClick()
	{
		if (mapSelect != null)
		{
			if (specialStage == false)
				mapSelect.OnMapStart(this.stageIndex);
			else
				mapSelect.OnSpecialMapStart(this.stageIndex);
		}
	}
}
