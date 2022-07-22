using UnityEngine;
using System.Collections;

public class Inven_Type
{
    public const int equip = 0;         // ����â
    public const int costume = 1;       // �콺����
    public const int costumeset = 2;      
    public const int item = 3;
    public const int material = 4;
}


public class Item_Type
{
    public const int material = 0;         // ����â
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
    GambleRate = 4,				// �ϰŷ� S��� Ȯ���ι�.
    StaminaRate = 5,			// ���׹̳� �ൿ�� �ݰ� �̺�Ʈ.
    RandomBox1 = 6,				// �����ڽ�1
    RandomBox2 = 7,				// �����ڽ�2 
    RandomBox3 = 8,				// �����ڽ�3
	
    SpecialItem = 9, 			// Ư����ǰ.
    StarterPack = 10,			// ��Ÿ����.
    kakaoLunching = 11	        // īī�� ��Ī �̺�Ʈ.
}
