using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleFace : MonoBehaviour {
	public GameObject warrior = null;
	public GameObject assassin = null;
	public GameObject wizard = null;
	
	public GameDef.ePlayerClass actorClass = GameDef.ePlayerClass.CLASS_WARRIOR;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SetClass(GameDef.ePlayerClass type)
	{
		actorClass = type;
		
		List<GameObject> list = new List<GameObject>();
		list.Add(warrior);
		list.Add(assassin);
		list.Add(wizard);
		foreach(GameObject obj in list)
		{
			if (obj != null)
				obj.SetActive(false);
		}
		
		switch(type)
		{
		case GameDef.ePlayerClass.CLASS_WARRIOR:
			if (warrior != null)
				warrior.SetActive(true);
			break;
		case GameDef.ePlayerClass.CLASS_ASSASSIN:
			if (assassin != null)
				assassin.SetActive(true);
			break;
		case GameDef.ePlayerClass.CLASS_WIZARD:
			if (wizard != null)
				wizard.SetActive(true);
			break;
		}
	}
}
