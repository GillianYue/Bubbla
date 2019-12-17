using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ivsInteractable : MonoBehaviour
{
    private int myGoToLine = -1;
    private int indexInArray = -1;

    private int interactableDist; //default

    public ivsInteractable(int arrayIndex, int goToLine)
    {
        indexInArray = arrayIndex;
        myGoToLine = goToLine;
    }

    // Start is called before the first frame update
    void Start()
    {
        interactableDist = 120;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //determines if the other GO (usually player) is close enough to this particular ivs interactable
    public bool closeEnough(GameObject other)
    {
        //-1: always interactable; dist<interactableDist = close enough
        float dist = Global.findVectorDist(transform.position, other.transform.position);

        return ( dist <= interactableDist);

    }

    public void setindexInArray(int index)
    {
        indexInArray = index;
    }

    public int getIndexInArray()
    {
        return indexInArray;
    }

    public void setIvsGoToLine(int line)
    {
        myGoToLine = line;
    }

    public int getIvsGoToLine()
    {
        return myGoToLine;
    }

    public void setInteractableDist(int dist)
    {
        interactableDist = dist;
    }
}
