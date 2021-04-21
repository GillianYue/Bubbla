using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


/**
 * GameControl is a global level game control script. It is in charge of starting the game
 * and switching between different stages of the game. It talks with GameFlow, which is a
 * game logic script on a more micro level, to run each level based on inputs. 
 * 
 * GameControl does not have access to the CSV scripts. It only has access to some of the
 * more permanent elements, like player, and the game play UI. It has access to an 
 * EnemySpawner, which is in charge of spawning the enemies. 
 * 
 * User input (touch screen) is also checked here in Update. Whenever there is input, the
 * GameControl global script sends the information to sub logic scripts correspondingly. 
 * It then acts on a macro level according to the feedback of those sub scripts.
 * For example, mode of the game is being determined in this way:
 * Mode transitions are written in csv files available in GameFlow;
 * Each line is performed out using a function in GameFlow (which is called here globally);
 * GameFlow has a pointer to know which line it's supposed to perform;
 * GameControl notifies GameFlow when it's time to update the pointer (user clicks screen);
 * when Mode changes (done in processLine in gameflow) GameControl acts accordingly globally;
 * if in DLG it only checks for clicks;
 * if in GAME it activates UI, wakes EnemySpawner, checks paint collection, etc.
 */
public class GameControl : MonoBehaviour {
    //screen: 320x480
    //world to screen factor: 1:35.6
    public GameObject mainCanvas, vfxCanvas, UICanvas; //vfxCanvas has a sorting order of 99 since it should be above everything
    public Camera mainCamera, uiCamera;
    public GameObject Hs_Holder, Ballz; //ballz is the empty parent GO holding all paintballs
    //Hs_holder likewise for hearts

    public GameObject[] gadgets, icons, interactables; //gadgets being hearts_container, bulletGauge, etc.
    //icons being interactive UI that if pressed, should avoid any gameplay logic being carried out
    public GameObject player;

    //prefabs
    private GameObject HeartPopVFX, aim;
    private GameObject[] hearts;

    //retrieved from player
    [Inject(InjectFrom.Anywhere)]
    public Player p;

    public GameObject GameOverC;

    [Inject(InjectFrom.Anywhere)]
    public BGManager bgManager;
    [Inject(InjectFrom.Anywhere)]
    public PrefabHolder prefabHolder;

    [Inject(InjectFrom.Anywhere)]
    public CustomEvents customEvents;
    [Inject(InjectFrom.Anywhere)]
    public EnemySpawner eSpawner;
    [Inject(InjectFrom.Anywhere)]
    public PaintballSpawner pSpawner;

    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;
    //Manually assign the right one
    public Dialogue dialogue;

    [Inject(InjectFrom.Anywhere)]
    public LoadScene loadScene;

    [Inject(InjectFrom.Anywhere)]
    public Backpack backpack;

    [Inject(InjectFrom.Anywhere)]
    public GameTestBehavior testBehav;

    [Inject(InjectFrom.Anywhere)]
    public TitleController titleControl;

    [Inject(InjectFrom.Anywhere)]
    public CameraFollow camFollow;

    private float pressTime=-1;
    public bool ckTouch = true;
    private float touchCooler = 0.5f; //half a second before reset; used for checking for fast double tap
    private int touchCount = 0; 
    public enum GameMode { QUEST_SELECT, GAME, TRAVEL }; //scene type
    public GameMode sceneType;

    public AudioSource bgmSource;

    void Awake()
    {
        int currScene = SceneManager.GetActiveScene().buildIndex;

        switchSceneReupdateReferences(currScene);

        //DontDestroyOnLoad(mainCanvas);
        DontDestroyOnLoad(UICanvas);
        DontDestroyOnLoad(mainCamera);
        DontDestroyOnLoad(uiCamera);
    }

    void Start () {

        if (GameOverC) GameOverC.SetActive(false);
        if (vfxCanvas) vfxCanvas.SetActive(false); //to prevent blocking of buttons

        StartCoroutine(StartGame());
    }

