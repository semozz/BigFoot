using UnityEngine;
using System.Collections;

public class DropGoldUI : BaseDamageUI {
	public UILabel infoLabel = null;
	public UISprite goldSprite = null;
	
	public Animation anim = null;
	public string normalAnim = "";
	public void SetGold(int gold)
	{
		string infoStr = string.Format("{0}", gold);
		if (infoLabel != null)
			infoLabel.text = infoStr;
		
		if (anim != null)
		{
			anim.Play(normalAnim);
		}
	}
	
}
