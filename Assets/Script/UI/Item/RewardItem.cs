using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RewardItem : MonoBehaviour {
	public UISprite Sprite = null;
	public UILabel Label = null;
	
	public void SetItem(string spriteName, int num)
	{
		if( Sprite != null )
			Sprite.spriteName = spriteName;
		
		if( Label != null )
		{
			if( num > 1 )
				Label.text = string.Format("{0:#,###,##0}", num);
			else
				Label.text = "";
		}
	}
}
