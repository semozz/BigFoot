using UnityEngine;
using System.Collections.Generic;


public enum LogLevel {Debug, Warning, Error, Fatal };

public class Logger : MonoBehaviour
{
	
	static List<string> mLines = new List<string>();
	static NGUIDebug mInstance = null;
	static LogLevel logLevel; 
	
	static public void SetLevel(LogLevel level)
	{
		logLevel = level;
	}
	
	static public void DebugLog (string text)
	{
		if (logLevel <= LogLevel.Debug)
			Log (text);
	}
	
	static public void ErrorLog (string text)
	{
		if (logLevel <= LogLevel.Error)
			Log (text);
	}
	
	static public void FatalLog (string text)
	{
		if (logLevel <= LogLevel.Fatal)
			Log (text);
	}
	
	static public void Log (string text)
	{
		Debug.Log(text);
		/*
		if (Application.isPlaying && logLevel ==  LogLevel.Debug)
		{
			//Debug.Log(text);

			if (mLines.Count > 20) 
				mLines.RemoveAt(0);
			
			mLines.Add(text);

			if (mInstance == null)
			{
				GameObject go = new GameObject("Bigfoot_Debug");
				mInstance = go.AddComponent<Logger>();
				DontDestroyOnLoad(go);
			}
		}
		*/
		
	}

	static public void DrawBounds (Bounds b)
	{
		Vector3 c = b.center;
		Vector3 v0 = b.center - b.extents;
		Vector3 v1 = b.center + b.extents;
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v1.x, v0.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v0.y, c.z), new Vector3(v0.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v1.x, v0.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
		Debug.DrawLine(new Vector3(v0.x, v1.y, c.z), new Vector3(v1.x, v1.y, c.z), Color.red);
	}
	
	void OnGUI()
	{
		for (int i = 0, imax = mLines.Count; i < imax; ++i)
		{
			GUILayout.Label(mLines[i]);
		}
	}
	
	void Update()
	{
		// 일정시간이 흐르면.
		// mLines.Clear();
	}
}