using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
 
public class EncryptedPlayerPrefs  {
 
    // Encrypted PlayerPrefs
    // Written by Sven Magnus
    // MD5 code by Matthew Wegner (from [url]http://www.unifycommunity.com/wiki/index.php?title=MD5[/url])
    
    
    // Modify this key in this file :
    private static string privateKey="9ETrEsWaFRach3gexaDrqwdS910";
    
    // Add some values to this array before using EncryptedPlayerPrefs
    public static string[] keys;
    
    
    public static string Md5(string strToEncrypt) {
        UTF8Encoding ue = new UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);
 
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);
 
        string hashString = "";
 
        for (int i = 0; i < hashBytes.Length; i++) {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
 
        return hashString.PadLeft(32, '0');
    }
    
    public static void SaveEncryption(string key, string type, string value) {
        int keyIndex = (int)Mathf.Floor(Random.value * keys.Length);
        string secretKey = keys[keyIndex];
        string check = Md5(type + "_" + privateKey + "_" + secretKey + "_" + value);
        PlayerPrefs.SetString(key + "_encryption_check", check);
        PlayerPrefs.SetInt(key + "_used_key", keyIndex);
    }
    
    public static bool CheckEncryption(string key, string type, string value) {
        int keyIndex = PlayerPrefs.GetInt(key + "_used_key");
        string secretKey = keys[keyIndex];
        string check = Md5(type + "_" + privateKey + "_" + secretKey + "_" + value);
        if(!PlayerPrefs.HasKey(key + "_encryption_check")) return false;
        string storedCheck = PlayerPrefs.GetString(key + "_encryption_check");
        return storedCheck == check;
    }
    
    public static void SetInt(string key, int value) {
        PlayerPrefs.SetInt(key, value);
        SaveEncryption(key, "int", value.ToString());
    }
    
    public static void SetFloat(string key, float value) {
        PlayerPrefs.SetFloat(key, value);
        SaveEncryption(key, "float", Mathf.Floor(value*1000).ToString());
    }
    
    public static void SetString(string key, string value) {
        PlayerPrefs.SetString(key, value);
        SaveEncryption(key, "string", value);
    }
    
    public static int GetInt(string key) {
        return GetInt(key, 0);
    }
    
    public static float GetFloat(string key) {
        return GetFloat(key, 0f);
    }
    
    public static string GetString(string key) {
        return GetString(key, "");
    }
    
    public static int GetInt(string key,int defaultValue) {
        int value = PlayerPrefs.GetInt(key);
        if(!CheckEncryption(key, "int", value.ToString())) return defaultValue;
        return value;
    }
    
    public static float GetFloat(string key, float defaultValue) {
        float value = PlayerPrefs.GetFloat(key);
        if(!CheckEncryption(key, "float", Mathf.Floor(value*1000).ToString())) return defaultValue;
        return value;
    }
    
    public static string GetString(string key, string defaultValue) {
        string value = PlayerPrefs.GetString(key);
        if(!CheckEncryption(key, "string", value)) return defaultValue;
        return value;
    }
    
    public static bool HasKey(string key) {
        return PlayerPrefs.HasKey(key);
    }
    
    public static void DeleteKey(string key) {
        PlayerPrefs.DeleteKey(key);
        PlayerPrefs.DeleteKey(key + "_encryption_check");
        PlayerPrefs.DeleteKey(key + "_used_key");
    }
    
	
	public static void SetStringByEncrypted(string _key, string _value)
	{
		MD5 md5Hash = new MD5CryptoServiceProvider();  
    	byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(privateKey));
		
		SetStringByEncrypted(_key, _value, secret);
	}
	
	public static void SetStringByEncrypted(string _key, string _value, byte[] _secret)  
	{  
	    // Hide '_key' string.  
	    MD5 md5Hash = MD5.Create();  
	    byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_key));  
	    string hashKey = System.Text.Encoding.UTF8.GetString(hashData);  
	
	    // Encrypt '_value' into a byte array  
	    byte[] bytes = System.Text.Encoding.UTF8.GetBytes(_value);  
	
	    // Eecrypt '_value' with 3DES.  
	    TripleDES des = new TripleDESCryptoServiceProvider();  
	    des.Key = _secret;  
	    des.Mode = CipherMode.ECB;  
	    ICryptoTransform xform = des.CreateEncryptor();  
	    byte[] encrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);  
	
	    // Convert encrypted array into a readable string.  
	    string encryptedString = System.Convert.ToBase64String(encrypted);  
	
	    // Set the ( key, encrypted value ) pair in regular PlayerPrefs.  
	    PlayerPrefs.SetString(hashKey, encryptedString);  
	          
	    //Debug.Log("SetString hashKey: " + hashKey + " Encrypted Data: " + encryptedString);  
	}
	
	public static string GetStringByEncrypted(string _key)
	{
		MD5 md5Hash = new MD5CryptoServiceProvider();  
    	byte[] secret = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(privateKey));
		
		return GetStringByEncrypted(_key, secret);
	}
	
	public static string GetStringByEncrypted(string _key, byte[] _secret)  
	{  
	    // Hide '_key' string.  
	    MD5 md5Hash = MD5.Create();  
	    byte[] hashData = md5Hash.ComputeHash(System.Text.Encoding.UTF8.GetBytes(_key));  
	    string hashKey = System.Text.Encoding.UTF8.GetString(hashData);  
	
	    // Retrieve encrypted '_value' and Base64 decode it.  
	    string _value = PlayerPrefs.GetString(hashKey);
		if (_value == "")
			return _value;
		
	    byte[] bytes = System.Convert.FromBase64String(_value);  
	
	    // Decrypt '_value' with 3DES.  
	    TripleDES des = new TripleDESCryptoServiceProvider();  
	    des.Key = _secret;  
	    des.Mode = CipherMode.ECB;  
	    ICryptoTransform xform = des.CreateDecryptor();  
	    byte[] decrypted = xform.TransformFinalBlock(bytes, 0, bytes.Length);  
	          
	    // decrypte_value as a proper string.  
	    string decryptedString = System.Text.Encoding.UTF8.GetString(decrypted);  
	          
	    //Debug.Log("GetString hashKey: " + hashKey + " GetData: " + _value + " Decrypted Data: " + decryptedString); 
	    return decryptedString;  
	}

}