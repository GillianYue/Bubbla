using UnityEngine;
using System.Collections;

public class BGMover : MonoBehaviour {

	public float topZ, bottomZ, startZ;
	//starting Z coordinate and ending Z coordinate
	public float scrollSpd;
	private bool scrollin;


	void Start () {
		gameObject.transform.position = new Vector3(0f, 0f, startZ);
		scrollin = true;
	}


	void Update () {
		if (gameObject.transform.position.z <= bottomZ) {
			//if it goes beyond the lower threshold
			gameObject.transform.position = new Vector3(0, 0, topZ);
			//reset
		}

		if (scrollin) {
			gameObject.transform.position -= new Vector3 (0, 0, scrollSpd);
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
