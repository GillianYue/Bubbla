using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoader : Loader
{

    private bool[] csvLoadDone; //when loadDone[0] == true, loading is done for the csv file 
    public TextAsset mapCsv;
    private string[,] data; //double array that stores all info 

    //all of the lists below start count at 0, which means item s will keep track of info for site s+1
    public Vector2[] siteLocations;
    public string[] siteNames;
    public List<List<string>> sublocationNames;
    public List<List<Sprite>> sublocationImages;

    private bool mapLoaderDone; //this will be set to true once is ready for usage
    public Sprite defaultSublocationSprite; //placeholder for sublocation image

    protected override void Start()
    {
        base.Start();

        defaultSublocationSprite = Sprite.Create(Texture2D.blackTexture, new Rect(), Vector2.zero);

        csvLoadDone = new bool[1];

        StartCoroutine(LoadScene.processCSV(csvLoadDone, mapCsv, setData, false)); //processCSV will call setData
        StartCoroutine(parseMapData()); //data will be parsed into local type arrays for speedy data retrieval

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
        return mapLoaderDone;
    }

    //functions below return info on a site with given site index
    public string getSiteNameOfIndex(int idx) { idx = Mathf.Clamp(idx, 1, siteNames.Length); return siteNames[idx-1]; }
    public Vector2 getSiteLocationOfIndex(int idx) { idx = Mathf.Clamp(idx, 1, siteNames.Length); return siteLocations[idx-1]; }
    public List<string> getSublocationNamesOfIndex(int idx) { idx = Mathf.Clamp(idx, 1, siteNames.Length); return sublocationNames[idx-1]; }
    public List<Sprite> getSublocationSpritesOfIndex(int idx) { idx = Mathf.Clamp(idx, 1, siteNames.Length); return sublocationImages[idx-1]; }

    IEnumerator parseMapData()
    {
        yield return new WaitUntil(() => csvLoadDone[0]); //this would mean that data is ready to be parsed

        int numSites = -1;
        int.TryParse(data[0, data.GetLength(1)-1], out numSites); //get last site's index (=total num of sites)

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

                currSiteIdx = sidx;
            }
            else //is a sublocation row, needs to load in sublocation name and image
            {
                sublocationNames[sublocationNames.Count - 1].Add(data[2, r]);

                if (data[3, r] != "")
                {
                    var spr = Resources.Load<Sprite>("Images/Sites/" + data[3, r]);
                    if (spr != null) sublocationImages[sublocationImages.Count - 1].Add(spr); //add sublocation image to "this site"'s group
                    else { 
                        Debug.LogError("load sublocation image failed: at " + "Images/Sites/" + data[3, r]);
                        sublocationImages[sublocationImages.Count - 1].Add(defaultSublocationSprite);
                    }
                }
                else
                {//add in default spaceholder if 
                    sublocationImages[sublocationImages.Count - 1].Add(defaultSublocationSprite);
                }
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

