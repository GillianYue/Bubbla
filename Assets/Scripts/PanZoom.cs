using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public GameObject moveAroundGO; //pos will change as mouse drag
    Vector3 moveGOstartPos;

    public float zoomOutMin = 1, zoomOutMax = 3;
    Vector2 extents = new Vector2(9999, 9999); //extents to which the moveAroundGO can move, details see notes in Map

    public delegate Vector2 RecalcExtents();
    public RecalcExtents recalcExtents;

    public bool checkForPanZoom;

    void Start()
    {
        
    }

    public void setExtentsCallback(RecalcExtents recalcE)
    {
        recalcExtents = recalcE;
        recalcExtents();
    }

    void Update()
    {
        if (checkForPanZoom)
        {
            if (Input.GetMouseButtonDown(0))
            {
                moveGOstartPos = moveAroundGO.transform.localPosition;
                touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 dest = moveGOstartPos - direction;
                if (Mathf.Abs(dest.x) <= extents.x && Mathf.Abs(dest.y) <= extents.y)
                {
                    moveAroundGO.transform.localPosition = dest;
                }
                else if (Mathf.Abs(dest.x) <= extents.x)
                {
                    Vector3 curr = moveAroundGO.transform.localPosition;
                    moveAroundGO.transform.localPosition = new Vector3(dest.x, curr.y, curr.z);
                }
                else if (Mathf.Abs(dest.y) <= extents.y)
                {
                    Vector3 curr = moveAroundGO.transform.localPosition;
                    moveAroundGO.transform.localPosition = new Vector3(curr.x, dest.y, curr.z);
                }

            }

            if (Input.GetMouseButtonUp(0))
            {
                moveGOstartPos = moveAroundGO.transform.localPosition;
            }

            if (Input.touchCount == 2) //TODO need test on ios
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
    }

    void zoom(float increment)
    {
        float scl = moveAroundGO.transform.localScale.x;
        float newScl = Mathf.Clamp(scl+increment, 
            zoomOutMin, zoomOutMax);
        moveAroundGO.transform.localScale = new Vector3(newScl, newScl, 1);

        if(recalcExtents != null) extents = recalcExtents(); //scale change will result in different extents

        //after scaling, we could be out of boundary, need to check and nudge back
        Vector3 curr = moveAroundGO.transform.localPosition;
        if (curr.x < -extents.x) curr.x = -extents.x;
        else if (curr.x > extents.x) curr.x = extents.x;

        if (curr.y < -extents.y) curr.y = -extents.y;
        else if (curr.y > extents.y) curr.y = extents.y;

        moveAroundGO.transform.localPosition = curr;
    }
}
