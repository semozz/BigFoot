using UnityEngine;
using System.Collections;

public class EventConditionTrigger : MonoBehaviour {
	public EventCondition _EventCondtion = null;
	
	public bool isActivate = false;
	public bool IsActivate
	{
		get { return isActivate; }	
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void OnChangeValue(int addValue)
	{
		if (isActivate == false)
			return;
		
		if (_EventCondtion != null)
			_EventCondtion.AddCondtionValue(addValue);
	}
	
	public virtual void OnComplete(int _value)
	{
		if (isActivate == false)
			return;
		
		if (_EventCondtion != null)
			_EventCondtion.SetConditinValue(_value);
	}
	
	public virtual void OnActivate()
	{
		isActivate = true;
	}
	
	public virtual void OnComplete()
	{
		
	}
	
	public virtual void UnActivate()
	{
		
	}
}
