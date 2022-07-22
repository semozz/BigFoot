using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EtcTypeInfo
{
	public EtcDamageUI.eEtcDamge type;
	public GameObject obj;
	
	public float delayTime = 1.0f;
}

public class EtcDamageUI : BaseDamageUI {
	public enum eEtcDamge
	{
		Avoid,
		Block,
		Absorption,
	}
	
	public List<EtcTypeInfo> etcTypeList = new List<EtcTypeInfo>();
	
	public void SetDamageType(eEtcDamge type)
	{
		EtcTypeInfo info = GetEtcTypeInfo(type);
		if (info != null)
		{
			SetObjectActive(type);
			DestroyObject(this.gameObject, info.delayTime);
		}
		else
		{
			DestroyObject(this.gameObject, 0.1f);
		}
	}
	
	public EtcTypeInfo GetEtcTypeInfo(eEtcDamge type)
	{
		EtcTypeInfo info = null;
		
		foreach(EtcTypeInfo temp in etcTypeList)
		{
			if (temp != null && temp.type == type)
			{
				info = temp;
				break;
			}
		}
		
		return info;
	}
	
	public void SetObjectActive(eEtcDamge type)
	{
		foreach(EtcTypeInfo info in etcTypeList)
		{
			if (info == null)
				continue;
			
			if (info.obj != null)
				info.obj.SetActive(info.type == type);
		}
	}
}
