using UnityEngine;
using System.Collections;

public class ScrollPanel : MonoBehaviour 
{
	public enum eStageType
	{
		FarStage,
		MiddleStage,
	}
	public eStageType stageType = eStageType.FarStage;
	
	public StageManager stageManager = null;
    public ScrollCamera scrollCamera = null;

    private float mScreenWidth = 0.0f;
    private float mScreenHeight = 0.0f;
	
	private float stageWidth = 0.0f;
	private float scrollWidth = 0.0f;
    
	private float stageHeight = 0.0f;
    private float scrollHeight = 0.0f;
	
	[HideInInspector]
	public float scrollVWeight = 1.0f;
	
	private float ScreenHeightRate = 1.0f;
	void Start () 
	{
		if (stageManager)
		{
			float screenHeight = Screen.height;// * scaleheight;
			float screenWidth = Screen.width;// * scaleheight;
			
			Camera cam = null;
			if (scrollCamera != null && scrollCamera.gameCamera != null)
				cam = scrollCamera.gameCamera.camera;
			
			float camWidth = screenWidth;
			float camHeight = screenHeight;
			if (cam != null)
			{
				Rect rect = cam.rect;
				camWidth = rect.width * screenWidth;
	        	camHeight = rect.height * screenHeight;
			}
			
			ScreenHeightRate = stageManager.ScreenHeight / camHeight;
			
            mScreenWidth = camWidth;
            mScreenHeight = camHeight;
			
			if (stageType == eStageType.FarStage)
			{
				stageWidth = stageManager.FarStageWidth;
				stageHeight = stageManager.FarStageHeight;
			}
			else
			{
				stageWidth = stageManager.MiddleStageWidth;
				stageHeight = stageManager.MiddleStageHeight;
			}
			
			scrollWidth = stageWidth - (mScreenWidth * ScreenHeightRate);//stageManager.ScreenWidth;
            scrollHeight = stageHeight - (mScreenHeight * ScreenHeightRate);//stageManager.ScreenHeight;
			
			
		}
	}
	
	void LateUpdate ()
	{
		if (scrollCamera == null) return;

        Vector3 vTargetPos = scrollCamera.GetTargetPos();
		
		/*
		float fX = vTargetPos.x - (mScreenWidth * 0.5f * ScreenHeightRate);
		if (fX < 0) fX = 0.0f;
		if (fX > (mNearStageWidth - mScreenWidth))
			fX = (mNearStageWidth - mScreenWidth);
		
		float fWScrollRate = fX / mNearWidth;
        float fFarX = (fX - (mFarWidth * fWScrollRate));// * ScreenHeightRate;
		
		float fY = vTargetPos.y - (mScreenHeight * 0.5f);// * ScreenHeightRate);
        if (fY < 0) fY = 0.0f;
        if (fY > mNearStageScrollHeight - mScreenHeight) fY = mNearStageScrollHeight - mScreenHeight;

        float fHScrollRate = fY / mNearHeight;
        float fFarY = fY - (mFarHeight * fHScrollRate);
		*/
		
		Vector3 moveRate = scrollCamera.GetMoveRate();
		moveRate.y = scrollVWeight;
		
		float fX = vTargetPos.x - (mScreenWidth * 0.5f * ScreenHeightRate);
		float fFarX = fX - (scrollWidth * moveRate.x);
		
		float fY = vTargetPos.y - (mScreenHeight * 0.5f * ScreenHeightRate);
		//float fFarY = fY - (scrollHeight * moveRate.y);
        float fFarY = fY * scrollVWeight;
		
        Vector3 vNewPos = transform.position;
        vNewPos.x = fFarX;
        vNewPos.y = fFarY;

        transform.position = vNewPos;
		//vNewPos = Vector3.Lerp(transform.position, vNewPos, 0.3f);
		//transform.position = vNewPos;
	}
}
