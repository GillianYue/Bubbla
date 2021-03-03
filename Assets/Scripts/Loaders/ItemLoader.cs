using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoader : MonoBehaviour
{
    private bool[] loadDone; //when loadDone[0] == true, loading is done for the csv file
    public TextAsset itemCsv;
    private string[,] data; //double array that stores all info 

    int[] itemCode; 
    public string[] itemName;
    public string[] itemDescriptions;
    public float[] sizeScale, colliderScale;
    public string[] s0_anim, s1_anim, s2_anim; //path to animations
    public AnimationClip[] S0_ANIM, S1_ANIM, S2_ANIM;
    public Sprite[] S0_SPRITE; //for inanimate items, sprite instead of anim is used
    public int[] itemType; //, movement; //way of moving TODO

    GameObject inGameItemMold, itemMold;

    public bool itemLoaderDone; //this will be set to true once EnemyLoader is ready for usage


    void Start()
    {
        loadDone = new bool[1];

        loadItemMold(); //ready the mold prefab(s)
        StartCoroutine(LoadScene.processCSV(loadDone, itemCsv, setData, false)); //processCSV will call setData
        StartCoroutine(parseItemData()); //data will be parsed into local type arrays for speedy data retrieval

    }

    // Update is called once per frame
    void Update()
    {

    }

    /** this function should only be called by ItemSpawner, as it deals with base level data
     * 
     *  returns instance of instantiated item based on what type it is    
     */
    public GameObject getItemInstance(int eCode)
    {
        GameObject i = null;

        switch (itemType[eCode])
        {
            case 0: //inanimate
                if (itemMold == null)
                {
                    Debug.LogError("itemMold is null... Can't create item instance");
                    return null;
                }
                i = Instantiate(itemMold) as GameObject; //duplicate

                SpriteRenderer sr = i.GetComponent<SpriteRenderer>();
                sr.sprite = S0_SPRITE[eCode];

                ItemBehav item = i.GetComponent<ItemBehav>();
                item.itemName = itemName[eCode];
                item.description = itemDescriptions[eCode];
                item.setSizeScale(sizeScale[eCode]); //what we need is the setting of sizeScale variable here
                item.setType(itemType[eCode]);
                //TODO
                
                break;
            case 1: //items that have an animator & will use animations
                if (inGameItemMold == null)
                {
                    Debug.LogError("(in-game)itemMold is null... Can't create item instance");
                    return null;
                }

                i = Instantiate(inGameItemMold) as GameObject; //duplicate
                Animator animator = i.GetComponent<Animator>();

                AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

                if (S0_ANIM[eCode] != null)
                    animatorOverrideController["State0anim"] = S0_ANIM[eCode];

                if (S1_ANIM[eCode] != null)
                    animatorOverrideController["State1anim"] = S1_ANIM[eCode];

                if (S2_ANIM[eCode] != null)
                    animatorOverrideController["State2anim"] = S2_ANIM[eCode];

                animator.runtimeAnimatorController = animatorOverrideController;
                animator.Update(0.0f);

                ItemBehav itemG = i.GetComponent<ItemBehav>();
                itemG.itemName = itemName[eCode];
                itemG.setSizeScale(sizeScale[eCode]);
                itemG.setColliderScale(colliderScale[eCode]);
                itemG.setType(itemType[eCode]);
                //TODO movement

                break;
            default:
                Debug.LogError("can't find itemType: " + itemType[eCode]);
                break;
        }




        return i;
    }

    void loadItemMold()
    {
        inGameItemMold = Resources.Load("IngameItemMold") as GameObject;
        if (inGameItemMold == null) Debug.LogError("load in-game-ItemMold failed");

        itemMold = Resources.Load("Prefabs/ItemMold") as GameObject;
        if (itemMold == null) Debug.LogError("load ItemMold failed");
    }

    IEnumerator parseItemData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed


        int numRows = data.GetLength(1);
       itemCode = new int[numRows - 1]; //num rows, int[] is for the entire column
       itemName = new string[numRows - 1];
        itemDescriptions = new string[numRows - 1];
        sizeScale = new float[numRows - 1]; colliderScale = new float[numRows - 1];
        s0_anim = new string[numRows - 1]; s1_anim = new string[numRows - 1]; s2_anim = new string[numRows - 1];
        S0_ANIM = new AnimationClip[numRows - 1]; S1_ANIM = new AnimationClip[numRows - 1]; S2_ANIM = new AnimationClip[numRows - 1];
        S0_SPRITE = new Sprite[numRows - 1];
        itemType = new int[numRows - 1];  //movement = new int[numRows - 1];

        //skip row 0 because those are all descriptors
        for (int r = 1; r < numRows; r++) //-1 because title row doesn't count
        {
         
            itemName[r - 1] = data[1, r];
            itemDescriptions[r - 1] = data[2, r];
            float.TryParse(data[3, r], out sizeScale[r - 1]);
            float.TryParse(data[4, r], out colliderScale[r - 1]);
            s0_anim[r - 1] = data[5, r];
            s1_anim[r - 1] = data[6, r];
            s2_anim[r - 1] = data[7, r];
            int.TryParse(data[8, r], out itemType[r - 1]);
            
        }

        loadAnimationClips(S0_ANIM, S1_ANIM, S2_ANIM, S0_SPRITE);
        yield return new WaitUntil(() => {
            return (S0_ANIM[S0_ANIM.Length - 1] != null || S0_SPRITE[S0_SPRITE.Length - 1] != null);
        }); //anim loaded, theoretically everything all set

        itemLoaderDone = true;
        Debug.Log("ItemLoader ready");
    }

    public void setData(string[,] d)
    {
        data = d;
    }

    private void loadAnimationClips(AnimationClip[] s0, AnimationClip[] s1, AnimationClip[] s2, Sprite[] spr0)
    {

        //load the animation clips into the arrays
        for (int iCode = 0; iCode < s0.GetLength(0); iCode++)
        {
            if (itemType[iCode] != 0)
            {
                if (!s0_anim[iCode].Equals("")) //not inanimate
                {
                    var tmpAnim0 = Resources.Load("Animation/" + s0_anim[iCode]) as AnimationClip;

                    if (tmpAnim0 == null)
                    {
                        Debug.LogError("Animation 0 NOT found");
                    }
                    else
                    {
                        s0[iCode] = tmpAnim0;
                    }
                }


                if (!s1_anim[iCode].Equals(""))
                {
                    var tmpAnim1 = Resources.Load("Animation/" + s1_anim[iCode]) as AnimationClip;

                    if (tmpAnim1 == null)
                    {
                        Debug.LogError("Animation 1 NOT found");
                    }
                    else
                    {
                        s1[iCode] = tmpAnim1;
                    }
                }

                if (!s2_anim[iCode].Equals(""))
                {
                    var tmpAnim2 = Resources.Load("Animation/" + s2_anim[iCode]) as AnimationClip;

                    if (tmpAnim2 == null)
                    {
                        Debug.LogError("Animation 2 NOT found");
                    }
                    else
                    {
                        s2[iCode] = tmpAnim2;
                    }
                }
            }
            else
            {
                Sprite tmpSpr0 = Resources.Load<Sprite>("Sprites/" + s0_anim[iCode]);
                if (tmpSpr0 == null)
                {
                    Debug.LogError("Sprite 0 NOT found: "+ s0_anim[iCode]);
                }
                else
                {
                    spr0[iCode] = tmpSpr0;
                }
            }
        }
    }

}