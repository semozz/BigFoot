using UnityEngine;
using System.Collections;

public class RescureCounter: EventConditionTrigger {
	
	public override void OnActivate()
	{
		if (this.isActivate == true)
			return;
		
		base.OnActivate();
		
		EventConditionChecker checker = EventConditionChecker.Instance;
		if (checker != null)
			checker.AddRescureCounter(this);
	}
	
	public override void OnComplete()
	{
		this.isActivate = false;
		
		EventConditionChecker checker = EventConditionChecker.Instance;
		if (checker != null)
			checker.RemoveRescureCounter(this);
	}
}
