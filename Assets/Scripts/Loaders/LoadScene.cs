using UnityEngine;
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
    public TravelSceneManager travelSceneManager;

    private int scn_to_load;

	void Start () {
		scn_to_load = Global.Scene_To_Load;
		//loadScene();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public bool checkLoadDone(GameControl.Mode sceneMode) //TODO messy conditioning
    { //check if all loaders are ready for game
        if (sceneMode == GameControl.Mode.GAME)
        {
            return (gameFlow.checkGameFlowLoadDone() && enemyLoader.enemyLoaderDone
                && characterLoader.characterLoaderDone && itemLoader.itemLoaderDone);
        }
        else if (sceneMode == GameControl.Mode.TRAVEL)
        {
            return (travelSceneManager.travelLoadDone() && enemyLoader.enemyLoaderDone
                && characterLoader.characterLoaderDone && itemLoader.itemLoaderDone);
        }
        else if (sceneMode == GameControl.Mode.QUEST)
        {
            return itemLoader.itemLoaderDone;
        }
        else
        {
            return true;
        }
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
