using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour {
    public TextAsset csv; //dialogue file for a specific level
    private int pointer = 1; //indicates which line of script the game is at
    private bool lineDone = true, loadDone, pointerCheck = true,
    skipping = false;
    public bool canSkip;
    public Text NAME, DIALOGUE;
    private string[,] data; //double array that stores all info of this level
    public GameObject character, dlgBox;
    public enum Mode { DLG, GAME, END };
    public Mode currMode;
    public GameControl gameControl;
    public LevelScript levelScript;

    void Start() {
        StartCoroutine(processCSV());
        pointer = 1; //row 0 are names of the categories
                     //	character = gameObject.transform.Find ("Background").Find (
                     //		"SpriteBox").GetChild (0).gameObject;
    }

    IEnumerator processCSV() { //TODO move this to loading (a scene), as opposed to in game
        data = CSVReader.SplitCsvGrid(csv.text);
        while (!(data.Length > 0)) {
            yield return null;
        }
        loadDone = true;
        //levelScript.setLevelCsvData(data);
    }

    public bool checkIfEnded() { //checks if ended
        return (currMode.Equals(Mode.END)); //the "big" done
    }

    public bool checkLoadDone() { //check if csv is loaded and dialogue is ready tb displayed
        return loadDone;
    }

    public bool checkCurrentLineDone() {
        return lineDone;
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
                //Assumes character is already given, TODO set this based on data
                Global.resizeSpriteToDLG(character, character.transform.parent.gameObject);

                lineDone = false; //dialogue is shown one char at a time
                NAME.text = data[1, pointer];

                int SpriteNum;
                int.TryParse(data[3, pointer], out SpriteNum);
                character.GetComponent<Animator>().SetInteger("State",
                    SpriteNum);

                float disp_spd;
                float.TryParse(data[4, pointer], out disp_spd); //converts string to int
                string[] store = GetFormattedText
                    (DIALOGUE, data[2, pointer]).ToArray(typeof(string)) as string[];
                Canvas.ForceUpdateCanvases();
                DIALOGUE.text = "";

                int special;
                int.TryParse(data[7, pointer], out special);

                if (special == 0)
                character.GetComponent<Animator>().SetBool("Talking", true);


                //string[] param1 = GetFormattedText
                //    (DIALOGUE, data[8, pointer]).ToArray(typeof(string)) as string[];
                //string[] param2 = GetFormattedText
                //(DIALOGUE, data[9, pointer]).ToArray(typeof(string)) as string[];
                int param1, param2;
                int.TryParse(data[8, pointer], out param1);
                int.TryParse(data[9, pointer], out param2);

                for (int s = 0; s < store.Length; s++) {
                    for (int n = 0; n < store[s].Length; n++) {
                        DIALOGUE.text += store[s][n];
                        if(special == 1 && n == param1)
                        {
                            character.GetComponent<Animator>().SetInteger("State",
                    param2);
                        }
                        if (!skipping) {
                            yield return new WaitForSeconds(- 1.0333f * disp_spd + 1.1f);
                        }
                    }
                    if (!(s == (store.Length - 1))) {
                        DIALOGUE.text = ""; //reset and print the second section
                    }
                }
                character.GetComponent<Animator>().SetBool("Talking", false);
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
                    levelScript.customEvent(index, parameters);
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
        character.GetComponent<Animator>().SetInteger("State",
            SpriteNum);

        int disp_spd;
        int.TryParse(data[3, line], out disp_spd); //converts string to int
        string[] store = GetFormattedText
            (DIALOGUE, data[1, line]).ToArray(typeof(string)) as string[];
        DIALOGUE.text = store[0];
        //Canvas.ForceUpdateCanvases();
        DIALOGUE.text = "";


        character.GetComponent<Animator>().SetBool("Talking", true);
        character.GetComponent<Animator>().SetBool("Typing", true);

        for (int s = 0; s < store.Length; s++) {
            for (int n = 0; n < store[s].Length; n++) {
                DIALOGUE.text += store[s][n];
                yield return new WaitForSeconds(0.02f * disp_spd + 0.05f); //TODO adjust
            }
            if (!(s == (store.Length - 1))) {
                DIALOGUE.text = ""; //reset and print the second section
            }
        }
        character.GetComponent<Animator>().SetBool("Talking", false);
        character.GetComponent<Animator>().SetBool("Typing", false);
    }


    /*
     * prevents long words at end of line from jumping to the next when rendering
     *     
     */
    private ArrayList GetFormattedText(Text textUI, string text) {
        ArrayList result = new ArrayList();
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
        return result; //result.length is how many "pages" there are, each with multiple '\n' within
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


}
