using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CouponWindow : PopupBaseWindow {

	public List<UIInput> couponNumbers = new List<UIInput>();
	
	public UIButton couponOK = null;
	
	public UILabel errorMessageLabel = null;
	
	public UILabel normalMessageLabel = null;
	
	
	public int requestCount = 0;
	
	private StringTable stringTable = null;
	void Start()
	{
		TableManager tableManager = TableManager.Instance;
		if (tableManager != null)
			stringTable = tableManager.stringTable;
		
		GameUI.Instance.couponWindow = this;
	}
	
	void OnDestroy()
	{
		GameUI.Instance.couponWindow = null;
	}
	
	public void InitWindow()
	{
		
	}
	
	public void OnInputCoupon()
	{
		if (requestCount > 0)
			return;
		
		List<string> inputNumbers = new List<string>();
		int maxCount = couponNumbers.Count;
		
		if (CheckInputCouponNumber(inputNumbers) == true)
		{
			OnErrorMessage(NetErrorCode.CouponInvalid, null);
			return;
		}
		else
		{
			IPacketSender sender = Game.Instance.PacketSender;
			if (sender != null)
			{
				sender.SendRequestCoupon(MakeCouponNumber(inputNumbers));
			
				requestCount++;
			}			
		}
	}
	
	private string MakeCouponNumber(List<string> inputNumbers)
	{
		System.Text.StringBuilder builder = new System.Text.StringBuilder();
		foreach(string number in inputNumbers)
			builder.Append(number);
		
		return builder.ToString();
	}
	
	private bool CheckInputCouponNumber(List<string> inputNumbers)
	{
		bool isError = false;
		foreach(UIInput input in couponNumbers)
		{
			string couponNum = input.text;
			if (couponNum.Length != 4)
			{
				isError = true;
				break;
			}
			else
			{
				inputNumbers.Add(couponNum);
			}
		}
		
		return isError;
	}
	
	public void OnResult(NetErrorCode errorCode)
	{
		if (errorCode == NetErrorCode.OK)
		{
			OnInputCouponOK();
		}
		else
		{
			OnErrorMessage(errorCode, null);
		}
	}
	
	public void OnResult(NetErrorCode errorCode, string error_message)
	{
		if (errorCode == NetErrorCode.OK)
		{
			OnInputCouponOK();
		}
		else
		{
			OnErrorMessage(errorCode, error_message, null);
		}
	}
	
	public string couponCompletePrefab = "UI/Option/Coupon_Recieve_popup";
	public void OnInputCouponOK()
	{
		BaseConfirmPopup popup = ResourceManager.CreatePrefab<BaseConfirmPopup>(couponCompletePrefab, this.popupNode, Vector3.zero);
		if (popup != null)
		{
			this.popupList.Add(popup);
			
			popup.okButtonMessage.target = this.gameObject;
			popup.okButtonMessage.functionName = "OnBack";
		}
	}
	
	public void OnErrorMessage(NetErrorCode errorCode, string error_message, PopupBaseWindow popupBase)
	{
		if (string.IsNullOrEmpty(error_message) == false)
		{
			if (normalMessageLabel != null)
				normalMessageLabel.gameObject.SetActive(false);
			
			if (errorMessageLabel != null)
			{
				errorMessageLabel.gameObject.SetActive(true);
				errorMessageLabel.text = stringTable.GetData((int)errorCode);
			}
			
			ResetInputCouponNumbers();
		}
		else
		{
			base.OnErrorMessage (errorCode, popupBase);
		}
		
		requestCount = 0;
	}
	
	public override void OnErrorMessage (NetErrorCode errorCode, PopupBaseWindow popupBase)
	{
		switch(errorCode)
		{
		case NetErrorCode.CouponInvalidDate:
		case NetErrorCode.UsedCoupon:
		case NetErrorCode.UseCountOverCoupon:
		case NetErrorCode.CountOverCoupon:
		case NetErrorCode.CouponInvalid:
			if (normalMessageLabel != null)
				normalMessageLabel.gameObject.SetActive(false);
			
			if (errorMessageLabel != null)
			{
				errorMessageLabel.gameObject.SetActive(true);
				errorMessageLabel.text = stringTable.GetData((int)errorCode);
			}
			
			ResetInputCouponNumbers();
			break;
		default:
			base.OnErrorMessage (errorCode, popupBase);
			break;
		}
		
		requestCount = 0;
	}
	
	public void ResetInputCouponNumbers()
	{
		foreach(UIInput input in couponNumbers)
			input.text = "";
		
		Invoke("ResetMessage", 2.0f);
	}
	
	public void ResetMessage()
	{
		if (normalMessageLabel != null)
			normalMessageLabel.gameObject.SetActive(true);
		
		if (errorMessageLabel != null)
			errorMessageLabel.gameObject.SetActive(false);
	}
	
	public override void OnBack ()
	{
		base.OnBack ();
		
		DestroyObject(this.gameObject, 0.2f);
	}
}
