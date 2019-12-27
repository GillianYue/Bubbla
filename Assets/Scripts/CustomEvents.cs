using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/*
 * this class is in charge of carrying out the custom events (indicated by a code of 99 in game mode)
 * whenever game flow gets to that command. Each event has a code number. With the params read in along
 * with the custom event code, special effects can be created. 
 */
public class CustomEvents : MonoBehaviour
{

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;

    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;

    [Inject(InjectFrom.Anywhere)]
    public EnemySpawner eSpawner;

    [Inject(InjectFrom.Anywhere)]
    public PaintballSpawner pSpawner;

    protected GameObject vfxCanvas;
    public LevelScript levelScript;

    //protected string[,] data;
    /*
     * identified is a List that keeps track of all existing GOs with an identifier script attached
     * to. This way, when we look for an identified GO (not tagged because Unity has an inflexible
     * tag system), we need only look inside this list. 
     * 
     * the list is local to each level and would as GOs get destroyed and created.    
     */
    public List<identifier> identified;

    // Use this for initialization
    void Start()
    {

        vfxCanvas = (gameControl.vfxCanvas) ? gameControl.vfxCanvas : null;

        identified = new List<identifier>();
        foreach (identifier i in FindObjectsOfType<identifier>())
        {
            identified.Add(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void customEvent(int index, string[] prms)
    {
        StartCoroutine(CustomEvent(index, prms));
    }


    /*
     * the method called by GameFlow when it sees 99 (customEvent) in GAME mode
     * 5 (for now) parameters are passed in as strings stored in string[] prms
     * 
     * contains a switch statement for different custom events
     * the custom events all use the params parsed and the done bool array instantiated at the start
     */
    IEnumerator CustomEvent(int index, string[] prms)
    {

        bool[] done = new bool[1];
        done[0] = false;

        switch (index)
        {
            case 1:
                genEnemy(done, prms);
                break;
            case 2:
                genItem(done, prms);
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
            case 12:
                clearEnemiesOrPBs(done, prms);
                break;
            case 20:
                StartCoroutine(vfx(done, prms));
                break;
            case 30:
                variable(done, prms);
                break;
            case 31:
                conditionalSwitch(done, prms);
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
        if(index != 31) //conditional switch modifies the pointer already, should not have an additional increment
        gameFlow.incrementPointer();
    }

    /*
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

    /* TODO haven't tested out yet, need check
     * event #2
     * 
     * creating an in-game item, or other non-living-things
     * 
     * param 0: item code indicating which item we want to create
     * optional param 1: pos X,Y,Z, will use paintballSpawner.spawnValues if param is ""
     * optional param 2: size
     * optional param 3: id
     * optional param 4: additional param TODO
     */
    public void genItem(bool[] done, string[] prms)
    {
        int itemCode;
        int.TryParse(prms[0], out itemCode);

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


        GameObject i = pSpawner.genItem(itemCode, x, y, z);

        int size;
        if (!prms[2].Equals(""))
        {
            int.TryParse(prms[3], out size);
            i.GetComponent<ItemBehav>().setSize(size);
        }

        if (!prms[3].Equals(""))
        {
            setIdentifier(i, prms[3]);
        }

        done[0] = true;
    }


    /*
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
        int r, g, b;

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
        if (enemyMoverActive == 0)
        {
            p.GetComponent<EnemyMover>().enabled = false;
            p.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        }
        done[0] = true;
    }

    /*
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

    /*
     * event #6
     * 
     * param 0: identifier of the gameObject
     * param 1: the integer to set "State" to
     * optional param 2: index of transition effects
     *      - 0: fade in (fade out of prev bg, change "State", fade in); this is used for in-between bg transitions
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

    /*
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

        if (target != null)
        {
            int active;
            int.TryParse(prms[1], out active);


            SpriteRenderer spr; int effect = -1; float origA = 1; Color orig = Color.black;
            if ((spr = target.GetComponent<SpriteRenderer>()) != null)
            {
                //means this GO has a spriteRenderer, ok
                orig = spr.color;
                origA = orig.a;

                if (prms.Length > 2 && !prms[2].Equals("")) //only parse effect when there is a spriteRenderer
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
        }

        done[0] = true;

    }

    /*
     * event #8
     * 
     * param 0: index of which boolean to set (hard-coded in script, here)
     *      - 0: setting GameControl.ckTouch //can still click on buttons and dialogues, just no in-game touch check
     *      - 1: gameFlow.canMovePointer
     *      - 2: bgMover.scrollin (IDed via id -- param 2)   
     *      - 3: buttons: backpack.backpackBtnActive && TODO setting button active & more
     * 
     * param 1: to true (1) or false (0)    
     *     
     * optional param 2: info param depending on index in param 0
     *      - for param 0_2: id of the GO that has a BGmover script    
     *          
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
            case 2:
                string id = prms[2];
                GameObject bg = findByIdentifier(id);
                if (b == 0)
                {
                    bg.GetComponent<BGMover>().stopBGScroll();
                }
                else if (b == 1)
                {
                    bg.GetComponent<BGMover>().resumeBGScroll();
                }
                break;
            case 3:
                gameControl.backpack.backpackBtnActive = (b == 1) ? true : false;
                break;
            default:
                break;
        }

        done[0] = true;
    }

    /*
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

        StartCoroutine(Global.moveTo(target, x, y, spd, done)); //will set done[0] to true
    }

    /*
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

        StartCoroutine(Global.moveToInSecs(target, x, y, secs, done)); //will set done[0] to true
    }

    /*
     * event #12
     * 
     *  destroys all entries of designated GO group
     * 
     * param 0: -0 is both enemies & pbs
     *          -1 only enemies
     *          -2 only pbs    
     */
    public void clearEnemiesOrPBs(bool[] done, string[] prms)
    {
        int index;
        int.TryParse(prms[0], out index);

        switch (index)
        {
            case 0:
                pSpawner.destroyAllpb();
                eSpawner.destroyAllEnemies();
                break;
            case 1:
                eSpawner.destroyAllEnemies();
                break;
            case 2:
                pSpawner.destroyAllpb();
                break;
        }

        done[0] = true;
    }

    /*
     * event #20
     * 
     * IEnumerator for all sorts of visual effects
     * 
     * param 0: vfx index
     *      -0: black fade out
     *      -1: black fade in    
     * param 1: whether to leave vfxCanvas active afterwards
     *      -0: false (set to inactive)
     *      -1: true (leave as active)    
     */
    public IEnumerator vfx(bool[] done, string[] prms)
    {
        vfxCanvas.SetActive(true);

        int index;
        int.TryParse(prms[0], out index);
        Image img = vfxCanvas.GetComponent<Image>();

        int leaveActive;
        int.TryParse(prms[1], out leaveActive);

        switch (index)
        {
            case 0: //fade out to black canvas; default fade out is 2 seconds in total
                if (img != null)
                {

                    for (float o = 0; o <= 1; o += 0.025f) //fade out of sprite
                    {
                        img.color = new Color(0, 0, 0, o);
                        yield return new WaitForSeconds(0.05f);
                    }
                    if (img.color.a < 1) img.color = new Color(0, 0, 0, 1); //black
                }
                break;
            case 1: //black canvas fading back into view, reverse process of case 0
                if (img != null)
                {

                    for (float o = 1; o >= 0; o -= 0.025f) //fade out of sprite
                    {
                        img.color = new Color(0, 0, 0, o);
                        yield return new WaitForSeconds(0.05f);
                    }
                    if (img.color.a > 0) img.color = new Color(0, 0, 0, 0); //black
                }
                break;

        }

        if (leaveActive == 0)
            vfxCanvas.SetActive(false);

        done[0] = true;
    }


    /*
    * event #30
    * 
    * to create a new or modify an old variable with a string name (linked in code via a dictionary)
     * 
     * param 0: variable type(s), separated by comma
     *      -0: bool
     *      -1: integer
     *      -2: string
     * param 1: string(s) to identify this variable, separated by comma
     * param 2: starting value(s) of this variable (type will be converted accordingly) OR value(s) to be mod into,
     * separated by comma
     * 
     */

    public void variable(bool[] done, string[] prms)
    {

        string[] varTypez = prms[0].Split(',');
        int[] varTypes = new int[varTypez.Length];
        for (int t = 0; t < varTypez.Length; t++)
        {
            int.TryParse(varTypez[t], out varTypes[t]);
        }

        string[] varNames = prms[1].Split(',');

        string[] varValues = prms[2].Split(',');

        for (int t = 0; t < varTypes.Length; t++)
        {
            
            switch (varTypes[t])
            {
                case 0:
                    bool b;
                    bool.TryParse(varValues[t], out b);
                    if (Global.boolVariables.ContainsKey(varNames[t]))
                        Global.boolVariables[varNames[t]] = b;
                    else
                        Global.boolVariables.Add(varNames[t], b);
                    break;
                case 1:
                    int i;
                    int.TryParse(varValues[t], out i);
                    if (Global.intVariables.ContainsKey(varNames[t]))
                        Global.intVariables[varNames[t]] = i;
                    else
                        Global.intVariables.Add(varNames[t], i);
                    break;
                case 2:
                    if (Global.stringVariables.ContainsKey(varNames[t]))
                        Global.stringVariables[varNames[t]] = varValues[t];
                    else
                        Global.stringVariables.Add(varNames[t], varValues[t]);
                    break;
                default:
                    Debug.Log("unclear variable type for custom event 30");
                    break;
            }

        }

        done[0] = true;
    }


    /*
     * event #31
     * 
     * conditional switch that changes the line pointer in GFlow according to conditions
     * 
     * param 0: variable type
     *      -0: bool
     *      -1: integer
     *      -2: string
     * param 1: string to identify this variable
     * param 2: comparison type
     *      -0: exact value comparison
     *      -1: range comparison (only for integers, e.g. values 2,5,7 with lines 11,12,13 will operated under ranges
     *      2-5, 5-7 and 7+, which means a value of 4 of the variable will set the pointer to 11)
     *      -2: multiple variable fitting (checks for one case for multiple variables, that is, whether they are all
     *      equal to the values provided in param 3. Param 4 will contain two lines, the first is for if all fit, the
     *      second for otherwise --e.g. a,b,c varNames, 0,0,1 varTypes, true, false, 3 varValues, 11,13 lines. If a==true,
     *      b==false and c==3, will point to line 11, otherwise point to 13.
     * param 3: the values of each case, separated by comma
     * param 4: the point-to line number for each case, separated by comma
     * 
     */

    public void conditionalSwitch(bool[] done, string[] prms)
    {
        string[] varTypez = prms[0].Split(',');
        int[] varTypes = new int[varTypez.Length];
        for (int t = 0; t < varTypez.Length; t++)
        {
            int.TryParse(varTypez[t], out varTypes[t]);
        }

        string varName = prms[1];

        int comparisonType;
        int.TryParse(prms[2], out comparisonType);

        string[] linez = prms[4].Split(',');
        int[] lines = new int[linez.Length];
        for(int n=0; n<linez.Length; n++)
        {
            int.TryParse(linez[n], out lines[n]);
        }

        string[] values = prms[3].Split(',');

        int goToLine = -1;

        if (comparisonType == 0)
        {
            switch (varTypes[0])
            {
                case 0:
                    if (!Global.boolVariables.ContainsKey(varName))
                    {
                        Debug.Log("variable with name " + varName + " not found");
                        break;
                    }
                    bool bValue = Global.boolVariables[varName];

                    bool[] bools = new bool[values.Length];
                    for (int b = 0; b < values.Length; b++)
                    {
                        bool.TryParse(values[b], out bools[b]);
                        if (bValue == bools[b])
                        {
                            //match
                            goToLine = lines[b];
                            break;
                        }
                    }
                    break;
                case 1:
                    if (!Global.intVariables.ContainsKey(varName))
                    {
                        Debug.Log("variable with name " + varName + " not found");
                        break;
                    }
                    int iValue = Global.intVariables[varName];

                    int[] ints = new int[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        int.TryParse(values[i], out ints[i]);
                        if (iValue == ints[i])
                        {
                            //match
                            goToLine = lines[i];
                            break;
                        }
                    }
                    break;
                case 2:
                    if (!Global.stringVariables.ContainsKey(varName))
                    {
                        Debug.Log("variable with name " + varName + " not found");
                        break;
                    }
                    string sValue = Global.stringVariables[varName];

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (sValue.Equals(values[i]))
                        {
                            //match
                            goToLine = lines[i];
                            break;
                        }
                    }
                    break;
                default:
                    Debug.Log("unclear variable type for conditional switch");
                    break;

            }//end switch statement

        }
        else if (comparisonType == 1) //def int; ranges
        {
            if (!Global.intVariables.ContainsKey(varName))
            {
                Debug.Log("variable with name " + varName + " not found");
            }
            int iValue = Global.intVariables[varName];

            int[] ints = new int[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                int.TryParse(values[i], out ints[i]);
                if (i > 0 && (iValue < ints[i] && iValue >= ints[i - 1])) //check if value is within previous range
                {
                    //match
                    goToLine = lines[i - 1];
                    break;
                }
            }
            if (iValue > ints[values.Length - 1]) //value bigger than the ranges, go to the last assigned line 
            {
                goToLine = lines[values.Length - 1];
            }
        }
        else if (comparisonType == 2) //multi-variables
        {
            bool ok = true;

            string[] varNames = varName.Split(',');
            for (int v = 0; v < varNames.Length; v++)
            {
                switch (varTypes[v]) {
                    case 0:
                        if (!Global.boolVariables.ContainsKey(varNames[v]))
                        {
                            Debug.Log("variable with name " + varNames[v] + " not found");
                        }

                        bool bValue = Global.boolVariables[varNames[v]];
                        bool b;
                        bool.TryParse(values[0], out b);
                        if (b != bValue) ok = false;
                        break;

                    case 1:
                if (!Global.intVariables.ContainsKey(varNames[v]))
                {
                    Debug.Log("variable with name " + varNames[v] + " not found");
                }

                        int iValue = Global.intVariables[varNames[v]];
                        int i;
                        int.TryParse(values[0], out i);
                        if (i != iValue) ok = false;

                        break;
                    case 2:
                        if (!Global.stringVariables.ContainsKey(varNames[v]))
                        {
                            Debug.Log("variable with name " + varNames[v] + " not found");
                        }

                        string sValue = Global.stringVariables[varNames[v]];
                        string s = values[0];
                        if (!s.Equals(sValue)) ok = false;

                        break;
                }//end switch
            }

            goToLine = lines[(ok ? 0 : 1)];
        }

            if (goToLine == -1)
        {
            Debug.Log("conditional switch failed to match with any case");
        }
        else
        {
            gameFlow.setPointer(goToLine);
            Debug.Log("conditional switch setting pointer to " + goToLine);
        }


        done[0] = true;
    }

    /*
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

    /*
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
