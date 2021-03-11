using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// stores and manages conditions and transitions for sub-events within a single quest
/// 
/// kept in globalSingleton
/// </summary>
public class QuestConditionManager : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;
    [Inject(InjectFrom.Anywhere)]
    public QuestLoader questLoader;
    [Inject(InjectFrom.Anywhere)]
    public GlobalSingleton globalSingleton; //data persist

    public bool active;

    public Quest questTracking; //the quest that's being tracked
    public List<QuestEvent> currQuestEvents;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void checkForEventConditions(ActionListener.Listener listener) //TODO more params
    {
        foreach(QuestEvent qEvent in currQuestEvents)
        {
            if (qEvent.conditionsMet()) //might need parameters here too
            {
                //conditions met

            }
        }
    }
}

/// <summary>
/// corresponds to a block of event in a csv file, has trigger conditions
/// </summary>
public struct QuestEvent
{
    public Func<bool> conditionsMet;
    public string eventName; //tag
    //TODO store pointer to csv file & line number

}
