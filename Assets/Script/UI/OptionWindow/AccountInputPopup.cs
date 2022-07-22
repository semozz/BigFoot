using UnityEngine;
using System.Collections;

public class AccountInputPopup : BaseConfirmPopup {

	public UIInput accountInput = null;
	public UIInput passwordInput = null;
	public AccountType accountType = AccountType.MonsterSide;
	
	public void SetAccountInfo(string account, string pass, AccountType type)
	{
		if (accountInput != null)
			accountInput.text = account;
		
		if (passwordInput != null)
			passwordInput.text = "";
		
		this.accountType = type;
	}
	
	
	public void GetAccountInfo(out string accountStr, out string passwordStr, out AccountType type)
	{
		accountStr = "";
		passwordStr = "";
		type = AccountType.MonsterSide;
		
		if (accountInput != null)
			accountStr = accountInput.text;
		
		if (passwordInput != null)
			passwordStr = passwordInput.text;
		
		type = this.accountType;
	}
	
	public void OnPassError()
	{
		if (this.messageLabel != null)
			this.messageLabel.text = "Input Password...";
		
		Invoke("ClearMessage", 2.0f);
	}
	
	public void ClearMessage()
	{
		if (this.messageLabel != null)
			this.messageLabel.text = "";
	}
}
