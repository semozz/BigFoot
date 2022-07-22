using UnityEngine;
using System.Collections;

public class StartPage : MonoBehaviour {
	//public string nextScene = "SelectCharacter";
	
	void Awake()
	{
		Game game = Game.Instance;
		TableManager tableMgr = TableManager.Instance;
		
		if (game != null && tableMgr != null)
		{
			
		}
	}
}
