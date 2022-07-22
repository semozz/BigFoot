using UnityEngine;
using System.Collections;

public class WebImageTexture : MonoBehaviour {
	public GameObject defaultObj = null;
	public UITexture sprite = null;
	public string imageURL = "http://www.test.com/test";
	
	// Use this for initialization
	void Start () {
		if (sprite == null)
		{
			sprite = GetComponent<UITexture>();
			if (sprite == null)
				return;
		}
		
		if (defaultObj != null)
			defaultObj.SetActive(true);
		
		if (imageURL != "")
		{
			if (sprite != null)
				sprite.enabled = true;
			
			StartCoroutine(LoadImage(imageURL));
		}
		else
		{
			if (sprite != null)
				sprite.enabled = false;
		}
	}
	
	public IEnumerator LoadImage(string url)
	{
		WWW loader = new WWW(url); 
		yield return loader;
		
		if (string.IsNullOrEmpty(loader.error) == false)
		{
			
		}
		else
		{
			var texture = loader.texture;
			
			if (texture != null)
			{
				Color[] pixels = texture.GetPixels();
				
				if (texture.height < 10)
				{
					sprite.enabled = false;
					
					if (defaultObj != null)
						defaultObj.SetActive(true);
				}
				else
				{
					sprite.enabled = true;
					
					if (defaultObj != null)
						defaultObj.SetActive(false);
					
					if (sprite != null)
						sprite.mainTexture = texture;
					
					UIButtonMessage buttonMessage = this.GetComponent<UIButtonMessage>();
					if (buttonMessage != null && buttonMessage.target != null)
						buttonMessage.target.SendMessage("LoadComplete", SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		
		loader.Dispose();
	}
	
	public void SetURL(string url)
	{
		imageURL = url;

		if (url == "")
		{
			sprite.enabled = false;
					
			if (defaultObj != null)
				defaultObj.SetActive(true);
			return;
		}
		
		StartCoroutine(LoadImage(imageURL));
	}
}
