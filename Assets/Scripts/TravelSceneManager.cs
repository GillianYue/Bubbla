using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelSceneManager : MonoBehaviour
{
    private bool[] loadDone;  //this bool is only for the level progress file, not everything

    public TextAsset sceneCsv; //dialogue file for a specific level
    private string[,] data; //double array that stores all info of this level
    public GameObject BG; //parent of background prefabs

    // Start is called before the first frame update
    void Start()
    {
        loadDone = new bool[2];
        bool[] parseDone = new bool[1];
        StartCoroutine(LoadScene.processCSV(loadDone, sceneCsv, setData, parseDone, true)); //if true, will match excel line num

        StartCoroutine(loadTravelScene());
    }

    public void setData(string[,] d, bool[] parseDone)
    {
        data = d;
        parseDone[0] = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool travelLoadDone() //data is ready
    {
        return loadDone[1]; //not 0, because loadDone[0] is for data ready, not for load in 
    }

    IEnumerator loadTravelScene()
    {
        yield return new WaitUntil(() => (data != null));
        loadBackgroundPrefab();

        loadDone[1] = true;
    }

    //will be indicated in the first line of the file
    void loadBackgroundPrefab()
    {
        if(BG.transform.childCount > 0)
        {
            Destroy(BG.transform.GetChild(0).gameObject); //so that only one background prefab is existent
        }

        if (data[0, 1].Equals("NAME")) //data[col, row]
        {
            GameObject bg = Resources.Load("TravelBG/Background_" + data[1, 1]) as GameObject;
            GameObject bgInstance = Instantiate(bg);

            if (bg == null)
            {
                Debug.LogError("Background prefab " + data[1, 1] + " NOT found");
            }
            else
            {
                bgInstance.transform.SetParent(BG.transform);
            }

        }
    }



}
