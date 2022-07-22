using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {
	public float origOrthSize = 3.2f;
	public float scaleFactor = 1.0f;
	
	private Camera camera = null;
	public float animOrthSizeValue = 1.0f;
	
    void Awake()
    {
        camera = GetComponent<Camera>();
		if (camera != null)
			origOrthSize = camera.orthographicSize;
		
		//float targetaspect = 16.0f / 9.0f;
	    //float windowaspect = (float)Screen.width / (float)Screen.height;
	    //scaleFactor = windowaspect / targetaspect;
		
		Debug.Log("Init camera size : " + origOrthSize * scaleFactor);
		
		InitCameraAspect();
    }
	
	public void Update()
	{
		float limitValue = origOrthSize * scaleFactor;
		
		float orthSize = limitValue * Mathf.Min(1.0f, animOrthSizeValue);
		
		if (camera != null)
			camera.orthographicSize = orthSize;
	}
	
	public void InitCameraSize()
	{
		Camera camera = GetComponent<Camera>();
		if (camera != null)
			camera.orthographicSize = origOrthSize * scaleFactor;
		
		Debug.Log("Init camera size : " + origOrthSize * scaleFactor);
	}
	
	void InitCameraAspect()
	{
		// set the desired aspect ratio (the values in this example are
	    // hard-coded for 16:9, but you could make them into public
	    // variables instead so you can set them at design time)
	    float targetaspect = 16.0f / 9.0f;
	
	    // determine the game window's current aspect ratio
	    float windowaspect = (float)Screen.width / (float)Screen.height;
	
	    // current viewport height should be scaled by this amount
	    float scaleheight = windowaspect / targetaspect;
	
	    // obtain camera component so we can modify its viewport
	    Camera camera = GetComponent<Camera>();
	
	    // if scaled height is less than current height, add letterbox
	    if (scaleheight < 1.0f)
	    {  
	        Rect rect = camera.rect;
	
	        rect.width = 1.0f;
	        rect.height = scaleheight;
	        rect.x = 0;
	        rect.y = (1.0f - scaleheight) / 2.0f;
	
	        camera.rect = rect;
	    }
	    else // add pillarbox
	    {
	        float scalewidth = 1.0f / scaleheight;
	
	        Rect rect = camera.rect;
	
	        rect.width = scalewidth;
	        rect.height = 1.0f;
	        rect.x = (1.0f - scalewidth) / 2.0f;
	        rect.y = 0;
	
	        camera.rect = rect;
	    }
	}
}
