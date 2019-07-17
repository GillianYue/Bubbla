using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public float startWait, enemySpawnWait, waveSpawnWait;
    private float range;
	//range is worldspace width on screen/2 (+ and then -)
	public GameObject enemiz;
	public GameObject[] enemies;
	public Vector3 spawnValues;


	/*
		Types of waves: ––, /, \, ^, T, U, A, X, *, #, S
	*/
	public void StartSpawn (int[] w, int[] m) {


        //int[] w: waves in this level

        //int[] m: types of monsters for each of the waves; the EXACT SAME SIZE as w[]
        StartCoroutine(setSpawnerValues());
        StartCoroutine (SpawnEnemyLevel (w, m));

	}

    public IEnumerator setSpawnerValues()
    {
        while ((int)range == 0)
        {
            range = Global.STWfactor.x * (Global.MainCanvasWidth / 2);
            spawnValues = new Vector3(0, Global.STWfactor.y * (Global.MainCanvasHeight / 2) + 200, 0);
            yield return new WaitForSeconds(0.1f);
        }

    }

    // Update is called once per frame
    void Update () {
	
	}

	IEnumerator SpawnEnemyLevel(int[] waveTypes, int[] enemTypes){ //types of waves
		yield return new WaitForSeconds (startWait);
		//start wait time

		for (int c = 0; c<waveTypes.Length; c++) {
			int currW = waveTypes [c];
			int currE = enemTypes [c];

			switch(currW){ //current wave
			case 0:
				SpawnHorizontal (3, currE);
				break;
			case 1: 
				yield return StartCoroutine (SpawnSlash(true, 3, currE));
				break;
			case 2:
				yield return StartCoroutine (SpawnSlash (false, 3, currE));
				break;
			case 3:
				yield return StartCoroutine (SpawnCaret (5, currE));
				break;

			default:
				break;
				
			}

				yield return new WaitForSeconds (waveSpawnWait);
			}
			//wait time between each wave

	}


	//SPAWN WAVES

	void SpawnHorizontal(int numEnemy, int enemyCode){
		
		for (int c = 0; c < numEnemy; c++) {
			
			genMonster((-range + c*(2*range/(float)(numEnemy-1))), spawnValues.y, 
				spawnValues.z, enemyCode);

		}
	}

	IEnumerator SpawnSlash(bool ForB, int numEnemy, int enemyCode){
		for (int c = 0; c < numEnemy; c++) {
				
			genMonster (((ForB) ? -1 : 1) * (range - c * (2 * range / (float)(numEnemy - 1))), 
				spawnValues.y, spawnValues.z, enemyCode);

			yield return new WaitForSeconds (enemySpawnWait+0.8f);
		}
	}

	IEnumerator SpawnCaret(int numEnemy, int enemyCode){
		//numEnemy has to be odd for this shaped formation
		for (int c = 0; c < (numEnemy/2); c++) {
			genMonster (-range + c * (2*range/(float)(numEnemy-1)), 
				spawnValues.y, spawnValues.z, enemyCode);
			genMonster (range - c * (2*range/(float)(numEnemy-1)), 
				spawnValues.y, spawnValues.z, enemyCode);

			yield return new WaitForSeconds (enemySpawnWait);
		}

		genMonster (0, spawnValues.y, spawnValues.z, enemyCode);
	}



	//ALL MONSTERS GENERATED HERE
	void genMonster(float x, float y, float z, int monsterCode){
		GameObject e = enemies [monsterCode];
		Vector3 spawnPosition = new Vector3 (x, y, z);
		e = Instantiate (e, spawnPosition, e.transform.rotation) as GameObject;

		int LIFE, ATTACK;
		switch (monsterCode) {
		case 0: //seaStar
			LIFE = 2;
			ATTACK = 1;
			break;
		case 1: //crab
			LIFE = 1;
			ATTACK = 1;
			break;
		
		default: 
			LIFE = 0;
			ATTACK = 0;
			break;
		}

		//NOTE: it's crucial that setLife is AFTER instantiation!
		e.GetComponent<Enemy> ().setValues (LIFE, ATTACK);
		e.transform.parent = enemiz.transform;
	}
		

}
