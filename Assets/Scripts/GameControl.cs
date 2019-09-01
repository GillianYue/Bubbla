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
    public GameObject mainCanvas;
    public Camera mainCamera;
    public GameObject Hs_Holder, Ballz; //ballz is the empty parent GO holding all paintballs
    //Hs_holder likewise for hearts

    public GameObject[] hearts, gadgets, icons; //gadgets being hearts_container, bulletGauge, etc.
    //icons being interactive UI that if pressed, should avoid any gameplay logic being carried out
    public GameObject HeartVFX, aim;
    public GameObject player;
    public GameObject GameOverC;
    public CustomEvents customEvents;
    public EnemySpawner eSpawner;
    public PaintballSpawner pSpawner;
    public GameFlow gFlow;
    public BGMover[] backgrounds;

    public GameObject fixedBG;
    public Backpack backpack;

    private float pressTime=-1;
    public bool ckTouch = true; //if false, won't check for user's touch input in GAME

    // Use this for initialization
    void Start () {
        
        GameOverC.SetActive (false);
        //player.GetComponent<Player> ().enabled = false;
        //gadgets are GOs like life container that are needed in game play but not in DLG mode
        foreach (GameObject g in gadgets) {
            g.SetActive (false);
        }

        StartCoroutine (StartGame());

        //correct sizing for the backgrounds of this level, fixed or non-fixed
        Global.resizeSpriteToRectX(fixedBG);

        //setting mask to the right dimension
        GameObject BGMask = GameObject.FindWithTag("BGMask");
        if (BGMask != null)
        {

            Vector3 sSize = BGMask.GetComponent<SpriteMask>().sprite.bounds.size;
            var ratioX = BGMask.GetComponent<RectTransform>().rect.width / sSize.x;
            var ratioY = BGMask.GetComponent<RectTransform>().rect.height / sSize.y;
            Vector3 scale = new Vector3(ratioX, ratioY-0.04f, 1);
            BGMask.GetComponent<RectTransform>().localScale = scale;
        }

    }

    // Update is called once per frame
    void Update () {

        switch(gFlow.currMode){
        case GameFlow.Mode.DLG:
        //mouseclick
            if (Input.GetMouseButtonDown (0)) {

                    if (gFlow.checkCurrentLineDone ()) {//time to move pointer and print new line
                        StartCoroutine (gFlow.movePointer ()); //move to next line in script
                    } else if (gFlow.Skippable ()) { //skip time
                        gFlow.skipDLG ();
                    }
                    return; //if so, no need to proceed
            }
            break;

        case GameFlow.Mode.GAME:
                if(ckTouch)
            if (Input.GetMouseButtonDown (0)) {
                foreach (Transform child in Ballz.transform) {//
                    Vector2 item = Global.WorldToScreen(child.GetComponent<
                        RectTransform>().position); //screen

                        //checks if clicking on any paintball
                        if (child.CompareTag("Paintball") &&
                           Global.touching(new Vector2(Input.mousePosition.x,
                               Input.mousePosition.y), //screen 
                               item, //screen
                                     //gets the radius of paintball on screen
                               child.GetComponent<SpriteRenderer>().sprite.rect.width * 
                               Global.WTSfactor.x * child.transform.localScale.x,
                               child.GetComponent<SpriteRenderer>().sprite.rect.height *
                               Global.WTSfactor.y * child.transform.localScale.y
                               )) {
                            /**if yes, terminate the method early so that
                        bullet wouldn't be launched; the interaction with
                        paintball will be handled by PaintballSpawner
                        **/

                        //checking if bulletgauge is full (the process takes in paint automatically)
                        if (player.GetComponent<Player> ().addPaint 
                        (child.GetComponentInParent<PaintballBehavior> ().getColor ())) {
                            //generates effect
                            child.GetComponentInParent<PaintballBehavior> ().getsAbsorbed ();
                        } 
                        return;
                    } else if (child.CompareTag ("Potion") &&
                              Global.touching (new Vector2(Input.mousePosition.x,
                               Input.mousePosition.y), //screen 
                               item, //screen
                                     //gets the radius of paintball on screen
                               child.GetComponent<SpriteRenderer>().sprite.rect.width *
                               Global.WTSfactor.x * child.transform.localScale.x,
                               child.GetComponent<SpriteRenderer>().sprite.rect.height *
                               Global.WTSfactor.y * child.transform.localScale.y)) {

                        child.GetComponent<ItemBehav> ().getsAbsorbed ();
                        player.GetComponent<Player> ().cure 
                    (child.GetComponent<ItemBehav> ().getCuringPotency ());

                        return;
                    }
                }//end check paintballs

                foreach (GameObject i in icons)
                        {
                            Vector2 icon = Global.WorldToScreen(i.GetComponent<
                            RectTransform>().position); //screen

                            if (Global.touching(new Vector2(Input.mousePosition.x,
                                Input.mousePosition.y), //screen 
                          icon, //screen
                    i.GetComponent<Image>().sprite.rect.width * Global.WTSfactor.x * i.transform.localScale.x,
                    i.GetComponent<Image>().sprite.rect.height * Global.WTSfactor.y * i.transform.localScale.y
                    ))
                            {
                                //if code reaches here, means that one icon is pressed
                                return; //prevent aim from being created
                            }
                        }

                //if code reaches here, treat as starting to press down, as opposed to a light tap on
                //paintball/potion
                GameObject a = aim; //GO with the aiming sprite

                    a = Instantiate (a, Global.ScreenToWorld(Input.mousePosition,
                    5), 
                    player.transform.rotation) as GameObject;
                    a.transform.SetParent(player.transform);

                    a.GetComponent<Animator> ().SetBool ("Focused", false);

                pressTime = Time.time;
            }

            if (pressTime != -1) { //if is currently pressing
                Vector2 mouseWorld = Global.ScreenToWorld (Input.mousePosition);

                Transform aimy = player.transform.Find ("Aim(Clone)");
                    if (aimy != null)
                    {
                        aimy.position = new Vector3(
                            mouseWorld.x, mouseWorld.y, aimy.position.z); //put aim at pressed position

                        if ((Time.time - pressTime) > 0.7 &&
                           !aimy.GetComponent<Animator>().GetBool("Focused"))
                        {
                            aimy.GetComponent<Animator>().SetBool("Focused", true);
                        }
                    }
                //start pointin cannon
                Vector3 mouse = Input.mousePosition;
                Vector3 direction = mouse -
                                   Camera.main.WorldToScreenPoint (player.transform.position);
                float tan = direction.x / direction.y;
                float angle = Mathf.Atan (tan); 
                //*************THE ANGLE IS HERE*************

                float dgAngle = -1 * (angle * Mathf.Rad2Deg); //convert from radian to dgr
                dgAngle += 45; //to account for the original angle of cannon the sprite
                if (direction.y < 0) {
                    dgAngle += 180;
                }
                if (direction.x < 0) {
                    player.GetComponent<Animator> ().SetBool ("left", true);
                } else {
                    player.GetComponent<Animator> ().SetBool ("left", false);
                }
                //even if no bullet, cannon points to the clicked direction
                Vector3 temp = player.transform.Find ("Cannon").localEulerAngles;
                temp.z = dgAngle;
                player.transform.Find ("Cannon").localEulerAngles = temp;
                //end pointin cannon
                player.GetComponent<Animator> ().SetBool ("aiming", true);


                if (Input.GetMouseButtonUp (0)) { //release
                
                    if(aimy != null)
                    Destroy (aimy.gameObject);
                
                    float pressedLength = (Time.time - pressTime);
                    pressTime = -1; //set to -1 so that we know it's not pressing

                    //if code reaches here, it shows that empty space was clicked, SHOOT BULLET

                    if (pressedLength < 0.7) {
                        //normal shooting

                        //the launchBullet function will check if there's bullet
                        player.GetComponent<Player> ().launchBullet (direction, angle);


                    } else if (pressedLength < 1.5) {
                        //second stage
                        player.GetComponent<Player> ().launch2Bullets (direction, angle);

                    } else {
                        //triple combo shoot
                    }
                    
                }//end release
            } else {//end pressing
                player.GetComponent<Animator> ().SetBool ("aiming", false);
            }
            break;

        default:
            break;

        }//end switch

    }

    IEnumerator StartGame(){ 
        while (!gFlow.checkLoadDone()) {//wait till csv's loaded
            yield return null;
        }

        int tempPT = -1; //temp pointer, is passive, updates as gFlow pointer updates
        do{//once code gets here, should be ready to start gameFlow
            if(tempPT != gFlow.getPointer()){ //avoid redundant work; only rerender if changed
                StartCoroutine(gFlow.processCurrentLine());
            tempPT = gFlow.getPointer(); 
            }
            yield return new WaitForSeconds(0.2f); //essentially check dialogue status every one s
        }while(!gFlow.checkIfEnded()); //as long as there's still something to be done
            
    }
        
    public void startEnemyWaves(int[] w, int[] e){
        bool[] esDone = new bool[1]; //a bool[] shared to ps and es so that they can be in sync
       pSpawner.StartSpawn (esDone);  //will end when enemySpawnerDone
        eSpawner.StartSpawn(gFlow, w, e, esDone);
        foreach(BGMover m in backgrounds){
            m.StartScrolling ();
        }
        player.GetComponent<Player> ().enabled = true; //generation of heart, updates, etc. 
        foreach (GameObject g in gadgets) {
            g.SetActive (true); //make bullet gauge and life container show
        }
    }

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
                (HeartVFX, Hs_Holder.transform.GetChild (0).position, 
                    Hs_Holder.transform.GetChild (0).rotation);
                Destroy (Hs_Holder.transform.GetChild (0).gameObject);
            }
        }
                
            }

    public void pauseGame()
    {
        Time.timeScale = 0.0f; //stop gameplay
        foreach (BGMover m in backgrounds)
        {
            m.stopBGScroll();
        }
    }

    public void resumeGame()
    {
        Time.timeScale = 1.0f; //resume gameplay
        foreach (BGMover m in backgrounds)
        {
            m.resumeBGScroll();
        }
    }

}