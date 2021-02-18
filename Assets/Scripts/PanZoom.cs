using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// map image size >= map mask rect size
/// 
/// </summary>
public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    public RectTransform moveAroundGO; //pos will change as mouse drag
    Vector3 moveGOstartPos;

    Vector2 extentsX, extentsY = new Vector2(9999, 9999);

    public bool checkForPanZoom;

    bool lerpMoving; //whether is in motion of zooming to target scale, will ignore input when true
    Vector3 panDest;
    float zoomDest;

    //variables below should be set in editor
    public float zMin, zMax;
    public float zInitialBase; //offset added to zMin and zMax (those two values are relative)
    public Vector2 extentsXMin, extentsXMax, //extentsXMin.x is left bound, .y is right bound; same for max
    extentsYMin, extentsYMax;  //.x is for top bound, .y is for bottom

    void Start()
    {
        zInitialBase = moveAroundGO.transform.position.z;
        recalcExtents();

    }


    public void recalcExtents()
    {
        float percent = (moveAroundGO.transform.position.z - zMin) / (zMax - zMin); //1% is at zMax
        extentsX = Vector2.Lerp(extentsXMax, extentsXMin, percent); extentsY = Vector2.Lerp(extentsYMax, extentsYMin, percent);

    }

    void Update()
    {
        if (lerpMoving)
        {
            moveAroundGO.localPosition = moveAroundGO.localPosition + 0.001f * panDest;
            float scl = moveAroundGO.localScale.x, newScl = scl + zoomDest * 0.001f;
            moveAroundGO.localScale = new Vector3(newScl, newScl, 1);

            if (checkForDestReach()) lerpMoving = false;
        }
        else if (checkForPanZoom) //when lerp, don't check for input control
        {
            if (Input.GetMouseButtonDown(0))
            {
                moveGOstartPos = moveAroundGO.anchoredPosition;
                touchStart = Input.mousePosition;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 direction = touchStart - Input.mousePosition;
                Vector3 dest = moveGOstartPos - direction * 0.5f;

                if (dest.x >= Mathf.Min(extentsX.x, extentsX.y) &&
                    dest.x <= Mathf.Max(extentsX.x, extentsX.y) &&
                    dest.y >= Mathf.Min(extentsY.x, extentsY.y) &&
                    dest.y <= Mathf.Max(extentsY.x, extentsY.y))
                {
                    moveAroundGO.anchoredPosition = dest;
                }
                else if (dest.x >= Mathf.Min(extentsX.x, extentsX.y) &&
                    dest.x <= Mathf.Max(extentsX.x, extentsX.y))
                {
                    Vector3 curr = moveAroundGO.anchoredPosition;
                    moveAroundGO.anchoredPosition = new Vector3(dest.x, curr.y, curr.z);
                }
                else if (dest.y >= Mathf.Min(extentsY.x, extentsY.y) &&
                    dest.y <= Mathf.Max(extentsY.x, extentsY.y))
                {
                    Vector3 curr = moveAroundGO.anchoredPosition;
                    moveAroundGO.anchoredPosition = new Vector3(curr.x, dest.y, curr.z);
                }

            }

            if (Input.GetMouseButtonUp(0))
            {
                moveGOstartPos = moveAroundGO.anchoredPosition;
            }

            if (Input.touchCount == 2) //TODO need test on ios
            {
                Touch t0 = Input.GetTouch(0), t1 = Input.GetTouch(1);
                Vector3 prevPos0 = t0.position - t0.deltaPosition,
                    prevPos1 = t1.position - t1.deltaPosition;

                float prevMag = (prevPos0 - prevPos1).magnitude,
                    currMag = (t0.position - t1.position).magnitude;

                float diff = currMag - prevMag;

                zoom(diff * 0.01f, Input.mousePosition);

            }
            else
            {
                zoom(Input.GetAxis("Mouse ScrollWheel"), Input.mousePosition);
            }

        }
    }


    void zoom(float increment, Vector2 center)
    {

        Vector2 mouseScreenPos = center;
        Ray mouseWorldRay = Camera.main.ScreenPointToRay(mouseScreenPos);

        Vector3 newPos = mouseWorldRay.origin + (mouseWorldRay.direction * increment * 3000);

        Vector3 newSetPos = Vector3.MoveTowards(Camera.main.transform.position,
            newPos, increment * 30000f * Time.deltaTime);
        Vector3 deltaPos = Camera.main.transform.position - newSetPos;

        Vector3 finalDest = moveAroundGO.transform.position + deltaPos * (increment > 0 ? 1 : -1);

        if (finalDest.z < zMax + zInitialBase && finalDest.z > zMin + zInitialBase)
        {
            moveAroundGO.transform.position = finalDest;
        }

        recalcExtents();


        if (increment != 0)
        {
            //after scaling, we could be out of boundary, need to check and nudge back
            Vector3 curr = moveAroundGO.anchoredPosition;

            if (curr.x < Mathf.Min(extentsX.x, extentsX.y)) curr.x = Mathf.Min(extentsX.x, extentsX.y);
            else if (curr.x > Mathf.Max(extentsX.x, extentsX.y)) curr.x = Mathf.Max(extentsX.x, extentsX.y);

            if (curr.y < Mathf.Min(extentsY.x, extentsY.y)) curr.y = Mathf.Min(extentsY.x, extentsY.y);
            else if (curr.y > Mathf.Max(extentsY.x, extentsY.y)) curr.y = Mathf.Max(extentsY.x, extentsY.y);

            moveAroundGO.anchoredPosition = curr;
        }

    }
    public bool isNotScaled()
    {
        return moveAroundGO.localScale.x == 1;
    }

    public void smoothLerpTo(Vector3 dest)
    {
        smoothLerpTo(dest, moveAroundGO.transform.localScale.x);
    }

    /// <summary>
    /// use local position for the item to zoom on
    /// </summary>
    /// <param name="scl"></param>
    public void smoothLerpTo(float zoomScl)
    {
        smoothLerpTo(moveAroundGO.transform.localPosition, zoomScl);
    }

    /// <summary>
    /// setting both panning and zooming destinations for lerp movement
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="scl"></param>
    public void smoothLerpTo(Vector3 dest, float zoomScl)
    {
        lerpMoving = true;
        zoomDest = zoomScl;
        panDest = dest;
    }

    bool checkForDestReach()
    {
        float currScl = moveAroundGO.transform.localScale.x;
        Vector3 currPos = moveAroundGO.transform.localPosition;

        return (Mathf.Abs(currScl - zoomDest) < 0.2f && findVectorDist(currPos, panDest) < 20);
    }


    public static float findVectorDist(Vector2 v1, Vector2 v2)
    {
        float d = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2));
        return d;
    }
}
