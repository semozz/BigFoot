using UnityEngine;
using System.Collections;

public class MyCharInfo : MonoBehaviour {

	public UILabel charLevelLabel = null;
	public UILabel charClassLabel = null;
	public UILabel charNameLabel = null;
	
	
	public void SetCharInfo(int level, GameDef.ePlayerClass classType, string charName)
	{
		if (charLevelLabel != null)
			charLevelLabel.text = string.Format("Lv. {0}", level);
		if (charClassLabel != null)
			charClassLabel.text = GameDef.MakeCharClassToString(classType);
		if (charNameLabel != null)
			charNameLabel.text = charName;
	}
}
