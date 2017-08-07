using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public float startWait, enemySpawnWait, waveSpawnWait, range;
	public GameObject enemy, enemiz;
	public Vector3 spawnValues;


	/*
		Types of waves: ––, /, \, ^, T, U, A, X, *, #, S
	*/
	// Use this for initialization
	void Start () {
		int[] t = new int[6]{0, 1, 2, 0, 0, 1};
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
				print ("horizontal!");
				SpawnHorizontal (3);
				break;
			case 1:
				print ("forward slash !");
				yield return StartCoroutine (SpawnSlash(true, 3));
				break;
			case 2:
				print ("back slash !");
				yield return StartCoroutine (SpawnSlash (false, 3));
				break;

			default:
				print ("default!");
				break;
				
			}

				yield return new WaitForSeconds (waveSpawnWait);
			}
			//wait time between each wave

	}

	void SpawnHorizontal(int numEnemy){
		
		for (int c = 0; c < numEnemy; c++) {
			GameObject e = enemy;

			Vector3 spawnPosition = new Vector3 (
				(-range + c*(2*range/(float)(numEnemy-1))), spawnValues.y, spawnValues.z);
			e = Instantiate (e, spawnPosition, e.transform.rotation)
			as GameObject;
			
			e.transform.parent = enemiz.transform;

		}
	}

	IEnumerator SpawnSlash(bool ForB, int numEnemy){
		for (int c = 0; c < numEnemy; c++) {

				GameObject e = enemy;

				Vector3 spawnPosition = new Vector3 (
				(((ForB)? -1:1) * (range - c*(2*range/(float)(numEnemy-1)))), 
				spawnValues.y, spawnValues.z);
				e = Instantiate (e, spawnPosition, e.transform.rotation)
					as GameObject;

				e.transform.parent = enemiz.transform;


			yield return new WaitForSeconds (enemySpawnWait);
		}
	}
}
