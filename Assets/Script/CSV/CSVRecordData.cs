using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; 

public class ValueData
{
	private string _data = "";
	
	public string Value
	{
		get { return _data; }
		set { _data = value; }
	}
	
	public int ToInt()
	{
		int _Value = -1;
		
		int.TryParse(_data, out _Value);
		
		return _Value;
	}
	
	public long ToLong()
	{
		long _Value = -1;
		
		long.TryParse(_data, out _Value);
		
		return _Value;
	}
	
	public float ToFloat()
	{
		float _Value = 0.0f;
		
		float.TryParse(_data, out _Value);
		
		return _Value;
	}
	
	public string ToText()
	{
		return _data;
	}
}

public class CSVRecordData
{
	private Dictionary<string, ValueData> recordData = new Dictionary<string, ValueData>();
	
	public ValueData GetValue(string fieldName)
	{
		ValueData _data = null;
		
		if (recordData.ContainsKey(fieldName) == true)
			_data = recordData[fieldName];
		
		return _data;
	}
	
	public void SetValue(string fieldName, string _value)
	{
		if (recordData.ContainsKey(fieldName) == true)
			recordData.Remove(fieldName);
		
		ValueData newValue = new ValueData();
		newValue.Value = _value;
		
		recordData.Add(fieldName, newValue);
	}
	
	public void OutputInfo(string key)
	{
		string msg = key + "\t |";
		foreach(var data in recordData)
		{
			msg += data.Value.ToText() + "\t |";
		}
		
		Debug.Log(msg);
	}
}

public class CSVDB
{
	//private string keyFieldName = "";
	public Dictionary<string, CSVRecordData> data = new Dictionary<string, CSVRecordData>();
	
	private string[] fieldNameList = null;
	
	public void SetField(string[] fieldList)
	{
		if (fieldList == null)
			return;
		
		fieldNameList = new string[fieldList.Length];
		for (int index = 0; index < fieldList.Length; ++index)
		{
			fieldNameList[index] = fieldList[index];
		}
	}
	
	public string GetFieldName(int index)
	{
		string fieldName = "";
		if (index < 0 || index >= fieldNameList.Length)
			return fieldName;
		
		fieldName = fieldNameList[index];
		return fieldName;
	}
	
	public ValueData GetData(string key, string fieldName)
	{
		ValueData _value = null;
		
		CSVRecordData record = null;
		if (data.ContainsKey(key) == true)
			record = data[key];
		
		if (record != null)
			_value = record.GetValue(fieldName);
		
		return _value;
	}
	
	public void AddData(string keyField, string fieldName, string valueData)
	{
		CSVRecordData record = null;
		if (data.ContainsKey(keyField) == false)
		{
			record = new CSVRecordData();
			data.Add(keyField, record);
		}
		else
			record = data[keyField];
		
		if (record != null)
			record.SetValue(fieldName, valueData);
	}
	
	public void ReadCSVFile(string csvText)
	{
		string tempText = csvText.Replace("\r", "");
		
		string[] lines = tempText.Split("\n"[0]); 
 
		// finds the max width of row
		int width = 0; 
		for (int i = 0; i < lines.Length; i++)
		{
			string[] row = SplitCsvLine( lines[i] ); 
			width = Mathf.Max(width, row.Length);
			
			if (i == 0)
				SetField(row);
		}
 
		string fieldName = "";
		string valueData = "";
		string keyField = "";
		
		for (int y = 1; y < lines.Length; y++)
		{
			string[] row = SplitCsvLine( lines[y] );
			
			keyField = "";
			fieldName = "";
			valueData = "";
			
			for (int x = 0; x < row.Length; x++) 
			{
				if (x == 0)
				{
					keyField = row[x];
					continue;
				}
				
				valueData = row[x];
				valueData = valueData.Replace("\"\"", "\"");
 				fieldName = GetFieldName(x);
				
				AddData(keyField, fieldName, valueData);
			}
		}
	}
	
	// splits a CSV row 
	public string[] SplitCsvLine(string line)
	{
		return (from System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(line,
		@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)", 
		System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
		select m.Groups[1].Value).ToArray();
	}
	
	public void OutputInfo()
	{
		string msg = "";
		foreach(string field in fieldNameList)
		{
			msg += field + "\t |";
		}
		Debug.Log(msg);
		
		foreach(var record in this.data)
		{
			record.Value.OutputInfo(record.Key);
		}
	}
}

