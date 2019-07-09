using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


/**
 * GameControl is a global level game control script. It is in charge of starting the game
 * and switching between different stages of the game. It talks with GameFlow, which is a
 * game logic script on a more micro level, to run each level based on inputs. 
 * 
 * GameControl does not have access to the CSV scripts. It only has access to some of the
 * more permanent elements, like player, and the game play UI. It has access to an 
 * EnemySpawner script, which is in charge of spawning the enemies. 
 * 
 * User input (touch screen) is also checked here in Update. Whenever there is input, the
 * GameControl global script sends the information to sub logic scripts correspondingly. 
 * It then acts on a macro level according to the feedback of those sub scripts.
 * For example, mode of the game is being determined in this way:
 * Mode transitions are written in csv files available in GameFlow;
 * Each line is performed out using a function in GameFlow (which is called here globally);
 * GameFlow has a pointer to know which line it's supposed to perform;
 * GameControl notifies GameFlow when it's time to update the pointer (user clicks screen);
 * when Mode changes (done in processLine in gameflow) GameControl acts accordingly globally;
 * if in DLG it only checks for clicks;
 * if in GAME it activates UI, wakes EnemySpawner, checks paint collection, etc.
 */
public class GameControl : MonoBehaviour {
	//screen: 320x480
	//world to screen factor: 1:35.6
	public GameObject paintBall, potion;
	public Camera mainCamera;
	public Vector3 spawnValues;
	public GameObject Ballz, Hs_Holder;
	public float startWait, pbSpawnWait, spawnRangeWidth;
	public double WTSfactor;
	public GameObject[] hearts, gadgets; //gadgets being hearts_container, bulletGauge, etc.
	public GameObject HeartVFX, aim;

	public GameObject player;
	public GameObject GameOverC;
	public EnemySpawner eSpawner;
	public GameFlow gFlow;
	public BGMover[] backgrounds;
	public ScriptableObject levelScript;

    public GameObject fixedBG;

	private float pressTime=-1;

	// Use this for initialization
	void Start () {
		
		GameOverC.SetActive (false);
		player.GetComponent<Player> ().enabled = false;
        //gadgets are GOs like life container that are needed in game play but not in DLG mode
		foreach (GameObject g in gadgets) {
			g.SetActive (false);
		}

		StartCoroutine (StartGame());

		Vector3 zero = mainCamera.WorldToScreenPoint (new Vector3 (0, 
			player.transform.position.y, player.transform.position.z));
		Vector3 one = mainCamera.WorldToScreenPoint (new Vector3 (1, 
			player.transform.position.y, player.transform.position.z));
		WTSfactor = (one.x - zero.x);

        //correct sizing for the backgrounds of this level, fixed or non-fixed
        Global.resizeSpriteToRectX(fixedBG);
        foreach (BGMover m in backgrounds)
        {
            Global.resizeSpriteToRectX(m.gameObject);
            Global.centerX(m.gameObject);
        }

    }

