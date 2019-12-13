using UnityEngine;
using System.Collections;

public delegate void setterDelegate(string[,] data);
public delegate void setterDelegateD(string[,] data, bool[] sd);

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


    public static IEnumerator processCSV(bool[] loadDone, TextAsset csv, setterDelegate setter, bool matchLineWithExcel)
    {
        string[,] data = CSVReader.SplitCsvGrid(csv.text, matchLineWithExcel);
        setter(data);
        while (!(data.Length > 0))
        {
            yield return null;
        }
        loadDone[0] = true;
        //levelScript.setLevelCsvData(data);
    }

    /*
     * overflow method for above; will wait till the entire setter function is done (checking for passed bool[])
     */
    public static IEnumerator processCSV(bool[] loadDone, TextAsset csv, setterDelegateD setter, bool[] setterDone, bool matchLineWithExcel)
    {
        string[,] data = CSVReader.SplitCsvGrid(csv.text, matchLineWithExcel);
        setter(data, setterDone);
        while (!(data.Length > 0 && setterDone[0]))
        {
            yield return null;
        }
        loadDone[0] = true;
        //levelScript.setLevelCsvData(data);
    }
}
