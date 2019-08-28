using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public GameObject backpack, itemSelected, spriteMask;
    private ItemSelect itemSelect; //script to select item, interact with item select btns, etc. Found on itemSelected GO
    public GameControl gameControl;
    public int numGrids, numItems;
    public int[] itemList; //array indicating items belonging to a user via itemIndex
    public int[] itemCount; //corresponds to the above array indicating num of item owned of that itemIndex
    public GameObject itemBG; //initial bg to duplicate
    private GameObject itemMold; //sprite to put in front of itemBG
    private ItemLoader itemLoader;
    public bool backpackBtnActive = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObject loader = GameObject.FindWithTag("Loader");
        if(loader != null)
        {
            itemLoader = loader.GetComponent<ItemLoader>();
        }
        else
        {
            Debug.LogError("loader GO not found!");
        }

        itemSelect = itemSelected.GetComponent<ItemSelect>();

        Sprite spr = spriteMask.GetComponent<SpriteMask>().sprite;
        Global.resizeSpriteToRectXY(spriteMask, spr); //resize itemsMask's spriteMask to its own rect transform

        loadItemMold();

        //for testing purposes, arbitrarily put in 
        numGrids = 12; numItems = 2;
        itemList = new int[numItems];
        itemCount = new int[numItems];

        itemList[0] = 0; itemList[1] = 1;
        itemCount[0] = 1; itemCount[1] = 5;

        //loadInItems(); eventually oughta switch back to this instead of the below line
        StartCoroutine(waitTilDone());
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
     *  tmp function to load backpack only after all loaders are ready. 
     * this wouldn't be used in game because all the loading would be done in the loading scene
     */
    IEnumerator waitTilDone()
    {
        yield return new WaitUntil(() => itemLoader.itemLoaderDone);
        loadInItems();
    }

    /**
     * this function should be called once we're sure that the backpack related data is all loaded
     * 
     * assumes itemLoader is ready, and that itemList/Count is ready (the latter achieved via loadUserData or something
     */
    public void loadInItems()
    {

        if (numItems > 8) numGrids = 4 * (int)Mathf.Ceil(numItems / 4.0f);

        itemBG.SetActive(true);
        itemMold.SetActive(true);

        GameObject items = itemBG.transform.parent.gameObject; RectTransform itmRT = items.GetComponent<RectTransform>();
        Global.setRectTransform(items, itmRT.rect.width,
            (numGrids / 4) * 100 + 20); //so that items' rect always fits perfectly the grids that are created
        itmRT.pivot = new Vector2(0, 1); //center properly

        //first create needed num of itemBG 
        for (int n = 0; n < numGrids; n++)
        {
            GameObject currItemBG = Instantiate(itemBG, itemBG.transform.parent);
            int row = n / 4; int col = n % 4;
            currItemBG.GetComponent<RectTransform>().offsetMax = new Vector2(100 * (col + 1), -(100 * row + 20));
            currItemBG.GetComponent<RectTransform>().offsetMin = new Vector2(100 * col + 20, -100 * (row + 1));
            currItemBG.GetComponent<identifier>().setID("itemBG" + n);

            if (n < numItems)
            {
                GameObject currItem = itemLoader.getItemInstance(itemList[n]);
                currItem.transform.parent = currItemBG.transform;
                Vector3 newPos = currItem.transform.localPosition;
                newPos.z = -1;
                currItem.transform.localPosition = newPos; //so that it shows on top of the background
                Global.centerSpriteInGO(currItem, currItemBG);
                currItem.GetComponent<SpriteRenderer>().sprite = itemLoader.S0_SPRITE[itemList[n]]; //itemlist[n] returns itemCode
            }

            if (n == 0) itemSelect.setItemSelected(currItemBG); //so that on start default is selecting the first item
        }

        itemBG.SetActive(false); //because this was in game
        //itemMold.SetActive(false);
    }

    public void openBackpackUI()
    {
        if (backpackBtnActive)
        {
            gameControl.ckTouch = false; //to stop checking on game progress
            gameControl.pauseGame();

            if (backpack != null)
            {
                backpack.SetActive(true);

            }
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
