using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {
	//screen: 320x480
	//world to screen factor: 1:35.6
	public GameObject paintBall;
	public Camera mainCamera;
	public Vector3 spawnValues;
	public GameObject Ballz, Hs_Holder;
	public float startWait, pbSpawnWait, spawnRangeWidth;
	public double WTSfactor;
	private int scWidth, scHeight;
	public GameObject[] hearts;
	public GameObject HeartVFX;

	public GameObject player;
	public GameObject GameOverC;
	public EnemySpawner eSpawner;

	// Use this for initialization
	void Start () {
		scWidth = Screen.width;
		scHeight = Screen.height;
		GameOverC.SetActive (false);

		StartCoroutine (SpawnPaintballs ());


		Vector3 zero = mainCamera.WorldToScreenPoint (new Vector3 (0, 
			player.transform.position.y, player.transform.position.z));
		Vector3 one = mainCamera.WorldToScreenPoint (new Vector3 (1, 
			player.transform.position.y, player.transform.position.z));
		WTSfactor = (one.x - zero.x);

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
					
					//checking if bulletgauge is full(the process takes in paint automatically)
					if (player.GetComponent<Player> ().addPaint 
						(child.GetComponentInParent<PaintballSpawner>().getColor())) {
						//generates effect
						child.GetComponentInParent<PaintballSpawner>().getsAbsorbed();
					}
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
			pb = Instantiate (pb, spawnPosition, pb.transform.rotation)
				as GameObject;
			pb.transform.Rotate (new Vector3(0,0,Random.Range(0, 360)));
			pb.transform.SetParent (Ballz.transform);
			yield return new WaitForSeconds (pbSpawnWait);
		}
	}

	public void gameOver(){
		GameOverC.SetActive (true);
		Time.timeScale = 0;
	}

	public void restart(){
		player.GetComponent<Player> ().respawn ();
		Time.timeScale = 1;
		SceneManager.LoadScene (0);
	}

	public void updateLife(int life){
		genHearts (life);
		updateBGM (life);
	}


	public void genHearts(int life){
		
		int dif = life - Hs_Holder.transform.childCount;

			
			if (dif > 0) {//needs to increase hearts sprites
			for (int c = 0; c < dif; c++) {
				Vector3 pos = Hs_Holder.transform.position + 
					new Vector3 (Random.Range (0.2f, 1.0f), 0, 1.0f);
				GameObject tmpH = Instantiate (hearts [(int)(Random.Range (0.0f, 3.99f))], 
					                 pos, hearts [0].transform.rotation) as GameObject;
				tmpH.transform.parent = Hs_Holder.transform;
			}
			} else if (dif < 0) {//needs to destroy hearts sprites
			for (int c = 0; c > dif; c--) {
				Instantiate 
				(HeartVFX, Hs_Holder.transform.GetChild (0).position, 
					Hs_Holder.transform.GetChild (0).rotation);
				Destroy (Hs_Holder.transform.GetChild (0).gameObject);
			}
		}
				
			}


	public void updateBGM(int life){
		if(life > player.GetComponent<Player>().getMaxLife()/3*2){
			GetComponent<AudioSource> ().pitch = 1;
		}else if(life > player.GetComponent<Player>().getMaxLife()/3){
			GetComponent<AudioSource> ().pitch = 0.9f;
		}else{
			GetComponent<AudioSource> ().pitch = 0.8f;
		}
		//special status: 1.2f!
	}

}