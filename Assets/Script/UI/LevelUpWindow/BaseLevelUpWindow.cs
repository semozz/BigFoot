using UnityEngine;
using System.Collections;

public class BaseLevelUpWindow : MonoBehaviour {
	public GameObject parentObj = null;
	
	public UILabel curLevelLabel = null;
	public UILabel charNameLabel = null;
	public UILabel userIDLabel = null;
	
	public float lifeTime = 2.0f;
	
	public virtual void Start()
	{
		if (this.audio != null)
			this.audio.mute = !GameOption.effectToggle;
	}
	
	public void SetCharInfo(int curLevel, string charName, string userID)
	{
		if (curLevelLabel != null)
			curLevelLabel.text = string.Format("Lv.{0}", curLevel);
		
		if (charNameLabel != null)
			charNameLabel.text = charName;
		
		if (userIDLabel != null)
			userIDLabel.text = userID;
	}
	
	public void Update()
	{
		lifeTime -= Time.deltaTime;
		
		if (lifeTime <= 0.0f)
		{
			if (parentObj != null)
				parentObj.SendMessage("OnLevelUpFinished", SendMessageOptions.DontRequireReceiver);
			
			DestroyObject(this.gameObject, 0.0f);
		}
	}
}
