//
// Copyright (c) 2011 Scott Clayton
//
// This file is part of the C# to PHP Encryption Library.
//   
// The C# to PHP Encryption Library is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//   
// The C# to PHP Encryption Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with the C# to PHP Encryption Library.  If not, see <http://www.gnu.org/licenses/>.
//
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;




namespace Secure
{

	public class Cryptography : MonoBehaviour
	{
	    public string address;
	    public bool connected;
		public bool enable = false;
	    
	    private RSAtoPHPCryptography rsa;
	    private AEStoPHPCryptography aes;
		public KeepSession keepSession;
		public NetMessageDispatcher messageDispatcher;	
		
	    /// <summary>
	    /// Gets the location of the PHP script that we are communicating with.
	    /// </summary>
	    public string PHPScriptLocation
	    {
	        get { return address; }
	    }
	
	    /// <summary>
	    /// Gets whether a secure connection has been established.
	    /// </summary>
	    public bool SecureConnectionEstablished
	    {
	        get { return connected; }
	    }
	
	    /// <summary>
	    /// Gets whether it is OK to send a message right now. A connection must be established, and there must be no other pending transmissions.
	    /// </summary>
	    public bool OKToSendMessage
	    {
	        get { return connected; }
	    }
	
	    /// <summary>
	    /// Create a secure connection with a PHP script.
	    /// </summary>
	    public Cryptography()
	    {
	        connected = false;
	
	        rsa = new RSAtoPHPCryptography();
	        aes = new AEStoPHPCryptography();
			//keepSession = new KeepSession();
	    }
		
		void Awake()
		{
			keepSession = gameObject.AddComponent<KeepSession>();
		}
		
		public void PreLogin()
		{
			StartCoroutine(SendGetKey());	
		}
	
	    /// <summary>
	    /// Set the location of the PHP script to use in this secure connection.
	    /// </summary>
	    /// <param name="phpScriptLocation">The URL of the php script to contact.</param>
	    public void SetRemotePhpScriptLocation(string phpScriptLocation)
	    {
			if (phpScriptLocation.Length > 0)
			{
				address = phpScriptLocation + "PreLogin.php";				
			}
			
	        connected = false;
	    }

		public void OnEstablishSecureConnection(string RSAKey)
		{
			enable = true;
			Logger.DebugLog("OnEstablishSecureConnection");
			
	        // Get the RSA public key that we will use			
	        rsa.LoadCertificateFromString(RSAKey);
	
	        // Generate the AES keys, encrypt them with the RSA public key, then send them to the PHP script.
	        aes.GenerateRandomKeys();
			
	        string rsakey = Utility.ToUrlSafeBase64(rsa.Encrypt(aes.EncryptionKey));
	        string rsaiv = Utility.ToUrlSafeBase64(rsa.Encrypt(aes.EncryptionIV));
			
			Dictionary<string, string> data = new Dictionary<string, string>();
			
			data.Add ("MsgID", Convert.ToString((int)NetID.AESKey));
			data.Add("key", rsakey);
			data.Add("iv", rsaiv);
			
			//key = Utility.ToUrlSafeBase64(aes.EncryptionKey);
	        //iv = Utility.ToUrlSafeBase64(aes.EncryptionIV);
			
			StartCoroutine(SendAESKey(data));
		}
		
		void OnError(string err)
		{
			if (string.IsNullOrEmpty(err))
				return;
			
			Logger.DebugLog (err);
			
			PacketError msg = new PacketError();
			
			msg.ErrorCode = (int)NetErrorCode.NotConnected;
			
			string json = LitJson.JsonMapper.ToJson(msg);
						
			messageDispatcher.MessageProcess(json);
		}
		
		void OnMessage(string respone)
		{
	        // If the PHP script sends this message back, then the connection is now good.
			if (string.IsNullOrEmpty(respone))
				return;
			
			// PreLogin은 무조건 암호화.
			try 
			{
				respone = aes.Decrypt(respone);	
				messageDispatcher.MessageProcess(respone);
			}
			catch(Exception ex)
			{
				Logger.DebugLog(ex.Message);
			}	
			
		}
		
		public bool AesEncrypt(string data, out string outdata)
		{
			if (!enable)
			{
				outdata = data;
				return true;
			}
			

			try 
			{
				outdata =  aes.Encrypt(data);	
				return true;
			}
			catch(CryptographicException ex)
			{
				outdata= "";
				Logger.DebugLog(ex.Message);
				return false;
			}
		}
		
		public bool AesDecrypt(string data, out string outdata)
		{
			if (!enable)
			{
				outdata = data;
				return true;
			}

			outdata = "";
				
			if (string.IsNullOrEmpty(data))
				return false;
			
			try 
			{
				outdata = aes.Decrypt(data);	
				
				return true;
			}
			catch(CryptographicException ex)
			{
				string msg = string.Format("{0} data:{1}", ex.Message, data);
				Logger.DebugLog(msg);
				
				GameUI.Instance.CancelWait();
				
				return false;
			}
			
			return false;			
		}
	
		IEnumerator SendGetKey()
		{
			Logger.DebugLog("Send GetKey" + this.PHPScriptLocation);
			yield return new WaitForEndOfFrame();
				
			WWWForm form = new WWWForm();
			
			form.AddField ("MsgID", Convert.ToString((int)NetID.PublicKey));
			
			WWW request = new WWW(address, form);
			
			 yield return request;
			
			if (!string.IsNullOrEmpty(request.error))
			{
			    OnError(request.error);
			}
			else
			{
				OnEstablishSecureConnection(request.text);				
			}	
		}
		
			
		IEnumerator SendAESKey(Dictionary<string,string> data)
		{
			yield return new WaitForEndOfFrame();
				
			WWWForm form = new WWWForm();			
			
			foreach (KeyValuePair<string, string> pair in data)
			{
				form.AddField (pair.Key, pair.Value);
			}
			
			WWW request = new WWW(address, form);
			
			 yield return request;
			
			if (!string.IsNullOrEmpty(request.error))
			{
			    OnError(request.error);
			}
			else
			{
				keepSession.SetCookie(request.responseHeaders["SET-COOKIE"]);				
				
				string[] result = request.text.Split('|');
				
				OnMessage(result[0]);
			}	
		}
		
	}
}
