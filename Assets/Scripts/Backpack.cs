using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public GameObject backpack;
    public GameControl gameControl;
    public int numItems;
    public int[] itemList; //array indicating items belonging to a user via itemIndex
    public int[] itemCount; //corresponds to the above array indicating num of item owned of that itemIndex

    // Start is called before the first frame update
    void Start()
    {

        //for testing purposes
        numItems = 3;
        itemList = new int[numItems];
        itemCount = new int[numItems];



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
