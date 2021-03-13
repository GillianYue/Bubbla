using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public void checkForEventConditions(ActionListener.Listener listener) //TODO more params
    {
        foreach(QuestEvent qEvent in GlobalSingleton.Instance.questStatus.activeQuestData.getQuestEvents())
        {
            if (qEvent.conditionsMet()) //might need parameters here too
            {
                //conditions met

            }
        }
    }
}


