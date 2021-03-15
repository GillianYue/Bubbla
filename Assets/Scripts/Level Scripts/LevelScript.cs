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


}
