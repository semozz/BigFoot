using UnityEngine;
using System.Collections;

public class SpecialAchievementListWindow : BaseAchievementListWindow {

	public WebImageTexture banner = null;
	
	public void SetBanner(string url)
	{
		if (banner != null)
			banner.SetURL(url);
	}
}
