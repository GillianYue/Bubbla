﻿using UnityEngine;
using System.Collections;

public class BGMover : MonoBehaviour {

	public float topY, bottomY, startY;
	//starting Z coordinate and ending Z coordinate
	public float scrollSpd;
	private bool scrollin;

	void Start(){
        Global.resizeSpriteToRectX(gameObject);
        Global.centerX(gameObject);
        GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, startY,
    GetComponent<RectTransform>().localPosition.z);
    }

	public void StartScrolling () {
		scrollin = true;
	}


	void Update () {
        Vector3 p = GetComponent<RectTransform>().localPosition;

        if (p.y <= bottomY) {
            //if it goes beyond the lower threshold
            GetComponent<RectTransform>().anchoredPosition = new Vector3(0, topY, p.z);
			//reset
		}

		if (scrollin) {
            GetComponent<RectTransform>().anchoredPosition3D -= new Vector3 (0, scrollSpd, 0);
			//negative is UP, so

		}
	}

	public void stopBGScroll(){
		scrollin = false;
	}

	public void resumeBGScroll(){
		scrollin = true;
	}
}
