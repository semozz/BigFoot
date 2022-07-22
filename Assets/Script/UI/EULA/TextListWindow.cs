using UnityEngine;
using System.Collections;

public class TextListWindow : MonoBehaviour {
	public UITextList textList = null;
	public UIScrollBar scroll = null;
	
	public WebText webText = null;
	
	public void Start()
	{
		if (scroll != null)
			scroll.onChange = new UIScrollBar.OnScrollBarChange(OnScrollBarChange);
		
		if (textList != null)
			textList.OnSelect(true);
	}
	
	public void OnScrollBarChange(UIScrollBar sb)
	{
		float scrollRate = 0.0f;
		
		if (sb == scroll && sb != null)
			scrollRate = sb.scrollValue;
		
		if (textList != null)
		{
			textList.OnSelect(true);
			textList.SetScrollRate(scrollRate);
		}
	}
	
}
