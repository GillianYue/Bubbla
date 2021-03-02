using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manages conditions and transitions for sub-events within a single quest
/// 
/// checks for what happens next & complete for a quest, and interacts with a 
/// </summary>
public class QuestProgressTracker : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;
    [Inject(InjectFrom.Anywhere)]
    public QuestLoader questLoader;
    [Inject(InjectFrom.Anywhere)]
    public GlobalSingleton globalSingleton; //data persist

    public bool active;

    public Quest questTracking;


    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
