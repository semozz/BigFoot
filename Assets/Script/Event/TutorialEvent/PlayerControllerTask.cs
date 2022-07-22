using UnityEngine;
using System.Collections;

public class PlayerControllerTask : TutorialTask
{
	public int tutorialControllerStep = 0;
	
	public override void DoStart ()
	{
		base.DoStart ();
		
		PlayerControlButtonPanel playerControls = GameObject.FindObjectOfType(typeof(PlayerControlButtonPanel)) as PlayerControlButtonPanel;
		if (playerControls != null)
			playerControls.SetTutorialMode(tutorialControllerStep);
	}
}
