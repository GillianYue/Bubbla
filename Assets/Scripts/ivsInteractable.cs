using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ivsInteractable : interactable
{
    private int myGoToLine = -1;
    private int indexInArray = -1;

    //private int interactableDist; //from parent

    // Start is called before the first frame update
    void Start()
    {
        interactableDist = 150;
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
