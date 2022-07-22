using UnityEngine;
using System.Collections;

public class Inven_Type
{
    public const int equip = 0;         // ÀåÂøÃ¢
    public const int costume = 1;       // Äì½ºÃõÅÇ
    public const int costumeset = 2;      
    public const int item = 3;
    public const int material = 4;
}


public class Item_Type
{
    public const int material = 0;         // ÀåÂøÃ¢
    public const int normal = 1;
    public const int costume = 2;
    public const int costumeset = 3;
    
}


public class BossDamage
{
    public string nick;
    public int amount;
}


public enum CMSEventType
{
    None = 0,
    Levelup = 1,
    Attandance = 2,
    SpecialMission = 3,
    GambleRate = 4,				// ¾Ï°Å·¡ Sµî±Þ È®·üµÎ¹è.
    StaminaRate = 5,			// ½ºÅ×¹Ì³Ê Çàµ¿·Â ¹Ý°ª ÀÌº¥Æ®.
    RandomBox1 = 6,				// ·£´ý¹Ú½º1
    RandomBox2 = 7,				// ·£´ý¹Ú½º2 
    RandomBox3 = 8,				// ·£´ý¹Ú½º3
	
    SpecialItem = 9, 			// Æ¯°¡»óÇ°.
    StarterPack = 10,			// ½ºÅ¸ÅÍÆÑ.
    kakaoLunching = 11	        // Ä«Ä«¿À ·±Äª ÀÌº¥Æ®.
}
