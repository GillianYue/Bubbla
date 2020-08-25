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
            Global.resizeSpriteToRectX(background, transform.parent.gameObject); //parent is legit Canvas
            Global.zeroX(background);
        }

        bool first = true;
            foreach (RectTransform rt in scrollTransforms) {
            GameObject background = rt.gameObject;

            Global.resizeSpriteToRectX(background, transform.parent.gameObject); //parent is legit Canvas
            Global.zeroX(background);

            if (first)
            {
                //setting limits to bg sprite scroll
                bottomY = -rt.rect.height;
                topY = -bottomY;
            }

            //initialize background starting position
            rt.anchoredPosition = new Vector3(0f, first? 0 : topY, rt.localPosition.z);
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
