using UnityEngine;
using System.Collections;

public class EventCondition : MonoBehaviour {
	public enum eEventCondition
	{
		None,
		Equal,
		Greater,
		GreaterThen,
		Less,
		LessThen,
	}
	
	public string ConditionName = "Condition##";
	public eEventCondition Condition = eEventCondition.None;
	public int ConditionValue = 0;
	public int TargetConditionValue = 0;
	
	private bool isComplete = false;
	public bool IsComplete
	{
		get { return isComplete; }	
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isComplete == true)
			return;
		
		if (OnCondtionCheck() == true)
		{
			isComplete = true;
			OnComplete();
		}
	}
	
	public void AddCondtionValue(int addValue)
	{
		ConditionValue += addValue;
	}
	
	public void SetConditinValue(int _value)
	{
		ConditionValue = _value;
	}
	
	public bool OnCondtionCheck()
	{
		bool bConditionCheck = false;
		switch(Condition)
		{
		case eEventCondition.None:
			break;
		case eEventCondition.Equal:
			bConditionCheck = (ConditionValue == TargetConditionValue);
			break;
		case eEventCondition.Greater:
			bConditionCheck = (ConditionValue > TargetConditionValue);
			break;
		case eEventCondition.GreaterThen:
			bConditionCheck = (ConditionValue >= TargetConditionValue);
			break;
		case eEventCondition.Less:
			bConditionCheck = (ConditionValue < TargetConditionValue);
			break;
		case eEventCondition.LessThen:
			bConditionCheck = (ConditionValue <= TargetConditionValue);
			break;
		}
		
		return bConditionCheck;
	}
	
	public void OnComplete()
	{
		//Debug.Log(ConditionName + " is Complete... " + ConditionValue + " " + Condition + " " + TargetConditionValue);
	}
	
	public void ForceComplete()
	{
		ConditionValue = TargetConditionValue;	
	}
}
