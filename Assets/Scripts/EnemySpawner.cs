﻿using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public float startWait, enemySpawnWait, waveSpawnWait;
    private float range;
    //range is worldspace width on screen/2 (+ and then -)
    public GameObject[] enemies;
    public Vector3 spawnValues;

    [Inject(InjectFrom.Anywhere)]
    public EnemyLoader enemyLoader;
    public GameObject enemiez; //parent of all spawned enemies

    public int waveIndex = 0;

    [Inject(InjectFrom.Anywhere)]
    public PathManager pathManager;

    void Start()
    {
        StartCoroutine(setSpawnerValues());
    }

    /*
        Types of waves: singleEnemy, ––, /, \, ^, T, U, A, X, *, #, S
    */
    public void StartSpawn (GameFlow gf, int[] w, int[] m, int[] waveWaits, float[] intv, int[] spnm, bool[] esDone) {


        //int[] w: waves in this level

        //int[] m: types of monsters for each of the waves; the EXACT SAME SIZE as w[]
        StartCoroutine (SpawnEnemyWaves (gf, w, m, waveWaits, intv, spnm, esDone, waveIndex));
        waveIndex++;
    }

    public IEnumerator setSpawnerValues()
    {
        while ((int)range == 0)
        {
            range =  (Screen.width / 2) - 40; //TODO do this more smartly, dependent on enemy
            spawnValues = new Vector3(0, (Screen.height / 2) + 200, spawnValues.z);
            yield return new WaitForSeconds(0.1f);
        }

    }

    // Update is called once per frame
    void Update () {
    
    }

    IEnumerator SpawnEnemyWaves(GameFlow gf, int[] waveTypes, int[] enemTypes, int[] waveWaits, float[] intervals, int[] spawnNum, bool[] esDone, int index)
    { //types of waves
        

        for (int c = 0; c<waveTypes.Length; c++) {

            if(waveWaits[c]!=-1) yield return new WaitForSeconds(waveWaits[c]); //-1 means wait until no enemies left on field
            else while (GameObject.FindWithTag("Enemy") != null) //infinite wait time until prev wave gone
                {
                    yield return new WaitForSeconds(0.5f);
                }

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
                StartCoroutine (SpawnSlash(true, 3, currE));
                break;
            case 3:
                StartCoroutine (SpawnSlash (false, 3, currE));
                break;
            case 4:
                StartCoroutine (SpawnCaret (5, currE));
                break;
/*            case 5:
                break;*/

            default:
                    if (currW >= 5) StartCoroutine(SpawnPath(spawnNum[c], currW - 5, currE, intervals[c]));
                break;
                
            }

            /**
             * when it's the last wave of enemies dispatch an enumerator checking whether 
             * the last enemies are defeated, that is, there is no "enemy" in the scene
             * anymore, if so proceed to what's after gameplay in gameflow            
             */            
            if(c == waveTypes.Length - 1)
            {
                StartCoroutine(PerformEndCheck(gf, esDone, index));
            }
                //yield return new WaitForSeconds (waveSpawnWait);

            }
            //wait time between each wave

    }

    IEnumerator PerformEndCheck(GameFlow gf, bool[] esDone, int index)
    {
        while (GameObject.FindWithTag("Enemy") != null)
        {
            yield return new WaitForSeconds(0.5f);
        }
        //done
        Debug.Log("current enemy wave done");
        esDone[0] = true; //to turn spawner off only to be turned on again later
        
        gf.incrementPointer(); //move on to the line after GAME
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

    public IEnumerator SpawnPath(int numEnemy, int pathCode, int enemyCode, float spawnInterval)
    {
        for (int c = 0; c < numEnemy; c++)
        {
            SteerPath p = pathManager.paths[pathCode];
            Vector3 point = p.getNodesAsVectorArray()[0];
            genMonster(point.x, point.y, point.z, enemyCode, p);

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    //ALL MONSTERS GENERATED HERE
    public GameObject genMonster(float x, float y, float z, int monsterCode){

        /**
         * the GameObject returned from enemyLoader is already instantiated (though at the origin)
         * and has all the stats set along with the animator override set up
         * 
         * for now the only thing needs to be done here in the spawner is to set the right position        
         */
        GameObject e = enemyLoader.getEnemyInstance(monsterCode); //is already Instantiated here

        Vector3 spawnPosition = new Vector3 (x, y, z);
        e.transform.position = spawnPosition;
        e.transform.parent = enemiez.transform;

        return e;
    }

    //overload for above, use when this enemy is supposed to be following a SteerPath
    public GameObject genMonster(float x, float y, float z, int monsterCode, SteerPath p)
    {

        GameObject e = genMonster(x, y, z, monsterCode);
        EnemySteering steer = e.GetComponent<EnemySteering>();
        steer.enabled = true;
        steer.setPath(pathManager, p);
        e.GetComponent<EnemyMover>().enabled = false;

        return e;
    }

    public void destroyAllEnemies()
    {
        foreach (Transform child in enemiez.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
