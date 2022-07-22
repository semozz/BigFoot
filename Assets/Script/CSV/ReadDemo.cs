using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
public class ReadDemo : MonoBehaviour {
 
	public TextAsset csv; 
 
	public CSVDB db = null;
	
	void Start () {
		db = new CSVDB();
		db.ReadCSVFile(csv.text);
		
		//db.OutputInfo();
		
		//CSVReader.DebugOutputGrid( CSVReader.SplitCsvGrid(csv.text) ); 
	}
}