using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ivsInteractable : MonoBehaviour
{
    private int myGoToLine = -1;
    private int indexInArray = -1;

    public ivsInteractable(int arrayIndex, int goToLine)
    {
        indexInArray = arrayIndex;
        myGoToLine = goToLine;
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
