using UnityEngine;
using System.Collections;

/*
 * this script is attached to the BG GOs in scene, which have backgrounds (each to its own) as children
 * 
 * BG GOs have transforms as opposed to RectTransforms
 */
public class BGMover : MonoBehaviour {

	public float topY, bottomY, startY;
	//starting Z coordinate and ending Z coordinate
	public float scrollSpd;
	private bool scrollin;
    private GameObject background;

	void Start(){

        background = transform.GetChild(0).gameObject;
        RectTransform bgRT = background.GetComponent<RectTransform>();

        //setting dimensions for child, background (the actual holder of sprite)
        Global.resizeSpriteToRectX(background, transform.parent.gameObject); //parent is legit Canvas
        Global.zeroX(background);

        float magicalNumber = background.GetComponent<SpriteRenderer>().sprite.rect.height * bgRT.localScale.y - Global.MainCanvasHeight;
        //setting limits to bg sprite scroll
        bottomY = -(magicalNumber);

        //initialize background starting position
        bgRT.anchoredPosition = new Vector3(0f, magicalNumber/2, bgRT.localPosition.z);
    }

	public void StartScrolling () {
		scrollin = true;
	}


	void Update () {

		if (scrollin) {
            Vector3 p = transform.localPosition;

            if (p.y <= bottomY)
            {
                //if it goes beyond the lower threshold
                transform.localPosition = new Vector3(0, topY, p.z);
                //reset
            }

            transform.localPosition -= new Vector3 (0, scrollSpd * Time.deltaTime * 100, 0);
			//negative is UP, so

		}
	}

	public void stopBGScroll(){
		scrollin = false;
	}

	public void resumeBGScroll(){
		scrollin = true;
	}

    public void jumpToPos(float pos)
    {
        transform.localPosition = new Vector3(0, pos, transform.localPosition.z);
    }

    public void revertToStartingPos()
    {
        transform.localPosition = new Vector3(0, 0, transform.localPosition.z);
    }
}
