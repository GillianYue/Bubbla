using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public float startWait, enemySpawnWait, waveSpawnWait;
    private float range;
	//range is worldspace width on screen/2 (+ and then -)
	public GameObject enemiz;
	public GameObject[] enemies;
	public Vector3 spawnValues;

    void Start()
    {
        StartCoroutine(setSpawnerValues());
    }

	/*
		Types of waves: singleEnemy, ––, /, \, ^, T, U, A, X, *, #, S
	*/
	public void StartSpawn (GameFlow gf, int[] w, int[] m) {


        //int[] w: waves in this level

        //int[] m: types of monsters for each of the waves; the EXACT SAME SIZE as w[]
        StartCoroutine (SpawnEnemyWaves (gf, w, m));

	}

    public IEnumerator setSpawnerValues()
    {
        while ((int)range == 0)
        {
            range = Global.STWfactor.x * (Global.MainCanvasWidth / 2);
            spawnValues = new Vector3(0, Global.STWfactor.y * (Global.MainCanvasHeight / 2) + 200, spawnValues.z);
            yield return new WaitForSeconds(0.1f);
        }

    }

    // Update is called once per frame
    void Update () {
	
	}

	IEnumerator SpawnEnemyWaves(GameFlow gf, int[] waveTypes, int[] enemTypes){ //types of waves
		yield return new WaitForSeconds (startWait);
		//start wait time

		for (int c = 0; c<waveTypes.Length; c++) {
			int currW = waveTypes [c];
			int currE = enemTypes [c];

			switch(currW){ //current wave
                case 0:
                //spawn SINGLE MONSTER
                    break;
                case 1:
				SpawnHorizontal (3, currE);
				break;
			case 2: 
				yield return StartCoroutine (SpawnSlash(true, 3, currE));
				break;
			case 3:
				yield return StartCoroutine (SpawnSlash (false, 3, currE));
				break;
			case 4:
				yield return StartCoroutine (SpawnCaret (5, currE));
				break;

			default:
				break;
				
			}

            /**
             * when it's the last wave of enemies dispatch an enumerator checking whether 
             * the last enemies are defeated, that is, there is no "enemy" in the scene
             * anymore, if so proceed to what's after gameplay in gameflow            
             */            
            if(c == waveTypes.Length - 1)
            {
                StartCoroutine(PerformEndCheck(gf));
            }
				yield return new WaitForSeconds (waveSpawnWait);
			}
			//wait time between each wave

	}

    IEnumerator PerformEndCheck(GameFlow gf)
    {
        while (GameObject.FindWithTag("Enemy") != null)
        {

            yield return new WaitForSeconds(0.5f);
        }
        //done
        gf.processCurrentLine(); //move on to the line after GAME
    }


    //SPAWN WAVES

    public void SpawnHorizontal(int numEnemy, int enemyCode){
		
		for (int c = 0; c < numEnemy; c++) {
			
			genMonster((-range + c*(2*range/(float)(numEnemy-1))), spawnValues.y, 
				spawnValues.z, enemyCode);

		}
	}

    public IEnumerator SpawnSlash(bool ForB, int numEnemy, int enemyCode){
		for (int c = 0; c < numEnemy; c++) {
				
			genMonster (((ForB) ? -1 : 1) * (range - c * (2 * range / (float)(numEnemy - 1))), 
				spawnValues.y, spawnValues.z, enemyCode);

			yield return new WaitForSeconds (enemySpawnWait+0.8f);
		}
	}

	public IEnumerator SpawnCaret(int numEnemy, int enemyCode){
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
	public GameObject genMonster(float x, float y, float z, int monsterCode){
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

        return e;
	//	e.transform.parent = enemiz.transform;
      // e.transform.localPosition.z = 0;
	}
		

}
