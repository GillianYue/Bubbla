using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//map related UI control; pan and zoom handled by PanZoom script
//a site is represented by a dot on the map, a site has many sublocations
public class Map : MonoBehaviour
{
    public Button[] dots;
    public GameObject dotPrefab;
    float mapWidth, mapHeight;

    public PanZoom panZoom;
    public GameObject siteDetailPanel; //UI

    public GameObject siteDetailListRect;
    public GameObject siteListItemPrefab; //sublocation list item prefab
    public Image sublocationImage; //display for sublocation image; assigned in editor

    public Camera myCam; //camera showing this map

    [Inject(InjectFrom.Anywhere)]
    public MapLoader mapLoader;
    [Inject(InjectFrom.Anywhere)]
    public LoadScene loadScene;

    public GameObject cancelScnPrefab;
    public GameObject cancel;

    public int currSelectSite = 1;

    void Awake()
    {
        mapWidth = GetComponent<RectTransform>().rect.width;
        mapHeight = GetComponent<RectTransform>().rect.height;

        panZoom = GetComponent<PanZoom>();
    }

    void Start()
    {
        StartCoroutine(Initialize());
    }

    void Update()
    {
        
    }

    IEnumerator Initialize()
    {
        yield return new WaitUntil(() => loadScene.isAllLoadDone());

        SpawnSiteDots();
    }

    /// <summary>
    /// instantiate site dot objects based on dot prefab; assigns data from mapLoader
    /// </summary>
    public void SpawnSiteDots()
    {
        int numSites = mapLoader.siteNames.Length; //e.g. 24 sites, site 1-24 data found in array[0-23]
        dots = new Button[numSites]; //starts with 0, so dots[s] is for site index s+1

        for (int s=0; s<numSites; s++)
        {
            GameObject dot = Instantiate(dotPrefab);
            dot.transform.parent = transform; //Map object's transform should be parent
            dots[s] = dot.GetComponent<Button>();

            var sIndex = s;

            SiteInfo info = dot.GetComponent<SiteInfo>();
            info.siteIndex = sIndex+1; //since first site index starts at 1
            info.siteName = mapLoader.getSiteNameOfIndex(info.siteIndex);
            info.sublocationImages = mapLoader.getSublocationSpritesOfIndex(info.siteIndex);
            info.sublocationNames = mapLoader.getSublocationNamesOfIndex(info.siteIndex);

            //sets local position of dot
            dot.transform.localPosition = mapLoader.getSiteLocationOfIndex(info.siteIndex);
            dot.transform.localScale = Vector3.one;

            //add onclick event to button
            dot.GetComponent<Button>().onClick.AddListener(() => {
                onClickSiteDot(info.siteIndex);
            });
        }
    }

    // opens up map UI 
    public void openUI()
    {
        if (panZoom != null) panZoom.enabled = true;
        siteDetailPanel.SetActive(false);
        panZoom.checkForPanZoom = true;
    }

    // closes map UI
    public void closeUI()
    {
        if (panZoom != null) panZoom.enabled = false;
    }

    public void openSiteDetailPanel()
    {
        siteDetailPanel.SetActive(true);
        panZoom.checkForPanZoom = false;

        //cancel will block interaction w background UIs
        cancel = Instantiate(cancelScnPrefab) as GameObject;
        cancel.transform.SetParent(siteDetailPanel.transform.parent, false);
        cancel.SetActive(true);

        siteDetailPanel.transform.SetAsLastSibling();

        Button cclB = cancel.GetComponent<Button>();
        cclB.onClick.AddListener(closeSiteDetailPanel);
    }

    public void closeSiteDetailPanel()
    {
        siteDetailPanel.SetActive(false);
        panZoom.checkForPanZoom = true;
        GameObject.Destroy(cancel);
    }

    /// <summary>
    /// index is for the sublocation
    /// </summary>
    /// <param name="listItem"></param>
    /// <param name="index"></param>
    void setSiteSublocationData(GameObject listItem, int index, List<string> namesList, List<Sprite> spritesList)//TODO
    {
        listItem.transform.GetChild(0).GetComponent<Text>().text = namesList[index];
        //listItem.transform.GetChild(1).GetComponent<Image>().sprite = TODO Load in some sprite here;
        //TODO set list item thing based on curr site info 

    }

    /// <summary>
    /// onclick event for dots[dotIndex], will pan & zoom to that site and open up the sublocation UI
    /// </summary>
    /// <param name="dotIndex"></param>
    public void onClickSiteDot(int siteIndex)
    {

        List<string> namesList = mapLoader.getSublocationNamesOfIndex(siteIndex);
        List<Sprite> spritesList = mapLoader.getSublocationSpritesOfIndex(siteIndex);

        //set up site detail for new site
        ListScroller.setupList(siteDetailListRect, siteListItemPrefab, mapLoader.getSublocationNamesOfIndex(siteIndex).Count, 
            (GameObject listItem, int index) => { setSiteSublocationData(listItem, index, namesList, spritesList); });

        //pan & zoom to site, and then open up site detail panel
        panToSite(dots[siteIndex-1].transform.position, openSiteDetailPanel);

        print("selected site at " + mapLoader.getSiteNameOfIndex(siteIndex));
    }

    /// <summary>
    /// moves view to spot where the dot for site is visible and suitable for further UI
    /// 
    /// will check which quadrant the dot is in relative to the map, then move accordingly
    /// 
    /// if the site is in upper left corner of map, then we will be seeing the dot relatively upper left too and show more of the 
    /// bottom left part of the map (also space for UI window)
    /// </summary>
    /// <param name="pos"></param>
    public void panToSite(Vector3 pos, Callback callback)
    {
        panZoom.smoothLerpTo(pos, panZoom.zMin, callback); //maximize zoom when lerp to site

    }

}
