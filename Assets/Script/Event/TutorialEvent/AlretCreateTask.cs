using UnityEngine;
using System.Collections;

public class AlretCreateTask : TutorialTask
{
	public string uiPrefab = "";
	
	public TutorialAlretWindow alretWindow = null;
	
	public override void DoStart ()
	{
		base.DoStart ();
		
		string prefabPath = string.Format("UI/Tutorial/{0}", uiPrefab);
		
		Transform uiRoot = null;
		if (GameUI.Instance.uiRootPanel != null)
			uiRoot = GameUI.Instance.uiRootPanel.popUpNode;
		
		alretWindow = ResourceManager.CreatePrefab<TutorialAlretWindow>(prefabPath, uiRoot);
		
		int charIndex = -1;
		if (isCommon == false)
		{
			charIndex = 0;
			
			if (Game.Instance != null && Game.Instance.connector != null)
				charIndex = Game.Instance.connector.charIndex;
		}
		
		if (alretWindow != null)
		{
			alretWindow.task = this;
			alretWindow.SetActiveIndex(charIndex);
		}
	}
	
	public override void DoEnd ()
	{
		base.DoEnd ();
		
		if (alretWindow != null)
		{
			DestroyObject(alretWindow.gameObject, 0.2f);
			alretWindow = null;
		}
	}
	
	public override void OnSkip ()
	{
		base.OnSkip ();
		
		if (lifeTime > 0.0f)
		{
			if (delayTime <= 1.0f)
				delayTime = 0.0f;
		}
	}
}
