using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public GameObject moveAroundGO; //pos will change as mouse drag
    Vector3 moveGOstartPos;

    public float zoomOutMin = 1,
        zoomOutMax = 3;

    public Camera camForZoom;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            moveGOstartPos = moveAroundGO.transform.position;
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            moveAroundGO.transform.position = moveGOstartPos - direction;

        }

        if (Input.GetMouseButtonUp(0))
        {
            moveGOstartPos = moveAroundGO.transform.position;
        }

        if(Input.touchCount == 2) //TODO need test on ios
        {
            Touch t0 = Input.GetTouch(0), t1 = Input.GetTouch(1);
            Vector3 prevPos0 = t0.position - t0.deltaPosition,
                prevPos1 = t1.position - t1.deltaPosition;

            float prevMag = (prevPos0 - prevPos1).magnitude,
                currMag = (t0.position - t1.position).magnitude;

            float diff = currMag - prevMag;

            zoom(diff * 0.01f);

        }
        else
        {
            zoom(Input.GetAxis("Mouse ScrollWheel"));
        }

        
    }

    void zoom(float increment)
    {
        float scl = moveAroundGO.transform.localScale.x;
        float newScl = Mathf.Clamp(scl+increment, 
            zoomOutMin, zoomOutMax);
        moveAroundGO.transform.localScale = new Vector3(newScl, newScl, 1);


    }
}
