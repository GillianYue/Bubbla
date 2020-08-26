using UnityEngine;
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
    public GameObject[] backgrounds; //will be resized; only assign static 
    public RectTransform[] scrollTransforms; //need not be in backgrounds; for now, can only rotate between 2

	void Start(){
        foreach (GameObject background in backgrounds)
        {
            SpriteRenderer scr = background.transform.GetChild(0).GetComponent<SpriteRenderer>();

            Global.zeroX(background);

            //setting child local offset to change pivot of the GO
            scr.transform.localPosition = new Vector3(0, scr.sprite.rect.height / 2, 0);
            scr.transform.localScale = Vector3.one;
        }

        StartCoroutine(setupScrollBGs());
    }

    IEnumerator setupScrollBGs()
    {
        bool first = true;
        foreach (RectTransform rt in scrollTransforms)
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
                bottomY = -rt.localScale.y * scr.sprite.bounds.size.y;
                topY = -bottomY;

            }

            //initialize background starting position
            rt.anchoredPosition = new Vector3(0f, first ? 0 : topY, rt.localPosition.z);
            first = false;
        }
    }

	public void StartScrolling () {
		scrollin = true;
	}


	void Update () {

		if (scrollin) {
            foreach (RectTransform rt in scrollTransforms)
            {
                Vector3 p = rt.localPosition;
                Debug.Log("p: " + p);

                if (p.y <= bottomY)
                {
                    //if it goes beyond the lower threshold
                    rt.localPosition = new Vector3(0, topY, p.z);
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
}
