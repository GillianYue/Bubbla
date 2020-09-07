using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

/**
 * script for csv file based game levels, linear flow following a pointer variable
 */
public class GameFlow : MonoBehaviour {

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;
    [Inject(InjectFrom.Anywhere)]
    public CustomEvents customEvents;
    [Inject(InjectFrom.Anywhere)]
    public Player player;
    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;

    [Inject(InjectFrom.Anywhere)]
    public EnemyLoader enemyLoader;
    [Inject(InjectFrom.Anywhere)]
    public CharacterLoader characterLoader;
    [Inject(InjectFrom.Anywhere)]
    public ItemLoader itemLoader;

    //IVS stands for investigate
    public enum Mode { DLG, GAME, IVS, END };
    public Mode currMode;

    //row 0 are names of the categories
    private int pointer = 2; //indicates which line of script the game is at; starting at 2 b/c of format
    private bool pointerCheck = true;
    private bool[] loadDone;  //this bool is only for the level progress file, not everything
    public bool canMovePointer = true; //canMovePointer MUST be set to true on start
    private int specialGoToLine = -1; //if set to anything other than -1, click in DLG will go to this line rather than increment

    public TextAsset DlgCsv; //dialogue file for a specific level
    private string[,] data; //double array that stores all info of this level

    public IEnumerator processLineInstance;


    public ArrayList specialDLGstarts, specialDLGends; //arraylist of ints

    void Start() {

        specialDLGstarts = new ArrayList(); specialDLGends = new ArrayList();

        loadDone = new bool[1];
        bool[] parseDone = new bool[1];
        StartCoroutine(LoadScene.processCSV(loadDone, DlgCsv, setData, parseDone, true));

    }


    public bool checkIfEnded() { //checks if ended
        return (currMode.Equals(Mode.END)); //the "big" done
    }

    public bool checkGameFlowLoadDone()
    {
        return (loadDone[0]);
    }

    public void setData(string[,] d, bool[] parseDone)
    {
        data = d;
        StartCoroutine(parseDLGcsvData(parseDone));
    }

    IEnumerator parseDLGcsvData(bool[] parseDone)
    {
        yield return new WaitUntil(() => (data != null));
        int numRows = data.GetLength(1); bool toggle = false;
        for (int r = 1; r < numRows; r++) //-1 because title row doesn't count
        {
               if(data[0, r].Equals("SPECIAL"))
            {
                if (!toggle)
                {
                    specialDLGstarts.Add(r + 1); //line after starting "SPECIAL"
                }
                else
                {
                    specialDLGends.Add(r - 1);
                }
                toggle = !toggle;
            }
        }

        parseDone[0] = true;
    }

    public void processCurrentLine()
    {
        if (processLineInstance != null)
        {
            StopCoroutine(processLineInstance);
        }

        processLineInstance = processCurrentLineCoroutine();
        StartCoroutine(processLineInstance);
    }

