using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//add dialogue integration here, specific map UI functions will be written in Map
public class Outing : MonoBehaviour
{

    public GameObject outing;
    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;

    public Vector3 startPos;
    public PanZoom panZoomControl;
    [Inject(InjectFrom.Anywhere)]
    public Map map;

    void Start()
    {
        if (outing == null) outing = gameObject;

        startPos = outing.transform.position;
        if(panZoomControl != null) panZoomControl.enabled = false;
    }

    void Update()
    {
        
    }

    public void openMapUI()
    {
        if (outing != null)
        {
            Global.changePos(outing, 0, 0);

            outing.SetActive(true);
            if (panZoomControl != null) panZoomControl.enabled = true;
        }
    }

    public void closeMapUI()
    {
        if (outing != null)
        {
            Global.changePos(outing, (int)startPos.x, (int)startPos.y);

            outing.SetActive(false);
            dialogue.gameObject.SetActive(true);
            if (panZoomControl != null) panZoomControl.enabled = false;
        }
    }
}
