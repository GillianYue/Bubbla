using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class SteerPath : MonoBehaviour
{

    public bool mouseGenPath; //whether mouse click setting path destination is enabled
    public Transform[] pathPoints; //assign points GOs to this array to set the path on start

    public List<Vector3> nodesVector3;
    public bool curve; //if curve, interpolate curve; else assumed to be straight lines in between points

    public GameObject markingPrefab;
    public Color pathColor = Color.yellow;

    public bool rotationByPath;   //whether 'Enemy' rotates in path direction or not
    public bool loop;         //if loop is true, 'Enemy' returns to the path starting point after completing the path

    void Start()
    {
    }

    //for testing purposes
    void Update()
    {
        if (mouseGenPath && Input.GetMouseButtonDown(0))
        {
            Vector3 mouse = Global.ScreenToWorld(Input.mousePosition);
            addNode(mouse);
            GameObject marking = GameObject.Instantiate(markingPrefab);
            marking.transform.position = mouse;

        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouse = Global.ScreenToWorld(Input.mousePosition);
            addNode(mouse);
        }
    }

    public SteerPath(List<Vector3> list)
    {
        nodesVector3 = list;
    }

    public SteerPath(Transform[] list)
    {
        List<Vector3> pathPositions = getPointPositions(list);

        nodesVector3 = pathPositions;
    }

    //used for initializing nodesVector3 list
    public List<Vector3> getPointPositions(Transform[] list)
    {
        List<Vector3> pathPositions = new List<Vector3>();
        for (int i = 0; i < list.Length; i++)
        {
            pathPositions.Add(list[i].position);
        }

        return pathPositions;
    }

    public void addNode(Vector3 coord)
    {
        nodesVector3.Add(coord);
    }


    public List<Vector3> getNodes()
    {
        return nodesVector3;
    }

    public List<Vector3> getNodesAsVectorArray()
    {
        if (nodesVector3.Count > 0) return nodesVector3;

        //set up Vector3 array based on exposed points
        nodesVector3 = getPointPositions(pathPoints);

        return nodesVector3;
    }

    /// / ////// //
    void OnDrawGizmos()
    {
        DrawPath(pathPoints);
    }

    void DrawPath(Transform[] path) //drawing the path in the Editor
    {
        if (curve)
        {
            Vector3[] pathPositions = new Vector3[path.Length];
            for (int i = 0; i < path.Length; i++)
            {
                pathPositions[i] = path[i].position;
            }
            Vector3[] newPathPositions = CreatePoints(pathPositions);
            Vector3 previosPositions = Interpolate(newPathPositions, 0);
            Gizmos.color = pathColor;
            int SmoothAmount = path.Length * 20;
            for (int i = 1; i <= SmoothAmount; i++)
            {
                float t = (float)i / SmoothAmount;
                Vector3 currentPositions = Interpolate(newPathPositions, t);
                Gizmos.DrawLine(currentPositions, previosPositions);
                previosPositions = currentPositions;
            }

        }
        else
        {
            for (int i = 0; i < path.Length-1; i++)
            {
                Gizmos.DrawLine(path[i].position, path[i+1].position);
            }
        }
    }



    //// ///  /////// /// functions for curve navigation
    ///

    public Vector3 NewPositionByPath(Vector3[] pathPos, float percent)
    {
        return Interpolate(CreatePoints(pathPos), percent);
    }

    public Vector3 Interpolate(Vector3[] path, float t)
    {
        int numSections = path.Length - 3;
        int currPt = Mathf.Min(Mathf.FloorToInt(t * numSections), numSections - 1);
        float u = t * numSections - currPt;
        Vector3 a = path[currPt];
        Vector3 b = path[currPt + 1];
        Vector3 c = path[currPt + 2];
        Vector3 d = path[currPt + 3];
        return 0.5f * ((-a + 3f * b - 3f * c + d) * (u * u * u) + (2f * a - 5f * b + 4f * c - d) * (u * u) + (-a + c) * u + 2f * b);
    }

    public Vector3[] CreatePoints(Vector3[] path)  //using interpolation method calculating the path along the path points
    {
        Vector3[] pathPositions;
        Vector3[] newPathPos;
        int dist = 2;
        pathPositions = path;
        newPathPos = new Vector3[pathPositions.Length + dist];
        Array.Copy(pathPositions, 0, newPathPos, 1, pathPositions.Length);
        newPathPos[0] = newPathPos[1] + (newPathPos[1] - newPathPos[2]);
        newPathPos[newPathPos.Length - 1] = newPathPos[newPathPos.Length - 2] + (newPathPos[newPathPos.Length - 2] - newPathPos[newPathPos.Length - 3]);
        if (newPathPos[1] == newPathPos[newPathPos.Length - 2])
        {
            Vector3[] LoopSpline = new Vector3[newPathPos.Length];
            Array.Copy(newPathPos, LoopSpline, newPathPos.Length);
            LoopSpline[0] = LoopSpline[LoopSpline.Length - 3];
            LoopSpline[LoopSpline.Length - 1] = LoopSpline[2];
            newPathPos = new Vector3[LoopSpline.Length];
            Array.Copy(LoopSpline, newPathPos, LoopSpline.Length);
        }
        return (newPathPos);
    }


}
