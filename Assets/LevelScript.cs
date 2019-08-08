using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * abstract base class for levelscripts, which contain custom functions for special events in each level
 */
public abstract class LevelScript : MonoBehaviour
{

    CustomEvents customEvents;

    void Start()
    {
        customEvents = gameObject.GetComponent<CustomEvents>();
    }

    void Update()
    {
        
    }

    public abstract IEnumerator levelScriptEvent(int code, bool[] done);



}
