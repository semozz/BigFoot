using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	public int coolTime = 15;
	
	public float leftTime = 0;
	
	private bool isActvate = false;
	public bool IsActivate
	{
		get { return isActvate; }
	}
	// Use this for initialization
	void Start () {
		
		//OnActive();
	}
	
	public void Update()
	{
		if (Game.Instance.Pause == true)
			return;
		
		if (isActvate == true && isDeactivate == false)
			leftTime -= Time.deltaTime;
	}
	
	public void OnActivate()
	{
		isActvate = true;
		
		leftTime = (float)coolTime;
	}
	
	private bool isDeactivate = false;
	public void OnDeactivate()
	{
		isDeactivate = true;
	}
	
	public System.TimeSpan GetLeftTime()
	{
		System.TimeSpan leftTimeSpan = Game.ToTimeSpan(leftTime);
		
		return leftTimeSpan;
	}
}
