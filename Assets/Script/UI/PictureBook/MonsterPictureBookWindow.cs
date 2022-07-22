using UnityEngine;
using System.Collections;

public class MonsterPictureBookWindow : BasePictureBookWindow {
	
	
	public void SetPictureBookInfo(MonsterPictureBook monsterPictureBook)
	{
		BasePictureBookListWindow listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.NormalMonster);
		if (listWindow != null)
			listWindow.SetInfos(monsterPictureBook.normalMonsterPictureBookList);
		
		listWindow = GetWindow(BasePictureBookListWindow.ePictureBookListType.HardMonster);
		if (listWindow != null)
			listWindow.SetInfos(monsterPictureBook.hardMonsterPictureBookList);
	}
}
