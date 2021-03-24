using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class MapLoader : Loader
{

    private bool[] csvLoadDone; //when loadDone[0] == true, loading is done for the csv file 
    public TextAsset mapCsv;
    private string[,] data; //double array that stores all info 

    //all of the lists below start count at 0, which means item s will keep track of info for site s+1
    private List<Site> sites;

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
    public string getSiteNameOfIndex(int idx) { idx = Mathf.Clamp(idx, 1, sites.Count-1); return sites[idx].siteName; }
    public Vector2 getSiteLocationOfIndex(int idx) { idx = Mathf.Clamp(idx, 1, sites.Count - 1); return sites[idx].coordinateOnMap; }
    public List<Sublocation> getSublocationsOfSite(int siteIdx) { siteIdx = Mathf.Clamp(siteIdx, 1, sites.Count - 1); return sites[siteIdx].sublocations.Values.ToList(); }

    public int getNumSites() { return sites.Count; }

    IEnumerator parseMapData()
    {
        yield return new WaitUntil(() => csvLoadDone[0]); //this would mean that data is ready to be parsed

        int numSites = -1;
        int.TryParse(data[0, data.GetLength(1)-1], out numSites); //get last site's index (=total num of sites)

        sites = new List<Site>();

        int currSiteIdx = -1;
        //skip row 0 because those are all descriptors
        for (int r = 1; r < data.GetLength(1); r++)
        {
            
            int sidx = -1;
            int.TryParse(data[0, r], out sidx);

            if (sidx != currSiteIdx) //arrived at a new site row (if same, means this is a sublocation)
            {
                Dictionary<int, Sublocation> currSiteSublocations = new Dictionary<int, Sublocation>();
                Site site = new Site();
                sites.Add(site);

                site.sublocations = currSiteSublocations;
                site.siteName = data[2, r];

                Vector2 siteLocation = new Vector2();
                Global.parseVector2Parameter(data[4, r], ref siteLocation);
                site.coordinateOnMap = siteLocation;

                currSiteIdx = sidx;
            }
            else //is a sublocation row, needs to load in sublocation name and image
            {
                Sublocation sub = new Sublocation();
                int subIndex = int.Parse(data[1, r]);

                sites[sites.Count - 1].sublocations.Add(subIndex, sub);
                sub.sublocationName = data[2, r];
                sub.sublocationIndex = subIndex;

                sub.prefabName = data[5, r];

                bool showOnMap = bool.Parse(data[6, r]);
                sub.showOnMap = showOnMap;

                if (data[3, r] != "" && showOnMap)
                {
                    var spr = Resources.Load<Sprite>("Images/Sites/" + data[3, r]);
                    if (spr != null) sub.displayImage = spr; //add sublocation image to "this site"'s group
                    else { 
                        Debug.LogError("load sublocation image failed: at " + "Images/Sites/" + data[3, r]);
                        sub.displayImage = defaultSublocationSprite;
                    }
                }//else, no displayImage is needed, since won't show on map


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

public class Site
{
    public Vector2 coordinateOnMap;
    public string siteName;
    public Dictionary<int, Sublocation> sublocations;

}

public class Sublocation
{
    public string sublocationName, prefabName;
    public Sprite displayImage;
    public bool showOnMap;
    public int sublocationIndex;
}

