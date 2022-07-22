using UnityEngine;
using System.Collections;

public class ComboCounter : MonoBehaviour {
	public int comboCount = 0;
	public float lastTime = -1.0f;
	public float limitTime = 2.5f;
	
	LifeManager lifeManager = null;
	void Start()
	{
		lifeManager = this.gameObject.GetComponent<LifeManager>();
	}
	
	public void AddComboCount()
	{
		if (lastTime == -1.0f)
		{
			ApplyCombo();
		}
		else
		{
			float delta = Time.time - lastTime;
			if (delta <= limitTime)
			{
				ApplyCombo();
			}
			else
			{
				comboCount = 0;
				ApplyCombo();
				
				lastTime = -1.0f;
			}
		}
		
		//string comboStr = string.Format("Combo : {0}", comboCount);
		//Debug.LogWarning(comboStr);
	}
	
	public void ResetCombo()
	{
		comboCount = 0;
		lastTime = -1.0f;
		
		if (lifeManager != null)
			lifeManager.DeleteComboUI();
	}
	
	public void ApplyCombo()
	{
		comboCount++;
		lastTime = Time.time;
		
		if (lifeManager != null)
			lifeManager.ApplyComboCount(comboCount);
	}
}
