using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {
	//screen: 320x480
	//world to screen factor: 1:35.6
	public GameObject paintBall;
	public Camera mainCamera;
	public Vector3 spawnValues;
	public GameObject Ballz;
	public float startWait, pbSpawnWait, spawnRangeWidth;
	public double WTSfactor = 35.6;

	public GameObject player;

	// Use this for initialization
	void Start () {
		StartCoroutine (SpawnPaintballs ());
		 
	}
	
	// Update is called once per frame
	void Update () {

		//mouseclick
		if (Input.GetMouseButtonDown (0)) {

			foreach (Transform child in Ballz.transform) {//
				Vector3 screen = mainCamera.WorldToScreenPoint
					(child.transform.position);
				//checks if clicking on any paintball
				if (Global.touching (new Vector2 (Input.mousePosition.x,
					Input.mousePosition.y),
					new Vector2 (screen.x, screen.y),
					child.GetComponentInParent<PaintballSpawner>
					().getScale() * WTSfactor)) {
					/**if yes, terminate the method so that
					bullet wouldn't be launched; the interaction with
					paintball will be handled by PaintballSpawner
					**/
					return;
				}

			}//

			//if code reaches here, it shows that empty space was clicked
			player.GetComponent<Player>().launchBullet(Input.mousePosition);
		}
	}

	IEnumerator SpawnPaintballs(){
		yield return new WaitForSeconds (startWait);
		while (true) {
			
			GameObject pb = paintBall;

			Vector3 spawnPosition = new Vector3 (
				Random.Range(spawnValues.x-spawnRangeWidth,
					spawnValues.x + spawnRangeWidth),
				spawnValues.y, spawnValues.z);
			pb = Instantiate (pb, spawnPosition, Quaternion.identity)
				as GameObject;
			pb.transform.parent = Ballz.transform;
			yield return new WaitForSeconds (pbSpawnWait);
		}
	}


}
