using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Button[] dots;
    float mapWidth, mapHeight;

    public PanZoom panZoom;

    void Start()
    {
        mapWidth = GetComponent<RectTransform>().rect.width;
        mapHeight = GetComponent<RectTransform>().rect.height;

        panZoom = GetComponent<PanZoom>();

        panZoom.setExtentsCallback(getCurrentMapExtents);
    }

    void Update()
    {
        
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
        if(pos.x < 0 && pos.y < 0) //bottom left
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
