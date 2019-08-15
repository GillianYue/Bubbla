using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTestBehavior : MonoBehaviour
{
    public int startLine; //which line for levelDLG to start
    public GameFlow gf;
    public bool test;
    public int speedUpRate;

    // Start is called before the first frame update
    void Start()
    {
        if (test)
        {
            gf.setPointer(startLine);
        }
    }

    // Update is called once per frame
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
        }

    }
}
