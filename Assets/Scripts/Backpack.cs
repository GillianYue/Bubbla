using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public GameObject backpack;
    public GameControl gameControl;
    public int numGrids, numItems;
    public int[] itemList; //array indicating items belonging to a user via itemIndex
    public int[] itemCount; //corresponds to the above array indicating num of item owned of that itemIndex
    public GameObject itemBG; //initial bg to duplicate
    public GameObject itemMold; //sprite to put in front of itemBG

    // Start is called before the first frame update
    void Start()
    {

        loadItemMold();

        //for testing purposes, arbitrarily put in 
        numGrids = 7; numItems = 2;
        itemList = new int[numItems];
        itemCount = new int[numItems];

        itemList[0] = 0; itemList[1] = 1;
        itemCount[0] = 1; itemCount[1] = 5;

        loadInItems();

    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     * this function should be called once we're sure that the backpack related data is all loaded
     * 
     * assumes itemLoader is ready, and that itemList/Count is ready (the latter achieved via loadUserData or something
     */
    public void loadInItems()
    {

        itemBG.SetActive(true);
        itemMold.SetActive(true);

        //first create needed num of itemBG 
        for (int n = 0; n < numGrids; n++)
        {
            GameObject currItemBG = Instantiate(itemBG, itemBG.transform.parent);
            int row = n / 4; int col = n % 4;
            currItemBG.GetComponent<RectTransform>().offsetMax = new Vector2(100 * (col + 1), -(100 * row + 20));
            currItemBG.GetComponent<RectTransform>().offsetMin = new Vector2(100 * col + 20, -100 * (row + 1));

            if (n < numItems)
            {
                GameObject currItem = Instantiate(itemMold, currItemBG.transform);
                Global.centerSpriteInGO(currItem, currItemBG);
            }
        }

        itemBG.SetActive(false);
        itemMold.SetActive(false);
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

    void loadItemMold()
    {
        itemMold = Resources.Load("ItemMold") as GameObject;
        if (itemMold == null)
        {
            Debug.LogError("load ItemMold failed");
        }
    }

}
