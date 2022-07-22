using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateList : MonoBehaviour {
	public Animation animObj = null;
	
	public List<CharStateInfo> stateList = new List<CharStateInfo>();
	
	public BaseState.eState defaultState = BaseState.eState.Stand;
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void AddNewState(CharStateInfo newState)
	{
		if (newState == null)
			return;
		
		stateList.Add(newState);
	}
	
	public void RemoveState(CharStateInfo state)
	{
		int index = 0;
		int nCount = stateList.Count;
		
		for (index = 0; index < nCount; ++index)
		{
			CharStateInfo temp = stateList[index];
			
			if (temp == state)
			{
				stateList.RemoveAt(index);
				break;
			}
		}
	}
	
	public CharStateInfo GetState(BaseState.eState state)
	{
		CharStateInfo stateInfo = null;
		
		foreach(CharStateInfo temp in stateList)
		{
			if (temp != null && temp.baseState.state == state)
			{
				stateInfo = temp;
				break;
			}
		}
		
		return stateInfo;
	}
	
	
}