	// Update is called once per frame
	void Update () {

		switch(gFlow.currMode){
		case GameFlow.Mode.DLG:
		//mouseclick
			if (Input.GetMouseButtonDown (0)) {

					if (gFlow.checkCurrentLineDone ()) {//time to move pointer and print new line
						StartCoroutine (gFlow.movePointer ()); //move to next line in script
					} else if (gFlow.Skippable ()) { //skip time
						gFlow.skipDLG ();
					}
					return; //if so, no need to proceed
			}
			break;

		case GameFlow.Mode.GAME:
			if (Input.GetMouseButtonDown (0)) {
				foreach (Transform child in Ballz.transform) {//
					Vector3 screen = mainCamera.WorldToScreenPoint
					(child.transform.position); //TODO debug? is paint transform based

					//checks if clicking on any paintball
					if (child.CompareTag ("Paintball") &&
					   Global.touching (new Vector2 (Input.mousePosition.x,
						   Input.mousePosition.y),
						   new Vector2 (screen.x, screen.y),
						   child.GetComponentInParent<PaintballBehavior>
						().getScale () * WTSfactor)) {
						/**if yes, terminate the method early so that
					bullet wouldn't be launched; the interaction with
					paintball will be handled by PaintballSpawner
					**/

						//checking if bulletgauge is full(the process takes in paint automatically)
						if (player.GetComponent<Player> ().addPaint 
						(child.GetComponentInParent<PaintballBehavior> ().getColor ())) {
							//generates effect
							child.GetComponentInParent<PaintballBehavior> ().getsAbsorbed ();
						} 
						return;
					} else if (child.CompareTag ("Potion") &&
					          Global.touching (new Vector2 (Input.mousePosition.x,
						          Input.mousePosition.y),
						          new Vector2 (screen.x, screen.y),
						          child.GetComponentInParent<PotionBehav>
						().getScale () * WTSfactor)) {

						child.GetComponent<PotionBehav> ().getsAbsorbed ();
						player.GetComponent<Player> ().cure 
					(child.GetComponent<PotionBehav> ().getCuringPotency ());

						return;
					}
				}//end check paintballs

				//if code reaches here, treat as starting to press down, as opposed to a light tap on
				//paintball/potion
				GameObject a = aim; //GO with the aiming sprite
				a = Instantiate (a, new Vector3 (
					Global.PixelToWorldFactor.x * Input.mousePosition.x,
					player.transform.position.y,
					Global.PixelToWorldFactor.y * Input.mousePosition.y),
					player.transform.rotation) as GameObject;
				a.transform.SetParent (Ballz.transform);
				a.GetComponent<Animator> ().SetBool ("Focused", false);

				pressTime = Time.time;
			}

			if (pressTime != -1) { //if is currently pressing
				Vector2 XZ = ConvertToWorldUnits (Input.mousePosition);

				Transform aimy = Ballz.transform.Find ("Aim(Clone)");
				aimy.position = new Vector3 (
					XZ.x,	player.transform.position.y, XZ.y); //put aim at pressed position

				if ((Time.time - pressTime) > 0.7 &&
				   !aimy.GetComponent<Animator> ().GetBool ("Focused")) {
					aimy.GetComponent<Animator> ().SetBool ("Focused", true);
				}

				//start pointin cannon
				Vector3 mouse = Input.mousePosition;
				Vector3 direction = mouse -
				                   Camera.main.WorldToScreenPoint (player.transform.position);
				float tan = direction.x / direction.y;
				float angle = Mathf.Atan (tan); 
				//*************THE ANGLE IS HERE*************

				float dgAngle = -1 * (angle * Mathf.Rad2Deg); //convert from radian to dgr
				dgAngle += 45; //to account for the original angle of cannon the sprite
				if (direction.y < 0) {
					dgAngle += 180;
				}
				if (direction.x < 0) {
					player.GetComponent<Animator> ().SetBool ("left", true);
				} else {
					player.GetComponent<Animator> ().SetBool ("left", false);
				}
				//even if no bullet, cannon points to the clicked direction
				Vector3 temp = player.transform.Find ("Cannon").localEulerAngles;
				temp.z = dgAngle;
				player.transform.Find ("Cannon").localEulerAngles = temp;
				//end pointin cannon
				player.GetComponent<Animator> ().SetBool ("aiming", true);


				if (Input.GetMouseButtonUp (0)) { //release

					Destroy (Ballz.transform.Find ("Aim(Clone)").gameObject);
				
					float pressedLength = (Time.time - pressTime);
					pressTime = -1; //set to -1 so that we know it's not pressing

					//if code reaches here, it shows that empty space was clicked, SHOOT BULLET

					if (pressedLength < 0.7) {
						//normal shooting

						//the launchBullet function will check if there's bullet
						player.GetComponent<Player> ().launchBullet (direction, angle);


					} else if (pressedLength < 1.5) {
						//second stage
						player.GetComponent<Player> ().launch2Bullets (direction, angle);

					} else {
						//triple combo shoot
					}
					
				}//end release
			} else {//end pressing
				player.GetComponent<Animator> ().SetBool ("aiming", false);
			}
			break;

		default:
			break;

		}//end switch

	}

	IEnumerator StartGame(){ 
		while (!gFlow.checkLoadDone()) {//wait till csv's loaded
			yield return null;
		}

		int tempPT = -1; //temp pointer, is passive, updates as gFlow pointer updates
		do{//once code gets here, should be ready to start gameFlow
			if(tempPT != gFlow.getPointer()){ //avoid redundant work; only rerender if changed
				StartCoroutine(gFlow.processCurrentLine());
			tempPT = gFlow.getPointer(); 
			}
			yield return new WaitForSeconds(0.2f); //essentially check dialogue status every one s
		}while(!gFlow.checkIfEnded()); //as long as there's still something to be done
			
	}
		
	public void startEnemyWaves(int[] w, int[] e){
		StartCoroutine (SpawnPaintballs ()); 
		eSpawner.StartSpawn(w, e);
		foreach(BGMover m in backgrounds){
			m.StartScrolling ();
		}
		player.GetComponent<Player> ().enabled = true; //generation of heart, updates, etc. 
		foreach (GameObject g in gadgets) {
			g.SetActive (true); //make bullet gauge and life container show
		}
	}

    //TODO move this to an actual paintball spawner script
	IEnumerator SpawnPaintballs(){
		yield return new WaitForSeconds (startWait);
		while (true) {
			if(Random.Range(0, 99.99f)>5){
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
			
			}else{//5% chance to generate potion
				GameObject pt = potion;

				Vector3 spawnPosition = new Vector3 (
					Random.Range(spawnValues.x-spawnRangeWidth,
						spawnValues.x + spawnRangeWidth),
					spawnValues.y, spawnValues.z);
				pt = Instantiate (pt, spawnPosition, pt.transform.rotation)
					as GameObject;
				pt.transform.SetParent (Ballz.transform);
			}
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
	}

    /*
     * checks for existing hearts vs. life (num hearts there should be) and gens as needed
     */
	public void genHearts(int life){
		
		int dif = life - Hs_Holder.transform.childCount;

			
			if (dif > 0) {//needs to increase hearts sprites
			for (int c = 0; c < dif; c++) {
				Vector3 pos = Hs_Holder.transform.position + 
					new Vector3 (Random.Range (0.2f, 1.0f), 0, 1.0f);
                //picks a random heart prefab out of the hearts prefab group
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


		

	public Vector2 ConvertToWorldUnits(Vector2 TouchLocation)
	{
		Vector2 returnVec2;

		returnVec2.x = ((TouchLocation.x * Global.PixelToWorldFactor.x) - 
			(Global.WorldUnitsInCamera.x / 2)) + mainCamera.transform.position.x;
		returnVec2.y = ((TouchLocation.y * Global.PixelToWorldFactor.y) - 
			(Global.WorldUnitsInCamera.y / 2)) + mainCamera.transform.position.z;

		return returnVec2;
	}

}