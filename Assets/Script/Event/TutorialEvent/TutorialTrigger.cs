using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialTrigger : EventConditionTrigger {

	public List<TutorialTask> tutorialTasks = new List<TutorialTask>();
	private TutorialTask currentTask = null;
	
	public void Update()
	{
		if (isActivate == false)
			return;
		
		bool isCompleteTask = false;
		
		if (currentTask == null || currentTask.IsComplete() == true)
			isCompleteTask = true;
		
		if (isCompleteTask == false)
			return;
		
		int nCount = tutorialTasks.Count;
		if (currentTask != null)
		{
			currentTask.DoEnd();
			
			nCount = tutorialTasks.Count;
			if (nCount > 0)
				tutorialTasks.RemoveAt(0);
		}
		
		TutorialTask nextTask = null;
		nCount = tutorialTasks.Count;
		if (nCount > 0)
			nextTask = tutorialTasks[0];
		
		if (nextTask != null)
		{
			nextTask.DoStart();
			
			currentTask = nextTask;
		}
		else
			this._EventCondtion.AddCondtionValue(1);
	}
}
