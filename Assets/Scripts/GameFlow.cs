﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour {

    public GameControl gameControl;
    public CustomEvents customEvents;

    private int pointer = 1; //indicates which line of script the game is at
    private bool lineDone = true, pointerCheck = true, skipping = false;
    private bool[] loadDone;
    public bool canSkip;

    public Text NAME, DIALOGUE;
    public GameObject character, dlgBox; //character is current character speaking
    public Image bgBox;

    public enum Mode { DLG, GAME, END };
    public Mode currMode;

    public Text animIndicator; //number to show which state the character is
    public bool animBool0, animBool1; //names of the two bool switches of the character's animator, e.g. talking, typing
    public string prevChaName; //to inform whether there's a need to set animator for current line

    public TextAsset DlgCsv; //dialogue file for a specific level
    private string[,] data; //double array that stores all info of this level
    public EnemyLoader enemyLoader;
    public CharacterLoader characterLoader;

    private string openTag, endTag; //those two variables are used for dialogue tag processing

    void Start() {
        loadDone = new bool[1];
        StartCoroutine(LoadScene.processCSV(loadDone, DlgCsv, setData));
        pointer = 1; //row 0 are names of the categories
                     //	character = gameObject.transform.Find ("Background").Find (
                     //		"SpriteBox").GetChild (0).gameObject;
    }


    public bool checkIfEnded() { //checks if ended
        return (currMode.Equals(Mode.END)); //the "big" done
    }

    public bool checkLoadDone() { //check if csv is loaded and dialogue is ready tb displayed
        return loadDone[0];
    }

    public bool checkCurrentLineDone() {
        return lineDone;
    }

    public void setData(string[,] d)
    {
        data = d;
    }

    /**
     * this function is called in GameControl. Thus, GameFlow is not in charge of 
     * calling process itself, only updates the pointer. When GameControl sees a change
     * in pointer, this function is invoked.    
     */   
    public IEnumerator processCurrentLine() { //current being where the pointer is

        if (data[0, pointer] != "") {//check if story done (if yes move on to actual game play)
            if (currMode == Mode.DLG) disableDialogueBox(); //if transitioning from dlg to others
                currMode = (Mode)System.Enum.Parse(typeof(Mode), data[0, pointer]); ///////////
            if (currMode == Mode.DLG) enableDialogueBox(); //if the new mode is actually dlg
            print("Mode changed to " + currMode);
        }

        switch (currMode) {
            case Mode.DLG: //still in dialogue mode

                //sprite fixes size of background square
                Global.resizeSpriteToDLG(character, character.transform.parent.gameObject);

                lineDone = false; //dialogue is shown one char at a time
                NAME.text = data[1, pointer];

                if (!prevChaName.Equals(NAME.text))
                {
                    int index = characterLoader.getIndex(NAME.text);
                    character.GetComponent<Animator>().runtimeAnimatorController = 
                        characterLoader.getAnimatorByIndex(index);
                    prevChaName = NAME.text;
                    bgBox.color = characterLoader.getColorByIndex(index);
                    Debug.Log("color loaded is :" + bgBox.color);
                }

                int SpriteNum;
                int.TryParse(data[3, pointer], out SpriteNum);

                float disp_spd;
                float.TryParse(data[4, pointer], out disp_spd); //converts string to int
                string[] store; ArrayList result = new ArrayList();
                int[] tags = GetFormattedText(DIALOGUE, data[2, pointer], result);
                store = result.ToArray(typeof(string)) as string[];
                Canvas.ForceUpdateCanvases();
                DIALOGUE.text = "";

                int special;
                int.TryParse(data[7, pointer], out special);

                if (special != 1) 
                { //1 is changing sprite in the middle of talking
                    setBoolParam(character, 0, true); //talking is param 0
                }

                setAnimBaseState(character, SpriteNum);
                /**
                 * the two params for special event could be int, could be int arrays, so to cover all 
                 * cases we create variables for all possibilities                
                 */
                int[] PARAM1, PARAM2, PARAM3;
                PARAM1 = new int[1]; PARAM2 = new int[1]; PARAM3 = new int[1];
                if (special != 0) //if there is special, parse
                {
                    parseDLGspecialParams(PARAM1, PARAM2, PARAM3);
                }
                //end parsing special param, start adding chars 

                int wordCount = 1; int[] paramPointer = new int[3]; //paramPointer[2] would be pointer for special #2
                for (int s = 0; s < store.Length; s++) {
                    for (int n = 0; n < store[s].Length; n++) {
                        //tagging is for tags in rich text, not a part of the special effects system
                        if(n == tags[0]) //we're at the first letter that needs to be tagged
                        {
                            DIALOGUE.text += openTag;
                        }else if(tags[0] != -1 && n > tags[0] && n <= tags[2])
                        {
                            DIALOGUE.text = DIALOGUE.text.Remove
                                (DIALOGUE.text.Length - (endTag.Length)); //remove temp ending tag
                        }

                        DIALOGUE.text += store[s][n]; //the actual adding of the char

                        if(store[s][n] == ' ') //new word
                        {
                            wordCount++;
                        }

                        if (tags[0]!=-1 && n >= tags[0] && n <= tags[2])
                        {
                            DIALOGUE.text += endTag; //add temp ending tag
                        }

                        switch (special)
                        {
                            case 1:
                                if(n == PARAM1[0])
                                {
                                    setAnimBaseState(character, PARAM2[paramPointer[1]]);

                                    if (paramPointer[1] < PARAM1.Length - 1)
                                    {
                                        paramPointer[1]++;
                                    }
                                }
                                break;
                            case 2:
                                if(wordCount == PARAM3[paramPointer[2]])
                                {
                                    setBoolParam(character, PARAM1[paramPointer[2]],
                                        (PARAM2[paramPointer[2]] == 1));
                                    if (paramPointer[2] < PARAM3.Length - 1)
                                    {
                                        paramPointer[2]++;
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        if (!skipping) {
                            yield return new WaitForSeconds(- 1.0333f * disp_spd + 1.1f);
                        }
                    }
                    if (!(s == (store.Length - 1))) {
                        DIALOGUE.text = ""; //reset and print the second section
                    }
                }
                //character.GetComponent<Animator>().SetBool("Talking", false);
                setBoolParam(character, 0, false); //talking is param 0
                lineDone = true;
                skipping = false;
                break;

            case Mode.GAME:
                Global.scaleRatio = (int)GameObject.FindWithTag("Player").transform.localScale.x;
                if (data[1, pointer].Equals("99")) {
                    //special customized event
                    int index;
                    int.TryParse(data[2, pointer], out index);
                    //for now we assume there's at most 5 parameters to a custom event
                    string[] parameters = new string[5];
                    for (int p = 0; p < 5; p++)
                    {
                        parameters[p] = data[p+3, pointer];
                    }
                    customEvents.customEvent(index, parameters);
                } else {
                    string[] waves = data[1, pointer].Split(',');
                    string[] enemies = data[2, pointer].Split(',');
                    string[] rgb = data[3, pointer].Split(',');
                    float mx_D;
                    float.TryParse(data[4, pointer], out mx_D);
                    int r, g, b;
                    int.TryParse(rgb[0], out r);
                    int.TryParse(rgb[1], out g);
                    int.TryParse(rgb[2], out b);
                    PaintballBehavior.standard = new Color(r, g, b);
                    PaintballBehavior.setMaxD(mx_D);
                    int[] wv, em;
                    wv = System.Array.ConvertAll<string, int>(waves, int.Parse);
                    em = System.Array.ConvertAll<string, int>(enemies, int.Parse);
                    gameControl.startEnemyWaves(wv, em);
                }
                break;

            case Mode.END:
                print("level ended!");
                break;

            default: break;
        }
    }

    public IEnumerator displayTitleDLG() { //ONLY applies to the special csv file of title

        //randomly chooses dialogue for bunny to say
        int ttlWeight = 0; int line = -1;
        for (int r = 1; r < data.GetLength(1); r++) {
            ttlWeight += int.Parse(data[4, r]);
        }
        float rdm = UnityEngine.Random.Range(0, ttlWeight);
        for (int r = 1; r < data.GetLength(1); r++) {
            rdm -= int.Parse(data[4, r]);
            if (rdm < 0) {
                line = r;
                break;
            }
        }

        //Assumes character is already given, TODO set this based on data
        Global.resizeSpriteToDLG(character, character.transform.parent.gameObject);

        int SpriteNum;
        int.TryParse(data[2, line], out SpriteNum);
        setAnimBaseState(character, SpriteNum);

        int disp_spd;
        int.TryParse(data[3, line], out disp_spd); //converts string to int
        string[] store; ArrayList result = new ArrayList();
        int[] tags = GetFormattedText (DIALOGUE, data[1, line], result);
        store = result.ToArray(typeof(string)) as string[];
        //Canvas.ForceUpdateCanvases();
        DIALOGUE.text = "";

        setBoolParam(character, 0, true); //talking is param 0
        setBoolParam(character, 1, true); //typing is param 1

        for (int s = 0; s < store.Length; s++) {
            for (int n = 0; n < store[s].Length; n++) {
                DIALOGUE.text += store[s][n];
                yield return new WaitForSeconds(0.02f * disp_spd + 0.05f); //TODO adjust
            }
            if (!(s == (store.Length - 1))) {
                DIALOGUE.text = ""; //reset and print the second section
            }
        }

        setBoolParam(character, 0, false); //talking is param 0
        setBoolParam(character, 1, false); //typing is param 1
    }


    /*
     * prevents long words at end of line from jumping to the next when rendering
     *     
     * adds the parsed sentences to ArrayList result one by one.
     *     
     * Checks if there are tags first. 
     * If yes, stores the positions of the opening and ending tags in an int array that's returned.
     * If no, the returned array[0] will be -1, indicating that there's no tags in this sentence.
     * 
     */
    private int[] GetFormattedText(Text textUI, string text, ArrayList result) {
    
        int[] tagPos = new int[4];

        if (text.Contains("<")) //check if there's tag in this sentence
        {
            tagPos[0] = text.IndexOf('<'); //the starting pos of the opening tag
            tagPos[1] = text.IndexOf('>'); //the ending pos of the opening tag

            int ot_length = tagPos[1] - tagPos[0] + 1;
            openTag = text.Substring(tagPos[0], ot_length);
            text =  text.Remove(tagPos[0], ot_length);

            tagPos[2] = text.IndexOf('<'); //the starting pos of the ending tag AFTER openTag is removed
            tagPos[3] = text.IndexOf('>'); //the ending pos of the ending tag
            endTag = text.Substring(tagPos[2], tagPos[3]-tagPos[2]+1);
            text = text.Remove(tagPos[2], tagPos[3] - tagPos[2] + 1);

        }
        else
        {
            tagPos[0] = -1;
        }


        string[] words = text.Split(' ');

        int width = Mathf.Abs(Mathf.FloorToInt(textUI.rectTransform.rect.width));
        int height = Mathf.Abs(Mathf.FloorToInt(DIALOGUE.rectTransform.rect.height));
        int space = GetWordSize(textUI, " ", textUI.font, textUI.fontSize);
        int charH = space + Mathf.CeilToInt(DIALOGUE.lineSpacing); //height of single char
        int charW = space; //treat character as little square
        int maxChar = (height / charH) * (width / charW) - 4; //in dlg box, -4 is for error (tested)

        string newText = string.Empty;
        int count = 0;
        for (int i = 0; i < words.Length; i++) {
            int size = GetWordSize(textUI, words[i], textUI.font, textUI.fontSize);
            //size of the current word
            if (newText.Length + words[i].Length > maxChar) {
                result.Add(newText); //one time displayable dlg record complete
                newText = ""; //start anew
            }

            if (i == 0) {
                newText += words[i];
                count += size;
            } else if (count + space > width || count + space + size > width) {
                if (!newText.Equals("")) {
                    newText += "\n";
                }
                newText += words[i];
                count = size;
            } else if (count + space + size <= width) {
                newText += " " + words[i];
                count += space + size;
            }
        }
        result.Add(newText);
        return tagPos; //result.length is how many "pages" there are, each with multiple '\n' within
    }

    private int GetWordSize(Text textUI, string word, Font font, int fontSize) {
        char[] arr = word.ToCharArray();
        CharacterInfo info;
        int size = 0;
        for (int i = 0; i < arr.Length; i++) {
            textUI.font.RequestCharactersInTexture(word, fontSize, textUI.fontStyle);
            font.GetCharacterInfo(arr[i], out info, fontSize);
            size += info.advance;
        }
        return size;
    }

    private void parseDLGspecialParams(int[] PARAM1, int[] PARAM2, int[] PARAM3)
    {

            if (data[8, pointer].Contains(","))
            {
                string[] parsed = data[8, pointer].Split(',');
                PARAM1 = new int[parsed.Length];
                int c = 0;
                foreach (string num in parsed)
                {
                    int.TryParse(num, out PARAM1[c]);
                    c++;
                }
            }
            else
            {
                int.TryParse(data[8, pointer], out PARAM1[0]);
            }

            if (data[9, pointer].Contains(","))
            {
                string[] parsed = data[9, pointer].Split(',');
                PARAM2 = new int[parsed.Length];
                int c = 0;
                foreach (string num in parsed)
                {
                    int.TryParse(num, out PARAM2[c]);
                    c++;
                }
            }
            else
            {
                int.TryParse(data[9, pointer], out PARAM2[0]);
            }

        if (data[10, pointer].Contains(","))
        {
            string[] parsed = data[10, pointer].Split(',');
            PARAM3 = new int[parsed.Length];
            int c = 0;
            foreach (string num in parsed)
            {
                int.TryParse(num, out PARAM3[c]);
                c++;
            }
        }
        else
        {
            int.TryParse(data[10, pointer], out PARAM3[0]);
        }

    }

    //pointer check is a buffer to prevent multiple pointer addition from a single click
    //also movePointer is not called here, it's called in GameControl, the global control
    //this should only be used for dialogues, not in other modes
    //in other modes, incrementPointer, which is instantaneous, should be used
    public IEnumerator movePointer() {
        if (lineDone && pointerCheck) {
            pointerCheck = false;
            pointer++;
            yield return new WaitForSeconds(0.2f);
            pointerCheck = true;
        }
    }

    public void incrementPointer()
    {
        pointer++; 
    }

    public int getPointer() {
        return pointer;
    }

    public void disableDialogueBox() {
        dlgBox.SetActive(false);
    }

    public void enableDialogueBox() {
        dlgBox.SetActive(true);
    }

    //not used yet, but will prob be used for crucial parts of the story
    public bool Skippable() {
        return canSkip;
    }

    public void skipDLG() {
        skipping = true;
    }

    public void setAnimState(GameObject c, float n)
    {
        Animator a = c.GetComponent<Animator>();

        a.SetFloat("State", n);
        animIndicator.text = n.ToString();

    }

    public void setAnimBaseState(GameObject c, int n)
    {
        Animator a = c.GetComponent<Animator>();

        float bools = a.GetFloat("State") - Mathf.Floor(a.GetFloat("State"));
        setAnimState(c, n + bools);
    }

    /**
     * calculates the overall State float based on the two bool params
     */
    public void setBoolParam(GameObject c, int num, bool b)
    {
        Animator a = c.GetComponent<Animator>();
        // a.SetBool(name, b);

        if(num == 0)
        {
            animBool0 = b;
        }else if (num == 1)
        {
            animBool1 = b; 
        }

        bool p0 = animBool0, p1 = animBool1;
        
    if(p0 && p1)
        {
            setAnimState(c, Mathf.Floor(a.GetFloat("State")) + 0.15f);

        }else if (p0 && (!p1))
        {
            setAnimState(c, Mathf.Floor(a.GetFloat("State")) + 0.1f);
        }
        else if ((!p0) && p1)
        {
            setAnimState(c, Mathf.Floor(a.GetFloat("State")) + 0.05f);
        }
        else //neither
        {
            setAnimState(c, Mathf.Floor(a.GetFloat("State")));
        }


    }
}
