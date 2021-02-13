using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{

    private bool[] loadDone; //when loadDone[0] == true, loading is done for the csv file
    public TextAsset characterCsv;
    private string[,] data; //double array that stores all info 

    int[] characterCode; 
    ArrayList cName;
    public List<string[]> baseStateAnimationClipNames, Part1AnimationClipNames, Part2AnimationClipNames; //holds transition conditions for each character
    string[] pathName; //animators, voices will have the same local path (e.g. CaptainBuns) for files
    RuntimeAnimatorController[] cAnimators;
    AudioClip[] voiceClips;
    public Color[] bgColor; 

    GameObject cMold;

    public bool characterLoaderDone; //this will be set to true once is ready for usage


    void Start()
    {
        loadDone = new bool[1];

        loadCharacterMold(); //ready the mold prefab(s)
        StartCoroutine(LoadScene.processCSV(loadDone, characterCsv, setData, false)); //processCSV will call setData
        StartCoroutine(parseCharacterData()); //data will be parsed into local type arrays for speedy data retrieval

    }

    void Update()
    {

    }


    void loadCharacterMold()
    {
        cMold = Resources.Load("cMold") as GameObject;
        if (cMold == null) Debug.LogError("load CharacterMold failed");
    }

    IEnumerator parseCharacterData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed

        int numRows = 0;
        for(int i=0; i< data.GetLength(1); i++) //we do this because we only want to record valid entries (number coded)
        {
            if (!data[1, i].Equals(""))
            {
                numRows++; //valid row++
            }
            else
            {
                break;
            }
        }

        characterCode = new int[numRows - 1]; //num rows, int[] is for the entire column
        cName = new ArrayList();
        baseStateAnimationClipNames = new List<string[]>();
        Part1AnimationClipNames = new List<string[]>();
        Part2AnimationClipNames = new List<string[]>();

        //bool1Name = new string[numRows - 1]; bool2Name = new string[numRows - 1];
        pathName = new string[numRows - 1]; 
        cAnimators = new RuntimeAnimatorController[numRows - 1];
        voiceClips = new AudioClip[numRows - 1];
        bgColor = new Color[numRows - 1];

        //skip row 0 because those are all descriptors
        for (int r = 1; r < numRows; r++) 
        {
            cName.Add(data[1, r]);
            baseStateAnimationClipNames.Add(new string[numRows - 1]);
            Part1AnimationClipNames.Add(new string[numRows - 1]);
            Part2AnimationClipNames.Add(new string[numRows - 1]);

            //bool1Name[r - 1] = data[2, r]; left blank for now
            //bool2Name[r - 1] = data[3, r];

            pathName[r - 1] = data[4, r]; //col 4 is file path;
            int R, G, B;
            int.TryParse(data[5, r], out R);
            int.TryParse(data[6, r], out G);
            int.TryParse(data[7, r], out B);
            bgColor[r - 1] = new Color(R, G, B);
        }

        loadAnimators(cAnimators);
        loadVoices(voiceClips);
        yield return new WaitUntil(() =>
        {
            return (cAnimators[pathName.Length - 1] != null);
        }); //anim loaded, theoretically everything all set

        characterLoaderDone = true;
        Debug.Log("CharacterLoader ready");
    }

    public void setData(string[,] d)
    {
        data = d;
    }

    //given data for a character's animator transition conitions, set variables accordingly
    //in-game state transitions will depend on those variables
    public void setStateMachineData(string[,] d, int cCode)
    {
        Debug.Log("state machine data first row first col: " + d[0, 0]);

        for (int i = 0; i < d.GetLength(1); i++) 
        {
            string animClipName = d[i, 0];
            int stateNum = -1;

            if (d[i, 1] != "") //clip belongs to BASE layer
            {
                int.TryParse(d[i, 1], out stateNum);
                baseStateAnimationClipNames[cCode][stateNum] = animClipName;
            } else if (d[i, 2] != "") //PART 1 layer
            {
                int.TryParse(d[i, 2], out stateNum);
                Part1AnimationClipNames[cCode][stateNum] = animClipName;
            } else if (d[i, 3] != "") //PART 2 layer
            {
                int.TryParse(d[i, 3], out stateNum);
                Part2AnimationClipNames[cCode][stateNum] = animClipName;
            }
            else{
                Debug.LogError(d[i,0] + " no proper transition condition found");
            }
        }
    }

    private void loadAnimators(RuntimeAnimatorController[] cAnim)
    {

        //load the animation clips into the arrays
        for (int cCode = 0; cCode < cAnim.GetLength(0); cCode++)
        {
            if (!pathName[cCode].Equals(""))
            {
                //load in animator
                var amt = Resources.Load("Animator/" + pathName[cCode]) as RuntimeAnimatorController;
                var transitionConditionCSV = Resources.Load("Animator/StateMachineCSV/" + pathName[cCode]+"Animator") as TextAsset;

                if (amt == null || transitionConditionCSV == null)
                {
                    Debug.Log("Animator for c"+cCode+" NOT found");
                }
                else
                {
                    cAnim[cCode] = amt;

                    bool[] csvLoadDone = new bool[1];
                    //sets transition data for each character based on csv
                    StartCoroutine(LoadScene.processCSV(csvLoadDone, transitionConditionCSV, (string[,] d) => setStateMachineData(d, cCode), false));
                    //TODO do we wait for done here
                }
            }
        }

    }


    private void loadVoices(AudioClip[] voices)
    {

        for (int cCode = 0; cCode < voices.GetLength(0); cCode++)
        {

                var v = Resources.Load("Voices/" + pathName[cCode]) as AudioClip;

                if (v == null)
                {
                    Debug.LogError("Voice for c" + cCode + " NOT found");
                }
                else
                {
                    voices[cCode] = v;
                }
        }

    }

    public RuntimeAnimatorController getAnimatorByName(string name)
    {
        int index = cName.IndexOf(name);
        return getAnimatorByIndex(index);
    }

    public RuntimeAnimatorController getAnimatorByIndex(int index)
    {
        if (index != -1)
        {
            return cAnimators[index];
        }
        else
        {
            Debug.LogError("no animator for character " + index + " found");
            return null;
        }
    }

    public AudioClip getVoiceByName(string name)
    {
        int index = cName.IndexOf(name);
        return getVoiceByIndex(index);
    }

    public AudioClip getVoiceByIndex(int index)
    {
        if (index != -1)
        {
            return voiceClips[index];
        }
        else
        {
            Debug.LogError("no voice for character " + index + " found");
            return null;
        }
    }

    //TODO can optimize (maybe with a dictionary)
    public int getIndex(string name)
    {
        int index = cName.IndexOf(name);
        return index;
    }

    public Color getColorByIndex(int index)
    {
        return bgColor[index];
    }
}

