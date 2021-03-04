using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack : MonoBehaviour
{
    public GameObject backpack, itemSelected, spriteMask;

    [Inject(InjectFrom.Anywhere)]
    public ItemSelect itemSelect; //script to select item, interact with item select btns, etc. Found on itemSelected GO
    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;
    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;

    public int numGrids, numItems;
    public int[] itemList; //array indicating items belonging to a user via itemIndex
    public int[] itemCount; //corresponds to the above array indicating num of item owned of that itemIndex
    public GameObject itemBG; //initial bg to duplicate
    private GameObject itemMold; //sprite to put in front of itemBG

    [Inject(InjectFrom.Anywhere)]
    public ItemLoader itemLoader;

    Vector3 startPos;

    public bool backpackBtnActive = true,
        fullScreenMode; //if checked, affects num item listed in one row & size scale of displayed item image

    // Start is called before the first frame update
    void Start()
    {

        startPos = backpack.transform.position;

        Sprite spr = spriteMask.GetComponent<SpriteMask>().sprite;
        Global.resizeSpriteToRectXY(spriteMask, spr, spriteMask); //resize itemsMask's spriteMask to its own rect transform

        loadItemMold();

        //for testing purposes, arbitrarily put in 
        numGrids = 12; numItems = 2;
        itemList = new int[numItems];
        itemCount = new int[numItems];

        itemList[0] = 0; itemList[1] = 1;
        itemCount[0] = 1; itemCount[1] = 5;

        //loadInItems(); eventually oughta switch back to this instead of the below line

        StartCoroutine(Global.Chain(this, waitTilDone(), Global.Do(() => closeBackpackUI())));
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
        if(itemLoader != null)
        yield return new WaitUntil(() => itemLoader.isLoadDone());
        bool[] loadItemsDone = new bool[1];
        loadInItems(loadItemsDone);
        yield return new WaitUntil(() => loadItemsDone[0]);
    }

    /**
     * this function should be called once we're sure that the backpack related data is all loaded
     * 
     * assumes itemLoader is ready, and that itemList/Count is ready (the latter achieved via loadUserData or something
     */
    public void loadInItems(bool[] done)
    {
        int nc = (fullScreenMode) ? 5 : 4; //num of items in one row; can fit 4 for width of 420, 5 for width of 520

        if (numItems > nc*2) numGrids = nc * (int)Mathf.Ceil(numItems / nc);

        itemBG.SetActive(true);
        itemMold.SetActive(true);

        GameObject items = itemBG.transform.parent.gameObject; RectTransform itmRT = items.GetComponent<RectTransform>();


            Global.setRectShape(items, itmRT.rect.width,
                (numGrids / nc) * 100 + 20); //so that items' rect always fits perfectly the grids that are created

        itmRT.pivot = new Vector2(0, 1); //center properly

        //first create needed num of itemBG 
        for (int n = 0; n < numGrids; n++)
        {
            GameObject currItemBG = Instantiate(itemBG, itemBG.transform.parent);
            int row = n / nc; int col = n % nc;
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

        done[0] = true;
    }

    public void openBackpackUI(bool fullScreen)
    {
        fullScreenMode = fullScreen; //TODO what

        if (backpackBtnActive)
        {
            if(gameControl != null) {
            gameControl.ckTouch = false; //to stop checking on game progress
            gameControl.pauseGame();
            }

            if (backpack != null)
            {
                backpack.SetActive(true);
                Global.changePos(backpack, 0, 0);
            }
        }
    }

    public void closeBackpackUI()
    {
        if (gameControl)
        {
            gameControl.ckTouch = true; //to start checking on game progress
            gameControl.resumeGame();

            if (gameControl.player) //if player exists --> which could be false if in title scene backpack view
            {
                Transform aimy = gameControl.player.transform.Find("Aim(Clone)");
                if (aimy != null)
                {
                    Destroy(aimy.gameObject);
                }
            }
        }

        Time.timeScale = 1.0f; //resume gameplay



        //might eventually play an animation (UI screen folding or something) here before setting to inactive

        if (backpack != null)
        {
            Global.changePos(backpack, (int)startPos.x, (int)startPos.y);
            backpack.SetActive(false);
            dialogue.gameObject.SetActive(true);
        }
    }

    void loadItemMold()
    {
        itemMold = Resources.Load("Prefabs/ItemMold") as GameObject;
        if (itemMold == null)
        {
            Debug.LogError("load ItemMold failed");
        }
    }


}
