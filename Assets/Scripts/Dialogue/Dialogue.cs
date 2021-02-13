﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{

	[Inject(InjectFrom.Anywhere)]
	public EnemyLoader enemyLoader;
	[Inject(InjectFrom.Anywhere)]
	public CharacterLoader characterLoader;
	[Inject(InjectFrom.Anywhere)]
	public ItemLoader itemLoader;
	[Inject(InjectFrom.Anywhere)]
	public GameFlow gameFlow;
	[Inject(InjectFrom.Anywhere)]
	public DlgOptions dlgOptions;

	public Text NAME, DIALOGUE;
	public Font ArcadeClassic, Invasion2000;
	public GameObject character, dlgBox; //character is current character speaking
	public AudioSource cVoiceSource;
	public Image bgBox;

	public Text animIndicator; //number to show which state the character is
	public bool animBool0, animBool1; //names of the two bool switches of the character's animator, e.g. talking, typing
	public string prevChaName; //to inform whether there's a need to set animator for current line

	private bool lineDone = true, skipping = false;
	public bool canSkip = true;
	private string openTag, endTag; //those two variables are used for dialogue tag processing

	private bool[] loadDone; //TODO related to title, debug

	void Start()
    {
		if (ArcadeClassic != null) NAME.font = ArcadeClassic;

		cVoiceSource = character.GetComponent<AudioSource>(); //audioSource for the voice blips

		loadDone = new bool[1]; //TODO
	}

    void Update()
    {
        
    }

    public void displayOneLine(string c_name, string content, string sprite_state, string display_spd, string not_found_param,
		string special_index, string param_1, string param_2, string param_3)
	{
		StartCoroutine(oneLine(c_name, content, sprite_state, display_spd, not_found_param, special_index, param_1, param_2,
            param_3));
	}

    private IEnumerator oneLine(string c_name, string content, string sprite_state, string display_spd, string not_found_param,
        string special_index, string param_1, string param_2, string param_3)
	{

		//sprite fixes size of background square
		//Global.resizeSpriteToDLG(character, character.transform.parent.gameObject); no need, using images
		AudioClip cVoiceClip = null;

		lineDone = false; //dialogue is shown one char at a time
		NAME.text = c_name;
		int index = -1; //cCode of the upcoming character

		if (!prevChaName.Equals(NAME.text)) //if equal, no need to change animator
		{
			index = characterLoader.getIndex(NAME.text);

			if (index == -1) //not found, check for special param instructing which Animator to use
			{
				if (Invasion2000 != null) NAME.font = Invasion2000;
				int.TryParse(not_found_param, out index); //if success, assign animator accordingly
			}
			else
			{
				if (ArcadeClassic != null) NAME.font = ArcadeClassic;
			}

			if (index == -1) //not assigned, no animator
			{
				bgBox.color = new Color(0, 0, 0); //black 
			}
			else
			{
				// assign animator
				character.GetComponent<Animator>().runtimeAnimatorController =
				characterLoader.getAnimatorByIndex(index);
				cVoiceClip = characterLoader.getVoiceByIndex(index);
				cVoiceSource.clip = cVoiceClip;
				Color c = bgBox.color;
				c = characterLoader.getColorByIndex(index);
				bgBox.color = new Color(c.r / 255.0f, c.g / 255.0f, c.b / 255.0f);
			}

			prevChaName = NAME.text;
		}

		int SpriteNum;
		int.TryParse(sprite_state, out SpriteNum);

		float disp_spd;
		float.TryParse(display_spd, out disp_spd); //converts string to int
		string[] store; ArrayList result = new ArrayList();
		int[] tags = GetFormattedText(DIALOGUE, content, result);
		store = result.ToArray(typeof(string)) as string[];
		Canvas.ForceUpdateCanvases();
		DIALOGUE.text = "";

		//character start talking (default talking anim state); assumes part1 is always mouth 
		setPartLayerParam(index, character, 1, 1);

		setAnimBaseState(index, character, SpriteNum);

        int special;
        int.TryParse(special_index, out special);
        /*
		 * the params for special events could be int, could be int arrays, so to cover all 
		 * cases we create variables for all possibilities                
		 */
        ArrayList PARAM1, PARAM2, PARAM3;
		PARAM1 = new ArrayList(); PARAM2 = new ArrayList(); PARAM3 = new ArrayList();
		if (special != 0) //if there is special, parse params
		{
			parseDLGspecialParams(PARAM1, PARAM2, PARAM3, param_1, param_2, param_3);
		}
		//end parsing special param, start adding chars 

		int wordCount = 1; int[] paramPointer = new int[4]; //paramPointer[2] would be pointer for special #2, pointer tracks curr position in string

		if (special == 3) //SPECIAL 3 disables users from clicking to proceed; will auto proceed after certain seconds
		{
			gameFlow.canMovePointer = false;
			//chains a WaitForSecond with <what should be done afterwards>
			StartCoroutine(Global.Chain(this, Global.WaitForSeconds((int)PARAM1[0]), Global.Do(() =>
			{
				gameFlow.canMovePointer = true;
				gameFlow.incrementPointer();
			})));
		}
		else if (special == 6)
		{
			//SPECIAL 6: incrementPointer() will go to PARAM1[0] instead of the next line
			gameFlow.setSpecialGoToLine ((int)PARAM1[0]);
		}



		for (int s = 0; s < store.Length; s++)
		{
			for (int n = 0; n < store[s].Length; n++)
			{
				//tagging is for tags in rich text, not a part of the special effects system
				if (n == tags[0]) //we're at the first letter that needs to be tagged
				{
					DIALOGUE.text += openTag;
				}
				else if (tags[0] != -1 && n > tags[0] && n <= tags[2])
				{
					DIALOGUE.text = DIALOGUE.text.Remove
						(DIALOGUE.text.Length - (endTag.Length)); //remove temp ending tag
				}

				DIALOGUE.text += store[s][n]; //the actual adding of the char

				//Sound effect play
				if (cVoiceSource.clip && (n % 2 == 0))
				{
					cVoiceSource.Play();

				}

				if (store[s][n] == ' ') //new word
				{
					wordCount++;
				}

				if (tags[0] != -1 && n >= tags[0] && n <= tags[2])
				{
					DIALOGUE.text += endTag; //add temp ending tag
				}

				switch (special)
				{
					/*
					 * SPECIAL mode 1, changing of sprite of the speaking character
					 *  -param 1: indices of char count to change sprite
					 *  -param 2: number for State variable of the character (will assign sprite accordingly)
					 */
					case 1:
						if (n == (int)PARAM1[paramPointer[1]]) //if current char is char at which a switch should happen
						{
							setAnimBaseState(index, character, (int)PARAM2[paramPointer[1]]);
							if (paramPointer[1] < PARAM1.Count - 1)
							{
								paramPointer[1]++; 
							}
						}
						break;
					/*
					 * SPECIAL mode 2, the changing of motion states of the character's body parts (Part1, Part2, etc.)
					 * -param 1: "which" state(s) to be set 
					 * the same state can appear for multiple times (e.g. 0, 1, 0 will set, for example);
                     * NOTE: this needs to match the number of items in param2/3; in other words, even "[1];[1]" is necessary
					 * -param 2: value(s) to set those state(s), integer
					 * -param 3: word count indices at which the state(s) are to be set
					 * 
					 * e.g. [0,1];[3,5];[2,3] sets part 1(0) to "3" at word 2, sets part 2(1) to "5" at word 3
					 */
					case 2:
						if (wordCount == (int)PARAM3[paramPointer[2]])
						{
							setPartLayerParam(index, character, (int)PARAM1[paramPointer[2]],
								(int)PARAM2[paramPointer[2]]);

							if (paramPointer[2] < PARAM3.Count - 1)
							{
								paramPointer[2]++;
							}
						}
						break;
					//SPECIAL mode 3 disables users from clicking to proceed, will auto proceed after certain seconds
					case 3:
						//called once above previous to the double for loop
						break;
					/*
					 * SPECIAL mode 4, character speaking speed change in dialogue
					 * -param 1: indices of char count to change dialogue speed
					 * -param 2: spd(s) to change into
					 */
					case 4:
						if (n == (int)PARAM1[paramPointer[4]])
						{
							disp_spd = (float)PARAM2[paramPointer[4]];
							if (paramPointer[4] < PARAM1.Count - 1)
							{
								paramPointer[4]++;
							}
						}
						break;
					/*
						* SPECIAL mode 5, opens up option prompt for user to choose after character finishes talking
						* -param 1: texts of choices, separated by comma
						* -param 2: lines to point to after selecting those choices
						*/
					case 5:
						//happens after the entire dialogue is displayed, so isn't called repetitively here
						break;
					/*
					 * SPECIAL mode 6, directs pointer to provided line other than increment by 1
						* -param 1: line to point to
						*/
					case 6:
						//needs only happen once, is invoked up above
						break;
					default:
						break;
				}

				if (!skipping)
				{
					if (store[s][n].Equals(','))
					{
						//for break in between (half) sentences, give a longer wait time
						yield return new WaitForSeconds(0.2f);
					}
					else if (store[s][n].Equals('.'))
					{
						yield return new WaitForSeconds(0.4f);
					}
					else
					{
						yield return new WaitForSeconds(-1.0333f * disp_spd + 1.07f);
					}
				}
			}
			if (!(s == (store.Length - 1)))
			{
				DIALOGUE.text = ""; //reset and print the second section
			}
		}

		if (special == 5)
		{//only happens after entirety of text is up

			gameFlow.canMovePointer = false;
			int[] lines = new int[PARAM2.Count];
			string[] messages = new string[PARAM1.Count];
			for (int i = 0; i < PARAM2.Count; i++)
			{
				lines[i] = (int)PARAM2[i];
				messages[i] = (string)PARAM1[i];
			}

			dlgOptions.showOptions(PARAM1.Count, messages, lines);
		}

		//character stop talking (default talking anim state)
		setPartLayerParam(index, character, 1, 0);
		lineDone = true;
		skipping = false;

	}

	private void parseDLGspecialParams(ArrayList PARAM1, ArrayList PARAM2, ArrayList PARAM3, string param_1, string param_2, string param_3)
	{

		if (param_1.Contains(","))
		{
			string[] parsed = param_1.Split(',');
			int c = 0;

			foreach (string num in parsed)
			{
				int res;
				if (int.TryParse(num, out res))
					PARAM1.Add(res);
				else
					PARAM1.Add(num);
				c++;
			}
		}
		else
		{
			int res;
			int.TryParse(param_1, out res);
			PARAM1.Add(res);
		}

		if (param_2.Contains(","))
		{
			string[] parsed = param_2.Split(',');
			int c = 0;
			foreach (string num in parsed)
			{
				int res;
				if (int.TryParse(num, out res))
					PARAM2.Add(res);
				else
					PARAM2.Add(num);
				c++;
			}
		}
		else
		{
			int res;
			int.TryParse(param_2, out res);
			PARAM2.Add(res);
		}

		if (param_3.Contains(","))
		{
			string[] parsed = param_3.Split(',');

			int c = 0;
			foreach (string num in parsed)
			{
				int res;
				if (int.TryParse(num, out res))
					PARAM3.Add(res);
				else
					PARAM3.Add(num);
				c++;
			}
		}
		else
		{
			int res;
			int.TryParse(param_3, out res);
			PARAM3.Add(res);
		}


	}

	public bool checkCurrentLineDone()
	{
		return lineDone;
	}

	public void disableDialogueBox()
	{
		dlgBox.SetActive(false);
	}

	public void enableDialogueBox()
	{
		dlgBox.SetActive(true);
	}

	//not used yet, but will prob be used for crucial parts of the story
	public bool Skippable()
	{
		return canSkip;
	}

	public void skipDLG()
	{
		skipping = true;
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
	private int[] GetFormattedText(Text textUI, string text, ArrayList result)
	{

		int[] tagPos = new int[4];

		if (text.Contains("<")) //check if there's tag in this sentence
		{
			tagPos[0] = text.IndexOf('<'); //the starting pos of the opening tag
			tagPos[1] = text.IndexOf('>'); //the ending pos of the opening tag

			int ot_length = tagPos[1] - tagPos[0] + 1;
			openTag = text.Substring(tagPos[0], ot_length);
			text = text.Remove(tagPos[0], ot_length);

			tagPos[2] = text.IndexOf('<'); //the starting pos of the ending tag AFTER openTag is removed
			tagPos[3] = text.IndexOf('>'); //the ending pos of the ending tag
			endTag = text.Substring(tagPos[2], tagPos[3] - tagPos[2] + 1);
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
		for (int i = 0; i < words.Length; i++)
		{
			int size = GetWordSize(textUI, words[i], textUI.font, textUI.fontSize);
			//size of the current word
			if (newText.Length + words[i].Length > maxChar)
			{
				result.Add(newText); //one time displayable dlg record complete
				newText = ""; //start anew
			}

			if (i == 0)
			{
				newText += words[i];
				count += size;
			}
			else if (count + space > width || count + space + size > width)
			{
				if (!newText.Equals(""))
				{
					newText += "\n";
				}
				newText += words[i];
				count = size;
			}
			else if (count + space + size <= width)
			{
				newText += " " + words[i];
				count += space + size;
			}
		}
		result.Add(newText);
		return tagPos; //result.length is how many "pages" there are, each with multiple '\n' within
	}

	private int GetWordSize(Text textUI, string word, Font font, int fontSize)
	{
		char[] arr = word.ToCharArray();
		CharacterInfo info;
		int size = 0;
		for (int i = 0; i < arr.Length; i++)
		{
			textUI.font.RequestCharactersInTexture(word, fontSize, textUI.fontStyle);
			font.GetCharacterInfo(arr[i], out info, fontSize);
			size += info.advance;
		}
		return size;
	}


	public void setAnimBaseState(int cCode, GameObject c, int n)
	{
		Animator a = c.GetComponent<Animator>();
		print("setting anim base state to " + characterLoader.baseStateAnimationClipNames[cCode][n]);
		a.Play(characterLoader.baseStateAnimationClipNames[cCode][n]);
	}


	/**
	 * sets the parameter for the part's animation layer to be value given
	 */
	public void setPartLayerParam(int cCode, GameObject c, int partIndex, int value)
	{
		Animator a = c.GetComponent<Animator>();

        switch (partIndex)
        {
			case 1: //part 1
				a.Play(characterLoader.Part1AnimationClipNames[cCode][value]);
				break;
			case 2: //part 2
				a.Play(characterLoader.Part2AnimationClipNames[cCode][value]);
				break;
			default:
				Debug.Log("unknown layer");
				break;
        }

	}

	//** TITLE

	public IEnumerator displayTitleDLG(string[,] data) //TODO need to refactor
	{ //ONLY applies to the special csv file of title

		//randomly chooses dialogue for bunny to say
		int ttlWeight = 0; int line = -1;
		for (int r = 1; r < data.GetLength(1); r++)
		{
			ttlWeight += int.Parse(data[4, r]);
		}
		float rdm = UnityEngine.Random.Range(0, ttlWeight);
		for (int r = 1; r < data.GetLength(1); r++)
		{
			rdm -= int.Parse(data[4, r]);
			if (rdm < 0)
			{
				line = r;
				break;
			}
		}

		//Assumes character is already given, TODO set this based on data
		Global.resizeSpriteToDLG(character, character.transform.parent.gameObject);

		int SpriteNum;
		int.TryParse(data[2, line], out SpriteNum);

		int index = characterLoader.getIndex(NAME.text); //
		setAnimBaseState(index, character, SpriteNum);

		int disp_spd;
		int.TryParse(data[3, line], out disp_spd); //converts string to int
		string[] store; ArrayList result = new ArrayList();
		int[] tags = GetFormattedText(DIALOGUE, data[1, line], result);
		store = result.ToArray(typeof(string)) as string[];
		//Canvas.ForceUpdateCanvases();
		DIALOGUE.text = "";

		setPartLayerParam(index, character, 1, 1); //start talking

		for (int s = 0; s < store.Length; s++)
		{
			for (int n = 0; n < store[s].Length; n++)
			{
				DIALOGUE.text += store[s][n];
				yield return new WaitForSeconds(0.02f * disp_spd + 0.05f); //TODO adjust
			}
			if (!(s == (store.Length - 1)))
			{
				DIALOGUE.text = ""; //reset and print the second section
			}
		}

		setPartLayerParam(index, character, 1, 0); //end talking
	}

}
