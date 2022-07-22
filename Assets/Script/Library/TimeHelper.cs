using UnityEngine;
using System;
using System.Collections;

static public class TimeHelper 
{
    static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 9, 0, 0, 0, DateTimeKind.Utc);

	static public System.DateTime GetTimeAfterSec(int Sec)
	{
		System.DateTime result = System.DateTime.Now;
		
		int day = Sec / 86400;
		int hour = Sec % 86400 / 360;
		int minite = Sec % 86400 % 360 / 60;
		int second = Sec % 86400 % 360 % 60;
		
		System.TimeSpan duration = new System.TimeSpan(day, hour, minite, second);
		
		return result.Add(duration);		
	}

    public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
    {
        return UnixEpoch.AddSeconds(unixTimeStamp);
    }

    public static int DateTimeToUnixTimeStamp(DateTime datetime)
    {
        TimeSpan ts = datetime - UnixEpoch;
        return (int)ts.TotalSeconds;
    }

    public static int GetTimeDiffSec(DateTime bigger, DateTime smaller)
    {
        TimeSpan ts = bigger - smaller;
        
        // return Math.Abs(ts.Seconds);
        return (int)ts.TotalSeconds;
    }

    public static int GetTimeDiffSec(string bigger, DateTime smaller)
    { 
        DateTime biggerDatetime = Convert.ToDateTime(bigger);
        return GetTimeDiffSec(biggerDatetime, smaller);
    }

}