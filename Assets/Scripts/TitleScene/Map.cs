﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//map related UI control; pan and zoom handled by PanZoom script
//a site is represented by a dot on the map, a site has many sublocations
public class Map : MonoBehaviour
{
    public Button[] dots;
    float mapWidth, mapHeight;

    public PanZoom panZoom;
    public GameObject siteDetailPanel; //UI

    public GameObject siteDetailListRect;
    public GameObject siteListItemPrefab; //sublocation list item prefab
    public Image sublocationImage; //display for sublocation image; assigned in editor

    void Start()
    {
        mapWidth = GetComponent<RectTransform>().rect.width;
        mapHeight = GetComponent<RectTransform>().rect.height;

        panZoom = GetComponent<PanZoom>();

        panZoom.setExtentsCallback(getCurrentMapExtents);

        //TODO some load in dots process, need to associate dots with the sublocations that they hold
        //there should be something that keeps track of all dots, their locations, indices and sublocations (sublocations tracking sub-images and names)

        for(int d = 0; d < dots.Length; d++)
        {
            var fix = d; //because d is dynamic, can't be used for callback functions
            dots[d].GetComponent<SiteInfo>().siteIndex = d;

            dots[fix].onClick.AddListener(() => {
                onClickSiteDot(dots[fix].GetComponent<SiteInfo>().siteIndex); 
            });
        }

        ListScroller.setupListComponents(siteDetailListRect, siteListItemPrefab, 15); //set to a default location on start
        ListScroller.genListItems(siteListItemPrefab, 15, siteDetailListRect, setSiteSublocationData);
    }

    void Update()
    {
        
    }

    public void openUI()
    {
        if (panZoom != null) panZoom.enabled = true;
        siteDetailPanel.SetActive(false);
        panZoom.checkForPanZoom = true;
    }

    public void closeUI()
    {
        if (panZoom != null) panZoom.enabled = false;
    }

    public void openSiteDetailPanel()
    {
        siteDetailPanel.SetActive(true);
        panZoom.checkForPanZoom = false;
    }

    public void closeSiteDetailPanel()
    {
        siteDetailPanel.SetActive(false);
        panZoom.checkForPanZoom = true;
    }

    /// <summary>
    /// returns the furthest the map can be moved under the current scale
    /// 
    /// if returning (200,300), means map can have a maximum x of 200 and a minimum x of -200
    /// </summary>
    /// <returns></returns>
    Vector2 getCurrentMapExtents()
    {
        Vector3 scl = transform.localScale;
        float xExt = (scl.x - 1) * mapWidth / 2, yExt = (scl.y - 1) * mapHeight / 2;
        return new Vector2(xExt, yExt);
    }

    void setSiteSublocationData(GameObject listItem, int index)
    {
        listItem.transform.GetChild(0).GetComponent<Text>().text = "sublocation " + index.ToString();
        //listItem.transform.GetChild(1).GetComponent<Image>().sprite = TODO Load in some sprite here;
    }

    /// <summary>
    /// onclick event for dots[dotIndex]
    /// </summary>
    /// <param name="dotIndex"></param>
    public void onClickSiteDot(int dotIndex)
    {
        ListScroller.setupListComponents(siteDetailListRect, siteListItemPrefab, 5); //needs to match "that site"

        panToSite(dots[dotIndex].transform.localPosition);
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
    public void panToSite(Vector3 pos)
    {
        panZoom.smoothLerpTo(pos, 2);

        if (pos.x < 0 && pos.y < 0) //bottom left
        {

        }else if(pos.x < 0 && pos.y > 0) //upper left
        {

        }else if (pos.y < 0) //bottom right
        {

        }
        else //upper right
        {

        }
    }

}
