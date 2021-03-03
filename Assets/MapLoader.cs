using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : MonoBehaviour
{

    private bool[] loadDone; //when loadDone[0] == true, loading is done for the csv file
    public TextAsset mapCsv;
    private string[,] data; //double array that stores all info 

    public Vector2[] siteLocations;
    public string[] siteNames;
    public List<List<string>> sublocationNames;
    public List<List<Sprite>> sublocationImages;

    public bool mapLoaderDone; //this will be set to true once is ready for usage


    void Start()
    {
        loadDone = new bool[1];

        StartCoroutine(LoadScene.processCSV(loadDone, mapCsv, setData, false)); //processCSV will call setData
        StartCoroutine(parseMapData()); //data will be parsed into local type arrays for speedy data retrieval

    }

    void Update()
    {

    }

    IEnumerator parseMapData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed

        int numSites = -1;
        int.TryParse(data[0, data.GetLength(1)], out numSites); //get last site's index (=total num of sites)
        print("num sites parsed: " + numSites);

        siteNames = new string[numSites]; //num rows, int[] is for the entire column
        siteLocations = new Vector2[numSites];
        sublocationNames = new List<List<string>>();
        sublocationImages = new List<List<Sprite>>();

        int currSiteIdx = -1;
        //skip row 0 because those are all descriptors
        for (int r = 1; r < data.GetLength(1); r++)
        {
            int sidx = -1;
            int.TryParse(data[0, r], out sidx);

            if (sidx != currSiteIdx) //arrived at a new site row (if same, means this is a sublocation)
            {
                List<Sprite> currSiteSublocationSprites = new List<Sprite>();
                List<string> currSiteSublocationNames = new List<string>();
                sublocationImages.Add(currSiteSublocationSprites);
                sublocationNames.Add(currSiteSublocationNames);

                siteNames[sidx - 1] = data[2, r];

                siteLocations[sidx-1] = new Vector2();
                Global.parseVector2Parameter(data[4, r], ref siteLocations[sidx - 1]);

            }
            else //is a sublocation row, needs to load in sublocation name and image
            {
                sublocationNames[sublocationNames.Count - 1].Add(data[2, r]);
                
                var spr = Resources.Load("Images/Sites/" + data[3, r]) as Sprite;
                if (spr) sublocationImages[sublocationImages.Count - 1].Add(spr); //add sublocation image to "this site"'s group
                else Debug.LogError("load sublocation image failed: at " + "Images/Sites/" + data[3, r]);
            }

        }

        mapLoaderDone = true;
        Debug.Log("MapLoader ready");
    }

    public void setData(string[,] d)
    {
        data = d;
    }


}

