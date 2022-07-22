using UnityEngine;
using System.Collections;

public class EventTask : MonoBehaviour {
	public float DelayTime = 1.0f;
	public bool isActivate = false;
	
	protected bool isComplete = false;
	public bool IsComplete
	{
		get { return isComplete; }	
	}
	
	public bool isGamePause = false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (isActivate == false)
			return;
		
		DelayTime -= Time.deltaTime;
		if (DelayTime <= 0.0f)
		{
			isComplete = true;	
		}
	}
	
	
	public virtual void DoStart()
	{
		isActivate = true;
		
		if (isGamePause == true)
			Game.Instance.Pause = true;
	}
	
	public virtual void DoEnd()
	{
		isActivate = false;
		
		EventTriggerCall toDoTask = gameObject.GetComponent<EventTriggerCall>();
		if (toDoTask != null)
			toDoTask.OnTriggerEvent();
		
		if (isGamePause == true)
			Game.Instance.Pause = false;
	}
}
