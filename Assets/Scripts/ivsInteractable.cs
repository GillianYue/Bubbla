using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ivsInteractable : interactable
{
    private int myGoToLine = -1;
    private int indexInArray = -1;

    [Inject(InjectFrom.Anywhere)]
    public ActionListenerManager actionListenerManager;

    //private int interactableDist; //from parent

    void Start()
    {
        interactableDist = 150;

        if (!actionListenerManager) actionListenerManager = FindObjectOfType<ActionListenerManager>();
    }

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

    public override void interact()
    {
        string[] eventParams = Global.makeParamString(GetComponent<identifier>().id);

        //trigger listener
        actionListenerManager.onTriggerListener(ActionListener.Listener.interactWithObject, eventParams);
        
        //TODO if triggers event related thing, do that
        //else, check if local has content??? or maybe put base layer local interactions in the global space through metafile too. DK
    }

}