    void Update () {

/*        if (sceneType == GameMode.GAME) 
        {*/
            switch (gameFlow.currMode)
            {
                case GameFlow.ScriptMode.DLG:
                    //mouseclick
                    if (Input.GetMouseButtonDown(0) && ckTouch)
                    {

                        if (dialogue.checkCurrentLineDone())
                        {//time to move pointer and print new line
                            StartCoroutine(gameFlow.movePointer()); //move to next line in script
                        }
                        else if (dialogue.Skippable())
                        { //skip time
                            dialogue.skipDLG();
                        }
                        return; //if so, no need to proceed
                    }
                    break;

                case GameFlow.ScriptMode.GAME:

                        if (touchCooler > 0) { touchCooler -= 1 * Time.deltaTime; }
                        else { 
                            touchCount = 0; //reset 
                        }

                    if (Input.GetMouseButtonDown(0) && ckTouch) //on first press/click
                        {

                            if (p.selectGauge(new Vector2(Input.mousePosition.x, Input.mousePosition.y)))
                            {
                                p.freezeMoveToBriefly();

                                if (touchCooler > 0 && touchCount == 1)
                                {
                                    //Has double tapped
                                    p.removePaint(p.whichGauge(Input.mousePosition));
                                }
                                else
                                {
                                    touchCooler = 0.3f;
                                    touchCount += 1;
                                }

                                return; //do not check for attack logic
                            }

                            foreach (GameObject i in icons) //check for UI trigger
                            {
                                Vector2 icon = Global.WorldToScreen(mainCamera, i.GetComponent<
                                RectTransform>().position); //screen

                                if (Global.touching(new Vector2(Input.mousePosition.x,
                                    Input.mousePosition.y), //screen 
                                    icon, //screen
                                    i.GetComponent<Image>().sprite.rect.width * i.transform.localScale.x,
                                    i.GetComponent<Image>().sprite.rect.height * i.transform.localScale.y
                                    ))
                                {
                                    //if code reaches here, means that one icon is pressed
                                    return; //prevents held interactions
                                }
                            }

                        }//end first press check

                        if (Input.GetMouseButton(0) && ckTouch) //if held
                            {


                            p.moveTo(Input.mousePosition.x, Input.mousePosition.y);
                            p.fireAtRate(Input.mousePosition);

                            if (Input.GetMouseButton(1)) //second touch; do angle here
                            {
                                Debug.Log("second touch");
                            }
                            else
                            {

                            }

                        }
                        else
                        {
                            //Debug.Log("Not held");
                        }


                    break;
                case GameFlow.ScriptMode.IVS:
                print("IVS updating");
                    if (Input.GetMouseButtonDown(0) && ckTouch)
                    {
                        foreach (GameObject i in interactables)
                        {
                            Vector2 go = Global.WorldToScreen(mainCamera, i.transform.position); //screen

                            if (Global.touching(new Vector2(Input.mousePosition.x,
                                Input.mousePosition.y), //screen 
                                go, //screen
                                i.GetComponent<SpriteRenderer>().sprite.rect.width * i.transform.localScale.x,
                                i.GetComponent<SpriteRenderer>().sprite.rect.height * i.transform.localScale.y
                              ))
                            {
                                //if code reaches here, means that one sprite is clicked, get the ivs-related script & call function
                                interactable itr = i.GetComponent<interactable>();
                                if (itr.closeEnough(player)) //if within interactable distance
                                {
                                    // player.GetComponent<Player>().setNavigationMode(Player.Mode.FREEZE);
                                    //gFlow.setPointer(ivs.getIvsGoToLine());
                                    //TODO
                                    itr.interact();
                                    Debug.Log("interacting with GO");
                                    //non-linear way of displaying message
                                }
                                return;
                            }
                        }//end foreach

                        p.startNudge(); //notifies player to start moving toward dest
                    }
                    else if (Input.GetMouseButtonUp(0) && ckTouch)
                    {
                        p.stopNudge();
                    }

                    break;
                default:
                    break;

            }//end switch for gameFlow state
/*        }
        else if(sceneType == GameMode.TRAVEL)
        {




        }
        else if (sceneType == GameMode.QUEST_SELECT)
        {
            //in title scene


        }*/

    }

    IEnumerator StartGame(){

        yield return new WaitUntil(() => loadScene.isAllLoadDone());

        if (sceneType == GameMode.GAME)
        {
            int tempPT = -1; //temp pointer, is passive, updates as gFlow pointer updates
            int gfP; //game flow pointer
            do
            {//once code gets here, should be ready to start gameFlow
                if (tempPT != (gfP = gameFlow.getCurrentLineNumber()))
                { //avoid redundant work; only rerender if changed
                    tempPT = gfP;
                    gameFlow.processCurrentLine();
                }
                yield return new WaitForSeconds(0.1f); //essentially check dialogue status every one s
            } while (!gameFlow.checkIfEnded()); //as long as there's still something to be done
        }else if(sceneType == GameMode.QUEST_SELECT)
        {
            titleControl.startTitle();
        }
    }
        
    public void startEnemyWaves(int[] w, int[] e, int[] wvwt, float[] intv, int[] spnm){
        bool[] esDone = new bool[1]; //a bool[] shared to ps and es so that they can be in sync
       pSpawner.StartSpawn (esDone);  //will end when enemySpawnerDone
        eSpawner.StartSpawn(gameFlow, w, e, wvwt, intv, spnm, esDone);
        bgManager.setBackgroundsActive(true);
        player.GetComponent<Player> ().enabled = true; //generation of heart, updates, etc. 
        foreach (GameObject g in gadgets) {
            g.SetActive (true); //make bullet gauge and life container show
        }
    }

    public void hideAllIcons() { foreach (GameObject icon in icons) { icon.SetActive(false); } }

    public void showAllIcons() { foreach (GameObject icon in icons) { icon.SetActive(true); } }

    public void gameOver(){
        customEvents.levelScript.gameOver(GameOverC); //may differ based on levelscript
    }

