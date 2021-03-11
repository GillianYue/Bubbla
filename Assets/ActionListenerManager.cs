using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListenerManager : MonoBehaviour
{
    public QuestConditionManager questConditionManager;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    /// <summary>
    /// during game, relevant listeners will call this function to indicate that the condition being listened to has happened
    /// 
    /// will talk to quest condition manager 
    /// </summary>
    /// <param name="listener"></param>
    public void onTriggerListener(ActionListener.Listener listener)
    {
        switch (listener)
        {
            case ActionListener.Listener.enterSite:
                //extract info specific to this type of listener
                break;
            case ActionListener.Listener.enterArea:

                break;
            case ActionListener.Listener.interactWithObject:

                break;
            //TODO more
        }
    }

}
