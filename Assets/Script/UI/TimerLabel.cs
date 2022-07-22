using UnityEngine;
using System.Collections;

public class TimerLabel : MonoBehaviour {
	public UILabel timerInfo = null;
	
	public Timer timer = null;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		System.TimeSpan leftTime = new System.TimeSpan(0);
		if (timer != null)
			leftTime = timer.GetLeftTime();
		
		string timeInfoStr = string.Format("{0:D2}:{1:D2}:{2:D2}", leftTime.Hours, leftTime.Minutes, leftTime.Seconds);
		
		if (timerInfo != null)
			timerInfo.text = timeInfoStr;
	}
}