    public void restart(){
        player.GetComponent<Player> ().respawn ();
        Time.timeScale = 1;
        SceneManager.LoadScene (0);
    }

    public void updateLife(int life){
        genHearts (life);
    }

    /// <summary>
    /// updates references to objects, while also (de)activating things appropriately
    /// 
    /// NOTE: does not include dependency injection's ResolveScene(), needs to be called manually if needed. See Global.LoadTravelSceneWithSiteCoroutine()
    /// </summary>
    /// <param name="scene_number"></param>
    public void switchSceneReupdateReferences(int scene_number)
    {
        if (p == null) p = FindObjectOfType<Player>();
        if (player == null) player = p.gameObject;
        if (mainCanvas == null) mainCanvas = GameObject.FindWithTag("Canvas");
        if (UICanvas == null) UICanvas = GameObject.FindWithTag("UICanvas");
        if (mainCamera == null) mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        if (uiCamera == null) uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();

        if (prefabHolder == null) prefabHolder = FindObjectOfType<PrefabHolder>();
        if (vfxCanvas == null) { vfxCanvas = GameObject.FindWithTag("VfxCanvas"); }
        if (dialogue == null) dialogue = GameObject.FindGameObjectWithTag("Dialogue").GetComponent<Dialogue>();

        switch (scene_number)
        {
            case Global.TITLE_SCENE_NUMBER:
                sceneType = GameMode.QUEST_SELECT;

                break;
            case Global.GAME_SCENE_NUMBER:
                sceneType = GameMode.GAME;

                if(Hs_Holder==null) Hs_Holder = GameObject.Find("HeartsHolder"); //////////TODO
                if(Ballz==null) Ballz = mainCanvas.transform.Find("Ballz").gameObject;

                HeartPopVFX = prefabHolder.heartPop;
                hearts = prefabHolder.hearts;
                aim = prefabHolder.aim;

                //gadgets are GOs like life container that are needed in game play but not in DLG mode
                if (gadgets != null)
                    foreach (GameObject g in gadgets)
                    {
                        g.SetActive(false);
                    }

                //setting mask to the right dimension
                GameObject BGMask = GameObject.FindWithTag("BGMask");
                if (BGMask != null)
                {

                    Vector3 sSize = BGMask.GetComponent<SpriteMask>().sprite.bounds.size;
                    var ratioX = BGMask.GetComponent<RectTransform>().rect.width / sSize.x;
                    var ratioY = BGMask.GetComponent<RectTransform>().rect.height / sSize.y;
                    Vector3 scale = new Vector3(ratioX, ratioY - 0.04f, 1);
                    BGMask.GetComponent<RectTransform>().localScale = scale;
                }

                p.navigationMode = Player.NavMode.ACCL;
                break;
            case Global.TRAVEL_SCENE_NUMBER:
                sceneType = GameMode.TRAVEL;
                camFollow.active = true;

                p.navigationMode = Player.NavMode.TOUCH;
                break;
        }
    }

    /*
     * checks for existing hearts vs. life (num hearts there should be) and gens as needed
     * 
     * assumes the heartsHolder GO is provided (the one that has the rigidbodies of rects)
     *     
     */
    public void genHearts(int life){
        
        int dif = life - Hs_Holder.transform.childCount;

            
            if (dif > 0) {//needs to increase hearts sprites
            for (int c = 0; c < dif; c++) {
                Vector3 pos = Hs_Holder.transform.position + 
                    new Vector3 (Random.Range (-20f, 20f), 30, 0);
                //picks a random heart prefab out of the hearts prefab group
                GameObject tmpH = Instantiate (hearts [(int)(Random.Range (0.0f, 3.99f))], 
                                     pos, hearts [0].transform.rotation) as GameObject;
                tmpH.transform.parent = Hs_Holder.transform;
            }
            } else if (dif < 0) {//needs to destroy hearts sprites
            for (int c = 0; c > dif; c--) {
                Instantiate 
                (HeartPopVFX, Hs_Holder.transform.GetChild (0).position, 
                    Hs_Holder.transform.GetChild (0).rotation);
                Destroy (Hs_Holder.transform.GetChild (0).gameObject);
            }
        }
                
            }

    public void pauseGame()
    {
        Time.timeScale = 0.0f; //stop gameplay
        if(bgManager) bgManager.setBackgroundsActive(false);
    }

    public void resumeGame()
    {
        Time.timeScale = 1.0f; //resume gameplay
        if (bgManager) bgManager.setBackgroundsActive(true);
    }

    /*
     * a function best called in customized GameOver()s, 
     * it stops all spawner processes and clears existing enemies and pbs on field
     */
    public void endGame()
    {
        pSpawner.StopAllCoroutines(); //stop endCheck, stop any potential wave spawn processes
        eSpawner.StopAllCoroutines(); //stop endCheck and any enemySpawn

        string[] prms = new string[1];
        prms[0] = "0";
        customEvents.clearEnemiesOrPBs(new bool[1], prms);
        pSpawner.destroyAllpb();
    }

}