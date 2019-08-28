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
    public bool pbSpawnSwitch; //when start flip to true, elsewhere will set it to false to end pb Spawn
    public ItemLoader itemLoader; 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setSpawnerValues());
    }

    public IEnumerator setSpawnerValues()
    {
        while ((int)spawnRangeWidth == 0) {
            spawnRangeWidth = Global.STWfactor.x * (Global.MainCanvasWidth / 2) - 20;
            spawnValues = new Vector3(0, Global.STWfactor.y * (Global.MainCanvasHeight / 2) + 200, 0);
            yield return new WaitForSeconds(0.1f);
        }

    }

    public void StartSpawn(bool[] esDone)
    {


        //int[] w: waves in this level

        //int[] m: types of monsters for each of the waves; the EXACT SAME SIZE as w[]

        StartCoroutine(SpawnPaintballs(esDone));

    }

    public IEnumerator SpawnPaintballs(bool[] esDone) //esDone is always specific to this one IEnumerator process
    {

        yield return new WaitForSeconds(startWait);
        while (!esDone[0])
        {
            //if (Random.Range(0, 99.99f) > 5)
            //{

                genPaintball(Random.Range(spawnValues.x - spawnRangeWidth,
                        spawnValues.x + spawnRangeWidth),
                    spawnValues.y, spawnValues.z);
                yield return new WaitForSeconds(pbSpawnWait);

            //}
            //else
            //{//5% chance to generate potion
            //    genItem(2, Random.Range(spawnValues.x - spawnRangeWidth,
            //    spawnValues.x + spawnRangeWidth),
            //spawnValues.y, spawnValues.z);
            //}
        }
    }

    /**
     * generates a paintball at location, with parent set to Ballz
     */
    public GameObject genPaintball(float x, float y, float z)
    {
        GameObject pb = paintBall;

        Vector3 spawnPosition = new Vector3(x, y, z);
        pb = Instantiate(pb, spawnPosition, pb.transform.rotation)
            as GameObject;
        pb.transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)));
        pb.transform.SetParent(Ballz.transform);

        return pb;
    }

    public GameObject genPaintball(Color c, float x, float y, float z)
    {
        GameObject pb = genPaintball(x, y, z);
        pb.GetComponent<PaintballBehavior>().setColor(c);
        return pb;
    }

    public GameObject genPaintball(float R, float G, float B, float x, float y, float z)
    {
        GameObject pb = genPaintball(x, y, z);
        pb.GetComponent<PaintballBehavior>().setColor(R,G,B);
        return pb;
    }

    // creates an item based off of code that behaves like a paintball
    public GameObject genItem(int itemCode, float x, float y, float z)
    {
        GameObject item = itemLoader.getItemInstance(itemCode); //already instantiated

        Vector3 spawnPosition = new Vector3(x, y, z);
        item.GetComponent<RectTransform>().position = spawnPosition;
        item.transform.SetParent(Ballz.transform);

        return item;
    }

}
