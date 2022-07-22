using UnityEngine;
using System;
using System.Collections;

public class WaveStepInfo : MonoBehaviour {
	public UILabel waveInfoLabel = null;
	public UILabel waveStepInfo = null;
	public UILabel waveMaxStepInfo = null;
	public UILabel waveTimeInfo = null;
	
	private int currentWaveStep = -1;
	private float waveTime = -1.0f;
	public float ClearWaveTime
	{
		get { return waveTime; }	
	}
	
	// Use this for initialization
	void Awake () {
		WaveManager mgr = GameUI.Instance.waveManager;
		if (mgr != null)
		{
			waveMaxStepInfo.text = string.Format("/{0:##}", mgr.GetWaveMaxCount());
			waveStepInfo.text = string.Format("{0:0#}", 1);
			
			TimeSpan timeSpan = TimeSpan.FromSeconds(0.0f);
			waveTimeInfo.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
		}
	}
	
	// Update is called once per frame
	void Update () {
		WaveManager mgr = GameUI.Instance.waveManager;
		if (mgr != null)
		{
			int curStep = mgr.CurWaveStep;
			if (currentWaveStep != curStep)
			{
				waveStepInfo.text = string.Format("{0:0#}", curStep + 1);
				currentWaveStep = curStep;
			}
			
			float _time = mgr.WaveTime;
			if (_time != waveTime)
			{
				TimeSpan timeSpan = TimeSpan.FromSeconds(_time);
				waveTimeInfo.text = string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
				waveTime = _time;
			}
		}
	}
}
