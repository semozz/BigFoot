using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KeepSession : MonoBehaviour 
{

	// will be set to some session id after login
	private Hashtable session_ident = new Hashtable();
	
	public string errmsg; 
	// and some helper functions and properties
	
	public void ClearSessionCookie()
	{
	    session_ident["Cookie"] = null;
	}
	  
	void SetSessionCookie(string s)
	{
	    session_ident["Cookie"] = s;
	}
	
	public void SetCookie(string cookie)
	{
		ClearSessionCookie();
		
		char[] splitter = {';'};
		
		string[] v = cookie.Split(splitter);

		foreach (string s in v)
        {
            if (string.IsNullOrEmpty(s)) continue;
			
			if (s.Contains("PHPSESSID"))
			{
                SetSessionCookie(s);
				break;
            }
        }
	}
	  
	public Hashtable SessionCookie
	{
	    get { return session_ident; }
	}
	  
	public string GetSessionCookie()
	{
	    return session_ident["Cookie"] as string;
	}
	  
	public bool SessionCookieIsSet
	{
	    get { return session_ident["Cookie"] != null; }
	}
}
