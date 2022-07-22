using UnityEngine;
using System.Collections;

public class LevelUpEventWindow : BaseEventWindow {

	public override void OnBack ()
	{
		base.OnBack ();
		
		DestroyObject(this.gameObject, 0.2f);
		
		CharInfoData charData = Game.Instance.charInfoData;
		int charIndex = -1;
		if (Game.Instance.connector != null)
			charIndex = Game.Instance.connector.charIndex;
		
		CharPrivateData privateData = null;
		if (charData != null)
			privateData = charData.GetPrivateData(charIndex);
		
		if (privateData != null)
			privateData.levelupRewardEventCheck = 0;
		
		TownUI townUI = GameUI.Instance.townUI;
		if (townUI != null)
			townUI.OnEnterTown();
	}
}
