using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintballSpawner : MonoBehaviour
{
    /**spawnRangeWidth should be half the full screen width in world space
     * spawnValues is the center point to add spawnRangeWidth to 
     * startWait is time (in seconds) before game starts generating paintballs
     * pbSpawnWait is time between generation of each paintball
     */    
    public float startWait, pbSpawnWait, spawnRangeWidth;
    public GameObject paintBall, potion; //prefabs to be cloned in game
    public Vector3 spawnValues;
    public GameObject Ballz;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setSpawnerValues());
    }

    public IEnumerator setSpawnerValues()
    {
        while ((int)spawnRangeWidth == 0) {
            spawnRangeWidth = Global.STWfactor.x * (Global.MainCanvasWidth / 2);
            spawnValues = new Vector3(0, Global.STWfactor.y * (Global.MainCanvasHeight / 2) + 200, 0);
            yield return new WaitForSeconds(0.1f);
        }

    }

    public void StartSpawn()
    {


        //int[] w: waves in this level

        //int[] m: types of monsters for each of the waves; the EXACT SAME SIZE as w[]

        StartCoroutine(SpawnPaintballs());

    }

    public IEnumerator SpawnPaintballs()
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            if (Random.Range(0, 99.99f) > 5)
            {
                GameObject pb = paintBall;

                Vector3 spawnPosition = new Vector3(
                    Random.Range(spawnValues.x - spawnRangeWidth,
                        spawnValues.x + spawnRangeWidth),
                    spawnValues.y, spawnValues.z);
                pb = Instantiate(pb, spawnPosition, pb.transform.rotation)
                    as GameObject;
                pb.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
                pb.transform.SetParent(Ballz.transform);

                yield return new WaitForSeconds(pbSpawnWait);

            }
            else
            {//5% chance to generate potion
                GameObject pt = potion;

                Vector3 spawnPosition = new Vector3(
                    Random.Range(spawnValues.x - spawnRangeWidth,
                        spawnValues.x + spawnRangeWidth),
                    spawnValues.y, spawnValues.z);
                pt = Instantiate(pt, spawnPosition, pt.transform.rotation)
                    as GameObject;
                pt.transform.SetParent(Ballz.transform);
            }
        }
    }
}
