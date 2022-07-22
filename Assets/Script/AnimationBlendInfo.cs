using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BlendInfo
{
	public float fBlendTime = 0.0f;
}

[System.Serializable]
public class AnimationBlendInfo : ScriptableObject {
	public int AnimationClipCount = 0;
	public List<string> animationClipNames = new List<string>();
	public Dictionary<string, int> animationNameToIndexs = new Dictionary<string, int>();
	public Dictionary<int, string> animationIndexToNames = new Dictionary<int, string>();
	
	public List<BlendInfo> BlendInfos = new List<BlendInfo>();
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void InitData(AnimationClip[] animationClips)
	{
		if (animationClips != null && BlendInfos.Count == 0)
		{
			int clipIndex = 0;
			foreach (AnimationClip clip in animationClips)
			{
				animationClipNames.Add(clip.name);
				
				animationNameToIndexs.Add(clip.name, clipIndex);
				animationIndexToNames.Add(clipIndex, clip.name);
				
				++clipIndex;
			}
			
			AnimationClipCount = animationClipNames.Count;
			for (int nRowIndex = 0; nRowIndex < AnimationClipCount; ++nRowIndex)
			{
				for (int nColIndex = 0; nColIndex < AnimationClipCount; ++nColIndex)
				{
					BlendInfos.Add(new BlendInfo());
				}
			}
		}
	}
	
	public bool IsContainAnimationName(string animationName)
	{
		foreach(string name in animationClipNames)
		{
			if (name == animationName)
				return true;
		}
		
		return false;
	}
					
	
	public void UpdateBlendInfo(AnimationClip[] animationClips)
	{
		if (animationClips != null)
		{
			//현재 있는 애니메니션 갯수가 시작 index
			int clipIndex = animationClipNames.Count;
			int nAddCount = 0;
			
			foreach (AnimationClip clip in animationClips)
			{
				//기존 애니메이션 이름이 없는 녀석만 추가 한다.
				if (IsContainAnimationName(clip.name) == true)
					continue;
				
				animationClipNames.Add(clip.name);
				
				animationNameToIndexs.Add(clip.name, clipIndex + nAddCount);
				animationIndexToNames.Add(clipIndex + nAddCount, clip.name);
				
				++nAddCount;
			}
			
			//기존 Animation 갯수
			int oldAnimationCount = clipIndex;
			
			//새로운 애니메니션 갯수
			int newAnimationCount = AnimationClipCount = animationClipNames.Count;
			
			//오른쪽 Colum 추가
			// |01|02|03|  --> |01|02|03|04|05|
			// |04|05|06|  --> |06|07|08|09|10|
			// |07|08|09|  --> |11|12|13|14|15|
			int nAddColum = 0;
			for (int nRowIndex = 0; nRowIndex < oldAnimationCount; ++nRowIndex)
			{
				//Colum에 추가
				for (int nColIndex = 0; nColIndex < nAddCount; ++nColIndex)
				{
					int nStartIndex = (nRowIndex * oldAnimationCount) + oldAnimationCount + nAddColum;
					
					BlendInfos.Insert(nStartIndex, new BlendInfo());
					//Debug.Log("Add index : " + nStartIndex);
					
					++nAddColum;					
				}
			}
			
			//아랫쪽으로 Row 전체 추가
			// |16|17|18|19|20|
			// |21|22|23|24|25|
			for (int nNewRowIndex = oldAnimationCount; nNewRowIndex < newAnimationCount; ++nNewRowIndex)
			{
				for (int nNewColIndex = 0; nNewColIndex < newAnimationCount; ++nNewColIndex)
				{
					BlendInfos.Add(new BlendInfo());
					//Debug.Log("Add [Row( " + nNewRowIndex + "), Col(" + nNewColIndex + ")] ....");
				}
			}			
		}
	}
	
	public void rebuildKeys()
	{
		if (AnimationClipCount > 0 && animationClipNames.Count > 0 )
		{
			if (animationNameToIndexs.Count == 0 ||
			    animationIndexToNames.Count == 0)
			{
				animationNameToIndexs.Clear();
				animationIndexToNames.Clear();
				
				int clipIndex = 0;
				string keyName = "";
				foreach (string name in animationClipNames)
				{
					keyName = name;
					if (keyName == "KonckDown_Start")
						keyName = "KnockDown_Start";
					
					animationNameToIndexs.Add(keyName, clipIndex);
					animationIndexToNames.Add(clipIndex, keyName);
					
					++clipIndex;
				}				
			}
		}
	}
	
	public int GetAnimationClipCount()
	{
		return animationClipNames.Count;
	}
	
	public string GetAnimationName(int nIndex)
	{
		if (animationIndexToNames.Keys.Count == 0 ||
		    animationIndexToNames.ContainsKey(nIndex) == false)
			return "None";
		
		return animationIndexToNames[nIndex];
	}
	
	public int GetAnimationIndex(string name)
	{
		if (animationNameToIndexs.Keys.Count == 0 ||
		    animationNameToIndexs.ContainsKey(name) == false)
			return -1;
		
		return animationNameToIndexs[name];
	}
	
	public BlendInfo GetBlendInfo(int nRowIndex, int nColIndex)
	{
		int nListIndex = (nRowIndex * AnimationClipCount) + nColIndex;
		if (nListIndex < 0 ||
		    nListIndex >= BlendInfos.Count)
			return null;
		
		return BlendInfos[nListIndex];	
	}
}
