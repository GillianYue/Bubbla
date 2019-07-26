using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * this class is in charge of carrying out the custom events (indicated by a code of 99 in game mode)
 * whenever game flow gets to that command. Each event has a code number. With the params read in along
 * with the custom event code, special effects can be created. 
 */
public class CustomEvents : MonoBehaviour {

    protected GameControl gameControl;
    protected GameFlow gameFlow;
    protected EnemySpawner eSpawner;
    protected PaintballSpawner pSpawner;
    //protected string[,] data;
    /**
     * identified is a List that keeps track of all existing GOs with an identifier script attached
     * to. This way, when we look for an identified GO (not tagged because Unity has an inflexible
     * tag system), we need only look inside this list. 
     * 
     * the list is local to each level and would as GOs get destroyed and created.    
     */
    public List<identifier> identified;

	// Use this for initialization
	void Start () {
        GameObject GC = GameObject.FindWithTag("GameController");
        gameControl = GC.GetComponent<GameControl>();
        gameFlow = GC.GetComponent<GameFlow>();
        eSpawner = GC.GetComponent<EnemySpawner>();
        pSpawner = GC.GetComponent<PaintballSpawner>();

        identified = new List<identifier>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //public void setLevelCsvData(string[,] d)
    //{
    //    data = d;

    //}

    public void customEvent(int index, string[] prms)
    {
        StartCoroutine(CustomEvent(index, prms));
    }


    /**
     * the method called by GameFlow when it sees 99 (customEvent) in GAME mode
     * 5 (for now) parameters are passed in as strings stored in string[] prms
     * 
     * contains a switch statement for different custom events
     * the custom events all use the params parsed and the done bool array instantiated at the start
     */
    public IEnumerator CustomEvent(int index, string[] prms) {

        bool[] done = new bool[1];
        done[0] = false;

        switch (index)
        {
            case 1:
                genEnemy(done, prms);
                break;
            case 5:
                StartCoroutine(wait(done, prms));
                break;
            case 10:
                moveTo(done, prms);
                break;
            case 11:
                moveToInSecs(done, prms);
                break;

            default:
                Debug.Log("custom event index not recognized: " + index);
                break;
        }

        yield return new WaitUntil(() => done[0]);

        //move on to the next command, we only need to update the csv pointer in game flow
        gameFlow.incrementPointer();
    }

    /**
     * event #1
     * 
     * param 0: enemy code indicating which enemy we want to generate
     * optional param 1: pos X, will use enemySpawner.spawnValues.x if param is ""
     * optional param 2: pos Y, will use enemySpawner.spawnValues.y if param is ""
     * optional param 3: pos Z, will use enemySpawner.spawnValues.z if param is ""
     * optional param 4: an identifier id for the enemy, will not assign identifier if param is ""
     */
    void genEnemy(bool[] done, string[] prms)
    {
        int enemyCode;
        int.TryParse(prms[0], out enemyCode);

        float x, y, z;
        if (!prms[1].Equals(""))
        {
            float.TryParse(prms[1], out x);
        }
        else
        {
            x = eSpawner.spawnValues.x;
        }
        if (!prms[2].Equals(""))
        {
            float.TryParse(prms[2], out y);
        }
        else
        {
            y = eSpawner.spawnValues.y;
        }
        if (!prms[3].Equals(""))
        {
            float.TryParse(prms[3], out z);
        }
        else
        {
            z = eSpawner.spawnValues.z;
        }

        GameObject e = eSpawner.genMonster(x, y, z, enemyCode);
        if (!prms[4].Equals(""))
        {
            setIdentifier(e, prms[4]);
        }
        done[0] = true;
    }

    /**
 * event #2
 * 
 * creating a paintball, item, non-living-things
 * 
 * param 0: item code indicating which item we want to create
 * optional param 1: pos X, will use enemySpawner.spawnValues.x if param is ""
 * optional param 2: pos Y, will use enemySpawner.spawnValues.y if param is ""
 * optional param 3: pos Z, will use enemySpawner.spawnValues.z if param is ""
 * optional param 4: an identifier id for the enemy, will not assign identifier if param is ""
 */
    void genItem(bool[] done, string[] prms)
    {
        int enemyCode;
        int.TryParse(prms[0], out enemyCode);

        float x, y, z;
        if (!prms[1].Equals(""))
        {
            float.TryParse(prms[1], out x);
        }
        else
        {
            x = eSpawner.spawnValues.x;
        }
        if (!prms[2].Equals(""))
        {
            float.TryParse(prms[2], out y);
        }
        else
        {
            y = eSpawner.spawnValues.y;
        }
        if (!prms[3].Equals(""))
        {
            float.TryParse(prms[3], out z);
        }
        else
        {
            z = eSpawner.spawnValues.z;
        }

        GameObject e = eSpawner.genMonster(x, y, z, enemyCode);
        if (!prms[4].Equals(""))
        {
            setIdentifier(e, prms[4]);
        }
        done[0] = true;
    }

    /**
     * event #5
     * 
     * param 0: time to wait and do nothing in seconds
     */
    IEnumerator wait(bool[] done, string[] prms)
    {
        float secs;
        float.TryParse(prms[0], out secs);

        yield return new WaitForSeconds(secs);
        done[0] = true;
    }



    /**
     * event #10
     * 
     * param 0: GameObject identifier id
     * param 1: pos x of destination
     * param 2: pos y of destination
     * param 3: speed of movement    
     * 
     * Note: we are not in charge of setting done[0] to true here because done is 
     * passed to a sub function in Global, who is in charge of changing done after
     * completing the actions.
     */
    void moveTo(bool[] done, string[] prms)
    {
        //look in identified for an identifier with the right id and return its gameObject
        GameObject target = identified.Find(i => i.id.Equals(prms[0])).gameObject;

        int x;
        int.TryParse(prms[1], out x);

        int y;
        int.TryParse(prms[2], out y);

        float spd;
        float.TryParse(prms[3], out spd);

        StartCoroutine(Global.moveTo(target, x, y, spd, done));
    }

    /**
     * event #11
     * 
     * param 0: GameObject identifier id
     * param 1: pos x of destination
     * param 2: pos y of destination
     * param 3: time to complete the move in seconds   
     */
    void moveToInSecs(bool[] done, string[] prms)
    {
        GameObject target = identified.Find(i => i.id.Equals(prms[0])).gameObject;

        int x;
        int.TryParse(prms[1], out x);

        int y;
        int.TryParse(prms[2], out y);

        float secs;
        float.TryParse(prms[3], out secs);

        StartCoroutine(Global.moveToInSecs(target, x, y, secs, done));
    }



    //~~~~~~~~~~~~~~~~~~~~~~~~~~helper functions~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /**
         * attaches an identifier script to the go, sets id to given, and adds to local 
         * identified list        
         */
    private void setIdentifier(GameObject go, string id)
    {
        identifier i = go.AddComponent<identifier>();
        i.setID(id);
        identified.Add(i);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~helper functions~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
}
