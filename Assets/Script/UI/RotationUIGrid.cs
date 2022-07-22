using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIPanel))]
public class RotationUIGrid : MonoBehaviour {
	
	public delegate void onSetIndex(int i, bool isInc);
	public onSetIndex OnSetIndex;
	
	private UIPanel panelObject;
	public UIGrid gridObject = null;
	
	private int index;
	private float panelY;
	private float pastY;
	private int buffer;
	public int maxNum;
	// Use this for initialization
	void Start () {
		panelObject = GetComponent<UIPanel>();
		
		index = 0;
		buffer = 2;
		panelY = panelObject.transform.localPosition.y;
		pastY = panelY;
	}
	
	// Update is called once per frame
	void Update () {
		while(index <= maxNum+buffer && panelObject.transform.localPosition.y > pastY &&
			panelObject.transform.localPosition.y - panelY > gridObject.cellHeight*(index+4))
		{
			if(index >= buffer && index < maxNum+buffer)
			{
				if(OnSetIndex != null)
					OnSetIndex(index-buffer+1, true);
			}
			index++;
		}
		
		while(index >= 0 && panelObject.transform.localPosition.y < pastY &&
			panelObject.transform.localPosition.y - panelY < gridObject.cellHeight*(index))
		{
			if(index >= buffer && index < maxNum+buffer)
			{
				if(OnSetIndex != null)
					OnSetIndex(index-buffer, false);
			}
			index--;
		}
		pastY = panelObject.transform.localPosition.y;
	}
	
	public void Reset()
	{
		index = 0;	
	}
}
