using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public GameObject backpack;
    public GameControl gameControl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void openBackpackUI()
    {
        gameControl.ckTouch = false; //to stop checking on game progress
        gameControl.pauseGame();

            if (backpack != null)
        {
            backpack.SetActive(true);

        }
    }

    public void closeBackpackUI()
    {
        gameControl.ckTouch = true; //to start checking on game progress
        gameControl.resumeGame();

        Time.timeScale = 1.0f; //resume gameplay

        Transform aimy = gameControl.player.transform.Find("Aim(Clone)");
        if (aimy != null)
        {
            Destroy(aimy.gameObject);
        }

        //might eventually play an animation (UI screen folding or something) here before setting to inactive

        if (backpack != null)
        {
            backpack.SetActive(false);

        }
    }
     
    }
