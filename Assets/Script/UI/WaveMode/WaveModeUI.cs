using UnityEngine;
using System.Collections;

public class WaveModeUI : MonoBehaviour {

	public FaceHP gateHP = null;
	public UILabel gateHPInfoLabel = null;
	
	public FaceHP catapultHP = null;
	public UILabel catapultHPInfoLabel = null;
	
	public WaveStepInfo waveStepInfo = null;
	
	void Awake()
	{
		GameUI.Instance.waveModeUI = this;
	}
}