    /**
     * this function is called in GameControl. Thus, GameFlow is not in charge of 
     * calling process itself, only updates the pointer. When GameControl sees a change
     * in pointer, this function is invoked.    
     */   
    private IEnumerator processCurrentLineCoroutine() { //current being where the pointer is

        if (data[0, pointer] != "") {//check if story done (if yes move on to actual game play)
            if (data[0, pointer].Equals("SPECIAL")) //this will only happen to the ending SPECIAL tags
                /* customEvents is in charge of calling levelScript, which is 
                 * in charge of using certain SPECIAL blocks (with specific index)
                 * 
                 * e.g. levelscript.levelScriptEvent(3) calls a coroutine which uses SPECIAL block 5 and SPECIAL block 6              
                 */
            {
                int ln;
                int.TryParse(data[1, pointer], out ln);
                if (ln != 0) setPointer(ln); else Debug.LogError("SPECIAL block end didn't specify goto line number");
                yield break; //breaks out of the coroutine
            }
            if (currMode == Mode.DLG) dialogue.disableDialogueBox(); //if transitioning from dlg to others
                currMode = (Mode)System.Enum.Parse(typeof(Mode), data[0, pointer]); ///////////the actual changing of mode
            if (currMode == Mode.DLG) { dialogue.enableDialogueBox();
                gameControl.bgManager.setBackgroundsActive(false); //pause
            } //if the new mode is actually dlg
            else if(currMode == Mode.IVS)
            {
                player.setNavigationMode(Player.Mode.TOUCH);
            }
            else if (currMode == Mode.GAME)
            {
                player.setNavigationMode(Player.Mode.ACCL);
            }

            if (currMode != Mode.GAME)
            {
                try
                {
                    GameObject go = gameControl.player.transform.Find("Aim(Clone)").gameObject;
                    if (go != null)
                    {
                        Destroy(go);
                        gameControl.player.GetComponent<Animator>().SetBool("aiming", false);
                    }
                }
                catch {} //supress error here
             
            }
//            print("Mode changed to " + currMode + " at line "+pointer);
        }

        switch (currMode) {
            case Mode.DLG: //still in dialogue mode
                dialogue.displayOneLine(data[1, pointer], data[2, pointer], data[3, pointer], data[4, pointer], data[5, pointer],
                    data[7, pointer], data[8, pointer], data[9, pointer], data[10, pointer]); //
                break;

            case Mode.GAME:
                Global.scaleRatio = (int) GameObject.FindWithTag("Player").transform.localScale.x;
                if (data[1, pointer].Equals("99")) {
                    //special customized event
                    int indexCE;
                    int.TryParse(data[2, pointer], out indexCE);
                    //for now we assume there's at most 5 parameters to a custom event
                    string[] parameters = new string[5];
                    for (int p = 0; p < 5; p++)
                    {
                        parameters[p] = data[p+3, pointer];
                    }
                    customEvents.customEvent(indexCE, parameters);
                } else { //spawn wave
                    string[] waves = data[1, pointer].Split(',');
                    string[] enemies = data[2, pointer].Split(',');
                    string[] colorModes = data[3, pointer].Split(',');
                    string[] colorModeWeights = data[4, pointer].Split(',');

                    bool loop = data[5, pointer].Equals("TRUE");

                    List<(PaintballBehavior.ColorMode, float)> tempList = new List<(PaintballBehavior.ColorMode, float)>();
                    for(int i=0; i < colorModes.Length; i++)
                    {
                        string cm = colorModes[i], cw = colorModeWeights[i];
                        Enum.TryParse(cm, true, out PaintballBehavior.ColorMode col);
                        float.TryParse(cw, out float colW);
                        tempList.Add((col, colW));
                    }
                    PaintballBehavior.standards = tempList;

                    int[] wv, em;
                    wv = System.Array.ConvertAll<string, int>(waves, int.Parse);
                    em = System.Array.ConvertAll<string, int>(enemies, int.Parse);
                    gameControl.startEnemyWaves(wv, em);
                }
                break;
            case Mode.IVS:
                int indexIVS = -1;
                int.TryParse(data[1, pointer], out indexIVS);
                switch(indexIVS)
                {
                    case 0: //setup; the current version of code requires all setup be done in one line b/c assignment of string[]
                        string[] IDs = data[2, pointer].Split(',');
                        string[] linez = data[3, pointer].Split(',');
                        int[] lines = new int[linez.Length];

                        GameObject[] ivsGOs = new GameObject[linez.Length];
                        for(int l=0; l<linez.Length; l++)
                        {
                            int.TryParse(linez[l], out lines[l]);
                            ivsGOs[l] = customEvents.findByIdentifier(IDs[l]); //getting the ivs gameobject by id
                            ivsInteractable ivs = ivsGOs[l].AddComponent<ivsInteractable>();
                            ivs.setindexInArray(l);
                            ivs.setIvsGoToLine(lines[l]);
                        }

                        gameControl.interactables = ivsGOs;
                        break;
                    case 1: //conditional looping for investigate
                        //NOTE: only break after condition is fulfilled, pointer will increment outside the switch statement
                        int varType;
                        int.TryParse(data[2, pointer], out varType);

                        string varName = data[3, pointer];

                        string varValueS = data[4, pointer]; int goalValueI=-10000; bool goalValueB=false; string goalValueS=null;

                        switch (varType)
                        {
                            case 0:
                                bool.TryParse(varValueS, out goalValueB);
                                break;
                            case 1:
                                int.TryParse(varValueS, out goalValueI);
                                break;
                            case 2:
                                goalValueS = varValueS;
                                break;
                        }

                        yield return new WaitUntil(() => { //loops until condition meets
                            switch (varType)
                            {
                                case 0:
                                    if (!Global.boolVariables.ContainsKey(varName))
                                    {
                                        //var not created yet
                                        return false;
                                    }
                                    bool bValue = Global.boolVariables[varName];

                                    return (bValue == goalValueB);

                                case 1:
                                    if (!Global.intVariables.ContainsKey(varName))
                                    {
                                        //var not created yet
                                        return false;
                                    }
                                    int iValue = Global.intVariables[varName];
                                    return (iValue == goalValueI);

                                case 2:
                                    if (!Global.stringVariables.ContainsKey(varName))
                                    {
                                        //var not created yet
                                        return false;
                                    }
                                    string sValue = Global.stringVariables[varName];
                                    return (sValue.Equals(goalValueS));

                                default:
                                    Debug.Log("unclear variable type for ivs");
                                    return false;

                            }//end switch statement
                        });

                        break;
                    default:
                        Debug.Log("error in IVS: " + data[1, pointer] + " not a valid number");
                        break;
                }
                incrementPointer(); //this is b/c gameControl can not increment pointer in IVS mode, has to happen here

                break;

            case Mode.END:
                print("level ended!");
                break;

            default: break;
        }//end switch

        processLineInstance = null; //resetting the reference now that it's done
    }


    //pointer check is a buffer to prevent multiple pointer addition from a single click
    //also movePointer is not called here, it's called in GameControl, the global control
    //this should only be used for dialogues, not in other modes
    //in other modes, incrementPointer, which is instantaneous, should be used
    public IEnumerator movePointer() {
        if (canMovePointer && dialogue.checkCurrentLineDone() && pointerCheck) {
            pointerCheck = false;
            if (specialGoToLine == -1)
            { //normally increment
                pointer++;
            }
            else
            {
                pointer = specialGoToLine;
                specialGoToLine = -1; //reset
            }
            yield return new WaitForSeconds(0.2f);
            pointerCheck = true;
        }
    }

    public void incrementPointer()
    {
        if (canMovePointer)
        pointer++; 
    }

    public void setPointer(int p)
    {
        pointer = p;
    }

    public void setPointerToSpecial(int index)
    {
        int a = (int)specialDLGstarts[index];
        setPointer((int)specialDLGstarts[index]);

    }

    public int getPointer() {
        return pointer;
    }

    public void setSpecialGoToLine(int line)
    {
        specialGoToLine = line;
    }
}
