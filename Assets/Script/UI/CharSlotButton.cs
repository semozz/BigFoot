using UnityEngine;
using System.Collections;

public class CharSlotButton : MonoBehaviour {

	public UISprite charSlotSprite = null;
	public GameDef.ePlayerClass _class = GameDef.ePlayerClass.CLASS_NONE;
	
	void Awake()
	{
		if (charSlotSprite != null)
		{
			charSlotSprite.spriteName = "";
		}
	}
}
