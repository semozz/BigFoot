using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour {
	private List<EventStep> eventSteps = new List<EventStep>();
	
	private EventStep currentStep = null;
	private int currentStepIndex = -1;
	
	private bool isActivate = false;
	private bool isComplete = false;
	// Use this for initialization
	void Start () {
		
	}
	
	void Awake()
	{
		ScanEventSteps();
	}
	
	private void ScanEventSteps()
	{
		eventSteps.Clear();

		EventStep obj;
		Component[] objs = transform.GetComponentsInChildren(typeof(EventStep), true);

		for (int i = 0; i < objs.Length; ++i)
		{
			obj = (EventStep)objs[i];
			
			obj.ScaneChild();
			obj.UnActivate();
			
			eventSteps.Add(obj);
		}
		
		eventSteps.Sort(EventStep.CompareIndices);
	}
	
	// Update is called once per frame
	void Update () {
		if (isActivate == false || isComplete == true)
			return;
		
		if (currentStep != null)
		{
			if (currentStep.IsComplete == true)
			{
				ChangeNextStep(currentStep);
			}
		}
	}
	
	public void Activate()
	{
		if (currentStep == null && currentStepIndex == -1)
		{
			currentStepIndex = 0;
			
			currentStep = GetEventStep(currentStepIndex);
			if (currentStep != null)
				currentStep.OnActivate();
		}
		
		isActivate = true;
	}
	
	private EventStep GetEventStep(int index)
	{
		if (index < 0 || index >= eventSteps.Count)
			return null;
		
		return eventSteps[index];
	}
	
	public void ChangeNextStep(EventStep oldStep)
	{
		//if (oldStep != null)
		//	oldStep.OnCompleteStep();
		
		++currentStepIndex;
		if (currentStepIndex >= eventSteps.Count)
		{
			OnEndEvent();
		}
		else
		{
			currentStep = eventSteps[currentStepIndex];
			if (currentStep != null && currentStep.isAutoActivate == true)
				currentStep.OnActivate();
		}
	}
	
	public void OnEndEvent()
	{
		isComplete = true;
	}
}
