using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * abstract base class for levelscripts, which contain custom functions for special events in each level
 */
public abstract class LevelScript : MonoBehaviour
{
    protected GameControl gameControl;
    protected GameFlow gameFlow;
    protected CustomEvents customEvents;

    public void Start()
    {
        customEvents = GameObject.FindWithTag("CustomEvent").GetComponent<CustomEvents>();
        GameObject gameController = GameObject.FindWithTag("GameController");
        gameControl = gameController.GetComponent<GameControl>();
        gameFlow = gameController.GetComponent<GameFlow>();

    }

    void Update()
    {

    }

    public abstract IEnumerator levelScriptEvent(int code, bool[] done);

    public virtual void gameOver(GameObject GameOverC)
    {
        GameOverC.SetActive(true);
        Time.timeScale = 0;
    }

    public static string[] makeParamString(string a, string b)
    {
        string[] prms = new string[2];
        prms[0] = a; prms[1] = b; 
        return prms;
    }

    public static string[] makeParamString(string a, string b, string c)
    {
        string[] prms = new string[3];
        prms[0] = a; prms[1] = b; prms[2] = c;
        return prms;
    }

    public static string[] makeParamString(string a, string b, string c, string d)
    {
        string[] prms = new string[4];
        prms[0] = a; prms[1] = b; prms[2] = c; prms[3] = d;
        return prms;
    }

    public static string[] makeParamString(string a, string b, string c, string d, string e)
    {
        string[] prms = new string[5];
        prms[0] = a; prms[1] = b; prms[2] = c; prms[3] = d; prms[4] = e;
        return prms;
    }
}
