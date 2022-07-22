using UnityEngine;
using System.Collections;

public class TutorialTask : MonoBehaviour
{
	public float lifeTime = 1.0f;
	public float delayTime = 0.0f;
	
	public bool isPuase = false;
	public bool isInputPause = false;
	
	public bool isCommon = false;
	
	public virtual void DoStart()
	{
		delayTime = lifeTime;
		
		if (isPuase == true)
			Game.Instance.Pause = true;
		if (isInputPause == true)
			Game.Instance.InputPause = true;
	}
	
	public virtual void DoEnd()
	{
		if (isPuase == true)
			Game.Instance.Pause = false;
		if (isInputPause == true)
			Game.Instance.InputPause = false;
	}
	
	public void Update()
	{
		if (lifeTime > 0.0f)
			delayTime -= Time.deltaTime;
	}
	
	public virtual bool IsComplete()
	{
		if (lifeTime > 0.0f)
		{
			if (delayTime <= 0.0f)
				return true;
		}
		
		return false;
	}
	
	public virtual void OnSkip()
	{
		
	}
}
