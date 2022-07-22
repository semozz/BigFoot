using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LabelAnimate : MonoBehaviour
{
    public float CharSpeed = 50.0f;
	public float StartPos = 200.0f;
	private float leftPos = -150.0f;
	
    public UILabel LabelNotice;
	private float widthToDistance = 3.75f;
    private float maxWidth;
	private float loopTime = 0.0f;
	private int noticeIndex = 0;

    public void Initial()
    {
		if (noticeIndex >= Game.Instance.noticeInTown.Count/2)
			noticeIndex %= Game.Instance.noticeInTown.Count/2;
		
		string timeString = Game.Instance.noticeInTown[noticeIndex*2+1];
		if (timeString != "")
		{
			if (DateTime.Parse(timeString) <= DateTime.Now)
			{
				Game.Instance.noticeInTown.RemoveRange(noticeIndex*2, 2);
				Initial();
				return;
			}
		}
		LabelNotice.text = Game.Instance.noticeInTown[noticeIndex*2];
		maxWidth = GetMaxWidthLabel(LabelNotice);
		loopTime = (maxWidth/widthToDistance+StartPos-leftPos)/CharSpeed;
		noticeIndex++;
    }

   	void Update()
    {
		if (Game.Instance.noticeInTown.Count > 0)
		{
			LabelNotice.transform.localPosition = new Vector3(StartPos - CharSpeed*((maxWidth/widthToDistance+StartPos-leftPos)/CharSpeed - loopTime), 0.0f, 0.0f);
			loopTime -= Time.deltaTime;
			if (loopTime <= 0.0f)
				Initial();
		}
    }

    float GetMaxWidthLabel(UILabel label)
    {
        return label.relativeSize.x * label.font.size;
    }
}
