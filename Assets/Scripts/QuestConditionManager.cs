using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[SerializeField] public enum QuestCondition { tap, varEqual, varOver, varUnder }

/// <summary>
/// stores and manages conditions and transitions for sub-events within a single quest
/// 
/// kept in globalSingleton; persists but does not contain data that needs to be saved
/// 
/// </summary>
public class QuestConditionManager : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;
    [Inject(InjectFrom.Anywhere)]
    public QuestLoader questLoader;
    [Inject(InjectFrom.Anywhere)]
    public GlobalSingleton globalSingleton; //data persist

    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    /// <summary>
    /// go through events and see if anything meets condition
    /// </summary>
    /// <param name="listener"></param>
    public void checkForEventConditions(ActionListener.Listener listener, string[] eventParams)
    {
        QuestEvent newEvent = null;

        //loop through existing events in the curr active quest
        foreach(QuestEvent qEvent in GlobalSingleton.Instance.questStatus.activeQuestData.getQuestEvents())
        {
            if (qEvent.conditionsMet(listener, eventParams)) //might need parameters here too
            {
                //conditions met
                newEvent = qEvent;
                break;
            }
        }

        if(newEvent != null) //newEvent condition met, should switch to event's starting line
        {
            gameFlow.setPointer(newEvent.startLineNumber);
            Debug.Log("switched to event: " + newEvent.eventName);
        }
    }


}


