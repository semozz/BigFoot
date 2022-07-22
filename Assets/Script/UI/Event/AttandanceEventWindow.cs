using UnityEngine;
using System.Collections;

public class AttandanceEventWindow : BaseEventWindow {
	
	public override void OnBack ()
	{
		base.OnBack ();
		
		DestroyObject(this.gameObject, 0.2f);
		
		CharInfoData charData = Game.Instance.charInfoData;
		if (charData != null)
			charData.attandanceCheck = 0;
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
			townUI.OnEnterTown();
	}
}
