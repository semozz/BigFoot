using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager
{
	private static WeaponManager mInstance = null;
	public static WeaponManager Instance
	{
		get
		{
			if (mInstance == null)
				mInstance = new WeaponManager();
			
			return mInstance;
		}	
	}
	
	public List<BaseWeapon> projectiles = new List<BaseWeapon>();
	
	public void AddWeapon(BaseWeapon weapon)
	{
		if (weapon == null)
			return;
		
		projectiles.Add(weapon);
	}
	
	public void RemoveWeapon(BaseWeapon weapon)
	{
		if (weapon == null)
			return;
		
		projectiles.Remove(weapon);
	}
	
}
