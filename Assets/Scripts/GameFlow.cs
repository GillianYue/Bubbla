using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameFlow : MonoBehaviour {
	public TextAsset csv; //dialogue file for a specific level
	private int pointer = 1; //indicates which line of script the game is at
	private bool lineDone = true, loadDone = false, pointerCheck = true,
	skipping = false;
	public bool canSkip;
	public Text NAME, DIALOGUE;
	private string[,] data; //stores all info of this level
	public GameObject character, dlgBox;
	public enum Mode {DLG, GAME, END};
	public Mode currMode;
	public GameControl gameControl;

	void Start () {
		StartCoroutine(processCSV ());
		pointer = 1; //row 0 are names of the categories
	//	character = gameObject.transform.Find ("Background").Find (
	//		"SpriteBox").GetChild (0).gameObject;
	}

	IEnumerator processCSV(){ //TODO move this to loading, as opposed to in game
		data = CSVReader.SplitCsvGrid (csv.text); 
		while (!(data.Length > 0)) {
			yield return null;
		}
		loadDone = true;
	}

	public bool checkIfEnded(){ //checks if ended
		return (currMode.Equals(Mode.END)); //the "big" done
	}

	public bool checkLoadDone(){ //check if csv is loaded and dialogue is ready tb displayed
		return loadDone; 
	}

	public bool checkCurrentLineDone(){
		return lineDone;
	}

	public IEnumerator processCurrentLine(){ //current being where the pointer is

		if (data[0, pointer] != "") {//check if done (if yes move on to actual game play)
			if(currMode == Mode.DLG) disableDialogueBox(); //if transitioning from dlg to others
			currMode = (Mode)System.Enum.Parse(typeof(Mode), data[0,pointer]);
			if (currMode == Mode.DLG) enableDialogueBox ();
			print ("Mode changed to " + currMode);
		}

		switch(currMode){
		case Mode.DLG: //still in dialogue mode
			lineDone = false; //dialogue is shown one char at a time
			NAME.text = data [1, pointer];
			int StageNum;
			int.TryParse (data [3, pointer], out StageNum);
			character.GetComponent<Animator> ().SetInteger ("State", 
				StageNum);

			int disp_spd;
			int.TryParse (data [4, pointer], out disp_spd); //converts string to int
			string[] store = GetFormattedText 
				(DIALOGUE, data [2, pointer]).ToArray(typeof(string)) as string[];
			DIALOGUE.text = store[0];
			Canvas.ForceUpdateCanvases();
			DIALOGUE.text = "";
			character.GetComponent<Animator> ().SetBool ("Talking", true);

			for (int s = 0; s < store.Length; s++) {
				for (int n = 0; n < store[s].Length; n++) {
					DIALOGUE.text += store [s] [n];
					if (!skipping) {
						yield return new WaitForSeconds (0.02f * disp_spd + 0.05f); //TODO adjust
					} 
				}
				if(!(s == (store.Length-1))){
					DIALOGUE.text = ""; //reset and print the second section
				}
			}
			character.GetComponent<Animator> ().SetBool ("Talking", false);
			lineDone = true;
			skipping = false;
		break;

		case Mode.GAME:
			string[] waves = data [1, pointer].Split (',');
			string[] enemies = data [2, pointer].Split (',');
			string[] rgb = data [3, pointer].Split (',');
			float mx_D;
			float.TryParse (data [4, pointer], out mx_D);
			int r, g, b;
			int.TryParse (rgb [0], out r);
			int.TryParse (rgb [1], out g);
			int.TryParse (rgb [2], out b);
			PaintballSpawner.standard = new Color (r, g, b);
			PaintballSpawner.setMaxD (mx_D);
			int[] wv, em;
			wv = System.Array.ConvertAll<string,int> (waves, int.Parse);
			em = System.Array.ConvertAll<string,int> (enemies, int.Parse);
			gameControl.startEnemyWaves (wv, em);
			break;

		case Mode.END:
			print ("level ended!");
			break;

		default: break;
		}
	}

	public IEnumerator displayTitleDLG(){ //ONLY applies to the special csv file of title

		int ttlWeight = 0; int line = -1;
		for (int r = 1; r < data.GetLength (1); r++) {
            ttlWeight += int.Parse (data [4, r]);
		}
		float rdm = UnityEngine.Random.Range (0, ttlWeight); 
		for (int r = 1; r < data.GetLength (1); r++) {
			rdm -= int.Parse (data [4, r]);
			if (rdm < 0) {
				line = r;
				break;
			}
		}


		int StageNum;
		int.TryParse (data [2, line], out StageNum);
		character.GetComponent<Animator> ().SetInteger ("State", 
			StageNum);

		int disp_spd;
		int.TryParse (data [3, line], out disp_spd); //converts string to int
		string[] store = GetFormattedText 
			(DIALOGUE, data [1, line]).ToArray(typeof(string)) as string[];
		DIALOGUE.text = store[0];
		Canvas.ForceUpdateCanvases();
		DIALOGUE.text = "";
		character.GetComponent<Animator> ().SetBool ("Talking", true);
		character.GetComponent<Animator> ().SetBool ("Typing", true);

		for (int s = 0; s < store.Length; s++) {
			for (int n = 0; n < store[s].Length; n++) {
				DIALOGUE.text += store[s][n];
				yield return new WaitForSeconds (0.02f * disp_spd + 0.05f); //TODO adjust
			}
			if(!(s == (store.Length-1))){
				DIALOGUE.text = ""; //reset and print the second section
			}
		}
		character.GetComponent<Animator> ().SetBool ("Talking", false);
		character.GetComponent<Animator> ().SetBool ("Typing", false);
	}


    /*
     * prevents long words at end of line from jumping to the next when rendering
     *     
     */
    private ArrayList GetFormattedText (Text textUI, string text) {
		ArrayList result = new ArrayList();
		string[] words = text.Split(' ');

		int width = Mathf.FloorToInt(textUI.rectTransform.sizeDelta.x);
		int height = Mathf.FloorToInt(DIALOGUE.rectTransform.sizeDelta.y);
		int space = GetWordSize(textUI, " ", textUI.font, textUI.fontSize);
		int charH = space + Mathf.CeilToInt(DIALOGUE.lineSpacing); //height of single char
		int charW = space; //treat character as little square
		int maxChar = (height / charH) * (width / charW);

		string newText = string.Empty;
		int count = 0;
		for (int i = 0; i < words.Length; i++) {
			int size = GetWordSize(textUI, words[i], textUI.font, textUI.fontSize);
			//size of the current word
			if (newText.Length + words [i].Length > maxChar) {
				result.Add (newText); //one time displayable dlg record complete
				newText = ""; //start anew
			}

			if (i == 0) {
				newText += words [i];
				count += size;
			} else if (count + space > width || count + space + size > width) {
				if (!newText .Equals ("")) {
					newText += "\n";
				}
				newText += words[i];
				count = size;
			} else if (count + space + size <= width) {
				newText += " " + words[i];
				count += space + size;
			}
		}
		result.Add (newText);
		return result;
	}

	private int GetWordSize (Text textUI, string word, Font font, int fontSize) {
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

	public IEnumerator movePointer(){
		if (lineDone && pointerCheck) {
			pointerCheck = false;
			pointer++;
			yield return new WaitForSeconds (0.2f);
			pointerCheck = true;
		}
	}

	public int getPointer(){
		return pointer;
	}

	public void disableDialogueBox(){
		dlgBox.SetActive (false);
	}

	public void enableDialogueBox(){
		dlgBox.SetActive (true);
	}

	public bool Skippable(){
		return canSkip;
	}

	public void skipDLG(){
		skipping = true;
	}
}
