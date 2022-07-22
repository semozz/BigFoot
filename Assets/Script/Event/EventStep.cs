using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventStep : MonoBehaviour {
	private List<EventCondition> Conditions = new List<EventCondition>();
	private List<EventConditionTrigger> ConditionTargetObjects = new List<EventConditionTrigger>();
	
	public string EventStepName = "EventStep##";
	protected bool isActivate = false;
	protected bool isComplete = false;
	public bool IsComplete
	{
		get { return isComplete; }
	}
	
	public bool isRequireGamePause = false;
	
	public bool isAutoActivate = false;
	public int index = 0;
	
	private bool isScaneChild = false;
	
	public enum eEventStepState
	{
		None = -1,
		Begin,
		Doing,
		End,
		Max_Count,
	}
	protected eEventStepState _StepState = eEventStepState.None;
	
	public EventTask BeginTask = null;
	public EventTask DoingTask = null;
	public EventTask EndTask = null;
	
	public static int CompareIndices(EventStep a, EventStep b)
	{
		return a.index - b.index;
	}
	
	// Use this for initialization
	public virtual void Start () {
		/*
		if (this.isAutoActivate == true)
		{
			this.isActivate = true;
			ChangeState(EventStep.eEventStepState.Begin);
		}
		*/
	}
	
	void Awake()
	{
		ScaneChild();	
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (isActivate == false)
			return;
				
		bool isCompleteTask = false;
		
		switch(_StepState)
		{
		case eEventStepState.Begin:
			isCompleteTask = IsCompleteTask(BeginTask);
			if (isCompleteTask == true)
			{
				ChangeState(EventStep.eEventStepState.Doing);
			}
			break;
		case eEventStepState.Doing:
			isCompleteTask = IsCompleteTask(DoingTask);
			if (isCompleteTask == true && CheckCondition() == true)
			{
				ChangeState(EventStep.eEventStepState.End);
			}
			break;
		case eEventStepState.End:
			isCompleteTask = IsCompleteTask(EndTask);
			if (isCompleteTask == true)
			{
				isComplete = true;
				ChangeState(EventStep.eEventStepState.None);
			}
			break;
		}
	}
	
	private bool IsCompleteTask(EventTask task)
	{
		if (task == null)
			return true;
		else
			return task.IsComplete;
	}
	
	public void ScaneChild()
	{
		if (isScaneChild == false)
		{
			isScaneChild = true;
		
			ScanConditionObjects();
			ScanConditionTargets();
		}
	}
	
	private void ScanConditionObjects()
	{
		Conditions.Clear();

		EventCondition obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(EventCondition), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			obj = (EventCondition)objs[i];

			Conditions.Add(obj);
		}
	}
	private void ScanConditionTargets()
	{
		ConditionTargetObjects.Clear();

		EventConditionTrigger obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(EventConditionTrigger), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			obj = (EventConditionTrigger)objs[i];

			ConditionTargetObjects.Add(obj);
		}
	}
	
	public virtual void OnActivate()
	{
		isActivate = true;
		isComplete = false;
		
		ChangeState(EventStep.eEventStepState.Begin);
	}
	
	protected void ChangeState(EventStep.eEventStepState state)
	{
		if (this._StepState == state)
			return;
		
		switch(_StepState)
		{
		case eEventStepState.Begin:
			if (BeginTask != null)
				BeginTask.DoEnd();
			break;
		case eEventStepState.Doing:
			if (DoingTask != null)
				DoingTask.DoEnd();
			break;
		case eEventStepState.End:
			if (EndTask != null)
				EndTask.DoEnd();
			
			OnCompleteStep();
			break;
		}
		
		switch(state)
		{
		case eEventStepState.Begin:
			DoStart();
			
			if (BeginTask != null)
				BeginTask.DoStart();
			break;
		case eEventStepState.Doing:
			if (DoingTask != null)
				DoingTask.DoStart();
			break;
		case eEventStepState.End:
			if (EndTask != null)
				EndTask.DoStart();
			break;
		}
		
		_StepState = state;
	}
	
	private void DoStart()
	{
		foreach(EventConditionTrigger trigger in ConditionTargetObjects)
		{
			if (trigger != null)
				trigger.OnActivate();
		}		
		
		if (isRequireGamePause == true)
		{
			Game.Instance.Pause = true;
			Game.Instance.InputPause = true;
		}
	}
	
	private bool CheckCondition()
	{
		foreach(EventCondition condition in Conditions)
		{
			if (condition != null && condition.IsComplete == false)
				return false;
		}
		
		return true;
	}
	
	public virtual void OnCompleteStep()
	{
		foreach(EventConditionTrigger trigger in ConditionTargetObjects)
		{
			if (trigger != null)
				trigger.OnComplete();
		}
		
		isActivate = false;
		isComplete = true;
		
		if (isRequireGamePause == true)
		{
			Game.Instance.Pause = false;
			Game.Instance.InputPause = false;
		}
	}
	
	public void UnActivate()
	{
		foreach(EventConditionTrigger trigger in ConditionTargetObjects)
		{
			if (trigger != null)
				trigger.UnActivate();
		}
	}
}
