using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public float startWait, enemySpawnWait, waveSpawnWait;
    private float range;
    //range is worldspace width on screen/2 (+ and then -)
    public GameObject[] enemies;
    public Vector3 spawnValues;

    public EnemyLoader enemyLoader;
    public GameObject enemiez; //parent of all spawned enemies

    public int waveIndex = 0; 

    void Start()
    {
        StartCoroutine(setSpawnerValues());
    }

    /*
        Types of waves: singleEnemy, ––, /, \, ^, T, U, A, X, *, #, S
    */
    public void StartSpawn (GameFlow gf, int[] w, int[] m, bool[] esDone) {


        //int[] w: waves in this level

        //int[] m: types of monsters for each of the waves; the EXACT SAME SIZE as w[]
        StartCoroutine (SpawnEnemyWaves (gf, w, m, esDone, waveIndex));
        waveIndex++;
    }

    public IEnumerator setSpawnerValues()
    {
        while ((int)range == 0)
        {
            range = Global.STWfactor.x * (Global.MainCanvasWidth / 2) - 40; //TODO do this more smartly, dependent on enemy
            spawnValues = new Vector3(0, Global.STWfactor.y * (Global.MainCanvasHeight / 2) + 200, spawnValues.z);
            yield return new WaitForSeconds(0.1f);
        }

    }

    // Update is called once per frame
    void Update () {
    
    }

    IEnumerator SpawnEnemyWaves(GameFlow gf, int[] waveTypes, int[] enemTypes, bool[] esDone, int index)
    { //types of waves
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
                StartCoroutine(PerformEndCheck(gf, esDone, index));
            }
                yield return new WaitForSeconds (waveSpawnWait);
            }
            //wait time between each wave

    }

    IEnumerator PerformEndCheck(GameFlow gf, bool[] esDone, int index)
    {
        while (GameObject.FindWithTag("Enemy") != null)
        {
            Debug.Log("end check for wave " + index + " still going on");
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
        

}
