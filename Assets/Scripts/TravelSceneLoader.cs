using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelSceneLoader : Loader
{
    private bool[] loadDone;  //this bool is only for the level progress file, not everything

    public TextAsset sceneCsv; //dialogue file for a specific level
    private string[,] data; //double array that stores all info of this level
    public GameObject BG; //parent of background prefabs

    private bool travelSceneLoadDone;

    protected override void Start()
    {
        base.Start();
        loadDone = new bool[1];
        bool[] parseDone = new bool[1];
        StartCoroutine(LoadScene.processCSV(loadDone, sceneCsv, setData, parseDone, true)); //if true, will match excel line num

        StartCoroutine(loadTravelScene());
    }

    public void setData(string[,] d, bool[] parseDone)
    {
        data = d;
        parseDone[0] = true;
    }

    void Update()
    {
        
    }

    /// <summary>
    /// overrides parent
    /// </summary>
    /// <returns></returns>
    public override bool isLoadDone()
    {
        return travelSceneLoadDone;
    }


    IEnumerator loadTravelScene()
    {
        yield return new WaitUntil(() => (data != null));
        loadBackgroundPrefab();

        travelSceneLoadDone = true;
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
