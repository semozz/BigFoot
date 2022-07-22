using UnityEngine;
using System.Collections;

public class WebText : MonoBehaviour {
	public UILabel message = null;
	public UITextList textList = null;
	
	public string resourceURL = "http://urlprofiler.com/eula/";
	
	// Use this for initialization
	void Start () {
		if (resourceURL != "")
			StartCoroutine(LoadText(resourceURL));
	}
	
	public IEnumerator LoadText(string url)
	{
		WWW loader = new WWW(url);
		yield return loader;
		
		if (message != null)
			message.text = loader.text;
		else if (textList != null)
			textList.Add(loader.text);
		
		
		loader.Dispose();
	}
	
	public void SetURL(string url)
	{
		resourceURL = url;
		
		if (url == "")
			return;
		
		StartCoroutine(LoadText(resourceURL));
	}
}
