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
    public LevelScript levelScript;

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
        foreach ( identifier i in FindObjectsOfType<identifier>())
        {
            identified.Add(i);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}


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
    IEnumerator CustomEvent(int index, string[] prms) {

        bool[] done = new bool[1];
        done[0] = false;

        switch (index)
        {
            case 1:
                genEnemy(done, prms);
                break;
            case 3:
                genPaintball(done, prms);
                break;
            case 5:
                StartCoroutine(wait(done, prms));
                break;
            case 6:
                StartCoroutine(changeAnimState(done, prms));
                break;
            case 7:
                StartCoroutine(setGOActive(done, prms));
                break;
            case 8:
                setScriptBoolean(done, prms);
                break;
            case 10:
                moveTo(done, prms);
                break;
            case 11:
                moveToInSecs(done, prms);
                break;
            case 99:
                levelScriptEvent(done, prms);
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
    public void genEnemy(bool[] done, string[] prms)
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

    /**TODO
     * event #2
     * 
     * creating a item, non-living-things
     * 
     * param 0: item code indicating which item we want to create
     * optional param 1: pos X, will use enemySpawner.spawnValues.x if param is ""
     * optional param 2: pos Y, will use enemySpawner.spawnValues.y if param is ""
     * optional param 3: pos Z, will use enemySpawner.spawnValues.z if param is ""
     * optional param 4: an identifier id for the enemy, will not assign identifier if param is ""
     */
    public void genItem(bool[] done, string[] prms)
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
     * event #3
     * 
     * create a paintball  
     * 
     * optional param 0: r,g,b of color, will use paintballSpawner's current atmospherical color if param is ""
     * optional param 1: pos X,Y,Z, will use paintballSpawner.spawnValues if param is ""
     * optional param 2: an identifier id for the paintball, will not assign if empty    
     * optional param 3: size  
     * param 4: whether EnemyMover is active initially (0 is inactive, 1 active)
     *   
     */
    public void genPaintball(bool[] done, string[] prms)
    {
        int r,g,b;

        float x, y, z;
        if (!prms[1].Equals(""))
        {
            string[] res = prms[1].Split(',');
            float.TryParse(res[0], out x);
            float.TryParse(res[1], out y);
            float.TryParse(res[2], out z);
        }
        else
        {
            x = pSpawner.spawnValues.x;
            y = pSpawner.spawnValues.y;
            z = pSpawner.spawnValues.z;
        }
     

        GameObject p;

        if (!prms[0].Equals(""))
        {
            string[] res = prms[0].Split(',');
            int.TryParse(res[0], out r);
            int.TryParse(res[1], out g);
            int.TryParse(res[2], out b);

            p = pSpawner.genPaintball(r, g, b, x, y, z); //need not worry about conversion, taken care of in PBbehavior.setColor
        }
        else
        {
            p = pSpawner.genPaintball(x, y, z);
        }

        int size;

        if (!prms[3].Equals(""))
        {
            int.TryParse(prms[3], out size);
            p.GetComponent<PaintballBehavior>().setSize(size);
        }

        if (!prms[2].Equals(""))
        {
            setIdentifier(p, prms[2]);
        }

        int enemyMoverActive;
        int.TryParse(prms[4], out enemyMoverActive);
        if(enemyMoverActive == 0)
        {
            p.GetComponent<EnemyMover>().enabled = false;
            p.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        done[0] = true;
    }

    /**
     * event #5
     * 
     * param 0: time to wait and do nothing in seconds
     */
    public IEnumerator wait(bool[] done, string[] prms)
    {
        float secs;
        float.TryParse(prms[0], out secs);

        yield return new WaitForSeconds(secs);
        done[0] = true;
    }

    /**
     * event #6
     * 
     * param 0: identifier of the gameObject
     * param 1: the integer to set "State" to
     * optional param 2: index of transition effects
     *      - 0: fade in (fade out of prev bg, change "State", fade in)
     *     
     */
    public IEnumerator changeAnimState(bool[] done, string[] prms)
    {
        //look in identified for an identifier with the right id and return its gameObject
        GameObject target = findByIdentifier(prms[0]);
        Animator anim = target.GetComponent<Animator>();
        SpriteRenderer spr = target.GetComponent<SpriteRenderer>();
        Color orig = spr.color;
        float origA = orig.a;

        int x;
        int.TryParse(prms[1], out x);

        int effect = -1;
        if (!prms[2].Equals(""))
        {
            int.TryParse(prms[2], out effect);
        }

        if (effect == 0)
        {
            for (float o = origA; o > 0.5f; o -= 0.3f) //fade out of current bg
            {
                spr.color = new Color(orig.r, orig.g, orig.b, o);
                yield return new WaitForSeconds(0.1f);
            }

        }

        anim.SetInteger("State", x);

        orig = spr.color;
        if (effect == 0)
        {
            for (float o = 0.5f; o <= origA; o += 0.2f) //fade in of new bg
            {
                spr.color = new Color(orig.r, orig.g, orig.b, o);
                yield return new WaitForSeconds(0.1f);
            }
            if (Mathf.Abs(spr.color.a - origA) > 0.001f) //if unequal
            {
                spr.color = new Color(orig.r, orig.g, orig.b, origA);
            }
        }
        done[0] = true;
    }

    /**
     * event #7
     * 
     * param 0: identifier
     * param 1: set to active (1) or inactive (0)    
     * optional param 2: effect
     *      - 0: fade in/out, depending on setting to active or inactive
     */
    public IEnumerator setGOActive(bool[] done, string[] prms)
    {
        GameObject target = findByIdentifier(prms[0]);

        int active;
        int.TryParse(prms[1], out active);

        SpriteRenderer spr; int effect = -1; float origA = 1; Color orig = Color.black;
        if ((spr = target.GetComponent<SpriteRenderer>()) != null)
        {
            //means this GO has a spriteRenderer, ok
            orig = spr.color;
            origA = orig.a;

            if (!prms[2].Equals("")) //only parse effect when there is a spriteRenderer
            {
                int.TryParse(prms[2], out effect);
            }

            if (effect == 0)
            {
                if (active == 0)
                {
                    for (float o = origA; o > 0.3f; o -= 0.15f) //fade out of sprite
                    {
                        spr.color = new Color(orig.r, orig.g, orig.b, o);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
        } //this all wouldn't happen if a spriterenderer doesn't exist


        //normal procedure 
        if (active == 0)
        {
            target.SetActive(false);
        }
        else if (active == 1)
        {
            target.SetActive(true);
        }

        //continue from prev spr effect; fade in only happens after GO is set to active
        if (effect == 0)
        {
            for (float o = 0; o <= origA; o += 0.15f) //fade in of sprite
            {
                spr.color = new Color(orig.r, orig.g, orig.b, o);
                yield return new WaitForSeconds(0.2f);
            }
            if (Mathf.Abs(spr.color.a - origA) > 0.001f) //if unequal
            {
                spr.color = new Color(orig.r, orig.g, orig.b, origA);
            }
        }

        done[0] = true;
    }

    /**
     * event #8
     * 
     * param 0: index of which boolean to set (hard-coded in script, here)
     *      - 0: setting GameControl.ckTouch
     *      - 1: gameFlow.canMovePointer
     * 
     * param 1: to true (1) or false (0)    
     */
     public void setScriptBoolean(bool[] done, string[] prms)
    {
        int index;
        int.TryParse(prms[0], out index);

        int b;
        int.TryParse(prms[1], out b);

        switch (index)
        {
            case 0:
                gameControl.ckTouch = (b == 1) ? true : false;
                break;
            case 1:
                gameFlow.canMovePointer = (b == 1) ? true : false;
                break;
            default:
                break;
        }

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
    public void moveTo(bool[] done, string[] prms)
    {
        //look in identified for an identifier with the right id and return its gameObject
        GameObject target = findByIdentifier(prms[0]);

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
    public void moveToInSecs(bool[] done, string[] prms)
    {
        GameObject target = findByIdentifier(prms[0]);

        int x;
        int.TryParse(prms[1], out x);

        int y;
        int.TryParse(prms[2], out y);

        float secs;
        float.TryParse(prms[3], out secs);

        StartCoroutine(Global.moveToInSecs(target, x, y, secs, done));
    }

    /**
     * event #99
     * 
     * param 0: index for event in levelScript
     * other params dependent to levelScript functions
     *
     */    
    public void levelScriptEvent(bool[] done, string[] prms)
    {
        int index;
        int.TryParse(prms[0], out index);

        StartCoroutine(levelScript.levelScriptEvent(index, done));

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

    public GameObject findByIdentifier(string id)
    {

        int target = identified.FindIndex(i => (i.id.Equals(id)));
        if (target == -1)
        {
            return null;
        }
        else
        {
            return identified[target].gameObject;
        }
    }

    public void removeFromIdentified(identifier i)
    {
        identified.Remove(i);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~helper functions~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
}
