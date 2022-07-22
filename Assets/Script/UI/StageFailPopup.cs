using UnityEngine;
using System.Collections;

public class StageFailPopup : MonoBehaviour {
	public delegate void eventButtonClicked();
	public event eventButtonClicked GambleButtonClicked;
	public event eventButtonClicked ShopButtonClicked;
	public event eventButtonClicked TownButtonClicked;
	
	public void GambleButtonClick()
	{
		if( GambleButtonClicked != null )
			GambleButtonClicked();
	}
	
	public void ShopButtonClick()
	{
		if( ShopButtonClicked != null )
			ShopButtonClicked();
	}
	
	public void TownButtonClick()
	{
		if( TownButtonClicked != null )
			TownButtonClicked();
	}
}
