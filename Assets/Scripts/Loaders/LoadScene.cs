using UnityEngine;
using System.Collections;

public delegate void setterDelegate(string[,] data);

public class LoadScene : MonoBehaviour {
	//script specifically for loading screen
	private int scn_to_load;

	void Start () {
		scn_to_load = Global.Scene_To_Load;
		StartCoroutine (load ());
	}
	
	// Update is called once per frame
	void Update () {
	}

	IEnumerator load(){
		
		//play loading animation and other good stuff
		yield return new WaitForSeconds (3);
		StartCoroutine (Global.LoadAsyncScene (scn_to_load));
	}


    public static IEnumerator processCSV(bool[] loadDone, TextAsset csv, setterDelegate setter)
    {
        string[,] data = CSVReader.SplitCsvGrid(csv.text);
        setter(data);
        while (!(data.Length > 0))
        {
            yield return null;
        }
        loadDone[0] = true;
        //levelScript.setLevelCsvData(data);
    }
}
