using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTestBehavior : MonoBehaviour
{
    public int startLine; //which line for levelDLG to start

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;
    [Inject(InjectFrom.Anywhere)]
    public GameFlow gf;
    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;
    [Inject(InjectFrom.Anywhere)]
    public Player player;

    [Inject(InjectFrom.Anywhere)]
    public SetUpQuestBoard qb;

    public Text GameModeIndicator, ScriptModeIndicator;

    public bool test, invincible, goToLineOnStart, displayDebugTexts;
    public int speedUpRate;

    void Start()
    {
        if (goToLineOnStart && gf!=null)
        {
            gf.setPointer(startLine); //for testing I'm putting in what line I'm seeing in the csv file
        }

        if (invincible) player.invincible = true;
    }

    void Update()
    {
        if (test)
        {
            if (Input.GetKeyDown("space")) //space for speeding time up
            {
                Time.timeScale = 1.0f * speedUpRate;
            }

            if (Input.GetKeyDown("enter")) 
            {
                gf.incrementPointer(); //to skip lines when necessary
            }

            if (Input.GetKeyUp("space")) //release to go back to normal time
            {
                Time.timeScale = 1.0f;
            }

            if (displayDebugTexts) { 
                if (GameModeIndicator) GameModeIndicator.text = "GameMode: "+gameControl.sceneType.ToString();
                if (ScriptModeIndicator) ScriptModeIndicator.text = "ScriptMode: " + gf.currMode.ToString();
            }
            //if (Input.GetKeyDown(KeyCode.S)) //save
            //{
            //    saveLoad.SaveQuestStatus(qb.getCurrentQuestStatus());
            //}
        }

    }

    public static void pauseGame()
    {
        Time.timeScale = 0;
    }

    public static void resumeGame()
    {
        Time.timeScale = 1;
    }
}
