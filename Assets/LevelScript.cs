using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * abstract base class for levelscripts, which contain custom functions for special events in each level
 */
public abstract class LevelScript : MonoBehaviour
{

    protected CustomEvents customEvents;

    void Start()
    {
        customEvents = gameObject.GetComponent<CustomEvents>();
    }

    void Update()
    {
        
    }

    public abstract IEnumerator levelScriptEvent(int code, bool[] done);

    public string[] makeParamString(string a, string b)
    {
        string[] prms = new string[2];
        prms[0] = a; prms[1] = b; 
        return prms;
    }

    public string[] makeParamString(string a, string b, string c)
    {
        string[] prms = new string[3];
        prms[0] = a; prms[1] = b; prms[2] = c;
        return prms;
    }

    public string[] makeParamString(string a, string b, string c, string d)
    {
        string[] prms = new string[4];
        prms[0] = a; prms[1] = b; prms[2] = c; prms[3] = d;
        return prms;
    }

    public string[] makeParamString(string a, string b, string c, string d, string e)
    {
        string[] prms = new string[5];
        prms[0] = a; prms[1] = b; prms[2] = c; prms[3] = d; prms[4] = e;
        return prms;
    }
}
