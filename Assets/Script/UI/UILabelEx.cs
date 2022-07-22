using UnityEngine;
using System.Collections;
using System.Text;



public class UILabelEx : MonoBehaviour {
	
	UILabel text;
	
	int currentValue = 0;
	int targetValue = 0;
	
	bool bEnable;
	UILabelEffectShow effectShow;
	// Use this for initialization
	void Awake () 
	{
		
		text = gameObject.GetComponent<UILabel>();
		
		if (text)
			text.text = currentValue.ToString();
		
		currentValue = System.Convert.ToInt32(text.text);
		targetValue = currentValue;
		
		bEnable = false;
		
		//effectShow = ResourceLoader.Instance.CloneComponent<UILabelEffectShow>(this.gameObject.transform.parent.transform, "UI/LabelEffectShow");
		effectShow = ResourceManager.CreatePrefab<UILabelEffectShow>("UI/LabelEffectShow", this.gameObject.transform.parent.transform);		
	}
	
	void UpdateLabel()
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!text)
			return;
		
		if (!bEnable)
			return;
		
		int diff = Mathf.Abs(currentValue - targetValue);
		float diffRate = (float)diff / (float)targetValue;
		if (diffRate < 0.1f || diff <= 0.01f)
		{
			currentValue = targetValue;
			
			bEnable = false;
		}
		else
		{
			currentValue = (int)Mathf.Lerp((float)currentValue, (float)targetValue, 0.4f);
		}	
		
		text.text = string.Format("{0:N0}", currentValue);
	}
	
	public void SetValue(int val)
	{
		SetValue(val, true);
	}
	
	public void SetValue(int val, bool increaseEffect)
	{
		
		if (!increaseEffect)
		{
			targetValue = currentValue = val;
			bEnable = false;
			
			if (text)
				text.text = string.Format("{0:N0}", currentValue);
			
			return;
		}
		
		if (currentValue == val)
		{
			targetValue = currentValue = val;
			bEnable = false;
			
			if (text)
				text.text = string.Format("{0:N0}", currentValue);
				
			return;
		}
			
		bEnable = true;
		
		targetValue = val;			
		
		if (effectShow)
			effectShow.SetValue(currentValue, targetValue);
	}
	
	public void ClearEffect()
	{
		if (effectShow != null)
			effectShow.OnLabelEffectEnd();
	}

}
