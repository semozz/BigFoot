using UnityEngine;
using System.Collections;

public class FaceHP : NormalHP {
	public UISprite faceSprite = null;
	
	public void SetFace(string spriteName)
	{
		if (faceSprite != null)
			faceSprite.spriteName = spriteName;
	}
	
	
}
