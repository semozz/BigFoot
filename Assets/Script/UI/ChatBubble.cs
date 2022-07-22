using UnityEngine;
using System.Collections;

public class ChatBubble : MonoBehaviour {
	public UILabel msgText = null;
	public UISprite msgBG = null;
	
	public float lifeTime = -1.0f;
	public float curLifeTime = -1.0f;
	
	public bool bDestroy = true;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (lifeTime > 0.0f)
		{
			curLifeTime -= Time.deltaTime;
			float timeRate = curLifeTime / lifeTime;
			
			if (timeRate <= 0.3f)
			{
				if (msgText != null)
					msgText.alpha = timeRate;
				
				if (msgBG != null)
					msgBG.alpha = timeRate;
				
				if (timeRate <= 0.0f)
					this.gameObject.SetActive(false);
			}
		}
	}
	
	public void SetMsg(string msg, float time)
	{
		if (msgText != null)
		{
			if (msg == null)
				msgText.text = "";
			else
				msgText.text = msg;
		}
		
		curLifeTime = lifeTime = time;
		
		if (msgText != null)
			msgText.alpha = 1.0f;
		
		if (msgBG != null)
			msgBG.alpha = 1.0f;
		
		if (time != -1.0f)
		{
			if (bDestroy == true)
				DestroyObject(this.gameObject, lifeTime);
		}
	}
}
