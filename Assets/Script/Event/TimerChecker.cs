using UnityEngine;
using System.Collections;

public class TimerChecker : EventConditionTrigger {
	public Timer checkTimer = null;
	
	// Update is called once per frame
	private bool isChecked = false;
	
	void Update () {
		if (IsActivate == true)
		{
			if (checkTimer != null && checkTimer.IsActivate == true)
			{
				System.TimeSpan leftTime = checkTimer.GetLeftTime();
				if (isChecked == false &&
					leftTime.TotalSeconds <= 0)
				{
					OnChangeValue(1);
					isChecked = true;
				}
			}
		}
	}
}
