using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	
	public float startWait, enemySpawnWait, waveSpawnWait, range;
	//range is worldspace width on screen/2 (+ and then -)
	public GameObject spike, enemiz, bird;
	public Vector3 spawnValues;


	/*
		Types of waves: ––, /, \, ^, T, U, A, X, *, #, S
	*/
	// Use this for initialization
	void Start () {
		int[] t = new int[6]{0, 1, 2, 3, 0, 1};
		StartCoroutine (SpawnEnemyLevel (t));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator SpawnEnemyLevel(int[] types){
		yield return new WaitForSeconds (startWait);
		//start wait time

			foreach (int type in types) {
			int curr = type;
			switch(curr){
			case 0:
				SpawnHorizontal (3);
				break;
			case 1:
				yield return StartCoroutine (SpawnSlash(true, 3));
				break;
			case 2:
				yield return StartCoroutine (SpawnSlash (false, 3));
				break;
			case 3:
				yield return StartCoroutine (SpawnCaret (5));
				break;

			default:
				break;
				
			}

				yield return new WaitForSeconds (waveSpawnWait);
			}
			//wait time between each wave

	}


	//SPAWN WAVES

	void SpawnHorizontal(int numEnemy){
		
		for (int c = 0; c < numEnemy; c++) {
			
		genSpike((-range + c*(2*range/(float)(numEnemy-1))), spawnValues.y, spawnValues.z);

		}
	}

	IEnumerator SpawnSlash(bool ForB, int numEnemy){
		for (int c = 0; c < numEnemy; c++) {
				
			genBird (((ForB) ? -1 : 1) * (range - c * (2 * range / (float)(numEnemy - 1))), 
				spawnValues.y, spawnValues.z);

			yield return new WaitForSeconds (enemySpawnWait+0.5f);
		}
	}

	IEnumerator SpawnCaret(int numEnemy){
		//numEnemy has to be odd for this shaped formation
		for (int c = 0; c < (numEnemy/2); c++) {
			genSpike (-range + c * (2*range/(float)(numEnemy-1)), spawnValues.y, spawnValues.z);
			genSpike (range - c * (2*range/(float)(numEnemy-1)), spawnValues.y, spawnValues.z);

			yield return new WaitForSeconds (enemySpawnWait);
		}

		genSpike (0, spawnValues.y, spawnValues.z);
	}



	//GENERATE MONSTERS

	void genSpike(float x, float y, float z){
		GameObject s = spike;

		Vector3 spawnPosition = new Vector3 (x, y, z);
		s = Instantiate (s, spawnPosition, s.transform.rotation) as GameObject;

		//NOTE: it's crucial that setLife is AFTER instantiation!
		s.GetComponent<Enemy> ().setValues (1, 1);
		s.transform.parent = enemiz.transform;

		if(Random.Range(0, 100.0f)<5){
			s.GetComponent<Animator> ().enabled = false;
			s.GetComponent<Enemy>().setValues(s.GetComponent<Enemy>().getLife(), 0);
		}
	}

	void genBird(float x, float y, float z){
		GameObject b = bird;

		Vector3 spawnPosition = new Vector3 (x, y, z);
		b = Instantiate (b, spawnPosition, b.transform.rotation) as GameObject;

		//NOTE: it's crucial that setLife is AFTER instantiation!
		b.GetComponent<Enemy> ().setValues (1, 1);
		b.transform.parent = enemiz.transform;
	}


}
