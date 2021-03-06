﻿using UnityEngine;
using System.Collections;

/*
 * this script is attached to the BG GOs in scene, which have backgrounds (each to its own) as children
 * 
 * BG GOs have transforms as opposed to RectTransforms
 */
public class BGMover : MonoBehaviour {

	public float topY, bottomY;
	//starting Z coordinate and ending Z coordinate
	public float scrollSpd;
	private bool scrollin;
    public GameObject[] backgrounds; //will be resized; only assign static (non scrollable)
    public RectTransform[] scrollTransforms; //need not be in backgrounds; for now, can only rotate between 2
    private RectTransform[] scrollSequence; //things that will actually be scrolled; set to scrollTransforms on start;
    //also holds only 2 instances; whatever is not "inView (see function)" will not be visible 
    public RectTransform[] lingerSpots; //e.g. boss fight scene backgrounds, investigation spots, etc.

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;

    private int scrollCount; //keeps track of how many bg scroll switches have taken place; 
    //this value will be automatically updated to Global.intVariables as "scrollCount"; TODO not done yet
    RectTransform scrollToggle; //toggled every time bg scroll takes place; is set to rect that is recently shifted up to out of screen

    void Start(){


         //   Global.intVariables.Add("scrollCount", 0);

        foreach (GameObject background in backgrounds)
        {
            SpriteRenderer scr = background.transform.GetChild(0).GetComponent<SpriteRenderer>();

            Global.zeroX(background);

            //setting child local offset to change pivot of the GO
            scr.transform.localPosition = new Vector3(0, scr.sprite.rect.height / 2, 0);
            scr.transform.localScale = Vector3.one;
        }

        scrollSequence = scrollTransforms;

        StartCoroutine(setupScrollBGs(scrollTransforms, true));
        if(lingerSpots != null) StartCoroutine(setupScrollBGs(lingerSpots, false));
    }

    //will need canvas resize on start to be done for this to properly take place
    IEnumerator setupScrollBGs(RectTransform[] rts, bool setToActive)
    {
        bool first = true;
        foreach (RectTransform rt in rts)
        {
            GameObject background = rt.gameObject;
            //sprites stored in child GO of transform GO because of a need for local offset of the sprite;
            //child GO's scale will be kept to 1 (sprite's original size) its sole purpose is to maintain the offset
            //child GO does not have a rect transform but only the normal transform to avoid confusion
            SpriteRenderer scr = rt.GetChild(0).GetComponent<SpriteRenderer>();

            //set the correct local offset so that rt's "0" will be at the bottom of the sprite
            scr.transform.localPosition = new Vector3(0, scr.sprite.rect.height / 2, 0);
            scr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            scr.transform.localScale = Vector3.one;

            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 0);
            rt.pivot = new Vector2(0.5f, 0);

            ResizeOnStart resize = rt.GetComponent<ResizeOnStart>();

            yield return new WaitUntil(() => resize.checkResizeDone());

            if (first)
            {
                //setting limits to bg sprite scroll
                bottomY = (-rt.localScale.y * scr.sprite.bounds.size.y);
                topY = -bottomY;

            }

            //initialize background starting position
            rt.anchoredPosition = new Vector3(0f, first ? 0 : topY, rt.localPosition.z);
            first = false;

            if (setToActive) rt.gameObject.SetActive(true); else rt.gameObject.SetActive(false);
        }
    }

	public void StartScrolling () {
		scrollin = true;
	}


	void Update () {

		if (scrollin) {
            foreach (RectTransform rt in scrollSequence)
            {
                Vector3 p = rt.anchoredPosition;


                if (p.y <= bottomY)
                {
                    scrollToggle = rt;

                    //if it goes beyond the lower threshold
                    rt.anchoredPosition += new Vector2(0, topY - bottomY);
                    //reset

                }

                rt.localPosition -= new Vector3(0, scrollSpd * Time.deltaTime * 100, 0);

            }

        }
	}

	public void stopBGScroll(){
		scrollin = false;
	}

    public void revertToStartingPos() //resets all transforms to their starting positions
    {
        bool first = true;
        foreach (RectTransform rt in scrollTransforms)
        {
            //initialize background starting position
            rt.anchoredPosition = new Vector3(0f, first ? 0 : topY, rt.localPosition.z);
            first = false;
        }
    }

    //checks for whether the player is seeing this target through position
    public bool isBGInView(RectTransform target)
    {

        return (target.anchoredPosition.y >= -getBackgroundHeight(target) && target.anchoredPosition.y <= Screen.height);
    }

    //customEvent #32
    public void swapScroll(string boolName, int index)
    {
        Global.boolVariables.Add(boolName, false);

            StartCoroutine(waitForScrollFinish(boolName, index));
    }

    /// <summary>
    /// wait until one of the two scroll bg is not in view, does the swap, then wait until the (swapped) linger spot bg pic reaches top and sets global bool to true
    /// 
    /// when this happens, we know it's time to stop all scroll activities 
    /// </summary>
    /// <returns></returns>
    private IEnumerator waitForScrollFinish(string boolName, int index)
    {
        //wait for condition to swap; loop until condition met

        int target = -1; 

        while (target == -1)
        {
            for (int i = 0; i < scrollSequence.Length; i++)
            {
                if (!isBGInView(scrollSequence[i])) //bg out of screen (above) found
                {
                    target = i;
                    break;
                } //finding the out-of-screen instance, since there's only two 
            }

            if (target == -1) yield return new WaitForSeconds(0.5f);

            else
            {
                lingerSpots[index].transform.localPosition = scrollSequence[target].localPosition;
                lingerSpots[index].gameObject.SetActive(true);
                scrollSequence[target].gameObject.SetActive(false);
                scrollSequence[target] = lingerSpots[index]; //the swap when found

                //GameTestBehavior.pauseGame();

            }
        }


        //wait until reach/settle
        RectTransform spot = scrollSequence[target];

        float spriteHeight = getBackgroundHeight(spot);

        float reachPos = Screen.height - spriteHeight;
        Debug.Log("sprite height calced is " + spriteHeight + " and reachPos is " + reachPos);

        yield return new WaitUntil(() => (spot.anchoredPosition.y <= reachPos));
        Debug.Log("now reached: " + spot.anchoredPosition.y);
        scrollin = false;
        Global.boolVariables[boolName] = true;
    }

    //specific to bg GO
    private float getBackgroundHeight(RectTransform spot)
    {
        return spot.GetChild(0).GetComponent<SpriteRenderer>().sprite.rect.height * spot.transform.lossyScale.y;
    }
}
