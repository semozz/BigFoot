using UnityEngine;
using System.Collections;

public class BIController : MonoBehaviour {

	public string nextSceneName = "EmptyStart";
	public string introMoviefile = "Intro01.mp4";
	
	public void PlayIntroMovie()
	{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		Handheld.PlayFullScreenMovie(introMoviefile, Color.black, FullScreenMovieControlMode.CancelOnInput, FullScreenMovieScalingMode.AspectFit);
#endif
		LoadNextScene();
	}
	
	public void LoadNextScene()
	{
		Application.LoadLevel(nextSceneName);
	}
}
