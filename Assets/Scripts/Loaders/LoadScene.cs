using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public delegate void setterDelegate(string[,] data);
public delegate void setterDelegateD(string[,] data, bool[] sd);

//script in charge of all loading-related functionalities
public class LoadScene : MonoBehaviour {

    [Inject(InjectFrom.Anywhere)]
    public EnemyLoader enemyLoader;
    [Inject(InjectFrom.Anywhere)]
    public CharacterLoader characterLoader;
    [Inject(InjectFrom.Anywhere)]
    public ItemLoader itemLoader;


    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;
    [Inject(InjectFrom.Anywhere)]
    public TravelSceneLoader travelSceneLoader;

    //allLoadersToCheck will be added loader from the subloaders separately
    public List<Loader> allLoadersToCheck;

    private int scn_to_load;

	void Start () {
		scn_to_load = Global.Scene_To_Load;
	}
	
	void Update () {
	}

    public IEnumerator waitForLoadDone()
    {
        //wait until all loaders are ready for game
        bool allDone = false;
        while (!allDone) { 
            allDone = true;
            foreach (Loader l in allLoadersToCheck)
            {
                if (!l.isLoadDone()) { allDone = false; break; }
            }
            if (!allDone || allLoadersToCheck.Count == 0) yield return new WaitForSeconds(0.5f);
        }

        print("all loaders are loaded!");
    }

    public void loadScene()
    {
        StartCoroutine(load());
    }

	private IEnumerator load(){
		
		//play loading animation and other good stuff
		yield return new WaitForSeconds (3);
		StartCoroutine (Global.LoadAsyncScene (scn_to_load));
	}


    public static IEnumerator processCSV(bool[] loadDone, TextAsset csv, setterDelegate setter, bool matchLineWithExcel)
    {
        //print("parsing " + csv.name);
        string[,] data = CSVReader.SplitCsvGrid(csv.text, matchLineWithExcel); 

        setter(data);
        while (!(data.Length > 0))
        {
            yield return null;
        }

        loadDone[0] = true;
    }

    /*
     * overflow method for above; will wait till the entire setter function is done (checking for passed bool[])
     */
    public static IEnumerator processCSV(bool[] loadDone, TextAsset csv, setterDelegateD setter, bool[] setterDone, bool matchLineWithExcel)
    {
        //print("parsing " + csv.name);
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
